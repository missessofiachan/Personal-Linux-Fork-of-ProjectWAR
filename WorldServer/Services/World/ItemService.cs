using Common;
using Common.Database.World.Items;
using FrameWork;
using System.Collections.Generic;
using System.Linq;

namespace WorldServer.Services.World
{
    [Service]
    public class ItemService : ServiceBase
    {
        public static Dictionary<uint, Item_Info> _Item_Info;

        [LoadingFunction(true)]
        public static void LoadItem_Info()
        {
            Log.Debug("WorldMgr", "Loading Item_Info...");

            int i;
            bool useMythicSourceTables = Program.Config != null && Program.Config.UseMythicActionCoverageTables;

            Log.Info("ItemService",
                useMythicSourceTables
                    ? "Item loader source: mythic_src_item_infos (UseMythicActionCoverageTables=true)."
                    : "Item loader source: item_infos (UseMythicActionCoverageTables=false).");

            if (useMythicSourceTables)
            {
                Dictionary<uint, MythicSourceItemInfo> mythicItems =
                    Database.MapAllObjects<uint, MythicSourceItemInfo>("Entry", "Name != ''", 100000);

                _Item_Info = mythicItems.ToDictionary(kvp => kvp.Key, kvp => (Item_Info)kvp.Value);
            }
            else
            {
                _Item_Info = Database.MapAllObjects<uint, Item_Info>("Entry", "Name != ''", 100000);
            }

            foreach (Item_Info info in _Item_Info.Values)
            {
                foreach (KeyValuePair<byte, ushort> kp in info._Stats)
                {
                    if (kp.Key >= byte.MaxValue || kp.Value >= ushort.MaxValue)
                    {
                        info.Stats = "";
                        info.SellPrice = 0;
                        info._Stats.Clear();
                        break;
                    }
                }

                foreach (KeyValuePair<byte, ushort> kp in info._Crafts)
                {
                    if (kp.Key >= byte.MaxValue || kp.Value > ushort.MaxValue)
                    {
                        info.Crafts = "";
                        info.SellPrice = 0;
                        info._Crafts.Clear();
                        break;
                    }
                }

                if (info.Speed != 0 && info.Dps == 0)
                {
                    // Why is this here? - Az
                    //info.SellPrice = 0;
                    info.Speed = 0;
                }
                else if (info.Dps != 0 && info.Speed == 0)
                {
                    info.Dps = 0;
                    // Ditto - Az
                    //info.SellPrice = 0;
                }

                if (info.Unk27[4] != 3 || info.Unk27[5] != 2)
                {
                    for (i = 0; i < info.Unk27.Length; ++i)
                    {
                        info.Unk27[i] = 0;
                    }

                    info.Unk27[4] = 3;
                    info.Unk27[5] = 2;
                }
            }

            Log.Success("LoadItem_Info", "Loaded " + _Item_Info.Count + " Item_Info");

            foreach (Item_Info info in _Item_Info.Values)
            {
                info.RequiredItems = new List<KeyValuePair<Item_Info, ushort>>(info._SellRequiredItems.Count);
                foreach (KeyValuePair<uint, ushort> kp in info._SellRequiredItems)
                {
                    info.RequiredItems.Add(new KeyValuePair<Item_Info, ushort>(GetItem_Info(kp.Key), kp.Value));
                }
            }
        }

        public static Item_Info GetItem_Info(uint entry)
        {
            Item_Info info;
            _Item_Info.TryGetValue(entry, out info);
            return info;
        }

        public static Dictionary<uint, Item_Set> _Item_Sets;

        [LoadingFunction(true)]
        public static void LoadItem_Set()
        {
            Log.Debug("WorldMgr", "Loading Item_Set...");
            bool useMythicSourceTables = Program.Config != null && Program.Config.UseMythicActionCoverageTables;

            Log.Info("ItemService",
                useMythicSourceTables
                    ? "Item set loader source: mythic_src_item_sets (UseMythicActionCoverageTables=true)."
                    : "Item set loader source: item_sets (UseMythicActionCoverageTables=false).");

            _Item_Sets = new Dictionary<uint, Item_Set>();

            IList<Item_Set> infos = useMythicSourceTables
                ? Database.SelectAllObjects<MythicSourceItemSet>().Cast<Item_Set>().ToList()
                : Database.SelectAllObjects<Item_Set>();

            foreach (Item_Set info in infos)
                _Item_Sets.Add(info.Entry, info);

            Log.Success("LoadItem_Set", "Loaded " + _Item_Sets.Count + " Item_Set");
        }

        public static Item_Set GetItem_Set(uint entry)
        {
            if (_Item_Sets.ContainsKey(entry))
                return _Item_Sets[entry];
            return null;
        }


        public static List<BlackMarketItem> _BlackMarket_Items;

        [LoadingFunction(true)]
        public static void LoadBlackMarketItems()
        {
            Log.Debug("WorldMgr", "LoadBlackMarketItems...");

            _BlackMarket_Items = new List<BlackMarketItem>();

            IList<BlackMarketItem> items = Database.SelectAllObjects<BlackMarketItem>();

            foreach (var blackMarketItem in items)
            {
                _BlackMarket_Items.Add(blackMarketItem);
            }

            Log.Success("LoadBlackMarketItems", "Loaded " + _Item_Sets.Count + " LoadBlackMarketItems");
        }

    }
}
