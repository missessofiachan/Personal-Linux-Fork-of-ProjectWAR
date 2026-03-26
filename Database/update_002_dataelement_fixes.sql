-- Fixes for DataElement changes that broke database mappings

-- ==============================================================================
-- DATABASE: war_characters
-- ==============================================================================
USE war_characters;

-- Add new Tactic slots
DELIMITER //
CREATE PROCEDURE AddTacticColumns()
BEGIN
    DECLARE table_count INT;
    SELECT COUNT(*) INTO table_count FROM information_schema.tables WHERE table_schema = 'war_characters' AND table_name = 'characters_value';
    
    IF table_count > 0 THEN
        IF NOT EXISTS (SELECT * FROM information_schema.columns WHERE table_schema = 'war_characters' AND table_name = 'characters_value' AND column_name = 'Tactic5') THEN
            SET @s = 'ALTER TABLE characters_value ADD COLUMN Tactic5 smallint unsigned DEFAULT NULL';
            PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
        END IF;
        IF NOT EXISTS (SELECT * FROM information_schema.columns WHERE table_schema = 'war_characters' AND table_name = 'characters_value' AND column_name = 'Tactic6') THEN
            SET @s = 'ALTER TABLE characters_value ADD COLUMN Tactic6 smallint unsigned DEFAULT NULL';
            PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
        END IF;
        IF NOT EXISTS (SELECT * FROM information_schema.columns WHERE table_schema = 'war_characters' AND table_name = 'characters_value' AND column_name = 'Tactic7') THEN
            SET @s = 'ALTER TABLE characters_value ADD COLUMN Tactic7 smallint unsigned DEFAULT NULL';
            PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
        END IF;
        IF NOT EXISTS (SELECT * FROM information_schema.columns WHERE table_schema = 'war_characters' AND table_name = 'characters_value' AND column_name = 'Tactic8') THEN
            SET @s = 'ALTER TABLE characters_value ADD COLUMN Tactic8 smallint unsigned DEFAULT NULL';
            PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
        END IF;
    END IF;
END //
DELIMITER ;
CALL AddTacticColumns();
DROP PROCEDURE AddTacticColumns;

-- ==============================================================================
-- DATABASE: war_world
-- ==============================================================================
USE war_world;

-- Tok_Bestiary table rename
DELIMITER //
CREATE PROCEDURE RenameBestiaryTable()
BEGIN
    DECLARE table_count INT;
    SELECT COUNT(*) INTO table_count FROM information_schema.tables WHERE table_schema = 'war_world' AND table_name = 'tok_bestary';
    IF table_count > 0 THEN
        SET @s = 'RENAME TABLE tok_bestary TO tok_bestiary';
        PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
    END IF;
END //
DELIMITER ;
CALL RenameBestiaryTable();
DROP PROCEDURE RenameBestiaryTable;

-- Bestary_ID column rename
DELIMITER //
CREATE PROCEDURE RenameBestiaryColumn()
BEGIN
    DECLARE table_count INT;
    SELECT COUNT(*) INTO table_count FROM information_schema.tables WHERE table_schema = 'war_world' AND table_name = 'tok_bestiary';
    IF table_count > 0 THEN
        IF EXISTS (SELECT * FROM information_schema.columns WHERE table_schema = 'war_world' AND table_name = 'tok_bestiary' AND column_name = 'Bestary_ID') THEN
            SET @s = 'ALTER TABLE tok_bestiary CHANGE COLUMN Bestary_ID Bestiary_ID int unsigned NOT NULL DEFAULT 0';
            PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
        END IF;
    END IF;
END //
DELIMITER ;
CALL RenameBestiaryColumn();
DROP PROCEDURE RenameBestiaryColumn;

-- creature_smart_abilities column rename
DELIMITER //
CREATE PROCEDURE RenameSmartAbilitySoundColumn()
BEGIN
    DECLARE table_count INT;
    SELECT COUNT(*) INTO table_count FROM information_schema.tables WHERE table_schema = 'war_world' AND table_name = 'creature_smart_abilities';
    IF table_count > 0 THEN
        IF EXISTS (SELECT * FROM information_schema.columns WHERE table_schema = 'war_world' AND table_name = 'creature_smart_abilities' AND column_name = 'Sound') THEN
            SET @s = 'ALTER TABLE creature_smart_abilities CHANGE COLUMN Sound SpellCastSound varchar(255) DEFAULT NULL';
            PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
        END IF;
    END IF;
END //
DELIMITER ;
CALL RenameSmartAbilitySoundColumn();
DROP PROCEDURE RenameSmartAbilitySoundColumn;

-- gameobject_protos column rename
DELIMITER //
CREATE PROCEDURE RenameGameObjectUnksColumn()
BEGIN
    DECLARE table_count INT;
    SELECT COUNT(*) INTO table_count FROM information_schema.tables WHERE table_schema = 'war_world' AND table_name = 'gameobject_protos';
    IF table_count > 0 THEN
        IF EXISTS (SELECT * FROM information_schema.columns WHERE table_schema = 'war_world' AND table_name = 'gameobject_protos' AND column_name = 'Unks') THEN
            SET @s = 'ALTER TABLE gameobject_protos CHANGE COLUMN Unks UnksString varchar(255) DEFAULT NULL';
            PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
        END IF;
    END IF;
END //
DELIMITER ;
CALL RenameGameObjectUnksColumn();
DROP PROCEDURE RenameGameObjectUnksColumn;

-- creature_protos add Source column
DELIMITER //
CREATE PROCEDURE AddCreatureProtoSource()
BEGIN
    DECLARE table_count INT;
    SELECT COUNT(*) INTO table_count FROM information_schema.tables WHERE table_schema = 'war_world' AND table_name = 'creature_protos';
    IF table_count > 0 THEN
        IF NOT EXISTS (SELECT * FROM information_schema.columns WHERE table_schema = 'war_world' AND table_name = 'creature_protos' AND column_name = 'Source') THEN
            SET @s = 'ALTER TABLE creature_protos ADD COLUMN Source tinyint unsigned NOT NULL DEFAULT 0';
            PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
        END IF;
    END IF;
END //
DELIMITER ;
CALL AddCreatureProtoSource();
DROP PROCEDURE AddCreatureProtoSource;
