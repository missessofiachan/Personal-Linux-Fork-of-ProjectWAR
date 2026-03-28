# Remaining Work

Generated UTC: `2026-03-28T07:01:42.9293787Z`

Extracted root: `C:\Users\Admin\Downloads\myps`

## Summary

- Areas: 6
- Items: 15
- Critical: 6
- High: 5
- Coverage gaps: 26627
- High-signal conflicts: 0
- Unknown fields: 0
- Structural fields: 0
- Requirement rows with unresolved fields: 0
- Token gaps: 0
- Identity-domain risks: 1
- Default next batch file: `overview/remaining-work-next.md`
- Default operation field packet file: `overview/remaining-work-operation-fields.md`
- Default control literal file: `overview/remaining-work-control-literals.md`

## Next Batch Preview

| Global | Area | Priority | Score | Title | Subject | Action | ExampleAbilityId |
| --- | --- | --- | --- | --- | --- | --- | ---: |
| 1 | Coverage Gaps | Critical | 190 | Common missing pattern: csv, bin, effect-text, effect-row, components | csv, bin, effect-text, effect-row, components | Resolve the missing root effect link so ability flow can be traced through the client effect chain. |  |
| 2 | Coverage Gaps | Critical | 175 | Common missing pattern: bin, effect-text, components | bin, effect-text, components | Recover the missing component linkage before trying to interpret operation semantics. | 4 |
| 3 | Coverage Gaps | Critical | 175 | Common missing pattern: bin, effect-text, effect-row, components | bin, effect-text, effect-row, components | Resolve the missing root effect link so ability flow can be traced through the client effect chain. | 209 |
| 4 | Coverage Gaps | Critical | 175 | Common missing pattern: csv, effect-text | csv, effect-text | Backfill the missing localized strings so reports stop depending on internal-only labels. | 564 |
| 5 | Coverage Gaps | Critical | 175 | Common missing pattern: csv, effect-text, effect-row | csv, effect-text, effect-row | Resolve the missing root effect link so ability flow can be traced through the client effect chain. | 987 |
| 6 | Coverage Gaps | Critical | 175 | Common missing pattern: effect-text, effect-row | effect-text, effect-row | Resolve the missing root effect link so ability flow can be traced through the client effect chain. | 153 |
| 7 | Coverage Gaps | High | 167 | Common missing pattern: csv, effect-text, effect-row, components | csv, effect-text, effect-row, components | Resolve the missing root effect link so ability flow can be traced through the client effect chain. | 5979 |
| 8 | Identity Domain Risks | High | 165 | Career Name Entry IDs | CareerName.EntryId | Keep this numeric domain isolated until extracted-client evidence proves a stronger canonical identity mapping. |  |
| 9 | Coverage Gaps | High | 160 | StringsOnly ability bucket | StringsOnly | Find the missing BIN, effect, and component evidence so this stops being a text-only ability shell. |  |
| 10 | Coverage Gaps | High | 154 | Common missing pattern: bin, components | bin, components | Recover the missing component linkage before trying to interpret operation semantics. | 20 |
| 11 | Coverage Gaps | High | 145 | Partial ability bucket | Partial | Close the missing extracted-client pieces called out in the pattern list before spending time on deeper semantic decoding. | 4 |

## Area Summary

| Area | Items | Critical | High | Peak | Bucket | Summary | Next Step |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Coverage Gaps | 10 | 6 | 4 | 190 | Critical | 26627 abilities remain below `Mapped`; the largest buckets are StringsOnly, Partial. | Push the largest shared missing-pattern buckets first so ability reports move from sparse or string-only states into repeatable mapped states. |
| Conflict Hotspots | 4 | 0 | 0 | 110 | Medium | 0 high-signal conflicts remain after noise suppression; the biggest groups are StringMismatch (AbilityDescription), StringMismatch (AbilityName), StringMismatch (EffectName). | Close the highest-signal conflict groups by codifying source precedence instead of treating every disagreement as equally actionable. |
| Unknown Field Hotspots | 0 | 0 | 0 | 0 | Low | No unknown or structural component fields are currently outstanding. | Use the unknown-triage evidence to turn structural layout roles into stable named semantics before widening emulator-side enums. |
| Requirement Semantics | 0 | 0 | 0 | 0 | Low | No requirement rows currently have unresolved field semantics. | Focus on requirement rows with direct ability usage and child links so later linkage work stays evidence-based. |
| Token Gaps | 0 | 0 | 0 | 0 | Low | No unknown or Londo token definitions are currently outstanding. | Prioritize tokens that still depend on Londo or that block natural-language rendering of high-signal component fields. |
| Identity Domain Risks | 1 | 0 | 1 | 165 | High | 1 identity domains still carry non-canonical or duplicate-meaning risk. | Finish the race or career identity collision pass before renaming any repeated client string-entry domains into runtime IDs. |

## Coverage Gaps Top Items

| Rank | Global | Priority | Score | Title | Subject | Summary | Evidence | Action | ExampleAbilityId |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | ---: |
| 1 | 1 | Critical | 190 | Common missing pattern: csv, bin, effect-text, effect-row, components | csv, bin, effect-text, effect-row, components | 14987 abilities still share this extracted-client gap pattern. | Statuses: StringsOnly. Samples: 980, 981, 982, 983, 984, 985, 986, 1113, 1114, 1250, 1855. | Resolve the missing root effect link so ability flow can be traced through the client effect chain. |  |
| 2 | 2 | Critical | 175 | Common missing pattern: bin, effect-text, components | bin, effect-text, components | 2137 abilities still share this extracted-client gap pattern. | Statuses: Partial. Samples: 4, 36, 39, 40, 50, 51, 52, 53, 62, 63, 64, 69. | Recover the missing component linkage before trying to interpret operation semantics. | 4 |
| 3 | 3 | Critical | 175 | Common missing pattern: bin, effect-text, effect-row, components | bin, effect-text, effect-row, components | 128 abilities still share this extracted-client gap pattern. | Statuses: Partial. Samples: 209, 229, 286, 358, 455, 456, 457, 458, 459, 603, 604, 626. | Resolve the missing root effect link so ability flow can be traced through the client effect chain. | 209 |
| 4 | 4 | Critical | 175 | Common missing pattern: csv, effect-text | csv, effect-text | 7133 abilities still share this extracted-client gap pattern. | Statuses: Partial. Samples: 564, 1727, 1830, 1851, 1852, 1853, 1854, 1856, 1857, 1858, 1859, 1860. | Backfill the missing localized strings so reports stop depending on internal-only labels. | 564 |
| 5 | 5 | Critical | 175 | Common missing pattern: csv, effect-text, effect-row | csv, effect-text, effect-row | 1633 abilities still share this extracted-client gap pattern. | Statuses: Partial. Samples: 987, 988, 989, 990, 991, 5240, 5250, 5264, 5281, 5294, 5300, 5325. | Resolve the missing root effect link so ability flow can be traced through the client effect chain. | 987 |
| 6 | 6 | Critical | 175 | Common missing pattern: effect-text, effect-row | effect-text, effect-row | 561 abilities still share this extracted-client gap pattern. | Statuses: Partial. Samples: 153, 156, 168, 169, 175, 177, 188, 189, 196, 197, 208, 216. | Resolve the missing root effect link so ability flow can be traced through the client effect chain. | 153 |
| 7 | 7 | High | 167 | Common missing pattern: csv, effect-text, effect-row, components | csv, effect-text, effect-row, components | 22 abilities still share this extracted-client gap pattern. | Statuses: Partial. Samples: 5979, 14059, 23500, 23639, 23689, 23690, 23691, 23692, 23693, 23696, 24598, 24855. | Resolve the missing root effect link so ability flow can be traced through the client effect chain. | 5979 |
| 8 | 9 | High | 160 | StringsOnly ability bucket | StringsOnly | 14992 abilities still sit in `StringsOnly` coverage instead of a fully mapped state. | Samples: 980, 981, 982, 983, 984, 985, 986, 995, 996, 997, 998. Common missing pieces: csv, bin, effect-text, effect-row, components, csv, bin, effect-row, components. | Find the missing BIN, effect, and component evidence so this stops being a text-only ability shell. |  |
| 9 | 10 | High | 154 | Common missing pattern: bin, components | bin, components | 9 abilities still share this extracted-client gap pattern. | Statuses: Partial. Samples: 20, 121, 126, 1000, 1003, 1016, 1017, 1018, 1019. | Recover the missing component linkage before trying to interpret operation semantics. | 20 |
| 10 | 11 | High | 145 | Partial ability bucket | Partial | 11635 abilities still sit in `Partial` coverage instead of a fully mapped state. | Samples: 4, 20, 36, 39, 40, 50, 51, 52, 53, 62, 63, 64. Common missing pieces: bin, effect-text, components, bin, components, effect-text, effect-row. | Close the missing extracted-client pieces called out in the pattern list before spending time on deeper semantic decoding. | 4 |

## Conflict Hotspots Top Items

| Rank | Global | Priority | Score | Title | Subject | Summary | Evidence | Action | ExampleAbilityId |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | ---: |
| 1 | 12 | Medium | 110 | StringMismatch (AbilityDescription) | StringMismatch \| AbilityDescription | 16 non-noise conflicts across 16 subjects. | Peak triage: 85. High-signal rows: 0. Sample subjects: Ability:1900, Ability:1901, Ability:1902, Ability:1903, Ability:1904, Ability:1905. | Inspect the claim-level evidence and encode an explicit canonical rule for this disagreement. | 1900 |
| 2 | 13 | Medium | 110 | StringMismatch (AbilityName) | StringMismatch \| AbilityName | 2110 non-noise conflicts across 2110 subjects. | Peak triage: 85. High-signal rows: 0. Sample subjects: Ability:10, Ability:1008, Ability:1020, Ability:1053, Ability:1054, Ability:1064. | Separate player-facing names from internal placeholders and keep the canonical string rule explicit. | 10 |
| 3 | 14 | Low | 85 | StringMismatch (EffectName) | StringMismatch \| EffectName | 2 non-noise conflicts across 2 subjects. | Peak triage: 75. High-signal rows: 0. Sample subjects: Effect:3353, Effect:48. | Inspect the claim-level evidence and encode an explicit canonical rule for this disagreement. |  |
| 4 | 15 | Low | 52 | InternalOnlyAbilityNameMismatch (AbilityName) | InternalOnlyAbilityNameMismatch \| AbilityName | 4 non-noise conflicts across 4 subjects. | Peak triage: 32. High-signal rows: 0. Sample subjects: Ability:106, Ability:14, Ability:26, Ability:3266. | Separate player-facing names from internal placeholders and keep the canonical string rule explicit. | 106 |

## Unknown Field Hotspots Top Items

No rows found.

## Requirement Semantics Top Items

No rows found.

## Token Gaps Top Items

No rows found.

## Identity Domain Risks Top Items

| Rank | Global | Priority | Score | Title | Subject | Summary | Evidence | Action | ExampleAbilityId |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | ---: |
| 1 | 8 | High | 165 | Career Name Entry IDs | CareerName.EntryId | Confidence `Inferred` with 24 duplicate-meaning group(s). | Canonicality: Not canonical as a proven CareerId domain. Values: 132. Recommended usage: Use as a client string-entry table only until a real numeric CareerId mapping is proven from extracted files.. Notes: careernames_m.txt contains many repeated display names across different numeric entry ids. That means these ids are not currently safe to rename to CareerId without stronger extracted-client evidence. Duplicate display-name groups: 24.. | Keep this numeric domain isolated until extracted-client evidence proves a stronger canonical identity mapping. |  |

