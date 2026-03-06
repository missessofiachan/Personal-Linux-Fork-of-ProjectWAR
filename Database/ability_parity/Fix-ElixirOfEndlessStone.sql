-- Targeted compatibility fix for item 204067 (Elixir of Endless Stone).
-- The item currently points to SpellId 24972, but this spell chain is missing in war_world_curated.
-- This script creates a self-buff ability/buff chain matching the item tooltip intent.

USE war_world_curated;

START TRANSACTION;

INSERT INTO `abilities` (`Entry`,`Name`,`EffectID`,`IconId`)
VALUES (24972,'Elixir of Endless Stone',24972,20338)
ON DUPLICATE KEY UPDATE
  `Name` = VALUES(`Name`),
  `EffectID` = VALUES(`EffectID`),
  `IconId` = VALUES(`IconId`);

DELETE FROM `ability_commands` WHERE `Entry` = 24972;
INSERT INTO `ability_commands`
  (`Entry`,`AbilityName`,`CommandID`,`CommandSequence`,`CommandName`,`PrimaryValue`,`SecondaryValue`,`Target`)
VALUES
  (24972,'Elixir of Endless Stone',0,0,'InvokeBuff',24972,NULL,'Caster');

INSERT INTO `buff_infos`
  (`Entry`,`Name`,`Group`,`MaxStack`,`Duration`,`PersistsOnDeath`,`CanRefresh`,`Silent`)
VALUES
  (24972,'Elixir of Endless Stone',22,1,300,5,1,0)
ON DUPLICATE KEY UPDATE
  `Name` = VALUES(`Name`),
  `Group` = VALUES(`Group`),
  `MaxStack` = VALUES(`MaxStack`),
  `Duration` = VALUES(`Duration`),
  `PersistsOnDeath` = VALUES(`PersistsOnDeath`),
  `CanRefresh` = VALUES(`CanRefresh`),
  `Silent` = VALUES(`Silent`);

DELETE FROM `buff_commands` WHERE `Entry` = 24972;
INSERT INTO `buff_commands`
  (`Entry`,`Name`,`CommandID`,`CommandSequence`,`CommandName`,`PrimaryValue`,`SecondaryValue`,`InvokeOn`,`Target`,`BuffLine`)
VALUES
  (24972,'Elixir of Endless Stone',0,0,'ModifyStat',14,75,5,'Host',1),
  (24972,'Elixir of Endless Stone',1,0,'ModifyStat',15,75,5,'Host',1),
  (24972,'Elixir of Endless Stone',2,0,'ModifyStat',16,75,5,'Host',1),
  (24972,'Elixir of Endless Stone',3,0,'ModifyPercentageStat',67,-100,5,'Host',1),
  (24972,'Elixir of Endless Stone',4,0,'ModifySpeed',-75,NULL,5,'Host',1);

COMMIT;
