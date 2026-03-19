-- Fixes for DataElement changes that broke database mappings

-- ==============================================================================
-- DATABASE: war_characters
-- ==============================================================================
USE war_characters;

-- Add new Tactic slots mapped in Characters_info.cs
ALTER TABLE characters_value
    ADD COLUMN IF NOT EXISTS Tactic5 smallint(5) unsigned DEFAULT NULL,
    ADD COLUMN IF NOT EXISTS Tactic6 smallint(5) unsigned DEFAULT NULL,
    ADD COLUMN IF NOT EXISTS Tactic7 smallint(5) unsigned DEFAULT NULL,
    ADD COLUMN IF NOT EXISTS Tactic8 smallint(5) unsigned DEFAULT NULL;

-- ==============================================================================
-- DATABASE: war_world
-- ==============================================================================
USE war_world;

-- Tok_Bestiary table and column renames
RENAME TABLE IF EXISTS tok_bestary TO tok_bestiary;

DELIMITER //
CREATE PROCEDURE RenameBestiaryColumn()
BEGIN
    IF EXISTS (SELECT * FROM information_schema.columns WHERE table_schema = 'war_world' AND table_name = 'tok_bestiary' AND column_name = 'Bestary_ID') THEN
        ALTER TABLE tok_bestiary CHANGE COLUMN Bestary_ID Bestiary_ID int(11) unsigned NOT NULL DEFAULT 0;
    END IF;
END //
DELIMITER ;
CALL RenameBestiaryColumn();
DROP PROCEDURE RenameBestiaryColumn;

-- creature_smart_abilities column rename (Sound -> SpellCastSound)
DELIMITER //
CREATE PROCEDURE RenameSmartAbilitySoundColumn()
BEGIN
    IF EXISTS (SELECT * FROM information_schema.columns WHERE table_schema = 'war_world' AND table_name = 'creature_smart_abilities' AND column_name = 'Sound') THEN
        ALTER TABLE creature_smart_abilities CHANGE COLUMN Sound SpellCastSound varchar(255) DEFAULT NULL;
    END IF;
END //
DELIMITER ;
CALL RenameSmartAbilitySoundColumn();
DROP PROCEDURE RenameSmartAbilitySoundColumn;

-- gameobject_protos column rename (Unks -> UnksString)
DELIMITER //
CREATE PROCEDURE RenameGameObjectUnksColumn()
BEGIN
    IF EXISTS (SELECT * FROM information_schema.columns WHERE table_schema = 'war_world' AND table_name = 'gameobject_protos' AND column_name = 'Unks') THEN
        ALTER TABLE gameobject_protos CHANGE COLUMN Unks UnksString varchar(255) DEFAULT NULL;
    END IF;
END //
DELIMITER ;
CALL RenameGameObjectUnksColumn();
DROP PROCEDURE RenameGameObjectUnksColumn;

-- creature_proto add Source column
ALTER TABLE creature_proto
    ADD COLUMN IF NOT EXISTS Source tinyint(3) unsigned NOT NULL DEFAULT 0;
