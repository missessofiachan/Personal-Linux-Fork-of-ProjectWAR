using Common;
using FrameWork;
using GameData;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldServer.World.Abilities.Components;

namespace WorldServer.World.Abilities
{
    public sealed class MythicAbilityGraphTranslationReport
    {
        public string DataSource;
        public string Note;

        public int AbilityRows;
        public int AbilityBinRows;
        public int ComponentBinRows;
        public int ComponentLinkRows;
        public int ExpressionRows;
        public int RequirementRows;
        public int UpgradeBinRows;
        public int UpgradeEntryRows;
        public int ItemAbilityRows;

        public int ParsedAbilityRows;
        public int ParsedComponents;
        public int UnsupportedComponents;
        public int FailedComponentPayloads;

        public int AddedAbilityCommands;
        public int AddedAbilityDamageRows;
        public int AddedBuffInfos;
        public int AddedBuffCommands;
        public int AddedBuffDamageRows;

        public int SkippedExistingAbilityEntries;
        public int SkippedUnknownAbilityEntries;
        public int SkippedEmptyComponentRows;

        public string ToLogLine()
        {
            return
                $"Mythic graph source={DataSource}, ability_rows={AbilityRows}, abilitybin_rows={AbilityBinRows}, " +
                $"componentbin_rows={ComponentBinRows}, component_links={ComponentLinkRows}, expressions={ExpressionRows}, requirements={RequirementRows}, " +
                $"upgrades={UpgradeBinRows}/{UpgradeEntryRows}, itemability_rows={ItemAbilityRows}, parsed_abilities={ParsedAbilityRows}, parsed_components={ParsedComponents}, " +
                $"added_commands={AddedAbilityCommands}, added_damage={AddedAbilityDamageRows}, added_buffinfos={AddedBuffInfos}, added_buffcmds={AddedBuffCommands}, " +
                $"added_buffdamage={AddedBuffDamageRows}, unsupported_components={UnsupportedComponents}, failed_component_payloads={FailedComponentPayloads}, " +
                $"skipped_existing_entries={SkippedExistingAbilityEntries}, skipped_unknown_entries={SkippedUnknownAbilityEntries}, skipped_empty_rows={SkippedEmptyComponentRows}.";
        }
    }

    internal sealed class MythicAbilityGraphTranslator
    {
        private readonly IObjectDatabase _db;
        private readonly bool _allowOverride;

        public MythicAbilityGraphTranslator(IObjectDatabase db, bool allowOverride)
        {
            _db = db;
            _allowOverride = allowOverride;
        }

        public bool TryTranslate(
            ISet<ushort> knownAbilityEntries,
            List<AbilityCommandInfo> abilityCommands,
            List<AbilityDamageInfo> abilityDamageInfos,
            List<BuffInfo> buffInfos,
            List<BuffCommandInfo> buffCommands,
            out MythicAbilityGraphTranslationReport report)
        {
            report = new MythicAbilityGraphTranslationReport();

            if (_db == null)
            {
                report.Note = "Database handle was null.";
                return false;
            }

            if (knownAbilityEntries == null || knownAbilityEntries.Count == 0)
            {
                report.Note = "No known ability entries were provided.";
                return false;
            }

            List<MythicBinAbilityRow> abilityRows;
            List<MythicBinAbilityBinRow> abilityBinRows;
            List<MythicBinAbilityComponentBinRow> componentBinRows;
            List<MythicBinAbilityComponentLinkRow> componentLinkRows;
            List<MythicBinAbilityExpressionRow> expressionRows;
            List<MythicBinAbilityRequirementBinRow> requirementRows;
            List<MythicBinAbilityUpgradeBinRow> upgradeBinRows;
            List<MythicBinAbilityUpgradeEntryRow> upgradeEntryRows;
            List<MythicBinItemAbilityRow> itemAbilityRows;

            if (!TryLoadGraphRows(
                report,
                out abilityRows,
                out abilityBinRows,
                out componentBinRows,
                out componentLinkRows,
                out expressionRows,
                out requirementRows,
                out upgradeBinRows,
                out upgradeEntryRows,
                out itemAbilityRows))
            {
                return false;
            }

            report.AbilityRows = abilityRows.Count;
            report.AbilityBinRows = abilityBinRows.Count;
            report.ComponentBinRows = componentBinRows.Count;
            report.ComponentLinkRows = componentLinkRows.Count;
            report.ExpressionRows = expressionRows.Count;
            report.RequirementRows = requirementRows.Count;
            report.UpgradeBinRows = upgradeBinRows.Count;
            report.UpgradeEntryRows = upgradeEntryRows.Count;
            report.ItemAbilityRows = itemAbilityRows.Count;

            if (abilityRows.Count == 0)
            {
                report.Note = "No ability rows were available in mythic graph tables.";
                return false;
            }

            HashSet<ushort> existingAbilityCommandEntries = new HashSet<ushort>(abilityCommands.Select(x => x.Entry));
            HashSet<ushort> existingBuffCommandEntries = new HashSet<ushort>(buffCommands.Select(x => x.Entry));
            Dictionary<ushort, BuffInfo> buffInfoMap = BuildBuffInfoMap(buffInfos);

            foreach (MythicBinAbilityRow abilityRow in abilityRows.OrderBy(x => x.ID))
            {
                if (abilityRow.ID <= 0 || abilityRow.ID > ushort.MaxValue)
                    continue;

                ushort entry = (ushort)abilityRow.ID;

                if (!knownAbilityEntries.Contains(entry))
                {
                    report.SkippedUnknownAbilityEntries++;
                    continue;
                }

                if (!_allowOverride && existingAbilityCommandEntries.Contains(entry))
                {
                    report.SkippedExistingAbilityEntries++;
                    continue;
                }

                if (_allowOverride && existingAbilityCommandEntries.Contains(entry))
                {
                    RemoveAbilityEntryRows(entry, abilityCommands, abilityDamageInfos);
                    existingAbilityCommandEntries.Remove(entry);
                }

                int parseFailures = 0;
                List<MythicGraphComponent> components = ParseComponentPayload(abilityRow.MythicComponentData, ref parseFailures);
                report.FailedComponentPayloads += parseFailures;

                if (components.Count == 0)
                {
                    report.SkippedEmptyComponentRows++;
                    continue;
                }

                report.ParsedAbilityRows++;

                int nextAbilityCommandId = 0;
                int nextBuffCommandId = 0;

                foreach (MythicGraphComponent component in components)
                {
                    report.ParsedComponents++;

                    if (TryTranslateComponent(
                        abilityRow,
                        component,
                        ref nextAbilityCommandId,
                        ref nextBuffCommandId,
                        abilityCommands,
                        abilityDamageInfos,
                        buffInfos,
                        buffCommands,
                        buffInfoMap,
                        existingBuffCommandEntries,
                        report))
                    {
                        existingAbilityCommandEntries.Add(entry);
                    }
                    else
                    {
                        report.UnsupportedComponents++;
                    }
                }
            }

            return true;
        }

        private bool TryLoadGraphRows(
            MythicAbilityGraphTranslationReport report,
            out List<MythicBinAbilityRow> abilityRows,
            out List<MythicBinAbilityBinRow> abilityBinRows,
            out List<MythicBinAbilityComponentBinRow> componentBinRows,
            out List<MythicBinAbilityComponentLinkRow> componentLinkRows,
            out List<MythicBinAbilityExpressionRow> expressionRows,
            out List<MythicBinAbilityRequirementBinRow> requirementRows,
            out List<MythicBinAbilityUpgradeBinRow> upgradeBinRows,
            out List<MythicBinAbilityUpgradeEntryRow> upgradeEntryRows,
            out List<MythicBinItemAbilityRow> itemAbilityRows)
        {
            abilityRows = new List<MythicBinAbilityRow>();
            abilityBinRows = new List<MythicBinAbilityBinRow>();
            componentBinRows = new List<MythicBinAbilityComponentBinRow>();
            componentLinkRows = new List<MythicBinAbilityComponentLinkRow>();
            expressionRows = new List<MythicBinAbilityExpressionRow>();
            requirementRows = new List<MythicBinAbilityRequirementBinRow>();
            upgradeBinRows = new List<MythicBinAbilityUpgradeBinRow>();
            upgradeEntryRows = new List<MythicBinAbilityUpgradeEntryRow>();
            itemAbilityRows = new List<MythicBinItemAbilityRow>();

            IList<MythicBinAbilityRow> mythicRows = TrySelectAll<MythicBinAbilityRow>();
            if (mythicRows != null && mythicRows.Count > 0)
            {
                report.DataSource = "mythic_bin_*";
                abilityRows = mythicRows.ToList();
                abilityBinRows = ToBaseList<MythicBinAbilityBinRow>(TrySelectAll<MythicBinAbilityBinRow>());
                componentBinRows = ToBaseList<MythicBinAbilityComponentBinRow>(TrySelectAll<MythicBinAbilityComponentBinRow>());
                componentLinkRows = ToBaseList<MythicBinAbilityComponentLinkRow>(TrySelectAll<MythicBinAbilityComponentLinkRow>());
                expressionRows = ToBaseList<MythicBinAbilityExpressionRow>(TrySelectAll<MythicBinAbilityExpressionRow>());
                requirementRows = ToBaseList<MythicBinAbilityRequirementBinRow>(TrySelectAll<MythicBinAbilityRequirementBinRow>());
                upgradeBinRows = ToBaseList<MythicBinAbilityUpgradeBinRow>(TrySelectAll<MythicBinAbilityUpgradeBinRow>());
                upgradeEntryRows = ToBaseList<MythicBinAbilityUpgradeEntryRow>(TrySelectAll<MythicBinAbilityUpgradeEntryRow>());
                itemAbilityRows = ToBaseList<MythicBinItemAbilityRow>(TrySelectAll<MythicBinItemAbilityRow>());
                return true;
            }

            IList<LondoAbilityRow> londoRows = TrySelectAll<LondoAbilityRow>();
            if (londoRows == null || londoRows.Count == 0)
            {
                report.DataSource = "none";
                report.Note = "Neither mythic_bin_ability nor Ability tables were available with data.";
                return false;
            }

            report.DataSource = "Londo Ability* fallback";
            abilityRows = londoRows.Cast<MythicBinAbilityRow>().ToList();
            abilityBinRows = ToBaseList<MythicBinAbilityBinRow, LondoAbilityBinRow>(TrySelectAll<LondoAbilityBinRow>());
            componentBinRows = ToBaseList<MythicBinAbilityComponentBinRow, LondoAbilityComponentBinRow>(TrySelectAll<LondoAbilityComponentBinRow>());
            componentLinkRows = ToBaseList<MythicBinAbilityComponentLinkRow, LondoAbilityComponentLinkRow>(TrySelectAll<LondoAbilityComponentLinkRow>());
            expressionRows = ToBaseList<MythicBinAbilityExpressionRow, LondoAbilityExpressionRow>(TrySelectAll<LondoAbilityExpressionRow>());
            requirementRows = ToBaseList<MythicBinAbilityRequirementBinRow, LondoAbilityRequirementBinRow>(TrySelectAll<LondoAbilityRequirementBinRow>());
            upgradeBinRows = ToBaseList<MythicBinAbilityUpgradeBinRow, LondoAbilityUpgradeBinRow>(TrySelectAll<LondoAbilityUpgradeBinRow>());
            upgradeEntryRows = ToBaseList<MythicBinAbilityUpgradeEntryRow, LondoAbilityUpgradeEntryRow>(TrySelectAll<LondoAbilityUpgradeEntryRow>());
            itemAbilityRows = ToBaseList<MythicBinItemAbilityRow, LondoItemAbilityRow>(TrySelectAll<LondoItemAbilityRow>());
            return true;
        }

        private static Dictionary<ushort, BuffInfo> BuildBuffInfoMap(List<BuffInfo> buffInfos)
        {
            Dictionary<ushort, BuffInfo> map = new Dictionary<ushort, BuffInfo>();
            foreach (BuffInfo buffInfo in buffInfos)
            {
                if (!map.ContainsKey(buffInfo.Entry))
                    map.Add(buffInfo.Entry, buffInfo);
                else
                    map[buffInfo.Entry] = buffInfo;
            }

            return map;
        }

        private bool TryTranslateComponent(
            MythicBinAbilityRow abilityRow,
            MythicGraphComponent component,
            ref int nextAbilityCommandId,
            ref int nextBuffCommandId,
            List<AbilityCommandInfo> abilityCommands,
            List<AbilityDamageInfo> abilityDamageInfos,
            List<BuffInfo> buffInfos,
            List<BuffCommandInfo> buffCommands,
            Dictionary<ushort, BuffInfo> buffInfoMap,
            HashSet<ushort> existingBuffCommandEntries,
            MythicAbilityGraphTranslationReport report)
        {
            switch (component.Type)
            {
                case 1:
                case 3:
                    return TranslateDamageLikeComponent(
                        abilityRow,
                        component,
                        ref nextAbilityCommandId,
                        ref nextBuffCommandId,
                        abilityCommands,
                        abilityDamageInfos,
                        buffInfos,
                        buffCommands,
                        buffInfoMap,
                        existingBuffCommandEntries,
                        report);

                case 8:
                    return TranslateModifySpeedComponent(
                        abilityRow,
                        component,
                        ref nextAbilityCommandId,
                        ref nextBuffCommandId,
                        abilityCommands,
                        buffInfos,
                        buffCommands,
                        buffInfoMap,
                        abilityDamageInfos,
                        existingBuffCommandEntries,
                        report);

                case 22:
                    return TranslateStatOrShieldComponent(
                        abilityRow,
                        component,
                        ref nextAbilityCommandId,
                        ref nextBuffCommandId,
                        abilityCommands,
                        buffInfos,
                        buffCommands,
                        buffInfoMap,
                        abilityDamageInfos,
                        existingBuffCommandEntries,
                        report);

                case 23:
                    return TranslateInvokeBuffComponent(
                        abilityRow,
                        component,
                        ref nextAbilityCommandId,
                        abilityCommands,
                        report);

                default:
                    return false;
            }
        }

        private bool TranslateDamageLikeComponent(
            MythicBinAbilityRow abilityRow,
            MythicGraphComponent component,
            ref int nextAbilityCommandId,
            ref int nextBuffCommandId,
            List<AbilityCommandInfo> abilityCommands,
            List<AbilityDamageInfo> abilityDamageInfos,
            List<BuffInfo> buffInfos,
            List<BuffCommandInfo> buffCommands,
            Dictionary<ushort, BuffInfo> buffInfoMap,
            HashSet<ushort> existingBuffCommandEntries,
            MythicAbilityGraphTranslationReport report)
        {
            ushort entry = (ushort)abilityRow.ID;
            bool periodic = component.DurationMs > 0 && component.IntervalMs > 0;

            if (periodic)
            {
                if (!TryCreateOrReusePeriodicDamageBuff(
                    abilityRow,
                    component,
                    ref nextBuffCommandId,
                    buffInfos,
                    buffCommands,
                    abilityDamageInfos,
                    buffInfoMap,
                    existingBuffCommandEntries,
                    report))
                {
                    return false;
                }

                AbilityCommandInfo invokeBuff = BuildAbilityCommand(
                    entry,
                    abilityRow.Name,
                    nextAbilityCommandId++,
                    "InvokeBuff",
                    entry,
                    0,
                    component,
                    ResolveCommandTargetType(abilityRow, component, false));

                abilityCommands.Add(invokeBuff);
                report.AddedAbilityCommands++;
                return true;
            }

            int scaledBaseValue = ResolveScaledValue(component, 0);
            if (scaledBaseValue <= 0)
                return false;

            DamageTypes damageType = ResolveDamageType(abilityRow, component.Type);
            bool noLevelScaling = component.NoLevelScaling;

            AbilityCommandInfo direct = BuildAbilityCommand(
                entry,
                abilityRow.Name,
                nextAbilityCommandId++,
                "DealDamage",
                0,
                0,
                component,
                ResolveCommandTargetType(abilityRow, component, true));

            AbilityDamageInfo damageInfo = new AbilityDamageInfo
            {
                Entry = entry,
                DisplayEntry = entry,
                Index = 0,
                ParentCommandID = direct.CommandID,
                ParentCommandSequence = direct.CommandSequence,
                MinDamage = ToUShortNonNegative(scaledBaseValue),
                MaxDamage = noLevelScaling ? ToUShortNonNegative(scaledBaseValue) : (ushort)0,
                DamageType = damageType,
                DamageVariance = 0,
                CastTimeDamageMult = 1f,
                WeaponMod = WeaponDamageContribution.None,
                WeaponDamageScale = 0,
                NoCrits = false,
                Undefendable = false,
                OverrideDefenseEvent = 0,
                StatUsed = 0,
                StatDamageScale = 0,
                ArmorResistPenFactor = 0,
                HatredScale = 1f,
                HealHatredScale = 1f,
                ResourceBuild = 0,
                CastPlayerSubID = 0,
                PriStatMultiplier = 0
            };

            abilityCommands.Add(direct);
            abilityDamageInfos.Add(damageInfo);
            report.AddedAbilityCommands++;
            report.AddedAbilityDamageRows++;
            return true;
        }

        private bool TryCreateOrReusePeriodicDamageBuff(
            MythicBinAbilityRow abilityRow,
            MythicGraphComponent component,
            ref int nextBuffCommandId,
            List<BuffInfo> buffInfos,
            List<BuffCommandInfo> buffCommands,
            List<AbilityDamageInfo> abilityDamageInfos,
            Dictionary<ushort, BuffInfo> buffInfoMap,
            HashSet<ushort> existingBuffCommandEntries,
            MythicAbilityGraphTranslationReport report)
        {
            ushort entry = (ushort)abilityRow.ID;
            bool isDebuff = IsTrue(abilityRow.IsDebuff);

            if (_allowOverride && existingBuffCommandEntries.Contains(entry))
            {
                RemoveBuffEntryRows(entry, buffInfos, buffInfoMap, buffCommands, abilityDamageInfos);
                existingBuffCommandEntries.Remove(entry);
                nextBuffCommandId = 0;
            }
            else if (existingBuffCommandEntries.Contains(entry))
            {
                return true;
            }

            BuffInfo buff = EnsureBuffInfo(entry, abilityRow.Name, component, isDebuff, buffInfos, buffInfoMap, report);

            int scaledBaseValue = ResolveScaledValue(component, 0);
            if (scaledBaseValue <= 0)
                return false;

            BuffCommandInfo buffCommand = BuildBuffCommand(
                entry,
                buff.Name,
                nextBuffCommandId++,
                "DamageOverTime",
                0,
                0,
                component,
                7);

            DamageTypes damageType = ResolveDamageType(abilityRow, component.Type);
            bool noLevelScaling = component.NoLevelScaling;

            AbilityDamageInfo damageInfo = new AbilityDamageInfo
            {
                Entry = entry,
                DisplayEntry = entry,
                Index = 1,
                ParentCommandID = buffCommand.CommandID,
                ParentCommandSequence = buffCommand.CommandSequence,
                MinDamage = ToUShortNonNegative(scaledBaseValue),
                MaxDamage = noLevelScaling ? ToUShortNonNegative(scaledBaseValue) : (ushort)0,
                DamageType = damageType,
                DamageVariance = 0,
                CastTimeDamageMult = 1f,
                WeaponMod = WeaponDamageContribution.None,
                WeaponDamageScale = 0,
                NoCrits = false,
                Undefendable = false,
                OverrideDefenseEvent = 0,
                StatUsed = 0,
                StatDamageScale = 0,
                ArmorResistPenFactor = 0,
                HatredScale = 1f,
                HealHatredScale = 1f,
                ResourceBuild = 0,
                CastPlayerSubID = 0,
                PriStatMultiplier = 0
            };

            buffCommands.Add(buffCommand);
            abilityDamageInfos.Add(damageInfo);
            existingBuffCommandEntries.Add(entry);
            report.AddedBuffCommands++;
            report.AddedBuffDamageRows++;
            return true;
        }

        private bool TranslateModifySpeedComponent(
            MythicBinAbilityRow abilityRow,
            MythicGraphComponent component,
            ref int nextAbilityCommandId,
            ref int nextBuffCommandId,
            List<AbilityCommandInfo> abilityCommands,
            List<BuffInfo> buffInfos,
            List<BuffCommandInfo> buffCommands,
            Dictionary<ushort, BuffInfo> buffInfoMap,
            List<AbilityDamageInfo> abilityDamageInfos,
            HashSet<ushort> existingBuffCommandEntries,
            MythicAbilityGraphTranslationReport report)
        {
            ushort entry = (ushort)abilityRow.ID;

            if (_allowOverride && existingBuffCommandEntries.Contains(entry))
            {
                RemoveBuffEntryRows(entry, buffInfos, buffInfoMap, buffCommands, abilityDamageInfos);
                existingBuffCommandEntries.Remove(entry);
                nextBuffCommandId = 0;
            }
            else if (existingBuffCommandEntries.Contains(entry))
            {
                return true;
            }

            int speedPrimary = ResolveScaledValue(component, 0);
            int speedSecondary = ResolveScaledValue(component, 1);

            if (speedPrimary == 0 && component.Values.Length > 0)
                speedPrimary = component.Values[0];

            if (speedSecondary == 0 && component.Values.Length > 1)
                speedSecondary = component.Values[1];

            BuffInfo buff = EnsureBuffInfo(
                entry,
                abilityRow.Name,
                component,
                IsTrue(abilityRow.IsDebuff),
                buffInfos,
                buffInfoMap,
                report);

            BuffCommandInfo buffCommand = BuildBuffCommand(
                entry,
                buff.Name,
                nextBuffCommandId++,
                "ModifySpeed",
                speedPrimary,
                speedSecondary,
                component,
                5);

            buffCommands.Add(buffCommand);
            existingBuffCommandEntries.Add(entry);
            report.AddedBuffCommands++;

            AbilityCommandInfo invokeBuff = BuildAbilityCommand(
                entry,
                abilityRow.Name,
                nextAbilityCommandId++,
                "InvokeBuff",
                entry,
                0,
                component,
                ResolveCommandTargetType(abilityRow, component, false));

            abilityCommands.Add(invokeBuff);
            report.AddedAbilityCommands++;
            return true;
        }

        private bool TranslateStatOrShieldComponent(
            MythicBinAbilityRow abilityRow,
            MythicGraphComponent component,
            ref int nextAbilityCommandId,
            ref int nextBuffCommandId,
            List<AbilityCommandInfo> abilityCommands,
            List<BuffInfo> buffInfos,
            List<BuffCommandInfo> buffCommands,
            Dictionary<ushort, BuffInfo> buffInfoMap,
            List<AbilityDamageInfo> abilityDamageInfos,
            HashSet<ushort> existingBuffCommandEntries,
            MythicAbilityGraphTranslationReport report)
        {
            ushort entry = (ushort)abilityRow.ID;

            if (_allowOverride && existingBuffCommandEntries.Contains(entry))
            {
                RemoveBuffEntryRows(entry, buffInfos, buffInfoMap, buffCommands, abilityDamageInfos);
                existingBuffCommandEntries.Remove(entry);
                nextBuffCommandId = 0;
            }
            else if (existingBuffCommandEntries.Contains(entry))
            {
                return true;
            }

            if (component.Values.Length == 0)
                return false;

            int statId = component.Values[0];
            string commandName;
            int primary;
            int secondary;

            if (statId == (int)Stats.DamageAbsorb)
            {
                commandName = "Shield";

                int rawShield = component.Values.Length > 1 ? component.Values[1] : component.Values[0];
                int shieldAmount = ResolveScaledValue(component, component.Values.Length > 1 ? 1 : 0);

                if (shieldAmount <= 0)
                    shieldAmount = rawShield;

                if (shieldAmount <= 0)
                    return false;

                primary = shieldAmount;
                secondary = shieldAmount;
            }
            else
            {
                if (statId <= 0 || statId >= (int)Stats.MaxStatCount)
                    return false;

                commandName = "ModifyStat";
                primary = statId;
                secondary = ResolveScaledValue(component, component.Values.Length > 1 ? 1 : 0);
                if (secondary == 0 && component.Values.Length > 1)
                    secondary = component.Values[1];
            }

            BuffInfo buff = EnsureBuffInfo(
                entry,
                abilityRow.Name,
                component,
                IsTrue(abilityRow.IsDebuff),
                buffInfos,
                buffInfoMap,
                report);

            BuffCommandInfo buffCommand = BuildBuffCommand(
                entry,
                buff.Name,
                nextBuffCommandId++,
                commandName,
                primary,
                secondary,
                component,
                5);

            buffCommands.Add(buffCommand);
            existingBuffCommandEntries.Add(entry);
            report.AddedBuffCommands++;

            AbilityCommandInfo invokeBuff = BuildAbilityCommand(
                entry,
                abilityRow.Name,
                nextAbilityCommandId++,
                "InvokeBuff",
                entry,
                0,
                component,
                ResolveCommandTargetType(abilityRow, component, false));

            abilityCommands.Add(invokeBuff);
            report.AddedAbilityCommands++;
            return true;
        }

        private bool TranslateInvokeBuffComponent(
            MythicBinAbilityRow abilityRow,
            MythicGraphComponent component,
            ref int nextAbilityCommandId,
            List<AbilityCommandInfo> abilityCommands,
            MythicAbilityGraphTranslationReport report)
        {
            if (component.Values.Length == 0)
                return false;

            int linkedBuffEntry = component.Values[0];
            if (linkedBuffEntry <= 0 || linkedBuffEntry > ushort.MaxValue)
                return false;

            ushort entry = (ushort)abilityRow.ID;
            AbilityCommandInfo command = BuildAbilityCommand(
                entry,
                abilityRow.Name,
                nextAbilityCommandId++,
                "InvokeBuff",
                linkedBuffEntry,
                0,
                component,
                ResolveCommandTargetType(abilityRow, component, false));

            abilityCommands.Add(command);
            report.AddedAbilityCommands++;
            return true;
        }

        private static AbilityCommandInfo BuildAbilityCommand(
            ushort entry,
            string abilityName,
            int commandId,
            string commandName,
            int primaryValue,
            int secondaryValue,
            MythicGraphComponent component,
            CommandTargetTypes targetType)
        {
            return new AbilityCommandInfo
            {
                Entry = entry,
                AbilityName = abilityName ?? string.Empty,
                CommandID = ToByte(commandId),
                CommandSequence = 0,
                CommandName = commandName,
                PrimaryValue = primaryValue,
                SecondaryValue = secondaryValue,
                EffectRadius = ToEffectRadiusFeet(component.RadiusRaw),
                EffectAngle = ToByte(component.ConeAngle),
                TargetType = targetType,
                AoESource = CommandTargetTypes.Last,
                MaxTargets = ToByte(component.MaxTargets),
                AttackingStat = 0,
                IsDelayedEffect = component.ActivationDelayMs > 0,
                FromAllTargets = false,
                NoAutoUse = false
            };
        }

        private static BuffCommandInfo BuildBuffCommand(
            ushort entry,
            string buffName,
            int commandId,
            string commandName,
            int primaryValue,
            int secondaryValue,
            MythicGraphComponent component,
            byte invokeOn)
        {
            return new BuffCommandInfo
            {
                Entry = entry,
                Name = buffName ?? string.Empty,
                CommandID = ToByte(commandId),
                CommandSequence = 0,
                CommandName = commandName,
                BuffClass = BuffClass.Standard,
                PrimaryValue = primaryValue,
                SecondaryValue = secondaryValue,
                TertiaryValue = 0,
                InvokeOn = invokeOn,
                EffectRadius = ToEffectRadiusFeet(component.RadiusRaw),
                EffectAngle = (short)Math.Max(short.MinValue, Math.Min(short.MaxValue, component.ConeAngle)),
                TargetType = CommandTargetTypes.Host,
                AoESource = CommandTargetTypes.Last,
                MaxTargets = ToByte(component.MaxTargets),
                EventCheck = string.Empty,
                EventCheckParam = 0,
                EventChance = 0,
                RetriggerInterval = 0,
                ConsumesStack = false,
                BuffLine = 1,
                NoAutoUse = false
            };
        }

        private static BuffInfo EnsureBuffInfo(
            ushort entry,
            string name,
            MythicGraphComponent component,
            bool isDebuff,
            List<BuffInfo> buffInfos,
            Dictionary<ushort, BuffInfo> buffInfoMap,
            MythicAbilityGraphTranslationReport report)
        {
            BuffInfo existing;
            if (buffInfoMap.TryGetValue(entry, out existing))
                return existing;

            uint durationSeconds = 0;
            if (component.DurationMs > 0)
                durationSeconds = (uint)Math.Max(1, (component.DurationMs + 999) / 1000);

            ushort intervalMs = 0;
            if (component.IntervalMs > 0)
                intervalMs = (ushort)Math.Min(ushort.MaxValue, component.IntervalMs);

            BuffInfo buff = new BuffInfo
            {
                Entry = entry,
                Name = string.IsNullOrWhiteSpace(name) ? ("Ability " + entry) : name,
                BuffClass = BuffClass.Standard,
                Type = isDebuff ? BuffTypes.Hex : BuffTypes.None,
                Group = 0,
                AuraPropagation = string.Empty,
                MaxCopies = 0,
                InitialStacks = 1,
                MaxStack = 1,
                StackLine = 1,
                StacksFromCaster = false,
                Duration = durationSeconds,
                LeadInDelay = component.ActivationDelayMs,
                Interval = intervalMs,
                PersistsOnDeath = 0,
                CanRefresh = false,
                FriendlyEffectID = 0,
                EnemyEffectID = 0,
                EffectType = 0,
                MasteryTree = 0,
                CommandInfo = new List<BuffCommandInfo>()
            };

            if (intervalMs > 0 && durationSeconds > 0)
                buff.BuffIntervals = (byte)Math.Max(1, Math.Min(byte.MaxValue, (durationSeconds * 1000) / intervalMs));
            else
                buff.BuffIntervals = 0;

            buffInfos.Add(buff);
            buffInfoMap[entry] = buff;
            report.AddedBuffInfos++;
            return buff;
        }

        private static void RemoveAbilityEntryRows(ushort entry, List<AbilityCommandInfo> abilityCommands, List<AbilityDamageInfo> abilityDamageInfos)
        {
            abilityCommands.RemoveAll(x => x.Entry == entry);
            abilityDamageInfos.RemoveAll(x => x.Entry == entry && x.Index == 0);
        }

        private static void RemoveBuffEntryRows(
            ushort entry,
            List<BuffInfo> buffInfos,
            Dictionary<ushort, BuffInfo> buffInfoMap,
            List<BuffCommandInfo> buffCommands,
            List<AbilityDamageInfo> abilityDamageInfos)
        {
            buffInfos.RemoveAll(x => x.Entry == entry);
            buffInfoMap.Remove(entry);
            buffCommands.RemoveAll(x => x.Entry == entry);
            abilityDamageInfos.RemoveAll(x => x.Entry == entry && x.Index == 1);
        }

        private static CommandTargetTypes ResolveCommandTargetType(MythicBinAbilityRow abilityRow, MythicGraphComponent component, bool isDamageAction)
        {
            switch (component.TargetTypeRaw)
            {
                case 1:
                    return CommandTargetTypes.Enemy;
                case 2:
                    return CommandTargetTypes.Ally;
                case 3:
                    return CommandTargetTypes.AllyOrSelf;
                case 5:
                    return CommandTargetTypes.Caster;
            }

            if (IsTrue(abilityRow.IsDebuff) || IsTrue(abilityRow.IsDamaging))
                return CommandTargetTypes.Enemy;

            if (IsTrue(abilityRow.IsHealing))
                return CommandTargetTypes.AllyOrSelf;

            if (IsTrue(abilityRow.IsBuff))
                return CommandTargetTypes.AllyOrSelf;

            return isDamageAction ? CommandTargetTypes.Enemy : CommandTargetTypes.AllyOrSelf;
        }

        private static DamageTypes ResolveDamageType(MythicBinAbilityRow abilityRow, int componentType)
        {
            if (componentType == 3 || IsTrue(abilityRow.IsHealing))
                return DamageTypes.Healing;

            int spellDamageType = abilityRow.SpellDamageType.HasValue ? abilityRow.SpellDamageType.Value : 0;
            switch (spellDamageType)
            {
                case 1:
                    return DamageTypes.Spiritual;
                case 2:
                    return DamageTypes.Elemental;
                case 3:
                    return DamageTypes.Corporeal;
                case 4:
                    return DamageTypes.Healing;
                default:
                    return DamageTypes.Physical;
            }
        }

        private static bool IsTrue(bool? flag)
        {
            return flag.HasValue && flag.Value;
        }

        private static ushort ToUShortNonNegative(int value)
        {
            if (value <= 0)
                return 0;
            if (value > ushort.MaxValue)
                return ushort.MaxValue;
            return (ushort)value;
        }

        private static byte ToByte(int value)
        {
            if (value <= 0)
                return 0;
            if (value >= byte.MaxValue)
                return byte.MaxValue;
            return (byte)value;
        }

        private static byte ToEffectRadiusFeet(int radiusRaw)
        {
            if (radiusRaw <= 0)
                return 0;

            int radiusFeet = (int)Math.Round(radiusRaw / 10f);
            if (radiusFeet < 0)
                radiusFeet = 0;
            if (radiusFeet > byte.MaxValue)
                radiusFeet = byte.MaxValue;
            return (byte)radiusFeet;
        }

        private static int ResolveScaledValue(MythicGraphComponent component, int valueIndex)
        {
            if (component.Values.Length == 0 || valueIndex < 0 || valueIndex >= component.Values.Length)
                return 0;

            int value = component.Values[valueIndex];
            int multiplier = 100;
            if (component.Multipliers.Length > valueIndex)
                multiplier = component.Multipliers[valueIndex];

            long scaled = (long)value * multiplier;
            scaled /= 100L;

            if (scaled < int.MinValue)
                return int.MinValue;
            if (scaled > int.MaxValue)
                return int.MaxValue;
            return (int)scaled;
        }

        private static List<MythicGraphComponent> ParseComponentPayload(string rawPayload, ref int parseFailures)
        {
            List<MythicGraphComponent> components = new List<MythicGraphComponent>();
            if (string.IsNullOrWhiteSpace(rawPayload))
                return components;

            JToken rootToken;
            try
            {
                rootToken = JToken.Parse(rawPayload);
            }
            catch
            {
                parseFailures++;
                return components;
            }

            JArray array = rootToken as JArray;
            if (array == null)
            {
                parseFailures++;
                return components;
            }

            foreach (JToken token in array)
            {
                JObject wrapper = token as JObject;
                if (wrapper == null)
                {
                    parseFailures++;
                    continue;
                }

                int type = ReadInt(wrapper["Type"], 0);
                JObject payload = ExtractPayload(wrapper["Data"], ref parseFailures);
                if (payload == null)
                    payload = wrapper;

                if (type == 0)
                    type = ReadInt(payload["ComponentAdjustmentType"], 0);

                if (type == 0)
                {
                    parseFailures++;
                    continue;
                }

                MythicGraphComponent component = new MythicGraphComponent
                {
                    Type = type,
                    Index = ReadInt(payload["Index"], components.Count),
                    Values = ParseIntArray(payload["Values"]),
                    Multipliers = ParseIntArray(payload["Multipliers"]),
                    DurationMs = ReadInt(payload["Duration"], 0),
                    IntervalMs = ReadInt(payload["Interval"], 0),
                    ActivationDelayMs = ReadInt(payload["ActivationDelay"], 0),
                    RadiusRaw = ReadInt(payload["Radius"], 0),
                    ConeAngle = ReadInt(payload["ConeAngle"], 0),
                    MaxTargets = ReadInt(payload["MaxTargets"], 0),
                    TargetTypeRaw = ReadInt(payload["TargetType"], 0),
                    NoLevelScaling = ReadBool(payload["NoLevelScaling"], false)
                };

                components.Add(component);
            }

            return components.OrderBy(x => x.Index).ThenBy(x => x.Type).ToList();
        }

        private static JObject ExtractPayload(JToken token, ref int parseFailures)
        {
            if (token == null || token.Type == JTokenType.Null)
                return null;

            if (token.Type == JTokenType.Object)
                return (JObject)token;

            if (token.Type == JTokenType.String)
            {
                string raw = token.Value<string>();
                if (string.IsNullOrWhiteSpace(raw))
                    return null;

                try
                {
                    JToken parsed = JToken.Parse(raw);
                    return parsed as JObject;
                }
                catch
                {
                    parseFailures++;
                    return null;
                }
            }

            return null;
        }

        private static int[] ParseIntArray(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
                return new int[0];

            if (token.Type == JTokenType.Array)
            {
                List<int> values = new List<int>();
                foreach (JToken element in (JArray)token)
                    values.Add(ReadInt(element, 0));
                return values.ToArray();
            }

            if (token.Type == JTokenType.String)
            {
                string raw = token.Value<string>();
                if (string.IsNullOrWhiteSpace(raw))
                    return new int[0];

                string[] split = raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<int> values = new List<int>(split.Length);
                foreach (string part in split)
                {
                    int parsed;
                    if (int.TryParse(part.Trim(), out parsed))
                        values.Add(parsed);
                }
                return values.ToArray();
            }

            return new[] { ReadInt(token, 0) };
        }

        private static int ReadInt(JToken token, int fallback)
        {
            if (token == null || token.Type == JTokenType.Null)
                return fallback;

            if (token.Type == JTokenType.Integer)
                return token.Value<int>();

            if (token.Type == JTokenType.Float)
                return Convert.ToInt32(token.Value<double>());

            if (token.Type == JTokenType.Boolean)
                return token.Value<bool>() ? 1 : 0;

            if (token.Type == JTokenType.String)
            {
                int parsed;
                if (int.TryParse(token.Value<string>(), out parsed))
                    return parsed;
            }

            return fallback;
        }

        private static bool ReadBool(JToken token, bool fallback)
        {
            if (token == null || token.Type == JTokenType.Null)
                return fallback;

            if (token.Type == JTokenType.Boolean)
                return token.Value<bool>();

            if (token.Type == JTokenType.Integer)
                return token.Value<int>() != 0;

            if (token.Type == JTokenType.String)
            {
                bool boolValue;
                if (bool.TryParse(token.Value<string>(), out boolValue))
                    return boolValue;

                int intValue;
                if (int.TryParse(token.Value<string>(), out intValue))
                    return intValue != 0;
            }

            return fallback;
        }

        private IList<T> TrySelectAll<T>() where T : DataObject
        {
            try
            {
                return _db.SelectAllObjects<T>();
            }
            catch
            {
                return null;
            }
        }

        private static List<TBase> ToBaseList<TBase>(IList<TBase> input)
        {
            return input != null ? input.ToList() : new List<TBase>();
        }

        private static List<TBase> ToBaseList<TBase, TDerived>(IList<TDerived> input)
            where TDerived : TBase
        {
            if (input == null)
                return new List<TBase>();
            return input.Cast<TBase>().ToList();
        }

        private sealed class MythicGraphComponent
        {
            public int Type;
            public int Index;
            public int[] Values = new int[0];
            public int[] Multipliers = new int[0];
            public int DurationMs;
            public int IntervalMs;
            public int ActivationDelayMs;
            public int RadiusRaw;
            public int ConeAngle;
            public int MaxTargets;
            public int TargetTypeRaw;
            public bool NoLevelScaling;
        }
    }
}
