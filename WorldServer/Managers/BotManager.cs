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

            Character character = CharMgr.GetCharacter(name);
            if (character == null)
            {
                byte race = 0;
                foreach (var record in CharacterIdentityCatalog.CareerLineRecords)
                {
                    if (record.CareerLine == career)
                    {
                        race = record.Race;
                        break;
                    }
                }

                character = new Character
                {
                    Name = name,
                    AccountId = _botAccount.AccountId,
                    CareerLine = career,
                    Realm = (byte)realm,
                    Race = race,
                    ModelId = 0 
                };

                if (!CharMgr.CreateChar(character))
                {
                    Log.Error("BotManager", $"Failed to create bot character {name}");
                    return null;
                }

                // Create Character_value for the bot
                character.Value = new Character_value
                {
                    CharacterId = character.CharacterId,
                    Level = level,
                    RenownRank = renownRank,
                    Online = false,
                    Speed = 100
                };
                CharMgr.Database.AddObject(character.Value);
            }
            else if (character.Value == null)
            {
                // Ensure Value is loaded if character exists
                character.Value = CharMgr.Database.SelectObject<Character_value>("CharacterId=" + character.CharacterId);
            }

            BotClient client = new BotClient(Program.Server);
            client._Account = _botAccount;

            Player player = Player.CreatePlayer(client, character);
            player.IsBot = true;
            player.Role = role;

            return player;
        }

        public Group SpawnBotGroup(Realms realm, int tier, int rr, string groupPrefix, ushort zoneId)
        {
            Point3D spawnPos = BattleFrontService.GetWarcampEntrance(zoneId, realm);
            if (spawnPos == null)
            {
                Log.Error("BotManager", $"No warcamp entrance found for zone {zoneId}, realm {realm}");
                return null;
            }

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
                if (bot != null)
                {
                    bot.SetPosition((ushort)spawnPos.X, (ushort)spawnPos.Y, (ushort)spawnPos.Z, 0, zoneId);
                    ApplyLoadout(bot, tier, rr);
                    WorldMgr.GetRegion(zoneId, true).AddObject(bot, zoneId);
                }
            }

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
                    Level = 40,
                    Realm = (byte)firstBot.Realm,
                    Xp = 0,
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

            foreach (var bot in bots)
            {
                if (bot == null) continue;
                if (bot.GldInterface.Guild != null) continue; // Already in a guild

                Guild_member member = new Guild_member
                {
                    CharacterId = bot.CharacterId,
                    GuildId = guild.Info.GuildId,
                    RankId = (bot == firstBot && isNewGuild) ? (byte)9 : (byte)1,
                    PublicNote = "",
                    OfficerNote = "",
                    Member = bot.Info,
                    JoinDate = (uint)TCPManager.GetTimeStamp()
                };
                
                // Add member to dictionary to prevent duplicate key errors if guild was just loaded/created
                if (!guild.Info.Members.ContainsKey(bot.CharacterId))
                {
                    guild.Info.Members.Add(bot.CharacterId, member);
                    CharMgr.Database.AddObject(member);
                }

                bot.GldInterface.Guild = guild;
                guild.AddOnlineMember(bot);
                
                guild.AddGuildLog(0, bot.Name);
                if (bot == firstBot && isNewGuild)
                    guild.AddGuildLog(11, bot.Name);
            }
            
            CharMgr.Database.ForceSave();
        }

        private void ApplyLoadout(Player bot, int tier, int rr)
        {
            BotLoadoutManager.BotTier bTier = GetBotTier(tier, rr);
            var loadout = BotLoadoutManager.GetLoadout(bTier, (byte)bot.Info.CareerLine);
            if (loadout == null) return;

            foreach (var itemEntry in loadout.SlotItems)
            {
                bot.ItmInterface.CreateItem(itemEntry.Value, 1, false, (ushort)itemEntry.Key);
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
    }
}
