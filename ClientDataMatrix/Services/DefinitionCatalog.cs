using ClientDataMatrix.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ClientDataMatrix.Services
{
    public sealed class DefinitionCatalog
    {
        private static readonly Dictionary<uint, string> TacticTypes = new Dictionary<uint, string> { { 0, "CAREER" }, { 1, "RENOWN" }, { 2, "TOME" }, { 3, "FIRST" } };
        private static readonly Dictionary<uint, string> TargetTypes = new Dictionary<uint, string> { { 0, "NONE" }, { 1, "ENEMY" }, { 2, "SELF" }, { 3, "ALLY" }, { 4, "GROUP" }, { 5, "PET" }, { 6, "GROUND" } };
        private static readonly Dictionary<uint, string> AbilityTypes = new Dictionary<uint, string> { { 0, "FIRST" }, { 1, "DEFAULT" }, { 2, "MORALE" }, { 3, "TACTIC" }, { 4, "GRANTED" }, { 5, "PASSIVE" }, { 6, "PET" }, { 7, "GUILD" } };
        private static readonly Dictionary<uint, string> AttackTypes = new Dictionary<uint, string> { { 0, "NONE" }, { 1, "MELEE" }, { 2, "RANGED" }, { 3, "MAGIC" } };
        private static readonly Dictionary<uint, string> TriggerTypes = new Dictionary<uint, string> { { 0, "OnEnd" }, { 1, "OnApply" }, { 2, "OnPreviousComponentEndCast" }, { 3, "OnPreviousComponentApplied" }, { 5, "OnBuffEnded" }, { 6, "OnEventTriggered" }, { 7, "OnPreviousComponentTick" }, { 8, "OnPreviousComponentBuffTick" }, { 10, "OnBuffEndedRemoved" } };
        private static readonly Dictionary<uint, string> ComponentOperations = new Dictionary<uint, string> { { 0, "NONE" }, { 1, "DAMAGE" }, { 2, "STAT_CHANGE" }, { 3, "HEAL" }, { 4, "DAMAGE_CHANGE" }, { 5, "ARMOR_CHANGE" }, { 6, "AP_CHANGE" }, { 7, "HATE" }, { 8, "VELOCITY" }, { 9, "INTERRUPT" }, { 10, "RESSURRECT" }, { 11, "DISPEL_BUFF" }, { 12, "CC" }, { 13, "EVENT_LISTENER" }, { 15, "EFFECT_BUFF" }, { 16, "DEFENSIVE_STAT_CHANGE" }, { 17, "AP_REGEN_CHANGE" }, { 18, "MORALE_REGEN_CHANGE" }, { 19, "MORALE_CHANGE" }, { 20, "COOLDOWN_CHANGE" }, { 21, "CASTTIME_CHANGE" }, { 22, "BONUS_TYPE_ADJUST" }, { 23, "APPLY_ABILITY" }, { 24, "KNOCKBACK" }, { 25, "UPDATE_COUNTER" }, { 26, "MONSTER_CONTROLLER" }, { 28, "GRANTED_ABILITY" }, { 31, "BINDING_TELEPORT" }, { 33, "AUTO_ATTACK_ADJUST" }, { 34, "STEALTH" }, { 35, "SUMMON_MOUNT" }, { 36, "SERVER_COMMAND" }, { 37, "RANK_CHANGE" }, { 38, "IMMUNITY" }, { 39, "MONSTER_FORCE_TARGET" }, { 42, "RECOVER_STANDARD" }, { 44, "DISK_UPDATE" }, { 46, "CATAPULT" } };

        private readonly Dictionary<uint, string> _careerLines;
        private readonly Dictionary<int, string> _effects;
        private readonly Dictionary<ushort, BinaryComponentRecord> _components;
        private readonly Dictionary<string, DefinitionDomain> _domains;
        private readonly ComponentSchemaCatalog _componentSchemas;
        private readonly RequirementCatalog _requirements;

        public DefinitionCatalog(AbilityDataset dataset)
        {
            _careerLines = dataset == null ? new Dictionary<uint, string>() : dataset.CareerLines.GroupBy(row => row.EntryId).ToDictionary(group => group.Key, group => group.Select(row => row.NormalizedValue).FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty);
            _effects = dataset == null ? new Dictionary<int, string>() : dataset.ClientEffects.GroupBy(row => (int)row.EffectId).ToDictionary(group => group.Key, group => group.OrderBy(row => row.LineNumber).Select(row => row.Name).FirstOrDefault() ?? string.Empty);
            _components = dataset == null ? new Dictionary<ushort, BinaryComponentRecord>() : dataset.BinaryComponents.GroupBy(row => row.ComponentId).ToDictionary(group => group.Key, group => group.OrderBy(row => row.RecordIndex).First());
            _componentSchemas = new ComponentSchemaCatalog(dataset);
            _requirements = new RequirementCatalog(dataset);
            _domains = BuildDomains(dataset);
        }

        public List<DefinitionEntry> BuildAbilityDefinitions(AbilityAnalysisResult report)
        {
            List<DefinitionEntry> entries = new List<DefinitionEntry>();
            if (report == null)
                return entries;

            BinaryAbilityRecord abilityRow = report.BinaryAbilityRows.FirstOrDefault();
            if (abilityRow != null)
            {
                AddEntry(entries, "Ability.EffectId", "Ability.EffectId", abilityRow.EffectId.ToString(CultureInfo.InvariantCulture), DescribeEffectId(abilityRow.EffectId), "effects.csv", SemanticConfidence.Confirmed, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                AddEntry(entries, "Ability.TacticType", "Ability.TacticType", abilityRow.TacticType.ToString(CultureInfo.InvariantCulture), DescribeTacticType(abilityRow.TacticType), "Static enum reference", SemanticConfidence.Inferred, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                AddEntry(entries, "Ability.TargetType", "Ability.TargetType", abilityRow.TargetType.ToString(CultureInfo.InvariantCulture), DescribeTargetType(abilityRow.TargetType), "Static enum reference", SemanticConfidence.Inferred, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                AddEntry(entries, "Ability.AbilityType", "Ability.AbilityType", abilityRow.AbilityType.ToString(CultureInfo.InvariantCulture), DescribeAbilityType(abilityRow.AbilityType), "Static enum reference", SemanticConfidence.Inferred, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                AddEntry(entries, "Ability.AttackType", "Ability.AttackType", abilityRow.AttackType.ToString(CultureInfo.InvariantCulture), DescribeAttackType(abilityRow.AttackType), "Static enum reference", SemanticConfidence.Inferred, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                AddEntry(entries, "Ability.CareerLine", "Ability.CareerLine", abilityRow.CareerLine.ToString(CultureInfo.InvariantCulture), DescribeCareerLine(abilityRow.CareerLine), "careerlines_m.txt", SemanticConfidence.Confirmed, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                AddEntry(entries, "Ability.MoraleLevel", "Ability.MoraleLevel", abilityRow.MoraleLevel.ToString(CultureInfo.InvariantCulture), DescribeMoraleLevel(abilityRow.MoraleLevel), "Derived explanation", SemanticConfidence.Inferred, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                AddEntry(entries, "Ability.Range", "Ability.Range", abilityRow.Range.ToString(CultureInfo.InvariantCulture), abilityRow.Range.ToString(CultureInfo.InvariantCulture) + " feet", "Derived units", SemanticConfidence.Inferred, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                AddEntry(entries, "Ability.CastTime", "Ability.CastTime", abilityRow.CastTime.ToString(CultureInfo.InvariantCulture), abilityRow.CastTime.ToString(CultureInfo.InvariantCulture) + " ms", "Derived units", SemanticConfidence.Inferred, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                AddEntry(entries, "Ability.Cooldown", "Ability.Cooldown", abilityRow.Cooldown.ToString(CultureInfo.InvariantCulture), abilityRow.Cooldown.ToString(CultureInfo.InvariantCulture) + " ms", "Derived units", SemanticConfidence.Inferred, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                AddEntry(entries, "Ability.ApCost", "Ability.ApCost", abilityRow.ApCost.ToString(CultureInfo.InvariantCulture), abilityRow.ApCost.ToString(CultureInfo.InvariantCulture) + " action points", "Derived units", SemanticConfidence.Inferred, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                AddEntry(entries, "Ability.Faction", "Ability.Faction", abilityRow.Faction.ToString(CultureInfo.InvariantCulture), DescribeFaction(abilityRow.Faction), "Unresolved client BIN byte", SemanticConfidence.Unknown, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);

                for (int index = 0; index < abilityRow.ComponentIds.Count; ++index)
                {
                    ushort componentId = abilityRow.ComponentIds[index];
                    if (componentId <= 0)
                        continue;

                    AddEntry(entries, "Ability.Component[" + index.ToString(CultureInfo.InvariantCulture) + "]", "Ability.ComponentId", componentId.ToString(CultureInfo.InvariantCulture), DescribeComponentId(componentId), "abilityexport.bin and abilitycomponentexport.bin", SemanticConfidence.Inferred, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                }

                for (int index = 0; index < abilityRow.Triggers.Count; ++index)
                {
                    uint triggerValue = abilityRow.Triggers[index];
                    if (triggerValue == 0)
                        continue;

                    AddEntry(entries, "Ability.Trigger[" + index.ToString(CultureInfo.InvariantCulture) + "]", "Ability.Trigger", triggerValue.ToString(CultureInfo.InvariantCulture), DescribeTrigger(triggerValue), "Static enum reference", SemanticConfidence.Inferred, abilityRow.TableName, abilityRow.SourcePath, FormatLocation(abilityRow), string.Empty);
                }
            }

            foreach (BinaryComponentRecord componentRow in report.BinaryComponentRows)
            {
                string componentPrefix = "Component[" + componentRow.ComponentId.ToString(CultureInfo.InvariantCulture) + "]";
                AddEntry(entries, componentPrefix + ".Operation", "Component.Operation", componentRow.Operation.ToString(CultureInfo.InvariantCulture), DescribeComponentOperation(componentRow.Operation), "Static enum reference", SemanticConfidence.Inferred, componentRow.TableName, componentRow.SourcePath, FormatLocation(componentRow), string.Empty);
                AddSchemaEntry(entries, report, componentRow, componentPrefix + ".Duration", "Duration", componentRow.Duration.ToString(CultureInfo.InvariantCulture));
                AddSchemaEntry(entries, report, componentRow, componentPrefix + ".Interval", "Interval", componentRow.Interval.ToString(CultureInfo.InvariantCulture));
                AddSchemaEntry(entries, report, componentRow, componentPrefix + ".Radius", "Radius", componentRow.Radius.ToString(CultureInfo.InvariantCulture));

                if (componentRow.ActivationDelay > 0)
                    AddSchemaEntry(entries, report, componentRow, componentPrefix + ".ActivationDelay", "ActivationDelay", componentRow.ActivationDelay.ToString(CultureInfo.InvariantCulture));
                if (componentRow.MaxTargets > 0)
                    AddSchemaEntry(entries, report, componentRow, componentPrefix + ".MaxTargets", "MaxTargets", componentRow.MaxTargets.ToString(CultureInfo.InvariantCulture));

                for (int index = 0; index < componentRow.Values.Count; ++index)
                {
                    int value = componentRow.Values[index];
                    if (value == 0)
                        continue;

                    AddSchemaEntry(entries, report, componentRow, componentPrefix + ".Value[" + index.ToString(CultureInfo.InvariantCulture) + "]", "Value[" + index.ToString(CultureInfo.InvariantCulture) + "]", value.ToString(CultureInfo.InvariantCulture));
                }

                for (int index = 0; index < componentRow.Multipliers.Count; ++index)
                {
                    int value = componentRow.Multipliers[index];
                    if (value == 0)
                        continue;

                    AddSchemaEntry(entries, report, componentRow, componentPrefix + ".Multiplier[" + index.ToString(CultureInfo.InvariantCulture) + "]", "Multiplier[" + index.ToString(CultureInfo.InvariantCulture) + "]", value.ToString(CultureInfo.InvariantCulture));
                }

                foreach (BinaryExtDataRecord extRecord in componentRow.ExtData)
                {
                    AddExtDataEntry(entries, report, componentRow, extRecord, 1, extRecord.Val1);
                    AddExtDataEntry(entries, report, componentRow, extRecord, 2, extRecord.Val2);
                    AddExtDataEntry(entries, report, componentRow, extRecord, 3, extRecord.Val3);
                    AddExtDataEntry(entries, report, componentRow, extRecord, 4, extRecord.Val4);
                    AddExtDataEntry(entries, report, componentRow, extRecord, 5, extRecord.Val5);
                    AddExtDataEntry(entries, report, componentRow, extRecord, 6, extRecord.Val6);
                    AddExtDataEntry(entries, report, componentRow, extRecord, 7, extRecord.Val7);
                    AddExtDataEntry(entries, report, componentRow, extRecord, 8, extRecord.Val8);
                    if (extRecord.Val9 != 0)
                        AddSchemaEntry(entries, report, componentRow, componentPrefix + ".ExtData[" + extRecord.SlotIndex.ToString(CultureInfo.InvariantCulture) + "].Val9", "ExtData[" + extRecord.SlotIndex.ToString(CultureInfo.InvariantCulture) + "].Val9", extRecord.Val9.ToString(CultureInfo.InvariantCulture));
                }
            }

            foreach (RequirementReferenceRecord reference in report.RequirementReferences ?? new List<RequirementReferenceRecord>())
            {
                string fieldPath = reference.SourceKind + "[" + reference.SourceId.ToString(CultureInfo.InvariantCulture) + "]." + reference.SourceField;
                string rawValue = reference.RequirementId.ToString(CultureInfo.InvariantCulture);
                if (entries.Any(entry => string.Equals(entry.FieldPath, fieldPath, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(entry.DomainKey, "Requirement.RequirementId", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(entry.RawValue, rawValue, StringComparison.OrdinalIgnoreCase)))
                    continue;

                RequirementSemanticDescription description = _requirements.DescribeRequirement(reference.RequirementId);
                AddEntry(entries,
                    fieldPath,
                    "Requirement.RequirementId",
                    rawValue,
                    description.Meaning,
                    "Inferred requirement link",
                    reference.Confidence,
                    reference.SourceTableName,
                    reference.SourcePath,
                    reference.SourceLocation,
                    string.IsNullOrWhiteSpace(description.Notes)
                        ? reference.Notes
                        : reference.Notes + " " + description.Notes);
            }

            return entries;
        }

        public DefinitionExplorerModel BuildExplorer(DefinitionEntry entry)
        {
            DefinitionDomain domain = entry == null ? null : GetDomain(entry.DomainKey);
            List<DefinitionExplorerRow> rows = domain == null ? new List<DefinitionExplorerRow>() : domain.Options.Select(option => new DefinitionExplorerRow
            {
                IsCurrent = string.Equals(option.RawValue, entry.RawValue, StringComparison.OrdinalIgnoreCase),
                RawValue = option.RawValue,
                Meaning = option.Meaning,
                Confidence = option.Confidence,
                Source = option.Source,
                SourcePath = option.SourcePath,
                Location = option.Location,
                Notes = option.Notes
            }).OrderByDescending(row => row.IsCurrent).ThenBy(row => ParseSortBucket(row.RawValue)).ThenBy(row => row.RawValue).ThenBy(row => row.Meaning).ToList();

            if (rows.Count == 0 && entry != null)
            {
                rows.Add(new DefinitionExplorerRow
                {
                    IsCurrent = true,
                    RawValue = entry.RawValue,
                    Meaning = entry.Meaning,
                    Confidence = entry.Confidence,
                    Source = string.IsNullOrWhiteSpace(entry.ValueSource) ? entry.DefinitionSource : entry.ValueSource,
                    SourcePath = entry.ValueSourcePath,
                    Location = entry.ValueSourceLocation,
                    Notes = "This field is currently modeled as a scalar or unresolved byte field, not as a closed enum."
                });
            }

            return new DefinitionExplorerModel
            {
                FieldPath = entry == null ? string.Empty : entry.FieldPath,
                DomainKey = entry == null ? string.Empty : entry.DomainKey,
                CurrentRawValue = entry == null ? string.Empty : entry.RawValue,
                CurrentMeaning = entry == null ? string.Empty : entry.Meaning,
                CurrentConfidence = entry == null ? string.Empty : entry.Confidence,
                CurrentDefinitionSource = entry == null ? string.Empty : entry.DefinitionSource,
                CurrentValueSource = entry == null ? string.Empty : entry.ValueSource,
                CurrentValueSourcePath = entry == null ? string.Empty : entry.ValueSourcePath,
                CurrentValueLocation = entry == null ? string.Empty : entry.ValueSourceLocation,
                CurrentNotes = entry == null ? string.Empty : entry.Notes,
                DomainDisplayName = domain == null ? "Unknown domain" : domain.DisplayName,
                DomainDescription = domain == null ? "No domain metadata is available for this field." : domain.Description,
                DomainNotes = domain == null ? string.Empty : domain.Notes,
                IsFiniteDomain = domain != null && domain.IsFiniteDomain,
                Rows = rows
            };
        }

        public string DescribeTacticType(uint value) { return DescribeEnum(value, TacticTypes, "Unknown tactic bucket"); }
        public string DescribeTargetType(uint value) { return DescribeEnum(value, TargetTypes, "Unknown target type"); }
        public string DescribeAbilityType(uint value) { return DescribeEnum(value, AbilityTypes, "Unknown ability type"); }
        public string DescribeAttackType(uint value) { return DescribeEnum(value, AttackTypes, "Unknown attack type"); }
        public string DescribeTrigger(uint value) { return DescribeTriggerValue(value); }
        public static string DescribeTriggerValue(uint value) { return DescribeEnum(value, TriggerTypes, "Unknown trigger"); }
        public string DescribeComponentOperation(uint value) { return DescribeComponentOperationValue(value); }
        public static string DescribeComponentOperationValue(uint value) { return DescribeEnum(value, ComponentOperations, "Unknown component operation"); }
        public string DescribeCareerLine(uint value) { return _careerLines.ContainsKey(value) && !string.IsNullOrWhiteSpace(_careerLines[value]) ? _careerLines[value] : "Unknown career line " + value.ToString(CultureInfo.InvariantCulture); }
        public string DescribeMoraleLevel(byte value) { return value <= 0 ? "Not marked as a morale ability" : "Morale rank " + value.ToString(CultureInfo.InvariantCulture); }
        public string DescribeFaction(byte value) { return value == 0 ? "Raw faction byte is zero. No extracted string mapping is known yet." : "Raw faction byte " + value.ToString(CultureInfo.InvariantCulture) + ". Meaning is not decoded yet."; }
        public string DescribeEffectId(int effectId) { return effectId > 0 && _effects.ContainsKey(effectId) && !string.IsNullOrWhiteSpace(_effects[effectId]) ? _effects[effectId] : "Unknown effect " + effectId.ToString(CultureInfo.InvariantCulture); }

        public string DescribeComponentId(ushort componentId)
        {
            BinaryComponentRecord row;
            if (_components.TryGetValue(componentId, out row))
            {
                string operation = DescribeComponentOperation(row.Operation);
                return string.IsNullOrWhiteSpace(row.Description) ? operation : operation + " | " + row.Description;
            }

            return "Unknown component " + componentId.ToString(CultureInfo.InvariantCulture);
        }

        private DefinitionDomain GetDomain(string domainKey)
        {
            DefinitionDomain domain;
            if (string.IsNullOrWhiteSpace(domainKey))
                return null;

            if (_domains.TryGetValue(domainKey, out domain))
                return domain;

            return _componentSchemas == null ? null : _componentSchemas.BuildDomain(domainKey);
        }

        private Dictionary<string, DefinitionDomain> BuildDomains(AbilityDataset dataset)
        {
            Dictionary<string, DefinitionDomain> domains = new Dictionary<string, DefinitionDomain>(StringComparer.OrdinalIgnoreCase);
            domains["Ability.EffectId"] = CreateOptionsDomain("Ability.EffectId", "Client effect identifier", "Effect IDs referenced by abilities and defined in effects.csv.", dataset == null ? Enumerable.Empty<DefinitionOption>() : dataset.ClientEffects.OrderBy(row => row.EffectId).ThenBy(row => row.LineNumber).Select(row => new DefinitionOption
            {
                RawValue = row.EffectId.ToString(CultureInfo.InvariantCulture),
                Meaning = string.IsNullOrWhiteSpace(row.Name) ? "(unnamed effect)" : row.Name,
                Confidence = SemanticConfidence.Confirmed,
                Source = row.TableName,
                SourcePath = row.SourcePath,
                Location = "line " + row.LineNumber.ToString(CultureInfo.InvariantCulture),
                Notes = BuildEffectNotes(row)
            }));
            domains["Ability.TacticType"] = CreateOptionsDomain("Ability.TacticType", "Tactic bucket", "The client BIN tactic source bucket for the ability.", TacticTypes.Select(entry => ToOption(entry.Key, entry.Value, "Static enum reference", SemanticConfidence.Inferred)));
            domains["Ability.TargetType"] = CreateOptionsDomain("Ability.TargetType", "Target type", "The client target-selection bucket for the ability.", TargetTypes.Select(entry => ToOption(entry.Key, entry.Value, "Static enum reference", SemanticConfidence.Inferred)));
            domains["Ability.AbilityType"] = CreateOptionsDomain("Ability.AbilityType", "Ability type", "The client ability family for this entry.", AbilityTypes.Select(entry => ToOption(entry.Key, entry.Value, "Static enum reference", SemanticConfidence.Inferred)));
            domains["Ability.AttackType"] = CreateOptionsDomain("Ability.AttackType", "Attack type", "The client combat typing attached to the ability.", AttackTypes.Select(entry => ToOption(entry.Key, entry.Value, "Static enum reference", SemanticConfidence.Inferred)));
            domains["Ability.CareerLine"] = CreateOptionsDomain("Ability.CareerLine", "Career line", "Career line values resolved from extracted client strings.", dataset == null ? Enumerable.Empty<DefinitionOption>() : dataset.CareerLines.OrderBy(row => row.EntryId).ThenBy(row => row.LineNumber).Select(row => new DefinitionOption
            {
                RawValue = row.EntryId.ToString(CultureInfo.InvariantCulture),
                Meaning = string.IsNullOrWhiteSpace(row.NormalizedValue) ? "(unnamed career line)" : row.NormalizedValue,
                Confidence = SemanticConfidence.Confirmed,
                Source = row.TableName,
                SourcePath = row.SourcePath,
                Location = "line " + row.LineNumber.ToString(CultureInfo.InvariantCulture),
                Notes = string.Empty
            }));
            domains["Ability.Trigger"] = CreateOptionsDomain("Ability.Trigger", "Ability trigger", "Trigger values that describe when a component fires relative to the cast chain.", TriggerTypes.Select(entry => ToOption(entry.Key, entry.Value, "Static enum reference", SemanticConfidence.Inferred)));
            domains["Component.Operation"] = CreateOptionsDomain("Component.Operation", "Component operation", "Operation codes that describe what a component does when executed.", ComponentOperations.Select(entry => ToOption(entry.Key, entry.Value, "Static enum reference", SemanticConfidence.Inferred)));
            domains["Ability.ComponentId"] = CreateOptionsDomain("Ability.ComponentId", "Component identifier", "Component IDs decoded from abilitycomponentexport.bin.", dataset == null ? Enumerable.Empty<DefinitionOption>() : dataset.BinaryComponents.OrderBy(row => row.ComponentId).ThenBy(row => row.RecordIndex).Select(row => new DefinitionOption
            {
                RawValue = row.ComponentId.ToString(CultureInfo.InvariantCulture),
                Meaning = DescribeComponentId(row.ComponentId),
                Confidence = SemanticConfidence.Inferred,
                Source = row.TableName,
                SourcePath = row.SourcePath,
                Location = "byte " + row.ByteOffset.ToString(CultureInfo.InvariantCulture),
                Notes = BuildComponentNotes(row)
            }));
            domains["Requirement.RequirementId"] = CreateOptionsDomain("Requirement.RequirementId", "Requirement identifier", "Requirement rows decoded from abilityrequirementexport.bin. Links into this domain are inferred when another extracted BIN field points at one of these ids.", dataset == null ? Enumerable.Empty<DefinitionOption>() : dataset.BinaryRequirements.OrderBy(row => row.RequirementId).ThenBy(row => row.RecordIndex).Select(row =>
            {
                RequirementSemanticDescription description = _requirements.DescribeRequirement(row.RequirementId);
                return new DefinitionOption
                {
                    RawValue = row.RequirementId.ToString(CultureInfo.InvariantCulture),
                    Meaning = description.Meaning,
                    Confidence = description.Confidence,
                    Source = row.TableName,
                    SourcePath = row.SourcePath,
                    Location = "byte " + row.ByteOffset.ToString(CultureInfo.InvariantCulture),
                    Notes = description.Notes
                };
            }));
            domains["Ability.MoraleLevel"] = CreateOptionsDomain("Ability.MoraleLevel", "Morale rank", "Morale rank bucket used by morale abilities.", new[] { ToOption(0, "Not a morale ability", "Derived explanation", SemanticConfidence.Inferred), ToOption(1, "Morale rank 1", "Derived explanation", SemanticConfidence.Inferred), ToOption(2, "Morale rank 2", "Derived explanation", SemanticConfidence.Inferred), ToOption(3, "Morale rank 3", "Derived explanation", SemanticConfidence.Inferred), ToOption(4, "Morale rank 4", "Derived explanation", SemanticConfidence.Inferred) });
            domains["Ability.Range"] = CreateScalarDomain("Ability.Range", "Range in feet", "Unsigned client BIN field measured in feet.", "This field is numeric rather than a closed enum.");
            domains["Ability.CastTime"] = CreateScalarDomain("Ability.CastTime", "Cast time in milliseconds", "Unsigned client BIN field measured in milliseconds.", "This field is numeric rather than a closed enum.");
            domains["Ability.Cooldown"] = CreateScalarDomain("Ability.Cooldown", "Cooldown in milliseconds", "Unsigned client BIN field measured in milliseconds.", "This field is numeric rather than a closed enum.");
            domains["Ability.ApCost"] = CreateScalarDomain("Ability.ApCost", "Action point cost", "Unsigned client BIN field measured in AP.", "This field is numeric rather than a closed enum.");
            domains["Ability.Faction"] = CreateScalarDomain("Ability.Faction", "Faction byte", "The client BIN faction byte is present, but its meaning is not fully decoded yet.", "Treat this as unresolved until more retail/client evidence is mapped.");
            domains["Component.Duration"] = CreateScalarDomain("Component.Duration", "Component duration in milliseconds", "Unsigned client BIN field measured in milliseconds.", "This field is numeric rather than a closed enum.");
            domains["Component.Interval"] = CreateScalarDomain("Component.Interval", "Component interval in milliseconds", "Unsigned client BIN field measured in milliseconds.", "This field is numeric rather than a closed enum.");
            domains["Component.Radius"] = CreateScalarDomain("Component.Radius", "Component radius in feet", "Unsigned client BIN field measured in feet.", "This field is numeric rather than a closed enum.");
            return domains;
        }

        private static void AddEntry(List<DefinitionEntry> entries, string fieldPath, string domainKey, string rawValue, string meaning, string definitionSource, string confidence, string valueSource, string valueSourcePath, string valueSourceLocation, string notes)
        {
            entries.Add(new DefinitionEntry
            {
                FieldPath = fieldPath,
                DomainKey = domainKey,
                RawValue = rawValue,
                Meaning = meaning,
                DefinitionSource = definitionSource,
                Confidence = confidence,
                ValueSource = valueSource,
                ValueSourcePath = valueSourcePath,
                ValueSourceLocation = valueSourceLocation,
                Notes = notes
            });
        }

        private void AddSchemaEntry(List<DefinitionEntry> entries, AbilityAnalysisResult report, BinaryComponentRecord componentRow, string fieldPath, string fieldKey, string rawValue)
        {
            ComponentFieldSemantic semantic = _componentSchemas.ResolveFieldSemantic(report, componentRow, fieldKey, rawValue);
            AddEntry(entries, fieldPath, semantic.DomainKey, rawValue, semantic.Meaning, semantic.Source, semantic.Confidence, componentRow.TableName, componentRow.SourcePath, FormatLocation(componentRow), semantic.Notes);
        }

        private void AddExtDataEntry(List<DefinitionEntry> entries, AbilityAnalysisResult report, BinaryComponentRecord componentRow, BinaryExtDataRecord extRecord, int valueIndex, int value)
        {
            if (value == 0)
                return;

            AddSchemaEntry(
                entries,
                report,
                componentRow,
                "Component[" + componentRow.ComponentId.ToString(CultureInfo.InvariantCulture) + "].ExtData[" + extRecord.SlotIndex.ToString(CultureInfo.InvariantCulture) + "].Val" + valueIndex.ToString(CultureInfo.InvariantCulture),
                "ExtData[" + extRecord.SlotIndex.ToString(CultureInfo.InvariantCulture) + "].Val" + valueIndex.ToString(CultureInfo.InvariantCulture),
                value.ToString(CultureInfo.InvariantCulture));
        }

        private static DefinitionDomain CreateOptionsDomain(string domainKey, string displayName, string description, IEnumerable<DefinitionOption> options)
        {
            return new DefinitionDomain { DomainKey = domainKey, DisplayName = displayName, Description = description, Notes = string.Empty, IsFiniteDomain = true, Options = options.ToList() };
        }

        private static DefinitionDomain CreateScalarDomain(string domainKey, string displayName, string description, string notes)
        {
            return new DefinitionDomain { DomainKey = domainKey, DisplayName = displayName, Description = description, Notes = notes, IsFiniteDomain = false, Options = new List<DefinitionOption>() };
        }

        private static DefinitionOption ToOption(uint rawValue, string meaning, string source, string confidence)
        {
            return new DefinitionOption { RawValue = rawValue.ToString(CultureInfo.InvariantCulture), Meaning = meaning, Confidence = confidence, Source = source, SourcePath = string.Empty, Location = string.Empty, Notes = string.Empty };
        }

        private static string DescribeEnum(uint value, IDictionary<uint, string> map, string fallback)
        {
            string resolved;
            return map.TryGetValue(value, out resolved) ? resolved : fallback + " (" + value.ToString(CultureInfo.InvariantCulture) + ")";
        }

        private static long ParseSortBucket(string rawValue)
        {
            long parsedValue;
            return long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedValue) ? parsedValue : long.MaxValue;
        }

        private static string BuildEffectNotes(ClientEffectRecord row)
        {
            return "BuildUp=" + row.BuildUpId.ToString(CultureInfo.InvariantCulture)
                + ", Cast=" + row.CastId.ToString(CultureInfo.InvariantCulture)
                + ", Activate=" + row.ActivateId.ToString(CultureInfo.InvariantCulture)
                + ", Projectile=" + row.ProjectileId.ToString(CultureInfo.InvariantCulture)
                + ", Impact=" + row.ImpactId.ToString(CultureInfo.InvariantCulture)
                + ", AOE=" + row.AoeId.ToString(CultureInfo.InvariantCulture)
                + ", Channel=" + row.ChannelingId.ToString(CultureInfo.InvariantCulture);
        }

        private string BuildComponentNotes(BinaryComponentRecord row)
        {
            return "Operation=" + DescribeComponentOperation(row.Operation)
                + ", Duration=" + row.Duration.ToString(CultureInfo.InvariantCulture)
                + ", Interval=" + row.Interval.ToString(CultureInfo.InvariantCulture)
                + ", Radius=" + row.Radius.ToString(CultureInfo.InvariantCulture)
                + (string.IsNullOrWhiteSpace(row.Description) ? string.Empty : ", Description=" + row.Description);
        }

        private static string FormatLocation(SourceRowBase row)
        {
            if (row == null)
                return string.Empty;

            if (row.ByteOffset > 0)
                return "byte " + row.ByteOffset.ToString(CultureInfo.InvariantCulture);

            if (row.LineNumber > 0)
                return "line " + row.LineNumber.ToString(CultureInfo.InvariantCulture);

            return string.Empty;
        }
    }

    public sealed class DefinitionEntry
    {
        public string FieldPath { get; set; }
        public string DomainKey { get; set; }
        public string RawValue { get; set; }
        public string Meaning { get; set; }
        public string DefinitionSource { get; set; }
        public string Confidence { get; set; }
        public string ValueSource { get; set; }
        public string ValueSourcePath { get; set; }
        public string ValueSourceLocation { get; set; }
        public string Notes { get; set; }
    }

    public sealed class DefinitionDomain
    {
        public string DomainKey { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool IsFiniteDomain { get; set; }
        public List<DefinitionOption> Options { get; set; }
    }

    public sealed class DefinitionOption
    {
        public string RawValue { get; set; }
        public string Meaning { get; set; }
        public string Confidence { get; set; }
        public string Source { get; set; }
        public string SourcePath { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
    }

    public sealed class DefinitionExplorerModel
    {
        public string FieldPath { get; set; }
        public string DomainKey { get; set; }
        public string CurrentRawValue { get; set; }
        public string CurrentMeaning { get; set; }
        public string CurrentConfidence { get; set; }
        public string CurrentDefinitionSource { get; set; }
        public string CurrentValueSource { get; set; }
        public string CurrentValueSourcePath { get; set; }
        public string CurrentValueLocation { get; set; }
        public string CurrentNotes { get; set; }
        public string DomainDisplayName { get; set; }
        public string DomainDescription { get; set; }
        public string DomainNotes { get; set; }
        public bool IsFiniteDomain { get; set; }
        public List<DefinitionExplorerRow> Rows { get; set; }
    }

    public sealed class DefinitionExplorerRow
    {
        public bool IsCurrent { get; set; }
        public string RawValue { get; set; }
        public string Meaning { get; set; }
        public string Confidence { get; set; }
        public string Source { get; set; }
        public string SourcePath { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public string CurrentMarker { get { return IsCurrent ? "Current" : string.Empty; } }
    }
}
