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
| 2026-03-01 | Database Restoration Plan | 1_4_8_RESTORATION_PLAN.md | Created comprehensive domain-based strategy to strictly parse reverse-engineered 1.4.8 DB files instead of blindly merging legacy master dumps. |
| 2026-03-01 | Domain 2: Creature Art Update | Database/updates/update_148_restore_creature_art.sql | Generated SQL to restore 1,464 authentic visual models by parsing MYP character art.csv. |
| 2026-03-01 | Domain 1: Item Restoration | Database/updates/update_148_restore_item_stats.sql | Re-mapped 2,617 existing item stats, vendor prices, and requirements by generating SQL from Londos 1.4.8 dump. |
| 2026-03-01 | Domain 3: Item Sets Restoration | Database/updates/update_148_restore_item_sets.sql | Re-mapped 294 Item Sets with authentic 1.4.8 Item requirements (ItemsString) and stat/ability bonuses (BonusString) using Londos DB mappings. |
| 2026-03-01 | Domain 4: Abilities Restoration | Database/updates/update_148_restore_abilities.sql | Re-mapped 2,629 Abilities with authentic 1.4.8 stats including Cast Time, Cooldowns, AP Cost, and Range using the Londos DB mappings. |
| 2026-03-01 | Domain 5: Advanced Ability Logic | Database/updates/update_148_restore_ability_logic.sql | Restored 2,629 Ability logic constraints including Min/Max Ranges and Critting limitations (CantCrit -> NoCrits) using Londos DB mappings. |
| 2026-03-01 | Domain 6: Ability Values Restoration | Database/updates/update_148_restore_ability_values.sql | Parsed authentic 1.4.8 MythicComponentData (JSON) to restore base damage and heal values for 593 ability components in the bility_damage_heals table. |
| 2026-03-01 | Domain 7: Ability Visuals & Name Cleaning | Database/updates/update_148_restore_ability_visuals.sql | Restored authentic 1.4.8 EffectID for 2,629 abilities and cleaned up emulator name strings (removing gender/metadata markers like ^M, ^f). |
| 2026-03-01 | Domain 8: Creature Protos Restoration | Database/updates/update_148_restore_creature_protos.sql | Restored authentic 1.4.8 Level, Scale, and Faction for 1,317 creature prototypes and cleaned up name strings. |
| 2026-03-01 | Domain 10: NPC Equipment Restoration | Database/updates/update_148_restore_creature_items.sql | Mapped 7,101 authentic 1.4.8 visual equipment slots (ModelID, Dyes) for NPCs by parsing the monsteritem table from the Londos RE dump. |
| 2026-03-01 | Domain 11: Creature Spawn Restoration | Database/updates/update_148_restore_creature_spawns.sql | Restored authentic 1.4.8 coordinates (X, Y, Z, Heading) for 1,312 static NPCs by mapping truth records to emulator spawns via proximity and name matching. |
| 2026-03-01 | Domain 14: Zone Respawn Points | Database/updates/update_148_restore_zone_respawns.sql | Restored 356 authentic 1.4.8 player respawn points by converting world coordinates to relative zone pins using retail offsets. |
| 2026-03-02 | Database Recovery & Ability Mapping Fix | `Player.cs`, `war_world.abilities` | Recovered database bitmasks for `CareerLine`, fixed ability mismapping (Witch Hunter/Black Orc), and enabled level-up fanfare/sync during login states. |
| 2026-03-02 | Ability Trainer Restoration | `Database/updates/` | Verified restoration of `abilities` table from `war_world.sql` and re-applied 1.4.8 patches (skipping corrupted `update_006`). |
| 2026-03-02 | Character State Sync Fix | `Player.cs` | Modified `SetLevel` to allow `F_PLAYER_LEVEL_UP` and `F_PLAYER_XP` packets if client state is `WorldEnter` or `Playing`. |
| 2026-03-02 | Database Integrity Maintenance | `N/A` | Identified and excluded `update_006_ability_stats_update.sql` from the automated restoration due to corrupted index-based mappings. |
| 2026-03-02 | GM Hierarchy Simplification | `EGmLevel.cs`, `AccountMgr.cs`, `Account.cs`, `Client.cs`, `Player.cs`, `GameObject.cs`, `BaseCommands.cs`, `CommandDeclarations.cs`, `CreateAccount.cs` | Simplified GM system from bitmask (0-63) to hierarchical (1-5), updated all access checks to `>=`, fixed level 0 login, and enforced range checks. |
| 2026-03-02 | GM Hierarchy Priority Correction | `EGmLevel.cs`, `CreateAccount.cs`, `Player.cs` | Reordered hierarchy such that Admin is Level 5 and Developer is Level 4. Updated chat tags and help text to reflect this change. |
| 2026-03-02 | Ability & Creature Integrity Fix | `Database/updates/update_150_fix_ability_and_creatures.sql`, `SYSTEMIC_DIVERGENCES.md` | Fixed Squig Herder "Git Em!" availability by moving it to Core. Corrected widespread creature corruption (mismatched models, wrong faction, half-scale) by restoring scale 100, re-mapping factions to emulator-compliant ranges, and repairing broken art mappings. |
