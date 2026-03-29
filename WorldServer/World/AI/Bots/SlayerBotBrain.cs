using WorldServer.World.Objects;

namespace WorldServer.World.AI.Bots
{
    public class SlayerBotBrain : BotBrain
    {
        public SlayerBotBrain(Unit unit) : base(unit)
        {
        }

        public override void TryUseAbilities()
        {
            Player bot = _unit as Player;
            if (bot == null || bot.AbtInterface.IsCasting()) return;

            Unit target = bot.CbtInterface.GetCurrentTarget();
            if (target == null) return;

            if (bot.GetDistanceTo(target) < 5)
            {
                // Relentless Strike (1431)
                SimpleCast(bot, target, "Relentless Strike", 1431);

                // Deep Wound (1434)
                SimpleCast(bot, target, "Deep Wound", 1434);

                // Rampage (1459)
                SimpleCast(bot, target, "Rampage", 1459);
            }
        }
    }
}
