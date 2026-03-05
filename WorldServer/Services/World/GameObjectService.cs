using Common;
using FrameWork;
using System.Collections.Generic;
using System.Threading;

namespace WorldServer.Services.World
{
    [Service(typeof(ItemService))]
    public class GameObjectService : ServiceBase
    {
        #region GameObjects
        public static Dictionary<uint, GameObject_proto> GameObjectProtos;
        public static Dictionary<uint, GameObject_spawn> GameObjectSpawns;
        public static int MaxGameObjectGUID;
        private static readonly HashSet<uint> RuntimeDoorFallbackProtos = new HashSet<uint>();
        // Entries sourced from the project door data set to keep legacy doors/gates spawning
        // when gameobject_protos is incomplete.
        private static readonly Dictionary<uint, string> DoorToolDoorEntries = new Dictionary<uint, string>
        {
            [2] = "Door",
            [6] = "Crypt Gate",
            [36] = "Cemetery Gate",
            [38] = "Bell Tower Door",
            [59] = "Monastery Door",
            [72] = "Postern Door",
            [86] = "Gate",
            [91] = "Castle Gatehouse",
            [100] = "Keep Door",
            [127] = "Lair Door",
            [128] = "Tower Door",
            [193] = "Jail Door",
            [391] = "Main Lock Door",
            [500] = "Belltower Door",
            [525] = "Pandemonium's Door",
            [526] = "Milegate Door",
            [543] = "Tavern Door",
            [562] = "Inn Door",
            [577] = "Empire Bar Door",
            [581] = "Slums Door",
            [582] = "Gate of Flames",
            [98724] = "Doors of Flame",
            [98806] = "Gatehouse Door",
            [98818] = "City Door",
            [98877] = "Mourkain Door",
            [98883] = "Door Switch",
            [98892] = "Pillars Door Object",
            [98959] = "Mingol Kurdak Door",
            [99010] = "Griffin Gate",
            [99163] = "Tower Doors",
            [99201] = "Gates of Thargrund",
            [99212] = "Pen Gate",
            [99213] = "Gatehouse",
            [99279] = "Cage Doors",
            [99283] = "Karaz-A-Karak Tower Door",
            [99322] = "Hallenfurt Manor Door",
            [99364] = "Estate Door",
            [99414] = "Ebon Keep Door",
            [99443] = "Monastery Gate",
            [99489] = "Dragon Gate",
            [99540] = "Cage Door",
            [99548] = "Gate of Chaos",
            [99757] = "Shrine Door",
            [99758] = "Empire Gate",
            [99762] = "Blackfire Pass Gate",
            [99800] = "Tomb Door",
            [99820] = "Gatehouse Door",
            [99886] = "Palace Gates",
            [99904] = "Unicorn Gate",
            [99999] = "Temp Door",
            [100300] = "Dead-bolted Postern Door",
            [100340] = "Morkfang Door",
            [100518] = "Cell Door",
            [100546] = "Mucus Door",
            [100551] = "Vine Gate",
            [100552] = "Gate of Nature",
            [200096] = "Door 2",
            [200162] = "Dwarf Door",
            [2000354] = "Dwarf Door Knocker",
            [2000366] = "Large Vine Gate",
            [2000456] = "Locked Gate"
        };

        public static int GenerateGameObjectSpawnGUID()
        {
            return Interlocked.Increment(ref MaxGameObjectGUID);
        }

        private static readonly Dictionary<uint, GameObject_proto> RequiredFallbackProtos = new Dictionary<uint, GameObject_proto>
        {
            [72] = BuildFallbackProto(
                72,
                "Postern Door",
                65535,
                50,
                1,
                0,
                1,
                "0",
                "7680 1 25966 0 5 45974",
                100,
                0),
            [100] = BuildFallbackProto(
                100,
                "Keep Door",
                65535,
                50,
                1,
                0,
                1,
                "0",
                "7682 1 26615 6 30441 30739",
                100,
                1),
            [188] = BuildFallbackProto(
                188,
                "Chest",
                1658,
                50,
                1,
                0,
                1,
                string.Empty,
                "0",
                0,
                0)
        };

        private static GameObject_proto BuildFallbackProto(
            uint entry,
            string name,
            ushort displayId,
            ushort scale,
            byte level,
            byte faction,
            uint healthPoints,
            string tokUnlock,
            string unks,
            uint unk3,
            byte isAttackable)
        {
            var proto = new GameObject_proto
            {
                Entry = entry,
                Name = name,
                DisplayID = displayId,
                Scale = scale,
                Level = level,
                Faction = faction,
                HealthPoints = healthPoints,
                ScriptName = string.Empty,
                TokUnlock = tokUnlock,
                Unk1 = 0,
                Unk2 = 0,
                Unk3 = unk3,
                Unk4 = 0,
                CreatureId = 0,
                CreatureCount = 0,
                CreatureSpawnText = null,
                CreatureCooldownMinutes = 0,
                IsAttackable = isAttackable
            };

            proto.UnksString = unks;
            return proto;
        }

        private static bool TryGetFallbackProto(uint entry, out GameObject_proto fallbackProto)
        {
            if (RequiredFallbackProtos.TryGetValue(entry, out fallbackProto))
                return true;

            fallbackProto = null;
            return false;
        }

        private static void EnsureRequiredFallbackProtos()
        {
            foreach (var fallback in RequiredFallbackProtos)
            {
                if (GameObjectProtos.ContainsKey(fallback.Key))
                    continue;

                var clone = BuildFallbackProto(
                    fallback.Value.Entry,
                    fallback.Value.Name,
                    fallback.Value.DisplayID,
                    fallback.Value.Scale,
                    fallback.Value.Level,
                    fallback.Value.Faction,
                    fallback.Value.HealthPoints,
                    fallback.Value.TokUnlock,
                    fallback.Value.UnksString,
                    fallback.Value.Unk3,
                    fallback.Value.IsAttackable);

                GameObjectProtos[fallback.Key] = clone;
                Log.Notice("WorldMgr", $"Injected fallback GameObject proto {fallback.Key} ({fallback.Value.Name}) because database entry is missing.");
            }
        }

        [LoadingFunction(true)]
        public static void LoadGameObjectProtos()
        {
            Log.Debug("WorldMgr", "Loading GameObject_Protos...");

            GameObjectProtos = Database.MapAllObjects<uint, GameObject_proto>("Entry");
            if (GameObjectProtos == null)
                GameObjectProtos = new Dictionary<uint, GameObject_proto>();

            EnsureRequiredFallbackProtos();

            Log.Success("WorldMgr", "Loaded " + GameObjectProtos.Count + " GameObject_Protos");
        }

        public static GameObject_proto GetGameObjectProto(uint Entry)
        {
            GameObject_proto Proto;
            if (GameObjectProtos != null && GameObjectProtos.TryGetValue(Entry, out Proto))
                return Proto;

            if (TryGetFallbackProto(Entry, out Proto))
            {
                if (GameObjectProtos == null)
                    GameObjectProtos = new Dictionary<uint, GameObject_proto>();

                GameObjectProtos[Entry] = Proto;
                Log.Notice("WorldMgr", $"Using fallback GameObject proto {Entry} ({Proto.Name}) at runtime.");
                return Proto;
            }

            return Proto;
        }

        public static GameObject_proto GetOrCreateRuntimeDoorProto(GameObject_spawn spawn)
        {
            if (spawn == null || spawn.Entry == 0)
                return null;

            GameObject_proto proto = GetGameObjectProto(spawn.Entry);
            if (proto != null)
                return proto;

            if (GameObjectProtos == null)
                GameObjectProtos = new Dictionary<uint, GameObject_proto>();

            ushort displayId = spawn.DisplayID > ushort.MaxValue ? (ushort)65535 : (ushort)spawn.DisplayID;
            if (displayId == 0)
                displayId = 65535;

            string unks = "0";
            if (spawn.Unks != null && spawn.Unks.Length > 0)
                unks = Utils.ConvertArrayToString(spawn.Unks);

            string name = GetRuntimeDoorName(spawn);
            proto = BuildFallbackProto(
                spawn.Entry,
                name,
                displayId,
                50,
                1,
                0,
                1,
                "0",
                unks,
                spawn.Unk3,
                0);

            GameObjectProtos[spawn.Entry] = proto;

            if (RuntimeDoorFallbackProtos.Add(spawn.Entry))
                Log.Notice("WorldMgr", $"Generated runtime fallback GameObject proto {spawn.Entry} ({name}) for door spawn guid={spawn.Guid}, doorId={spawn.DoorId}, zone={spawn.ZoneId}.");

            return proto;
        }

        public static bool ShouldCreateRuntimeDoorProto(GameObject_spawn spawn)
        {
            if (spawn == null || spawn.Entry == 0)
                return false;

            if (spawn.DoorId != 0)
                return true;

            if (DoorToolDoorEntries.ContainsKey(spawn.Entry))
                return true;

            if (!string.IsNullOrWhiteSpace(spawn.AlternativeName))
            {
                string alternativeName = spawn.AlternativeName.ToLowerInvariant();
                if (alternativeName.Contains("door") || alternativeName.Contains("gate"))
                    return true;
            }

            return false;
        }

        private static string GetRuntimeDoorName(GameObject_spawn spawn)
        {
            if (spawn == null)
                return "Runtime Door";

            if (DoorToolDoorEntries.TryGetValue(spawn.Entry, out string knownName) && !string.IsNullOrWhiteSpace(knownName))
                return knownName;

            if (!string.IsNullOrWhiteSpace(spawn.AlternativeName))
                return spawn.AlternativeName;

            return $"Runtime Door {spawn.Entry}";
        }

        [LoadingFunction(true)]
        public static void LoadGameObjectSpawns()
        {
            Log.Debug("WorldMgr", "Loading GameObject_Spawns...");

            GameObjectSpawns = Database.MapAllObjects<uint, GameObject_spawn>("Guid", 25000);

            foreach (GameObject_spawn Spawn in GameObjectSpawns.Values)
            {
                if (Spawn.Guid > MaxGameObjectGUID)
                    MaxGameObjectGUID = (int)Spawn.Guid;
            }

            Log.Success("WorldMgr", "Loaded " + GameObjectSpawns.Count + " GameObject_Spawns");
        }
        #endregion

        #region GameObjectLoots

        public static Dictionary<uint, List<GameObject_loot>> GameObjectLoots = new Dictionary<uint, List<GameObject_loot>>();
        private static void LoadGameObjectLoots(uint entry)
        {
            if (!GameObjectLoots.ContainsKey(entry))
            {
                Log.Debug("WorldMgr", "Loading GameObject Loots of " + entry + " ...");

                List<GameObject_loot> Loots = new List<GameObject_loot>();
                IList<GameObject_loot> ILoots = Database.SelectObjects<GameObject_loot>("Entry=" + entry);
                foreach (GameObject_loot Loot in ILoots)
                    Loots.Add(Loot);

                GameObjectLoots.Add(entry, Loots);

                long MissingGameObject = 0;
                long MissingItemProto = 0;

                if (GetGameObjectProto(entry) == null)
                {
                    Log.Debug("LoadLoots", "[" + entry + "] Invalid GameObject Proto");
                    ++MissingGameObject;
                }

                foreach (GameObject_loot Loot in GameObjectLoots[entry].ToArray())
                {
                    Loot.Info = ItemService.GetItem_Info(Loot.ItemId);

                    if (Loot.Info == null)
                    {
                        Log.Debug("LoadLoots", "[" + Loot.ItemId + "] Invalid Item Info");
                        GameObjectLoots[entry].Remove(Loot);
                        ++MissingItemProto;
                    }
                }

                if (MissingItemProto > 0)
                    Log.Error("LoadLoots", "[" + MissingItemProto + "] Missing Item Info");

                if (MissingGameObject > 0)
                    Log.Error("LoadLoots", "[" + MissingGameObject + "] Misssing GameObject proto");
            }
        }
        public static List<GameObject_loot> GetGameObjectLoots(uint Entry)
        {
            LoadGameObjectLoots(Entry);

            List<GameObject_loot> Loots;

            if (!GameObjectLoots.TryGetValue(Entry, out Loots))
                Loots = new List<GameObject_loot>();

            return Loots;
        }
        #endregion
    }
}
