-- Enforce Mythic-only item spell links.
-- Strategy:
-- 1) Remap known safe cases to Mythic ability IDs (manual overrides + unique name matches).
-- 2) Disable all remaining non-Mythic spell links by setting SpellId = 0.
--
-- This intentionally removes custom/non-Mythic item spell behavior.

USE war_world_curated;

START TRANSACTION;

CREATE TABLE IF NOT EXISTS `mythic_item_spell_sync_audit` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `RunStamp` datetime NOT NULL,
  `ItemEntry` int unsigned NOT NULL,
  `ItemName` varchar(255) NOT NULL,
  `OldSpellId` int unsigned NOT NULL,
  `NewSpellId` int unsigned NOT NULL,
  `Reason` varchar(64) NOT NULL,
  `ChangedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `idx_mythic_item_spell_sync_audit_run` (`RunStamp`),
  KEY `idx_mythic_item_spell_sync_audit_entry` (`ItemEntry`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SET @run_stamp := NOW();

DROP TEMPORARY TABLE IF EXISTS `tmp_mythic_name_unique`;
CREATE TEMPORARY TABLE `tmp_mythic_name_unique` (
  `Name` varchar(255) NOT NULL,
  `AbilityId` int unsigned NOT NULL,
  PRIMARY KEY (`Name`)
) ENGINE=Memory;

INSERT INTO `tmp_mythic_name_unique` (`Name`, `AbilityId`)
SELECT `Name`, MIN(`AbilityId`) AS `AbilityId`
FROM `mythic_csv_abilities`
WHERE `Name` IS NOT NULL AND `Name` <> ''
GROUP BY `Name`
HAVING COUNT(*) = 1;

DROP TEMPORARY TABLE IF EXISTS `tmp_mythic_overrides`;
CREATE TEMPORARY TABLE `tmp_mythic_overrides` (
  `ItemEntry` int unsigned NOT NULL,
  `AbilityId` int unsigned NOT NULL,
  `Reason` varchar(64) NOT NULL,
  PRIMARY KEY (`ItemEntry`)
) ENGINE=Memory;

-- Explicit deterministic overrides.
INSERT INTO `tmp_mythic_overrides` (`ItemEntry`, `AbilityId`, `Reason`) VALUES
  (204067, 4914, 'manual_override');

DROP TEMPORARY TABLE IF EXISTS `tmp_mythic_spell_remap`;
CREATE TEMPORARY TABLE `tmp_mythic_spell_remap` (
  `ItemEntry` int unsigned NOT NULL,
  `ItemName` varchar(255) NOT NULL,
  `OldSpellId` int unsigned NOT NULL,
  `NewSpellId` int unsigned NOT NULL,
  `Reason` varchar(64) NOT NULL,
  PRIMARY KEY (`ItemEntry`)
) ENGINE=Memory;

INSERT INTO `tmp_mythic_spell_remap` (`ItemEntry`, `ItemName`, `OldSpellId`, `NewSpellId`, `Reason`)
SELECT
  i.`Entry` AS `ItemEntry`,
  i.`Name` AS `ItemName`,
  i.`SpellId` AS `OldSpellId`,
  COALESCE(o.`AbilityId`, u.`AbilityId`) AS `NewSpellId`,
  CASE
    WHEN o.`AbilityId` IS NOT NULL THEN o.`Reason`
    ELSE 'unique_name_match'
  END AS `Reason`
FROM `item_infos` i
LEFT JOIN `tmp_mythic_overrides` o ON o.`ItemEntry` = i.`Entry`
LEFT JOIN `tmp_mythic_name_unique` u ON u.`Name` = i.`Name`
WHERE i.`SpellId` > 0
  AND NOT EXISTS (
    SELECT 1
    FROM `mythic_csv_abilities` m
    WHERE m.`AbilityId` = i.`SpellId`
  )
  AND COALESCE(o.`AbilityId`, u.`AbilityId`) IS NOT NULL
  AND COALESCE(o.`AbilityId`, u.`AbilityId`) <> i.`SpellId`;

INSERT INTO `mythic_item_spell_sync_audit`
  (`RunStamp`, `ItemEntry`, `ItemName`, `OldSpellId`, `NewSpellId`, `Reason`)
SELECT
  @run_stamp,
  r.`ItemEntry`,
  r.`ItemName`,
  r.`OldSpellId`,
  r.`NewSpellId`,
  r.`Reason`
FROM `tmp_mythic_spell_remap` r;

UPDATE `item_infos` i
JOIN `tmp_mythic_spell_remap` r ON r.`ItemEntry` = i.`Entry`
SET i.`SpellId` = r.`NewSpellId`;

DROP TEMPORARY TABLE IF EXISTS `tmp_mythic_spell_disable`;
CREATE TEMPORARY TABLE `tmp_mythic_spell_disable` (
  `ItemEntry` int unsigned NOT NULL,
  `ItemName` varchar(255) NOT NULL,
  `OldSpellId` int unsigned NOT NULL,
  `NewSpellId` int unsigned NOT NULL,
  `Reason` varchar(64) NOT NULL,
  PRIMARY KEY (`ItemEntry`)
) ENGINE=Memory;

INSERT INTO `tmp_mythic_spell_disable` (`ItemEntry`, `ItemName`, `OldSpellId`, `NewSpellId`, `Reason`)
SELECT
  i.`Entry`,
  i.`Name`,
  i.`SpellId`,
  0 AS `NewSpellId`,
  'disable_non_mythic_spell' AS `Reason`
FROM `item_infos` i
WHERE i.`SpellId` > 0
  AND NOT EXISTS (
    SELECT 1
    FROM `mythic_csv_abilities` m
    WHERE m.`AbilityId` = i.`SpellId`
  );

INSERT INTO `mythic_item_spell_sync_audit`
  (`RunStamp`, `ItemEntry`, `ItemName`, `OldSpellId`, `NewSpellId`, `Reason`)
SELECT
  @run_stamp,
  d.`ItemEntry`,
  d.`ItemName`,
  d.`OldSpellId`,
  d.`NewSpellId`,
  d.`Reason`
FROM `tmp_mythic_spell_disable` d;

UPDATE `item_infos` i
JOIN `tmp_mythic_spell_disable` d ON d.`ItemEntry` = i.`Entry`
SET i.`SpellId` = 0;

COMMIT;

-- Optional post-checks:
-- SELECT COUNT(*) FROM item_infos WHERE SpellId > 0 AND NOT EXISTS (SELECT 1 FROM mythic_csv_abilities m WHERE m.AbilityId = item_infos.SpellId);
-- SELECT Reason, COUNT(*) FROM mythic_item_spell_sync_audit WHERE RunStamp = @run_stamp GROUP BY Reason ORDER BY Reason;
