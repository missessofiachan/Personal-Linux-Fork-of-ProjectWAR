using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "mythic_src_abilities", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicSourceAbilityInfo : DBAbilityInfo
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_src_ability_commands", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicSourceAbilityCommandInfo : DBAbilityCommandInfo
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_src_buff_infos", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicSourceBuffInfo : DBBuffInfo
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_src_buff_commands", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicSourceBuffCommandInfo : DBBuffCommandInfo
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_src_ability_damage_heals", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicSourceAbilityDamageInfo : DBAbilityDamageInfo
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_src_ability_knockback_info", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicSourceAbilityKnockbackInfo : AbilityKnockbackInfo
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_src_ability_modifiers", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicSourceAbilityModifierEffect : AbilityModifierEffect
    {
    }

    [DataTable(PreCache = false, TableName = "mythic_src_ability_modifier_checks", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class MythicSourceAbilityModifierCheck : AbilityModifierCheck
    {
    }
}
