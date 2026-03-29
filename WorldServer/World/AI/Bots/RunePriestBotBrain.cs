using WorldServer.World.Objects;

namespace WorldServer.World.AI.Bots
{
    public class RunepriestBotBrain : BotBrain
    {
        public RunepriestBotBrain(Unit unit) : base(unit)
        {
        }

        public override void TryUseAbilities()
        {
            Player bot = _unit as Player;
            if (bot == null || bot.AbtInterface.IsCasting()) return;

            // Healer logic: prioritize healing allies
            Unit ally = GetLowestHealthAlly(bot, 100);
            if (ally != null && ally.PctHealth < 95)
            {
                if (ally.PctHealth < 40)
                    SimpleCast(bot, ally, "Grungni's Gift", 1587); // Big heal
                else if (ally.PctHealth < 70)
                    SimpleCast(bot, ally, "Rune of Mending", 1599); // Fast heal

                if (!ally.BuffInterface.HasBuffInAnyState(1590, null))
                    SimpleCast(bot, ally, "Rune of Regeneration", 1590); // HoT
            }
        }
    }
}
