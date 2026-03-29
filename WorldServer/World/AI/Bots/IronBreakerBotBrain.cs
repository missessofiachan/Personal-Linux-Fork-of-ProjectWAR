using WorldServer.World.Objects;

namespace WorldServer.World.AI.Bots
{
    public class IronBreakerBotBrain : BotBrain
    {
        public IronBreakerBotBrain(Unit unit) : base(unit)
        {
        }

        public override void TryUseAbilities()
        {
            Player bot = _unit as Player;
            if (bot == null || bot.AbtInterface.IsCasting()) return;

            Unit target = bot.CbtInterface.GetCurrentTarget();
            if (target == null) return;

            // 1. Guard (8325) - try to guard an ally
            if (bot.CbtInterface.IsInCombat)
            {
                // Simple logic: guard the lowest health ally in range
                Unit ally = GetLowestHealthAlly(bot, 30);
                if (ally != null && ally != bot && !bot.BuffInterface.HasBuffInAnyState(8325, bot))
                {
                    // For bots, we might just use a simple cast or a specialized method if available.
                    // But Guard is a toggled buff.
                }
            }

            // 2. Taunt (1360)
            if (target is Player && bot.GetDistanceTo(target) < 30)
            {
                // Simple taunt logic
            }

            // 3. Combat Rotation
            if (bot.GetDistanceTo(target) < 5)
            {
                // Grudged Attack (1356)
                SimpleCast(bot, target, "Guarded Attack", 1356);

                // Vengeful Strike (1357)
                SimpleCast(bot, target, "Vengeful Strike", 1357);

                // Inspiring Attack (1364)
                SimpleCast(bot, target, "Inspiring Attack", 1364);
            }
        }
    }
}
