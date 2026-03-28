using Common;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldServer.NetWork;
using WorldServer.World.Objects;
using WorldServer.World.Positions;
using WorldServer.Services.World;
using GameData;

namespace WorldServer.Managers
{
    public class BotFaction
    {
        public string Name;
        public byte[] Careers; // Healer, Ranged, Tank, Melee
    }

    public class BotManager
    {
        private static BotManager _instance;
        public static BotManager Instance => _instance ??= new BotManager();

        private Account _botAccount;
        private const string BOT_ACCOUNT_NAME = "BotAccount";
        private const int BOT_ACCOUNT_ID = 9999;

        private static readonly BotFaction[] OrderFactions = new BotFaction[]
        {
            new BotFaction { Name = "Empire", Careers = new byte[] { 12, 11, 10, 9 } },
            new BotFaction { Name = "Dwarfs", Careers = new byte[] { 3, 4, 1, 2 } },
            new BotFaction { Name = "High Elves", Careers = new byte[] { 20, 18, 17, 19 } }
        };

        private static readonly BotFaction[] DestroFactions = new BotFaction[]
        {
            new BotFaction { Name = "Chaos", Careers = new byte[] { 15, 16, 13, 14 } },
            new BotFaction { Name = "Greenskins", Careers = new byte[] { 7, 8, 5, 6 } },
            new BotFaction { Name = "Dark Elves", Careers = new byte[] { 23, 24, 21, 22 } }
        };

        public void Initialize()
        {
            _botAccount = Program.AcctMgr.GetAccount(BOT_ACCOUNT_NAME);
            if (_botAccount == null)
            {
                Log.Info("BotManager", $"Creating bot account: {BOT_ACCOUNT_NAME}");
                if (Program.AcctMgr.CreateAccount(BOT_ACCOUNT_NAME, "botpassword", 1))
                {
                    _botAccount = Program.AcctMgr.GetAccount(BOT_ACCOUNT_NAME);
                    if (_botAccount != null)
                        Log.Info("BotManager", "Created BotAccount.");
                    else
                        Log.Error("BotManager", "Created BotAccount but failed to load it!");
                }
                else
                {
                    Log.Error("BotManager", "Failed to create BotAccount!");
                }
            }
        }

        public Player CreateOrLoadBot(string name, byte career, byte level, byte renownRank, Realms realm, BotRole role)
        {
            if (_botAccount == null)
            {
                Log.Error("BotManager", "Cannot create bot: BotAccount is null.");
                return null;
            }

            if (!TryGetBotTemplate(career, out CharacterInfo template, out CharacterIdentityRecord identity))
                return null;

            Character character = CharMgr.GetCharacter(name);
            bool saveCharacter = false;
            bool saveValue = false;

            if (character == null)
            {
                character = new Character
                {
                    Anonymous = false,
                    Name = name,
                    Surname = string.Empty,
                    OldName = string.Empty,
                    PetName = string.Empty,
                    AccountId = _botAccount.AccountId,
                    RealmId = Program.Rm?.RealmId ?? 1,
                    Career = template.Career,
                    CareerLine = career,
                    Realm = identity.Realm,
                    Race = identity.Race,
                    ModelId = 0,
                    Sex = 0,
                    Hidden = false,
                    PetModel = 0,
                    bTraits = new byte[8],
                    FirstConnect = false
                };

                if (!CharMgr.CreateChar(character, true))
                {
                    Log.Error("BotManager", $"Failed to create bot character {name}");
                    return null;
                }

                character.Value = CreateBotCharacterValue(character, template, level, renownRank, realm);
                CharMgr.Database.AddObject(character.Value);
                character.ClientData = new CharacterClientData { CharacterId = character.CharacterId };
                CharMgr.Database.AddObject(character.ClientData);
            }
            else
            {
                if (character.Value == null)
                    character.Value = CharMgr.Database.SelectObject<Character_value>("CharacterId=" + character.CharacterId);

                if (character.ClientData == null)
                    character.ClientData = CharMgr.Database.SelectObject<CharacterClientData>("CharacterId=" + character.CharacterId);

                saveCharacter = RepairBotCharacter(character, template, identity);

                if (character.Value == null)
                {
                    character.Value = CreateBotCharacterValue(character, template, level, renownRank, realm);
                    CharMgr.Database.AddObject(character.Value);
                }
                else
                    saveValue = RepairBotCharacterValue(character, template, level, renownRank, realm);

                if (character.ClientData == null)
                {
                    character.ClientData = new CharacterClientData { CharacterId = character.CharacterId };
                    CharMgr.Database.AddObject(character.ClientData);
                }
            }

            if (EnsureBotCollections(character))
                saveCharacter = true;

            if (saveCharacter)
                CharMgr.Database.SaveObject(character);

            if (saveValue)
                CharMgr.Database.SaveObject(character.Value);

            Player existingPlayer = Player.GetPlayersSnapshot()
                .FirstOrDefault(player => player.IsBot && player.Info != null && player.Info.CharacterId == character.CharacterId);

            if (existingPlayer != null)
            {
                existingPlayer.IsBot = true;
                existingPlayer.Role = role;
                existingPlayer.CbtInterface.IsPvp = true;
                existingPlayer.Info.FirstConnect = false;
                if (existingPlayer.MaxActionPoints < 250)
                    existingPlayer.MaxActionPoints = 250;
                if (existingPlayer.ActionPoints == 0)
                    existingPlayer.ActionPoints = existingPlayer.MaxActionPoints;

                return existingPlayer;
            }

            BotClient client = new BotClient(Program.Server);
            client._Account = _botAccount;

            Player player = Player.CreatePlayer(client, character);
            if (player == null)
            {
                Log.Error("BotManager", $"Failed to create bot player object for {name}");
                return null;
            }

            player.IsBot = true;
            player.Role = role;
            player.CbtInterface.IsPvp = true;
            player.Info.FirstConnect = false;
            if (player.MaxActionPoints < 250)
                player.MaxActionPoints = 250;
            if (player.ActionPoints == 0)
                player.ActionPoints = player.MaxActionPoints;

            return player;
        }

        private static Point3D ResolveSpawnPoint(ushort zoneId, Realms realm, Zone_Info zoneInfo)
        {
            // 1. Warcamp entrance (preferred).
            Point3D warcampPin = BattleFrontService.GetWarcampEntrance(zoneId, realm);
            if (warcampPin != null)
                return ZoneService.GetWorldPosition(zoneInfo, (ushort)warcampPin.X, (ushort)warcampPin.Y, (ushort)warcampPin.Z);

            // 2. Zone respawn point for this realm (or any realm as fallback).
            Zone_Respawn respawn = ZoneService.GetZoneRespawn(zoneId, (byte)realm);
            if (respawn != null)
                return ZoneService.GetWorldPosition(zoneInfo, respawn.PinX, respawn.PinY, respawn.PinZ);

            // 3. Zone grid center.
            Log.Info("BotManager", $"No spawn anchor found for zone {zoneId} realm {realm}; using zone center.");
            return new Point3D((zoneInfo.OffX + 4) << 12, (zoneInfo.OffY + 4) << 12, 0);
        }

        public Group SpawnBotGroup(Realms realm, int tier, int rr, string groupPrefix, ushort zoneId)
        {
            Zone_Info zoneInfo = ZoneService.GetZone_Info(zoneId);
            if (zoneInfo == null)
            {
                Log.Error("BotManager", $"No zone info found for zone {zoneId}");
                return null;
            }

            var region = WorldMgr.GetRegion(zoneInfo.Region, true);
            if (region == null)
            {
                Log.Error("BotManager", $"No region found for zone {zoneId}");
                return null;
            }

            Point3D worldSpawn = ResolveSpawnPoint(zoneId, realm, zoneInfo);
            BotFaction[] factions = realm == Realms.REALMS_REALM_ORDER ? OrderFactions : DestroFactions;
            BotFaction faction = factions[RandomMgr.Next(factions.Length)];

            List<Player> bots = new List<Player>();
            byte[] careers = faction.Careers;

            byte healerCareer = careers[0];
            byte rangedCareer = careers[1];
            byte tankCareer = careers[2];
            byte meleeCareer = careers[3];

            // 1. Healer
            bots.Add(CreateOrLoadBot($"{groupPrefix}_H", healerCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.Healer));
            // 2. Ranged DPS
            bots.Add(CreateOrLoadBot($"{groupPrefix}_R", rangedCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.RangedDPS));
            // 3. Main Tank (Shield)
            bots.Add(CreateOrLoadBot($"{groupPrefix}_MT", tankCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.MainTank_Shield));
            // 4. Off Tank (2H)
            bots.Add(CreateOrLoadBot($"{groupPrefix}_OT", tankCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.OffTank_2H));
            // 5. Melee 1
            bots.Add(CreateOrLoadBot($"{groupPrefix}_M1", meleeCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.MeleeDPS));
            // 6. Melee 2
            bots.Add(CreateOrLoadBot($"{groupPrefix}_M2", meleeCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.MeleeDPS));

            if (bots.Any(bot => bot == null))
            {
                Log.Error("BotManager", $"Failed to create a full bot group for {groupPrefix}");
                return null;
            }

            Group group = new Group();
            Player leader = bots.FirstOrDefault(b => b != null);
            if (leader == null) return null;

            group.InitializeSolo(leader);
            foreach (var bot in bots.Skip(1))
            {
                if (bot != null)
                    group.AddMember(bot);
            }

            Player mt = bots[2];
            if (mt != null)
                group.SetMainAssist(mt);

            AssignToGuild(bots, faction.Name);

            foreach (var bot in bots)
            {
                StageBotSpawn(bot, zoneInfo, worldSpawn);
                ApplyLoadout(bot, tier, rr);
                region.AddObject(bot, zoneId);
            }

            CharMgr.Database.ForceSave();

            return group;
        }

        private void AssignToGuild(List<Player> bots, string guildName)
        {
            World.Guild.Guild guild = World.Guild.Guild.GetGuild(guildName);
            bool isNewGuild = false;
            
            Player firstBot = bots.FirstOrDefault(b => b != null);
            if (firstBot == null) return;

            if (guild == null)
            {
                isNewGuild = true;
                Guild_info info = new Guild_info
                {
                    Name = guildName,
                    Motd = "Bot Faction Guild",
                    LeaderId = firstBot.CharacterId,
                    AboutUs = "",
                    BriefDescription = string.Empty,
                    Summary = string.Empty,
                    Level = 40,
                    Realm = (byte)firstBot.Realm,
                    Xp = 0,
                    Tax = 0,
                    Money = 0,
                    Heraldry = string.Empty,
                    Banners = string.Empty,
                    guildvaultpurchased = new byte[5],
                    GuildTacticsPurchased = new ushort[40],
                    CreateDate = TCPManager.GetTimeStamp(),
                    GuildId = World.Guild.Guild.GenerateMaxGuildId(),
                    Members = new Dictionary<uint, Guild_member>(),
                    Ranks = new Dictionary<byte, Guild_rank>(),
                    Logs = new List<Guild_log>()
                };
                CharMgr.Database.AddObject(info);

                Guild_rank rank0 = new Guild_rank { GuildId = info.GuildId, RankId = 0, Name = "Initiate", Permissions = "00 30 00 02 89 44 20 10", Enabled = true };
                CharMgr.Database.AddObject(rank0); info.Ranks.Add(0, rank0);
                Guild_rank rank1 = new Guild_rank { GuildId = info.GuildId, RankId = 1, Name = "Member", Permissions = "00 B0 80 06 8F CF 60 10", Enabled = true };
                CharMgr.Database.AddObject(rank1); info.Ranks.Add(1, rank1);
                
                for (byte i = 2; i < 8; i++)
                {
                    Guild_rank rankUnused = new Guild_rank { GuildId = info.GuildId, RankId = i, Name = "Unused Status", Permissions = "00 00 00 00 08 00 00 00", Enabled = false };
                    CharMgr.Database.AddObject(rankUnused);
                    info.Ranks.Add(i, rankUnused);
                }

                Guild_rank rank8 = new Guild_rank { GuildId = info.GuildId, RankId = 8, Name = "Officer", Permissions = "2F F0 BF 9E 9F DF E5 33", Enabled = true };
                CharMgr.Database.AddObject(rank8); info.Ranks.Add(8, rank8);
                Guild_rank rank9 = new Guild_rank { GuildId = info.GuildId, RankId = 9, Name = "Guild Leader", Permissions = "FF FF FF FF FF FF FF 3F", Enabled = true };
                CharMgr.Database.AddObject(rank9); info.Ranks.Add(9, rank9);

                guild = new World.Guild.Guild(info);
                World.Guild.Guild.Guilds.Add(guild);
                guild.AddGuildLog(12, info.Name);
            }
            else if (!guild.Info.Members.ContainsKey(guild.Info.LeaderId))
            {
                guild.Info.LeaderId = firstBot.CharacterId;
                CharMgr.Database.SaveObject(guild.Info);
            }

            foreach (var bot in bots)
            {
                if (bot == null) continue;

                RemoveBotFromOtherGuilds(bot, guild);

                if (!guild.Info.Members.TryGetValue(bot.CharacterId, out Guild_member member))
                {
                    member = new Guild_member
                    {
                        CharacterId = bot.CharacterId,
                        GuildId = guild.Info.GuildId,
                        RankId = (bot == firstBot && isNewGuild) ? (byte)9 : (byte)1,
                        PublicNote = "",
                        OfficerNote = "",
                        Member = bot.Info,
                        JoinDate = (uint)TCPManager.GetTimeStamp()
                    };

                    guild.Info.Members.Add(bot.CharacterId, member);
                    CharMgr.Database.AddObject(member);
                    guild.AddGuildLog(0, bot.Name);
                    if (bot == firstBot && isNewGuild)
                        guild.AddGuildLog(11, bot.Name);
                }
                else
                {
                    member.Member = bot.Info;
                }

                bot.GldInterface.Guild = guild;
                bot.Info.Value.LeftSystemGuild = true;
            }
        }

        private void ApplyLoadout(Player bot, int tier, int rr)
        {
            if (bot?.Info == null)
                return;

            BotLoadoutManager.BotTier bTier = GetBotTier(tier, rr);
            var loadout = BotLoadoutManager.GetLoadout(bTier, (byte)bot.Info.CareerLine);
            if (loadout == null) return;

            List<CharacterItem> existingItems = CharMgr.GetItemsForCharacter(bot.Info) ?? new List<CharacterItem>();

            foreach (KeyValuePair<ushort, uint> itemEntry in loadout.SlotItems)
            {
                ushort slotId = itemEntry.Key;
                CharacterItem existing = existingItems.FirstOrDefault(item => item.SlotId == slotId);
                if (existing != null)
                    continue;

                Item_Info itemInfo = ItemService.GetItem_Info(itemEntry.Value);
                if (itemInfo == null)
                {
                    Log.Error("BotManager", $"Missing loadout item {itemEntry.Value} for {bot.Name}");
                    continue;
                }

                CharacterItem item = new CharacterItem
                {
                    CharacterId = bot.CharacterId,
                    Entry = itemEntry.Value,
                    ModelId = itemInfo.ModelId,
                    SlotId = slotId,
                    Counts = 1,
                    PrimaryDye = 0,
                    SecondaryDye = 0,
                    BoundtoPlayer = false
                };

                CharMgr.CreateItem(item);
                existingItems.Add(item);
            }
        }

        private BotLoadoutManager.BotTier GetBotTier(int tier, int rr)
        {
            if (tier < 4) return tier switch { 1 => BotLoadoutManager.BotTier.T1, 2 => BotLoadoutManager.BotTier.T2, 3 => BotLoadoutManager.BotTier.T3, _ => BotLoadoutManager.BotTier.T1 };

            return rr switch {
                >= 100 => BotLoadoutManager.BotTier.T4_RR100,
                >= 90 => BotLoadoutManager.BotTier.T4_RR90,
                >= 80 => BotLoadoutManager.BotTier.T4_RR80,
                >= 70 => BotLoadoutManager.BotTier.T4_RR70,
                _ => BotLoadoutManager.BotTier.T4_RR40
            };
        }

        private byte GetMaxLevel(int tier)
        {
            return tier switch { 1 => 11, 2 => 21, 3 => 31, 4 => 40, _ => 40 };
        }

        private bool TryGetBotTemplate(byte careerLine, out CharacterInfo template, out CharacterIdentityRecord identity)
        {
            template = CharMgr.GetCharacterInfo(careerLine);
            if (template == null)
                template = CharMgr.CharacterInfos.Values.FirstOrDefault(info => info.CareerLine == careerLine);

            if (template == null)
            {
                Log.Error("BotManager", $"No CharacterInfo found for career line {careerLine}");
                identity = default;
                return false;
            }

            if (!CharacterIdentityCatalog.TryGetByCareerLine(careerLine, out identity))
            {
                Log.Error("BotManager", $"No identity mapping found for career line {careerLine}");
                return false;
            }

            return true;
        }

        private bool RepairBotCharacter(Character character, CharacterInfo template, CharacterIdentityRecord identity)
        {
            bool dirty = false;

            if (character.AccountId != _botAccount.AccountId)
            {
                character.AccountId = _botAccount.AccountId;
                dirty = true;
            }

            if (character.RealmId != (Program.Rm?.RealmId ?? 1))
            {
                character.RealmId = Program.Rm?.RealmId ?? 1;
                dirty = true;
            }

            if (character.Career != template.Career)
            {
                character.Career = template.Career;
                dirty = true;
            }

            if (character.CareerLine != identity.CareerLine)
            {
                character.CareerLine = identity.CareerLine;
                dirty = true;
            }

            if (character.Realm != identity.Realm)
            {
                character.Realm = identity.Realm;
                dirty = true;
            }

            if (character.Race != identity.Race)
            {
                character.Race = identity.Race;
                dirty = true;
            }

            if (character.bTraits == null || character.bTraits.Length < 8)
            {
                character.bTraits = new byte[8];
                dirty = true;
            }

            if (character.Surname == null)
            {
                character.Surname = string.Empty;
                dirty = true;
            }

            if (character.OldName == null)
            {
                character.OldName = string.Empty;
                dirty = true;
            }

            if (character.PetName == null)
            {
                character.PetName = string.Empty;
                dirty = true;
            }

            if (character.Sex > 1)
            {
                character.Sex = 0;
                dirty = true;
            }

            if (character.Anonymous)
            {
                character.Anonymous = false;
                dirty = true;
            }

            if (character.Hidden)
            {
                character.Hidden = false;
                dirty = true;
            }

            character.FirstConnect = false;
            return dirty;
        }

        private static Character_value CreateBotCharacterValue(Character character, CharacterInfo template, byte level, byte renownRank, Realms realm)
        {
            Character_value value = new Character_value
            {
                CharacterId = character.CharacterId,
                Level = level,
                Money = 0,
                Online = false,
                RallyPoint = template.RallyPt,
                RegionId = ZoneService.GetZone_Info(template.ZoneId)?.Region ?? template.Region,
                Renown = 0,
                RenownRank = renownRank,
                RestXp = 0,
                Skills = template.Skills,
                Speed = 100,
                PlayedTime = 0,
                WorldO = template.WorldO,
                WorldX = template.WorldX,
                WorldY = template.WorldY,
                WorldZ = template.WorldZ,
                Xp = 0,
                ZoneId = template.ZoneId,
                LeftSystemGuild = true
            };

            if (WorldMgr.NormalizeCharacterWorldPosition(value, (byte)realm, character.Name, out string spawnReason))
                Log.Info("BotManager", $"Normalized bot position for {character.Name}: {spawnReason}");

            return value;
        }

        private static bool RepairBotCharacterValue(Character character, CharacterInfo template, byte level, byte renownRank, Realms realm)
        {
            Character_value value = character.Value;
            bool dirty = false;

            if (value.CharacterId != character.CharacterId)
            {
                value.CharacterId = character.CharacterId;
                dirty = true;
            }

            if (value.Level != level)
            {
                value.Level = level;
                dirty = true;
            }

            if (value.RenownRank != renownRank)
            {
                value.RenownRank = renownRank;
                dirty = true;
            }

            if (value.Speed <= 0)
            {
                value.Speed = 100;
                dirty = true;
            }

            if (value.RallyPoint == 0)
            {
                value.RallyPoint = template.RallyPt;
                dirty = true;
            }

            if (value.RegionId <= 0)
            {
                value.RegionId = ZoneService.GetZone_Info(template.ZoneId)?.Region ?? template.Region;
                dirty = true;
            }

            if (value.ZoneId == 0)
            {
                value.ZoneId = template.ZoneId;
                dirty = true;
            }

            if (value.WorldX == 0 && value.WorldY == 0)
            {
                value.WorldX = template.WorldX;
                value.WorldY = template.WorldY;
                value.WorldZ = template.WorldZ;
                value.WorldO = template.WorldO;
                dirty = true;
            }

            if (value.Skills == 0)
            {
                value.Skills = template.Skills;
                dirty = true;
            }

            if (!value.LeftSystemGuild)
            {
                value.LeftSystemGuild = true;
                dirty = true;
            }

            if (WorldMgr.NormalizeCharacterWorldPosition(value, (byte)realm, character.Name, out string spawnReason))
            {
                dirty = true;
                Log.Info("BotManager", $"Repaired bot position for {character.Name}: {spawnReason}");
            }

            return dirty;
        }

        private static bool EnsureBotCollections(Character character)
        {
            bool dirty = false;

            if (character.Mails == null)
            {
                character.Mails = new List<Character_mail>();
                dirty = true;
            }

            if (character.Quests == null)
            {
                character.Quests = new List<Character_quest>();
                dirty = true;
            }

            if (character.Toks == null)
            {
                character.Toks = new List<Character_tok>();
                dirty = true;
            }

            if (character.TokKills == null)
            {
                character.TokKills = new List<Character_tok_kills>();
                dirty = true;
            }

            if (character.Socials == null)
            {
                character.Socials = new List<Character_social>();
                dirty = true;
            }

            if (character.Influences == null)
            {
                character.Influences = new List<Characters_influence>();
                dirty = true;
            }

            if (character.Bag_Pools == null)
            {
                character.Bag_Pools = new List<Characters_bag_pools>();
                dirty = true;
            }

            if (character.Buffs == null)
            {
                character.Buffs = new List<CharacterSavedBuff>();
                dirty = true;
            }

            return dirty;
        }

        private static void StageBotSpawn(Player bot, Zone_Info zoneInfo, Point3D worldSpawn)
        {
            if (bot?.Info?.Value == null || zoneInfo == null || worldSpawn == null)
                return;

            bot.Info.Value.ZoneId = zoneInfo.ZoneId;
            bot.Info.Value.RegionId = zoneInfo.Region;
            bot.Info.Value.WorldX = worldSpawn.X;
            bot.Info.Value.WorldY = worldSpawn.Y;
            bot.Info.Value.WorldZ = worldSpawn.Z;
            bot.Info.Value.WorldO = 0;
            bot.Info.Value.LeftSystemGuild = true;
        }

        private static void RemoveBotFromOtherGuilds(Player bot, World.Guild.Guild targetGuild)
        {
            if (bot == null)
                return;

            foreach (World.Guild.Guild guild in World.Guild.Guild.Guilds.ToList())
            {
                if (guild == null || guild == targetGuild)
                    continue;

                if (!guild.Info.Members.TryGetValue(bot.CharacterId, out Guild_member member))
                    continue;

                guild.Info.Members.Remove(bot.CharacterId);
                CharMgr.Database.DeleteObject(member);

                if (guild.IsSystemGuild() && bot.Info?.Value != null)
                    bot.Info.Value.LeftSystemGuild = true;
            }
        }
    }
}
