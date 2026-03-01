USE war_world;

-- Update the newly imported MYP extract rows to Source = 1
UPDATE item_infos SET Source = 1 WHERE Entry NOT IN (1, 2, 3, 4, 5, 206260, 206261, 206262, 206263, 206264, 206265, 206266, 208573, 212170, 216254, 216255, 216256, 216257);

-- For creatures, anything that was newly added has a Source = 0 initially because of the column default, 
-- but we skipped the first 629 hand curated ones.
-- The python script inserted from 630 onwards (roughly). Let's just set everything to 1 first, then set the specific hand-curated ones back to 0.
UPDATE creature_protos SET Source = 1 WHERE Entry > 630;

-- Same for abilities, python skipped the first 2629. We'll set the higher ones to 1.
UPDATE abilities SET Source = 1 WHERE Entry > 2630;

-- Now migrate Londo's item stats over the MYP items (Source = 1)
-- Only update items where our MYP extraction resulted in 0 DPS/Speed/Armor but Londo has proper stats
UPDATE war_world.item_infos i
INNER JOIN war_londo.item l ON i.Entry = l.ID
SET 
    i.Dps = IFNULL(l.DPS, i.Dps),
    i.Speed = IFNULL(l.Speed, i.Speed),
    i.Armor = IFNULL(l.Unk18, i.Armor), -- Unk18 appears to map to armor in Londo's DB
    i.MinRank = IFNULL(l.MinLevel, i.MinRank),
    i.MinRenown = IFNULL(l.MinRenownLevel, i.MinRenown),
    i.SellPrice = IFNULL(l.SellPrice, i.SellPrice),
    i.MaxStack = IFNULL(l.MaxStackCount, i.MaxStack),
    i.TalismanSlots = IFNULL(l.TalismanSlotCount, i.TalismanSlots),
    i.UniqueEquiped = IFNULL(l.UniqueEquipped, i.UniqueEquiped),
    i.Source = 2 -- Mark as Londo's Server Import
WHERE 
    i.Source = 1 AND (l.DPS > 0 OR l.Speed > 0 OR l.Unk18 > 0);
