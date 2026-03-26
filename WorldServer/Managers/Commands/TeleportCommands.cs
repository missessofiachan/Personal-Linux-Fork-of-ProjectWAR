using Common;
using System;
using System.Collections.Generic;
using SystemData;
using System.Linq;
using static WorldServer.Managers.Commands.GMUtils;
using GameData;
using WorldServer.Services.World;
using WorldServer.World.Objects;
using WorldServer.World.Positions;

namespace WorldServer.Managers.Commands
{
    /// <summary>Contains the list of teleportation commands under .teleport</summary>
    internal class TeleportCommands
    {
        private enum DerivedTeleportSource
        {
            ZoneRespawn,
            ZoneTaxi,
            RallyPoint,
            ChapterPin,
            ExternalZoneJump,
            InternalZoneJump,
            ZoneInfoCenter,
            ZoneInfoEntry,
            SafeCenterFallback
        }

        private sealed class DerivedPinAnchor
        {
            public ushort PinX { get; set; }
            public ushort PinY { get; set; }
            public string Label { get; set; }
        }

        private sealed class DerivedTeleportPoint
        {
            public ushort ZoneId { get; set; }
            public uint WorldX { get; set; }
            public uint WorldY { get; set; }
            public ushort WorldZ { get; set; }
            public ushort WorldO { get; set; }
            public DerivedTeleportSource Source { get; set; }
            public string Detail { get; set; }
            public ushort? SourceZoneId { get; set; }
            public int Weight { get; set; }
            public bool RealmMatch { get; set; }
            public bool TerrainValidated { get; set; }
            public ushort? PinX { get; set; }
            public ushort? PinY { get; set; }

            public bool IsPortalJump
            {
                get
                {
                    return Source == DerivedTeleportSource.ExternalZoneJump || Source == DerivedTeleportSource.InternalZoneJump;
                }
            }

            public bool IsExternalPortal
            {
                get { return Source == DerivedTeleportSource.ExternalZoneJump; }
            }

            public bool IsSafeAnchor
            {
                get { return Source != DerivedTeleportSource.InternalZoneJump; }
            }

            public string Describe()
            {
                return $"{TeleportCommands.GetSourceLabel(Source)} {Detail}" + (TerrainValidated ? " terrain" : string.Empty);
            }
        }

        private static readonly int[] ZoneInfoRingOffsets = { 0, 4096, -4096, 8192, -8192, 12288, -12288, 16384, -16384 };
        private static readonly int[] CandidateSnapOffsets = { 0, 1024, -1024, 2048, -2048 };

        /// <summary>
        /// Teleports you to the specified world coordinates in a given zone (byte ZoneID , uint WorldX, uint WorldY, uint WorldZ)
        /// </summary>
        /// <param name="plr">Player that initiated the command</param>
        /// <param name="values">List of command arguments (after command name)</param>
        /// <returns>True if command was correctly handled, false if operation was canceled</returns>
        public static bool TeleportMap(Player plr, ref List<string> values)
        {
            int zoneID = GetZoneId(plr, ref values);
            if (zoneID == -1)
                return false;
            int worldX = GetInt(ref values);
            int worldY = GetInt(ref values);
            int worldZ = GetInt(ref values);

            plr.Teleport((ushort)zoneID, (uint)worldX, (uint)worldY, (ushort)worldZ, 0);

            GMCommandLog log = new GMCommandLog();
            log.PlayerName = plr.Name;
            log.AccountId = (uint)plr.Client._Account.AccountId;
            log.Command = "TELEPORT TO " + zoneID + " " + worldX + " " + worldY;
            log.Date = DateTime.Now;
            CharMgr.Database.AddObject(log);

            return true;
        }

        /// <summary>
        /// Teleport to the centre of the given map.
        /// </summary>
        /// <param name="plr">Player that initiated the command</param>
        /// <param name="values">List of command arguments (after command name)</param>
        /// <returns>True if command was correctly handled, false if operation was canceled</returns>
        public static bool TeleportCenter(Player plr, ref List<string> values)
        {
            Zone_Info zone;
            byte realm;
            if (!TryResolveZoneAndRealm(plr, ref values, out zone, out realm))
                return false;

            List<DerivedTeleportPoint> candidates = BuildDerivedTeleportPoints(zone, realm);
            DerivedTeleportPoint center = ResolveSafeCenter(candidates);
            if (center == null)
            {
                plr.SendClientMessage($"TELEPORT CENTER: Could not derive a safe point for {zone.Name}.", ChatLogFilters.CHATLOGFILTERS_USER_ERROR);
                return true;
            }

            plr.SendClientMessage(BuildPreviewLine("TELEPORT CENTER", zone, realm, center));
            plr.Teleport(center.ZoneId, center.WorldX, center.WorldY, center.WorldZ, center.WorldO);
            LogTeleportCommand(plr, $"TELEPORT CENTER {zone.ZoneId} {center.WorldX} {center.WorldY} {center.WorldZ} [{center.Describe()}]");

            return true;
        }

        /// <summary>
        /// Teleport to the derived portal arrival point for the given map.
        /// </summary>
        /// <param name="plr">Player that initiated the command</param>
        /// <param name="values">List of command arguments (after command name)</param>
        /// <returns>True if command was correctly handled, false if operation was canceled</returns>
        public static bool TeleportEntry(Player plr, ref List<string> values)
        {
            Zone_Info zone;
            byte realm;
            if (!TryResolveZoneAndRealm(plr, ref values, out zone, out realm))
                return false;

            List<DerivedTeleportPoint> candidates = BuildDerivedTeleportPoints(zone, realm);
            DerivedTeleportPoint center = ResolveSafeCenter(candidates);
            DerivedTeleportPoint entry = ResolvePortalArrival(plr, zone, candidates, center);

            if (entry == null)
            {
                plr.SendClientMessage($"TELEPORT ENTRY: Could not derive a portal landing for {zone.Name}.", ChatLogFilters.CHATLOGFILTERS_USER_ERROR);
                return true;
            }

            plr.SendClientMessage(BuildPreviewLine("TELEPORT ENTRY", zone, realm, entry));
            plr.Teleport(entry.ZoneId, entry.WorldX, entry.WorldY, entry.WorldZ, entry.WorldO);
            LogTeleportCommand(plr, $"TELEPORT ENTRY {zone.ZoneId} {entry.WorldX} {entry.WorldY} {entry.WorldZ} [{entry.Describe()}]");

            return true;
        }

        /// <summary>
        /// Displays the derived safe center and portal arrival point for the given map without teleporting.
        /// </summary>
        /// <param name="plr">Player that initiated the command</param>
        /// <param name="values">List of command arguments (after command name)</param>
        /// <returns>True if command was correctly handled, false if operation was canceled</returns>
        public static bool TeleportInfo(Player plr, ref List<string> values)
        {
            Zone_Info zone;
            byte realm;
            if (!TryResolveZoneAndRealm(plr, ref values, out zone, out realm))
                return false;

            List<DerivedTeleportPoint> candidates = BuildDerivedTeleportPoints(zone, realm);
            DerivedTeleportPoint center = ResolveSafeCenter(candidates);
            DerivedTeleportPoint entry = ResolvePortalArrival(plr, zone, candidates, center);

            plr.SendClientMessage($"TELEPORT INFO: {zone.Name} [{GetRealmLabel(realm)}]");
            if (center != null)
                plr.SendClientMessage("CENTER: " + BuildCoordinateLine(center));
            else
                plr.SendClientMessage("CENTER: unavailable");

            if (entry != null)
                plr.SendClientMessage("ENTRY: " + BuildCoordinateLine(entry));
            else
                plr.SendClientMessage("ENTRY: unavailable");

            return true;
        }

        /// <summary>
        /// Teleport to an objective.
        /// </summary>
        /// <param name="plr">Player that initiated the command</param>
        /// <param name="values">List of command arguments (after command name. One value - respawnId</param>
        /// <returns>True if command was correctly handled, false if operation was canceled</returns>
        public static bool TeleportObjective(Player plr, ref List<string> values)
        {
            IList<BattleFront_Objective> BattleFrontObjectives = WorldMgr.Database.SelectAllObjects<BattleFront_Objective>();
            var respawnToTravelTo = GetInt(ref values);

            var BattleFrontObjective = BattleFrontObjectives.SingleOrDefault(x => x.Entry == respawnToTravelTo);
            if (BattleFrontObjective == null)
                return false;

            // X+50 so you dont get stuck on flags on the objective
            plr.Teleport((ushort)BattleFrontObjective.ZoneId, (uint)BattleFrontObjective.X + 50, (uint)BattleFrontObjective.Y, (ushort)BattleFrontObjective.Z, 0);

            return true;
        }

        /// <summary>
        /// Teleports you to a player's location (string playerName)
        /// </summary>
        /// <param name="plr">Player that initiated the command</param>
        /// <param name="values">List of command arguments (after command name)</param>
        /// <returns>True if command was correctly handled, false if operation was canceled</returns>
        public static bool TeleportAppear(Player plr, ref List<string> values)
        {
            string playerName = GetString(ref values);

            Player target = Player.GetPlayer(playerName);

            if (target == null)
            {
                plr.SendClientMessage($"TELEPORT APPEAR: {playerName} could not be found.");
                return true;
            }

            if (target.Zone == null)
                return false;

            plr.Teleport(target.Region, target.Zone.ZoneId, (uint)target.WorldPosition.X, (uint)target.WorldPosition.Y, (ushort)target.WorldPosition.Z, target.Heading);

            GMCommandLog log = new GMCommandLog
            {
                PlayerName = plr.Name,
                AccountId = (uint)plr.Client._Account.AccountId,
                Command = $"TELEPORTED TO PLAYER {target.Name} AT ZONE {target.Zone.ZoneId} LOCATION {target._Value.WorldX} {target._Value.WorldY}",
                Date = DateTime.Now
            };
            CharMgr.Database.AddObject(log);

            return true;
        }

        /// <summary>
        /// Summons a player/group to your location (string playerName optional GROUP)
        /// </summary>
        /// <param name="plr">Player that initiated the command</param>
        /// <param name="values">List of command arguments (after command name)</param>
        /// <returns>True if command was correctly handled, false if operation was canceled</returns>
        public static bool TeleportSummon(Player plr, ref List<string> values)
        {
            string playerName = GetString(ref values);
            bool group = false;

            if (values.Count > 1)
                group = GetString(ref values).ToUpper().Trim() == "GROUP";

            Player t = Player.GetPlayer(playerName);

            if (t == null)
            {
                plr.SendClientMessage("TELEPORT SUMMON: " + playerName + " could not be found.");
                return true;
            }

            var list = new List<Player>() { t };
            if (t.WorldGroup != null && group)
                list.AddRange(t.WorldGroup.GetPlayerListCopy().Where(e => e.Info.CharacterId != t.Info.CharacterId).ToList());

            foreach (var target in list)
            {
                target.Teleport(plr.Region, plr.Zone.ZoneId, (uint)plr.WorldPosition.X, (uint)plr.WorldPosition.Y, (ushort)plr.WorldPosition.Z, 0);
                target.IsSummoned = true;

                //allow summoned player to enter illegal area (if summoned to it), unset it after 30 seconds
                target.EvtInterface.AddEvent((player) =>
                {
                    ((Player)player).IsSummoned = false;
                }, 30000, 1, target);

                GMCommandLog log = new GMCommandLog();
                log.PlayerName = plr.Name;
                log.AccountId = (uint)plr.Client._Account.AccountId;
                log.Command = "SUMMON PLAYER " + target.Name + " TO " + plr.Zone.ZoneId + " " + plr._Value.WorldX + " " + plr._Value.WorldY;
                log.Date = DateTime.UtcNow;
                CharMgr.Database.AddObject(log);

                target.SendLocalizeString(plr.Name, ChatLogFilters.CHATLOGFILTERS_SAY, Localized_text.TEXT_BEEN_SUMMONED_TO_X);
            }

            return true;
        }

        /// <summary>
        /// Sets offline/online players coordinates in database (player_name byte byte ZoneID , uint WorldX, uint WorldY, uint WorldZ)
        /// </summary>
        /// <param name="plr">Player that initiated the command</param>
        /// <param name="values">List of command arguments (after command name)</param>
        /// <returns>True if command was correctly handled, false if operation was canceled</returns>
        public static bool TeleportSet(Player plr, ref List<string> values)
        {
            string playerName = GetString(ref values);
            int zoneID = GetZoneId(plr, ref values);
            if (zoneID == -1)
                return false;
            int worldX = GetInt(ref values);
            int worldY = GetInt(ref values);
            int worldZ = GetInt(ref values);

            Zone_Info zone = ZoneService.GetZone_Info((ushort)zoneID);
            if (zone == null)
                zone = ZoneService._Zone_Info[0];

            var existingChar = CharMgr.GetCharacter(Player.AsCharacterName(playerName), false);
            if (existingChar == null)
            {
                plr.SendClientMessage("Player with name '" + values[0] + "' not found.", ChatLogFilters.CHATLOGFILTERS_USER_ERROR);
                return true;
            }

            var player = Player.GetPlayer(playerName);

            GMCommandLog log = new GMCommandLog
            {
                PlayerName = plr.Name,
                AccountId = (uint)plr.Client._Account.AccountId,
                Command = "TELEPORT offline player '" + existingChar.Name + "' TO " + zoneID + " " + worldX + " " + worldY,
                Date = DateTime.Now
            };
            CharMgr.Database.AddObject(log);

            if (player != null)
                player.Teleport((ushort)zoneID, (uint)worldX, (uint)worldY, (ushort)worldZ, 0);

            existingChar.Value.WorldX = worldX;
            existingChar.Value.WorldY = worldY;
            existingChar.Value.WorldZ = worldZ;
            existingChar.Value.ZoneId = zone.ZoneId;
            existingChar.Value.RegionId = zone.Region;

            CharMgr.Database.SaveObject(existingChar.Value);
            CharMgr.Database.ForceSave();

            return true;
        }

        private static bool TryResolveZoneAndRealm(Player plr, ref List<string> values, out Zone_Info zone, out byte realm)
        {
            zone = null;
            realm = GetDefaultRealm(plr);

            int zoneID = GetZoneId(plr, ref values);
            if (zoneID == -1)
                return false;

            zone = ZoneService.GetZone_Info((ushort)zoneID);
            if (zone == null)
            {
                plr.SendClientMessage($"TELEPORT: Zone {zoneID} does not exist.", ChatLogFilters.CHATLOGFILTERS_USER_ERROR);
                return false;
            }

            if (values.Count == 0)
                return true;

            if (values.Count > 1)
            {
                plr.SendClientMessage("TELEPORT: Too many arguments. Use <zone> [order|destruction|neutral].", ChatLogFilters.CHATLOGFILTERS_USER_ERROR);
                return false;
            }

            string realmToken = TrimMatchingQuotes(values[0]);
            byte parsedRealm;
            if (!TryParseRealm(realmToken, out parsedRealm))
            {
                plr.SendClientMessage($"TELEPORT: Invalid realm '{realmToken}'. Use order, destruction, or neutral.", ChatLogFilters.CHATLOGFILTERS_USER_ERROR);
                return false;
            }

            values.RemoveAt(0);
            realm = parsedRealm;
            return true;
        }

        private static byte GetDefaultRealm(Player plr)
        {
            if (plr.Realm == Realms.REALMS_REALM_ORDER)
                return (byte)Realms.REALMS_REALM_ORDER;

            if (plr.Realm == Realms.REALMS_REALM_DESTRUCTION)
                return (byte)Realms.REALMS_REALM_DESTRUCTION;

            return (byte)Realms.REALMS_REALM_ORDER;
        }

        private static bool TryParseRealm(string rawRealm, out byte realm)
        {
            realm = 0;
            if (string.IsNullOrWhiteSpace(rawRealm))
                return false;

            switch (rawRealm.Trim().ToLowerInvariant())
            {
                case "0":
                case "neutral":
                case "any":
                    realm = (byte)Realms.REALMS_REALM_NEUTRAL;
                    return true;
                case "1":
                case "order":
                case "ord":
                case "o":
                    realm = (byte)Realms.REALMS_REALM_ORDER;
                    return true;
                case "2":
                case "destruction":
                case "destro":
                case "des":
                case "d":
                    realm = (byte)Realms.REALMS_REALM_DESTRUCTION;
                    return true;
                default:
                    return false;
            }
        }

        private static List<DerivedTeleportPoint> BuildDerivedTeleportPoints(Zone_Info zone, byte realm)
        {
            List<DerivedTeleportPoint> candidates = new List<DerivedTeleportPoint>();

            AddRespawnCandidates(candidates, zone.ZoneId, realm);
            AddTaxiCandidates(candidates, zone.ZoneId, realm);
            AddRallyPointCandidates(candidates, zone.ZoneId);
            AddChapterCandidates(candidates, zone);
            AddZoneJumpCandidates(candidates, zone.ZoneId);
            AddZoneInfoCenterCandidate(candidates, zone);

            return candidates;
        }

        private static void AddRespawnCandidates(List<DerivedTeleportPoint> candidates, ushort zoneId, byte realm)
        {
            List<Zone_Respawn> respawns = ZoneService.GetZoneRespawns(zoneId);
            if (respawns == null)
                return;

            foreach (Zone_Respawn respawn in respawns)
            {
                ushort destinationZoneId = respawn.InZoneID != 0 ? (ushort)respawn.InZoneID : (ushort)respawn.ZoneID;
                if (destinationZoneId != zoneId)
                    continue;

                SpawnPoint spawnPoint = new SpawnPoint(respawn);
                if (spawnPoint.X <= 0 || spawnPoint.Y <= 0 || spawnPoint.Z < 0)
                    continue;

                bool realmMatch = realm == (byte)Realms.REALMS_REALM_NEUTRAL || respawn.Realm == realm;
                int weight = realmMatch ? 120 : 55;
                AddCandidate(candidates,
                    destinationZoneId,
                    (uint)spawnPoint.X,
                    (uint)spawnPoint.Y,
                    (ushort)spawnPoint.Z,
                    respawn.WorldO,
                    DerivedTeleportSource.ZoneRespawn,
                    $"zone_respawns#{respawn.RespawnID} realm={respawn.Realm}",
                    weight,
                    realmMatch,
                    null);
            }
        }

        private static void AddTaxiCandidates(List<DerivedTeleportPoint> candidates, ushort zoneId, byte realm)
        {
            for (byte taxiRealm = (byte)Realms.REALMS_REALM_ORDER; taxiRealm <= (byte)Realms.REALMS_REALM_DESTRUCTION; ++taxiRealm)
            {
                Zone_Taxi taxi = ZoneService.GetZoneTaxi(zoneId, taxiRealm);
                if (taxi == null)
                    continue;

                bool realmMatch = realm == (byte)Realms.REALMS_REALM_NEUTRAL || taxiRealm == realm;
                int weight = realmMatch ? 110 : 50;
                if (!taxi.Enable)
                    weight -= 20;

                AddCandidate(candidates,
                    zoneId,
                    taxi.WorldX,
                    taxi.WorldY,
                    taxi.WorldZ,
                    taxi.WorldO,
                    DerivedTeleportSource.ZoneTaxi,
                    $"zone_taxis realm={taxiRealm}" + (taxi.Enable ? string.Empty : " disabled"),
                    weight,
                    realmMatch,
                    null);
            }
        }

        private static void AddRallyPointCandidates(List<DerivedTeleportPoint> candidates, ushort zoneId)
        {
            if (RallyPointService.RallyPoints == null)
                return;

            foreach (RallyPoint rallyPoint in RallyPointService.RallyPoints)
            {
                if (rallyPoint.ZoneID != zoneId)
                    continue;

                AddCandidate(candidates,
                    zoneId,
                    rallyPoint.WorldX,
                    rallyPoint.WorldY,
                    rallyPoint.WorldZ,
                    rallyPoint.WorldO,
                    DerivedTeleportSource.RallyPoint,
                    $"rallypoints#{rallyPoint.Id}",
                    95,
                    true,
                    null);
            }
        }

        private static void AddChapterCandidates(List<DerivedTeleportPoint> candidates, Zone_Info zone)
        {
            List<Chapter_Info> chapters = ChapterService.GetChapters(zone.ZoneId);
            if (chapters == null)
                return;

            foreach (Chapter_Info chapter in chapters)
            {
                int height = ClientFileMgr.GetHeight(zone.ZoneId, chapter.PinX, chapter.PinY);
                if (height < 0)
                    continue;

                Point3D world = ZoneService.GetWorldPosition(zone, chapter.PinX, chapter.PinY, (ushort)height);
                if (world.X <= 0 || world.Y <= 0 || world.Z < 0)
                    continue;

                AddCandidate(candidates,
                    zone.ZoneId,
                    (uint)world.X,
                    (uint)world.Y,
                    (ushort)world.Z,
                    0,
                    DerivedTeleportSource.ChapterPin,
                    $"chapter_infos#{chapter.Entry} {chapter.Name}",
                    80,
                    true,
                    null);
            }
        }

        private static void AddZoneJumpCandidates(List<DerivedTeleportPoint> candidates, ushort zoneId)
        {
            if (ZoneService.Zone_Jumps == null)
                return;

            foreach (Zone_jump jump in ZoneService.Zone_Jumps.Values)
            {
                if (jump.ZoneID != zoneId)
                    continue;

                GameObject_spawn sourceSpawn = null;
                ushort? sourceZoneId = null;
                bool external = false;

                if (GameObjectService.GameObjectSpawns != null && GameObjectService.GameObjectSpawns.TryGetValue(jump.Entry, out sourceSpawn))
                {
                    sourceZoneId = sourceSpawn.ZoneId;
                    external = sourceSpawn.ZoneId != zoneId;
                }

                AddCandidate(candidates,
                    jump.ZoneID,
                    jump.WorldX,
                    jump.WorldY,
                    jump.WorldZ,
                    jump.WorldO,
                    external ? DerivedTeleportSource.ExternalZoneJump : DerivedTeleportSource.InternalZoneJump,
                    BuildZoneJumpDetail(jump, sourceSpawn),
                    external ? 65 : 25,
                    true,
                    sourceZoneId);
            }
        }

        private static void AddZoneInfoCenterCandidate(List<DerivedTeleportPoint> candidates, Zone_Info zone)
        {
            DerivedTeleportPoint fallbackPoint = TryCreateZoneInfoPoint(
                zone,
                GetCenterAnchors(),
                DerivedTeleportSource.ZoneInfoCenter,
                $"zone_infos off={zone.OffX},{zone.OffY}",
                20);

            if (fallbackPoint != null)
                candidates.Add(fallbackPoint);
        }

        private static void AddCandidate(List<DerivedTeleportPoint> candidates, ushort zoneId, uint worldX, uint worldY, ushort worldZ, ushort worldO, DerivedTeleportSource source, string detail, int weight, bool realmMatch, ushort? sourceZoneId)
        {
            if (worldX == 0 || worldY == 0)
                return;

            DerivedTeleportPoint candidate = new DerivedTeleportPoint
            {
                ZoneId = zoneId,
                WorldX = worldX,
                WorldY = worldY,
                WorldZ = worldZ,
                WorldO = worldO,
                Source = source,
                Detail = detail,
                Weight = weight,
                RealmMatch = realmMatch,
                SourceZoneId = sourceZoneId
            };

            AlignCandidateToTerrain(candidate);
            candidates.Add(candidate);
        }

        private static DerivedTeleportPoint ResolveSafeCenter(List<DerivedTeleportPoint> candidates)
        {
            DerivedTeleportPoint preferred = SelectCenterCandidate(candidates, candidate => IsHighConfidenceCenterAnchor(candidate) && candidate.RealmMatch && candidate.TerrainValidated);
            if (preferred != null)
                return preferred;

            preferred = SelectCenterCandidate(candidates, candidate => IsHighConfidenceCenterAnchor(candidate) && candidate.TerrainValidated);
            if (preferred != null)
                return preferred;

            preferred = SelectCenterCandidate(candidates, candidate => candidate.Source == DerivedTeleportSource.ExternalZoneJump && candidate.TerrainValidated);
            if (preferred != null)
                return preferred;

            preferred = SelectCenterCandidate(candidates, candidate => candidate.Source == DerivedTeleportSource.ZoneInfoCenter);
            if (preferred != null)
                return preferred;

            preferred = SelectCenterCandidate(candidates, candidate => IsHighConfidenceCenterAnchor(candidate) && candidate.RealmMatch);
            if (preferred != null)
                return preferred;

            preferred = SelectCenterCandidate(candidates, candidate => IsHighConfidenceCenterAnchor(candidate));
            if (preferred != null)
                return preferred;

            preferred = SelectCenterCandidate(candidates, candidate => candidate.IsPortalJump && candidate.TerrainValidated);
            if (preferred != null)
                return preferred;

            return SelectCenterCandidate(candidates, candidate => candidate.IsPortalJump);
        }

        private static DerivedTeleportPoint SelectCenterCandidate(List<DerivedTeleportPoint> candidates, Func<DerivedTeleportPoint, bool> predicate)
        {
            List<DerivedTeleportPoint> selection = candidates.Where(predicate).ToList();
            return selection.Count > 0 ? SelectRepresentative(selection) : null;
        }

        private static bool IsHighConfidenceCenterAnchor(DerivedTeleportPoint candidate)
        {
            switch (candidate.Source)
            {
                case DerivedTeleportSource.ZoneRespawn:
                case DerivedTeleportSource.ZoneTaxi:
                case DerivedTeleportSource.RallyPoint:
                case DerivedTeleportSource.ChapterPin:
                    return true;
                default:
                    return false;
            }
        }

        private static DerivedTeleportPoint ResolvePortalArrival(Player plr, Zone_Info zone, List<DerivedTeleportPoint> candidates, DerivedTeleportPoint safeCenter)
        {
            List<DerivedTeleportPoint> externalPortals = candidates.Where(candidate => candidate.IsExternalPortal && candidate.TerrainValidated).ToList();
            if (externalPortals.Count > 0)
                return SelectPortalArrivalCandidate(plr, externalPortals, safeCenter);

            List<DerivedTeleportPoint> allPortals = candidates.Where(candidate => candidate.IsPortalJump && candidate.TerrainValidated).ToList();
            if (allPortals.Count > 0)
                return SelectPortalArrivalCandidate(plr, allPortals, safeCenter);

            DerivedTeleportPoint zoneInfoEntry = TryCreateZoneInfoEntryPoint(zone, plr?.Zone?.Info);
            if (zoneInfoEntry != null)
                return zoneInfoEntry;

            externalPortals = candidates.Where(candidate => candidate.IsExternalPortal).ToList();
            if (externalPortals.Count > 0)
                return SelectPortalArrivalCandidate(plr, externalPortals, safeCenter);

            allPortals = candidates.Where(candidate => candidate.IsPortalJump).ToList();
            if (allPortals.Count > 0)
                return SelectPortalArrivalCandidate(plr, allPortals, safeCenter);

            if (safeCenter == null)
                return null;

            return new DerivedTeleportPoint
            {
                ZoneId = safeCenter.ZoneId,
                WorldX = safeCenter.WorldX,
                WorldY = safeCenter.WorldY,
                WorldZ = safeCenter.WorldZ,
                WorldO = safeCenter.WorldO,
                Source = DerivedTeleportSource.SafeCenterFallback,
                Detail = $"from {safeCenter.Describe()}",
                Weight = safeCenter.Weight,
                RealmMatch = safeCenter.RealmMatch,
                SourceZoneId = safeCenter.SourceZoneId,
                TerrainValidated = safeCenter.TerrainValidated,
                PinX = safeCenter.PinX,
                PinY = safeCenter.PinY
            };
        }

        private static DerivedTeleportPoint SelectPortalArrivalCandidate(Player plr, List<DerivedTeleportPoint> portalCandidates, DerivedTeleportPoint safeCenter)
        {
            if (plr.Zone != null)
            {
                List<DerivedTeleportPoint> fromCurrentZone = portalCandidates
                    .Where(candidate => candidate.SourceZoneId.HasValue && candidate.SourceZoneId.Value == plr.Zone.ZoneId)
                    .ToList();

                if (fromCurrentZone.Count > 0)
                {
                    List<DerivedTeleportPoint> validatedFromCurrentZone = fromCurrentZone.Where(candidate => candidate.TerrainValidated).ToList();
                    if (validatedFromCurrentZone.Count > 0)
                        return safeCenter != null ? SelectNearestPoint(validatedFromCurrentZone, safeCenter.WorldX, safeCenter.WorldY) : SelectRepresentative(validatedFromCurrentZone);

                    return safeCenter != null ? SelectNearestPoint(fromCurrentZone, safeCenter.WorldX, safeCenter.WorldY) : SelectRepresentative(fromCurrentZone);
                }
            }

            List<DerivedTeleportPoint> validatedCandidates = portalCandidates.Where(candidate => candidate.TerrainValidated).ToList();
            if (validatedCandidates.Count > 0)
                return safeCenter != null ? SelectNearestPoint(validatedCandidates, safeCenter.WorldX, safeCenter.WorldY) : SelectRepresentative(validatedCandidates);

            if (safeCenter != null)
                return SelectNearestPoint(portalCandidates, safeCenter.WorldX, safeCenter.WorldY);

            return SelectRepresentative(portalCandidates);
        }

        private static DerivedTeleportPoint TryCreateZoneInfoEntryPoint(Zone_Info targetZone, Zone_Info sourceZone)
        {
            List<DerivedPinAnchor> anchors = GetEntryAnchors(targetZone, sourceZone);
            return TryCreateZoneInfoPoint(
                targetZone,
                anchors,
                DerivedTeleportSource.ZoneInfoEntry,
                $"zone_infos entry off={targetZone.OffX},{targetZone.OffY}" + (sourceZone != null ? $" from={sourceZone.ZoneId}" : string.Empty),
                18);
        }

        private static DerivedTeleportPoint TryCreateZoneInfoPoint(Zone_Info zone, List<DerivedPinAnchor> anchors, DerivedTeleportSource source, string detailPrefix, int weight)
        {
            if (zone == null || anchors == null || anchors.Count == 0)
                return null;

            DerivedPinAnchor resolvedAnchor;
            ushort height;
            if (!TryResolveTerrainPin(zone, anchors, ZoneInfoRingOffsets, out resolvedAnchor, out height))
                return null;

            Point3D world = ZoneService.GetWorldPosition(zone, resolvedAnchor.PinX, resolvedAnchor.PinY, height);
            if (world.X <= 0 || world.Y <= 0 || world.Z < 0)
                return null;

            return new DerivedTeleportPoint
            {
                ZoneId = zone.ZoneId,
                WorldX = (uint)world.X,
                WorldY = (uint)world.Y,
                WorldZ = (ushort)world.Z,
                WorldO = 0,
                Source = source,
                Detail = $"{detailPrefix} {resolvedAnchor.Label} pin={resolvedAnchor.PinX},{resolvedAnchor.PinY}",
                Weight = weight,
                RealmMatch = true,
                TerrainValidated = true,
                PinX = resolvedAnchor.PinX,
                PinY = resolvedAnchor.PinY
            };
        }

        private static void AlignCandidateToTerrain(DerivedTeleportPoint candidate)
        {
            Zone_Info zone = ZoneService.GetZone_Info(candidate.ZoneId);
            if (zone == null)
                return;

            ushort pinX;
            ushort pinY;
            if (!TryGetPinFromWorld(zone, candidate.WorldX, candidate.WorldY, out pinX, out pinY))
                return;

            DerivedPinAnchor resolvedAnchor;
            ushort height;
            if (!TryResolveTerrainPin(zone,
                    new List<DerivedPinAnchor> { new DerivedPinAnchor { PinX = pinX, PinY = pinY, Label = "db" } },
                    CandidateSnapOffsets,
                    out resolvedAnchor,
                    out height))
                return;

            Point3D world = ZoneService.GetWorldPosition(zone, resolvedAnchor.PinX, resolvedAnchor.PinY, height);
            if (world.X <= 0 || world.Y <= 0 || world.Z < 0)
                return;

            candidate.WorldX = (uint)world.X;
            candidate.WorldY = (uint)world.Y;
            candidate.WorldZ = (ushort)world.Z;
            candidate.TerrainValidated = true;
            candidate.PinX = resolvedAnchor.PinX;
            candidate.PinY = resolvedAnchor.PinY;
            candidate.Detail += resolvedAnchor.PinX == pinX && resolvedAnchor.PinY == pinY
                ? $" pin={resolvedAnchor.PinX},{resolvedAnchor.PinY}"
                : $" snap={resolvedAnchor.PinX},{resolvedAnchor.PinY} from={pinX},{pinY}";
        }

        private static bool TryGetPinFromWorld(Zone_Info zone, uint worldX, uint worldY, out ushort pinX, out ushort pinY)
        {
            pinX = 0;
            pinY = 0;

            if (zone == null)
                return false;

            int localX = (int)worldX - (zone.OffX << 12);
            int localY = (int)worldY - (zone.OffY << 12);
            if (localX < 0 || localX > UInt16.MaxValue || localY < 0 || localY > UInt16.MaxValue)
                return false;

            pinX = (ushort)localX;
            pinY = (ushort)localY;
            return true;
        }

        private static bool TryResolveTerrainPin(Zone_Info zone, List<DerivedPinAnchor> anchors, int[] ringOffsets, out DerivedPinAnchor resolvedAnchor, out ushort height)
        {
            resolvedAnchor = null;
            height = 0;
            if (zone == null || anchors == null || anchors.Count == 0)
                return false;

            HashSet<int> visitedPins = new HashSet<int>();
            foreach (DerivedPinAnchor anchor in anchors)
            {
                foreach (DerivedPinAnchor candidate in ExpandAnchor(anchor, ringOffsets))
                {
                    int visitedKey = (candidate.PinX << 16) | candidate.PinY;
                    if (!visitedPins.Add(visitedKey))
                        continue;

                    int terrainHeight = ClientFileMgr.GetHeight(zone.ZoneId, candidate.PinX, candidate.PinY);
                    if (terrainHeight < 0)
                        continue;

                    resolvedAnchor = candidate;
                    height = (ushort)terrainHeight;
                    return true;
                }
            }

            return false;
        }

        private static List<DerivedPinAnchor> ExpandAnchor(DerivedPinAnchor anchor, int[] ringOffsets)
        {
            List<DerivedPinAnchor> expanded = new List<DerivedPinAnchor>();
            if (anchor == null || ringOffsets == null || ringOffsets.Length == 0)
                return expanded;

            for (int ring = 0; ring < ringOffsets.Length; ++ring)
            {
                int offset = ringOffsets[ring];
                AddExpandedAnchor(expanded, anchor, offset, 0, ring);
                if (offset == 0)
                    continue;

                AddExpandedAnchor(expanded, anchor, 0, offset, ring);
                AddExpandedAnchor(expanded, anchor, offset, offset, ring);
                AddExpandedAnchor(expanded, anchor, offset, -offset, ring);
            }

            return expanded;
        }

        private static void AddExpandedAnchor(List<DerivedPinAnchor> expanded, DerivedPinAnchor anchor, int offsetX, int offsetY, int ring)
        {
            ushort pinX = ClampPin(anchor.PinX + offsetX);
            ushort pinY = ClampPin(anchor.PinY + offsetY);

            expanded.Add(new DerivedPinAnchor
            {
                PinX = pinX,
                PinY = pinY,
                Label = ring == 0 ? anchor.Label : $"{anchor.Label}-ring{ring}"
            });
        }

        private static ushort ClampPin(int pin)
        {
            if (pin < 4096)
                return 4096;

            if (pin > 61440)
                return 61440;

            return (ushort)pin;
        }

        private static List<DerivedPinAnchor> GetCenterAnchors()
        {
            return new List<DerivedPinAnchor>
            {
                new DerivedPinAnchor { PinX = 32768, PinY = 32768, Label = "center" },
                new DerivedPinAnchor { PinX = 28672, PinY = 32768, Label = "center-west" },
                new DerivedPinAnchor { PinX = 36864, PinY = 32768, Label = "center-east" },
                new DerivedPinAnchor { PinX = 32768, PinY = 28672, Label = "center-north" },
                new DerivedPinAnchor { PinX = 32768, PinY = 36864, Label = "center-south" },
                new DerivedPinAnchor { PinX = 28672, PinY = 28672, Label = "center-northwest" },
                new DerivedPinAnchor { PinX = 36864, PinY = 28672, Label = "center-northeast" },
                new DerivedPinAnchor { PinX = 28672, PinY = 36864, Label = "center-southwest" },
                new DerivedPinAnchor { PinX = 36864, PinY = 36864, Label = "center-southeast" },
                new DerivedPinAnchor { PinX = 24576, PinY = 32768, Label = "center-west-inner" },
                new DerivedPinAnchor { PinX = 40960, PinY = 32768, Label = "center-east-inner" },
                new DerivedPinAnchor { PinX = 32768, PinY = 24576, Label = "center-north-inner" },
                new DerivedPinAnchor { PinX = 32768, PinY = 40960, Label = "center-south-inner" }
            };
        }

        private static List<DerivedPinAnchor> GetEntryAnchors(Zone_Info targetZone, Zone_Info sourceZone)
        {
            List<DerivedPinAnchor> anchors = new List<DerivedPinAnchor>();
            if (targetZone == null)
                return anchors;

            if (sourceZone == null || sourceZone.ZoneId == targetZone.ZoneId)
            {
                AddDirectionalAnchorSet(anchors, "entry-west", 12288, 32768, true);
                AddDirectionalAnchorSet(anchors, "entry-east", 53248, 32768, true);
                AddDirectionalAnchorSet(anchors, "entry-north", 32768, 12288, false);
                AddDirectionalAnchorSet(anchors, "entry-south", 32768, 53248, false);
                anchors.Add(new DerivedPinAnchor { PinX = 32768, PinY = 32768, Label = "entry-center" });
                return anchors;
            }

            int targetCenterX = (targetZone.OffX << 12) + 32768;
            int targetCenterY = (targetZone.OffY << 12) + 32768;
            int sourceCenterX = (sourceZone.OffX << 12) + 32768;
            int sourceCenterY = (sourceZone.OffY << 12) + 32768;

            int deltaX = sourceCenterX - targetCenterX;
            int deltaY = sourceCenterY - targetCenterY;

            if (Math.Abs(deltaX) >= Math.Abs(deltaY))
            {
                if (deltaX < 0)
                    AddDirectionalAnchorSet(anchors, "entry-west", 12288, 32768, true);
                else
                    AddDirectionalAnchorSet(anchors, "entry-east", 53248, 32768, true);

                if (deltaY < 0)
                    AddDirectionalAnchorSet(anchors, "entry-north", 32768, 12288, false);
                else
                    AddDirectionalAnchorSet(anchors, "entry-south", 32768, 53248, false);
            }
            else
            {
                if (deltaY < 0)
                    AddDirectionalAnchorSet(anchors, "entry-north", 32768, 12288, false);
                else
                    AddDirectionalAnchorSet(anchors, "entry-south", 32768, 53248, false);

                if (deltaX < 0)
                    AddDirectionalAnchorSet(anchors, "entry-west", 12288, 32768, true);
                else
                    AddDirectionalAnchorSet(anchors, "entry-east", 53248, 32768, true);
            }

            anchors.Add(new DerivedPinAnchor { PinX = 32768, PinY = 32768, Label = "entry-center" });
            return anchors;
        }

        private static void AddDirectionalAnchorSet(List<DerivedPinAnchor> anchors, string labelPrefix, ushort basePinX, ushort basePinY, bool verticalEdge)
        {
            if (anchors == null)
                return;

            int[] offsets = { 0, -8192, 8192, -16384, 16384, -4096, 4096 };
            for (int index = 0; index < offsets.Length; ++index)
            {
                anchors.Add(new DerivedPinAnchor
                {
                    PinX = verticalEdge ? basePinX : ClampPin(basePinX + offsets[index]),
                    PinY = verticalEdge ? ClampPin(basePinY + offsets[index]) : basePinY,
                    Label = index == 0 ? labelPrefix : $"{labelPrefix}-{index}"
                });
            }

            ushort innerPinX = verticalEdge
                ? ClampPin(basePinX == 12288 ? 16384 : 49152)
                : basePinX;
            ushort innerPinY = verticalEdge
                ? basePinY
                : ClampPin(basePinY == 12288 ? 16384 : 49152);

            for (int index = 0; index < offsets.Length; ++index)
            {
                anchors.Add(new DerivedPinAnchor
                {
                    PinX = verticalEdge ? innerPinX : ClampPin(innerPinX + offsets[index]),
                    PinY = verticalEdge ? ClampPin(innerPinY + offsets[index]) : innerPinY,
                    Label = $"{labelPrefix}-inner-{index}"
                });
            }
        }

        private static DerivedTeleportPoint SelectRepresentative(List<DerivedTeleportPoint> candidates)
        {
            if (candidates == null || candidates.Count == 0)
                return null;

            if (candidates.Count == 1)
                return candidates[0];

            long totalWeight = 0;
            double weightedX = 0;
            double weightedY = 0;

            foreach (DerivedTeleportPoint candidate in candidates)
            {
                int weight = candidate.Weight > 0 ? candidate.Weight : 1;
                totalWeight += weight;
                weightedX += candidate.WorldX * weight;
                weightedY += candidate.WorldY * weight;
            }

            if (totalWeight <= 0)
                return candidates[0];

            double centerX = weightedX / totalWeight;
            double centerY = weightedY / totalWeight;

            DerivedTeleportPoint bestCandidate = null;
            double bestScore = double.MaxValue;

            foreach (DerivedTeleportPoint candidate in candidates)
            {
                double score = GetDistanceSquared(candidate.WorldX, candidate.WorldY, centerX, centerY);
                if (bestCandidate == null || score < bestScore || (Math.Abs(score - bestScore) < 0.001d && candidate.Weight > bestCandidate.Weight))
                {
                    bestCandidate = candidate;
                    bestScore = score;
                }
            }

            return bestCandidate;
        }

        private static DerivedTeleportPoint SelectNearestPoint(List<DerivedTeleportPoint> candidates, uint anchorX, uint anchorY)
        {
            DerivedTeleportPoint bestCandidate = null;
            double bestScore = double.MaxValue;

            foreach (DerivedTeleportPoint candidate in candidates)
            {
                double score = GetDistanceSquared(candidate.WorldX, candidate.WorldY, anchorX, anchorY);
                if (bestCandidate == null || score < bestScore || (Math.Abs(score - bestScore) < 0.001d && candidate.Weight > bestCandidate.Weight))
                {
                    bestCandidate = candidate;
                    bestScore = score;
                }
            }

            return bestCandidate;
        }

        private static double GetDistanceSquared(uint worldX, uint worldY, double targetX, double targetY)
        {
            double deltaX = worldX - targetX;
            double deltaY = worldY - targetY;
            return deltaX * deltaX + deltaY * deltaY;
        }

        private static string BuildZoneJumpDetail(Zone_jump jump, GameObject_spawn sourceSpawn)
        {
            string detail = $"zone_jumps#{jump.Entry}";

            if (!jump.Enabled)
                detail += " disabled";

            if (sourceSpawn != null)
                detail += $" source_guid={sourceSpawn.Guid} source_zone={sourceSpawn.ZoneId} source_entry={sourceSpawn.Entry}";

            return detail;
        }

        private static string BuildPreviewLine(string prefix, Zone_Info zone, byte realm, DerivedTeleportPoint point)
        {
            return $"{prefix}: {zone.Name} [{GetRealmLabel(realm)}] -> {BuildCoordinateLine(point)}";
        }

        private static string BuildCoordinateLine(DerivedTeleportPoint point)
        {
            return $"{point.ZoneId} {point.WorldX} {point.WorldY} {point.WorldZ} o:{point.WorldO} via {point.Describe()}";
        }

        private static string GetRealmLabel(byte realm)
        {
            switch ((Realms)realm)
            {
                case Realms.REALMS_REALM_ORDER:
                    return "Order";
                case Realms.REALMS_REALM_DESTRUCTION:
                    return "Destruction";
                default:
                    return "Neutral";
            }
        }

        private static string GetSourceLabel(DerivedTeleportSource source)
        {
            switch (source)
            {
                case DerivedTeleportSource.ZoneRespawn:
                    return "zone_respawn";
                case DerivedTeleportSource.ZoneTaxi:
                    return "zone_taxi";
                case DerivedTeleportSource.RallyPoint:
                    return "rally_point";
                case DerivedTeleportSource.ChapterPin:
                    return "chapter_pin";
                case DerivedTeleportSource.ExternalZoneJump:
                    return "external_zone_jump";
                case DerivedTeleportSource.InternalZoneJump:
                    return "internal_zone_jump";
                case DerivedTeleportSource.ZoneInfoCenter:
                    return "zone_info_center";
                case DerivedTeleportSource.ZoneInfoEntry:
                    return "zone_info_entry";
                case DerivedTeleportSource.SafeCenterFallback:
                    return "safe_center_fallback";
                default:
                    return "unknown";
            }
        }

        private static void LogTeleportCommand(Player plr, string command)
        {
            GMCommandLog log = new GMCommandLog
            {
                PlayerName = plr.Name,
                AccountId = (uint)plr.Client._Account.AccountId,
                Command = command,
                Date = DateTime.Now
            };

            CharMgr.Database.AddObject(log);
        }
    }
}
