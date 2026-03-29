# Player-Like Bot System Documentation

## Overview
The bot system creates persisted `Player` characters that behave like server-controlled
realm players in RvR and scenarios. Bots live on the shared `BotAccount` (account id
`9999`), use real character records in the database, and run through the normal world,
range, combat, and group systems.

Bots do not need separate accounts to be visible. Their synthetic `BotClient` only
swallows packets sent to the bot itself; real players still receive the normal
create/state/inventory packets when the bot is properly activated in the world.

---

## Core Components

### 1. Networking (`BotClient.cs`)
- `BotClient` inherits from `GameClient` and starts in the `Playing` state.
- `SendPacket`, `SendPacketNoBlock`, and `SendPacketsNoBlock` are no-ops — no real TCP session cost.
- Account `9999` is exempt from the one-session-per-account guard so many bots can share the same account safely.

### 2. Management and Persistence (`BotManager.cs`, `BotLoadoutManager.cs`)
- Bots are created or loaded from the normal `characters` tables.
- Legacy bot records are repaired on startup by `RepairPersistedBotAppearances()`.
- Faction selection is resolved from the active zone pairing for lore-correct racial factions.
- Bot groups are assigned to persistent guilds and use normal group membership.
- Gear is discovered and equipped from the normal item data through `BotLoadoutManager`.

#### Item Restriction Filters (all three applied to every item query)
| Filter | Purpose |
|--------|---------|
| `Career == 0 \|\| (Career & careerMask) > 0` | Career bitmask — only career-appropriate gear |
| `Realm == 0 \|\| Realm == realmByte` | Faction — Order bots (careers 1–12) never get Destruction items |
| `MinRenown <= maxRenownForTier` | Renown cap — RR40 bots cannot equip Sovereign/Warpforged gear |
| `Rarity >= 2` | Quality floor — no grey/white junk or debug items |
| Name excludes `debug`, `test`, `_gm_` | Explicit debug/test item exclusion |

### 3. Intelligence and Combat (`BotBrain.cs`, `Player.cs`)
Bots form standard 6-man groups with role suffixes:

| Suffix | Role |
|--------|------|
| `_MT` | MainTank — shield, defensive gear |
| `_OT` | OffTank — 2H weapon, offensive gear |
| `_H`  | Healer — healing gear, resurrects dead group members |
| `_R`  | Ranged DPS — ranged weapon, defensive gear |
| `_M1` | Melee DPS |
| `_M2` | Melee DPS |

The Main Assist (`_MT` by default) selects combat targets; non-MA bots assist. PvP
is enabled through the normal player PvP path. On death bots use the short respawn
path.

### 4. Waypoint Navigation (`BotPathfinder.cs`, `BotBrain.cs`)
`BotPathfinder.BuildPath(start, destination)` builds an ordered route from the bot's
current position to its target battlefield objective:

1. Iterates `WaypointService.TableWaypoints` (NPC patrol data, already in memory).
2. Projects each waypoint onto the direct start→dest vector.
3. Keeps waypoints ≤ 2000 raw units off the direct line and between start and dest.
4. Sorts survivors by progress, thins to ≥ 200-unit spacing.
5. Appends the destination (with formation offset) as the final step.

`BotBrain.MarchAlongPath()` walks bots through this list, advancing when within
300 raw units (~25 ft) of each intermediate waypoint. Formation offset is applied
only to the final step so the whole group follows the same road but spreads around
the capture flag on arrival.

Fallback: if no waypoints are found for a path, bots move directly to the destination.

### 5. Dynamic Deployment (`DynamicBotManager.cs`)
- `Start()` performs the initial battlefield scan immediately during startup.
- Recurring timer: first run at `120` seconds, then every `60` seconds.
- Four color groups (Red, Green, Blue, Yellow) per realm per campaign tier, each
  offset by an angle-distributed point 600 units from the warcamp entrance.
- If a complete group is missing → `BotManager.SpawnBotGroup()`.
- If a group exists in the wrong zone or > 15 000 units from spawn → teleport.
- Scenario bots checked every `30` seconds and queued when a full initialized group
  is available.

---

## Movement Architecture

```
BotBrain.Think()
  └─ MarchAlongPath(player, objective)
       └─ BotPathfinder.BuildPath(start, dest)
            samples WaypointService.TableWaypoints
       Walk path[index] → advance when within 300 raw units
       └─ player.MvtInterface.Move(waypoint)
            └─ MovementInterface.UpdateMove(tick)
                 Lerp world position over time
                 destZone.CalculPin(x, y)   ← uses destination zone
                 unit.SetPosition(pin, zoneId)
```

Cross-zone movement is handled correctly: `UpdateMove()` uses `destZone.CalculPin()`
(not `_unit.Zone.CalculPin()`) so pin coordinates are always relative to the zone
the bot is entering, not the zone it is leaving. This was the root cause of the
5-second walk → snap-back-to-spawn bug.

---

## Visibility and Client Loading
Bot visibility uses the same post-load activation path as human players.

- `Player.EndInit()` calls `ActivateBotAfterInit()` for bots.
- `ActivateBotAfterInit()` sets `IsActive = true` and runs `OnClientLoaded()`.

Without this step bots exist server-side and interact with objectives but remain
invisible to human clients (inactive players are filtered from range visibility).

---

## Bot Name Convention
| Pattern | Context |
|---------|---------|
| `Bot_T{tier}_{O\|D}_{Color}_{suffix}` | Campaign RvR bots |
| `Bot_Scen_{O\|D}_{Color}_{suffix}` | Scenario bots |

Suffixes: `_H`, `_R`, `_MT`, `_OT`, `_M1`, `_M2`.

---

## GM Commands
- `.bot spawn <realm> <tier> <rr> <namePrefix>`
  Spawns a full 6-man group in the current zone.
  Realms: `1` = Order, `2` = Destruction.
