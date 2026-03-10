# AI Handoff Checkpoint

Date: `2026-03-10`

Branch: `ability-parity-mythic-v1`

## Scope

This checkpoint covers two parallel tracks that are both present in the working tree:

1. server hardening and cleanup work
2. ClientDataMatrix backlog, reporting, and discovery work for ability parity

The current user direction is discovery-first. Do not add more matrix program features unless the user changes direction again.

## What Changed

### Server/runtime

- TLS is now explicit opt-in in `FrameWork/NetWork/TCPManager.cs`.
- The TLS certificate password is no longer hardcoded.
- Client disconnect cleanup was tightened in `BaseClient.cs` and `GameClient.cs`.
- Early country IP block support was added in `FrameWork/NetWork/CountryBlockPolicy.cs` and wired into both listener paths.
- A null guard was added in `CombatHandlers.cs`.
- Several hot-path player collection consumers were switched to safer access/snapshot patterns.
- `BattlefieldObjective.cs` now reinitializes and starts the objective FSM before the open-battlefront event.

### ClientDataMatrix

- Added cleanup support for temp/output clutter.
- Added remaining-work cataloging and next-batch ranking.
- Added operation-field packet work packet reporting.
- Added CLI and WinForms surfaces for the new reporting/cleanup paths.
- Updated docs and generated a new set of matrix reports under `docs/data-matrix/`.

## Current High-Confidence Discovery State

### APPLY_ABILITY

- `Values[0]` is the applied child ability id.
- `Val1` behaves like a branch selector, not a direct scalar payload.
- `Val3` separates meaningful payload profiles:
  - `1`: general follow-up child ability profile
  - `4`: proc/reactive profile
  - `6`: control-overlay profile
- The main unresolved value-bearing fields are still `Val2` and `Val7`.

### CC

- `FlagsRaw` is the raw component flags field from `abilitycomponentexport.bin`.
- The decompile-backed bit meanings recovered so far include:
  - `1 Root`
  - `2 Melee`
  - `4 Ranged`
  - `8 Magic`
  - `32 AutoAttack`
  - `64 AllAbilities`
  - `128 DisableDefending`
  - `1024 Stunned`
  - `8192 SquigArmor`
- `Val2=2` is the dominant base CC payload family.
- `Val2=10` behaves like an indirection or wrapper form.
- `Val2=6` behaves like an inline or literal form.
- Higher composite values like `2175` and `2303` are still only partially decoded.

### Requirements

- Logic operator `8` = AND, `9` = OR, `10` = addition, `11` = subtraction.
- `9622`, `9625`, and `9269` are wrapper rows around child requirements, not primary semantic rows.
- `9100` should not be treated as a clean nested ability/requirement link.
- Best current model for unresolved ops:
  - `81`: boolean target or controller-class check
  - `85`: boolean entity-availability or existence check
  - `89`: signed state/index for that entity class
- These names are still provisional. The evidence is stronger than before, but not final.

### Conflicts

The top conflict families currently worth keeping in view are:

- `AbilityIdMirrorEffectId`
- `MountOverlayEffectId`
- `ZeroVsEffectIdGap`

Representative sample reports already generated:

- `docs/data-matrix/ability/10.md`
- `docs/data-matrix/ability/1052.md`
- `docs/data-matrix/ability/122.md`
- `docs/data-matrix/ability/1370.md`
- `docs/data-matrix/ability/183.md`
- `docs/data-matrix/ability/3702.md`
- `docs/data-matrix/ability/779.md`

## Most Useful Repo Reports

- `docs/data-matrix/overview/remaining-work.md`
- `docs/data-matrix/overview/remaining-work-next.md`
- `docs/data-matrix/overview/remaining-work-operation-fields.md`
- `docs/data-matrix/conflicts/client-conflicts.md`
- `docs/data-matrix/reference/requirement-ledger.md`
- `docs/data-matrix/reference/component-operation-schemas.md`

## External Evidence Already Checked

### WAR-RE-Toolkit

Path: `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit`

Useful files:

- `docs/research/abilities-bin-file-exporter/AbilityExport.cs`
- `docs/research/abilities-bin-file-exporter/AbilityEnums.cs`
- `docs/research/abilities-bin-file-exporter/txt/abilitynames.txt`
- `docs/research/abilities-bin-file-exporter/txt/abilitydesc.txt`

These resolved the `ExtData` field layout and part of the `CC` bit vocabulary.

### Full decompile

Path: `C:\Users\Admin\Pictures\WAR/ProjectWAR/war_decompiled.c`

Useful anchors:

- ability component export loader around line `189492`
- ability requirement export loader around line `190602`
- `AbilityComponentRequirementExpression` around line `199356`
- `AbilityRequirementExpression` around line `199577`
- requirement dispatch and logic combination around line `199867`

This is what confirmed logic operators `10` and `11`.

### Packet/logger material

Path: `C:\Users\Admin\Music\Warhammer\Warhammer`

Most useful items:

- `PacketLogger-master-master/PacketLogger/bin/Debug/packet_d28h11m43.txt`
- `PacketLogger-master-master/PacketLogger/PacketHandler.cs`
- `PacketLogger-master-master/PacketLogger/Crypto.cs`
- `WarhammerPacketTester-master`
- `Documents/Ability testing in process.docx`

The decoded packet log is useful but noisy. Treat it as supporting evidence, not ground truth without correlation.

## Best Next Discovery Work

Do these in order:

1. Mine `packet_d28h11m43.txt` for:
   - `F_USE_ABILITY`
   - `F_DO_ABILITY`
   - `F_KNOCKBACK`
   - `F_COMMAND_CONTROLLED`
   - `F_PET_INFO`
   - `F_ACTIVE_EFFECTS`
2. Correlate those packet traces with unresolved requirement ops `81`, `85`, and `89`.
3. Trace the real client `CC` implementation in `war_decompiled.c` to finish the remaining flag bits and break behavior.
4. Compare recovered client semantics against emulator handlers only after the client-side names are firmer.

## Important Caveats

- The current matrix reports are good at structural clustering, but some labels should not be trusted without checking raw text exports or decompiled code.
- Packet logs in the Warhammer folder contain both valid decoded packets and misaligned noise.
- The user explicitly said to stop making new matrix program changes for now and focus on discovery.

## Validation State Before Handoff

Before this checkpoint request, the following had already been successfully built during the working session:

- `ClientDataMatrix/ClientDataMatrix.csproj`
- `WorldServer/WorldServer.csproj`

If more code changes are made after this checkpoint, rebuild both again before handoff.
