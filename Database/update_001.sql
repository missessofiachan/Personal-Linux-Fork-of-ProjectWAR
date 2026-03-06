-- ProjectWAR Update Script 001
-- Description: Performance index additions identified during 2026-03-06 code review.
-- Author: Claude Sonnet 4.6 (AI Code Review 2026-03-06)
-- Apply to: war_world
-- Command: mysql -uroot -ppassword -hlocalhost war_world < Database/update_001.sql

USE `war_world`;

-- Add index on creature_spawns.Enabled to fix 578ms full-table-scan at startup.
-- The server queries: SELECT ... FROM creature_spawns WHERE Enabled = 1
-- Without this index the query does a full table scan (confirmed: 578ms in WorldServer log).
ALTER TABLE `creature_spawns`
    ADD INDEX `idx_enabled` (`Enabled`);
