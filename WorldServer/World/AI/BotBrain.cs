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
using WorldServer.NetWork.Handler;

namespace WorldServer.World.AI
{
    public class BotBrain : ABrain
    {
        /// <summary>The battlefield objective this bot is currently marching toward.</summary>
        public BattlefieldObjective CurrentTargetObjective { get; private set; }

        private HoldObject _currentTargetScenarioObject;
        private long _nextObjectiveCheck = 0;
        private long _nextInteractAttempt = 0;
        private readonly Point3D _formationOffset = new Point3D(0, 0, 0);

        private CombatInterface_Player PlayerCombat => _unit?.CbtInterface as CombatInterface_Player;

        /// <summary>Distance in feet at which a bot will attempt to interact with a flag.</summary>
        private const int INTERACT_RANGE_FT = 13;
        /// <summary>Milliseconds between successive interact attempts on the same flag.</summary>
        private const long INTERACT_COOLDOWN_MS = 2500;
        /// <summary>How often (ms) each bot picks or re-evaluates its objective target.</summary>
        private const long OBJECTIVE_RECHECK_MS = 5000;
        /// <summary>If non-MA bot is further than this (feet) from the MA, it follows the MA.</summary>
        private const int MAX_FOLLOW_DISTANCE_FT = 30;

        public BotBrain(Unit unit) : base(unit)
        {
            // Unique formation offset per bot so the group doesn't stack on one tile.
            Random rand = new Random(unit.Oid);
            _formationOffset.X = rand.Next(-8, 8);
            _formationOffset.Y = rand.Next(-8, 8);
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
            if (player == null || combat == null) return;

            Unit target = combat.GetCurrentTarget();
            if (target == null || target.IsDead || target.PendingDisposal || target.IsDisposed || !CombatInterface.CanAttack(player, target))
            {
                AI.ProcessCombatEnd();
                return;
            }

            if (player.IsDisabled || player.IsStaggered) return;

            combat.IsAttacking = true;
            FollowCombatTarget(player, target);
        }

        public override bool EndCombat()
        {
            Player player = _unit as Player;
            CombatInterface_Player combat = PlayerCombat;
            if (player == null || combat == null) return false;

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

            // ── 1. Scenario join ──────────────────────────────────────────────
            if (player.ScnInterface.PendingScenario != null)
            {
                player.ScnInterface.PendingScenario.EnqueueScenarioAction(
                    new ScenarioQueueAction(EScenarioQueueAction.AddPlayer, player));
                return;
            }

            // ── 2. Assist train / formation follow ────────────────────────────
            Group group = player.PriorityGroup;
            if (group != null)
            {
                Player ma = group.MainAssist;
                if (ma != null && ma != player && !ma.IsDead)
                {
                    // Pick up the MA's combat target.
                    Unit maTarget = ma.CbtInterface.GetCurrentTarget();
                    if (maTarget != null && maTarget != player.CbtInterface.GetCurrentTarget() && !maTarget.IsDead)
                        player.AiInterface.ProcessCombatStart(maTarget);

                    // Follow MA if the group has spread too far apart.
                    if (player.GetDistanceTo(ma) > MAX_FOLLOW_DISTANCE_FT)
                    {
                        player.MvtInterface.Move(new Point3D(
                            ma.WorldPosition.X + _formationOffset.X,
                            ma.WorldPosition.Y + _formationOffset.Y,
                            ma.WorldPosition.Z));
                        return;
                    }
                }
            }

            if (player.CbtInterface.IsInCombat) return;

            Unit nearbyTarget = player.AiInterface.GetAttackableUnit();
            if (nearbyTarget != null)
            {
                player.AiInterface.ProcessCombatStart(nearbyTarget);
                return;
            }

            // ── 3. Scenario objective ─────────────────────────────────────────
            if (player.ScnInterface.Scenario != null)
            {
                UpdateScenarioObjective(player, tick);
                return;
            }

            // ── 4. Open-world RvR objective march & capture ───────────────────
            if (tick >= _nextObjectiveCheck)
            {
                _nextObjectiveCheck = tick + OBJECTIVE_RECHECK_MS;
                UpdateObjectiveGoal(player, group);
            }

            if (CurrentTargetObjective == null || CurrentTargetObjective.IsDisposed || !CurrentTargetObjective.IsActive)
            {
                // Invalidate stale target; it will be refreshed next recheck.
                CurrentTargetObjective = null;
                return;
            }

            float dist = player.GetDistanceTo(CurrentTargetObjective);

            if (dist <= INTERACT_RANGE_FT)
            {
                // Within capture range — attempt to interact with the flag.
                if (tick >= _nextInteractAttempt)
                {
                    _nextInteractAttempt = tick + INTERACT_COOLDOWN_MS;
                    CurrentTargetObjective.SendInteract(player, new InteractMenu());

                    // If the bot owns this objective now, clear target immediately so
                    // UpdateObjectiveGoal picks the next one on the next recheck.
                    if (CurrentTargetObjective.OwningRealm == (Realms)player.Realm)
                        CurrentTargetObjective = null;
                }
            }
            else
            {
                // Still marching toward the flag.
                player.MvtInterface.Move(new Point3D(
                    CurrentTargetObjective.WorldPosition.X + _formationOffset.X,
                    CurrentTargetObjective.WorldPosition.Y + _formationOffset.Y,
                    CurrentTargetObjective.WorldPosition.Z));
            }
        }

        // ── Objective selection ───────────────────────────────────────────────

        private void UpdateObjectiveGoal(Player player, Group group)
        {
            if (player.Region?.Campaign == null) return;

            // Non-MA bots mirror the MA's chosen target so the whole group converges.
            if (group != null && group.MainAssist != null && group.MainAssist != player)
            {
                if (group.MainAssist.AiInterface.CurrentBrain is BotBrain maBrain
                    && maBrain.CurrentTargetObjective != null
                    && !maBrain.CurrentTargetObjective.IsDisposed)
                {
                    CurrentTargetObjective = maBrain.CurrentTargetObjective;
                    return;
                }
            }

            var objectives = player.Region.Campaign.Objectives;
            if (objectives == null || objectives.Count == 0) return;

            // Target the nearest objective the bot's realm doesn't own.
            BattlefieldObjective best = objectives
                .Where(o => o.OwningRealm != (Realms)player.Realm)
                .OrderBy(o => player.GetDistanceTo(o))
                .FirstOrDefault();

            CurrentTargetObjective = best;
        }

        // ── Scenario logic ────────────────────────────────────────────────────

        private void UpdateScenarioObjective(Player player, long tick)
        {
            if (tick < _nextObjectiveCheck) return;
            _nextObjectiveCheck = tick + 2000;

            // Only the MA drives toward the flag; others follow via formation.
            if (player.PriorityGroup != null && player.PriorityGroup.MainAssist != player) return;

            Scenario scenario = player.ScnInterface.Scenario;
            var obj = scenario.GetHoldObjects().FirstOrDefault();
            if (obj == null) return;

            _currentTargetScenarioObject = obj;
            float dist = player.GetDistanceTo(obj);
            if (dist > 10)
                player.MvtInterface.Move(obj.WorldPosition);
            else
                obj.BeginInteraction(player);
        }

        // ── Combat helpers ────────────────────────────────────────────────────

        private void FollowCombatTarget(Player player, Unit target, bool forceMove = false)
        {
            int desired = GetDesiredCombatRange(player);
            int min = Math.Max(5, desired - 5);
            int max = Math.Max(min + 1, desired);
            player.MvtInterface.Follow(target, min, max, false, forceMove);
        }

        private int GetDesiredCombatRange(Player player)
        {
            switch (player.Role)
            {
                case BotRole.Healer:     return 80;
                case BotRole.RangedDPS:  return 70;
                default:                 return 5;
            }
        }
    }
}
