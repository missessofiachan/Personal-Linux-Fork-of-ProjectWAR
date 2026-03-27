using ClientDataMatrix.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ClientDataMatrix.Services
{
    public static class SemanticConfidence
    {
        public const string Confirmed = "Confirmed";
        public const string Inferred = "Inferred";
        public const string Structural = "Structural";
        public const string Unknown = "Unknown";
        public const string Londo = "Londo";
    }

    public sealed class ComponentSchemaCatalog
    {
        private static readonly Regex ComponentTokenRegex = new Regex(@"COM_(\d+)_([A-Z0-9_]+)", RegexOptions.Compiled);
        private static readonly HashSet<uint> PriorityOperations = new HashSet<uint> { 12U, 23U, 24U, 38U };
        private const string OverrideRelativePath = @"ClientDataMatrix\Configuration\component-schema-overrides.tsv";

        private readonly AbilityDataset _dataset;
        private readonly Dictionary<ushort, BinaryComponentRecord> _componentsById;
        private readonly Dictionary<ushort, BinaryRequirementRecord> _requirementsById;
        private readonly HashSet<ushort> _knownRequirementIds;
        private readonly Dictionary<ushort, string> _abilityNamesById;
        private readonly RequirementCatalog _requirements;
        private readonly Dictionary<string, List<ComponentTokenEvidence>> _evidenceByDomainKey;
        private readonly Dictionary<string, List<ComponentTokenEvidence>> _evidenceByFieldKey;
        private readonly Dictionary<string, List<ComponentTokenEvidence>> _evidenceByAbilityComponentField;
        private readonly Dictionary<string, ComponentSchemaOverride> _overridesByDomainKey;

        public ComponentSchemaCatalog(AbilityDataset dataset)
        {
            _dataset = dataset;
            if (dataset == null)
            {
                _componentsById = new Dictionary<ushort, BinaryComponentRecord>();
                _requirementsById = new Dictionary<ushort, BinaryRequirementRecord>();
                _knownRequirementIds = new HashSet<ushort>();
                _abilityNamesById = new Dictionary<ushort, string>();
                _requirements = new RequirementCatalog(null);
                _evidenceByDomainKey = new Dictionary<string, List<ComponentTokenEvidence>>(StringComparer.OrdinalIgnoreCase);
                _evidenceByFieldKey = new Dictionary<string, List<ComponentTokenEvidence>>(StringComparer.OrdinalIgnoreCase);
                _evidenceByAbilityComponentField = new Dictionary<string, List<ComponentTokenEvidence>>(StringComparer.OrdinalIgnoreCase);
                _overridesByDomainKey = new Dictionary<string, ComponentSchemaOverride>(StringComparer.OrdinalIgnoreCase);
                return;
            }

            _componentsById = dataset.BinaryComponents
                .GroupBy(row => row.ComponentId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.RecordIndex).First());
            _requirementsById = dataset.BinaryRequirements
                .GroupBy(row => row.RequirementId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.RecordIndex).First());
            _knownRequirementIds = new HashSet<ushort>(_requirementsById.Keys);
            _abilityNamesById = BuildAbilityNames(dataset);
            _requirements = new RequirementCatalog(dataset);
            _evidenceByDomainKey = new Dictionary<string, List<ComponentTokenEvidence>>(StringComparer.OrdinalIgnoreCase);
            _evidenceByFieldKey = new Dictionary<string, List<ComponentTokenEvidence>>(StringComparer.OrdinalIgnoreCase);
            _evidenceByAbilityComponentField = new Dictionary<string, List<ComponentTokenEvidence>>(StringComparer.OrdinalIgnoreCase);
            _overridesByDomainKey = LoadOverrides();

            BuildClientTokenEvidence(dataset);
        }

        public static string BuildOperationFieldDomainKey(uint operation, string fieldKey)
        {
            return "Component.Operation." + operation.ToString(CultureInfo.InvariantCulture) + "." + fieldKey;
        }

        public ComponentFieldSemantic ResolveFieldSemantic(AbilityAnalysisResult report, BinaryComponentRecord componentRow, string fieldKey, string rawValue)
        {
            if (componentRow == null || string.IsNullOrWhiteSpace(fieldKey))
                return CreateUnknown("No component field context is available.");

            string domainKey = BuildOperationFieldDomainKey(componentRow.Operation, fieldKey);
            string abilitySpecificKey = report == null
                ? string.Empty
                : BuildAbilityComponentFieldKey(report.AbilityId, componentRow.ComponentId, fieldKey);

            List<ComponentTokenEvidence> directEvidence;
            if (!string.IsNullOrWhiteSpace(abilitySpecificKey) && _evidenceByAbilityComponentField.TryGetValue(abilitySpecificKey, out directEvidence) && directEvidence.Count > 0)
            {
                return new ComponentFieldSemantic
                {
                    DomainKey = domainKey,
                    Meaning = BuildEvidenceMeaning(directEvidence),
                    Confidence = SemanticConfidence.Confirmed,
                    Source = BuildSourceLabel(directEvidence),
                    SourcePath = directEvidence[0].SourcePath,
                    SourceLocation = directEvidence[0].SourceLocation,
                    Notes = BuildEvidenceNotes(directEvidence)
                };
            }

            List<ComponentTokenEvidence> domainEvidence;
            if (_evidenceByDomainKey.TryGetValue(domainKey, out domainEvidence) && domainEvidence.Count > 0)
            {
                return new ComponentFieldSemantic
                {
                    DomainKey = domainKey,
                    Meaning = "Operation-level client text evidence: " + BuildEvidenceMeaning(domainEvidence),
                    Confidence = SemanticConfidence.Inferred,
                    Source = "Aggregated client strings",
                    SourcePath = domainEvidence[0].SourcePath,
                    SourceLocation = domainEvidence[0].SourceLocation,
                    Notes = BuildEvidenceNotes(domainEvidence)
                };
            }

            ComponentSchemaOverride schemaOverride;
            if (_overridesByDomainKey.TryGetValue(domainKey, out schemaOverride))
            {
                return new ComponentFieldSemantic
                {
                    DomainKey = domainKey,
                    Meaning = schemaOverride.Meaning,
                    Confidence = string.IsNullOrWhiteSpace(schemaOverride.Confidence) ? SemanticConfidence.Londo : schemaOverride.Confidence,
                    Source = schemaOverride.SourceLabel,
                    SourcePath = schemaOverride.SourcePath,
                    SourceLocation = schemaOverride.SourceLocation,
                    Notes = schemaOverride.Notes
                };
            }

            ComponentFieldSemantic requirementSemantic;
            if (TryResolveRequirementFieldSemantic(fieldKey, rawValue, out requirementSemantic))
                return requirementSemantic;

            ComponentFieldSemantic extDataVal2Semantic;
            if (TryResolveExtDataVal2Semantic(componentRow.Operation, fieldKey, rawValue, out extDataVal2Semantic))
                return extDataVal2Semantic;

            ComponentFieldSemantic extDataVal3Semantic;
            if (TryResolveExtDataVal3Semantic(fieldKey, rawValue, out extDataVal3Semantic))
                return extDataVal3Semantic;

            ComponentFieldSemantic extDataVal4Semantic;
            if (TryResolveExtDataVal4Semantic(fieldKey, rawValue, out extDataVal4Semantic))
                return extDataVal4Semantic;

            ComponentFieldSemantic applyAbilityExtDataSemantic;
            if (TryResolveApplyAbilityExtDataKnownFieldSemantic(componentRow.Operation, fieldKey, rawValue, out applyAbilityExtDataSemantic))
                return applyAbilityExtDataSemantic;

            ComponentFieldSemantic structuralSemantic;
            if (TryResolveOperationSpecificStructuralSemantic(componentRow.Operation, fieldKey, rawValue, out structuralSemantic))
                return structuralSemantic;

            ComponentFieldSemantic genericFlagsSemantic;
            if (TryResolveGenericFlagsRawSemantic(componentRow.Operation, fieldKey, rawValue, out genericFlagsSemantic))
                return genericFlagsSemantic;

            ComponentFieldSemantic namedControlSemantic;
            if (TryResolveNamedControlFieldSemantic(componentRow.Operation, fieldKey, rawValue, out namedControlSemantic))
                return namedControlSemantic;

            List<ComponentTokenEvidence> sharedFieldEvidence;
            if (IsSharedFieldFallbackAllowed(fieldKey)
                && _evidenceByFieldKey.TryGetValue(fieldKey, out sharedFieldEvidence)
                && sharedFieldEvidence.Count > 0)
            {
                return new ComponentFieldSemantic
                {
                    DomainKey = domainKey,
                    Meaning = "Shared component-field evidence: " + BuildEvidenceMeaning(sharedFieldEvidence),
                    Confidence = SemanticConfidence.Inferred,
                    Source = "Aggregated client strings from other operations",
                    SourcePath = sharedFieldEvidence[0].SourcePath,
                    SourceLocation = sharedFieldEvidence[0].SourceLocation,
                    Notes = BuildSharedEvidenceNotes(fieldKey, sharedFieldEvidence)
                };
            }

            return new ComponentFieldSemantic
            {
                DomainKey = domainKey,
                Meaning = "No extracted-client semantic mapping is known yet for this field.",
                Confidence = SemanticConfidence.Unknown,
                Source = "No direct evidence",
                SourcePath = string.Empty,
                SourceLocation = string.Empty,
                Notes = "This field is present in the client BIN data, but no direct client-string token or override currently explains its semantics."
            };
        }

        public DefinitionDomain BuildDomain(string domainKey)
        {
            if (string.IsNullOrWhiteSpace(domainKey))
                return null;

            List<DefinitionOption> options = new List<DefinitionOption>();
            bool usesSharedFieldEvidence = false;
            List<ComponentTokenEvidence> evidenceRows;
            if (_evidenceByDomainKey.TryGetValue(domainKey, out evidenceRows))
            {
                foreach (IGrouping<string, ComponentTokenEvidence> group in evidenceRows.GroupBy(row => row.TokenBody, StringComparer.OrdinalIgnoreCase).OrderBy(group => group.Key))
                {
                    ComponentTokenEvidence example = group.OrderBy(row => row.AbilityId).ThenBy(row => row.SourcePath).ThenBy(row => row.SourceLocation).First();
                    options.Add(new DefinitionOption
                    {
                        RawValue = group.Key,
                        Meaning = example.Meaning,
                        Confidence = SemanticConfidence.Confirmed,
                        Source = BuildSourceLabel(group),
                        SourcePath = example.SourcePath,
                        Location = example.SourceLocation,
                        Notes = BuildEvidenceNotes(group)
                    });
                }
            }

            string fieldKey;
            List<ComponentTokenEvidence> sharedFieldEvidenceRows;
            if (options.Count == 0
                && TryExtractFieldKey(domainKey, out fieldKey)
                && IsSharedFieldFallbackAllowed(fieldKey)
                && _evidenceByFieldKey.TryGetValue(fieldKey, out sharedFieldEvidenceRows))
            {
                foreach (IGrouping<string, ComponentTokenEvidence> group in sharedFieldEvidenceRows.GroupBy(row => row.TokenBody, StringComparer.OrdinalIgnoreCase).OrderBy(group => group.Key))
                {
                    ComponentTokenEvidence example = group.OrderBy(row => row.AbilityId).ThenBy(row => row.SourcePath).ThenBy(row => row.SourceLocation).First();
                    options.Add(new DefinitionOption
                    {
                        RawValue = group.Key,
                        Meaning = example.Meaning,
                        Confidence = SemanticConfidence.Inferred,
                        Source = BuildSourceLabel(group),
                        SourcePath = example.SourcePath,
                        Location = example.SourceLocation,
                        Notes = BuildSharedEvidenceNotes(fieldKey, group)
                    });
                }

                usesSharedFieldEvidence = options.Count > 0;
            }

            ComponentSchemaOverride schemaOverride;
            if (_overridesByDomainKey.TryGetValue(domainKey, out schemaOverride))
            {
                options.Add(new DefinitionOption
                {
                    RawValue = "override",
                    Meaning = schemaOverride.Meaning,
                    Confidence = string.IsNullOrWhiteSpace(schemaOverride.Confidence) ? SemanticConfidence.Londo : schemaOverride.Confidence,
                    Source = schemaOverride.SourceLabel,
                    SourcePath = schemaOverride.SourcePath,
                    Location = schemaOverride.SourceLocation,
                    Notes = schemaOverride.Notes
                });
            }

            return new DefinitionDomain
            {
                DomainKey = domainKey,
                DisplayName = BuildDisplayName(domainKey),
                Description = usesSharedFieldEvidence
                    ? "Shared semantic evidence for this component field gathered from extracted client text across other component operations. The rows below are semantic tokens and notes, not a closed numeric enum."
                    : "Operation-specific semantic evidence for a component field. The rows below are semantic tokens and notes, not a closed numeric enum.",
                Notes = options.Count == 0
                    ? "No client-token evidence or manual override exists yet for this field."
                    : usesSharedFieldEvidence
                        ? "No direct operation-specific token evidence exists yet. These rows come from the same field key observed in other component operations and are therefore inferred for this operation."
                        : "Client-token evidence comes from extracted ability text. Londo rows are last-resort toolkit references and should be treated cautiously.",
                IsFiniteDomain = false,
                Options = options
            };
        }

        public TokenDictionaryDocument BuildTokenDictionary(string extractedRootPath)
        {
            List<TokenDefinitionRecord> definitions = _evidenceByDomainKey
                .SelectMany(pair => pair.Value)
                .GroupBy(row => row.TokenBody, StringComparer.OrdinalIgnoreCase)
                .Select(group =>
                {
                    ComponentTokenEvidence example = group
                        .OrderBy(row => row.AbilityId)
                        .ThenBy(row => row.ComponentSlotIndex)
                        .ThenBy(row => row.ComponentId)
                        .First();

                    return new TokenDefinitionRecord
                    {
                        TokenBody = group.Key,
                        ExampleToken = "COM_" + example.ComponentSlotIndex.ToString(CultureInfo.InvariantCulture) + "_" + group.Key,
                        FieldKey = example.FieldKey,
                        PlainEnglishMeaning = BuildPlainEnglishDefinition(example),
                        Confidence = SemanticConfidence.Confirmed,
                        Source = BuildSourceLabel(group),
                        SourcePath = example.SourcePath,
                        ContextTags = group.SelectMany(row => row.ContextTags ?? new List<string>()).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value).ToList(),
                        Notes = BuildGlossaryNotes(group),
                        ExampleAbilityIds = group.Select(row => row.AbilityId).Distinct().OrderBy(value => value).Take(24).ToList(),
                        Evidence = group
                            .OrderBy(row => row.AbilityId)
                            .ThenBy(row => row.ComponentSlotIndex)
                            .ThenBy(row => row.SourcePath)
                            .ThenBy(row => row.SourceLocation)
                            .Select(row => new TokenEvidenceRecord
                            {
                                ExampleToken = "COM_" + row.ComponentSlotIndex.ToString(CultureInfo.InvariantCulture) + "_" + row.TokenBody,
                                AbilityId = row.AbilityId,
                                ComponentSlotIndex = row.ComponentSlotIndex,
                                ComponentId = row.ComponentId,
                                Operation = row.Operation,
                                Source = row.Source,
                                SourcePath = row.SourcePath,
                                SourceLocation = row.SourceLocation,
                                Meaning = BuildPlainEnglishDefinition(row),
                                AbilityName = row.AbilityName,
                                TextExcerpt = row.TextExcerpt,
                                ContextTags = row.ContextTags == null ? new List<string>() : row.ContextTags.ToList()
                            })
                            .ToList()
                    };
                })
                .OrderBy(row => row.TokenBody, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new TokenDictionaryDocument
            {
                GeneratedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                ExtractedRootPath = extractedRootPath,
                TokenGrammar = "COM_<ComponentSlotIndex>_<TokenBody>",
                TokenGrammarMeaning = "The number after COM is the ability-local component slot index. The suffix identifies which field from that component is being formatted into client text.",
                OverrideLedgerPath = ResolveOverridePath(),
                Definitions = definitions
            };
        }

        public OperationSchemaDocument BuildOperationSchemas(string extractedRootPath)
        {
            List<ComponentOperationSchemaRecord> operations = new List<ComponentOperationSchemaRecord>();
            if (_dataset != null)
            {
                Dictionary<ushort, List<AbilityComponentReference>> referencesByComponentId = BuildAbilityReferencesByComponentId();
                IEnumerable<IGrouping<uint, BinaryComponentRecord>> groupedOperations = _dataset.BinaryComponents
                    .GroupBy(row => row.Operation)
                    .OrderBy(group => PriorityOperations.Contains(group.Key) ? 0 : 1)
                    .ThenBy(group => DefinitionCatalog.DescribeComponentOperationValue(group.Key), StringComparer.OrdinalIgnoreCase)
                    .ThenBy(group => group.Key);

                foreach (IGrouping<uint, BinaryComponentRecord> group in groupedOperations)
                {
                    List<BinaryComponentRecord> components = group
                        .OrderBy(row => row.ComponentId)
                        .ThenBy(row => row.RecordIndex)
                        .ToList();
                    List<AbilityComponentReference> abilityReferences = DistinctAbilityReferences(components, referencesByComponentId);
                    List<ComponentOperationFieldRecord> fields = BuildOperationFieldRecords(group.Key, components, abilityReferences);
                    List<string> contextTags = abilityReferences
                        .SelectMany(row => row.ContextTags ?? new List<string>())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    operations.Add(new ComponentOperationSchemaRecord
                    {
                        OperationId = group.Key,
                        OperationName = DefinitionCatalog.DescribeComponentOperationValue(group.Key),
                        IsPriority = PriorityOperations.Contains(group.Key),
                        ComponentCount = components.Select(row => row.ComponentId).Distinct().Count(),
                        AbilityCount = abilityReferences.Select(row => row.AbilityId).Distinct().Count(),
                        LayoutVariantsText = JoinValues(components.Select(row => row.LayoutVariant).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value, StringComparer.OrdinalIgnoreCase)),
                        ContextTags = contextTags,
                        SampleComponentIds = components.Select(row => row.ComponentId).Distinct().OrderBy(value => value).Take(24).ToList(),
                        Fields = fields,
                        SampleAbilities = abilityReferences
                            .OrderBy(row => row.AbilityId)
                            .ThenBy(row => row.ComponentSlotIndex)
                            .ThenBy(row => row.ComponentId)
                            .Take(48)
                            .Select(row => new ComponentOperationAbilityRecord
                            {
                                AbilityId = row.AbilityId,
                                AbilityName = row.AbilityName,
                                ComponentId = row.ComponentId,
                                ComponentSlotIndex = row.ComponentSlotIndex,
                                TriggerText = row.TriggerText,
                                ContextTagsText = JoinValues(row.ContextTags ?? new List<string>()),
                                TextExcerpt = row.TextExcerpt,
                                SourcePath = row.SourcePath,
                                SourceLocation = row.SourceLocation
                            })
                            .ToList()
                    });
                }
            }

            return new OperationSchemaDocument
            {
                GeneratedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                ExtractedRootPath = extractedRootPath,
                OverrideLedgerPath = ResolveOverridePath(),
                TableStatuses = _dataset == null
                    ? new List<TableLoadStatus>()
                    : _dataset.TableStatuses.OrderBy(row => row.SourceFamily).ThenBy(row => row.TableName).ToList(),
                PriorityOperationIds = PriorityOperations.OrderBy(value => value).ToList(),
                Operations = operations
            };
        }

        public List<ComponentOperationFieldValueRecord> BuildOperationFieldValueEvidence(uint operationId, string fieldKey)
        {
            if (_dataset == null || string.IsNullOrWhiteSpace(fieldKey))
                return new List<ComponentOperationFieldValueRecord>();

            Dictionary<ushort, List<AbilityComponentReference>> referencesByComponentId = BuildAbilityReferencesByComponentId();
            List<FieldValueObservation> observations = new List<FieldValueObservation>();

            foreach (BinaryComponentRecord component in _dataset.BinaryComponents.Where(row => row.Operation == operationId))
            {
                string rawValue;
                if (!TryReadFieldRawValue(component, fieldKey, out rawValue))
                    continue;

                List<AbilityComponentReference> references;
                if (!referencesByComponentId.TryGetValue(component.ComponentId, out references))
                    references = new List<AbilityComponentReference>();

                observations.Add(new FieldValueObservation
                {
                    RawValue = rawValue,
                    ComponentId = component.ComponentId,
                    AbilityReferences = references
                });
            }

            return observations
                .GroupBy(row => row.RawValue, StringComparer.OrdinalIgnoreCase)
                .Select(group =>
                {
                    List<AbilityComponentReference> sampleAbilities = group
                        .SelectMany(row => row.AbilityReferences ?? new List<AbilityComponentReference>())
                        .GroupBy(row => row.AbilityId.ToString(CultureInfo.InvariantCulture)
                            + "|"
                            + row.ComponentId.ToString(CultureInfo.InvariantCulture)
                            + "|"
                            + row.ComponentSlotIndex.ToString(CultureInfo.InvariantCulture), StringComparer.OrdinalIgnoreCase)
                        .Select(match => match.First())
                        .OrderBy(row => row.AbilityId)
                        .ThenBy(row => row.ComponentSlotIndex)
                        .ThenBy(row => row.ComponentId)
                        .Take(24)
                        .ToList();

                    return new ComponentOperationFieldValueRecord
                    {
                        RawValue = group.Key,
                        ObservationCount = group.Count(),
                        DistinctComponentCount = group.Select(row => row.ComponentId).Distinct().Count(),
                        DistinctAbilityCount = sampleAbilities.Select(row => row.AbilityId).Distinct().Count(),
                        SampleComponentIdsText = JoinValues(group.Select(row => row.ComponentId.ToString(CultureInfo.InvariantCulture)).Distinct().OrderBy(value => value).Take(12)),
                        SampleAbilityIdsText = JoinValues(sampleAbilities.Select(row => row.AbilityId.ToString(CultureInfo.InvariantCulture)).Distinct().OrderBy(value => value).Take(12)),
                        SampleAbilities = sampleAbilities.Select(row => new ComponentOperationAbilityRecord
                        {
                            AbilityId = row.AbilityId,
                            AbilityName = row.AbilityName,
                            ComponentId = row.ComponentId,
                            ComponentSlotIndex = row.ComponentSlotIndex,
                            TriggerText = row.TriggerText,
                            ContextTagsText = JoinValues(row.ContextTags ?? new List<string>()),
                            TextExcerpt = row.TextExcerpt,
                            SourcePath = row.SourcePath,
                            SourceLocation = row.SourceLocation
                        }).ToList()
                    };
                })
                .OrderByDescending(row => row.ObservationCount)
                .ThenByDescending(row => row.DistinctAbilityCount)
                .ThenBy(row => row.RawValue, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public ComponentOperationFieldValueInsightRecord BuildOperationFieldValueInsight(uint operationId, string fieldKey, string rawValue)
        {
            ComponentOperationFieldValueInsightRecord empty = new ComponentOperationFieldValueInsightRecord
            {
                OperationId = operationId,
                FieldKey = fieldKey,
                RawValue = rawValue,
                TriggerSummaryText = "No sampled trigger evidence is available for this value yet.",
                ContextTagSummaryText = "No sampled context tags are available for this value yet.",
                CompanionSummaryText = "No strong non-zero companion fields were isolated for this value yet.",
                CompanionFields = new List<ComponentOperationFieldCorrelationRecord>()
            };

            if (_dataset == null || string.IsNullOrWhiteSpace(fieldKey) || string.IsNullOrWhiteSpace(rawValue))
                return empty;

            List<BinaryComponentRecord> matchingComponents = _dataset.BinaryComponents
                .Where(row => row.Operation == operationId)
                .Where(row =>
                {
                    string candidateValue;
                    return TryReadFieldRawValue(row, fieldKey, out candidateValue)
                        && string.Equals(candidateValue, rawValue, StringComparison.OrdinalIgnoreCase);
                })
                .ToList();
            if (matchingComponents.Count == 0)
                return empty;

            Dictionary<ushort, List<AbilityComponentReference>> referencesByComponentId = BuildAbilityReferencesByComponentId();
            List<AbilityComponentReference> abilityReferences = DistinctAbilityReferences(matchingComponents, referencesByComponentId);
            Dictionary<string, List<FieldObservation>> observationsByField = BuildFieldObservationsByField(matchingComponents);

            List<ComponentOperationFieldCorrelationRecord> allCompanionFields = observationsByField
                .Where(pair => !string.Equals(pair.Key, fieldKey, StringComparison.OrdinalIgnoreCase))
                .Select(pair => BuildCompanionFieldCorrelation(pair.Key, pair.Value, matchingComponents.Count))
                .Where(row => row != null)
                .OrderByDescending(row => row.CoveragePercent)
                .ThenByDescending(row => row.MatchCount)
                .ThenByDescending(row => row.ObservationCount)
                .ThenBy(row => GetFieldSortKey(row.FieldKey), StringComparer.OrdinalIgnoreCase)
                .ToList();
            List<ComponentOperationFieldCorrelationRecord> nonMultiplierCompanions = allCompanionFields
                .Where(row => !row.FieldKey.StartsWith("Multiplier[", StringComparison.OrdinalIgnoreCase))
                .ToList();
            List<ComponentOperationFieldCorrelationRecord> companionFields = (nonMultiplierCompanions.Count == 0 ? allCompanionFields : nonMultiplierCompanions)
                .Take(12)
                .ToList();

            List<string> highSignalCompanions = companionFields
                .Where(row => row.CoveragePercent >= 75)
                .Take(5)
                .Select(row => row.FieldKey
                    + "="
                    + row.DominantValue
                    + " ("
                    + row.MatchCount.ToString(CultureInfo.InvariantCulture)
                    + "/"
                    + matchingComponents.Count.ToString(CultureInfo.InvariantCulture)
                    + ", "
                    + row.CoveragePercent.ToString(CultureInfo.InvariantCulture)
                    + "%)")
                .ToList();

            empty.ObservationCount = matchingComponents.Count;
            empty.DistinctComponentCount = matchingComponents.Select(row => row.ComponentId).Distinct().Count();
            empty.DistinctAbilityCount = abilityReferences.Select(row => row.AbilityId).Distinct().Count();
            empty.TriggerSummaryText = BuildValueCountSummary(
                abilityReferences.Select(row => row.TriggerText),
                6,
                "No sampled trigger evidence is available for this value yet.");
            empty.ContextTagSummaryText = BuildValueCountSummary(
                abilityReferences.SelectMany(row => row.ContextTags ?? new List<string>()),
                8,
                "No sampled context tags are available for this value yet.");
            empty.CompanionSummaryText = highSignalCompanions.Count == 0
                ? "No strong non-zero companion fields were isolated for this value yet."
                : string.Join("; ", highSignalCompanions);
            empty.CompanionFields = companionFields;
            return empty;
        }

        private void BuildClientTokenEvidence(AbilityDataset dataset)
        {
            Dictionary<ushort, List<IndexedStringRecord>> descriptionsById = dataset.AbilityDescriptions
                .Where(row => row.EntryId <= ushort.MaxValue)
                .GroupBy(row => (ushort)row.EntryId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.LineNumber).ToList());
            Dictionary<ushort, List<IndexedStringRecord>> effectTextsById = dataset.AbilityEffectTexts
                .Where(row => row.EntryId <= ushort.MaxValue)
                .GroupBy(row => (ushort)row.EntryId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.LineNumber).ToList());

            foreach (BinaryAbilityRecord abilityRow in dataset.BinaryAbilities)
            {
                List<IndexedStringRecord> textRows = new List<IndexedStringRecord>();
                List<IndexedStringRecord> descriptionRows;
                if (descriptionsById.TryGetValue(abilityRow.AbilityId, out descriptionRows))
                    textRows.AddRange(descriptionRows);
                List<IndexedStringRecord> effectRows;
                if (effectTextsById.TryGetValue(abilityRow.AbilityId, out effectRows))
                    textRows.AddRange(effectRows);

                foreach (IndexedStringRecord textRow in textRows)
                {
                    foreach (Match match in ComponentTokenRegex.Matches(textRow.Value ?? string.Empty))
                    {
                        int componentSlotIndex;
                        if (!int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out componentSlotIndex))
                            continue;
                        if (componentSlotIndex < 0 || componentSlotIndex >= abilityRow.ComponentIds.Count)
                            continue;

                        ushort componentId = abilityRow.ComponentIds[componentSlotIndex];
                        if (componentId <= 0)
                            continue;

                        BinaryComponentRecord componentRow;
                        if (!_componentsById.TryGetValue(componentId, out componentRow))
                            continue;

                        string tokenBody = match.Groups[2].Value;
                        ComponentFieldDescriptor descriptor;
                        if (!TryParseTokenField(tokenBody, componentRow.Operation, out descriptor))
                            continue;

                        ComponentTokenEvidence evidence = new ComponentTokenEvidence
                        {
                            AbilityId = abilityRow.AbilityId,
                            AbilityName = LookupAbilityName(abilityRow.AbilityId),
                            ComponentId = componentId,
                            ComponentSlotIndex = componentSlotIndex,
                            Operation = componentRow.Operation,
                            DomainKey = descriptor.DomainKey,
                            FieldKey = descriptor.FieldKey,
                            TokenBody = tokenBody,
                            Meaning = descriptor.TokenMeaning,
                            Source = textRow.TableName,
                            SourcePath = textRow.SourcePath,
                            SourceLocation = "line " + textRow.LineNumber.ToString(CultureInfo.InvariantCulture),
                            TextExcerpt = textRow.NormalizedValue,
                            ContextTags = BuildContextTags(textRow.NormalizedValue)
                        };

                        AddEvidence(_evidenceByDomainKey, descriptor.DomainKey, evidence);
                        AddEvidence(_evidenceByFieldKey, descriptor.FieldKey, evidence);
                        AddEvidence(_evidenceByAbilityComponentField, BuildAbilityComponentFieldKey(abilityRow.AbilityId, componentId, descriptor.FieldKey), evidence);
                    }
                }
            }
        }

        private Dictionary<ushort, List<AbilityComponentReference>> BuildAbilityReferencesByComponentId()
        {
            Dictionary<ushort, List<AbilityComponentReference>> referencesByComponentId = new Dictionary<ushort, List<AbilityComponentReference>>();
            if (_dataset == null)
                return referencesByComponentId;

            Dictionary<ushort, List<IndexedStringRecord>> textRowsByAbilityId = BuildAbilityTextRowsByAbilityId();
            foreach (BinaryAbilityRecord abilityRow in _dataset.BinaryAbilities.OrderBy(row => row.AbilityId).ThenBy(row => row.RecordIndex))
            {
                List<IndexedStringRecord> textRows;
                textRowsByAbilityId.TryGetValue(abilityRow.AbilityId, out textRows);

                for (int slotIndex = 0; slotIndex < abilityRow.ComponentIds.Count; ++slotIndex)
                {
                    ushort componentId = abilityRow.ComponentIds[slotIndex];
                    if (componentId <= 0)
                        continue;

                    AbilityTextSelection textSelection = SelectAbilityText(textRows, slotIndex);
                    AbilityComponentReference reference = new AbilityComponentReference
                    {
                        AbilityId = abilityRow.AbilityId,
                        AbilityName = LookupAbilityName(abilityRow.AbilityId),
                        ComponentId = componentId,
                        ComponentSlotIndex = slotIndex,
                        TriggerValue = slotIndex < abilityRow.Triggers.Count ? abilityRow.Triggers[slotIndex] : 0,
                        TriggerText = BuildTriggerText(slotIndex < abilityRow.Triggers.Count ? abilityRow.Triggers[slotIndex] : 0),
                        TextExcerpt = textSelection.TextExcerpt,
                        SourcePath = string.IsNullOrWhiteSpace(textSelection.SourcePath) ? abilityRow.SourcePath : textSelection.SourcePath,
                        SourceLocation = string.IsNullOrWhiteSpace(textSelection.SourceLocation) ? "byte " + abilityRow.ByteOffset.ToString(CultureInfo.InvariantCulture) : textSelection.SourceLocation,
                        ContextTags = textSelection.ContextTags
                    };

                    AddReference(referencesByComponentId, componentId, reference);
                }
            }

            return referencesByComponentId;
        }

        private Dictionary<ushort, List<IndexedStringRecord>> BuildAbilityTextRowsByAbilityId()
        {
            Dictionary<ushort, List<IndexedStringRecord>> rowsByAbilityId = new Dictionary<ushort, List<IndexedStringRecord>>();
            if (_dataset == null)
                return rowsByAbilityId;

            foreach (IndexedStringRecord row in _dataset.AbilityDescriptions.Where(row => row.EntryId <= ushort.MaxValue).OrderBy(row => row.EntryId).ThenBy(row => row.LineNumber))
                AddTextRow(rowsByAbilityId, (ushort)row.EntryId, row);
            foreach (IndexedStringRecord row in _dataset.AbilityEffectTexts.Where(row => row.EntryId <= ushort.MaxValue).OrderBy(row => row.EntryId).ThenBy(row => row.LineNumber))
                AddTextRow(rowsByAbilityId, (ushort)row.EntryId, row);

            return rowsByAbilityId;
        }

        private static void AddTextRow(Dictionary<ushort, List<IndexedStringRecord>> rowsByAbilityId, ushort abilityId, IndexedStringRecord row)
        {
            List<IndexedStringRecord> rows;
            if (!rowsByAbilityId.TryGetValue(abilityId, out rows))
            {
                rows = new List<IndexedStringRecord>();
                rowsByAbilityId[abilityId] = rows;
            }

            rows.Add(row);
        }

        private static AbilityTextSelection SelectAbilityText(List<IndexedStringRecord> rows, int slotIndex)
        {
            string tokenNeedle = "COM_" + slotIndex.ToString(CultureInfo.InvariantCulture) + "_";
            IndexedStringRecord preferred = rows == null
                ? null
                : rows.FirstOrDefault(row => !string.IsNullOrWhiteSpace(row.NormalizedValue) && row.NormalizedValue.IndexOf(tokenNeedle, StringComparison.OrdinalIgnoreCase) >= 0);
            if (preferred == null && rows != null)
                preferred = rows.FirstOrDefault(row => !string.IsNullOrWhiteSpace(row.NormalizedValue));

            string excerpt = preferred == null ? string.Empty : preferred.NormalizedValue;
            return new AbilityTextSelection
            {
                TextExcerpt = excerpt,
                SourcePath = preferred == null ? string.Empty : preferred.SourcePath,
                SourceLocation = preferred == null ? string.Empty : "line " + preferred.LineNumber.ToString(CultureInfo.InvariantCulture),
                ContextTags = BuildContextTags(excerpt)
            };
        }

        private static void AddReference(Dictionary<ushort, List<AbilityComponentReference>> referencesByComponentId, ushort componentId, AbilityComponentReference reference)
        {
            List<AbilityComponentReference> items;
            if (!referencesByComponentId.TryGetValue(componentId, out items))
            {
                items = new List<AbilityComponentReference>();
                referencesByComponentId[componentId] = items;
            }

            items.Add(reference);
        }

        private static List<AbilityComponentReference> DistinctAbilityReferences(IEnumerable<BinaryComponentRecord> components, Dictionary<ushort, List<AbilityComponentReference>> referencesByComponentId)
        {
            Dictionary<string, AbilityComponentReference> unique = new Dictionary<string, AbilityComponentReference>(StringComparer.OrdinalIgnoreCase);
            foreach (BinaryComponentRecord component in components)
            {
                List<AbilityComponentReference> references;
                if (!referencesByComponentId.TryGetValue(component.ComponentId, out references))
                    continue;

                foreach (AbilityComponentReference reference in references)
                {
                    string key = reference.AbilityId.ToString(CultureInfo.InvariantCulture)
                        + "|"
                        + reference.ComponentId.ToString(CultureInfo.InvariantCulture)
                        + "|"
                        + reference.ComponentSlotIndex.ToString(CultureInfo.InvariantCulture);
                    if (!unique.ContainsKey(key))
                        unique[key] = reference;
                }
            }

            return unique.Values.ToList();
        }

        private List<ComponentOperationFieldRecord> BuildOperationFieldRecords(uint operationId, List<BinaryComponentRecord> components, List<AbilityComponentReference> abilityReferences)
        {
            Dictionary<string, List<FieldObservation>> observationsByField = BuildFieldObservationsByField(components);

            List<string> operationContextTags = abilityReferences
                .SelectMany(row => row.ContextTags ?? new List<string>())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                .ToList();
            int operationAbilityCount = abilityReferences
                .Select(row => row.AbilityId)
                .Distinct()
                .Count();
            List<ComponentOperationFieldRecord> rows = new List<ComponentOperationFieldRecord>();
            foreach (KeyValuePair<string, List<FieldObservation>> pair in observationsByField.OrderBy(entry => GetFieldSortKey(entry.Key), StringComparer.OrdinalIgnoreCase))
            {
                string domainKey = BuildOperationFieldDomainKey(operationId, pair.Key);
                DefinitionDomain domain = BuildDomain(domainKey);
                FieldObservationInference inference = BuildFieldObservationInference(operationId, pair.Key, pair.Value);
                List<string> tokenRenderings = domain == null
                    ? new List<string>()
                    : domain.Options.Where(option => !string.Equals(option.RawValue, "override", StringComparison.OrdinalIgnoreCase)).Select(option => option.RawValue).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToList();
                List<string> semanticMeanings = domain == null
                    ? new List<string>()
                    : domain.Options.Select(option => option.Meaning).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToList();
                List<string> domainContextTags = GetDomainContextTags(domainKey);
                string notes = BuildOperationFieldNotes(pair.Key, pair.Value, domain, components.Count);
                if (inference != null && !string.IsNullOrWhiteSpace(inference.Notes))
                    notes += " " + inference.Notes;
                string confidence = semanticMeanings.Count == 0 && inference != null ? inference.Confidence : DetermineDomainConfidence(domain);
                FieldTriage triage = BuildFieldTriage(operationId, pair.Key, pair.Value, confidence, operationAbilityCount);

                rows.Add(new ComponentOperationFieldRecord
                {
                    FieldKey = pair.Key,
                    NonZeroCount = pair.Value.Count,
                    DistinctValueCount = pair.Value.Select(row => row.RawValue).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                    SampleValuesText = JoinValues(pair.Value.Select(row => row.RawValue).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value, StringComparer.OrdinalIgnoreCase).Take(12)),
                    TokenRenderingsText = tokenRenderings.Count == 0 ? "(none)" : JoinValues(tokenRenderings),
                    SemanticSummary = semanticMeanings.Count == 0
                        ? inference == null ? "No extracted-client semantic mapping is known yet for this field." : inference.SemanticSummary
                        : string.Join(" ", semanticMeanings),
                    Confidence = confidence,
                    ContextTagsText = JoinValues(domainContextTags.Count == 0 ? operationContextTags : domainContextTags),
                    Notes = notes,
                    TriageScore = triage.Score,
                    TriageBucket = triage.Bucket,
                    TriageNotes = triage.Notes
                });
            }

            return rows;
        }

        private static Dictionary<string, List<FieldObservation>> BuildFieldObservationsByField(IEnumerable<BinaryComponentRecord> components)
        {
            Dictionary<string, List<FieldObservation>> observationsByField = new Dictionary<string, List<FieldObservation>>(StringComparer.OrdinalIgnoreCase);
            foreach (BinaryComponentRecord component in components ?? Enumerable.Empty<BinaryComponentRecord>())
            {
                AddFieldObservation(observationsByField, "ActivationDelay", component.ComponentId, component.LayoutVariant, component.ActivationDelay);
                AddFieldObservation(observationsByField, "Duration", component.ComponentId, component.LayoutVariant, component.Duration);
                AddFieldObservation(observationsByField, "FlagsRaw", component.ComponentId, component.LayoutVariant, component.FlagsRaw);
                AddFieldObservation(observationsByField, "Value08", component.ComponentId, component.LayoutVariant, component.Value08);
                AddFieldObservation(observationsByField, "Radius", component.ComponentId, component.LayoutVariant, component.Radius);
                AddFieldObservation(observationsByField, "ConeAngle", component.ComponentId, component.LayoutVariant, component.ConeAngle);
                AddFieldObservation(observationsByField, "FlightSpeed", component.ComponentId, component.LayoutVariant, component.FlightSpeed);
                AddFieldObservation(observationsByField, "Value15", component.ComponentId, component.LayoutVariant, component.Value15);
                AddFieldObservation(observationsByField, "MaxTargets", component.ComponentId, component.LayoutVariant, component.MaxTargets);
                AddFieldObservation(observationsByField, "Value00", component.ComponentId, component.LayoutVariant, component.Value00);
                AddFieldObservation(observationsByField, "Interval", component.ComponentId, component.LayoutVariant, component.Interval);

                for (int index = 0; index < component.Values.Count; ++index)
                    AddFieldObservation(observationsByField, "Value[" + index.ToString(CultureInfo.InvariantCulture) + "]", component.ComponentId, component.LayoutVariant, component.Values[index]);
                for (int index = 0; index < component.Multipliers.Count; ++index)
                    AddFieldObservation(observationsByField, "Multiplier[" + index.ToString(CultureInfo.InvariantCulture) + "]", component.ComponentId, component.LayoutVariant, component.Multipliers[index]);

                foreach (BinaryExtDataRecord extData in component.ExtData)
                {
                    AddFieldObservation(observationsByField, BuildExtDataFieldKey(extData.SlotIndex, 1), component.ComponentId, component.LayoutVariant, extData.Val1);
                    AddFieldObservation(observationsByField, BuildExtDataFieldKey(extData.SlotIndex, 2), component.ComponentId, component.LayoutVariant, extData.Val2);
                    AddFieldObservation(observationsByField, BuildExtDataFieldKey(extData.SlotIndex, 3), component.ComponentId, component.LayoutVariant, extData.Val3);
                    AddFieldObservation(observationsByField, BuildExtDataFieldKey(extData.SlotIndex, 4), component.ComponentId, component.LayoutVariant, extData.Val4);
                    AddFieldObservation(observationsByField, BuildExtDataFieldKey(extData.SlotIndex, 5), component.ComponentId, component.LayoutVariant, extData.Val5);
                    AddFieldObservation(observationsByField, BuildExtDataFieldKey(extData.SlotIndex, 6), component.ComponentId, component.LayoutVariant, extData.Val6);
                    AddFieldObservation(observationsByField, BuildExtDataFieldKey(extData.SlotIndex, 7), component.ComponentId, component.LayoutVariant, extData.Val7);
                    AddFieldObservation(observationsByField, BuildExtDataFieldKey(extData.SlotIndex, 8), component.ComponentId, component.LayoutVariant, extData.Val8);
                    AddFieldObservation(observationsByField, BuildExtDataFieldKey(extData.SlotIndex, 9), component.ComponentId, component.LayoutVariant, extData.Val9);
                }
            }

            return observationsByField;
        }

        private static void AddFieldObservation(Dictionary<string, List<FieldObservation>> observationsByField, string fieldKey, ushort componentId, string layoutVariant, uint rawValue)
        {
            if (rawValue == 0)
                return;

            AddFieldObservation(observationsByField, fieldKey, componentId, layoutVariant, rawValue.ToString(CultureInfo.InvariantCulture));
        }

        private static void AddFieldObservation(Dictionary<string, List<FieldObservation>> observationsByField, string fieldKey, ushort componentId, string layoutVariant, int rawValue)
        {
            if (rawValue == 0)
                return;

            AddFieldObservation(observationsByField, fieldKey, componentId, layoutVariant, rawValue.ToString(CultureInfo.InvariantCulture));
        }

        private static void AddFieldObservation(Dictionary<string, List<FieldObservation>> observationsByField, string fieldKey, ushort componentId, string layoutVariant, byte rawValue)
        {
            if (rawValue == 0)
                return;

            AddFieldObservation(observationsByField, fieldKey, componentId, layoutVariant, rawValue.ToString(CultureInfo.InvariantCulture));
        }

        private static void AddFieldObservation(Dictionary<string, List<FieldObservation>> observationsByField, string fieldKey, ushort componentId, string layoutVariant, string rawValue)
        {
            if (string.IsNullOrWhiteSpace(fieldKey) || string.IsNullOrWhiteSpace(rawValue))
                return;

            List<FieldObservation> rows;
            if (!observationsByField.TryGetValue(fieldKey, out rows))
            {
                rows = new List<FieldObservation>();
                observationsByField[fieldKey] = rows;
            }

            rows.Add(new FieldObservation
            {
                FieldKey = fieldKey,
                RawValue = rawValue,
                ComponentId = componentId,
                LayoutVariant = layoutVariant
            });
        }

        private static string BuildExtDataFieldKey(int slotIndex, int valueIndex)
        {
            return "ExtData[" + slotIndex.ToString(CultureInfo.InvariantCulture) + "].Val" + valueIndex.ToString(CultureInfo.InvariantCulture);
        }

        private static bool TryReadFieldRawValue(BinaryComponentRecord component, string fieldKey, out string rawValue)
        {
            rawValue = null;
            if (component == null || string.IsNullOrWhiteSpace(fieldKey))
                return false;

            if (string.Equals(fieldKey, "ActivationDelay", StringComparison.OrdinalIgnoreCase))
                return TryFormatFieldValue(component.ActivationDelay, out rawValue);
            if (string.Equals(fieldKey, "Duration", StringComparison.OrdinalIgnoreCase))
                return TryFormatFieldValue(component.Duration, out rawValue);
            if (string.Equals(fieldKey, "FlagsRaw", StringComparison.OrdinalIgnoreCase))
                return TryFormatFieldValue(component.FlagsRaw, out rawValue);
            if (string.Equals(fieldKey, "Value08", StringComparison.OrdinalIgnoreCase))
                return TryFormatFieldValue(component.Value08, out rawValue);
            if (string.Equals(fieldKey, "Radius", StringComparison.OrdinalIgnoreCase))
                return TryFormatFieldValue(component.Radius, out rawValue);
            if (string.Equals(fieldKey, "ConeAngle", StringComparison.OrdinalIgnoreCase))
                return TryFormatFieldValue(component.ConeAngle, out rawValue);
            if (string.Equals(fieldKey, "FlightSpeed", StringComparison.OrdinalIgnoreCase))
                return TryFormatFieldValue(component.FlightSpeed, out rawValue);
            if (string.Equals(fieldKey, "Value15", StringComparison.OrdinalIgnoreCase))
                return TryFormatFieldValue(component.Value15, out rawValue);
            if (string.Equals(fieldKey, "MaxTargets", StringComparison.OrdinalIgnoreCase))
                return TryFormatFieldValue(component.MaxTargets, out rawValue);
            if (string.Equals(fieldKey, "Value00", StringComparison.OrdinalIgnoreCase))
                return TryFormatFieldValue(component.Value00, out rawValue);
            if (string.Equals(fieldKey, "Interval", StringComparison.OrdinalIgnoreCase))
                return TryFormatFieldValue(component.Interval, out rawValue);

            int index;
            if (TryParseIndexedField(fieldKey, "Value[", out index))
            {
                if (index >= 0 && index < component.Values.Count)
                    return TryFormatFieldValue(component.Values[index], out rawValue);
                return false;
            }

            if (TryParseIndexedField(fieldKey, "Multiplier[", out index))
            {
                if (index >= 0 && index < component.Multipliers.Count)
                    return TryFormatFieldValue(component.Multipliers[index], out rawValue);
                return false;
            }

            int slotIndex;
            int valueIndex;
            if (TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
            {
                BinaryExtDataRecord extRecord = component.ExtData.FirstOrDefault(row => row.SlotIndex == slotIndex);
                if (extRecord == null)
                    return false;

                switch (valueIndex)
                {
                    case 1: return TryFormatFieldValue(extRecord.Val1, out rawValue);
                    case 2: return TryFormatFieldValue(extRecord.Val2, out rawValue);
                    case 3: return TryFormatFieldValue(extRecord.Val3, out rawValue);
                    case 4: return TryFormatFieldValue(extRecord.Val4, out rawValue);
                    case 5: return TryFormatFieldValue(extRecord.Val5, out rawValue);
                    case 6: return TryFormatFieldValue(extRecord.Val6, out rawValue);
                    case 7: return TryFormatFieldValue(extRecord.Val7, out rawValue);
                    case 8: return TryFormatFieldValue(extRecord.Val8, out rawValue);
                    case 9: return TryFormatFieldValue(extRecord.Val9, out rawValue);
                }
            }

            return false;
        }

        private static bool TryParseIndexedField(string fieldKey, string prefix, out int index)
        {
            index = -1;
            if (string.IsNullOrWhiteSpace(fieldKey)
                || string.IsNullOrWhiteSpace(prefix)
                || !fieldKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                || !fieldKey.EndsWith("]", StringComparison.OrdinalIgnoreCase))
                return false;

            return int.TryParse(fieldKey.Substring(prefix.Length, fieldKey.Length - prefix.Length - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out index);
        }

        private static bool TryParseExtDataField(string fieldKey, out int slotIndex, out int valueIndex)
        {
            slotIndex = -1;
            valueIndex = -1;
            if (string.IsNullOrWhiteSpace(fieldKey)
                || !fieldKey.StartsWith("ExtData[", StringComparison.OrdinalIgnoreCase))
                return false;

            int closeBracket = fieldKey.IndexOf(']');
            if (closeBracket <= "ExtData[".Length)
                return false;

            int dotIndex = fieldKey.IndexOf(".Val", StringComparison.OrdinalIgnoreCase);
            if (dotIndex <= closeBracket)
                return false;

            return int.TryParse(fieldKey.Substring("ExtData[".Length, closeBracket - "ExtData[".Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out slotIndex)
                && int.TryParse(fieldKey.Substring(dotIndex + 4), NumberStyles.Integer, CultureInfo.InvariantCulture, out valueIndex);
        }

        private static bool TryDescribeDamageExtDataField(int valueIndex, out string semanticSummary, out string roleNotes)
        {
            semanticSummary = null;
            roleNotes = null;

            switch (valueIndex)
            {
                case 1:
                    semanticSummary = "DAMAGE ext-data branch selector for the slot-local damage payload.";
                    roleNotes = "This low-cardinality field splits the dominant DAMAGE payload branches and tracks closely with the neighboring Val2, Val3, and Val7 fields.";
                    return true;
                case 2:
                    semanticSummary = "DAMAGE ext-data payload-A field for the slot-local damage payload.";
                    roleNotes = "This field mixes compact selector-like values with scalar-looking payloads, so it needs to be read together with the neighboring Val1, Val3, and Val7 fields.";
                    return true;
                case 3:
                    semanticSummary = "DAMAGE ext-data profile selector for the slot-local damage payload.";
                    roleNotes = "This compact enum-like field behaves like the main profile selector inside the recurring DAMAGE ext-data block.";
                    return true;
                case 4:
                    semanticSummary = "DAMAGE ext-data family marker for the slot-local damage payload.";
                    roleNotes = "This field behaves like a layout-family tag rather than a free scalar and overwhelmingly stays in the 8-family with a small 9-variant minority.";
                    return true;
                case 5:
                    semanticSummary = "DAMAGE ext-data auxiliary selector for minority slot-local damage branches.";
                    roleNotes = "This low-cardinality field only appears in minority DAMAGE ext-data branches and behaves more like an auxiliary selector or modifier than a wide scalar.";
                    return true;
                case 6:
                    semanticSummary = "DAMAGE ext-data reference/link field for the slot-local damage payload.";
                    roleNotes = "This field behaves like a branch-local reference slot: some DAMAGE branches carry exact RequirementId matches here, while other branches carry identifier-like values instead of scalar payloads.";
                    return true;
                case 7:
                    semanticSummary = "DAMAGE ext-data payload-B field for the slot-local damage payload.";
                    roleNotes = "This field behaves like the second profile-specific payload slot and carries the widest scalar-looking values in the recurring DAMAGE ext-data block.";
                    return true;
                case 8:
                    semanticSummary = "DAMAGE ext-data sparse tail payload field for minority slot-local damage branches.";
                    roleNotes = "This field only appears in minority DAMAGE ext-data branches after the main Val7 payload and behaves like an auxiliary tail parameter.";
                    return true;
                case 9:
                    semanticSummary = "DAMAGE ext-data slot-presence marker for minority slot-local damage branches.";
                    roleNotes = "This field is almost always fixed at 1 when present, so it behaves like a branch marker or enabled-flag rather than a free scalar.";
                    return true;
                default:
                    return false;
            }
        }

        private static bool TryDescribeBonusTypeAdjustExtDataField(int valueIndex, out string semanticSummary, out string roleNotes)
        {
            semanticSummary = null;
            roleNotes = null;

            switch (valueIndex)
            {
                case 1:
                    semanticSummary = "BONUS_TYPE_ADJUST ext-data branch selector for the slot-local bonus payload.";
                    roleNotes = "This low-cardinality field splits the dominant BONUS_TYPE_ADJUST payload branches and tracks closely with the neighboring Val2, Val3, and Val7 fields.";
                    return true;
                case 2:
                    semanticSummary = "BONUS_TYPE_ADJUST ext-data payload-A field for the slot-local bonus payload.";
                    roleNotes = "This field mixes compact selector-like values with scalar-looking payloads, so it needs to be read together with the neighboring Val1, Val3, and Val7 fields.";
                    return true;
                case 3:
                    semanticSummary = "BONUS_TYPE_ADJUST ext-data profile selector for the slot-local bonus payload.";
                    roleNotes = "This compact enum-like field behaves like the main profile selector inside the recurring BONUS_TYPE_ADJUST ext-data block.";
                    return true;
                case 4:
                    semanticSummary = "BONUS_TYPE_ADJUST ext-data family marker for the slot-local bonus payload.";
                    roleNotes = "This field behaves like a layout-family tag rather than a free scalar and overwhelmingly stays in the 8-family with a small 9-variant minority.";
                    return true;
                case 5:
                    semanticSummary = "BONUS_TYPE_ADJUST ext-data auxiliary selector for minority slot-local bonus branches.";
                    roleNotes = "This low-cardinality field only appears in minority BONUS_TYPE_ADJUST ext-data branches and behaves more like an auxiliary selector or modifier than a wide scalar.";
                    return true;
                case 6:
                    semanticSummary = "BONUS_TYPE_ADJUST ext-data reference/link field for the slot-local bonus payload.";
                    roleNotes = "This field behaves like a branch-local reference slot: some BONUS_TYPE_ADJUST branches carry exact RequirementId matches here, while other branches carry identifier-like values instead of scalar payloads.";
                    return true;
                case 7:
                    semanticSummary = "BONUS_TYPE_ADJUST ext-data payload-B field for the slot-local bonus payload.";
                    roleNotes = "This field behaves like the second profile-specific payload slot and carries the widest scalar-looking values in the recurring BONUS_TYPE_ADJUST ext-data block.";
                    return true;
                case 8:
                    semanticSummary = "BONUS_TYPE_ADJUST ext-data sparse tail payload field for minority slot-local bonus branches.";
                    roleNotes = "This field only appears in minority BONUS_TYPE_ADJUST ext-data branches after the main Val7 payload and behaves like an auxiliary tail parameter.";
                    return true;
                case 9:
                    semanticSummary = "BONUS_TYPE_ADJUST ext-data slot-presence marker for minority slot-local bonus branches.";
                    roleNotes = "This field is almost always fixed at 1 when present, so it behaves like a branch marker or enabled-flag rather than a free scalar.";
                    return true;
                default:
                    return false;
            }
        }

        private static bool TryDescribeApplyAbilityExtDataField(int valueIndex, out string semanticSummary, out string roleNotes)
        {
            semanticSummary = null;
            roleNotes = null;

            switch (valueIndex)
            {
                case 1:
                    semanticSummary = "APPLY_ABILITY ext-data branch selector for the slot-local embedded payload.";
                    roleNotes = "This low-cardinality field splits the dominant APPLY_ABILITY payload branches, but the exact retail meaning of each raw value is still unresolved.";
                    return true;
                case 2:
                    semanticSummary = "APPLY_ABILITY ext-data operation-type code for the slot-local embedded payload.";
                    roleNotes = "This field consistently holds a ComponentOperation-family type code across extracted client BIN data. Values in the known range map to ComponentOperation names; values above the known range are undocumented operation types.";
                    return true;
                case 3:
                    semanticSummary = "APPLY_ABILITY ext-data profile selector for the slot-local embedded payload.";
                    roleNotes = "This compact enum-like field is the clearest profile or behavior selector inside the APPLY_ABILITY ext-data block.";
                    return true;
                case 4:
                    semanticSummary = "APPLY_ABILITY ext-data family marker for the slot-local embedded payload.";
                    roleNotes = "This field behaves like a payload-layout tag rather than a free scalar; extracted rows overwhelmingly use one dominant value with a small minority variant.";
                    return true;
                case 5:
                    semanticSummary = "APPLY_ABILITY ext-data auxiliary selector for minority slot-local payload branches.";
                    roleNotes = "This low-cardinality field only appears in minority APPLY_ABILITY ext-data branches and behaves more like an auxiliary selector or modifier than a wide scalar.";
                    return true;
                case 6:
                    semanticSummary = "APPLY_ABILITY ext-data reference/link field for the slot-local embedded payload.";
                    roleNotes = "This field behaves like a branch-local reference slot: some neighboring APPLY_ABILITY branches carry exact RequirementId matches here, while other branches carry large identifier-like values instead of scalar payloads.";
                    return true;
                case 7:
                    semanticSummary = "APPLY_ABILITY ext-data payload-B field for the slot-local embedded payload.";
                    roleNotes = "This field behaves like the second profile-specific payload slot and varies far more widely than the small selector fields around it.";
                    return true;
                case 8:
                    semanticSummary = "APPLY_ABILITY ext-data sparse tail payload field for minority slot-local payload branches.";
                    roleNotes = "This field only appears in minority APPLY_ABILITY ext-data branches after the main Val7 payload and behaves like an auxiliary tail parameter.";
                    return true;
                case 9:
                    semanticSummary = "APPLY_ABILITY ext-data slot-presence marker for minority slot-local payload branches.";
                    roleNotes = "This field is almost always fixed at 1 when present, so it behaves like a branch marker or enabled-flag rather than a free scalar.";
                    return true;
                default:
                    return false;
            }
        }

        private static bool TryDescribeCcExtDataField(int valueIndex, out string semanticSummary, out string roleNotes)
        {
            semanticSummary = null;
            roleNotes = null;

            switch (valueIndex)
            {
                case 1:
                    semanticSummary = "CC ext-data branch selector for the slot-local control payload.";
                    roleNotes = "This low-cardinality field splits the dominant CC payload branches and tracks closely with the neighboring Val2, Val3, and Val7 fields.";
                    return true;
                case 2:
                    semanticSummary = "CC ext-data operation-type code for the slot-local control payload.";
                    roleNotes = "This field consistently holds a ComponentOperation-family type code across extracted client BIN data. Values in the known range map to ComponentOperation names; values above the known range are undocumented operation types.";
                    return true;
                case 3:
                    semanticSummary = "CC ext-data profile selector for the slot-local control payload.";
                    roleNotes = "This compact enum-like field behaves like the main profile selector inside the recurring CC ext-data block.";
                    return true;
                case 4:
                    semanticSummary = "CC ext-data family marker for the slot-local control payload.";
                    roleNotes = "This field is almost fixed at 8 with a minority 9 variant, so it behaves like a layout-family tag rather than a free scalar.";
                    return true;
                case 5:
                    semanticSummary = "CC ext-data auxiliary selector for minority slot-local control branches.";
                    roleNotes = "This low-cardinality field only appears in minority CC ext-data branches and behaves more like an auxiliary selector or modifier than a wide scalar.";
                    return true;
                case 6:
                    semanticSummary = "CC ext-data reference/link field for the slot-local control payload.";
                    roleNotes = "This field behaves like a branch-local reference slot: some nearby CC branches carry exact RequirementId matches here, while other branches carry identifier-like values instead of scalar payloads.";
                    return true;
                case 7:
                    semanticSummary = "CC ext-data payload-B field for the slot-local control payload.";
                    roleNotes = "This field behaves like the second profile-specific payload slot and carries the widest scalar-looking values in the recurring CC ext-data block.";
                    return true;
                case 8:
                    semanticSummary = "CC ext-data sparse tail payload field for minority slot-local control branches.";
                    roleNotes = "This field only appears in minority CC ext-data branches after the main Val7 payload and behaves like an auxiliary tail parameter.";
                    return true;
                case 9:
                    semanticSummary = "CC ext-data slot-presence marker for minority slot-local control branches.";
                    roleNotes = "This field is almost always fixed at 1 when present, so it behaves like a branch marker or enabled-flag rather than a free scalar.";
                    return true;
                default:
                    return false;
            }
        }

        private static bool TryDescribeKnockbackExtDataField(int valueIndex, out string semanticSummary, out string roleNotes)
        {
            semanticSummary = null;
            roleNotes = null;

            switch (valueIndex)
            {
                case 1:
                    semanticSummary = "KNOCKBACK ext-data branch selector for the slot-local displacement payload.";
                    roleNotes = "This low-cardinality field splits the dominant KNOCKBACK payload branches and tracks closely with the neighboring Val2, Val3, and Val7 fields.";
                    return true;
                case 2:
                    semanticSummary = "KNOCKBACK ext-data payload-A field for the slot-local displacement payload.";
                    roleNotes = "This field mixes compact selector-like values with scalar-looking payloads, so it needs to be read together with the neighboring Val1, Val3, and Val7 fields.";
                    return true;
                case 3:
                    semanticSummary = "KNOCKBACK ext-data profile selector for the slot-local displacement payload.";
                    roleNotes = "This compact enum-like field behaves like the main profile selector inside the recurring KNOCKBACK ext-data block.";
                    return true;
                case 4:
                    semanticSummary = "KNOCKBACK ext-data family marker for the slot-local displacement payload.";
                    roleNotes = "This field behaves like a layout-family tag rather than a free scalar and overwhelmingly stays in the 8-family with small minority variants.";
                    return true;
                case 5:
                    semanticSummary = "KNOCKBACK ext-data auxiliary selector for minority slot-local displacement branches.";
                    roleNotes = "This low-cardinality field only appears in minority KNOCKBACK ext-data branches and behaves more like an auxiliary selector or modifier than a wide scalar.";
                    return true;
                case 6:
                    semanticSummary = "KNOCKBACK ext-data reference/link field for the slot-local displacement payload.";
                    roleNotes = "This field behaves like a branch-local reference slot: some KNOCKBACK branches carry exact RequirementId matches here, while other branches carry identifier-like values instead of scalar payloads.";
                    return true;
                case 7:
                    semanticSummary = "KNOCKBACK ext-data payload-B field for the slot-local displacement payload.";
                    roleNotes = "This field behaves like the second profile-specific payload slot and carries the widest scalar-looking values in the recurring KNOCKBACK ext-data block.";
                    return true;
                case 8:
                    semanticSummary = "KNOCKBACK ext-data sparse tail payload field for minority slot-local displacement branches.";
                    roleNotes = "This field only appears in minority KNOCKBACK ext-data branches after the main Val7 payload and behaves like an auxiliary tail parameter.";
                    return true;
                case 9:
                    semanticSummary = "KNOCKBACK ext-data slot-presence marker for minority slot-local displacement branches.";
                    roleNotes = "This field is almost always fixed at 1 when present, so it behaves like a branch marker or enabled-flag rather than a free scalar.";
                    return true;
                default:
                    return false;
            }
        }

        private static bool TryDescribeImmunityExtDataField(int valueIndex, out string semanticSummary, out string roleNotes)
        {
            semanticSummary = null;
            roleNotes = null;

            switch (valueIndex)
            {
                case 1:
                    semanticSummary = "IMMUNITY ext-data branch selector for the slot-local immunity payload.";
                    roleNotes = "This low-cardinality field splits the dominant IMMUNITY payload branches and tracks closely with the neighboring Val2, Val3, and Val7 fields.";
                    return true;
                case 2:
                    semanticSummary = "IMMUNITY ext-data payload-A field for the slot-local immunity payload.";
                    roleNotes = "This field mixes compact selector-like values with scalar-looking payloads, so it needs to be read together with the neighboring Val1, Val3, and Val7 fields.";
                    return true;
                case 3:
                    semanticSummary = "IMMUNITY ext-data profile selector for the slot-local immunity payload.";
                    roleNotes = "This compact enum-like field behaves like the main profile selector inside the recurring IMMUNITY ext-data block and drives the dominant IMMUNITY layout family.";
                    return true;
                case 4:
                    semanticSummary = "IMMUNITY ext-data family marker for the slot-local immunity payload.";
                    roleNotes = "This field behaves like a layout-family tag rather than a free scalar and stays almost entirely in the 8-family with a minority 9-variant.";
                    return true;
                case 5:
                    semanticSummary = "IMMUNITY ext-data auxiliary selector for minority slot-local immunity branches.";
                    roleNotes = "This low-cardinality field only appears in minority IMMUNITY ext-data branches and behaves more like an auxiliary selector or modifier than a wide scalar.";
                    return true;
                case 6:
                    semanticSummary = "IMMUNITY ext-data reference/link field for the slot-local immunity payload.";
                    roleNotes = "This field behaves like a branch-local reference slot: some IMMUNITY branches carry exact RequirementId matches here, while other branches carry identifier-like values instead of scalar payloads.";
                    return true;
                case 7:
                    semanticSummary = "IMMUNITY ext-data payload-B field for the slot-local immunity payload.";
                    roleNotes = "This field behaves like the second profile-specific payload slot and varies more widely than the small selector fields around it.";
                    return true;
                case 8:
                    semanticSummary = "IMMUNITY ext-data sparse tail payload field for minority slot-local immunity branches.";
                    roleNotes = "This field only appears in minority IMMUNITY ext-data branches after the main Val7 payload and behaves like an auxiliary tail parameter.";
                    return true;
                case 9:
                    semanticSummary = "IMMUNITY ext-data slot-presence marker for minority slot-local immunity branches.";
                    roleNotes = "This field is almost always fixed at 1 when present, so it behaves like a branch marker or enabled-flag rather than a free scalar.";
                    return true;
                default:
                    return false;
            }
        }

        private static bool TryDescribeKnockbackValueField(string fieldKey, out string semanticSummary, out string roleNotes)
        {
            semanticSummary = null;
            roleNotes = null;

            if (string.Equals(fieldKey, "Value[0]", StringComparison.OrdinalIgnoreCase))
            {
                semanticSummary = "KNOCKBACK primary displacement magnitude for the component payload.";
                roleNotes = "This large-magnitude field is populated on nearly every KNOCKBACK row, including pure shove rows like Rune of Sundering and airborne launch rows like Gore, Khaine's Wrath, Exoneration, and Unleash the Winds, so it behaves like the base displacement strength rather than a branch selector.";
                return true;
            }

            if (string.Equals(fieldKey, "Value[1]", StringComparison.OrdinalIgnoreCase))
            {
                semanticSummary = "KNOCKBACK secondary launch or lift control for the component payload.";
                roleNotes = "This field is zero on flatter shove rows such as Rune of Sundering, but commonly lands in the 300-950 band on airborne launch rows and staged launch families such as Azyr's Beckoning, so it behaves like a secondary launch-vector or lift parameter rather than a branch selector.";
                return true;
            }

            if (string.Equals(fieldKey, "Value[2]", StringComparison.OrdinalIgnoreCase))
            {
                semanticSummary = "KNOCKBACK tertiary trajectory-bias control for the component payload.";
                roleNotes = "This field stays at 0 on simpler shove rows such as Rune of Sundering and Flames Of Fate, appears as small positive values on compact knockback test rows such as Knockback and Raid Boss Test Ability 1, and flips negative on mount-launch rows such as Mount - Arabian Disc - Midnight and Aphotic, so it behaves like a secondary trajectory-axis or bias control rather than another primary magnitude.";
                return true;
            }

            if (string.Equals(fieldKey, "Value[3]", StringComparison.OrdinalIgnoreCase))
            {
                semanticSummary = "KNOCKBACK displacement profile selector for the component payload.";
                roleNotes = "This low-cardinality field clusters with distinct motion families: 0 on simpler shove rows, 2 on the dominant player-launch family, and 3 or higher on minority scripted variants, so it behaves like a motion-profile selector rather than a scalar magnitude.";
                return true;
            }

            return false;
        }

        private static bool TryDescribeNamedControlField(string fieldKey, out string semanticSummary, out string roleNotes)
        {
            semanticSummary = null;
            roleNotes = null;

            if (string.Equals(fieldKey, "ActivationDelay", StringComparison.OrdinalIgnoreCase))
            {
                semanticSummary = "Delay before the component payload activates after its trigger fires.";
                roleNotes = "The client-side field name and the observed round-number values indicate a timing control field rather than an enum or identifier.";
                return true;
            }

            if (string.Equals(fieldKey, "ConeAngle", StringComparison.OrdinalIgnoreCase))
            {
                semanticSummary = "Directional cone arc width used by the component payload.";
                roleNotes = "The client-side field name and the observed values align with common arc widths such as 45, 90, 120, 135, 180, and 360.";
                return true;
            }

            if (string.Equals(fieldKey, "FlightSpeed", StringComparison.OrdinalIgnoreCase))
            {
                semanticSummary = "Projectile or travel-speed control for the component payload.";
                roleNotes = "The client-side field name and the observed values behave like speed magnitudes rather than compact selector enums.";
                return true;
            }

            if (string.Equals(fieldKey, "MaxTargets", StringComparison.OrdinalIgnoreCase))
            {
                semanticSummary = "Maximum target-count cap for the component payload.";
                roleNotes = "The client-side field name and the observed small integer values indicate a hard cap on how many targets the component may affect.";
                return true;
            }

            return false;
        }

        private static bool TryDescribeGenericFlagsRawField(string fieldKey, out string semanticSummary, out string roleNotes)
        {
            semanticSummary = null;
            roleNotes = null;

            if (!string.Equals(fieldKey, "FlagsRaw", StringComparison.OrdinalIgnoreCase))
                return false;

            semanticSummary = "Packed raw flag bitfield for the component payload.";
            roleNotes = "The client field name is explicit, but the exact meaning of each bit is still operation-specific and unresolved. Value-note bit positions are zero-based.";
            return true;
        }

        private static string BuildDominantRawValueSummary(IEnumerable<FieldObservation> observations, int limit)
        {
            List<FieldObservation> items = observations == null ? new List<FieldObservation>() : observations.ToList();
            if (items.Count == 0)
                return string.Empty;

            return string.Join(", ", items
                .GroupBy(row => row.RawValue, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
                .Take(limit)
                .Select(group => group.Key + " x" + group.Count().ToString(CultureInfo.InvariantCulture)));
        }

        private static bool TryBuildDamageStructuralInference(uint operationId, string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (operationId != 1 || observations == null || observations.Count == 0)
                return false;

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                return false;

            string semanticSummary;
            string roleNotes;
            if (!TryDescribeDamageExtDataField(valueIndex, out semanticSummary, out roleNotes))
                return false;

            string dominantValues = BuildDominantRawValueSummary(observations, 6);
            inference = new FieldObservationInference
            {
                SemanticSummary = semanticSummary,
                Confidence = SemanticConfidence.Structural,
                Notes = "This is a structural inference from recurring DAMAGE ext-data layouts in extracted client BIN rows for ExtData slot "
                    + slotIndex.ToString(CultureInfo.InvariantCulture)
                    + ". "
                    + roleNotes
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant raw values: " + dominantValues + ".")
                    + " Exact per-value retail semantics are still unresolved; use the value-profile evidence to inspect raw-value clusters."
            };
            return true;
        }

        private static bool TryBuildBonusTypeAdjustStructuralInference(uint operationId, string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (operationId != 22 || observations == null || observations.Count == 0)
                return false;

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                return false;

            string semanticSummary;
            string roleNotes;
            if (!TryDescribeBonusTypeAdjustExtDataField(valueIndex, out semanticSummary, out roleNotes))
                return false;

            string dominantValues = BuildDominantRawValueSummary(observations, 6);
            inference = new FieldObservationInference
            {
                SemanticSummary = semanticSummary,
                Confidence = SemanticConfidence.Structural,
                Notes = "This is a structural inference from recurring BONUS_TYPE_ADJUST ext-data layouts in extracted client BIN rows for ExtData slot "
                    + slotIndex.ToString(CultureInfo.InvariantCulture)
                    + ". "
                    + roleNotes
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant raw values: " + dominantValues + ".")
                    + " Exact per-value retail semantics are still unresolved; use the value-profile evidence to inspect raw-value clusters."
            };
            return true;
        }

        private static bool TryBuildExtDataVal2Inference(string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (observations == null || observations.Count == 0)
                return false;

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex) || valueIndex != 2)
                return false;

            // Tally how many observed values map to a known ComponentOperation name.
            int totalCount = observations.Count;
            int knownCount = 0;
            int zeroCount = 0;
            Dictionary<string, int> valueCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (FieldObservation obs in observations)
            {
                string rv = obs.RawValue ?? string.Empty;
                if (!valueCounts.ContainsKey(rv))
                    valueCounts[rv] = 0;
                valueCounts[rv]++;

                long num;
                if (long.TryParse(rv, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) && num >= 0)
                {
                    if (num == 0)
                    {
                        zeroCount++;
                        knownCount++; // 0 = NONE / inactive — known sentinel
                    }
                    else
                    {
                        string opName = DefinitionCatalog.DescribeComponentOperationValue((uint)num);
                        if (!string.IsNullOrWhiteSpace(opName) && !opName.StartsWith("Unknown", StringComparison.OrdinalIgnoreCase))
                            knownCount++;
                    }
                }
            }

            string coverageLine = knownCount.ToString(CultureInfo.InvariantCulture)
                + " of " + totalCount.ToString(CultureInfo.InvariantCulture)
                + " observed values (" + (totalCount > 0 ? (knownCount * 100 / totalCount).ToString(CultureInfo.InvariantCulture) : "0") + "%) resolve to a named ComponentOperation.";

            string dominantValues = string.Join(", ", valueCounts
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
                .Take(8)
                .Select(kvp =>
                {
                    long num;
                    string label = kvp.Key;
                    if (long.TryParse(kvp.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) && num > 0)
                    {
                        string opName = DefinitionCatalog.DescribeComponentOperationValue((uint)num);
                        if (!string.IsNullOrWhiteSpace(opName) && !opName.StartsWith("Unknown", StringComparison.OrdinalIgnoreCase))
                            label = kvp.Key + "=" + opName;
                    }
                    return label + " x" + kvp.Value.ToString(CultureInfo.InvariantCulture);
                }));

            inference = new FieldObservationInference
            {
                SemanticSummary = "ExtData slot " + slotIndex.ToString(CultureInfo.InvariantCulture) + " Val2 — ComponentOperation type code for this ext-data expression slot.",
                Confidence = SemanticConfidence.Inferred,
                Notes = "Across all component operations in the extracted client BIN, ExtData Val2 consistently encodes a ComponentOperation-family type code "
                    + "identifying what kind of effect the slot describes (DAMAGE, STAT_CHANGE, HEAL, AP_CHANGE, etc.). "
                    + coverageLine
                    + (zeroCount > 0 ? " Value 0 (" + zeroCount.ToString(CultureInfo.InvariantCulture) + " occurrences) = NONE / inactive slot." : string.Empty)
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant observed values: " + dominantValues + ".")
                    + " Values not yet present in the ComponentOperation enum represent undocumented operation type codes observed in client data."
            };
            return true;
        }

        private static bool TryBuildApplyAbilityExtDataKnownFieldInference(uint operationId, string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (operationId != 23 || observations == null || observations.Count == 0)
                return false;

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                return false;

            string semanticSummary;
            string fieldNotes;

            switch (valueIndex)
            {
                case 1:
                    semanticSummary = "ExtData slot " + slotIndex.ToString(CultureInfo.InvariantCulture) + " Val1 — application target for the embedded APPLY_ABILITY sub-effect.";
                    fieldNotes = "Val1=1 = Self (apply sub-effect to the caster). Val1=2 = Target (apply sub-effect to the ability target or enemy). "
                        + "STAT_CHANGE and DAMAGE sub-ops are target-directed (Val1=2) in 98%+ of cases; "
                        + "INTERRUPT, AP_REGEN_CHANGE, AP_CHANGE, and HEAL sub-ops are self-directed (Val1=1) in 95%+ of cases.";
                    break;

                default:
                    return false;
            }

            string dominantValues = string.Join(", ", observations
                .GroupBy(row => row.RawValue, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
                .Take(6)
                .Select(group => group.Key + " x" + group.Count().ToString(CultureInfo.InvariantCulture)));

            inference = new FieldObservationInference
            {
                SemanticSummary = semanticSummary,
                Confidence = SemanticConfidence.Inferred,
                Notes = "Pattern analysis of 2075 APPLY_ABILITY component records in the extracted client BIN. "
                    + fieldNotes
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant observed values: " + dominantValues + ".")
            };
            return true;
        }

        private static bool TryBuildApplyAbilityStructuralInference(uint operationId, string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (operationId != 23 || observations == null || observations.Count == 0)
                return false;

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                return false;

            string semanticSummary;
            string roleNotes;
            if (!TryDescribeApplyAbilityExtDataField(valueIndex, out semanticSummary, out roleNotes))
                return false;

            string dominantValues = BuildDominantRawValueSummary(observations, 6);
            inference = new FieldObservationInference
            {
                SemanticSummary = semanticSummary,
                Confidence = SemanticConfidence.Structural,
                Notes = "This is a structural inference from recurring APPLY_ABILITY ext-data layouts in extracted client BIN rows for ExtData slot "
                    + slotIndex.ToString(CultureInfo.InvariantCulture)
                    + ". "
                    + roleNotes
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant raw values: " + dominantValues + ".")
                    + " Exact per-value retail semantics are still unresolved; use the value-profile evidence to inspect raw-value clusters."
            };
            return true;
        }

        private static bool TryBuildNamedControlFieldInference(string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (observations == null || observations.Count == 0)
                return false;

            string semanticSummary;
            string roleNotes;
            if (!TryDescribeNamedControlField(fieldKey, out semanticSummary, out roleNotes))
                return false;

            string dominantValues = BuildDominantRawValueSummary(observations, 6);
            inference = new FieldObservationInference
            {
                SemanticSummary = semanticSummary,
                Confidence = SemanticConfidence.Inferred,
                Notes = "This inferred meaning comes from the descriptive client field name and the observed value shapes in extracted client BIN rows. "
                    + roleNotes
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant raw values: " + dominantValues + ".")
                    + " Exact display formatting is not directly token-confirmed yet."
            };
            return true;
        }

        private static bool TryBuildGenericFlagsRawInference(string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (observations == null || observations.Count == 0)
                return false;

            string semanticSummary;
            string roleNotes;
            if (!TryDescribeGenericFlagsRawField(fieldKey, out semanticSummary, out roleNotes))
                return false;

            string dominantValues = BuildDominantRawValueSummary(observations, 6);
            inference = new FieldObservationInference
            {
                SemanticSummary = semanticSummary,
                Confidence = SemanticConfidence.Structural,
                Notes = "This structural inference comes from the explicit client field name and the recurring raw bit-pattern values in extracted client BIN rows. "
                    + roleNotes
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant raw values: " + dominantValues + ".")
                    + " Exact per-bit retail semantics are still unresolved; use the value-profile evidence to inspect recurring flag combinations."
            };
            return true;
        }

        private static bool TryBuildCcStructuralInference(uint operationId, string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (operationId != 12 || observations == null || observations.Count == 0)
                return false;

            string dominantValues = BuildDominantRawValueSummary(observations, 6);
            if (string.Equals(fieldKey, "FlagsRaw", StringComparison.OrdinalIgnoreCase))
            {
                inference = new FieldObservationInference
                {
                    SemanticSummary = "CC packed control bitfield for branch and behavior selection.",
                    Confidence = SemanticConfidence.Structural,
                    Notes = "This is a structural inference from recurring CC layouts in extracted client BIN rows. "
                        + "The field behaves like a packed bitfield or mode mask and should be read together with Value15 and the leading ExtData selector fields."
                        + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant raw values: " + dominantValues + ".")
                        + " Exact per-bit retail semantics are still unresolved; use the value-profile evidence to inspect branch clusters."
                };
                return true;
            }

            if (string.Equals(fieldKey, "Value15", StringComparison.OrdinalIgnoreCase))
            {
                inference = new FieldObservationInference
                {
                    SemanticSummary = "CC auxiliary profile marker paired with FlagsRaw.",
                    Confidence = SemanticConfidence.Structural,
                    Notes = "This is a structural inference from recurring CC layouts in extracted client BIN rows. "
                        + "The field is almost always a small fixed marker and behaves like a companion selector or layout tag rather than a free scalar."
                        + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant raw values: " + dominantValues + ".")
                        + " Exact per-value retail semantics are still unresolved; use the value-profile evidence to inspect the paired FlagsRaw branches."
                };
                return true;
            }

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                return false;

            string semanticSummary;
            string roleNotes;
            if (!TryDescribeCcExtDataField(valueIndex, out semanticSummary, out roleNotes))
                return false;

            inference = new FieldObservationInference
            {
                SemanticSummary = semanticSummary,
                Confidence = SemanticConfidence.Structural,
                Notes = "This is a structural inference from recurring CC ext-data layouts in extracted client BIN rows for ExtData slot "
                    + slotIndex.ToString(CultureInfo.InvariantCulture)
                    + ". "
                    + roleNotes
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant raw values: " + dominantValues + ".")
                    + " Exact per-value retail semantics are still unresolved; use the value-profile evidence to inspect raw-value clusters."
            };
            return true;
        }

        private static bool TryBuildKnockbackStructuralInference(uint operationId, string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (operationId != 24 || observations == null || observations.Count == 0)
                return false;

            string semanticSummary;
            string roleNotes;
            string dominantValues = BuildDominantRawValueSummary(observations, 6);
            if (TryDescribeKnockbackValueField(fieldKey, out semanticSummary, out roleNotes))
            {
                inference = new FieldObservationInference
                {
                    SemanticSummary = semanticSummary,
                    Confidence = SemanticConfidence.Structural,
                    Notes = "This is a structural inference from recurring KNOCKBACK value patterns in extracted client BIN rows. "
                        + roleNotes
                        + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant raw values: " + dominantValues + ".")
                        + " Exact retail presentation is still unresolved; use the value-profile evidence to inspect raw-value clusters."
                };
                return true;
            }

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                return false;

            if (!TryDescribeKnockbackExtDataField(valueIndex, out semanticSummary, out roleNotes))
                return false;

            inference = new FieldObservationInference
            {
                SemanticSummary = semanticSummary,
                Confidence = SemanticConfidence.Structural,
                Notes = "This is a structural inference from recurring KNOCKBACK ext-data layouts in extracted client BIN rows for ExtData slot "
                    + slotIndex.ToString(CultureInfo.InvariantCulture)
                    + ". "
                    + roleNotes
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant raw values: " + dominantValues + ".")
                    + " Exact per-value retail semantics are still unresolved; use the value-profile evidence to inspect raw-value clusters."
            };
            return true;
        }

        private static bool TryBuildImmunityStructuralInference(uint operationId, string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (operationId != 38 || observations == null || observations.Count == 0)
                return false;

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                return false;

            string semanticSummary;
            string roleNotes;
            if (!TryDescribeImmunityExtDataField(valueIndex, out semanticSummary, out roleNotes))
                return false;

            string dominantValues = BuildDominantRawValueSummary(observations, 6);
            inference = new FieldObservationInference
            {
                SemanticSummary = semanticSummary,
                Confidence = SemanticConfidence.Structural,
                Notes = "This is a structural inference from recurring IMMUNITY ext-data layouts in extracted client BIN rows for ExtData slot "
                    + slotIndex.ToString(CultureInfo.InvariantCulture)
                    + ". "
                    + roleNotes
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant raw values: " + dominantValues + ".")
                    + " Exact per-value retail semantics are still unresolved; use the value-profile evidence to inspect raw-value clusters."
            };
            return true;
        }

        private static string BuildDamageStructuralValueNote(int valueIndex, string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return string.Empty;

            switch (valueIndex)
            {
                case 1:
                    if (string.Equals(rawValue, "2", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant DAMAGE Val1 branch marker in the recurring ext-data layout.";
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the secondary DAMAGE Val1 branch marker in the recurring ext-data layout.";
                    break;
                case 4:
                    if (string.Equals(rawValue, "8", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant DAMAGE family marker across the recurring ext-data layout.";
                    if (string.Equals(rawValue, "9", StringComparison.OrdinalIgnoreCase))
                        return "This is a minority DAMAGE family-marker variant.";
                    break;
                case 5:
                    if (string.Equals(rawValue, "3", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant DAMAGE Val5 auxiliary-selector branch in the minority tail layout.";
                    break;
                case 6:
                    long damageLinkValue;
                    if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out damageLinkValue))
                    {
                        if (damageLinkValue >= 1000)
                            return "This large DAMAGE Val6 value behaves like an identifier-style link target rather than a scalar payload.";
                        if (damageLinkValue > 0)
                            return "This DAMAGE Val6 value behaves like a compact branch-local link marker rather than a free scalar.";
                    }
                    break;
                case 7:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant compact DAMAGE Val7 branch in the recurring ext-data layout.";

                    long damagePayloadValue;
                    if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out damagePayloadValue)
                        && damagePayloadValue >= 100)
                        return "This is a scalar-looking DAMAGE Val7 branch variant compared with the dominant compact 1-branch.";
                    break;
                case 8:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant sparse DAMAGE Val8 tail branch.";
                    break;
                case 9:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This fixed 1-value behaves like a slot-presence marker rather than a scalar payload.";
                    break;
            }

            return string.Empty;
        }

        private static string BuildBonusTypeAdjustStructuralValueNote(int valueIndex, string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return string.Empty;

            switch (valueIndex)
            {
                case 1:
                    if (string.Equals(rawValue, "2", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant BONUS_TYPE_ADJUST Val1 branch marker in the recurring ext-data layout.";
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the secondary BONUS_TYPE_ADJUST Val1 branch marker in the recurring ext-data layout.";
                    break;
                case 4:
                    if (string.Equals(rawValue, "8", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant BONUS_TYPE_ADJUST family marker across the recurring ext-data layout.";
                    if (string.Equals(rawValue, "9", StringComparison.OrdinalIgnoreCase))
                        return "This is a minority BONUS_TYPE_ADJUST family-marker variant.";
                    break;
                case 5:
                    if (string.Equals(rawValue, "3", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant BONUS_TYPE_ADJUST Val5 auxiliary-selector branch in the minority tail layout.";
                    break;
                case 6:
                    long bonusLinkValue;
                    if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out bonusLinkValue))
                    {
                        if (bonusLinkValue >= 1000)
                            return "This large BONUS_TYPE_ADJUST Val6 value behaves like an identifier-style link target rather than a scalar payload.";
                        if (bonusLinkValue > 0)
                            return "This BONUS_TYPE_ADJUST Val6 value behaves like a compact branch-local link marker rather than a free scalar.";
                    }
                    break;
                case 7:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant compact BONUS_TYPE_ADJUST Val7 branch in the recurring ext-data layout.";

                    long bonusPayloadValue;
                    if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out bonusPayloadValue)
                        && bonusPayloadValue >= 100)
                        return "This is a scalar-looking BONUS_TYPE_ADJUST Val7 branch variant compared with the dominant compact 1-branch.";
                    break;
                case 8:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant sparse BONUS_TYPE_ADJUST Val8 tail branch.";
                    break;
                case 9:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This fixed 1-value behaves like a slot-presence marker rather than a scalar payload.";
                    break;
            }

            return string.Empty;
        }

        private static string BuildApplyAbilityStructuralValueNote(int valueIndex, string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return string.Empty;

            switch (valueIndex)
            {
                case 1:
                    if (string.Equals(rawValue, "2", StringComparison.OrdinalIgnoreCase))
                        return "This raw value is the dominant branch marker and frequently appears with Val2=2, Val3=1, and Val7=1.";
                    break;
                case 2:
                    if (string.Equals(rawValue, "2", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant Val2 branch and is strongly paired with Val1=2, Val3=1, Val4=8, and Val7=1.";
                    if (string.Equals(rawValue, "9", StringComparison.OrdinalIgnoreCase))
                        return "This common secondary branch is strongly paired with Val1=1, Val3=4, Val4=8, and often Val6=100.";
                    break;
                case 3:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant Val3 profile and is strongly paired with Val1=2, Val2=2, Val4=8, and Val7=1.";
                    if (string.Equals(rawValue, "4", StringComparison.OrdinalIgnoreCase))
                        return "This secondary Val3 profile is strongly paired with Val1=1, Val2=9, Val4=8, and often Val6=100.";
                    break;
                case 4:
                    if (string.Equals(rawValue, "8", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant APPLY_ABILITY family marker across the extracted rows.";
                    if (string.Equals(rawValue, "9", StringComparison.OrdinalIgnoreCase))
                        return "This is a minority variant of the dominant 8-family marker.";
                    break;
                case 5:
                    if (string.Equals(rawValue, "3", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant APPLY_ABILITY Val5 auxiliary-selector branch in the minority tail layout.";
                    if (string.Equals(rawValue, "5", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "8", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "9", StringComparison.OrdinalIgnoreCase))
                        return "This is a minority APPLY_ABILITY Val5 auxiliary-selector variant.";
                    break;
                case 6:
                    if (string.Equals(rawValue, "100", StringComparison.OrdinalIgnoreCase))
                        return "This compact APPLY_ABILITY Val6 link marker is common in the secondary Val1=1, Val2=9, Val3=4 branch.";

                    long applyAbilityLinkValue;
                    if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out applyAbilityLinkValue))
                    {
                        if (applyAbilityLinkValue >= 1000)
                            return "This large APPLY_ABILITY Val6 value behaves like an identifier-style link target rather than a scalar payload.";
                        if (applyAbilityLinkValue > 0)
                            return "This APPLY_ABILITY Val6 value behaves like a compact branch-local link marker rather than a free scalar.";
                    }
                    break;
                case 7:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant Val7 payload branch and is strongly paired with Val1=2, Val2=2, Val3=1, and Val4=8.";
                    if (string.Equals(rawValue, "5", StringComparison.OrdinalIgnoreCase))
                        return "This common secondary Val7 branch is strongly paired with Val1=1, Val2=9, Val3=4, Val4=8, and Val6=100.";
                    if (string.Equals(rawValue, "250", StringComparison.OrdinalIgnoreCase))
                        return "This branch is strongly paired with Val2=3, Val3=4, and Val4=8.";
                    break;
                case 8:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant sparse APPLY_ABILITY Val8 tail branch.";
                    if (string.Equals(rawValue, "2", StringComparison.OrdinalIgnoreCase) || string.Equals(rawValue, "3", StringComparison.OrdinalIgnoreCase))
                        return "This is a minority sparse APPLY_ABILITY Val8 tail variant.";
                    break;
                case 9:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This fixed 1-value behaves like a slot-presence marker rather than a scalar payload.";
                    break;
            }

            return string.Empty;
        }

        private static string BuildKnockbackStructuralValueNote(int valueIndex, string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return string.Empty;

            switch (valueIndex)
            {
                case 1:
                    if (string.Equals(rawValue, "2", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant KNOCKBACK Val1 branch marker in the recurring ext-data layout.";
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the secondary KNOCKBACK Val1 branch marker in the recurring ext-data layout.";
                    break;
                case 4:
                    if (string.Equals(rawValue, "8", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant KNOCKBACK family marker across the recurring ext-data layout.";
                    if (string.Equals(rawValue, "9", StringComparison.OrdinalIgnoreCase))
                        return "This is a minority KNOCKBACK family-marker variant.";
                    break;
                case 5:
                    if (string.Equals(rawValue, "3", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant KNOCKBACK Val5 auxiliary-selector branch in the minority tail layout.";
                    break;
                case 6:
                    long knockbackLinkValue;
                    if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out knockbackLinkValue))
                    {
                        if (knockbackLinkValue >= 1000)
                            return "This large KNOCKBACK Val6 value behaves like an identifier-style link target rather than a scalar payload.";
                        if (knockbackLinkValue > 0)
                            return "This KNOCKBACK Val6 value behaves like a compact branch-local link marker rather than a free scalar.";
                    }
                    break;
                case 7:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant compact KNOCKBACK Val7 branch in the recurring ext-data layout.";

                    long knockbackPayloadValue;
                    if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out knockbackPayloadValue)
                        && knockbackPayloadValue >= 100)
                        return "This is a scalar-looking KNOCKBACK Val7 branch variant compared with the dominant compact 1-branch.";
                    break;
                case 8:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant sparse KNOCKBACK Val8 tail branch.";
                    break;
                case 9:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This fixed 1-value behaves like a slot-presence marker rather than a scalar payload.";
                    break;
            }

            return string.Empty;
        }

        private static string BuildImmunityStructuralValueNote(int valueIndex, string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return string.Empty;

            switch (valueIndex)
            {
                case 4:
                    if (string.Equals(rawValue, "8", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant IMMUNITY family marker across the recurring ext-data layout.";
                    if (string.Equals(rawValue, "9", StringComparison.OrdinalIgnoreCase))
                        return "This is a minority IMMUNITY family-marker variant.";
                    break;
                case 5:
                    if (string.Equals(rawValue, "3", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant IMMUNITY Val5 auxiliary-selector branch in the minority tail layout.";
                    break;
                case 6:
                    long immunityLinkValue;
                    if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out immunityLinkValue))
                    {
                        if (immunityLinkValue >= 1000)
                            return "This large IMMUNITY Val6 value behaves like an identifier-style link target rather than a scalar payload.";
                        if (immunityLinkValue > 0)
                            return "This IMMUNITY Val6 value behaves like a compact branch-local link marker rather than a free scalar.";
                    }
                    break;
                case 7:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant compact IMMUNITY Val7 branch in the recurring ext-data layout.";

                    long immunityPayloadValue;
                    if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out immunityPayloadValue)
                        && immunityPayloadValue >= 1000)
                        return "This is an identifier-like IMMUNITY Val7 branch variant compared with the dominant compact 1-branch.";
                    break;
                case 8:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant sparse IMMUNITY Val8 tail branch.";
                    break;
                case 9:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This fixed 1-value behaves like a slot-presence marker rather than a scalar payload.";
                    break;
            }

            return string.Empty;
        }

        private static string BuildKnockbackValueFieldNote(string fieldKey, string rawValue)
        {
            long numericValue;
            if (string.IsNullOrWhiteSpace(rawValue)
                || !long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out numericValue))
                return string.Empty;

            if (string.Equals(fieldKey, "Value[0]", StringComparison.OrdinalIgnoreCase))
            {
                if (numericValue <= 25000)
                    return "This is a short-range KNOCKBACK displacement branch compared with the common 40000-65000 launch band.";
                if (numericValue >= 80000)
                    return "This is a high-magnitude KNOCKBACK displacement branch compared with the common 40000-65000 launch band.";
                return "This is a midrange KNOCKBACK displacement value in the common player-launch band.";
            }

            if (string.Equals(fieldKey, "Value[1]", StringComparison.OrdinalIgnoreCase))
            {
                if (numericValue == 0)
                    return "This zero value matches flatter shove rows that do not use a secondary lift control.";
                if (numericValue <= 450)
                    return "This is a shallow secondary launch-control value in the lower observed lift band.";
                if (numericValue >= 800)
                    return "This is a steep secondary launch-control value in the upper observed lift band.";
                return "This is a midrange secondary launch-control value in the common KNOCKBACK lift band.";
            }

            if (string.Equals(fieldKey, "Value[2]", StringComparison.OrdinalIgnoreCase))
            {
                if (numericValue < 0)
                    return "This negative trajectory-bias branch appears on mount-style launch rows rather than the common shove layout.";
                if (numericValue == 0)
                    return "This zero trajectory-bias branch aligns with the simpler shove layout.";
                if (numericValue <= 100)
                    return "This small positive trajectory-bias branch appears on compact knockback test and short-range shove variants.";
                return "This large positive trajectory-bias branch appears on minority scripted launch variants rather than the common shove layout.";
            }

            if (string.Equals(fieldKey, "Value[3]", StringComparison.OrdinalIgnoreCase))
            {
                if (numericValue == 0)
                    return "This value matches the simplest shove profile with no extra motion-profile selector.";
                if (numericValue == 2)
                    return "This is the dominant KNOCKBACK motion-profile selector in the player-launch family.";
                if (numericValue == 3)
                    return "This is a secondary KNOCKBACK motion-profile selector seen in mixed or hidden side-effect rows.";
                return "This is a minority KNOCKBACK motion-profile selector variant.";
            }

            return string.Empty;
        }

        private static string BuildNamedControlFieldValueNote(string fieldKey, string rawValue)
        {
            long numericValue;
            if (string.IsNullOrWhiteSpace(rawValue)
                || !long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out numericValue))
                return string.Empty;

            if (string.Equals(fieldKey, "ActivationDelay", StringComparison.OrdinalIgnoreCase))
            {
                if (numericValue <= 100)
                    return "This is a short activation-delay branch compared with the common 250-1500 range.";
                if (numericValue >= 10000)
                    return "This is an unusually long activation-delay branch compared with the common sub-2-second timings.";
                return "This is a midrange activation-delay value in the common extracted-client timing band.";
            }

            if (string.Equals(fieldKey, "ConeAngle", StringComparison.OrdinalIgnoreCase))
            {
                if (numericValue >= 360)
                    return "This value behaves like a full-circle or effectively omni-directional arc.";
                if (numericValue >= 180)
                    return "This value behaves like a very wide directional arc.";
                return "This value behaves like a standard forward cone width.";
            }

            if (string.Equals(fieldKey, "FlightSpeed", StringComparison.OrdinalIgnoreCase))
            {
                if (numericValue <= 500)
                    return "This is on the slow end of the observed projectile-speed range.";
                if (numericValue >= 1500)
                    return "This is on the fast end of the observed projectile-speed range.";
                return "This is a midrange projectile-speed value in the observed set.";
            }

            if (string.Equals(fieldKey, "MaxTargets", StringComparison.OrdinalIgnoreCase))
            {
                if (numericValue <= 1)
                    return "This branch is capped to a single target.";
                if (numericValue >= 9)
                    return "This branch allows a broad multi-target cap compared with the common small counts.";
                return "This branch allows a small multi-target cap.";
            }

            return string.Empty;
        }

        private static string BuildGenericFlagsRawValueNote(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return string.Empty;

            long numericValue;
            if (!long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out numericValue))
                return "This raw value still behaves like an opaque packed flag field rather than a free scalar.";

            if (numericValue < 0)
                return "This negative raw value is unusual for a flags field and should be treated as a signed packed mask.";

            if (numericValue == 0)
                return "This zero raw value indicates that no flag bits are set.";

            ulong unsignedValue = (ulong)numericValue;
            int bitCount = CountSetBits(unsignedValue);
            string bitSummary = BuildSetBitSummary(unsignedValue, 6);

            if (bitCount <= 1)
                return "This raw value sets bit " + bitSummary + " only, so it behaves like an isolated flag branch.";

            if (bitCount <= 3)
                return "This raw value combines " + bitCount.ToString(CultureInfo.InvariantCulture) + " flag bits (" + bitSummary + "), so it behaves like a compact mode mask.";

            return "This raw value combines " + bitCount.ToString(CultureInfo.InvariantCulture) + " flag bits (" + bitSummary + "), so it behaves like a packed mode mask rather than a scalar.";
        }

        private static string BuildCcStructuralValueNote(string fieldKey, int? valueIndex, string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return string.Empty;

            if (string.Equals(fieldKey, "FlagsRaw", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(rawValue, "2175", StringComparison.OrdinalIgnoreCase))
                    return "Dominant packed CC FlagsRaw branch; commonly pairs with Value15=4 and ExtData Val1=2, Val3=1, Val4=8, Val7=1.";
                if (string.Equals(rawValue, "2303", StringComparison.OrdinalIgnoreCase))
                    return "Common packed CC FlagsRaw branch; stays in the dominant CC layout family and pairs with Value15=4.";
                if (string.Equals(rawValue, "8", StringComparison.OrdinalIgnoreCase))
                    return "Compact low-bit CC FlagsRaw branch appearing in shorter CC layouts.";
                if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                    return "Minimal single-flag CC FlagsRaw variant inside the broader bitfield family.";
                return string.Empty;
            }

            if (string.Equals(fieldKey, "Value15", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(rawValue, "4", StringComparison.OrdinalIgnoreCase))
                    return "This is the dominant CC Value15 marker and appears across almost every major FlagsRaw branch.";
                if (string.Equals(rawValue, "5", StringComparison.OrdinalIgnoreCase))
                    return "This is a rare Value15 variant inside the dominant CC layout family.";
                return string.Empty;
            }

            if (!valueIndex.HasValue)
                return string.Empty;

            switch (valueIndex.Value)
            {
                case 1:
                    if (string.Equals(rawValue, "2", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant CC Val1 branch marker and most often pairs with Val3=1 and Val4=8.";
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the secondary CC Val1 branch marker and commonly appears with the non-dominant Val3 profiles.";
                    break;
                case 2:
                    if (string.Equals(rawValue, "2", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant compact CC Val2 branch inside the recurring ext-data layout.";
                    if (string.Equals(rawValue, "15", StringComparison.OrdinalIgnoreCase))
                        return "This is a common secondary compact CC Val2 branch inside the recurring ext-data layout.";
                    if (string.Equals(rawValue, "88", StringComparison.OrdinalIgnoreCase))
                        return "This is a high scalar-looking Val2 variant compared with the compact selector-like CC branches.";
                    break;
                case 3:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant CC Val3 profile in the recurring ext-data layout.";
                    if (string.Equals(rawValue, "4", StringComparison.OrdinalIgnoreCase))
                        return "This is the main secondary CC Val3 profile in the recurring ext-data layout.";
                    if (string.Equals(rawValue, "5", StringComparison.OrdinalIgnoreCase) || string.Equals(rawValue, "6", StringComparison.OrdinalIgnoreCase))
                        return "This is a minority CC Val3 profile variant in the recurring ext-data layout.";
                    break;
                case 4:
                    if (string.Equals(rawValue, "8", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant CC family marker across the recurring ext-data layout.";
                    if (string.Equals(rawValue, "9", StringComparison.OrdinalIgnoreCase))
                        return "This is a minority CC family-marker variant.";
                    break;
                case 5:
                    if (string.Equals(rawValue, "3", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant CC Val5 auxiliary-selector branch in the minority tail layout.";
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "5", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "8", StringComparison.OrdinalIgnoreCase))
                        return "This is a minority CC Val5 auxiliary-selector variant.";
                    break;
                case 6:
                    long ccLinkValue;
                    if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out ccLinkValue))
                    {
                        if (ccLinkValue >= 1000)
                            return "This large CC Val6 value behaves like an identifier-style link target rather than a scalar payload.";
                        if (ccLinkValue > 0)
                            return "This CC Val6 value behaves like a compact branch-local link marker rather than a free scalar.";
                    }
                    break;
                case 7:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant compact CC Val7 branch in the recurring ext-data layout.";
                    if (string.Equals(rawValue, "20", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "50", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "100", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "200", StringComparison.OrdinalIgnoreCase))
                        return "This is a scalar-looking CC Val7 branch variant compared with the dominant compact 1-branch.";
                    break;
                case 8:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant sparse CC Val8 tail branch.";
                    if (string.Equals(rawValue, "16", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "25", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "34", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "150", StringComparison.OrdinalIgnoreCase))
                        return "This is a minority scalar-looking CC Val8 tail variant.";
                    break;
                case 9:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This fixed 1-value behaves like a slot-presence marker rather than a scalar payload.";
                    break;
            }

            return string.Empty;
        }

        private static bool TryResolveExtDataVal2Semantic(uint operationId, string fieldKey, string rawValue, out ComponentFieldSemantic semantic)
        {
            semantic = null;

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex) || valueIndex != 2)
                return false;

            string valueNote = BuildExtDataVal2ValueNote(operationId, rawValue);
            semantic = new ComponentFieldSemantic
            {
                DomainKey = "ComponentOperation",
                Meaning = "Operation-type code identifying what kind of effect this ext-data expression slot describes.",
                Confidence = SemanticConfidence.Inferred,
                Source = "Cross-operation ExtData Val2 pattern analysis (extracted client BIN)",
                SourcePath = string.Empty,
                SourceLocation = string.Empty,
                Notes = "Across all component operations in extracted client BIN data, ExtData Val2 consistently holds a ComponentOperation-family type code. "
                    + "Values in the known range map directly to ComponentOperation names. "
                    + "Values above the known range represent operation type codes not yet named in the current enum. "
                    + (string.IsNullOrWhiteSpace(valueNote) ? string.Empty : valueNote)
            };
            return true;
        }

        private static string BuildExtDataVal2ValueNote(uint operationId, string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return string.Empty;

            long numericValue;
            if (!long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out numericValue) || numericValue < 0)
                return string.Empty;

            if (numericValue == 0)
                return "Value 0 = NONE / inactive slot.";

            string opName = DefinitionCatalog.DescribeComponentOperationValue((uint)numericValue);
            if (!string.IsNullOrWhiteSpace(opName) && !opName.StartsWith("Unknown", StringComparison.OrdinalIgnoreCase))
                return "ComponentOperation: " + opName + " (op=" + numericValue.ToString(CultureInfo.InvariantCulture) + ").";

            return "Extended operation code " + numericValue.ToString(CultureInfo.InvariantCulture) + " — present in extracted client BIN data but not yet named in the ComponentOperation enum.";
        }

        private static bool TryResolveExtDataVal3Semantic(string fieldKey, string rawValue, out ComponentFieldSemantic semantic)
        {
            semantic = null;
            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex) || valueIndex != 3)
                return false;

            long numericValue = 0;
            bool hasNumeric = !string.IsNullOrWhiteSpace(rawValue)
                && long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out numericValue);

            string valueNote = string.Empty;
            if (hasNumeric)
            {
                switch ((int)numericValue)
                {
                    case 0: valueNote = "Val3=0: slot inactive / no profile assigned."; break;
                    case 1: valueNote = "Val3=1 (Direct): immediate single application on trigger."; break;
                    case 4: valueNote = "Val3=4 (Event-triggered): proc on event; Val7 encodes chance %, duration ms, or amount depending on sub-op type."; break;
                    case 5: valueNote = "Val3=5 (Periodic/recovery): periodic or stackable application."; break;
                    case 6: valueNote = "Val3=6 (Rate-modifier): ongoing resource or AP rate modification; Val7 is typically 0."; break;
                    case 7: valueNote = "Val3=7 (Conditional/damage-variant): common in DAMAGE ext-data; exact retail semantics unresolved."; break;
                    default: valueNote = "Val3=" + rawValue + " is an uncommon profile code present in client BIN data; exact retail semantics unresolved."; break;
                }
            }

            semantic = new ComponentFieldSemantic
            {
                DomainKey = "ExtDataApplicationProfile",
                Meaning = "ExtData slot " + slotIndex.ToString(CultureInfo.InvariantCulture) + " Val3 — application profile encoding the delivery mode of the sub-effect for this slot.",
                Confidence = SemanticConfidence.Inferred,
                Source = "Cross-operation ExtData Val3 pattern analysis (18,526 extracted client BIN component records)",
                SourcePath = string.Empty,
                SourceLocation = string.Empty,
                Notes = "Val3 encodes the same profile-type across all component operations in the extracted client BIN. "
                    + "Val3=1 (Direct, 66%): immediate single application on trigger. "
                    + "Val3=4 (Event-triggered, 8%): proc on event, often with a chance % or duration in Val7. "
                    + "Val3=5 (Periodic/recovery, 2%): periodic or stackable application. "
                    + "Val3=6 (Rate-modifier, 11%): ongoing resource/AP rate modification. "
                    + "Val3=7 (10%): frequently seen in DAMAGE-operation contexts; exact semantics unresolved. "
                    + (string.IsNullOrWhiteSpace(valueNote) ? string.Empty : valueNote)
            };
            return true;
        }

        private static bool TryBuildExtDataVal3Inference(string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (observations == null || observations.Count == 0)
                return false;

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex) || valueIndex != 3)
                return false;

            int totalCount = observations.Count;
            int knownCount = 0;
            foreach (FieldObservation obs in observations)
            {
                long num;
                if (long.TryParse(obs.RawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out num)
                    && (num == 0 || num == 1 || num == 4 || num == 5 || num == 6 || num == 7))
                    knownCount++;
            }

            string coverageLine = knownCount.ToString(CultureInfo.InvariantCulture)
                + " of " + totalCount.ToString(CultureInfo.InvariantCulture)
                + " observed values (" + (totalCount > 0 ? (knownCount * 100 / totalCount).ToString(CultureInfo.InvariantCulture) : "0") + "%) map to a named application profile.";

            string dominantValues = BuildDominantRawValueSummary(observations, 6);

            inference = new FieldObservationInference
            {
                SemanticSummary = "ExtData slot " + slotIndex.ToString(CultureInfo.InvariantCulture) + " Val3 — application profile (delivery mode) for this ext-data sub-effect slot.",
                Confidence = SemanticConfidence.Inferred,
                Notes = "Val3 encodes the same profile-type across all component operations. "
                    + "Val3=1 (Direct): 66% of all records. "
                    + "Val3=4 (Event-triggered): 8%. "
                    + "Val3=5 (Periodic/recovery): 2%. "
                    + "Val3=6 (Rate-modifier): 11%. "
                    + "Val3=7 (Conditional/damage-variant): 10%, most common in DAMAGE-operation ext-data. "
                    + coverageLine
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant observed values: " + dominantValues + ".")
            };
            return true;
        }

        private static bool TryResolveExtDataVal4Semantic(string fieldKey, string rawValue, out ComponentFieldSemantic semantic)
        {
            semantic = null;
            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex) || valueIndex != 4)
                return false;

            long numericValue = 0;
            bool hasNumeric = !string.IsNullOrWhiteSpace(rawValue)
                && long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out numericValue);

            string valueNote = string.Empty;
            if (hasNumeric)
            {
                if (numericValue == 8) valueNote = "Val4=8: standard layout — this is the expected value for the vast majority of ext-data slots.";
                else if (numericValue == 9) valueNote = "Val4=9: requirement-conditional variant — Val6 in this slot carries a RequirementId chain reference instead of a scalar payload.";
                else valueNote = "Val4=" + rawValue + " is an uncommon layout tag present in client BIN data.";
            }

            semantic = new ComponentFieldSemantic
            {
                DomainKey = "ExtDataLayoutTag",
                Meaning = "ExtData slot " + slotIndex.ToString(CultureInfo.InvariantCulture) + " Val4 — layout tag identifying the encoding variant for this ext-data slot.",
                Confidence = SemanticConfidence.Inferred,
                Source = "Cross-operation ExtData Val4 pattern analysis (18,526 extracted client BIN component records)",
                SourcePath = string.Empty,
                SourceLocation = string.Empty,
                Notes = "Across all component operations in extracted client BIN data, Val4 is almost always 8 (standard layout, 97% of records). "
                    + "Val4=9 (2%) marks a requirement-conditional variant where Val6 carries a RequirementId chain reference. "
                    + (string.IsNullOrWhiteSpace(valueNote) ? string.Empty : valueNote)
            };
            return true;
        }

        private static bool TryBuildExtDataVal4Inference(string fieldKey, List<FieldObservation> observations, out FieldObservationInference inference)
        {
            inference = null;
            if (observations == null || observations.Count == 0)
                return false;

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex) || valueIndex != 4)
                return false;

            int totalCount = observations.Count;
            int knownCount = observations.Count(obs =>
            {
                long num;
                return long.TryParse(obs.RawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) && (num == 8 || num == 9);
            });

            string coverageLine = knownCount.ToString(CultureInfo.InvariantCulture)
                + " of " + totalCount.ToString(CultureInfo.InvariantCulture)
                + " observed values (" + (totalCount > 0 ? (knownCount * 100 / totalCount).ToString(CultureInfo.InvariantCulture) : "0") + "%) resolve to a named layout tag.";

            string dominantValues = BuildDominantRawValueSummary(observations, 4);

            inference = new FieldObservationInference
            {
                SemanticSummary = "ExtData slot " + slotIndex.ToString(CultureInfo.InvariantCulture) + " Val4 — layout tag for the ext-data slot encoding variant (8=Standard, 9=Requirement-conditional).",
                Confidence = SemanticConfidence.Inferred,
                Notes = "Val4 is almost always 8 (standard layout, 97% globally). "
                    + "Val4=9 marks requirement-conditional slots where Val6 carries a RequirementId chain reference. "
                    + coverageLine
                    + (string.IsNullOrWhiteSpace(dominantValues) ? string.Empty : " Dominant observed values: " + dominantValues + ".")
            };
            return true;
        }

        private static bool TryResolveApplyAbilityExtDataKnownFieldSemantic(uint operationId, string fieldKey, string rawValue, out ComponentFieldSemantic semantic)
        {
            semantic = null;
            if (operationId != 23)
                return false;

            int slotIndex;
            int valueIndex;
            if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                return false;

            long numericValue = 0;
            bool hasNumeric = !string.IsNullOrWhiteSpace(rawValue)
                && long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out numericValue);

            string meaning;
            string notes;
            string valueNote = string.Empty;

            switch (valueIndex)
            {
                case 1:
                    meaning = "APPLY_ABILITY ext-data application target: indicates whether the embedded sub-effect applies to the caster (self) or the ability target.";
                    notes = "Across 2075 APPLY_ABILITY component records in the extracted client BIN, ExtData Val1 consistently encodes the sub-effect application direction. "
                        + "Value 1 = Self (apply to the caster); Value 2 = Target (apply to the ability target or enemy). "
                        + "STAT_CHANGE and DAMAGE sub-ops use Val1=2 (target) in 98%+ of cases; "
                        + "INTERRUPT, AP_REGEN_CHANGE, AP_CHANGE, and HEAL sub-ops use Val1=1 (self) in 95%+ of cases.";
                    if (hasNumeric)
                    {
                        if (numericValue == 1) valueNote = "Val1=1: apply embedded sub-effect to SELF (the caster).";
                        else if (numericValue == 2) valueNote = "Val1=2: apply embedded sub-effect to TARGET (the ability target or enemy).";
                        else valueNote = "Val1=" + rawValue + " is an unrecognised application-target code for APPLY_ABILITY.";
                    }
                    break;

                default:
                    return false;
            }

            semantic = new ComponentFieldSemantic
            {
                DomainKey = "ApplyAbilityExtData",
                Meaning = meaning,
                Confidence = SemanticConfidence.Inferred,
                Source = "Cross-record ExtData pattern analysis of 2075 APPLY_ABILITY component rows in extracted client BIN",
                SourcePath = string.Empty,
                SourceLocation = string.Empty,
                Notes = notes + (string.IsNullOrWhiteSpace(valueNote) ? string.Empty : " " + valueNote)
            };
            return true;
        }

        private static bool TryResolveOperationSpecificStructuralSemantic(uint operationId, string fieldKey, string rawValue, out ComponentFieldSemantic semantic)
        {
            semantic = null;

            if (operationId == 1)
            {
                int slotIndex;
                int valueIndex;
                if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                    return false;

                string semanticSummary;
                string roleNotes;
                if (!TryDescribeDamageExtDataField(valueIndex, out semanticSummary, out roleNotes))
                    return false;

                string valueNote = BuildDamageStructuralValueNote(valueIndex, rawValue);
                semantic = new ComponentFieldSemantic
                {
                    DomainKey = BuildOperationFieldDomainKey(operationId, fieldKey),
                    Meaning = semanticSummary,
                    Confidence = SemanticConfidence.Structural,
                    Source = "DAMAGE structural inference",
                    SourcePath = string.Empty,
                    SourceLocation = string.Empty,
                    Notes = "This extracted-client-only inference comes from recurring DAMAGE ext-data layouts in slot "
                        + slotIndex.ToString(CultureInfo.InvariantCulture)
                        + ". "
                        + roleNotes
                        + (string.IsNullOrWhiteSpace(rawValue) ? string.Empty : " Current raw value: " + rawValue + ".")
                        + (string.IsNullOrWhiteSpace(valueNote) ? string.Empty : " " + valueNote)
                };
                return true;
            }

            if (operationId == 22)
            {
                int slotIndex;
                int valueIndex;
                if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                    return false;

                string semanticSummary;
                string roleNotes;
                if (!TryDescribeBonusTypeAdjustExtDataField(valueIndex, out semanticSummary, out roleNotes))
                    return false;

                string valueNote = BuildBonusTypeAdjustStructuralValueNote(valueIndex, rawValue);
                semantic = new ComponentFieldSemantic
                {
                    DomainKey = BuildOperationFieldDomainKey(operationId, fieldKey),
                    Meaning = semanticSummary,
                    Confidence = SemanticConfidence.Structural,
                    Source = "BONUS_TYPE_ADJUST structural inference",
                    SourcePath = string.Empty,
                    SourceLocation = string.Empty,
                    Notes = "This extracted-client-only inference comes from recurring BONUS_TYPE_ADJUST ext-data layouts in slot "
                        + slotIndex.ToString(CultureInfo.InvariantCulture)
                        + ". "
                        + roleNotes
                        + (string.IsNullOrWhiteSpace(rawValue) ? string.Empty : " Current raw value: " + rawValue + ".")
                        + (string.IsNullOrWhiteSpace(valueNote) ? string.Empty : " " + valueNote)
                };
                return true;
            }

            if (operationId == 23)
            {
                int slotIndex;
                int valueIndex;
                if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                    return false;

                string semanticSummary;
                string roleNotes;
                if (!TryDescribeApplyAbilityExtDataField(valueIndex, out semanticSummary, out roleNotes))
                    return false;

                string valueNote = BuildApplyAbilityStructuralValueNote(valueIndex, rawValue);
                semantic = new ComponentFieldSemantic
                {
                    DomainKey = BuildOperationFieldDomainKey(operationId, fieldKey),
                    Meaning = semanticSummary,
                    Confidence = SemanticConfidence.Structural,
                    Source = "APPLY_ABILITY structural inference",
                    SourcePath = string.Empty,
                    SourceLocation = string.Empty,
                    Notes = "This extracted-client-only inference comes from recurring APPLY_ABILITY ext-data layouts in slot "
                        + slotIndex.ToString(CultureInfo.InvariantCulture)
                        + ". "
                        + roleNotes
                        + (string.IsNullOrWhiteSpace(rawValue) ? string.Empty : " Current raw value: " + rawValue + ".")
                        + (string.IsNullOrWhiteSpace(valueNote) ? string.Empty : " " + valueNote)
                };
                return true;
            }

            if (operationId == 24)
            {
                string semanticSummary;
                string roleNotes;
                if (TryDescribeKnockbackValueField(fieldKey, out semanticSummary, out roleNotes))
                {
                    string knockbackValueNote = BuildKnockbackValueFieldNote(fieldKey, rawValue);
                    semantic = new ComponentFieldSemantic
                    {
                        DomainKey = BuildOperationFieldDomainKey(operationId, fieldKey),
                        Meaning = semanticSummary,
                        Confidence = SemanticConfidence.Structural,
                        Source = "KNOCKBACK structural inference",
                        SourcePath = string.Empty,
                        SourceLocation = string.Empty,
                        Notes = "This extracted-client-only inference comes from recurring KNOCKBACK value patterns in the component rows. "
                            + roleNotes
                            + (string.IsNullOrWhiteSpace(rawValue) ? string.Empty : " Current raw value: " + rawValue + ".")
                            + (string.IsNullOrWhiteSpace(knockbackValueNote) ? string.Empty : " " + knockbackValueNote)
                    };
                    return true;
                }

                int slotIndex;
                int valueIndex;
                if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                    return false;

                if (!TryDescribeKnockbackExtDataField(valueIndex, out semanticSummary, out roleNotes))
                    return false;

                string valueNote = BuildKnockbackStructuralValueNote(valueIndex, rawValue);
                semantic = new ComponentFieldSemantic
                {
                    DomainKey = BuildOperationFieldDomainKey(operationId, fieldKey),
                    Meaning = semanticSummary,
                    Confidence = SemanticConfidence.Structural,
                    Source = "KNOCKBACK structural inference",
                    SourcePath = string.Empty,
                    SourceLocation = string.Empty,
                    Notes = "This extracted-client-only inference comes from recurring KNOCKBACK ext-data layouts in slot "
                        + slotIndex.ToString(CultureInfo.InvariantCulture)
                        + ". "
                        + roleNotes
                        + (string.IsNullOrWhiteSpace(rawValue) ? string.Empty : " Current raw value: " + rawValue + ".")
                        + (string.IsNullOrWhiteSpace(valueNote) ? string.Empty : " " + valueNote)
                };
                return true;
            }

            if (operationId == 38)
            {
                int slotIndex;
                int valueIndex;
                if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                    return false;

                string semanticSummary;
                string roleNotes;
                if (!TryDescribeImmunityExtDataField(valueIndex, out semanticSummary, out roleNotes))
                    return false;

                string valueNote = BuildImmunityStructuralValueNote(valueIndex, rawValue);
                semantic = new ComponentFieldSemantic
                {
                    DomainKey = BuildOperationFieldDomainKey(operationId, fieldKey),
                    Meaning = semanticSummary,
                    Confidence = SemanticConfidence.Structural,
                    Source = "IMMUNITY structural inference",
                    SourcePath = string.Empty,
                    SourceLocation = string.Empty,
                    Notes = "This extracted-client-only inference comes from recurring IMMUNITY ext-data layouts in slot "
                        + slotIndex.ToString(CultureInfo.InvariantCulture)
                        + ". "
                        + roleNotes
                        + (string.IsNullOrWhiteSpace(rawValue) ? string.Empty : " Current raw value: " + rawValue + ".")
                        + (string.IsNullOrWhiteSpace(valueNote) ? string.Empty : " " + valueNote)
                };
                return true;
            }

            if (operationId != 12)
                return false;

            string fieldSemanticSummary;
            string fieldRoleNotes;
            string fieldValueNote;
            string sourceLabel;

            if (string.Equals(fieldKey, "FlagsRaw", StringComparison.OrdinalIgnoreCase))
            {
                fieldSemanticSummary = "CC packed control bitfield for branch and behavior selection.";
                fieldRoleNotes = "This field behaves like a packed bitfield or mode mask and should be read together with Value15 and the leading ExtData selector fields.";
                fieldValueNote = BuildCcStructuralValueNote(fieldKey, null, rawValue);
                sourceLabel = "CC structural inference";
            }
            else if (string.Equals(fieldKey, "Value15", StringComparison.OrdinalIgnoreCase))
            {
                fieldSemanticSummary = "CC auxiliary profile marker paired with FlagsRaw.";
                fieldRoleNotes = "This field behaves like a small companion selector or layout tag rather than a free scalar.";
                fieldValueNote = BuildCcStructuralValueNote(fieldKey, null, rawValue);
                sourceLabel = "CC structural inference";
            }
            else
            {
                int slotIndex;
                int valueIndex;
                if (!TryParseExtDataField(fieldKey, out slotIndex, out valueIndex))
                    return false;

                if (!TryDescribeCcExtDataField(valueIndex, out fieldSemanticSummary, out fieldRoleNotes))
                    return false;

                fieldValueNote = BuildCcStructuralValueNote(fieldKey, valueIndex, rawValue);
                sourceLabel = "CC structural inference";
                semantic = new ComponentFieldSemantic
                {
                    DomainKey = BuildOperationFieldDomainKey(operationId, fieldKey),
                    Meaning = fieldSemanticSummary,
                    Confidence = SemanticConfidence.Structural,
                    Source = sourceLabel,
                    SourcePath = string.Empty,
                    SourceLocation = string.Empty,
                    Notes = "This extracted-client-only inference comes from recurring CC ext-data layouts in slot "
                        + slotIndex.ToString(CultureInfo.InvariantCulture)
                        + ". "
                        + fieldRoleNotes
                        + (string.IsNullOrWhiteSpace(rawValue) ? string.Empty : " Current raw value: " + rawValue + ".")
                        + (string.IsNullOrWhiteSpace(fieldValueNote) ? string.Empty : " " + fieldValueNote)
                };
                return true;
            }

            semantic = new ComponentFieldSemantic
            {
                DomainKey = BuildOperationFieldDomainKey(operationId, fieldKey),
                Meaning = fieldSemanticSummary,
                Confidence = SemanticConfidence.Structural,
                Source = sourceLabel,
                SourcePath = string.Empty,
                SourceLocation = string.Empty,
                Notes = "This extracted-client-only inference comes from recurring CC component layouts. "
                    + fieldRoleNotes
                    + (string.IsNullOrWhiteSpace(rawValue) ? string.Empty : " Current raw value: " + rawValue + ".")
                    + (string.IsNullOrWhiteSpace(fieldValueNote) ? string.Empty : " " + fieldValueNote)
            };
            return true;
        }

        private static bool TryResolveNamedControlFieldSemantic(uint operationId, string fieldKey, string rawValue, out ComponentFieldSemantic semantic)
        {
            semantic = null;

            string semanticSummary;
            string roleNotes;
            if (!TryDescribeNamedControlField(fieldKey, out semanticSummary, out roleNotes))
                return false;

            string valueNote = BuildNamedControlFieldValueNote(fieldKey, rawValue);
            semantic = new ComponentFieldSemantic
            {
                DomainKey = BuildOperationFieldDomainKey(operationId, fieldKey),
                Meaning = semanticSummary,
                Confidence = SemanticConfidence.Inferred,
                Source = "Named control field inference",
                SourcePath = string.Empty,
                SourceLocation = string.Empty,
                Notes = "This inferred meaning comes from the descriptive client field name and the observed value shapes in extracted client BIN rows. "
                    + roleNotes
                    + (string.IsNullOrWhiteSpace(rawValue) ? string.Empty : " Current raw value: " + rawValue + ".")
                    + (string.IsNullOrWhiteSpace(valueNote) ? string.Empty : " " + valueNote)
                    + " Exact display formatting is not directly token-confirmed yet."
            };
            return true;
        }

        private static bool TryResolveGenericFlagsRawSemantic(uint operationId, string fieldKey, string rawValue, out ComponentFieldSemantic semantic)
        {
            semantic = null;

            string semanticSummary;
            string roleNotes;
            if (!TryDescribeGenericFlagsRawField(fieldKey, out semanticSummary, out roleNotes))
                return false;

            string valueNote = BuildGenericFlagsRawValueNote(rawValue);
            semantic = new ComponentFieldSemantic
            {
                DomainKey = BuildOperationFieldDomainKey(operationId, fieldKey),
                Meaning = semanticSummary,
                Confidence = SemanticConfidence.Structural,
                Source = "FlagsRaw structural inference",
                SourcePath = string.Empty,
                SourceLocation = string.Empty,
                Notes = "This extracted-client-only inference comes from the explicit client field name and the recurring raw bit-pattern values in the component rows. "
                    + roleNotes
                    + (string.IsNullOrWhiteSpace(rawValue) ? string.Empty : " Current raw value: " + rawValue + ".")
                    + (string.IsNullOrWhiteSpace(valueNote) ? string.Empty : " " + valueNote)
            };
            return true;
        }

        private static int CountSetBits(ulong value)
        {
            int bitCount = 0;
            while (value != 0)
            {
                if ((value & 1UL) != 0)
                    bitCount++;

                value >>= 1;
            }

            return bitCount;
        }

        private static string BuildSetBitSummary(ulong value, int limit)
        {
            if (value == 0)
                return "0";

            List<string> bits = new List<string>();
            int bitIndex = 0;
            int totalCount = 0;
            ulong remaining = value;

            while (remaining != 0)
            {
                if ((remaining & 1UL) != 0)
                {
                    totalCount++;
                    if (bits.Count < limit)
                        bits.Add(bitIndex.ToString(CultureInfo.InvariantCulture));
                }

                remaining >>= 1;
                bitIndex++;
            }

            if (bits.Count == 0)
                return string.Empty;

            string summary = string.Join(", ", bits);
            if (totalCount > bits.Count)
                summary += ", ...";

            return summary;
        }

        private static bool TryFormatFieldValue(long rawValue, out string valueText)
        {
            valueText = null;
            if (rawValue == 0)
                return false;

            valueText = rawValue.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        private static ComponentOperationFieldCorrelationRecord BuildCompanionFieldCorrelation(string fieldKey, List<FieldObservation> observations, int selectedComponentCount)
        {
            if (string.IsNullOrWhiteSpace(fieldKey)
                || observations == null
                || observations.Count == 0
                || selectedComponentCount <= 0)
                return null;

            List<FieldValueCount> values = observations
                .GroupBy(row => row.RawValue, StringComparer.OrdinalIgnoreCase)
                .Select(group => new FieldValueCount
                {
                    RawValue = group.Key,
                    Count = group.Select(row => row.ComponentId).Distinct().Count()
                })
                .OrderByDescending(row => row.Count)
                .ThenBy(row => row.RawValue, StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (values.Count == 0)
                return null;

            FieldValueCount dominant = values[0];
            int observationCount = observations.Select(row => row.ComponentId).Distinct().Count();
            int coveragePercent = dominant.Count * 100 / selectedComponentCount;
            int minimumObservations = Math.Max(3, selectedComponentCount / 5);
            if (coveragePercent < 50 && observationCount < minimumObservations)
                return null;

            return new ComponentOperationFieldCorrelationRecord
            {
                FieldKey = fieldKey,
                DominantValue = dominant.RawValue,
                MatchCount = dominant.Count,
                ObservationCount = observationCount,
                CoveragePercent = coveragePercent,
                DistinctValueCount = values.Count,
                SampleValuesText = JoinValues(values.Select(row => row.RawValue).Take(6))
            };
        }

        private static string BuildValueCountSummary(IEnumerable<string> values, int maxItems, string emptyText)
        {
            List<string> items = values == null
                ? new List<string>()
                : values
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .GroupBy(value => value, StringComparer.OrdinalIgnoreCase)
                    .Select(group => new FieldValueCount
                    {
                        RawValue = group.Key,
                        Count = group.Count()
                    })
                    .OrderByDescending(row => row.Count)
                    .ThenBy(row => row.RawValue, StringComparer.OrdinalIgnoreCase)
                    .Select(row => row.RawValue + " x" + row.Count.ToString(CultureInfo.InvariantCulture))
                    .Take(maxItems)
                    .ToList();

            if (items.Count == 0)
                return emptyText;

            return string.Join(", ", items);
        }

        private List<string> GetDomainContextTags(string domainKey)
        {
            List<ComponentTokenEvidence> evidenceRows;
            if (!_evidenceByDomainKey.TryGetValue(domainKey, out evidenceRows))
                return new List<string>();

            return evidenceRows
                .SelectMany(row => row.ContextTags ?? new List<string>())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string DetermineDomainConfidence(DefinitionDomain domain)
        {
            if (domain == null || domain.Options == null || domain.Options.Count == 0)
                return SemanticConfidence.Unknown;
            if (domain.Options.Any(option => string.Equals(option.Confidence, SemanticConfidence.Confirmed, StringComparison.OrdinalIgnoreCase)))
                return SemanticConfidence.Confirmed;
            if (domain.Options.Any(option => string.Equals(option.Confidence, SemanticConfidence.Inferred, StringComparison.OrdinalIgnoreCase)))
                return SemanticConfidence.Inferred;
            if (domain.Options.Any(option => string.Equals(option.Confidence, SemanticConfidence.Structural, StringComparison.OrdinalIgnoreCase)))
                return SemanticConfidence.Structural;
            if (domain.Options.Any(option => string.Equals(option.Confidence, SemanticConfidence.Londo, StringComparison.OrdinalIgnoreCase)))
                return SemanticConfidence.Londo;
            return SemanticConfidence.Unknown;
        }

        private static string BuildOperationFieldNotes(string fieldKey, List<FieldObservation> observations, DefinitionDomain domain, int componentCount)
        {
            List<string> sampleComponents = observations.Select(row => row.ComponentId.ToString(CultureInfo.InvariantCulture)).Distinct().OrderBy(value => value).Take(12).ToList();
            List<string> sampleLayouts = observations.Select(row => row.LayoutVariant).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value, StringComparer.OrdinalIgnoreCase).Take(6).ToList();
            string summary = observations.Count.ToString(CultureInfo.InvariantCulture)
                + " non-zero observation(s) across "
                + componentCount.ToString(CultureInfo.InvariantCulture)
                + " component row(s); example component ids: "
                + JoinValues(sampleComponents)
                + ".";

            if (sampleLayouts.Count > 0)
                summary += " Layouts: " + JoinValues(sampleLayouts) + ".";
            if (domain != null && !string.IsNullOrWhiteSpace(domain.Notes))
                summary += " " + domain.Notes;

            return summary.Trim();
        }

        private static FieldTriage BuildFieldTriage(uint operationId, string fieldKey, List<FieldObservation> observations, string confidence, int operationAbilityCount)
        {
            bool isStructural = string.Equals(confidence, SemanticConfidence.Structural, StringComparison.OrdinalIgnoreCase);
            if (!string.Equals(confidence, SemanticConfidence.Unknown, StringComparison.OrdinalIgnoreCase) && !isStructural)
            {
                return new FieldTriage
                {
                    Score = 0,
                    Bucket = "Resolved",
                    Notes = "This field already has extracted-client semantic coverage and does not need unknown-first triage."
                };
            }

            int nonZeroCount = observations == null ? 0 : observations.Count;
            int distinctValueCount = observations == null ? 0 : observations.Select(row => row.RawValue).Distinct(StringComparer.OrdinalIgnoreCase).Count();
            int score = 0;
            List<string> reasons = new List<string>();

            if (PriorityOperations.Contains(operationId))
            {
                score += 100;
                reasons.Add("priority operation");
            }

            if (operationAbilityCount > 0)
            {
                score += Math.Min(30, operationAbilityCount / 40);
                if (operationAbilityCount >= 200)
                    reasons.Add(operationAbilityCount.ToString(CultureInfo.InvariantCulture) + " referencing abilities");
            }

            if (nonZeroCount > 0)
            {
                score += Math.Min(60, nonZeroCount / 20);
                if (nonZeroCount >= 100)
                    reasons.Add(nonZeroCount.ToString(CultureInfo.InvariantCulture) + " non-zero rows");
            }

            if (distinctValueCount > 0)
            {
                score += Math.Min(40, distinctValueCount * 2);
                if (distinctValueCount >= 10)
                    reasons.Add(distinctValueCount.ToString(CultureInfo.InvariantCulture) + " distinct values");
            }

            if (fieldKey.StartsWith("ExtData[", StringComparison.OrdinalIgnoreCase))
            {
                score += 35;
                reasons.Add("ext-data slot");
            }
            else if (string.Equals(fieldKey, "FlagsRaw", StringComparison.OrdinalIgnoreCase))
            {
                score += 30;
                reasons.Add("flag bitfield candidate");
            }
            else if (string.Equals(fieldKey, "ActivationDelay", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fieldKey, "Interval", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fieldKey, "MaxTargets", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fieldKey, "ConeAngle", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fieldKey, "FlightSpeed", StringComparison.OrdinalIgnoreCase))
            {
                score += 20;
                reasons.Add("gameplay control field");
            }
            else if (fieldKey.StartsWith("Value[", StringComparison.OrdinalIgnoreCase))
            {
                score += 10;
                reasons.Add("generic value slot");
            }

            if (fieldKey.StartsWith("Multiplier[", StringComparison.OrdinalIgnoreCase))
            {
                score -= 80;
                reasons.Add("multiplier-array noise");
            }

            if (isStructural)
            {
                score = Math.Max(0, score - 20);
                reasons.Add("structural role inferred");
            }

            if (score < 0)
                score = 0;

            return new FieldTriage
            {
                Score = score,
                Bucket = DetermineFieldTriageBucket(score),
                Notes = reasons.Count == 0
                    ? "Low-signal unknown field."
                    : string.Join("; ", reasons.Distinct(StringComparer.OrdinalIgnoreCase)) + "."
            };
        }

        private static string DetermineFieldTriageBucket(int score)
        {
            if (score >= 170)
                return "Critical";
            if (score >= 120)
                return "High";
            if (score >= 70)
                return "Medium";
            if (score > 0)
                return "Low";
            return "Noise";
        }

        private static string JoinValues(IEnumerable<string> values)
        {
            List<string> items = values == null
                ? new List<string>()
                : values.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            return items.Count == 0 ? string.Empty : string.Join(", ", items);
        }

        private static string BuildTriggerText(uint triggerValue)
        {
            if (triggerValue == 0)
                return "0 -> OnEnd";

            return triggerValue.ToString(CultureInfo.InvariantCulture) + " -> " + DefinitionCatalog.DescribeTriggerValue(triggerValue);
        }

        private static string GetFieldSortKey(string fieldKey)
        {
            int bucket = GetFieldSortBucket(fieldKey);
            return bucket.ToString("D3", CultureInfo.InvariantCulture) + "|" + fieldKey;
        }

        private static int GetFieldSortBucket(string fieldKey)
        {
            if (string.Equals(fieldKey, "ActivationDelay", StringComparison.OrdinalIgnoreCase)) return 10;
            if (string.Equals(fieldKey, "Duration", StringComparison.OrdinalIgnoreCase)) return 20;
            if (string.Equals(fieldKey, "Interval", StringComparison.OrdinalIgnoreCase)) return 30;
            if (string.Equals(fieldKey, "Radius", StringComparison.OrdinalIgnoreCase)) return 40;
            if (string.Equals(fieldKey, "ConeAngle", StringComparison.OrdinalIgnoreCase)) return 50;
            if (string.Equals(fieldKey, "FlightSpeed", StringComparison.OrdinalIgnoreCase)) return 60;
            if (string.Equals(fieldKey, "MaxTargets", StringComparison.OrdinalIgnoreCase)) return 70;
            if (string.Equals(fieldKey, "FlagsRaw", StringComparison.OrdinalIgnoreCase)) return 80;
            if (string.Equals(fieldKey, "Value00", StringComparison.OrdinalIgnoreCase)) return 90;
            if (string.Equals(fieldKey, "Value08", StringComparison.OrdinalIgnoreCase)) return 100;
            if (string.Equals(fieldKey, "Value15", StringComparison.OrdinalIgnoreCase)) return 110;
            if (fieldKey.StartsWith("Value[", StringComparison.OrdinalIgnoreCase)) return 200;
            if (fieldKey.StartsWith("Multiplier[", StringComparison.OrdinalIgnoreCase)) return 300;
            if (fieldKey.StartsWith("ExtData[", StringComparison.OrdinalIgnoreCase)) return 400;
            return 900;
        }

        private static void AddEvidence(Dictionary<string, List<ComponentTokenEvidence>> target, string key, ComponentTokenEvidence evidence)
        {
            List<ComponentTokenEvidence> items;
            if (!target.TryGetValue(key, out items))
            {
                items = new List<ComponentTokenEvidence>();
                target[key] = items;
            }

            items.Add(evidence);
        }

        private FieldObservationInference BuildFieldObservationInference(uint operationId, string fieldKey, List<FieldObservation> observations)
        {
            if (observations == null || observations.Count == 0)
                return null;

            if (IsRequirementLinkField(fieldKey) && _knownRequirementIds.Count > 0)
            {
                List<ushort> matchedRequirementIds = observations
                    .Select(row => ParseRequirementId(row.RawValue))
                    .Where(value => value.HasValue)
                    .Select(value => value.Value)
                    .Distinct()
                    .OrderBy(value => value)
                    .ToList();
                if (matchedRequirementIds.Count > 0)
                {
                    int distinctValueCount = observations.Select(row => row.RawValue).Distinct(StringComparer.OrdinalIgnoreCase).Count();
                    string exampleIds = string.Join(", ", matchedRequirementIds.Take(12).Select(value => value.ToString(CultureInfo.InvariantCulture)));
                    return new FieldObservationInference
                    {
                        SemanticSummary = matchedRequirementIds.Count == distinctValueCount
                            ? "Exact raw values in this field match known RequirementId rows in abilityrequirementexport.bin."
                            : "This field includes raw values that match known RequirementId rows in abilityrequirementexport.bin.",
                        Confidence = SemanticConfidence.Inferred,
                        Notes = matchedRequirementIds.Count == distinctValueCount
                            ? "Every distinct observed value in this field matches a known RequirementId; example requirement ids: " + exampleIds + "."
                            : matchedRequirementIds.Count.ToString(CultureInfo.InvariantCulture) + " distinct value(s) in this field match known RequirementId rows; example requirement ids: " + exampleIds + "."
                    };
                }
            }

            FieldObservationInference structuralInference;
            if (TryBuildNamedControlFieldInference(fieldKey, observations, out structuralInference))
                return structuralInference;

            // Val2/Val3/Val4 universal decodes fire before per-operation structural handlers.
            if (TryBuildExtDataVal2Inference(fieldKey, observations, out structuralInference))
                return structuralInference;

            if (TryBuildExtDataVal3Inference(fieldKey, observations, out structuralInference))
                return structuralInference;

            if (TryBuildExtDataVal4Inference(fieldKey, observations, out structuralInference))
                return structuralInference;

            if (TryBuildDamageStructuralInference(operationId, fieldKey, observations, out structuralInference))
                return structuralInference;

            if (TryBuildBonusTypeAdjustStructuralInference(operationId, fieldKey, observations, out structuralInference))
                return structuralInference;

            if (TryBuildCcStructuralInference(operationId, fieldKey, observations, out structuralInference))
                return structuralInference;

            if (TryBuildKnockbackStructuralInference(operationId, fieldKey, observations, out structuralInference))
                return structuralInference;

            if (TryBuildImmunityStructuralInference(operationId, fieldKey, observations, out structuralInference))
                return structuralInference;

            if (TryBuildGenericFlagsRawInference(fieldKey, observations, out structuralInference))
                return structuralInference;

            if (TryBuildApplyAbilityExtDataKnownFieldInference(operationId, fieldKey, observations, out structuralInference))
                return structuralInference;

            return TryBuildApplyAbilityStructuralInference(operationId, fieldKey, observations, out structuralInference)
                ? structuralInference
                : null;
        }

        private static string BuildAbilityComponentFieldKey(ushort abilityId, ushort componentId, string fieldKey)
        {
            return abilityId.ToString(CultureInfo.InvariantCulture)
                + "|"
                + componentId.ToString(CultureInfo.InvariantCulture)
                + "|"
                + fieldKey;
        }

        private static bool TryParseTokenField(string tokenBody, uint operation, out ComponentFieldDescriptor descriptor)
        {
            descriptor = null;
            if (string.IsNullOrWhiteSpace(tokenBody))
                return false;

            if (tokenBody.StartsWith("DURA_", StringComparison.OrdinalIgnoreCase))
            {
                descriptor = CreateFieldDescriptor(operation, "Duration", tokenBody, BuildMeaningForTokenBody(tokenBody, 0));
                return true;
            }

            if (tokenBody.StartsWith("FREQ_", StringComparison.OrdinalIgnoreCase))
            {
                descriptor = CreateFieldDescriptor(operation, "Interval", tokenBody, BuildMeaningForTokenBody(tokenBody, 0));
                return true;
            }

            if (tokenBody.StartsWith("RADI_", StringComparison.OrdinalIgnoreCase))
            {
                descriptor = CreateFieldDescriptor(operation, "Radius", tokenBody, BuildMeaningForTokenBody(tokenBody, 0));
                return true;
            }

            if (tokenBody.StartsWith("VAL", StringComparison.OrdinalIgnoreCase))
            {
                int index = 3;
                while (index < tokenBody.Length && char.IsDigit(tokenBody[index]))
                    ++index;

                int valueIndex;
                if (!int.TryParse(tokenBody.Substring(3, index - 3), NumberStyles.Integer, CultureInfo.InvariantCulture, out valueIndex))
                    return false;

                descriptor = CreateFieldDescriptor(operation, "Value[" + valueIndex.ToString(CultureInfo.InvariantCulture) + "]", tokenBody, BuildMeaningForTokenBody(tokenBody, 0));
                return true;
            }

            return false;
        }

        private static ComponentFieldDescriptor CreateFieldDescriptor(uint operation, string fieldKey, string tokenBody, string meaning)
        {
            return new ComponentFieldDescriptor
            {
                FieldKey = fieldKey,
                DomainKey = BuildOperationFieldDomainKey(operation, fieldKey),
                TokenBody = tokenBody,
                TokenMeaning = meaning
            };
        }

        private static string BuildEvidenceMeaning(IEnumerable<ComponentTokenEvidence> evidenceRows)
        {
            List<string> meanings = evidenceRows
                .Select(row => row.Meaning)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                .ToList();
            return meanings.Count == 0 ? "No semantic text evidence is known." : string.Join(" ", meanings);
        }

        private static string BuildEvidenceNotes(IEnumerable<ComponentTokenEvidence> evidenceRows)
        {
            List<ComponentTokenEvidence> rows = evidenceRows.ToList();
            if (rows.Count == 0)
                return string.Empty;

            string tokens = string.Join(", ", rows.Select(row => row.TokenBody).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value));
            string abilities = string.Join(", ", rows.Select(row => row.AbilityId.ToString(CultureInfo.InvariantCulture)).Distinct().OrderBy(value => value).Take(12));
            return rows.Count.ToString(CultureInfo.InvariantCulture) + " evidence row(s); tokens: " + tokens + "; example ability ids: " + abilities + ".";
        }

        private static string BuildSharedEvidenceNotes(string fieldKey, IEnumerable<ComponentTokenEvidence> evidenceRows)
        {
            List<ComponentTokenEvidence> rows = evidenceRows.ToList();
            if (rows.Count == 0)
                return string.Empty;

            string tokens = string.Join(", ", rows.Select(row => row.TokenBody).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value, StringComparer.OrdinalIgnoreCase));
            string operations = string.Join(", ", rows.Select(row => DefinitionCatalog.DescribeComponentOperationValue(row.Operation) + " [" + row.Operation.ToString(CultureInfo.InvariantCulture) + "]").Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value, StringComparer.OrdinalIgnoreCase).Take(8));
            string abilities = string.Join(", ", rows.Select(row => row.AbilityId.ToString(CultureInfo.InvariantCulture)).Distinct().OrderBy(value => value).Take(12));
            return rows.Count.ToString(CultureInfo.InvariantCulture)
                + " shared-field evidence row(s) for "
                + fieldKey
                + "; tokens: "
                + tokens
                + "; source operations: "
                + operations
                + "; example ability ids: "
                + abilities
                + ".";
        }

        private static string BuildSourceLabel(IEnumerable<ComponentTokenEvidence> evidenceRows)
        {
            return string.Join(", ", evidenceRows.Select(row => row.Source).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value));
        }

        private bool TryResolveRequirementFieldSemantic(string fieldKey, string rawValue, out ComponentFieldSemantic semantic)
        {
            semantic = null;
            if (!IsRequirementLinkField(fieldKey))
                return false;

            ushort requirementId;
            if (!TryParseRequirementId(rawValue, out requirementId))
                return false;

            BinaryRequirementRecord requirementRow;
            if (!_requirementsById.TryGetValue(requirementId, out requirementRow))
                return false;

            RequirementSemanticDescription description = _requirements.DescribeRequirement(requirementId);
            semantic = new ComponentFieldSemantic
            {
                DomainKey = "Requirement.RequirementId",
                Meaning = description.Meaning,
                Confidence = description.Confidence,
                Source = "Inferred requirement link",
                SourcePath = requirementRow.SourcePath,
                SourceLocation = FormatSourceLocation(requirementRow),
                Notes = description.Notes
            };
            return true;
        }

        private static ComponentFieldSemantic CreateUnknown(string notes)
        {
            return new ComponentFieldSemantic
            {
                DomainKey = string.Empty,
                Meaning = "No extracted-client semantic mapping is known yet for this field.",
                Confidence = SemanticConfidence.Unknown,
                Source = "No direct evidence",
                SourcePath = string.Empty,
                SourceLocation = string.Empty,
                Notes = notes
            };
        }

        private static string BuildDisplayName(string domainKey)
        {
            string[] parts = (domainKey ?? string.Empty).Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4)
                return domainKey ?? string.Empty;

            uint operation;
            string operationLabel = uint.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out operation)
                ? "Operation " + operation.ToString(CultureInfo.InvariantCulture)
                : "Operation";
            return operationLabel + " :: " + string.Join(".", parts.Skip(3));
        }

        private static bool TryExtractFieldKey(string domainKey, out string fieldKey)
        {
            fieldKey = string.Empty;
            const string prefix = "Component.Operation.";
            if (string.IsNullOrWhiteSpace(domainKey) || !domainKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return false;

            int separatorIndex = domainKey.IndexOf('.', prefix.Length);
            if (separatorIndex < 0 || separatorIndex + 1 >= domainKey.Length)
                return false;

            fieldKey = domainKey.Substring(separatorIndex + 1);
            return !string.IsNullOrWhiteSpace(fieldKey);
        }

        private static bool IsRequirementLinkField(string fieldKey)
        {
            return !string.IsNullOrWhiteSpace(fieldKey) && fieldKey.EndsWith("Val6", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSharedFieldFallbackAllowed(string fieldKey)
        {
            return string.Equals(fieldKey, "Duration", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fieldKey, "Interval", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fieldKey, "Radius", StringComparison.OrdinalIgnoreCase);
        }

        private ushort? ParseRequirementId(string rawValue)
        {
            ushort requirementId;
            return TryParseRequirementId(rawValue, out requirementId) ? requirementId : (ushort?)null;
        }

        private bool TryParseRequirementId(string rawValue, out ushort requirementId)
        {
            requirementId = 0;
            if (string.IsNullOrWhiteSpace(rawValue))
                return false;

            if (!ushort.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out requirementId))
                return false;

            return _knownRequirementIds.Contains(requirementId);
        }

        private static string FormatSourceLocation(SourceRowBase row)
        {
            if (row == null)
                return string.Empty;
            if (row.ByteOffset > 0)
                return "byte " + row.ByteOffset.ToString(CultureInfo.InvariantCulture);
            if (row.LineNumber > 0)
                return "line " + row.LineNumber.ToString(CultureInfo.InvariantCulture);
            return string.Empty;
        }

        private static string BuildPlainEnglishDefinition(ComponentTokenEvidence evidence)
        {
            if (evidence == null)
                return string.Empty;

            return "Read component slot "
                + evidence.ComponentSlotIndex.ToString(CultureInfo.InvariantCulture)
                + " (component id "
                + evidence.ComponentId.ToString(CultureInfo.InvariantCulture)
                + "), then "
                + TrimTrailingPeriod(evidence.Meaning);
        }

        private static string BuildGlossaryNotes(IEnumerable<ComponentTokenEvidence> evidenceRows)
        {
            List<ComponentTokenEvidence> rows = evidenceRows.ToList();
            if (rows.Count == 0)
                return string.Empty;

            ComponentTokenEvidence example = rows[0];
            string operations = string.Join(", ", rows.Select(row => row.Operation.ToString(CultureInfo.InvariantCulture)).Distinct().OrderBy(value => value));
            string abilityIds = string.Join(", ", rows.Select(row => row.AbilityId.ToString(CultureInfo.InvariantCulture)).Distinct().OrderBy(value => value).Take(24));
            string exampleText = rows.Select(row => row.TextExcerpt).FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
            string contextTags = string.Join(", ", rows.SelectMany(row => row.ContextTags ?? new List<string>()).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value));
            return "Observed in "
                + rows.Count.ToString(CultureInfo.InvariantCulture)
                + " extracted string occurrence(s); component field "
                + example.FieldKey
                + "; component operation ids "
                + operations
                + (string.IsNullOrWhiteSpace(contextTags) ? string.Empty : "; context tags " + contextTags)
                + "; example ability ids "
                + abilityIds
                + (string.IsNullOrWhiteSpace(exampleText) ? "." : "; example text: \"" + Truncate(exampleText, 220) + "\".");
        }

        private static string BuildMeaningForTokenBody(string tokenBody, int depth)
        {
            if (string.IsNullOrWhiteSpace(tokenBody))
                return "read an unresolved component field and format it for client text.";
            if (depth > 12)
                return "resolve a deeply nested token chain that exceeds the current decomposition limit.";

            if (tokenBody.StartsWith("DURA_", StringComparison.OrdinalIgnoreCase))
                return "read the selected component's Duration field and format it as " + DescribeRenderSuffix(tokenBody.Substring(5), depth + 1) + ".";

            if (tokenBody.StartsWith("FREQ_", StringComparison.OrdinalIgnoreCase))
                return "read the selected component's Interval field and format it as " + DescribeRenderSuffix(tokenBody.Substring(5), depth + 1) + ".";

            if (tokenBody.StartsWith("RADI_", StringComparison.OrdinalIgnoreCase))
                return "read the selected component's Radius field and format it as " + DescribeRenderSuffix(tokenBody.Substring(5), depth + 1) + ".";

            if (tokenBody.StartsWith("VAL", StringComparison.OrdinalIgnoreCase))
            {
                int index = 3;
                while (index < tokenBody.Length && char.IsDigit(tokenBody[index]))
                    ++index;

                int valueIndex;
                if (!int.TryParse(tokenBody.Substring(3, index - 3), NumberStyles.Integer, CultureInfo.InvariantCulture, out valueIndex))
                    return "read an unresolved component value field and format it for client text.";

                string suffix = index < tokenBody.Length && tokenBody[index] == '_' ? tokenBody.Substring(index + 1) : string.Empty;
                if (string.IsNullOrWhiteSpace(suffix))
                    return "read the selected component's Value[" + valueIndex.ToString(CultureInfo.InvariantCulture) + "] field and insert the raw numeric result into client text.";

                return "read the selected component's Value[" + valueIndex.ToString(CultureInfo.InvariantCulture) + "] field and render it as " + DescribeRenderSuffix(suffix, depth + 1) + " in this text context.";
            }

            return "resolve token `" + tokenBody + "` using a client-text formatting rule that has not been named yet.";
        }

        private static string DescribeRenderSuffix(string suffix, int depth)
        {
            if (string.IsNullOrWhiteSpace(suffix))
                return "a raw client value";
            if (depth > 12)
                return "a deeply nested client value";

            if (suffix.StartsWith("COM_", StringComparison.OrdinalIgnoreCase))
            {
                EmbeddedComReference reference;
                if (TryParseEmbeddedComReference(suffix, out reference))
                {
                    return "the same presentation as `" + reference.FullToken + "` (" + TrimTrailingPeriod(BuildEmbeddedComMeaning(reference, depth + 1)) + ")";
                }
            }

            string normalized = NormalizeSpecialSuffix(suffix);
            return string.IsNullOrWhiteSpace(normalized) ? suffix.ToLowerInvariant().Replace("_", " ") : normalized;
        }

        private static string BuildEmbeddedComMeaning(EmbeddedComReference reference, int depth)
        {
            return "read component slot "
                + reference.ComponentSlotIndex.ToString(CultureInfo.InvariantCulture)
                + ", then "
                + TrimTrailingPeriod(BuildMeaningForTokenBody(reference.TokenBody, depth + 1));
        }

        private static bool TryParseEmbeddedComReference(string rawValue, out EmbeddedComReference reference)
        {
            reference = null;
            if (string.IsNullOrWhiteSpace(rawValue) || !rawValue.StartsWith("COM_", StringComparison.OrdinalIgnoreCase))
                return false;

            int cursor = 4;
            int digitStart = cursor;
            while (cursor < rawValue.Length && char.IsDigit(rawValue[cursor]))
                ++cursor;

            if (cursor <= digitStart || cursor >= rawValue.Length || rawValue[cursor] != '_')
                return false;

            int componentSlotIndex;
            if (!int.TryParse(rawValue.Substring(digitStart, cursor - digitStart), NumberStyles.Integer, CultureInfo.InvariantCulture, out componentSlotIndex))
                return false;

            string tokenBody = rawValue.Substring(cursor + 1);
            reference = new EmbeddedComReference
            {
                ComponentSlotIndex = componentSlotIndex,
                TokenBody = tokenBody,
                FullToken = "COM_" + componentSlotIndex.ToString(CultureInfo.InvariantCulture) + "_" + tokenBody
            };
            return true;
        }

        private static string NormalizeSpecialSuffix(string suffix)
        {
            if (string.IsNullOrWhiteSpace(suffix))
                return string.Empty;

            string upper = suffix.ToUpperInvariant();
            if (upper == "TOD")
                return "an over-time amount";
            if (upper == "DAMAGE")
                return "damage";
            if (upper == "HEALTH")
                return "health";
            if (upper == "ACTIONPOINTS")
                return "action points";
            if (upper == "SECONDS")
                return "seconds";
            if (upper == "MINUTES")
                return "minutes";
            if (upper == "HOURS")
                return "hours";
            if (upper == "FEET")
                return "feet";
            if (upper == "ELEMENTALDAMAGE")
                return "elemental damage";
            if (upper == "SPIRITDAMAGE")
                return "spirit damage";
            if (upper == "CORPOREALDAMAGE")
                return "corporeal damage";
            if (upper.StartsWith("TOD_", StringComparison.OrdinalIgnoreCase))
                return NormalizeSpecialSuffix(upper.Substring(4)) + " over time";

            return upper.ToLowerInvariant().Replace("_", " ");
        }

        private static string TrimTrailingPeriod(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.TrimEnd().TrimEnd('.');
        }

        private string LookupAbilityName(ushort abilityId)
        {
            string name;
            return _abilityNamesById.TryGetValue(abilityId, out name) ? name : string.Empty;
        }

        private static Dictionary<ushort, string> BuildAbilityNames(AbilityDataset dataset)
        {
            Dictionary<ushort, string> names = new Dictionary<ushort, string>();
            foreach (ClientAbilityRecord row in dataset.ClientAbilities.Where(row => row.AbilityId <= ushort.MaxValue))
            {
                ushort abilityId = (ushort)row.AbilityId;
                if (!names.ContainsKey(abilityId) && !string.IsNullOrWhiteSpace(row.Name))
                    names[abilityId] = row.Name;
            }

            foreach (IndexedStringRecord row in dataset.AbilityNames.Where(row => row.EntryId <= ushort.MaxValue))
            {
                ushort abilityId = (ushort)row.EntryId;
                if (!names.ContainsKey(abilityId) && !string.IsNullOrWhiteSpace(row.NormalizedValue))
                    names[abilityId] = row.NormalizedValue;
            }

            return names;
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length <= maxLength)
                return value ?? string.Empty;

            return value.Substring(0, Math.Max(0, maxLength - 3)).TrimEnd() + "...";
        }

        private static List<string> BuildContextTags(string textExcerpt)
        {
            List<string> tags = new List<string>();
            string text = (textExcerpt ?? string.Empty).ToLowerInvariant();
            AddTagIfPresent(tags, text, "knockback", "Knockback");
            AddTagIfPresent(tags, text, "knock down", "Knockdown");
            AddTagIfPresent(tags, text, "knockdown", "Knockdown");
            AddTagIfPresent(tags, text, "crowd control", "CrowdControl");
            AddTagIfPresent(tags, text, "immunity", "Immunity");
            AddTagIfPresent(tags, text, "immune", "Immunity");
            AddTagIfPresent(tags, text, "stagger", "Stagger");
            AddTagIfPresent(tags, text, "stun", "Stun");
            AddTagIfPresent(tags, text, "root", "Root");
            AddTagIfPresent(tags, text, "snare", "Snare");
            AddTagIfPresent(tags, text, "silence", "Silence");
            AddTagIfPresent(tags, text, "disarm", "Disarm");
            AddTagIfPresent(tags, text, "heal", "Heal");
            AddTagIfPresent(tags, text, "damage", "Damage");
            return tags;
        }

        private static void AddTagIfPresent(List<string> tags, string text, string needle, string tag)
        {
            if (text.Contains(needle) && !tags.Contains(tag))
                tags.Add(tag);
        }

        private static Dictionary<string, ComponentSchemaOverride> LoadOverrides()
        {
            Dictionary<string, ComponentSchemaOverride> overrides = new Dictionary<string, ComponentSchemaOverride>(StringComparer.OrdinalIgnoreCase);
            string path = ResolveOverridePath();
            if (!File.Exists(path))
                return overrides;

            foreach (string rawLine in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(rawLine) || rawLine.StartsWith("#", StringComparison.Ordinal))
                    continue;

                string[] columns = rawLine.Split('\t');
                if (columns.Length < 6 || string.Equals(columns[0], "DomainKey", StringComparison.OrdinalIgnoreCase))
                    continue;

                overrides[columns[0]] = new ComponentSchemaOverride
                {
                    DomainKey = columns[0],
                    Meaning = columns[1],
                    Confidence = columns[2],
                    SourceLabel = columns[3],
                    SourcePath = columns[4],
                    SourceLocation = columns[5],
                    Notes = columns.Length > 6 ? columns[6] : string.Empty
                };
            }

            return overrides;
        }

        private static string ResolveOverridePath()
        {
            foreach (string root in new[]
            {
                AppDomain.CurrentDomain.BaseDirectory,
                Path.GetDirectoryName(typeof(ComponentSchemaCatalog).Assembly.Location),
                Environment.CurrentDirectory
            }.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                string resolved = TryResolveOverridePath(root);
                if (!string.IsNullOrWhiteSpace(resolved))
                    return resolved;
            }

            return Path.GetFullPath(OverrideRelativePath);
        }

        private static string TryResolveOverridePath(string startDirectory)
        {
            string current = startDirectory;
            for (int depth = 0; depth < 8 && !string.IsNullOrWhiteSpace(current); ++depth)
            {
                string candidate = Path.Combine(current, OverrideRelativePath);
                if (File.Exists(candidate))
                    return candidate;

                DirectoryInfo parent = Directory.GetParent(current);
                current = parent == null ? null : parent.FullName;
            }

            return string.Empty;
        }

        private sealed class ComponentFieldDescriptor
        {
            public string FieldKey { get; set; }
            public string DomainKey { get; set; }
            public string TokenBody { get; set; }
            public string TokenMeaning { get; set; }
        }

        private sealed class ComponentTokenEvidence
        {
            public ushort AbilityId { get; set; }
            public string AbilityName { get; set; }
            public ushort ComponentId { get; set; }
            public int ComponentSlotIndex { get; set; }
            public uint Operation { get; set; }
            public string DomainKey { get; set; }
            public string FieldKey { get; set; }
            public string TokenBody { get; set; }
            public string Meaning { get; set; }
            public string Source { get; set; }
            public string SourcePath { get; set; }
            public string SourceLocation { get; set; }
            public string TextExcerpt { get; set; }
            public List<string> ContextTags { get; set; }
        }

        private sealed class ComponentSchemaOverride
        {
            public string DomainKey { get; set; }
            public string Meaning { get; set; }
            public string Confidence { get; set; }
            public string SourceLabel { get; set; }
            public string SourcePath { get; set; }
            public string SourceLocation { get; set; }
            public string Notes { get; set; }
        }

        private sealed class AbilityTextSelection
        {
            public string TextExcerpt { get; set; }
            public string SourcePath { get; set; }
            public string SourceLocation { get; set; }
            public List<string> ContextTags { get; set; }
        }

        private sealed class AbilityComponentReference
        {
            public ushort AbilityId { get; set; }
            public string AbilityName { get; set; }
            public ushort ComponentId { get; set; }
            public int ComponentSlotIndex { get; set; }
            public uint TriggerValue { get; set; }
            public string TriggerText { get; set; }
            public string TextExcerpt { get; set; }
            public string SourcePath { get; set; }
            public string SourceLocation { get; set; }
            public List<string> ContextTags { get; set; }
        }

        private sealed class FieldObservation
        {
            public string FieldKey { get; set; }
            public string RawValue { get; set; }
            public ushort ComponentId { get; set; }
            public string LayoutVariant { get; set; }
        }

        private sealed class FieldObservationInference
        {
            public string SemanticSummary { get; set; }
            public string Confidence { get; set; }
            public string Notes { get; set; }
        }

        private sealed class FieldValueObservation
        {
            public string RawValue { get; set; }
            public ushort ComponentId { get; set; }
            public List<AbilityComponentReference> AbilityReferences { get; set; }
        }

        private sealed class FieldValueCount
        {
            public string RawValue { get; set; }
            public int Count { get; set; }
        }

        private sealed class FieldTriage
        {
            public int Score { get; set; }
            public string Bucket { get; set; }
            public string Notes { get; set; }
        }

        private sealed class EmbeddedComReference
        {
            public int ComponentSlotIndex { get; set; }
            public string TokenBody { get; set; }
            public string FullToken { get; set; }
        }
    }

    public sealed class ComponentFieldSemantic
    {
        public string DomainKey { get; set; }
        public string Meaning { get; set; }
        public string Confidence { get; set; }
        public string Source { get; set; }
        public string SourcePath { get; set; }
        public string SourceLocation { get; set; }
        public string Notes { get; set; }
    }
}
