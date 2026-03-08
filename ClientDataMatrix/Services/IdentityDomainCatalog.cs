using ClientDataMatrix.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ClientDataMatrix.Services
{
    public sealed class IdentityDomainCatalog
    {
        private readonly AbilityDataset _dataset;

        public IdentityDomainCatalog(AbilityDataset dataset)
        {
            _dataset = dataset;
        }

        public DomainLedgerDocument BuildDomainLedger(string extractedRootPath)
        {
            return new DomainLedgerDocument
            {
                GeneratedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                ExtractedRootPath = extractedRootPath,
                TableStatuses = _dataset == null
                    ? new List<TableLoadStatus>()
                    : _dataset.TableStatuses.OrderBy(row => row.SourceFamily).ThenBy(row => row.TableName).ToList(),
                Domains = BuildDomains()
            };
        }

        private List<IdentityDomainRecord> BuildDomains()
        {
            if (_dataset == null)
                return new List<IdentityDomainRecord>();

            List<IdentityDomainRecord> domains = new List<IdentityDomainRecord>
            {
                BuildRaceDomain(),
                BuildCareerLineDomain(),
                BuildCareerNameDomain()
            };

            return domains.OrderBy(row => row.DisplayName, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private IdentityDomainRecord BuildRaceDomain()
        {
            List<IndexedStringRecord> rows = _dataset.RaceNames
                .OrderBy(row => row.EntryId)
                .ThenBy(row => row.LineNumber)
                .ToList();

            return CreateDomain(
                "Race.EntryId",
                "Race Entry IDs",
                rows,
                SemanticConfidence.Confirmed,
                "Canonical for the extracted race display-name domain",
                "Use for extracted-client race display names only.",
                "These entry ids come directly from racenames_m.txt. They are stable for the race-name string table and are safe to call Race IDs in that specific client-string domain.");
        }

        private IdentityDomainRecord BuildCareerLineDomain()
        {
            Dictionary<uint, int> usageCounts = _dataset.BinaryAbilities
                .GroupBy(row => row.CareerLine)
                .ToDictionary(group => group.Key, group => group.Count());
            List<IndexedStringRecord> rows = _dataset.CareerLines
                .OrderBy(row => row.EntryId)
                .ThenBy(row => row.LineNumber)
                .ToList();

            IdentityDomainRecord record = CreateDomain(
                "CareerLine.EntryId",
                "Career Line Entry IDs",
                rows,
                SemanticConfidence.Confirmed,
                "Canonical for the extracted career-line domain",
                "Use for fields explicitly named CareerLine, including abilityexport.bin CareerLine.",
                "This is the numeric domain currently referenced by abilityexport.bin CareerLine. It should stay named CareerLine unless another extracted source proves a different identity domain.");

            foreach (IdentityDomainValueRecord value in record.Values)
            {
                uint rawValue;
                if (uint.TryParse(value.RawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out rawValue) && usageCounts.ContainsKey(rawValue))
                    value.Notes = AppendNote(value.Notes, "Referenced by " + usageCounts[rawValue].ToString(CultureInfo.InvariantCulture) + " ability BIN row(s).");
            }

            return record;
        }

        private IdentityDomainRecord BuildCareerNameDomain()
        {
            List<IndexedStringRecord> rows = _dataset.CareerNames
                .OrderBy(row => row.EntryId)
                .ThenBy(row => row.LineNumber)
                .ToList();

            IdentityDomainRecord record = CreateDomain(
                "CareerName.EntryId",
                "Career Name Entry IDs",
                rows,
                SemanticConfidence.Inferred,
                "Not canonical as a proven CareerId domain",
                "Use as a client string-entry table only until a real numeric CareerId mapping is proven from extracted files.",
                "careernames_m.txt contains many repeated display names across different numeric entry ids. That means these ids are not currently safe to rename to CareerId without stronger extracted-client evidence.");

            Dictionary<string, List<string>> duplicateMeanings = record.Values
                .Where(row => !string.IsNullOrWhiteSpace(row.Meaning))
                .GroupBy(row => row.Meaning, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Select(row => row.RawValue).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1)
                .ToDictionary(group => group.Key, group => group.Select(row => row.RawValue).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToList(), StringComparer.OrdinalIgnoreCase);

            foreach (IdentityDomainValueRecord value in record.Values)
            {
                List<string> duplicates;
                if (duplicateMeanings.TryGetValue(value.Meaning, out duplicates))
                    value.Notes = AppendNote(value.Notes, "This display name repeats across entry ids: " + string.Join(", ", duplicates) + ".");
            }

            record.Notes = AppendNote(record.Notes, "Duplicate display-name groups: " + record.DuplicateMeaningCount.ToString(CultureInfo.InvariantCulture) + ".");
            return record;
        }

        private static IdentityDomainRecord CreateDomain(string domainKey, string displayName, List<IndexedStringRecord> rows, string confidence, string canonicality, string recommendedUsage, string notes)
        {
            List<IdentityDomainValueRecord> values = rows.Select(row => new IdentityDomainValueRecord
            {
                RawValue = row.EntryId.ToString(CultureInfo.InvariantCulture),
                Meaning = string.IsNullOrWhiteSpace(row.NormalizedValue) ? "(empty)" : row.NormalizedValue,
                Confidence = confidence,
                Source = row.TableName,
                SourcePath = row.SourcePath,
                SourceLocation = "line " + row.LineNumber.ToString(CultureInfo.InvariantCulture),
                Notes = string.Empty
            }).ToList();

            int duplicateMeaningCount = values
                .Where(row => !string.IsNullOrWhiteSpace(row.Meaning) && !string.Equals(row.Meaning, "(empty)", StringComparison.OrdinalIgnoreCase))
                .GroupBy(row => row.Meaning, StringComparer.OrdinalIgnoreCase)
                .Count(group => group.Select(row => row.RawValue).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1);

            return new IdentityDomainRecord
            {
                DomainKey = domainKey,
                DisplayName = displayName,
                Confidence = confidence,
                Canonicality = canonicality,
                SourceFilesText = string.Join(", ", rows.Select(row => row.TableName).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value, StringComparer.OrdinalIgnoreCase)),
                ValueCount = values.Count,
                DistinctMeaningCount = values.Select(row => row.Meaning).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                DuplicateMeaningCount = duplicateMeaningCount,
                RecommendedUsage = recommendedUsage,
                Notes = notes,
                Values = values
            };
        }

        private static string AppendNote(string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left))
                return right ?? string.Empty;
            if (string.IsNullOrWhiteSpace(right))
                return left;
            return left + " " + right;
        }
    }
}
