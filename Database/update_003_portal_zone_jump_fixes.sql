-- Apply this after importing the base dumps and any prior incremental updates.
--
-- Portal zone-jump audit notes for this patch:
--   1. Verified against current ProjectWAR DB state:
--      - war_world.gameobject_spawns
--      - war_world.zone_jumps
--      - war_world.zone_infos
--   2. Cross-referenced against extracted client/system data:
--      - interface/interfacecore/maps/zoneXXX/mappoints.xml
--      - data/strings/english/maps/zoneXXX/map_point_names.txt
--      - data/gamedata/jumppoints.csv
--      - extracted portal fixture positions where available
--   3. Re-checked server behavior:
--      - direct portal objects in WorldServer/World/Objects/GameObject.cs use zone_jumps.Entry = gameobject_spawns.Guid
--      - client-driven F_ZONEJUMP portals use opaque portal keys that are not exposed in the text-side extracted files
--      - Lost Vale exit handling in WorldServer/NetWork/Handler/MovementHandlers.cs special-cases
--        destinationId = 272804328 and then redirects through instance_infos.OrderExitZoneJumpID /
--        instance_infos.DestrExitZoneJumpID when those rows exist
--
-- High-confidence findings:
--   - The classic realm-pair portals already have valid destination rows:
--       Butcher's Pass <-> The Maw <-> Fell Landing
--       Stonewatch <-> Reikwald <-> Shining Way
--   - Dragonwake <-> Isle of the Dead threshold portals already have valid destination rows.
--   - The Altdorf <-> Sigmar's Hammer capital portal already has both directions wired:
--       250646 (zone 162 -> 198) and 1258309 (zone 198 -> 162).
--   - The Inevitable City <-> The Viper Pit pair is missing exactly one direct portal row:
--       gameobject_spawns.Guid = 1258219, Entry = 100610, ZoneId = 178.
--   - Lost Vale currently has two distinct issues:
--       1. instance_infos routes both realms to 272804328, so Lost Vale exits collapse onto the same
--          Avelorn-side landing even though the client exposes both west and east Lost Vale portals.
--       2. zone_jumps contains exactly two zone 260 instance-entrance rows (211812584, 211812648),
--          both Type = 6 / InstanceID = 260, and they already land in the same small coastal staging cluster.
--
-- Why 1258219 is patched to the same city landing as 1258218:
--   - 1258218 (Entry 100611) is the sibling portal in the same Viper Pit portal cluster and already exits to zone 161.
--   - 1258218 lands near the extracted "The Viper Pit" landmark in zone 161.
--   - 1258219 is the only missing hardcoded direct-portal spawn in the current DB.
--   - zone_jumps.Entry = 65825 lands near 1258219 inside zone 178, showing this is a second arrival/exit point in the same dungeon portal staging area.
--   - The extracted capital map data exposes only one Viper Pit city landmark/landing area in zone 161.
--   - zone_jumps also enforces a unique (WorldX, WorldY, ZoneId) destination key, so 1258219 cannot
--     be a byte-for-byte clone of 1258218.
--   - Reusing the validated 1258218 landing cluster with a one-unit X offset is therefore the
--     best-supported non-ambiguous fix that still satisfies the DB constraint.
--
-- Why Lost Vale is patched here:
--   - Extracted Avelorn client map data exposes two Lost Vale landmarks:
--       west  marker local ( 2916, 45743) => world (1411940, 1454767)
--       east  marker local (40620, 50841) => world (1449644, 1459865)
--   - Extracted client map + tome text place Gaen Mere (High Elf / Order) on the east side and
--     Isha's Fall (Dark Elf / Destruction) on the west side. The realm-specific exit split below
--     follows that extracted client evidence.
--   - Existing zone_jumps.Entry = 1409298687 already lands almost exactly on the east Lost Vale marker,
--     while 272804328 is the hardcoded fallback trigger used by MovementHandlers.cs.
--   - No corresponding west-side Avelorn landing row exists in the current DB, so Destruction cannot be
--     routed to the west Lost Vale portal without adding one.
--   - Nearby west-side Avelorn creature spawns sit at Z ~= 3544-3551, so the new west row uses the
--     west marker's world X/Y with a terrain-aligned Z of 3544.
--   - The two zone 260 entrance rows already differ by only 21 X, 417 Y, 60 Z, share the same type and
--     instance id, and sit in the same entrance staging cluster. Per user guidance that west/east exterior
--     Lost Vale portals should share the same interior landing, this patch normalizes 211812648 onto the
--     same landing cluster as 211812584.
--   - zone_jumps also enforces a unique (WorldX, WorldY, ZoneId) destination key, so that normalization
--     uses the canonical landing with a one-unit X offset instead of an exact byte-for-byte duplicate.
--
-- Explicitly not patched here because the corresponding client portal Entry key was still not recoverable
-- with high confidence from the extracted text/system files on this machine:
--   - zone 201 "Serynal"
--   - zone 209 "Shrine of Lileath"
-- These still need additional binary-side client evidence before we should create or rewrite zone_jumps rows.

USE war_world;

START TRANSACTION;

-- Mirror the validated Viper Pit -> Inevitable City landing used by sibling portal 1258218.
-- zone_jumps has a unique (WorldX, WorldY, ZoneId) key, so the sibling row uses an adjacent
-- one-unit X offset while preserving the same landing cluster and heading.
INSERT INTO zone_jumps (Entry, ZoneId, WorldX, WorldY, WorldZ, WorldO, Enabled, Type, InstanceID)
SELECT
    1258219 AS Entry,
    source.ZoneId,
    source.WorldX + 1 AS WorldX,
    source.WorldY,
    source.WorldZ,
    source.WorldO,
    source.Enabled,
    source.Type,
    source.InstanceID
FROM zone_jumps AS source
WHERE source.Entry = 1258218
  AND NOT EXISTS (
      SELECT 1
      FROM zone_jumps AS existing
      WHERE existing.Entry = 1258219
  );

UPDATE zone_jumps AS target
JOIN zone_jumps AS source
    ON source.Entry = 1258218
SET
    target.ZoneId = source.ZoneId,
    target.WorldX = source.WorldX + 1,
    target.WorldY = source.WorldY,
    target.WorldZ = source.WorldZ,
    target.WorldO = source.WorldO,
    target.Enabled = source.Enabled,
    target.Type = source.Type,
    target.InstanceID = source.InstanceID
WHERE target.Entry = 1258219
  AND (
      target.ZoneId <> source.ZoneId
      OR target.WorldX <> source.WorldX + 1
      OR target.WorldY <> source.WorldY
      OR target.WorldZ <> source.WorldZ
      OR target.WorldO <> source.WorldO
      OR target.Enabled <> source.Enabled
      OR target.Type <> source.Type
      OR IFNULL(target.InstanceID, 0) <> IFNULL(source.InstanceID, 0)
  );

-- Lost Vale uses two near-duplicate instance-entrance rows for zone 260.
-- Normalize the sibling landing so both exterior Avelorn Lost Vale portals share the same
-- interior arrival point, matching user-confirmed retail behavior.
-- As with the Viper Pit fix, the sibling keeps a one-unit X offset to satisfy the unique
-- (WorldX, WorldY, ZoneId) key in zone_jumps.
UPDATE zone_jumps AS sibling
JOIN zone_jumps AS canonical
    ON canonical.Entry = 211812584
SET
    sibling.ZoneId = canonical.ZoneId,
    sibling.WorldX = canonical.WorldX + 1,
    sibling.WorldY = canonical.WorldY,
    sibling.WorldZ = canonical.WorldZ,
    sibling.WorldO = canonical.WorldO,
    sibling.Enabled = canonical.Enabled,
    sibling.Type = canonical.Type,
    sibling.InstanceID = canonical.InstanceID
WHERE sibling.Entry = 211812648
  AND (
      sibling.ZoneId <> canonical.ZoneId
      OR sibling.WorldX <> canonical.WorldX + 1
      OR sibling.WorldY <> canonical.WorldY
      OR sibling.WorldZ <> canonical.WorldZ
      OR sibling.WorldO <> canonical.WorldO
      OR sibling.Enabled <> canonical.Enabled
      OR sibling.Type <> canonical.Type
      OR IFNULL(sibling.InstanceID, 0) <> IFNULL(canonical.InstanceID, 0)
  );

-- Add the missing west-side Avelorn Lost Vale return landing.
-- 1409298687 is the existing east-side landing that aligns with the east map marker.
-- 2818588672 is a new server-side west landing id used only through instance_infos realm routing.
INSERT INTO zone_jumps (Entry, ZoneId, WorldX, WorldY, WorldZ, WorldO, Enabled, Type, InstanceID)
SELECT
    2818588672 AS Entry,
    source.ZoneId,
    1411940 AS WorldX,
    1454767 AS WorldY,
    3544 AS WorldZ,
    source.WorldO,
    source.Enabled,
    source.Type,
    source.InstanceID
FROM zone_jumps AS source
WHERE source.Entry = 1409298687
  AND NOT EXISTS (
      SELECT 1
      FROM zone_jumps AS existing
      WHERE existing.Entry = 2818588672
  );

UPDATE zone_jumps AS target
JOIN zone_jumps AS source
    ON source.Entry = 1409298687
SET
    target.ZoneId = source.ZoneId,
    target.WorldX = 1411940,
    target.WorldY = 1454767,
    target.WorldZ = 3544,
    target.WorldO = source.WorldO,
    target.Enabled = source.Enabled,
    target.Type = source.Type,
    target.InstanceID = source.InstanceID
WHERE target.Entry = 2818588672
  AND (
      target.ZoneId <> source.ZoneId
      OR target.WorldX <> 1411940
      OR target.WorldY <> 1454767
      OR target.WorldZ <> 3544
      OR target.WorldO <> source.WorldO
      OR target.Enabled <> source.Enabled
      OR target.Type <> source.Type
      OR IFNULL(target.InstanceID, 0) <> IFNULL(source.InstanceID, 0)
  );

-- Lost Vale exit handling special-cases destinationId 272804328 and redirects through these ids.
-- Order follows the extracted east-side High Elf portal near Gaen Mere.
-- Destruction follows the newly added west-side Dark Elf portal near Isha's Fall.
UPDATE instance_infos
SET
    OrderExitZoneJumpID = 1409298687,
    DestrExitZoneJumpID = 2818588672
WHERE Entry = 260
  AND ZoneID = 260
  AND (
      OrderExitZoneJumpID <> 1409298687
      OR DestrExitZoneJumpID <> 2818588672
  );

COMMIT;

-- Verification: direct hardcoded portal spawns present in the current DB.
SELECT
    go.Guid,
    go.Entry AS GameObjectEntry,
    gp.Name AS GameObjectName,
    go.ZoneId AS SourceZoneId,
    go.WorldX AS SourceWorldX,
    go.WorldY AS SourceWorldY,
    zj.ZoneId AS DestZoneId,
    zj.WorldX AS DestWorldX,
    zj.WorldY AS DestWorldY,
    zj.WorldZ AS DestWorldZ,
    zj.WorldO AS DestWorldO,
    zj.Enabled,
    zj.Type,
    zj.InstanceID
FROM gameobject_spawns AS go
LEFT JOIN zone_jumps AS zj
    ON zj.Entry = go.Guid
LEFT JOIN gameobject_protos AS gp
    ON gp.Entry = go.Entry
WHERE go.Entry IN (99644, 100610, 100611, 100612)
ORDER BY go.Guid;

-- Verification: Lost Vale rows touched by this patch.
SELECT Entry, ZoneId, WorldX, WorldY, WorldZ, WorldO, Enabled, Type, InstanceID
FROM zone_jumps
WHERE Entry IN (211812584, 211812648, 272804328, 1409298687, 2818588672)
ORDER BY ZoneId, Entry;

SELECT instance_infos_ID, Entry, ZoneID, Name, OrderExitZoneJumpID, DestrExitZoneJumpID
FROM instance_infos
WHERE Entry = 260
  AND ZoneID = 260;

-- Verification: unresolved client-driven portal families intentionally left for later review.
SELECT Entry, ZoneId, WorldX, WorldY, WorldZ, WorldO
FROM zone_jumps
WHERE ZoneId IN (201, 209)
ORDER BY ZoneId, Entry;
