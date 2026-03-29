using WorldServer.World.Objects;
using GameData;
using WorldServer.World.Interfaces;

namespace WorldServer.World.AI.Bots
{
    public class DiscipleOfKhaineBotBrain : BotBrain
    {
        public DiscipleOfKhaineBotBrain(Unit unit) : base(unit)
        {
        }

        public override void TryUseAbilities()
        {
            Player bot = _unit as Player;
            if (bot == null || bot.IsDisabled || bot.IsStaggered || bot.AbtInterface.IsCasting())
                return;

            Unit target = bot.CbtInterface.GetCurrentTarget();
            Unit ally = GetLowestHealthAlly(bot, 100);

            // 1. Healing / Defensive Buffs
            if (ally != null && ally.PctHealth < 85)
            {
                bot.CbtInterface.SetTarget(ally.Oid, TargetTypes.TARGETTYPES_TARGET_ALLY);

                if (ally.PctHealth < 40)
                    SimpleCast(bot, ally, "Khaine's Vigor", 9565); // Group HoT
                
                // Transfer Essence (Heal on hit)
                SimpleCast(bot, bot, "Transfer Essence", 9561);
            }

            // 2. Self Buffs
            if (!bot.BuffInterface.HasBuffInAnyState(9568, null)) // Covenant of Tenacity (assuming ID based on context)
                SimpleCast(bot, bot, "Covenant of Tenacity", 9568);

            // 3. Offensive Combat
            if (target != null && CombatInterface.CanAttack(bot, target))
            {
                // Lifetap if someone needs healing
                if (ally != null && ally.PctHealth < 90)
                {
                    bot.CbtInterface.SetTarget(ally.Oid, TargetTypes.TARGETTYPES_TARGET_ALLY);
                    SimpleCast(bot, target, "Rend Soul", 9554);
                }

                // Execute
                if (target.PctHealth < 20)
                    SimpleCast(bot, target, "Khaine's Embrace", 9572);

                // Basic attack + bleed
                SimpleCast(bot, target, "Lacerate", 9541);
            }
        }
    }
}
