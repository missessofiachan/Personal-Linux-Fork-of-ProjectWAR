CREATE TABLE IF NOT EXISTS `ability_entry_resolver` (
  `SourceEntry` smallint unsigned NOT NULL,
  `CanonicalAbilityEntry` smallint unsigned NOT NULL,
  `CanonicalBuffEntry` smallint unsigned NOT NULL DEFAULT '0',
  `ResolutionSource` varchar(48) NOT NULL DEFAULT 'manual',
  `Enabled` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `Notes` varchar(255) DEFAULT NULL,
  `UpdatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`SourceEntry`),
  KEY `idx_ability_entry_resolver_enabled` (`Enabled`),
  KEY `idx_ability_entry_resolver_canonical` (`CanonicalAbilityEntry`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
