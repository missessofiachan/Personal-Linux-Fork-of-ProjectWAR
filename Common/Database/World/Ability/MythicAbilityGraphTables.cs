using FrameWork;
using System;

namespace Common
{
    // Name translation guide for future maintainers:
    // - Mythic/Londo "Ability" row ~= ProjectWAR "abilities" core constants row.
    // - Mythic/Londo "AbilityBin" row ~= client-facing ability metadata bundle.
    // - Mythic/Londo "AbilityComponentBin" row ~= component payload definition.
    // - Mythic/Londo "AbilityComponentXComponent" row ~= ability->component link + trigger order.
    // - Mythic/Londo "AbilityExpression/RequirmentBin" rows ~= conditional checks used by components.
    //
    // We keep both table-name families mapped:
    // 1) mythic_bin_*  : isolated ProjectWAR import tables
    // 2) Londo names   : direct toolkit import fallback (Ability, AbilityBin, ...)

    [DataTable(PreCache = false, TableName = "mythic_bin_ability", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicBinAbilityRow : DataObject
    {
        [PrimaryKey]
        public long ID { get; set; }

        [DataElement(Varchar = 500)]
        public string Name { get; set; }

        [DataElement]
        public string Description { get; set; }

        [DataElement]
        public int? EffectID { get; set; }

        [DataElement]
        public int? MinLevel { get; set; }

        [DataElement]
        public bool? IsBuff { get; set; }

        [DataElement]
        public bool? IsDebuff { get; set; }

        [DataElement]
        public bool? IsDamaging { get; set; }

        [DataElement]
        public bool? IsHealing { get; set; }

        [DataElement]
        public bool? IsGranted { get; set; }

        [DataElement]
        public bool? IsPassive { get; set; }

        [DataElement]
        public bool? IsOffensive { get; set; }

        [DataElement]
        public int? Flags { get; set; }

        [DataElement]
        public int? Casttime { get; set; }

        [DataElement]
        public int? Cooldown { get; set; }

        [DataElement]
        public int? AP { get; set; }

        [DataElement]
        public int? Specialization { get; set; }

        [DataElement]
        public int? ChannelInterval { get; set; }

        [DataElement]
        public int? TacticType { get; set; }

        [DataElement]
        public int? Range { get; set; }

        [DataElement]
        public int? MoraleLevel { get; set; }

        [DataElement]
        public int? MoraleCost { get; set; }

        [DataElement]
        public int? CastType { get; set; }

        [DataElement]
        public int? SpellDamageType { get; set; }

        [DataElement]
        public long? TargetType { get; set; }

        [DataElement]
        public int? MaxTargets { get; set; }

        [DataElement]
        public string MythicComponentData { get; set; }

        [DataElement]
        public string ComponentData { get; set; }

        [DataElement]
        public string RequirmentsData { get; set; }

        [DataElement]
        public string ComponentDataValues { get; set; }

        [DataElement]
        public string ComponentTriggers { get; set; }

        [DataElement]
        public string FlagBits { get; set; }
    }

    [DataTable(PreCache = false, TableName = "Ability", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class LondoAbilityRow : MythicBinAbilityRow
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_bin_abilitybin", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicBinAbilityBinRow : DataObject
    {
        [PrimaryKey]
        public long ID { get; set; }

        [DataElement]
        public int? EffectID { get; set; }

        [DataElement]
        public int? MinLevel { get; set; }

        [DataElement]
        public long? TargetType { get; set; }

        [DataElement]
        public int? Range { get; set; }

        [DataElement]
        public int? AP { get; set; }

        [DataElement]
        public int? Castime { get; set; }

        [DataElement]
        public int? Cooldown { get; set; }
    }

    [DataTable(PreCache = false, TableName = "AbilityBin", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class LondoAbilityBinRow : MythicBinAbilityBinRow
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_bin_abilitycomponentbin", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicBinAbilityComponentBinRow : DataObject
    {
        [PrimaryKey]
        public long ID { get; set; }

        [DataElement]
        public int? A00 { get; set; }

        [DataElement]
        public string Values { get; set; }

        [DataElement]
        public string Multipliers { get; set; }

        [DataElement]
        public long? ActivationDelay { get; set; }

        [DataElement]
        public long? Duration { get; set; }

        [DataElement]
        public long? Flags { get; set; }

        [DataElement]
        public long? Interval { get; set; }

        [DataElement]
        public int? Radius { get; set; }

        [DataElement]
        public int? ConeAngle { get; set; }

        [DataElement]
        public int? FlightSpeed { get; set; }

        [DataElement]
        public byte? MaxTargets { get; set; }

        [DataElement]
        public string Description { get; set; }
    }

    [DataTable(PreCache = false, TableName = "AbilityComponentBin", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class LondoAbilityComponentBinRow : MythicBinAbilityComponentBinRow
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_bin_abilitycomponentlink", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicBinAbilityComponentLinkRow : DataObject
    {
        [PrimaryKey]
        public long ID { get; set; }

        [DataElement]
        public long? AbilityID { get; set; }

        [DataElement]
        public long? ComponentID { get; set; }

        [DataElement]
        public long? Trigger { get; set; }

        [DataElement]
        public byte? VfxID { get; set; }

        [DataElement]
        public byte? Index { get; set; }

        [DataElement]
        public bool? Disabled { get; set; }
    }

    [DataTable(PreCache = false, TableName = "AbilityComponentXComponent", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class LondoAbilityComponentLinkRow : MythicBinAbilityComponentLinkRow
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_bin_abilityexpression", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicBinAbilityExpressionRow : DataObject
    {
        [PrimaryKey]
        public long ID { get; set; }

        [DataElement]
        public long? AbilityID { get; set; }

        [DataElement]
        public long? ComponentID { get; set; }

        [DataElement]
        public int? Index { get; set; }

        [DataElement]
        public long? Type { get; set; }

        [DataElement]
        public long? Operation { get; set; }

        [DataElement]
        public long? Condition { get; set; }

        [DataElement]
        public long? LogicOperator { get; set; }

        [DataElement]
        public long? RequirmentID { get; set; }

        [DataElement]
        public bool? Disabled { get; set; }
    }

    [DataTable(PreCache = false, TableName = "AbilityExpression", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class LondoAbilityExpressionRow : MythicBinAbilityExpressionRow
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_bin_abilityrequirmentbin", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicBinAbilityRequirementBinRow : DataObject
    {
        [PrimaryKey]
        public long ID { get; set; }

        [DataElement]
        public string Name { get; set; }

        [DataElement]
        public bool? Disabled { get; set; }
    }

    [DataTable(PreCache = false, TableName = "AbilityRequirmentBin", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class LondoAbilityRequirementBinRow : MythicBinAbilityRequirementBinRow
    {
        // Spelling is intentionally kept as "Requirment" to match Londo SQL source table naming.
    }

    [DataTable(PreCache = false, TableName = "mythic_bin_abilityupgradebin", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicBinAbilityUpgradeBinRow : DataObject
    {
        [PrimaryKey]
        public long ID { get; set; }

        [DataElement]
        public long? UpgradeID { get; set; }
    }

    [DataTable(PreCache = false, TableName = "AbilityUpgradeBin", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class LondoAbilityUpgradeBinRow : MythicBinAbilityUpgradeBinRow
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_bin_abilityupgradeentry", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicBinAbilityUpgradeEntryRow : DataObject
    {
        [PrimaryKey]
        public long ID { get; set; }

        [DataElement]
        public long? AbilityUpgradeBinID { get; set; }

        [DataElement]
        public int? Index { get; set; }

        [DataElement]
        public int? V1 { get; set; }

        [DataElement]
        public int? V2 { get; set; }

        [DataElement]
        public int? V3 { get; set; }

        [DataElement]
        public int? V4 { get; set; }

        [DataElement]
        public int? V5 { get; set; }

        [DataElement]
        public int? V6 { get; set; }

        [DataElement]
        public int? V7 { get; set; }

        [DataElement]
        public int? V8 { get; set; }

        [DataElement]
        public string Values { get; set; }
    }

    [DataTable(PreCache = false, TableName = "AbilityUpgradeEntry", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class LondoAbilityUpgradeEntryRow : MythicBinAbilityUpgradeEntryRow
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_bin_itemability", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicBinItemAbilityRow : DataObject
    {
        [PrimaryKey]
        public long ID { get; set; }

        [DataElement]
        public long? AbilityID { get; set; }

        [DataElement]
        public long? ItemID { get; set; }

        [DataElement]
        public long? Time { get; set; }

        [DataElement]
        public int? Unk1 { get; set; }

        [DataElement]
        public int? Cooldown { get; set; }
    }

    [DataTable(PreCache = false, TableName = "ItemAbility", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class LondoItemAbilityRow : MythicBinItemAbilityRow
    {
    }
}
