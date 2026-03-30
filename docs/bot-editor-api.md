# Bot Editor API

`WorldServer` now exposes a local HTTP JSON API for inspecting and editing bot gear loadouts from an external UI.

This API is intended to be the integration point for a future bot editor in `WAR-RE-Toolkit`.

## Purpose

The bot system now has three distinct layers:

- Base gear templates in `BotLoadoutManager`
- Per-bot persisted gear overrides in `bot_gear_overrides`
- Normal bot loadout application through `BotManager`

The HTTP API sits above those layers so an external tool can:

- list all bot characters
- inspect a bot's template gear, custom overrides, effective loadout, and currently equipped items
- search valid items for a specific slot
- replace or clear custom per-bot overrides
- reapply a changed loadout to an already loaded bot

## Server Configuration

`WorldServer` starts the bot editor API at process startup when enabled in `Configs/World.xml`.

Config fields:

- `EnableBotEditorAPI` default: `true`
- `BotEditorAPIAddress` default: `127.0.0.1`
- `BotEditorAPIPort` default: `51933`

Current defaults intentionally keep the service local-only.

## Persistence Model

Custom bot gear edits are stored in `war_characters.bot_gear_overrides`.

Schema:

- `CharacterId`
- `SlotId`
- `ItemEntry`

The table is created by `Database/update_011_bot_gear_overrides.sql`.

Resolution order for a bot sheet:

1. Determine the base template for `career + tier + role`
2. Load any per-bot overrides
3. Validate each override against career, race, skills, rank, renown, slot legality, and unique-equipped restrictions
4. Merge valid overrides into the effective loadout
5. If requested, reapply that effective loadout to a loaded bot

## Routes

Base URL:

```text
http://127.0.0.1:51933/api/bot-editor/
```

Routes:

- `GET health`
- `GET bots`
- `GET bots/{characterId}`
- `GET bots/{characterId}/items?q={query}&slotId={slotId}&limit={limit}`
- `PUT bots/{characterId}/gear`
- `DELETE bots/{characterId}/gear?reapply=true`

The API sends CORS headers for local UI development:

- `Access-Control-Allow-Origin: *`
- `Access-Control-Allow-Methods: GET,PUT,DELETE,OPTIONS`

## Response Shapes

`GET bots` returns bot summaries:

- `characterId`
- `name`
- `groupPrefix`
- `loaded`
- `role`
- `careerLine`
- `careerName`
- `realm`
- `realmName`
- `level`
- `renownRank`
- `zoneId`
- `hasGearOverrides`

`GET bots/{characterId}` returns a full bot sheet:

- `characterId`
- `name`
- `groupPrefix`
- `loaded`
- `role`
- `careerLine`
- `careerName`
- `realm`
- `realmName`
- `level`
- `renownRank`
- `zoneId`
- `templateGear`
- `customGearOverrides`
- `effectiveLoadout`
- `equippedGear`

Each gear list entry contains:

- `slotId`
- `slotName`
- `itemEntry`
- `itemName`
- `modelId`
- `objectLevel`
- `minRank`
- `minRenown`
- `rarity`
- `primaryDye`
- `secondaryDye`

`GET bots/{characterId}/items` returns valid candidate items for one slot only.

## Update Contract

`PUT bots/{characterId}/gear` accepts:

```json
{
  "replaceOverrides": true,
  "reapply": true,
  "slots": [
    { "slotId": 10, "itemEntry": 2000157 },
    { "slotId": 11, "itemEntry": 0 }
  ]
}
```

Rules:

- `replaceOverrides=true` starts from an empty override set
- `replaceOverrides=false` modifies the existing override set in place
- `itemEntry=0` or `null` removes the override for that slot
- `reapply=true` immediately reapplies the effective loadout if the bot is currently loaded

`DELETE bots/{characterId}/gear` clears all custom overrides for that bot.

## Validation Rules

Every override and item-search result is validated with the same bot loadout checks:

- slot must be a managed equipment slot
- item must be allowed in that slot
- item must be usable by the bot's career, race, skills, level, and renown
- unique-equipped items cannot duplicate another unique item already present in the effective loadout

Invalid overrides are rejected by the API or ignored during effective-loadout assembly.

## Toolkit Integration Status

This API is now consumed by `WAR-RE-Toolkit`.

Implemented toolkit pieces:

- `apps/warclient/Services/ServerRPC.cs` now supports `GET`, `PUT`, and `DELETE` with correct host, port, and query-string handling
- `apps/warclient/Services/BotEditorApiClient.cs` provides a typed client for all six bot editor routes
- `tools/ToolkitControlCenter/WarToolkitHub/Views/BotEditorView.xaml` hosts the embedded bot editor UI
- `tools/ToolkitControlCenter/program_ui_profiles.json` registers the tool in the hub navigation

Current toolkit capabilities:

- connect to the local bot editor API
- list bot characters
- inspect template gear, overrides, effective loadout, and equipped gear
- search valid items for a selected slot
- apply a single-slot override
- clear a single slot override
- clear all overrides for a bot
- request live reapply for already loaded bots

Current limitation:

- the client and server builds are verified, but end-to-end runtime interaction against a live `WorldServer` instance has not yet been validated in this repo

## Operational Notes

- This API does not create or delete bots; it edits the gear state of existing bots
- Existing bots can be edited even when unloaded because overrides persist in `war_characters`
- Loaded bots can be updated live through `reapply=true`
- The API is separate from the legacy debug-only binary API in `WorldServer/API/Server.cs`
- The first supported toolkit host is `WAR-RE-Toolkit/tools/ToolkitControlCenter/WarToolkitHub`

## Verification

The current implementation builds successfully with:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\amd64\MSBuild.exe' WorldServer\WorldServer.csproj /t:Build /p:Configuration=Release /p:Platform=x64
```
