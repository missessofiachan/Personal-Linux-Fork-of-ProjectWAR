using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "ability_entry_resolver", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class AbilityEntryResolver : DataObject
    {
        [PrimaryKey]
        public ushort SourceEntry { get; set; }

        [DataElement]
        public ushort CanonicalAbilityEntry { get; set; }

        [DataElement]
        public ushort CanonicalBuffEntry { get; set; }

        [DataElement(Varchar = 48)]
        public string ResolutionSource { get; set; }

        [DataElement]
        public bool Enabled { get; set; } = true;

        [DataElement(Varchar = 255)]
        public string Notes { get; set; }
    }
}
