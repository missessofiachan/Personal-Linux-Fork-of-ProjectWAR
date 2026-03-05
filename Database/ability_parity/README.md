# Ability Parity Workstream

This folder contains tooling for rebuilding the emulator ability system toward Mythic data parity.

## Phase 1 Goal

Establish a repeatable baseline that compares ability IDs and basic wiring coverage across:

- Client export (`abilities.csv`)
- Londo SQL export (`War_Ability.sql`)
- Emulator world snapshot (`Database/Database.7z` -> `war_world.sql`)

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
