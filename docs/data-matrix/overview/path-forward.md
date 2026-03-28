# ClientDataMatrix: Path Forward

Last updated: 2026-03-28 (post-commit c193be3e)

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

## Area 3: SERVER_COMMAND Value[2] — Polymorphic Command Argument (High)

### What is unresolved

`SERVER_COMMAND (op=36)` `Value[2]`: 337 non-zero, 59 distinct values. Currently documented as a polymorphic argument whose role depends on companion fields (FlagsRaw command code + Value[3/4/5] context).

Three observed modes:
- Small enum values (1–10): likely command sub-type selectors
- ID references (100–10000+): likely entity/ability/effect IDs
- Sentinel values (-1, 100000): boundary/max/all semantics

### Approach

**Step 1 — Enumerate FlagsRaw command codes.**
`SERVER_COMMAND FlagsRaw` has 8 distinct values. Map each FlagsRaw code to its Value[2] distribution. If each command code has a consistent Value[2] type (enum vs ID vs sentinel), the polymorphism is resolved by conditioning on FlagsRaw.

**Step 2 — Cross-reference server-side command handler.**
The `WorldServer` codebase must have a command dispatch handler that reads `op=36` component data. Search for the SERVER_COMMAND enum or op=36 parsing code. The handler argument parsing will name each Value[2] role per command code.

**Step 3 — Cross-reference WAR-RE-Toolkit.**
The client code that serializes SERVER_COMMAND arguments should mirror the server-side parser. Look for the BIN export class that writes Value[2] for op=36 components.

**Step 4 — Update inference.**
Once each (FlagsRaw, Value[2]) combination is semantically labeled, extend `TryBuildServerCommandStructuralInference` to emit per-command-code named inferences instead of the single "polymorphic command argument" Inferred label.

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

Eight operation codes have no confirmed name. Their fields are fully decoded structurally, but the semantic purpose of the operation itself is unknown.

| Op | Records | Key field patterns |
|---|---|---|
| 29 | 11 | Value[0] = ID refs (66001–66300 range), FlagsRaw = CC bits |
| 30 | 1 | Value[0] = 18 (single record) |
| 32 | 15 | Value[0] = high-range IDs (5424–10020) |
| 40 | 1 | Value[0] = 1 (single record) |
| 41 | 4 | Value[0] = 14; Value[1/2/3] = sequential ID groups (187701–187706) |
| 43 | 2 | Value[0] = 21 or 27 |
| 47 | 8 | Value[1] = regular 10-unit increments (10,20,30,40) |
| 51 | 39 | Value[0] = high-range ID refs; FlagsRaw = bit2; Value[2] = small enum; Value[3] = ordinal |

### Approach

**Step 1 — WAR-RE-Toolkit decompiled enum.**
The client's `ComponentOperation` or equivalent enum almost certainly names all operation codes. Look in `D:\Repos\Shmerrick\WAR-RE-Toolkit\docs\research\abilities-bin-file-exporter\` for the exported BIN parser class — it will have a switch statement or enum mapping that names ops 29, 30, 32, 40, 41, 43, 47, 51.

**Step 2 — Anchor via ability clusters.**
Op=51 (39 records) and op=47 (8 records) have enough samples to cluster by ability. Run the ability report for the abilities that use op=51. If they are all in one career or one ability category, the operation purpose follows.

**Step 3 — Anchor op=41 via op=42 similarity.**
Op=41's Value[1/2/3] use the same 187701–187706 ID range as op=42 (`RECOVER_STANDARD`). They may be related operations (e.g., RECOVER_STANDARD_ALT or RECOVER_STANDARD_CLEAR). Cross-reference which abilities use both ops together.

**Step 4 — Once named, update ComponentOperationType enum.**
Add the resolved names to the operation type enum in the emulator and update the schema catalog labels.

---

## Area 7: Identity Domain — CareerName.EntryId (Low-Medium)

### What is unresolved

`careernames_m.txt` contains 132 distinct entry IDs but 24 groups of duplicate display names. This means entry IDs cannot currently be used as canonical `CareerId` values without risking collision.

### Approach

**Step 1 — Find CareerId in `abilityexport.bin` or `pregame_chars.xml`.**
The `abilityexport.bin` CareerLine field uses a numeric career identifier. Cross-reference it against the string entry IDs in `careernames_m.txt`. If the numeric values from BIN match a subset of the string entry IDs, those are canonical.

**Step 2 — Verify uniqueness in the matched subset.**
If the 24 duplicate groups all fall outside the matched subset (i.e., the BIN uses only a small set of non-duplicated IDs), the risk is resolved.

**Step 3 — Promote confidence from Inferred to Confirmed.**
Once the canonical CareerId domain is established from BIN evidence, update the identity domain registry in the tool.

---

## Summary Priority Table

| Area | Status | Priority | Blocker For | Entry Point |
|---|---|---|---|---|
| ~~Requirement semantics (558 rows)~~ | **RESOLVED** | — | — | — |
| ~~DAMAGE Value[1] / FlagsRaw~~ | **RESOLVED** | — | — | — |
| ~~EffectId conflicts (2,897)~~ | **RESOLVED** | — | — | — |
| Coverage gaps (12,664 abilities) | Open | **High** | Ability reports below Mapped status | Investigate broken effect chains in Partial bucket |
| SERVER_COMMAND Value[2] | Open | **High** | Full SERVER_COMMAND semantic decode | WorldServer op=36 handler + RE toolkit |
| Unknown op names (8 ops) | Open | **Medium** | Named operation dispatch in emulator | RE toolkit ComponentOperation enum |
| CareerName identity domain | Open | **Low-Medium** | CareerId canonical mapping | BIN CareerLine field cross-reference |
