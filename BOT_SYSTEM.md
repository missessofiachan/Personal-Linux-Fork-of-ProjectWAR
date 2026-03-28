# Player-Like Bot System Documentation

## Overview
The Bot System is designed to populate the world with autonomous, player-like entities that participate in the RvR campaign and Scenarios. These bots are real characters in the database but run with zero network overhead.

## Core Components

### 1. Networking (BotClient.cs)
- **Inheritance**: Inherits from GameClient.
- **Optimization**: Overrides SendPacket and buffer allocation to perform no-op operations. This allows the server to treat them as connected players without any TCP or serialization cost.

### 2. Management & Persistence (BotManager.cs & BotLoadoutManager.cs)
- **Account**: All bots are linked to Account ID 9999 ("BotAccount").
- **Persistence**: Bots are saved in the characters table. If a bot group is requested and characters don't exist, they are generated natively.
- **Faction Integration**: Bots are organized into exact race-matched sub-factions (e.g. Empire, Dwarfs, High Elves for Order; Chaos, Greenskins, Dark Elves for Destruction).
- **Permanent Guilds**: Groups are automatically assigned to permanent guilds bearing their faction's name, identical to player-created guilds in the database, complete with all ranks and full persistence.
- **Gear Discovery**: BotLoadoutManager automatically scans the ItemService on startup to find and equip sets like Annihilator, Warlord, and Sovereign based on the bot's tier and renown rank.

### 3. Intelligence (BotBrain.cs)
- **Assist Train**: Non-MA bots in a group automatically target and attack whatever the Main Assist (MA) is targeting.
- **Formation**: Bots use unique offsets to prevent "stacking" on a single coordinate, ensuring they occupy physical space at choke points.
- **RvR Logic**: Groups spawn at Warcamps and march to neutral/enemy Battlefield Objectives to capture them.
- **Scenario Logic**: Dedicated groups queue for Scenarios, auto-accept invites, and play objectives (carrying flags/balls).

### 4. Dynamic Deployment (DynamicBotManager.cs)
- **Automatic Monitoring**: A background service that runs every 60 seconds.
- **Reinforcement**: Monitors population (AAO) and zone push status to spawn or upgrade bot groups (RR40 -> RR100) as needed.

## Key Modifications to Core Engine
- **TCPServer.cs**: Exempted Account 9999 from the "one session per account" check to allow 50+ bots on one account.
- **Player.cs**: Added IsBot flag. Modified SendPacket to exit early if IsBot is true, saving massive CPU cycles.
- **Group.cs**: Exposed MainAssist for AI targeting.
- **Program.cs**: Added initialization hooks for the Bot services.

## Automatic Deployment

`DynamicBotManager` runs every 60 seconds (initial delay: 10 seconds after server start). It:
1. Queries the active campaign from `WorldMgr.UpperTierCampaignManager` and `WorldMgr.LowerTierCampaignManager`.
2. For each campaign zone, checks that warcamp entrance data exists in `BattleFrontObjects` for both realms.
3. If a complete 6-man bot group (suffix pattern `_H _R _MT _OT _M1 _M2`) is not already online, calls `BotManager.SpawnBotGroup`.
4. Scenario bots are checked every 30 seconds and queued into active scenarios.

Bot group names follow the pattern `Bot_T{tier}_{O|D}_{zoneId}` for RvR and `Bot_Scen_{O|D}` for scenarios.

## Startup Dependency

Bots require the WorldServer to complete its full startup (campaign, region, and zone loading) before the 10-second initial timer fires. If the server crashed at startup, no bots will have spawned. Verify the server started cleanly before investigating bot absence.

## Prerequisite: Warcamp Entrance Data

`DynamicBotManager` will skip a campaign zone and log an error if `BattleFrontObjects` has no warcamp entrance entry (`Type = WARCAMP_ENTRANCE`) for either realm in that zone. Bots for that zone are silently skipped until data is present.

## Usage (GM Commands)
- .bot spawn <realm> <tier> <rr> <namePrefix>
  - Spawns a full 6-man tactical group at the current zone's Warcamp.
  - Realms: 1 (Order), 2 (Destruction).
