# Internal Bug Tracker

This document tracks internal bugs, regression, and known issues within the ProjectWAR emulator.

## Active Bugs

| ID | Summary | Description | Status | Priority | Reported |
|:---|:---|:---|:---|:---|:---|
| BUG-001 | Invader items not unlocking Superior Ward | Equipping the Invader items does not unlock the equip entries in the tome of knowledge for the section about wards, specifically the Superior Ward. There is a third tokunlock that is not being activated on equip. | Open | Medium | 2026-03-30 |
| BUG-002 | Floating chest objects | Chest objects are not always finding the nearest Z-height collision, resulting in the chest object floating above the ground. | Open | Low | 2026-03-30 |
| BUG-003 | RvR zone transition crash | Transitioning from Reikland to Reikwald crashes the server. | Open | High | 2026-03-30 |
| BUG-004 | LOS vertex position precision error | ~16-unit max Y error from NIF world-matrix accumulation during native LOS generation. | Open | Low | 2026-03-30 |
| BUG-005 | LOS missing water generation | Zone 280 (and others) missing `water.xml` in current extracted data, leading to missing water chunks in generated LOS. | Open | Low | 2026-03-30 |
| BUG-006 | LOS multi-zone coverage gap | Native LOS generation currently only fully supported for zone 280 due to missing source files for other zones in current extraction. | Open | Medium | 2026-03-30 |
| BUG-007 | Ability data gaps | 12,664 abilities identified with Partial or StringsOnly coverage in `ClientDataMatrix` analysis. | Open | Low | 2026-03-30 |
| BUG-008 | Unknown server-side operations | Component operations 29, 30, 32, 40, 41, 47, 51 remain unnamed and their semantics are opaque. | Open | Low | 2026-03-30 |

## Bug Reporting Guidelines

When adding a new bug, please include:
- **Summary**: A concise title.
- **Description**: Detailed steps to reproduce and expected vs. actual behavior.
- **Priority**: Low, Medium, High, or Critical.
- **Status**: Open, In Progress, Resolved, or Closed.
