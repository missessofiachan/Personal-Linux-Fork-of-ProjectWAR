using Common;
using GameData;
using WorldServer.World.Objects;

namespace WorldServer.World.AI.Bots
{
    public static class BotBrainFactory
    {
        public static BotBrain CreateBotBrain(Player player)
        {
            switch ((CareerLine)player.Info.CareerLine)
            {
                case CareerLine.CAREERLINE_IRON_BREAKER: return new IronBreakerBotBrain(player);
                case CareerLine.CAREERLINE_SLAYER: return new SlayerBotBrain(player);
                case CareerLine.CAREERLINE_RUNE_PRIEST: return new RunepriestBotBrain(player);
                case CareerLine.CAREERLINE_ENGINEER: return new EngineerBotBrain(player);
                case CareerLine.CAREERLINE_BLACK_ORC: return new BlackOrcBotBrain(player);
                case CareerLine.CAREERLINE_CHOPPA: return new ChoppaBotBrain(player);
                case CareerLine.CAREERLINE_SHAMAN: return new ShamanBotBrain(player);
                case CareerLine.CAREERLINE_SQUIG_HERDER: return new SquigHerderBotBrain(player);
                case CareerLine.CAREERLINE_WITCH_HUNTER: return new WitchHunterBotBrain(player);
                case CareerLine.CAREERLINE_KNIGHT_OF_THE_BLAZING_SUN: return new KnightOfTheBlazingSunBotBrain(player);
                case CareerLine.CAREERLINE_BRIGHT_WIZARD: return new BrightWizardBotBrain(player);
                case CareerLine.CAREERLINE_WARRIOR_PRIEST: return new WarriorPriestBotBrain(player);
                case CareerLine.CAREERLINE_CHOSEN: return new ChosenBotBrain(player);
                case CareerLine.CAREERLINE_MARAUDER: return new MarauderBotBrain(player);
                case CareerLine.CAREERLINE_ZEALOT: return new ZealotBotBrain(player);
                case CareerLine.CAREERLINE_MAGUS: return new MagusBotBrain(player);
                case CareerLine.CAREERLINE_SWORDMASTER: return new SwordmasterBotBrain(player);
                case CareerLine.CAREERLINE_SHADOW_WARRIOR: return new ShadowWarriorBotBrain(player);
                case CareerLine.CAREERLINE_WHITE_LION: return new WhiteLionBotBrain(player);
                case CareerLine.CAREERLINE_ARCHMAGE: return new ArchmageBotBrain(player);
                case CareerLine.CAREERLINE_BLACK_GUARD: return new BlackGuardBotBrain(player);
                case CareerLine.CAREERLINE_WITCH_ELF: return new WitchElfBotBrain(player);
                case CareerLine.CAREERLINE_DISCIPLE_OF_KHAINE: return new DiscipleOfKhaineBotBrain(player);
                case CareerLine.CAREERLINE_SORCERER: return new SorcererBotBrain(player);
                default: return new BotBrain(player);
            }
        }
    }
}
