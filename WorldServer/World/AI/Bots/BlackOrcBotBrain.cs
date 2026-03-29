using WorldServer.World.Objects;

namespace WorldServer.World.AI.Bots
{
    public class BlackOrcBotBrain : BotBrain
    {
        public BlackOrcBotBrain(Unit unit) : base(unit)
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
                // Clobber (1664)
                SimpleCast(bot, target, "Clobber", 1664);

                // Big Slash (assume mirror of Knight's Shining Blade)
                // Actually I'll just use Clobber for now as it's a solid attack.
            }
        }
    }
}
