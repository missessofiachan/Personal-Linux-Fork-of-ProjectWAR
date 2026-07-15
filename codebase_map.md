# Codebase File Map

**Root Directory:** `ProjectWAR/`

## Directory Tree
```text
ProjectWAR/
├── .db_setup_done
├── .gitignore
├── .vscode/
│   └── settings.json
├── AGENTS.md
├── AccountCacher/
│   ├── AccountCacher.csproj
│   ├── Configs/
│   │   └── AccountConfigs.cs
│   ├── Console/
│   │   └── CreateAccount.cs
│   ├── Core.cs
│   ├── Properties/
│   │   └── AssemblyInfo.cs
│   ├── account.ico
│   ├── app.config
│   └── packages.config
├── Common/
│   ├── Appccelerate.StateMachine.sln
│   ├── Common.csproj
│   ├── Database/
│   │   ├── Account/
│   │   │   ├── Account.cs
│   │   │   ├── AccountSanctionInfo.cs
│   │   │   ├── Account_pending.cs
│   │   │   ├── Account_value.cs
│   │   │   ├── Ip_ban.cs
│   │   │   └── Realm.cs
│   │   ├── Character/
│   │   │   ├── Auction.cs
│   │   │   ├── BannedNameRecord.cs
│   │   │   ├── Bug_report.cs
│   │   │   ├── CharacterClientData.cs
│   │   │   ├── CharacterDeletionRecord.cs
│   │   │   ├── CharacterSavedBuff.cs
│   │   │   ├── Characters.cs
│   │   │   ├── Characters_ability.cs
│   │   │   ├── Characters_bag_pools.cs
│   │   │   ├── Characters_influence.cs
│   │   │   ├── Characters_info.cs
│   │   │   ├── Characters_items.cs
│   │   │   ├── Characters_mails.cs
│   │   │   ├── Characters_quests.cs
│   │   │   ├── Characters_socials.cs
│   │   │   ├── Characters_tok.cs
│   │   │   ├── Characters_tok_Kills.cs
│   │   │   ├── GMCommandLog.cs
│   │   │   ├── GuildVaultItem.cs
│   │   │   ├── Guild_Alliance_info.cs
│   │   │   ├── Guild_event.cs
│   │   │   ├── Guild_info.cs
│   │   │   ├── Guild_log.cs
│   │   │   ├── Guild_members.cs
│   │   │   ├── Guild_rank.cs
│   │   │   └── ScenarioDurationRecord.cs
│   │   ├── GameData.cs
│   │   ├── Patch/
│   │   │   └── Launcher_myps.cs
│   │   ├── SystemData.cs
│   │   └── World/
│   │       ├── Ability/
│   │       │   ├── AbilityCommands.cs
│   │       │   ├── AbilityDamageHeals.cs
│   │       │   ├── AbilityKnockbackInfo.cs
│   │       │   ├── AbilityModifierChecks.cs
│   │       │   ├── AbilityModifierEffects.cs
│   │       │   ├── BuffCommands.cs
│   │       │   ├── DBAbilityInfo.cs
│   │       │   └── DB_BuffInfo.cs
│   │       ├── Battlefront/
│   │       │   ├── BattleFrontKeepStatus.cs
│   │       │   ├── BattlefrontObject.cs
│   │       │   ├── BattlefrontObjectType.cs
│   │       │   ├── BattlefrontResourceSpawn.cs
│   │       │   ├── BattlefrontStatus.cs
│   │       │   ├── Battlefront_Guard.cs
│   │       │   ├── Battlefront_Objective.cs
│   │       │   ├── CampaignObjectiveBuff.cs
│   │       │   ├── ContributionAnalytics.cs
│   │       │   ├── ContributionDefinition.cs
│   │       │   ├── HonorHistory.cs
│   │       │   ├── KeepLockBagRewardHistory.cs
│   │       │   ├── KeepLockEligibilityHistory.cs
│   │       │   ├── KeepSiegeSpawnPoints.cs
│   │       │   ├── Keep_Creature.cs
│   │       │   ├── Keep_Door.cs
│   │       │   ├── Keep_Info.cs
│   │       │   ├── KillTracking.cs
│   │       │   ├── PlayerContribution.cs
│   │       │   ├── PlayerKeepSpawn.cs
│   │       │   ├── PlayerKillRewardHistory.cs
│   │       │   ├── RVRAreaPolygon.cs
│   │       │   ├── RVRKeepLockReward.cs
│   │       │   ├── RVRMetrics.cs
│   │       │   ├── RVRPairing.cs
│   │       │   ├── RVRPlayerBagBonus.cs
│   │       │   ├── RVRProgression.cs
│   │       │   ├── RVRRewardFortItems.cs
│   │       │   ├── RVRRewardKeepItems.cs
│   │       │   ├── RVRZoneLockReward.cs
│   │       │   ├── RvRObjectInfo.cs
│   │       │   ├── ZoneLockEligibilityHistory.cs
│   │       │   └── ZoneLockSummary.cs
│   │       ├── Chapters/
│   │       │   ├── Chapter_Info.cs
│   │       │   ├── Chapter_Reward.cs
│   │       │   └── Tok_Info.cs
│   │       ├── Characters/
│   │       │   ├── CharacterInfo.cs
│   │       │   ├── CharacterInfo_item.cs
│   │       │   ├── CharacterInfo_renown.cs
│   │       │   ├── CharacterInfo_stats.cs
│   │       │   ├── HonorReward.cs
│   │       │   ├── HonorRewardCooldon.cs
│   │       │   ├── PetMasteryModifiers.cs
│   │       │   ├── PetStatOverride.cs
│   │       │   ├── Random_names.cs
│   │       │   └── Tok_Bestary.cs
│   │       ├── Creatures/
│   │       │   ├── Boss_Phases.cs
│   │       │   ├── Boss_Spawn.cs
│   │       │   ├── Boss_Spawn_Abilities.cs
│   │       │   ├── Creature_Smart_Abilities.cs
│   │       │   ├── Creature_abilities.cs
│   │       │   ├── Creature_item.cs
│   │       │   ├── Creature_proto.cs
│   │       │   ├── Creature_spawn.cs
│   │       │   ├── Creature_stats.cs
│   │       │   ├── Creature_text.cs
│   │       │   ├── Creature_vendor.cs
│   │       │   └── Vendor_items.cs
│   │       ├── GMCommands/
│   │       │   └── GMCommands.cs
│   │       ├── GameObjects/
│   │       │   ├── GameObject_loot.cs
│   │       │   ├── GameObject_proto.cs
│   │       │   └── GameObject_spawn.cs
│   │       ├── Instances/
│   │       │   ├── InstanceAttribute.cs
│   │       │   ├── Instance_Boss_Spawn.cs
│   │       │   ├── Instance_Encounter.cs
│   │       │   ├── Instance_Event.cs
│   │       │   ├── Instance_Event_Command.cs
│   │       │   ├── Instance_Info.cs
│   │       │   ├── Instance_Lockouts.cs
│   │       │   ├── Instance_Object.cs
│   │       │   ├── Instance_Script.cs
│   │       │   ├── Instance_Spawn.cs
│   │       │   ├── Instance_Spawn_State.cs
│   │       │   ├── Instance_Spawn_State_Ability.cs
│   │       │   └── Instances_Statistics.cs
│   │       ├── Items/
│   │       │   ├── BlackMarketItem.cs
│   │       │   ├── Dye_Info.cs
│   │       │   ├── Item_Info.cs
│   │       │   ├── Item_Set.cs
│   │       │   ├── MailItem.cs
│   │       │   └── Mount_Info.cs
│   │       ├── Levels/
│   │       │   ├── Guild_Xp.cs
│   │       │   ├── Renown_Info.cs
│   │       │   └── Xp_Info.cs
│   │       ├── LiveEvents/
│   │       │   ├── LiveEventReward_Info.cs
│   │       │   ├── LiveEventSubTask_info.cs
│   │       │   ├── LiveEventTask_Info.cs
│   │       │   └── LiveEvent_Info.cs
│   │       ├── LootGroups/
│   │       │   ├── Loot_Group.cs
│   │       │   ├── Loot_Group_Item.cs
│   │       │   └── Loot_Groups_Butcher.cs
│   │       ├── Maps/
│   │       │   ├── CellSpawns.cs
│   │       │   ├── RallyPoint.cs
│   │       │   ├── Zone_Area.cs
│   │       │   ├── Zone_Info.cs
│   │       │   ├── Zone_Jump.cs
│   │       │   ├── Zone_Respawn.cs
│   │       │   └── Zone_Taxi.cs
│   │       ├── PQuests/
│   │       │   ├── PQuest_Info.cs
│   │       │   ├── PQuest_Loot.cs
│   │       │   ├── PQuest_Loot_Crafting.cs
│   │       │   ├── PQuest_Objective.cs
│   │       │   └── PQuest_Spawn.cs
│   │       ├── Quests/
│   │       │   ├── Quests.cs
│   │       │   ├── Quests_Creature_Finisher.cs
│   │       │   ├── Quests_Creature_Starter.cs
│   │       │   ├── Quests_Map.cs
│   │       │   └── Quests_Objectives.cs
│   │       ├── Scenarios/
│   │       │   ├── Scenario_Info.cs
│   │       │   └── Scenario_Object.cs
│   │       ├── TimedAnnounce.cs
│   │       ├── Waypoint.cs
│   │       └── World_Settings.cs
│   ├── Email/
│   │   └── EmailClient.cs
│   ├── FastRandom.cs
│   ├── HonorCalculation.cs
│   ├── Icons/
│   │   ├── account.ico
│   │   ├── launcher.ico
│   │   ├── lobby.ico
│   │   └── world.ico
│   ├── Properties/
│   │   └── AssemblyInfo.cs
│   ├── Rpc/
│   │   ├── AccountMgr.cs
│   │   └── CharacterMgr.cs
│   ├── app.config
│   └── packages.config
├── Database/
│   ├── Database.7z
│   ├── cooler_sai_sql_changes.7z
│   ├── war_accounts.sql
│   ├── war_characters.sql
│   └── war_world.sql
├── FUTURE_IMPROVEMENTS.md
├── FrameWork/
│   ├── Config/
│   │   ├── ConfigMgr.cs
│   │   ├── Console/
│   │   │   ├── ConsoleHandlerAttribute.cs
│   │   │   ├── ConsoleMgr.cs
│   │   │   └── IConsoleHandler.cs
│   │   └── aConfigAttributes.cs
│   ├── Console/
│   │   ├── ConsoleHandlerAttribute.cs
│   │   ├── ConsoleMgr.cs
│   │   └── IConsoleHandler.cs
│   ├── Database/
│   │   ├── Attributes/
│   │   │   ├── DataElement.cs
│   │   │   ├── DataTable.cs
│   │   │   ├── Index.cs
│   │   │   ├── PrimaryKey.cs
│   │   │   ├── ReadOnly.cs
│   │   │   └── Relation.cs
│   │   ├── Cache/
│   │   │   ├── CacheException.cs
│   │   │   ├── ICache.cs
│   │   │   └── SimpleCache.cs
│   │   ├── Connection/
│   │   │   ├── ConnectionType.cs
│   │   │   ├── DataConnection.cs
│   │   │   ├── IDataConnection.cs
│   │   │   ├── MySqlDataConnection.cs
│   │   │   └── SqlDataConnection.cs
│   │   ├── DBManager.cs
│   │   ├── DataBinder.cs
│   │   ├── DataObject.cs
│   │   ├── DataTableHandler.cs
│   │   ├── DatabaseExeption.cs
│   │   ├── Handler/
│   │   │   ├── MysqlObjectDatabase.cs
│   │   │   └── SqlObjectDatabase.cs
│   │   ├── IObjectDatabase.cs
│   │   ├── MySqlExpressionDataBinder.cs
│   │   ├── ObjectDatabase.cs
│   │   ├── Transaction/
│   │   │   └── IsolationLevel.cs
│   │   └── UniqueID/
│   │       └── IDGenerator.cs
│   ├── FrameWork.csproj
│   ├── FrameWork.sln
│   ├── Loader/
│   │   ├── LoaderMgr.cs
│   │   ├── LoadingFunctionAttribute.cs
│   │   └── ServiceAttribute.cs
│   ├── Logs/
│   │   ├── Log.cs
│   │   └── LogConfig.cs
│   ├── Misc/
│   │   ├── CrashGuard.cs
│   │   └── Insensitive.cs
│   ├── NetWork/
│   │   ├── ByteSwap.cs
│   │   ├── Clients/
│   │   │   ├── BaseClient.cs
│   │   │   ├── IPacketHandler.cs
│   │   │   ├── PacketHandlerAttribute.cs
│   │   │   ├── PacketIn.cs
│   │   │   └── PacketOut.cs
│   │   ├── Crypt/
│   │   │   ├── CryptAttribute.cs
│   │   │   └── ICryptHandler.cs
│   │   ├── EGmLevel.cs
│   │   ├── FrameBuffer.cs
│   │   ├── Marshal.cs
│   │   ├── ProtocolBuffers/
│   │   │   ├── AuthSessionTokenReplyProto.cs
│   │   │   ├── AuthSessionTokenReqProto.cs
│   │   │   ├── GetAcctPropListReplyProto.cs
│   │   │   ├── GetCharSummaryListReplyProto.cs
│   │   │   ├── GetClusterListReplyProto.cs
│   │   │   ├── VerifyProtocolReplyProto.cs
│   │   │   └── VerifyProtocolReqProto.cs
│   │   └── TCPManager.cs
│   ├── Properties/
│   │   └── AssemblyInfo.cs
│   ├── Remoting/
│   │   ├── ClientInfo.cs
│   │   ├── ClientMgr.cs
│   │   ├── RpcClient.cs
│   │   ├── RpcObject.cs
│   │   ├── RpcServer.cs
│   │   └── ServerMgr.cs
│   ├── Utils/
│   │   ├── CSV.cs
│   │   ├── CircularBuffer.cs
│   │   ├── Color.cs
│   │   ├── ObjectPool.cs
│   │   ├── RandomMgr.cs
│   │   ├── StaticRandom.cs
│   │   ├── Utils.cs
│   │   └── Vectors.cs
│   ├── app.config
│   ├── packages.config
│   └── zlib/
│       ├── Adler32.cs
│       ├── Deflate.cs
│       ├── InfBlocks.cs
│       ├── InfCodes.cs
│       ├── InfTree.cs
│       ├── Inflate.cs
│       ├── StaticTree.cs
│       ├── SupportClass.cs
│       ├── Tree.cs
│       ├── ZInputStream.cs
│       ├── ZOutputStream.cs
│       ├── ZStream.cs
│       ├── ZStreamException.cs
│       ├── Zlib.cs
│       └── ZlibMgr.cs
├── ImportIntoProject/
│   ├── Abilities/
│   │   ├── Chaos humans login-logout all classes.txt
│   │   ├── Dark elves login-logout all classes.txt
│   │   ├── Dwarves and their classes login-logout.txt
│   │   ├── High elves and their classes login-logout.txt
│   │   ├── Humans and their classes login-logout.txt
│   │   └── Orcs login-logout all classes.txt
│   ├── Configs/
│   │   └── mythloginserviceconfig.xml
│   ├── los/
│   │   ├── 1.bin
│   │   ├── 10.bin
│   │   ├── 100.bin
│   │   ├── 101.bin
│   │   ├── 102.bin
│   │   ├── 103.bin
│   │   ├── 104.bin
│   │   ├── 105.bin
│   │   ├── 106.bin
│   │   ├── 107.bin
│   │   ├── 108.bin
│   │   ├── 109.bin
│   │   ├── 11.bin
│   │   ├── 110.bin
│   │   ├── 111.bin
│   │   ├── 112.bin
│   │   ├── 113.bin
│   │   ├── 114.bin
│   │   ├── 115.bin
│   │   ├── 116.bin
│   │   ├── 117.bin
│   │   ├── 118.bin
│   │   ├── 12.bin
│   │   ├── 120.bin
│   │   ├── 121.bin
│   │   ├── 122.bin
│   │   ├── 123.bin
│   │   ├── 124.bin
│   │   ├── 125.bin
│   │   ├── 126.bin
│   │   ├── 127.bin
│   │   ├── 128.bin
│   │   ├── 129.bin
│   │   ├── 13.bin
│   │   ├── 130.bin
│   │   ├── 131.bin
│   │   ├── 132.bin
│   │   ├── 133.bin
│   │   ├── 134.bin
│   │   ├── 135.bin
│   │   ├── 136.bin
│   │   ├── 137.bin
│   │   ├── 138.bin
│   │   ├── 139.bin
│   │   ├── 14.bin
│   │   ├── 142.bin
│   │   ├── 143.bin
│   │   ├── 144.bin
│   │   ├── 145.bin
│   │   ├── 146.bin
│   │   ├── 147.bin
│   │   ├── 15.bin
│   │   ├── 150.bin
│   │   ├── 151.bin
│   │   ├── 152.bin
│   │   ├── 153.bin
│   │   ├── 154.bin
│   │   ├── 155.bin
│   │   ├── 156.bin
│   │   ├── 157.bin
│   │   ├── 158.bin
│   │   ├── 159.bin
│   │   ├── 16.bin
│   │   ├── 160.bin
│   │   ├── 161.bin
│   │   ├── 162.bin
│   │   ├── 163.bin
│   │   ├── 164.bin
│   │   ├── 165.bin
│   │   ├── 166.bin
│   │   ├── 167.bin
│   │   ├── 168.bin
│   │   ├── 169.bin
│   │   ├── 17.bin
│   │   ├── 170.bin
│   │   ├── 171.bin
│   │   ├── 172.bin
│   │   ├── 173.bin
│   │   ├── 174.bin
│   │   ├── 175.bin
│   │   ├── 176.bin
│   │   ├── 177.bin
│   │   ├── 178.bin
│   │   ├── 179.bin
│   │   ├── 18.bin
│   │   ├── 180.bin
│   │   ├── 181.bin
│   │   ├── 182.bin
│   │   ├── 183.bin
│   │   ├── 184.bin
│   │   ├── 185.bin
│   │   ├── 186.bin
│   │   ├── 187.bin
│   │   ├── 188.bin
│   │   ├── 19.bin
│   │   ├── 190.bin
│   │   ├── 191.bin
│   │   ├── 192.bin
│   │   ├── 193.bin
│   │   ├── 194.bin
│   │   ├── 195.bin
│   │   ├── 196.bin
│   │   ├── 197.bin
│   │   ├── 198.bin
│   │   ├── 199.bin
│   │   ├── 2.bin
│   │   ├── 20.bin
│   │   ├── 200.bin
│   │   ├── 201.bin
│   │   ├── 202.bin
│   │   ├── 203.bin
│   │   ├── 204.bin
│   │   ├── 205.bin
│   │   ├── 206.bin
│   │   ├── 207.bin
│   │   ├── 208.bin
│   │   ├── 209.bin
│   │   ├── 21.bin
│   │   ├── 210.bin
│   │   ├── 211.bin
│   │   ├── 212.bin
│   │   ├── 213.bin
│   │   ├── 214.bin
│   │   ├── 215.bin
│   │   ├── 216.bin
│   │   ├── 217.bin
│   │   ├── 218.bin
│   │   ├── 219.bin
│   │   ├── 22.bin
│   │   ├── 220.bin
│   │   ├── 221.bin
│   │   ├── 222.bin
│   │   ├── 223.bin
│   │   ├── 224.bin
│   │   ├── 225.bin
│   │   ├── 226.bin
│   │   ├── 227.bin
│   │   ├── 228.bin
│   │   ├── 229.bin
│   │   ├── 23.bin
│   │   ├── 230.bin
│   │   ├── 231.bin
│   │   ├── 232.bin
│   │   ├── 234.bin
│   │   ├── 235.bin
│   │   ├── 236.bin
│   │   ├── 237.bin
│   │   ├── 238.bin
│   │   ├── 24.bin
│   │   ├── 241.bin
│   │   ├── 242.bin
│   │   ├── 243.bin
│   │   ├── 244.bin
│   │   ├── 245.bin
│   │   ├── 246.bin
│   │   ├── 247.bin
│   │   ├── 248.bin
│   │   ├── 249.bin
│   │   ├── 25.bin
│   │   ├── 26.bin
│   │   ├── 260.bin
│   │   ├── 263.bin
│   │   ├── 264.bin
│   │   ├── 265.bin
│   │   ├── 266.bin
│   │   ├── 27.bin
│   │   ├── 275.bin
│   │   ├── 276.bin
│   │   ├── 277.bin
│   │   ├── 278.bin
│   │   ├── 279.bin
│   │   ├── 28.bin
│   │   ├── 280.bin
│   │   ├── 281.bin
│   │   ├── 282.bin
│   │   ├── 283.bin
│   │   ├── 284.bin
│   │   ├── 285.bin
│   │   ├── 286.bin
│   │   ├── 287.bin
│   │   ├── 288.bin
│   │   ├── 289.bin
│   │   ├── 290.bin
│   │   ├── 291.bin
│   │   ├── 292.bin
│   │   ├── 294.bin
│   │   ├── 295.bin
│   │   ├── 296.bin
│   │   ├── 297.bin
│   │   ├── 298.bin
│   │   ├── 299.bin
│   │   ├── 3.bin
│   │   ├── 30.bin
│   │   ├── 301.bin
│   │   ├── 302.bin
│   │   ├── 303.bin
│   │   ├── 304.bin
│   │   ├── 305.bin
│   │   ├── 306.bin
│   │   ├── 307.bin
│   │   ├── 31.bin
│   │   ├── 32.bin
│   │   ├── 33.bin
│   │   ├── 34.bin
│   │   ├── 36.bin
│   │   ├── 37.bin
│   │   ├── 38.bin
│   │   ├── 39.bin
│   │   ├── 4.bin
│   │   ├── 41.bin
│   │   ├── 410.bin
│   │   ├── 411.bin
│   │   ├── 412.bin
│   │   ├── 42.bin
│   │   ├── 43.bin
│   │   ├── 44.bin
│   │   ├── 45.bin
│   │   ├── 5.bin
│   │   ├── 50.bin
│   │   ├── 51.bin
│   │   ├── 52.bin
│   │   ├── 6.bin
│   │   ├── 60.bin
│   │   ├── 63.bin
│   │   ├── 64.bin
│   │   ├── 65.bin
│   │   ├── 66.bin
│   │   ├── 7.bin
│   │   ├── 70.bin
│   │   ├── 71.bin
│   │   ├── 72.bin
│   │   ├── 73.bin
│   │   ├── 74.bin
│   │   ├── 75.bin
│   │   ├── 76.bin
│   │   ├── 77.bin
│   │   ├── 78.bin
│   │   ├── 79.bin
│   │   ├── 8.bin
│   │   ├── 82.bin
│   │   ├── 83.bin
│   │   ├── 84.bin
│   │   ├── 85.bin
│   │   ├── 86.bin
│   │   ├── 87.bin
│   │   ├── 88.bin
│   │   ├── 89.bin
│   │   ├── 9.bin
│   │   └── 90.bin
│   └── zones/
│       ├── mappieces.csv
│       ├── zone001/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   └── terrain.png
│       ├── zone002/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   └── terrain.png
│       ├── zone003/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   ├── piece11.jpg
│       │   ├── piece12.jpg
│       │   ├── piece13.jpg
│       │   ├── piece14.jpg
│       │   └── terrain.png
│       ├── zone004/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone005/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   ├── piece11.jpg
│       │   ├── piece12.jpg
│       │   ├── piece13.jpg
│       │   ├── piece14.jpg
│       │   └── terrain.png
│       ├── zone006/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   └── terrain.png
│       ├── zone007/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   └── terrain.png
│       ├── zone008/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   └── terrain.png
│       ├── zone009/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   ├── piece11.jpg
│       │   └── terrain.png
│       ├── zone010/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone011/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   ├── piece11.jpg
│       │   ├── piece12.jpg
│       │   └── terrain.png
│       ├── zone012/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone013/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone014/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone015/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone016/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone017/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone018/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone019/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone020/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone021/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone022/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone023/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone024/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone025/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone026/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   └── terrain.png
│       ├── zone027/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   └── terrain.png
│       ├── zone028/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone030/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone031/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone032/
│       │   ├── offset.png
│       │   ├── shademap.png
│       │   └── terrain.png
│       ├── zone033/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone034/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone036/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone038/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone039/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone041/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone042/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone043/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone044/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone045/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone060/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone063/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone064/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone065/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone066/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone070/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone071/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone072/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone073/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone074/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone075/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone076/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone077/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone078/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone079/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone080/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone082/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone083/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone084/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone085/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone087/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone088/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone089/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone100/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   └── terrain.png
│       ├── zone101/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   ├── piece11.jpg
│       │   ├── piece12.jpg
│       │   ├── piece13.jpg
│       │   ├── piece14.jpg
│       │   ├── piece15.jpg
│       │   ├── piece16.jpg
│       │   ├── piece17.jpg
│       │   └── terrain.png
│       ├── zone102/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   ├── piece11.jpg
│       │   ├── piece12.jpg
│       │   └── terrain.png
│       ├── zone103/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   └── terrain.png
│       ├── zone104/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone105/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   └── terrain.png
│       ├── zone106/
│       │   ├── areas106.png
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── pqarea106.png
│       │   └── terrain.png
│       ├── zone107/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   ├── piece11.jpg
│       │   ├── piece12.jpg
│       │   ├── piece13.jpg
│       │   ├── piece14.jpg
│       │   ├── piece15.jpg
│       │   ├── piece16.jpg
│       │   └── terrain.png
│       ├── zone108/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   ├── piece11.jpg
│       │   ├── piece12.jpg
│       │   └── terrain.png
│       ├── zone109/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   └── terrain.png
│       ├── zone110/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone111/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone112/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone113/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone115/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone116/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone117/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone118/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone120/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone121/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone123/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone125/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone126/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone127/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone128/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone129/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone130/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone131/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone132/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone133/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone134/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone135/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone136/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone137/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone138/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone139/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone140/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone142/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone143/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone144/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone147/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone152/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone153/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone154/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone155/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone156/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone160/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone161/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone162/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone163/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone164/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone165/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone166/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone167/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone168/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone169/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone170/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone171/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone172/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone173/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone174/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone175/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone176/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone177/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone178/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone179/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone180/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone181/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone182/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone184/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone189/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone190/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone191/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   └── terrain.png
│       ├── zone192/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone193/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone194/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone195/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone196/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone198/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone199/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone200/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   ├── piece11.jpg
│       │   ├── piece12.jpg
│       │   ├── piece13.jpg
│       │   ├── piece14.jpg
│       │   ├── piece15.jpg
│       │   ├── piece16.jpg
│       │   └── terrain.png
│       ├── zone201/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   └── terrain.png
│       ├── zone202/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   └── terrain.png
│       ├── zone203/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   └── terrain.png
│       ├── zone204/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone205/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   └── terrain.png
│       ├── zone206/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   └── terrain.png
│       ├── zone207/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   └── terrain.png
│       ├── zone208/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   ├── piece07.jpg
│       │   ├── piece08.jpg
│       │   ├── piece09.jpg
│       │   ├── piece10.jpg
│       │   ├── piece11.jpg
│       │   ├── piece12.jpg
│       │   └── terrain.png
│       ├── zone209/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   ├── piece04.jpg
│       │   ├── piece05.jpg
│       │   ├── piece06.jpg
│       │   └── terrain.png
│       ├── zone210/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone211/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone212/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone213/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone214/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone215/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone216/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone217/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone218/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone219/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone220/
│       │   ├── influenceids.csv
│       │   ├── mappieces.csv
│       │   ├── offset.png
│       │   ├── piece01.jpg
│       │   ├── piece02.jpg
│       │   ├── piece03.jpg
│       │   └── terrain.png
│       ├── zone221/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone222/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone223/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone224/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone225/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone226/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone227/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone228/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone229/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone230/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone231/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone232/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone234/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone235/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone236/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone237/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone238/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone241/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone242/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone243/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone244/
│       │   ├── influenceids.csv
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone245/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone246/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone247/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone248/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone249/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone260/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone275/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone276/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone277/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone278/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone279/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone280/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone281/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone282/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone283/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone284/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone285/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone286/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone287/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone288/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone289/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone290/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone291/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone292/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone294/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone295/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone297/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone298/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone303/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone304/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone306/
│       │   ├── offset.png
│       │   └── terrain.png
│       ├── zone307/
│       │   ├── offset.png
│       │   └── terrain.png
│       └── zoneinfo.txt
├── Interface/
│   └── AddOns/
├── Launcher/
│   ├── ApocLauncher.Designer.cs
│   ├── ApocLauncher.cs
│   ├── ApocLauncher.resx
│   ├── Artwork/
│   │   ├── WAR.ico
│   │   ├── WAR.png
│   │   ├── background.png
│   │   └── logo-2_yellow.png
│   ├── HttpUtil.cs
│   ├── Launcher.csproj
│   ├── NLog.config
│   ├── NLog.xsd
│   ├── NetWork/
│   │   ├── CRC32.cs
│   │   ├── Client.cs
│   │   ├── MYP.cs
│   │   ├── Marshal.cs
│   │   ├── Opcodes.cs
│   │   ├── PacketIn.cs
│   │   └── PacketOut.cs
│   ├── Patcher.cs
│   ├── Program.cs
│   ├── Properties/
│   │   ├── AssemblyInfo.cs
│   │   ├── Resources.Designer.cs
│   │   └── Resources.resx
│   ├── Resources/
│   │   └── M55rLv3.jpg
│   ├── WAR.ico
│   ├── app.config
│   └── packages.config
├── LauncherServer/
│   ├── Addon/
│   │   └── WARAddon.cs
│   ├── Config/
│   │   └── LauncherConfig.cs
│   ├── Console/
│   │   └── State.cs
│   ├── Core.cs
│   ├── LauncherServer.csproj
│   ├── PatchMgr.cs
│   ├── Properties/
│   │   └── AssemblyInfo.cs
│   ├── Server/
│   │   ├── Client.cs
│   │   ├── Handler/
│   │   │   └── Packets.cs
│   │   ├── Opcodes.cs
│   │   ├── PatcherFile.cs
│   │   └── TCPServer.cs
│   ├── app.config
│   └── launcher.ico
├── LobbyServer/
│   ├── Configs/
│   │   └── LobbyConfigs.cs
│   ├── Core.cs
│   ├── LobbyServer.csproj
│   ├── NetWork/
│   │   ├── Client.cs
│   │   ├── Handler/
│   │   │   └── AuthentificationHandlers.cs
│   │   ├── Opcodes.cs
│   │   └── TCPServer.cs
│   ├── Properties/
│   │   └── AssemblyInfo.cs
│   ├── app.config
│   └── lobby.ico
├── NuGet.config
├── OPTIMIZATIONS.md
├── ProjectWAR.sln
├── README.md
├── ServerLauncher/
│   ├── Form1.Designer.cs
│   ├── Form1.cs
│   ├── Form1.resx
│   ├── Program.cs
│   ├── Properties/
│   │   ├── AssemblyInfo.cs
│   │   ├── Resources.Designer.cs
│   │   ├── Resources.resx
│   │   ├── Settings.Designer.cs
│   │   └── Settings.settings
│   ├── ServerLauncher.csproj
│   ├── app.config
│   └── server.ico
├── WarZone/
│   ├── CMakeLists.txt
│   ├── Intersections.cpp
│   ├── Intersections.h
│   ├── KDTreeCPU.cpp
│   ├── KDTreeCPU.h
│   ├── KDTreeNode.cpp
│   ├── KDTreeNode.h
│   ├── LinuxPort.h
│   ├── Platform.h
│   ├── Source.cpp
│   ├── Stopwatch.h
│   ├── Util.cpp
│   ├── Util.h
│   ├── WarZone.sln
│   ├── WarZone.vcxproj
│   ├── WarZone.vcxproj.filters
│   ├── Zone.h
│   ├── ZoneManager.cpp
│   ├── ZoneManager.h
│   ├── boundingbox.h
│   ├── glm/
│   │   ├── CMakeLists.txt
│   │   ├── common.hpp
│   │   ├── core/
│   │   │   ├── _detail.hpp
│   │   │   ├── _fixes.hpp
│   │   │   ├── _swizzle.hpp
│   │   │   ├── _swizzle_func.hpp
│   │   │   ├── _vectorize.hpp
│   │   │   ├── dummy.cpp
│   │   │   ├── func_common.hpp
│   │   │   ├── func_common.inl
│   │   │   ├── func_exponential.hpp
│   │   │   ├── func_exponential.inl
│   │   │   ├── func_geometric.hpp
│   │   │   ├── func_geometric.inl
│   │   │   ├── func_integer.hpp
│   │   │   ├── func_integer.inl
│   │   │   ├── func_matrix.hpp
│   │   │   ├── func_matrix.inl
│   │   │   ├── func_noise.hpp
│   │   │   ├── func_noise.inl
│   │   │   ├── func_packing.hpp
│   │   │   ├── func_packing.inl
│   │   │   ├── func_trigonometric.hpp
│   │   │   ├── func_trigonometric.inl
│   │   │   ├── func_vector_relational.hpp
│   │   │   ├── func_vector_relational.inl
│   │   │   ├── hint.hpp
│   │   │   ├── intrinsic_common.hpp
│   │   │   ├── intrinsic_common.inl
│   │   │   ├── intrinsic_exponential.hpp
│   │   │   ├── intrinsic_exponential.inl
│   │   │   ├── intrinsic_geometric.hpp
│   │   │   ├── intrinsic_geometric.inl
│   │   │   ├── intrinsic_matrix.hpp
│   │   │   ├── intrinsic_matrix.inl
│   │   │   ├── intrinsic_trigonometric.hpp
│   │   │   ├── intrinsic_trigonometric.inl
│   │   │   ├── intrinsic_vector_relational.hpp
│   │   │   ├── intrinsic_vector_relational.inl
│   │   │   ├── setup.hpp
│   │   │   ├── type.hpp
│   │   │   ├── type_float.hpp
│   │   │   ├── type_gentype.hpp
│   │   │   ├── type_gentype.inl
│   │   │   ├── type_half.hpp
│   │   │   ├── type_half.inl
│   │   │   ├── type_int.hpp
│   │   │   ├── type_mat.hpp
│   │   │   ├── type_mat.inl
│   │   │   ├── type_mat2x2.hpp
│   │   │   ├── type_mat2x2.inl
│   │   │   ├── type_mat2x3.hpp
│   │   │   ├── type_mat2x3.inl
│   │   │   ├── type_mat2x4.hpp
│   │   │   ├── type_mat2x4.inl
│   │   │   ├── type_mat3x2.hpp
│   │   │   ├── type_mat3x2.inl
│   │   │   ├── type_mat3x3.hpp
│   │   │   ├── type_mat3x3.inl
│   │   │   ├── type_mat3x4.hpp
│   │   │   ├── type_mat3x4.inl
│   │   │   ├── type_mat4x2.hpp
│   │   │   ├── type_mat4x2.inl
│   │   │   ├── type_mat4x3.hpp
│   │   │   ├── type_mat4x3.inl
│   │   │   ├── type_mat4x4.hpp
│   │   │   ├── type_mat4x4.inl
│   │   │   ├── type_size.hpp
│   │   │   ├── type_vec.hpp
│   │   │   ├── type_vec.inl
│   │   │   ├── type_vec1.hpp
│   │   │   ├── type_vec1.inl
│   │   │   ├── type_vec2.hpp
│   │   │   ├── type_vec2.inl
│   │   │   ├── type_vec3.hpp
│   │   │   ├── type_vec3.inl
│   │   │   ├── type_vec4.hpp
│   │   │   └── type_vec4.inl
│   │   ├── detail/
│   │   │   ├── _features.hpp
│   │   │   ├── _fixes.hpp
│   │   │   ├── _noise.hpp
│   │   │   ├── _swizzle.hpp
│   │   │   ├── _swizzle_func.hpp
│   │   │   ├── _vectorize.hpp
│   │   │   ├── dummy.cpp
│   │   │   ├── func_common.hpp
│   │   │   ├── func_common.inl
│   │   │   ├── func_exponential.hpp
│   │   │   ├── func_exponential.inl
│   │   │   ├── func_geometric.hpp
│   │   │   ├── func_geometric.inl
│   │   │   ├── func_integer.hpp
│   │   │   ├── func_integer.inl
│   │   │   ├── func_matrix.hpp
│   │   │   ├── func_matrix.inl
│   │   │   ├── func_noise.hpp
│   │   │   ├── func_noise.inl
│   │   │   ├── func_packing.hpp
│   │   │   ├── func_packing.inl
│   │   │   ├── func_trigonometric.hpp
│   │   │   ├── func_trigonometric.inl
│   │   │   ├── func_vector_relational.hpp
│   │   │   ├── func_vector_relational.inl
│   │   │   ├── glm.cpp
│   │   │   ├── intrinsic_common.hpp
│   │   │   ├── intrinsic_common.inl
│   │   │   ├── intrinsic_exponential.hpp
│   │   │   ├── intrinsic_exponential.inl
│   │   │   ├── intrinsic_geometric.hpp
│   │   │   ├── intrinsic_geometric.inl
│   │   │   ├── intrinsic_integer.hpp
│   │   │   ├── intrinsic_integer.inl
│   │   │   ├── intrinsic_matrix.hpp
│   │   │   ├── intrinsic_matrix.inl
│   │   │   ├── intrinsic_trigonometric.hpp
│   │   │   ├── intrinsic_trigonometric.inl
│   │   │   ├── intrinsic_vector_relational.hpp
│   │   │   ├── intrinsic_vector_relational.inl
│   │   │   ├── precision.hpp
│   │   │   ├── setup.hpp
│   │   │   ├── type_float.hpp
│   │   │   ├── type_gentype.hpp
│   │   │   ├── type_gentype.inl
│   │   │   ├── type_half.hpp
│   │   │   ├── type_half.inl
│   │   │   ├── type_int.hpp
│   │   │   ├── type_mat.hpp
│   │   │   ├── type_mat.inl
│   │   │   ├── type_mat2x2.hpp
│   │   │   ├── type_mat2x2.inl
│   │   │   ├── type_mat2x3.hpp
│   │   │   ├── type_mat2x3.inl
│   │   │   ├── type_mat2x4.hpp
│   │   │   ├── type_mat2x4.inl
│   │   │   ├── type_mat3x2.hpp
│   │   │   ├── type_mat3x2.inl
│   │   │   ├── type_mat3x3.hpp
│   │   │   ├── type_mat3x3.inl
│   │   │   ├── type_mat3x4.hpp
│   │   │   ├── type_mat3x4.inl
│   │   │   ├── type_mat4x2.hpp
│   │   │   ├── type_mat4x2.inl
│   │   │   ├── type_mat4x3.hpp
│   │   │   ├── type_mat4x3.inl
│   │   │   ├── type_mat4x4.hpp
│   │   │   ├── type_mat4x4.inl
│   │   │   ├── type_vec.hpp
│   │   │   ├── type_vec.inl
│   │   │   ├── type_vec1.hpp
│   │   │   ├── type_vec1.inl
│   │   │   ├── type_vec2.hpp
│   │   │   ├── type_vec2.inl
│   │   │   ├── type_vec3.hpp
│   │   │   ├── type_vec3.inl
│   │   │   ├── type_vec4.hpp
│   │   │   ├── type_vec4.inl
│   │   │   ├── type_vec4_avx.inl
│   │   │   ├── type_vec4_avx2.inl
│   │   │   └── type_vec4_sse2.inl
│   │   ├── exponential.hpp
│   │   ├── ext.hpp
│   │   ├── fwd.hpp
│   │   ├── geometric.hpp
│   │   ├── glm/
│   │   │   ├── CMakeLists.txt
│   │   │   ├── common.hpp
│   │   │   ├── detail/
│   │   │   │   ├── _features.hpp
│   │   │   │   ├── _fixes.hpp
│   │   │   │   ├── _literals.hpp
│   │   │   │   ├── _noise.hpp
│   │   │   │   ├── _swizzle.hpp
│   │   │   │   ├── _swizzle_func.hpp
│   │   │   │   ├── _vectorize.hpp
│   │   │   │   ├── dummy.cpp
│   │   │   │   ├── func_common.hpp
│   │   │   │   ├── func_common.inl
│   │   │   │   ├── func_exponential.hpp
│   │   │   │   ├── func_exponential.inl
│   │   │   │   ├── func_geometric.hpp
│   │   │   │   ├── func_geometric.inl
│   │   │   │   ├── func_integer.hpp
│   │   │   │   ├── func_integer.inl
│   │   │   │   ├── func_matrix.hpp
│   │   │   │   ├── func_matrix.inl
│   │   │   │   ├── func_noise.hpp
│   │   │   │   ├── func_noise.inl
│   │   │   │   ├── func_packing.hpp
│   │   │   │   ├── func_packing.inl
│   │   │   │   ├── func_trigonometric.hpp
│   │   │   │   ├── func_trigonometric.inl
│   │   │   │   ├── func_vector_relational.hpp
│   │   │   │   ├── func_vector_relational.inl
│   │   │   │   ├── glm.cpp
│   │   │   │   ├── hint.hpp
│   │   │   │   ├── intrinsic_common.hpp
│   │   │   │   ├── intrinsic_common.inl
│   │   │   │   ├── intrinsic_exponential.hpp
│   │   │   │   ├── intrinsic_exponential.inl
│   │   │   │   ├── intrinsic_geometric.hpp
│   │   │   │   ├── intrinsic_geometric.inl
│   │   │   │   ├── intrinsic_integer.hpp
│   │   │   │   ├── intrinsic_integer.inl
│   │   │   │   ├── intrinsic_matrix.hpp
│   │   │   │   ├── intrinsic_matrix.inl
│   │   │   │   ├── intrinsic_trigonometric.hpp
│   │   │   │   ├── intrinsic_trigonometric.inl
│   │   │   │   ├── intrinsic_vector_relational.hpp
│   │   │   │   ├── intrinsic_vector_relational.inl
│   │   │   │   ├── precision.hpp
│   │   │   │   ├── precision.inl
│   │   │   │   ├── setup.hpp
│   │   │   │   ├── type_float.hpp
│   │   │   │   ├── type_gentype.hpp
│   │   │   │   ├── type_gentype.inl
│   │   │   │   ├── type_half.hpp
│   │   │   │   ├── type_half.inl
│   │   │   │   ├── type_int.hpp
│   │   │   │   ├── type_mat.hpp
│   │   │   │   ├── type_mat.inl
│   │   │   │   ├── type_mat2x2.hpp
│   │   │   │   ├── type_mat2x2.inl
│   │   │   │   ├── type_mat2x3.hpp
│   │   │   │   ├── type_mat2x3.inl
│   │   │   │   ├── type_mat2x4.hpp
│   │   │   │   ├── type_mat2x4.inl
│   │   │   │   ├── type_mat3x2.hpp
│   │   │   │   ├── type_mat3x2.inl
│   │   │   │   ├── type_mat3x3.hpp
│   │   │   │   ├── type_mat3x3.inl
│   │   │   │   ├── type_mat3x4.hpp
│   │   │   │   ├── type_mat3x4.inl
│   │   │   │   ├── type_mat4x2.hpp
│   │   │   │   ├── type_mat4x2.inl
│   │   │   │   ├── type_mat4x3.hpp
│   │   │   │   ├── type_mat4x3.inl
│   │   │   │   ├── type_mat4x4.hpp
│   │   │   │   ├── type_mat4x4.inl
│   │   │   │   ├── type_vec.hpp
│   │   │   │   ├── type_vec.inl
│   │   │   │   ├── type_vec1.hpp
│   │   │   │   ├── type_vec1.inl
│   │   │   │   ├── type_vec2.hpp
│   │   │   │   ├── type_vec2.inl
│   │   │   │   ├── type_vec3.hpp
│   │   │   │   ├── type_vec3.inl
│   │   │   │   ├── type_vec4.hpp
│   │   │   │   └── type_vec4.inl
│   │   │   ├── exponential.hpp
│   │   │   ├── ext.hpp
│   │   │   ├── fwd.hpp
│   │   │   ├── geometric.hpp
│   │   │   ├── glm.hpp
│   │   │   ├── gtc/
│   │   │   │   ├── constants.hpp
│   │   │   │   ├── constants.inl
│   │   │   │   ├── epsilon.hpp
│   │   │   │   ├── epsilon.inl
│   │   │   │   ├── matrix_access.hpp
│   │   │   │   ├── matrix_access.inl
│   │   │   │   ├── matrix_integer.hpp
│   │   │   │   ├── matrix_inverse.hpp
│   │   │   │   ├── matrix_inverse.inl
│   │   │   │   ├── matrix_transform.hpp
│   │   │   │   ├── matrix_transform.inl
│   │   │   │   ├── noise.hpp
│   │   │   │   ├── noise.inl
│   │   │   │   ├── packing.hpp
│   │   │   │   ├── packing.inl
│   │   │   │   ├── quaternion.hpp
│   │   │   │   ├── quaternion.inl
│   │   │   │   ├── random.hpp
│   │   │   │   ├── random.inl
│   │   │   │   ├── reciprocal.hpp
│   │   │   │   ├── reciprocal.inl
│   │   │   │   ├── type_precision.hpp
│   │   │   │   ├── type_precision.inl
│   │   │   │   ├── type_ptr.hpp
│   │   │   │   ├── type_ptr.inl
│   │   │   │   ├── ulp.hpp
│   │   │   │   └── ulp.inl
│   │   │   ├── gtx/
│   │   │   │   ├── associated_min_max.hpp
│   │   │   │   ├── associated_min_max.inl
│   │   │   │   ├── bit.hpp
│   │   │   │   ├── bit.inl
│   │   │   │   ├── closest_point.hpp
│   │   │   │   ├── closest_point.inl
│   │   │   │   ├── color_space.hpp
│   │   │   │   ├── color_space.inl
│   │   │   │   ├── color_space_YCoCg.hpp
│   │   │   │   ├── color_space_YCoCg.inl
│   │   │   │   ├── compatibility.hpp
│   │   │   │   ├── compatibility.inl
│   │   │   │   ├── component_wise.hpp
│   │   │   │   ├── component_wise.inl
│   │   │   │   ├── constants.hpp
│   │   │   │   ├── dual_quaternion.hpp
│   │   │   │   ├── dual_quaternion.inl
│   │   │   │   ├── epsilon.hpp
│   │   │   │   ├── euler_angles.hpp
│   │   │   │   ├── euler_angles.inl
│   │   │   │   ├── extend.hpp
│   │   │   │   ├── extend.inl
│   │   │   │   ├── extented_min_max.hpp
│   │   │   │   ├── extented_min_max.inl
│   │   │   │   ├── fast_exponential.hpp
│   │   │   │   ├── fast_exponential.inl
│   │   │   │   ├── fast_square_root.hpp
│   │   │   │   ├── fast_square_root.inl
│   │   │   │   ├── fast_trigonometry.hpp
│   │   │   │   ├── fast_trigonometry.inl
│   │   │   │   ├── gradient_paint.hpp
│   │   │   │   ├── gradient_paint.inl
│   │   │   │   ├── handed_coordinate_space.hpp
│   │   │   │   ├── handed_coordinate_space.inl
│   │   │   │   ├── inertia.hpp
│   │   │   │   ├── inertia.inl
│   │   │   │   ├── int_10_10_10_2.hpp
│   │   │   │   ├── int_10_10_10_2.inl
│   │   │   │   ├── integer.hpp
│   │   │   │   ├── integer.inl
│   │   │   │   ├── intersect.hpp
│   │   │   │   ├── intersect.inl
│   │   │   │   ├── io.hpp
│   │   │   │   ├── io.inl
│   │   │   │   ├── log_base.hpp
│   │   │   │   ├── log_base.inl
│   │   │   │   ├── matrix_cross_product.hpp
│   │   │   │   ├── matrix_cross_product.inl
│   │   │   │   ├── matrix_interpolation.hpp
│   │   │   │   ├── matrix_interpolation.inl
│   │   │   │   ├── matrix_major_storage.hpp
│   │   │   │   ├── matrix_major_storage.inl
│   │   │   │   ├── matrix_operation.hpp
│   │   │   │   ├── matrix_operation.inl
│   │   │   │   ├── matrix_query.hpp
│   │   │   │   ├── matrix_query.inl
│   │   │   │   ├── mixed_product.hpp
│   │   │   │   ├── mixed_product.inl
│   │   │   │   ├── multiple.hpp
│   │   │   │   ├── multiple.inl
│   │   │   │   ├── noise.hpp
│   │   │   │   ├── norm.hpp
│   │   │   │   ├── norm.inl
│   │   │   │   ├── normal.hpp
│   │   │   │   ├── normal.inl
│   │   │   │   ├── normalize_dot.hpp
│   │   │   │   ├── normalize_dot.inl
│   │   │   │   ├── number_precision.hpp
│   │   │   │   ├── number_precision.inl
│   │   │   │   ├── optimum_pow.hpp
│   │   │   │   ├── optimum_pow.inl
│   │   │   │   ├── orthonormalize.hpp
│   │   │   │   ├── orthonormalize.inl
│   │   │   │   ├── perpendicular.hpp
│   │   │   │   ├── perpendicular.inl
│   │   │   │   ├── polar_coordinates.hpp
│   │   │   │   ├── polar_coordinates.inl
│   │   │   │   ├── projection.hpp
│   │   │   │   ├── projection.inl
│   │   │   │   ├── quaternion.hpp
│   │   │   │   ├── quaternion.inl
│   │   │   │   ├── random.hpp
│   │   │   │   ├── raw_data.hpp
│   │   │   │   ├── raw_data.inl
│   │   │   │   ├── reciprocal.hpp
│   │   │   │   ├── rotate_normalized_axis.hpp
│   │   │   │   ├── rotate_normalized_axis.inl
│   │   │   │   ├── rotate_vector.hpp
│   │   │   │   ├── rotate_vector.inl
│   │   │   │   ├── scalar_relational.hpp
│   │   │   │   ├── scalar_relational.inl
│   │   │   │   ├── simd_mat4.hpp
│   │   │   │   ├── simd_mat4.inl
│   │   │   │   ├── simd_quat.hpp
│   │   │   │   ├── simd_quat.inl
│   │   │   │   ├── simd_vec4.hpp
│   │   │   │   ├── simd_vec4.inl
│   │   │   │   ├── spline.hpp
│   │   │   │   ├── spline.inl
│   │   │   │   ├── std_based_type.hpp
│   │   │   │   ├── std_based_type.inl
│   │   │   │   ├── string_cast.hpp
│   │   │   │   ├── string_cast.inl
│   │   │   │   ├── transform.hpp
│   │   │   │   ├── transform.inl
│   │   │   │   ├── transform2.hpp
│   │   │   │   ├── transform2.inl
│   │   │   │   ├── ulp.hpp
│   │   │   │   ├── unsigned_int.hpp
│   │   │   │   ├── unsigned_int.inl
│   │   │   │   ├── vec1.hpp
│   │   │   │   ├── vec1.inl
│   │   │   │   ├── vector_angle.hpp
│   │   │   │   ├── vector_angle.inl
│   │   │   │   ├── vector_query.hpp
│   │   │   │   ├── vector_query.inl
│   │   │   │   ├── wrap.hpp
│   │   │   │   └── wrap.inl
│   │   │   ├── integer.hpp
│   │   │   ├── mat2x2.hpp
│   │   │   ├── mat2x3.hpp
│   │   │   ├── mat2x4.hpp
│   │   │   ├── mat3x2.hpp
│   │   │   ├── mat3x3.hpp
│   │   │   ├── mat3x4.hpp
│   │   │   ├── mat4x2.hpp
│   │   │   ├── mat4x3.hpp
│   │   │   ├── mat4x4.hpp
│   │   │   ├── matrix.hpp
│   │   │   ├── packing.hpp
│   │   │   ├── trigonometric.hpp
│   │   │   ├── vec2.hpp
│   │   │   ├── vec3.hpp
│   │   │   ├── vec4.hpp
│   │   │   ├── vector_relational.hpp
│   │   │   └── virtrev/
│   │   │       └── xstream.hpp
│   │   ├── glm.hpp
│   │   ├── gtc/
│   │   │   ├── bitfield.hpp
│   │   │   ├── bitfield.inl
│   │   │   ├── constants.hpp
│   │   │   ├── constants.inl
│   │   │   ├── epsilon.hpp
│   │   │   ├── epsilon.inl
│   │   │   ├── half_float.hpp
│   │   │   ├── half_float.inl
│   │   │   ├── integer.hpp
│   │   │   ├── integer.inl
│   │   │   ├── matrix_access.hpp
│   │   │   ├── matrix_access.inl
│   │   │   ├── matrix_integer.hpp
│   │   │   ├── matrix_inverse.hpp
│   │   │   ├── matrix_inverse.inl
│   │   │   ├── matrix_transform.hpp
│   │   │   ├── matrix_transform.inl
│   │   │   ├── noise.hpp
│   │   │   ├── noise.inl
│   │   │   ├── packing.hpp
│   │   │   ├── packing.inl
│   │   │   ├── quaternion.hpp
│   │   │   ├── quaternion.inl
│   │   │   ├── random.hpp
│   │   │   ├── random.inl
│   │   │   ├── reciprocal.hpp
│   │   │   ├── reciprocal.inl
│   │   │   ├── round.hpp
│   │   │   ├── round.inl
│   │   │   ├── swizzle.hpp
│   │   │   ├── swizzle.inl
│   │   │   ├── type_precision.hpp
│   │   │   ├── type_precision.inl
│   │   │   ├── type_ptr.hpp
│   │   │   ├── type_ptr.inl
│   │   │   ├── ulp.hpp
│   │   │   ├── ulp.inl
│   │   │   ├── vec1.hpp
│   │   │   └── vec1.inl
│   │   ├── gtx/
│   │   │   ├── associated_min_max.hpp
│   │   │   ├── associated_min_max.inl
│   │   │   ├── bit.hpp
│   │   │   ├── bit.inl
│   │   │   ├── closest_point.hpp
│   │   │   ├── closest_point.inl
│   │   │   ├── color_cast.hpp
│   │   │   ├── color_cast.inl
│   │   │   ├── color_space.hpp
│   │   │   ├── color_space.inl
│   │   │   ├── color_space_YCoCg.hpp
│   │   │   ├── color_space_YCoCg.inl
│   │   │   ├── common.hpp
│   │   │   ├── common.inl
│   │   │   ├── compatibility.hpp
│   │   │   ├── compatibility.inl
│   │   │   ├── component_wise.hpp
│   │   │   ├── component_wise.inl
│   │   │   ├── constants.hpp
│   │   │   ├── constants.inl
│   │   │   ├── dual_quaternion.hpp
│   │   │   ├── dual_quaternion.inl
│   │   │   ├── epsilon.hpp
│   │   │   ├── epsilon.inl
│   │   │   ├── euler_angles.hpp
│   │   │   ├── euler_angles.inl
│   │   │   ├── extend.hpp
│   │   │   ├── extend.inl
│   │   │   ├── extented_min_max (1).inl
│   │   │   ├── extented_min_max.hpp
│   │   │   ├── extented_min_max.inl
│   │   │   ├── fast_exponential.hpp
│   │   │   ├── fast_exponential.inl
│   │   │   ├── fast_square_root.hpp
│   │   │   ├── fast_square_root.inl
│   │   │   ├── fast_trigonometry.hpp
│   │   │   ├── fast_trigonometry.inl
│   │   │   ├── gradient_paint.hpp
│   │   │   ├── gradient_paint.inl
│   │   │   ├── handed_coordinate_space.hpp
│   │   │   ├── handed_coordinate_space.inl
│   │   │   ├── inertia.hpp
│   │   │   ├── inertia.inl
│   │   │   ├── int_10_10_10_2.hpp
│   │   │   ├── int_10_10_10_2.inl
│   │   │   ├── integer (1).inl
│   │   │   ├── integer.hpp
│   │   │   ├── integer.inl
│   │   │   ├── intersect.hpp
│   │   │   ├── intersect.inl
│   │   │   ├── io.hpp
│   │   │   ├── io.inl
│   │   │   ├── log_base.hpp
│   │   │   ├── log_base.inl
│   │   │   ├── matrix_cross_product.hpp
│   │   │   ├── matrix_cross_product.inl
│   │   │   ├── matrix_decompose.hpp
│   │   │   ├── matrix_decompose.inl
│   │   │   ├── matrix_interpolation.hpp
│   │   │   ├── matrix_interpolation.inl
│   │   │   ├── matrix_major_storage.hpp
│   │   │   ├── matrix_major_storage.inl
│   │   │   ├── matrix_operation.hpp
│   │   │   ├── matrix_operation.inl
│   │   │   ├── matrix_query.hpp
│   │   │   ├── matrix_query.inl
│   │   │   ├── matrix_transform_2d.hpp
│   │   │   ├── matrix_transform_2d.inl
│   │   │   ├── mixed_product.hpp
│   │   │   ├── mixed_product.inl
│   │   │   ├── multiple.hpp
│   │   │   ├── multiple.inl
│   │   │   ├── noise.hpp
│   │   │   ├── noise.inl
│   │   │   ├── norm.hpp
│   │   │   ├── norm.inl
│   │   │   ├── normal.hpp
│   │   │   ├── normal.inl
│   │   │   ├── normalize_dot.hpp
│   │   │   ├── normalize_dot.inl
│   │   │   ├── number_precision.hpp
│   │   │   ├── number_precision.inl
│   │   │   ├── ocl_type.hpp
│   │   │   ├── ocl_type.inl
│   │   │   ├── optimum_pow.hpp
│   │   │   ├── optimum_pow.inl
│   │   │   ├── orthonormalize.hpp
│   │   │   ├── orthonormalize.inl
│   │   │   ├── perpendicular.hpp
│   │   │   ├── perpendicular.inl
│   │   │   ├── polar_coordinates.hpp
│   │   │   ├── polar_coordinates.inl
│   │   │   ├── projection.hpp
│   │   │   ├── projection.inl
│   │   │   ├── quaternion.hpp
│   │   │   ├── quaternion.inl
│   │   │   ├── random.hpp
│   │   │   ├── random.inl
│   │   │   ├── range.hpp
│   │   │   ├── raw_data (1).hpp
│   │   │   ├── raw_data.hpp
│   │   │   ├── raw_data.inl
│   │   │   ├── reciprocal.hpp
│   │   │   ├── reciprocal.inl
│   │   │   ├── rotate_normalized_axis.hpp
│   │   │   ├── rotate_normalized_axis.inl
│   │   │   ├── rotate_vector.hpp
│   │   │   ├── rotate_vector.inl
│   │   │   ├── scalar_multiplication.hpp
│   │   │   ├── scalar_relational.hpp
│   │   │   ├── scalar_relational.inl
│   │   │   ├── simd_mat4.hpp
│   │   │   ├── simd_mat4.inl
│   │   │   ├── simd_quat.hpp
│   │   │   ├── simd_quat.inl
│   │   │   ├── simd_vec4.hpp
│   │   │   ├── simd_vec4.inl
│   │   │   ├── simplex (1).hpp
│   │   │   ├── simplex.hpp
│   │   │   ├── simplex.inl
│   │   │   ├── spline.hpp
│   │   │   ├── spline.inl
│   │   │   ├── std_based_type.hpp
│   │   │   ├── std_based_type.inl
│   │   │   ├── string_cast.hpp
│   │   │   ├── string_cast.inl
│   │   │   ├── transform.hpp
│   │   │   ├── transform.inl
│   │   │   ├── transform2.hpp
│   │   │   ├── transform2.inl
│   │   │   ├── type_aligned.hpp
│   │   │   ├── type_aligned.inl
│   │   │   ├── ulp.hpp
│   │   │   ├── ulp.inl
│   │   │   ├── unsigned_int.hpp
│   │   │   ├── unsigned_int.inl
│   │   │   ├── vec1.hpp
│   │   │   ├── vec1.inl
│   │   │   ├── vector_access.hpp
│   │   │   ├── vector_access.inl
│   │   │   ├── vector_angle.hpp
│   │   │   ├── vector_angle.inl
│   │   │   ├── vector_query.hpp
│   │   │   ├── vector_query.inl
│   │   │   ├── verbose_operator.hpp
│   │   │   ├── verbose_operator.inl
│   │   │   ├── wrap.hpp
│   │   │   └── wrap.inl
│   │   ├── integer.hpp
│   │   ├── mat2x2.hpp
│   │   ├── mat2x3.hpp
│   │   ├── mat2x4.hpp
│   │   ├── mat3x2.hpp
│   │   ├── mat3x3.hpp
│   │   ├── mat3x4.hpp
│   │   ├── mat4x2.hpp
│   │   ├── mat4x3.hpp
│   │   ├── mat4x4.hpp
│   │   ├── matrix.hpp
│   │   ├── packing.hpp
│   │   ├── trigonometric.hpp
│   │   ├── vec2.hpp
│   │   ├── vec3.hpp
│   │   ├── vec4.hpp
│   │   ├── vector_relational.hpp
│   │   └── virtrev/
│   │       └── xstream.hpp
│   └── x64/
│       ├── Debug/
│       │   ├── WarZone.Build.CppClean.log
│       │   ├── WarZone.dll.recipe
│       │   ├── WarZone.log
│       │   └── WarZone.vcxproj.FileListAbsolute.txt
│       └── Release/
│           ├── Intersections.obj
│           ├── KDTreeCPU.obj
│           ├── KDTreeNode.obj
│           ├── Source.obj
│           ├── WarZone.log
│           ├── WarZone.tlog/
│           │   ├── CL.11688.write.1.tlog
│           │   ├── CL.command.1.tlog
│           │   ├── CL.read.1.tlog
│           │   ├── WarZone.lastbuildstate
│           │   ├── link.command.1.tlog
│           │   ├── link.read.1.tlog
│           │   ├── link.write.1.tlog
│           │   └── link.write.2u.tlog
│           ├── WarZone.vcxproj.FileListAbsolute.txt
│           ├── WarZone64.Build.CppClean.log
│           ├── WarZone64.dll.recipe
│           ├── WarZone64.iobj
│           └── ZoneManager.obj
├── WorldServer/
│   ├── API/
│   │   ├── ApiPacket.cs
│   │   ├── CircularBuffer.cs
│   │   ├── Client.cs
│   │   ├── Opcodes.cs
│   │   ├── Packet.cs
│   │   ├── Protocol.cs
│   │   └── Server.cs
│   ├── Abilities/
│   │   ├── Chaos humans login-logout all classes.txt
│   │   ├── Dark elves login-logout all classes.txt
│   │   ├── Dwarves and their classes login-logout.txt
│   │   ├── High elves and their classes login-logout.txt
│   │   ├── Humans and their classes login-logout.txt
│   │   └── Orcs login-logout all classes.txt
│   ├── Configs/
│   │   ├── LocalDevelopment/
│   │   │   ├── Account.xml
│   │   │   ├── Launcher.xml
│   │   │   ├── Lobby.xml
│   │   │   └── World.xml
│   │   └── WorldConfigs.cs
│   ├── Console/
│   │   └── ReloadScriptsCommand.cs
│   ├── Core.cs
│   ├── Managers/
│   │   ├── AreaMapMgr.cs
│   │   ├── CharMgr.cs
│   │   ├── ClientFileMgr.cs
│   │   ├── CommandMgr.cs
│   │   ├── Commands/
│   │   │   ├── AbilityCommands.cs
│   │   │   ├── AddCommands.cs
│   │   │   ├── BaseCommands.cs
│   │   │   ├── CampaignCommands.cs
│   │   │   ├── ChapterCommands.cs
│   │   │   ├── CheckCommands.cs
│   │   │   ├── CommandDeclarations.cs
│   │   │   ├── CommandDocumentation.cs
│   │   │   ├── CommandsBuilder.cs
│   │   │   ├── DatabaseCommands.cs
│   │   │   ├── EquipCommands.cs
│   │   │   ├── EventCommands.cs
│   │   │   ├── GMUtils.cs
│   │   │   ├── GmCommandHandler.cs
│   │   │   ├── GmMgr.cs
│   │   │   ├── GoCommands.cs
│   │   │   ├── InstanceCommands.cs
│   │   │   ├── ModifyCommands.cs
│   │   │   ├── MountCommands.cs
│   │   │   ├── NpcCommands.cs
│   │   │   ├── OcclusionCommands.cs
│   │   │   ├── PqCommands.cs
│   │   │   ├── RespawnCommands.cs
│   │   │   ├── RespecCommands.cs
│   │   │   ├── ScenarioCommands.cs
│   │   │   ├── SearchCommands.cs
│   │   │   ├── SettingCommands.cs
│   │   │   ├── StatesCommand.cs
│   │   │   ├── TeleportCommands.cs
│   │   │   ├── TestCommands.cs
│   │   │   ├── TicketCommands.cs
│   │   │   └── WaypointCommands.cs
│   │   ├── LootsMgr.cs
│   │   └── WorldMgr.cs
│   ├── NLog.config
│   ├── NLog.xsd
│   ├── NetWork/
│   │   ├── Crypt/
│   │   │   └── RC4Crypto.cs
│   │   ├── GameClient.cs
│   │   ├── Handler/
│   │   │   ├── AuctionHandlers.cs
│   │   │   ├── AuthentificationHandlers.cs
│   │   │   ├── CharacterHandlers.cs
│   │   │   ├── ClientDatas.cs
│   │   │   ├── CombatHandlers.cs
│   │   │   ├── F_UNK1.cs
│   │   │   ├── GroupHandlers.cs
│   │   │   ├── GuildHandlers.cs
│   │   │   ├── HelpHandlers.cs
│   │   │   ├── InterfaceHandlers.cs
│   │   │   ├── InventoryHandlers.cs
│   │   │   ├── MailHandlers.cs
│   │   │   ├── MovementHandlers.cs
│   │   │   ├── QuestHandlers.cs
│   │   │   ├── ScenarioHandlers.cs
│   │   │   ├── SiegeHandlers.cs
│   │   │   ├── SocialHandlers.cs
│   │   │   ├── State2.cs
│   │   │   └── TchatHandlers.cs
│   │   ├── Opcodes.cs
│   │   ├── Packets/
│   │   │   └── UpdateState.cs
│   │   ├── ProtocolConstants.cs
│   │   └── TCPServer.cs
│   ├── Properties/
│   │   └── AssemblyInfo.cs
│   ├── Services/
│   │   ├── ServiceBase.cs
│   │   └── World/
│   │       ├── AnnounceService.cs
│   │       ├── BagService.cs
│   │       ├── BattlefrontService.cs
│   │       ├── BountyService.cs
│   │       ├── CellSpawnService.cs
│   │       ├── ChapterService.cs
│   │       ├── CreatureService.cs
│   │       ├── DyeService.cs
│   │       ├── GameObjectService.cs
│   │       ├── GuildService.cs
│   │       ├── HonorService.cs
│   │       ├── InstanceService.cs
│   │       ├── ItemService.cs
│   │       ├── LiveEventService.cs
│   │       ├── MailService.cs
│   │       ├── PQuestService.cs
│   │       ├── QuestService.cs
│   │       ├── RVRProgressionService.cs
│   │       ├── RVRZoneRewardService.cs
│   │       ├── RallyPointService.cs
│   │       ├── RewardService.cs
│   │       ├── ScenarioService.cs
│   │       ├── TokService.cs
│   │       ├── VendorService.cs
│   │       ├── WaypointService.cs
│   │       ├── XpRenownService.cs
│   │       └── ZoneService.cs
│   ├── Utils/
│   │   └── WorldUtils.cs
│   ├── World/
│   │   ├── AI/
│   │   │   ├── ABrain.cs
│   │   │   ├── Abilities/
│   │   │   │   ├── Conditions.cs
│   │   │   │   └── Executions.cs
│   │   │   ├── AggressiveBrain.cs
│   │   │   ├── BT/
│   │   │   │   ├── ChosenBehaviourTree.cs
│   │   │   │   └── ChosenBrain.cs
│   │   │   ├── BlackguardBrain.cs
│   │   │   ├── BossBrain.cs
│   │   │   ├── BossSpawn.cs
│   │   │   ├── BrainType.cs
│   │   │   ├── ChoppaBrain.cs
│   │   │   ├── CombatCreatureStateMachine.cs
│   │   │   ├── DummyBrain.cs
│   │   │   ├── EnhancedAI/
│   │   │   │   ├── CombatCreatureStateMachine.cs
│   │   │   │   ├── EnhancedCreature.cs
│   │   │   │   ├── NonCombatCreatureStateMachine.cs
│   │   │   │   ├── TargetSelectionMethod.cs
│   │   │   │   └── UnitAdaptor.cs
│   │   │   ├── EnhancedCreature.cs
│   │   │   ├── GuardBrain.cs
│   │   │   ├── HealerBrain.cs
│   │   │   ├── InstanceBossBrain.cs
│   │   │   ├── IronBreakerBrain.cs
│   │   │   ├── KnightBrain.cs
│   │   │   ├── MarauderBrain.cs
│   │   │   ├── MdpsStateMachine.cs
│   │   │   ├── NonCombatCreatureStateMachine.cs
│   │   │   ├── PassiveBrain.cs
│   │   │   ├── PathFinding/
│   │   │   │   ├── AStar/
│   │   │   │   │   ├── ComparePfNodeMatrix.cs
│   │   │   │   │   ├── HeuristicFormula.cs
│   │   │   │   │   ├── IPathFinder.cs
│   │   │   │   │   ├── IPriorityQueue.cs
│   │   │   │   │   ├── PathFinder.cs
│   │   │   │   │   ├── PathFinderNode.cs
│   │   │   │   │   ├── PathFinderNodeFast.cs
│   │   │   │   │   ├── PathFinderOptions.cs
│   │   │   │   │   ├── Point.cs
│   │   │   │   │   └── PriorityQueueB.cs
│   │   │   │   ├── AStar.cs
│   │   │   │   ├── Map.cs
│   │   │   │   └── SearchEngine.cs
│   │   │   ├── RangedBrain.cs
│   │   │   ├── RunepriestBrain.cs
│   │   │   ├── SimpleLVHealerBrain.cs
│   │   │   ├── SlayerBrain.cs
│   │   │   ├── TargetSelectionMethod.cs
│   │   │   ├── UnitAdaptor.cs
│   │   │   ├── WitchElfBrain.cs
│   │   │   ├── WitchHunterBrain.cs
│   │   │   └── ZealotBrain.cs
│   │   ├── Abilities/
│   │   │   ├── AbilityEffectInvoker.cs
│   │   │   ├── AbilityInterface.cs
│   │   │   ├── AbilityMgr.cs
│   │   │   ├── AbilityModifierInvoker.cs
│   │   │   ├── AbilityProcessor.cs
│   │   │   ├── Buffs/
│   │   │   │   ├── BuffEffectInvoker.cs
│   │   │   │   ├── BuffInterface.cs
│   │   │   │   ├── LinkedBuffInteraction.cs
│   │   │   │   ├── NewBuff.cs
│   │   │   │   └── SpecialBuffs/
│   │   │   │       ├── AuraBuff.cs
│   │   │   │       ├── BouncingBuff.cs
│   │   │   │       ├── DarkProtectorBuff.cs
│   │   │   │       ├── GuardBuff.cs
│   │   │   │       ├── HoldObjectBuff.cs
│   │   │   │       ├── InteractionBuff.cs
│   │   │   │       ├── OYGAuraBuff.cs
│   │   │   │       ├── OYGBuff.cs
│   │   │   │       ├── OathFriendBuff.cs
│   │   │   │       └── RationBuff.cs
│   │   │   ├── CareerInterfaces/
│   │   │   │   ├── CareerInterface.cs
│   │   │   │   ├── CareerInterface_AMShaman.cs
│   │   │   │   ├── CareerInterface_BWSorc.cs
│   │   │   │   ├── CareerInterface_BlackOrc.cs
│   │   │   │   ├── CareerInterface_Blackguard.cs
│   │   │   │   ├── CareerInterface_EngineerMagus.cs
│   │   │   │   ├── CareerInterface_Ironbreaker.cs
│   │   │   │   ├── CareerInterface_KnightChosen.cs
│   │   │   │   ├── CareerInterface_Marauder.cs
│   │   │   │   ├── CareerInterface_RPZealot.cs
│   │   │   │   ├── CareerInterface_ShadowWarrior.cs
│   │   │   │   ├── CareerInterface_SlayerChoppa.cs
│   │   │   │   ├── CareerInterface_SquigHerder.cs
│   │   │   │   ├── CareerInterface_Swordmaster.cs
│   │   │   │   ├── CareerInterface_WHWE.cs
│   │   │   │   ├── CareerInterface_WPDoK.cs
│   │   │   │   └── CareerInterface_WhiteLion.cs
│   │   │   ├── CombatManager.cs
│   │   │   ├── Components/
│   │   │   │   ├── AbilityCommandInfo.cs
│   │   │   │   ├── AbilityConstants.cs
│   │   │   │   ├── AbilityDamageInfo.cs
│   │   │   │   ├── AbilityEnums.cs
│   │   │   │   ├── AbilityInfo.cs
│   │   │   │   ├── BuffCommandInfo.cs
│   │   │   │   └── BuffInfo.cs
│   │   │   ├── Console/
│   │   │   │   └── ReloadScriptsCommand.cs
│   │   │   ├── NewChannelHandler.cs
│   │   │   ├── Objects/
│   │   │   │   ├── BuffHostObject.cs
│   │   │   │   ├── GroundTarget.cs
│   │   │   │   └── LandMine.cs
│   │   │   └── RunicBlessingsHandler.cs
│   │   ├── Auction/
│   │   │   └── AuctionHouse.cs
│   │   ├── Battlefronts/
│   │   │   ├── AAOTracker.cs
│   │   │   ├── Apocalypse/
│   │   │   │   ├── AAOTracker.cs
│   │   │   │   ├── ApocCommunications.cs
│   │   │   │   ├── BattleFrontStatus.cs
│   │   │   │   ├── BattlefieldObjective.cs
│   │   │   │   ├── BattlefrontConstants.cs
│   │   │   │   ├── Campaign.cs
│   │   │   │   ├── CampaignMetrics.cs
│   │   │   │   ├── CampaignObjectiveStateMachine.cs
│   │   │   │   ├── DeploymentReason.cs
│   │   │   │   ├── EnemyKeepLocationComparitor.cs
│   │   │   │   ├── FriendlyKeepLocationComparitor.cs
│   │   │   │   ├── GuildClaimObjective.cs
│   │   │   │   ├── IApocBattleFront.cs
│   │   │   │   ├── IApocCommunications.cs
│   │   │   │   ├── IBattlefrontManager.cs
│   │   │   │   ├── ILocationComparitor.cs
│   │   │   │   ├── KeepStateMachine.cs
│   │   │   │   ├── Loot/
│   │   │   │   │   ├── BagContentSelector.cs
│   │   │   │   │   ├── BlackMarketManager.cs
│   │   │   │   │   ├── ILootBagBuilder.cs
│   │   │   │   │   ├── ILootDecider.cs
│   │   │   │   │   ├── IRandomGenerator.cs
│   │   │   │   │   ├── IRewardAssigner.cs
│   │   │   │   │   ├── KeepLockRewardDistributor.cs
│   │   │   │   │   ├── KeepLockTracker.cs
│   │   │   │   │   ├── LootBagBuilder.cs
│   │   │   │   │   ├── LootBagRarity.cs
│   │   │   │   │   ├── LootBagTypeDefinition.cs
│   │   │   │   │   ├── RandomGenerator.cs
│   │   │   │   │   ├── RewardAssigner.cs
│   │   │   │   │   ├── SimpleContribution.cs
│   │   │   │   │   └── ZoneLockRewardDistributor.cs
│   │   │   │   ├── LowerTierCampaignManager.cs
│   │   │   │   ├── PlayerContributionManager.cs
│   │   │   │   ├── PlayerRewardOptions.cs
│   │   │   │   ├── PlayerUtil.cs
│   │   │   │   ├── RVRRewardManager.cs
│   │   │   │   ├── RamSpawnFlagComparitor.cs
│   │   │   │   ├── RealmCaptainManager.cs
│   │   │   │   ├── RegionLockManager.cs
│   │   │   │   ├── SiegeManager.cs
│   │   │   │   ├── SiegeMerchantLocationComparitor.cs
│   │   │   │   ├── SiegeTracker.cs
│   │   │   │   ├── UpperTierCampaignManager.cs
│   │   │   │   ├── VictoryPoint.cs
│   │   │   │   └── VictoryPointProgress.cs
│   │   │   ├── Bounty/
│   │   │   │   ├── BountyException.cs
│   │   │   │   ├── BountyManager.cs
│   │   │   │   ├── CharacterBounty.cs
│   │   │   │   ├── ContributionManager.cs
│   │   │   │   ├── ContributionStage.cs
│   │   │   │   ├── IBountyManager.cs
│   │   │   │   ├── IContributionManager.cs
│   │   │   │   ├── IImpactMatrixManager.cs
│   │   │   │   ├── IRewardManager.cs
│   │   │   │   ├── ImpactMatrixManager.cs
│   │   │   │   ├── PlayerContribution.cs
│   │   │   │   ├── PlayerImpact.cs
│   │   │   │   ├── PlayerRVRGearDrop.cs
│   │   │   │   ├── RenownBandRVRObjectiveTick.cs
│   │   │   │   ├── RenownBandRVRPairingLock.cs
│   │   │   │   ├── RenownBandRVRZoneLock.cs
│   │   │   │   ├── Reward.cs
│   │   │   │   ├── RewardManager.cs
│   │   │   │   ├── RewardPlayerKill.cs
│   │   │   │   └── StaticWrapper.cs
│   │   │   ├── ContributionTracker.cs
│   │   │   ├── Keeps/
│   │   │   │   ├── BattleFrontKeep.cs
│   │   │   │   ├── HardPoint.cs
│   │   │   │   ├── IKeepCommunications.cs
│   │   │   │   ├── KeepCommunications.cs
│   │   │   │   ├── KeepCreature.cs
│   │   │   │   ├── KeepDoor.cs
│   │   │   │   ├── KeepMessage.cs
│   │   │   │   ├── KeepNpcCreature.cs
│   │   │   │   ├── KeepRewardManager.cs
│   │   │   │   └── KeepTimer.cs
│   │   │   └── Objectives/
│   │   │       ├── PortalBase.cs
│   │   │       ├── PortalToGatehouse.cs
│   │   │       ├── PortalToObjective.cs
│   │   │       ├── PortalToWarcamp.cs
│   │   │       └── StateFlags.cs
│   │   ├── Bots/
│   │   │   └── AIPlayerBot.cs
│   │   ├── Guild/
│   │   │   └── Guild.cs
│   │   ├── Interfaces/
│   │   │   ├── AIInterface.cs
│   │   │   ├── BaseInterface.cs
│   │   │   ├── CombatInterface/
│   │   │   │   ├── CombatInterface_NPC.cs
│   │   │   │   ├── CombatInterface_Pet.cs
│   │   │   │   └── CombatInterface_Player.cs
│   │   │   ├── CombatInterface.cs
│   │   │   ├── CraftingApoInterface.cs
│   │   │   ├── CraftingTalInterface.cs
│   │   │   ├── CultivationInterface.cs
│   │   │   ├── EventInterface.cs
│   │   │   ├── GatheringInterface.cs
│   │   │   ├── GroupInterface.cs
│   │   │   ├── GuildInterface.cs
│   │   │   ├── ItemsInterface.cs
│   │   │   ├── LiveEventInterface.cs
│   │   │   ├── MailInterface.cs
│   │   │   ├── MovementInterface.cs
│   │   │   ├── ObjectStateInterface.cs
│   │   │   ├── QuestsInterface.cs
│   │   │   ├── RenownInterface.cs
│   │   │   ├── ScenarioInterface.cs
│   │   │   ├── ScriptsInterface.cs
│   │   │   ├── SiegeInterface.cs
│   │   │   ├── SocialInterface.cs
│   │   │   ├── StatsInterface.cs
│   │   │   ├── TacticsInterface.cs
│   │   │   └── TokInterface.cs
│   │   ├── Map/
│   │   │   ├── BulletOcclusion.cs
│   │   │   ├── CellMgr.cs
│   │   │   ├── HotSpot.cs
│   │   │   ├── IOcclusionProvider.cs
│   │   │   ├── Occlusion.cs
│   │   │   ├── Physics/
│   │   │   │   └── Format.cs
│   │   │   ├── RegionMgr.cs
│   │   │   └── ZoneMgr.cs
│   │   ├── Mounts/
│   │   │   └── Mount.cs
│   │   ├── Objects/
│   │   │   ├── AdvancedCreature.cs
│   │   │   ├── BlackMarketVendorItem.cs
│   │   │   ├── Boss.cs
│   │   │   ├── ChapterObject.cs
│   │   │   ├── Creature.cs
│   │   │   ├── CreatureState.cs
│   │   │   ├── DynamicVendor.cs
│   │   │   ├── GameObject.cs
│   │   │   ├── GoldChest.cs
│   │   │   ├── Group.cs
│   │   │   ├── HonorVendorItem.cs
│   │   │   ├── Instances/
│   │   │   │   ├── Bastion Stairs/
│   │   │   │   │   ├── SimpleKaarntheVanquisher.cs
│   │   │   │   │   ├── SimpleLordSlaurith.cs
│   │   │   │   │   ├── SimpleSkullLordVarIthrok.cs
│   │   │   │   │   └── SimpleTharlgnan.cs
│   │   │   │   ├── Bilerot Burrow/
│   │   │   │   │   ├── SimpleBartholomeustheSickly.cs
│   │   │   │   │   ├── SimpleSsrydianMorbidae.cs
│   │   │   │   │   └── SimpleTheBileLord.cs
│   │   │   │   ├── Bloodwrought Enclave/
│   │   │   │   │   ├── SimpleBarakustheGodslayer.cs
│   │   │   │   │   ├── SimpleCuliusEmbervine.cs
│   │   │   │   │   ├── SimpleKorthuktheRaging.cs
│   │   │   │   │   └── SimpleSarlothBloodtouched.cs
│   │   │   │   ├── Gunbad ROR/
│   │   │   │   │   ├── Arathremia.cs
│   │   │   │   │   ├── ArdtaFeed.cs
│   │   │   │   │   ├── BasicGunbad.cs
│   │   │   │   │   ├── BlazAndVelkyrrix.cs
│   │   │   │   │   ├── BroodMotherSzikalax.cs
│   │   │   │   │   ├── ElderKizzig.cs
│   │   │   │   │   ├── FoulMoufdaUngry.cs
│   │   │   │   │   ├── GarrolaththePoxbearer.cs
│   │   │   │   │   ├── GlompdaSquigMasta.cs
│   │   │   │   │   ├── GriblikdaStinka.cs
│   │   │   │   │   ├── HeraldofSolithex.cs
│   │   │   │   │   ├── Logazor.cs
│   │   │   │   │   ├── MastaMixa.cs
│   │   │   │   │   ├── MastaWranglaGlix.cs
│   │   │   │   │   ├── RedeyeBigOaf.cs
│   │   │   │   │   └── WightLordSolithex.cs
│   │   │   │   ├── Hunters Vale/
│   │   │   │   │   ├── SimpleSpiritofKurnous.cs
│   │   │   │   │   ├── SimpleThananTreeLord.cs
│   │   │   │   │   └── SimpleTheCadaithaineLion.cs
│   │   │   │   ├── Instance.cs
│   │   │   │   ├── InstanceBossSpawn.cs
│   │   │   │   ├── InstanceDoor.cs
│   │   │   │   ├── InstanceMgr.cs
│   │   │   │   ├── InstanceObject.cs
│   │   │   │   ├── InstanceSpawn.cs
│   │   │   │   ├── Mount Gunbad/
│   │   │   │   │   ├── SimpleArdtaFeed.cs
│   │   │   │   │   ├── SimpleGlompdaSquigMasta.cs
│   │   │   │   │   ├── SimpleMastaMixa.cs
│   │   │   │   │   └── SimpleWightLordSolithex.cs
│   │   │   │   ├── RemovedBosses/
│   │   │   │   │   ├── SimpleFulgurThunderborn.cs
│   │   │   │   │   └── SimpleTonragThunderborn.cs
│   │   │   │   ├── SacellumDungeonsEastWingSacellum3/
│   │   │   │   │   ├── SimpleGhalmarRagehorn.cs
│   │   │   │   │   ├── SimpleUzhaktheBetrayer.cs
│   │   │   │   │   └── SimpleVultheBloodchosen.cs
│   │   │   │   ├── SacellumDungeonsSouthWingSacellum1/
│   │   │   │   │   ├── SimpleGoremane.cs
│   │   │   │   │   ├── SimpleSnaptailtheBreeder.cs
│   │   │   │   │   └── SimpleViraxiltheBroken.cs
│   │   │   │   ├── SacellumDungeonsWestWingSacellum2/
│   │   │   │   │   ├── SimpleHoarfrost.cs
│   │   │   │   │   ├── SimpleLorthThunderbelly.cs
│   │   │   │   │   ├── SimpleSebcrawtheDiscarded.cs
│   │   │   │   │   └── SimpleSlorthThunderbelly.cs
│   │   │   │   ├── SigmarCrypts/
│   │   │   │   │   ├── SimpleArchLectorVerrimus.cs
│   │   │   │   │   ├── SimpleArchLectorZakarai.cs
│   │   │   │   │   ├── SimpleCryptwebQueen.cs
│   │   │   │   │   ├── SimpleNecromancerMalcidious.cs
│   │   │   │   │   ├── SimpleSeraphinePaleEye.cs
│   │   │   │   │   ├── SimpleSisterEudocia.cs
│   │   │   │   │   ├── SimpleTheReaper.cs
│   │   │   │   │   └── SimpleTobiastheFallen.cs
│   │   │   │   ├── The Lost Vale/
│   │   │   │   │   ├── SimpleAhzranok.cs
│   │   │   │   │   ├── SimpleButcherGutbeater.cs
│   │   │   │   │   ├── SimpleChulEarthkeeper.cs
│   │   │   │   │   ├── SimpleDralel.cs
│   │   │   │   │   ├── SimpleGoraktheAncient.cs
│   │   │   │   │   ├── SimpleHorgulul.cs
│   │   │   │   │   ├── SimpleLargtheDevourer.cs
│   │   │   │   │   ├── SimpleMalghorGreathorn.cs
│   │   │   │   │   ├── SimpleNKariKeeperofSecrets.cs
│   │   │   │   │   ├── SimpleSarthaintheWorldbearer.cs
│   │   │   │   │   ├── SimpleSechartheDarkpromiseChieftain.cs
│   │   │   │   │   ├── SimpleTheDarkpromiseBeast2.cs
│   │   │   │   │   ├── SimpleTheDeamonicBeast.cs
│   │   │   │   │   └── SimpleZaarthePainseeker.cs
│   │   │   │   ├── TheSewersofAltdorfWing1Sewers 2/
│   │   │   │   │   └── SimpleKokritManEater.cs
│   │   │   │   ├── TheSewersofAltdorfWing2Sewers3/
│   │   │   │   │   ├── SimpleBulbousOne.cs
│   │   │   │   │   ├── SimpleProtFangchitter.cs
│   │   │   │   │   └── SimpleVermerFangchitter.cs
│   │   │   │   ├── TheSewersofAltdorfWing3Sewers/
│   │   │   │   │   ├── AltdorfSewers3/
│   │   │   │   │   │   └── AltdorfSewersWingIII.cs
│   │   │   │   │   ├── SimpleGoradiantheCreator.cs
│   │   │   │   │   └── SimpleMasterMoulderVitchek.cs
│   │   │   │   ├── TomboftheVultureLord/
│   │   │   │   │   ├── SimpleAkiltheShrewd.cs
│   │   │   │   │   ├── SimpleHandofUalatp.cs
│   │   │   │   │   ├── SimpleHierophantEutrata.cs
│   │   │   │   │   ├── SimpleHighPriestHerakh1.cs
│   │   │   │   │   ├── SimpleHighPriestHerakh2.cs
│   │   │   │   │   ├── SimpleHighPriestHerakh3.cs
│   │   │   │   │   ├── SimpleJahitheIndignant.cs
│   │   │   │   │   ├── SimpleKingAmenemhetumtheVultureLord.cs
│   │   │   │   │   ├── SimpleTumainitheHopeless.cs
│   │   │   │   │   ├── SimpleUsiriansKeeper.cs
│   │   │   │   │   └── TOTVL.cs
│   │   │   │   ├── Tombs/
│   │   │   │   │   ├── SimpleBennuApeht.cs
│   │   │   │   │   ├── SimpleHapuShebikef.cs
│   │   │   │   │   ├── SimpleSaaKhasef.cs
│   │   │   │   │   └── SimpleTsekaniHeyafa.cs
│   │   │   │   ├── WarpbladeTunnels1/
│   │   │   │   │   ├── SimpleSkivRedwarp.cs
│   │   │   │   │   └── SimpleWarlockPeenk.cs
│   │   │   │   └── WarpbladeTunnels2/
│   │   │   │       ├── SimpleBrauk.cs
│   │   │   │       ├── SimpleGreySeerQuoltik.cs
│   │   │   │       └── SimpleMasterMoulderSkrot.cs
│   │   │   ├── Item.cs
│   │   │   ├── LootChest.cs
│   │   │   ├── NPCAbility.cs
│   │   │   ├── Object.cs
│   │   │   ├── Party.cs
│   │   │   ├── Pet.cs
│   │   │   ├── Player.cs
│   │   │   ├── PublicQuests/
│   │   │   │   ├── ContributionInfo.cs
│   │   │   │   ├── PQuestCreature.cs
│   │   │   │   ├── PQuestGameObject.cs
│   │   │   │   ├── PQuestObjective.cs
│   │   │   │   ├── PQuestStage.cs
│   │   │   │   └── PublicQuest.cs
│   │   │   ├── RVRArea.cs
│   │   │   ├── RVRRewardItem.cs
│   │   │   ├── RvRStructure.cs
│   │   │   ├── ScenarioGroupsHandler.cs
│   │   │   ├── Siege.cs
│   │   │   ├── SpawnPoint.cs
│   │   │   ├── Standard.cs
│   │   │   ├── Unit.cs
│   │   │   └── WarbandHandler.cs
│   │   ├── Positions/
│   │   │   ├── IPoint2D.cs
│   │   │   ├── IPoint3D.cs
│   │   │   ├── Point2D.cs
│   │   │   └── Point3D.cs
│   │   ├── Scenarios/
│   │   │   ├── CaptureTheFlagScenario.cs
│   │   │   ├── DominationScenario.cs
│   │   │   ├── DominationScenarioEC.cs
│   │   │   ├── DominationScenarioKhaine.cs
│   │   │   ├── DominationScenarioPush.cs
│   │   │   ├── DominationScenarioPushCenter.cs
│   │   │   ├── DoubleDominationScenario.cs
│   │   │   ├── DropBombScenario.cs
│   │   │   ├── DropPartScenario.cs
│   │   │   ├── FlagDominationScenario.cs
│   │   │   ├── MurderballScenario.cs
│   │   │   ├── Objects/
│   │   │   │   ├── Bomb.cs
│   │   │   │   ├── CapturePoint.cs
│   │   │   │   ├── ClickFlag.cs
│   │   │   │   ├── GunPowder.cs
│   │   │   │   ├── HoldObject.cs
│   │   │   │   ├── Part.cs
│   │   │   │   └── ProximityFlag.cs
│   │   │   ├── Scenario.cs
│   │   │   └── ScenarioMgr.cs
│   │   ├── Scripting/
│   │   │   ├── AGeneralScript.cs
│   │   │   ├── APublicQuestScript.cs
│   │   │   ├── AltdorfSewersWingIII.cs
│   │   │   ├── ArdtaFeed.cs
│   │   │   ├── BasicScript.cs
│   │   │   ├── BehaviorLib.cs
│   │   │   ├── Creatures/
│   │   │   │   └── WorldMountsScript.cs
│   │   │   ├── Dungeons/
│   │   │   │   └── AltdorfSewersWingIII.cs
│   │   │   ├── Events/
│   │   │   │   ├── BrightWizardCollegeReopen/
│   │   │   │   │   ├── BalthasarGelt.cs
│   │   │   │   │   └── ThyrusGormann.cs
│   │   │   │   └── Halloween/
│   │   │   │       └── BasicHalloween.cs
│   │   │   ├── GameObject/
│   │   │   │   ├── Door.cs
│   │   │   │   └── MailBoxScript.cs
│   │   │   ├── GeneralScriptAttributes.cs
│   │   │   ├── Items/
│   │   │   │   └── PotionScript.cs
│   │   │   ├── Lairs/
│   │   │   │   └── Beastlords/
│   │   │   │       ├── AllBeastlords.cs
│   │   │   │       └── BasicBeastlord.cs
│   │   │   ├── PacketSenderScript.cs
│   │   │   ├── PublicQuests/
│   │   │   │   ├── BasicPublicQuest.cs
│   │   │   │   ├── Destro/
│   │   │   │   │   └── DeathstoneQuarry.cs
│   │   │   │   └── Order/
│   │   │   │       └── RavenHostVanguard.cs
│   │   │   └── Quests/
│   │   │       ├── BasicQuest.cs
│   │   │       ├── GrimmenhagenBurning.cs
│   │   │       └── HeartsAndMinds.cs
│   │   ├── WarHelper/
│   │   │   ├── RandomHelper.cs
│   │   │   └── TimeHelper.cs
│   │   └── WorldSettings/
│   │       ├── WorldSettings.cs
│   │       └── WorldSettingsMgr.cs
│   ├── WorldServer.csproj
│   ├── app.config
│   ├── packages.config
│   └── world.ico
├── cache/
│   └── audio.cache
├── deps/
│   ├── EasyMyp/
│   ├── NifLib/
│   │   └── OpenTK.xml
│   ├── protobuf/
│   └── zones/
│       ├── .vs\ProjectSettings.json
│       ├── .vs\ProjectWAR-Zones.slnx\
│       ├── .vs\ProjectWAR-Zones.slnx\FileContentIndex\84a39748-fa96-4836-a1ce-2944ac4ea176.vsidx
│       ├── .vs\ProjectWAR-Zones.slnx\v18\DocumentLayout.json
│       ├── .vs\ProjectWAR-Zones\
│       ├── .vs\ProjectWAR-Zones\v18\workspaceFileList.bin
│       ├── .vs\VSWorkspaceState.json
│       ├── .vs\slnx.sqlite
│       ├── README.md
│       ├── mappieces.csv
│       ├── zone001\1_map.jpg
│       ├── zone001\areas001.png
│       ├── zone001\influenceids.csv
│       ├── zone001\mappieces.csv
│       ├── zone001\offset.png
│       ├── zone001\piece01.jpg
│       ├── zone001\piece02.jpg
│       ├── zone001\piece03.jpg
│       ├── zone001\piece04.jpg
│       ├── zone001\piece05.jpg
│       ├── zone001\piece06.jpg
│       ├── zone001\piece07.jpg
│       ├── zone001\piece08.jpg
│       ├── zone001\pqarea001.png
│       ├── zone001\terrain.png
│       ├── zone002\2_map.jpg
│       ├── zone002\areas002.png
│       ├── zone002\influenceids.csv
│       ├── zone002\mappieces.csv
│       ├── zone002\offset.png
│       ├── zone002\piece01.jpg
│       ├── zone002\piece02.jpg
│       ├── zone002\piece03.jpg
│       ├── zone002\piece04.jpg
│       ├── zone002\piece05.jpg
│       ├── zone002\piece06.jpg
│       ├── zone002\piece07.jpg
│       ├── zone002\piece08.jpg
│       ├── zone002\pqarea002.png
│       ├── zone002\terrain.png
│       ├── zone003\3_map.jpg
│       ├── zone003\areas003.png
│       ├── zone003\influenceids.csv
│       ├── zone003\mappieces.csv
│       ├── zone003\offset.png
│       ├── zone003\piece01.jpg
│       ├── zone003\piece02.jpg
│       ├── zone003\piece03.jpg
│       ├── zone003\piece04.jpg
│       ├── zone003\piece05.jpg
│       ├── zone003\piece06.jpg
│       ├── zone003\piece07.jpg
│       ├── zone003\piece08.jpg
│       ├── zone003\piece09.jpg
│       ├── zone003\piece10.jpg
│       ├── zone003\piece11.jpg
│       ├── zone003\piece12.jpg
│       ├── zone003\piece13.jpg
│       ├── zone003\piece14.jpg
│       ├── zone003\pqarea003.png
│       ├── zone003\terrain.png
│       ├── zone004\areas004.png
│       ├── zone004\influenceids.csv
│       ├── zone004\offset.png
│       ├── zone004\pqarea004.png
│       ├── zone004\terrain.png
│       ├── zone005\5_map.jpg
│       ├── zone005\areas005.png
│       ├── zone005\influenceids.csv
│       ├── zone005\mappieces.csv
│       ├── zone005\offset.png
│       ├── zone005\piece01.jpg
│       ├── zone005\piece02.jpg
│       ├── zone005\piece03.jpg
│       ├── zone005\piece04.jpg
│       ├── zone005\piece05.jpg
│       ├── zone005\piece06.jpg
│       ├── zone005\piece07.jpg
│       ├── zone005\piece08.jpg
│       ├── zone005\piece09.jpg
│       ├── zone005\piece10.jpg
│       ├── zone005\piece11.jpg
│       ├── zone005\piece12.jpg
│       ├── zone005\piece13.jpg
│       ├── zone005\piece14.jpg
│       ├── zone005\pqarea005.png
│       ├── zone005\terrain.png
│       ├── zone006\areas006.png
│       ├── zone006\influenceids.csv
│       ├── zone006\mappieces.csv
│       ├── zone006\offset.png
│       ├── zone006\piece01.jpg
│       ├── zone006\piece02.jpg
│       ├── zone006\piece03.jpg
│       ├── zone006\piece04.jpg
│       ├── zone006\piece05.jpg
│       ├── zone006\piece06.jpg
│       ├── zone006\piece07.jpg
│       ├── zone006\pqarea006.png
│       ├── zone006\terrain.png
│       ├── zone007\7_map.jpg
│       ├── zone007\areas007.png
│       ├── zone007\influenceids.csv
│       ├── zone007\mappieces.csv
│       ├── zone007\offset.png
│       ├── zone007\piece01.jpg
│       ├── zone007\piece02.jpg
│       ├── zone007\piece03.jpg
│       ├── zone007\piece04.jpg
│       ├── zone007\piece05.jpg
│       ├── zone007\piece06.jpg
│       ├── zone007\piece07.jpg
│       ├── zone007\piece08.jpg
│       ├── zone007\piece09.jpg
│       ├── zone007\pqarea007.png
│       ├── zone007\terrain.png
│       ├── zone008\8_map.jpg
│       ├── zone008\areas008.png
│       ├── zone008\influenceids.csv
│       ├── zone008\mappieces.csv
│       ├── zone008\offset.png
│       ├── zone008\piece01.jpg
│       ├── zone008\piece02.jpg
│       ├── zone008\piece03.jpg
│       ├── zone008\piece04.jpg
│       ├── zone008\piece05.jpg
│       ├── zone008\piece06.jpg
│       ├── zone008\piece07.jpg
│       ├── zone008\piece08.jpg
│       ├── zone008\piece09.jpg
│       ├── zone008\piece10.jpg
│       ├── zone008\pqarea008.png
│       ├── zone008\terrain.png
│       ├── zone009\9_map.jpg
│       ├── zone009\areas009.png
│       ├── zone009\influenceids.csv
│       ├── zone009\mappieces.csv
│       ├── zone009\offset.png
│       ├── zone009\piece01.jpg
│       ├── zone009\piece02.jpg
│       ├── zone009\piece03.jpg
│       ├── zone009\piece04.jpg
│       ├── zone009\piece05.jpg
│       ├── zone009\piece06.jpg
│       ├── zone009\piece07.jpg
│       ├── zone009\piece08.jpg
│       ├── zone009\piece09.jpg
│       ├── zone009\piece10.jpg
│       ├── zone009\piece11.jpg
│       ├── zone009\pqarea009.png
│       ├── zone009\terrain.png
│       ├── zone010\areas010.png
│       ├── zone010\influenceids.csv
│       ├── zone010\offset.png
│       ├── zone010\pqarea010.png
│       ├── zone010\terrain.png
│       ├── zone011\areas011.png
│       ├── zone011\influenceids.csv
│       ├── zone011\mappieces.csv
│       ├── zone011\offset.png
│       ├── zone011\piece01.jpg
│       ├── zone011\piece02.jpg
│       ├── zone011\piece03.jpg
│       ├── zone011\piece04.jpg
│       ├── zone011\piece05.jpg
│       ├── zone011\piece06.jpg
│       ├── zone011\piece07.jpg
│       ├── zone011\piece08.jpg
│       ├── zone011\piece09.jpg
│       ├── zone011\piece10.jpg
│       ├── zone011\piece11.jpg
│       ├── zone011\piece12.jpg
│       ├── zone011\pqarea011.png
│       ├── zone011\terrain.png
│       ├── zone012\offset.png
│       ├── zone012\terrain.png
│       ├── zone013\offset.png
│       ├── zone013\terrain.png
│       ├── zone014\offset.png
│       ├── zone014\terrain.png
│       ├── zone015\offset.png
│       ├── zone015\terrain.png
│       ├── zone016\offset.png
│       ├── zone016\terrain.png
│       ├── zone017\offset.png
│       ├── zone017\terrain.png
│       ├── zone018\offset.png
│       ├── zone018\terrain.png
│       ├── zone019\offset.png
│       ├── zone019\terrain.png
│       ├── zone020\offset.png
│       ├── zone020\terrain.png
│       ├── zone021\offset.png
│       ├── zone021\terrain.png
│       ├── zone022\offset.png
│       ├── zone022\terrain.png
│       ├── zone023\offset.png
│       ├── zone023\terrain.png
│       ├── zone024\offset.png
│       ├── zone024\terrain.png
│       ├── zone025\offset.png
│       ├── zone025\terrain.png
│       ├── zone026\areas026.png
│       ├── zone026\influenceids.csv
│       ├── zone026\mappieces.csv
│       ├── zone026\offset.png
│       ├── zone026\piece01.jpg
│       ├── zone026\piece02.jpg
│       ├── zone026\piece03.jpg
│       ├── zone026\piece04.jpg
│       ├── zone026\piece05.jpg
│       ├── zone026\piece06.jpg
│       ├── zone026\pqarea026.png
│       ├── zone026\terrain.png
│       ├── zone027\areas027.png
│       ├── zone027\influenceids.csv
│       ├── zone027\mappieces.csv
│       ├── zone027\offset.png
│       ├── zone027\piece01.jpg
│       ├── zone027\piece02.jpg
│       ├── zone027\piece03.jpg
│       ├── zone027\piece04.jpg
│       ├── zone027\piece05.jpg
│       ├── zone027\piece06.jpg
│       ├── zone027\piece07.jpg
│       ├── zone027\piece08.jpg
│       ├── zone027\pqarea027.png
│       ├── zone027\terrain.png
│       ├── zone028\offset.png
│       ├── zone028\terrain.png
│       ├── zone030\offset.png
│       ├── zone030\terrain.png
│       ├── zone031\offset.png
│       ├── zone031\terrain.png
│       ├── zone032\offset.png
│       ├── zone032\shademap.png
│       ├── zone032\terrain.png
│       ├── zone033\offset.png
│       ├── zone033\terrain.png
│       ├── zone034\offset.png
│       ├── zone034\terrain.png
│       ├── zone036\offset.png
│       ├── zone036\terrain.png
│       ├── zone038\offset.png
│       ├── zone038\terrain.png
│       ├── zone039\offset.png
│       ├── zone039\terrain.png
│       ├── zone041\offset.png
│       ├── zone041\terrain.png
│       ├── zone042\offset.png
│       ├── zone042\terrain.png
│       ├── zone043\offset.png
│       ├── zone043\terrain.png
│       ├── zone044\offset.png
│       ├── zone044\terrain.png
│       ├── zone045\offset.png
│       ├── zone045\terrain.png
│       ├── zone060\60_map.dds
│       ├── zone060\areas060.png
│       ├── zone060\influenceids.csv
│       ├── zone060\offset.png
│       ├── zone060\pqarea060.png
│       ├── zone060\terrain.png
│       ├── zone063\offset.png
│       ├── zone063\terrain.png
│       ├── zone064\offset.png
│       ├── zone064\terrain.png
│       ├── zone065\offset.png
│       ├── zone065\terrain.png
│       ├── zone066\offset.png
│       ├── zone066\terrain.png
│       ├── zone070\offset.png
│       ├── zone070\terrain.png
│       ├── zone071\offset.png
│       ├── zone071\terrain.png
│       ├── zone072\offset.png
│       ├── zone072\terrain.png
│       ├── zone073\offset.png
│       ├── zone073\terrain.png
│       ├── zone074\offset.png
│       ├── zone074\terrain.png
│       ├── zone075\offset.png
│       ├── zone075\terrain.png
│       ├── zone076\offset.png
│       ├── zone076\terrain.png
│       ├── zone077\offset.png
│       ├── zone077\terrain.png
│       ├── zone078\offset.png
│       ├── zone078\terrain.png
│       ├── zone079\offset.png
│       ├── zone079\terrain.png
│       ├── zone080\offset.png
│       ├── zone080\terrain.png
│       ├── zone082\offset.png
│       ├── zone082\terrain.png
│       ├── zone083\offset.png
│       ├── zone083\terrain.png
│       ├── zone084\offset.png
│       ├── zone084\terrain.png
│       ├── zone085\offset.png
│       ├── zone085\terrain.png
│       ├── zone087\offset.png
│       ├── zone087\terrain.png
│       ├── zone088\offset.png
│       ├── zone088\terrain.png
│       ├── zone089\offset.png
│       ├── zone089\terrain.png
│       ├── zone100\areas100.png
│       ├── zone100\influenceids.csv
│       ├── zone100\mappieces.csv
│       ├── zone100\offset.png
│       ├── zone100\piece01.jpg
│       ├── zone100\piece02.jpg
│       ├── zone100\piece03.jpg
│       ├── zone100\piece04.jpg
│       ├── zone100\piece05.jpg
│       ├── zone100\piece06.jpg
│       ├── zone100\pqarea100.png
│       ├── zone100\terrain.png
│       ├── zone101\101_map.jpg
│       ├── zone101\areas101.png
│       ├── zone101\influenceids.csv
│       ├── zone101\mappieces.csv
│       ├── zone101\offset.png
│       ├── zone101\piece01.jpg
│       ├── zone101\piece02.jpg
│       ├── zone101\piece03.jpg
│       ├── zone101\piece04.jpg
│       ├── zone101\piece05.jpg
│       ├── zone101\piece06.jpg
│       ├── zone101\piece07.jpg
│       ├── zone101\piece08.jpg
│       ├── zone101\piece09.jpg
│       ├── zone101\piece10.jpg
│       ├── zone101\piece11.jpg
│       ├── zone101\piece12.jpg
│       ├── zone101\piece13.jpg
│       ├── zone101\piece14.jpg
│       ├── zone101\piece15.jpg
│       ├── zone101\piece16.jpg
│       ├── zone101\piece17.jpg
│       ├── zone101\pqarea101.png
│       ├── zone101\tc_colorize.jpg
│       ├── zone101\tc_colorize.psd
│       ├── zone101\tc_final.psd
│       ├── zone101\terrain.png
│       ├── zone101\trollcountry.psd
│       ├── zone102\102_map.jpg
│       ├── zone102\areas102.png
│       ├── zone102\influenceids.csv
│       ├── zone102\mappieces.csv
│       ├── zone102\offset.png
│       ├── zone102\piece01.jpg
│       ├── zone102\piece02.jpg
│       ├── zone102\piece03.jpg
│       ├── zone102\piece04.jpg
│       ├── zone102\piece05.jpg
│       ├── zone102\piece06.jpg
│       ├── zone102\piece07.jpg
│       ├── zone102\piece08.jpg
│       ├── zone102\piece09.jpg
│       ├── zone102\piece10.jpg
│       ├── zone102\piece11.jpg
│       ├── zone102\piece12.jpg
│       ├── zone102\pqarea102.png
│       ├── zone102\terrain.png
│       ├── zone103\103_map.jpg
│       ├── zone103\areas103.png
│       ├── zone103\influenceids.csv
│       ├── zone103\mappieces.csv
│       ├── zone103\offset.png
│       ├── zone103\piece01.jpg
│       ├── zone103\piece02.jpg
│       ├── zone103\piece03.jpg
│       ├── zone103\piece04.jpg
│       ├── zone103\piece05.jpg
│       ├── zone103\piece06.jpg
│       ├── zone103\piece07.jpg
│       ├── zone103\piece08.jpg
│       ├── zone103\piece09.jpg
│       ├── zone103\pqarea103.png
│       ├── zone103\terrain.png
│       ├── zone104\areas104.png
│       ├── zone104\influenceids.csv
│       ├── zone104\offset.png
│       ├── zone104\pqarea104.png
│       ├── zone104\terrain.png
│       ├── zone105\105_map.jpg
│       ├── zone105\Original_areas105.png
│       ├── zone105\areas105.png
│       ├── zone105\influenceids.csv
│       ├── zone105\mappieces.csv
│       ├── zone105\offset.png
│       ├── zone105\piece01.jpg
│       ├── zone105\piece02.jpg
│       ├── zone105\piece03.jpg
│       ├── zone105\piece04.jpg
│       ├── zone105\piece05.jpg
│       ├── zone105\piece06.jpg
│       ├── zone105\piece07.jpg
│       ├── zone105\piece08.jpg
│       ├── zone105\piece09.jpg
│       ├── zone105\pqarea105.png
│       ├── zone105\terrain.png
│       ├── zone106\areas106.png
│       ├── zone106\areas106_OLD.png
│       ├── zone106\influenceids.csv
│       ├── zone106\mappieces.csv
│       ├── zone106\offset.png
│       ├── zone106\piece01.jpg
│       ├── zone106\piece02.jpg
│       ├── zone106\piece03.jpg
│       ├── zone106\piece04.jpg
│       ├── zone106\piece05.jpg
│       ├── zone106\piece06.jpg
│       ├── zone106\piece07.jpg
│       ├── zone106\pqarea106.png
│       ├── zone106\terrain.png
│       ├── zone107\107_map.jpg
│       ├── zone107\areas107.png
│       ├── zone107\influenceids.csv
│       ├── zone107\mappieces.csv
│       ├── zone107\offset.png
│       ├── zone107\piece01.jpg
│       ├── zone107\piece02.jpg
│       ├── zone107\piece03.jpg
│       ├── zone107\piece04.jpg
│       ├── zone107\piece05.jpg
│       ├── zone107\piece06.jpg
│       ├── zone107\piece07.jpg
│       ├── zone107\piece08.jpg
│       ├── zone107\piece09.jpg
│       ├── zone107\piece10.jpg
│       ├── zone107\piece11.jpg
│       ├── zone107\piece12.jpg
│       ├── zone107\piece13.jpg
│       ├── zone107\piece14.jpg
│       ├── zone107\piece15.jpg
│       ├── zone107\piece16.jpg
│       ├── zone107\pqarea107.png
│       ├── zone107\terrain.png
│       ├── zone108\108_map.jpg
│       ├── zone108\areas108.png
│       ├── zone108\influenceids.csv
│       ├── zone108\mappieces.csv
│       ├── zone108\offset.png
│       ├── zone108\piece01.jpg
│       ├── zone108\piece02.jpg
│       ├── zone108\piece03.jpg
│       ├── zone108\piece04.jpg
│       ├── zone108\piece05.jpg
│       ├── zone108\piece06.jpg
│       ├── zone108\piece07.jpg
│       ├── zone108\piece08.jpg
│       ├── zone108\piece09.jpg
│       ├── zone108\piece10.jpg
│       ├── zone108\piece11.jpg
│       ├── zone108\piece12.jpg
│       ├── zone108\pqarea108.png
│       ├── zone108\terrain.png
│       ├── zone109\109_map.jpg
│       ├── zone109\areas109.png
│       ├── zone109\influenceids.csv
│       ├── zone109\mappieces.csv
│       ├── zone109\offset.png
│       ├── zone109\piece01.jpg
│       ├── zone109\piece02.jpg
│       ├── zone109\piece03.jpg
│       ├── zone109\piece04.jpg
│       ├── zone109\piece05.jpg
│       ├── zone109\piece06.jpg
│       ├── zone109\pqarea109.png
│       ├── zone109\terrain.png
│       ├── zone110\areas110.png
│       ├── zone110\influenceids.csv
│       ├── zone110\offset.png
│       ├── zone110\pqarea105.png
│       ├── zone110\pqarea110.png
│       ├── zone110\terrain.png
│       ├── zone111\offset.png
│       ├── zone111\terrain.png
│       ├── zone112\offset.png
│       ├── zone112\terrain.png
│       ├── zone113\offset.png
│       ├── zone113\terrain.png
│       ├── zone115\offset.png
│       ├── zone115\terrain.png
│       ├── zone116\offset.png
│       ├── zone116\terrain.png
│       ├── zone117\offset.png
│       ├── zone117\terrain.png
│       ├── zone118\offset.png
│       ├── zone118\terrain.png
│       ├── zone120\areas120.png
│       ├── zone120\influenceids.csv
│       ├── zone120\offset.png
│       ├── zone120\piece01.jpg
│       ├── zone120\piece02.jpg
│       ├── zone120\piece03.jpg
│       ├── zone120\piece06.jpg
│       ├── zone120\pqarea120.png
│       ├── zone120\terrain.png
│       ├── zone121\offset.png
│       ├── zone121\terrain.png
│       ├── zone123\offset.png
│       ├── zone123\terrain.png
│       ├── zone125\offset.png
│       ├── zone125\terrain.png
│       ├── zone126\offset.png
│       ├── zone126\terrain.png
│       ├── zone127\offset.png
│       ├── zone127\terrain.png
│       ├── zone128\offset.png
│       ├── zone128\terrain.png
│       ├── zone129\offset.png
│       ├── zone129\terrain.png
│       ├── zone130\offset.png
│       ├── zone130\terrain.png
│       ├── zone131\offset.png
│       ├── zone131\terrain.png
│       ├── zone132\offset.png
│       ├── zone132\terrain.png
│       ├── zone133\offset.png
│       ├── zone133\terrain.png
│       ├── zone134\offset.png
│       ├── zone134\terrain.png
│       ├── zone135\offset.png
│       ├── zone135\terrain.png
│       ├── zone136\offset.png
│       ├── zone136\terrain.png
│       ├── zone137\offset.png
│       ├── zone137\terrain.png
│       ├── zone138\offset.png
│       ├── zone138\terrain.png
│       ├── zone139\offset.png
│       ├── zone139\terrain.png
│       ├── zone140\offset.png
│       ├── zone140\terrain.png
│       ├── zone142\offset.png
│       ├── zone142\terrain.png
│       ├── zone143\offset.png
│       ├── zone143\terrain.png
│       ├── zone144\offset.png
│       ├── zone144\terrain.png
│       ├── zone147\offset.png
│       ├── zone147\terrain.png
│       ├── zone152\offset.png
│       ├── zone152\terrain.png
│       ├── zone153\offset.png
│       ├── zone153\terrain.png
│       ├── zone154\offset.png
│       ├── zone154\terrain.png
│       ├── zone155\offset.png
│       ├── zone155\terrain.png
│       ├── zone156\offset.png
│       ├── zone156\terrain.png
│       ├── zone160\influenceids.csv
│       ├── zone160\offset.png
│       ├── zone160\terrain.png
│       ├── zone161\influenceids.csv
│       ├── zone161\offset.png
│       ├── zone161\terrain.png
│       ├── zone162\influenceids.csv
│       ├── zone162\offset.png
│       ├── zone162\terrain.png
│       ├── zone163\offset.png
│       ├── zone163\terrain.png
│       ├── zone164\offset.png
│       ├── zone164\terrain.png
│       ├── zone165\offset.png
│       ├── zone165\terrain.png
│       ├── zone166\offset.png
│       ├── zone166\terrain.png
│       ├── zone167\offset.png
│       ├── zone167\terrain.png
│       ├── zone168\offset.png
│       ├── zone168\terrain.png
│       ├── zone169\offset.png
│       ├── zone169\terrain.png
│       ├── zone170\offset.png
│       ├── zone170\terrain.png
│       ├── zone171\offset.png
│       ├── zone171\terrain.png
│       ├── zone172\offset.png
│       ├── zone172\terrain.png
│       ├── zone173\offset.png
│       ├── zone173\terrain.png
│       ├── zone174\offset.png
│       ├── zone174\terrain.png
│       ├── zone175\offset.png
│       ├── zone175\terrain.png
│       ├── zone176\offset.png
│       ├── zone176\terrain.png
│       ├── zone177\offset.png
│       ├── zone177\terrain.png
│       ├── zone178\offset.png
│       ├── zone178\terrain.png
│       ├── zone179\influenceids.csv
│       ├── zone179\offset.png
│       ├── zone179\terrain.png
│       ├── zone180\offset.png
│       ├── zone180\terrain.png
│       ├── zone181\offset.png
│       ├── zone181\terrain.png
│       ├── zone182\offset.png
│       ├── zone182\terrain.png
│       ├── zone184\offset.png
│       ├── zone184\terrain.png
│       ├── zone189\offset.png
│       ├── zone189\terrain.png
│       ├── zone190\offset.png
│       ├── zone190\terrain.png
│       ├── zone191\areas191.png
│       ├── zone191\influenceids.csv
│       ├── zone191\mappieces.csv
│       ├── zone191\offset.png
│       ├── zone191\piece01.jpg
│       ├── zone191\piece02.jpg
│       ├── zone191\piece03.jpg
│       ├── zone191\piece04.jpg
│       ├── zone191\piece05.jpg
│       ├── zone191\piece06.jpg
│       ├── zone191\pqarea191.png
│       ├── zone191\terrain.png
│       ├── zone192\offset.png
│       ├── zone192\terrain.png
│       ├── zone193\offset.png
│       ├── zone193\terrain.png
│       ├── zone194\offset.png
│       ├── zone194\terrain.png
│       ├── zone195\offset.png
│       ├── zone195\terrain.png
│       ├── zone196\offset.png
│       ├── zone196\terrain.png
│       ├── zone198\offset.png
│       ├── zone198\terrain.png
│       ├── zone199\offset.png
│       ├── zone199\terrain.png
│       ├── zone200\areas200.png
│       ├── zone200\influenceids.csv
│       ├── zone200\mappieces.csv
│       ├── zone200\offset.png
│       ├── zone200\piece01.jpg
│       ├── zone200\piece02.jpg
│       ├── zone200\piece03.jpg
│       ├── zone200\piece04.jpg
│       ├── zone200\piece05.jpg
│       ├── zone200\piece06.jpg
│       ├── zone200\piece07.jpg
│       ├── zone200\piece08.jpg
│       ├── zone200\piece09.jpg
│       ├── zone200\piece10.jpg
│       ├── zone200\piece11.jpg
│       ├── zone200\piece12.jpg
│       ├── zone200\piece13.jpg
│       ├── zone200\piece14.jpg
│       ├── zone200\piece15.jpg
│       ├── zone200\piece16.jpg
│       ├── zone200\pqarea200.png
│       ├── zone200\terrain.png
│       ├── zone201\201_map.jpg
│       ├── zone201\areas201.png
│       ├── zone201\influenceids.csv
│       ├── zone201\mappieces.csv
│       ├── zone201\offset.png
│       ├── zone201\piece01.jpg
│       ├── zone201\piece02.jpg
│       ├── zone201\piece03.jpg
│       ├── zone201\piece04.jpg
│       ├── zone201\piece05.jpg
│       ├── zone201\piece06.jpg
│       ├── zone201\piece07.jpg
│       ├── zone201\pqarea201.png
│       ├── zone201\terrain.png
│       ├── zone202\202_map.jpg
│       ├── zone202\areas202.png
│       ├── zone202\influenceids.csv
│       ├── zone202\mappieces.csv
│       ├── zone202\offset.png
│       ├── zone202\piece01.jpg
│       ├── zone202\piece02.jpg
│       ├── zone202\piece03.jpg
│       ├── zone202\piece04.jpg
│       ├── zone202\piece05.jpg
│       ├── zone202\piece06.jpg
│       ├── zone202\piece07.jpg
│       ├── zone202\pqarea202.png
│       ├── zone202\terrain.png
│       ├── zone203\203_map.jpg
│       ├── zone203\areas203.png
│       ├── zone203\influenceids.csv
│       ├── zone203\mappieces.csv
│       ├── zone203\offset.png
│       ├── zone203\piece01.jpg
│       ├── zone203\piece02.jpg
│       ├── zone203\piece03.jpg
│       ├── zone203\piece04.jpg
│       ├── zone203\piece05.jpg
│       ├── zone203\piece06.jpg
│       ├── zone203\piece07.jpg
│       ├── zone203\pqarea203.png
│       ├── zone203\terrain.png
│       ├── zone204\areas204.png
│       ├── zone204\influenceids.csv
│       ├── zone204\offset.png
│       ├── zone204\pqarea204.png
│       ├── zone204\terrain.png
│       ├── zone205\205_map.jpg
│       ├── zone205\areas205.png
│       ├── zone205\influenceids.csv
│       ├── zone205\mappieces.csv
│       ├── zone205\offset.png
│       ├── zone205\piece01.jpg
│       ├── zone205\piece02.jpg
│       ├── zone205\piece03.jpg
│       ├── zone205\piece04.jpg
│       ├── zone205\piece05.jpg
│       ├── zone205\piece06.jpg
│       ├── zone205\piece07.jpg
│       ├── zone205\piece08.jpg
│       ├── zone205\piece09.jpg
│       ├── zone205\piece10.jpg
│       ├── zone205\pqarea205.png
│       ├── zone205\terrain.png
│       ├── zone206\areas206.png
│       ├── zone206\influenceids.csv
│       ├── zone206\mappieces.csv
│       ├── zone206\offset.png
│       ├── zone206\piece01.jpg
│       ├── zone206\piece02.jpg
│       ├── zone206\piece03.jpg
│       ├── zone206\piece04.jpg
│       ├── zone206\piece05.jpg
│       ├── zone206\piece06.jpg
│       ├── zone206\piece07.jpg
│       ├── zone206\piece08.jpg
│       ├── zone206\pqarea206.png
│       ├── zone206\terrain.png
│       ├── zone207\207_map.jpg
│       ├── zone207\areas207.png
│       ├── zone207\influenceids.csv
│       ├── zone207\mappieces.csv
│       ├── zone207\offset.png
│       ├── zone207\piece01.jpg
│       ├── zone207\piece02.jpg
│       ├── zone207\piece03.jpg
│       ├── zone207\piece04.jpg
│       ├── zone207\piece05.jpg
│       ├── zone207\piece06.jpg
│       ├── zone207\piece07.jpg
│       ├── zone207\pqarea207.png
│       ├── zone207\terrain.png
│       ├── zone208\208_map.jpg
│       ├── zone208\areas208.png
│       ├── zone208\influenceids.csv
│       ├── zone208\mappieces.csv
│       ├── zone208\offset.png
│       ├── zone208\piece01.jpg
│       ├── zone208\piece02.jpg
│       ├── zone208\piece03.jpg
│       ├── zone208\piece04.jpg
│       ├── zone208\piece05.jpg
│       ├── zone208\piece06.jpg
│       ├── zone208\piece07.jpg
│       ├── zone208\piece08.jpg
│       ├── zone208\piece09.jpg
│       ├── zone208\piece10.jpg
│       ├── zone208\piece11.jpg
│       ├── zone208\piece12.jpg
│       ├── zone208\pqarea208.png
│       ├── zone208\terrain.png
│       ├── zone209\209_map.jpg
│       ├── zone209\areas209.png
│       ├── zone209\influenceids.csv
│       ├── zone209\mappieces.csv
│       ├── zone209\offset.png
│       ├── zone209\piece01.jpg
│       ├── zone209\piece02.jpg
│       ├── zone209\piece03.jpg
│       ├── zone209\piece04.jpg
│       ├── zone209\piece05.jpg
│       ├── zone209\piece06.jpg
│       ├── zone209\pqarea209.png
│       ├── zone209\terrain.png
│       ├── zone210\areas210.png
│       ├── zone210\influenceids.csv
│       ├── zone210\offset.png
│       ├── zone210\pqarea210.png
│       ├── zone210\terrain.png
│       ├── zone211\offset.png
│       ├── zone211\terrain.png
│       ├── zone212\offset.png
│       ├── zone212\terrain.png
│       ├── zone213\offset.png
│       ├── zone213\terrain.png
│       ├── zone214\offset.png
│       ├── zone214\terrain.png
│       ├── zone215\offset.png
│       ├── zone215\terrain.png
│       ├── zone216\offset.png
│       ├── zone216\terrain.png
│       ├── zone217\offset.png
│       ├── zone217\terrain.png
│       ├── zone218\offset.png
│       ├── zone218\terrain.png
│       ├── zone219\offset.png
│       ├── zone219\terrain.png
│       ├── zone220\areas220.png
│       ├── zone220\influenceids.csv
│       ├── zone220\mappieces.csv
│       ├── zone220\offset.png
│       ├── zone220\piece01.jpg
│       ├── zone220\piece02.jpg
│       ├── zone220\piece03.jpg
│       ├── zone220\piece04.jpg
│       ├── zone220\piece05.jpg
│       ├── zone220\pqarea220.png
│       ├── zone220\terrain.png
│       ├── zone221\offset.png
│       ├── zone221\terrain.png
│       ├── zone222\offset.png
│       ├── zone222\terrain.png
│       ├── zone223\offset.png
│       ├── zone223\terrain.png
│       ├── zone224\offset.png
│       ├── zone224\terrain.png
│       ├── zone225\offset.png
│       ├── zone225\terrain.png
│       ├── zone226\offset.png
│       ├── zone226\terrain.png
│       ├── zone227\offset.png
│       ├── zone227\terrain.png
│       ├── zone228\offset.png
│       ├── zone228\terrain.png
│       ├── zone229\offset.png
│       ├── zone229\terrain.png
│       ├── zone230\offset.png
│       ├── zone230\terrain.png
│       ├── zone231\offset.png
│       ├── zone231\terrain.png
│       ├── zone232\offset.png
│       ├── zone232\terrain.png
│       ├── zone234\offset.png
│       ├── zone234\terrain.png
│       ├── zone235\offset.png
│       ├── zone235\terrain.png
│       ├── zone236\offset.png
│       ├── zone236\terrain.png
│       ├── zone237\offset.png
│       ├── zone237\terrain.png
│       ├── zone238\offset.png
│       ├── zone238\terrain.png
│       ├── zone241\influenceids.csv
│       ├── zone241\offset.png
│       ├── zone241\terrain.png
│       ├── zone242\influenceids.csv
│       ├── zone242\offset.png
│       ├── zone242\terrain.png
│       ├── zone243\influenceids.csv
│       ├── zone243\offset.png
│       ├── zone243\terrain.png
│       ├── zone244\influenceids.csv
│       ├── zone244\offset.png
│       ├── zone244\terrain.png
│       ├── zone245\offset.png
│       ├── zone245\terrain.png
│       ├── zone246\offset.png
│       ├── zone246\terrain.png
│       ├── zone247\offset.png
│       ├── zone247\terrain.png
│       ├── zone248\offset.png
│       ├── zone248\terrain.png
│       ├── zone249\offset.png
│       ├── zone249\terrain.png
│       ├── zone260\offset.png
│       ├── zone260\terrain.png
│       ├── zone275\offset.png
│       ├── zone275\terrain.png
│       ├── zone276\offset.png
│       ├── zone276\terrain.png
│       ├── zone277\offset.png
│       ├── zone277\terrain.png
│       ├── zone278\offset.png
│       ├── zone278\terrain.png
│       ├── zone279\offset.png
│       ├── zone279\terrain.png
│       ├── zone280\offset.png
│       ├── zone280\terrain.png
│       ├── zone281\offset.png
│       ├── zone281\terrain.png
│       ├── zone282\offset.png
│       ├── zone282\terrain.png
│       ├── zone283\offset.png
│       ├── zone283\terrain.png
│       ├── zone284\offset.png
│       ├── zone284\terrain.png
│       ├── zone285\offset.png
│       ├── zone285\terrain.png
│       ├── zone286\offset.png
│       ├── zone286\terrain.png
│       ├── zone287\offset.png
│       ├── zone287\terrain.png
│       ├── zone288\offset.png
│       ├── zone288\terrain.png
│       ├── zone289\offset.png
│       ├── zone289\terrain.png
│       ├── zone290\offset.png
│       ├── zone290\terrain.png
│       ├── zone291\offset.png
│       ├── zone291\terrain.png
│       ├── zone292\offset.png
│       ├── zone292\terrain.png
│       ├── zone294\offset.png
│       ├── zone294\terrain.png
│       ├── zone295\offset.png
│       ├── zone295\terrain.png
│       ├── zone297\offset.png
│       ├── zone297\terrain.png
│       ├── zone298\offset.png
│       ├── zone298\terrain.png
│       ├── zone303\offset.png
│       ├── zone303\terrain.png
│       ├── zone304\offset.png
│       ├── zone304\terrain.png
│       ├── zone306\offset.png
│       ├── zone306\terrain.png
│       ├── zone307\offset.png
│       ├── zone307\terrain.png
│       ├── zone474\areas191.png
│       ├── zone474\influenceids.csv
│       ├── zone474\mappieces.csv
│       ├── zone474\offset.png
│       ├── zone474\piece01.jpg
│       ├── zone474\piece02.jpg
│       ├── zone474\piece03.jpg
│       ├── zone474\piece04.jpg
│       ├── zone474\piece05.jpg
│       ├── zone474\piece06.jpg
│       ├── zone474\pqarea191.png
│       ├── zone474\terrain.png
│       └── zoneinfo.txt
├── find.py
├── knowledge_base.md
├── packages/
│   ├── Appccelerate.StateMachine.4.4.0/
│   │   ├── .signature.p7s
│   │   ├── Appccelerate.StateMachine.4.4.0.nupkg
│   │   ├── lib/
│   │   │   └── netstandard1.0/
│   │   │       └── Appccelerate.StateMachine.xml
│   │   └── src/
│   │       ├── ActiveStateMachine.cs
│   │       ├── AsyncMachine/
│   │       │   ├── ActionHolders/
│   │       │   │   ├── ActionHoldersExceptionMessages.cs
│   │       │   │   ├── ArgumentActionHolder.cs
│   │       │   │   ├── ArgumentLessActionHolder.cs
│   │       │   │   ├── IActionHolder.cs
│   │       │   │   └── ParametrizedActionHolder{T}.cs
│   │       │   ├── AsyncExtensionBase.cs
│   │       │   ├── Contexts/
│   │       │   │   └── TransitionContext.cs
│   │       │   ├── Events/
│   │       │   │   ├── ContextEventArgs.cs
│   │       │   │   ├── IFactory.cs
│   │       │   │   ├── TransitionCompletedEventArgs.cs
│   │       │   │   ├── TransitionEventArgs.cs
│   │       │   │   └── TransitionExceptionEventArgs.cs
│   │       │   ├── ExceptionMessages.cs
│   │       │   ├── GuardHolders/
│   │       │   │   ├── ArgumentGuardHolder.cs
│   │       │   │   ├── ArgumentLessGuardHolder.cs
│   │       │   │   ├── GuardHoldersExceptionMessages.cs
│   │       │   │   └── IGuardHolder.cs
│   │       │   ├── HierarchyBuilder.cs
│   │       │   ├── IExtension.cs
│   │       │   ├── IExtensionHost.cs
│   │       │   ├── INotifier.cs
│   │       │   ├── IState.cs
│   │       │   ├── IStateDictionary.cs
│   │       │   ├── IStateMachineReport.cs
│   │       │   ├── ITransition.cs
│   │       │   ├── ITransitionContext.cs
│   │       │   ├── ITransitionDictionary.cs
│   │       │   ├── ITransitionResult.cs
│   │       │   ├── Missable.cs
│   │       │   ├── Missing.cs
│   │       │   ├── RecordType.cs
│   │       │   ├── StandardFactory.cs
│   │       │   ├── StateBuilder.g.cs
│   │       │   ├── StateDictionary.cs
│   │       │   ├── StateMachine.cs
│   │       │   ├── StateMachineException.cs
│   │       │   ├── StateMachineInitializer.cs
│   │       │   ├── States/
│   │       │   │   ├── State.cs
│   │       │   │   └── StatesExceptionMessages.cs
│   │       │   └── Transitions/
│   │       │       ├── Transition.cs
│   │       │       ├── TransitionDictionary.cs
│   │       │       ├── TransitionInfo.cs
│   │       │       ├── TransitionResult.cs
│   │       │       └── TransitionsExceptionMessages.cs
│   │       ├── AsyncPassiveStateMachine.cs
│   │       ├── AsyncSyntax/
│   │       │   ├── IEntryActionSyntax.cs
│   │       │   ├── IEventSyntax.cs
│   │       │   ├── IExitActionSyntax.cs
│   │       │   ├── IGotoInIfSyntax.cs
│   │       │   ├── IGotoSyntax.cs
│   │       │   ├── IHierarchySyntax.cs
│   │       │   ├── IIfOrOtherwiseSyntax.cs
│   │       │   ├── IIfSyntax.cs
│   │       │   ├── IOnSyntax.cs
│   │       │   └── IOtherwiseSyntax.cs
│   │       ├── EventInformation.cs
│   │       ├── Extensions/
│   │       │   └── ExtensionBase.cs
│   │       ├── Guard.cs
│   │       ├── HistoryType.cs
│   │       ├── IAsyncStateMachine.cs
│   │       ├── IStateMachine.cs
│   │       ├── IStateMachineInformation.cs
│   │       ├── Infrastructure/
│   │       │   └── Initializable.cs
│   │       ├── ListExtensionMethods.cs
│   │       ├── Machine/
│   │       │   ├── ActionHolders/
│   │       │   │   ├── ActionHoldersExceptionMessages.cs
│   │       │   │   ├── ArgumentActionHolder.cs
│   │       │   │   ├── ArgumentLessActionHolder.cs
│   │       │   │   ├── IActionHolder.cs
│   │       │   │   └── ParametrizedActionHolder{T}.cs
│   │       │   ├── Contexts/
│   │       │   │   └── TransitionContext.cs
│   │       │   ├── Events/
│   │       │   │   ├── ContextEventArgs.cs
│   │       │   │   ├── IFactory.cs
│   │       │   │   ├── TransitionCompletedEventArgs.cs
│   │       │   │   ├── TransitionEventArgs.cs
│   │       │   │   └── TransitionExceptionEventArgs.cs
│   │       │   ├── ExceptionMessages.cs
│   │       │   ├── GuardHolders/
│   │       │   │   ├── ArgumentGuardHolder.cs
│   │       │   │   ├── ArgumentLessGuardHolder.cs
│   │       │   │   ├── GuardHoldersExceptionMessages.cs
│   │       │   │   └── IGuardHolder.cs
│   │       │   ├── HierarchyBuilder.cs
│   │       │   ├── IExtension.cs
│   │       │   ├── IExtensionHost.cs
│   │       │   ├── INotifier.cs
│   │       │   ├── IState.cs
│   │       │   ├── IStateDictionary.cs
│   │       │   ├── IStateMachineReport.cs
│   │       │   ├── ITransition.cs
│   │       │   ├── ITransitionContext.cs
│   │       │   ├── ITransitionDictionary.cs
│   │       │   ├── ITransitionResult.cs
│   │       │   ├── Missable.cs
│   │       │   ├── Missing.cs
│   │       │   ├── RecordType.cs
│   │       │   ├── StandardFactory.cs
│   │       │   ├── StateBuilder.g.cs
│   │       │   ├── StateDictionary.cs
│   │       │   ├── StateMachine.cs
│   │       │   ├── StateMachineException.cs
│   │       │   ├── StateMachineInitializer.cs
│   │       │   ├── States/
│   │       │   │   ├── State.cs
│   │       │   │   └── StatesExceptionMessages.cs
│   │       │   └── Transitions/
│   │       │       ├── Transition.cs
│   │       │       ├── TransitionDictionary.cs
│   │       │       ├── TransitionInfo.cs
│   │       │       ├── TransitionResult.cs
│   │       │       └── TransitionsExceptionMessages.cs
│   │       ├── PassiveStateMachine.cs
│   │       ├── Persistence/
│   │       │   ├── IAsyncStateMachineLoader.cs
│   │       │   ├── IAsyncStateMachineSaver.cs
│   │       │   ├── IStateMachineLoader.cs
│   │       │   └── IStateMachineSaver.cs
│   │       ├── Reports/
│   │       │   ├── CsvStateMachineReportGenerator.cs
│   │       │   ├── CsvStatesWriter.cs
│   │       │   ├── CsvTransitionsWriter.cs
│   │       │   ├── StateMachineReportGenerator.cs
│   │       │   └── YEdStateMachineReportGenerator.cs
│   │       ├── Syntax/
│   │       │   ├── IEntryActionSyntax.cs
│   │       │   ├── IEventSyntax.cs
│   │       │   ├── IExitActionSyntax.cs
│   │       │   ├── IGotoInIfSyntax.cs
│   │       │   ├── IGotoSyntax.cs
│   │       │   ├── IHierarchySyntax.cs
│   │       │   ├── IIfOrOtherwiseSyntax.cs
│   │       │   ├── IIfSyntax.cs
│   │       │   ├── IOnSyntax.cs
│   │       │   └── IOtherwiseSyntax.cs
│   │       └── TypeExtensionMethods.cs
│   ├── Appccelerate.StateMachine.6.0.0/
│   │   ├── .signature.p7s
│   │   ├── Appccelerate.StateMachine.6.0.0.nupkg
│   │   ├── LICENSE
│   │   ├── lib/
│   │   │   └── netstandard2.0/
│   │   │       └── Appccelerate.StateMachine.xml
│   │   └── nuget.png
│   ├── BehaviourTree.1.0.73/
│   │   ├── .signature.p7s
│   │   ├── BehaviourTree.1.0.73.nupkg
│   │   └── lib/
│   │       └── netstandard1.0/
│   ├── BehaviourTree.FluentBuilder.1.0.70/
│   │   ├── .signature.p7s
│   │   ├── BehaviourTree.FluentBuilder.1.0.70.nupkg
│   │   └── lib/
│   │       └── netstandard1.0/
│   ├── BouncyCastle.Cryptography.2.2.1/
│   │   ├── .signature.p7s
│   │   ├── BouncyCastle.Cryptography.2.2.1.nupkg
│   │   ├── LICENSE.md
│   │   ├── README.md
│   │   ├── lib/
│   │   │   ├── net461/
│   │   │   │   └── BouncyCastle.Cryptography.xml
│   │   │   ├── net6.0/
│   │   │   │   └── BouncyCastle.Cryptography.xml
│   │   │   └── netstandard2.0/
│   │   │       └── BouncyCastle.Cryptography.xml
│   │   └── packageIcon.png
│   ├── BouncyCastle.Cryptography.2.6.2/
│   │   ├── .signature.p7s
│   │   ├── BouncyCastle.Cryptography.2.6.2.nupkg
│   │   ├── LICENSE.md
│   │   ├── README.md
│   │   ├── lib/
│   │   │   ├── net461/
│   │   │   │   └── BouncyCastle.Cryptography.xml
│   │   │   ├── net6.0/
│   │   │   │   └── BouncyCastle.Cryptography.xml
│   │   │   └── netstandard2.0/
│   │   │       └── BouncyCastle.Cryptography.xml
│   │   └── packageIcon.png
│   ├── BulletSharp.0.11.1/
│   │   ├── .signature.p7s
│   │   ├── BulletSharp.0.11.1.nupkg
│   │   └── lib/
│   │       └── net40-client/
│   ├── Castle.Core.5.2.1/
│   │   ├── .signature.p7s
│   │   ├── ASL - Apache Software Foundation License.txt
│   │   ├── CHANGELOG.md
│   │   ├── Castle.Core.5.2.1.nupkg
│   │   ├── LICENSE
│   │   ├── castle-logo.png
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── Castle.Core.xml
│   │   │   ├── net6.0/
│   │   │   │   └── Castle.Core.xml
│   │   │   ├── netstandard2.0/
│   │   │   │   └── Castle.Core.xml
│   │   │   └── netstandard2.1/
│   │   │       └── Castle.Core.xml
│   │   └── readme.txt
│   ├── Evolve.3.1.0/
│   │   ├── .signature.p7s
│   │   ├── Evolve.3.1.0.nupkg
│   │   └── lib/
│   │       └── netstandard2.0/
│   │           └── Evolve.xml
│   ├── Evolve.3.2.0/
│   │   ├── .signature.p7s
│   │   ├── Evolve.3.2.0.nupkg
│   │   └── lib/
│   │       └── netstandard2.0/
│   │           └── Evolve.xml
│   ├── FakeItEasy.7.4.0/
│   │   ├── .signature.p7s
│   │   ├── FakeItEasy.7.4.0.nupkg
│   │   ├── images/
│   │   │   └── FakeItEasy.png
│   │   └── lib/
│   │       ├── net45/
│   │       │   └── FakeItEasy.xml
│   │       ├── net5.0/
│   │       │   └── FakeItEasy.xml
│   │       ├── netstandard2.0/
│   │       │   └── FakeItEasy.xml
│   │       └── netstandard2.1/
│   │           └── FakeItEasy.xml
│   ├── FakeItEasy.8.3.0/
│   │   ├── .signature.p7s
│   │   ├── FakeItEasy.8.3.0.nupkg
│   │   ├── README.md
│   │   ├── images/
│   │   │   └── FakeItEasy.png
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── FakeItEasy.xml
│   │       ├── net6.0/
│   │       │   └── FakeItEasy.xml
│   │       ├── net8.0/
│   │       │   └── FakeItEasy.xml
│   │       ├── netstandard2.0/
│   │       │   └── FakeItEasy.xml
│   │       └── netstandard2.1/
│   │           └── FakeItEasy.xml
│   ├── Google.Protobuf.3.23.3/
│   │   ├── .signature.p7s
│   │   ├── Google.Protobuf.3.23.3.nupkg
│   │   └── lib/
│   │       ├── net45/
│   │       │   └── Google.Protobuf.xml
│   │       ├── net5.0/
│   │       │   └── Google.Protobuf.xml
│   │       ├── netstandard1.1/
│   │       │   └── Google.Protobuf.xml
│   │       └── netstandard2.0/
│   │           └── Google.Protobuf.xml
│   ├── Google.Protobuf.3.32.0/
│   │   ├── .signature.p7s
│   │   ├── Google.Protobuf.3.32.0.nupkg
│   │   └── lib/
│   │       ├── net45/
│   │       │   └── Google.Protobuf.xml
│   │       ├── net5.0/
│   │       │   └── Google.Protobuf.xml
│   │       ├── netstandard1.1/
│   │       │   └── Google.Protobuf.xml
│   │       └── netstandard2.0/
│   │           └── Google.Protobuf.xml
│   ├── K4os.Compression.LZ4.1.3.5/
│   │   ├── .signature.p7s
│   │   ├── K4os.Compression.LZ4.1.3.5.nupkg
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── K4os.Compression.LZ4.xml
│   │       ├── net5.0/
│   │       │   └── K4os.Compression.LZ4.xml
│   │       ├── net6.0/
│   │       │   └── K4os.Compression.LZ4.xml
│   │       ├── netstandard2.0/
│   │       │   └── K4os.Compression.LZ4.xml
│   │       └── netstandard2.1/
│   │           └── K4os.Compression.LZ4.xml
│   ├── K4os.Compression.LZ4.1.3.8/
│   │   ├── .signature.p7s
│   │   ├── K4os.Compression.LZ4.1.3.8.nupkg
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── K4os.Compression.LZ4.xml
│   │       ├── net5.0/
│   │       │   └── K4os.Compression.LZ4.xml
│   │       ├── net6.0/
│   │       │   └── K4os.Compression.LZ4.xml
│   │       ├── netstandard2.0/
│   │       │   └── K4os.Compression.LZ4.xml
│   │       └── netstandard2.1/
│   │           └── K4os.Compression.LZ4.xml
│   ├── K4os.Compression.LZ4.Streams.1.3.5/
│   │   ├── .signature.p7s
│   │   ├── K4os.Compression.LZ4.Streams.1.3.5.nupkg
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── K4os.Compression.LZ4.Streams.xml
│   │       ├── net5.0/
│   │       │   └── K4os.Compression.LZ4.Streams.xml
│   │       ├── net6.0/
│   │       │   └── K4os.Compression.LZ4.Streams.xml
│   │       ├── netstandard2.0/
│   │       │   └── K4os.Compression.LZ4.Streams.xml
│   │       └── netstandard2.1/
│   │           └── K4os.Compression.LZ4.Streams.xml
│   ├── K4os.Compression.LZ4.Streams.1.3.8/
│   │   ├── .signature.p7s
│   │   ├── K4os.Compression.LZ4.Streams.1.3.8.nupkg
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── K4os.Compression.LZ4.Streams.xml
│   │       ├── net5.0/
│   │       │   └── K4os.Compression.LZ4.Streams.xml
│   │       ├── net6.0/
│   │       │   └── K4os.Compression.LZ4.Streams.xml
│   │       ├── netstandard2.0/
│   │       │   └── K4os.Compression.LZ4.Streams.xml
│   │       └── netstandard2.1/
│   │           └── K4os.Compression.LZ4.Streams.xml
│   ├── K4os.Hash.xxHash.1.0.8/
│   │   ├── .signature.p7s
│   │   ├── K4os.Hash.xxHash.1.0.8.nupkg
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── K4os.Hash.xxHash.xml
│   │       ├── net5.0/
│   │       │   └── K4os.Hash.xxHash.xml
│   │       ├── net6.0/
│   │       │   └── K4os.Hash.xxHash.xml
│   │       ├── netstandard2.0/
│   │       │   └── K4os.Hash.xxHash.xml
│   │       └── netstandard2.1/
│   │           └── K4os.Hash.xxHash.xml
│   ├── Microsoft.Bcl.AsyncInterfaces.7.0.0/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── Microsoft.Bcl.AsyncInterfaces.7.0.0.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── Microsoft.Bcl.AsyncInterfaces.targets
│   │   │   └── net462/
│   │   │       └── _._
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── Microsoft.Bcl.AsyncInterfaces.xml
│   │   │   ├── netstandard2.0/
│   │   │   │   └── Microsoft.Bcl.AsyncInterfaces.xml
│   │   │   └── netstandard2.1/
│   │   │       └── Microsoft.Bcl.AsyncInterfaces.xml
│   │   └── useSharedDesignerContext.txt
│   ├── Microsoft.Bcl.AsyncInterfaces.9.0.8/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── Microsoft.Bcl.AsyncInterfaces.9.0.8.nupkg
│   │   ├── PACKAGE.md
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── Microsoft.Bcl.AsyncInterfaces.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net8.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── Microsoft.Bcl.AsyncInterfaces.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── Microsoft.Bcl.AsyncInterfaces.xml
│   │   │   ├── netstandard2.0/
│   │   │   │   └── Microsoft.Bcl.AsyncInterfaces.xml
│   │   │   └── netstandard2.1/
│   │   │       └── Microsoft.Bcl.AsyncInterfaces.xml
│   │   └── useSharedDesignerContext.txt
│   ├── Microsoft.CSharp.4.7.0/
│   │   ├── .signature.p7s
│   │   ├── LICENSE.TXT
│   │   ├── Microsoft.CSharp.4.7.0.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── netcore50/
│   │   │   ├── netcoreapp2.0/
│   │   │   │   └── _._
│   │   │   ├── netstandard1.3/
│   │   │   ├── netstandard2.0/
│   │   │   │   └── Microsoft.CSharp.xml
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── uap10.0.16299/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   ├── ref/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── netcore50/
│   │   │   │   ├── Microsoft.CSharp.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── Microsoft.CSharp.xml
│   │   │   ├── netcoreapp2.0/
│   │   │   │   └── _._
│   │   │   ├── netstandard1.0/
│   │   │   │   ├── Microsoft.CSharp.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── Microsoft.CSharp.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── Microsoft.CSharp.xml
│   │   │   ├── netstandard2.0/
│   │   │   │   └── Microsoft.CSharp.xml
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── uap10.0.16299/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   ├── useSharedDesignerContext.txt
│   │   └── version.txt
│   ├── Microsoft.DotNet.PlatformAbstractions.3.1.6/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── Microsoft.DotNet.PlatformAbstractions.3.1.6.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   └── lib/
│   │       ├── net45/
│   │       │   └── Microsoft.DotNet.PlatformAbstractions.xml
│   │       ├── netstandard1.3/
│   │       │   └── Microsoft.DotNet.PlatformAbstractions.xml
│   │       └── netstandard2.0/
│   │           └── Microsoft.DotNet.PlatformAbstractions.xml
│   ├── Microsoft.Extensions.DependencyInjection.Abstractions.9.0.8/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── Microsoft.Extensions.DependencyInjection.Abstractions.9.0.8.nupkg
│   │   ├── PACKAGE.md
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── Microsoft.Extensions.DependencyInjection.Abstractions.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net8.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── Microsoft.Extensions.DependencyInjection.Abstractions.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── Microsoft.Extensions.DependencyInjection.Abstractions.xml
│   │   │   ├── net8.0/
│   │   │   │   └── Microsoft.Extensions.DependencyInjection.Abstractions.xml
│   │   │   ├── net9.0/
│   │   │   │   └── Microsoft.Extensions.DependencyInjection.Abstractions.xml
│   │   │   ├── netstandard2.0/
│   │   │   │   └── Microsoft.Extensions.DependencyInjection.Abstractions.xml
│   │   │   └── netstandard2.1/
│   │   │       └── Microsoft.Extensions.DependencyInjection.Abstractions.xml
│   │   └── useSharedDesignerContext.txt
│   ├── Microsoft.Extensions.DependencyModel.7.0.0/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── Microsoft.Extensions.DependencyModel.7.0.0.nupkg
│   │   ├── README.md
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── Microsoft.Extensions.DependencyModel.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net6.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── Microsoft.Extensions.DependencyModel.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── Microsoft.Extensions.DependencyModel.xml
│   │   │   ├── net6.0/
│   │   │   │   └── Microsoft.Extensions.DependencyModel.xml
│   │   │   ├── net7.0/
│   │   │   │   └── Microsoft.Extensions.DependencyModel.xml
│   │   │   └── netstandard2.0/
│   │   │       └── Microsoft.Extensions.DependencyModel.xml
│   │   └── useSharedDesignerContext.txt
│   ├── Microsoft.Extensions.DependencyModel.9.0.8/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── Microsoft.Extensions.DependencyModel.9.0.8.nupkg
│   │   ├── PACKAGE.md
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── Microsoft.Extensions.DependencyModel.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net8.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── Microsoft.Extensions.DependencyModel.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── Microsoft.Extensions.DependencyModel.xml
│   │   │   ├── net8.0/
│   │   │   │   └── Microsoft.Extensions.DependencyModel.xml
│   │   │   ├── net9.0/
│   │   │   │   └── Microsoft.Extensions.DependencyModel.xml
│   │   │   └── netstandard2.0/
│   │   │       └── Microsoft.Extensions.DependencyModel.xml
│   │   └── useSharedDesignerContext.txt
│   ├── Microsoft.Extensions.Logging.Abstractions.9.0.8/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── Microsoft.Extensions.Logging.Abstractions.9.0.8.nupkg
│   │   ├── PACKAGE.md
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── analyzers/
│   │   │   └── dotnet/
│   │   │       ├── roslyn3.11/
│   │   │       │   └── cs/
│   │   │       │       ├── cs/
│   │   │       │       ├── de/
│   │   │       │       ├── es/
│   │   │       │       ├── fr/
│   │   │       │       ├── it/
│   │   │       │       ├── ja/
│   │   │       │       ├── ko/
│   │   │       │       ├── pl/
│   │   │       │       ├── pt-BR/
│   │   │       │       ├── ru/
│   │   │       │       ├── tr/
│   │   │       │       ├── zh-Hans/
│   │   │       │       └── zh-Hant/
│   │   │       ├── roslyn4.0/
│   │   │       │   └── cs/
│   │   │       │       ├── cs/
│   │   │       │       ├── de/
│   │   │       │       ├── es/
│   │   │       │       ├── fr/
│   │   │       │       ├── it/
│   │   │       │       ├── ja/
│   │   │       │       ├── ko/
│   │   │       │       ├── pl/
│   │   │       │       ├── pt-BR/
│   │   │       │       ├── ru/
│   │   │       │       ├── tr/
│   │   │       │       ├── zh-Hans/
│   │   │       │       └── zh-Hant/
│   │   │       └── roslyn4.4/
│   │   │           └── cs/
│   │   │               ├── cs/
│   │   │               ├── de/
│   │   │               ├── es/
│   │   │               ├── fr/
│   │   │               ├── it/
│   │   │               ├── ja/
│   │   │               ├── ko/
│   │   │               ├── pl/
│   │   │               ├── pt-BR/
│   │   │               ├── ru/
│   │   │               ├── tr/
│   │   │               ├── zh-Hans/
│   │   │               └── zh-Hant/
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── Microsoft.Extensions.Logging.Abstractions.targets
│   │   │   ├── net462/
│   │   │   │   └── Microsoft.Extensions.Logging.Abstractions.targets
│   │   │   ├── net8.0/
│   │   │   │   └── Microsoft.Extensions.Logging.Abstractions.targets
│   │   │   ├── netcoreapp2.0/
│   │   │   │   └── Microsoft.Extensions.Logging.Abstractions.targets
│   │   │   └── netstandard2.0/
│   │   │       └── Microsoft.Extensions.Logging.Abstractions.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── Microsoft.Extensions.Logging.Abstractions.xml
│   │   │   ├── net8.0/
│   │   │   │   └── Microsoft.Extensions.Logging.Abstractions.xml
│   │   │   ├── net9.0/
│   │   │   │   └── Microsoft.Extensions.Logging.Abstractions.xml
│   │   │   └── netstandard2.0/
│   │   │       └── Microsoft.Extensions.Logging.Abstractions.xml
│   │   └── useSharedDesignerContext.txt
│   ├── Microsoft.NETCore.Platforms.7.0.4/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── Microsoft.NETCore.Platforms.7.0.4.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── lib/
│   │   │   └── netstandard1.0/
│   │   │       └── _._
│   │   ├── runtime.json
│   │   └── useSharedDesignerContext.txt
│   ├── Microsoft.Win32.Primitives.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── Microsoft.Win32.Primitives.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net46/
│   │       ├── netstandard1.3/
│   │       │   ├── Microsoft.Win32.Primitives.xml
│   │       │   ├── de/
│   │       │   │   └── Microsoft.Win32.Primitives.xml
│   │       │   ├── es/
│   │       │   │   └── Microsoft.Win32.Primitives.xml
│   │       │   ├── fr/
│   │       │   │   └── Microsoft.Win32.Primitives.xml
│   │       │   ├── it/
│   │       │   │   └── Microsoft.Win32.Primitives.xml
│   │       │   ├── ja/
│   │       │   │   └── Microsoft.Win32.Primitives.xml
│   │       │   ├── ko/
│   │       │   │   └── Microsoft.Win32.Primitives.xml
│   │       │   ├── ru/
│   │       │   │   └── Microsoft.Win32.Primitives.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── Microsoft.Win32.Primitives.xml
│   │       │   └── zh-hant/
│   │       │       └── Microsoft.Win32.Primitives.xml
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── MySql.Data.8.0.33/
│   │   ├── .signature.p7s
│   │   ├── MySql.Data.8.0.33.nupkg
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── MySql.Data.xml
│   │   │   ├── net48/
│   │   │   │   └── MySql.Data.xml
│   │   │   ├── net6.0/
│   │   │   │   └── MySql.Data.xml
│   │   │   ├── net7.0/
│   │   │   │   └── MySql.Data.xml
│   │   │   ├── netstandard2.0/
│   │   │   │   └── MySql.Data.xml
│   │   │   └── netstandard2.1/
│   │   │       └── MySql.Data.xml
│   │   ├── logo-mysql-170x115.png
│   │   └── runtimes/
│   │       └── win-x64/
│   │           └── native/
│   ├── MySql.Data.9.4.0/
│   │   ├── .signature.p7s
│   │   ├── LICENSE
│   │   ├── MySql.Data.9.4.0.nupkg
│   │   ├── README
│   │   ├── README.md
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── MySql.Data.xml
│   │   │   ├── net48/
│   │   │   │   └── MySql.Data.xml
│   │   │   ├── net8.0/
│   │   │   │   └── MySql.Data.xml
│   │   │   ├── net9.0/
│   │   │   │   └── MySql.Data.xml
│   │   │   ├── netstandard2.0/
│   │   │   │   └── MySql.Data.xml
│   │   │   └── netstandard2.1/
│   │   │       └── MySql.Data.xml
│   │   ├── logo-mysql-170x115.png
│   │   └── runtimes/
│   │       └── win-x64/
│   │           └── native/
│   ├── NETStandard.Library.2.0.3/
│   │   ├── .signature.p7s
│   │   ├── LICENSE.TXT
│   │   ├── NETStandard.Library.2.0.3.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   └── lib/
│   │       └── netstandard1.0/
│   │           └── _._
│   ├── NLog.5.2.0/
│   │   ├── .signature.p7s
│   │   ├── N.png
│   │   ├── NLog.5.2.0.nupkg
│   │   └── lib/
│   │       ├── net35/
│   │       │   └── NLog.xml
│   │       ├── net45/
│   │       │   └── NLog.xml
│   │       ├── net46/
│   │       │   └── NLog.xml
│   │       ├── netstandard1.3/
│   │       │   └── NLog.xml
│   │       ├── netstandard1.5/
│   │       │   └── NLog.xml
│   │       └── netstandard2.0/
│   │           └── NLog.xml
│   ├── NLog.6.0.3/
│   │   ├── .signature.p7s
│   │   ├── N.png
│   │   ├── NLog.6.0.3.nupkg
│   │   └── lib/
│   │       ├── net35/
│   │       │   └── NLog.xml
│   │       ├── net45/
│   │       │   └── NLog.xml
│   │       ├── net46/
│   │       │   └── NLog.xml
│   │       ├── netstandard2.0/
│   │       │   └── NLog.xml
│   │       └── netstandard2.1/
│   │           └── NLog.xml
│   ├── Newtonsoft.Json.13.0.3/
│   │   ├── .signature.p7s
│   │   ├── LICENSE.md
│   │   ├── Newtonsoft.Json.13.0.3.nupkg
│   │   ├── README.md
│   │   ├── lib/
│   │   │   ├── net20/
│   │   │   │   └── Newtonsoft.Json.xml
│   │   │   ├── net35/
│   │   │   │   └── Newtonsoft.Json.xml
│   │   │   ├── net40/
│   │   │   │   └── Newtonsoft.Json.xml
│   │   │   ├── net45/
│   │   │   │   └── Newtonsoft.Json.xml
│   │   │   ├── net6.0/
│   │   │   │   └── Newtonsoft.Json.xml
│   │   │   ├── netstandard1.0/
│   │   │   │   └── Newtonsoft.Json.xml
│   │   │   ├── netstandard1.3/
│   │   │   │   └── Newtonsoft.Json.xml
│   │   │   └── netstandard2.0/
│   │   │       └── Newtonsoft.Json.xml
│   │   └── packageIcon.png
│   ├── Portable.BouncyCastle.1.9.0/
│   │   ├── .signature.p7s
│   │   ├── Portable.BouncyCastle.1.9.0.nupkg
│   │   └── lib/
│   │       ├── net40/
│   │       │   └── BouncyCastle.Crypto.xml
│   │       └── netstandard2.0/
│   │           └── BouncyCastle.Crypto.xml
│   ├── SSH.NET.2020.0.2/
│   │   ├── .signature.p7s
│   │   ├── SSH.NET.2020.0.2.nupkg
│   │   └── lib/
│   │       ├── net35/
│   │       │   └── Renci.SshNet.xml
│   │       ├── net40/
│   │       │   └── Renci.SshNet.xml
│   │       ├── netstandard1.3/
│   │       │   └── Renci.SshNet.xml
│   │       ├── netstandard2.0/
│   │       │   └── Renci.SshNet.xml
│   │       ├── sl4/
│   │       │   └── Renci.SshNet.xml
│   │       ├── sl5/
│   │       │   └── Renci.SshNet.xml
│   │       ├── uap10/
│   │       │   └── Renci.SshNet.xml
│   │       ├── wp71/
│   │       │   └── Renci.SshNet.xml
│   │       └── wp8/
│   │           └── Renci.SshNet.xml
│   ├── SSH.NET.2025.0.0/
│   │   ├── .signature.p7s
│   │   ├── README.md
│   │   ├── SS-NET-icon-h500.png
│   │   ├── SSH.NET.2025.0.0.nupkg
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── Renci.SshNet.xml
│   │       ├── net8.0/
│   │       │   └── Renci.SshNet.xml
│   │       ├── net9.0/
│   │       │   └── Renci.SshNet.xml
│   │       ├── netstandard2.0/
│   │       │   └── Renci.SshNet.xml
│   │       └── netstandard2.1/
│   │           └── Renci.SshNet.xml
│   ├── System.AppContext.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.AppContext.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── net463/
│   │   │   ├── netcore50/
│   │   │   ├── netstandard1.6/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   ├── ref/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── net463/
│   │   │   ├── netstandard/
│   │   │   │   └── _._
│   │   │   ├── netstandard1.3/
│   │   │   │   ├── System.AppContext.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.AppContext.xml
│   │   │   ├── netstandard1.6/
│   │   │   │   ├── System.AppContext.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.AppContext.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.AppContext.xml
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── runtimes/
│   │       └── aot/
│   │           └── lib/
│   │               └── netcore50/
│   ├── System.Buffers.4.6.1/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── PACKAGE.md
│   │   ├── System.Buffers.4.6.1.nupkg
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Buffers.targets
│   │   │   └── net462/
│   │   │       └── _._
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── System.Buffers.xml
│   │       ├── netcoreapp2.0/
│   │       │   └── _._
│   │       ├── netstandard2.0/
│   │       │   └── System.Buffers.xml
│   │       └── netstandard2.1/
│   │           └── _._
│   ├── System.Collections.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Collections.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Collections.xml
│   │       │   ├── de/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── es/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── it/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Collections.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Collections.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Collections.xml
│   │       │   ├── de/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── es/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── it/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Collections.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Collections.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Collections.xml
│   │       │   ├── de/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── es/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── it/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Collections.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Collections.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Collections.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Collections.Concurrent.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Collections.Concurrent.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── netcore50/
│   │   │   ├── netstandard1.3/
│   │   │   ├── portable-net45+win8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Collections.Concurrent.xml
│   │       │   ├── de/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── es/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── it/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Collections.Concurrent.xml
│   │       ├── netstandard1.1/
│   │       │   ├── System.Collections.Concurrent.xml
│   │       │   ├── de/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── es/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── it/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Collections.Concurrent.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Collections.Concurrent.xml
│   │       │   ├── de/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── es/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── it/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Collections.Concurrent.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Collections.Concurrent.xml
│   │       ├── portable-net45+win8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Configuration.ConfigurationManager.9.0.8/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── PACKAGE.md
│   │   ├── System.Configuration.ConfigurationManager.9.0.8.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Configuration.ConfigurationManager.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net8.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── System.Configuration.ConfigurationManager.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── System.Configuration.ConfigurationManager.xml
│   │   │   ├── net8.0/
│   │   │   │   └── System.Configuration.ConfigurationManager.xml
│   │   │   ├── net9.0/
│   │   │   │   └── System.Configuration.ConfigurationManager.xml
│   │   │   └── netstandard2.0/
│   │   │       └── System.Configuration.ConfigurationManager.xml
│   │   └── useSharedDesignerContext.txt
│   ├── System.Console.4.3.1/
│   │   ├── .signature.p7s
│   │   ├── System.Console.4.3.1.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net46/
│   │       ├── netstandard1.3/
│   │       │   ├── System.Console.xml
│   │       │   ├── de/
│   │       │   │   └── System.Console.xml
│   │       │   ├── es/
│   │       │   │   └── System.Console.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Console.xml
│   │       │   ├── it/
│   │       │   │   └── System.Console.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Console.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Console.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Console.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Console.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Console.xml
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Diagnostics.Debug.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Diagnostics.Debug.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Diagnostics.Debug.xml
│   │       │   ├── de/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── es/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── it/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Diagnostics.Debug.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Diagnostics.Debug.xml
│   │       │   ├── de/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── es/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── it/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Diagnostics.Debug.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Diagnostics.Debug.xml
│   │       │   ├── de/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── es/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── it/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Diagnostics.Debug.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Diagnostics.Debug.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Diagnostics.DiagnosticSource.9.0.8/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── System.Diagnostics.DiagnosticSource.9.0.8.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Diagnostics.DiagnosticSource.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net8.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── System.Diagnostics.DiagnosticSource.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── System.Diagnostics.DiagnosticSource.xml
│   │   │   ├── net8.0/
│   │   │   │   └── System.Diagnostics.DiagnosticSource.xml
│   │   │   ├── net9.0/
│   │   │   │   └── System.Diagnostics.DiagnosticSource.xml
│   │   │   └── netstandard2.0/
│   │   │       └── System.Diagnostics.DiagnosticSource.xml
│   │   └── useSharedDesignerContext.txt
│   ├── System.Diagnostics.Tools.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Diagnostics.Tools.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Diagnostics.Tools.xml
│   │       │   ├── de/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── es/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── it/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Diagnostics.Tools.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Diagnostics.Tools.xml
│   │       │   ├── de/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── es/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── it/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Diagnostics.Tools.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Diagnostics.Tools.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Diagnostics.Tracing.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Diagnostics.Tracing.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net462/
│   │   │   ├── portable-net45+win8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── net462/
│   │       ├── netcore50/
│   │       │   ├── System.Diagnostics.Tracing.xml
│   │       │   ├── de/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── es/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── it/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Diagnostics.Tracing.xml
│   │       ├── netstandard1.1/
│   │       │   ├── System.Diagnostics.Tracing.xml
│   │       │   ├── de/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── es/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── it/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Diagnostics.Tracing.xml
│   │       ├── netstandard1.2/
│   │       │   ├── System.Diagnostics.Tracing.xml
│   │       │   ├── de/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── es/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── it/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Diagnostics.Tracing.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Diagnostics.Tracing.xml
│   │       │   ├── de/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── es/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── it/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Diagnostics.Tracing.xml
│   │       ├── netstandard1.5/
│   │       │   ├── System.Diagnostics.Tracing.xml
│   │       │   ├── de/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── es/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── it/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Diagnostics.Tracing.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Diagnostics.Tracing.xml
│   │       ├── portable-net45+win8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Formats.Asn1.9.0.8/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── PACKAGE.md
│   │   ├── System.Formats.Asn1.9.0.8.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Formats.Asn1.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net8.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── System.Formats.Asn1.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── System.Formats.Asn1.xml
│   │   │   ├── net8.0/
│   │   │   │   └── System.Formats.Asn1.xml
│   │   │   ├── net9.0/
│   │   │   │   └── System.Formats.Asn1.xml
│   │   │   └── netstandard2.0/
│   │   │       └── System.Formats.Asn1.xml
│   │   └── useSharedDesignerContext.txt
│   ├── System.Globalization.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Globalization.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Globalization.xml
│   │       │   ├── de/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── es/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── it/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Globalization.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Globalization.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Globalization.xml
│   │       │   ├── de/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── es/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── it/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Globalization.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Globalization.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Globalization.xml
│   │       │   ├── de/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── es/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── it/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Globalization.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Globalization.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Globalization.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Globalization.Calendars.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Globalization.Calendars.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net46/
│   │       ├── netstandard1.3/
│   │       │   ├── System.Globalization.Calendars.xml
│   │       │   ├── de/
│   │       │   │   └── System.Globalization.Calendars.xml
│   │       │   ├── es/
│   │       │   │   └── System.Globalization.Calendars.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Globalization.Calendars.xml
│   │       │   ├── it/
│   │       │   │   └── System.Globalization.Calendars.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Globalization.Calendars.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Globalization.Calendars.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Globalization.Calendars.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Globalization.Calendars.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Globalization.Calendars.xml
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.IO.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.IO.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net462/
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── net462/
│   │       ├── netcore50/
│   │       │   ├── System.IO.xml
│   │       │   ├── de/
│   │       │   │   └── System.IO.xml
│   │       │   ├── es/
│   │       │   │   └── System.IO.xml
│   │       │   ├── fr/
│   │       │   │   └── System.IO.xml
│   │       │   ├── it/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ja/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ko/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ru/
│   │       │   │   └── System.IO.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.IO.xml
│   │       │   └── zh-hant/
│   │       │       └── System.IO.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.IO.xml
│   │       │   ├── de/
│   │       │   │   └── System.IO.xml
│   │       │   ├── es/
│   │       │   │   └── System.IO.xml
│   │       │   ├── fr/
│   │       │   │   └── System.IO.xml
│   │       │   ├── it/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ja/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ko/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ru/
│   │       │   │   └── System.IO.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.IO.xml
│   │       │   └── zh-hant/
│   │       │       └── System.IO.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.IO.xml
│   │       │   ├── de/
│   │       │   │   └── System.IO.xml
│   │       │   ├── es/
│   │       │   │   └── System.IO.xml
│   │       │   ├── fr/
│   │       │   │   └── System.IO.xml
│   │       │   ├── it/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ja/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ko/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ru/
│   │       │   │   └── System.IO.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.IO.xml
│   │       │   └── zh-hant/
│   │       │       └── System.IO.xml
│   │       ├── netstandard1.5/
│   │       │   ├── System.IO.xml
│   │       │   ├── de/
│   │       │   │   └── System.IO.xml
│   │       │   ├── es/
│   │       │   │   └── System.IO.xml
│   │       │   ├── fr/
│   │       │   │   └── System.IO.xml
│   │       │   ├── it/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ja/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ko/
│   │       │   │   └── System.IO.xml
│   │       │   ├── ru/
│   │       │   │   └── System.IO.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.IO.xml
│   │       │   └── zh-hant/
│   │       │       └── System.IO.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.IO.Compression.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.IO.Compression.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── portable-net45+win8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   ├── ref/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── netcore50/
│   │   │   │   ├── System.IO.Compression.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.IO.Compression.xml
│   │   │   ├── netstandard1.1/
│   │   │   │   ├── System.IO.Compression.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.IO.Compression.xml
│   │   │   ├── netstandard1.3/
│   │   │   │   ├── System.IO.Compression.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.IO.Compression.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.IO.Compression.xml
│   │   │   ├── portable-net45+win8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── runtimes/
│   │       ├── unix/
│   │       │   └── lib/
│   │       │       └── netstandard1.3/
│   │       └── win/
│   │           └── lib/
│   │               ├── net46/
│   │               └── netstandard1.3/
│   ├── System.IO.Compression.ZipFile.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.IO.Compression.ZipFile.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── netstandard1.3/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net46/
│   │       ├── netstandard1.3/
│   │       │   ├── System.IO.Compression.ZipFile.xml
│   │       │   ├── de/
│   │       │   │   └── System.IO.Compression.ZipFile.xml
│   │       │   ├── es/
│   │       │   │   └── System.IO.Compression.ZipFile.xml
│   │       │   ├── fr/
│   │       │   │   └── System.IO.Compression.ZipFile.xml
│   │       │   ├── it/
│   │       │   │   └── System.IO.Compression.ZipFile.xml
│   │       │   ├── ja/
│   │       │   │   └── System.IO.Compression.ZipFile.xml
│   │       │   ├── ko/
│   │       │   │   └── System.IO.Compression.ZipFile.xml
│   │       │   ├── ru/
│   │       │   │   └── System.IO.Compression.ZipFile.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.IO.Compression.ZipFile.xml
│   │       │   └── zh-hant/
│   │       │       └── System.IO.Compression.ZipFile.xml
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.IO.FileSystem.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.IO.FileSystem.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net46/
│   │       ├── netstandard1.3/
│   │       │   ├── System.IO.FileSystem.xml
│   │       │   ├── de/
│   │       │   │   └── System.IO.FileSystem.xml
│   │       │   ├── es/
│   │       │   │   └── System.IO.FileSystem.xml
│   │       │   ├── fr/
│   │       │   │   └── System.IO.FileSystem.xml
│   │       │   ├── it/
│   │       │   │   └── System.IO.FileSystem.xml
│   │       │   ├── ja/
│   │       │   │   └── System.IO.FileSystem.xml
│   │       │   ├── ko/
│   │       │   │   └── System.IO.FileSystem.xml
│   │       │   ├── ru/
│   │       │   │   └── System.IO.FileSystem.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.IO.FileSystem.xml
│   │       │   └── zh-hant/
│   │       │       └── System.IO.FileSystem.xml
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.IO.FileSystem.Primitives.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.IO.FileSystem.Primitives.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── netstandard1.3/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net46/
│   │       ├── netstandard1.3/
│   │       │   ├── System.IO.FileSystem.Primitives.xml
│   │       │   ├── de/
│   │       │   │   └── System.IO.FileSystem.Primitives.xml
│   │       │   ├── es/
│   │       │   │   └── System.IO.FileSystem.Primitives.xml
│   │       │   ├── fr/
│   │       │   │   └── System.IO.FileSystem.Primitives.xml
│   │       │   ├── it/
│   │       │   │   └── System.IO.FileSystem.Primitives.xml
│   │       │   ├── ja/
│   │       │   │   └── System.IO.FileSystem.Primitives.xml
│   │       │   ├── ko/
│   │       │   │   └── System.IO.FileSystem.Primitives.xml
│   │       │   ├── ru/
│   │       │   │   └── System.IO.FileSystem.Primitives.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.IO.FileSystem.Primitives.xml
│   │       │   └── zh-hant/
│   │       │       └── System.IO.FileSystem.Primitives.xml
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.IO.Pipelines.7.0.0/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── System.IO.Pipelines.7.0.0.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.IO.Pipelines.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net6.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── System.IO.Pipelines.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── System.IO.Pipelines.xml
│   │   │   ├── net6.0/
│   │   │   │   └── System.IO.Pipelines.xml
│   │   │   ├── net7.0/
│   │   │   │   └── System.IO.Pipelines.xml
│   │   │   └── netstandard2.0/
│   │   │       └── System.IO.Pipelines.xml
│   │   └── useSharedDesignerContext.txt
│   ├── System.IO.Pipelines.9.0.8/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── PACKAGE.md
│   │   ├── System.IO.Pipelines.9.0.8.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.IO.Pipelines.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net8.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── System.IO.Pipelines.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── System.IO.Pipelines.xml
│   │   │   ├── net8.0/
│   │   │   │   └── System.IO.Pipelines.xml
│   │   │   ├── net9.0/
│   │   │   │   └── System.IO.Pipelines.xml
│   │   │   └── netstandard2.0/
│   │   │       └── System.IO.Pipelines.xml
│   │   └── useSharedDesignerContext.txt
│   ├── System.Linq.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Linq.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net463/
│   │   │   ├── netcore50/
│   │   │   ├── netstandard1.6/
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── net463/
│   │       ├── netcore50/
│   │       │   ├── System.Linq.xml
│   │       │   ├── de/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── es/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── it/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Linq.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Linq.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Linq.xml
│   │       │   ├── de/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── es/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── it/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Linq.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Linq.xml
│   │       ├── netstandard1.6/
│   │       │   ├── System.Linq.xml
│   │       │   ├── de/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── es/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── it/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Linq.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Linq.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Linq.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Linq.Expressions.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Linq.Expressions.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net463/
│   │   │   ├── netcore50/
│   │   │   ├── netstandard1.6/
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   ├── ref/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net463/
│   │   │   ├── netcore50/
│   │   │   │   ├── System.Linq.Expressions.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.Linq.Expressions.xml
│   │   │   ├── netstandard1.0/
│   │   │   │   ├── System.Linq.Expressions.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.Linq.Expressions.xml
│   │   │   ├── netstandard1.3/
│   │   │   │   ├── System.Linq.Expressions.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.Linq.Expressions.xml
│   │   │   ├── netstandard1.6/
│   │   │   │   ├── System.Linq.Expressions.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.Linq.Expressions.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.Linq.Expressions.xml
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── runtimes/
│   │       └── aot/
│   │           └── lib/
│   │               └── netcore50/
│   ├── System.Memory.4.6.3/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── PACKAGE.md
│   │   ├── System.Memory.4.6.3.nupkg
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Memory.targets
│   │   │   └── net462/
│   │   │       └── _._
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── System.Memory.xml
│   │       ├── netcoreapp2.1/
│   │       │   └── _._
│   │       ├── netstandard2.0/
│   │       │   └── System.Memory.xml
│   │       └── netstandard2.1/
│   │           └── _._
│   ├── System.Net.Http.4.3.4/
│   │   ├── .signature.p7s
│   │   ├── System.Net.Http.4.3.4.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── Xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── monoandroid10/
│   │   │   │   └── _._
│   │   │   ├── monotouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── portable-net45+win8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   ├── ref/
│   │   │   ├── Xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── monoandroid10/
│   │   │   │   └── _._
│   │   │   ├── monotouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── netcore50/
│   │   │   ├── netstandard1.1/
│   │   │   ├── netstandard1.3/
│   │   │   ├── portable-net45+win8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── runtimes/
│   │       ├── unix/
│   │       │   └── lib/
│   │       │       └── netstandard1.6/
│   │       └── win/
│   │           └── lib/
│   │               ├── net46/
│   │               ├── netcore50/
│   │               └── netstandard1.3/
│   ├── System.Net.Primitives.4.3.1/
│   │   ├── .signature.p7s
│   │   ├── System.Net.Primitives.4.3.1.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Net.Primitives.xml
│   │       │   ├── de/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── es/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── it/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Net.Primitives.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Net.Primitives.xml
│   │       │   ├── de/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── es/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── it/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Net.Primitives.xml
│   │       ├── netstandard1.1/
│   │       │   ├── System.Net.Primitives.xml
│   │       │   ├── de/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── es/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── it/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Net.Primitives.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Net.Primitives.xml
│   │       │   ├── de/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── es/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── it/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Net.Primitives.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Net.Primitives.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Net.Sockets.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Net.Sockets.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net46/
│   │       ├── netstandard1.3/
│   │       │   ├── System.Net.Sockets.xml
│   │       │   ├── de/
│   │       │   │   └── System.Net.Sockets.xml
│   │       │   ├── es/
│   │       │   │   └── System.Net.Sockets.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Net.Sockets.xml
│   │       │   ├── it/
│   │       │   │   └── System.Net.Sockets.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Net.Sockets.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Net.Sockets.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Net.Sockets.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Net.Sockets.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Net.Sockets.xml
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Numerics.Vectors.4.6.1/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── PACKAGE.md
│   │   ├── System.Numerics.Vectors.4.6.1.nupkg
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Numerics.Vectors.targets
│   │   │   └── net462/
│   │   │       └── _._
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── System.Numerics.Vectors.xml
│   │       ├── netcoreapp2.0/
│   │       │   └── _._
│   │       ├── netstandard2.0/
│   │       │   └── System.Numerics.Vectors.xml
│   │       └── netstandard2.1/
│   │           └── _._
│   ├── System.ObjectModel.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.ObjectModel.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── netcore50/
│   │   │   ├── netstandard1.3/
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.ObjectModel.xml
│   │       │   ├── de/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── es/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── fr/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── it/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── ja/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── ko/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── ru/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   └── zh-hant/
│   │       │       └── System.ObjectModel.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.ObjectModel.xml
│   │       │   ├── de/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── es/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── fr/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── it/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── ja/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── ko/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── ru/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   └── zh-hant/
│   │       │       └── System.ObjectModel.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.ObjectModel.xml
│   │       │   ├── de/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── es/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── fr/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── it/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── ja/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── ko/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── ru/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.ObjectModel.xml
│   │       │   └── zh-hant/
│   │       │       └── System.ObjectModel.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Reflection.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Reflection.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net462/
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── net462/
│   │       ├── netcore50/
│   │       │   ├── System.Reflection.xml
│   │       │   ├── de/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── es/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── it/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Reflection.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Reflection.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Reflection.xml
│   │       │   ├── de/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── es/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── it/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Reflection.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Reflection.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Reflection.xml
│   │       │   ├── de/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── es/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── it/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Reflection.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Reflection.xml
│   │       ├── netstandard1.5/
│   │       │   ├── System.Reflection.xml
│   │       │   ├── de/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── es/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── it/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Reflection.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Reflection.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Reflection.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Reflection.Extensions.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Reflection.Extensions.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Reflection.Extensions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Reflection.Extensions.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Reflection.Extensions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Reflection.Extensions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Reflection.Extensions.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Reflection.Primitives.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Reflection.Primitives.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Reflection.Primitives.xml
│   │       │   ├── de/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── es/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── it/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Reflection.Primitives.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Reflection.Primitives.xml
│   │       │   ├── de/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── es/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── it/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Reflection.Primitives.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Reflection.Primitives.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Resources.ResourceManager.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Resources.ResourceManager.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Resources.ResourceManager.xml
│   │       │   ├── de/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── es/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── it/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Resources.ResourceManager.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Resources.ResourceManager.xml
│   │       │   ├── de/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── es/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── it/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Resources.ResourceManager.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Resources.ResourceManager.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Runtime.4.3.1/
│   │   ├── .signature.p7s
│   │   ├── System.Runtime.4.3.1.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net462/
│   │   │   ├── portable-net45+win8+wp80+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── net462/
│   │       ├── netcore50/
│   │       │   ├── System.Runtime.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Runtime.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.xml
│   │       ├── netstandard1.2/
│   │       │   ├── System.Runtime.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Runtime.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.xml
│   │       ├── netstandard1.5/
│   │       │   ├── System.Runtime.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.xml
│   │       ├── portable-net45+win8+wp80+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Runtime.CompilerServices.Unsafe.6.1.2/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── PACKAGE.md
│   │   ├── System.Runtime.CompilerServices.Unsafe.6.1.2.nupkg
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Runtime.CompilerServices.Unsafe.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net6.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── System.Runtime.CompilerServices.Unsafe.targets
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── System.Runtime.CompilerServices.Unsafe.xml
│   │       ├── net6.0/
│   │       │   └── System.Runtime.CompilerServices.Unsafe.xml
│   │       ├── net7.0/
│   │       │   └── _._
│   │       └── netstandard2.0/
│   │           └── System.Runtime.CompilerServices.Unsafe.xml
│   ├── System.Runtime.Extensions.4.3.1/
│   │   ├── .signature.p7s
│   │   ├── System.Runtime.Extensions.4.3.1.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net462/
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── net462/
│   │       ├── netcore50/
│   │       │   ├── System.Runtime.Extensions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.Extensions.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Runtime.Extensions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.Extensions.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Runtime.Extensions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.Extensions.xml
│   │       ├── netstandard1.5/
│   │       │   ├── System.Runtime.Extensions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.Extensions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.Extensions.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Runtime.Handles.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Runtime.Handles.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net46/
│   │       │   └── _._
│   │       ├── netstandard1.3/
│   │       │   ├── System.Runtime.Handles.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.Handles.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.Handles.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.Handles.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.Handles.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.Handles.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.Handles.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.Handles.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.Handles.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.Handles.xml
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Runtime.InteropServices.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Runtime.InteropServices.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net462/
│   │   │   ├── net463/
│   │   │   ├── portable-net45+win8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── net462/
│   │       ├── net463/
│   │       ├── netcore50/
│   │       │   ├── System.Runtime.InteropServices.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.InteropServices.xml
│   │       ├── netcoreapp1.1/
│   │       ├── netstandard1.1/
│   │       │   ├── System.Runtime.InteropServices.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.InteropServices.xml
│   │       ├── netstandard1.2/
│   │       │   ├── System.Runtime.InteropServices.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.InteropServices.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Runtime.InteropServices.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.InteropServices.xml
│   │       ├── netstandard1.5/
│   │       │   ├── System.Runtime.InteropServices.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.InteropServices.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.InteropServices.xml
│   │       ├── portable-net45+win8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Runtime.InteropServices.RuntimeInformation.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Runtime.InteropServices.RuntimeInformation.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   ├── netstandard1.1/
│   │   │   ├── win8/
│   │   │   ├── wpa81/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   ├── ref/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── netstandard1.1/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── runtimes/
│   │       ├── aot/
│   │       │   └── lib/
│   │       │       └── netcore50/
│   │       ├── unix/
│   │       │   └── lib/
│   │       │       └── netstandard1.1/
│   │       └── win/
│   │           └── lib/
│   │               ├── net45/
│   │               ├── netcore50/
│   │               └── netstandard1.1/
│   ├── System.Runtime.Numerics.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Runtime.Numerics.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── netcore50/
│   │   │   ├── netstandard1.3/
│   │   │   ├── portable-net45+win8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Runtime.Numerics.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.Numerics.xml
│   │       ├── netstandard1.1/
│   │       │   ├── System.Runtime.Numerics.xml
│   │       │   ├── de/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── es/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── it/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Runtime.Numerics.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Runtime.Numerics.xml
│   │       ├── portable-net45+win8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Security.Cryptography.Algorithms.4.3.1/
│   │   ├── .signature.p7s
│   │   ├── System.Security.Cryptography.Algorithms.4.3.1.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── net461/
│   │   │   ├── net463/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   ├── ref/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── net461/
│   │   │   ├── net463/
│   │   │   ├── netstandard1.3/
│   │   │   ├── netstandard1.4/
│   │   │   ├── netstandard1.6/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── runtimes/
│   │       ├── osx/
│   │       │   └── lib/
│   │       │       └── netstandard1.6/
│   │       ├── unix/
│   │       │   └── lib/
│   │       │       └── netstandard1.6/
│   │       └── win/
│   │           └── lib/
│   │               ├── net46/
│   │               ├── net461/
│   │               ├── net463/
│   │               ├── netcore50/
│   │               └── netstandard1.6/
│   ├── System.Security.Cryptography.Encoding.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Security.Cryptography.Encoding.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   ├── ref/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── netstandard1.3/
│   │   │   │   ├── System.Security.Cryptography.Encoding.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.Security.Cryptography.Encoding.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.Security.Cryptography.Encoding.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.Security.Cryptography.Encoding.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.Security.Cryptography.Encoding.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.Security.Cryptography.Encoding.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.Security.Cryptography.Encoding.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.Security.Cryptography.Encoding.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.Security.Cryptography.Encoding.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.Security.Cryptography.Encoding.xml
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── runtimes/
│   │       ├── unix/
│   │       │   └── lib/
│   │       │       └── netstandard1.3/
│   │       └── win/
│   │           └── lib/
│   │               ├── net46/
│   │               └── netstandard1.3/
│   ├── System.Security.Cryptography.Primitives.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Security.Cryptography.Primitives.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── netstandard1.3/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net46/
│   │       ├── netstandard1.3/
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Security.Cryptography.X509Certificates.4.3.2/
│   │   ├── .signature.p7s
│   │   ├── System.Security.Cryptography.X509Certificates.4.3.2.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── net461/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   ├── ref/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── net461/
│   │   │   ├── netstandard1.3/
│   │   │   ├── netstandard1.4/
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── runtimes/
│   │       ├── unix/
│   │       │   └── lib/
│   │       │       └── netstandard1.6/
│   │       └── win/
│   │           └── lib/
│   │               ├── net46/
│   │               ├── net461/
│   │               ├── netcore50/
│   │               └── netstandard1.6/
│   ├── System.Text.Encoding.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Text.Encoding.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Text.Encoding.xml
│   │       │   ├── de/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── es/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── it/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Text.Encoding.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Text.Encoding.xml
│   │       │   ├── de/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── es/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── it/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Text.Encoding.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Text.Encoding.xml
│   │       │   ├── de/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── es/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── it/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Text.Encoding.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Text.Encoding.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Text.Encoding.Extensions.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Text.Encoding.Extensions.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Text.Encoding.Extensions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Text.Encoding.Extensions.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Text.Encoding.Extensions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Text.Encoding.Extensions.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Text.Encoding.Extensions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Text.Encoding.Extensions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Text.Encoding.Extensions.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Text.Encodings.Web.7.0.0/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── System.Text.Encodings.Web.7.0.0.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Text.Encodings.Web.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net6.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── System.Text.Encodings.Web.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── System.Text.Encodings.Web.xml
│   │   │   ├── net6.0/
│   │   │   │   └── System.Text.Encodings.Web.xml
│   │   │   ├── net7.0/
│   │   │   │   └── System.Text.Encodings.Web.xml
│   │   │   └── netstandard2.0/
│   │   │       └── System.Text.Encodings.Web.xml
│   │   ├── runtimes/
│   │   │   └── browser/
│   │   │       └── lib/
│   │   │           ├── net6.0/
│   │   │           │   └── System.Text.Encodings.Web.xml
│   │   │           └── net7.0/
│   │   │               └── System.Text.Encodings.Web.xml
│   │   └── useSharedDesignerContext.txt
│   ├── System.Text.Encodings.Web.9.0.8/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── PACKAGE.md
│   │   ├── System.Text.Encodings.Web.9.0.8.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Text.Encodings.Web.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   ├── net8.0/
│   │   │   │   └── _._
│   │   │   └── netcoreapp2.0/
│   │   │       └── System.Text.Encodings.Web.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── System.Text.Encodings.Web.xml
│   │   │   ├── net8.0/
│   │   │   │   └── System.Text.Encodings.Web.xml
│   │   │   ├── net9.0/
│   │   │   │   └── System.Text.Encodings.Web.xml
│   │   │   └── netstandard2.0/
│   │   │       └── System.Text.Encodings.Web.xml
│   │   ├── runtimes/
│   │   │   └── browser/
│   │   │       └── lib/
│   │   │           ├── net8.0/
│   │   │           │   └── System.Text.Encodings.Web.xml
│   │   │           └── net9.0/
│   │   │               └── System.Text.Encodings.Web.xml
│   │   └── useSharedDesignerContext.txt
│   ├── System.Text.Json.7.0.3/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── README.md
│   │   ├── System.Text.Json.7.0.3.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── analyzers/
│   │   │   └── dotnet/
│   │   │       ├── roslyn3.11/
│   │   │       │   └── cs/
│   │   │       │       ├── cs/
│   │   │       │       ├── de/
│   │   │       │       ├── es/
│   │   │       │       ├── fr/
│   │   │       │       ├── it/
│   │   │       │       ├── ja/
│   │   │       │       ├── ko/
│   │   │       │       ├── pl/
│   │   │       │       ├── pt-BR/
│   │   │       │       ├── ru/
│   │   │       │       ├── tr/
│   │   │       │       ├── zh-Hans/
│   │   │       │       └── zh-Hant/
│   │   │       ├── roslyn4.0/
│   │   │       │   └── cs/
│   │   │       │       ├── cs/
│   │   │       │       ├── de/
│   │   │       │       ├── es/
│   │   │       │       ├── fr/
│   │   │       │       ├── it/
│   │   │       │       ├── ja/
│   │   │       │       ├── ko/
│   │   │       │       ├── pl/
│   │   │       │       ├── pt-BR/
│   │   │       │       ├── ru/
│   │   │       │       ├── tr/
│   │   │       │       ├── zh-Hans/
│   │   │       │       └── zh-Hant/
│   │   │       └── roslyn4.4/
│   │   │           └── cs/
│   │   │               ├── cs/
│   │   │               ├── de/
│   │   │               ├── es/
│   │   │               ├── fr/
│   │   │               ├── it/
│   │   │               ├── ja/
│   │   │               ├── ko/
│   │   │               ├── pl/
│   │   │               ├── pt-BR/
│   │   │               ├── ru/
│   │   │               ├── tr/
│   │   │               ├── zh-Hans/
│   │   │               └── zh-Hant/
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Text.Json.targets
│   │   │   ├── net462/
│   │   │   │   └── System.Text.Json.targets
│   │   │   ├── net6.0/
│   │   │   │   └── System.Text.Json.targets
│   │   │   ├── netcoreapp2.0/
│   │   │   │   └── System.Text.Json.targets
│   │   │   └── netstandard2.0/
│   │   │       └── System.Text.Json.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── System.Text.Json.xml
│   │   │   ├── net6.0/
│   │   │   │   └── System.Text.Json.xml
│   │   │   ├── net7.0/
│   │   │   │   └── System.Text.Json.xml
│   │   │   └── netstandard2.0/
│   │   │       └── System.Text.Json.xml
│   │   └── useSharedDesignerContext.txt
│   ├── System.Text.Json.9.0.8/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── LICENSE.TXT
│   │   ├── PACKAGE.md
│   │   ├── System.Text.Json.9.0.8.nupkg
│   │   ├── THIRD-PARTY-NOTICES.TXT
│   │   ├── analyzers/
│   │   │   └── dotnet/
│   │   │       ├── roslyn3.11/
│   │   │       │   └── cs/
│   │   │       │       ├── cs/
│   │   │       │       ├── de/
│   │   │       │       ├── es/
│   │   │       │       ├── fr/
│   │   │       │       ├── it/
│   │   │       │       ├── ja/
│   │   │       │       ├── ko/
│   │   │       │       ├── pl/
│   │   │       │       ├── pt-BR/
│   │   │       │       ├── ru/
│   │   │       │       ├── tr/
│   │   │       │       ├── zh-Hans/
│   │   │       │       └── zh-Hant/
│   │   │       ├── roslyn4.0/
│   │   │       │   └── cs/
│   │   │       │       ├── cs/
│   │   │       │       ├── de/
│   │   │       │       ├── es/
│   │   │       │       ├── fr/
│   │   │       │       ├── it/
│   │   │       │       ├── ja/
│   │   │       │       ├── ko/
│   │   │       │       ├── pl/
│   │   │       │       ├── pt-BR/
│   │   │       │       ├── ru/
│   │   │       │       ├── tr/
│   │   │       │       ├── zh-Hans/
│   │   │       │       └── zh-Hant/
│   │   │       └── roslyn4.4/
│   │   │           └── cs/
│   │   │               ├── cs/
│   │   │               ├── de/
│   │   │               ├── es/
│   │   │               ├── fr/
│   │   │               ├── it/
│   │   │               ├── ja/
│   │   │               ├── ko/
│   │   │               ├── pl/
│   │   │               ├── pt-BR/
│   │   │               ├── ru/
│   │   │               ├── tr/
│   │   │               ├── zh-Hans/
│   │   │               └── zh-Hant/
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Text.Json.targets
│   │   │   ├── net462/
│   │   │   │   └── System.Text.Json.targets
│   │   │   ├── net8.0/
│   │   │   │   └── System.Text.Json.targets
│   │   │   ├── netcoreapp2.0/
│   │   │   │   └── System.Text.Json.targets
│   │   │   └── netstandard2.0/
│   │   │       └── System.Text.Json.targets
│   │   ├── lib/
│   │   │   ├── net462/
│   │   │   │   └── System.Text.Json.xml
│   │   │   ├── net8.0/
│   │   │   │   └── System.Text.Json.xml
│   │   │   ├── net9.0/
│   │   │   │   └── System.Text.Json.xml
│   │   │   └── netstandard2.0/
│   │   │       └── System.Text.Json.xml
│   │   └── useSharedDesignerContext.txt
│   ├── System.Text.RegularExpressions.4.3.1/
│   │   ├── .signature.p7s
│   │   ├── System.Text.RegularExpressions.4.3.1.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net463/
│   │   │   ├── netcore50/
│   │   │   ├── netstandard1.6/
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── net463/
│   │       ├── netcore50/
│   │       │   ├── System.Text.RegularExpressions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Text.RegularExpressions.xml
│   │       ├── netcoreapp1.1/
│   │       ├── netstandard1.0/
│   │       │   ├── System.Text.RegularExpressions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Text.RegularExpressions.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Text.RegularExpressions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Text.RegularExpressions.xml
│   │       ├── netstandard1.6/
│   │       │   ├── System.Text.RegularExpressions.xml
│   │       │   ├── de/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── es/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── it/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Text.RegularExpressions.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Text.RegularExpressions.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Threading.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Threading.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── netcore50/
│   │   │   ├── netstandard1.3/
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   ├── ref/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── netcore50/
│   │   │   │   ├── System.Threading.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.Threading.xml
│   │   │   ├── netstandard1.0/
│   │   │   │   ├── System.Threading.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.Threading.xml
│   │   │   ├── netstandard1.3/
│   │   │   │   ├── System.Threading.xml
│   │   │   │   ├── de/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── es/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── fr/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── it/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── ja/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── ko/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── ru/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   ├── zh-hans/
│   │   │   │   │   └── System.Threading.xml
│   │   │   │   └── zh-hant/
│   │   │   │       └── System.Threading.xml
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── runtimes/
│   │       └── aot/
│   │           └── lib/
│   │               └── netcore50/
│   ├── System.Threading.Tasks.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Threading.Tasks.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Threading.Tasks.xml
│   │       │   ├── de/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── es/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── it/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Threading.Tasks.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Threading.Tasks.xml
│   │       │   ├── de/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── es/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── it/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Threading.Tasks.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Threading.Tasks.xml
│   │       │   ├── de/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── es/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── it/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Threading.Tasks.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Threading.Tasks.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Threading.Tasks.Extensions.4.6.3/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── PACKAGE.md
│   │   ├── System.Threading.Tasks.Extensions.4.6.3.nupkg
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.Threading.Tasks.Extensions.targets
│   │   │   └── net462/
│   │   │       └── _._
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── System.Threading.Tasks.Extensions.xml
│   │       ├── netcoreapp2.1/
│   │       │   └── _._
│   │       ├── netstandard2.0/
│   │       │   └── System.Threading.Tasks.Extensions.xml
│   │       └── netstandard2.1/
│   │           └── _._
│   ├── System.Threading.Timer.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Threading.Timer.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net451/
│   │   │   │   └── _._
│   │   │   ├── portable-net451+win81+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win81/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net451/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Threading.Timer.xml
│   │       │   ├── de/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── es/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── it/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Threading.Timer.xml
│   │       ├── netstandard1.2/
│   │       │   ├── System.Threading.Timer.xml
│   │       │   ├── de/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── es/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── it/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Threading.Timer.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Threading.Timer.xml
│   │       ├── portable-net451+win81+wpa81/
│   │       │   └── _._
│   │       ├── win81/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.ValueTuple.4.6.1/
│   │   ├── .signature.p7s
│   │   ├── Icon.png
│   │   ├── PACKAGE.md
│   │   ├── System.ValueTuple.4.6.1.nupkg
│   │   ├── buildTransitive/
│   │   │   ├── net461/
│   │   │   │   └── System.ValueTuple.targets
│   │   │   ├── net462/
│   │   │   │   └── _._
│   │   │   └── net471/
│   │   │       └── System.ValueTuple.targets
│   │   └── lib/
│   │       ├── net462/
│   │       │   └── System.ValueTuple.xml
│   │       ├── net47/
│   │       │   └── System.ValueTuple.xml
│   │       ├── net471/
│   │       │   └── _._
│   │       ├── netcoreapp2.0/
│   │       │   └── _._
│   │       └── netstandard2.0/
│   │           └── _._
│   ├── System.Xml.ReaderWriter.4.3.1/
│   │   ├── .signature.p7s
│   │   ├── System.Xml.ReaderWriter.4.3.1.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── net46/
│   │   │   ├── netcore50/
│   │   │   ├── netstandard1.3/
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── net46/
│   │       ├── netcore50/
│   │       │   ├── System.Xml.ReaderWriter.xml
│   │       │   ├── de/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── es/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── it/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Xml.ReaderWriter.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Xml.ReaderWriter.xml
│   │       │   ├── de/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── es/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── it/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Xml.ReaderWriter.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Xml.ReaderWriter.xml
│   │       │   ├── de/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── es/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── it/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Xml.ReaderWriter.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Xml.ReaderWriter.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── System.Xml.XDocument.4.3.0/
│   │   ├── .signature.p7s
│   │   ├── System.Xml.XDocument.4.3.0.nupkg
│   │   ├── ThirdPartyNotices.txt
│   │   ├── dotnet_library_license.txt
│   │   ├── lib/
│   │   │   ├── MonoAndroid10/
│   │   │   │   └── _._
│   │   │   ├── MonoTouch10/
│   │   │   │   └── _._
│   │   │   ├── net45/
│   │   │   │   └── _._
│   │   │   ├── netcore50/
│   │   │   ├── netstandard1.3/
│   │   │   ├── portable-net45+win8+wp8+wpa81/
│   │   │   │   └── _._
│   │   │   ├── win8/
│   │   │   │   └── _._
│   │   │   ├── wp80/
│   │   │   │   └── _._
│   │   │   ├── wpa81/
│   │   │   │   └── _._
│   │   │   ├── xamarinios10/
│   │   │   │   └── _._
│   │   │   ├── xamarinmac20/
│   │   │   │   └── _._
│   │   │   ├── xamarintvos10/
│   │   │   │   └── _._
│   │   │   └── xamarinwatchos10/
│   │   │       └── _._
│   │   └── ref/
│   │       ├── MonoAndroid10/
│   │       │   └── _._
│   │       ├── MonoTouch10/
│   │       │   └── _._
│   │       ├── net45/
│   │       │   └── _._
│   │       ├── netcore50/
│   │       │   ├── System.Xml.XDocument.xml
│   │       │   ├── de/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── es/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── it/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Xml.XDocument.xml
│   │       ├── netstandard1.0/
│   │       │   ├── System.Xml.XDocument.xml
│   │       │   ├── de/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── es/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── it/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Xml.XDocument.xml
│   │       ├── netstandard1.3/
│   │       │   ├── System.Xml.XDocument.xml
│   │       │   ├── de/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── es/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── fr/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── it/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── ja/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── ko/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── ru/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   ├── zh-hans/
│   │       │   │   └── System.Xml.XDocument.xml
│   │       │   └── zh-hant/
│   │       │       └── System.Xml.XDocument.xml
│   │       ├── portable-net45+win8+wp8+wpa81/
│   │       │   └── _._
│   │       ├── win8/
│   │       │   └── _._
│   │       ├── wp80/
│   │       │   └── _._
│   │       ├── wpa81/
│   │       │   └── _._
│   │       ├── xamarinios10/
│   │       │   └── _._
│   │       ├── xamarinmac20/
│   │       │   └── _._
│   │       ├── xamarintvos10/
│   │       │   └── _._
│   │       └── xamarinwatchos10/
│   │           └── _._
│   ├── ZstdSharp.Port.0.8.6/
│   │   ├── .signature.p7s
│   │   ├── ZstdSharp.Port.0.8.6.nupkg
│   │   └── lib/
│   │       ├── net462/
│   │       ├── net5.0/
│   │       ├── net6.0/
│   │       ├── net7.0/
│   │       ├── net8.0/
│   │       ├── net9.0/
│   │       ├── netcoreapp3.1/
│   │       ├── netstandard2.0/
│   │       └── netstandard2.1/
│   ├── nlog/
│   │   └── 6.0.3/
│   │       ├── .nupkg.metadata
│   │       ├── .signature.p7s
│   │       ├── N.png
│   │       ├── lib/
│   │       │   ├── net35/
│   │       │   │   └── NLog.xml
│   │       │   ├── net45/
│   │       │   │   └── NLog.xml
│   │       │   ├── net46/
│   │       │   │   └── NLog.xml
│   │       │   ├── netstandard2.0/
│   │       │   │   └── NLog.xml
│   │       │   └── netstandard2.1/
│   │       │       └── NLog.xml
│   │       ├── nlog.6.0.3.nupkg
│   │       ├── nlog.6.0.3.nupkg.sha512
│   │       └── nlog.nuspec
│   ├── xunit.abstractions.2.0.3/
│   │   ├── .signature.p7s
│   │   ├── lib/
│   │   │   ├── net35/
│   │   │   │   └── xunit.abstractions.xml
│   │   │   ├── netstandard1.0/
│   │   │   │   └── xunit.abstractions.xml
│   │   │   └── netstandard2.0/
│   │   │       └── xunit.abstractions.xml
│   │   └── xunit.abstractions.2.0.3.nupkg
│   ├── xunit.extensibility.core.2.4.2/
│   │   ├── .signature.p7s
│   │   ├── _content/
│   │   │   └── logo-128-transparent.png
│   │   ├── lib/
│   │   │   ├── net452/
│   │   │   │   ├── xunit.core.dll.tdnet
│   │   │   │   └── xunit.core.xml
│   │   │   └── netstandard1.1/
│   │   │       └── xunit.core.xml
│   │   └── xunit.extensibility.core.2.4.2.nupkg
│   └── xunit.extensibility.core.2.9.3/
│       ├── .signature.p7s
│       ├── _content/
│       │   ├── README.md
│       │   └── logo-128-transparent.png
│       ├── lib/
│       │   ├── net452/
│       │   │   ├── xunit.core.dll.tdnet
│       │   │   └── xunit.core.xml
│       │   └── netstandard1.1/
│       │       └── xunit.core.xml
│       └── xunit.extensibility.core.2.9.3.nupkg
├── scripts/
│   ├── compile.sh
│   ├── container-setup.sh
│   ├── db-setup.sh
│   ├── distrobox-create.sh
│   └── run-servers.sh
└── start-projectwar.sh
```
