CREATE TABLE IF NOT EXISTS `mythic_bin_ability` (
  `ID` bigint(20) NOT NULL,
  `Name` varchar(500) DEFAULT NULL,
  `Description` longtext,
  `EffectID` int(11) DEFAULT NULL,
  `MinLevel` int(11) DEFAULT NULL,
  `IsBuff` tinyint(1) DEFAULT NULL,
  `IsDebuff` tinyint(1) DEFAULT NULL,
  `IsDamaging` tinyint(1) DEFAULT NULL,
  `IsHealing` tinyint(1) DEFAULT NULL,
  `IsGranted` tinyint(1) DEFAULT NULL,
  `IsPassive` tinyint(1) DEFAULT NULL,
  `IsOffensive` tinyint(1) DEFAULT NULL,
  `Flags` int(11) DEFAULT NULL,
  `Casttime` int(11) DEFAULT NULL,
  `Cooldown` int(11) DEFAULT NULL,
  `AP` int(11) DEFAULT NULL,
  `Specialization` int(11) DEFAULT NULL,
  `ChannelInterval` int(11) DEFAULT NULL,
  `TacticType` int(11) DEFAULT NULL,
  `Range` int(11) DEFAULT NULL,
  `MoraleLevel` int(11) DEFAULT NULL,
  `MoraleCost` int(11) DEFAULT NULL,
  `CastType` int(11) DEFAULT NULL,
  `SpellDamageType` int(11) DEFAULT NULL,
  `TargetType` bigint(20) DEFAULT NULL,
  `MaxTargets` int(11) DEFAULT NULL,
  `MythicComponentData` longtext,
  `ComponentData` longtext,
  `RequirmentsData` longtext,
  `ComponentDataValues` longtext,
  `ComponentTriggers` longtext,
  `FlagBits` longtext,
  PRIMARY KEY (`ID`),
  KEY `idx_mythic_bin_ability_effect` (`EffectID`),
  KEY `idx_mythic_bin_ability_target` (`TargetType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_bin_abilitybin` (
  `ID` bigint(20) NOT NULL,
  `EffectID` int(11) DEFAULT NULL,
  `MinLevel` int(11) DEFAULT NULL,
  `TargetType` bigint(20) DEFAULT NULL,
  `Range` int(11) DEFAULT NULL,
  `AP` int(11) DEFAULT NULL,
  `Castime` int(11) DEFAULT NULL,
  `Cooldown` int(11) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `idx_mythic_bin_abilitybin_effect` (`EffectID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_bin_abilitycomponentbin` (
  `ID` bigint(20) NOT NULL,
  `A00` int(11) DEFAULT NULL,
  `Values` longtext,
  `Multipliers` longtext,
  `ActivationDelay` bigint(20) DEFAULT NULL,
  `Duration` bigint(20) DEFAULT NULL,
  `Flags` bigint(20) DEFAULT NULL,
  `Interval` bigint(20) DEFAULT NULL,
  `Radius` int(11) DEFAULT NULL,
  `ConeAngle` int(11) DEFAULT NULL,
  `FlightSpeed` int(11) DEFAULT NULL,
  `MaxTargets` tinyint(3) unsigned DEFAULT NULL,
  `Description` longtext,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_bin_abilitycomponentlink` (
  `ID` bigint(20) NOT NULL,
  `AbilityID` bigint(20) DEFAULT NULL,
  `ComponentID` bigint(20) DEFAULT NULL,
  `Trigger` bigint(20) DEFAULT NULL,
  `VfxID` tinyint(3) unsigned DEFAULT NULL,
  `Index` tinyint(3) unsigned DEFAULT NULL,
  `Disabled` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `idx_mythic_bin_componentlink_ability` (`AbilityID`),
  KEY `idx_mythic_bin_componentlink_component` (`ComponentID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_bin_abilityexpression` (
  `ID` bigint(20) NOT NULL,
  `AbilityID` bigint(20) DEFAULT NULL,
  `ComponentID` bigint(20) DEFAULT NULL,
  `Index` int(11) DEFAULT NULL,
  `Type` bigint(20) DEFAULT NULL,
  `Operation` bigint(20) DEFAULT NULL,
  `Condition` bigint(20) DEFAULT NULL,
  `LogicOperator` bigint(20) DEFAULT NULL,
  `RequirmentID` bigint(20) DEFAULT NULL,
  `Disabled` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `idx_mythic_bin_expression_ability` (`AbilityID`),
  KEY `idx_mythic_bin_expression_component` (`ComponentID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_bin_abilityrequirmentbin` (
  `ID` bigint(20) NOT NULL,
  `Name` longtext,
  `Disabled` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_bin_abilityupgradebin` (
  `ID` bigint(20) NOT NULL,
  `UpgradeID` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_bin_abilityupgradeentry` (
  `ID` bigint(20) NOT NULL,
  `AbilityUpgradeBinID` bigint(20) DEFAULT NULL,
  `Index` int(11) DEFAULT NULL,
  `V1` int(11) DEFAULT NULL,
  `V2` int(11) DEFAULT NULL,
  `V3` int(11) DEFAULT NULL,
  `V4` int(11) DEFAULT NULL,
  `V5` int(11) DEFAULT NULL,
  `V6` int(11) DEFAULT NULL,
  `V7` int(11) DEFAULT NULL,
  `V8` int(11) DEFAULT NULL,
  `Values` longtext,
  PRIMARY KEY (`ID`),
  KEY `idx_mythic_bin_upgradeentry_bin` (`AbilityUpgradeBinID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `mythic_bin_itemability` (
  `ID` bigint(20) NOT NULL,
  `AbilityID` bigint(20) DEFAULT NULL,
  `ItemID` bigint(20) DEFAULT NULL,
  `Time` bigint(20) DEFAULT NULL,
  `Unk1` int(11) DEFAULT NULL,
  `Cooldown` int(11) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `idx_mythic_bin_itemability_ability` (`AbilityID`),
  KEY `idx_mythic_bin_itemability_item` (`ItemID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
