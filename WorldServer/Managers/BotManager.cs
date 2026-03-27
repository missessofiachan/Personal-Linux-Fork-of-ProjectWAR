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
    public class BotManager
    {
        private static BotManager _instance;
        public static BotManager Instance => _instance ??= new BotManager();

        private Account _botAccount;
        private const string BOT_ACCOUNT_NAME = ""BotAccount"";
        private const int BOT_ACCOUNT_ID = 9999;

        public void Initialize()
        {
            _botAccount = Program.AcctMgr.LoadAccount(BOT_ACCOUNT_NAME);
            if (_botAccount == null)
            {
                _botAccount = new Account
                {
                    Username = BOT_ACCOUNT_NAME,
                    Password = ""botpassword"",
                    GmLevel = 1,
                    AccountId = BOT_ACCOUNT_ID
                };
                Program.AcctMgr.UpdateAccount(_botAccount);
                Log.Info(""BotManager"", ""Created BotAccount."");
            }
        }

        public Player CreateOrLoadBot(string name, byte career, byte level, byte renownRank, Realms realm, BotRole role)
        {
            Character character = CharMgr.GetCharacter(name);
            if (character == null)
            {
                character = new Character
                {
                    Name = name,
                    AccountId = _botAccount.AccountId,
                    CareerLine = career,
                    Level = level,
                    RenownRank = renownRank,
                    Realm = (byte)realm,
                    ModelId = 0, // Should be set based on race/career
                };

                if (!CharMgr.CreateChar(character))
                {
                    Log.Error(""BotManager"", $""Failed to create bot character {name}"");
                    return null;
                }
            }

            BotClient client = new BotClient(Program.AcctMgr.Server);
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
                Log.Error(""BotManager"", $""No warcamp entrance found for zone {zoneId}, realm {realm}"");
                return null;
            }

            List<Player> bots = new List<Player>();
            byte[] careers = GetCareersForRealm(realm);
            
            byte healerCareer = careers[0];
            byte rangedCareer = careers[1];
            byte tankCareer = careers[2];
            byte meleeCareer = careers[3];

            // 1. Healer
            bots.Add(CreateOrLoadBot($""{groupPrefix}_H"", healerCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.Healer));
            // 2. Ranged DPS
            bots.Add(CreateOrLoadBot($""{groupPrefix}_R"", rangedCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.RangedDPS));
            // 3. Main Tank (Shield)
            bots.Add(CreateOrLoadBot($""{groupPrefix}_MT"", tankCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.MainTank_Shield));
            // 4. Off Tank (2H)
            bots.Add(CreateOrLoadBot($""{groupPrefix}_OT"", tankCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.OffTank_2H));
            // 5. Melee 1
            bots.Add(CreateOrLoadBot($""{groupPrefix}_M1"", meleeCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.MeleeDPS));
            // 6. Melee 2
            bots.Add(CreateOrLoadBot($""{groupPrefix}_M2"", meleeCareer, GetMaxLevel(tier), (byte)rr, realm, BotRole.MeleeDPS));

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

            foreach (var bot in bots)
            {
                if (bot != null)
                {
                    bot.SetPosition((ushort)spawnPos.X, (ushort)spawnPos.Y, (ushort)spawnPos.Z, (ushort)spawnPos.O, zoneId);
                    ApplyLoadout(bot, tier, rr);
                    WorldMgr.GetRegion(zoneId, true).AddObject(bot, true);
                }
            }

            return group;
        }

        private void ApplyLoadout(Player bot, int tier, int rr)
        {
            BotLoadoutManager.BotTier bTier = GetBotTier(tier, rr);
            var loadout = BotLoadoutManager.GetLoadout(bTier, (byte)bot.Info.CareerLine);
            if (loadout == null) return;

            foreach (var itemEntry in loadout.Items)
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

        private byte[] GetCareersForRealm(Realms realm)
        {
            if (realm == Realms.REALMS_REALM_ORDER)
                return new byte[] { 12, 11, 10, 9 }; // WP, BW, Knight, WH
            else
                return new byte[] { 15, 16, 13, 14 }; // Zealot, Magus, Chosen, Marauder
        }

        private byte GetMaxLevel(int tier)
        {
            return tier switch { 1 => 11, 2 => 21, 3 => 31, 4 => 40, _ => 40 };
        }
    }
}
