-- Ensure we are using the correct database
USE war_characters;

-- Add LeftSystemGuild column to characters_value table
ALTER TABLE characters_value ADD COLUMN LeftSystemGuild TINYINT(1) DEFAULT 0;
