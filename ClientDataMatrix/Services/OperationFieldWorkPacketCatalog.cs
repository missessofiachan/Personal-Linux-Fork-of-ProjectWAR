using ClientDataMatrix.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ClientDataMatrix.Services
{
    public sealed class OperationFieldWorkPacketCatalog
    {
        public const int DefaultPacketCount = 12;
        public const int DefaultValueCount = 5;
        public const int DefaultAbilityCountPerValue = 6;

        private readonly ComponentSchemaCatalog _componentSchemas;

        public OperationFieldWorkPacketCatalog(ComponentSchemaCatalog componentSchemas)
        {
            _componentSchemas = componentSchemas;
        }

        public OperationFieldWorkPacketDocument BuildDocument(
            string extractedRootPath,
            RemainingWorkDocument remainingWork,
            OperationSchemaDocument operations,
            int topCount,
            string minimumPriorityBucket,
            string searchText)
        {
            int packetCount = topCount > 0 ? topCount : DefaultPacketCount;
            string effectivePriorityBucket = string.IsNullOrWhiteSpace(minimumPriorityBucket)
                ? RemainingWorkCatalog.DefaultNextBatchMinimumPriorityBucket
                : minimumPriorityBucket;
            string filterDescription = RemainingWorkCatalog.BuildFilterDescription("Operations", effectivePriorityBucket, searchText, packetCount);
            List<RemainingWorkItemRecord> items = RemainingWorkCatalog.FilterItems(remainingWork, "Operations", effectivePriorityBucket, searchText, packetCount)
                .Where(row => string.Equals(row.SubjectKind, "OperationField", StringComparison.OrdinalIgnoreCase))
                .ToList();

            Dictionary<uint, ComponentOperationSchemaRecord> operationsById = operations == null || operations.Operations == null
                ? new Dictionary<uint, ComponentOperationSchemaRecord>()
                : operations.Operations.ToDictionary(row => row.OperationId);

            List<OperationFieldWorkPacketRecord> packets = new List<OperationFieldWorkPacketRecord>();
            foreach (RemainingWorkItemRecord item in items)
            {
                uint operationId;
                string fieldKey;
                if (!TryParseOperationFieldSubjectKey(item.SubjectKey, out operationId, out fieldKey))
                    continue;

                ComponentOperationSchemaRecord operation;
                operationsById.TryGetValue(operationId, out operation);
                List<ComponentOperationFieldValueRecord> values = _componentSchemas == null
                    ? new List<ComponentOperationFieldValueRecord>()
                    : _componentSchemas.BuildOperationFieldValueEvidence(operationId, fieldKey).Take(DefaultValueCount).ToList();

                packets.Add(new OperationFieldWorkPacketRecord
                {
                    GlobalRank = item.GlobalRank,
                    AreaRank = item.Rank,
                    OperationId = operationId,
                    OperationName = operation == null ? string.Empty : operation.OperationName,
                    FieldKey = fieldKey,
                    PriorityBucket = item.PriorityBucket,
                    PriorityScore = item.PriorityScore,
                    Title = item.Title,
                    Summary = item.Summary,
                    Evidence = item.Evidence,
                    RecommendedAction = item.RecommendedAction,
                    ExampleAbilityId = item.ExampleAbilityId,
                    SampleValuesText = operation == null || operation.Fields == null
                        ? string.Empty
                        : operation.Fields
                            .Where(row => string.Equals(row.FieldKey, fieldKey, StringComparison.OrdinalIgnoreCase))
                            .Select(row => row.SampleValuesText)
                            .FirstOrDefault() ?? string.Empty,
                    TopValues = values.Select(value => BuildValueRecord(operationId, fieldKey, value)).ToList()
                });
            }

            return new OperationFieldWorkPacketDocument
            {
                GeneratedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                ExtractedRootPath = extractedRootPath,
                FilterDescription = filterDescription,
                Packets = packets
            };
        }

        private OperationFieldWorkPacketValueRecord BuildValueRecord(uint operationId, string fieldKey, ComponentOperationFieldValueRecord value)
        {
            ComponentOperationFieldValueInsightRecord insight = value == null || string.IsNullOrWhiteSpace(value.RawValue) || _componentSchemas == null
                ? null
                : _componentSchemas.BuildOperationFieldValueInsight(operationId, fieldKey, value.RawValue);
            return new OperationFieldWorkPacketValueRecord
            {
                RawValue = value == null ? string.Empty : value.RawValue,
                ObservationCount = value == null ? 0 : value.ObservationCount,
                DistinctComponentCount = value == null ? 0 : value.DistinctComponentCount,
                DistinctAbilityCount = value == null ? 0 : value.DistinctAbilityCount,
                SampleComponentIdsText = value == null ? string.Empty : value.SampleComponentIdsText,
                SampleAbilityIdsText = value == null ? string.Empty : value.SampleAbilityIdsText,
                TriggerSummaryText = insight == null ? string.Empty : insight.TriggerSummaryText,
                ContextTagSummaryText = insight == null ? string.Empty : insight.ContextTagSummaryText,
                CompanionSummaryText = insight == null ? string.Empty : insight.CompanionSummaryText,
                SampleAbilities = value == null || value.SampleAbilities == null
                    ? new List<ComponentOperationAbilityRecord>()
                    : value.SampleAbilities.Take(DefaultAbilityCountPerValue).ToList()
            };
        }

        private static bool TryParseOperationFieldSubjectKey(string subjectKey, out uint operationId, out string fieldKey)
        {
            operationId = 0;
            fieldKey = string.Empty;

            if (string.IsNullOrWhiteSpace(subjectKey))
                return false;

            int separatorIndex = subjectKey.IndexOf(':');
            if (separatorIndex <= 0 || separatorIndex >= subjectKey.Length - 1)
                return false;

            if (!uint.TryParse(subjectKey.Substring(0, separatorIndex), NumberStyles.Integer, CultureInfo.InvariantCulture, out operationId))
                return false;

            fieldKey = subjectKey.Substring(separatorIndex + 1);
            return !string.IsNullOrWhiteSpace(fieldKey);
        }
    }
}
