CREATE TABLE IF NOT EXISTS `mythic_csv_dataset_meta` (
  `DatasetName` varchar(64) NOT NULL,
  `SourceRelativePath` varchar(255) NOT NULL,
  `Sha256` char(64) NOT NULL,
  `HeaderRowText` text NULL,
  `ColumnRowText` text NULL,
  `RowCount` int unsigned NOT NULL DEFAULT 0,
  `ImportedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Notes` varchar(255) NULL,
  PRIMARY KEY (`DatasetName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_csv_raw_rows` (
  `DatasetName` varchar(64) NOT NULL,
  `RowIndex` int unsigned NOT NULL,
  `IdText` varchar(64) NULL,
  `IdInt` int NULL,
  `PayloadJson` mediumtext NOT NULL,
  PRIMARY KEY (`DatasetName`, `RowIndex`),
  KEY `idx_mythic_csv_raw_rows_idint` (`DatasetName`, `IdInt`),
  CONSTRAINT `fk_mythic_csv_raw_rows_dataset`
    FOREIGN KEY (`DatasetName`) REFERENCES `mythic_csv_dataset_meta` (`DatasetName`)
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_csv_abilities` (
  `AbilityId` int unsigned NOT NULL,
  `Name` varchar(255) NULL,
  `Description` text NULL,
  `Notes` text NULL,
  `IconId` int NULL,
  `AnimationId` int NULL,
  `EffectAbilityId` int NULL,
  `EffectId` int NULL,
  `SourceRowIndex` int unsigned NOT NULL,
  `RawJson` mediumtext NOT NULL,
  PRIMARY KEY (`AbilityId`),
  KEY `idx_mythic_csv_abilities_effect` (`EffectAbilityId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_csv_effects` (
  `EffectId` int unsigned NOT NULL,
  `Name` varchar(255) NULL,
  `BuildUpEffectId` int NULL,
  `BuildUpId` int NULL,
  `ActivateEffectId` int NULL,
  `ActivateId` int NULL,
  `CastEffectId` int NULL,
  `CastId` int NULL,
  `ProjectileMainId` int NULL,
  `ProjectileId` int NULL,
  `ImpactEffectId` int NULL,
  `ImpactId` int NULL,
  `AoeEffectId` int NULL,
  `AoeId` int NULL,
  `ChannelEffectId` int NULL,
  `ChannelingId` int NULL,
  `VfxRefId` int NULL,
  `VfxId` int NULL,
  `AoeTarget` int NULL,
  `AoeEffectsPerSec` int NULL,
  `AoeEffectsPerSecond` int NULL,
  `AoeRadius` int NULL,
  `AoeDuration` int NULL,
  `AoeLocation` int NULL,
  `WeaponTrail` int NULL,
  `ProjectileOff` int NULL,
  `ProjectileOverride` int NULL,
  `SourceRowIndex` int unsigned NOT NULL,
  `RawJson` mediumtext NOT NULL,
  PRIMARY KEY (`EffectId`),
  KEY `idx_mythic_csv_effects_activate` (`ActivateEffectId`),
  KEY `idx_mythic_csv_effects_cast` (`CastEffectId`),
  KEY `idx_mythic_csv_effects_impact` (`ImpactEffectId`),
  KEY `idx_mythic_csv_effects_aoe` (`AoeEffectId`),
  KEY `idx_mythic_csv_effects_channel` (`ChannelEffectId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Naming migration: add CSV-first canonical aliases while preserving legacy columns used by older code.
ALTER TABLE `mythic_csv_abilities`
  ADD COLUMN IF NOT EXISTS `EffectId` int NULL AFTER `EffectAbilityId`;

ALTER TABLE `mythic_csv_effects`
  ADD COLUMN IF NOT EXISTS `BuildUpId` int NULL AFTER `BuildUpEffectId`;
ALTER TABLE `mythic_csv_effects`
  ADD COLUMN IF NOT EXISTS `ActivateId` int NULL AFTER `ActivateEffectId`;
ALTER TABLE `mythic_csv_effects`
  ADD COLUMN IF NOT EXISTS `CastId` int NULL AFTER `CastEffectId`;
ALTER TABLE `mythic_csv_effects`
  ADD COLUMN IF NOT EXISTS `ProjectileId` int NULL AFTER `ProjectileMainId`;
ALTER TABLE `mythic_csv_effects`
  ADD COLUMN IF NOT EXISTS `ImpactId` int NULL AFTER `ImpactEffectId`;
ALTER TABLE `mythic_csv_effects`
  ADD COLUMN IF NOT EXISTS `AoeId` int NULL AFTER `AoeEffectId`;
ALTER TABLE `mythic_csv_effects`
  ADD COLUMN IF NOT EXISTS `ChannelingId` int NULL AFTER `ChannelEffectId`;
ALTER TABLE `mythic_csv_effects`
  ADD COLUMN IF NOT EXISTS `VfxId` int NULL AFTER `VfxRefId`;
ALTER TABLE `mythic_csv_effects`
  ADD COLUMN IF NOT EXISTS `AoeEffectsPerSecond` int NULL AFTER `AoeEffectsPerSec`;

-- Backfill both directions so either naming style can be used safely.
UPDATE `mythic_csv_abilities`
SET
  `EffectId` = COALESCE(NULLIF(`EffectId`, 0), `EffectAbilityId`),
  `EffectAbilityId` = COALESCE(NULLIF(`EffectAbilityId`, 0), `EffectId`);

UPDATE `mythic_csv_effects`
SET
  `BuildUpId` = COALESCE(NULLIF(`BuildUpId`, 0), `BuildUpEffectId`),
  `BuildUpEffectId` = COALESCE(NULLIF(`BuildUpEffectId`, 0), `BuildUpId`),
  `ActivateId` = COALESCE(NULLIF(`ActivateId`, 0), `ActivateEffectId`),
  `ActivateEffectId` = COALESCE(NULLIF(`ActivateEffectId`, 0), `ActivateId`),
  `CastId` = COALESCE(NULLIF(`CastId`, 0), `CastEffectId`),
  `CastEffectId` = COALESCE(NULLIF(`CastEffectId`, 0), `CastId`),
  `ProjectileId` = COALESCE(NULLIF(`ProjectileId`, 0), `ProjectileMainId`),
  `ProjectileMainId` = COALESCE(NULLIF(`ProjectileMainId`, 0), `ProjectileId`),
  `ImpactId` = COALESCE(NULLIF(`ImpactId`, 0), `ImpactEffectId`),
  `ImpactEffectId` = COALESCE(NULLIF(`ImpactEffectId`, 0), `ImpactId`),
  `AoeId` = COALESCE(NULLIF(`AoeId`, 0), `AoeEffectId`),
  `AoeEffectId` = COALESCE(NULLIF(`AoeEffectId`, 0), `AoeId`),
  `ChannelingId` = COALESCE(NULLIF(`ChannelingId`, 0), `ChannelEffectId`),
  `ChannelEffectId` = COALESCE(NULLIF(`ChannelEffectId`, 0), `ChannelingId`),
  `VfxId` = COALESCE(NULLIF(`VfxId`, 0), `VfxRefId`),
  `VfxRefId` = COALESCE(NULLIF(`VfxRefId`, 0), `VfxId`),
  `AoeEffectsPerSecond` = COALESCE(NULLIF(`AoeEffectsPerSecond`, 0), `AoeEffectsPerSec`),
  `AoeEffectsPerSec` = COALESCE(NULLIF(`AoeEffectsPerSec`, 0), `AoeEffectsPerSecond`);
