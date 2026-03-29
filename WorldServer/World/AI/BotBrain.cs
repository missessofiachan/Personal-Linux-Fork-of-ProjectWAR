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
using Object = WorldServer.World.Objects.Object;

namespace WorldServer.World.AI
{
    public class BotBrain : ABrain
    {
        /// <summary>The battlefield objective this bot is currently marching toward.</summary>
        public BattlefieldObjective CurrentTargetObjective { get; private set; }

        private Object _currentTargetScenarioObject;
        private ushort _rezAbilityId = 0;
        private long _nextObjectiveCheck = 0;
        private long _nextInteractAttempt = 0;
        private readonly Point3D _formationOffset = new Point3D(0, 0, 0);

        // Cached list of scenario objectives so UpdateScenarioObjective avoids
        // scanning all region objects every 2 seconds.
        private List<Object> _cachedScenarioObjectives;
        private Scenario _lastCachedScenario;

        private CombatInterface_Player PlayerCombat => _unit?.CbtInterface as CombatInterface_Player;

        /// <summary>Distance in feet at which a bot will attempt to interact with a flag.</summary>
        private const int INTERACT_RANGE_FT = 13;
        /// <summary>Milliseconds between successive interact attempts on the same flag.</summary>
        private const long INTERACT_COOLDOWN_MS = 2500;
        /// <summary>How often (ms) each bot picks or re-evaluates its objective target.</summary>
        private const long OBJECTIVE_RECHECK_MS = 5000;
        /// <summary>If non-MA bot is further than this (feet) from the MA, it follows the MA.</summary>
        private const int MAX_FOLLOW_DISTANCE_FT = 30;
        /// <summary>Maximum distance (feet) a healer bot will travel to resurrect a group member.</summary>
        private const int MAX_REZZ_DISTANCE_FT = 100;

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

            // Re-evaluate target if we are the Main Assist.
            if (player.PriorityGroup?.MainAssist == player)
            {
                Unit bestTarget = SelectCombatTarget(player);
                if (bestTarget != null && bestTarget != target)
                {
                    combat.SetTarget(bestTarget.Oid, TargetTypes.TARGETTYPES_TARGET_ENEMY);
                    target = bestTarget;
                }
            }

            combat.IsAttacking = true;
            FollowCombatTarget(player, target);
            TryUseAbilities();
        }

        public override void TryUseAbilities()
        {
            // Base bot logic: use basic attack if nothing else is defined.
        }

        protected void SimpleCast(Unit caster, Unit target, string description, ushort abilityId)
        {
            if (caster == null || target == null) return;
            if (caster.AbtInterface.IsCasting()) return;
            
            caster.AbtInterface.StartCast(caster, abilityId, 1);
        }

        protected Unit GetLowestHealthAlly(Player bot, float range)
        {
            if (bot.PriorityGroup != null)
            {
                var members = bot.PriorityGroup.Members;
                Player best = null;
                float bestPct = float.MaxValue;
                int rangeInt = (int)range;
                lock (members)
                {
                    foreach (var m in members)
                    {
                        if (m.IsDead) continue;
                        if (!bot.IsWithinRadiusFeet(m, rangeInt)) continue;
                        if (m.PctHealth < bestPct) { bestPct = m.PctHealth; best = m; }
                    }
                }
                return best;
            }
            return bot;
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

            // ── 2. Resurrection (Healers only, out of combat) ─────────────────
            if (player.Role == BotRole.Healer && !player.CbtInterface.IsInCombat)
            {
                if (CheckForDeadGroupMembers(player))
                    return;
            }

            // ── 3. Assist train / formation follow / Regrouping ───────────────
            Group group = player.PriorityGroup;
            if (group != null)
            {
                Player ma = group.MainAssist;
                
                // If MA is dead or missing, pick a temporary anchor (first alive member).
                if (ma == null || ma.IsDead)
                {
                    ma = group.Members.FirstOrDefault(m => !m.IsDead && m != player);
                }

                if (ma != null && ma != player)
                {
                    // Pick up the anchor's combat target if they are the real MA.
                    if (ma == group.MainAssist)
                    {
                        Unit maTarget = ma.CbtInterface.GetCurrentTarget();
                        if (maTarget != null && maTarget != player.CbtInterface.GetCurrentTarget() && !maTarget.IsDead)
                            player.AiInterface.ProcessCombatStart(maTarget);
                    }

                    // Follow anchor if the group has spread too far apart.
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

            Unit nearbyTarget = (group?.MainAssist == player) 
                ? SelectCombatTarget(player) 
                : player.AiInterface.GetAttackableUnit();

            if (nearbyTarget != null)
            {
                player.AiInterface.ProcessCombatStart(nearbyTarget);
                return;
            }

            // ── 4. Scenario objective ─────────────────────────────────────────
            if (player.ScnInterface.Scenario != null)
            {
                UpdateScenarioObjective(player, tick);
                return;
            }

            // ── 5. Open-world RvR objective march & capture ───────────────────
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
            // Use squared distance to avoid Math.Sqrt.
            BattlefieldObjective best = null;
            double bestSq = double.MaxValue;
            float px = player.X, py = player.Y, pz = player.Z;
            Realms realm = (Realms)player.Realm;
            foreach (var o in objectives)
            {
                if (o.OwningRealm == realm) continue;
                double dx = px - o.X, dy = py - o.Y, dz = pz - o.Z;
                double dsq = dx * dx + dy * dy + dz * dz;
                if (dsq < bestSq) { bestSq = dsq; best = o; }
            }

            CurrentTargetObjective = best;
        }

        private bool CheckForDeadGroupMembers(Player player)
        {
            if (player.PriorityGroup == null) return false;

            // Find resurrection ability if we haven't already.
            if (_rezAbilityId == 0)
            {
                var rezAbility = player.AbtInterface.GetAbilities().FirstOrDefault(ab => ab.ConstantInfo.AffectsDead);
                if (rezAbility != null)
                    _rezAbilityId = rezAbility.Entry;
            }

            if (_rezAbilityId == 0) return false;

            // Single pass: find nearest dead member within rez range using squared distance.
            Player deadMember = null;
            int deadMemberDist = int.MaxValue;
            foreach (var m in player.PriorityGroup.Members)
            {
                if (!m.IsDead) continue;
                int d = player.GetDistanceTo(m);
                if (d <= MAX_REZZ_DISTANCE_FT && d < deadMemberDist)
                {
                    deadMemberDist = d;
                    deadMember = m;
                }
            }

            if (deadMember != null)
            {
                if (deadMemberDist > 15)
                {
                    player.MvtInterface.Move(deadMember.WorldPosition);
                }
                else
                {
                    if (!player.AbtInterface.IsCasting())
                        player.AbtInterface.StartCast(player, _rezAbilityId, 1);
                }
                return true;
            }

            return false;
        }

        // ── Scenario logic ────────────────────────────────────────────────────

        private void UpdateScenarioObjective(Player player, long tick)
        {
            if (tick < _nextObjectiveCheck) return;
            _nextObjectiveCheck = tick + 2000;

            Scenario scenario = player.ScnInterface.Scenario;
            if (scenario == null) return;

            // Anchor logic: only the group anchor (MA or first alive) picks the objective.
            Group group = player.PriorityGroup;
            Player anchor = group?.MainAssist;
            if (anchor == null || anchor.IsDead)
                anchor = group?.Members.FirstOrDefault(m => !m.IsDead);

            // If we are not the anchor, we just follow the anchor via formation logic in Think().
            if (anchor != null && anchor != player) return;

            // Rebuild the objectives cache if the scenario changed (new instance).
            if (scenario != _lastCachedScenario)
            {
                _lastCachedScenario = scenario;
                _cachedScenarioObjectives = scenario.Region.Objects
                    .OfType<Object>()
                    .Where(o => (o is CapturePoint || o is HoldObject || o is ClickFlag || o is ProximityFlag)
                             && !o.IsDisposed)
                    .ToList();
            }
            else
            {
                // Prune disposed entries incrementally.
                _cachedScenarioObjectives?.RemoveAll(o => o.IsDisposed);
            }

            if (_cachedScenarioObjectives == null || _cachedScenarioObjectives.Count == 0) return;

            // Find the nearest active objective using squared-distance (no sqrt).
            Object targetObj = null;
            double bestDistSq = double.MaxValue;
            float px = player.X, py = player.Y, pz = player.Z;
            foreach (var o in _cachedScenarioObjectives)
            {
                if (!o.IsActive) continue;
                double dx = px - o.X, dy = py - o.Y, dz = pz - o.Z;
                double dsq = dx * dx + dy * dy + dz * dz;
                if (dsq < bestDistSq) { bestDistSq = dsq; targetObj = o; }
            }

            if (targetObj == null) return;

            // Check if our realm already owns it (if applicable).
            if (targetObj is CapturePoint cp && cp.OwningRealm == player.Realm) return;
            // (Other types like HoldObject/ClickFlag are usually interactive regardless of ownership).

            _currentTargetScenarioObject = targetObj;
            float dist = player.GetDistanceTo(targetObj);
            
            if (dist > 12)
            {
                player.MvtInterface.Move(targetObj.WorldPosition);
            }
            else
            {
                if (tick >= _nextInteractAttempt)
                {
                    _nextInteractAttempt = tick + INTERACT_COOLDOWN_MS;
                    targetObj.BeginInteraction(player);
                }
            }
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

        protected Unit SelectCombatTarget(Player player)
        {
            var enemies = player.AiInterface.RangedEnemies;
            if (enemies == null || enemies.Count == 0)
                return null;

            // Single pass: track best healer, best ranged DPS, best fallback — all using
            // squared distance so Math.Sqrt is never called.
            Unit bestHealer = null, bestRanged = null, bestFallback = null;
            double bestHealerSq = double.MaxValue, bestRangedSq = double.MaxValue, bestFallbackSq = double.MaxValue;
            float px = player.X, py = player.Y, pz = player.Z;

            lock (enemies)
            {
                foreach (Unit e in enemies)
                {
                    if (e.IsDead || !CombatInterface.CanAttack(player, e)) continue;
                    double dx = px - e.X, dy = py - e.Y, dz = pz - e.Z;
                    double dsq = dx * dx + dy * dy + dz * dz;

                    if (e is Player ep)
                    {
                        if (ep.Role == BotRole.Healer && dsq < bestHealerSq)
                        { bestHealerSq = dsq; bestHealer = ep; }
                        else if (ep.Role == BotRole.RangedDPS && dsq < bestRangedSq)
                        { bestRangedSq = dsq; bestRanged = ep; }
                    }

                    if (dsq < bestFallbackSq)
                    { bestFallbackSq = dsq; bestFallback = e; }
                }
            }

            return bestHealer ?? bestRanged ?? bestFallback;
        }
    }
}
