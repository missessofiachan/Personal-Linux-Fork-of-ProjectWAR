# Curated + selective master refresh

- Script: `2026_03_03_apply_curated_plus_selective_master.sql`
- Target schema: `war_world_curated`
- Sources:
  - `war_world_master_raw` (master base)
  - `war_world_sqlrepo_baseline` (stable baseline backfills)

## Current policy encoded by script
- Keep master behavior for ability/AI and core creature data tables.
- Backfill baseline data for known regressions:
  - `vendor_items`
  - `rallypoints`
  - `tok_bestary`
- Preserve baseline-only tables so information is not lost:
  - `entries`
  - `event_infos`
  - `event_spawns`
  - `item_bonus`
  - `scenario_gauntlet_spawn`

Run with:
`mysql --host=127.0.0.1 --port=3306 --user=root --password=password --database=war_world_curated --execute="source Database/curated_selective_master/2026_03_03_apply_curated_plus_selective_master.sql"`
