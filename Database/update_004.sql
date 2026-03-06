-- ProjectWAR Update Script 004
-- Description: Two targeted fixes from runtime warning analysis.
--
--   FIX-1: Add index on mythic_src_item_infos.Name (war_world)
--     Query: SELECT ... FROM mythic_src_item_infos WHERE Name != ''
--     Scans 88,727 rows to return 4 results — observed at 2312ms on startup.
--     Adding a prefix index on Name reduces this to an index range scan (~0ms).
--
--   FIX-2: Set missing PinX/PinY on chapter_infos entries 104 and 405 (war_world)
--     Both are Warcamp chapters that fire "Chapter (N)[Name] Invalid" on every boot
--     because PinX=0 AND PinY=0, failing the guard in WorldMgr.LoadChapters():
--       if (Zone == null || (Info.PinX <= 0 && Info.PinY <= 0))
--     Coordinates derived from the zone_respawns table — the warcamp respawn point
--     is the authoritative positional anchor for each warcamp in its zone.
--       Chapter 104 "Warcamp: Splitsnout's Crag" (Zone 6/Ekrund, Destruction):
--         Matched to zone_respawns RespawnID=27, Realm=2, ZoneID=6 (57971, 38663)
--       Chapter 405 "Warcamp: Roarhammer's Stand" (Zone 11/Mt Bloodhorn, Order):
--         Matched to zone_respawns RespawnID=31, Realm=1, ZoneID=11 (43820, 17344)
--
-- Author: Claude Sonnet 4.6 (AI Code Review 2026-03-06)
-- Apply to: war_world
-- Command: mysql -uroot -ppassword -hlocalhost war_world < Database/update_004.sql

USE `war_world`;

-- FIX-1: Index on mythic_src_item_infos.Name
-- The startup query WHERE Name != '' does a full scan of 88,727 rows.
-- A 50-char prefix index is sufficient — Name values are short strings or empty.
ALTER TABLE `mythic_src_item_infos`
    ADD INDEX `idx_name` (`Name`(50));

-- FIX-2: Warcamp chapter coordinates
UPDATE `chapter_infos` SET `PinX` = 57971, `PinY` = 38663 WHERE `Entry` = 104;
UPDATE `chapter_infos` SET `PinX` = 43820, `PinY` = 17344 WHERE `Entry` = 405;
