-- Curated + selective master refresh
-- Source schemas:
--   war_world_master_raw       : master import snapshot
--   war_world_sqlrepo_baseline : older stable baseline snapshot
-- Target schema:
--   war_world_curated

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS=0;

-- 1) Force core ability/AI behavior to master values
TRUNCATE TABLE war_world_curated.abilities;
INSERT INTO war_world_curated.abilities SELECT * FROM war_world_master_raw.abilities;

TRUNCATE TABLE war_world_curated.ability_commands;
INSERT INTO war_world_curated.ability_commands SELECT * FROM war_world_master_raw.ability_commands;

TRUNCATE TABLE war_world_curated.ability_damage_heals;
INSERT INTO war_world_curated.ability_damage_heals SELECT * FROM war_world_master_raw.ability_damage_heals;

TRUNCATE TABLE war_world_curated.creature_abilities;
INSERT INTO war_world_curated.creature_abilities SELECT * FROM war_world_master_raw.creature_abilities;

TRUNCATE TABLE war_world_curated.creature_smart_abilities;
INSERT INTO war_world_curated.creature_smart_abilities
(
  Guid,
  CreatureTypeId,
  CreatureSubTypeId,
  CreatureTypeDescription,
  SpellCastName,
  SpellCastSpeech,
  SpellCondition,
  SpellCastExecution,
  SpellExecuteChance,
  SpellCastCoolDown,
  SpellCastSound
)
SELECT
  Guid,
  CreatureTypeId,
  CreatureSubTypeId,
  CreatureTypeDescription,
  SpellCastName,
  SpellCastSpeech,
  SpellCondition,
  SpellCastExecution,
  SpellExecuteChance,
  SpellCastCoolDown,
  SpellCastSound
FROM war_world_master_raw.creature_smart_abilities;

TRUNCATE TABLE war_world_curated.waypoints;
INSERT INTO war_world_curated.waypoints SELECT * FROM war_world_master_raw.waypoints;

TRUNCATE TABLE war_world_curated.creature_spawns;
INSERT INTO war_world_curated.creature_spawns SELECT * FROM war_world_master_raw.creature_spawns;

TRUNCATE TABLE war_world_curated.creature_protos;
INSERT INTO war_world_curated.creature_protos SELECT * FROM war_world_master_raw.creature_protos;

TRUNCATE TABLE war_world_curated.creature_vendors;
INSERT INTO war_world_curated.creature_vendors SELECT * FROM war_world_master_raw.creature_vendors;

-- 2) Backfill known regressions from baseline
TRUNCATE TABLE war_world_curated.vendor_items;
INSERT INTO war_world_curated.vendor_items SELECT * FROM war_world_sqlrepo_baseline.vendor_items;

TRUNCATE TABLE war_world_curated.rallypoints;
INSERT INTO war_world_curated.rallypoints SELECT * FROM war_world_sqlrepo_baseline.rallypoints;

TRUNCATE TABLE war_world_curated.tok_bestary;
INSERT INTO war_world_curated.tok_bestary SELECT * FROM war_world_sqlrepo_baseline.tok_bestary;

-- 3) Preserve baseline-only tables that master no longer carries
CREATE TABLE IF NOT EXISTS war_world_curated.entries LIKE war_world_sqlrepo_baseline.entries;
TRUNCATE TABLE war_world_curated.entries;
INSERT INTO war_world_curated.entries SELECT * FROM war_world_sqlrepo_baseline.entries;

CREATE TABLE IF NOT EXISTS war_world_curated.event_infos LIKE war_world_sqlrepo_baseline.event_infos;
TRUNCATE TABLE war_world_curated.event_infos;
INSERT INTO war_world_curated.event_infos SELECT * FROM war_world_sqlrepo_baseline.event_infos;

CREATE TABLE IF NOT EXISTS war_world_curated.event_spawns LIKE war_world_sqlrepo_baseline.event_spawns;
TRUNCATE TABLE war_world_curated.event_spawns;
INSERT INTO war_world_curated.event_spawns SELECT * FROM war_world_sqlrepo_baseline.event_spawns;

CREATE TABLE IF NOT EXISTS war_world_curated.item_bonus LIKE war_world_sqlrepo_baseline.item_bonus;
TRUNCATE TABLE war_world_curated.item_bonus;
INSERT INTO war_world_curated.item_bonus SELECT * FROM war_world_sqlrepo_baseline.item_bonus;

CREATE TABLE IF NOT EXISTS war_world_curated.scenario_gauntlet_spawn LIKE war_world_sqlrepo_baseline.scenario_gauntlet_spawn;
TRUNCATE TABLE war_world_curated.scenario_gauntlet_spawn;
INSERT INTO war_world_curated.scenario_gauntlet_spawn SELECT * FROM war_world_sqlrepo_baseline.scenario_gauntlet_spawn;

SET FOREIGN_KEY_CHECKS=1;
