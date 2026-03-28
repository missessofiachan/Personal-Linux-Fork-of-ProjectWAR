using Common;
using GameData;
using WorldServer.World.AI;
using WorldServer.World.Objects;
using WorldServer.World.Interfaces;
using WorldServer.World.Battlefronts.Apocalypse;
using WorldServer.World.Scenarios;
using WorldServer.World.Scenarios.Objects;
using System.Linq;
using WorldServer.World.Positions;
using System;
using System.Collections.Generic;

namespace WorldServer.World.AI
{
    public class BotBrain : ABrain
    {
        private BattlefieldObjective _currentTargetObjective;
        private HoldObject _currentTargetScenarioObject;
        private long _nextObjectiveCheck = 0;
        private Point3D _formationOffset = new Point3D(0, 0, 0);

        public BotBrain(Unit unit) : base(unit)
        {
            Random rand = new Random(unit.Oid);
            _formationOffset.X = rand.Next(-10, 10);
            _formationOffset.Y = rand.Next(-10, 10);
        }

        public override void Think(long tick)
        {
            base.Think(tick);

            Player player = _unit as Player;
            if (player == null || !player.IsBot) return;

            // Scenario Join Logic
            if (player.ScnInterface.PendingScenario != null)
            {
                player.ScnInterface.PendingScenario.EnqueueScenarioAction(new ScenarioQueueAction(EScenarioQueueAction.AddPlayer, player));
                return;
            }

            // Combat logic (Assist Train)
            Group group = player.PriorityGroup;
            if (group != null)
            {
                Player ma = group.MainAssist;
                if (ma != null && ma != player && !ma.IsDead)
                {
                    Unit maTarget = ma.CbtInterface.GetCurrentTarget();
                    if (maTarget != null && maTarget != player.CbtInterface.GetCurrentTarget() && !maTarget.IsDead)
                    {
                        player.AiInterface.ProcessCombatStart(maTarget);
                    }
                    
                    if (player.GetDistanceTo(ma) > 20)
                    {
                        Point3D targetPos = new Point3D(ma.WorldPosition.X + _formationOffset.X, ma.WorldPosition.Y + _formationOffset.Y, ma.WorldPosition.Z);
                        player.MvtInterface.Move(targetPos);
                        return;
                    }
                }
            }

            if (player.CbtInterface.IsInCombat) return;

            // Scenario Objective Logic
            if (player.ScnInterface.Scenario != null)
            {
                UpdateScenarioObjective(player, tick);
                return;
            }

            // Open World RvR Objective Logic
            if (tick >= _nextObjectiveCheck)
            {
                _nextObjectiveCheck = tick + 5000;
                UpdateObjectiveGoal(player);
            }

            if (_currentTargetObjective != null)
            {
                float dist = player.GetDistanceTo(_currentTargetObjective);
                if (dist > 15)
                {
                    Point3D targetPos = new Point3D(_currentTargetObjective.WorldPosition.X + _formationOffset.X, _currentTargetObjective.WorldPosition.Y + _formationOffset.Y, _currentTargetObjective.WorldPosition.Z);
                    player.MvtInterface.Move(targetPos);
                }
            }
        }

        private void UpdateScenarioObjective(Player player, long tick)
        {
            if (tick < _nextObjectiveCheck) return;
            _nextObjectiveCheck = tick + 2000;

            if (player.PriorityGroup != null && player.PriorityGroup.MainAssist != player) return;

            Scenario scenario = player.ScnInterface.Scenario;
            var obj = scenario.GetHoldObjects().FirstOrDefault();
            if (obj != null)
            {
                _currentTargetScenarioObject = obj;
                float dist = player.GetDistanceTo(obj);
                if (dist > 10)
                {
                    player.MvtInterface.Move(obj.WorldPosition);
                }
                else
                {
                    // Interact with the object (e.g. pick up the flag)
                    obj.BeginInteraction(player);
                }
            }
        }

        private void UpdateObjectiveGoal(Player player)
        {
            if (player.Region?.Campaign == null) return;

            if (player.PriorityGroup != null && player.PriorityGroup.MainAssist != player) return;

            var objectives = player.Region.Campaign.Objectives;
            if (objectives == null || objectives.Count == 0) return;

            _currentTargetObjective = objectives
                .Where(o => o.OwningRealm != (Realms)player.Realm)
                .OrderBy(o => player.GetDistanceTo(o))
                .FirstOrDefault();
        }
    }
}
