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
- `Val2=9` with `Val3=4` and `Val6=100` is now a high-confidence proc/reactive branch.
- Within that proc branch, `Val7` behaves like the main proc scalar:
  - `5` maps cleanly to 5% proc families like `Hemorrage`, `Soul Barrier`, `Soul Haste`, and `Elixir of Tahoth`
  - `10` maps cleanly to 10% proc families like `Dissolve`
  - `25` maps cleanly to 25% proc families like `Rune Of Immolation`, `Spellbinding Rune`, and `;Rune Priest Spec Abilities`
- `Val7=250` is not a literal percentage. It clusters with AP-free or next-ability state families like `Judgement`, `Supplication`, `Bludgeon`, and mount/AP-free overlays.
- `Val4=9` is rare and concentrated in the control-overlay APPLY_ABILITY family; `Val4=8` is the dominant general family marker.

### CC

- `FlagsRaw` is the raw component flags field from `abilitycomponentexport.bin`.
- The decompile-backed bit meanings recovered so far include:
  - `1 Root`
  - `2 Melee`
  - `4 Ranged`
  - `8 Magic`
  - `16 Magic_1`
  - `32 AutoAttack`
  - `64 AllAbilities`
  - `128 DisableDefending`
  - `1024 Stunned`
  - `8192 SquigArmor`
- `FlagsRaw=8` maps cleanly to silence / magic-lock behavior.
- `FlagsRaw=1` maps cleanly to root behavior.
- `2175` and `2303` both share an unrecovered `2048` bit. `2303 = 2175 + 128`, so the shared high bit is more informative than the extra `DisableDefending` flag.
- `2303` is heavily contaminated by flourish / animation-only rows (`Auto2`, `greenskin RDPS flourish2`, `Dwarf Tank flourish1`, etc.), so it should not be treated as a clean gameplay-control family.
- `Val2=2` is the dominant base CC payload family.
- `Val2=10` behaves like an indirection or wrapper form.
- `Val2=6` behaves like an inline or literal form.
- Higher composite values like `2175` and `2303` are still only partially decoded.

### Requirements

- Logic operator `8` = AND, `9` = OR, `10` = addition, `11` = subtraction.
- `9622`, `9625`, and `9269` are wrapper rows around child requirements, not primary semantic rows.
- `9100` should not be treated as a clean nested ability/requirement link.
- In `9100`, the trailing recovered op is `16 = MonsterType`, so the `908` literal should be treated as a monster-type style literal, not as a nested requirement id.
- Best current model for unresolved ops:
  - `81`: boolean target or controller-class check
  - `85`: boolean entity-availability or existence check
  - `89`: signed state/index for that entity class
- These names are still provisional. The evidence is stronger than before, but not final.
- `89` now has stronger deployed-entity evidence than generic combat evidence:
  - summon-family rows `9356`, `9357`, `9358`
  - guild-standard/banner families under `9269`, `9270`, `9271`, `9282`, `9285`
  - siege-family row `9297`
- There are no current exported requirement rows using nearby recovered ops `79 = Mounted`, `80 = HasPet`, or `40 = SiegeControl`, which makes `81/85/89` more likely to be a separate controller/deployed-state family than mislabeled duplicates of those checks.

## Post-Checkpoint Discoveries

- Renown hidden-buff debugging produced a concrete emulator divergence:
  - `Player.SetMaxActionPoints()` was still carrying a legacy 2018 RR AP bonus path (`+25` above RR65, `+50` above RR75).
  - The newer RR70/RR80 hidden-buff system also grants AP through buff `22275`.
  - That means the emulator had two independent AP bonus paths active at once, which is the cleanest explanation for doubled AP.
- The RR70/RR80 hidden-buff path also had an idempotency hole:
  - `AbilityInterface.HasRenownSilentBuff()` only checked active buffs through `GetBuff(...)`.
  - It did not see pending buffs, queued buffs, or saved persistent-buff state.
  - That could allow silent renown buffs to be re-queued during load or renown-sync windows even when the character already had the buff logically present.
- The RR90/RR100 renown tactics should not trust raw `MinimumRenown` data alone.
  - The emulator was granting `27870/27871/27872` and `27873` according to the loaded constants, which is too permissive in current data.
  - The safe server-side rule is:
    - `27870/27871/27872` require RR90
    - `27873` requires RR100

- The old packet logger is not structurally reliable enough for exact semantics:
  - `PacketLogger-master-master/PacketLogger/Program.cs` computes packet length incorrectly.
  - Repeated and misaligned records in `packet_d28h11m43.txt` are consistent with that framing bug.
- The requirement evaluator shape is now clear:
  - `FUN_00557e13` in `war_decompiled.c` is a handler-table dispatch on requirement op id.
  - `AbilityComponentRequirementExpression`, `AbilityRequirementExpression`, and `AbilityFireExpression` all carry a shared evaluator context through `DAT_00f78288`.
  - The actual handler table initialization is still not recovered.
- The guild-standard family is one of the strongest anchors for the unresolved state ops:
  - `9273` is a shared state gate reused by banner/stat-standard families.
  - `9274`, `9275`, `9276`, `9281`, and `9284` are the paired child checks layered with that shared gate.
- Some sample-ability context in the generated requirement reports is less trustworthy than the raw requirement rows themselves.
  - Example: the checked-in reports currently associate requirement `9403` with `9258 (Funnel Essence)`, but the raw row for `9403` is a clean repeated `PrevCompActivated` pattern and does not support a broad semantic read from that label alone.
- The current best non-requirement breakthroughs are in `CC` and `APPLY_ABILITY`, not in the unresolved requirement ops:
  - `CC ExtData[1].Val2=6` is tightly associated with breakable-root behavior.
  - Direct examples: `1370 Get Movin'!`, `1519 Barbed Wire`, and `1681 Where You Going?` all explicitly say the root has a 50% chance to break on hit.
  - In `docs/data-matrix/ability/1370.md`, the root CC component uses `slot1=[2,6,1,8,0,1016,0,0,1]`, which is the cleanest current anchor for that family.
  - `CC ExtData[1].Val2=10` is not the same family. It clusters with secondary control overlays like `AE Root`, `Silence and Disarm`, `Lion's Roar`, `Incapacitate`, and `Sit Down!`.
- `1016` is now a useful cross-component anchor:
  - it appears in the breakable-root CC slot on `1370`
  - it also appears in the paired control-overlay APPLY_ABILITY slot on the same ability
  - it also appears in requirement rows like `9093`, which the matrix only sees in Root contexts
  - this is strong evidence that `1016` is a reused root-like control literal, not a nested requirement or child-ability id
- The `Val2=8 / Val3=6 / Val4=9` shape is now confirmed as a shared control-literal family, not a one-off:
  - it appears in APPLY_ABILITY control-overlay rows like the paired `1370` follow-up
  - it also appears repeatedly in requirement rows such as `9028`, `9047`, `9103`, `9142`, `9170`, `9214`, and `9409`
  - this is the best current bridge between control payload decoding and requirement-literal decoding
- `CC ExtData[0].Val2=15` and `=27` are both likely monster/NPC-heavy control families rather than core player-facing control branches.
  - `15` clusters with names like `Gasp of the Unliving`, `Tormenting Wail`, `Bat Screech`, `Sever Limb`, `Weak Limbs`, and `Ground Tremble`
  - `27` clusters with names like `Silence`, `Razor Web`, `Disarm`, `Root`, and `Paralyzing Filth`
  - both families have sparse or empty localized descriptions, which makes them weaker semantic anchors than the player-facing `2/6/10` families
- The shared requirement shape `Val2=8 / Val3=6 / Val4=9` is broader than pure CC-immunity gating.
  - `9170` uses that shape with the literal pair `1021/1022` and is tied to `Bolster` through component `1664 (BONUS_TYPE_ADJUST)`
  - this means the family is better described as a general state/control literal family, with some members clearly CC-related and others clearly state/balance related
- The immunity-buff family around `Resolute Defense` and `Inexorable Force` provides the strongest requirement-space anchor for known immunity literals:
  - `9596` is the parent row used by `Resolute Defense` and `Inexorable Force` IMMUNITY components
  - its child rows `9594` and `9595` carry `Val7=12` and `Val7=8`
  - those values line up exactly with the recovered toolkit immunity ids `Root = 12` and `CC = 8`
  - this is strong evidence that at least part of the requirement sublanguage embeds immunity-category literals directly, not just requirement ids or child-ability ids
- `9615` looks like a sibling immunity gate specific to `Inexorable Force`.
  - it uses `slot0=[1,8,6,8,0,1014,...]` and `slot1=[1,8,1,9,0,1324,...]`
  - the exact meanings of `1014` and `1324` are still unresolved, but the row is firmly in the `Immunity, Knockback, Knockdown, Root, Stagger` family
- `9488` is now the best sibling anchor for `9615`.
  - it uses the same shape as `9615` but swaps `1014` for `1015` while keeping the same trailing `1324`
  - because `1015` is already the recovered knockdown immunity/status literal, this strongly suggests `1014` is a neighboring control-state literal in the same domain
  - taken together, `9594/9595/9596` and `9488/9615` imply that the requirement sublanguage is mixing at least two literal domains:
    - immunity-category ids like `8` and `12`
    - control-state/status literals like `1014`, `1015`, and `1016`

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

## Latest Discovery Notes

This section reflects the latest raw-bin discovery pass after moving off the `81/85/89` loop.

- The best new evidence came from dumping `abilitycomponentexport.bin` directly using the layout in `ClientDataMatrix/Parsers/AbilityBinaryParser.cs`.
- `IMMUNITY Value[0]=12` is the dominant control-immunity bundle family.
  - Raw components `326/328/330/332/334/336` use `values=[12,0,6,...]`.
  - Raw components `463-474`, `475-480`, and `486-488` reuse the same payload with `values=[12,100,{1|3|6},...]`.
  - Those rows all share the exact same ext-data literal bundle:
    - `1030`
    - `1018`
    - `1011`
    - `1015`
    - plus trailing `slot4=[3,52,1,8,0,0,1,0,0]`
- Requirement row `9409` is now the strongest bridge between requirement space and raw IMMUNITY payloads.
  - Its literals are the same `1030 / 1018 / 1011 / 1015` bundle with the same trailing `op52` slot.
  - That means `9409` is not just another isolated requirement row; it mirrors the live hidden IMMUNITY bundle structure directly.
- `IMMUNITY Value[0]=24` is a separate knockback-side family.
  - Pure form: `358`, `17995`, `27833` with no ext-data.
  - Coupled form: `496`, `17982`, `27830` with `ext[0]=[7,8,6,8,0,1014,0,0,0]`.
- `IMMUNITY Value[0]=46` is a distinct adjacent family, not just noise.
  - Components `497`, `17983`, and `27831` all use the same `ext[0]=[7,8,6,8,0,1014,0,0,0]` shape as the `24` family.
  - This is the best current proof that `46` is a real immunity/status category that still lacks a recovered enum name.
- `1014`, `1015`, `1016`, `1018`, and `1030` also exist as direct standalone `IMMUNITY Value[0]` rows, not just ext-data literals.
  - Direct singleton rows:
    - `24942 -> 1014`
    - `24943 -> 1015`
    - `24944 -> 1016`
    - `24945 -> 1018`
    - `24947 -> 1030`
  - Short flagged forms:
    - `12858 -> values=[1030,0,0,...]`, `flags=1`, `ext[0]=[2,1,1,8,0,0,1,0,0]`
    - `12859 -> values=[1015,0,0,...]`, `flags=1`, `ext[0]=[2,1,1,8,0,0,1,0,0]`
- `1016` is now a firm root-side anchor.
  - `436` uses `values=[12,100,0,...]` with `ext[0]=[7,8,6,8,0,1016,0,0,0]`.
  - `28305` (`Inexorable Force`) also carries `1016` in its IMMUNITY payload and pairs it with requirement `9596`.
  - This is consistent with the earlier `CC` and requirement evidence tying `1016` to root-like control.
- `1014` is not a root-side literal.
  - It repeatedly appears with the `24/46` IMMUNITY family in `496/497`, `17982/17983`, and `27830/27831`.
  - It also appears in requirement `9615`.
  - Best current model: `1014` is in the knockback/stagger-side control-literal domain, but the exact retail name is still unresolved.
- `Inexorable Force` IMMUNITY rows are now structurally decoded from raw components:
  - `28305`: `values=[12,100,0,...]`, `ext[1].Val6=1016`, `ext[2].Val6=9596`
  - `28306`: `values=[12,100,5,...]`, ext literals `1044 / 1015 / 1030`, plus `9596`
  - `28307`: `values=[12,100,6,...]`, same ext-data as `28306`
  - `28308`: `values=[24,100,0,...]`, `ext[0].Val6=9615`, `ext[1].Val6=9596`
  - This is the strongest current evidence that:
    - `9596` is the shared control-immunity gate used across the `Inexorable Force` bundle
    - `9615` is a narrower knockback-side companion gate
    - `1044` is a real control literal adjacent to `1015` and `1030`, not a random scalar
- There is a reliability issue in the generated ability-name summaries around the low `398-408` internal immunity abilities.
  - Localized `abilitynames.txt` says:
    - `398 = Knockback Immunity`
    - `400 = Root Ward`
    - `401 = Root Ward Counters`
    - `402-407 = Unstoppable`
    - `408 = Immovable`
  - `component-operation-schemas.abilities.csv` labels some of that cluster differently.
  - For this low-level internal-buff cluster, prefer the localized `abilitynames.txt` labels over the generated summary labels.

## Best Next Discovery Work

Do these in order:

1. Trace the real client `IMMUNITY` handling in `war_decompiled.c` before going back to the `81/85/89` family.
2. Use the raw `9409`, `9596`, and `9615` anchors to name the remaining IMMUNITY/control literals, especially `46`, `1011`, `1014`, `1018`, and `1044`.
3. Trace the real client `CC` implementation in `war_decompiled.c` to finish the remaining flag bits and the `Val2=6 / 10 / 15 / 27` payload families.
4. Trace the real `APPLY_ABILITY` handling in `war_decompiled.c` to name the proc/reactive branch fields, especially `Val2`, `Val7`, and the rare `Val4=9` control-overlay family.
5. Re-check any requirement rows or generated ability-name labels whose checked-in sample context looks suspicious before using those labels as semantic evidence.
6. Compare recovered client semantics against emulator handlers only after the client-side names are firmer.

## Missing Knowledge

The main unknowns still blocking clean naming are:

- the exact human-readable names for requirement ops `81`, `85`, and `89`
- the specific data domain behind `89`
  - deployed-banner state
  - controlled entity state
  - siege/deploy controller state
  - or a shared state/index abstraction used by all three
- whether `81` is a pure class gate, a controller-presence gate, or a target/controller mode gate
- whether `85` means entity exists, entity is available, or entity is already deployed/owned
- the remaining high `CC.FlagsRaw` bit meanings beyond the recovered subset
- the exact symbolic meaning of the IMMUNITY `Value[0]=46` family
- the exact retail names for control literals `1011`, `1014`, `1018`, and `1044`
- the exact client-side enum or symbolic names for several requirement literals still only known structurally
- which checked-in matrix sample-ability links are fully trustworthy and which are artifacts of narrow `Val6 -> RequirementId` inference without enough semantic filtering

## Important Caveats

- The current matrix reports are good at structural clustering, but some labels should not be trusted without checking raw text exports or decompiled code.
- Packet logs in the Warhammer folder contain both valid decoded packets and misaligned noise.
- The old packet logger contains a packet-length framing bug, so packet output from it should not be treated as authoritative without correlation.
- The user later reopened matrix-program changes, so the current codebase now includes the focused control-literal crosswalk report described below.

## Matrix Program Additions After This Checkpoint

- Added a focused remaining-work artifact for shared control/immunity literals:
  - `overview/remaining-work-control-literals.md`
  - `overview/remaining-work-control-literals.json`
  - `overview/remaining-work-control-literals.csv`
  - `overview/remaining-work-control-literals.sources.csv`
- The report is built from live BIN data, not from scraping generated markdown.
- Current source coverage is intentionally narrow and discovery-focused:
  - `CC FlagsRaw`
  - `CC ExtData[*].Val6`
  - `APPLY_ABILITY ExtData[*].Val6`
  - `KNOCKBACK Value[0]`
  - `KNOCKBACK ExtData[*].Val6`
  - `IMMUNITY Value[0]`
  - `IMMUNITY ExtData[*].Val6`
  - `Requirement ExtData[*].Val6`
- The report is wired into:
  - `report remaining`
  - `report remaining literals`
  - the Remaining Work GUI tab via `Open Literal Crosswalk`

## New Discovery From The Literal Crosswalk

- The crosswalk strongly validates `1016` as the main shared root/control anchor.
  - It now shows up across six source groups: `CC`, `APPLY_ABILITY`, `KNOCKBACK`, `IMMUNITY Value[0]`, `IMMUNITY ExtData[*].Val6`, and requirement payloads.
  - The strongest stable companions remain:
    - `CC`: `Val2=6`, `Val3=1`, `Val4=8`, `FlagsRaw=1`
    - `IMMUNITY`: `Val2=8`, `Val3=6`, `Value[0]=12`
    - requirements: mostly `Val2=6`, with a smaller `Val3=6` branch
- `1014`, `1015`, `1018`, `1019`, and `1030` all survive the new cross-source ranking and remain credible shared control/immunity literals rather than one-off noise.
- `IMMUNITY Value[0]=12` is now more clearly the shared control-immunity bundle family.
  - It cross-links to `1014`, `1015`, `1016`, `1018`, `1019`, and `1030`.
- `IMMUNITY Value[0]=24` also survives the crosswalk and still points at the knockback-side family.
- `445` is now a real candidate cluster worth tracing in the client.
  - It appears in requirements plus `CC`, `KNOCKBACK`, and `APPLY_ABILITY`.
  - Stable companions are `Val2=3`, `Val3=5`, `Val4=8`, and `Val5=3`.
- `708` is also a real cross-source literal family.
  - It is heavily concentrated in `KNOCKBACK ExtData[*].Val6`, but it also leaks into `CC`, `APPLY_ABILITY`, `IMMUNITY Value[0]`, and requirement `9091`.
  - Best current read: it is some knockback-side literal or profile token, not a random scalar.
- `100` remains cross-source, but it is likely less important for control semantics than the `1014-1030` family.
  - The new report makes that visible by showing its dominant `APPLY_ABILITY` proc-profile companions: `Val2=9`, `Val3=4`, `Val4=8`.
- `CC FlagsRaw` families are cleaner in the crosswalk than they were in the earlier packet-style reports:
  - `1` stays the root-side family
  - `24` stays in the immunity/knockback-side family
  - `38` remains a mixed flag/literal cluster across `CC`, `IMMUNITY Value[0]`, and `KNOCKBACK Value[0]`
- The crosswalk also exposed one report-level caveat:
  - broad numeric ranking can still surface mixed literals like `100`, `1`, `2`, `3`, and `4`
  - that is useful for coverage, but it means the top of the list is not a pure “control-only enum” ranking
  - use the source breakdown and companion fields before naming any literal

## Latest Hatred And Movement-Control Discoveries

- `445` is no longer just a generic cross-source literal cluster.
  - It is a Hatred-threshold selector family.
  - Strongest anchors:
    - `9328 Exile`: `445` + `Val7=25/29/30/59/60/89/90/100` drives the medium/long/very-long knockback brackets described in the client text.
    - `3482 Spiteful Slam`: the same `445` threshold pairs drive the 2s/3s/4s/5s knockdown variants.
    - `3263 Hatred`: the same `445` threshold pairs choose child abilities `355 (Hatred x 10)`, `353 (Hatred x 5)`, and `354 (Hatred x 3)` across the `0-29`, `30-59`, and `60-100` bands.
    - `9345 None Shall Pass`: the low-band `0-29` branch applies `355 (Hatred x 10)`, which is strong evidence that the family is selecting by current Hatred range, not by a CC enum.
  - Best current model:
    - `ExtData[Val2=3, Val3=5, Val4=8, Val5=3, Val6=445, Val7=<threshold>]` is the low-edge selector.
    - `ExtData[Val2=3, Val3=4, Val4=8, Val5=3, Val6=445, Val7=<threshold>]` is the paired upper-edge selector.
    - Together they create explicit Hatred brackets such as `0-29`, `30-59`, `60-89`, and `90-100`.
- `1119` and `1120` are now meaningful, even though their exact retail names are still unknown.
  - They act like paired Hatred-tier selector bits, not free scalar literals.
  - Strongest anchors:
    - `9342 Blade of Ruin` and `9323 Monstrous Rending` both use four-state ladders where the component values step across the `1119/1120` combinations `00`, `10`, `01`, `11`.
    - The matching requirement rows `9399-9402` are just the four combinations of those two selectors.
    - Smaller two-band families like `9319 Driven By Hate`, `9331 Furious Howl`, `9336 Shield of Rage`, `9349 Choking Fury`, and `9350 Bolstering Anger` reuse the same selector style for only the active subset of Hatred tiers.
  - Best current model:
    - `1119` = Hatred tier selector bit A
    - `1120` = Hatred tier selector bit B
    - exact bit ordering is still provisional, but the four-state ladder is real
- `708` is broader than a pure knockback-distance constant.
  - Strongest anchors:
    - `4651 Knockback`: clean generic KNOCKBACK row with `ExtData[1].Val6=708`
    - `9514 Triumphant Blasting`: APPLY_ABILITY path to a moderate knockback/monster knockdown payload with `708`
    - `9328 Exile`: ability-level `708` on a Hate-scaled knockback skill
    - `24824 Snare Net`: both `VELOCITY` and `CC` rows use `708`
    - `23894 Boss Immunities`: `IMMUNITY Value[0]=708` exists as a standalone immunity row
  - Best current model:
    - `708` is a reused movement-control/displacement profile with strongest evidence in knockback families.
    - It is safe to describe as a movement-control/knockback literal family.
    - It is not yet safe to assign an exact retail enum name or to collapse it to “distance” only.
- The matrix app should now surface these as recovered findings rather than leaving them implicit in free-form notes.
  - The control-literal crosswalk is the right place for that because it already aggregates these families across `CC`, `APPLY_ABILITY`, `KNOCKBACK`, `IMMUNITY`, and requirement payloads.

## Latest Selector Refinements

- `1119` and `1120` are better bounded now.
  - `1119` appears by itself in the two-state Black Guard ladders:
    - `9319 Driven By Hate`
    - `9331 Furious Howl`
    - `9336 Shield of Rage`
    - `9349 Choking Fury`
    - `9350 Bolstering Anger`
  - `1120` only appears when the skill needs the full four-state ladder:
    - `9323 Monstrous Rending`
    - `9342 Blade of Ruin`
  - Best current model:
    - `1119` = first Hatred-tier selector bit
    - `1120` = second Hatred-tier selector bit
    - exact bit ordering is still provisional, but `1119` alone handles the two-band selectors and `1120` extends that into the four-state selector.
- Do not use raw string hits on component ids `1119` or `1120` as evidence for the literal meanings.
  - Those are cross-domain numeric collisions with unrelated component rows.
  - The earlier `componenteffects.txt` hit for `1120` was one of those collisions and should not be treated as semantic evidence for the literal.
- `445` has the same kind of collision risk.
  - Effect id `445` is `Siege Wrecker` in `effects.csv`, but that name belongs to the effect graph, not to the Hatred-threshold literal.
  - The literal meaning still comes from the Black Guard Hatred ladders, not from the effect-name row.
- `708` is now safer to describe as a movement-control profile than as a knockback-only value.
  - It is still strongest in `KNOCKBACK`, but `24824 Snare Net` proves that the same literal can also sit in both `VELOCITY` and `CC`.
  - Keep describing it as a movement-control/displacement family until a real retail enum name is recovered.
- One app-label caveat also came out of this pass:
  - component operation `11` is still labeled `DISPEL_BUFF` because that is the current emulator-facing catalog value.
  - the Black Guard hidden-selector rows use op `11` heavily, so do not over-trust that label as retail truth in discovery notes.

## Validation State Before Handoff

Before this checkpoint request, the following had already been successfully built during the working session:

- `ClientDataMatrix/ClientDataMatrix.csproj`
- `WorldServer/WorldServer.csproj`

If more code changes are made after this checkpoint, rebuild both again before handoff.
