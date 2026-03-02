# ProjectWAR Gemini 3.0 Flash Change Log

This file tracks all modifications and additions made by the Gemini 3.0 Flash AI agent.

| Date | Description of Change | Files Modified | Rationale |
|------|-----------------------|----------------|-----------|
| 2026-03-01 | Initial Fixes for T1 RvR and Ability Trainer | `LowerTierCampaignManager.cs`, `Creature.cs` | Resolved T1 initialization and Trainer index mismatch. |
| 2026-03-01 | T1 Zone Flipping and VP Broadcast Fixes | `Campaign.cs`, `BattlefieldObjective.cs`, `Player.cs` | Enabled T1 domination flips, appended VP to broadcasts, and fixed position saving in `ForceSave`. |
| 2026-03-01 | T1 Pairing-wide Domination & Visibility | `Campaign.cs` | Bypassed `ZoneId` filtering for Tier 1 in domination, objective sync, and buffs. |
| 2026-03-01 | AI Reliability & Documentation Update | `CLAUDE.md`, `change_log.md`, `*All Modified Files*` | Implemented new AI rules, running change log, and inline documentation for high-accuracy standard. |
| 2026-03-01 | T1 Zone Locking & Map Icon Fixes | `Campaign.cs`, `WorldMgr.cs` | Fixed domination checks to include Secured state, expanded T1 rewards/notifications to pairings, and updated campaign status packet for map visibility. |
| 2026-03-01 | Lowered Domination Time | `WorldConfigs.cs`, `Campaign.cs` | Reduced global domination timer to 3 minutes and added logic to reset the counter when domination is broken. |
| 2026-03-01 | Timer & Map Visual Fixes | `WorldServer\bin\Release\Configs\World.xml`, `BattlefieldObjective.cs` | Fixed 20-minute timer override in XML and swapped StateFlags to correctly show lockout icons on the map. |
| 2026-03-02 | Database Recovery & Ability Mapping Fix | `Player.cs`, `war_world.abilities` | Recovered database bitmasks for `CareerLine`, fixed ability mismapping (Witch Hunter/Black Orc), and enabled level-up fanfare/sync during login states. |
| 2026-03-02 | Ability Trainer Restoration | `Database/updates/` | Verified restoration of `abilities` table from `war_world.sql` and re-applied patches. |
| 2026-03-02 | Character State Sync Fix | `Player.cs` | Modified `SetLevel` to allow `F_PLAYER_LEVEL_UP` and `F_PLAYER_XP` packets if client state is `WorldEnter` or `Playing`. |
| 2026-03-02 | GM Hierarchy Simplification | `EGmLevel.cs`, `AccountMgr.cs`, `Account.cs`, `Client.cs`, `Player.cs`, `GameObject.cs`, `BaseCommands.cs`, `CommandDeclarations.cs`, `CreateAccount.cs` | Simplified GM system from bitmask (0-63) to hierarchical (1-5), updated all access checks to `>=`, fixed level 0 login, and enforced range checks. |
| 2026-03-02 | GM Hierarchy Priority Correction | `EGmLevel.cs`, `CreateAccount.cs`, `Player.cs` | Reordered hierarchy such that Admin is Level 5 and Developer is Level 4. Updated chat tags and help text to reflect this change. |
| 2026-03-02 | Ability & Creature Integrity Fix | `Database/updates/update_150_fix_ability_and_creatures.sql`, `SYSTEMIC_DIVERGENCES.md` | Fixed Squig Herder "Git Em!" availability by moving it to Core. Corrected widespread creature corruption (mismatched models, wrong faction, half-scale). |
| 2026-03-02 | AI Rules: Planning Mode Enforcement | `.cursorrules`, `.ai-rules`, `CLAUDE.md`, `openai-instructions.md` | Mandated that all AI conversations start in PLANNING mode (read-only) and require an approved implementation plan before execution. |
