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
        private static DynamicBotManager _instance;
        public static DynamicBotManager Instance => _instance ??= new DynamicBotManager();

        private DateTime _lastUpdate = DateTime.MinValue;
        private const int UPDATE_INTERVAL_MINUTES = 1;
        private long _nextScenarioQueueCheck = 0;

        public void Start()
        {
            Log.Info("DynamicBotManager", "Started bot monitoring service on server up.");
            ProcessBattlefields();
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
                var campaign = WorldMgr.UpperTierCampaignManager.GetActiveCampaign();
                if (campaign != null)
                {
                    MonitorCampaign(campaign);
                }
            }
            
            if (WorldMgr.LowerTierCampaignManager != null)
            {
                var campaign = WorldMgr.LowerTierCampaignManager.GetActiveCampaign();
                if (campaign != null)
                {
                    MonitorCampaign(campaign);
                }
            }
        }

        private void MonitorCampaign(Campaign campaign)
        {
            int tier = campaign.Tier;
            ushort zoneId = (ushort)campaign.ActiveBattleFrontStatus.ZoneId;
            
            string orderPrefix = $"Bot_T{tier}_O_{zoneId}";
            string destroPrefix = $"Bot_T{tier}_D_{zoneId}";

            var players = Player.GetPlayersSnapshot();
            if (!players.Any(p => p.Name.StartsWith(orderPrefix)))
            {
                BotManager.Instance.SpawnBotGroup(Realms.REALMS_REALM_ORDER, tier, 40, orderPrefix, zoneId);
            }

            if (!players.Any(p => p.Name.StartsWith(destroPrefix)))
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
            var botGroup = players.FirstOrDefault(p => p.Name.StartsWith(prefix) && p.IsBot && p.PriorityGroup != null)?.PriorityGroup;
            
            if (botGroup == null)
            {
                // Spawn a new group at a default zone (e.g. Altdorf or IC warcamp)
                // Using zone 1 (Altdorf) as a fallback
                ushort zoneId = (realm == Realms.REALMS_REALM_ORDER) ? (ushort)1 : (ushort)2; 
                botGroup = BotManager.Instance.SpawnBotGroup(realm, 4, 40, prefix, zoneId);
            }

            if (botGroup != null)
            {
                // Check if already in scenario or queued
                bool inScen = botGroup.Members.Any(m => m.ScnInterface.Scenario != null || m.ScnInterface.HasPendingScenario);
                if (!inScen)
                {
                    // Queue for all available scenarios
                    foreach (var scen in ScenarioService.ActiveScenarios)
                    {
                        WorldMgr.ScenarioMgr.EnqueueGroup(botGroup, scen.ScenarioId);
                    }
                }
            }
        }
    }
}
