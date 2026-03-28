# ClientDataMatrix: Path Forward

Last updated: 2026-03-28 (post-commit 7a76ddb3)

Component field decode is complete (Unknown=0, Structural=0). This document covers all remaining open work, ordered by impact and tractability.

---

## Area 1: Requirement Semantics (Critical — 558 rows)

### What is unresolved

`abilityrequirementexport.bin` contains requirement records referenced by component ExtData `Val6` fields (layout tag=9). Each requirement row has its own ExtData slots (Val1–Val9). Currently, 558 rows have fields that are Structural or Unknown — the tool knows the values exist but not what they mean.

The top rows by ability impact (from `remaining-work.md`):

| RequirementId | Unresolved fields | Direct abilities | Children |
|---|---|---|---|
| 9622 | 9 | 420 | 2 |
| 9625 | 9 | 420 | 2 |
| 9100 | 15 | 172 | 0 |
| 9260 | 9 | 72 | 0 |
| 9591 | 10 | 32 | 0 |

The most common unresolved fields are `ExtData[0].Val1`, `.Val2`, `.Val3`, `.Val4`, `.Val5` — i.e., the first five slots of the first ExtData block in each requirement row. Val6 and Val7 are already partially decoded (Val6 carries RequirementId child chain references).

### Why it matters

Requirement semantics are a prerequisite for widening the `ExtData[*].Val6 → RequirementId` linkage rule beyond its current narrow form. Until requirement ExtData is decoded, the tool cannot fully trace conditional ability logic (e.g., "ability X only when career rank ≥ N", "ability X only if tactic Y is slotted").

### Approach

**Step 1 — Anchor from known requirements.**
Requirement rows with a clear ability cluster (e.g., 9622/9625 both map to 420 basic career abilities) almost certainly encode a career gating check. Look at what the 420 abilities have in common: they are likely rank-1 career basics, meaning `ExtData[0].Val1` may be a check type code (e.g., "minimum career rank") and `ExtData[0].Val2` may be the threshold value.

**Step 2 — Cross-reference `abilityrequirementexport.bin` schema against WAR-RE-Toolkit.**
Source: `D:\Repos\Shmerrick\WAR-RE-Toolkit\docs\research\abilities-bin-file-exporter\`
The toolkit contains decompiled client requirement parsing code. Match the field offsets in the BIN parser against the unresolved Val positions. This is the highest-confidence decode path.

**Step 3 — Group by child-link structure.**
Requirements with `Children > 0` are parent nodes in a requirement tree. These likely encode boolean combinators (AND/OR). Requirements with `Parents > 0` are leaf conditions. Decode leaf nodes first, then infer parent semantics from their children.

**Step 4 — Group by ability cluster.**
Requirements shared by large homogeneous ability sets (career basics, career tactics, rank-gated abilities) should decode identically. Requirements shared by siege weapon abilities (10770–10775) likely gate on vehicle occupation or zone.

**Step 5 — Implement decode handlers.**
Add `TryBuildRequirement*` handlers in `RequirementSchemaCatalog.cs` (or equivalent) mirroring the pattern used in `ComponentSchemaCatalog.cs`. Start with the five highest-impact requirement IDs.

---

## Area 2: DAMAGE Value[1] — Secondary Formula Parameter (High)

### What is unresolved

`DAMAGE (op=1)` `Value[1]`: 591 non-zero records, 81 distinct values ranging from small integers to large IDs. Currently marked Inferred with distribution notes. The semantic meaning (e.g., damage formula multiplier, secondary damage table reference, armor penetration parameter) is not confirmed.

Distribution notes recorded:
- Cluster 1: values 1–20 (small integer range — likely a formula tier or secondary modifier)
- Cluster 2: values 100–1000 (percentage-like magnitudes)
- Cluster 3: sparse large values (>10000 — possible ID references)

### Approach

**Step 1 — WAR-RE-Toolkit cross-reference.**
The decompiled client damage formula code in the RE toolkit should name the parameter at offset corresponding to `Value[1]` in the component BIN layout. This is the primary resolution path.

**Step 2 — Correlate with known abilities.**
Generate a per-ability component report for a set of abilities with known DAMAGE semantics (e.g., a basic career melee attack, a DoT tick, a siege weapon shot). Correlate the `Value[1]` value against ability descriptions. If `Value[1]` tracks damage tier, it should scale monotonically with the ability's stated damage increase across tiers.

**Step 3 — Check against effects.csv.**
Some damage formula parameters are effect-level, not component-level. If `Value[1]` from the component matches a field in `effects.csv` for the same ability, the field may be a cross-reference rather than a scalar.

**Step 4 — Promote from Inferred to Confirmed.**
Once the semantic is established from client code, update `TryBuildDamageStructuralInference` to emit a named Confirmed inference rather than the current Inferred distribution note.

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

## Area 4: Coverage Gaps — 26,627 abilities below Mapped (High)

### What is unresolved

26,627 of ~40,000+ abilities are in `StringsOnly` or `Partial` coverage state. They lack one or more of: BIN row, effect text, effect row, component linkage.

Top gap patterns:
| Pattern | Count | Missing |
|---|---|---|
| csv + bin + effect-text + effect-row + components | 14,987 | Everything except strings |
| csv + effect-text | 7,133 | Localized strings only |
| bin + effect-text + components | 2,137 | BIN row + components |
| csv + effect-text + effect-row | 1,633 | Strings + effect row |

The 14,987 `StringsOnly` abilities with all extracted-client data missing are likely abilities that were removed or are internal-only (monster abilities, scripted abilities, developer test abilities) and genuinely have no BIN/effect data in the extracted client. These may be irrecoverable from current extraction.

### Approach

**Step 1 — Verify extraction completeness.**
Before assuming data is missing, verify that all relevant MYP archives were extracted. Check `C:\Users\Admin\Music\Warhammer` for any unextracted archives. The 14,987-ability `StringsOnly` bucket may reduce significantly if the gamedata MYP was not fully extracted.

**Step 2 — Prioritize the 2,137 `bin + effect-text + components` partial abilities.**
These have a BIN row but no component linkage. The component linkage typically flows through `EffectId → effects.csv → ComponentId → abilitycomponentexport.bin`. For these abilities, the effect chain is broken. Check if the `EffectId` in `abilityexport.bin` for these abilities points to rows that exist in `abilitycomponentexport.bin` under a different key path.

**Step 3 — Resolve the `csv + effect-text` gap (7,133 abilities).**
These have no `abilities.csv` row and no effect text. They may be pre-game or template abilities. The `pregame_chars.xml` file likely contains ability IDs for pre-game entities — check if the missing abilities are referenced there.

**Step 4 — Codify irrecoverable vs recoverable.**
After Steps 1–3, reclassify remaining `StringsOnly` abilities as `Irrecoverable` if they have no client BIN evidence at all. This prevents them from polluting coverage metrics indefinitely.

---

## Area 5: Conflict Hotspots — 2,897 EffectId Conflicts (High)

### What is unresolved

Three conflict patterns account for most of the high-signal count:

| Pattern | Conflicts | Description |
|---|---|---|
| `AbilityIdMirrorEffectId` | 2,546 | AbilityId == EffectId numerically — two sources disagree on which value is canonical |
| `ZeroVsEffectIdGap` | 289 | One source has EffectId=0, another has a real value |
| `MountOverlayEffectId` | 65 | Mount-specific overlay effect ID conflicts |

### Approach

**`AbilityIdMirrorEffectId` (2,546 rows):**
When an ability's numeric ID equals its effect's numeric ID, two different sources (e.g., `abilityexport.bin` vs `effects.csv` EffectId column) may both claim to be canonical. The correct resolution is: use `abilityexport.bin`'s `EffectId` field as the primary canonical source, and treat any other source's matching value as coincidental numeric equality. Codify this as an explicit precedence rule in the conflict resolver — it will collapse all 2,546 rows at once.

**`ZeroVsEffectIdGap` (289 rows):**
When one source has EffectId=0 and another has a real ID, the non-zero value is almost certainly correct (zero = absent/unlinked). Codify: prefer non-zero EffectId over zero from any source. This collapses all 289 rows.

**`MountOverlayEffectId` (65 rows):**
Mount overlay abilities (3702–3707 range) have an overlay effect that differs from the primary effect. Verify whether these abilities should carry the overlay EffectId or the primary EffectId for the canonical slot, then codify the rule.

**Implementation:** Add explicit conflict-resolution rules in the conflict resolver (likely `ConflictResolver.cs` or equivalent). Each rule should name the source precedence and the detection condition.

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

| Area | Priority | Blocker For | Entry Point |
|---|---|---|---|
| Requirement semantics (558 rows) | **Critical** | Full conditional-ability trace; ability flow completeness | WAR-RE-Toolkit BIN parser + top-5 requirement rows |
| Coverage gaps (26k abilities) | **High** | Ability reports below Mapped status | Verify MYP extraction completeness |
| EffectId conflicts (2,897) | **High** | Canonical EffectId resolution | Codify `AbilityIdMirrorEffectId` + `ZeroVsEffectIdGap` rules |
| DAMAGE Value[1] | **High** | Full DAMAGE formula reconstruction | WAR-RE-Toolkit damage formula decompile |
| SERVER_COMMAND Value[2] | **High** | Full SERVER_COMMAND semantic decode | WorldServer op=36 handler + RE toolkit |
| Unknown op names (8 ops) | **Medium** | Named operation dispatch in emulator | RE toolkit ComponentOperation enum |
| CareerName identity domain | **Low-Medium** | CareerId canonical mapping | BIN CareerLine field cross-reference |
