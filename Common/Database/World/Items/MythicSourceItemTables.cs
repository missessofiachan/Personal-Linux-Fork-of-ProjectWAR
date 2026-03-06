using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "mythic_src_item_infos", DatabaseName = "World")]
    [Serializable]
    public class MythicSourceItemInfo : Item_Info
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_src_item_sets", DatabaseName = "World")]
    [Serializable]
    public class MythicSourceItemSet : Item_Set
    {
    }
}
