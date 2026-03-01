USE war_world;

-- Drop previous procedures if they exist
DROP PROCEDURE IF EXISTS MigrateLondoItemStats;

-- Set delimiter for procedure creation
DELIMITER //

CREATE PROCEDURE MigrateLondoItemStats()
BEGIN
    -- Update item_infos with stats from war_londo.item
    -- Only update items where our MYP extraction resulted in 0 DPS/Armor but Londo has proper stats
    UPDATE war_world.item_infos i
    INNER JOIN war_londo.item l ON i.Entry = l.ID
    SET 
        i.Dps = IFNULL(l.DPS, i.Dps),
        i.Speed = IFNULL(l.Speed, i.Speed),
        i.Armor = IFNULL(l.Unk18, i.Armor), -- Unk18 appears to map to armor in Londo's DB based on previous reverse engineering
        i.MinRank = IFNULL(l.MinLevel, i.MinRank),
        i.MinRenown = IFNULL(l.MinRenownLevel, i.MinRenown),
        i.SellPrice = IFNULL(l.SellPrice, i.SellPrice),
        i.MaxStack = IFNULL(l.MaxStackCount, i.MaxStack),
        i.TalismanSlots = IFNULL(l.TalismanSlotCount, i.TalismanSlots),
        i.UniqueEquiped = IFNULL(l.UniqueEquipped, i.UniqueEquiped),
        i.Source = 2 -- Mark as Londo's Server Import
    WHERE 
        i.Source = 1 AND (l.DPS > 0 OR l.Speed > 0 OR l.Unk18 > 0);
        
    -- Note: We only update MYP extractions (Source = 1). We NEVER overwrite hand-curated emulator items (Source = 0).
END //

DELIMITER ;

-- Execute the migration
CALL MigrateLondoItemStats();

-- Clean up
DROP PROCEDURE MigrateLondoItemStats;
