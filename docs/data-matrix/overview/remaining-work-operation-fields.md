# Remaining Work Operation Field Packets

Generated UTC: `2026-03-27T21:39:08.3537633Z`

Extracted root: `C:\Users\Admin\Downloads\myps`

Filter: area `Operations`, priority >= `High`, top 12

Packets: 12

## Packet Summary

| Global | AreaRank | Operation | Field | Priority | Score | Title | ExampleAbilityId | SampleValues |
| --- | --- | --- | --- | --- | --- | --- | ---: | --- |
| 1 | 1 | APPLY_ABILITY (23) | ExtData[0].Val7 | Critical | 229 | APPLY_ABILITY :: ExtData[0].Val7 | 3 | 1, 10, 100, 11, 12, 120, 1200, 1800, 2, 20, 2000, 25 |
| 2 | 2 | APPLY_ABILITY (23) | ExtData[0].Val3 | Critical | 213 | APPLY_ABILITY :: ExtData[0].Val3 | 3 | 1, 3, 4, 5, 6, 7 |
| 3 | 3 | APPLY_ABILITY (23) | ExtData[1].Val7 | Critical | 207 | APPLY_ABILITY :: ExtData[1].Val7 | 3 | 1, 10, 100, 15, 1800, 2, 20, 2000, 22, 249, 25, 29 |
| 4 | 4 | APPLY_ABILITY (23) | ExtData[0].Val1 | Critical | 205 | APPLY_ABILITY :: ExtData[0].Val1 | 3 | 1, 2 |
| 5 | 5 | APPLY_ABILITY (23) | ExtData[0].Val4 | Critical | 205 | APPLY_ABILITY :: ExtData[0].Val4 | 3 | 8, 9 |
| 6 | 6 | CC (12) | FlagsRaw | Critical | 199 | CC :: FlagsRaw | 3 | 1, 1023, 1024, 12, 124, 126, 127, 128, 129, 137, 14, 16 |
| 10 | 7 | APPLY_ABILITY (23) | ExtData[1].Val3 | Critical | 185 | APPLY_ABILITY :: ExtData[1].Val3 | 3 | 1, 4, 5, 6, 7 |
| 11 | 8 | APPLY_ABILITY (23) | ExtData[1].Val1 | Critical | 181 | APPLY_ABILITY :: ExtData[1].Val1 | 3 | 1, 2, 6 |
| 12 | 9 | CC (12) | ExtData[1].Val7 | Critical | 181 | CC :: ExtData[1].Val7 | 3 | 1, 100, 168, 20, 200, 201, 2101, 24, 25, 3, 30, 33 |
| 14 | 10 | APPLY_ABILITY (23) | ExtData[1].Val4 | Critical | 179 | APPLY_ABILITY :: ExtData[1].Val4 | 3 | 8, 9 |
| 15 | 11 | APPLY_ABILITY (23) | FlagsRaw | Critical | 177 | APPLY_ABILITY :: FlagsRaw | 3 | 1, 12, 16, 18, 2, 24, 28, 3, 4, 5, 7, 8 |
| 22 | 12 | CC (12) | ExtData[0].Val7 | Critical | 174 | CC :: ExtData[0].Val7 | 3 | 1, 101, 14741, 15, 167, 2, 20, 201, 25, 26540, 50 |

## APPLY_ABILITY :: ExtData[0].Val7

- Operation: `APPLY_ABILITY` (`23`)
- Field: `ExtData[0].Val7`
- Ranks: global `1`, area `1`
- Priority: `Critical` `229`
- Example ability: `3`

Summary: APPLY_ABILITY ext-data payload-B field for the slot-local embedded payload.

Evidence: Confidence: Structural. Non-zero rows: 889. Distinct values: 33. Tags: Damage, Disarm, Heal, Immunity, Knockback, Knockdown, Root, Silence, Snare, Stagger, Stun. Samples: 1, 10, 100, 11, 12, 120, 1200, 1800, 2, 20, 2000, 25.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | 663 | 663 | 22 | 10, 183, 232, 233, 31, 383, 410, 5, 526, 550, 648, 670 | 6 -> OnEventTriggered x295, 3 -> OnPreviousComponentApplied x210, 1 -> OnApply x155, 7 -> OnPreviousComponentTick x23, 5 -> OnBuffEnded x15, 8 -> OnPreviousComponentBuffTick x14 | Damage x376, Heal x109, Silence x7, Snare x7, Knockback x4, Disarm x3, Stagger x3, Immunity x2 | ExtData[0].Val3=1 (661/663, 99%); ExtData[0].Val4=8 (661/663, 99%); ExtData[0].Val1=2 (602/663, 90%) |
| 5 | 55 | 55 | 23 | 10806, 13330, 14117, 15146, 18001, 18006, 18007, 18008, 18009, 18017, 18023, 18074 | 6 -> OnEventTriggered x49, 3 -> OnPreviousComponentApplied x2, 1 -> OnApply x1, 10 -> OnBuffEndedRemoved x1, 7 -> OnPreviousComponentTick x1 | Heal x17, Damage x8 | ExtData[0].Val4=8 (55/55, 100%); ExtData[0].Val1=1 (54/55, 98%); ExtData[0].Val3=4 (51/55, 92%); ExtData[0].Val2=9 (51/55, 92%); ExtData[0].Val6=100 (50/55, 90%) |
| 250 | 39 | 39 | 13 | 3738, 8236, 8237, 8240, 8250, 8251, 8252, 8253, 8254, 8255, 8256, 8257 | 3 -> OnPreviousComponentApplied x35, 1 -> OnApply x18, 7 -> OnPreviousComponentTick x12, 8 -> OnPreviousComponentBuffTick x2, 6 -> OnEventTriggered x1 | Damage x46, Heal x7, Snare x3, Silence x1 | ExtData[0].Val2=3 (39/39, 100%); ExtData[0].Val3=4 (39/39, 100%); ExtData[0].Val4=8 (39/39, 100%) |
| 4 | 20 | 20 | 23 | 13330, 14082, 14148, 18024, 21315, 24552, 24553, 24554, 24555, 24556, 24557, 24558 | 1 -> OnApply x20, 6 -> OnEventTriggered x3, 10 -> OnBuffEndedRemoved x2, 3 -> OnPreviousComponentApplied x1 | Damage x13, Heal x1, Silence x1 | ExtData[0].Val4=8 (19/20, 95%) |
| 2 | 18 | 18 | 18 | 13044, 13330, 14701, 14704, 14707, 14710, 14713, 14716, 14719, 15972, 15975, 1734 | 1 -> OnApply x13, 3 -> OnPreviousComponentApplied x3, 10 -> OnBuffEndedRemoved x1, 6 -> OnEventTriggered x1 | Damage x7 | ExtData[0].Val4=8 (18/18, 100%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 1 | 5 | KABOOM! | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 1 | 9 | Poisoned Spine | 1699 | 1 | 3 -> OnPreviousComponentApplied | Damage | Your Squig gores its target, dealing {COM_0_VAL0_DAMAGE} and an additional {ABIL_3881_COM_0_VAL0_TOD_DAMAGE} over {ABIL_3881_COM_0_DURA_SECONDS}. |
| 1 | 10 | Gore | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Squig slams it's head in to it's target, dealing {COM_0_VAL0_DAMAGE} and knocking them back. |
| 1 | 31 | Detonation | 3447 | 1 | 3 -> OnPreviousComponentApplied |  |  |
| 1 | 84 | Flamethrower | 3123 | 0 | 1 -> OnApply |  |  |
| 1 | 183 | Pierce | 493 | 0 | 1 -> OnApply |  |  |
| 5 | 10806 | Elixir of Tahoth | 18366 | 1 | 6 -> OnEventTriggered |  | Downing this concoction will provide you with a sample of Tahoth's power! For the next {COM_0_DURA_MINUTES}, every time you attack, there is a 5% chance that the next ability used within {ABIL_10810_COM_1_DURA_SECONDS} will not take any AP to use. This cannot trigger more than once every 10s. |
| 5 | 13330 | Portal to Beyond! | 13386 | 6 | 10 -> OnBuffEndedRemoved |  |  |
| 5 | 14117 | Invigorating Victory - Dark Elf | 14188 | 1 | 6 -> OnEventTriggered |  | Killing Dark Elves will double your AP regen rate 10% of the time. |
| 5 | 15146 | Exalted Glory of War | 18048 | 9 | 3 -> OnPreviousComponentApplied |  | For the next {COM_0_DURA_SECONDS}, everyone in the area will receive an extra {COM_3_VAL1}% bonus to experience, influence, and renown. |
| 5 | 18001 | Soul Barrier I | 18003 | 1 | 6 -> OnEventTriggered | Damage | Soul Barrier I - On Being Hit: 5% chance to absorb up to {ABIL_18037_COM_0_VAL1} damage over {ABIL_18037_COM_0_DURA_SECONDS}. This effect will not activate more than once every {ABIL_18073_COM_0_DURA_SECONDS}. |
| 5 | 18006 | Soul Haste I | 18022 | 1 | 6 -> OnEventTriggered |  | Soul Haste I - On Hit: 5% chance to increase attack speed by {ABIL_18042_COM_0_VAL1}% for {ABIL_18042_COM_0_DURA_SECONDS}. This effect will not activate more than once every {ABIL_18072_COM_0_DURA_SECONDS}. |
| 250 | 3738 | Mount - BAF Griffon - Order | 11747 | 0 | 1 -> OnApply |  | Your next ability will require no AP. |
| 250 | 8236 | Judgement | 9095 | 1 | 3 -> OnPreviousComponentApplied | Damage | You call down the holy judgement of Sigmar, dealing {COM_0_VAL0_SPIRITDAMAGE} to the enemy. |
| 250 | 8236 | Judgement | 9601 | 2 | 1 -> OnApply | Damage | You call down the holy judgement of Sigmar, dealing {COM_0_VAL0_SPIRITDAMAGE} to the enemy. |
| 250 | 8237 | Supplication | 8779 | 1 | 8 -> OnPreviousComponentBuffTick |  | For the next {COM_0_DURA_SECONDS}, you will convert 30 action points into {COM_1_VAL0_COM_0_VAL0} Righteous Fury every second.  If you run out of action points or break your concentration, this effect will end. |
| 250 | 8240 | Bludgeon | 8779 | 1 | 3 -> OnPreviousComponentApplied | Damage | A powerful attack which deals {COM_0_VAL0_DAMAGE} to your enemy. |
| 250 | 8240 | Bludgeon | 9601 | 2 | 1 -> OnApply | Damage | A powerful attack which deals {COM_0_VAL0_DAMAGE} to your enemy. |

## APPLY_ABILITY :: ExtData[0].Val3

- Operation: `APPLY_ABILITY` (`23`)
- Field: `ExtData[0].Val3`
- Ranks: global `2`, area `2`
- Priority: `Critical` `213`
- Example ability: `3`

Summary: APPLY_ABILITY ext-data profile selector for the slot-local embedded payload.

Evidence: Confidence: Structural. Non-zero rows: 1124. Distinct values: 6. Tags: Damage, Disarm, Heal, Immunity, Knockback, Knockdown, Root, Silence, Snare, Stagger, Stun. Samples: 1, 3, 4, 5, 6, 7.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | 812 | 812 | 23 | 10, 183, 230, 232, 233, 260, 264, 278, 3, 31, 311, 320 | 3 -> OnPreviousComponentApplied x797, 6 -> OnEventTriggered x339, 1 -> OnApply x221, 7 -> OnPreviousComponentTick x25, 5 -> OnBuffEnded x22, 8 -> OnPreviousComponentBuffTick x17 | Damage x455, Heal x122, Silence x31, Disarm x14, Snare x8, Stagger x8, Knockdown x6, Immunity x5 | ExtData[0].Val4=8 (809/812, 99%); ExtData[0].Val7=1 (661/812, 81%); ExtData[0].Val1=2 (627/812, 77%) |
| 4 | 172 | 172 | 14 | 1590, 1828, 1856, 3395, 3651, 3652, 3653, 3738, 7089, 758, 776, 779 | 6 -> OnEventTriggered x96, 1 -> OnApply x43, 3 -> OnPreviousComponentApplied x37, 7 -> OnPreviousComponentTick x22, 8 -> OnPreviousComponentBuffTick x3, 10 -> OnBuffEndedRemoved x1 | Damage x74, Heal x39, Snare x3, Silence x2 | ExtData[0].Val4=8 (172/172, 100%); ExtData[0].Val1=1 (139/172, 80%) |
| 6 | 99 | 99 | 19 | 1010, 1011, 1051, 1062, 1365, 1370, 1418, 1432, 1435, 1438, 1439, 1443 | 3 -> OnPreviousComponentApplied x224, 1 -> OnApply x41, 10 -> OnBuffEndedRemoved x9, 6 -> OnEventTriggered x8, 7 -> OnPreviousComponentTick x7, 5 -> OnBuffEnded x5 | Damage x114, Root x15, Snare x8, Heal x4, Knockback x2, Silence x1, Stagger x1 | ExtData[0].Val4=8 (96/99, 96%) |
| 5 | 36 | 36 | 20 | 1366, 14701, 14704, 14707, 14710, 14713, 14716, 14719, 15972, 15975, 24552, 24553 | 1 -> OnApply x26, 3 -> OnPreviousComponentApplied x3, 8 -> OnPreviousComponentBuffTick x2, 6 -> OnEventTriggered x1 | Damage x8 | ExtData[0].Val4=8 (36/36, 100%) |
| 7 | 4 | 4 | 1 | 10320 | 1 -> OnApply x1 | No sampled context tags are available for this value yet. | ExtData[0].Val1=1 (4/4, 100%); ExtData[0].Val4=8 (4/4, 100%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 1 | 3 | Bounce | 329 | 2 | 3 -> OnPreviousComponentApplied | Damage | You cause your Battle Squig to bounce, dealing {COM_0_VAL0_DAMAGE} and knocking your target down for {COM_1_DURA_SECONDS}. |
| 1 | 5 | KABOOM! | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 1 | 6 | ;/end | 329 | 1 | 3 -> OnPreviousComponentApplied | Damage | Your Squig bounces on it's target dealing {COM_2_VAL0_DAMAGE}, and knocking them down for {COM_0_DURA_SECONDS}. |
| 1 | 9 | Poisoned Spine | 1699 | 1 | 3 -> OnPreviousComponentApplied | Damage | Your Squig gores its target, dealing {COM_0_VAL0_DAMAGE} and an additional {ABIL_3881_COM_0_VAL0_TOD_DAMAGE} over {ABIL_3881_COM_0_DURA_SECONDS}. |
| 1 | 10 | Gore | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Squig slams it's head in to it's target, dealing {COM_0_VAL0_DAMAGE} and knocking them back. |
| 1 | 31 | Detonation | 3447 | 1 | 3 -> OnPreviousComponentApplied |  |  |
| 4 | 758 | Rune Of Immolation | 3053 | 1 | 6 -> OnEventTriggered |  | Any time you are hit, there is a 25% chance that your movement speed will be increased by {COM_1_VAL0_COM_0_VAL0}% for {COM_1_VAL0_COM_0_DURA_SECONDS}. |
| 4 | 776 | Spellbinding Rune | 3056 | 1 | 6 -> OnEventTriggered | Heal | Any time that you are healed by a direct heal, there is a 25% chance that you will regain an additional {COM_1_VAL0_COM_0_VAL0} hit points as well. |
| 4 | 779 | ;Rune Priest Spec Abilities | 1192 | 1 | 6 -> OnEventTriggered | Damage | Any time you are attacked, there is a 25% chance that you will become protected by a magical barrier for up to {COM_1_VAL0_COM_0_DURA_SECONDS}, which will absorb up to {COM_1_VAL0_COM_0_VAL1_DAMAGE}.<BR>This effect will not trigger more than once every 3 seconds. |
| 4 | 1590 | Fan The Flames | 1790 | 1 | 7 -> OnPreviousComponentTick | Heal | Heals for {COM_0_VAL0_TOD} life over {COM_0_DURA_SECONDS} |
| 4 | 1828 | Crushing Blows | 720 | 0 | 1 -> OnApply | Damage | You cause your pet to explode, dealing {ABIL_15_COM_0_VAL0_DAMAGE} to enemies within {ABIL_15_COM_0_RADI_FEET}.  For the next {COM_2_DURA_SECONDS}, the next different squig other than the one that just exploded will be summoned instantly. |
| 4 | 1856 | Strength In Numbas | 3059 | 2 | 6 -> OnEventTriggered |  | Any time one of your groupmates within 100 feet of you hits an enemy, there is a 25% chance that you will regain {COM_2_VAL0_COM_0_VAL0_TOD} points of Morale over {COM_2_VAL0_COM_0_DURA_SECONDS}.<BR>This effect will not trigger more than once every 3 seconds. |
| 6 | 5 | KABOOM! | 343 | 6 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 6 | 608 | Pulverizing Strike | 343 | 3 | 3 -> OnPreviousComponentApplied |  | Both you and your target are held tightly in place for {COM_1_DURA_SECONDS}, and neither one of you can move.<BR>This effect can not be dispelled or broken. |
| 6 | 648 | Hastened Punishment | 343 | 3 | 3 -> OnPreviousComponentApplied | Damage | A mid-range attack that deals {COM_0_VAL0_DAMAGE} and knocks targeted player away. Monsters will be knocked down. |
| 6 | 670 | Grievous Harm | 343 | 3 | 3 -> OnPreviousComponentApplied | Damage | A focused blast that deals {COM_0_VAL0_DAMAGE} and knocks the targeted player away. Monsters will be knocked down. |
| 6 | 672 | Unleash the Winds | 343 | 4 | 3 -> OnPreviousComponentApplied | Damage | Deals {COM_0_VAL0_DAMAGE} to all enemies within {COM_0_RADI_FEET}, and knocks them away from you. |
| 6 | 1010 | Arrow Attack - Strong | 14217 | 0 | 1 -> OnApply |  | Your pet doesn't have enough Morale |

## APPLY_ABILITY :: ExtData[1].Val7

- Operation: `APPLY_ABILITY` (`23`)
- Field: `ExtData[1].Val7`
- Ranks: global `3`, area `3`
- Priority: `Critical` `207`
- Example ability: `3`

Summary: APPLY_ABILITY ext-data payload-B field for the slot-local embedded payload.

Evidence: Confidence: Structural. Non-zero rows: 445. Distinct values: 24. Tags: Damage, Disarm, Heal, Immunity, Knockback, Knockdown, Root, Silence, Snare, Stagger, Stun. Samples: 1, 10, 100, 15, 1800, 2, 20, 2000, 22, 249, 25, 29.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | 133 | 133 | 19 | 1365, 1370, 1418, 1435, 1465, 1519, 1520, 1531, 1540, 1542, 1544, 1589 | 3 -> OnPreviousComponentApplied x183, 1 -> OnApply x105, 6 -> OnEventTriggered x30, 7 -> OnPreviousComponentTick x10, 8 -> OnPreviousComponentBuffTick x6, 5 -> OnBuffEnded x3 | Damage x117, Heal x22, Root x16, Snare x7, Silence x4, Knockback x3, Disarm x2, Stagger x2 | ExtData[1].Val4=8 (131/133, 98%); ExtData[0].Val4=8 (128/133, 96%); ExtData[1].Val3=1 (126/133, 94%); ExtData[0].Val1=2 (101/133, 75%) |
| 5 | 126 | 126 | 24 | 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10009, 10010, 10011, 10013 | 6 -> OnEventTriggered x124 | Damage x105, Heal x2 | ExtData[1].Val1=1 (126/126, 100%); ExtData[1].Val4=8 (126/126, 100%); ExtData[1].Val2=9 (122/126, 96%); ExtData[1].Val3=4 (122/126, 96%); ExtData[1].Val6=100 (121/126, 96%) |
| 10 | 55 | 55 | 24 | 10145, 10146, 10147, 10148, 10149, 10150, 10151, 10152, 10153, 10154, 10155, 10164 | 6 -> OnEventTriggered x54, 3 -> OnPreviousComponentApplied x1 | Damage x2, Heal x2 | ExtData[1].Val1=1 (55/55, 100%); ExtData[1].Val2=9 (55/55, 100%); ExtData[1].Val3=4 (55/55, 100%); ExtData[1].Val4=8 (55/55, 100%); ExtData[1].Val6=100 (55/55, 100%) |
| 2 | 49 | 49 | 24 | 10188, 10189, 10190, 10191, 10192, 10193, 10194, 10195, 10196, 10197, 10198, 10200 | 6 -> OnEventTriggered x48 | Damage x45 | ExtData[1].Val1=1 (49/49, 100%); ExtData[1].Val4=8 (49/49, 100%); ExtData[0].Val4=8 (48/49, 97%); ExtData[1].Val2=9 (47/49, 95%); ExtData[1].Val3=4 (47/49, 95%) |
| 25 | 25 | 25 | 24 | 1432, 1438, 1439, 1447, 1449, 1461, 1462, 1463, 1638, 1639, 1750, 1751 | 3 -> OnPreviousComponentApplied x19, 6 -> OnEventTriggered x14, 10 -> OnBuffEndedRemoved x4, 1 -> OnApply x1 | Damage x29, Heal x4, Snare x2, Knockback x1 | ExtData[1].Val4=8 (25/25, 100%); ExtData[1].Val2=9 (24/25, 96%); ExtData[1].Val3=4 (24/25, 96%); ExtData[0].Val4=8 (24/25, 96%); ExtData[1].Val6=100 (23/25, 92%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 1 | 5 | KABOOM! | 343 | 6 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 1 | 608 | Pulverizing Strike | 343 | 3 | 3 -> OnPreviousComponentApplied |  | Both you and your target are held tightly in place for {COM_1_DURA_SECONDS}, and neither one of you can move.<BR>This effect can not be dispelled or broken. |
| 1 | 648 | Hastened Punishment | 343 | 3 | 3 -> OnPreviousComponentApplied | Damage | A mid-range attack that deals {COM_0_VAL0_DAMAGE} and knocks targeted player away. Monsters will be knocked down. |
| 1 | 670 | Grievous Harm | 343 | 3 | 3 -> OnPreviousComponentApplied | Damage | A focused blast that deals {COM_0_VAL0_DAMAGE} and knocks the targeted player away. Monsters will be knocked down. |
| 1 | 672 | Unleash the Winds | 343 | 4 | 3 -> OnPreviousComponentApplied | Damage | Deals {COM_0_VAL0_DAMAGE} to all enemies within {COM_0_RADI_FEET}, and knocks them away from you. |
| 1 | 1365 | Ain't Done Yet! | 343 | 4 | 3 -> OnPreviousComponentApplied | Damage | A dismissive strike that deals {COM_0_VAL0_DAMAGE} and knocks the enemy away. |
| 5 | 9435 | For the Hag Queen! | 11045 | 5 | 6 -> OnEventTriggered |  | Your Frenzies will now have a 10% chance per point spent to knock the enemy down for {COM_1_VAL0_COM_0_DURA_SECONDS}. |
| 5 | 10001 | Hemorrage I | 10254 | 1 | 6 -> OnEventTriggered | Damage | Hemorrage I - On Hit: 5% chance to bleed target for {ABIL_10481_COM_0_VAL0_TOD_DAMAGE} over {ABIL_10481_COM_0_DURA_SECONDS}. |
| 5 | 10002 | Hemorrage II | 10255 | 1 | 6 -> OnEventTriggered | Damage | Hemorrage II - On Hit: 5% chance to bleed target for {ABIL_10482_COM_0_VAL0_TOD_DAMAGE} over {ABIL_10482_COM_0_DURA_SECONDS}. |
| 5 | 10003 | Hemorrage III | 10256 | 1 | 6 -> OnEventTriggered | Damage | Hemorrage III - On Hit: 5% chance to bleed target for {ABIL_10483_COM_0_VAL0_TOD_DAMAGE} over {ABIL_10483_COM_0_DURA_SECONDS}. |
| 5 | 10004 | Hemorrage IV | 10257 | 1 | 6 -> OnEventTriggered | Damage | Hemorrage IV - On Hit: 5% chance to bleed target for {ABIL_10484_COM_0_VAL0_TOD_DAMAGE} over {ABIL_10484_COM_0_DURA_SECONDS}. |
| 5 | 10005 | Hemorrage V | 10258 | 1 | 6 -> OnEventTriggered | Damage | Hemorrage V - On Hit: 5% chance to bleed target for {ABIL_10485_COM_0_VAL0_TOD_DAMAGE} over {ABIL_10485_COM_0_DURA_SECONDS}. |
| 10 | 8471 | Rend Winds | 8479 | 1 | 3 -> OnPreviousComponentApplied | Damage | Tears at your foe with fierce winds, hitting them three times for a total of {COM_0_VAL0_TOD_SPIRITDAMAGE}. |
| 10 | 10145 | Dissolve I | 10342 | 1 | 6 -> OnEventTriggered |  | Dissolve I - On Hit: 10% chance to reduce target's armor by {ABIL_10569_COM_0_VAL0} for {ABIL_10569_COM_0_DURA_SECONDS}. |
| 10 | 10146 | Dissolve II | 10343 | 1 | 6 -> OnEventTriggered |  | Dissolve II - On Hit: 10% chance to reduce target's armor by {ABIL_10570_COM_0_VAL0} for {ABIL_10570_COM_0_DURA_SECONDS}. |
| 10 | 10147 | Dissolve III | 10344 | 1 | 6 -> OnEventTriggered |  | Dissolve III - On Hit: 10% chance to reduce target's armor by {ABIL_10571_COM_0_VAL0} for {ABIL_10571_COM_0_DURA_SECONDS}. |
| 10 | 10148 | Dissolve IV | 10345 | 1 | 6 -> OnEventTriggered |  | Dissolve IV - On Hit: 10% chance to reduce target's armor by {ABIL_10572_COM_0_VAL0} for {ABIL_10572_COM_0_DURA_SECONDS}. |
| 10 | 10149 | Dissolve V | 10346 | 1 | 6 -> OnEventTriggered |  | Dissolve V - On Hit: 10% chance to reduce target's armor by {ABIL_10573_COM_0_VAL0} for {ABIL_10573_COM_0_DURA_SECONDS}. |

## APPLY_ABILITY :: ExtData[0].Val1

- Operation: `APPLY_ABILITY` (`23`)
- Field: `ExtData[0].Val1`
- Ranks: global `4`, area `4`
- Priority: `Critical` `205`
- Example ability: `3`

Summary: APPLY_ABILITY ext-data branch selector for the slot-local embedded payload.

Evidence: Confidence: Structural. Non-zero rows: 1124. Distinct values: 2. Tags: Damage, Disarm, Heal, Immunity, Knockback, Knockdown, Root, Silence, Snare, Stagger, Stun. Samples: 1, 2.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 2 | 716 | 716 | 19 | 10, 183, 230, 232, 233, 3, 379, 383, 410, 5, 526, 550 | 3 -> OnPreviousComponentApplied x930, 6 -> OnEventTriggered x299, 1 -> OnApply x124, 7 -> OnPreviousComponentTick x32, 5 -> OnBuffEnded x25, 8 -> OnPreviousComponentBuffTick x17 | Damage x416, Heal x109, Silence x31, Root x18, Disarm x13, Snare x9, Stagger x7, Knockback x6 | ExtData[0].Val4=8 (715/716, 99%); ExtData[0].Val3=1 (627/716, 87%); ExtData[0].Val7=1 (602/716, 84%) |
| 1 | 408 | 408 | 18 | 1010, 1011, 1364, 1366, 1397, 1432, 260, 264, 266, 278, 31, 311 | 1 -> OnApply x208, 6 -> OnEventTriggered x146, 3 -> OnPreviousComponentApplied x131, 7 -> OnPreviousComponentTick x22, 10 -> OnBuffEndedRemoved x12, 8 -> OnPreviousComponentBuffTick x7 | Damage x235, Heal x56, Snare x10, Silence x3, Stagger x2, Disarm x1 | ExtData[0].Val4=8 (403/408, 98%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 2 | 3 | Bounce | 329 | 2 | 3 -> OnPreviousComponentApplied | Damage | You cause your Battle Squig to bounce, dealing {COM_0_VAL0_DAMAGE} and knocking your target down for {COM_1_DURA_SECONDS}. |
| 2 | 5 | KABOOM! | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 2 | 5 | KABOOM! | 343 | 6 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 2 | 6 | ;/end | 329 | 1 | 3 -> OnPreviousComponentApplied | Damage | Your Squig bounces on it's target dealing {COM_2_VAL0_DAMAGE}, and knocking them down for {COM_0_DURA_SECONDS}. |
| 2 | 9 | Poisoned Spine | 1699 | 1 | 3 -> OnPreviousComponentApplied | Damage | Your Squig gores its target, dealing {COM_0_VAL0_DAMAGE} and an additional {ABIL_3881_COM_0_VAL0_TOD_DAMAGE} over {ABIL_3881_COM_0_DURA_SECONDS}. |
| 2 | 10 | Gore | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Squig slams it's head in to it's target, dealing {COM_0_VAL0_DAMAGE} and knocking them back. |
| 1 | 31 | Detonation | 3447 | 1 | 3 -> OnPreviousComponentApplied |  |  |
| 1 | 34 | Bite | 3113 | 2 | 8 -> OnPreviousComponentBuffTick |  |  |
| 1 | 260 | Scathe | 3108 | 4 | 5 -> OnBuffEnded |  | You are building a combo! |
| 1 | 264 | Cunning Rumination  | 1914 | 1 | 1 -> OnApply |  |  |
| 1 | 264 | Cunning Rumination  | 1915 | 2 | 1 -> OnApply |  |  |
| 1 | 266 | Quell Suffering | 1916 | 1 | 1 -> OnApply |  |  |

## APPLY_ABILITY :: ExtData[0].Val4

- Operation: `APPLY_ABILITY` (`23`)
- Field: `ExtData[0].Val4`
- Ranks: global `5`, area `5`
- Priority: `Critical` `205`
- Example ability: `3`

Summary: APPLY_ABILITY ext-data family marker for the slot-local embedded payload.

Evidence: Confidence: Structural. Non-zero rows: 1124. Distinct values: 2. Tags: Damage, Disarm, Heal, Immunity, Knockback, Knockdown, Root, Silence, Snare, Stagger, Stun. Samples: 8, 9.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 8 | 1118 | 1118 | 22 | 10, 183, 230, 232, 233, 260, 264, 266, 278, 3, 31, 311 | 3 -> OnPreviousComponentApplied x900, 6 -> OnEventTriggered x445, 1 -> OnApply x290, 7 -> OnPreviousComponentTick x54, 5 -> OnBuffEnded x25, 8 -> OnPreviousComponentBuffTick x24 | Damage x565, Heal x164, Silence x32, Snare x14, Disarm x13, Stagger x9, Knockdown x6, Immunity x5 | No strong non-zero companion fields were isolated for this value yet. |
| 9 | 6 | 6 | 23 | 1365, 1370, 1418, 1519, 1520, 1531, 1540, 1542, 1605, 1680, 1681, 1682 | 3 -> OnPreviousComponentApplied x161, 1 -> OnApply x42, 5 -> OnBuffEnded x2 | Damage x86, Root x15, Snare x5, Knockback x2, Silence x2, Disarm x1, Heal x1 | ExtData[0].Val1=1 (5/6, 83%); ExtData[1].Val1=1 (5/6, 83%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 8 | 3 | Bounce | 329 | 2 | 3 -> OnPreviousComponentApplied | Damage | You cause your Battle Squig to bounce, dealing {COM_0_VAL0_DAMAGE} and knocking your target down for {COM_1_DURA_SECONDS}. |
| 8 | 5 | KABOOM! | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 8 | 6 | ;/end | 329 | 1 | 3 -> OnPreviousComponentApplied | Damage | Your Squig bounces on it's target dealing {COM_2_VAL0_DAMAGE}, and knocking them down for {COM_0_DURA_SECONDS}. |
| 8 | 9 | Poisoned Spine | 1699 | 1 | 3 -> OnPreviousComponentApplied | Damage | Your Squig gores its target, dealing {COM_0_VAL0_DAMAGE} and an additional {ABIL_3881_COM_0_VAL0_TOD_DAMAGE} over {ABIL_3881_COM_0_DURA_SECONDS}. |
| 8 | 10 | Gore | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Squig slams it's head in to it's target, dealing {COM_0_VAL0_DAMAGE} and knocking them back. |
| 8 | 31 | Detonation | 3447 | 1 | 3 -> OnPreviousComponentApplied |  |  |
| 9 | 5 | KABOOM! | 343 | 6 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 9 | 608 | Pulverizing Strike | 343 | 3 | 3 -> OnPreviousComponentApplied |  | Both you and your target are held tightly in place for {COM_1_DURA_SECONDS}, and neither one of you can move.<BR>This effect can not be dispelled or broken. |
| 9 | 648 | Hastened Punishment | 343 | 3 | 3 -> OnPreviousComponentApplied | Damage | A mid-range attack that deals {COM_0_VAL0_DAMAGE} and knocks targeted player away. Monsters will be knocked down. |
| 9 | 670 | Grievous Harm | 343 | 3 | 3 -> OnPreviousComponentApplied | Damage | A focused blast that deals {COM_0_VAL0_DAMAGE} and knocks the targeted player away. Monsters will be knocked down. |
| 9 | 672 | Unleash the Winds | 343 | 4 | 3 -> OnPreviousComponentApplied | Damage | Deals {COM_0_VAL0_DAMAGE} to all enemies within {COM_0_RADI_FEET}, and knocks them away from you. |
| 9 | 1365 | Ain't Done Yet! | 343 | 4 | 3 -> OnPreviousComponentApplied | Damage | A dismissive strike that deals {COM_0_VAL0_DAMAGE} and knocks the enemy away. |

## CC :: FlagsRaw

- Operation: `CC` (`12`)
- Field: `FlagsRaw`
- Ranks: global `6`, area `6`
- Priority: `Critical` `199`
- Example ability: `3`

Summary: CC packed control bitfield for branch and behavior selection.

Evidence: Confidence: Structural. Non-zero rows: 582. Distinct values: 61. Tags: Damage, Disarm, Heal, Immunity, Knockdown, Root, Silence, Snare, Stun. Samples: 1, 1023, 1024, 12, 124, 126, 127, 128, 129, 137, 14, 16.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 2175 | 130 | 130 | 23 | 1369, 1384, 144, 1443, 1494, 1525, 1688, 1755, 1804, 19, 1908, 236 | 3 -> OnPreviousComponentApplied x191, 1 -> OnApply x86, 2 -> OnPreviousComponentEndCast x2, 5 -> OnBuffEnded x1 | Damage x30, Stun x5, Heal x2, Knockdown x2, Disarm x1, Immunity x1, Silence x1 | Value15=4 (130/130, 100%); ExtData[0].Val4=8 (106/130, 81%); ExtData[0].Val3=1 (104/130, 80%); ExtData[0].Val1=2 (98/130, 75%); ExtData[0].Val7=1 (98/130, 75%) |
| 2303 | 93 | 93 | 24 | 4340, 4664, 4665, 4666, 4667, 4668, 4669, 4670, 4671, 4672, 4673, 4674 | 1 -> OnApply x82, 5 -> OnBuffEnded x14, 3 -> OnPreviousComponentApplied x5, 6 -> OnEventTriggered x4, 10 -> OnBuffEndedRemoved x1 | No sampled context tags are available for this value yet. | ExtData[0].Val4=8 (81/93, 87%); ExtData[0].Val1=2 (78/93, 83%) |
| 8 | 87 | 87 | 24 | 1607, 1683, 1722, 1839, 3218, 3305, 3616, 3754, 3762, 3958, 4128, 4131 | 1 -> OnApply x56, 3 -> OnPreviousComponentApplied x30, 5 -> OnBuffEnded x3 | Silence x21, Damage x19, Heal x1 | ExtData[0].Val4=8 (81/87, 93%); ExtData[0].Val1=2 (80/87, 91%) |
| 1 | 73 | 73 | 23 | 122, 1370, 1418, 1519, 1681, 3631, 3941, 4023, 4032, 4093, 4196, 4197 | 1 -> OnApply x66, 3 -> OnPreviousComponentApplied x46, 5 -> OnBuffEnded x2, 0 -> OnEnd x1 | Root x20, Damage x11, Immunity x1, Knockdown x1 | Value15=4 (65/73, 89%) |
| 255 | 21 | 21 | 22 | 23705, 4022, 4073, 4091, 4094, 4126, 4130, 4133, 4209, 5008, 5011, 5015 | 1 -> OnApply x16, 3 -> OnPreviousComponentApplied x4, 5 -> OnBuffEnded x1, 7 -> OnPreviousComponentTick x1 | No sampled context tags are available for this value yet. | ExtData[0].Val4=8 (19/21, 90%); ExtData[0].Val1=2 (16/21, 76%); ExtData[0].Val3=1 (16/21, 76%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 2175 | 3 | Bounce | 3310 | 1 | 3 -> OnPreviousComponentApplied | Damage | You cause your Battle Squig to bounce, dealing {COM_0_VAL0_DAMAGE} and knocking your target down for {COM_1_DURA_SECONDS}. |
| 2175 | 6 | ;/end | 3446 | 0 | 1 -> OnApply | Damage | Your Squig bounces on it's target dealing {COM_2_VAL0_DAMAGE}, and knocking them down for {COM_0_DURA_SECONDS}. |
| 2175 | 19 | Git Em! | 1889 | 0 | 1 -> OnApply |  |  |
| 2175 | 144 | Poisoned Spine | 1348 | 0 | 1 -> OnApply |  | You've been knocked down |
| 2175 | 236 | Divine Protection | 3212 | 0 | 1 -> OnApply |  |  |
| 2175 | 237 | Alter Fate | 3212 | 0 | 1 -> OnApply |  |  |
| 2303 | 4340 | Auto2 | 4371 | 1 | 1 -> OnApply |  |  |
| 2303 | 4664 | greenskin RDPS flourish2 | 4612 | 0 | 1 -> OnApply |  |  |
| 2303 | 4665 | greenskin RDPS flourish3 | 4612 | 0 | 1 -> OnApply |  |  |
| 2303 | 4666 | Dwarf Tank acknowledge | 4612 | 0 | 1 -> OnApply |  |  |
| 2303 | 4667 | Dwarf Tank flourish1 | 4612 | 0 | 1 -> OnApply |  |  |
| 2303 | 4668 | Dwarf Tank flourish2 | 4612 | 0 | 1 -> OnApply |  |  |
| 8 | 885 | Quit Yer Squabblin' | 992 | 0 | 1 -> OnApply | Silence | All enemies within 30 feet are silenced for 7 seconds, making them unable to use magic. |
| 8 | 906 | Fling Choppa | 992 | 0 | 1 -> OnApply | Silence | All enemies within 30 feet are silenced for 7 seconds, making them unable to use magic. |
| 8 | 1607 | Ruin And Destruction | 3624 | 1 | 3 -> OnPreviousComponentApplied | Silence, Damage | Silences and wounds target for {COM_0_VAL0_TOD_ELEMENTALDAMAGE} over {COM_0_DURA_SECONDS}. |
| 8 | 1683 | Guilty Soul | 1524 | 3 | 3 -> OnPreviousComponentApplied | Silence, Damage | Deals {COM_1_VAL0_DAMAGE}. Silences target for {COM_3_DURA_SECONDS}. |
| 8 | 1722 | Suppression | 1556 | 1 | 3 -> OnPreviousComponentApplied | Silence, Damage | All targets within {COM_0_RADI_FEET} of you take {COM_0_VAL0_DAMAGE} and are Silenced, unable to use Magic for {COM_1_DURA_SECONDS}. |
| 8 | 1839 | Choking Arrer | 1817 | 1 | 1 -> OnApply | Silence, Damage | A Crippling shot that deals {COM_0_VAL0_DAMAGE} and silences your target for {COM_1_DURA_SECONDS}, making them unable to use magic. |

## APPLY_ABILITY :: ExtData[1].Val3

- Operation: `APPLY_ABILITY` (`23`)
- Field: `ExtData[1].Val3`
- Ranks: global `10`, area `7`
- Priority: `Critical` `185`
- Example ability: `3`

Summary: APPLY_ABILITY ext-data profile selector for the slot-local embedded payload.

Evidence: Confidence: Structural. Non-zero rows: 612. Distinct values: 5. Tags: Damage, Disarm, Heal, Immunity, Knockback, Knockdown, Root, Silence, Snare, Stagger, Stun. Samples: 1, 4, 5, 6, 7.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 4 | 298 | 298 | 19 | 1363, 1432, 1438, 1439, 1443, 1447, 1449, 1459, 1461, 183, 264, 266 | 6 -> OnEventTriggered x244, 3 -> OnPreviousComponentApplied x46, 1 -> OnApply x19, 8 -> OnPreviousComponentBuffTick x8, 10 -> OnBuffEndedRemoved x6, 7 -> OnPreviousComponentTick x2 | Damage x216, Heal x9, Snare x4 | ExtData[1].Val4=8 (298/298, 100%); ExtData[1].Val1=1 (289/298, 96%); ExtData[1].Val2=9 (272/298, 91%); ExtData[1].Val6=100 (260/298, 87%); ExtData[0].Val4=8 (256/298, 85%) |
| 1 | 269 | 269 | 19 | 10, 1010, 1051, 1062, 1365, 1370, 1418, 1435, 264, 334, 5, 608 | 3 -> OnPreviousComponentApplied x247, 1 -> OnApply x164, 6 -> OnEventTriggered x85, 7 -> OnPreviousComponentTick x11, 5 -> OnBuffEnded x8, 8 -> OnPreviousComponentBuffTick x5 | Damage x169, Heal x42, Root x17, Snare x8, Knockback x5, Silence x5, Disarm x2, Immunity x2 | ExtData[1].Val4=8 (267/269, 99%); ExtData[0].Val4=8 (263/269, 97%) |
| 6 | 25 | 25 | 21 | 1602, 1906, 3007, 5513, 8081, 8083, 8085, 8091, 8096, 8097, 8098, 8100 | 1 -> OnApply x29, 3 -> OnPreviousComponentApplied x7, 7 -> OnPreviousComponentTick x2 | Damage x30, Heal x5, Silence x1, Snare x1 | ExtData[1].Val1=1 (24/25, 96%); ExtData[0].Val4=8 (23/25, 92%); ExtData[1].Val4=8 (23/25, 92%) |
| 7 | 12 | 12 | 13 | 1010, 1011, 14024, 14506, 14509, 15996, 8113, 8116, 9404, 9410, 9422, 9425 | 1 -> OnApply x17 | Damage x7, Heal x1 | ExtData[1].Val1=1 (12/12, 100%); ExtData[1].Val4=8 (12/12, 100%); ExtData[0].Val4=8 (10/12, 83%); Value[1]=1 (9/12, 75%) |
| 5 | 8 | 8 | 5 | 24660, 24795, 24798, 9328, 9580 | 8 -> OnPreviousComponentBuffTick x3, 3 -> OnPreviousComponentApplied x2 | Damage x2, Knockback x1 | ExtData[0].Val1=2 (8/8, 100%); ExtData[0].Val4=8 (8/8, 100%); ExtData[1].Val4=8 (8/8, 100%); ExtData[1].Val1=1 (7/8, 87%); ExtData[1].Val7=1 (6/8, 75%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 4 | 183 | Pierce | 493 | 0 | 1 -> OnApply |  |  |
| 4 | 264 | Cunning Rumination  | 1914 | 1 | 1 -> OnApply |  |  |
| 4 | 266 | Quell Suffering | 1916 | 1 | 1 -> OnApply |  |  |
| 4 | 311 | Unwavering Faith | 8849 | 1 | 7 -> OnPreviousComponentTick |  |  |
| 4 | 320 | Soul Reaping Passive Build | 11758 | 1 | 7 -> OnPreviousComponentTick |  |  |
| 4 | 334 | Barrier Of Dementia | 12084 | 1 | 1 -> OnApply |  |  |
| 1 | 5 | KABOOM! | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 1 | 5 | KABOOM! | 343 | 6 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 1 | 10 | Gore | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Squig slams it's head in to it's target, dealing {COM_0_VAL0_DAMAGE} and knocking them back. |
| 1 | 264 | Cunning Rumination  | 1915 | 2 | 1 -> OnApply |  |  |
| 1 | 334 | Barrier Of Dementia | 12085 | 2 | 1 -> OnApply |  |  |
| 1 | 608 | Pulverizing Strike | 343 | 3 | 3 -> OnPreviousComponentApplied |  | Both you and your target are held tightly in place for {COM_1_DURA_SECONDS}, and neither one of you can move.<BR>This effect can not be dispelled or broken. |
| 6 | 1602 | Explosive Force | 3074 | 2 | 3 -> OnPreviousComponentApplied |  | Your rune cleanses an ally, removing one Curse or Ailment. |
| 6 | 1602 | Explosive Force | 3280 | 3 | 3 -> OnPreviousComponentApplied |  | Your rune cleanses an ally, removing one Curse or Ailment. |
| 6 | 1906 | Bomb Explode Monster Ability | 3205 | 1 | 3 -> OnPreviousComponentApplied | Damage | You cleanse an ally, removing one Curse or Ailment.  If an effect is removed, your ally will absorb up to {COM_4_VAL1_DAMAGE} over {COM_4_DURA_SECONDS}. |
| 6 | 1906 | Bomb Explode Monster Ability | 3279 | 3 | 3 -> OnPreviousComponentApplied | Damage | You cleanse an ally, removing one Curse or Ailment.  If an effect is removed, your ally will absorb up to {COM_4_VAL1_DAMAGE} over {COM_4_DURA_SECONDS}. |
| 6 | 3007 | Heartstopping Snort | 9889 | 0 | 1 -> OnApply |  |  |
| 6 | 5513 | Fate's Whirlwind | 5561 | 1 | 7 -> OnPreviousComponentTick |  |  |

## APPLY_ABILITY :: ExtData[1].Val1

- Operation: `APPLY_ABILITY` (`23`)
- Field: `ExtData[1].Val1`
- Ranks: global `11`, area `8`
- Priority: `Critical` `181`
- Example ability: `3`

Summary: APPLY_ABILITY ext-data branch selector for the slot-local embedded payload.

Evidence: Confidence: Structural. Non-zero rows: 612. Distinct values: 3. Tags: Damage, Disarm, Heal, Immunity, Knockback, Knockdown, Root, Silence, Snare, Stagger, Stun. Samples: 1, 2, 6.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | 503 | 503 | 21 | 10, 1010, 1011, 1051, 1062, 183, 264, 266, 311, 320, 334, 335 | 6 -> OnEventTriggered x296, 1 -> OnApply x179, 3 -> OnPreviousComponentApplied x132, 8 -> OnPreviousComponentBuffTick x15, 7 -> OnPreviousComponentTick x7, 10 -> OnBuffEndedRemoved x4 | Damage x354, Heal x34, Snare x8, Knockback x4, Silence x4, Root x2, Disarm x1, Immunity x1 | ExtData[1].Val4=8 (499/503, 99%); ExtData[0].Val4=8 (452/503, 89%) |
| 2 | 108 | 108 | 22 | 1365, 1370, 1418, 1435, 1519, 1520, 1531, 1539, 1540, 1542, 1544, 1601 | 3 -> OnPreviousComponentApplied x170, 1 -> OnApply x50, 6 -> OnEventTriggered x33, 5 -> OnBuffEnded x8, 7 -> OnPreviousComponentTick x8, 10 -> OnBuffEndedRemoved x4 | Damage x70, Heal x23, Root x15, Snare x5, Knockback x2, Silence x2, Stagger x2, Disarm x1 | ExtData[1].Val4=8 (108/108, 100%); ExtData[0].Val4=8 (107/108, 99%); ExtData[1].Val3=1 (98/108, 90%); ExtData[0].Val1=2 (81/108, 75%) |
| 6 | 1 | 1 | 0 |  | No sampled trigger evidence is available for this value yet. | No sampled context tags are available for this value yet. | Value[0]=3398 (1/1, 100%); ExtData[0].Val1=2 (1/1, 100%); ExtData[0].Val2=2 (1/1, 100%); ExtData[0].Val3=1 (1/1, 100%); ExtData[0].Val4=8 (1/1, 100%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 1 | 5 | KABOOM! | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 1 | 10 | Gore | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Squig slams it's head in to it's target, dealing {COM_0_VAL0_DAMAGE} and knocking them back. |
| 1 | 183 | Pierce | 493 | 0 | 1 -> OnApply |  |  |
| 1 | 264 | Cunning Rumination  | 1914 | 1 | 1 -> OnApply |  |  |
| 1 | 264 | Cunning Rumination  | 1915 | 2 | 1 -> OnApply |  |  |
| 1 | 266 | Quell Suffering | 1916 | 1 | 1 -> OnApply |  |  |
| 2 | 5 | KABOOM! | 343 | 6 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 2 | 608 | Pulverizing Strike | 343 | 3 | 3 -> OnPreviousComponentApplied |  | Both you and your target are held tightly in place for {COM_1_DURA_SECONDS}, and neither one of you can move.<BR>This effect can not be dispelled or broken. |
| 2 | 648 | Hastened Punishment | 343 | 3 | 3 -> OnPreviousComponentApplied | Damage | A mid-range attack that deals {COM_0_VAL0_DAMAGE} and knocks targeted player away. Monsters will be knocked down. |
| 2 | 670 | Grievous Harm | 343 | 3 | 3 -> OnPreviousComponentApplied | Damage | A focused blast that deals {COM_0_VAL0_DAMAGE} and knocks the targeted player away. Monsters will be knocked down. |
| 2 | 672 | Unleash the Winds | 343 | 4 | 3 -> OnPreviousComponentApplied | Damage | Deals {COM_0_VAL0_DAMAGE} to all enemies within {COM_0_RADI_FEET}, and knocks them away from you. |
| 2 | 779 | ;Rune Priest Spec Abilities | 1192 | 1 | 6 -> OnEventTriggered | Damage | Any time you are attacked, there is a 25% chance that you will become protected by a magical barrier for up to {COM_1_VAL0_COM_0_DURA_SECONDS}, which will absorb up to {COM_1_VAL0_COM_0_VAL1_DAMAGE}.<BR>This effect will not trigger more than once every 3 seconds. |

## CC :: ExtData[1].Val7

- Operation: `CC` (`12`)
- Field: `ExtData[1].Val7`
- Ranks: global `12`, area `9`
- Priority: `Critical` `181`
- Example ability: `3`

Summary: CC ext-data payload-B field for the slot-local control payload.

Evidence: Confidence: Structural. Non-zero rows: 169. Distinct values: 19. Tags: Damage, Disarm, Heal, Immunity, Knockdown, Root, Silence, Snare, Stun. Samples: 1, 100, 168, 20, 200, 201, 2101, 24, 25, 3, 30, 33.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | 128 | 128 | 24 | 122, 124, 1443, 1755, 3218, 3631, 3650, 4281, 4336, 4337, 4800, 4802 | 3 -> OnPreviousComponentApplied x148, 1 -> OnApply x75, 5 -> OnBuffEnded x11, 7 -> OnPreviousComponentTick x2 | Damage x12, Silence x6, Root x2, Disarm x1, Snare x1 | ExtData[1].Val4=8 (128/128, 100%); ExtData[1].Val3=1 (124/128, 96%); ExtData[0].Val4=8 (116/128, 90%); ExtData[0].Val1=2 (114/128, 89%); ExtData[1].Val1=2 (113/128, 88%) |
| 3 | 10 | 10 | 18 | 12007, 12008, 12017, 12024, 12028, 12037, 12066, 12075, 12077, 12086, 12096, 12097 | 3 -> OnPreviousComponentApplied x238, 1 -> OnApply x37 | No sampled context tags are available for this value yet. | ExtData[0].Val1=1 (10/10, 100%); ExtData[0].Val2=88 (10/10, 100%); ExtData[0].Val3=1 (10/10, 100%); ExtData[0].Val4=9 (10/10, 100%); ExtData[0].Val7=2 (10/10, 100%) |
| 200 | 4 | 4 | 3 | 13314, 144, 24578 | 1 -> OnApply x3 | No sampled context tags are available for this value yet. | Value15=4 (4/4, 100%); ExtData[0].Val4=8 (4/4, 100%); ExtData[1].Val2=4 (4/4, 100%); ExtData[1].Val3=4 (4/4, 100%); ExtData[1].Val4=8 (4/4, 100%) |
| 50 | 4 | 4 | 1 | 5707 | 3 -> OnPreviousComponentApplied x1 | No sampled context tags are available for this value yet. | ExtData[0].Val1=2 (4/4, 100%); ExtData[0].Val2=2 (4/4, 100%); ExtData[0].Val3=1 (4/4, 100%); ExtData[0].Val4=8 (4/4, 100%); ExtData[0].Val7=1 (4/4, 100%) |
| 100 | 4 | 4 | 0 |  | No sampled trigger evidence is available for this value yet. | No sampled context tags are available for this value yet. | FlagsRaw=2175 (4/4, 100%); Value15=4 (4/4, 100%); ExtData[0].Val4=8 (4/4, 100%); ExtData[1].Val1=1 (4/4, 100%); ExtData[1].Val4=8 (4/4, 100%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 1 | 6 | ;/end | 3446 | 0 | 1 -> OnApply | Damage | Your Squig bounces on it's target dealing {COM_2_VAL0_DAMAGE}, and knocking them down for {COM_0_DURA_SECONDS}. |
| 1 | 122 | Claw Sweep | 1194 | 0 | 1 -> OnApply | Root | AE Root |
| 1 | 124 | Lion's Roar | 1196 | 0 | 1 -> OnApply | Silence, Disarm | Silence and Disarm |
| 1 | 1443 | Sun's Blessing | 3715 | 1 | 3 -> OnPreviousComponentApplied | Damage | You render an incapacitating blow to your target, dealing {COM_0_VAL0_DAMAGE} and knocking them down for {COM_1_DURA_SECONDS}. This attack is undefendable. |
| 1 | 1755 | Embrace The Winds | 18273 | 1 | 3 -> OnPreviousComponentApplied | Damage | Deals {COM_0_VAL0_DAMAGE} and knocks down the target for {COM_1_DURA_SECONDS}. The attack is undefendable. |
| 1 | 3218 | Grave Dust | 3202 | 0 | 1 -> OnApply |  |  |
| 3 | 12007 | Gasp of the Unliving | 21008 | 2 | 3 -> OnPreviousComponentApplied |  |  |
| 3 | 12008 | Tormenting Wail | 21002 | 1 | 1 -> OnApply |  |  |
| 3 | 12017 | Bat Screech | 21008 | 4 | 3 -> OnPreviousComponentApplied |  |  |
| 3 | 12017 | Bat Screech | 21008 | 5 | 3 -> OnPreviousComponentApplied |  |  |
| 3 | 12024 | Sever Limb | 21010 | 1 | 1 -> OnApply |  |  |
| 3 | 12028 | Brutal Stomp | 21002 | 1 | 1 -> OnApply |  |  |
| 200 | 144 | Poisoned Spine | 1348 | 0 | 1 -> OnApply |  | You've been knocked down |
| 200 | 13314 | Curse of Verrimus | 13340 | 1 | 1 -> OnApply |  |  |
| 200 | 24578 | Divine Wrath | 26335 | 0 | 1 -> OnApply |  |  |
| 50 | 5707 | Raven's Wing | 5901 | 2 | 3 -> OnPreviousComponentApplied |  |  |

## APPLY_ABILITY :: ExtData[1].Val4

- Operation: `APPLY_ABILITY` (`23`)
- Field: `ExtData[1].Val4`
- Ranks: global `14`, area `10`
- Priority: `Critical` `179`
- Example ability: `3`

Summary: APPLY_ABILITY ext-data family marker for the slot-local embedded payload.

Evidence: Confidence: Structural. Non-zero rows: 612. Distinct values: 2. Tags: Damage, Disarm, Heal, Immunity, Knockback, Knockdown, Root, Silence, Snare, Stagger, Stun. Samples: 8, 9.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 8 | 608 | 608 | 18 | 10, 183, 264, 266, 311, 320, 334, 335, 5, 608, 648, 670 | 6 -> OnEventTriggered x329, 3 -> OnPreviousComponentApplied x302, 1 -> OnApply x187, 8 -> OnPreviousComponentBuffTick x16, 7 -> OnPreviousComponentTick x15, 10 -> OnBuffEndedRemoved x8 | Damage x382, Heal x56, Root x17, Snare x12, Knockback x6, Silence x5, Immunity x2, Knockdown x2 | ExtData[0].Val4=8 (560/608, 92%); ExtData[1].Val1=1 (499/608, 82%) |
| 9 | 4 | 4 | 24 | 8158, 8159, 8160, 8163, 8165, 8166, 8169, 8170, 8171, 8173, 8174, 8176 | 1 -> OnApply x42 | Damage x42, Disarm x1, Heal x1, Silence x1, Snare x1 | ExtData[0].Val1=1 (4/4, 100%); ExtData[0].Val4=9 (4/4, 100%); ExtData[1].Val1=1 (4/4, 100%); ExtData[2].Val1=1 (4/4, 100%); ExtData[2].Val4=9 (4/4, 100%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 8 | 5 | KABOOM! | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 8 | 5 | KABOOM! | 343 | 6 | 3 -> OnPreviousComponentApplied | Damage | Your Battle Squig explodes, causing you to be launched upward out of your Squiggy armor. All enemies within {COM_1_RADI_FEET} take {COM_3_VAL0_DAMAGE} and are knocked back. |
| 8 | 10 | Gore | 142 | 2 | 3 -> OnPreviousComponentApplied | Damage | Your Squig slams it's head in to it's target, dealing {COM_0_VAL0_DAMAGE} and knocking them back. |
| 8 | 183 | Pierce | 493 | 0 | 1 -> OnApply |  |  |
| 8 | 264 | Cunning Rumination  | 1914 | 1 | 1 -> OnApply |  |  |
| 8 | 264 | Cunning Rumination  | 1915 | 2 | 1 -> OnApply |  |  |
| 9 | 8158 | Ignite | 8588 | 0 | 1 -> OnApply | Damage | A Hex which causes your target to burst into flames, dealing {COM_2_VAL0_TOD_ELEMENTALDAMAGE} to them over {COM_2_DURA_SECONDS}. |
| 9 | 8159 | Fireball | 8588 | 0 | 1 -> OnApply | Damage | You conjure up a fiery missile which slams into your target, dealing {COM_2_VAL0_ELEMENTALDAMAGE}. |
| 9 | 8160 | Sear | 8588 | 0 | 1 -> OnApply | Damage | Causes a pillar of scalding gas to erupt from the ground, dealing {COM_2_VAL0_ELEMENTALDAMAGE} to your target. |
| 9 | 8163 | Scorched Earth | 8588 | 0 | 1 -> OnApply | Damage | You send a blast of heat into the ground around you, dealing {COM_2_VAL0_CORPOREALDAMAGE} to all enemies within {COM_2_RADI_FEET}. |
| 9 | 8165 | Boiling Blood | 8588 | 0 | 1 -> OnApply | Damage | Reduces your target's Willpower by {COM_2_VAL0} for {COM_2_DURA_SECONDS}.  When the effect ends, the victim will suffer {COM_3_VAL0_CORPOREALDAMAGE}. |
| 9 | 8166 | Fiery Blast | 8588 | 0 | 1 -> OnApply | Damage | You fling a massive ball of fire, dealing {COM_4_VAL0_ELEMENTALDAMAGE} to your target and all other enemies within {COM_4_RADI_FEET} of them. |

## APPLY_ABILITY :: FlagsRaw

- Operation: `APPLY_ABILITY` (`23`)
- Field: `FlagsRaw`
- Ranks: global `15`, area `11`
- Priority: `Critical` `177`
- Example ability: `3`

Summary: Packed raw flag bitfield for the component payload.

Evidence: Confidence: Structural. Non-zero rows: 260. Distinct values: 12. Tags: Damage, Disarm, Heal, Immunity, Knockback, Knockdown, Root, Silence, Snare, Stagger, Stun. Samples: 1, 12, 16, 18, 2, 24, 28, 3, 4, 5, 7, 8.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 16 | 79 | 79 | 16 | 1020, 15146, 24511, 24544, 24545, 24546, 24547, 24548, 24549, 24550, 24551, 24552 | 1 -> OnApply x66, 3 -> OnPreviousComponentApplied x6, 5 -> OnBuffEnded x3, 6 -> OnEventTriggered x2, 7 -> OnPreviousComponentTick x1 | Damage x4, Heal x1 | ExtData[0].Val4=8 (75/79, 94%); ExtData[1].Val3=1 (70/79, 88%); ExtData[1].Val4=8 (70/79, 88%); Radius=2000 (64/79, 81%); MaxTargets=1 (64/79, 81%) |
| 4 | 32 | 32 | 24 | 10887, 10888, 14288, 14289, 14290, 14291, 14292, 1598, 1608, 1828, 1852, 1908 | 1 -> OnApply x21, 3 -> OnPreviousComponentApplied x16, 10 -> OnBuffEndedRemoved x2, 6 -> OnEventTriggered x1 | Heal x9, Damage x3 | No strong non-zero companion fields were isolated for this value yet. |
| 1 | 32 | 32 | 19 | 10349, 24826, 3404, 3405, 3488, 3489, 3597, 3784, 8015, 8020, 8257, 8271 | 6 -> OnEventTriggered x9, 1 -> OnApply x4, 3 -> OnPreviousComponentApplied x4, 8 -> OnPreviousComponentBuffTick x3 | Damage x8, Heal x4 | No strong non-zero companion fields were isolated for this value yet. |
| 7 | 30 | 30 | 17 | 10846, 10847, 10848, 10849, 10850, 10851, 10852, 10853, 10854, 10855, 10856, 10857 | 1 -> OnApply x21, 3 -> OnPreviousComponentApplied x21, 6 -> OnEventTriggered x10 | Damage x31 | No strong non-zero companion fields were isolated for this value yet. |
| 12 | 28 | 28 | 24 | 13171, 133, 13306, 13308, 13316, 13319, 13328, 13345, 13387, 13400, 13409, 13440 | 1 -> OnApply x23, 3 -> OnPreviousComponentApplied x6, 5 -> OnBuffEnded x2, 7 -> OnPreviousComponentTick x1 | Damage x6, Heal x1 | ExtData[0].Val1=2 (21/28, 75%); ExtData[0].Val4=8 (21/28, 75%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 16 | 1020 | Fire - Line Nuke | 2130 | 1 | 3 -> OnPreviousComponentApplied |  | Construct a temporary mailbox to allow you and your allies to send and pick up mail. |
| 16 | 5458 | Infectious Poison | 5472 | 1 | 5 -> OnBuffEnded |  |  |
| 16 | 5459 | Infectious Poison | 5472 | 6 | 5 -> OnBuffEnded |  |  |
| 16 | 5459 | Infectious Poison | 5472 | 7 | 5 -> OnBuffEnded |  |  |
| 16 | 9268 | Shield of Saphery | 12857 | 2 | 3 -> OnPreviousComponentApplied | Damage | Surrounds an ally with a magical barrier for up to {ABIL_3919_COM_0_DURA_SECONDS} which will absorb up to {ABIL_3919_COM_0_VAL1_DAMAGE}. |
| 16 | 15146 | Exalted Glory of War | 18048 | 9 | 3 -> OnPreviousComponentApplied |  | For the next {COM_0_DURA_SECONDS}, everyone in the area will receive an extra {COM_3_VAL1}% bonus to experience, influence, and renown. |
| 4 | 1598 | Burn Through | 14750 | 4 | 3 -> OnPreviousComponentApplied | Heal | Resurrect target friendly dead player and restore 20% of their health. |
| 4 | 1608 | Heart Of Fire | 14750 | 7 | 3 -> OnPreviousComponentApplied | Heal | Increases targeted group member's Toughness by {COM_5_VAL0} for 1 hour. The rune holder will then be able to resurrect themselves with 20% health if killed within the next 10 minutes. Doing so will remove the Oath Rune.<BR>You may only give a player one Oath Rune, but players may bear multiple Oath Runes of different types at the same time. |
| 4 | 1828 | Crushing Blows | 720 | 0 | 1 -> OnApply | Damage | You cause your pet to explode, dealing {ABIL_15_COM_0_VAL0_DAMAGE} to enemies within {ABIL_15_COM_0_RADI_FEET}.  For the next {COM_2_DURA_SECONDS}, the next different squig other than the one that just exploded will be summoned instantly. |
| 4 | 1852 | Run Away! | 714 | 0 | 1 -> OnApply |  | You command your Squig to Taunt all enemies within {ABIL_3571_COM_0_RADI_FEET} and increases your movement speed by {COM_1_VAL0} % for {COM_1_DURA_SECONDS}. |
| 4 | 1908 | AE Knockdown - Explosion | 14750 | 4 | 3 -> OnPreviousComponentApplied | Heal | Resurrects your target with 20% health. |
| 4 | 3188 | Moraz Smash | 14750 | 1 | 10 -> OnBuffEndedRemoved |  |  |
| 1 | 3404 | Buff Font | 9852 | 1 | 6 -> OnEventTriggered |  |  |
| 1 | 3405 | Debuff Font | 9434 | 1 | 6 -> OnEventTriggered |  |  |
| 1 | 3488 | Centigor Frontal AOE | 9851 | 1 | 6 -> OnEventTriggered |  |  |
| 1 | 3489 | Fire Font | 9806 | 1 | 6 -> OnEventTriggered |  |  |
| 1 | 3597 | Hartsblood Draft | 1026 | 0 | 1 -> OnApply |  |  |
| 1 | 3784 | Blessed Bullets of Confession | 9897 | 0 | 1 -> OnApply |  |  |

## CC :: ExtData[0].Val7

- Operation: `CC` (`12`)
- Field: `ExtData[0].Val7`
- Ranks: global `22`, area `12`
- Priority: `Critical` `174`
- Example ability: `3`

Summary: CC ext-data payload-B field for the slot-local control payload.

Evidence: Confidence: Structural. Non-zero rows: 354. Distinct values: 11. Tags: Damage, Disarm, Heal, Immunity, Knockdown, Root, Silence, Snare, Stun. Samples: 1, 101, 14741, 15, 167, 2, 20, 201, 25, 26540, 50.

Recommended action: Turn this structural role into named per-value semantics using the value-evidence and companion-field clusters.

## Top Values

| RawValue | Obs | Components | Abilities | SampleAbilityIds | Triggers | Context | Companions |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | 333 | 333 | 24 | 122, 124, 1369, 1370, 1384, 144, 1443, 1494, 1519, 1525, 1536, 1607 | 1 -> OnApply x281, 3 -> OnPreviousComponentApplied x100, 5 -> OnBuffEnded x18, 6 -> OnEventTriggered x5, 2 -> OnPreviousComponentEndCast x2, 7 -> OnPreviousComponentTick x2 | Damage x63, Silence x25, Root x18, Disarm x11, Stun x5, Heal x3, Knockdown x2, Immunity x1 | ExtData[0].Val4=8 (333/333, 100%); ExtData[0].Val3=1 (332/333, 99%); ExtData[0].Val1=2 (328/333, 98%); ExtData[0].Val2=2 (304/333, 91%); Value15=4 (255/333, 76%) |
| 2 | 10 | 10 | 18 | 12007, 12008, 12017, 12024, 12028, 12037, 12066, 12075, 12077, 12086, 12096, 12097 | 3 -> OnPreviousComponentApplied x238, 1 -> OnApply x37 | No sampled context tags are available for this value yet. | ExtData[0].Val1=1 (10/10, 100%); ExtData[0].Val2=88 (10/10, 100%); ExtData[0].Val3=1 (10/10, 100%); ExtData[0].Val4=9 (10/10, 100%); ExtData[1].Val1=1 (10/10, 100%) |
| 20 | 2 | 2 | 20 | 14379, 14380, 14383, 14384, 14387, 14388, 14391, 14392, 14395, 14396, 14399, 14400 | 3 -> OnPreviousComponentApplied x20 | No sampled context tags are available for this value yet. | ActivationDelay=500 (2/2, 100%); FlagsRaw=2175 (2/2, 100%); Value15=4 (2/2, 100%); ExtData[0].Val1=1 (2/2, 100%); ExtData[0].Val2=9 (2/2, 100%) |
| 50 | 2 | 2 | 7 | 17004, 17012, 17020, 17028, 17036, 17044, 4091 | 3 -> OnPreviousComponentApplied x7 | No sampled context tags are available for this value yet. | ExtData[0].Val2=9 (2/2, 100%); ExtData[0].Val3=4 (2/2, 100%); ExtData[0].Val4=8 (2/2, 100%); ExtData[0].Val6=100 (2/2, 100%) |
| 14741 | 1 | 1 | 1 | 13641 | 1 -> OnApply x1 | No sampled context tags are available for this value yet. | Duration=10000 (1/1, 100%); FlagsRaw=34 (1/1, 100%); ExtData[0].Val1=2 (1/1, 100%); ExtData[0].Val2=64 (1/1, 100%); ExtData[0].Val3=1 (1/1, 100%) |

## Sample Abilities

| RawValue | AbilityId | AbilityName | ComponentId | Slot | Trigger | Context | TextExcerpt |
| --- | ---: | --- | ---: | --- | --- | --- | --- |
| 1 | 3 | Bounce | 3310 | 1 | 3 -> OnPreviousComponentApplied | Damage | You cause your Battle Squig to bounce, dealing {COM_0_VAL0_DAMAGE} and knocking your target down for {COM_1_DURA_SECONDS}. |
| 1 | 6 | ;/end | 3446 | 0 | 1 -> OnApply | Damage | Your Squig bounces on it's target dealing {COM_2_VAL0_DAMAGE}, and knocking them down for {COM_0_DURA_SECONDS}. |
| 1 | 122 | Claw Sweep | 1194 | 0 | 1 -> OnApply | Root | AE Root |
| 1 | 124 | Lion's Roar | 1196 | 0 | 1 -> OnApply | Silence, Disarm | Silence and Disarm |
| 1 | 144 | Poisoned Spine | 1348 | 0 | 1 -> OnApply |  | You've been knocked down |
| 1 | 379 | Warding - Empire | 1696 | 2 | 3 -> OnPreviousComponentApplied |  |  |
| 2 | 12007 | Gasp of the Unliving | 21008 | 2 | 3 -> OnPreviousComponentApplied |  |  |
| 2 | 12008 | Tormenting Wail | 21002 | 1 | 1 -> OnApply |  |  |
| 2 | 12017 | Bat Screech | 21008 | 4 | 3 -> OnPreviousComponentApplied |  |  |
| 2 | 12017 | Bat Screech | 21008 | 5 | 3 -> OnPreviousComponentApplied |  |  |
| 2 | 12024 | Sever Limb | 21010 | 1 | 1 -> OnApply |  |  |
| 2 | 12028 | Brutal Stomp | 21002 | 1 | 1 -> OnApply |  |  |
| 20 | 14379 | Organ Gun | 14124 | 2 | 3 -> OnPreviousComponentApplied |  |  |
| 20 | 14380 | Catapult | 14124 | 2 | 3 -> OnPreviousComponentApplied |  |  |
| 20 | 14383 | Cannon | 14124 | 2 | 3 -> OnPreviousComponentApplied |  |  |
| 20 | 14384 | Rock Lobba | 14124 | 2 | 3 -> OnPreviousComponentApplied |  |  |
| 20 | 14387 | Hellblaster | 14124 | 2 | 3 -> OnPreviousComponentApplied |  |  |
| 20 | 14388 | Catapult | 14124 | 2 | 3 -> OnPreviousComponentApplied |  |  |

