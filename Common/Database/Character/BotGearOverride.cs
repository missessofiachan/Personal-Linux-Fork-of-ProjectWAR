using System;
using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "bot_gear_overrides", DatabaseName = "Characters", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class BotGearOverride : DataObject
    {
        private uint _characterId;
        private ushort _slotId;
        private uint _itemEntry;

        [PrimaryKey]
        public uint CharacterId
        {
            get { return _characterId; }
            set { _characterId = value; Dirty = true; }
        }

        [PrimaryKey]
        public ushort SlotId
        {
            get { return _slotId; }
            set { _slotId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint ItemEntry
        {
            get { return _itemEntry; }
            set { _itemEntry = value; Dirty = true; }
        }
    }
}
