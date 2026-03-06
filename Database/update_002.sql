-- ProjectWAR Update Script 002
-- Description: Index additions for high-traffic character tables (DB-03 from code review).
-- Author: Claude Sonnet 4.6 (AI Code Review 2026-03-06)
-- Apply to: war_characters
-- Command: mysql -uroot -ppassword -hlocalhost war_characters < Database/update_002.sql

USE `war_characters`;

-- Add index on characters_items.CharacterId.
-- CharMgr.cs:1409 queries: SELECT ... FROM characters_items WHERE CharacterId='X'
-- This fires on every character login and item refresh. Without this index the query
-- is a full table scan across all characters' items.
ALTER TABLE `characters_items`
    ADD INDEX `idx_characterid` (`CharacterId`);

-- Add index on guild_members.GuildId.
-- CharMgr.cs:1287 queries: SELECT CharacterId FROM guild_members WHERE GuildId=X
-- Used as a subquery when loading guild rosters. Without this it scans every member row.
ALTER TABLE `guild_members`
    ADD INDEX `idx_guildid` (`GuildId`);
