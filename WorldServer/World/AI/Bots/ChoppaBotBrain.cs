using WorldServer.World.Objects;

namespace WorldServer.World.AI.Bots
{
    public class ChoppaBotBrain : BotBrain
    {
        public ChoppaBotBrain(Unit unit) : base(unit)
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
                // Bleed Em Out (1771)
                SimpleCast(bot, target, "Bleed Em Out", 1771);

                // Can't Stop Da Chop (1746)
                SimpleCast(bot, target, "Can't Stop Da Chop", 1746);

                // Reckless Blow (1761)
                SimpleCast(bot, target, "Reckless Blow", 1761);
            }
        }
    }
}
