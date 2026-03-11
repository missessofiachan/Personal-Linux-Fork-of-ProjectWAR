-- Apply this after importing the base dumps.
-- This update normalizes confirmed identity domains to the client-backed system:
--   Race (characters.Race)
--   Realm (characters.Realm, characterinfo.Realm)
--   CareerLine (characters.CareerLine)
--   CareerName (characterinfo.CareerName)
--
-- It intentionally does not rewrite characterinfo.Career or characters.Career because
-- that numeric domain is still an emulator-side lookup key, not a proven client CareerId.

START TRANSACTION;

UPDATE war_world.characterinfo
SET
    Realm = CASE CareerLine
        WHEN 1 THEN 1
        WHEN 2 THEN 1
        WHEN 3 THEN 1
        WHEN 4 THEN 1
        WHEN 5 THEN 2
        WHEN 6 THEN 2
        WHEN 7 THEN 2
        WHEN 8 THEN 2
        WHEN 9 THEN 1
        WHEN 10 THEN 1
        WHEN 11 THEN 1
        WHEN 12 THEN 1
        WHEN 13 THEN 2
        WHEN 14 THEN 2
        WHEN 15 THEN 2
        WHEN 16 THEN 2
        WHEN 17 THEN 1
        WHEN 18 THEN 1
        WHEN 19 THEN 1
        WHEN 20 THEN 1
        WHEN 21 THEN 2
        WHEN 22 THEN 2
        WHEN 23 THEN 2
        WHEN 24 THEN 2
        ELSE Realm
    END,
    CareerName = CASE CareerLine
        WHEN 1 THEN 'Ironbreaker'
        WHEN 2 THEN 'Slayer'
        WHEN 3 THEN 'Runepriest'
        WHEN 4 THEN 'Engineer'
        WHEN 5 THEN 'Black Orc'
        WHEN 6 THEN 'Choppa'
        WHEN 7 THEN 'Shaman'
        WHEN 8 THEN 'Squig Herder'
        WHEN 9 THEN 'Witch Hunter'
        WHEN 10 THEN 'Knight of the Blazing Sun'
        WHEN 11 THEN 'Bright Wizard'
        WHEN 12 THEN 'Warrior Priest'
        WHEN 13 THEN 'Chosen'
        WHEN 14 THEN 'Marauder'
        WHEN 15 THEN 'Zealot'
        WHEN 16 THEN 'Magus'
        WHEN 17 THEN 'Swordmaster'
        WHEN 18 THEN 'Shadow Warrior'
        WHEN 19 THEN 'White Lion'
        WHEN 20 THEN 'Archmage'
        WHEN 21 THEN 'Blackguard'
        WHEN 22 THEN 'Witch Elf'
        WHEN 23 THEN 'Disciple of Khaine'
        WHEN 24 THEN 'Sorcerer'
        ELSE CareerName
    END
WHERE CareerLine BETWEEN 1 AND 24;

-- Repair character rows whose stored CareerLine/Realm drifted away from characterinfo.
UPDATE war_characters.characters AS c
JOIN war_world.characterinfo AS ci
    ON ci.Career = c.Career
SET
    c.CareerLine = ci.CareerLine,
    c.Realm = ci.Realm
WHERE c.CareerLine <> ci.CareerLine
   OR c.Realm <> ci.Realm
   OR c.CareerLine = 0
   OR c.CareerLine > 24;

-- Normalize character Race/Realm from the confirmed CareerLine mapping.
UPDATE war_characters.characters
SET
    Race = CASE CareerLine
        WHEN 1 THEN 1
        WHEN 2 THEN 1
        WHEN 3 THEN 1
        WHEN 4 THEN 1
        WHEN 5 THEN 2
        WHEN 6 THEN 2
        WHEN 7 THEN 3
        WHEN 8 THEN 3
        WHEN 9 THEN 6
        WHEN 10 THEN 6
        WHEN 11 THEN 6
        WHEN 12 THEN 6
        WHEN 13 THEN 7
        WHEN 14 THEN 7
        WHEN 15 THEN 7
        WHEN 16 THEN 7
        WHEN 17 THEN 4
        WHEN 18 THEN 4
        WHEN 19 THEN 4
        WHEN 20 THEN 4
        WHEN 21 THEN 5
        WHEN 22 THEN 5
        WHEN 23 THEN 5
        WHEN 24 THEN 5
        ELSE Race
    END,
    Realm = CASE CareerLine
        WHEN 1 THEN 1
        WHEN 2 THEN 1
        WHEN 3 THEN 1
        WHEN 4 THEN 1
        WHEN 5 THEN 2
        WHEN 6 THEN 2
        WHEN 7 THEN 2
        WHEN 8 THEN 2
        WHEN 9 THEN 1
        WHEN 10 THEN 1
        WHEN 11 THEN 1
        WHEN 12 THEN 1
        WHEN 13 THEN 2
        WHEN 14 THEN 2
        WHEN 15 THEN 2
        WHEN 16 THEN 2
        WHEN 17 THEN 1
        WHEN 18 THEN 1
        WHEN 19 THEN 1
        WHEN 20 THEN 1
        WHEN 21 THEN 2
        WHEN 22 THEN 2
        WHEN 23 THEN 2
        WHEN 24 THEN 2
        ELSE Realm
    END
WHERE CareerLine BETWEEN 1 AND 24;

COMMIT;

-- Optional verification queries.
SELECT CareerLine, CareerName, Realm
FROM war_world.characterinfo
WHERE CareerLine BETWEEN 1 AND 24
ORDER BY CareerLine;

SELECT CareerLine, Race, Realm, COUNT(*) AS CharacterCount
FROM war_characters.characters
WHERE CareerLine BETWEEN 1 AND 24
GROUP BY CareerLine, Race, Realm
ORDER BY CareerLine;
