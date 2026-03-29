using WorldServer.World.Objects;

namespace WorldServer.World.AI.Bots
{
    public class ChosenBotBrain : BotBrain
    {
        public ChosenBotBrain(Unit unit) : base(unit)
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
                // Ravage (8323)
                SimpleCast(bot, target, "Ravage", 8323);

                // Seeping Wound (8320)
                SimpleCast(bot, target, "Seeping Wound", 8320);

                // Touch of Palsy (8338)
                SimpleCast(bot, target, "Touch of Palsy", 8338);
            }
        }
    }
}
