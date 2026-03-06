# Mythic / ProjectWAR / Londo Ability System Comparison

## Scope

This document compares three ability-system models currently relevant to this repository:

1. Mythic client-data model (CSV truth and linked datasets).
2. Current ProjectWAR emulator runtime model.
3. Londo SQL model from `WAR-RE-Toolkit`.

It focuses on architecture and behavior wiring, not balance tuning.

## 1) Mythic CSV-Based Ability System (Client Truth Model)

### 1.1 Core data chain

Mythic ability data is ID-linked across datasets, not defined as one flat command row list:

1. `abilities.csv` provides core ability identity and references (ability id, icon/animation, linked effect id).
2. `effects.csv` provides effect graph references (build-up, activate, cast, projectile, impact, aoe, channel, vfx refs).
3. Related effect datasets (`effectdef.csv`, `effectlists.csv`, `effectmisc.csv`, `effectnifs.csv`, `effectproj.csv`, `effectvfx.csv`, etc.) define visual/effect assets and references.
4. Additional datasets (`abilityline_to_bufftype.csv`, `flageffects.csv`, `stateeffects.csv`, `stepeffects.csv`, `weaponeffects.csv`, `petcommanddata.csv`) provide cross-system behavior inputs.

In this repository, `Database/ability_parity/Sync-MythicAbilityCsvBundle.ps1` stages these CSVs into:

- `mythic_csv_dataset_meta`
- `mythic_csv_raw_rows`
- `mythic_csv_abilities`
- `mythic_csv_effects`

Important: only `abilities` and `effects` are currently typed for runtime use. Everything else is preserved as raw row JSON.

### 1.2 What this means for execution semantics

A Mythic-style ability is graph-driven:

- ability id points to effect id
- effect id points to additional effect stages/definitions
- execution interpretation depends on linked data (and, in reconstructed server models, component/requirement bins)

So the client-truth model is relational/link-graph first, not "one ability row + one command delegate list."

## 2) Current ProjectWAR Ability System

### 2.1 Data model used by runtime

Runtime loads legacy world tables:

- `abilities`
- `ability_commands`
- `ability_damage_heals`
- `buff_infos`
- `buff_commands`
- `ability_modifiers`
- `ability_modifier_checks`
- `ability_knockback_info`

Loaded in `WorldServer/World/Abilities/AbilityMgr.cs`.

`DBAbilityCommandInfo` / `DBBuffCommandInfo` use string command names (`CommandName`) plus scalar values (`PrimaryValue`, `SecondaryValue`, etc.).

### 2.2 Runtime execution model

Current execution is command-delegate driven:

1. Ability row is loaded into `AbilityInfo` + `AbilityConstants`.
2. Command rows are assembled into command chains using `CommandID` + `CommandSequence`.
3. `AbilityEffectInvoker` and `BuffEffectInvoker` dispatch by `CommandName` string into C# handlers.

This is server-implementation-centric. The server behavior is whatever those delegates implement.

### 2.3 Mythic parity extensions already present

Current branch adds parity helpers, but not a full Mythic runtime:

- Toggle `UseMythicActionCoverageTables` switches loaders to `mythic_src_*` mirror tables.
  - `AbilityMgr`: mythic source ability/buff tables.
  - `ItemService`: `mythic_src_item_infos` / `mythic_src_item_sets`.
- `ability_entry_resolver` provides canonical alias mapping.
- `mythic_csv_abilities` + `mythic_csv_effects` are loaded to build linked fallback candidates.
  - resolver and linked BFS fallback help command/buff lookups when direct rows are missing.

This improves coverage and fallback resolution, but still executes through ProjectWAR command delegates.

### 2.4 Practical limitation

Because execution remains delegate/command-row based, this system cannot be 1:1 Mythic by data shape alone. It needs either:

1. a translator from Mythic component/requirement semantics into command delegates, or
2. a native component/requirement executor.

## 3) Londo Ability System (Toolkit SQL Model)

Source directory:

- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2`

### 3.1 Ability data architecture

Londo model is normalized and component-centric:

- `Ability` (rich ability metadata, including `MythicComponentData`, `ComponentData`, `RequirmentsData`, labels/flags fields).
- `AbilityBin`
- `AbilityComponentBin`
- `AbilityComponentXComponent`
- `AbilityExpression`
- `AbilityRequirmentBin`
- `AbilityUpgradeBin`
- `AbilityUpgradeEntry`
- `AbilityXGroup`, `AbilityXLabel`, `AbilityLabel`, `AbilityLine`

This model stores component definitions and links explicitly, including requirement/expression relationships.

### 3.2 Item-to-ability integration

Londo item model separates action types into dedicated tables:

- `ItemAbility` (item -> ability activation)
- `ItemBuff`
- `ItemStatistic`
- `ItemCraft`
- `ItemSetBonus`

This is cleaner than string-packed action fields in emulator `item_infos`.

### 3.3 Why it is closer to Mythic behavior construction

Londo separates:

- ability identity
- component payload
- component linking
- requirements/expressions
- upgrade vectors

That makes behavior derivation data-driven instead of delegate-name driven.

## 4) Direct Side-by-Side Summary

### Mythic CSV model

- Strength: canonical client linkage truth.
- Weakness: by itself, not a server runtime engine.
- Current repo status: staged and partially typed for resolver/fallback.

### Current ProjectWAR model

- Strength: battle-tested runtime with explicit C# handlers.
- Weakness: pseudo/manual mappings can diverge from client semantics.
- Current repo status: production runtime, plus alias/link fallback helpers.

### Londo model

- Strength: richer data model for real ability reconstruction, close to Mythic structure.
- Weakness: requires runtime integration/adaptation to execute directly in ProjectWAR.
- Current repo status: available in toolkit SQL, not yet native runtime format in this repo.

## 5) Key Gaps Between Current ProjectWAR and Mythic/Londo Semantics

1. Command delegate name dispatch is not equivalent to component/requirement execution.
2. Current Mythic CSV import only types `abilities` and `effects`; many linked datasets are not interpreted at runtime.
3. `mythic_src_*` tables are mirror copies of legacy tables (coverage isolation), not reconstructed Mythic behavior tables.
4. Item actions are still primarily string-packed in emulator schema, unlike Londo split tables.

## 6) Recommended Interpretation of "1:1 Mythic"

To reach true 1:1 behavior parity:

1. Treat client IDs/linkage as authoritative (already started via `mythic_csv_*` and resolver).
2. Adopt component/requirement data model (Londo-style tables or equivalent imported structures).
3. Build deterministic server execution from component + expression + requirement data.
4. Keep current delegate system as compatibility fallback while parity coverage expands.

Without step 3, parity remains partial even if IDs and links are synced.

## 7) Repository References

- `WorldServer/World/Abilities/AbilityMgr.cs`
- `WorldServer/Services/World/ItemService.cs`
- `WorldServer/Configs/WorldConfigs.cs`
- `Database/ability_parity/Sync-MythicAbilityCsvBundle.ps1`
- `Database/ability_parity/mythic_csv.schema.sql`
- `Database/ability_parity/Build-AbilityEntryResolver.ps1`
- `Database/ability_parity/mythic_action_coverage.snapshot.sql`
- `Database/ability_parity/mythic_bin.schema.sql`
- `Database/ability_parity/Sync-LondoAbilityGraph.ps1`
- `Common/Database/World/Ability/MythicCsvAbility.cs`
- `Common/Database/World/Ability/MythicCsvEffect.cs`
- `Common/Database/World/Ability/AbilityEntryResolver.cs`
- `Common/Database/World/Ability/MythicSourceAbilityTables.cs`
- `Common/Database/World/Items/MythicSourceItemTables.cs`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_Ability.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_AbilityBin.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_AbilityComponentBin.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_AbilityComponentXComponent.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_AbilityExpression.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_AbilityRequirmentBin.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_AbilityUpgradeBin.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_AbilityUpgradeEntry.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_Item.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_ItemAbility.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_ItemBuff.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_ItemStatistic.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_ItemCraft.sql`
- `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_ItemSetBonus.sql`
