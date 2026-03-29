using Common;
using GameData;
using System;
using System.Collections.Generic;
using WorldServer.Services.World;
using System.Linq;
using WorldServer.World.Objects;

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

        private static Dictionary<BotTier, Dictionary<byte, Dictionary<BotRole, Loadout>>> _loadouts = new Dictionary<BotTier, Dictionary<byte, Dictionary<BotRole, Loadout>>>();

        public static void Initialize()
        {
            SetupLoadouts();
        }

        public enum GearType
        {
            Defensive,
            Offensive,
            Healer
        }

        private static float GetItemScore(Item_Info item, GearType type)
        {
            float score = 0;
            var stats = item.GetStats();

            ushort GetStat(Stats stat)
            {
                return stats.TryGetValue((byte)stat, out ushort val) ? val : (ushort)0;
            }

            switch (type)
            {
                case GearType.Defensive:
                    score += GetStat(Stats.Toughness) * 1.0f;
                    score += GetStat(Stats.Wounds) * 1.0f;
                    score += GetStat(Stats.Armor) * 0.1f;
                    score += GetStat(Stats.Initiative) * 0.5f;
                    score += GetStat(Stats.Block) * 10.0f;
                    score += GetStat(Stats.Parry) * 5.0f;
                    score += GetStat(Stats.Evade) * 5.0f;
                    score += GetStat(Stats.Disrupt) * 5.0f;
                    break;

                case GearType.Offensive:
                    score += GetStat(Stats.Strength) * 1.0f;
                    score += GetStat(Stats.BallisticSkill) * 1.0f;
                    score += GetStat(Stats.Intelligence) * 1.0f;
                    score += GetStat(Stats.WeaponSkill) * 0.5f;
                    score += GetStat(Stats.MeleePower) * 0.1f;
                    score += GetStat(Stats.RangedPower) * 0.1f;
                    score += GetStat(Stats.MagicPower) * 0.1f;
                    score += GetStat(Stats.MeleeCritRate) * 5.0f;
                    score += GetStat(Stats.RangedCritRate) * 5.0f;
                    score += GetStat(Stats.MagicCritRate) * 5.0f;
                    break;

                case GearType.Healer:
                    score += GetStat(Stats.Willpower) * 1.0f;
                    score += GetStat(Stats.HealingPower) * 0.1f;
                    score += GetStat(Stats.HealCritRate) * 5.0f;
                    score += GetStat(Stats.Wounds) * 0.5f;
                    break;
            }

            return score;
        }

        public static void SetupLoadouts()
        {
            string[] setNames = { "Decimator", "Obliterator", "Devastator", "Conqueror", "Warlord", "Sovereign", "Doomflayer", "Warpforged" };
            BotTier[] tiers = { BotTier.T1, BotTier.T2, BotTier.T3, BotTier.T4_RR40, BotTier.T4_RR70, BotTier.T4_RR80, BotTier.T4_RR90, BotTier.T4_RR100 };

            for (int i = 0; i < setNames.Length; i++)
            {
                string setName = setNames[i];
                BotTier tier = tiers[i];

                var items = ItemService._Item_Info.Values
                    .Where(x => x.Name != null
                             && x.Name.Contains(setName)
                             && x.Rarity >= 2  // exclude grey/white junk and debug items
                             && x.Name.IndexOf("debug", StringComparison.OrdinalIgnoreCase) < 0
                             && x.Name.IndexOf("test",  StringComparison.OrdinalIgnoreCase) < 0
                             && x.Name.IndexOf("_gm_",  StringComparison.OrdinalIgnoreCase) < 0)
                    .ToList();

                for (byte career = 1; career <= 24; career++)
                {
                    long careerMask = 1L << (career - 1);
                    // Careers 1-12 are Order (Realm 1), 13-24 are Destruction (Realm 2).
                    byte realmByte = career <= 12 ? (byte)1 : (byte)2;
                    var careerItems = items.Where(x => (x.Career == 0 || (x.Career & careerMask) > 0)
                                                    && (x.Realm == 0 || x.Realm == realmByte)).ToList();

                    foreach (BotRole role in System.Enum.GetValues(typeof(BotRole)))
                    {
                        GearType gearType = role switch
                        {
                            BotRole.MainTank_Shield => GearType.Defensive,
                            BotRole.OffTank_2H => GearType.Offensive,
                            BotRole.Healer => GearType.Healer,
                            BotRole.MeleeDPS => GearType.Offensive,
                            BotRole.RangedDPS => GearType.Defensive, // Requested by user: Ranged bots to use DEF gear
                            _ => GearType.Offensive
                        };

                        Loadout loadout = new Loadout();
                        byte minRank = GetMinRankForTier(tier);
                        byte maxRank = GetMaxRankForTier(tier);
                        byte maxRenown = GetMaxRenownForTier(tier);

                        // Gear sets
                        var groupedBySlot = careerItems
                            .Where(x => x.MinRank >= minRank && x.MinRank <= maxRank
                                     && (maxRenown == 0 || x.MinRenown <= maxRenown))
                            .GroupBy(x => x.SlotId);
                        foreach (var group in groupedBySlot)
                        {
                            var bestItem = group.OrderByDescending(x => GetItemScore(x, gearType)).FirstOrDefault();
                            if (bestItem != null)
                                loadout.SlotItems[group.Key] = bestItem.Entry;
                        }

                        // Weapons

                        var weapons = ItemService._Item_Info.Values
                            .Where(x => x.SlotId == 10 || x.SlotId == 11 || x.SlotId == 12)
                            .Where(x => x.MinRank >= minRank && x.MinRank <= maxRank)
                            .Where(x => maxRenown == 0 || x.MinRenown <= maxRenown)
                            .Where(x => x.Career == 0 || (x.Career & careerMask) > 0)
                            .Where(x => x.Realm == 0 || x.Realm == realmByte)
                            .Where(x => x.Rarity >= 2)
                            .Where(x => x.Name == null
                                     || (x.Name.IndexOf("debug", StringComparison.OrdinalIgnoreCase) < 0
                                      && x.Name.IndexOf("test",  StringComparison.OrdinalIgnoreCase) < 0
                                      && x.Name.IndexOf("_gm_",  StringComparison.OrdinalIgnoreCase) < 0))
                            .OrderByDescending(x => GetItemScore(x, gearType))
                            .ThenByDescending(x => x.MinRank)
                            .ThenByDescending(x => x.Rarity)
                            .ToList();

                        if (role == BotRole.MainTank_Shield)
                        {
                            var mainHand = weapons.FirstOrDefault(x => x.SlotId == 10 && !x.TwoHanded);
                            if (mainHand != null) loadout.SlotItems[10] = mainHand.Entry;

                            var shield = weapons.FirstOrDefault(x => x.SlotId == 11 && x.Type == 4); // Type 4 is usually Shield
                            if (shield != null) loadout.SlotItems[11] = shield.Entry;
                        }
                        else if (role == BotRole.OffTank_2H)
                        {
                            var twoHanded = weapons.FirstOrDefault(x => x.SlotId == 10 && x.TwoHanded);
                            if (twoHanded != null) loadout.SlotItems[10] = twoHanded.Entry;
                            else
                            {
                                // Fallback to 1H + Offhand if no 2H found
                                var mainHand = weapons.FirstOrDefault(x => x.SlotId == 10);
                                if (mainHand != null) loadout.SlotItems[10] = mainHand.Entry;
                                var offHand = weapons.FirstOrDefault(x => x.SlotId == 11);
                                if (offHand != null) loadout.SlotItems[11] = offHand.Entry;
                            }
                        }
                        else
                        {
                            var mainHand = weapons.FirstOrDefault(x => x.SlotId == 10);
                            if (mainHand != null) loadout.SlotItems[10] = mainHand.Entry;

                            var offHand = weapons.FirstOrDefault(x => x.SlotId == 11);
                            if (offHand != null) loadout.SlotItems[11] = offHand.Entry;

                            var ranged = weapons.FirstOrDefault(x => x.SlotId == 12);
                            if (ranged != null) loadout.SlotItems[12] = ranged.Entry;
                        }

                        // Add Accessories (Gems, Cloak, Jewellery)
                        var accessories = ItemService._Item_Info.Values
                            .Where(x => (x.SlotId >= 25 && x.SlotId <= 27) || (x.SlotId >= 31 && x.SlotId <= 34))
                            .Where(x => x.MinRank >= minRank && x.MinRank <= maxRank)
                            .Where(x => maxRenown == 0 || x.MinRenown <= maxRenown)
                            .Where(x => x.Career == 0 || (x.Career & careerMask) > 0)
                            .Where(x => x.Realm == 0 || x.Realm == realmByte)
                            .Where(x => x.Rarity >= 2)
                            .Where(x => x.Name == null
                                     || (x.Name.IndexOf("debug", StringComparison.OrdinalIgnoreCase) < 0
                                      && x.Name.IndexOf("test",  StringComparison.OrdinalIgnoreCase) < 0
                                      && x.Name.IndexOf("_gm_",  StringComparison.OrdinalIgnoreCase) < 0))
                            .OrderByDescending(x => GetItemScore(x, gearType))
                            .ThenByDescending(x => x.MinRank)
                            .ThenByDescending(x => x.Rarity)
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

                        AddLoadout(tier, career, role, loadout);
                    }
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

        /// <summary>
        /// Returns the maximum MinRenown an item may require for bots of this tier.
        /// Returns 0 for tiers where renown is not yet relevant (T1-T3), meaning no cap.
        /// </summary>
        private static byte GetMaxRenownForTier(BotTier tier)
        {
            return tier switch
            {
                BotTier.T4_RR40  => 40,
                BotTier.T4_RR70  => 70,
                BotTier.T4_RR80  => 80,
                BotTier.T4_RR90  => 90,
                BotTier.T4_RR100 => 100,
                _                => 0   // T1/T2/T3: no renown restriction
            };
        }

        private static void AddLoadout(BotTier tier, byte career, BotRole role, Loadout loadout)
        {
            if (!_loadouts.ContainsKey(tier))
                _loadouts[tier] = new Dictionary<byte, Dictionary<BotRole, Loadout>>();
            if (!_loadouts[tier].ContainsKey(career))
                _loadouts[tier][career] = new Dictionary<BotRole, Loadout>();

            _loadouts[tier][career][role] = loadout;
        }

        public static Loadout GetLoadout(BotTier tier, byte career, BotRole role)
        {
            if (_loadouts.TryGetValue(tier, out var careerLoadouts))
            {
                if (careerLoadouts.TryGetValue(career, out var roleLoadouts))
                {
                    if (roleLoadouts.TryGetValue(role, out var loadout))
                        return loadout;
                }
            }
            return null;
        }
    }
}
