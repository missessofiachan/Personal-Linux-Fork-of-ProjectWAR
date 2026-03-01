USE war_world;

-- Add Source column to track data provenance:
-- 0 = Custom/Emulator hand-curated
-- 1 = MYP Extraction (WAR-RE-Toolkit)
-- 2 = Londo's Server Import

ALTER TABLE item_infos ADD COLUMN Source TINYINT UNSIGNED NOT NULL DEFAULT 0;
ALTER TABLE creature_protos ADD COLUMN Source TINYINT UNSIGNED NOT NULL DEFAULT 0;
ALTER TABLE abilities ADD COLUMN Source TINYINT UNSIGNED NOT NULL DEFAULT 0;

-- Update the newly imported MYP extract rows to Source = 1
UPDATE item_infos SET Source = 1 WHERE Entry NOT IN (SELECT Entry FROM (SELECT Entry FROM item_infos LIMIT 18) AS subquery);
UPDATE creature_protos SET Source = 1 WHERE Entry NOT IN (SELECT Entry FROM (SELECT Entry FROM creature_protos LIMIT 629) AS subquery);
UPDATE abilities SET Source = 1 WHERE Entry NOT IN (SELECT Entry FROM (SELECT Entry FROM abilities LIMIT 2629) AS subquery);
