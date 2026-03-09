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

            ComponentFieldSemantic structuralSemantic;
            if (TryResolveOperationSpecificStructuralSemantic(componentRow.Operation, fieldKey, rawValue, out structuralSemantic))
                return structuralSemantic;

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
                    semanticSummary = "APPLY_ABILITY ext-data payload-A field for the slot-local embedded payload.";
                    roleNotes = "This field mixes compact selector-like values with scalar-looking payloads, so it needs to be read together with the neighboring Val1, Val3, and Val7 fields.";
                    return true;
                case 3:
                    semanticSummary = "APPLY_ABILITY ext-data profile selector for the slot-local embedded payload.";
                    roleNotes = "This compact enum-like field is the clearest profile or behavior selector inside the APPLY_ABILITY ext-data block.";
                    return true;
                case 4:
                    semanticSummary = "APPLY_ABILITY ext-data family marker for the slot-local embedded payload.";
                    roleNotes = "This field behaves like a payload-layout tag rather than a free scalar; extracted rows overwhelmingly use one dominant value with a small minority variant.";
                    return true;
                case 7:
                    semanticSummary = "APPLY_ABILITY ext-data payload-B field for the slot-local embedded payload.";
                    roleNotes = "This field behaves like the second profile-specific payload slot and varies far more widely than the small selector fields around it.";
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
                    semanticSummary = "CC ext-data payload-A field for the slot-local control payload.";
                    roleNotes = "This field mixes compact selector-like values with scalar-looking payloads, so it needs to be read together with the neighboring Val1, Val3, and Val7 fields.";
                    return true;
                case 3:
                    semanticSummary = "CC ext-data profile selector for the slot-local control payload.";
                    roleNotes = "This compact enum-like field behaves like the main profile selector inside the recurring CC ext-data block.";
                    return true;
                case 4:
                    semanticSummary = "CC ext-data family marker for the slot-local control payload.";
                    roleNotes = "This field is almost fixed at 8 with a minority 9 variant, so it behaves like a layout-family tag rather than a free scalar.";
                    return true;
                case 7:
                    semanticSummary = "CC ext-data payload-B field for the slot-local control payload.";
                    roleNotes = "This field behaves like the second profile-specific payload slot and carries the widest scalar-looking values in the recurring CC ext-data block.";
                    return true;
                default:
                    return false;
            }
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
                case 7:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant Val7 payload branch and is strongly paired with Val1=2, Val2=2, Val3=1, and Val4=8.";
                    if (string.Equals(rawValue, "5", StringComparison.OrdinalIgnoreCase))
                        return "This common secondary Val7 branch is strongly paired with Val1=1, Val2=9, Val3=4, Val4=8, and Val6=100.";
                    if (string.Equals(rawValue, "250", StringComparison.OrdinalIgnoreCase))
                        return "This branch is strongly paired with Val2=3, Val3=4, and Val4=8.";
                    break;
            }

            return string.Empty;
        }

        private static string BuildCcStructuralValueNote(string fieldKey, int? valueIndex, string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return string.Empty;

            if (string.Equals(fieldKey, "FlagsRaw", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(rawValue, "2175", StringComparison.OrdinalIgnoreCase))
                    return "This dominant packed branch commonly appears with Value15=4 and the leading ExtData profile Val1=2, Val3=1, Val4=8, and Val7=1.";
                if (string.Equals(rawValue, "2303", StringComparison.OrdinalIgnoreCase))
                    return "This common packed branch stays in the same dominant CC layout family and still pairs with the usual Value15=4 marker.";
                if (string.Equals(rawValue, "8", StringComparison.OrdinalIgnoreCase))
                    return "This compact low-bit branch appears in shorter CC layouts that still reuse the same ext-data selector pattern.";
                if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                    return "This minimal branch behaves like a single-flag variant inside the broader CC bitfield family.";
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
                case 7:
                    if (string.Equals(rawValue, "1", StringComparison.OrdinalIgnoreCase))
                        return "This is the dominant compact CC Val7 branch in the recurring ext-data layout.";
                    if (string.Equals(rawValue, "20", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "50", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "100", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(rawValue, "200", StringComparison.OrdinalIgnoreCase))
                        return "This is a scalar-looking CC Val7 branch variant compared with the dominant compact 1-branch.";
                    break;
            }

            return string.Empty;
        }

        private static bool TryResolveOperationSpecificStructuralSemantic(uint operationId, string fieldKey, string rawValue, out ComponentFieldSemantic semantic)
        {
            semantic = null;

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
            if (TryBuildCcStructuralInference(operationId, fieldKey, observations, out structuralInference))
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
