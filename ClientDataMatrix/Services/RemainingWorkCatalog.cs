using ClientDataMatrix.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ClientDataMatrix.Services
{
    public sealed class RemainingWorkCatalog
    {
        public const int DefaultNextBatchTopCount = 50;
        public const string DefaultNextBatchMinimumPriorityBucket = "High";

        private readonly AbilityDataset _dataset;

        public RemainingWorkCatalog(AbilityDataset dataset)
        {
            _dataset = dataset;
        }

        public RemainingWorkDocument BuildRemainingWorkReport(
            string extractedRootPath,
            CoverageReportDocument coverage,
            ConflictReportDocument conflicts,
            DomainLedgerDocument domains,
            RequirementLedgerDocument requirements,
            TokenDictionaryDocument tokens,
            OperationSchemaDocument operations)
        {
            List<AreaBuildResult> areaResults = new List<AreaBuildResult>
            {
                BuildCoverageArea(coverage),
                BuildConflictArea(conflicts),
                BuildOperationArea(operations),
                BuildRequirementArea(requirements),
                BuildTokenArea(tokens),
                BuildDomainArea(domains)
            };

            List<RemainingWorkAreaRecord> areas = areaResults
                .Where(result => result != null && result.Area != null)
                .Select(result => result.Area)
                .ToList();

            Dictionary<string, int> areaOrder = areas
                .Select((area, index) => new { area.AreaKey, Index = index })
                .ToDictionary(row => row.AreaKey ?? string.Empty, row => row.Index, StringComparer.OrdinalIgnoreCase);

            List<RemainingWorkItemRecord> items = areaResults
                .Where(result => result != null)
                .SelectMany(result => result.Items ?? new List<RemainingWorkItemRecord>())
                .ToList();

            foreach (IGrouping<string, RemainingWorkItemRecord> group in items.GroupBy(row => row.AreaKey ?? string.Empty, StringComparer.OrdinalIgnoreCase))
            {
                int rank = 1;
                foreach (RemainingWorkItemRecord item in group
                    .OrderByDescending(row => row.PriorityScore)
                    .ThenBy(row => GetPrioritySortKey(row.PriorityBucket))
                    .ThenBy(row => row.Title, StringComparer.OrdinalIgnoreCase))
                {
                    item.Rank = rank++;
                }
            }

            int globalRank = 1;
            foreach (RemainingWorkItemRecord item in items
                .OrderByDescending(row => row.PriorityScore)
                .ThenBy(row => GetPrioritySortKey(row.PriorityBucket))
                .ThenBy(row => row.Title, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.AreaTitle, StringComparer.OrdinalIgnoreCase))
            {
                item.GlobalRank = globalRank++;
            }

            items = items
                .OrderBy(row => areaOrder.ContainsKey(row.AreaKey ?? string.Empty) ? areaOrder[row.AreaKey ?? string.Empty] : int.MaxValue)
                .ThenBy(row => row.Rank)
                .ThenByDescending(row => row.PriorityScore)
                .ThenBy(row => row.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new RemainingWorkDocument
            {
                GeneratedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                ExtractedRootPath = extractedRootPath,
                TableStatuses = _dataset == null
                    ? new List<TableLoadStatus>()
                    : _dataset.TableStatuses.OrderBy(row => row.SourceFamily).ThenBy(row => row.TableName).ToList(),
                Summary = BuildSummary(items, coverage, conflicts, requirements, tokens, domains, operations),
                Areas = areas,
                Items = items
            };
        }

        private AreaBuildResult BuildCoverageArea(CoverageReportDocument report)
        {
            List<CoverageAbilityRecord> gapRows = report == null || report.Abilities == null
                ? new List<CoverageAbilityRecord>()
                : report.Abilities
                    .Where(row => !string.Equals(row.CoverageStatus, "MappedWithRequirements", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(row.CoverageStatus, "Mapped", StringComparison.OrdinalIgnoreCase))
                    .ToList();

            List<RemainingWorkItemRecord> items = new List<RemainingWorkItemRecord>();

            foreach (IGrouping<string, CoverageAbilityRecord> group in gapRows
                .GroupBy(row => row.CoverageStatus ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(row => row.Count())
                .ThenBy(row => row.Key, StringComparer.OrdinalIgnoreCase))
            {
                List<CoverageAbilityRecord> rows = group.OrderBy(row => row.AbilityId).ToList();
                string status = string.IsNullOrWhiteSpace(group.Key) ? "(unspecified)" : group.Key;
                int score = DetermineCoverageScore(status);
                items.Add(CreateItem(
                    "Coverage",
                    "Coverage Gaps",
                    score,
                    DeterminePriorityBucket(score),
                    "CoverageStatus",
                    status,
                    status + " ability bucket",
                    rows.Count.ToString(CultureInfo.InvariantCulture) + " abilities still sit in `" + status + "` coverage instead of a fully mapped state.",
                    "Samples: " + JoinAbilityIds(rows.Select(row => row.AbilityId).Take(12)) + ". Common missing pieces: " + JoinValues(rows.Select(row => row.MissingText).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).Take(3)) + ".",
                    BuildCoverageAction(status),
                    rows.Select(row => row.AbilityId).FirstOrDefault()));
            }

            foreach (IGrouping<string, CoverageAbilityRecord> group in gapRows
                .GroupBy(row => NormalizeMissingText(row.MissingText), StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(row => row.Count())
                .ThenBy(row => row.Key, StringComparer.OrdinalIgnoreCase)
                .Take(8))
            {
                List<CoverageAbilityRecord> rows = group.OrderBy(row => row.AbilityId).ToList();
                int score = rows.Max(row => DetermineCoverageScore(row.CoverageStatus)) + Math.Min(rows.Count, 30);
                items.Add(CreateItem(
                    "Coverage",
                    "Coverage Gaps",
                    score,
                    DeterminePriorityBucket(score),
                    "MissingPattern",
                    group.Key,
                    "Common missing pattern: " + group.Key,
                    rows.Count.ToString(CultureInfo.InvariantCulture) + " abilities still share this extracted-client gap pattern.",
                    "Statuses: " + JoinValues(rows.Select(row => row.CoverageStatus).Distinct(StringComparer.OrdinalIgnoreCase)) + ". Samples: " + JoinAbilityIds(rows.Select(row => row.AbilityId).Take(12)) + ".",
                    BuildCoverageMissingAction(group.Key),
                    rows.Select(row => row.AbilityId).FirstOrDefault()));
            }

            string summary = gapRows.Count <= 0
                ? "No abilities are currently below `Mapped` coverage."
                : gapRows.Count.ToString(CultureInfo.InvariantCulture) + " abilities remain below `Mapped`; the largest buckets are " + JoinValues(items.Where(row => string.Equals(row.SubjectKind, "CoverageStatus", StringComparison.OrdinalIgnoreCase)).Take(3).Select(row => row.SubjectKey)) + ".";

            return CreateArea(
                "Coverage",
                "Coverage Gaps",
                items,
                summary,
                "Push the largest shared missing-pattern buckets first so ability reports move from sparse or string-only states into repeatable mapped states.");
        }

        private AreaBuildResult BuildConflictArea(ConflictReportDocument report)
        {
            List<ConflictRecord> rows = report == null || report.Conflicts == null
                ? new List<ConflictRecord>()
                : report.Conflicts.Where(row => !row.IsNoise && string.IsNullOrWhiteSpace(row.CanonicalValue)).ToList();

            List<RemainingWorkItemRecord> items = rows
                .GroupBy(BuildConflictGroupKey, StringComparer.OrdinalIgnoreCase)
                .Select(group =>
                {
                    List<ConflictRecord> conflicts = group.ToList();
                    ConflictRecord representative = conflicts
                        .OrderByDescending(row => row.TriageScore)
                        .ThenBy(row => row.SubjectKey, StringComparer.OrdinalIgnoreCase)
                        .FirstOrDefault();
                    int score = conflicts.Max(row => row.TriageScore) + Math.Min(conflicts.Count * 5, 25);
                    return CreateItem(
                        "Conflicts",
                        "Conflict Hotspots",
                        score,
                        DeterminePriorityBucket(score),
                        "ConflictGroup",
                        group.Key,
                        BuildConflictGroupTitle(representative),
                        conflicts.Count.ToString(CultureInfo.InvariantCulture) + " non-noise conflicts across " + conflicts.Select(row => row.SubjectKey).Distinct(StringComparer.OrdinalIgnoreCase).Count().ToString(CultureInfo.InvariantCulture) + " subjects.",
                        "Peak triage: " + conflicts.Max(row => row.TriageScore).ToString(CultureInfo.InvariantCulture) + ". High-signal rows: " + conflicts.Count(IsHighSignal).ToString(CultureInfo.InvariantCulture) + ". Sample subjects: " + JoinValues(conflicts.Select(row => row.SubjectKey).Take(6)) + ".",
                        BuildConflictAction(representative),
                        conflicts.Select(GetConflictAbilityId).FirstOrDefault(row => row.HasValue).GetValueOrDefault());
                })
                .OrderByDescending(row => row.PriorityScore)
                .ThenBy(row => row.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();

            int highSignalCount = rows.Count(IsHighSignal);
            string summary = rows.Count <= 0
                ? "No non-noise conflict groups are currently outstanding."
                : highSignalCount.ToString(CultureInfo.InvariantCulture) + " high-signal conflicts remain after noise suppression; the biggest groups are " + JoinValues(items.Take(3).Select(row => row.Title)) + ".";

            return CreateArea(
                "Conflicts",
                "Conflict Hotspots",
                items,
                summary,
                "Close the highest-signal conflict groups by codifying source precedence instead of treating every disagreement as equally actionable.");
        }

        private AreaBuildResult BuildOperationArea(OperationSchemaDocument report)
        {
            List<RemainingWorkItemRecord> items = (report == null || report.Operations == null ? Enumerable.Empty<ComponentOperationSchemaRecord>() : report.Operations)
                .SelectMany(operation => (operation.Fields ?? new List<ComponentOperationFieldRecord>())
                    .Where(field => string.Equals(field.Confidence, SemanticConfidence.Unknown, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(field.Confidence, SemanticConfidence.Structural, StringComparison.OrdinalIgnoreCase))
                    .Select(field =>
                    {
                        ComponentOperationAbilityRecord sampleAbility = (operation.SampleAbilities ?? new List<ComponentOperationAbilityRecord>()).FirstOrDefault();
                        return CreateItem(
                            "Operations",
                            "Unknown Field Hotspots",
                            field.TriageScore,
                            string.IsNullOrWhiteSpace(field.TriageBucket) ? DeterminePriorityBucket(field.TriageScore) : field.TriageBucket,
                            "OperationField",
                            operation.OperationId.ToString(CultureInfo.InvariantCulture) + ":" + field.FieldKey,
                            operation.OperationName + " :: " + field.FieldKey,
                            string.IsNullOrWhiteSpace(field.SemanticSummary)
                                ? "No extracted-client semantic mapping is known yet for this field."
                                : field.SemanticSummary,
                            "Confidence: " + field.Confidence + ". Non-zero rows: " + field.NonZeroCount.ToString(CultureInfo.InvariantCulture) + ". Distinct values: " + field.DistinctValueCount.ToString(CultureInfo.InvariantCulture) + ". Tags: " + NullToPlaceholder(field.ContextTagsText) + ". Samples: " + NullToPlaceholder(field.SampleValuesText) + ".",
                            BuildOperationAction(operation, field),
                            sampleAbility == null ? (ushort)0 : sampleAbility.AbilityId,
                            sampleAbility == null ? string.Empty : sampleAbility.SourcePath,
                            sampleAbility == null ? string.Empty : sampleAbility.SourceLocation);
                    }))
                .OrderByDescending(row => row.PriorityScore)
                .ThenBy(row => row.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();

            string summary = items.Count <= 0
                ? "No unknown or structural component fields are currently outstanding."
                : items.Count.ToString(CultureInfo.InvariantCulture) + " unknown or structural operation fields remain; the highest-signal hotspots are " + JoinValues(items.Take(3).Select(row => row.Title)) + ".";

            return CreateArea(
                "Operations",
                "Unknown Field Hotspots",
                items,
                summary,
                "Use the unknown-triage evidence to turn structural layout roles into stable named semantics before widening emulator-side enums.");
        }

        private AreaBuildResult BuildRequirementArea(RequirementLedgerDocument report)
        {
            List<RemainingWorkItemRecord> items = (report == null || report.Requirements == null ? Enumerable.Empty<RequirementLedgerRecord>() : report.Requirements)
                .Select(requirement =>
                {
                    List<RequirementFieldRecord> unresolvedFields = (requirement.Fields ?? new List<RequirementFieldRecord>())
                        .Where(field => string.Equals(field.Confidence, SemanticConfidence.Unknown, StringComparison.OrdinalIgnoreCase)
                            || string.Equals(field.Confidence, SemanticConfidence.Structural, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    if (unresolvedFields.Count <= 0)
                        return null;

                    int score = 80
                        + Math.Min(requirement.DirectAbilityCount, 40)
                        + Math.Min(requirement.DirectComponentCount, 30)
                        + Math.Min(requirement.ParentRequirementCount * 4, 20)
                        + Math.Min(requirement.ChildRequirementCount * 6, 24)
                        + Math.Min(unresolvedFields.Count * 3, 30);
                    return CreateItem(
                        "Requirements",
                        "Requirement Semantics",
                        score,
                        DeterminePriorityBucket(score),
                        "Requirement",
                        requirement.RequirementId.ToString(CultureInfo.InvariantCulture),
                        "Requirement " + requirement.RequirementId.ToString(CultureInfo.InvariantCulture),
                        unresolvedFields.Count.ToString(CultureInfo.InvariantCulture) + " requirement fields still lack named semantics across " + requirement.RecordCount.ToString(CultureInfo.InvariantCulture) + " exported row(s).",
                        "Direct abilities: " + requirement.DirectAbilityCount.ToString(CultureInfo.InvariantCulture) + ". Components: " + requirement.DirectComponentCount.ToString(CultureInfo.InvariantCulture) + ". Parents: " + requirement.ParentRequirementCount.ToString(CultureInfo.InvariantCulture) + ". Children: " + requirement.ChildRequirementCount.ToString(CultureInfo.InvariantCulture) + ". Fields: " + JoinValues(unresolvedFields.Take(6).Select(field => field.FieldKey)) + ". Sample abilities: " + NullToPlaceholder(requirement.SampleAbilitiesText) + ".",
                        "Decode the unresolved requirement ext-data fields before widening the current narrow `ExtData[*].Val6 -> RequirementId` linkage rule.",
                        ParseFirstAbilityId(requirement.SampleAbilitiesText));
                })
                .Where(row => row != null)
                .OrderByDescending(row => row.PriorityScore)
                .ThenBy(row => row.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();

            string summary = items.Count <= 0
                ? "No requirement rows currently have unresolved field semantics."
                : items.Count.ToString(CultureInfo.InvariantCulture) + " requirement rows still contain unresolved fields; prioritize rows with direct ability usage first.";

            return CreateArea(
                "Requirements",
                "Requirement Semantics",
                items,
                summary,
                "Focus on requirement rows with direct ability usage and child links so later linkage work stays evidence-based.");
        }

        private AreaBuildResult BuildTokenArea(TokenDictionaryDocument report)
        {
            List<RemainingWorkItemRecord> items = (report == null || report.Definitions == null ? Enumerable.Empty<TokenDefinitionRecord>() : report.Definitions)
                .Where(row => string.Equals(row.Confidence, SemanticConfidence.Unknown, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(row.Confidence, SemanticConfidence.Londo, StringComparison.OrdinalIgnoreCase))
                .Select(row =>
                {
                    int score = string.Equals(row.Confidence, SemanticConfidence.Londo, StringComparison.OrdinalIgnoreCase) ? 120 : 90;
                    score += Math.Min(row.ExampleAbilityIds == null ? 0 : row.ExampleAbilityIds.Count, 10);
                    return CreateItem(
                        "Tokens",
                        "Token Gaps",
                        score,
                        DeterminePriorityBucket(score),
                        "Token",
                        row.TokenBody,
                        row.TokenBody,
                        string.IsNullOrWhiteSpace(row.PlainEnglishMeaning)
                            ? "No extracted-client token meaning is recorded yet."
                            : row.PlainEnglishMeaning,
                        "Confidence: " + row.Confidence + ". Field: " + NullToPlaceholder(row.FieldKey) + ". Tags: " + JoinValues(row.ContextTags ?? new List<string>()) + ". Example abilities: " + JoinAbilityIds(row.ExampleAbilityIds ?? new List<ushort>()) + ". Source: " + NullToPlaceholder(row.Source) + ".",
                        string.Equals(row.Confidence, SemanticConfidence.Londo, StringComparison.OrdinalIgnoreCase)
                            ? "Replace this last-resort token meaning with extracted-client evidence or explicitly confirm that the override is still necessary."
                            : "Find client-string evidence that can move this token out of the unknown bucket, especially for nested COM-token expressions.",
                        row.ExampleAbilityIds == null ? (ushort)0 : row.ExampleAbilityIds.FirstOrDefault(),
                        row.SourcePath,
                        string.Empty);
                })
                .OrderByDescending(row => row.PriorityScore)
                .ThenBy(row => row.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();

            string summary = items.Count <= 0
                ? "No unknown or Londo token definitions are currently outstanding."
                : items.Count.ToString(CultureInfo.InvariantCulture) + " token definitions still need stronger semantics or extracted-client replacement evidence.";

            return CreateArea(
                "Tokens",
                "Token Gaps",
                items,
                summary,
                "Prioritize tokens that still depend on Londo or that block natural-language rendering of high-signal component fields.");
        }

        private AreaBuildResult BuildDomainArea(DomainLedgerDocument report)
        {
            List<RemainingWorkItemRecord> items = (report == null || report.Domains == null ? Enumerable.Empty<IdentityDomainRecord>() : report.Domains)
                .Where(row => !string.Equals(row.Confidence, SemanticConfidence.Confirmed, StringComparison.OrdinalIgnoreCase)
                    || row.DuplicateMeaningCount > 0
                    || !string.IsNullOrWhiteSpace(row.Canonicality) && row.Canonicality.IndexOf("not canonical", StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(row =>
                {
                    int score = 60 + Math.Min(row.DuplicateMeaningCount * 10, 80);
                    if (!string.Equals(row.Confidence, SemanticConfidence.Confirmed, StringComparison.OrdinalIgnoreCase))
                        score += 25;
                    return CreateItem(
                        "Domains",
                        "Identity Domain Risks",
                        score,
                        DeterminePriorityBucket(score),
                        "Domain",
                        row.DomainKey,
                        row.DisplayName,
                        "Confidence `" + row.Confidence + "` with " + row.DuplicateMeaningCount.ToString(CultureInfo.InvariantCulture) + " duplicate-meaning group(s).",
                        "Canonicality: " + NullToPlaceholder(row.Canonicality) + ". Values: " + row.ValueCount.ToString(CultureInfo.InvariantCulture) + ". Recommended usage: " + NullToPlaceholder(row.RecommendedUsage) + ". Notes: " + NullToPlaceholder(row.Notes) + ".",
                        "Keep this numeric domain isolated until extracted-client evidence proves a stronger canonical identity mapping.",
                        0);
                })
                .OrderByDescending(row => row.PriorityScore)
                .ThenBy(row => row.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();

            string summary = items.Count <= 0
                ? "No identity domains currently have duplicate-meaning or non-canonical warnings."
                : items.Count.ToString(CultureInfo.InvariantCulture) + " identity domains still carry non-canonical or duplicate-meaning risk.";

            return CreateArea(
                "Domains",
                "Identity Domain Risks",
                items,
                summary,
                "Finish the race or career identity collision pass before renaming any repeated client string-entry domains into runtime IDs.");
        }

        private RemainingWorkSummaryRecord BuildSummary(
            IList<RemainingWorkItemRecord> items,
            CoverageReportDocument coverage,
            ConflictReportDocument conflicts,
            RequirementLedgerDocument requirements,
            TokenDictionaryDocument tokens,
            DomainLedgerDocument domains,
            OperationSchemaDocument operations)
        {
            List<CoverageAbilityRecord> coverageRows = coverage == null || coverage.Abilities == null ? new List<CoverageAbilityRecord>() : coverage.Abilities;
            List<ConflictRecord> conflictRows = conflicts == null || conflicts.Conflicts == null ? new List<ConflictRecord>() : conflicts.Conflicts;
            List<RequirementLedgerRecord> requirementRows = requirements == null || requirements.Requirements == null ? new List<RequirementLedgerRecord>() : requirements.Requirements;
            List<TokenDefinitionRecord> tokenRows = tokens == null || tokens.Definitions == null ? new List<TokenDefinitionRecord>() : tokens.Definitions;
            List<IdentityDomainRecord> domainRows = domains == null || domains.Domains == null ? new List<IdentityDomainRecord>() : domains.Domains;
            List<ComponentOperationFieldRecord> operationFields = operations == null || operations.Operations == null
                ? new List<ComponentOperationFieldRecord>()
                : operations.Operations.SelectMany(row => row.Fields ?? new List<ComponentOperationFieldRecord>()).ToList();

            return new RemainingWorkSummaryRecord
            {
                AreaCount = 6,
                ItemCount = items.Count,
                CriticalCount = items.Count(row => string.Equals(row.PriorityBucket, "Critical", StringComparison.OrdinalIgnoreCase)),
                HighCount = items.Count(row => string.Equals(row.PriorityBucket, "High", StringComparison.OrdinalIgnoreCase)),
                CoverageGapAbilityCount = coverageRows.Count(row => !string.Equals(row.CoverageStatus, "MappedWithRequirements", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(row.CoverageStatus, "Mapped", StringComparison.OrdinalIgnoreCase)),
                HighSignalConflictCount = conflictRows.Count(row => IsHighSignal(row) && string.IsNullOrWhiteSpace(row.CanonicalValue)),
                UnknownFieldCount = operationFields.Count(row => string.Equals(row.Confidence, SemanticConfidence.Unknown, StringComparison.OrdinalIgnoreCase)),
                StructuralFieldCount = operationFields.Count(row => string.Equals(row.Confidence, SemanticConfidence.Structural, StringComparison.OrdinalIgnoreCase)),
                RequirementGapCount = requirementRows.Count(row => (row.Fields ?? new List<RequirementFieldRecord>()).Any(field => string.Equals(field.Confidence, SemanticConfidence.Unknown, StringComparison.OrdinalIgnoreCase) || string.Equals(field.Confidence, SemanticConfidence.Structural, StringComparison.OrdinalIgnoreCase))),
                TokenGapCount = tokenRows.Count(row => string.Equals(row.Confidence, SemanticConfidence.Unknown, StringComparison.OrdinalIgnoreCase) || string.Equals(row.Confidence, SemanticConfidence.Londo, StringComparison.OrdinalIgnoreCase)),
                DomainIssueCount = domainRows.Count(row => !string.Equals(row.Confidence, SemanticConfidence.Confirmed, StringComparison.OrdinalIgnoreCase)
                    || row.DuplicateMeaningCount > 0
                    || !string.IsNullOrWhiteSpace(row.Canonicality) && row.Canonicality.IndexOf("not canonical", StringComparison.OrdinalIgnoreCase) >= 0)
            };
        }

        public static List<RemainingWorkItemRecord> BuildNextBatch(RemainingWorkDocument report)
        {
            return FilterItems(report, null, DefaultNextBatchMinimumPriorityBucket, null, DefaultNextBatchTopCount);
        }

        public static List<RemainingWorkItemRecord> FilterItems(RemainingWorkDocument report, string areaKey, string minimumPriorityBucket, string searchText, int topCount)
        {
            IEnumerable<RemainingWorkItemRecord> filtered = report == null || report.Items == null
                ? Enumerable.Empty<RemainingWorkItemRecord>()
                : report.Items;

            bool hasAreaFilter = !string.IsNullOrWhiteSpace(areaKey);
            if (hasAreaFilter)
                filtered = filtered.Where(row => string.Equals(row.AreaKey, areaKey, StringComparison.OrdinalIgnoreCase));

            int priorityThreshold = GetPriorityThreshold(minimumPriorityBucket);
            if (priorityThreshold >= 0)
                filtered = filtered.Where(row => GetPrioritySortKey(row.PriorityBucket) <= priorityThreshold);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string search = searchText.Trim();
                filtered = filtered.Where(row => MatchesSearch(row, search));
            }

            List<RemainingWorkItemRecord> ordered = (hasAreaFilter
                    ? filtered
                        .OrderBy(row => row.Rank > 0 ? row.Rank : int.MaxValue)
                        .ThenBy(row => row.GlobalRank > 0 ? row.GlobalRank : int.MaxValue)
                    : filtered
                        .OrderBy(row => row.GlobalRank > 0 ? row.GlobalRank : int.MaxValue)
                        .ThenBy(row => row.AreaTitle, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(row => row.Rank > 0 ? row.Rank : int.MaxValue))
                .ThenByDescending(row => row.PriorityScore)
                .ThenBy(row => row.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return topCount > 0 ? ordered.Take(topCount).ToList() : ordered;
        }

        public static string BuildFilterDescription(string areaKey, string minimumPriorityBucket, string searchText, int topCount)
        {
            List<string> parts = new List<string>();
            parts.Add(string.IsNullOrWhiteSpace(areaKey) ? "all areas" : "area `" + areaKey + "`");

            if (!string.IsNullOrWhiteSpace(minimumPriorityBucket)
                && !string.Equals(minimumPriorityBucket, "All", StringComparison.OrdinalIgnoreCase))
                parts.Add("priority >= `" + minimumPriorityBucket + "`");

            if (!string.IsNullOrWhiteSpace(searchText))
                parts.Add("search `" + searchText.Trim() + "`");

            parts.Add(topCount > 0
                ? "top " + topCount.ToString(CultureInfo.InvariantCulture)
                : "all matching items");

            return string.Join(", ", parts);
        }

        private static AreaBuildResult CreateArea(string areaKey, string title, List<RemainingWorkItemRecord> items, string summary, string nextStep)
        {
            List<RemainingWorkItemRecord> safeItems = items ?? new List<RemainingWorkItemRecord>();
            foreach (RemainingWorkItemRecord item in safeItems)
            {
                item.AreaKey = areaKey;
                item.AreaTitle = title;
            }

            return new AreaBuildResult
            {
                Area = new RemainingWorkAreaRecord
                {
                    AreaKey = areaKey,
                    Title = title,
                    ItemCount = safeItems.Count,
                    CriticalCount = safeItems.Count(row => string.Equals(row.PriorityBucket, "Critical", StringComparison.OrdinalIgnoreCase)),
                    HighCount = safeItems.Count(row => string.Equals(row.PriorityBucket, "High", StringComparison.OrdinalIgnoreCase)),
                    PeakScore = safeItems.Count <= 0 ? 0 : safeItems.Max(row => row.PriorityScore),
                    PeakBucket = safeItems.Count <= 0 ? "Low" : safeItems.OrderByDescending(row => row.PriorityScore).Select(row => row.PriorityBucket).FirstOrDefault() ?? "Low",
                    Summary = summary,
                    RecommendedNextStep = nextStep
                },
                Items = safeItems
            };
        }

        private static RemainingWorkItemRecord CreateItem(
            string areaKey,
            string areaTitle,
            int score,
            string bucket,
            string subjectKind,
            string subjectKey,
            string title,
            string summary,
            string evidence,
            string recommendedAction,
            ushort exampleAbilityId,
            string referencePath = null,
            string referenceLocation = null)
        {
            return new RemainingWorkItemRecord
            {
                AreaKey = areaKey,
                AreaTitle = areaTitle,
                PriorityScore = score,
                PriorityBucket = string.IsNullOrWhiteSpace(bucket) ? DeterminePriorityBucket(score) : bucket,
                SubjectKind = subjectKind,
                SubjectKey = subjectKey,
                Title = title,
                Summary = summary,
                Evidence = evidence,
                RecommendedAction = recommendedAction,
                ExampleAbilityId = exampleAbilityId > 0 ? (ushort?)exampleAbilityId : null,
                ReferencePath = referencePath ?? string.Empty,
                ReferenceLocation = referenceLocation ?? string.Empty
            };
        }

        private static string BuildConflictGroupKey(ConflictRecord conflict)
        {
            if (conflict == null)
                return "(none)";

            string category = string.IsNullOrWhiteSpace(conflict.TriageCategory) ? "(uncategorized)" : conflict.TriageCategory;
            string domain = string.IsNullOrWhiteSpace(conflict.Domain) ? "(unknown-domain)" : conflict.Domain;
            return category + " | " + domain;
        }

        private static string BuildConflictGroupTitle(ConflictRecord conflict)
        {
            if (conflict == null)
                return "Conflict group";

            if (!string.IsNullOrWhiteSpace(conflict.TriageCategory) && !string.Equals(conflict.TriageCategory, conflict.Domain, StringComparison.OrdinalIgnoreCase))
                return conflict.TriageCategory + " (" + NullToPlaceholder(conflict.Domain) + ")";

            return NullToPlaceholder(conflict.Domain);
        }

        private static string BuildConflictAction(ConflictRecord conflict)
        {
            if (conflict == null)
                return "Inspect the claim-level evidence and encode an explicit canonical rule for this disagreement.";

            if (string.Equals(conflict.Domain, "EffectId", StringComparison.OrdinalIgnoreCase))
                return "Verify the preferred effect source for this pattern and codify the canonical `EffectId` choice so the conflict stays bounded.";
            if (string.Equals(conflict.Domain, "AbilityName", StringComparison.OrdinalIgnoreCase))
                return "Separate player-facing names from internal placeholders and keep the canonical string rule explicit.";
            return "Inspect the claim-level evidence and encode an explicit canonical rule for this disagreement.";
        }

        private static string BuildCoverageAction(string status)
        {
            if (string.Equals(status, "Sparse", StringComparison.OrdinalIgnoreCase))
                return "Backfill the core client contract first: CSV row, BIN row, root effect row, and component linkage.";
            if (string.Equals(status, "StringsOnly", StringComparison.OrdinalIgnoreCase))
                return "Find the missing BIN, effect, and component evidence so this stops being a text-only ability shell.";
            if (string.Equals(status, "PlayableSurface", StringComparison.OrdinalIgnoreCase))
                return "Close the remaining requirement and component gaps so the ability is fully mapped instead of merely usable on the surface.";
            return "Close the missing extracted-client pieces called out in the pattern list before spending time on deeper semantic decoding.";
        }

        private static string BuildCoverageMissingAction(string missingText)
        {
            if (missingText.IndexOf("effect-row", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Resolve the missing root effect link so ability flow can be traced through the client effect chain.";
            if (missingText.IndexOf("components", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Recover the missing component linkage before trying to interpret operation semantics.";
            if (missingText.IndexOf("effect-text", StringComparison.OrdinalIgnoreCase) >= 0
                || missingText.IndexOf("description", StringComparison.OrdinalIgnoreCase) >= 0
                || missingText.IndexOf("name", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Backfill the missing localized strings so reports stop depending on internal-only labels.";
            if (missingText.IndexOf("bin", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Recover the missing BIN record so the ability has a stable client-contract row.";
            return "Close this missing-evidence pattern in bulk so a whole bucket of abilities moves forward together.";
        }

        private static string BuildOperationAction(ComponentOperationSchemaRecord operation, ComponentOperationFieldRecord field)
        {
            if (field == null)
                return "Use the value-evidence view to turn this hotspot into a named semantic field.";

            if (string.Equals(field.Confidence, SemanticConfidence.Structural, StringComparison.OrdinalIgnoreCase))
                return "Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.";
            if (operation != null && operation.IsPriority)
                return "This is a priority operation hotspot. Use value evidence, trigger mix, and sample abilities to move it out of the unknown bucket.";
            return "Use the value-evidence view to turn this hotspot into a named semantic field.";
        }

        private static bool IsHighSignal(ConflictRecord conflict)
        {
            return conflict != null
                && (string.Equals(conflict.TriageBucket, "Critical", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(conflict.TriageBucket, "High", StringComparison.OrdinalIgnoreCase));
        }

        private static ushort? GetConflictAbilityId(ConflictRecord conflict)
        {
            if (conflict == null || string.IsNullOrWhiteSpace(conflict.SubjectKey))
                return null;

            const string prefix = "Ability:";
            if (!conflict.SubjectKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return null;

            ushort abilityId;
            return ushort.TryParse(conflict.SubjectKey.Substring(prefix.Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out abilityId)
                ? (ushort?)abilityId
                : null;
        }

        private static int DetermineCoverageScore(string status)
        {
            if (string.Equals(status, "Sparse", StringComparison.OrdinalIgnoreCase))
                return 180;
            if (string.Equals(status, "StringsOnly", StringComparison.OrdinalIgnoreCase))
                return 160;
            if (string.Equals(status, "Partial", StringComparison.OrdinalIgnoreCase))
                return 145;
            if (string.Equals(status, "PlayableSurface", StringComparison.OrdinalIgnoreCase))
                return 110;
            return 80;
        }

        private static string DeterminePriorityBucket(int score)
        {
            if (score >= 170)
                return "Critical";
            if (score >= 130)
                return "High";
            if (score >= 90)
                return "Medium";
            return "Low";
        }

        private static int GetPrioritySortKey(string bucket)
        {
            if (string.Equals(bucket, "Critical", StringComparison.OrdinalIgnoreCase))
                return 0;
            if (string.Equals(bucket, "High", StringComparison.OrdinalIgnoreCase))
                return 1;
            if (string.Equals(bucket, "Medium", StringComparison.OrdinalIgnoreCase))
                return 2;
            return 3;
        }

        private static int GetPriorityThreshold(string minimumPriorityBucket)
        {
            if (string.IsNullOrWhiteSpace(minimumPriorityBucket)
                || string.Equals(minimumPriorityBucket, "All", StringComparison.OrdinalIgnoreCase))
                return -1;

            return GetPrioritySortKey(minimumPriorityBucket);
        }

        private static bool MatchesSearch(RemainingWorkItemRecord row, string search)
        {
            return ContainsIgnoreCase(row == null ? null : row.AreaKey, search)
                || ContainsIgnoreCase(row == null ? null : row.AreaTitle, search)
                || ContainsIgnoreCase(row == null ? null : row.SubjectKind, search)
                || ContainsIgnoreCase(row == null ? null : row.SubjectKey, search)
                || ContainsIgnoreCase(row == null ? null : row.Title, search)
                || ContainsIgnoreCase(row == null ? null : row.Summary, search)
                || ContainsIgnoreCase(row == null ? null : row.Evidence, search)
                || ContainsIgnoreCase(row == null ? null : row.RecommendedAction, search)
                || ContainsIgnoreCase(row == null ? null : row.ReferencePath, search)
                || ContainsIgnoreCase(row == null ? null : row.ReferenceLocation, search)
                || row != null && row.ExampleAbilityId.HasValue && row.ExampleAbilityId.Value.ToString(CultureInfo.InvariantCulture).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool ContainsIgnoreCase(string value, string search)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.IndexOf(search ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string NormalizeMissingText(string missingText)
        {
            return string.IsNullOrWhiteSpace(missingText) ? "(unspecified)" : missingText.Trim();
        }

        private static string JoinValues(IEnumerable<string> values)
        {
            List<string> items = values == null
                ? new List<string>()
                : values.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            return items.Count <= 0 ? "(none)" : string.Join(", ", items);
        }

        private static string JoinAbilityIds(IEnumerable<ushort> abilityIds)
        {
            List<string> values = (abilityIds ?? Enumerable.Empty<ushort>())
                .Where(value => value > 0)
                .Distinct()
                .Select(value => value.ToString(CultureInfo.InvariantCulture))
                .ToList();
            return values.Count <= 0 ? "(none)" : string.Join(", ", values);
        }

        private static string NullToPlaceholder(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "(none)" : value;
        }

        private static ushort ParseFirstAbilityId(string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText))
                return 0;

            foreach (string token in rawText.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                ushort abilityId;
                if (ushort.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out abilityId))
                    return abilityId;
            }

            return 0;
        }

        private sealed class AreaBuildResult
        {
            public RemainingWorkAreaRecord Area { get; set; }
            public List<RemainingWorkItemRecord> Items { get; set; }
        }
    }
}
