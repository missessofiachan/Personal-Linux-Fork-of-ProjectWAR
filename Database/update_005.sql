-- ProjectWAR Update Script 005
-- Description: Fix Bilerot Burrow Sentinel loot generation.
--   - Adds Sentinel loot groups for Bilerot bosses:
--       48128 Bartholomeus the Sickly
--       52462 Ssrydian Morbidae
--       52594 The Bile Lord
--   - Adds a low-chance Sentinel trash loot group scoped to zone 196.
--   - Boss loot items are mapped from existing Darkpromise set rows by name transform:
--       Darkpromise ... -> Sentinel ...
-- Author: Codex (GPT-5) 2026-03-06
-- Apply to: war_world
-- Command: mysql -uroot -ppassword -hlocalhost war_world < Database/update_005.sql

USE `war_world`;

-- Boss + trash groups for Bilerot Burrow (zone 196).
INSERT INTO `loot_groups`
    (`Entry`, `Name`, `DropEvent`, `CreatureID`, `CreatureSubType`, `DropChance`, `DropCount`, `ReqGroupUsable`, `ReqActiveQuest`, `SpecificZone`)
VALUES
    (726, 'InstanceBoss: Bartholomeus the Sickly (Sentinel)', 4, 48128, 0, 1.00, 1, 1, 0, 0),
    (727, 'InstanceBoss: Ssrydian Morbidae (Sentinel)',       4, 52462, 0, 1.00, 1, 1, 0, 0),
    (728, 'InstanceBoss: The Bile Lord (Sentinel)',           4, 52594, 0, 1.00, 2, 1, 0, 0),
    (729, 'Bilerot Burrow: Sentinel Trash',                   4,     0, 0, 0.03, 1, 1, 0, 196)
ON DUPLICATE KEY UPDATE
    `Name` = VALUES(`Name`),
    `DropEvent` = VALUES(`DropEvent`),
    `CreatureID` = VALUES(`CreatureID`),
    `CreatureSubType` = VALUES(`CreatureSubType`),
    `DropChance` = VALUES(`DropChance`),
    `DropCount` = VALUES(`DropCount`),
    `ReqGroupUsable` = VALUES(`ReqGroupUsable`),
    `ReqActiveQuest` = VALUES(`ReqActiveQuest`),
    `SpecificZone` = VALUES(`SpecificZone`);

-- Boss item pools (726/727/728):
-- Source rows are Darkpromise groups 719/720, remapped to Sentinel item names.
INSERT INTO `loot_group_items`
    (`LootGroupID`, `ItemID`, `MinRank`, `MaxRank`, `MinRenown`, `MaxRenown`, `Loot_Group_Items_ID`)
SELECT
    726 AS `LootGroupID`,
    sent.`Entry` AS `ItemID`,
    src.`MinRank`,
    src.`MaxRank`,
    src.`MinRenown`,
    src.`MaxRenown`,
    CONCAT('726_', sent.`Entry`) AS `Loot_Group_Items_ID`
FROM `loot_group_items` src
JOIN `item_infos` darkp ON darkp.`Entry` = src.`ItemID`
JOIN `item_infos` sent ON sent.`Name` = REPLACE(darkp.`Name`, 'Darkpromise', 'Sentinel')
WHERE src.`LootGroupID` IN (719, 720)
  AND darkp.`Name` LIKE 'Darkpromise%'
GROUP BY sent.`Entry`, src.`MinRank`, src.`MaxRank`, src.`MinRenown`, src.`MaxRenown`
ON DUPLICATE KEY UPDATE
    `LootGroupID` = VALUES(`LootGroupID`),
    `ItemID` = VALUES(`ItemID`),
    `MinRank` = VALUES(`MinRank`),
    `MaxRank` = VALUES(`MaxRank`),
    `MinRenown` = VALUES(`MinRenown`),
    `MaxRenown` = VALUES(`MaxRenown`);

INSERT INTO `loot_group_items`
    (`LootGroupID`, `ItemID`, `MinRank`, `MaxRank`, `MinRenown`, `MaxRenown`, `Loot_Group_Items_ID`)
SELECT
    727 AS `LootGroupID`,
    sent.`Entry` AS `ItemID`,
    src.`MinRank`,
    src.`MaxRank`,
    src.`MinRenown`,
    src.`MaxRenown`,
    CONCAT('727_', sent.`Entry`) AS `Loot_Group_Items_ID`
FROM `loot_group_items` src
JOIN `item_infos` darkp ON darkp.`Entry` = src.`ItemID`
JOIN `item_infos` sent ON sent.`Name` = REPLACE(darkp.`Name`, 'Darkpromise', 'Sentinel')
WHERE src.`LootGroupID` IN (719, 720)
  AND darkp.`Name` LIKE 'Darkpromise%'
GROUP BY sent.`Entry`, src.`MinRank`, src.`MaxRank`, src.`MinRenown`, src.`MaxRenown`
ON DUPLICATE KEY UPDATE
    `LootGroupID` = VALUES(`LootGroupID`),
    `ItemID` = VALUES(`ItemID`),
    `MinRank` = VALUES(`MinRank`),
    `MaxRank` = VALUES(`MaxRank`),
    `MinRenown` = VALUES(`MinRenown`),
    `MaxRenown` = VALUES(`MaxRenown`);

INSERT INTO `loot_group_items`
    (`LootGroupID`, `ItemID`, `MinRank`, `MaxRank`, `MinRenown`, `MaxRenown`, `Loot_Group_Items_ID`)
SELECT
    728 AS `LootGroupID`,
    sent.`Entry` AS `ItemID`,
    src.`MinRank`,
    src.`MaxRank`,
    src.`MinRenown`,
    src.`MaxRenown`,
    CONCAT('728_', sent.`Entry`) AS `Loot_Group_Items_ID`
FROM `loot_group_items` src
JOIN `item_infos` darkp ON darkp.`Entry` = src.`ItemID`
JOIN `item_infos` sent ON sent.`Name` = REPLACE(darkp.`Name`, 'Darkpromise', 'Sentinel')
WHERE src.`LootGroupID` IN (719, 720)
  AND darkp.`Name` LIKE 'Darkpromise%'
GROUP BY sent.`Entry`, src.`MinRank`, src.`MaxRank`, src.`MinRenown`, src.`MaxRenown`
ON DUPLICATE KEY UPDATE
    `LootGroupID` = VALUES(`LootGroupID`),
    `ItemID` = VALUES(`ItemID`),
    `MinRank` = VALUES(`MinRank`),
    `MaxRank` = VALUES(`MaxRank`),
    `MinRenown` = VALUES(`MinRenown`),
    `MaxRenown` = VALUES(`MaxRenown`);

-- Trash pool (729):
-- Sentinel rare (rank 37) items so Bilerot trash can drop Sentinel gear at low chance.
INSERT INTO `loot_group_items`
    (`LootGroupID`, `ItemID`, `MinRank`, `MaxRank`, `MinRenown`, `MaxRenown`, `Loot_Group_Items_ID`)
SELECT
    729 AS `LootGroupID`,
    ii.`Entry` AS `ItemID`,
    1 AS `MinRank`,
    40 AS `MaxRank`,
    0 AS `MinRenown`,
    80 AS `MaxRenown`,
    CONCAT('729_', ii.`Entry`) AS `Loot_Group_Items_ID`
FROM `item_infos` ii
WHERE ii.`Name` LIKE 'Sentinel %'
  AND ii.`MinRank` = 37
  AND ii.`Rarity` = 3
GROUP BY ii.`Entry`
ON DUPLICATE KEY UPDATE
    `LootGroupID` = VALUES(`LootGroupID`),
    `ItemID` = VALUES(`ItemID`),
    `MinRank` = VALUES(`MinRank`),
    `MaxRank` = VALUES(`MaxRank`),
    `MinRenown` = VALUES(`MinRenown`),
    `MaxRenown` = VALUES(`MaxRenown`);
