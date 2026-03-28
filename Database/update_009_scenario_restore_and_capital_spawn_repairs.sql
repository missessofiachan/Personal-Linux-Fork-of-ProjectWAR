-- Apply this after importing the base dumps and prior incremental updates.
--
-- Root causes addressed here:
--   1. Most scenario_infos rows are present in the ProjectWAR world DB but remain disabled and
--      have RegionId = 0, so the scenario pool is largely absent at runtime.
--   2. Several capital / city scenario maps are missing zone_respawns entirely, which prevents
--      Scenario construction because WorldMgr.GetZoneRespawn(...) cannot produce stable starts.
--   3. zone_infos.ZoneId = 41 ("Altdorf War Quaters") has the wrong OffX/OffY origin in the DB.
--      The existing scenario_objects rows for ScenarioId 2012 only line up when zone 41 uses
--      OffX = 72 and OffY = 60.
--   4. College of Corruption (zone 411) shares the Bright Wizard College map origin (zone 197)
--      but has no scenario respawns in the live DB. The live zone 197 destruction respawn is the
--      strongest directly compatible anchor, and the order start is mirrored across the Aqshy Core.
--
-- Intentional scope limits for stability:
--   - ScenarioIds 2009 and 2010 remain disabled because MapID = 0.
--   - ScenarioIds 2107 and 2123 remain disabled because they currently have no scenario_objects.
--   - ScenarioId 2007 remains disabled pending better realm-specific start evidence for zone 37.

START TRANSACTION;

USE war_world;

-- Fix the bad Altdorf War Quarters origin so its existing scenario object coordinates resolve to
-- the same local pins the extracted map data uses.
UPDATE zone_infos
SET OffX = 72,
    OffY = 60
WHERE ZoneId = 41
  AND (OffX <> 72 OR OffY <> 60);

-- scenario_infos.RegionId should match zone_infos.Region for scenario-backed maps.
UPDATE scenario_infos AS si
JOIN zone_infos AS zi
  ON zi.ZoneId = si.MapID
SET si.RegionId = zi.Region
WHERE si.MapID <> 0
  AND si.RegionId <> zi.Region;

-- Zone 41: derived from the Warmachine (east) and Militia (west) objective bands in scenario 2012.
INSERT INTO zone_respawns (ZoneID, Realm, PinX, PinY, PinZ, WorldO, InZoneID)
SELECT 41, 1, 36864, 49480, 14239, 0, 0
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1
    FROM zone_respawns
    WHERE ZoneID = 41
      AND Realm = 1
);

UPDATE zone_respawns
SET PinX = 36864,
    PinY = 49480,
    PinZ = 14239,
    WorldO = 0,
    InZoneID = 0
WHERE ZoneID = 41
  AND Realm = 1
  AND (PinX <> 36864 OR PinY <> 49480 OR PinZ <> 14239 OR WorldO <> 0 OR InZoneID <> 0);

INSERT INTO zone_respawns (ZoneID, Realm, PinX, PinY, PinZ, WorldO, InZoneID)
SELECT 41, 2, 14336, 49480, 14239, 0, 0
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1
    FROM zone_respawns
    WHERE ZoneID = 41
      AND Realm = 2
);

UPDATE zone_respawns
SET PinX = 14336,
    PinY = 49480,
    PinZ = 14239,
    WorldO = 0,
    InZoneID = 0
WHERE ZoneID = 41
  AND Realm = 2
  AND (PinX <> 14336 OR PinY <> 49480 OR PinZ <> 14239 OR WorldO <> 0 OR InZoneID <> 0);

-- Zone 42 mirrors the validated local start geometry already present in zone 167.
INSERT INTO zone_respawns (ZoneID, Realm, PinX, PinY, PinZ, WorldO, InZoneID)
SELECT 42, 1, 28306, 32144, 17198, 3436, 0
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1
    FROM zone_respawns
    WHERE ZoneID = 42
      AND Realm = 1
);

UPDATE zone_respawns
SET PinX = 28306,
    PinY = 32144,
    PinZ = 17198,
    WorldO = 3436,
    InZoneID = 0
WHERE ZoneID = 42
  AND Realm = 1
  AND (PinX <> 28306 OR PinY <> 32144 OR PinZ <> 17198 OR WorldO <> 3436 OR InZoneID <> 0);

INSERT INTO zone_respawns (ZoneID, Realm, PinX, PinY, PinZ, WorldO, InZoneID)
SELECT 42, 2, 31174, 59428, 17483, 2060, 0
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1
    FROM zone_respawns
    WHERE ZoneID = 42
      AND Realm = 2
);

UPDATE zone_respawns
SET PinX = 31174,
    PinY = 59428,
    PinZ = 17483,
    WorldO = 2060,
    InZoneID = 0
WHERE ZoneID = 42
  AND Realm = 2
  AND (PinX <> 31174 OR PinY <> 59428 OR PinZ <> 17483 OR WorldO <> 2060 OR InZoneID <> 0);

-- Zone 167: promote the validated south / north portal landings to explicit realm respawns.
INSERT INTO zone_respawns (ZoneID, Realm, PinX, PinY, PinZ, WorldO, InZoneID)
SELECT 167, 1, 28306, 32144, 17198, 3436, 0
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1
    FROM zone_respawns
    WHERE ZoneID = 167
      AND Realm = 1
);

UPDATE zone_respawns
SET PinX = 28306,
    PinY = 32144,
    PinZ = 17198,
    WorldO = 3436,
    InZoneID = 0
WHERE ZoneID = 167
  AND Realm = 1
  AND (PinX <> 28306 OR PinY <> 32144 OR PinZ <> 17198 OR WorldO <> 3436 OR InZoneID <> 0);

INSERT INTO zone_respawns (ZoneID, Realm, PinX, PinY, PinZ, WorldO, InZoneID)
SELECT 167, 2, 31174, 59428, 17483, 2060, 0
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1
    FROM zone_respawns
    WHERE ZoneID = 167
      AND Realm = 2
);

UPDATE zone_respawns
SET PinX = 31174,
    PinY = 59428,
    PinZ = 17483,
    WorldO = 2060,
    InZoneID = 0
WHERE ZoneID = 167
  AND Realm = 2
  AND (PinX <> 31174 OR PinY <> 59428 OR PinZ <> 17483 OR WorldO <> 2060 OR InZoneID <> 0);

-- Zone 411 / College of Corruption:
--   - Realm 2 reuses the directly compatible Bright Wizard College respawn pin from zone 197.
--   - Realm 1 is mirrored across the Aqshy Core local X = 33280 on the shared zone 197/411 origin.
INSERT INTO zone_respawns (ZoneID, Realm, PinX, PinY, PinZ, WorldO, InZoneID)
SELECT 411, 1, 34641, 34570, 24162, 1930, 0
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1
    FROM zone_respawns
    WHERE ZoneID = 411
      AND Realm = 1
);

UPDATE zone_respawns
SET PinX = 34641,
    PinY = 34570,
    PinZ = 24162,
    WorldO = 1930,
    InZoneID = 0
WHERE ZoneID = 411
  AND Realm = 1
  AND (PinX <> 34641 OR PinY <> 34570 OR PinZ <> 24162 OR WorldO <> 1930 OR InZoneID <> 0);

INSERT INTO zone_respawns (ZoneID, Realm, PinX, PinY, PinZ, WorldO, InZoneID)
SELECT 411, 2, 31919, 34570, 24162, 3978, 0
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1
    FROM zone_respawns
    WHERE ZoneID = 411
      AND Realm = 2
);

UPDATE zone_respawns
SET PinX = 31919,
    PinY = 34570,
    PinZ = 24162,
    WorldO = 3978,
    InZoneID = 0
WHERE ZoneID = 411
  AND Realm = 2
  AND (PinX <> 31919 OR PinY <> 34570 OR PinZ <> 24162 OR WorldO <> 3978 OR InZoneID <> 0);

-- Re-enable the scenario set that now has valid map ids, region ids, objectives, and spawn coverage.
UPDATE scenario_infos
SET Enabled = CASE
    WHEN ScenarioID IN (2000,2001,2002,2003,2004,2005,2006,2008,2011,2012,2013,2015,
                        2100,2101,2102,2103,2104,2105,2106,2108,2109,2110,2111,2136,
                        2200,2201,2202,2203,2204,2205,2206,2207)
        THEN 1
    ELSE 0
END
WHERE ScenarioID IN (2000,2001,2002,2003,2004,2005,2006,2007,2008,2009,2010,2011,2012,2013,2015,
                     2100,2101,2102,2103,2104,2105,2106,2107,2108,2109,2110,2111,2123,2136,
                     2200,2201,2202,2203,2204,2205,2206,2207);

COMMIT;

-- Verification:
USE war_world;

SELECT ZoneId, OffX, OffY, Region
FROM zone_infos
WHERE ZoneId = 41;

SELECT ScenarioID, Name, Enabled, MapID, RegionId
FROM scenario_infos
WHERE ScenarioID IN (2000,2001,2002,2003,2004,2005,2006,2007,2008,2009,2010,2011,2012,2013,2015,
                     2100,2101,2102,2103,2104,2105,2106,2107,2108,2109,2110,2111,2123,2136,
                     2200,2201,2202,2203,2204,2205,2206,2207)
ORDER BY ScenarioID;

SELECT ZoneID, Realm, PinX, PinY, PinZ, WorldO, InZoneID
FROM zone_respawns
WHERE ZoneID IN (41,42,167,411)
ORDER BY ZoneID, Realm, RespawnID;
