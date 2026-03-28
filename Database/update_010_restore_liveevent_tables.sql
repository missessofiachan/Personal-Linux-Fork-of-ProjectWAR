-- Restores the archived live-event seed rows that were removed from war_world.
-- Source rows recovered from repo history:
--   - commit 665e2578d84e6168c50afdfed26d1f834921faff
--     SQL/001Uploaded/2021/MARCH/2021_03_26_00_liveevent_infos.sql
--   - commit b2b62266a8db24a26f645a57ba8a9e499bd6d2a6
--     SQL/liveevents db/liveevent_reward_infos.sql
--     SQL/liveevents db/liveevent_subtask_infos.sql
--     SQL/liveevents db/liveevent_task_infos.sql
--
-- All restored events are disabled by default so the emulator returns to a stable
-- dormant state. Set Allowed=1 on a specific event to activate it.

CREATE TABLE IF NOT EXISTS `liveevent_infos` (
  `Entry` int unsigned NOT NULL,
  `Title` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SubTitle` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TasksDescription` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ImageId` int unsigned NOT NULL,
  `StartDate` datetime DEFAULT NULL,
  `EndDate` datetime DEFAULT NULL,
  `Allowed` tinyint unsigned DEFAULT NULL,
  PRIMARY KEY (`Entry`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci ROW_FORMAT=DYNAMIC;

CREATE TABLE IF NOT EXISTS `liveevent_task_infos` (
  `Entry` int unsigned NOT NULL,
  `LiveEventId` int unsigned NOT NULL,
  `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TotalTasks` int NOT NULL,
  PRIMARY KEY (`Entry`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci ROW_FORMAT=DYNAMIC;

CREATE TABLE IF NOT EXISTS `liveevent_subtask_infos` (
  `Entry` int unsigned NOT NULL,
  `LiveEventTaskId` int unsigned NOT NULL,
  `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TaskCount` int unsigned NOT NULL,
  PRIMARY KEY (`Entry`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci ROW_FORMAT=DYNAMIC;

CREATE TABLE IF NOT EXISTS `liveevent_reward_infos` (
  `Entry` int unsigned NOT NULL,
  `LiveEventId` int unsigned NOT NULL,
  `RewardGroupId` int unsigned NOT NULL,
  `ItemId` int unsigned NOT NULL,
  PRIMARY KEY (`Entry`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci ROW_FORMAT=DYNAMIC;

INSERT INTO `liveevent_infos` (`Entry`, `Title`, `SubTitle`, `Description`, `TasksDescription`, `ImageId`, `StartDate`, `EndDate`, `Allowed`)
VALUES
  (1, 'Wild Hunt', 'The Age of Reckoning''s Most Dangerous Game', 'The Wild Hunt is upon the Elves, and Ulthuan is already sodden with blood. Hunters stalk the fields and forests in search of rare and savage prey, to demonstrate their skill to the God of the Hunt and perhaps gain his favor. Others seek merely to glimpse the fabled White Stag and gain Kurnous''s blessing. But Kurnous is a true hunter, and incurring his wrath carries a price ... of poetic justice.', 'Players who please Kurnous may receive a Bleeding Hart charm of the Fleet Stag Mantle. In Elf RVR lands, players can vie to capture Vale Vines and unlock an exclusive new zone: the Hunter''s Vale, where the God of the Hunt tests their mettle. The truly lucky may sight the White Stag as it wanders from zone to zone--and the truly unlucky may encounter the vicious Hounds of Kurnous, guardians of the sacred oaks.', 9, '2019-03-16 00:00:00', '2019-04-16 00:00:00', 0),
  (2, 'The Witching Night', 'Witching', 'The Witching Night Approaches in the Warhammer World. During this time, the divisions between the living and the dead grow thin, and the power of Shyish, the Purple Wind of Magic, waxes. Evil cults, witches, and necromancers use this time to their advantage, easily raising the dead and calling upon them to spread wickedness across the land.', 'The Witching Night is a Live Event and avaible only for a short time. Kill Restless Spirits, Withered Crones, and participate in the Conflict Public Quests in the RvR areas to unlock influance rewards and to aquire rare Witching Night masks!', 2, '2019-03-16 00:00:00', '2019-04-16 00:00:00', 0),
  (3, 'Heavy Metal', 'Metal', 'Every day of this event, from November 17th through December 1st, you can complete a single task to earn influence. That influence will contribute to the Event bar below (like a Chapter), and you can earn great rewards. Visit the Herald in Altdorf or the Inevitable City to receive your rewards.', 'The front lines of Order''s righteous struggle against the malign forces of Destruction will soon be bolstered by the Knights of the Blazing Sun! The Knights, called from all corners of the Empire to perform their duty, Will gather together in numbers never seen before! As elite warriors, expert tacticians, and brave templars of the goddess Myrmidia, the Knight of the Blazing Sun are dedicated to claiming complete victory over Chaos and its allies! Do your part to ensure their succes! The Empire''s survival depends upon your contribution!', 15, '2019-03-16 00:00:00', '2019-04-16 00:00:00', 0),
  (4, 'Keg End', '', 'The Dwarf Celebrations of Keg End approaches in the Warhammer World.At this time,all of the year''s ale must be consumed before the New Year arrives or else a terrible bad luck will befall the citizens of the Old World rise to the occasion.A certain amount of competitive boasting is traditional as well.The forces of destruction are eager as allways to spoil the event by drinking the ale for themselves or by stealing any of the holiday celebrations.', 'Gain Event Influence by completing any of the tasks below. Massive Ogres lurk in the open RvR Battlefields. Brew-Thirsty Ogres, Drunken Gnoblars, and Explosive Snotlings have stolen caches of beer and crates of fireworks. Scouting reports suggest their locations to be in PvE areas along the roads and near major landmarks or Public Quests. Enemy players are also a great scource for aquiring any of these items. Amazingly - rumors of the legendary Golden stein appearing have surfaced. Scour the RvR Battlefields of Nordland, Barak Varr, Black Fire Pass, Praag, Thunder Mountain, and Dragon Wake to obtain it. All those who bask in its foamy radiance will be rewarded. And for the exceptionally lucky participating in any of these tasks may yield the elusive Keg Backpack.', 1, '2021-03-22 00:00:00', '2021-03-31 00:00:00', 0)
ON DUPLICATE KEY UPDATE
  `Title` = VALUES(`Title`),
  `SubTitle` = VALUES(`SubTitle`),
  `Description` = VALUES(`Description`),
  `TasksDescription` = VALUES(`TasksDescription`),
  `ImageId` = VALUES(`ImageId`),
  `StartDate` = VALUES(`StartDate`),
  `EndDate` = VALUES(`EndDate`),
  `Allowed` = VALUES(`Allowed`);

INSERT INTO `liveevent_task_infos` (`Entry`, `LiveEventId`, `Name`, `Description`, `TotalTasks`)
VALUES
  (29, 4, 'Massive Ogres Tyrants Slain', 'Ogres Tyrants Slain', 20),
  (30, 4, '/Boast or /Toast a member of every career', 'Boast or Toast', 24),
  (31, 4, 'Drink from 3 steins within the pubs and feast halls of the capital cities', 'Drink from steins', 3),
  (32, 4, '/Boast 20 different dead enemy players', 'Boast dead enemy players', 20),
  (33, 4, 'Drink Dwarf Beer Kegs retrieved from Brew-Thirsty Ogres, Drunken Gnoblars, and Explosive Snotlings', 'Drink Dwarf Beer Kegs', 100),
  (34, 4, 'Explosive Snotlings Slain', 'Snotlings', 50),
  (35, 4, 'Brew-Thirsty Ogres Slain', 'Ogres', 50),
  (36, 4, 'Drunken Gnoblar Slain', 'Gnoblar', 50),
  (37, 4, 'Keg Caches Guzzled', 'Keg Caches', 50),
  (38, 4, 'Launch 10 Common Fireworks', 'Common Fireworks', 10),
  (39, 4, 'Launch 5 Impressive Fireworks', 'Impressive Fireworks', 5),
  (40, 4, 'Launch 1 Magnificent Firework', 'Magnificent Firework', 1),
  (41, 4, 'Defeat 20 Enemy Players', 'Enemy Players', 20),
  (42, 4, 'Complete all Keg End Tasks Title: Keg Slayer!', 'Complete all Keg End Tasks', 13)
ON DUPLICATE KEY UPDATE
  `LiveEventId` = VALUES(`LiveEventId`),
  `Name` = VALUES(`Name`),
  `Description` = VALUES(`Description`),
  `TotalTasks` = VALUES(`TotalTasks`);

INSERT INTO `liveevent_reward_infos` (`Entry`, `LiveEventId`, `RewardGroupId`, `ItemId`)
VALUES
  (1, 4, 1, 7190),
  (2, 4, 2, 129838731),
  (3, 4, 4, 206655)
ON DUPLICATE KEY UPDATE
  `LiveEventId` = VALUES(`LiveEventId`),
  `RewardGroupId` = VALUES(`RewardGroupId`),
  `ItemId` = VALUES(`ItemId`);
