-- Apply this after importing the base dumps and prior incremental updates.
--
-- Root cause:
--   The four Greenskin starter templates in war_world.characterinfo are stored as local zone pins
--   for Mt Bloodhorn (zone 11):
--     WorldX = 44301
--     WorldY = 50742
--   while this table is otherwise world-space. The corresponding rally point already confirms the
--   correct world-space landing:
--     rallypoints.Id = 133
--     ZoneID = 11
--     WorldX = 847117
--     WorldY = 894518
--     WorldZ = 6909
--     WorldO = 792
--
-- User-visible symptom:
--   New Greenskin characters can be corrected into the wrong fallback respawn, landing near the
--   Chapter 3 / Kron-Komar Gap side of Mt Bloodhorn instead of Skarzag's Stompin Grounds.
--
-- This patch:
--   1. Normalizes the four Greenskin starter templates to the validated world-space rally point.
--   2. Repairs any already-created Greenskin characters in war_characters.characters_value that still
--      have the same local-pin coordinate format.

START TRANSACTION;

USE war_world;

UPDATE characterinfo AS ci
JOIN rallypoints AS rp
    ON rp.Id = 133
   AND rp.ZoneID = 11
SET
    ci.Region = 1,
    ci.ZoneId = rp.ZoneID,
    ci.WorldX = rp.WorldX,
    ci.WorldY = rp.WorldY,
    ci.WorldZ = rp.WorldZ,
    ci.WorldO = rp.WorldO,
    ci.RallyPt = rp.Id
WHERE ci.CareerLine IN (5, 6, 7, 8)
  AND ci.ZoneId = 11
  AND (
      ci.WorldX <> rp.WorldX
      OR ci.WorldY <> rp.WorldY
      OR ci.WorldZ <> rp.WorldZ
      OR ci.WorldO <> rp.WorldO
      OR ci.RallyPt <> rp.Id
      OR ci.Region <> 1
  );

USE war_characters;

UPDATE characters_value AS cv
SET
    cv.RegionId = 1,
    cv.ZoneId = 11,
    cv.WorldX = 847117,
    cv.WorldY = 894518,
    cv.WorldZ = 6909,
    cv.WorldO = 792
WHERE cv.ZoneId = 11
  AND cv.RegionId = 1
  AND cv.WorldX BETWEEN 0 AND 65535
  AND cv.WorldY BETWEEN 0 AND 65535;

COMMIT;

-- Verification:
USE war_world;
SELECT CareerLine, CareerName, ZoneId, WorldX, WorldY, WorldZ, WorldO, RallyPt
FROM characterinfo
WHERE CareerLine IN (5, 6, 7, 8)
ORDER BY CareerLine;

USE war_characters;
SELECT COUNT(*) AS RemainingLocalPinGreenskins
FROM characters_value
WHERE ZoneId = 11
  AND RegionId = 1
  AND WorldX BETWEEN 0 AND 65535
  AND WorldY BETWEEN 0 AND 65535;
