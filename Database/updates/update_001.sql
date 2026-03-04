SET NAMES utf8mb4;

UPDATE war_accounts.realms SET Port=51933;
-- Note: 'war_world' is the default database name for the World Server.
-- If your database is named differently, change the USE statement below.

USE war_world;

-- DB Fixes for array parsing errors on WorldServer startup
-- Fix corrupted byte/ushort arrays in item_infos
UPDATE item_infos SET Unk27 = REPLACE(Unk27, ' 321 ', ' 255 ');
UPDATE item_infos SET Unk27 = REPLACE(Unk27, ' 10r', ' 10');
UPDATE item_infos SET Unk27 = REPLACE(Unk27, '  ', ' ');
UPDATE item_infos SET Unk27 = REPLACE(REPLACE(Unk27, '\r', ''), '\n', '');

-- Fix corrupted ushort arrays in gameobject_protos
UPDATE gameobject_protos SET Unks = REPLACE(REPLACE(REPLACE(REPLACE(Unks, '.teleport ', ''), 'map 162 ', ''), '111058 ', '0 '), '136620 ', '0 ') WHERE Entry = 200082;
UPDATE gameobject_protos SET Unks = REPLACE(REPLACE(Unks, '\r', ''), '\n', '');

-- Fix corrupted ushort arrays in gameobject_spawns
UPDATE gameobject_spawns SET Unks = REPLACE(Unks, '138647', '0');
UPDATE gameobject_spawns SET Unks = REPLACE(Unks, '27648889', '0');
UPDATE gameobject_spawns SET Unks = REPLACE(Unks, '27620037', '0');
UPDATE gameobject_spawns SET Unks = REPLACE(REPLACE(Unks, '\r', ''), '\n', '');

-- Fix corrupted byte arrays and carriage returns in creature_protos
UPDATE creature_protos SET FigLeafData = REPLACE(FigLeafData, '10r', '10');
UPDATE creature_protos SET FigLeafData = REPLACE(REPLACE(FigLeafData, '\r', ''), '\n', '');
UPDATE creature_protos SET States = REPLACE(REPLACE(States, '\r', ''), '\n', '');
