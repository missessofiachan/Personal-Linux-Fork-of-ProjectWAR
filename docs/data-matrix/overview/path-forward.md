# ClientDataMatrix: Path Forward

Last updated: 2026-03-28 (Londos DB + WorldServer searched; op=43 confirmed; ops 40/41/51/29/47 have contextual evidence)

Component field decode is complete (Unknown=0, Structural=0). Requirement semantics fully decoded. This document covers all remaining open work, ordered by impact and tractability.

---

## ~~Area 1: Requirement Semantics~~ — RESOLVED (commit df9fff32)

All 558 requirement rows are fully decoded. Schema confirmed from decompiled client `AbilityExport.cs`:
- Val1 = `AbilitySourceType` (Self/Cast/Apply/Watch/EventReq/Result/Immunity)
- Val2 = `AbilityOperation` (89-value enum — requirement check type)
- Val3 = `AbilityCondition` (None/Equal/LessThanEqual/GreaterThanEqual/LessThan/GreaterThan/FriendlyTarget)
- Val4 = `AbilityLogicOperator` (None/And/Or/Unk10/Unk11/Unk12)
- Val5–Val9 = operation-specific threshold, child RequirementId ref, aux params, binary flag

---

## ~~Area 2: DAMAGE Value[1]~~ — RESOLVED (commit c193be3e)

Confirmed from decompiled server-side `DAMAGE.cs`:
- `Value[1]` = **`MaxCounter`** — counter cap / tick limit (zero = no limit)
- `FlagsRaw` = **`DamageFlag`** enum (NONE=0, UNMITIGATABLE=1)

---

## Area 3: SERVER_COMMAND — Field Mapping Clarification Needed (High)

### What is unresolved

`SERVER_COMMAND (op=36)` field mapping. Our BIN analysis documented `FlagsRaw` as the command code (8 distinct values) and `Value[2]` as the polymorphic argument (337 non-zero, 59 distinct values). The Londos DB (see below) shows `Values[0]` = command code and `Values[1]` = primary argument, with `Flags=0` on all rows. These two pictures need reconciling before per-command inference can be written.

The Londos DB reveals **72+ distinct command codes** and their argument patterns. The most frequent commands:

| Command code | Count | Values[1] pattern | Values[2] | Interpretation (Inferred) |
|---|---|---|---|---|
| 32 | 310 | 30000–60000 IDs | 0 | Award/grant item or ability by ID |
| 50 | 101 | 30000–50000 IDs | 0 | Similar to 32 |
| 327 | 90 | 1 (constant) | 1 | Toggle/enable state (sub-type 1) |
| 328 | 90 | 1 (constant) | 0 | Toggle/disable state (sub-type 1) |
| 332 | 32 | career/mastery IDs | 5, 5 | Career mastery or specialization unlock |
| 53 | 64 | 87000+ IDs | 0/1/2/3 enum | ID grant with variant selector |
| 173 | 26 | 0 | large IDs | Context reversed — Values[2] = ability ID |
| 23 | 26 | small IDs | 0 | Small-enum command arg |

### Sources searched (2026-03-28)

| Source | Path searched | Result |
|---|---|---|
| RE toolkit AbilityEnums.cs | `D:\Repos\Shmerrick\WAR-RE-Toolkit\docs\research\abilities-bin-file-exporter\AbilityEnums.cs` | `ComponentOperationType` enum names op=36 as `SERVER_COMMAND` but has no Value field names |
| RE toolkit ComponentOP.cs | `D:\Repos\Shmerrick\WAR-RE-Toolkit\apps\admintool\MYPLib\MYPLib\ComponentOP.cs` | Alternative op names for 12/13/15/22/23/25/28; nothing for op=36 Value fields |
| WorldServer MythicAbilityGraphTranslator.cs | `WorldServer/World/Abilities/MythicAbilityGraphTranslator.cs` | Component dispatch switch implements only ops 1, 3, 8, 22, 23. No case for op=36 |
| **Londos DB War_AbilityComponentBin.sql** | `D:\Repos\Shmerrick\WAR-RE-Toolkit\data\database-tables\Londos Server v2\War_AbilityComponentBin.sql` | **1065 op=36 rows; Flags=0 on all rows; Values[0] = command code (72+ distinct); Values[1] = primary arg; Values[2]/[3] = secondary args; confirms polymorphic structure keyed on Values[0]** |

### Remaining approach

**Step 1 — Reconcile FlagsRaw vs Values[0].**
Our BIN says "FlagsRaw = command code, 8 distinct". Londos says "Values[0] = command code, Flags=0". These may be consistent if the Londos devs remapped FlagsRaw → Values[0] on import. Verify by checking what values our `abilitycomponentexport.bin` FlagsRaw field holds for op=36 rows. If those FlagsRaw values match known Londos command codes (32, 50, etc.), the mapping is confirmed.

**Step 2 — Per-command inference.**
Once the field that holds the command code is confirmed, extend `TryBuildServerCommandStructuralInference` to emit per-command-code named inferences (e.g., "command=32: grant item/ability ID via Values[1]") instead of the single "polymorphic command argument" label.

**Step 3 — Identify specific command semantics.**
Work through the 72 command codes. Many can be inferred by correlating Values[1] with known ability/item ID ranges and by examining the ability descriptions of abilities that use each command.

---

## Area 4: Coverage Gaps — 12,664 abilities below Mapped (High)

### Findings from investigation (commit fbb2a83d)

The string files (`abilitynames.txt`, `abilitydesc.txt`) contain 29,001 sequential indexed entries, most of which are blank placeholders. These were inflating the gap count. Fixed by adding a new `BlankSlot` coverage status — fires when an ability's only source data is blank string-table entries. BlankSlot abilities (13,963) are excluded from coverage gap counts as irrecoverable empty placeholder IDs.

True coverage gap is now **12,664** abilities:

| Status | Count | Description |
|---|---|---|
| `Partial` | 11,635 | Have BIN or CSV but missing some pieces |
| `StringsOnly` | 1,029 | Have actual named strings but no BIN/CSV/effect/component |

Top gap patterns (post-BlankSlot fix):
| Pattern | Count | Missing |
|---|---|---|
| bin + effect-text + components | ~2,137 | BIN row but broken effect/component chain |
| csv + effect-text | ~7,133 | Has abilities.csv but no BIN row |
| csv + effect-text + effect-row + components | ~1,633 | Has CSV but no BIN/effect chain |

Extraction is complete — `C:\Users\Admin\Downloads\myps` has all expected files (abilityexport.bin with 11,736 rows, abilitycomponentexport.bin with 18,526 rows, effects.csv with 4,445 rows).

### Remaining approach

**Partial/bin-missing (2,137 abilities with `bin + effect-text + components`):**
These have a BIN row but no component linkage. The effect chain from `EffectId → effects.csv → ComponentId → abilitycomponentexport.bin` is broken. Investigation needed: check if the `EffectId` in `abilityexport.bin` for these abilities links to an effects.csv row that lacks a component chain.

**Partial/csv-missing (~7,133 abilities with `csv + effect-text`):**
Have a BIN row but no abilities.csv row and no effect text. These may be NPC/monster abilities that aren't in the player-facing CSV. Not urgent — the BIN data is the authoritative source.

**StringsOnly (1,029 abilities):**
Have actual non-blank names/descriptions in string files but no BIN/CSV/effect/component. These are likely removed or renamed abilities from a different game version. Low recovery probability.

---

## ~~Area 5: Conflict Hotspots — 2,897 EffectId Conflicts~~ — RESOLVED (commit df9fff32)

All three EffectId conflict groups (AbilityIdMirrorEffectId=2,546, ZeroVsEffectIdGap=289, MountOverlayEffectId=65) already had resolution rules with `CanonicalValue` set. The remaining-work catalog was not filtering resolved conflicts. Fixed by adding `string.IsNullOrWhiteSpace(row.CanonicalValue)` to the `BuildConflictArea` filter and `HighSignalConflictCount` — dropping high-signal conflicts from 2,897 to 0.

Remaining string-mismatch conflicts (AbilityName, AbilityDescription, EffectName) are low-priority noise; all have triage score ≤ 110.

---

## Area 6: Unknown Operation Names (ops 29, 30, 32, 40, 41, 43, 47, 51)

### What is unresolved

Eight operation codes have no confirmed name. Their fields are fully decoded structurally, but the semantic purpose of the operation itself is unknown. Londos DB investigation (2026-03-28) provided contextual evidence for several ops. Op=43 now has a confirmed description. Op=40, 41 have strong contextual inference. Op=51, 47, 29 have partial context. Ops 30 and 32 remain most ambiguous.

| Op | Records | Key field patterns | Londos context (2026-03-28) |
|---|---|---|---|
| 29 | 11 | Value[0] = ID refs (66001–66300 range) or small enum (10, 11); FlagsRaw = CC bits | Witch Hunter "Accusations" ability chain. Values=[10,11] (head) + Values=[66298/99/300, 11, 10] (chain). 66001–66300 may be Witch Hunter accusation-target ability IDs |
| 30 | 1 | Value[0] = 18 (single record) | HEAD of "Crack Shot" (AbilityID=1536, Engi ranged disarm). No Londos description on component; ability desc = "deals damage and disarms target" |
| 32 | 15 | Value[0] = high-range IDs (5424–10020) | HEAD of "Eye of Sheerian" (AbilityID=8606, group armor+resist buff). Component desc template = "{COM_0_VAL0}" = armor amount. Likely GROUP_AREA_STAT or AURA_STAT |
| 40 | 2 | Values=[0] or [1] — binary | Standard-bearing context: adjacent to SIEGE_CARRY (op=28) and "If you die with a standard in-hand, it will be destroyed!" proc. Values=[0]=off, [1]=on. Likely STANDARD_CARRIER_STATE or BANNER_FLAG |
| 41 | 4 | Value[0] = 14; Value[1/2/3] = sequential IDs (187701–187706) | Same standard-bearing ability chain as op=40; Values=[14, 187701–187706] shares 187701–187706 ID range with op=42 (RECOVER_STANDARD). Likely TAKE_STANDARD or CAPTURE_STANDARD |
| 43 | 2 | Value[0] = 21 or 27 | **Confirmed via Londos component description**: "Can autoattack while moving." HEAD of "Cleave" (AbilityID=5104). Suggests MOVEMENT_AUTOATTACK |
| 47 | 8 | Value[0] = 0 or 1; Value[1] = 10/20/30/40 | Adjacent to op=48/49/50 (also unknown) and Halloween event transforms (pig/boar/wolf transforms, pet summons). Pairs: [0,10],[1,10],[0,20],[1,20],[0,30],[1,30],[0,40],[1,40]. Value[0]=direction/toggle, Value[1]=magnitude step |
| 51 | 39 | Value[0] = ability/effect ID; Value[2] = small enum; Value[3] = ordinal | Multiple clusters: HEAD of "Searing Vitality" (tactic, AbilityID=8205, vals=[17666,0,0,0]); "You are a coward!" debuff (vals=[17643,0,0,13/14]); battlefront/fort ordinals 39–42 (IDs 20760–20767) and 89–91 (IDs 31018–31020). Value[3] = rank/progression ordinal |

### Sources searched (2026-03-28)

| Source | Path searched | Result |
|---|---|---|
| RE toolkit AbilityEnums.cs | `D:\Repos\Shmerrick\WAR-RE-Toolkit\docs\research\abilities-bin-file-exporter\AbilityEnums.cs` | `ComponentOperationType` enum — **does not contain** ops 29, 30, 32, 40, 41, 43, 47, 51 |
| RE toolkit ComponentOP.cs | `D:\Repos\Shmerrick\WAR-RE-Toolkit\apps\admintool\MYPLib\MYPLib\ComponentOP.cs` | Contains DISABLE=12, PROC=13, BUFF_PARAM=15, BONUS_TYPE=22, LINKED_ABILITY=23, CAREER_RESOURCE=25, SIEGE_CARRY=28. **Does not contain** ops 29, 30, 32, 40, 41, 43, 47, 51 |
| WorldServer MythicAbilityGraphTranslator.cs | `WorldServer/World/Abilities/MythicAbilityGraphTranslator.cs` | Component dispatch switch: only cases 1, 3, 8, 22, 23. No implementation for any of the 8 unknown ops |
| RE toolkit broader search | `D:\Repos\Shmerrick\WAR-RE-Toolkit\apps\` and `docs\research\` | No additional op-name source found beyond AbilityEnums.cs and ComponentOP.cs. AbilityEngine.cs uses `ComponentOperationType` without adding new values |
| **Londos DB War_AbilityComponentBin.sql** | `D:\Repos\Shmerrick\WAR-RE-Toolkit\data\database-tables\Londos Server v2\War_AbilityComponentBin.sql` | **18,524 component rows; component `Description` field names the buff tooltip. Op=43 component 3431: desc="Can autoattack while moving." Op=51 component 27992/27993: desc="You are a coward!". Op=40/41: standard-bearing context. Op=29: Witch Hunter Accusations context. Op=47: near Halloween event transforms. No op code names in schema itself.** |

### Key Londos DB chain context (2026-03-28)

**Op=40 / Op=41 — Standard mechanic:**
```
Component 14199 op=8  "Mounted speed +10%"
Component 14200 op=15  blank BUFF_PARAM
Component 14201 op=40  vals=[0]          ← state OFF
Component 14202 op=15  blank
...
Component 14209 op=40  vals=[1]          ← state ON
...
Component 14212 op=13  "If you die with a standard in-hand, it will be destroyed!"
Component 14213 op=23  vals=[99999]      LINKED_ABILITY (sentinel = any)
Component 14214 op=41  vals=[14,187701,187702,187703]   ← same ID range as op=42 RECOVER_STANDARD
Component 14215 op=41  vals=[14,187704,187705,187706]
Component 14220 op=28  vals=[14508]      SIEGE_CARRY standard ID
```

**Op=43 — Autoattack while moving (confirmed):**
```
Component 3431 op=43 vals=[27] desc="Can autoattack while moving."  HEAD of Cleave (AbilityID=5104)
Component 9497 op=43 vals=[21] desc=""
```

**Op=51 — Progression ordinal / ability-linked rank:**
```
Component 1597  op=51 vals=[17666,0,0,0]   desc=""  HEAD of "Searing Vitality" (tactic)
Component 27992 op=51 vals=[17643,0,0,13]  desc="You are a coward!"
Component 27993 op=51 vals=[17643,0,0,14]  desc="You are a coward!"
Components 26661–26668: vals=[20760–20767, 0, 0/1/2/3, 39–42]  (battlefront ordinals)
Components 26941–26943: vals=[31018–31020, 0, 0, 89–91]         (fortress ordinals)
```

### Remaining approach

**Step 1 — Op=43: Update ComponentOperationType enum.**
Name is effectively confirmed as MOVEMENT_AUTOATTACK (or equivalent). Add to enum and update schema catalog inference from Inferred to Confirmed.

**Step 2 — Op=40 and op=41: Cross-check with RECOVER_STANDARD.**
Op=42 = RECOVER_STANDARD. Op=41 uses the same 187701–187706 ID range. Op=40 is a binary toggle in the same ability chain. Likely names: op=41 = TAKE_STANDARD, op=40 = STANDARD_CARRIER_STATE (or similar). Confirm by reading the full "Carry the Standard" ability description from AbilityBin.

**Step 3 — Op=51: Read associated abilities.**
Ability "Searing Vitality" (AbilityID=8205) has op=51 as head component with vals=[17666,0,0,0]. Read the full ability chain and description to determine if op=51 = tactic modifier or progression rank. The "coward" debuff (vals=[17643,0,0,13/14]) and battlefront ordinals (vals=[...,0,0,89-91]) suggest a rank/tier mechanism.

**Step 4 — Op=29: Identify 66001–66300 ID range.**
Search `abilityexport.bin` for abilities in the 66001–66300 ID range. If they are all Witch Hunter Accusations or a related sub-ability set, op=29 = ACCUSATION_APPLY or equivalent.

**Step 5 — Once named, update ComponentOperationType enum.**
Add confirmed names to the operation type enum and update schema catalog labels.

---

## ~~Area 7: Identity Domain — CareerName.EntryId~~ — RESOLVED (commit 2a4c1bd7)

`careernames_m.txt` has **120 entries (IDs 12–131) = 5 display-context groups × 24 careers**. Each career name repeats 5 times with different IDs — one per UI display context. The duplicates are structurally expected, not a CareerId collision risk.

Canonical CareerId mapping: **`careerlines_m.txt` IDs 0–24** (25 entries, unique, confirmed by `abilityexport.bin` CareerLine field). `CareerName.EntryId` is a string-context table, not an identity domain.

Implementation: Added `DuplicatesAreExpected` to `IdentityDomainRecord`; updated catalog and filter. Identity-domain risks: 1 → 0.

---

## Summary Priority Table

| Area | Status | Priority | Blocker For | Entry Point |
|---|---|---|---|---|
| ~~Requirement semantics (558 rows)~~ | **RESOLVED** | — | — | — |
| ~~DAMAGE Value[1] / FlagsRaw~~ | **RESOLVED** | — | — | — |
| ~~EffectId conflicts (2,897)~~ | **RESOLVED** | — | — | — |
| Coverage gaps (12,664 abilities) | Open | **High** | Ability reports below Mapped status | Investigate broken effect chains in Partial bucket |
| SERVER_COMMAND field mapping | Open | **High** | Full SERVER_COMMAND semantic decode | Reconcile FlagsRaw vs Values[0]; Londos shows Values[0]=command code |
| Unknown op names (8 ops) | Open | **Medium** | Named operation dispatch in emulator | Op=43 confirmed MOVEMENT_AUTOATTACK; ops 40/41/51 have strong contextual inference |
| ~~CareerName identity domain~~ | **RESOLVED** | — | — | — |
