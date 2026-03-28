CREATE DATABASE  IF NOT EXISTS `war_accounts` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `war_accounts`;
-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: war_accounts
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `account_sanction_logs`
--

DROP TABLE IF EXISTS `account_sanction_logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `account_sanction_logs` (
  `AccountId` int DEFAULT NULL,
  `IssuedBy` varchar(24) DEFAULT NULL,
  `ActionType` varchar(24) DEFAULT NULL,
  `IssuerGmLevel` int DEFAULT NULL,
  `ActionDuration` text,
  `ActionLog` varchar(255) DEFAULT NULL,
  `ActionTime` int DEFAULT NULL,
  `account_sanction_logs_ID` varchar(255) NOT NULL,
  PRIMARY KEY (`account_sanction_logs_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `account_sanction_logs`
--

LOCK TABLES `account_sanction_logs` WRITE;
/*!40000 ALTER TABLE `account_sanction_logs` DISABLE KEYS */;
/*!40000 ALTER TABLE `account_sanction_logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `account_value`
--

DROP TABLE IF EXISTS `account_value`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `account_value` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `AccountId` int DEFAULT NULL,
  `InstallId` text,
  `IP` text,
  `MAC` text,
  `HDSerialHash` text,
  `CPUIDHash` text,
  `ModifyDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `account_value`
--

LOCK TABLES `account_value` WRITE;
/*!40000 ALTER TABLE `account_value` DISABLE KEYS */;
/*!40000 ALTER TABLE `account_value` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `accounts`
--

DROP TABLE IF EXISTS `accounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `accounts` (
  `AccountId` int NOT NULL AUTO_INCREMENT,
  `PacketLog` tinyint unsigned DEFAULT NULL,
  `Username` varchar(255) DEFAULT NULL,
  `Password` varchar(255) DEFAULT NULL,
  `CryptPassword` varchar(255) DEFAULT NULL,
  `Ip` varchar(255) DEFAULT NULL,
  `Token` varchar(255) DEFAULT NULL,
  `GmLevel` tinyint NOT NULL,
  `Banned` int NOT NULL,
  `BanReason` text,
  `AdviceBlockEnd` int DEFAULT NULL,
  `StealthMuteEnd` int DEFAULT NULL,
  `CoreLevel` int DEFAULT NULL,
  `LastLogged` int DEFAULT NULL,
  `LastNameChanged` int DEFAULT NULL,
  `LastPatcherLog` text,
  `InvalidPasswordCount` int unsigned NOT NULL,
  `noSurname` tinyint NOT NULL,
  `Email` text,
  PRIMARY KEY (`AccountId`),
  UNIQUE KEY `Username` (`Username`)
) ENGINE=InnoDB AUTO_INCREMENT=125394 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accounts`
--

LOCK TABLES `accounts` WRITE;
/*!40000 ALTER TABLE `accounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `accounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `blogs`
--

DROP TABLE IF EXISTS `blogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `blogs` (
  `BlogId` int NOT NULL AUTO_INCREMENT,
  `BlogTimestamp` datetime(6) DEFAULT NULL,
  `BlogText` mediumtext,
  `BlogUrl` varchar(200) DEFAULT NULL,
  `BlogTitle` varchar(80) DEFAULT NULL,
  PRIMARY KEY (`BlogId`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `blogs`
--

LOCK TABLES `blogs` WRITE;
/*!40000 ALTER TABLE `blogs` DISABLE KEYS */;
/*!40000 ALTER TABLE `blogs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characters_value_24hr`
--

DROP TABLE IF EXISTS `characters_value_24hr`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characters_value_24hr` (
  `characterIdint` int NOT NULL,
  `Levelint` int DEFAULT NULL,
  `xpint` int DEFAULT NULL,
  `RenownRankint` int DEFAULT NULL,
  `Moneyint` int DEFAULT NULL,
  `timestampdatetime` timestamp NOT NULL,
  `characterId` int NOT NULL,
  PRIMARY KEY (`characterId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characters_value_24hr`
--

LOCK TABLES `characters_value_24hr` WRITE;
/*!40000 ALTER TABLE `characters_value_24hr` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_value_24hr` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ip_bans`
--

DROP TABLE IF EXISTS `ip_bans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ip_bans` (
  `Ip` varchar(255) NOT NULL,
  `Expire` int DEFAULT NULL,
  PRIMARY KEY (`Ip`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ip_bans`
--

LOCK TABLES `ip_bans` WRITE;
/*!40000 ALTER TABLE `ip_bans` DISABLE KEYS */;
/*!40000 ALTER TABLE `ip_bans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `launcher_files`
--

DROP TABLE IF EXISTS `launcher_files`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `launcher_files` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(2000) DEFAULT NULL,
  `CRC32` int unsigned DEFAULT NULL,
  `Size` bigint DEFAULT NULL,
  `GmLevel` int unsigned DEFAULT NULL,
  `ModifyDate` datetime DEFAULT NULL,
  `ModifyAccountId` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `launcher_files`
--

LOCK TABLES `launcher_files` WRITE;
/*!40000 ALTER TABLE `launcher_files` DISABLE KEYS */;
/*!40000 ALTER TABLE `launcher_files` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `launcher_hashes`
--

DROP TABLE IF EXISTS `launcher_hashes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `launcher_hashes` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(2000) DEFAULT NULL,
  `File` varchar(4000) DEFAULT NULL,
  `CRC32` int unsigned DEFAULT NULL,
  `Hash` bigint unsigned DEFAULT NULL,
  `Size` bigint DEFAULT NULL,
  `MetaDataSize` int unsigned DEFAULT NULL,
  `ArchiveId` int DEFAULT NULL,
  `ModifyDate` datetime DEFAULT NULL,
  `ModifyAccountId` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `launcher_hashes`
--

LOCK TABLES `launcher_hashes` WRITE;
/*!40000 ALTER TABLE `launcher_hashes` DISABLE KEYS */;
/*!40000 ALTER TABLE `launcher_hashes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `launcher_info`
--

DROP TABLE IF EXISTS `launcher_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `launcher_info` (
  `LauncherId` int NOT NULL AUTO_INCREMENT,
  `GmLevel` tinyint NOT NULL,
  `PatchNotes` text NOT NULL,
  `ServerState` int NOT NULL,
  `Version` int DEFAULT NULL,
  `FilePath` varchar(2000) DEFAULT NULL,
  `ModifiedDate` datetime DEFAULT NULL,
  `ModifyAccountId` int DEFAULT NULL,
  PRIMARY KEY (`LauncherId`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `launcher_info`
--

LOCK TABLES `launcher_info` WRITE;
/*!40000 ALTER TABLE `launcher_info` DISABLE KEYS */;
/*!40000 ALTER TABLE `launcher_info` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `launcher_myps`
--

DROP TABLE IF EXISTS `launcher_myps`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `launcher_myps` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(2000) DEFAULT NULL,
  `CRC32` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `launcher_myps`
--

LOCK TABLES `launcher_myps` WRITE;
/*!40000 ALTER TABLE `launcher_myps` DISABLE KEYS */;
/*!40000 ALTER TABLE `launcher_myps` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `realms`
--

DROP TABLE IF EXISTS `realms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `realms` (
  `RealmId` tinyint unsigned NOT NULL DEFAULT '0',
  `Name` varchar(255) DEFAULT NULL,
  `Language` varchar(255) DEFAULT NULL,
  `Adresse` varchar(255) DEFAULT NULL,
  `Port` int NOT NULL,
  `AllowTrials` varchar(32) DEFAULT NULL,
  `CharfxerAvailable` varchar(32) DEFAULT NULL,
  `Legacy` varchar(32) DEFAULT NULL,
  `BonusDestruction` varchar(32) DEFAULT NULL,
  `BonusOrder` varchar(32) DEFAULT NULL,
  `Redirect` varchar(32) DEFAULT NULL,
  `Region` varchar(32) DEFAULT NULL,
  `Retired` varchar(32) DEFAULT NULL,
  `WaitingDestruction` varchar(32) DEFAULT NULL,
  `WaitingOrder` varchar(32) DEFAULT NULL,
  `DensityDestruction` varchar(32) DEFAULT NULL,
  `DensityOrder` varchar(32) DEFAULT NULL,
  `OpenRvr` varchar(32) DEFAULT NULL,
  `Rp` varchar(32) DEFAULT NULL,
  `Status` varchar(32) DEFAULT NULL,
  `Online` tinyint unsigned NOT NULL,
  `OnlineDate` datetime DEFAULT NULL,
  `OnlinePlayers` int unsigned DEFAULT NULL,
  `OrderCount` int unsigned DEFAULT NULL,
  `DestructionCount` int unsigned DEFAULT NULL,
  `MaxPlayers` int unsigned DEFAULT NULL,
  `OrderCharacters` int unsigned DEFAULT NULL,
  `DestruCharacters` int unsigned DEFAULT NULL,
  `NextRotationTime` bigint DEFAULT NULL,
  `MasterPassword` text,
  `BootTime` int DEFAULT NULL,
  PRIMARY KEY (`RealmId`),
  UNIQUE KEY `RealmId` (`RealmId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `realms`
--

LOCK TABLES `realms` WRITE;
/*!40000 ALTER TABLE `realms` DISABLE KEYS */;
INSERT INTO `realms` VALUES (1,'ProjectWAR','EN','127.0.0.1',10300,'0','0','0','0','0','0','STR_REGION_NORTHAMERICA','0','0','0','0','0','0','0','0',1,'2019-12-18 16:32:29',0,0,0,1000,0,0,1532563200,'',1576704749);
/*!40000 ALTER TABLE `realms` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-28 18:29:05
