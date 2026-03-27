-- Apply this after importing the base dumps and prior incremental updates.
--
-- Land of the Dead expedition access is driven by a realm resource race:
--   1. Order and Destruction earn points from real Tier 4 battlefront locks.
--   2. Once a realm reaches the threshold, expedition flights to zone 191 unlock for that realm only.
--   3. The opposing realm loses access for the ownership window.
--   4. After the ownership timer expires, the race resets and starts again.
--
-- Evidence used for this implementation:
--   - zone_taxis already contains the expedition destinations for zone 191 for both realms.
--   - client/system data exposes RRQ display type ERRQDISPLAY_TOMB_KINGS plus
--     TEXT_TOMB_KINGS_DUNGEON_ACCESS_LINE1, TEXT_TOMB_KINGS_DUNGEON_ACCESS_LINE2,
--     and TEXT_TOMB_KINGS_RRQ_UNPAUSED.
--   - retail packet captures for F_RRQ show a Tomb Kings tracker threshold of 500.
--
-- Configurable defaults seeded here:
--   - Threshold = 500
--   - PointsPerBattlefrontLock = 100
--   - UnlockDurationMinutes = 30
--
-- The threshold is backed by client packet evidence.
-- The per-lock award and ownership duration are server defaults because an exact retail
-- lock-value schedule was not recoverable from the local extracted text/system files alone.

USE war_world;

START TRANSACTION;

CREATE TABLE IF NOT EXISTS lotd_resource_tracker (
    TrackerId TINYINT UNSIGNED NOT NULL,
    State TINYINT UNSIGNED NOT NULL,
    OwningRealm TINYINT UNSIGNED NOT NULL,
    OrderResourcePoints INT NOT NULL,
    DestructionResourcePoints INT NOT NULL,
    Threshold INT NOT NULL,
    PointsPerBattlefrontLock INT NOT NULL,
    UnlockDurationMinutes INT NOT NULL,
    UnlockEndsOnUtc DATETIME NULL,
    LastScoringRealm TINYINT UNSIGNED NOT NULL,
    LastUpdatedOnUtc DATETIME NOT NULL,
    PRIMARY KEY (TrackerId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

INSERT INTO lotd_resource_tracker (
    TrackerId,
    State,
    OwningRealm,
    OrderResourcePoints,
    DestructionResourcePoints,
    Threshold,
    PointsPerBattlefrontLock,
    UnlockDurationMinutes,
    UnlockEndsOnUtc,
    LastScoringRealm,
    LastUpdatedOnUtc
)
SELECT
    1,
    0,
    0,
    0,
    0,
    500,
    100,
    30,
    NULL,
    0,
    UTC_TIMESTAMP()
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1
    FROM lotd_resource_tracker
    WHERE TrackerId = 1
);

COMMIT;

-- Verification:
SELECT
    TrackerId,
    State,
    OwningRealm,
    OrderResourcePoints,
    DestructionResourcePoints,
    Threshold,
    PointsPerBattlefrontLock,
    UnlockDurationMinutes,
    UnlockEndsOnUtc,
    LastScoringRealm,
    LastUpdatedOnUtc
FROM lotd_resource_tracker;

SELECT ZoneID, RealmID, WorldX, WorldY, WorldZ, WorldO, Tier, Enable
FROM zone_taxis
WHERE ZoneID = 191
ORDER BY RealmID;
