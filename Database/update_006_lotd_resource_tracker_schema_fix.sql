-- Apply this after update_005 if your existing war_world.lotd_resource_tracker table was created
-- from an earlier draft that used INT UNSIGNED for the point and timer fields.
--
-- Symptom:
--   WorldServer logs show schema binding failures such as:
--     Table lotd_resource_tracker column orderresourcepoints Type mismatch (INT UNSIGNED in DB - INT in emulator)
--
-- Result:
--   The LOTD tracker object does not load, the RRQ bar is never sent to players, and expedition
--   flights remain hidden.

USE war_world;

START TRANSACTION;

ALTER TABLE lotd_resource_tracker
    MODIFY COLUMN OrderResourcePoints INT NOT NULL,
    MODIFY COLUMN DestructionResourcePoints INT NOT NULL,
    MODIFY COLUMN Threshold INT NOT NULL,
    MODIFY COLUMN PointsPerBattlefrontLock INT NOT NULL,
    MODIFY COLUMN UnlockDurationMinutes INT NOT NULL;

COMMIT;

-- Verification:
DESCRIBE lotd_resource_tracker;
