-- Drop CharacterUtility-specific artifacts from war_world
-- These columns were computed/populated exclusively by CharacterUtility
-- and are not referenced by the WorldServer or any other server component.

USE war_world;

-- Drop computed columns added by CharacterUtility to item_sets
ALTER TABLE item_sets
    DROP COLUMN IF EXISTS ItemSetList,
    DROP COLUMN IF EXISTS ItemSetFullDescription,
    DROP COLUMN IF EXISTS ClassId;

-- Drop the item_bonus table - only ever read by CharacterUtility
-- (used to resolve stat IDs to human-readable names for export)
-- NOT referenced by WorldServer.
DROP TABLE IF EXISTS item_bonus;

-- Drop the classes table - only ever read by CharacterUtility
-- (used to resolve career class IDs to class names for export)
-- NOT referenced by WorldServer.
DROP TABLE IF EXISTS classes;
