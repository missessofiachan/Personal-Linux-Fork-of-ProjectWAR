using WorldServer.World.Objects;
using GameData;
using WorldServer.World.Interfaces;

namespace WorldServer.World.AI.Bots
{
    public class WarriorPriestBotBrain : BotBrain
    {
        public WarriorPriestBotBrain(Unit unit) : base(unit)
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
                    SimpleCast(bot, ally, "Pious Restoration", 8265); // Group HoT
                
                // Sigmar's Shield (Heal on hit)
                SimpleCast(bot, bot, "Sigmar's Shield", 8267);
            }

            // 2. Self Buffs
            if (!bot.BuffInterface.HasBuffInAnyState(8269, null)) // Sigmar's Grace
                SimpleCast(bot, bot, "Sigmar's Grace", 8269);

            // 3. Offensive Combat
            if (target != null && CombatInterface.CanAttack(bot, target))
            {
                // Lifetap if someone needs healing
                if (ally != null && ally.PctHealth < 90)
                {
                    bot.CbtInterface.SetTarget(ally.Oid, TargetTypes.TARGETTYPES_TARGET_ALLY);
                    SimpleCast(bot, target, "Divine Assault", 8244);
                }

                // Execute
                if (target.PctHealth < 20)
                    SimpleCast(bot, target, "Hammer of Sigmar", 8272);

                // Basic attack + buff
                SimpleCast(bot, target, "Sigmar's Fist", 8253);
                
                // Spirit damage
                SimpleCast(bot, target, "Judgement", 8236);
            }
        }
    }
}
