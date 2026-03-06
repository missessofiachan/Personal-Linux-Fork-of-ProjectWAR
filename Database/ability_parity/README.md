# Ability Parity Workstream

This folder contains tooling for rebuilding the emulator ability system toward Mythic data parity.

## Current Checkpoint

- Status snapshot: `Database/ability_parity/STATUS_2026-03-06.md`

## Phase 1 Goal

Establish a repeatable baseline that compares ability IDs and basic wiring coverage across:

- Client export (`abilities.csv`)
- Londo SQL export (`War_Ability.sql`)
- Emulator world snapshot (`Database/Database.7z` -> `war_world.sql`)

## Naming Conventions

This workstream follows a strict naming priority:

1. Mythic CSV semantics first.
2. Londo SQL semantics when CSV naming is ambiguous.
3. Legacy emulator naming only for compatibility shims.

Reference:

- `Database/ability_parity/NAMING_CONVENTIONS.md`

## Run

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Database\ability_parity\Generate-AbilityParityReport.ps1
```

## Outputs

- `Database/ability_parity/reports/ability_parity_<timestamp>.md`
- `Database/ability_parity/reports/ability_parity_<timestamp>.json`

The report surfaces:

- Source overlap and missing IDs
- World abilities with no `ability_commands`
- Hardcoded `AbilityMgr` entry constants that are not present in world DB

## Next Phase

Use the baseline report to drive resolver work:

1. Replace pseudo/fallback ability entry usage with canonical data-mapped entries.
2. Add importer/mapper for component and requirement semantics (client + Londo -> emulator runtime).
3. Validate with targeted ability parity tests by career and tactic tier.

## Phase 2: CSV Truth Sync + Canonical Resolver Build

Workflow:

1. Sync `war_world_curated.abilities` name/icon/effect metadata from client CSV.
2. Generate resolver rows from CSV effect linkage for entries that have no local command rows.

Scripts:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Database\ability_parity\Sync-AbilitiesFromCsv.ps1 -Apply
powershell -NoProfile -ExecutionPolicy Bypass -File Database\ability_parity\Build-AbilityEntryResolver.ps1 -Apply
```

Output SQL:

- `Database/ability_parity/work/abilities_csv_sync.generated.sql`
- `Database/ability_parity/work/ability_entry_resolver.generated.sql`

Runtime wiring:

- `ability_entry_resolver` DB table is loaded by `AbilityMgr`.
- Heuristic alias guessing is disabled in favor of explicit resolver rows.

## Phase 3: Mythic CSV Staging Import

Goal:

- Keep a versioned copy of ability-adjacent client CSVs in-repo.
- Import them into isolated `mythic_csv_*` tables (no overlap with legacy world tables).
- Use these tables as the authoritative link graph for command/buff fallback resolution.

Script:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Database\ability_parity\Sync-MythicAbilityCsvBundle.ps1 -Apply
```

Testing database target:

- Default script target is `war_world_curated` (`-WorldSchema` default).
- If you are running local WorldServer with `World.xml` pointing at `war_world`, run with:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Database\ability_parity\Sync-MythicAbilityCsvBundle.ps1 -Apply -WorldSchema war_world
```

Imported source files (repo):

- `Database/ability_parity/csv/gamedata/*.csv`

DB tables:

- `mythic_csv_dataset_meta`
- `mythic_csv_raw_rows`
- `mythic_csv_abilities`
- `mythic_csv_effects`

Runtime wiring:

- `AbilityMgr` loads `mythic_csv_abilities` and `mythic_csv_effects`.
- Ability and buff command resolution follows Mythic effect-link candidates when direct local command rows are missing.

## Phase 4: Ability + Item Action Coverage Snapshot

Goal:

- Mirror legacy gameplay tables into isolated `mythic_src_*` copies without losing columns.
- Expand string-packed item actions (`Stats`, `Effects`, `Crafts`, `SellRequiredItems`, set strings) into normalized action rows.
- Produce a single action index for quick coverage diffs before runtime rewiring.

Script:

```sql
USE war_world_curated;
SOURCE Database/ability_parity/mythic_action_coverage.snapshot.sql;
```

Created tables:

- Mirror tables: `mythic_src_abilities`, `mythic_src_ability_commands`, `mythic_src_buff_infos`, `mythic_src_buff_commands`, `mythic_src_ability_damage_heals`, `mythic_src_ability_knockback_info`, `mythic_src_ability_modifiers`, `mythic_src_ability_modifier_checks`, `mythic_src_item_infos`, `mythic_src_item_sets`
- Normalized item actions: `mythic_item_ability_actions`, `mythic_item_stat_actions`, `mythic_item_effect_actions`, `mythic_item_craft_actions`, `mythic_item_sell_requirement_actions`, `mythic_itemset_item_actions`, `mythic_itemset_bonus_actions`
- Coverage index/meta: `mythic_action_index`, `mythic_action_snapshot_meta`

Runtime toggle:

- In `World.xml`, set `<UseMythicActionCoverageTables>true</UseMythicActionCoverageTables>` to load abilities/items from `mythic_src_*` tables.
- Default is `false` (legacy `abilities`, `item_infos`, etc.).

## Phase 5: Londo Graph Import to `mythic_bin_*`

Goal:

- Import Londo/Mythic normalized ability graph tables from `WAR-RE-Toolkit`.
- Copy those rows into isolated `mythic_bin_*` tables inside world DB (no overwrite of legacy tables).
- Feed runtime graph translation with a stable, versioned source.

Script:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Database\ability_parity\Sync-LondoAbilityGraph.ps1 -Apply
```

Generated SQL:

- `Database/ability_parity/work/mythic_bin_sync.generated.sql`

Created/filled tables:

- `mythic_bin_ability`
- `mythic_bin_abilitybin`
- `mythic_bin_abilitycomponentbin`
- `mythic_bin_abilitycomponentlink`
- `mythic_bin_abilityexpression`
- `mythic_bin_abilityrequirmentbin`
- `mythic_bin_abilityupgradebin`
- `mythic_bin_abilityupgradeentry`
- `mythic_bin_itemability`

Runtime toggles:

- `<UseMythicAbilityGraphTables>true</UseMythicAbilityGraphTables>`
- `<MythicAbilityGraphOverrideExistingCommands>false</MythicAbilityGraphOverrideExistingCommands>` to only fill gaps.
- `<MythicAbilityGraphOverrideExistingCommands>true</MythicAbilityGraphOverrideExistingCommands>` to replace existing command rows per translated entry.
