using Common;
using FrameWork;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldServer.Services.World;
using WorldServer.World.Interfaces;
using WorldServer.World.Objects;

namespace WorldServer.Managers
{
    public class BotLoadoutManager
    {
        public enum BotTier
        {
            T1,
            T2,
            T3,
            T4_RR40,
            T4_RR70,
            T4_RR80,
            T4_RR90,
            T4_RR100
        }

        public class Loadout
        {
            public Dictionary<ushort, uint> SlotItems { get; } = new Dictionary<ushort, uint>();

            public Loadout Clone()
            {
                Loadout clone = new Loadout();
                foreach (KeyValuePair<ushort, uint> entry in SlotItems)
                    clone.SlotItems[entry.Key] = entry.Value;

                return clone;
            }
        }

        private static readonly BotTier[] SharedT4Tiers =
        {
            BotTier.T4_RR40,
            BotTier.T4_RR70,
            BotTier.T4_RR80,
            BotTier.T4_RR90,
            BotTier.T4_RR100
        };

        private static readonly ushort[] _managedEquipmentSlots =
        {
            (ushort)EquipSlot.MAIN_HAND,
            (ushort)EquipSlot.OFF_HAND,
            (ushort)EquipSlot.RANGED_WEAPON,
            (ushort)EquipSlot.BODY,
            (ushort)EquipSlot.GLOVES,
            (ushort)EquipSlot.BOOTS,
            (ushort)EquipSlot.HELM,
            (ushort)EquipSlot.SHOULDER,
            (ushort)EquipSlot.POCKET_1,
            (ushort)EquipSlot.POCKET_2,
            (ushort)EquipSlot.BACK,
            (ushort)EquipSlot.BELT,
            (ushort)EquipSlot.JEWELLERY_1,
            (ushort)EquipSlot.JEWELLERY_2,
            (ushort)EquipSlot.JEWELLERY_3,
            (ushort)EquipSlot.JEWELLERY_4
        };

        private static readonly Dictionary<TemplateKey, Loadout> _templates = new Dictionary<TemplateKey, Loadout>();

        public static IReadOnlyList<ushort> ManagedEquipmentSlots => _managedEquipmentSlots;

        public static void Initialize()
        {
            _templates.Clear();
            RegisterSharedT4Templates();
        }

        public static Loadout GetLoadout(Player bot, BotTier tier, BotRole role)
        {
            if (bot?.Info == null)
                return null;

            Character_value value = bot.Info.Value ?? bot._Value;
            if (value == null)
                return GetBaseLoadout(bot.Info.CareerLine, tier, role);

            return GetLoadout(bot.CharacterId, bot.Info.CareerLine, bot.Info.Race, value.Skills, value.Level, value.RenownRank, tier, role);
        }

        public static Loadout GetLoadout(uint characterId, byte careerLine, byte race, long playerSkills, byte level, byte renownRank, BotTier tier, BotRole role)
        {
            Loadout baseLoadout = GetBaseLoadout(careerLine, tier, role);
            if (baseLoadout == null)
            {
                Log.Error("BotLoadoutManager", $"No explicit template or starter fallback found for CharacterId {characterId} ({careerLine}/{role}/{tier}).");
                return null;
            }

            Loadout effectiveLoadout = baseLoadout.Clone();
            ApplyOverrides(effectiveLoadout, characterId, careerLine, race, playerSkills, level, renownRank);
            return effectiveLoadout;
        }

        public static Loadout GetBaseLoadout(byte careerLine, BotTier tier, BotRole role)
        {
            TemplateKey key = new TemplateKey(tier, careerLine, role);
            if (_templates.TryGetValue(key, out Loadout template))
                return template;

            return BuildStartingLoadout(careerLine);
        }

        public static bool IsManagedEquipmentSlot(ushort slotId)
        {
            return _managedEquipmentSlots.Contains(slotId);
        }

        public static bool IsManagedItemInfo(Item_Info info)
        {
            return info != null && IsManagedSlotCandidate(info.SlotId);
        }

        private static bool IsManagedSlotCandidate(ushort slotId)
        {
            if (slotId == (ushort)EquipSlot.EITHER_HAND)
                return true;

            return _managedEquipmentSlots.Contains(slotId);
        }

        public static bool CanOccupySlot(Item_Info item, ushort slotId)
        {
            if (item == null)
                return false;

            EquipSlot itemSlot = (EquipSlot)item.SlotId;
            EquipSlot equipSlot = (EquipSlot)slotId;

            if (slotId == item.SlotId)
                return true;

            if (itemSlot == EquipSlot.EITHER_HAND && (equipSlot == EquipSlot.MAIN_HAND || equipSlot == EquipSlot.OFF_HAND))
                return true;

            if ((itemSlot == EquipSlot.POCKET_1 || itemSlot == EquipSlot.POCKET_2) && (equipSlot == EquipSlot.POCKET_1 || equipSlot == EquipSlot.POCKET_2))
                return true;

            if (itemSlot >= EquipSlot.JEWELLERY_1 && itemSlot <= EquipSlot.JEWELLERY_4
                && equipSlot >= EquipSlot.JEWELLERY_1 && equipSlot <= EquipSlot.JEWELLERY_4)
                return true;

            return false;
        }

        public static bool CanUseItemInLoadout(byte careerLine, byte race, long playerSkills, byte level, byte renownRank, ushort slotId, Item_Info item, ISet<uint> uniqueEquipped = null)
        {
            if (item == null || !IsManagedItemInfo(item))
                return false;

            if (!CanOccupySlot(item, slotId))
                return false;

            if (!ItemsInterface.CanUseForCharacter(item, careerLine, race, playerSkills, level, renownRank))
                return false;

            return ItemsInterface.CanUseItemTypeForCareer(item, careerLine, race, playerSkills, slotId, uniqueEquipped);
        }

        private static Loadout BuildStartingLoadout(byte careerLine)
        {
            Loadout loadout = new Loadout();

            foreach (CharacterInfo_item templateItem in CharMgr.GetCharacterInfoItem(careerLine))
            {
                Item_Info info = ItemService.GetItem_Info(templateItem.Entry);
                if (!IsManagedItemInfo(info))
                    continue;

                ushort slotId = IsManagedEquipmentSlot(templateItem.SlotId) ? templateItem.SlotId : info.SlotId;
                if (!CanOccupySlot(info, slotId))
                    continue;

                loadout.SlotItems[slotId] = templateItem.Entry;
            }

            return loadout.SlotItems.Count > 0 ? loadout : null;
        }

        private static void ApplyOverrides(Loadout loadout, uint characterId, byte careerLine, byte race, long playerSkills, byte level, byte renownRank)
        {
            Dictionary<ushort, uint> overrides = BotGearOverrideService.GetOverrideEntries(characterId);
            if (overrides.Count == 0)
                return;

            HashSet<uint> uniqueEquipped = BuildUniqueEntrySet(loadout);
            foreach (KeyValuePair<ushort, uint> overrideEntry in overrides.OrderBy(entry => entry.Key))
            {
                ushort slotId = overrideEntry.Key;
                if (!IsManagedEquipmentSlot(slotId))
                {
                    Log.Error("BotLoadoutManager", $"Ignoring bot gear override for CharacterId {characterId}: slot {slotId} is not managed.");
                    continue;
                }

                uint previousEntry = 0;
                Item_Info previousInfo = null;
                if (loadout.SlotItems.TryGetValue(slotId, out previousEntry))
                {
                    previousInfo = ItemService.GetItem_Info(previousEntry);
                    if (previousInfo?.UniqueEquiped == 1)
                        uniqueEquipped.Remove(previousEntry);
                }

                Item_Info itemInfo = ItemService.GetItem_Info(overrideEntry.Value);
                if (!CanUseItemInLoadout(careerLine, race, playerSkills, level, renownRank, slotId, itemInfo, uniqueEquipped))
                {
                    Log.Error("BotLoadoutManager", $"Ignoring invalid bot gear override for CharacterId {characterId}: item {overrideEntry.Value} in slot {slotId}.");
                    if (previousInfo?.UniqueEquiped == 1)
                        uniqueEquipped.Add(previousEntry);
                    continue;
                }

                loadout.SlotItems[slotId] = overrideEntry.Value;
                if (itemInfo.UniqueEquiped == 1)
                    uniqueEquipped.Add(itemInfo.Entry);
            }
        }

        private static HashSet<uint> BuildUniqueEntrySet(Loadout loadout)
        {
            HashSet<uint> uniqueEquipped = new HashSet<uint>();
            foreach (uint itemEntry in loadout.SlotItems.Values)
            {
                Item_Info info = ItemService.GetItem_Info(itemEntry);
                if (info?.UniqueEquiped == 1)
                    uniqueEquipped.Add(itemEntry);
            }

            return uniqueEquipped;
        }

        private static void RegisterSharedT4Templates()
        {
            RegisterSharedT4Template((byte)CareerLine.CAREERLINE_RUNE_PRIEST, BotRole.Healer,
                Entry(EquipSlot.BODY, 129838067),
                Entry(EquipSlot.HELM, 129838068),
                Entry(EquipSlot.SHOULDER, 129838069),
                Entry(EquipSlot.GLOVES, 129838070),
                Entry(EquipSlot.BOOTS, 129838071),
                Entry(EquipSlot.BELT, 5850423),
                Entry(EquipSlot.BACK, 473671),
                Entry(EquipSlot.POCKET_1, 129838806),
                Entry(EquipSlot.POCKET_2, 129838811),
                Entry(EquipSlot.JEWELLERY_1, 129838516),
                Entry(EquipSlot.JEWELLERY_2, 129838623),
                Entry(EquipSlot.JEWELLERY_3, 129838737),
                Entry(EquipSlot.JEWELLERY_4, 5850421),
                Entry(EquipSlot.MAIN_HAND, 2000157));

            RegisterSharedT4Template((byte)CareerLine.CAREERLINE_ENGINEER, BotRole.RangedDPS,
                Entry(EquipSlot.BODY, 475376),
                Entry(EquipSlot.HELM, 472976),
                Entry(EquipSlot.SHOULDER, 477008),
                Entry(EquipSlot.GLOVES, 2017018),
                Entry(EquipSlot.BOOTS, 434219),
                Entry(EquipSlot.BELT, 476936),
                Entry(EquipSlot.BACK, 473648),
                Entry(EquipSlot.POCKET_1, 129838806),
                Entry(EquipSlot.POCKET_2, 129838811),
                Entry(EquipSlot.JEWELLERY_1, 129838623),
                Entry(EquipSlot.JEWELLERY_2, 129838516),
                Entry(EquipSlot.JEWELLERY_3, 129838737),
                Entry(EquipSlot.JEWELLERY_4, 5850417),
                Entry(EquipSlot.MAIN_HAND, 2000112),
                Entry(EquipSlot.RANGED_WEAPON, 2000165));

            RegisterSharedT4Template((byte)CareerLine.CAREERLINE_IRON_BREAKER, BotRole.MainTank_Shield,
                Entry(EquipSlot.BODY, 435680),
                Entry(EquipSlot.HELM, 435704),
                Entry(EquipSlot.SHOULDER, 435692),
                Entry(EquipSlot.GLOVES, 435728),
                Entry(EquipSlot.BOOTS, 435716),
                Entry(EquipSlot.BELT, 435740),
                Entry(EquipSlot.BACK, 473669),
                Entry(EquipSlot.POCKET_1, 129838806),
                Entry(EquipSlot.POCKET_2, 129838811),
                Entry(EquipSlot.JEWELLERY_1, 129838623),
                Entry(EquipSlot.JEWELLERY_2, 129838516),
                Entry(EquipSlot.JEWELLERY_3, 129838737),
                Entry(EquipSlot.JEWELLERY_4, 129838781),
                Entry(EquipSlot.MAIN_HAND, 475085),
                Entry(EquipSlot.OFF_HAND, 472901));

            RegisterSharedT4Template((byte)CareerLine.CAREERLINE_IRON_BREAKER, BotRole.OffTank_2H,
                Entry(EquipSlot.BODY, 435680),
                Entry(EquipSlot.HELM, 472973),
                Entry(EquipSlot.SHOULDER, 435692),
                Entry(EquipSlot.GLOVES, 435728),
                Entry(EquipSlot.BOOTS, 435716),
                Entry(EquipSlot.BELT, 435740),
                Entry(EquipSlot.BACK, 473645),
                Entry(EquipSlot.POCKET_1, 129838806),
                Entry(EquipSlot.POCKET_2, 129838811),
                Entry(EquipSlot.JEWELLERY_1, 129838623),
                Entry(EquipSlot.JEWELLERY_2, 129838516),
                Entry(EquipSlot.JEWELLERY_3, 129838737),
                Entry(EquipSlot.JEWELLERY_4, 472877),
                Entry(EquipSlot.MAIN_HAND, 2000149));

            RegisterSharedT4Template((byte)CareerLine.CAREERLINE_SLAYER, BotRole.MeleeDPS,
                Entry(EquipSlot.BODY, 475374),
                Entry(EquipSlot.HELM, 472974),
                Entry(EquipSlot.SHOULDER, 477006),
                Entry(EquipSlot.GLOVES, 435729),
                Entry(EquipSlot.BOOTS, 434217),
                Entry(EquipSlot.BELT, 435741),
                Entry(EquipSlot.BACK, 473646),
                Entry(EquipSlot.POCKET_1, 129838806),
                Entry(EquipSlot.POCKET_2, 129838811),
                Entry(EquipSlot.JEWELLERY_1, 129838623),
                Entry(EquipSlot.JEWELLERY_2, 129838516),
                Entry(EquipSlot.JEWELLERY_3, 129838737),
                Entry(EquipSlot.JEWELLERY_4, 472878),
                Entry(EquipSlot.MAIN_HAND, 2000149));

            RegisterSharedT4Template((byte)CareerLine.CAREERLINE_SHAMAN, BotRole.Healer,
                Entry(EquipSlot.BODY, 2017032),
                Entry(EquipSlot.HELM, 2017030),
                Entry(EquipSlot.SHOULDER, 2017031),
                Entry(EquipSlot.GLOVES, 2017033),
                Entry(EquipSlot.BOOTS, 2017034),
                Entry(EquipSlot.BELT, 5850423),
                Entry(EquipSlot.BACK, 473719),
                Entry(EquipSlot.POCKET_1, 129838806),
                Entry(EquipSlot.POCKET_2, 129838811),
                Entry(EquipSlot.JEWELLERY_1, 129838516),
                Entry(EquipSlot.JEWELLERY_2, 129838623),
                Entry(EquipSlot.JEWELLERY_3, 129838737),
                Entry(EquipSlot.JEWELLERY_4, 5850421),
                Entry(EquipSlot.MAIN_HAND, 2000157));

            RegisterSharedT4Template((byte)CareerLine.CAREERLINE_SQUIG_HERDER, BotRole.RangedDPS,
                Entry(EquipSlot.BODY, 475424),
                Entry(EquipSlot.HELM, 473024),
                Entry(EquipSlot.SHOULDER, 477056),
                Entry(EquipSlot.GLOVES, 418471),
                Entry(EquipSlot.BOOTS, 434291),
                Entry(EquipSlot.BELT, 476984),
                Entry(EquipSlot.BACK, 473696),
                Entry(EquipSlot.POCKET_1, 129838806),
                Entry(EquipSlot.POCKET_2, 129838811),
                Entry(EquipSlot.JEWELLERY_1, 129838623),
                Entry(EquipSlot.JEWELLERY_2, 129838516),
                Entry(EquipSlot.JEWELLERY_3, 129838737),
                Entry(EquipSlot.JEWELLERY_4, 5850417),
                Entry(EquipSlot.MAIN_HAND, 2000155),
                Entry(EquipSlot.OFF_HAND, 129838247),
                Entry(EquipSlot.RANGED_WEAPON, 2000164));

            RegisterSharedT4Template((byte)CareerLine.CAREERLINE_BLACK_ORC, BotRole.MainTank_Shield,
                Entry(EquipSlot.BODY, 435752),
                Entry(EquipSlot.HELM, 435776),
                Entry(EquipSlot.SHOULDER, 435764),
                Entry(EquipSlot.GLOVES, 435800),
                Entry(EquipSlot.BOOTS, 435788),
                Entry(EquipSlot.BELT, 435812),
                Entry(EquipSlot.BACK, 473717),
                Entry(EquipSlot.POCKET_1, 129838806),
                Entry(EquipSlot.POCKET_2, 129838811),
                Entry(EquipSlot.JEWELLERY_1, 129838623),
                Entry(EquipSlot.JEWELLERY_2, 129838516),
                Entry(EquipSlot.JEWELLERY_3, 129838737),
                Entry(EquipSlot.JEWELLERY_4, 129838781),
                Entry(EquipSlot.MAIN_HAND, 475133),
                Entry(EquipSlot.OFF_HAND, 472949));

            RegisterSharedT4Template((byte)CareerLine.CAREERLINE_BLACK_ORC, BotRole.OffTank_2H,
                Entry(EquipSlot.BODY, 435752),
                Entry(EquipSlot.HELM, 473021),
                Entry(EquipSlot.SHOULDER, 435764),
                Entry(EquipSlot.GLOVES, 435800),
                Entry(EquipSlot.BOOTS, 435788),
                Entry(EquipSlot.BELT, 435812),
                Entry(EquipSlot.BACK, 473693),
                Entry(EquipSlot.POCKET_1, 129838806),
                Entry(EquipSlot.POCKET_2, 129838811),
                Entry(EquipSlot.JEWELLERY_1, 129838623),
                Entry(EquipSlot.JEWELLERY_2, 129838516),
                Entry(EquipSlot.JEWELLERY_3, 129838737),
                Entry(EquipSlot.JEWELLERY_4, 472925),
                Entry(EquipSlot.MAIN_HAND, 2000144));

            RegisterSharedT4Template((byte)CareerLine.CAREERLINE_CHOPPA, BotRole.MeleeDPS,
                Entry(EquipSlot.BODY, 2017027),
                Entry(EquipSlot.HELM, 2017025),
                Entry(EquipSlot.SHOULDER, 2017026),
                Entry(EquipSlot.GLOVES, 2017028),
                Entry(EquipSlot.BOOTS, 2017029),
                Entry(EquipSlot.BELT, 476982),
                Entry(EquipSlot.BACK, 473694),
                Entry(EquipSlot.POCKET_1, 129838806),
                Entry(EquipSlot.POCKET_2, 129838811),
                Entry(EquipSlot.JEWELLERY_1, 129838623),
                Entry(EquipSlot.JEWELLERY_2, 129838516),
                Entry(EquipSlot.JEWELLERY_3, 129838737),
                Entry(EquipSlot.JEWELLERY_4, 472926),
                Entry(EquipSlot.MAIN_HAND, 2000144));
        }

        private static void RegisterSharedT4Template(byte careerLine, BotRole role, params SlotEntry[] entries)
        {
            Loadout loadout = CreateLoadout(entries);
            foreach (BotTier tier in SharedT4Tiers)
                _templates[new TemplateKey(tier, careerLine, role)] = loadout;
        }

        private static Loadout CreateLoadout(IEnumerable<SlotEntry> entries)
        {
            Loadout loadout = new Loadout();

            foreach (SlotEntry entry in entries)
            {
                Item_Info itemInfo = ItemService.GetItem_Info(entry.Entry);
                if (itemInfo == null)
                {
                    Log.Error("BotLoadoutManager", $"Template item {entry.Entry} is missing from item_infos.");
                    continue;
                }

                if (!CanOccupySlot(itemInfo, entry.SlotId))
                {
                    Log.Error("BotLoadoutManager", $"Template item {entry.Entry} cannot occupy slot {entry.SlotId}.");
                    continue;
                }

                loadout.SlotItems[entry.SlotId] = entry.Entry;
            }

            return loadout;
        }

        private static SlotEntry Entry(EquipSlot slot, uint entry)
        {
            return new SlotEntry((ushort)slot, entry);
        }

        private readonly struct SlotEntry
        {
            public SlotEntry(ushort slotId, uint entry)
            {
                SlotId = slotId;
                Entry = entry;
            }

            public ushort SlotId { get; }
            public uint Entry { get; }
        }

        private readonly struct TemplateKey : IEquatable<TemplateKey>
        {
            public TemplateKey(BotTier tier, byte careerLine, BotRole role)
            {
                Tier = tier;
                CareerLine = careerLine;
                Role = role;
            }

            public BotTier Tier { get; }
            public byte CareerLine { get; }
            public BotRole Role { get; }

            public bool Equals(TemplateKey other)
            {
                return Tier == other.Tier && CareerLine == other.CareerLine && Role == other.Role;
            }

            public override bool Equals(object obj)
            {
                return obj is TemplateKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (int)Tier;
                    hashCode = (hashCode * 397) ^ CareerLine;
                    hashCode = (hashCode * 397) ^ (int)Role;
                    return hashCode;
                }
            }
        }
    }
}
