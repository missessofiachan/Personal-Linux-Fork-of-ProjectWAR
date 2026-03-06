-- Build a full action-coverage snapshot for abilities + items using isolated mythic_* tables.
-- Run this in the target world schema (example: USE war_world_curated;).
--
-- Goal:
-- 1) Mirror the current legacy tables exactly so no behavior-bearing columns are lost.
-- 2) Expand string-packed item actions into normalized rows for deterministic validation.

SET SESSION sql_safe_updates = 0;

-- Mirror tables (exact copies of legacy structures + rows).
DROP TABLE IF EXISTS `mythic_src_abilities`;
CREATE TABLE `mythic_src_abilities` LIKE `abilities`;
INSERT INTO `mythic_src_abilities` SELECT * FROM `abilities`;

DROP TABLE IF EXISTS `mythic_src_ability_commands`;
CREATE TABLE `mythic_src_ability_commands` LIKE `ability_commands`;
INSERT INTO `mythic_src_ability_commands` SELECT * FROM `ability_commands`;

DROP TABLE IF EXISTS `mythic_src_buff_infos`;
CREATE TABLE `mythic_src_buff_infos` LIKE `buff_infos`;
INSERT INTO `mythic_src_buff_infos` SELECT * FROM `buff_infos`;

DROP TABLE IF EXISTS `mythic_src_buff_commands`;
CREATE TABLE `mythic_src_buff_commands` LIKE `buff_commands`;
INSERT INTO `mythic_src_buff_commands` SELECT * FROM `buff_commands`;

DROP TABLE IF EXISTS `mythic_src_ability_damage_heals`;
CREATE TABLE `mythic_src_ability_damage_heals` LIKE `ability_damage_heals`;
INSERT INTO `mythic_src_ability_damage_heals` SELECT * FROM `ability_damage_heals`;

DROP TABLE IF EXISTS `mythic_src_ability_knockback_info`;
CREATE TABLE `mythic_src_ability_knockback_info` LIKE `ability_knockback_info`;
INSERT INTO `mythic_src_ability_knockback_info` SELECT * FROM `ability_knockback_info`;

DROP TABLE IF EXISTS `mythic_src_ability_modifiers`;
CREATE TABLE `mythic_src_ability_modifiers` LIKE `ability_modifiers`;
INSERT INTO `mythic_src_ability_modifiers` SELECT * FROM `ability_modifiers`;

DROP TABLE IF EXISTS `mythic_src_ability_modifier_checks`;
CREATE TABLE `mythic_src_ability_modifier_checks` LIKE `ability_modifier_checks`;
INSERT INTO `mythic_src_ability_modifier_checks` SELECT * FROM `ability_modifier_checks`;

DROP TABLE IF EXISTS `mythic_src_item_infos`;
CREATE TABLE `mythic_src_item_infos` LIKE `item_infos`;
INSERT INTO `mythic_src_item_infos` SELECT * FROM `item_infos`;

DROP TABLE IF EXISTS `mythic_src_item_sets`;
CREATE TABLE `mythic_src_item_sets` LIKE `item_sets`;
INSERT INTO `mythic_src_item_sets` SELECT * FROM `item_sets`;

-- Metadata.
CREATE TABLE IF NOT EXISTS `mythic_action_snapshot_meta` (
  `SnapshotKey` varchar(64) NOT NULL,
  `LastBuiltAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Notes` varchar(255) NULL,
  PRIMARY KEY (`SnapshotKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `mythic_action_snapshot_meta` (`SnapshotKey`, `LastBuiltAt`, `Notes`)
VALUES (
  'abilities_items_actions_v1',
  CURRENT_TIMESTAMP,
  'Mirrors legacy ability/item tables and expands packed item action strings.'
)
ON DUPLICATE KEY UPDATE
  `LastBuiltAt` = VALUES(`LastBuiltAt`),
  `Notes` = VALUES(`Notes`);

-- Normalized item action tables.
CREATE TABLE IF NOT EXISTS `mythic_item_ability_actions` (
  `ItemEntry` int unsigned NOT NULL,
  `SpellId` int unsigned NOT NULL,
  `ExistsInLocalAbilities` tinyint(1) unsigned NOT NULL DEFAULT 0,
  `Source` varchar(48) NOT NULL DEFAULT 'item_infos.SpellId',
  PRIMARY KEY (`ItemEntry`, `SpellId`, `Source`),
  KEY `idx_mythic_item_ability_actions_spell` (`SpellId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_item_stat_actions` (
  `ItemEntry` int unsigned NOT NULL,
  `SourceOrder` smallint unsigned NOT NULL,
  `StatId` int unsigned NOT NULL,
  `StatValue` int NOT NULL,
  `SourceToken` varchar(96) NOT NULL,
  PRIMARY KEY (`ItemEntry`, `SourceOrder`),
  KEY `idx_mythic_item_stat_actions_stat` (`StatId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_item_effect_actions` (
  `ItemEntry` int unsigned NOT NULL,
  `SourceOrder` smallint unsigned NOT NULL,
  `EffectListId` int unsigned NOT NULL,
  `SourceToken` varchar(48) NOT NULL,
  PRIMARY KEY (`ItemEntry`, `SourceOrder`),
  KEY `idx_mythic_item_effect_actions_effect` (`EffectListId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_item_craft_actions` (
  `ItemEntry` int unsigned NOT NULL,
  `SourceOrder` smallint unsigned NOT NULL,
  `CraftType` int unsigned NOT NULL,
  `CraftValue` int unsigned NOT NULL,
  `SourceToken` varchar(96) NOT NULL,
  PRIMARY KEY (`ItemEntry`, `SourceOrder`),
  KEY `idx_mythic_item_craft_actions_type` (`CraftType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_item_sell_requirement_actions` (
  `ItemEntry` int unsigned NOT NULL,
  `SourceOrder` smallint unsigned NOT NULL,
  `RequiredItemEntry` int unsigned NOT NULL,
  `RequiredCount` int unsigned NOT NULL,
  `SourceToken` varchar(96) NOT NULL,
  PRIMARY KEY (`ItemEntry`, `SourceOrder`),
  KEY `idx_mythic_item_sell_req_required_item` (`RequiredItemEntry`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_itemset_item_actions` (
  `ItemSetEntry` int unsigned NOT NULL,
  `SourceOrder` smallint unsigned NOT NULL,
  `ItemKey` int unsigned NOT NULL,
  `ItemValue` varchar(255) NOT NULL,
  `SourceToken` varchar(512) NOT NULL,
  PRIMARY KEY (`ItemSetEntry`, `SourceOrder`),
  KEY `idx_mythic_itemset_item_actions_itemkey` (`ItemKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_itemset_bonus_actions` (
  `ItemSetEntry` int unsigned NOT NULL,
  `SourceOrder` smallint unsigned NOT NULL,
  `BonusKey` int unsigned NOT NULL,
  `BonusValue` varchar(255) NOT NULL,
  `SourceToken` varchar(512) NOT NULL,
  PRIMARY KEY (`ItemSetEntry`, `SourceOrder`),
  KEY `idx_mythic_itemset_bonus_actions_bonuskey` (`BonusKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Cross-source action index for quick coverage checks.
CREATE TABLE IF NOT EXISTS `mythic_action_index` (
  `SourceType` varchar(24) NOT NULL,
  `Entry` int unsigned NOT NULL,
  `ActionKind` varchar(48) NOT NULL,
  `ActionCount` int unsigned NOT NULL,
  PRIMARY KEY (`SourceType`, `Entry`, `ActionKind`),
  KEY `idx_mythic_action_index_kind` (`ActionKind`, `ActionCount`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

TRUNCATE TABLE `mythic_item_ability_actions`;
TRUNCATE TABLE `mythic_item_stat_actions`;
TRUNCATE TABLE `mythic_item_effect_actions`;
TRUNCATE TABLE `mythic_item_craft_actions`;
TRUNCATE TABLE `mythic_item_sell_requirement_actions`;
TRUNCATE TABLE `mythic_itemset_item_actions`;
TRUNCATE TABLE `mythic_itemset_bonus_actions`;
TRUNCATE TABLE `mythic_action_index`;

DROP TEMPORARY TABLE IF EXISTS `tmp_mythic_seq`;
CREATE TEMPORARY TABLE `tmp_mythic_seq` (
  `n` int unsigned NOT NULL,
  PRIMARY KEY (`n`)
);

INSERT INTO `tmp_mythic_seq` (`n`)
WITH RECURSIVE seq AS (
  SELECT 1 AS n
  UNION ALL
  SELECT n + 1 FROM seq WHERE n < 128
)
SELECT n FROM seq;

INSERT INTO `mythic_item_ability_actions` (`ItemEntry`, `SpellId`, `ExistsInLocalAbilities`, `Source`)
SELECT
  i.`Entry`,
  i.`SpellId`,
  CASE
    WHEN EXISTS (SELECT 1 FROM `mythic_src_abilities` a WHERE a.`Entry` = i.`SpellId`)
    THEN 1 ELSE 0
  END,
  'item_infos.SpellId'
FROM `mythic_src_item_infos` i
WHERE i.`SpellId` > 0;

INSERT INTO `mythic_item_stat_actions` (`ItemEntry`, `SourceOrder`, `StatId`, `StatValue`, `SourceToken`)
SELECT
  t.`ItemEntry`,
  t.`SourceOrder`,
  CAST(SUBSTRING_INDEX(t.`Token`, ':', 1) AS unsigned),
  CAST(SUBSTRING_INDEX(SUBSTRING_INDEX(t.`Token`, ':', 2), ':', -1) AS signed),
  t.`Token`
FROM (
  SELECT
    i.`Entry` AS `ItemEntry`,
    s.`n` AS `SourceOrder`,
    TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(COALESCE(i.`Stats`, ''), ';', s.`n`), ';', -1)) AS `Token`
  FROM `mythic_src_item_infos` i
  JOIN `tmp_mythic_seq` s
    ON s.`n` <= CASE
      WHEN COALESCE(i.`Stats`, '') = '' THEN 0
      ELSE 1 + LENGTH(i.`Stats`) - LENGTH(REPLACE(i.`Stats`, ';', ''))
    END
) t
WHERE t.`Token` REGEXP '^[0-9]+:-?[0-9]+(:.*)?$';

INSERT INTO `mythic_item_effect_actions` (`ItemEntry`, `SourceOrder`, `EffectListId`, `SourceToken`)
SELECT
  t.`ItemEntry`,
  t.`SourceOrder`,
  CAST(t.`Token` AS unsigned),
  t.`Token`
FROM (
  SELECT
    i.`Entry` AS `ItemEntry`,
    s.`n` AS `SourceOrder`,
    TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(COALESCE(i.`Effects`, ''), ';', s.`n`), ';', -1)) AS `Token`
  FROM `mythic_src_item_infos` i
  JOIN `tmp_mythic_seq` s
    ON s.`n` <= CASE
      WHEN COALESCE(i.`Effects`, '') = '' THEN 0
      ELSE 1 + LENGTH(i.`Effects`) - LENGTH(REPLACE(i.`Effects`, ';', ''))
    END
) t
WHERE t.`Token` REGEXP '^[0-9]+$';

INSERT INTO `mythic_item_craft_actions` (`ItemEntry`, `SourceOrder`, `CraftType`, `CraftValue`, `SourceToken`)
SELECT
  t.`ItemEntry`,
  t.`SourceOrder`,
  CAST(SUBSTRING_INDEX(t.`Token`, ':', 1) AS unsigned),
  CASE
    WHEN LOCATE(':', t.`Token`) > 0
    THEN CAST(SUBSTRING_INDEX(t.`Token`, ':', -1) AS unsigned)
    ELSE 0
  END,
  t.`Token`
FROM (
  SELECT
    i.`Entry` AS `ItemEntry`,
    s.`n` AS `SourceOrder`,
    TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(COALESCE(i.`Crafts`, ''), ';', s.`n`), ';', -1)) AS `Token`
  FROM `mythic_src_item_infos` i
  JOIN `tmp_mythic_seq` s
    ON s.`n` <= CASE
      WHEN COALESCE(i.`Crafts`, '') = '' THEN 0
      ELSE 1 + LENGTH(i.`Crafts`) - LENGTH(REPLACE(i.`Crafts`, ';', ''))
    END
) t
WHERE t.`Token` REGEXP '^[0-9]+(:[0-9]+)?$';

INSERT INTO `mythic_item_sell_requirement_actions`
(`ItemEntry`, `SourceOrder`, `RequiredItemEntry`, `RequiredCount`, `SourceToken`)
SELECT
  t.`ItemEntry`,
  t.`SourceOrder`,
  CAST(SUBSTRING_INDEX(t.`Token`, ':', 1) AS unsigned),
  CAST(SUBSTRING_INDEX(t.`Token`, ':', -1) AS unsigned),
  t.`Token`
FROM (
  SELECT
    i.`Entry` AS `ItemEntry`,
    s.`n` AS `SourceOrder`,
    TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(COALESCE(i.`SellRequiredItems`, ''), ';', s.`n`), ';', -1)) AS `Token`
  FROM `mythic_src_item_infos` i
  JOIN `tmp_mythic_seq` s
    ON s.`n` <= CASE
      WHEN COALESCE(i.`SellRequiredItems`, '') = '' THEN 0
      ELSE 1 + LENGTH(i.`SellRequiredItems`) - LENGTH(REPLACE(i.`SellRequiredItems`, ';', ''))
    END
) t
WHERE t.`Token` REGEXP '^[0-9]+:[0-9]+$';

INSERT INTO `mythic_itemset_item_actions` (`ItemSetEntry`, `SourceOrder`, `ItemKey`, `ItemValue`, `SourceToken`)
SELECT
  t.`ItemSetEntry`,
  t.`SourceOrder`,
  CAST(SUBSTRING_INDEX(t.`Token`, ':', 1) AS unsigned),
  TRIM(SUBSTRING_INDEX(t.`Token`, ':', -1)),
  t.`Token`
FROM (
  SELECT
    i.`Entry` AS `ItemSetEntry`,
    s.`n` AS `SourceOrder`,
    TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(COALESCE(i.`ItemsString`, ''), '|', s.`n`), '|', -1)) AS `Token`
  FROM `mythic_src_item_sets` i
  JOIN `tmp_mythic_seq` s
    ON s.`n` <= CASE
      WHEN COALESCE(i.`ItemsString`, '') = '' THEN 0
      ELSE 1 + LENGTH(i.`ItemsString`) - LENGTH(REPLACE(i.`ItemsString`, '|', ''))
    END
) t
WHERE t.`Token` REGEXP '^[0-9]+:.+$';

INSERT INTO `mythic_itemset_bonus_actions` (`ItemSetEntry`, `SourceOrder`, `BonusKey`, `BonusValue`, `SourceToken`)
SELECT
  t.`ItemSetEntry`,
  t.`SourceOrder`,
  CAST(SUBSTRING_INDEX(t.`Token`, ':', 1) AS unsigned),
  TRIM(SUBSTRING_INDEX(t.`Token`, ':', -1)),
  t.`Token`
FROM (
  SELECT
    i.`Entry` AS `ItemSetEntry`,
    s.`n` AS `SourceOrder`,
    TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(COALESCE(i.`BonusString`, ''), '|', s.`n`), '|', -1)) AS `Token`
  FROM `mythic_src_item_sets` i
  JOIN `tmp_mythic_seq` s
    ON s.`n` <= CASE
      WHEN COALESCE(i.`BonusString`, '') = '' THEN 0
      ELSE 1 + LENGTH(i.`BonusString`) - LENGTH(REPLACE(i.`BonusString`, '|', ''))
    END
) t
WHERE t.`Token` REGEXP '^[0-9]+:.+$';

INSERT INTO `mythic_action_index` (`SourceType`, `Entry`, `ActionKind`, `ActionCount`)
SELECT 'ability', CAST(a.`Entry` AS unsigned), 'ability_core', 1
FROM `mythic_src_abilities` a
UNION ALL
SELECT 'ability', CAST(ac.`Entry` AS unsigned), 'ability_command', COUNT(*)
FROM `mythic_src_ability_commands` ac
GROUP BY ac.`Entry`
UNION ALL
SELECT 'ability', CAST(ah.`Entry` AS unsigned), 'ability_damage_heal', COUNT(*)
FROM `mythic_src_ability_damage_heals` ah
GROUP BY ah.`Entry`
UNION ALL
SELECT 'ability', CAST(k.`Entry` AS unsigned), 'ability_knockback', COUNT(*)
FROM `mythic_src_ability_knockback_info` k
GROUP BY k.`Entry`
UNION ALL
SELECT 'ability', CAST(m.`Entry` AS unsigned), 'ability_modifier', COUNT(*)
FROM `mythic_src_ability_modifiers` m
GROUP BY m.`Entry`
UNION ALL
SELECT 'ability', CAST(mc.`Entry` AS unsigned), 'ability_modifier_check', COUNT(*)
FROM `mythic_src_ability_modifier_checks` mc
GROUP BY mc.`Entry`
UNION ALL
SELECT 'buff', CAST(b.`Entry` AS unsigned), 'buff_core', 1
FROM `mythic_src_buff_infos` b
UNION ALL
SELECT 'buff', CAST(bc.`Entry` AS unsigned), 'buff_command', COUNT(*)
FROM `mythic_src_buff_commands` bc
GROUP BY bc.`Entry`
UNION ALL
SELECT 'item', CAST(i.`Entry` AS unsigned), 'item_core', 1
FROM `mythic_src_item_infos` i
UNION ALL
SELECT 'item', ia.`ItemEntry`, 'item_spell', COUNT(*)
FROM `mythic_item_ability_actions` ia
GROUP BY ia.`ItemEntry`
UNION ALL
SELECT 'item', ist.`ItemEntry`, 'item_stat', COUNT(*)
FROM `mythic_item_stat_actions` ist
GROUP BY ist.`ItemEntry`
UNION ALL
SELECT 'item', ie.`ItemEntry`, 'item_effect', COUNT(*)
FROM `mythic_item_effect_actions` ie
GROUP BY ie.`ItemEntry`
UNION ALL
SELECT 'item', ic.`ItemEntry`, 'item_craft', COUNT(*)
FROM `mythic_item_craft_actions` ic
GROUP BY ic.`ItemEntry`
UNION ALL
SELECT 'item', isr.`ItemEntry`, 'item_sell_requirement', COUNT(*)
FROM `mythic_item_sell_requirement_actions` isr
GROUP BY isr.`ItemEntry`
UNION ALL
SELECT 'item_set', CAST(s.`Entry` AS unsigned), 'item_set_core', 1
FROM `mythic_src_item_sets` s
UNION ALL
SELECT 'item_set', isa.`ItemSetEntry`, 'item_set_item', COUNT(*)
FROM `mythic_itemset_item_actions` isa
GROUP BY isa.`ItemSetEntry`
UNION ALL
SELECT 'item_set', iba.`ItemSetEntry`, 'item_set_bonus', COUNT(*)
FROM `mythic_itemset_bonus_actions` iba
GROUP BY iba.`ItemSetEntry`;
