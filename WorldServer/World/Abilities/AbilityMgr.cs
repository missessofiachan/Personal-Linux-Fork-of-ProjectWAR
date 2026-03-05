using Common;
using FrameWork;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldServer.Managers;
using WorldServer.World.Abilities.Components;
using WorldServer.World.Objects;

namespace WorldServer.World.Abilities
{
    [Service(typeof(WorldMgr))]
    public static class AbilityMgr
    {
        #region Ability System Setup

        // Abilities
        public static Dictionary<ushort, AbilityInfo> NewAbilityVolatiles = new Dictionary<ushort, AbilityInfo>();

        public static Dictionary<ushort, List<AbilityCommandInfo>> AbilityCommandInfos = new Dictionary<ushort, List<AbilityCommandInfo>>();

        // Modifiers
        public static Dictionary<ushort, List<AbilityModifier>> AbilityPreCastModifiers = new Dictionary<ushort, List<AbilityModifier>>();

        public static Dictionary<ushort, List<AbilityModifier>> AbilityModifiers = new Dictionary<ushort, List<AbilityModifier>>();
        public static Dictionary<ushort, List<AbilityModifier>> AbilityDelayedModifiers = new Dictionary<ushort, List<AbilityModifier>>();
        public static Dictionary<ushort, List<AbilityModifier>> BuffModifiers = new Dictionary<ushort, List<AbilityModifier>>();

        // Buffs
        public static Dictionary<ushort, BuffInfo> BuffInfos = new Dictionary<ushort, BuffInfo>();

        public static Dictionary<ushort, List<BuffCommandInfo>> BuffCommandInfos = new Dictionary<ushort, List<BuffCommandInfo>>();

        // Knockback
        public static Dictionary<ushort, List<AbilityKnockbackInfo>> KnockbackInfos = new Dictionary<ushort, List<AbilityKnockbackInfo>>();

        // Extra Damage Info (type-2 damage)
        public static Dictionary<ushort, List<List<AbilityDamageInfo>>> ExtraDamage = new Dictionary<ushort, List<List<AbilityDamageInfo>>>();

        // Career abilities
        public static List<AbilityInfo>[] CareerAbilities = new List<AbilityInfo>[24];

        // Canonical entry maps used to resolve pseudo/alias ability and buff entries.
        private static readonly Dictionary<ushort, ushort> AbilityEntryAliases = new Dictionary<ushort, ushort>();
        private static readonly Dictionary<ushort, ushort> BuffEntryAliases = new Dictionary<ushort, ushort>();

        private const ushort RenownEmpoweredMasteryEntry = 27870;
        private const ushort RenownEternalMasteryEntry = 27871;
        private const ushort RenownInfiniteMasteryEntry = 27872;
        private const ushort RenownAugmentVigorEntry = 27873;
        private const ushort RenownSilentBonusRankOneEntry = 22275;
        private const ushort RenownSilentBonusRankTwoEntry = 27875;
        private const ushort RenownSilentActionPointBonus = 50;
        private const ushort ImDaBiggestBuffEntry = 734;

        public static void ReloadAbilities()
        {
            NewAbilityVolatiles.Clear();
            AbilityCommandInfos.Clear();
            AbilityPreCastModifiers.Clear();
            AbilityModifiers.Clear();
            AbilityDelayedModifiers.Clear();

            BuffInfos.Clear();
            BuffCommandInfos.Clear();
            BuffModifiers.Clear();

            KnockbackInfos.Clear();

            ExtraDamage.Clear();
            AbilityEntryAliases.Clear();
            BuffEntryAliases.Clear();

            for (byte i = 0; i < 24; ++i)
                CareerAbilities[i].Clear();

            LoadNewAbilityInfo();
        }

        [LoadingFunction(true)]
        public static void LoadNewAbilityInfo()
        {
            Log.Info("AbilityMgr", "Loading New Ability Info...");

            IObjectDatabase db = WorldMgr.Database;

            #region Database

            List<DBAbilityInfo> dbAbilities = (List<DBAbilityInfo>)db.SelectAllObjects<DBAbilityInfo>();

            List<AbilityInfo> abVolatiles = AbilityInfo.Convert(dbAbilities);
            Dictionary<ushort, AbilityConstants> abConstants = AbilityConstants.Convert(dbAbilities).ToDictionary(key => key.Entry);
            List<AbilityDamageInfo> abDmgHeals = AbilityDamageInfo.Convert(db.SelectAllObjects<DBAbilityDamageInfo>().OrderBy(dmg => dmg.ParentCommandID).ThenBy(dmg => dmg.ParentCommandSequence).ToList());
            List<AbilityCommandInfo> abCommands = AbilityCommandInfo.Convert(db.SelectAllObjects<DBAbilityCommandInfo>().OrderBy(cmd => cmd.CommandID).ToList());

            IList<AbilityModifierCheck> abChecks = db.SelectAllObjects<AbilityModifierCheck>().OrderBy(check => check.ID).ToList();
            IList<AbilityModifierEffect> abMods = db.SelectAllObjects<AbilityModifierEffect>().OrderBy(mod => mod.Sequence).ToList();

            List<BuffInfo> buffInfos = BuffInfo.Convert((List<DBBuffInfo>)db.SelectAllObjects<DBBuffInfo>());
            List<BuffCommandInfo> buffCommands = BuffCommandInfo.Convert(db.SelectAllObjects<DBBuffCommandInfo>().OrderBy(buffcmd => buffcmd.CommandID).ToList());

            IList<AbilityKnockbackInfo> knockbackInfos = db.SelectAllObjects<AbilityKnockbackInfo>().OrderBy(kbinfo => kbinfo.Id).ToList();

            List<AbilityCommandInfo> slaveCommands = new List<AbilityCommandInfo>();
            List<BuffCommandInfo> slaveBuffCommands = new List<BuffCommandInfo>();

            Dictionary<ushort, int> damageTypeDictionary = new Dictionary<ushort, int>();

            #endregion Database

            for (byte i = 0; i < 24; ++i)
                CareerAbilities[i] = new List<AbilityInfo>();

            #region AbilityChecks

            foreach (AbilityModifierCheck check in abChecks)
            {
                switch (check.PreOrPost)
                {
                    case 0:
                        if (!AbilityPreCastModifiers.ContainsKey(check.Entry))
                        {
                            AbilityPreCastModifiers.Add(check.Entry, new List<AbilityModifier>());

                            while (AbilityPreCastModifiers[check.Entry].Count < check.ID + 1)
                                AbilityPreCastModifiers[check.Entry].Add(new AbilityModifier(check.Entry, check.Affecting));

                            AbilityPreCastModifiers[check.Entry][check.ID].AddCheck(check);
                        }
                        else
                        {
                            if (AbilityPreCastModifiers[check.Entry].Count == check.ID)
                                AbilityPreCastModifiers[check.Entry].Add(new AbilityModifier(check.Entry, check.Affecting));
                            AbilityPreCastModifiers[check.Entry][check.ID].AddCheck(check);
                        }
                        break;

                    case 1:
                        if (!AbilityModifiers.ContainsKey(check.Entry))
                        {
                            AbilityModifiers.Add(check.Entry, new List<AbilityModifier>());

                            while (AbilityModifiers[check.Entry].Count < check.ID + 1)
                                AbilityModifiers[check.Entry].Add(new AbilityModifier(check.Entry, check.Affecting));
                            AbilityModifiers[check.Entry][check.ID].AddCheck(check);
                        }
                        else
                        {
                            if (AbilityModifiers[check.Entry].Count == check.ID)
                                AbilityModifiers[check.Entry].Add(new AbilityModifier(check.Entry, check.Affecting));
                            AbilityModifiers[check.Entry][check.ID].AddCheck(check);
                        }
                        break;

                    case 2:
                        if (!BuffModifiers.ContainsKey(check.Entry))
                        {
                            BuffModifiers.Add(check.Entry, new List<AbilityModifier>());
                            while (BuffModifiers[check.Entry].Count < check.ID + 1)
                                BuffModifiers[check.Entry].Add(new AbilityModifier(check.Entry, check.Affecting));
                            BuffModifiers[check.Entry][check.ID].AddCheck(check);
                        }
                        else
                        {
                            if (BuffModifiers[check.Entry].Count == check.ID)
                                BuffModifiers[check.Entry].Add(new AbilityModifier(check.Entry, check.Affecting));
                            BuffModifiers[check.Entry][check.ID].AddCheck(check);
                        }
                        break;

                    case 3:
                        if (!AbilityDelayedModifiers.ContainsKey(check.Entry))
                        {
                            AbilityDelayedModifiers.Add(check.Entry, new List<AbilityModifier>());

                            while (AbilityDelayedModifiers[check.Entry].Count < check.ID + 1)
                                AbilityDelayedModifiers[check.Entry].Add(new AbilityModifier(check.Entry, check.Affecting));

                            AbilityDelayedModifiers[check.Entry][check.ID].AddCheck(check);
                        }
                        else
                        {
                            if (AbilityDelayedModifiers[check.Entry].Count == check.ID)
                                AbilityDelayedModifiers[check.Entry].Add(new AbilityModifier(check.Entry, check.Affecting));
                            AbilityDelayedModifiers[check.Entry][check.ID].AddCheck(check);
                        }
                        break;
                }
            }

            #endregion AbilityChecks

            #region AbilityModifiers

            foreach (AbilityModifierEffect effect in abMods)
            {
                switch (effect.PreOrPost)
                {
                    case 0:
                        if (!AbilityPreCastModifiers.ContainsKey(effect.Entry))
                        {
                            AbilityPreCastModifiers.Add(effect.Entry, new List<AbilityModifier>());
                            AbilityPreCastModifiers[effect.Entry].Add(new AbilityModifier(effect.Entry, effect.Affecting));
                            AbilityPreCastModifiers[effect.Entry][0].AddModifier(effect);
                        }
                        else
                        {
                            if (AbilityPreCastModifiers[effect.Entry].Count == effect.Sequence)
                                AbilityPreCastModifiers[effect.Entry].Add(new AbilityModifier(effect.Entry, effect.Affecting));
                            AbilityPreCastModifiers[effect.Entry][effect.Sequence].AddModifier(effect);
                        }
                        break;

                    case 1:
                        if (!AbilityModifiers.ContainsKey(effect.Entry))
                        {
                            AbilityModifiers.Add(effect.Entry, new List<AbilityModifier>());
                            AbilityModifiers[effect.Entry].Add(new AbilityModifier(effect.Entry, effect.Affecting));
                            AbilityModifiers[effect.Entry][0].AddModifier(effect);
                        }
                        else
                        {
                            if (AbilityModifiers[effect.Entry].Count == effect.Sequence)
                                AbilityModifiers[effect.Entry].Add(new AbilityModifier(effect.Entry, effect.Affecting));
                            AbilityModifiers[effect.Entry][effect.Sequence].AddModifier(effect);
                        }
                        break;

                    case 2:
                        if (!BuffModifiers.ContainsKey(effect.Entry))
                        {
                            BuffModifiers.Add(effect.Entry, new List<AbilityModifier>());
                            BuffModifiers[effect.Entry].Add(new AbilityModifier(effect.Entry, effect.Affecting));
                            BuffModifiers[effect.Entry][0].AddModifier(effect);
                        }
                        else
                        {
                            if (BuffModifiers[effect.Entry].Count == effect.Sequence)
                                BuffModifiers[effect.Entry].Add(new AbilityModifier(effect.Entry, effect.Affecting));
                            BuffModifiers[effect.Entry][effect.Sequence].AddModifier(effect);
                        }
                        break;

                    case 3:
                        if (!AbilityDelayedModifiers.ContainsKey(effect.Entry))
                        {
                            AbilityDelayedModifiers.Add(effect.Entry, new List<AbilityModifier>());
                            AbilityDelayedModifiers[effect.Entry].Add(new AbilityModifier(effect.Entry, effect.Affecting));
                            AbilityDelayedModifiers[effect.Entry][0].AddModifier(effect);
                        }
                        else
                        {
                            if (AbilityDelayedModifiers[effect.Entry].Count == effect.Sequence)
                                AbilityDelayedModifiers[effect.Entry].Add(new AbilityModifier(effect.Entry, effect.Affecting));
                            AbilityDelayedModifiers[effect.Entry][effect.Sequence].AddModifier(effect);
                        }
                        break;
                }
            }

            #endregion AbilityModifiers

            #region CommandInfo

            // Ability commands
            foreach (AbilityCommandInfo abCommand in abCommands)
            {
                if (abCommand.CommandSequence != 0)
                    slaveCommands.Add(abCommand);
                else
                {
                    if (!AbilityCommandInfos.ContainsKey(abCommand.Entry))
                        AbilityCommandInfos.Add(abCommand.Entry, new List<AbilityCommandInfo>());

                    AbilityCommandInfos[abCommand.Entry].Add(abCommand);
                }
            }

            foreach (AbilityCommandInfo slaveCommand in slaveCommands)
            {
                if (AbilityCommandInfos.ContainsKey(slaveCommand.Entry))
                    AbilityCommandInfos[slaveCommand.Entry][slaveCommand.CommandID].AddCommandToChain(slaveCommand);
                else
                    Log.Debug("AbilityMgr", "Slave command with entry " + slaveCommand.Entry + " and depending upon master command ID " + slaveCommand.CommandID + " has no master!");
            }

            #endregion CommandInfo

            #region BuffCommands

            foreach (BuffCommandInfo buffCommand in buffCommands)
            {
                if (buffCommand.CommandSequence != 0)
                    slaveBuffCommands.Add(buffCommand);
                else
                {
                    if (!BuffCommandInfos.ContainsKey(buffCommand.Entry))
                        BuffCommandInfos.Add(buffCommand.Entry, new List<BuffCommandInfo>());
                    BuffCommandInfos[buffCommand.Entry].Add(buffCommand);
                }
            }

            foreach (BuffCommandInfo slaveBuffCommand in slaveBuffCommands)
            {
                if (BuffCommandInfos.ContainsKey(slaveBuffCommand.Entry))
                    BuffCommandInfos[slaveBuffCommand.Entry][slaveBuffCommand.CommandID].AddCommandToChain(slaveBuffCommand);
                else
                    Log.Debug("AbilityMgr", "Slave buff command with entry " + slaveBuffCommand.Entry + " and depending upon master command ID " + slaveBuffCommand.CommandID + " has no master!");
            }

            #endregion BuffCommands

            #region Damage/Heals

            // Damage and heal info gets tacked onto the command that's going to use it
            foreach (AbilityDamageInfo abDmgHeal in abDmgHeals)
            {
                if (abDmgHeal.DisplayEntry == 0)
                    abDmgHeal.DisplayEntry = abDmgHeal.Entry;
                switch (abDmgHeal.Index)
                {
                    case 0:
                        if (AbilityCommandInfos.ContainsKey(abDmgHeal.Entry))
                        {
                            AbilityCommandInfo desiredCommand = AbilityCommandInfos[abDmgHeal.Entry][abDmgHeal.ParentCommandID].GetSubcommand(abDmgHeal.ParentCommandSequence);
                            if (desiredCommand != null)
                                desiredCommand.DamageInfo = abDmgHeal;
                        }

                        if (!damageTypeDictionary.ContainsKey(abDmgHeal.Entry))
                            damageTypeDictionary.Add(abDmgHeal.Entry, (int)abDmgHeal.DamageType);
                        break;

                    case 1:
                        if (BuffCommandInfos.ContainsKey(abDmgHeal.Entry))
                        {
                            try
                            {
                                BuffCommandInfo desiredCommand = BuffCommandInfos[abDmgHeal.Entry][abDmgHeal.ParentCommandID].GetSubcommand(abDmgHeal.ParentCommandSequence);
                                if (desiredCommand != null)
                                    desiredCommand.DamageInfo = abDmgHeal;
                            }
                            catch
                            {
                                Log.Error("AbilityMgr", "Failed Load: " + abDmgHeal.Entry + " " + abDmgHeal.ParentCommandID);
                            }

                            if (!damageTypeDictionary.ContainsKey(abDmgHeal.Entry))
                                damageTypeDictionary.Add(abDmgHeal.Entry, (int)abDmgHeal.DamageType);
                        }
                        break;

                    case 2:
                        if (!ExtraDamage.ContainsKey(abDmgHeal.Entry))
                            ExtraDamage.Add(abDmgHeal.Entry, new List<List<AbilityDamageInfo>>());
                        if (ExtraDamage[abDmgHeal.Entry].Count == abDmgHeal.ParentCommandID)
                            ExtraDamage[abDmgHeal.Entry].Add(new List<AbilityDamageInfo>());
                        ExtraDamage[abDmgHeal.Entry][abDmgHeal.ParentCommandID].Add(abDmgHeal);
                        break;

                    default:
                        throw new Exception("Invalid index specified for ability damage with ID " + abDmgHeal.Entry);
                }
            }

            #endregion Damage/Heals

            #region KnockbackInfo

            foreach (var kbInfo in knockbackInfos)
            {
                if (!KnockbackInfos.ContainsKey(kbInfo.Entry))
                    KnockbackInfos.Add(kbInfo.Entry, new List<AbilityKnockbackInfo>());
                KnockbackInfos[kbInfo.Entry].Add(kbInfo);
            }

            #endregion KnockbackInfo

            // Volatiles -> Constants
            //           -> Commands -> DamageHeals
            foreach (AbilityInfo abVolatile in abVolatiles)
            {
                if (!NewAbilityVolatiles.ContainsKey(abVolatile.Entry))
                    NewAbilityVolatiles.Add(abVolatile.Entry, abVolatile);

                if (AbilityCommandInfos.ContainsKey(abVolatile.Entry))
                {
                    abVolatile.TargetType = AbilityCommandInfos[abVolatile.Entry][0].TargetType;
                    if (AbilityCommandInfos[abVolatile.Entry][0].AoESource != 0)
                        abVolatile.TargetType = AbilityCommandInfos[abVolatile.Entry][0].AoESource;
                }
            }

            #region ConstantInfo

            foreach (AbilityConstants abConstant in abConstants.Values)
            {
                if (NewAbilityVolatiles.ContainsKey(abConstant.Entry))
                {
                    NewAbilityVolatiles[abConstant.Entry].ConstantInfo = abConstant;

                    if (damageTypeDictionary.ContainsKey(abConstant.Entry))
                    {
                        if (damageTypeDictionary[abConstant.Entry] == (ushort)DamageTypes.Healing || damageTypeDictionary[abConstant.Entry] == (ushort)DamageTypes.RawHealing)
                            abConstant.IsHealing = true;
                        else abConstant.IsDamaging = true;
                    }

                    uint careerRequirement = abConstant.CareerLine;
                    byte count = 0;

                    while (careerRequirement > 0 && count < 24)
                    {
                        if ((careerRequirement & 1) > 0)
                            CareerAbilities[count].Add(NewAbilityVolatiles[abConstant.Entry]);
                        careerRequirement = careerRequirement >> 1;
                        count++;
                    }
                }
            }

            #endregion ConstantInfo

            #region Damage to ConstantInfo linkage

            foreach (AbilityDamageInfo damageInfo in abDmgHeals)
            {
                if (abConstants.ContainsKey(damageInfo.Entry))
                    damageInfo.MasteryTree = abConstants[damageInfo.Entry].MasteryTree;
            }

            #endregion Damage to ConstantInfo linkage

            #region Buff/Command linkage

            foreach (BuffInfo buffInfo in buffInfos)
            {
                if (!BuffInfos.ContainsKey(buffInfo.Entry))
                    BuffInfos.Add(buffInfo.Entry, buffInfo);

                if (BuffCommandInfos.ContainsKey(buffInfo.Entry))
                    buffInfo.CommandInfo = BuffCommandInfos[buffInfo.Entry];

                if (abConstants.ContainsKey(buffInfo.Entry))
                    buffInfo.MasteryTree = abConstants[buffInfo.Entry].MasteryTree;
            }

            EnsureRenownTacticBuffs();
            BuildCanonicalEntryAliases();

            #endregion Buff/Command linkage

            Log.Success("AbilityMgr", "Finished loading " + NewAbilityVolatiles.Count + " abilities and " + BuffInfos.Count + " buffs!");

            LoadCreatureAbilities();
        }

        #region Creature Abilities

        public static Dictionary<uint, List<NPCAbility>> CreatureAbilities = new Dictionary<uint, List<NPCAbility>>();

        public static void LoadCreatureAbilities()
        {
            CreatureAbilities.Clear();

            IList<Creature_abilities> creaAbs = WorldMgr.Database.SelectAllObjects<Creature_abilities>();

            Dictionary<uint, List<NPCAbility>> temp = new Dictionary<uint, List<NPCAbility>>();

            foreach (var cAb in creaAbs)
            {
                if (!temp.ContainsKey(cAb.ProtoEntry))
                    temp.Add(cAb.ProtoEntry, new List<NPCAbility>());

                AbilityInfo abInfo = GetAbilityInfo(cAb.AbilityId);
                if (abInfo != null)
                {
                    temp[cAb.ProtoEntry].Add(new NPCAbility(cAb.AbilityId, abInfo.ConstantInfo.AIRange, Math.Max(abInfo.Cooldown, cAb.Cooldown), true, cAb.Text, cAb.TimeStart, cAb.ActivateAtHealthPercent, cAb.AbilityCycle, cAb.Active, cAb.ActivateOnCombatStart, cAb.RandomTarget, cAb.TargetFocus, cAb.DisableAtHealthPercent, cAb.MinRange));
                    Log.Dump("Entry: " + cAb.ProtoEntry, cAb.AbilityId + " " + abInfo.Name + " ~ Loaded");
                }
                else
                {
                    Log.Error("Entry: " + cAb.ProtoEntry, cAb.AbilityId + " ~ Failed loading");
                }
            }

            foreach (uint key in temp.Keys)
                CreatureAbilities[key] = temp[key].OrderByDescending(x => x.Cooldown).ToList();
        }

        public static List<NPCAbility> GetCreatureAbilities(uint entry)
        {
            if (CreatureAbilities.ContainsKey(entry))
                return CreatureAbilities[entry];
            return null;
        }

        #endregion Creature Abilities

        #endregion Ability System Setup

        private static void EnsureRenownTacticBuffs()
        {
            EnsureMasteryBonusTacticBuff(RenownEmpoweredMasteryEntry, "Empowered Mastery", Stats.Mastery1Bonus);
            EnsureMasteryBonusTacticBuff(RenownEternalMasteryEntry, "Eternal Mastery", Stats.Mastery2Bonus);
            EnsureMasteryBonusTacticBuff(RenownInfiniteMasteryEntry, "Infinite Mastery", Stats.Mastery3Bonus);
            EnsureAugmentVigorBuff();
            EnsureRenownSilentBonusBuffs();
        }

        private static void EnsureRenownSilentBonusBuffs()
        {
            EnsureRenownSilentBonusBuff(RenownSilentBonusRankOneEntry, "Booster Renown Bonuses", true);
            EnsureRenownSilentBonusBuff(RenownSilentBonusRankTwoEntry, "Booster Renown Bonuses II", false);
        }

        private static void EnsureRenownSilentBonusBuff(ushort entry, string name, bool addsActionPoints)
        {
            BuffInfo buffInfo = CreatePersistentSilentBuffShell(entry, name);
            byte buffLine = buffInfo.StackLine == 0 ? (byte)1 : buffInfo.StackLine;
            buffInfo.StackLine = buffLine;

            if (addsActionPoints)
            {
                buffInfo.CommandInfo.Add(new BuffCommandInfo
                {
                    Entry = entry,
                    Name = name,
                    CommandID = 0,
                    CommandSequence = 0,
                    CommandName = "ModifyStat",
                    BuffClass = BuffClass.Persist,
                    PrimaryValue = (int)Stats.MaxActionPoints,
                    SecondaryValue = RenownSilentActionPointBonus,
                    InvokeOn = 5,
                    TargetType = CommandTargetTypes.Host,
                    BuffLine = buffLine
                });
            }

            BuffInfos[entry] = buffInfo;
            Log.Info("AbilityMgr", addsActionPoints
                ? $"Installed renown silent buff {entry} ({name}) with +{RenownSilentActionPointBonus} AP."
                : $"Installed renown silent buff {entry} ({name}) as a persistent mastery marker.");
        }

        private static void EnsureMasteryBonusTacticBuff(ushort entry, string name, Stats masteryBonusStat)
        {
            BuffInfo buffInfo = CreateTacticBuffShell(entry, name);
            byte buffLine = buffInfo.StackLine == 0 ? (byte)1 : buffInfo.StackLine;
            buffInfo.StackLine = buffLine;
            buffInfo.CommandInfo.Add(new BuffCommandInfo
            {
                Entry = entry,
                Name = name,
                CommandID = 0,
                CommandSequence = 0,
                CommandName = "ModifyStat",
                BuffClass = BuffClass.Tactic,
                PrimaryValue = (int)masteryBonusStat,
                SecondaryValue = 1,
                InvokeOn = 5,
                TargetType = CommandTargetTypes.Host,
                BuffLine = buffLine
            });

            BuffInfos[entry] = buffInfo;
            Log.Info("AbilityMgr", $"Installed mastery tactic buff {entry} ({name}) using {masteryBonusStat}.");
        }

        private static void EnsureAugmentVigorBuff()
        {
            if (BuffInfos.ContainsKey(RenownAugmentVigorEntry))
                return;

            if (!BuffInfos.ContainsKey(ImDaBiggestBuffEntry))
            {
                Log.Error("AbilityMgr", $"Unable to generate fallback buff for tactic {RenownAugmentVigorEntry} (Augment Vigor): source buff {ImDaBiggestBuffEntry} missing.");
                return;
            }

            BuffInfo augmentVigor = BuffInfos[ImDaBiggestBuffEntry].Clone();
            augmentVigor.Entry = RenownAugmentVigorEntry;
            augmentVigor.Name = "Augment Vigor";
            augmentVigor.MasteryTree = 0;

            if (augmentVigor.CommandInfo != null)
            {
                foreach (BuffCommandInfo command in augmentVigor.CommandInfo)
                {
                    command.Entry = RenownAugmentVigorEntry;
                    command.Name = augmentVigor.Name;
                }
            }
            else
            {
                augmentVigor.CommandInfo = new List<BuffCommandInfo>();
            }

            BuffInfos[RenownAugmentVigorEntry] = augmentVigor;
            Log.Info("AbilityMgr", $"Generated fallback tactic buff {RenownAugmentVigorEntry} ({augmentVigor.Name}) from {ImDaBiggestBuffEntry}.");
        }

        private static BuffInfo CreateTacticBuffShell(ushort entry, string name)
        {
            BuffInfo shell = BuffInfos.ContainsKey(ImDaBiggestBuffEntry)
                ? BuffInfos[ImDaBiggestBuffEntry].Clone()
                : new BuffInfo();

            shell.Entry = entry;
            shell.Name = name;
            shell.BuffClass = BuffClass.Tactic;
            shell.Type = BuffTypes.None;
            shell.Group = 0;
            shell.MaxCopies = 0;
            shell.MaxStack = 1;
            shell.InitialStacks = 1;
            shell.StacksFromCaster = false;
            shell.Duration = 0;
            shell.LeadInDelay = 0;
            shell.Interval = 0;
            shell.BuffIntervals = 0;
            shell.PersistsOnDeath = 1;
            shell.CanRefresh = false;
            shell.MasteryTree = 0;
            shell.CommandInfo = new List<BuffCommandInfo>();
            return shell;
        }

        private static BuffInfo CreatePersistentSilentBuffShell(ushort entry, string name)
        {
            BuffInfo shell = BuffInfos.ContainsKey(entry)
                ? BuffInfos[entry].Clone()
                : (BuffInfos.ContainsKey(ImDaBiggestBuffEntry) ? BuffInfos[ImDaBiggestBuffEntry].Clone() : new BuffInfo());

            shell.Entry = entry;
            shell.Name = name;
            shell.BuffClass = BuffClass.Persist;
            shell.Type = BuffTypes.None;
            shell.Group = 0;
            shell.MaxCopies = 0;
            shell.MaxStack = 1;
            shell.InitialStacks = 1;
            shell.StacksFromCaster = false;
            shell.Duration = 0;
            shell.LeadInDelay = 0;
            shell.Interval = 0;
            shell.BuffIntervals = 0;
            shell.PersistsOnDeath = 1;
            shell.CanRefresh = false;
            shell.MasteryTree = 0;
            shell.FriendlyEffectID = 0;
            shell.EnemyEffectID = 0;
            shell.EffectType = 0;
            shell.CommandInfo = new List<BuffCommandInfo>();
            return shell;
        }

        private static void BuildCanonicalEntryAliases()
        {
            AbilityEntryAliases.Clear();
            BuffEntryAliases.Clear();

            foreach (AbilityInfo ability in NewAbilityVolatiles.Values)
            {
                ushort canonicalEntry = GetEffectAliasTarget(ability);
                if (canonicalEntry == 0 || canonicalEntry == ability.Entry)
                    continue;

                AbilityEntryAliases[ability.Entry] = canonicalEntry;
            }

            foreach (var aliasPair in AbilityEntryAliases)
            {
                if (!BuffInfos.ContainsKey(aliasPair.Key) && BuffInfos.ContainsKey(aliasPair.Value))
                    BuffEntryAliases[aliasPair.Key] = aliasPair.Value;
            }

            foreach (AbilityInfo ability in NewAbilityVolatiles.Values)
            {
                ushort effectEntry = ability.ConstantInfo?.EffectID ?? 0;
                if (effectEntry == 0 || effectEntry == ability.Entry)
                    continue;

                if (!BuffInfos.ContainsKey(ability.Entry) && BuffInfos.ContainsKey(effectEntry))
                    BuffEntryAliases[ability.Entry] = effectEntry;
            }

            if (AbilityEntryAliases.Count > 0 || BuffEntryAliases.Count > 0)
            {
                Log.Info("AbilityMgr",
                    $"Installed canonical ability aliases: {AbilityEntryAliases.Count}, buff aliases: {BuffEntryAliases.Count}.");
            }
        }

        private static ushort GetEffectAliasTarget(AbilityInfo sourceAbility)
        {
            if (sourceAbility == null || sourceAbility.ConstantInfo == null)
                return 0;

            ushort sourceEntry = sourceAbility.Entry;
            ushort effectEntry = sourceAbility.ConstantInfo.EffectID;
            if (effectEntry == 0 || effectEntry == sourceEntry)
                return 0;

            if (!NewAbilityVolatiles.TryGetValue(effectEntry, out AbilityInfo targetAbility))
                return 0;

            if (HasDirectImplementation(sourceEntry))
                return 0;

            if (!HasDirectImplementation(effectEntry))
                return 0;

            if (!IsLikelyAliasPair(sourceAbility, targetAbility))
                return 0;

            return effectEntry;
        }

        private static bool IsLikelyAliasPair(AbilityInfo sourceAbility, AbilityInfo targetAbility)
        {
            if (sourceAbility?.ConstantInfo == null || targetAbility?.ConstantInfo == null)
                return false;

            if (sourceAbility.ConstantInfo.CareerLine != 0
                && targetAbility.ConstantInfo.CareerLine != 0
                && sourceAbility.ConstantInfo.CareerLine != targetAbility.ConstantInfo.CareerLine)
            {
                return false;
            }

            if (NamesEquivalent(sourceAbility.Name, targetAbility.Name))
                return true;

            return sourceAbility.ConstantInfo.MasteryTree == targetAbility.ConstantInfo.MasteryTree;
        }

        private static bool NamesEquivalent(string left, string right)
        {
            if (left == null || right == null)
                return false;

            return left.Trim().Equals(right.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasDirectImplementation(ushort entry)
        {
            if (AbilityCommandInfos.ContainsKey(entry))
                return true;

            return NewAbilityVolatiles.ContainsKey(entry) && NewAbilityVolatiles[entry].ConstantInfo?.ChannelID > 0;
        }

        private static ushort ResolveAlias(ushort entry, Dictionary<ushort, ushort> aliasMap)
        {
            ushort current = entry;
            byte guard = 0;

            while (guard < 8 && aliasMap.TryGetValue(current, out ushort next) && next != current)
            {
                current = next;
                guard++;
            }

            return current;
        }

        public static ushort ResolveAbilityEntry(ushort entry)
        {
            return ResolveAlias(entry, AbilityEntryAliases);
        }

        public static ushort ResolveBuffEntry(ushort entry)
        {
            ushort resolved = ResolveAlias(entry, BuffEntryAliases);
            if (resolved != entry)
                return resolved;

            if (BuffInfos.ContainsKey(entry))
                return entry;

            ushort resolvedAbility = ResolveAbilityEntry(entry);
            if (resolvedAbility != entry && BuffInfos.ContainsKey(resolvedAbility))
                return resolvedAbility;

            if (NewAbilityVolatiles.TryGetValue(entry, out AbilityInfo ability))
            {
                ushort effectEntry = ability.ConstantInfo?.EffectID ?? 0;
                if (effectEntry != 0 && BuffInfos.ContainsKey(effectEntry))
                    return effectEntry;
            }

            return entry;
        }

        #region Accessors

        public static bool HasPreCastModifiers(ushort entry)
        {
            if (AbilityPreCastModifiers.ContainsKey(entry))
                return true;

            return AbilityPreCastModifiers.ContainsKey(ResolveAbilityEntry(entry));
        }

        public static List<AbilityModifier> GetAbilityPreCastModifiers(ushort entry)
        {
            if (AbilityPreCastModifiers.ContainsKey(entry))
                return AbilityPreCastModifiers[entry];

            ushort resolvedEntry = ResolveAbilityEntry(entry);
            return AbilityPreCastModifiers.ContainsKey(resolvedEntry) ? AbilityPreCastModifiers[resolvedEntry] : null;
        }

        public static bool HasModifiers(ushort entry)
        {
            if (AbilityModifiers.ContainsKey(entry))
                return true;

            return AbilityModifiers.ContainsKey(ResolveAbilityEntry(entry));
        }

        public static List<AbilityModifier> GetAbilityModifiers(ushort entry)
        {
            if (AbilityModifiers.ContainsKey(entry))
                return AbilityModifiers[entry];

            ushort resolvedEntry = ResolveAbilityEntry(entry);
            return AbilityModifiers.ContainsKey(resolvedEntry) ? AbilityModifiers[resolvedEntry] : null;
        }

        public static List<AbilityModifier> GetAbilityDelayedModifiers(ushort entry)
        {
            if (AbilityDelayedModifiers.ContainsKey(entry))
                return AbilityDelayedModifiers[entry];

            ushort resolvedEntry = ResolveAbilityEntry(entry);
            return AbilityDelayedModifiers.ContainsKey(resolvedEntry) ? AbilityDelayedModifiers[resolvedEntry] : null;
        }

        public static List<AbilityInfo> GetAvailableCareerAbilities(byte careerLine, int minRank, int maxRank)
        {
            List<AbilityInfo> charAbilities = new List<AbilityInfo>();

            foreach (AbilityInfo ab in CareerAbilities[careerLine - 1])
            {
                if (ab.ConstantInfo.MasteryTree > 0 && ab.ConstantInfo.PointCost > 0)
                    continue;

                if (ab.ConstantInfo.MinimumRank < minRank || ab.ConstantInfo.MinimumRank > maxRank)
                    continue;

                charAbilities.Add(ab);
            }

            return charAbilities;
        }

        public static List<AbilityInfo> GetMasteryAbilities(byte careerLine)
        {
            List<AbilityInfo> masteryAbilities = new List<AbilityInfo>();

            foreach (AbilityInfo ab in CareerAbilities[careerLine - 1])
            {
                if (ab.ConstantInfo.MasteryTree > 0 && ab.ConstantInfo.PointCost > 0)
                    masteryAbilities.Add(ab);
            }

            return masteryAbilities;
        }

        public static AbilityInfo GetAbilityInfo(ushort entry)
        {
            if (NewAbilityVolatiles.TryGetValue(entry, out AbilityInfo directInfo))
                return directInfo.Clone();

            ushort resolvedEntry = ResolveAbilityEntry(entry);
            if (resolvedEntry != entry && NewAbilityVolatiles.TryGetValue(resolvedEntry, out AbilityInfo resolvedInfo))
                return resolvedInfo.Clone();

            return null;
        }

        public static string GetAbilityNameFor(ushort abilityEntry)
        {
            if (NewAbilityVolatiles.ContainsKey(abilityEntry))
                return NewAbilityVolatiles[abilityEntry].Name;

            ushort resolvedAbility = ResolveAbilityEntry(abilityEntry);
            if (resolvedAbility != abilityEntry && NewAbilityVolatiles.ContainsKey(resolvedAbility))
                return NewAbilityVolatiles[resolvedAbility].Name;

            ushort resolvedBuff = ResolveBuffEntry(abilityEntry);
            if (BuffInfos.ContainsKey(resolvedBuff))
                return BuffInfos[resolvedBuff].Name;

            return "attack";
        }

        public static byte GetMasteryTreeFor(ushort entry)
        {
            ushort resolvedEntry = ResolveAbilityEntry(entry);
            if (NewAbilityVolatiles.ContainsKey(resolvedEntry))
                return NewAbilityVolatiles[resolvedEntry].ConstantInfo.MasteryTree;
            return 0;
        }

        public static ushort GetCooldownFor(ushort entry)
        {
            ushort resolvedEntry = ResolveAbilityEntry(entry);
            if (NewAbilityVolatiles.ContainsKey(resolvedEntry))
                return NewAbilityVolatiles[resolvedEntry].Cooldown;
            return 0;
        }

        public static AbilityDamageInfo GetExtraDamageFor(ushort entry, byte id, byte index)
        {
            ushort resolvedEntry = ResolveAbilityEntry(entry);
            try
            {
                return ExtraDamage[resolvedEntry][id][index].Clone();
            }
            catch (Exception)
            {
                Log.Error("AbilityMgr", "Couldn't get damage info for Entry " + entry + " (resolved " + resolvedEntry + ") ID " + id + " Index " + index);
                return null;
            }
        }

        public static bool RequiresResource(ushort entry)
        {
            ushort resolvedEntry = ResolveAbilityEntry(entry);
            if (!NewAbilityVolatiles.ContainsKey(resolvedEntry))
                return false;

            return NewAbilityVolatiles[resolvedEntry].SpecialCost > 0;
        }

        public static AbilityKnockbackInfo GetKnockbackInfo(ushort entry, int id)
        {
            ushort resolvedEntry = ResolveAbilityEntry(entry);
            return KnockbackInfos[resolvedEntry][id];
        }

        public static bool HasCommandsFor(ushort abilityEntry)
        {
            if (AbilityCommandInfos.ContainsKey(abilityEntry))
                return true;

            return AbilityCommandInfos.ContainsKey(ResolveAbilityEntry(abilityEntry));
        }

        public static void GetCommandsFor(Unit caster, AbilityInfo abInfo)
        {
            ushort commandEntry = AbilityCommandInfos.ContainsKey(abInfo.Entry)
                ? abInfo.Entry
                : ResolveAbilityEntry(abInfo.Entry);

            if (AbilityCommandInfos.ContainsKey(commandEntry))
            {
                // Add commands to the new info if they're applicable to the player.
                // Has to be done here because of bloody tactics and career crap
                foreach (AbilityCommandInfo abCommand in AbilityCommandInfos[commandEntry])
                {
                    if (!abCommand.NoAutoUse)
                    {
                        abInfo.CommandInfo.Add(abCommand.Clone(caster));

                        for (AbilityCommandInfo slaveCommand = abCommand.NextCommand; slaveCommand != null; slaveCommand = slaveCommand.NextCommand)
                        {
                            if (!slaveCommand.NoAutoUse)
                                abInfo.CommandInfo[(byte)(abInfo.CommandInfo.Count - 1)].AddCommandToChain(slaveCommand.Clone(caster));
                        }
                    }
                }
            }
        }

        public static AbilityCommandInfo GetAbilityCommand(Unit caster, ushort entry, byte comIndex)
        {
            ushort resolvedEntry = AbilityCommandInfos.ContainsKey(entry) ? entry : ResolveAbilityEntry(entry);
            return AbilityCommandInfos[resolvedEntry][comIndex].Clone(caster);
        }

        public static AbilityCommandInfo GetAbilityCommand(Unit caster, ushort entry, byte comIndex, byte comSeq)
        {
            ushort resolvedEntry = AbilityCommandInfos.ContainsKey(entry) ? entry : ResolveAbilityEntry(entry);
            return AbilityCommandInfos[resolvedEntry][comIndex].GetSubcommand(comSeq).Clone(caster);
        }

        #endregion Accessors

        #region Buff Interface

        public static bool HasBuffModifiers(ushort entry)
        {
            if (BuffModifiers.ContainsKey(entry))
                return true;

            return BuffModifiers.ContainsKey(ResolveBuffEntry(entry));
        }

        public static List<AbilityModifier> GetBuffModifiers(ushort entry)
        {
            if (BuffModifiers.ContainsKey(entry))
                return BuffModifiers[entry];

            ushort resolvedEntry = ResolveBuffEntry(entry);
            return BuffModifiers.ContainsKey(resolvedEntry) ? BuffModifiers[resolvedEntry] : null;
        }

        public static BuffInfo GetBuffInfo(ushort entry)
        {
            ushort resolvedEntry = ResolveBuffEntry(entry);
            if (BuffInfos.ContainsKey(resolvedEntry))
                return BuffInfos[resolvedEntry].Clone();
            Log.Error("GetBuffInfo(entry)", $"Nonexistent buff: {entry}");
            return null;
        }

        public static BuffInfo GetBuffInfo(ushort entry, Unit caster, Unit target)
        {
            ushort resolvedEntry = ResolveBuffEntry(entry);
            if (BuffInfos.ContainsKey(resolvedEntry))
            {
                BuffInfo buffInfo = BuffInfos[resolvedEntry].Clone();

                List<AbilityModifier> myModifiers = GetBuffModifiers(resolvedEntry);
                if (myModifiers != null)
                {
                    foreach (var modifier in myModifiers)
                        modifier.ModifyBuff(caster, target, buffInfo);
                }

                if (caster is Player)
                    ((Player)caster).TacInterface.ModifyBuff(buffInfo, target);

                return buffInfo;
            }
            Log.Error("GetBuffInfo(entry, caster, target)", $"Nonexistent buff: {entry}");
            return null;
        }

        public static BuffCommandInfo GetBuffCommand(ushort entry, byte commandIndex)
        {
            ushort resolvedEntry = BuffCommandInfos.ContainsKey(entry) ? entry : ResolveBuffEntry(entry);
            return BuffCommandInfos[resolvedEntry][commandIndex].CloneChain();
        }

        public static BuffCommandInfo GetBuffCommand(ushort entry, byte commandIndex, byte comSeq)
        {
            ushort resolvedEntry = BuffCommandInfos.ContainsKey(entry) ? entry : ResolveBuffEntry(entry);
            return BuffCommandInfos[resolvedEntry][commandIndex].GetSubcommand(comSeq).CloneChain();
        }

        #endregion Buff Interface
    }
}
