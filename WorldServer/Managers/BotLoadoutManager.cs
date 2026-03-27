using Common;
using GameData;
using System.Collections.Generic;
using WorldServer.Services.World;
using System.Linq;

namespace WorldServer.Managers
{
    public class BotLoadoutManager
    {
        public enum BotTier
        {
            T1, T2, T3, T4_RR40, T4_RR70, T4_RR80, T4_RR90, T4_RR100
        }

        public class Loadout
        {
            public Dictionary<ushort, uint> SlotItems = new Dictionary<ushort, uint>();
        }

        private static Dictionary<BotTier, Dictionary<byte, Loadout>> _loadouts = new Dictionary<BotTier, Dictionary<byte, Loadout>>();

        public static void Initialize()
        {
            // T4 RR40 Annihilator (Example search-based initialization)
            // In a real scenario, we would query the DB for "Annihilator" set items per career.
            // For now, we provide the structure to map sets to BotTiers.
        }

        public static void SetupLoadouts()
        {
             // This is where we would use the DB to find items like 'Annihilator', 'Warlord', 'Sovereign', etc.
             // Since I am an AI, I will provide the logic to find them by name in the ItemService cache.
             
             string[] setNames = { ""Annihilator"", ""Warlord"", ""Sovereign"", ""Doomflayer"", ""Warpforged"" };
             BotTier[] tiers = { BotTier.T4_RR40, BotTier.T4_RR70, BotTier.T4_RR80, BotTier.T4_RR90, BotTier.T4_RR100 };

             for(int i=0; i < setNames.Length; i++)
             {
                 string setName = setNames[i];
                 BotTier tier = tiers[i];

                 var items = ItemService._Item_Info.Values.Where(x => x.Name.Contains(setName)).ToList();
                 foreach(var itm in items)
                 {
                     // Extract Career from bitmask and Slot
                     // itm.Career bitmask mapping to CareerLine
                     // itm.SlotId mapping to EquipSlots
                     
                     // Logic to populate _loadouts[tier][career]
                 }
             }
        }

        private static void AddLoadout(BotTier tier, byte career, Loadout loadout)
        {
            if (!_loadouts.ContainsKey(tier))
                _loadouts[tier] = new Dictionary<byte, Loadout>();
            _loadouts[tier][career] = loadout;
        }

        public static Loadout GetLoadout(BotTier tier, byte career)
        {
            if (_loadouts.TryGetValue(tier, out var careerLoadouts))
            {
                if (careerLoadouts.TryGetValue(career, out var loadout))
                    return loadout;
            }
            return null;
        }
    }
}
