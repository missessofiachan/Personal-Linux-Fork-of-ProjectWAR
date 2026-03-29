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
            // Initialized during server startup after ItemService
        }

        public static void SetupLoadouts()
        {
             string[] setNames = { "Decimator", "Obliterator", "Devastator", "Conqueror", "Warlord", "Sovereign", "Doomflayer", "Warpforged" };
             BotTier[] tiers = { BotTier.T1, BotTier.T2, BotTier.T3, BotTier.T4_RR40, BotTier.T4_RR70, BotTier.T4_RR80, BotTier.T4_RR90, BotTier.T4_RR100 };

             for(int i = 0; i < setNames.Length; i++)
             {
                 string setName = setNames[i];
                 BotTier tier = tiers[i];

                 var items = ItemService._Item_Info.Values.Where(x => x.Name != null && x.Name.Contains(setName)).ToList();
                 
                 for (byte career = 1; career <= 24; career++)
                 {
                     long careerMask = 1L << (career - 1);
                     var careerItems = items.Where(x => x.Career == 0 || (x.Career & careerMask) > 0).ToList();
                     
                     Loadout loadout = new Loadout();
                     
                     foreach (var itm in careerItems)
                     {
                         // Pick the best item per slot for this set
                         if (!loadout.SlotItems.ContainsKey(itm.SlotId))
                             loadout.SlotItems[itm.SlotId] = itm.Entry;
                     }
                     
                     // Find weapons appropriate for tier and career
                     byte minRank = GetMinRankForTier(tier);
                     byte maxRank = GetMaxRankForTier(tier);

                     if (!loadout.SlotItems.ContainsKey(10) && !loadout.SlotItems.ContainsKey(11))
                     {
                         var weapons = ItemService._Item_Info.Values
                             .Where(x => x.SlotId == 10 || x.SlotId == 11 || x.SlotId == 12)
                             .Where(x => x.MinRank >= minRank && x.MinRank <= maxRank)
                             .Where(x => x.Career == 0 || (x.Career & careerMask) > 0)
                             .OrderByDescending(x => x.MinRank)
                             .ThenByDescending(x => x.MinRenown)
                             .ToList();

                         var mainHand = weapons.FirstOrDefault(x => x.SlotId == 10);
                         if (mainHand != null)
                             loadout.SlotItems[10] = mainHand.Entry;

                         var offHand = weapons.FirstOrDefault(x => x.SlotId == 11);
                         if (offHand != null)
                             loadout.SlotItems[11] = offHand.Entry;
                         
                         var ranged = weapons.FirstOrDefault(x => x.SlotId == 12);
                         if (ranged != null)
                             loadout.SlotItems[12] = ranged.Entry;
                     }

                     // Add Accessories (Gems, Cloak, Jewellery)
                     var accessories = ItemService._Item_Info.Values
                         .Where(x => (x.SlotId >= 25 && x.SlotId <= 27) || (x.SlotId >= 31 && x.SlotId <= 34))
                         .Where(x => x.MinRank >= minRank && x.MinRank <= maxRank)
                         .Where(x => x.Career == 0 || (x.Career & careerMask) > 0)
                         .OrderByDescending(x => x.MinRank)
                         .ThenByDescending(x => x.MinRenown)
                         .ToList();

                     var cloaks = accessories.Where(x => x.SlotId == 27).ToList();
                     if (cloaks.Any() && !loadout.SlotItems.ContainsKey(27))
                         loadout.SlotItems[27] = cloaks[0].Entry;

                     var gems = accessories.Where(x => x.SlotId == 25 || x.SlotId == 26).ToList();
                     if (gems.Count > 0 && !loadout.SlotItems.ContainsKey(25))
                         loadout.SlotItems[25] = gems[0].Entry;
                     if (gems.Count > 1 && !loadout.SlotItems.ContainsKey(26))
                         loadout.SlotItems[26] = gems[1].Entry;
                     else if (gems.Count > 0 && !loadout.SlotItems.ContainsKey(26))
                         loadout.SlotItems[26] = gems[0].Entry;

                     var jewellery = accessories.Where(x => x.SlotId >= 31 && x.SlotId <= 34).ToList();
                     ushort[] jSlots = { 31, 32, 33, 34 };
                     int jIndex = 0;
                     foreach (var jSlot in jSlots)
                     {
                         if (!loadout.SlotItems.ContainsKey(jSlot))
                         {
                             if (jIndex < jewellery.Count)
                             {
                                 loadout.SlotItems[jSlot] = jewellery[jIndex].Entry;
                                 jIndex++;
                             }
                             else if (jewellery.Count > 0)
                             {
                                 loadout.SlotItems[jSlot] = jewellery[0].Entry;
                             }
                         }
                     }

                     AddLoadout(tier, career, loadout);
                 }
             }
        }

        private static byte GetMinRankForTier(BotTier tier)
        {
            return tier switch { BotTier.T1 => 1, BotTier.T2 => 12, BotTier.T3 => 22, _ => 32 };
        }

        private static byte GetMaxRankForTier(BotTier tier)
        {
            return tier switch { BotTier.T1 => 11, BotTier.T2 => 21, BotTier.T3 => 31, _ => 40 };
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
