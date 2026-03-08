# Client Data Matrix Tool Plan

## Purpose

Build a read-only analysis tool inside this repo that links extracted client data into one relational graph so maintainers can:

- trace one game concept across all known sources
- detect contradictions between sources
- document identifier-domain mismatches explicitly
- generate a canonical evidence trail before changing runtime code

This tool is intended to improve emulator correctness first. It is not intended to become a new runtime dependency for `WorldServer`.

## Why This Exists

The current emulator contains multiple overlapping truth sources for core systems, especially abilities. The immediate problems are:

- client-facing IDs and server-facing IDs are mixed together
- alias tables and fake IDs obscure the actual execution path
- documentation often describes a concept without preserving the exact source evidence
- different tables can assign different meanings to the same number

The tool must make those contradictions visible instead of flattening them away.

## Non-Negotiable Requirements

1. The tool is read-only against source data.
2. Every mapping must carry source provenance.
3. Conflicts must be preserved, not silently resolved.
4. Identifier domains must be explicit.
5. Documentation output must be exhaustive enough to show:
   - what source claimed a value
   - what exact field claimed it
   - what other source disagreed
   - whether the conflict is unresolved or intentionally overridden

## First Delivery Scope

The first slice is extracted client files for abilities and core identities:

- `C:\Users\Admin\Pictures\WAR_extracted\data\gamedata\abilities.csv`
- `C:\Users\Admin\Pictures\WAR_extracted\data\gamedata\effects.csv`
- `C:\Users\Admin\Pictures\WAR_extracted\data\strings\english\abilitynames.txt`
- `C:\Users\Admin\Pictures\WAR_extracted\data\strings\english\abilitydesc.txt`
- `C:\Users\Admin\Pictures\WAR_extracted\data\strings\english\abilityeffect.txt`
- `C:\Users\Admin\Pictures\WAR_extracted\data\strings\english\racenames_m.txt`
- `C:\Users\Admin\Pictures\WAR_extracted\data\strings\english\careernames_m.txt`
- `C:\Users\Admin\Pictures\WAR_extracted\data\strings\english\careerlines_m.txt`
- `C:\Users\Admin\Pictures\WAR_extracted\data\gamedata\pregame_chars.xml`

The extracted BIN exports are still in scope, but as a second pass once a parser exists:

- `abilityexport.bin`
- `abilitycomponentexport.bin`
- `abilityrequirementexport.bin`

## Architecture

### High-Level Structure

The tool should live in its own console project and should:

1. read extracted files directly from `C:\Users\Admin\Pictures\WAR_extracted`
2. ingest rows into a typed graph model
3. preserve source file, line, column, and raw-value provenance
4. emit human-readable reports and machine-readable exports

### Core Concept

The internal model should be a graph with typed nodes and typed edges.

Example node categories:

- ability
- effect
- buff
- ability_command
- buff_command
- graph_component
- graph_requirement
- alias_rule
- identifier_value
- table_row
- field_claim

Example edge categories:

- references
- invokes
- aliases_to
- sourced_from
- contradicts
- matches
- derived_from
- linked_by_effect
- implemented_by_command
- constrained_by_requirement

## Provenance Model

Every claim must record:

- source family
- table name
- row identifier
- field name
- raw value
- normalized value
- domain
- confidence
- notes

Example:

- source family: `client_csv`
- table name: `abilities.csv`
- row identifier: `line=6;AbilityId=1`
- field name: `Effect`
- raw value: `1`
- normalized value: `1`
- domain: `EffectId`
- confidence: `authoritative_for_client_contract`

## Conflict Documentation Rule

If one source says:

- `CSV XYZ: Empire = RaceId 1`

and another source says:

- `CSV ABC: RaceId 1 = Orc`

the tool must record both claims and emit a conflict report that includes:

- both source locations
- both raw values
- the shared normalized domain being disputed
- whether the conflict is exact, likely, or inferred
- whether a manual override exists

The tool must never collapse this into a single silent answer.

## Commands To Build First

### `doctor ability <id>`

Show the full lineage for one ability:

- client-facing ability row from `abilities.csv`
- client-facing effect row and linked effect chain from `effects.csv`
- localized string rows for name, description, and effect text
- any extracted XML/UI references that point at the same ability
- extracted BIN-export availability and parse status
- conflicts and missing links
- reasons the client contract appears valid or invalid

### `export graph`

Emit a graph snapshot for later analysis. Initial formats:

- JSON
- Graphviz DOT
- Markdown summaries

### `report conflicts`

Emit a conflict ledger grouped by domain:

- `RaceId`
- `CareerId`
- `AbilityId`
- `EffectId`
- `BuffId`
- other discovered identifier domains

### `report coverage`

Show which castable abilities have:

- CSV row only
- CSV row plus localized strings
- missing effect links
- missing localized text
- referenced from pregame/UI sources
- unresolved effect links

## Documentation Outputs

The tool must generate documentation files that are suitable for maintainers, not just debugging.

Initial output categories:

- `docs/data-matrix/ability/<id>.md`
- `docs/data-matrix/conflicts/<domain>.md`
- `docs/data-matrix/coverage/ability-summary.md`

Each generated document should include:

- source evidence tables
- inline field-level citations including source file, row, and column
- explicit conflict sections
- unresolved questions
- normalization notes

## Implementation Sequence

1. Create the console project and add it to the solution.
2. Add a typed in-memory graph model.
3. Implement file readers for extracted CSV, TXT, and XML sources.
4. Implement ability-source ingestion with provenance capture.
5. Implement `doctor ability <id>`.
6. Implement conflict ledger export.
7. Implement Markdown and JSON output.
8. Add extracted BIN parsing once the file format reader is in place.

## Guardrails

- Do not make `WorldServer` depend on this tool.
- Do not mutate live data as part of analysis.
- Do not hide source contradictions behind “best guess” values.
- Do not use ambiguous property names like `Race` when the domain is actually `RaceMask`, `ClientRaceRowId`, or `RaceId`.

## Crash Recovery Notes

If work stops unexpectedly, the next step is:

1. add the new console project
2. wire it to extracted client files under `C:\Users\Admin\Pictures\WAR_extracted`
3. implement ability/effect/string ingestion first
4. generate the first `doctor ability` report before expanding scope

## Checkpoint Status - 2026-03-08

The plan above has been partially implemented. Recovery should resume from the current code state, not from the original empty-project assumptions.

### Implemented So Far

1. Added the new console project at `ClientDataMatrix/`.
2. Added the project to `ProjectWAR.sln`.
3. Implemented initial commands:
   - `doctor ability <abilityId>`
   - `export graph ability <abilityId>`
   - `report conflicts`
4. Removed all DB and `World.xml` dependencies from the tool.
5. Implemented file-backed analysis models for nodes, edges, claims, conflicts, and typed source rows.
6. Implemented extracted-file ingestion for:
   - `data\gamedata\abilities.csv`
   - `data\gamedata\effects.csv`
   - `data\strings\english\abilitynames.txt`
   - `data\strings\english\abilitydesc.txt`
   - `data\strings\english\abilityeffect.txt`
   - `data\strings\english\componenteffects.txt`
   - `data\strings\english\racenames_m.txt`
   - `data\strings\english\careernames_m.txt`
   - `data\strings\english\careerlines_m.txt`
   - `data\gamedata\pregame_chars.xml`
   - `data\bin\abilityexport.bin`
   - `data\bin\abilitycomponentexport.bin`
   - `data\bin\abilityrequirementexport.bin`
7. Implemented report writers for:
   - Markdown
   - JSON
   - DOT
   - CSV
8. Verified `abilityexport.bin` parses cleanly with the extracted client layout:
   - header `0x24`
   - record count `11736`
9. Extended generated ability reports so they now include:
   - BIN ability rows
   - BIN component rows
   - component description rows
   - ext-data summaries
   - source path, line, and byte-offset provenance
10. Ability reports now infer requirement links when extracted `ExtData[*].Val6` values match known `RequirementId` rows, expand nested requirement dependencies, and surface those rows in markdown, the tree view, the narrative, and the definitions explorer.
11. Coverage reporting now exists as both a persisted ledger and a GUI tab, so the full extracted ability set can be sorted by readiness and missing evidence.
12. Shared field-name evidence now carries `Duration`, `Interval`, and `Radius` semantics across operations when direct per-operation token evidence is absent, and operation-schema reports now flag exact `ExtData[*].Val6` matches against known `RequirementId` rows inline.
13. Requirement ledger reporting now exists as both a persisted ledger and a GUI tab, so `RequirementId` rows can be inspected through inbound links, direct child requirement chains, active ext-data fields, and direct ability-context tags.

### Current Code Locations

- `ClientDataMatrix/Program.cs`
- `ClientDataMatrix/Configuration/ExtractedDataRootResolver.cs`
- `ClientDataMatrix/Model/AnalysisTypes.cs`
- `ClientDataMatrix/Model/AbilityDataset.cs`
- `ClientDataMatrix/Parsers/AbilityBinaryParser.cs`
- `ClientDataMatrix/Services/AbilityGraphBuilder.cs`
- `ClientDataMatrix/Services/ComponentSchemaCatalog.cs`
- `ClientDataMatrix/Services/DefinitionCatalog.cs`
- `ClientDataMatrix/Output/ReportWriters.cs`

### Build Note

`ClientDataMatrix` builds as a standalone extracted-file tool. It does not need `Common`, `FrameWork`, `DBManager`, or `World.xml`.

It now supports both:

- a WinForms GUI when launched without arguments
- the original CLI commands when launched with command arguments

### Verified Build Command

```powershell
$env:DOTNET_CLI_HOME=(Resolve-Path '.dotnet_home').Path
dotnet msbuild ClientDataMatrix\ClientDataMatrix.csproj /t:Build /p:Configuration=Debug /p:Platform=x64 /v:m
```

Expected successful output binary:

- `bin\Debug\ClientDataMatrix.exe`

### Verified Working Command

```powershell
.\bin\Debug\ClientDataMatrix.exe --help
```

This command prints usage correctly and confirms the executable starts.

### GUI Usage

The GUI now launches by default:

```powershell
.\bin\Debug\ClientDataMatrix.exe
```

The GUI provides:

1. path controls for the extracted WAR client root and report output root
2. a main-page `Generate All` button that runs the shared ledgers in sequence and also generates the currently selected ability report when the ability id box is valid
3. an `Ability Doctor` tab with searchable catalog, report generation, tree-view evidence output, an inferred `What Happens` narrative, decoded field definitions, a double-click field-domain explorer, and open-file/open-folder actions
4. a `Token Dictionary` tab with plain-English COM token definitions built from extracted client string evidence
5. a `Domains` tab with explicit extracted-client identifier-domain ledgers
6. a `Requirements` tab with requirement-row summaries, raw ext-data rows, field activity, and inbound/outbound link browsing
7. a `Coverage` tab with whole-dataset ability readiness browsing
8. a `Conflict Ledger` tab with per-domain conflict summaries and filtered conflict detail browsing
9. a `Source Status` tab for file load outcomes
10. a `Log` tab for timestamped session actions
11. an `Operation Schemas` tab for browsing component operations, recurring non-zero fields, semantic hints, and sample abilities

### Confirmed Direction Change

The file-backed extracted dataset is the required source of truth for this tool.

Confirmed high-value source files:

- `data\gamedata\abilities.csv`
- `data\gamedata\effects.csv`
- `data\strings\english\abilitynames.txt`
- `data\strings\english\abilitydesc.txt`
- `data\strings\english\abilityeffect.txt`
- `data\strings\english\racenames_m.txt`
- `data\strings\english\careernames_m.txt`
- `data\strings\english\careerlines_m.txt`
- `data\gamedata\pregame_chars.xml`

Confirmed follow-up files not yet parsed:
- none for the initial ability slice; the three extracted BIN exports are now parsed

### Current Gaps

The next major gaps are no longer file ingestion gaps. They are relationship and documentation depth gaps:

1. The conflict report is broad, but it is not yet grouped into dedicated per-domain output files.
2. The GUI now decodes the primary ability enum fields, but many raw ext-data values and some unknown bytes still need named semantics.
3. The current slice is ability-centric; race/career identity collision reports still need their own dedicated pass.
4. The current GUI explains likely ability flow from effect links, timings, components, and inferred requirement rows, but it does not yet visualize the node/edge graph directly.
5. The field-domain explorer now shows known raw-value options with provenance, but many scalar fields still need deeper semantic decoding before they can become strong emulator-side enums.
6. A first-pass component schema engine now uses extracted client text tokens like `COM_0_DURA_SECONDS` and `COM_0_VAL0_DAMAGE` to classify component fields by operation, but ext-data still has large `Unknown` coverage.
7. The token dictionary now writes plain-English glossary pages, but complex nested tokens like `VAL0_COM_0_VAL0_DAMAGE` still need hand-reviewed decomposition to read naturally.
8. Requirement linkage now exists, but it is intentionally narrow and currently limited to the inferred `ExtData[*].Val6 -> RequirementId` rule.
9. Coverage reporting now exists, but the current status buckets are still coarse and should eventually become domain-specific.
10. The token dictionary now includes ability names, source-text excerpts, and extracted-text context tags such as `Knockback`, `Knockdown`, `Immunity`, and `CrowdControl`.
11. A new operation-schema report now groups `abilitycomponentexport.bin` rows by operation, records recurring non-zero fields, carries semantic summaries from extracted client evidence, and exposes sample abilities in both the GUI and generated docs.
12. Requirement rows are no longer only raw blobs: the new ledger shows which abilities, components, and parent requirement rows reference each row, which child requirements it points at, and which ext-data fields are active, but most individual requirement-field meanings are still unresolved.

### Next Recovery Step

Resume from the working file-backed implementation and expand the source coverage.

The preferred recovery path is:

1. keep the tool read-only
2. preserve exact provenance capture for every new source family
3. continue the component schema pass by operation, especially `CC`, `KNOCKBACK`, `IMMUNITY`, and `APPLY_ABILITY`
4. deepen operation-by-operation field decoding, especially ext-data semantics for `CC`, `KNOCKBACK`, `IMMUNITY`, and `APPLY_ABILITY`
5. deepen requirement-field decoding inside the new requirement ledger so individual ext-data fields move from structural summaries toward named semantics
6. refine coverage classification so maintainers can separate client-contract gaps from text-only gaps and requirement-only gaps
7. expand domain-ledger reporting for `RaceId`, `CareerId`, `AbilityId`, and `EffectId`
8. add direct graph visualization and richer domain filters to the GUI once the reporting surface is broader
9. continue generating documentation with source file, line, and byte-offset citations

### Working Validation Commands

Build:

```powershell
$env:DOTNET_CLI_HOME=(Resolve-Path '.dotnet_home').Path
dotnet msbuild ClientDataMatrix\ClientDataMatrix.csproj /t:Build /p:Configuration=Debug /p:Platform=x64 /v:m
```

Ability report:

```powershell
Start-Process -FilePath (Resolve-Path '.\bin\Debug\ClientDataMatrix.exe') -ArgumentList 'doctor','ability','1','--root','C:\Users\Admin\Pictures\WAR_extracted','--output','tmp-data-matrix-bin' -Wait -NoNewWindow
```

Conflict report:

```powershell
Start-Process -FilePath (Resolve-Path '.\bin\Debug\ClientDataMatrix.exe') -ArgumentList 'report','conflicts','--root','C:\Users\Admin\Pictures\WAR_extracted','--output','tmp-data-matrix-bin' -Wait -NoNewWindow
```

Coverage report:

```powershell
Start-Process -FilePath (Resolve-Path '.\bin\Debug\ClientDataMatrix.exe') -ArgumentList 'report','coverage','--root','C:\Users\Admin\Pictures\WAR_extracted','--output','docs\data-matrix' -Wait -NoNewWindow
```

Requirement ledger:

```powershell
Start-Process -FilePath (Resolve-Path '.\bin\Debug\ClientDataMatrix.exe') -ArgumentList 'report','requirements','--root','C:\Users\Admin\Pictures\WAR_extracted','--output','tmp-data-matrix-requirements' -Wait -NoNewWindow
```

Operation schema report:

```powershell
Start-Process -FilePath (Resolve-Path '.\bin\Debug\ClientDataMatrix.exe') -ArgumentList 'report','operations','--root','C:\Users\Admin\Pictures\WAR_extracted','--output','tmp-data-matrix-operations' -Wait -NoNewWindow
```

### Current Result Snapshot

The current tool produces:

- per-ability Markdown, JSON, DOT, node CSV, edge CSV, and claim CSV
- a full conflict ledger across client CSV, TXT, XML, and BIN-backed claims
- exact source path, line number, and byte-offset provenance on claims
- ability reports that show the client ability row, linked effect chain, BIN ability row, related BIN components, and localized component text in one place
- in-GUI field-domain exploration so maintainers can double-click a decoded field and inspect every known raw value, meaning, and source provenance for that variable domain
- confidence-tiered component field semantics using `Confirmed`, `Inferred`, `Unknown`, and `Londo`
- a last-resort override ledger at `ClientDataMatrix\Configuration\component-schema-overrides.tsv`
- a generated COM token glossary at `docs\data-matrix\reference\com-token-dictionary.md`
- COM token evidence rows that now carry ability name, text excerpt, and context tags from surrounding client strings
- a requirement ledger at `docs\data-matrix\reference\requirement-ledger.md` with per-requirement usage counts, direct child links, raw ext-data rows, and active field summaries
- an operation schema ledger at `docs\data-matrix\reference\component-operation-schemas.md` with per-operation field summaries and sample abilities
- shared-field semantic fallback for `Duration`, `Interval`, and `Radius` when an operation has no direct token evidence for those stable fields
- inferred requirement-link sections in per-ability reports, including linked `abilityrequirementexport.bin` rows and recursive requirement dependencies
- inline requirement-id hints in operation-schema rows when `ExtData[*].Val6` values exactly match known requirement rows
- a coverage ledger at `docs\data-matrix\coverage\ability-coverage.md` so the extracted ability set can be sorted by readiness

Example verified output:

- `tmp-data-matrix-bin\ability\1.md`
- `tmp-data-matrix-bin\ability\7538.md`
- `tmp-data-matrix-requirements-validation\reference\requirement-ledger.md`

Known remaining limitation:

- many ext-data fields still do not have extracted-client semantics and therefore remain `Unknown`
- operation-level schema reporting exists now, but the deeper meaning of many ext-data slots still needs operation-specific decoding
- nested COM token expressions still need clearer human decomposition beyond the first-pass plain-English rendering
- requirement rows are linked and summarized now, but most individual requirement-field meanings still lack plain-English decoding
- coverage classification is useful now, but still needs finer-grained status buckets
