-- update_010_column_type_fixes.sql
-- Aligns DB column types with C# entity class property types to eliminate
-- "Type mismatch" errors logged by the WorldServer ORM on startup.
--
-- Apply to: war_world database
-- Safe to run multiple times (MODIFY is idempotent for type changes).

USE war_world;

-- creature_stats.statvalue: C# uses int (signed) to support negative stat values
-- (e.g. debuffs). DB had INT UNSIGNED which the ORM reports as a mismatch.
ALTER TABLE creature_stats MODIFY COLUMN statvalue INT NOT NULL;

-- pet_mastery_modifiers.masterymodifieraddition: C# uses ushort (SMALLINT UNSIGNED).
-- DB had INT which is wider than necessary and causes a type mismatch warning.
ALTER TABLE pet_mastery_modifiers MODIFY COLUMN masterymodifieraddition SMALLINT UNSIGNED NOT NULL DEFAULT 0;

-- instance_infos: exit zone jump ID columns use uint (INT UNSIGNED) in C#
-- but DB had signed INT. Zone jump IDs are always positive so UNSIGNED is correct.
ALTER TABLE instance_infos MODIFY COLUMN orderexitzonejumpid INT UNSIGNED NOT NULL DEFAULT 0;
ALTER TABLE instance_infos MODIFY COLUMN destrexitzonejumpid INT UNSIGNED NOT NULL DEFAULT 0;
