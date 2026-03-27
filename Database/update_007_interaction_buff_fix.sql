-- Apply this after importing the base dumps and prior incremental updates.
--
-- Problem:
--   Battlefield objective and world-object capture flows use GameBuffs.Interaction (entry 60000).
--   If buff_infos does not contain that row, interaction attempts can never complete and BO flags
--   appear clickable but never capture.
--
-- Evidence:
--   - WorldServer logs show:
--       BuffInterface Attempt to queue a null Buff Info
--   - Object.BeginInteraction requests AbilityMgr.GetBuffInfo(60000).
--   - Interaction timing/completion is handled by InteractionBuff in code, so this repair only needs
--     the buff_infos shell row. No buff_commands rows are required for entry 60000.

USE war_world;

START TRANSACTION;

INSERT INTO buff_infos (
    Entry,
    Name,
    BuffClassString,
    TypeString,
    `Group`,
    AuraPropagation,
    MaxCopies,
    UseMaxStackAsInitial,
    MaxStack,
    StackLine,
    StacksFromCaster,
    Duration,
    LeadInDelay,
    Interval,
    PersistsOnDeath,
    CanRefresh,
    FriendlyEffectID,
    EnemyEffectID,
    Silent
)
VALUES (
    60000,
    'Interaction',
    'Standard',
    'None',
    0,
    '',
    1,
    0,
    1,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0
)
ON DUPLICATE KEY UPDATE
    Name = VALUES(Name),
    BuffClassString = VALUES(BuffClassString),
    TypeString = VALUES(TypeString),
    `Group` = VALUES(`Group`),
    AuraPropagation = VALUES(AuraPropagation),
    MaxCopies = VALUES(MaxCopies),
    UseMaxStackAsInitial = VALUES(UseMaxStackAsInitial),
    MaxStack = VALUES(MaxStack),
    StackLine = VALUES(StackLine),
    StacksFromCaster = VALUES(StacksFromCaster),
    Duration = VALUES(Duration),
    LeadInDelay = VALUES(LeadInDelay),
    Interval = VALUES(Interval),
    PersistsOnDeath = VALUES(PersistsOnDeath),
    CanRefresh = VALUES(CanRefresh),
    FriendlyEffectID = VALUES(FriendlyEffectID),
    EnemyEffectID = VALUES(EnemyEffectID),
    Silent = VALUES(Silent);

COMMIT;

-- Verification:
SELECT
    Entry,
    Name,
    BuffClassString,
    TypeString,
    `Group`,
    MaxCopies,
    MaxStack,
    Duration,
    PersistsOnDeath
FROM buff_infos
WHERE Entry = 60000;
