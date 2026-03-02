-- ProjectWAR Database Update: update_149_remove_experimental_mode.sql
-- Description: Removes the orphaned ExperimentalMode column and associated ability modifier checks.

-- 1. Remove ExperimentalMode column from characters_value in war_characters database
-- Note: Assuming the user is connected to the correct database or we specify it.
ALTER TABLE `war_characters`.`characters_value` DROP COLUMN `ExperimentalMode`;

-- 2. Remove any ability modifier checks that use ExperimentalMode in war_world database
DELETE FROM `war_world`.`ability_modifier_checks` WHERE `CommandName` = 'ExperimentalMode';
