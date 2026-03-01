-- Fix guid column types to INT to match the C# emulator
ALTER TABLE `war_world`.`creature_spawns` MODIFY COLUMN `guid` int(11) NOT NULL;
ALTER TABLE `war_world`.`gameobject_spawns` MODIFY COLUMN `guid` int(11) NOT NULL;
ALTER TABLE `war_world`.`pquest_objectives` MODIFY COLUMN `guid` int(11) NOT NULL;
ALTER TABLE `war_world`.`waypoints` MODIFY COLUMN `guid` int(11) NOT NULL;
