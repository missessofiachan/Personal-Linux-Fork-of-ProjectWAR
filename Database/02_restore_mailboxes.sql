-- Restores the mailbox gameobject prototypes from the April 2020 backup
USE `war_world`;

REPLACE INTO `gameobject_protos` 
(`Entry`, `Name`, `DisplayID`, `Scale`, `Level`, `Faction`, `HealthPoints`, `ScriptName`, `TokUnlock`, `UnksString`, `Unk1`, `Unk2`, `Unk3`, `Unk4`, `CreatureId`, `CreatureCount`, `CreatureSpawnText`, `CreatureCooldownMinutes`, `IsAttackable`)
VALUES 
(44,'Mailbox',8301,50,1,0,1,'MailBoxScript',NULL,'7745 0 50943 0 5 44655',0,0,100,0,0,0,NULL,0,0),
(132,'Mailfing',8304,50,1,0,1,'MailBoxScript',NULL,'7746 0 50950 4 5 49458',0,0,100,0,0,0,NULL,0,0),
(200028,'Letter Carrier',8302,50,1,0,1,'MailBoxScript',NULL,'7745 0 50943 0 5 44655',0,0,100,0,0,0,NULL,0,0),
(2000570,'Mailbox chosen',4496,50,1,0,1,'MailBoxScript',NULL,'7745 0 50943 0 5 44655',0,0,100,0,0,0,NULL,0,0);
