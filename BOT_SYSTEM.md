# Player-Like Bot System Documentation

## Overview
The bot system creates persisted `Player` characters that behave like server-controlled realm players in RvR and scenarios. Bots live on the shared `BotAccount` (account id `9999`), use real character records in the database, and run through the normal world, range, combat, and group systems.

Bots do not need separate accounts to be visible. Their synthetic `BotClient` only swallows packets sent to the bot itself; real players still receive the normal create/state/inventory packets when the bot is properly activated in the world.

## Core Components

### 1. Networking (`BotClient.cs`)
- `BotClient` inherits from `GameClient` and starts in the `Playing` state.
- `SendPacket`, `SendPacketNoBlock`, and `SendPacketsNoBlock` are no-ops, so bots have no real TCP session cost.
- Account `9999` is exempt from the one-session-per-account guard so many bots can share the same account safely.

### 2. Management and Persistence (`BotManager.cs`, `BotLoadoutManager.cs`)
- Bots are created or loaded from the normal `characters` tables.
- Legacy bot records are repaired on startup by `RepairPersistedBotAppearances()`, which fixes invalid model ids, sex, and trait bytes.
- `Database/update_010_repair_bot_appearance.sql` exists for environments that still hold pre-fix bot rows.
- Faction selection is resolved from the active zone pairing, not random race choice, so zone-specific bot factions stay lore-correct.
- Bot groups are assigned to persistent guilds and use normal group membership.
- Gear is discovered and equipped from the normal item data through `BotLoadoutManager`.

### 3. Intelligence and Combat (`BotBrain.cs`, `Player.cs`)
- Bots form standard 6-man groups with `_H`, `_R`, `_MT`, `_OT`, `_M1`, and `_M2` suffix roles.
- Non-main-assist bots follow the main assist target.
- Bots now acquire nearby hostile targets before resuming battlefield-objective movement.
- PvP is enabled through the normal player PvP path via `EnsureBotPvpEnabled()`, not by only setting a raw flag.
- On death, bots use the short respawn path (`PreRespawnPlayer()`) instead of waiting on the generic long fallback timer.

### 4. Dynamic Deployment (`DynamicBotManager.cs`)
- `DynamicBotManager.Start()` performs the initial battlefield scan immediately during startup.
- The recurring bot timer starts `120` seconds later and then runs every `60` seconds. This prevents the old double-spawn race during server boot.
- Active campaign zones spawn one Order and one Destruction 6-man group when a complete group is missing.
- When battlefield objective data is available, campaign bots spawn directly at the primary BO through `ResolveBoSpawnPoint()`.
- If no BO anchor is available, spawn resolution falls back to warcamp entrance, then zone respawn, then zone center.
- Scenario bots are checked every `30` seconds and queued once a full initialized group is available.

## Visibility and Client Loading
Bot visibility depends on the same post-load activation path that human players use after the client finishes entering the world.

- `Player.EndInit()` now calls `ActivateBotAfterInit()` for bots.
- `ActivateBotAfterInit()` sets `IsActive = true` and runs `OnClientLoaded()`.
- Without that activation step, bots can exist server-side, move, and interact with objectives while remaining invisible to human clients because inactive players are filtered out of range visibility.

This was the root cause of the "teleport to bot works but the bot is invisible" bug.

## Automatic Deployment Summary
`DynamicBotManager` currently does the following:
1. Reads the active upper-tier and lower-tier campaigns.
2. Resolves the active zone for each campaign.
3. Checks whether the expected 6-man bot groups already exist.
4. Spawns missing campaign groups, preferring direct BO placement.
5. Checks scenario groups every `30` seconds and queues them when idle.

Campaign bot names use `Bot_T{tier}_{O|D}_{zoneId}`.
Scenario bot names use `Bot_Scen_{O|D}`.

## Operational Notes
- A full restart is recommended after appearance or activation changes so persisted bots respawn from corrected records.
- Shared-account architecture is intentional and is not a visibility blocker.
- If a bot exists in the database but is not visible in-world, check activation and range visibility before changing appearance data again.

## Usage
- `.bot spawn <realm> <tier> <rr> <namePrefix>`
  - Spawns a full 6-man group in the current zone using normal bot creation and fallback spawn resolution.
  - Realms: `1` = Order, `2` = Destruction.
