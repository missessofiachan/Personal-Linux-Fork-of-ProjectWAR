# Implementation Plan - Fix Career Trainer Ability Purchase (Revised)

This plan focuses on resolving the indexing desync between the server's purchasable ability list and the client's trainer UI.

## Problem Description

The Career Trainer UI in the Warhammer Online client masks certain ability categories (Tactics and certain Morales). When the server sends a global list that includes these "hidden" categories, the client-side indices shift relative to the server's list. This causes the "random different ability" bug and failure to purchase high-level abilities as the drift accumulates.

## Proposed Changes

### [WorldServer]

#### [MODIFY] [AbilityConstants.cs](file:///c:/Users/Admin/source/repos/Shmerrick/ProjectWAR/WorldServer/World/Abilities/Components/AbilityConstants.cs)
- Add `public byte Category;` to store the category data from the `abilities` table.
- Update the constructor to populate this field.

#### [MODIFY] [AbilityInterface.cs](file:///c:/Users/Admin/source/repos/Shmerrick/ProjectWAR/WorldServer/World/Abilities/AbilityInterface.cs)
- Update `GetPurchasableCareerAbilities` to filter out categories that the client hides from the trainer UI (Keep: 0, 2; Filter: 1, 3, 4, 5).
- In `F_BUY_CAREER_PACKAGE`, add `player.SendClientMessage` debug output:
  - "INDEX BUY: Client sent PackgeID=[id], Resolve Entry=[entry] ([name])"
  - This allows the user to see exactly what the server thinks it's buying versus what the client asked for.

#### [MODIFY] [Creature.cs](file:///c:/Users/Admin/source/repos/Shmerrick/ProjectWAR/WorldServer/World/Objects/Creature.cs)
- In `SendCareerTrainerAbilityList`, update the `F_CAREER_PACKAGE_INFO` packet:
  - Write actual `MinimumRank` and `MinimumRenown`.
- Add `player.SendClientMessage` debug output for each ability sent:
  - "TRAINER LIST: Index=[idx] Entry=[entry] Name=[name] Cat=[cat]"
  - This helps identify if the list matches the client UI's visual order.

## Verification Plan

### Manual Verification
1.  **Trainer Purchase**:
    - Login with a character and visit a Career Trainer.
    - Verify that only core abilities and career morales are listed.
    - Purchase multiple abilities and verify that the correct ability is learned every time.
3. **Debug Log Verification**:
    - Observe the in-game chat messages when opening the trainer and purchasing.
    - Confirm that the `TRAINER LIST` indices match the visual order in the UI.
    - Confirm that `INDEX BUY` correctly resolves the intended ability.
