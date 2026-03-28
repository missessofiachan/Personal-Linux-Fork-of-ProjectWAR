using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FrameWork;
using WorldServer.Services.World;
using WorldServer.World.Battlefronts.Apocalypse;
using WorldServer.World.Objects;
using GameData;
using WorldServer.World.Positions;
using WorldServer.World.Scenarios;

namespace WorldServer.Managers
{
    public class DynamicBotManager
    {
        private static readonly string[] ExpectedBotSuffixes = { "_H", "_R", "_MT", "_OT", "_M1", "_M2" };
        private static DynamicBotManager _instance;
        public static DynamicBotManager Instance => _instance ??= new DynamicBotManager();

        private DateTime _lastUpdate = DateTime.MinValue;
        private const int UPDATE_INTERVAL_MINUTES = 1;
        private long _nextScenarioQueueCheck = 0;
        private bool _started;

        public void Start()
        {
            if (_started)
                return;

            _started = true;
            Log.Info("DynamicBotManager", "Started bot monitoring service on server up.");
        }

        public void Update(object state)
        {
            try
            {
                long tick = TCPManager.GetTimeStampMS();
                ProcessBattlefields();
                ProcessScenarios(tick);
            }
            catch (Exception ex)
            {
                Log.Error("DynamicBotManager", $"Error in monitoring loop: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void ProcessBattlefields()
        {
            if (WorldMgr.UpperTierCampaignManager != null)
            {
                Campaign campaign = null;
                try
                {
                    campaign = WorldMgr.UpperTierCampaignManager.GetActiveCampaign();
                }
                catch (Exception ex)
                {
                    Log.Error("DynamicBotManager", $"Failed to query upper-tier campaign state: {ex.Message}");
                }

                if (campaign != null)
                    MonitorCampaign(campaign);
            }
            
            if (WorldMgr.LowerTierCampaignManager != null)
            {
                Campaign campaign = null;
                try
                {
                    campaign = WorldMgr.LowerTierCampaignManager.GetActiveCampaign();
                }
                catch (Exception ex)
                {
                    Log.Error("DynamicBotManager", $"Failed to query lower-tier campaign state: {ex.Message}");
                }

                if (campaign != null)
                    MonitorCampaign(campaign);
            }
        }

        private void MonitorCampaign(Campaign campaign)
        {
            if (!TryGetCampaignZoneId(campaign, out ushort zoneId))
                return;

            int tier = campaign.Tier;

            if (BattleFrontService.GetWarcampEntrance(zoneId, Realms.REALMS_REALM_ORDER) == null
                || BattleFrontService.GetWarcampEntrance(zoneId, Realms.REALMS_REALM_DESTRUCTION) == null)
            {
                Log.Error("DynamicBotManager", $"Skipping bot spawn for zone {zoneId}: missing warcamp entrance data.");
                return;
            }
            
            string orderPrefix = $"Bot_T{tier}_O_{zoneId}";
            string destroPrefix = $"Bot_T{tier}_D_{zoneId}";

            var players = Player.GetPlayersSnapshot();
            if (!HasCompleteBotGroup(orderPrefix, players))
            {
                BotManager.Instance.SpawnBotGroup(Realms.REALMS_REALM_ORDER, tier, 40, orderPrefix, zoneId);
            }

            if (!HasCompleteBotGroup(destroPrefix, players))
            {
                BotManager.Instance.SpawnBotGroup(Realms.REALMS_REALM_DESTRUCTION, tier, 40, destroPrefix, zoneId);
            }
        }

        private void ProcessScenarios(long tick)
        {
            if (tick < _nextScenarioQueueCheck) return;
            _nextScenarioQueueCheck = tick + 30000; // Check every 30s

            // Spawn and queue scenario bots
            // We want at least one group per realm queued for scenarios
            string orderScenPrefix = "Bot_Scen_O";
            string destroScenPrefix = "Bot_Scen_D";

            var players = Player.GetPlayersSnapshot();
            
            EnsureScenarioGroup(Realms.REALMS_REALM_ORDER, orderScenPrefix, players);
            EnsureScenarioGroup(Realms.REALMS_REALM_DESTRUCTION, destroScenPrefix, players);
        }

        private void EnsureScenarioGroup(Realms realm, string prefix, List<Player> players)
        {
            var botGroup = GetCompleteBotGroup(prefix, players);
            
            if (botGroup == null)
            {
                if (!TryResolveScenarioStagingZone(out ushort zoneId))
                    return;

                botGroup = BotManager.Instance.SpawnBotGroup(realm, 4, 40, prefix, zoneId);
            }

            if (botGroup == null)
                return;

            if (!botGroup.Members.All(member => member != null && member.Loaded && member.Initialized))
                return;

            if (botGroup != null)
            {
                // Check if already in scenario or queued
                bool inScen = botGroup.Members.Any(m => m.ScnInterface.Scenario != null || m.ScnInterface.HasPendingScenario);
                if (!inScen)
                {
                    Scenario_Info targetScenario = GetPreferredScenarioForBots();
                    if (targetScenario != null)
                        WorldMgr.ScenarioMgr.EnqueueGroup(botGroup, targetScenario.ScenarioId);
                }
            }
        }

        private bool TryResolveScenarioStagingZone(out ushort zoneId)
        {
            zoneId = 0;

            if (TryGetScenarioZoneFromCampaign(GetActiveCampaignSafely(WorldMgr.UpperTierCampaignManager, "upper-tier"), out zoneId))
                return true;

            if (TryGetScenarioZoneFromCampaign(GetActiveCampaignSafely(WorldMgr.LowerTierCampaignManager, "lower-tier"), out zoneId))
                return true;

            Log.Error("DynamicBotManager", "Unable to find a valid staging zone for scenario bots.");
            return false;
        }

        private static bool TryGetScenarioZoneFromCampaign(Campaign campaign, out ushort zoneId)
        {
            zoneId = 0;

            if (!TryGetCampaignZoneId(campaign, out ushort candidateZoneId))
                return false;

            if (BattleFrontService.GetWarcampEntrance(candidateZoneId, Realms.REALMS_REALM_ORDER) == null
                || BattleFrontService.GetWarcampEntrance(candidateZoneId, Realms.REALMS_REALM_DESTRUCTION) == null)
                return false;

            zoneId = candidateZoneId;
            return true;
        }

        private static bool TryGetCampaignZoneId(Campaign campaign, out ushort zoneId)
        {
            zoneId = 0;
            if (campaign == null)
                return false;

            var activeBattleFront = campaign.BattleFrontManager?.ActiveBattleFront;
            if (activeBattleFront == null)
                return false;

            int activeZoneId = 0;
            try
            {
                activeZoneId = campaign.BattleFrontManager.GetBattleFrontStatus(activeBattleFront.BattleFrontId)?.ZoneId ?? 0;
            }
            catch
            {
                activeZoneId = 0;
            }

            if (activeZoneId <= 0)
                activeZoneId = activeBattleFront.ZoneId;

            if (activeZoneId <= 0)
                return false;

            zoneId = (ushort)activeZoneId;
            return true;
        }

        private Campaign GetActiveCampaignSafely(object manager, string label)
        {
            if (manager == null)
                return null;

            try
            {
                return manager switch
                {
                    UpperTierCampaignManager upperTierManager => upperTierManager.GetActiveCampaign(),
                    LowerTierCampaignManager lowerTierManager => lowerTierManager.GetActiveCampaign(),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                Log.Error("DynamicBotManager", $"Failed to query {label} scenario staging campaign: {ex.Message}");
                return null;
            }
        }

        private static bool HasCompleteBotGroup(string prefix, List<Player> players)
        {
            return ExpectedBotSuffixes.All(suffix =>
                players.Any(player => player.IsBot && player.Name.Equals(prefix + suffix, StringComparison.OrdinalIgnoreCase)));
        }

        private static Group GetCompleteBotGroup(string prefix, List<Player> players)
        {
            if (!HasCompleteBotGroup(prefix, players))
                return null;

            return players
                .FirstOrDefault(player => player.IsBot
                    && player.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                    && player.PriorityGroup != null)
                ?.PriorityGroup;
        }

        private static Scenario_Info GetPreferredScenarioForBots()
        {
            return ScenarioService.ActiveScenarios
                       .FirstOrDefault(scenario => scenario.QueueType == (int)ScenarioQueueType.Standard)
                   ?? ScenarioService.ActiveScenarios
                       .FirstOrDefault(scenario => scenario.QueueType == (int)ScenarioQueueType.Premade)
                   ?? ScenarioService.ActiveScenarios.FirstOrDefault();
        }
    }
}
