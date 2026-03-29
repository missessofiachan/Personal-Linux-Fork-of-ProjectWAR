# Bot Checkpoint - 2026-03-29

## Scope
This checkpoint captures the bot-system stabilization work completed during the March 29, 2026 session.

## Fixed
- Dynamic bot startup race that could spawn duplicate characters with the same `CharacterId`.
- Bot campaign deployment now prefers direct battlefield-objective placement instead of only warcamp-based staging.
- Persisted bot appearance repair for invalid `ModelId`, `Sex`, and zeroed `Traits`.
- Zone-aware faction selection so active-pairing bots use the correct racial faction for the zone.
- Bot visibility bug caused by bots never completing the post-load activation path.
- Bot PvP enablement now uses the normal player PvP setup path.
- Bot death now uses the short respawn path so bots can recycle back into fights.
- Scenario respawn points now get a conservative terrain snap plus vertical lift to avoid floor clipping on entry/respawn.
- Character delete now forces a save immediately after purge.
- `FirstConnect` is persisted properly so system-guild autojoin survives restarts.

## Most Important Root Cause
Invisible bots were not primarily an account problem.

The decisive issue was that bots never executed the same "client finished loading" activation step used by human players after dump-statics. As a result:
- bots could exist server-side,
- objective interaction could still happen,
- teleport-to-bot could still work,
- but nearby human players would never receive normal range visibility for them.

That is now fixed by activating bots at the end of initialization.

## Current State
- T4 Praag bots are visible after restart on the fixed build.
- Bot visibility no longer requires packet-level guessing or separate bot accounts.
- The shared `BotAccount` design remains valid.
- Bot combat prerequisites are in place, but ability sophistication is still limited compared to a real player.

## Files With Material Changes
- `WorldServer/Managers/BotManager.cs`
- `WorldServer/Managers/DynamicBotManager.cs`
- `WorldServer/World/AI/BotBrain.cs`
- `WorldServer/World/Objects/Player.cs`
- `WorldServer/World/Objects/SpawnPoint.cs`
- `WorldServer/Services/World/ScenarioService.cs`
- `WorldServer/Managers/CharMgr.cs`
- `Common/Database/Character/Characters.cs`
- `WorldServer/Program.cs`
- `Database/update_010_repair_bot_appearance.sql`

## Verification
- `WorldServer.csproj` builds successfully to `bin/Release`.
- `WorldServer.csproj` also builds successfully to `out_verify/WorldServer`.

## Remaining Follow-Up Areas
- Verify live bot-vs-bot kill loops under sustained combat in campaign zones.
- Improve combat behavior beyond target acquisition and basic assist/follow logic.
- If any remaining visibility oddities appear, compare live create/state packets only after confirming the bot is active and in range.
