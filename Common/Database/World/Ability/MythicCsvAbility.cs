using FrameWork;
using System;

namespace Common
{
    // CSV-first naming: properties map to typed fields extracted from abilities.csv.
    // If a future rename is ambiguous, prefer Londo SQL terminology over legacy emulator names.
    [DataTable(PreCache = false, TableName = "mythic_csv_abilities", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicCsvAbility : DataObject
    {
        [PrimaryKey]
        public uint AbilityId { get; set; }

        [DataElement(Varchar = 255)]
        public string Name { get; set; }

        [DataElement]
        public string Description { get; set; }

        [DataElement]
        public string Notes { get; set; }

        [DataElement]
        public int IconId { get; set; }

        [DataElement]
        public int AnimationId { get; set; }

        [DataElement]
        public int EffectAbilityId { get; set; }

        // CSV-first semantic alias used by runtime code.
        public int EffectId
        {
            get => EffectAbilityId;
            set => EffectAbilityId = value;
        }

        [DataElement]
        public uint SourceRowIndex { get; set; }

        [DataElement]
        public string RawJson { get; set; }
    }
}
