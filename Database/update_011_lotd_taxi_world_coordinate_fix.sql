-- Apply this after importing the base dumps and prior incremental updates.
--
-- The shipped Land of the Dead taxi rows store local zone pins in zone_taxis.WorldX / WorldY
-- instead of final world coordinates. WorldServer now normalizes such rows at runtime, but the
-- database should persist the corrected world positions so imports and direct SQL inspection match
-- live server behavior.

START TRANSACTION;

USE war_world;

UPDATE zone_taxis
SET WorldX = 254486,
    WorldY = 1498271,
    WorldZ = 10328,
    WorldO = 728,
    Tier = 4,
    Enable = 1
WHERE ZoneID = 191
  AND RealmID = 1
  AND (WorldX <> 254486 OR WorldY <> 1498271 OR WorldZ <> 10328 OR WorldO <> 728 OR Tier <> 4 OR Enable <> 1);

UPDATE zone_taxis
SET WorldX = 257648,
    WorldY = 1536559,
    WorldZ = 10248,
    WorldO = 835,
    Tier = 4,
    Enable = 1
WHERE ZoneID = 191
  AND RealmID = 2
  AND (WorldX <> 257648 OR WorldY <> 1536559 OR WorldZ <> 10248 OR WorldO <> 835 OR Tier <> 4 OR Enable <> 1);

COMMIT;

SELECT ZoneID, RealmID, WorldX, WorldY, WorldZ, WorldO, Tier, Enable
FROM zone_taxis
WHERE ZoneID = 191
ORDER BY RealmID;
