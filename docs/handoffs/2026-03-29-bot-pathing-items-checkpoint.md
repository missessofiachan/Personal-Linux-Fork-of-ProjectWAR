# Bot Pathing & Item Restrictions Checkpoint — 2026-03-29

## Scope
Second bot-system pass completed in the same session. Addresses cross-zone
movement snap-back, incorrect item equipping, and structured waypoint navigation.

## Fixed

### 1. Cross-zone movement snap-back (`MovementInterface.cs`)
`UpdateMove()` was computing zone-pin coordinates with `_unit.Zone.CalculPin()`.
When a bot's interpolated world position crossed into a new zone mid-movement,
`_unit.Zone` was still the source zone, so the pin values were relative to the
wrong origin. `SetPosition` then placed the bot at a nonsensical location, causing
it to snap back to spawn. Fixed by using `destZone.CalculPin()` — the zone that
owns the current interpolated position.

### 2. Wrong-faction and over-renown items (`BotLoadoutManager.cs`)
Three filters added to every item query (gear set armor, weapons, accessories):

| Filter | Problem it solves |
|--------|------------------|
| `x.Realm == 0 \|\| x.Realm == realmByte` | Order bots were equipping Destruction-only items and vice versa. Career 1–12 = Order (Realm 1), 13–24 = Destruction (Realm 2). |
| `maxRenown == 0 \|\| x.MinRenown <= maxRenown` | RR40 bots were equipping Sovereign/Warpforged gear. New `GetMaxRenownForTier()` caps at 40/70/80/90/100 per T4 sub-tier; T1–T3 uncapped (0). |
| Removed `ThenByDescending(x => x.MinRenown)` | Sorting was actively preferring the highest-renown item; replaced with `ThenByDescending(x => x.Rarity)`. |

### 3. Waypoint-guided navigation (`BotPathfinder.cs`, `BotBrain.cs`)
New `BotPathfinder` static class samples `WaypointService.TableWaypoints` (NPC
patrol routes already loaded in memory) to build an ordered list of intermediate
positions between any start and destination:
- Projects each loaded waypoint onto the direct start→dest vector.
- Keeps waypoints ≤ 2000 raw units lateral of the path and between the endpoints.
- Sorts by progress, thins to ≥ 200-unit spacing.
- Always appends the destination as the final point.

`BotBrain.MarchAlongPath()` replaces all direct `Move(objectivePosition)` calls:
- Rebuilds path when `CurrentTargetObjective` changes.
- Advances through waypoints when within 300 raw units (~25 ft) of each one.
- Formation offset applied only to the final step (the flag) so bots share the
  road but arrive spread around the capture point.
- Falls back to direct movement if the waypoint table yields no intermediates.

Both MA and non-MA bots use `MarchAlongPath`, so the whole group routes
through the same road waypoints.

### 4. `BotPathfinder.cs` registered in `WorldServer.csproj`
New file was compiled by MSBuild's glob but invisible to the Visual Studio
project file (legacy .NET Framework `.csproj` requires explicit `<Compile>`
entries). Added `<Compile Include="Managers\BotPathfinder.cs" />`.

## Files Changed
- `WorldServer/World/Interfaces/MovementInterface.cs`
- `WorldServer/Managers/BotLoadoutManager.cs`
- `WorldServer/Managers/BotPathfinder.cs` ← new
- `WorldServer/World/AI/BotBrain.cs`
- `WorldServer/WorldServer.csproj`

## Build Status
`WorldServer.csproj` builds cleanly to `bin/Release/WorldServer.exe` — zero
errors, zero warnings after fix.

## Remaining Follow-Up Areas
- Verify bots successfully traverse warcamp → RvR zone boundary under live conditions.
- Confirm waypoint coverage for active campaign zones (requires live DB inspection).
  If a zone's NPC patrol data is sparse, bots will fall back to direct movement
  (still correct, just less road-aware).
- Career-specific ability implementations in sub-brain classes are stubs; combat
  sophistication is still limited.
