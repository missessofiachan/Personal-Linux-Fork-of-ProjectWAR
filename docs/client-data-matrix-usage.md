# ClientDataMatrix Usage

`ClientDataMatrix` supports both a GUI workflow and the original command-line workflow.

## GUI Launch

Launch the GUI by running the executable without arguments, or by double-clicking the executable in Explorer:

```powershell
.\bin\Debug\ClientDataMatrix.exe
```

You can also launch the GUI explicitly:

```powershell
.\bin\Debug\ClientDataMatrix.exe gui
```

Default roots:

- extracted root: `C:\Users\Admin\Pictures\WAR_extracted`
- output root: `docs\data-matrix`

## GUI Workflow

1. Confirm the extracted root points at the WAR client extraction tree.
2. Confirm or change the output root where generated docs should be written.
3. Click `Reload Data`.
4. Use `Generate All` on the main page when you want to build every shared ledger in one pass. If the current ability id box contains a valid value, it also generates that ability report.
5. Use `Ability Doctor` to:
   - search the catalog by ID or name
   - select an ability
   - generate the report
   - inspect the generated ability tree in-app
   - read the `What Happens` narrative tab for the inferred ability flow
   - open the `Definitions` tab to decode numeric fields like `CareerLine`, `TargetType`, `AbilityType`, `AttackType`, trigger values, and component operations
   - inspect inferred requirement links and linked `abilityrequirementexport.bin` rows when the tool finds `ExtData[*].Val6` values that match known `RequirementId` rows
   - double-click any definition row to open the field-domain explorer and see every known raw value, its plain-English meaning, and the source file or offset where that mapping came from
   - open the markdown file or output folder
6. Use `Conflict Ledger` to:
   - generate the full conflict ledger
   - browse conflict counts grouped by domain
   - inspect the first 500 conflicts for the selected domain
   - open the full markdown ledger on disk
7. Use `Token Dictionary` to:
   - generate the COM token glossary from extracted client strings
   - review plain-English definitions for tokens like `COM_0_DURA_SECONDS`, `COM_0_RADI_FEET`, and `COM_0_VAL0_DAMAGE`
   - see whether each definition is `Confirmed`, `Inferred`, `Unknown`, or `Londo`
   - inspect context tags like `Knockback`, `Knockdown`, `Immunity`, and `CrowdControl` that were inferred from the surrounding client text
   - open the generated markdown glossary on disk
8. Use `Coverage` to:
   - generate a whole-dataset ledger of ability readiness
   - browse which abilities are `MappedWithRequirements`, `Mapped`, `Partial`, or `StringsOnly`
   - see exactly which extracted-client pieces are missing for each ability, such as BIN rows, effect rows, localized text, or component rows
   - open the generated markdown ledger on disk
9. Use `Requirements` to:
   - generate the requirement ledger from `abilityrequirementexport.bin`
   - browse each `RequirementId` row in a summary grid with direct-ability counts, component counts, parent/child requirement links, and context tags
   - inspect the raw exported ext-data rows for the selected requirement
   - inspect active ext-data fields and see when `ExtData[*].Val6` behaves like a nested `RequirementId` pointer under the current narrow rule
   - inspect inbound and outbound links for the selected requirement, including which abilities or components pointed at it
   - open the generated markdown ledger on disk
10. Use `Operation Schemas` to:
   - generate an operation-family ledger from extracted client BIN rows and client string evidence
   - inspect which fields are non-zero for each component operation
   - see recurring sample values, COM-token renderings, semantic summaries, and confidence levels for those fields
   - inherit shared evidence for stable fields like `Duration`, `Interval`, and `Radius` when an operation lacks its own direct token rows
   - spot inline requirement-reference hints when `ExtData[*].Val6` values exactly match known `abilityrequirementexport.bin` rows
   - browse sample abilities that use the selected operation, including trigger text and client-text excerpts
   - focus first on priority operations such as `CC`, `APPLY_ABILITY`, `KNOCKBACK`, and `IMMUNITY`
11. Use `Source Status` to review file load success, row counts, and parse failures.
12. Use `Log` to keep a timestamped execution trail for the current session.

## Definition Explorer

The `Definitions` tab is now a field-domain browser, not just a static decode list.

When you double-click a definition row, `ClientDataMatrix` opens a modal explorer that shows:

- the selected field path
- the current raw value and decoded meaning
- the current confidence level: `Confirmed`, `Inferred`, `Unknown`, or `Londo`
- where the current raw value came from in the extracted client files
- the domain description for that field
- every other known raw value for the same field domain
- the plain-English meaning for each raw value
- provenance for each mapping, including file name and line or byte offset where available

This is intended to make it easier to lift client-facing variable domains into emulator code later without inventing ad hoc meanings.

Requirement links now show up there too. When the tool detects that an extracted BIN field points at a known `RequirementId`, the selected raw value is decoded through the requirement ledger instead of being left as an unexplained integer, so the explorer can show direct usage counts, context tags, and nested requirement chains.

## COM Token Dictionary

The COM token dictionary now does more than list token names.

It records:

- the plain-English meaning of the token
- the ability-local component slot being referenced
- nested COM-token decomposition when one token is rendered using another token's format
- the ability name and source text excerpt that demonstrated the token
- context tags inferred from the client text, such as `Knockback`, `Knockdown`, `Immunity`, `CrowdControl`, `Root`, `Snare`, and related combat-state terms

This makes the token glossary a real evidence ledger instead of a bare list of placeholders.

## Confidence Levels

`ClientDataMatrix` now distinguishes between evidence strengths:

- `Confirmed`: direct extracted-client evidence exists, usually from client strings or explicit extracted rows
- `Inferred`: the tool is aggregating a pattern from extracted client evidence, but the mapping is not directly named in one source row
- `Unknown`: the raw field is present, but the extracted client data does not yet explain what it means
- `Londo`: last-resort information from `WAR-RE-Toolkit` reference material; these rows must be treated cautiously and are intentionally segregated

## Requirement Link Rule

Current requirement linkage is deliberately narrow:

- if an extracted `ExtData[*].Val6` value exactly matches a known row in `abilityrequirementexport.bin`, `ClientDataMatrix` records an inferred requirement reference
- linked requirement rows are then expanded recursively if those requirement rows themselves point at more `RequirementId` values through the same field
- this is currently documented as `Inferred`, not `Confirmed`, because the client files expose the referential pattern but do not explicitly name the field as a requirement pointer

## Component Schema Overrides

The last-resort override ledger lives at:

- `ClientDataMatrix\Configuration\component-schema-overrides.tsv`

This file is optional and starts empty. It should only be used when extracted-client evidence is absent. If you add a row sourced from toolkit SQL, decompile, or packet material, mark it with `Confidence=Londo`.

## CLI Usage

The CLI commands still work, although this is now a Windows GUI executable and console capture can be less predictable from automation:

```powershell
.\bin\Debug\ClientDataMatrix.exe doctor ability 1
```

```powershell
.\bin\Debug\ClientDataMatrix.exe export graph ability 1
```

```powershell
.\bin\Debug\ClientDataMatrix.exe report conflicts
```

```powershell
.\bin\Debug\ClientDataMatrix.exe report coverage
```

```powershell
.\bin\Debug\ClientDataMatrix.exe report requirements
```

```powershell
.\bin\Debug\ClientDataMatrix.exe report tokens
```

```powershell
.\bin\Debug\ClientDataMatrix.exe report operations
```

If you want PowerShell to wait reliably for a CLI run and keep the printed status lines, use `Start-Process -Wait`:

```powershell
Start-Process -FilePath (Resolve-Path '.\bin\Debug\ClientDataMatrix.exe') -ArgumentList 'report','operations','--root','C:\Users\Admin\Pictures\WAR_extracted','--output','docs\data-matrix' -Wait -NoNewWindow
```

Optional overrides:

- `--root <path>` to point at a different extracted client tree
- `--output <path>` to change the generated-doc output root

## Output Files

Generated ability reports are written under:

- `docs\data-matrix\ability\`

Generated conflict ledgers are written under:

- `docs\data-matrix\conflicts\`

Generated coverage ledgers are written under:

- `docs\data-matrix\coverage\`

Generated token dictionaries are written under:

- `docs\data-matrix\reference\`

Generated requirement ledgers are also written under:

- `docs\data-matrix\reference\`

Generated operation schema ledgers are also written under:

- `docs\data-matrix\reference\`

Primary files:

- `<abilityId>.md`
- `<abilityId>.json`
- `<abilityId>.dot`
- `<abilityId>.nodes.csv`
- `<abilityId>.edges.csv`
- `<abilityId>.claims.csv`
- `client-conflicts.md`
- `client-conflicts.json`
- `client-conflicts.claims.csv`
- `ability-coverage.md`
- `ability-coverage.json`
- `ability-coverage.csv`
- `requirement-ledger.md`
- `requirement-ledger.json`
- `requirement-ledger.csv`
- `requirement-ledger.fields.csv`
- `requirement-ledger.references.csv`
- `requirement-ledger.rows.csv`
- `com-token-dictionary.md`
- `com-token-dictionary.json`
- `com-token-dictionary.csv`
- `component-operation-schemas.md`
- `component-operation-schemas.json`
- `component-operation-schemas.csv`
- `component-operation-schemas.fields.csv`
- `component-operation-schemas.abilities.csv`
