using WorldServer.World.Objects;

namespace WorldServer.World.AI.Bots
{
    public class KnightOfTheBlazingSunBotBrain : BotBrain
    {
        public KnightOfTheBlazingSunBotBrain(Unit unit) : base(unit)
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
                // Guard implementation
            }

            // 2. Combat Rotation
            if (bot.GetDistanceTo(target) < 5)
            {
                // Precision Strike (8005)
                SimpleCast(bot, target, "Precision Strike", 8005);

                // Shining Blade (8035)
                SimpleCast(bot, target, "Shining Blade", 8035);

                // Now's Our Chance! (8036)
                SimpleCast(bot, target, "Now's Our Chance!", 8036);
            }
        }
    }
}
