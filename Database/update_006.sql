-- ProjectWAR Update Script 006
-- Description: Fix Lost Vale entrance portal routing, including Destruction entrance ID 211812648.
--   - Ensures both Lost Vale entrance jump rows exist and are correctly configured
--     for instance entry into zone 260.
--   - Normalizes the Lost Vale instance exit jump references.
-- Author: Codex (GPT-5) 2026-03-06
-- Apply to: war_world
-- Command: mysql -uroot -ppassword -hlocalhost war_world < Database/update_006.sql

USE `war_world`;

-- Lost Vale entrances (order + destruction).
-- Keep these as type 6 (group instance portal) with InstanceID 260.
INSERT INTO `zone_jumps`
    (`Entry`, `ZoneId`, `WorldX`, `WorldY`, `WorldZ`, `WorldO`, `Enabled`, `Type`, `InstanceID`)
VALUES
    (211812584, 260, 1410113, 1588043, 5888, 2034, 1, 6, 260),
    (211812648, 260, 1410092, 1588460, 5828, 2054, 1, 6, 260)
ON DUPLICATE KEY UPDATE
    `ZoneId` = VALUES(`ZoneId`),
    `WorldX` = VALUES(`WorldX`),
    `WorldY` = VALUES(`WorldY`),
    `WorldZ` = VALUES(`WorldZ`),
    `WorldO` = VALUES(`WorldO`),
    `Enabled` = VALUES(`Enabled`),
    `Type` = VALUES(`Type`),
    `InstanceID` = VALUES(`InstanceID`);

-- Wire Lost Vale exit jumps explicitly to the known LV exit portal entry.
UPDATE `instance_infos`
SET `OrderExitZoneJumpID` = 272804328,
    `DestrExitZoneJumpID` = 272804328
WHERE `Entry` = 260 OR `ZoneID` = 260;
