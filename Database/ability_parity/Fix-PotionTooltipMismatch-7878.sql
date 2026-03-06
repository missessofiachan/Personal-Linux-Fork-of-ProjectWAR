-- Fix tooltip/effect parity for potion spell 7878 (Lasting Elixir of Allaying).
-- Mythic component data is Value=50, Multiplier=160, Duration=24s, Interval=4s.
-- Expected total over duration = (50 * 160 / 100) * (24 / 4) = 480.
-- Existing value 3600 causes major over-heal and tooltip mismatch.

USE war_world_curated;

START TRANSACTION;

UPDATE `ability_damage_heals`
SET `MinDamage` = 480
WHERE `Entry` = 7878
  AND `Index` = 1
  AND `ParentCommandID` = 0
  AND `ParentCommandSequence` = 0;

UPDATE `mythic_src_ability_damage_heals`
SET `MinDamage` = 480
WHERE `Entry` = 7878
  AND `Index` = 1
  AND `ParentCommandID` = 0
  AND `ParentCommandSequence` = 0;

COMMIT;
