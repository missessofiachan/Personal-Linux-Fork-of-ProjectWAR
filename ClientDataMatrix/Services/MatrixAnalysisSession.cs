using ClientDataMatrix.Configuration;
using ClientDataMatrix.Model;
using ClientDataMatrix.Output;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ClientDataMatrix.Services
{
    public sealed class MatrixAnalysisSession
    {
        private readonly AbilityGraphBuilder _builder;
        private readonly ComponentSchemaCatalog _componentSchemas;
        private readonly IdentityDomainCatalog _identityDomains;
        private readonly RequirementCatalog _requirements;

        private MatrixAnalysisSession(string extractedRootPath, AbilityDataset dataset)
        {
            ExtractedRootPath = extractedRootPath;
            Dataset = dataset;
            _builder = new AbilityGraphBuilder(dataset);
            _componentSchemas = new ComponentSchemaCatalog(dataset);
            _identityDomains = new IdentityDomainCatalog(dataset);
            _requirements = new RequirementCatalog(dataset);
        }

        public string ExtractedRootPath { get; private set; }
        public AbilityDataset Dataset { get; private set; }

        public static MatrixAnalysisSession Load(string extractedRootPath)
        {
            string resolvedRootPath = ExtractedDataRootResolver.Resolve(extractedRootPath);
            AbilityDataset dataset = AbilityDataset.Load(resolvedRootPath);
            return new MatrixAnalysisSession(resolvedRootPath, dataset);
        }

        public AbilityAnalysisResult BuildAbilityAnalysis(ushort abilityId)
        {
            return _builder.BuildAbilityAnalysis(abilityId, ExtractedRootPath);
        }

        public ConflictReportDocument BuildConflictReport()
        {
            return _builder.BuildConflictReport(ExtractedRootPath);
        }

        public CoverageReportDocument BuildCoverageReport()
        {
            return _builder.BuildCoverageReport(ExtractedRootPath);
        }

        public DomainLedgerDocument BuildDomainLedger()
        {
            return _identityDomains.BuildDomainLedger(ExtractedRootPath);
        }

        public RequirementLedgerDocument BuildRequirementLedger()
        {
            return _requirements.BuildRequirementLedger(ExtractedRootPath);
        }

        public TokenDictionaryDocument BuildTokenDictionary()
        {
            return _componentSchemas.BuildTokenDictionary(ExtractedRootPath);
        }

        public OperationSchemaDocument BuildOperationSchemas()
        {
            return _componentSchemas.BuildOperationSchemas(ExtractedRootPath);
        }

        public List<ComponentOperationFieldValueRecord> BuildOperationFieldValueEvidence(uint operationId, string fieldKey)
        {
            return _componentSchemas.BuildOperationFieldValueEvidence(operationId, fieldKey);
        }

        public ComponentOperationFieldValueInsightRecord BuildOperationFieldValueInsight(uint operationId, string fieldKey, string rawValue)
        {
            return _componentSchemas.BuildOperationFieldValueInsight(operationId, fieldKey, rawValue);
        }

        public string WriteAbilityReport(string outputRoot, AbilityAnalysisResult report)
        {
            string resolvedOutputRoot = ResolveOutputRoot(outputRoot);
            ReportWriters.WriteAbilityReport(resolvedOutputRoot, report);
            return Path.Combine(resolvedOutputRoot, "ability", report.AbilityId.ToString(CultureInfo.InvariantCulture) + ".md");
        }

        public string WriteConflictReport(string outputRoot, ConflictReportDocument report)
        {
            string resolvedOutputRoot = ResolveOutputRoot(outputRoot);
            ReportWriters.WriteConflictReport(resolvedOutputRoot, report);
            return Path.Combine(resolvedOutputRoot, "conflicts", "client-conflicts.md");
        }

        public string WriteCoverageReport(string outputRoot, CoverageReportDocument report)
        {
            string resolvedOutputRoot = ResolveOutputRoot(outputRoot);
            ReportWriters.WriteCoverageReport(resolvedOutputRoot, report);
            return Path.Combine(resolvedOutputRoot, "coverage", "ability-coverage.md");
        }

        public string WriteDomainLedgerReport(string outputRoot, DomainLedgerDocument report)
        {
            string resolvedOutputRoot = ResolveOutputRoot(outputRoot);
            ReportWriters.WriteDomainLedgerReport(resolvedOutputRoot, report);
            return Path.Combine(resolvedOutputRoot, "reference", "core-identity-domains.md");
        }

        public string WriteRequirementLedgerReport(string outputRoot, RequirementLedgerDocument report)
        {
            string resolvedOutputRoot = ResolveOutputRoot(outputRoot);
            ReportWriters.WriteRequirementLedgerReport(resolvedOutputRoot, report);
            return Path.Combine(resolvedOutputRoot, "reference", "requirement-ledger.md");
        }

        public string WriteTokenDictionaryReport(string outputRoot, TokenDictionaryDocument report)
        {
            string resolvedOutputRoot = ResolveOutputRoot(outputRoot);
            ReportWriters.WriteTokenDictionaryReport(resolvedOutputRoot, report);
            return Path.Combine(resolvedOutputRoot, "reference", "com-token-dictionary.md");
        }

        public string WriteOperationSchemaReport(string outputRoot, OperationSchemaDocument report)
        {
            string resolvedOutputRoot = ResolveOutputRoot(outputRoot);
            ReportWriters.WriteOperationSchemaReport(resolvedOutputRoot, report);
            return Path.Combine(resolvedOutputRoot, "reference", "component-operation-schemas.md");
        }

        public List<AbilityCatalogEntry> BuildAbilityCatalog()
        {
            Dictionary<ushort, List<ClientAbilityRecord>> clientRowsById = Dataset.ClientAbilities
                .Where(row => row.AbilityId <= ushort.MaxValue)
                .GroupBy(row => (ushort)row.AbilityId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.LineNumber).ToList());

            Dictionary<ushort, List<BinaryAbilityRecord>> binRowsById = Dataset.BinaryAbilities
                .GroupBy(row => row.AbilityId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.RecordIndex).ToList());

            Dictionary<ushort, List<IndexedStringRecord>> nameRowsById = Dataset.AbilityNames
                .Where(row => row.EntryId <= ushort.MaxValue)
                .GroupBy(row => (ushort)row.EntryId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.LineNumber).ToList());

            HashSet<ushort> abilityIds = new HashSet<ushort>(clientRowsById.Keys);
            foreach (ushort abilityId in binRowsById.Keys)
                abilityIds.Add(abilityId);
            foreach (ushort abilityId in nameRowsById.Keys)
                abilityIds.Add(abilityId);

            List<AbilityCatalogEntry> entries = new List<AbilityCatalogEntry>();
            foreach (ushort abilityId in abilityIds.OrderBy(value => value))
            {
                List<ClientAbilityRecord> clientRows = GetRows(clientRowsById, abilityId);
                List<BinaryAbilityRecord> binRows = GetRows(binRowsById, abilityId);
                List<IndexedStringRecord> nameRows = GetRows(nameRowsById, abilityId);
                string displayName = GetDisplayName(clientRows, nameRows);
                int effectId = GetPreferredEffectId(clientRows, binRows);
                bool hasClientCsv = clientRows.Count > 0;
                bool hasClientBin = binRows.Count > 0;
                bool hasLocalizedName = nameRows.Count > 0;
                List<string> sources = new List<string>();
                if (hasClientCsv) sources.Add("csv");
                if (hasClientBin) sources.Add("bin");
                if (hasLocalizedName) sources.Add("strings");

                entries.Add(new AbilityCatalogEntry
                {
                    AbilityId = abilityId,
                    Name = displayName,
                    EffectId = effectId > 0 ? (int?)effectId : null,
                    Sources = string.Join(", ", sources),
                    HasClientCsv = hasClientCsv,
                    HasClientBin = hasClientBin,
                    HasLocalizedName = hasLocalizedName,
                    SearchText = (abilityId.ToString(CultureInfo.InvariantCulture) + " " + displayName + " " + string.Join(" ", sources)).ToLowerInvariant()
                });
            }

            return entries;
        }

        public string ResolveOutputRoot(string outputRoot)
        {
            string candidate = string.IsNullOrWhiteSpace(outputRoot) ? Path.Combine("docs", "data-matrix") : outputRoot;
            return Path.GetFullPath(candidate);
        }

        private static List<T> GetRows<T>(Dictionary<ushort, List<T>> rowsById, ushort key)
        {
            List<T> rows;
            return rowsById.TryGetValue(key, out rows) ? rows : new List<T>();
        }

        private static string GetDisplayName(IList<ClientAbilityRecord> clientRows, IList<IndexedStringRecord> nameRows)
        {
            ClientAbilityRecord clientRow = clientRows.FirstOrDefault(row => !string.IsNullOrWhiteSpace(row.Name));
            if (clientRow != null)
                return clientRow.Name;

            IndexedStringRecord stringRow = nameRows.FirstOrDefault(row => !string.IsNullOrWhiteSpace(row.NormalizedValue));
            if (stringRow != null)
                return stringRow.NormalizedValue;

            return "(unnamed)";
        }

        private static int GetPreferredEffectId(IList<ClientAbilityRecord> clientRows, IList<BinaryAbilityRecord> binRows)
        {
            ClientAbilityRecord clientRow = clientRows.FirstOrDefault(row => row.EffectId > 0);
            if (clientRow != null)
                return clientRow.EffectId;

            BinaryAbilityRecord binRow = binRows.FirstOrDefault(row => row.EffectId > 0);
            if (binRow != null)
                return binRow.EffectId;

            return 0;
        }
    }

    public sealed class AbilityCatalogEntry
    {
        public ushort AbilityId { get; set; }
        public string Name { get; set; }
        public int? EffectId { get; set; }
        public string Sources { get; set; }
        public bool HasClientCsv { get; set; }
        public bool HasClientBin { get; set; }
        public bool HasLocalizedName { get; set; }
        public string SearchText { get; set; }

        public string EffectIdText
        {
            get { return EffectId.HasValue ? EffectId.Value.ToString(CultureInfo.InvariantCulture) : string.Empty; }
        }
    }
}
