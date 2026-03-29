using WorldServer.World.Objects;

namespace WorldServer.World.AI.Bots
{
    public class BlackGuardBotBrain : BotBrain
    {
        public BlackGuardBotBrain(Unit unit) : base(unit)
        {
        }

        public override void TryUseAbilities()
        {
            Player bot = _unit as Player;
            if (bot == null || bot.AbtInterface.IsCasting()) return;

            Unit target = bot.CbtInterface.GetCurrentTarget();
            if (target == null) return;

            // 1. Guard (8325)
            Unit ally = GetLowestHealthAlly(bot, 30);
            if (ally != null && ally != bot && !bot.BuffInterface.HasBuffInAnyState(8325, bot))
            {
                // Guard implementation for bots
            }

            // 2. Combat Rotation
            if (bot.GetDistanceTo(target) < 5)
            {
                // Hateful Strike (9315)
                SimpleCast(bot, target, "Hateful Strike", 9315);

                // Blade of Ruin (9342)
                SimpleCast(bot, target, "Blade of Ruin", 9342);

                // Choking Fury (9349)
                SimpleCast(bot, target, "Choking Fury", 9349);
            }
        }
    }
}
