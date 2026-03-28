using System;
using GameData;

namespace Common
{
    public struct CharacterIdentityRecord
    {
        public CharacterIdentityRecord(byte careerLine, byte race, byte realm, string careerName)
        {
            CareerLine = careerLine;
            Race = race;
            Realm = realm;
            CareerName = careerName ?? string.Empty;
        }

        public byte CareerLine { get; }
        public byte Race { get; }
        public byte Realm { get; }
        public string CareerName { get; }
    }

    public static class CharacterIdentityCatalog
    {
        public static readonly CharacterIdentityRecord[] CareerLineRecords =
        {
            default(CharacterIdentityRecord),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_IRON_BREAKER, (byte)Races.RACES_DWARF, (byte)Realms.REALMS_REALM_ORDER, "Ironbreaker"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_SLAYER, (byte)Races.RACES_DWARF, (byte)Realms.REALMS_REALM_ORDER, "Slayer"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_RUNE_PRIEST, (byte)Races.RACES_DWARF, (byte)Realms.REALMS_REALM_ORDER, "Runepriest"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_ENGINEER, (byte)Races.RACES_DWARF, (byte)Realms.REALMS_REALM_ORDER, "Engineer"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_BLACK_ORC, (byte)Races.RACES_ORC, (byte)Realms.REALMS_REALM_DESTRUCTION, "Black Orc"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_CHOPPA, (byte)Races.RACES_ORC, (byte)Realms.REALMS_REALM_DESTRUCTION, "Choppa"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_SHAMAN, (byte)Races.RACES_GOBLIN, (byte)Realms.REALMS_REALM_DESTRUCTION, "Shaman"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_SQUIG_HERDER, (byte)Races.RACES_GOBLIN, (byte)Realms.REALMS_REALM_DESTRUCTION, "Squig Herder"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_WITCH_HUNTER, (byte)Races.RACES_EMPIRE, (byte)Realms.REALMS_REALM_ORDER, "Witch Hunter"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_KNIGHT_OF_THE_BLAZING_SUN, (byte)Races.RACES_EMPIRE, (byte)Realms.REALMS_REALM_ORDER, "Knight of the Blazing Sun"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_BRIGHT_WIZARD, (byte)Races.RACES_EMPIRE, (byte)Realms.REALMS_REALM_ORDER, "Bright Wizard"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_WARRIOR_PRIEST, (byte)Races.RACES_EMPIRE, (byte)Realms.REALMS_REALM_ORDER, "Warrior Priest"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_CHOSEN, (byte)Races.RACES_CHAOS, (byte)Realms.REALMS_REALM_DESTRUCTION, "Chosen"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_MARAUDER, (byte)Races.RACES_CHAOS, (byte)Realms.REALMS_REALM_DESTRUCTION, "Marauder"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_ZEALOT, (byte)Races.RACES_CHAOS, (byte)Realms.REALMS_REALM_DESTRUCTION, "Zealot"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_MAGUS, (byte)Races.RACES_CHAOS, (byte)Realms.REALMS_REALM_DESTRUCTION, "Magus"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_SWORDMASTER, (byte)Races.RACES_HIGH_ELF, (byte)Realms.REALMS_REALM_ORDER, "Swordmaster"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_SHADOW_WARRIOR, (byte)Races.RACES_HIGH_ELF, (byte)Realms.REALMS_REALM_ORDER, "Shadow Warrior"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_WHITE_LION, (byte)Races.RACES_HIGH_ELF, (byte)Realms.REALMS_REALM_ORDER, "White Lion"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_ARCHMAGE, (byte)Races.RACES_HIGH_ELF, (byte)Realms.REALMS_REALM_ORDER, "Archmage"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_BLACK_GUARD, (byte)Races.RACES_DARK_ELF, (byte)Realms.REALMS_REALM_DESTRUCTION, "Blackguard"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_WITCH_ELF, (byte)Races.RACES_DARK_ELF, (byte)Realms.REALMS_REALM_DESTRUCTION, "Witch Elf"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_DISCIPLE_OF_KHAINE, (byte)Races.RACES_DARK_ELF, (byte)Realms.REALMS_REALM_DESTRUCTION, "Disciple of Khaine"),
            new CharacterIdentityRecord((byte)CareerLine.CAREERLINE_SORCERER, (byte)Races.RACES_DARK_ELF, (byte)Realms.REALMS_REALM_DESTRUCTION, "Sorcerer")
        };

        public static bool TryGetByCareerLine(byte careerLine, out CharacterIdentityRecord record)
        {
            if (careerLine > 0
                && careerLine < CareerLineRecords.Length
                && CareerLineRecords[careerLine].CareerLine != 0)
            {
                record = CareerLineRecords[careerLine];
                return true;
            }

            record = default(CharacterIdentityRecord);
            return false;
        }

        public static string GetCareerName(byte careerLine)
        {
            CharacterIdentityRecord record;
            return TryGetByCareerLine(careerLine, out record)
                ? record.CareerName
                : "Career " + careerLine;
        }

        public static string GetRaceName(byte race)
        {
            if (race > 0 && race < Constants.RacesName.Length)
                return Constants.RacesName[race];

            return "Race " + race;
        }

        public static bool Matches(CharacterInfo info)
        {
            CharacterIdentityRecord record;
            return info != null
                && TryGetByCareerLine(info.CareerLine, out record)
                && info.Realm == record.Realm
                && string.Equals(info.CareerName ?? string.Empty, record.CareerName, StringComparison.Ordinal);
        }
    }
}
