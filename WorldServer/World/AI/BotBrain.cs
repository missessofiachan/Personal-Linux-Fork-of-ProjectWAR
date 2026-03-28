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
        private CombatInterface_Player PlayerCombat => _unit?.CbtInterface as CombatInterface_Player;

        public BotBrain(Unit unit) : base(unit)
        {
            Random rand = new Random(unit.Oid);
            _formationOffset.X = rand.Next(-10, 10);
            _formationOffset.Y = rand.Next(-10, 10);
        }

        public override bool StartCombat(Unit fighter)
        {
            Player player = _unit as Player;
            CombatInterface_Player combat = PlayerCombat;

            if (player == null || combat == null || fighter == null || fighter.IsDead || player.IsDead)
                return false;

            GetAggro(fighter.Oid).DamageReceived += 100;

            combat.SetTarget(fighter.Oid, TargetTypes.TARGETTYPES_TARGET_ENEMY);
            combat.IsAttacking = true;
            player.CbtInterface.RefreshCombatTimer();

            FollowCombatTarget(player, fighter, true);
            return true;
        }

        public override void Fight()
        {
            Player player = _unit as Player;
            CombatInterface_Player combat = PlayerCombat;

            if (player == null || combat == null)
                return;

            Unit target = combat.GetCurrentTarget();
            if (target == null || target.IsDead || target.PendingDisposal || target.IsDisposed || !CombatInterface.CanAttack(player, target))
            {
                AI.ProcessCombatEnd();
                return;
            }

            if (player.IsDisabled || player.IsStaggered)
                return;

            combat.IsAttacking = true;
            FollowCombatTarget(player, target);
        }

        public override bool EndCombat()
        {
            Player player = _unit as Player;
            CombatInterface_Player combat = PlayerCombat;

            if (player == null || combat == null)
                return false;

            combat.IsAttacking = false;
            combat.SetTarget((ushort)0, TargetTypes.TARGETTYPES_TARGET_ENEMY);
            player.MvtInterface.StopMove();
            Aggros = new Dictionary<ushort, AggroInfo>();
            return true;
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

        private void FollowCombatTarget(Player player, Unit target, bool forceMove = false)
        {
            int desiredRange = GetDesiredCombatRange(player);
            int minTolerance = Math.Max(5, desiredRange - 5);
            int maxTolerance = Math.Max(minTolerance + 1, desiredRange);

            player.MvtInterface.Follow(target, minTolerance, maxTolerance, false, forceMove);
        }

        private int GetDesiredCombatRange(Player player)
        {
            switch (player.Role)
            {
                case BotRole.Healer:
                    return 80;
                case BotRole.RangedDPS:
                    return 70;
                default:
                    return 5;
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
