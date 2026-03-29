using WorldServer.World.Objects;

namespace WorldServer.World.AI.Bots
{
    public class ZealotBotBrain : BotBrain
    {
        public ZealotBotBrain(Unit unit) : base(unit)
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
                    SimpleCast(bot, ally, "Flash of Chaos", 8569); // Big heal
                else if (ally.PctHealth < 70)
                    SimpleCast(bot, ally, "Elixir of Dark Blessings", 8566); // Fast heal

                if (!ally.BuffInterface.HasBuffInAnyState(8557, null))
                    SimpleCast(bot, ally, "Leaping Alteration", 8557); // HoT
            }
        }
    }
}
