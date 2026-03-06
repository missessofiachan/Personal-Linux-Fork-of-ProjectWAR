using FrameWork;
using System;

namespace Common
{
    // CSV-first naming: properties map to typed effect-link fields extracted from effects.csv.
    // For uncertain mappings, prefer Londo SQL semantic names as fallback.
    [DataTable(PreCache = false, TableName = "mythic_csv_effects", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicCsvEffect : DataObject
    {
        [PrimaryKey]
        public uint EffectId { get; set; }

        [DataElement(Varchar = 255)]
        public string Name { get; set; }

        [DataElement]
        public int BuildUpEffectId { get; set; }
        public int BuildUpId
        {
            get => BuildUpEffectId;
            set => BuildUpEffectId = value;
        }

        [DataElement]
        public int ActivateEffectId { get; set; }
        public int ActivateId
        {
            get => ActivateEffectId;
            set => ActivateEffectId = value;
        }

        [DataElement]
        public int CastEffectId { get; set; }
        public int CastId
        {
            get => CastEffectId;
            set => CastEffectId = value;
        }

        [DataElement]
        public int ProjectileMainId { get; set; }
        public int ProjectileId
        {
            get => ProjectileMainId;
            set => ProjectileMainId = value;
        }

        [DataElement]
        public int ImpactEffectId { get; set; }
        public int ImpactId
        {
            get => ImpactEffectId;
            set => ImpactEffectId = value;
        }

        [DataElement]
        public int AoeEffectId { get; set; }
        public int AoeId
        {
            get => AoeEffectId;
            set => AoeEffectId = value;
        }

        [DataElement]
        public int ChannelEffectId { get; set; }
        public int ChannelingId
        {
            get => ChannelEffectId;
            set => ChannelEffectId = value;
        }

        [DataElement]
        public int VfxRefId { get; set; }
        public int VfxId
        {
            get => VfxRefId;
            set => VfxRefId = value;
        }

        [DataElement]
        public int AoeTarget { get; set; }

        [DataElement]
        public int AoeEffectsPerSec { get; set; }
        public int AoeEffectsPerSecond
        {
            get => AoeEffectsPerSec;
            set => AoeEffectsPerSec = value;
        }

        [DataElement]
        public int AoeRadius { get; set; }

        [DataElement]
        public int AoeDuration { get; set; }

        [DataElement]
        public int AoeLocation { get; set; }

        [DataElement]
        public int WeaponTrail { get; set; }

        [DataElement]
        public int ProjectileOff { get; set; }

        [DataElement]
        public int ProjectileOverride { get; set; }

        [DataElement]
        public uint SourceRowIndex { get; set; }

        [DataElement]
        public string RawJson { get; set; }
    }
}
