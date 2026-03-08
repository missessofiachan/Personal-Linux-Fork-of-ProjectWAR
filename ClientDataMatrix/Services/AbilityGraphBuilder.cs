using ClientDataMatrix.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ClientDataMatrix.Services
{
    public sealed class AbilityGraphBuilder
    {
        private readonly AbilityDataset _dataset;
        private readonly Dictionary<uint, List<ClientEffectRecord>> _effectsById;
        private readonly Dictionary<ushort, List<BinaryRequirementRecord>> _requirementsById;
        private readonly HashSet<ushort> _knownRequirementIds;

        public AbilityGraphBuilder(AbilityDataset dataset)
        {
            _dataset = dataset;
            _effectsById = dataset.ClientEffects.GroupBy(x => x.EffectId).ToDictionary(x => x.Key, x => x.OrderBy(y => y.LineNumber).ToList());
            _requirementsById = dataset.BinaryRequirements.GroupBy(x => x.RequirementId).ToDictionary(x => x.Key, x => x.OrderBy(y => y.RecordIndex).ToList());
            _knownRequirementIds = new HashSet<ushort>(_requirementsById.Keys);
        }

        public AbilityAnalysisResult BuildAbilityAnalysis(ushort abilityId, string extractedRootPath)
        {
            AnalysisGraph graph = new AnalysisGraph();
            List<string> warnings = new List<string>();
            List<ClientAbilityRecord> clientAbilityRows = _dataset.ClientAbilities.Where(x => x.AbilityId == abilityId).OrderBy(x => x.LineNumber).ToList();
            List<BinaryAbilityRecord> binaryAbilityRows = _dataset.BinaryAbilities.Where(x => x.AbilityId == abilityId).OrderBy(x => x.RecordIndex).ToList();
            HashSet<int> relatedEffectIds = new HashSet<int>(clientAbilityRows.Select(x => x.EffectId).Where(x => x > 0));
            foreach (BinaryAbilityRecord row in binaryAbilityRows)
                if (row.EffectId > 0)
                    relatedEffectIds.Add(row.EffectId);
            QueueLinkedClientEffects(relatedEffectIds);

            HashSet<ushort> relatedComponentIds = new HashSet<ushort>(binaryAbilityRows.SelectMany(x => x.ComponentIds).Where(x => x > 0));
            List<ClientEffectRecord> clientEffectRows = _dataset.ClientEffects.Where(x => relatedEffectIds.Contains((int)x.EffectId)).OrderBy(x => x.EffectId).ThenBy(x => x.LineNumber).ToList();
            List<BinaryComponentRecord> binaryComponentRows = _dataset.BinaryComponents.Where(x => relatedComponentIds.Contains(x.ComponentId)).OrderBy(x => x.ComponentId).ThenBy(x => x.RecordIndex).ToList();
            List<IndexedStringRecord> abilityNameRows = _dataset.AbilityNames.Where(x => x.EntryId == abilityId).OrderBy(x => x.LineNumber).ToList();
            List<IndexedStringRecord> abilityDescriptionRows = _dataset.AbilityDescriptions.Where(x => x.EntryId == abilityId).OrderBy(x => x.LineNumber).ToList();
            List<IndexedStringRecord> abilityEffectTextRows = _dataset.AbilityEffectTexts.Where(x => x.EntryId == abilityId).OrderBy(x => x.LineNumber).ToList();
            List<IndexedStringRecord> componentEffectRows = _dataset.ComponentEffects.Where(x => x.EntryId <= ushort.MaxValue && relatedComponentIds.Contains((ushort)x.EntryId)).OrderBy(x => x.EntryId).ThenBy(x => x.LineNumber).ToList();
            List<PregameCharacterRecord> pregameRows = _dataset.PregameCharacters.Where(x => Match(x.OnLoadAbilityId, abilityId) || Match(x.MouseOverAbilityId, abilityId) || Match(x.OnClickAbilityId, abilityId)).OrderBy(x => x.Sequence).ToList();
            List<RequirementReferenceRecord> requirementReferences = BuildRequirementReferences(binaryAbilityRows, binaryComponentRows);
            HashSet<ushort> relatedRequirementIds = new HashSet<ushort>(requirementReferences.Select(x => x.RequirementId));
            QueueRequirementDependencies(relatedRequirementIds, requirementReferences);
            List<BinaryRequirementRecord> binaryRequirementRows = relatedRequirementIds
                .Where(x => _requirementsById.ContainsKey(x))
                .SelectMany(x => _requirementsById[x])
                .OrderBy(x => x.RequirementId)
                .ThenBy(x => x.RecordIndex)
                .ToList();

            GraphNode abilityNode = graph.AddNode("ability", abilityId.ToString(CultureInfo.InvariantCulture), "Ability " + abilityId.ToString(CultureInfo.InvariantCulture));
            foreach (ClientAbilityRecord row in clientAbilityRows)
            {
                GraphNode rowNode = AddRowNode(graph, row);
                graph.AddEdge("declared_by", abilityNode, rowNode, "abilities.csv", null);
                AddClaim(graph, "Ability:" + row.AbilityId, "Name", "AbilityName", row.Name, row.Name, row, "name", "client_contract", "abilities.csv");
                AddClaim(graph, "Ability:" + row.AbilityId, "EffectId", "EffectId", row.EffectId.ToString(CultureInfo.InvariantCulture), row.EffectId.ToString(CultureInfo.InvariantCulture), row, "Effect", "client_contract", "abilities.csv");
            }
            foreach (BinaryAbilityRecord row in binaryAbilityRows)
            {
                GraphNode rowNode = AddRowNode(graph, row);
                graph.AddEdge("declared_by", abilityNode, rowNode, "abilityexport.bin", null);
                AddClaim(graph, "Ability:" + row.AbilityId, "EffectId", "EffectId", row.EffectId.ToString(CultureInfo.InvariantCulture), row.EffectId.ToString(CultureInfo.InvariantCulture), row, "EffectId", "client_bin", "abilityexport.bin");
                AddClaim(graph, "Ability:" + row.AbilityId, "CastTime", "Milliseconds", row.CastTime.ToString(CultureInfo.InvariantCulture), row.CastTime.ToString(CultureInfo.InvariantCulture), row, "CastTime", "client_bin", "abilityexport.bin");
                AddClaim(graph, "Ability:" + row.AbilityId, "Cooldown", "Milliseconds", row.Cooldown.ToString(CultureInfo.InvariantCulture), row.Cooldown.ToString(CultureInfo.InvariantCulture), row, "Cooldown", "client_bin", "abilityexport.bin");
                AddClaim(graph, "Ability:" + row.AbilityId, "Range", "Feet", row.Range.ToString(CultureInfo.InvariantCulture), row.Range.ToString(CultureInfo.InvariantCulture), row, "Range", "client_bin", "abilityexport.bin");
                AddClaim(graph, "Ability:" + row.AbilityId, "ApCost", "ActionPoints", row.ApCost.ToString(CultureInfo.InvariantCulture), row.ApCost.ToString(CultureInfo.InvariantCulture), row, "ApCost", "client_bin", "abilityexport.bin");
                AddClaim(graph, "Ability:" + row.AbilityId, "CareerLine", "CareerLineId", row.CareerLine.ToString(CultureInfo.InvariantCulture), row.CareerLine.ToString(CultureInfo.InvariantCulture), row, "CareerLine", "client_bin", "abilityexport.bin");
                foreach (ushort componentId in row.ComponentIds.Where(x => x > 0))
                    graph.AddEdge("uses_component", abilityNode, graph.AddNode("component", componentId.ToString(CultureInfo.InvariantCulture), "Component " + componentId), "abilityexport.bin", null);
            }
            foreach (ClientEffectRecord row in clientEffectRows)
            {
                GraphNode effectNode = graph.AddNode("effect", row.EffectId.ToString(CultureInfo.InvariantCulture), "Effect " + row.EffectId);
                graph.AddEdge("declared_by", effectNode, AddRowNode(graph, row), "effects.csv", null);
                AddClaim(graph, "Effect:" + row.EffectId, "Name", "EffectName", row.Name, row.Name, row, "Name", "client_contract", "effects.csv");
                AddEffectLinkClaim(graph, effectNode, row, "BuildUpId", row.BuildUpId, "Build Up");
                AddEffectLinkClaim(graph, effectNode, row, "ActivateId", row.ActivateId, "Activate");
                AddEffectLinkClaim(graph, effectNode, row, "CastId", row.CastId, "Cast");
                AddEffectLinkClaim(graph, effectNode, row, "ProjectileId", row.ProjectileId, "Projectile");
                AddEffectLinkClaim(graph, effectNode, row, "ImpactId", row.ImpactId, "Impact");
                AddEffectLinkClaim(graph, effectNode, row, "AoeId", row.AoeId, "AOE");
                AddEffectLinkClaim(graph, effectNode, row, "ChannelingId", row.ChannelingId, "Channeling");
            }
            foreach (BinaryComponentRecord row in binaryComponentRows)
            {
                GraphNode componentNode = graph.AddNode("component", row.ComponentId.ToString(CultureInfo.InvariantCulture), "Component " + row.ComponentId);
                graph.AddEdge("declared_by", componentNode, AddRowNode(graph, row), "abilitycomponentexport.bin", null);
                AddClaim(graph, "Component:" + row.ComponentId, "Operation", "ComponentOperation", row.Operation.ToString(CultureInfo.InvariantCulture), row.Operation.ToString(CultureInfo.InvariantCulture), row, "Operation", "client_bin", "abilitycomponentexport.bin");
                AddClaim(graph, "Component:" + row.ComponentId, "Radius", "Radius", row.Radius.ToString(CultureInfo.InvariantCulture), row.Radius.ToString(CultureInfo.InvariantCulture), row, "Radius", "client_bin", "abilitycomponentexport.bin");
                AddClaim(graph, "Component:" + row.ComponentId, "Duration", "Milliseconds", row.Duration.ToString(CultureInfo.InvariantCulture), row.Duration.ToString(CultureInfo.InvariantCulture), row, "Duration", "client_bin", "abilitycomponentexport.bin");
                AddClaim(graph, "Component:" + row.ComponentId, "Interval", "Milliseconds", row.Interval.ToString(CultureInfo.InvariantCulture), row.Interval.ToString(CultureInfo.InvariantCulture), row, "Interval", "client_bin", "abilitycomponentexport.bin");
            }
            foreach (BinaryRequirementRecord row in binaryRequirementRows)
            {
                GraphNode requirementNode = graph.AddNode("requirement", row.RequirementId.ToString(CultureInfo.InvariantCulture), "Requirement " + row.RequirementId);
                graph.AddEdge("declared_by", requirementNode, AddRowNode(graph, row), "abilityrequirementexport.bin", null);
                AddClaim(graph, "Requirement:" + row.RequirementId, "RequirementId", "RequirementId", row.RequirementId.ToString(CultureInfo.InvariantCulture), row.RequirementId.ToString(CultureInfo.InvariantCulture), row, "RequirementId", "client_bin", "abilityrequirementexport.bin");
            }
            foreach (RequirementReferenceRecord reference in requirementReferences.OrderBy(x => x.SourceKind).ThenBy(x => x.SourceId).ThenBy(x => x.ExtSlotIndex).ThenBy(x => x.RequirementId))
                AddRequirementReference(graph, abilityNode, reference);
            foreach (IndexedStringRecord row in abilityNameRows)
                AddString(graph, abilityNode, "Ability:" + row.EntryId, row, "localized_name", "Name", "AbilityName");
            foreach (IndexedStringRecord row in abilityDescriptionRows)
                AddString(graph, abilityNode, "Ability:" + row.EntryId, row, "localized_description", "Description", "AbilityDescription");
            foreach (IndexedStringRecord row in abilityEffectTextRows)
                AddString(graph, abilityNode, "Ability:" + row.EntryId, row, "localized_effect_text", "EffectText", "AbilityEffectText");
            foreach (IndexedStringRecord row in componentEffectRows)
                AddString(graph, graph.AddNode("component", row.EntryId.ToString(CultureInfo.InvariantCulture), "Component " + row.EntryId), "Component:" + row.EntryId, row, "localized_component_description", "Description", "ComponentDescription");
            foreach (PregameCharacterRecord row in pregameRows)
                graph.AddEdge("pregame_reference", AddRowNode(graph, row), abilityNode, row.CareerName + " / " + row.RaceName, null);

            if (clientAbilityRows.Count == 0) warnings.Add("No row was found in abilities.csv for the requested ability ID.");
            if (binaryAbilityRows.Count == 0) warnings.Add("No row was found in abilityexport.bin for the requested ability ID.");
            if (abilityNameRows.Count == 0) warnings.Add("No english ability name row was found in abilitynames.txt.");
            if (abilityDescriptionRows.Count == 0) warnings.Add("No english ability description row was found in abilitydesc.txt.");
            if (abilityEffectTextRows.Count == 0) warnings.Add("No english ability effect-text row was found in abilityeffect.txt.");
            if (binaryComponentRows.Count == 0 && relatedComponentIds.Count > 0) warnings.Add("The BIN ability row references component IDs, but no matching rows were found in abilitycomponentexport.bin.");
            if (_dataset.BinaryRequirements.Count > 0 && requirementReferences.Count == 0) warnings.Add("No inferred requirement references were found for this ability under the current ExtData[*].Val6 linking rule.");
            if (requirementReferences.Count > 0) warnings.Add("Requirement linkage is inferred from ExtData[*].Val6 values that match known RequirementId rows in abilityrequirementexport.bin.");

            return new AbilityAnalysisResult
            {
                AbilityId = abilityId,
                GeneratedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                ExtractedRootPath = extractedRootPath,
                Graph = graph,
                TableStatuses = _dataset.TableStatuses.OrderBy(x => x.SourceFamily).ThenBy(x => x.TableName).ToList(),
                Warnings = warnings,
                ClientAbilityRows = clientAbilityRows,
                BinaryAbilityRows = binaryAbilityRows,
                ClientEffectRows = clientEffectRows,
                AbilityNameRows = abilityNameRows,
                AbilityDescriptionRows = abilityDescriptionRows,
                AbilityEffectTextRows = abilityEffectTextRows,
                ComponentEffectRows = componentEffectRows,
                BinaryComponentRows = binaryComponentRows,
                BinaryRequirementRows = binaryRequirementRows,
                RequirementReferences = requirementReferences,
                PregameRows = pregameRows,
                RelatedEffectIds = relatedEffectIds.OrderBy(x => x).ToList(),
                RelatedComponentIds = relatedComponentIds.OrderBy(x => x).ToList()
            };
        }

        public ConflictReportDocument BuildConflictReport(string extractedRootPath)
        {
            AnalysisGraph graph = new AnalysisGraph();
            foreach (ClientAbilityRecord row in _dataset.ClientAbilities)
            {
                AddClaim(graph, "Ability:" + row.AbilityId, "Name", "AbilityName", row.Name, row.Name, row, "name", "client_contract", "abilities.csv");
                AddClaim(graph, "Ability:" + row.AbilityId, "Description", "AbilityDescription", row.Description, row.Description, row, "Description", "client_contract", "abilities.csv");
                AddClaim(graph, "Ability:" + row.AbilityId, "EffectId", "EffectId", row.EffectId.ToString(CultureInfo.InvariantCulture), row.EffectId.ToString(CultureInfo.InvariantCulture), row, "Effect", "client_contract", "abilities.csv");
            }
            foreach (BinaryAbilityRecord row in _dataset.BinaryAbilities)
            {
                AddClaim(graph, "Ability:" + row.AbilityId, "EffectId", "EffectId", row.EffectId.ToString(CultureInfo.InvariantCulture), row.EffectId.ToString(CultureInfo.InvariantCulture), row, "EffectId", "client_bin", "abilityexport.bin");
                AddClaim(graph, "Ability:" + row.AbilityId, "CastTime", "Milliseconds", row.CastTime.ToString(CultureInfo.InvariantCulture), row.CastTime.ToString(CultureInfo.InvariantCulture), row, "CastTime", "client_bin", "abilityexport.bin");
                AddClaim(graph, "Ability:" + row.AbilityId, "Cooldown", "Milliseconds", row.Cooldown.ToString(CultureInfo.InvariantCulture), row.Cooldown.ToString(CultureInfo.InvariantCulture), row, "Cooldown", "client_bin", "abilityexport.bin");
                AddClaim(graph, "Ability:" + row.AbilityId, "Range", "Feet", row.Range.ToString(CultureInfo.InvariantCulture), row.Range.ToString(CultureInfo.InvariantCulture), row, "Range", "client_bin", "abilityexport.bin");
                AddClaim(graph, "Ability:" + row.AbilityId, "ApCost", "ActionPoints", row.ApCost.ToString(CultureInfo.InvariantCulture), row.ApCost.ToString(CultureInfo.InvariantCulture), row, "ApCost", "client_bin", "abilityexport.bin");
            }
            foreach (ClientEffectRecord row in _dataset.ClientEffects)
            {
                AddClaim(graph, "Effect:" + row.EffectId, "Name", "EffectName", row.Name, row.Name, row, "Name", "client_contract", "effects.csv");
                AddClaim(graph, "Effect:" + row.EffectId, "BuildUpId", "EffectId", row.BuildUpId.ToString(CultureInfo.InvariantCulture), row.BuildUpId.ToString(CultureInfo.InvariantCulture), row, "Build Up", "client_contract", "effects.csv");
                AddClaim(graph, "Effect:" + row.EffectId, "ActivateId", "EffectId", row.ActivateId.ToString(CultureInfo.InvariantCulture), row.ActivateId.ToString(CultureInfo.InvariantCulture), row, "Activate", "client_contract", "effects.csv");
                AddClaim(graph, "Effect:" + row.EffectId, "CastId", "EffectId", row.CastId.ToString(CultureInfo.InvariantCulture), row.CastId.ToString(CultureInfo.InvariantCulture), row, "Cast", "client_contract", "effects.csv");
                AddClaim(graph, "Effect:" + row.EffectId, "ProjectileId", "EffectId", row.ProjectileId.ToString(CultureInfo.InvariantCulture), row.ProjectileId.ToString(CultureInfo.InvariantCulture), row, "Projectile", "client_contract", "effects.csv");
                AddClaim(graph, "Effect:" + row.EffectId, "ImpactId", "EffectId", row.ImpactId.ToString(CultureInfo.InvariantCulture), row.ImpactId.ToString(CultureInfo.InvariantCulture), row, "Impact", "client_contract", "effects.csv");
            }
            AddIndexedStringClaims(graph, _dataset.AbilityNames, "Ability", "Name", "AbilityName");
            AddIndexedStringClaims(graph, _dataset.AbilityDescriptions, "Ability", "Description", "AbilityDescription");
            AddIndexedStringClaims(graph, _dataset.AbilityEffectTexts, "Ability", "EffectText", "AbilityEffectText");
            AddIndexedStringClaims(graph, _dataset.ComponentEffects, "Component", "Description", "ComponentDescription");
            AddIndexedStringClaims(graph, _dataset.RaceNames, "Race", "Name", "RaceName");
            AddIndexedStringClaims(graph, _dataset.CareerNames, "Career", "Name", "CareerName");
            AddIndexedStringClaims(graph, _dataset.CareerLines, "CareerLine", "Name", "CareerLineName");
            foreach (PregameCharacterRecord row in _dataset.PregameCharacters)
            {
                AddClaim(graph, "PregameChar:" + row.Sequence, "RaceName", "RaceName", row.RaceName, row.RaceName, row, "race", "client_xml", "pregame_chars.xml");
                AddClaim(graph, "PregameChar:" + row.Sequence, "CareerName", "CareerName", row.CareerName, row.CareerName, row, "career", "client_xml", "pregame_chars.xml");
            }
            return new ConflictReportDocument { GeneratedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture), ExtractedRootPath = extractedRootPath, TableStatuses = _dataset.TableStatuses.OrderBy(x => x.SourceFamily).ThenBy(x => x.TableName).ToList(), Claims = graph.Claims, Conflicts = graph.GetConflicts() };
        }

        public CoverageReportDocument BuildCoverageReport(string extractedRootPath)
        {
            Dictionary<ushort, List<ClientAbilityRecord>> clientRowsById = _dataset.ClientAbilities
                .Where(row => row.AbilityId <= ushort.MaxValue)
                .GroupBy(row => (ushort)row.AbilityId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.LineNumber).ToList());
            Dictionary<ushort, List<BinaryAbilityRecord>> binRowsById = _dataset.BinaryAbilities
                .GroupBy(row => row.AbilityId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.RecordIndex).ToList());
            Dictionary<ushort, List<IndexedStringRecord>> nameRowsById = _dataset.AbilityNames
                .Where(row => row.EntryId <= ushort.MaxValue)
                .GroupBy(row => (ushort)row.EntryId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.LineNumber).ToList());
            Dictionary<ushort, List<IndexedStringRecord>> descriptionRowsById = _dataset.AbilityDescriptions
                .Where(row => row.EntryId <= ushort.MaxValue)
                .GroupBy(row => (ushort)row.EntryId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.LineNumber).ToList());
            Dictionary<ushort, List<IndexedStringRecord>> effectTextRowsById = _dataset.AbilityEffectTexts
                .Where(row => row.EntryId <= ushort.MaxValue)
                .GroupBy(row => (ushort)row.EntryId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.LineNumber).ToList());
            Dictionary<ushort, List<BinaryComponentRecord>> componentRowsById = _dataset.BinaryComponents
                .GroupBy(row => row.ComponentId)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.RecordIndex).ToList());
            Dictionary<ushort, List<PregameCharacterRecord>> pregameRowsByAbilityId = new Dictionary<ushort, List<PregameCharacterRecord>>();
            foreach (PregameCharacterRecord row in _dataset.PregameCharacters)
            {
                AddPregameRow(pregameRowsByAbilityId, row.OnLoadAbilityId, row);
                AddPregameRow(pregameRowsByAbilityId, row.MouseOverAbilityId, row);
                AddPregameRow(pregameRowsByAbilityId, row.OnClickAbilityId, row);
            }

            HashSet<ushort> abilityIds = new HashSet<ushort>(clientRowsById.Keys);
            foreach (ushort abilityId in binRowsById.Keys)
                abilityIds.Add(abilityId);
            foreach (ushort abilityId in nameRowsById.Keys)
                abilityIds.Add(abilityId);
            foreach (ushort abilityId in descriptionRowsById.Keys)
                abilityIds.Add(abilityId);
            foreach (ushort abilityId in effectTextRowsById.Keys)
                abilityIds.Add(abilityId);
            foreach (ushort abilityId in pregameRowsByAbilityId.Keys)
                abilityIds.Add(abilityId);

            List<CoverageAbilityRecord> rows = new List<CoverageAbilityRecord>();
            foreach (ushort abilityId in abilityIds.OrderBy(value => value))
            {
                List<ClientAbilityRecord> clientRows = GetRows(clientRowsById, abilityId);
                List<BinaryAbilityRecord> binRows = GetRows(binRowsById, abilityId);
                List<IndexedStringRecord> nameRows = GetRows(nameRowsById, abilityId);
                List<IndexedStringRecord> descriptionRows = GetRows(descriptionRowsById, abilityId);
                List<IndexedStringRecord> effectTextRows = GetRows(effectTextRowsById, abilityId);
                List<PregameCharacterRecord> pregameRows = GetRows(pregameRowsByAbilityId, abilityId);

                HashSet<int> relatedEffectIds = new HashSet<int>(clientRows.Select(row => row.EffectId).Where(value => value > 0));
                foreach (BinaryAbilityRecord row in binRows)
                    if (row.EffectId > 0)
                        relatedEffectIds.Add(row.EffectId);
                QueueLinkedClientEffects(relatedEffectIds);

                HashSet<ushort> relatedComponentIds = new HashSet<ushort>(binRows.SelectMany(row => row.ComponentIds).Where(value => value > 0));
                List<BinaryComponentRecord> relatedComponentRows = new List<BinaryComponentRecord>();
                foreach (ushort componentId in relatedComponentIds)
                {
                    List<BinaryComponentRecord> componentRows;
                    if (componentRowsById.TryGetValue(componentId, out componentRows))
                        relatedComponentRows.AddRange(componentRows);
                }

                List<RequirementReferenceRecord> requirementReferences = BuildRequirementReferences(binRows, relatedComponentRows);
                HashSet<ushort> relatedRequirementIds = new HashSet<ushort>(requirementReferences.Select(row => row.RequirementId));
                QueueRequirementDependencies(relatedRequirementIds, requirementReferences);

                int preferredEffectId = GetPreferredEffectId(clientRows, binRows);
                bool hasClientCsv = clientRows.Count > 0;
                bool hasClientBin = binRows.Count > 0;
                bool hasLocalizedName = nameRows.Count > 0 || clientRows.Any(row => !string.IsNullOrWhiteSpace(row.Name));
                bool hasDescriptionText = descriptionRows.Count > 0;
                bool hasEffectText = effectTextRows.Count > 0;
                bool hasRootEffectRow = preferredEffectId > 0 && _effectsById.ContainsKey((uint)preferredEffectId);

                rows.Add(new CoverageAbilityRecord
                {
                    AbilityId = abilityId,
                    Name = GetDisplayName(clientRows, nameRows),
                    EffectId = preferredEffectId > 0 ? (int?)preferredEffectId : null,
                    HasClientCsv = hasClientCsv,
                    HasClientBin = hasClientBin,
                    HasLocalizedName = hasLocalizedName,
                    HasDescriptionText = hasDescriptionText,
                    HasEffectText = hasEffectText,
                    HasRootEffectRow = hasRootEffectRow,
                    RelatedEffectCount = relatedEffectIds.Count,
                    ComponentCount = relatedComponentIds.Count,
                    RequirementLinkCount = requirementReferences.Count,
                    RequirementRowCount = relatedRequirementIds.Count,
                    HasPregameReference = pregameRows.Count > 0,
                    CoverageStatus = DetermineCoverageStatus(hasClientCsv, hasClientBin, hasLocalizedName, hasDescriptionText, hasRootEffectRow, relatedComponentIds.Count, requirementReferences.Count),
                    MissingText = BuildCoverageMissingText(hasClientCsv, hasClientBin, hasLocalizedName, hasDescriptionText, hasEffectText, hasRootEffectRow, relatedComponentIds.Count),
                    SourcesText = BuildCoverageSourcesText(hasClientCsv, hasClientBin, hasLocalizedName || hasDescriptionText || hasEffectText, pregameRows.Count > 0, requirementReferences.Count > 0)
                });
            }

            return new CoverageReportDocument
            {
                GeneratedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                ExtractedRootPath = extractedRootPath,
                TableStatuses = _dataset.TableStatuses.OrderBy(x => x.SourceFamily).ThenBy(x => x.TableName).ToList(),
                Abilities = rows
            };
        }

        private static void AddPregameRow(Dictionary<ushort, List<PregameCharacterRecord>> rowsByAbilityId, int? abilityId, PregameCharacterRecord row)
        {
            if (!abilityId.HasValue || abilityId.Value <= 0 || abilityId.Value > ushort.MaxValue)
                return;

            ushort key = (ushort)abilityId.Value;
            List<PregameCharacterRecord> rows;
            if (!rowsByAbilityId.TryGetValue(key, out rows))
            {
                rows = new List<PregameCharacterRecord>();
                rowsByAbilityId[key] = rows;
            }

            rows.Add(row);
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

            IndexedStringRecord nameRow = nameRows.FirstOrDefault(row => !string.IsNullOrWhiteSpace(row.NormalizedValue));
            return nameRow == null ? "(unnamed)" : nameRow.NormalizedValue;
        }

        private static int GetPreferredEffectId(IList<ClientAbilityRecord> clientRows, IList<BinaryAbilityRecord> binRows)
        {
            ClientAbilityRecord clientRow = clientRows.FirstOrDefault(row => row.EffectId > 0);
            if (clientRow != null)
                return clientRow.EffectId;

            BinaryAbilityRecord binRow = binRows.FirstOrDefault(row => row.EffectId > 0);
            return binRow == null ? 0 : binRow.EffectId;
        }

        private static string DetermineCoverageStatus(bool hasClientCsv, bool hasClientBin, bool hasLocalizedName, bool hasDescriptionText, bool hasRootEffectRow, int componentCount, int requirementLinkCount)
        {
            if (hasClientCsv && hasClientBin && hasLocalizedName && hasDescriptionText && hasRootEffectRow && componentCount > 0 && requirementLinkCount > 0)
                return "MappedWithRequirements";
            if (hasClientCsv && hasClientBin && hasLocalizedName && hasDescriptionText && hasRootEffectRow && componentCount > 0)
                return "Mapped";
            if (hasClientCsv && hasClientBin && hasRootEffectRow && componentCount > 0)
                return "PlayableSurface";
            if (hasClientCsv || hasClientBin)
                return "Partial";
            if (hasLocalizedName || hasDescriptionText)
                return "StringsOnly";
            return "Sparse";
        }

        private static string BuildCoverageMissingText(bool hasClientCsv, bool hasClientBin, bool hasLocalizedName, bool hasDescriptionText, bool hasEffectText, bool hasRootEffectRow, int componentCount)
        {
            List<string> missing = new List<string>();
            if (!hasClientCsv) missing.Add("csv");
            if (!hasClientBin) missing.Add("bin");
            if (!hasLocalizedName) missing.Add("name");
            if (!hasDescriptionText) missing.Add("description");
            if (!hasEffectText) missing.Add("effect-text");
            if (!hasRootEffectRow) missing.Add("effect-row");
            if (componentCount <= 0) missing.Add("components");
            return missing.Count == 0 ? "(none)" : string.Join(", ", missing);
        }

        private static string BuildCoverageSourcesText(bool hasClientCsv, bool hasClientBin, bool hasStringData, bool hasPregameReference, bool hasRequirementLinks)
        {
            List<string> sources = new List<string>();
            if (hasClientCsv) sources.Add("csv");
            if (hasClientBin) sources.Add("bin");
            if (hasStringData) sources.Add("strings");
            if (hasPregameReference) sources.Add("pregame");
            if (hasRequirementLinks) sources.Add("requirements");
            return string.Join(", ", sources);
        }

        private List<RequirementReferenceRecord> BuildRequirementReferences(List<BinaryAbilityRecord> abilityRows, List<BinaryComponentRecord> componentRows)
        {
            Dictionary<string, RequirementReferenceRecord> references = new Dictionary<string, RequirementReferenceRecord>(StringComparer.OrdinalIgnoreCase);

            foreach (BinaryAbilityRecord row in abilityRows)
                AddRequirementReferencesFromExtData(references, "Ability", row.AbilityId, "Ability " + row.AbilityId.ToString(CultureInfo.InvariantCulture), row.ExtData, row.SourceFamily, row.TableName, row.SourcePath, row.RowKey, row.LineNumber, row.ByteOffset);
            foreach (BinaryComponentRecord row in componentRows)
                AddRequirementReferencesFromExtData(references, "Component", row.ComponentId, "Component " + row.ComponentId.ToString(CultureInfo.InvariantCulture) + " (" + DefinitionCatalog.DescribeComponentOperationValue(row.Operation) + ")", row.ExtData, row.SourceFamily, row.TableName, row.SourcePath, row.RowKey, row.LineNumber, row.ByteOffset);

            return references.Values.OrderBy(x => x.SourceKind).ThenBy(x => x.SourceId).ThenBy(x => x.ExtSlotIndex).ThenBy(x => x.RequirementId).ToList();
        }

        private void QueueRequirementDependencies(HashSet<ushort> relatedRequirementIds, List<RequirementReferenceRecord> references)
        {
            Queue<ushort> pending = new Queue<ushort>(relatedRequirementIds.OrderBy(x => x));
            HashSet<ushort> expanded = new HashSet<ushort>();

            while (pending.Count > 0)
            {
                ushort requirementId = pending.Dequeue();
                if (!expanded.Add(requirementId))
                    continue;

                List<BinaryRequirementRecord> requirementRows;
                if (!_requirementsById.TryGetValue(requirementId, out requirementRows))
                    continue;

                foreach (BinaryRequirementRecord row in requirementRows)
                {
                    foreach (BinaryExtDataRecord extRecord in row.ExtData)
                    {
                        ushort nestedRequirementId;
                        if (!TryGetKnownRequirementId(extRecord.Val6, out nestedRequirementId))
                            continue;

                        RequirementReferenceRecord reference = CreateRequirementReference("Requirement", row.RequirementId, "Requirement " + row.RequirementId.ToString(CultureInfo.InvariantCulture), extRecord.SlotIndex, nestedRequirementId, row.SourceFamily, row.TableName, row.SourcePath, row.RowKey, row.LineNumber, row.ByteOffset);
                        AddRequirementReferenceRecord(references, reference);
                        if (relatedRequirementIds.Add(nestedRequirementId))
                            pending.Enqueue(nestedRequirementId);
                    }
                }
            }
        }

        private void AddRequirementReferencesFromExtData(Dictionary<string, RequirementReferenceRecord> references, string sourceKind, ushort sourceId, string sourceLabel, IList<BinaryExtDataRecord> extDataRows, string sourceFamily, string sourceTableName, string sourcePath, string sourceRowKey, int lineNumber, long byteOffset)
        {
            foreach (BinaryExtDataRecord extRecord in extDataRows ?? new List<BinaryExtDataRecord>())
            {
                ushort requirementId;
                if (!TryGetKnownRequirementId(extRecord.Val6, out requirementId))
                    continue;

                AddRequirementReferenceRecord(references, CreateRequirementReference(sourceKind, sourceId, sourceLabel, extRecord.SlotIndex, requirementId, sourceFamily, sourceTableName, sourcePath, sourceRowKey, lineNumber, byteOffset));
            }
        }

        private bool TryGetKnownRequirementId(int rawValue, out ushort requirementId)
        {
            requirementId = 0;
            if (rawValue <= 0 || rawValue > ushort.MaxValue)
                return false;

            requirementId = (ushort)rawValue;
            return _knownRequirementIds.Contains(requirementId);
        }

        private static RequirementReferenceRecord CreateRequirementReference(string sourceKind, ushort sourceId, string sourceLabel, int slotIndex, ushort requirementId, string sourceFamily, string sourceTableName, string sourcePath, string sourceRowKey, int lineNumber, long byteOffset)
        {
            return new RequirementReferenceRecord
            {
                SourceKind = sourceKind,
                SourceId = sourceId,
                SourceLabel = sourceLabel,
                SourceField = "ExtData[" + slotIndex.ToString(CultureInfo.InvariantCulture) + "].Val6",
                ExtSlotIndex = slotIndex,
                RequirementId = requirementId,
                Confidence = SemanticConfidence.Inferred,
                Notes = "Inferred requirement reference because ExtData[*].Val6 matches a known RequirementId row in abilityrequirementexport.bin.",
                SourceFamily = sourceFamily,
                SourceTableName = sourceTableName,
                SourcePath = sourcePath,
                SourceRowKey = sourceRowKey,
                LineNumber = lineNumber,
                ByteOffset = byteOffset,
                SourceLocation = lineNumber > 0 ? "line " + lineNumber.ToString(CultureInfo.InvariantCulture) : "byte " + byteOffset.ToString(CultureInfo.InvariantCulture)
            };
        }

        private static void AddRequirementReferenceRecord(Dictionary<string, RequirementReferenceRecord> references, RequirementReferenceRecord reference)
        {
            string key = BuildRequirementReferenceKey(reference);
            if (!references.ContainsKey(key))
                references[key] = reference;
        }

        private static void AddRequirementReferenceRecord(List<RequirementReferenceRecord> references, RequirementReferenceRecord reference)
        {
            if (!references.Any(existing => IsSameRequirementReference(existing, reference)))
                references.Add(reference);
        }

        private static bool IsSameRequirementReference(RequirementReferenceRecord left, RequirementReferenceRecord right)
        {
            return string.Equals(BuildRequirementReferenceKey(left), BuildRequirementReferenceKey(right), StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildRequirementReferenceKey(RequirementReferenceRecord reference)
        {
            return reference.SourceKind + "|" + reference.SourceId.ToString(CultureInfo.InvariantCulture) + "|" + reference.SourceField + "|" + reference.RequirementId.ToString(CultureInfo.InvariantCulture);
        }

        private void AddRequirementReference(AnalysisGraph graph, GraphNode abilityNode, RequirementReferenceRecord reference)
        {
            GraphNode sourceNode = ResolveRequirementSourceNode(graph, abilityNode, reference);
            GraphNode requirementNode = graph.AddNode("requirement", reference.RequirementId.ToString(CultureInfo.InvariantCulture), "Requirement " + reference.RequirementId.ToString(CultureInfo.InvariantCulture));
            ClaimRecord claim = graph.AddClaim(new ClaimRecord
            {
                SubjectKey = reference.SourceKind + ":" + reference.SourceId.ToString(CultureInfo.InvariantCulture),
                FactName = "RequirementId",
                Domain = "RequirementId",
                ValueKey = reference.RequirementId.ToString(CultureInfo.InvariantCulture),
                RawValue = reference.RequirementId.ToString(CultureInfo.InvariantCulture),
                NormalizedValue = reference.RequirementId.ToString(CultureInfo.InvariantCulture),
                SourceFamily = reference.SourceFamily,
                TableName = reference.SourceTableName,
                SourcePath = reference.SourcePath,
                RowKey = reference.SourceRowKey,
                LineNumber = reference.LineNumber,
                ByteOffset = reference.ByteOffset,
                FieldName = reference.SourceField,
                Confidence = reference.Confidence,
                Notes = reference.Notes
            });
            graph.AddEdge("requires", sourceNode, requirementNode, reference.SourceField, claim == null ? null : claim.Id);
        }

        private static GraphNode ResolveRequirementSourceNode(AnalysisGraph graph, GraphNode abilityNode, RequirementReferenceRecord reference)
        {
            if (string.Equals(reference.SourceKind, "Ability", StringComparison.OrdinalIgnoreCase))
                return abilityNode;
            if (string.Equals(reference.SourceKind, "Component", StringComparison.OrdinalIgnoreCase))
                return graph.AddNode("component", reference.SourceId.ToString(CultureInfo.InvariantCulture), "Component " + reference.SourceId.ToString(CultureInfo.InvariantCulture));
            return graph.AddNode("requirement", reference.SourceId.ToString(CultureInfo.InvariantCulture), "Requirement " + reference.SourceId.ToString(CultureInfo.InvariantCulture));
        }

        private GraphNode AddRowNode(AnalysisGraph graph, SourceRowBase row)
        {
            GraphNode node = graph.AddNode("row", row.TableName + "|" + row.RowKey, row.TableName + " " + row.RowKey);
            graph.AddNodeProperty(node, "source_path", row.SourcePath);
            if (row.LineNumber > 0) graph.AddNodeProperty(node, "line_number", row.LineNumber.ToString(CultureInfo.InvariantCulture));
            if (row.ByteOffset > 0) graph.AddNodeProperty(node, "byte_offset", row.ByteOffset.ToString(CultureInfo.InvariantCulture));
            return node;
        }

        private void AddString(AnalysisGraph graph, GraphNode subjectNode, string subjectKey, IndexedStringRecord row, string edgeKind, string factName, string domain)
        {
            GraphNode stringNode = AddRowNode(graph, row);
            graph.AddNodeProperty(stringNode, "raw_value", row.Value);
            graph.AddNodeProperty(stringNode, "normalized_value", row.NormalizedValue);
            ClaimRecord claim = AddClaim(graph, subjectKey, factName, domain, row.Value, row.NormalizedValue, row, "value", "localized_string", row.TableName);
            graph.AddEdge(edgeKind, subjectNode, stringNode, row.TableName, claim == null ? null : claim.Id);
        }

        private void AddEffectLinkClaim(AnalysisGraph graph, GraphNode sourceNode, ClientEffectRecord row, string factName, int targetEffectId, string field)
        {
            if (targetEffectId <= 0) return;
            ClaimRecord claim = AddClaim(graph, "Effect:" + row.EffectId, factName, "EffectId", targetEffectId.ToString(CultureInfo.InvariantCulture), targetEffectId.ToString(CultureInfo.InvariantCulture), row, field, "client_contract", "effects.csv");
            graph.AddEdge("linked_effect", sourceNode, graph.AddNode("effect", targetEffectId.ToString(CultureInfo.InvariantCulture), "Effect " + targetEffectId), field, claim == null ? null : claim.Id);
        }

        private static void AddIndexedStringClaims(AnalysisGraph graph, IEnumerable<IndexedStringRecord> rows, string prefix, string factName, string domain)
        {
            foreach (IndexedStringRecord row in rows)
                AddClaim(graph, prefix + ":" + row.EntryId, factName, domain, row.Value, row.NormalizedValue, row, "value", "localized_string", row.TableName);
        }

        private void QueueLinkedClientEffects(HashSet<int> relatedEffectIds)
        {
            Queue<int> pending = new Queue<int>(relatedEffectIds.Where(x => x > 0).Distinct());
            HashSet<int> seen = new HashSet<int>();
            while (pending.Count > 0)
            {
                int effectId = pending.Dequeue();
                if (!seen.Add(effectId)) continue;
                List<ClientEffectRecord> effectRows;
                if (!_effectsById.TryGetValue((uint)effectId, out effectRows)) continue;
                foreach (ClientEffectRecord row in effectRows)
                {
                    Enqueue(relatedEffectIds, pending, row.BuildUpId);
                    Enqueue(relatedEffectIds, pending, row.ActivateId);
                    Enqueue(relatedEffectIds, pending, row.CastId);
                    Enqueue(relatedEffectIds, pending, row.ProjectileId);
                    Enqueue(relatedEffectIds, pending, row.ImpactId);
                    Enqueue(relatedEffectIds, pending, row.AoeId);
                    Enqueue(relatedEffectIds, pending, row.ChannelingId);
                }
            }
        }

        private static void Enqueue(HashSet<int> all, Queue<int> pending, int effectId)
        {
            if (effectId <= 0) return;
            if (all.Add(effectId)) pending.Enqueue(effectId);
        }

        private static ClaimRecord AddClaim(AnalysisGraph graph, string subjectKey, string factName, string domain, string rawValue, string normalizedValue, SourceRowBase row, string fieldName, string confidence, string notes)
        {
            if (row == null) return null;
            return graph.AddClaim(new ClaimRecord { SubjectKey = subjectKey, FactName = factName, Domain = domain, ValueKey = normalizedValue ?? string.Empty, RawValue = rawValue ?? string.Empty, NormalizedValue = normalizedValue ?? string.Empty, SourceFamily = row.SourceFamily, TableName = row.TableName, SourcePath = row.SourcePath, RowKey = row.RowKey, LineNumber = row.LineNumber, ByteOffset = row.ByteOffset, FieldName = fieldName, Confidence = confidence, Notes = notes });
        }

        private static bool Match(int? candidateAbilityId, ushort abilityId)
        {
            return candidateAbilityId.HasValue && candidateAbilityId.Value == abilityId;
        }
    }
}
