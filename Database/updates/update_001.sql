-- Note: 'war_world' is the default database name for the World Server.
-- If your database is named differently, change the USE statement below.
USE war_world;

-- General database schema fixes and realm configuration

ALTER TABLE pet_mastery_modifiers MODIFY COLUMN masterymodifieraddition SMALLINT UNSIGNED NOT NULL DEFAULT 0;
ALTER TABLE creature_spawns MODIFY COLUMN Guid INT NOT NULL AUTO_INCREMENT;
ALTER TABLE creature_stats MODIFY COLUMN StatValue INT NOT NULL DEFAULT 0;
ALTER TABLE gameobject_spawns MODIFY COLUMN Guid INT NOT NULL AUTO_INCREMENT;
ALTER TABLE instance_infos MODIFY COLUMN OrderExitZoneJumpId INT UNSIGNED NOT NULL DEFAULT 0;
ALTER TABLE instance_infos MODIFY COLUMN DestrExitZoneJumpId INT UNSIGNED NOT NULL DEFAULT 0;
ALTER TABLE pquest_objectives MODIFY COLUMN Guid INT NOT NULL DEFAULT 0;
ALTER TABLE waypoints MODIFY COLUMN GUID INT NOT NULL DEFAULT 0;
-- Note: 'war_accounts' is the default database name for the Account Cache module.
-- It stores user accounts and the realm definitions for the launcher.
-- If your Account Cache database is named differently, change the schema below.
UPDATE war_accounts.realms SET Port=51933;
