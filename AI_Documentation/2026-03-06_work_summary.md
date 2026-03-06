# 2026-03-06 Work Summary

This commit packages two groups of fixes completed in the current workspace.

## Startup and Data Fixes

- Added `Database/update_001.sql` to index `creature_spawns.Enabled`.
- Added `Database/update_002.sql` to index `characters_items.CharacterId` and `guild_members.GuildId`.
- Added `Database/update_003.sql` to populate missing `gameobject_protos` rows from spawn data.
- Added `Database/update_004.sql` to index `mythic_src_item_infos.Name` and fix chapter pin coordinates for entries `104` and `405`.
- Updated [Directory.Build.targets](C:/Users/Admin/source/repos/Shmerrick/ProjectWAR/Directory.Build.targets) to tolerate the transient copy race for `System.Threading.Tasks.Extensions.dll`.
- Updated [PQuestService.cs](C:/Users/Admin/source/repos/Shmerrick/ProjectWAR/WorldServer/Services/World/PQuestService.cs) to stop double-loading PQ crafting loot and to log the correct loaded count.
- Updated [WorldMgr.cs](C:/Users/Admin/source/repos/Shmerrick/ProjectWAR/WorldServer/Managers/WorldMgr.cs) to preload campaign regions before campaign attachment.

## Instance and Lockout Fixes

- Updated [InstanceSpawn.cs](C:/Users/Admin/source/repos/Shmerrick/ProjectWAR/WorldServer/World/Objects/Instances/InstanceSpawn.cs) to preserve spawn group and boss linkage correctly and notify the instance when trash dies.
- Updated [Instance.cs](C:/Users/Admin/source/repos/Shmerrick/ProjectWAR/WorldServer/World/Objects/Instances/Instance.cs) to:
  - reset lockouts at the next server-local midnight instead of using a fixed duration
  - parse existing lockout boss data safely
  - open Bilerot Burrow mucus doors when dungeon mobs die
  - propagate spawn group ids for boss-linked trash
- Updated [InstanceBossSpawn.cs](C:/Users/Admin/source/repos/Shmerrick/ProjectWAR/WorldServer/World/Objects/Instances/InstanceBossSpawn.cs) so boss loot remains available to players who were eligible at kill time, even after the lockout is applied.
- Updated [InstanceMgr.cs](C:/Users/Admin/source/repos/Shmerrick/ProjectWAR/WorldServer/World/Objects/Instances/InstanceMgr.cs) to parse zone and local instance ids correctly when applying or checking lockouts.
- Updated [Player.cs](C:/Users/Admin/source/repos/Shmerrick/ProjectWAR/WorldServer/World/Objects/Player.cs) to check boss lockouts by exact boss id instead of substring matching.
- Updated [Characters_info.cs](C:/Users/Admin/source/repos/Shmerrick/ProjectWAR/Common/Database/Character/Characters_info.cs) to merge character lockouts by zone with deduplicated boss ids.
- Added `Database/update_005.sql` to create Sentinel loot groups for Bilerot Burrow bosses and trash.
- Added `Database/update_006.sql` to normalize Lost Vale portal routing, including portal `211812648`.

## Validation Notes

- `update_005.sql` and `update_006.sql` were applied locally and spot-checked against MySQL.
- A full `dotnet build` was not reliable in this shell because the solution mixes legacy project types and the local CLI environment is not a clean fit for the older build graph.
