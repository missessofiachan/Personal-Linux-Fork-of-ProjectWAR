using Common;
using GameData;
using WorldServer.World.Objects;
using WorldServer.World.Positions;
using System;
using System.Collections.Generic;
using static WorldServer.Managers.Commands.BaseCommands;
using static WorldServer.Managers.Commands.GMUtils;

namespace WorldServer.Managers.Commands
{
    internal static class BotCommands
    {
        public static bool BotSpawn(Player player, ref List<string> values)
        {
            try
            {
                if (values.Count < 4)
                {
                    player.SendClientMessage("Usage: .bot spawn <realm> <tier> <rr> <namePrefix>");
                    return true;
                }

                Realms realm = (Realms)int.Parse(GetString(ref values));
                int tier = int.Parse(GetString(ref values));
                int rr = int.Parse(GetString(ref values));
                string prefix = GetTotalString(ref values);

                BotManager.Instance.SpawnBotGroup(realm, tier, rr, prefix, player.Zone?.ZoneId ?? 0);
                player.SendClientMessage($"Spawned bot group {prefix} for realm {realm} at Warcamp in zone {player.Zone?.ZoneId ?? 0}");
            }
            catch (Exception ex)
            {
                player.SendClientMessage($"Error spawning bots: {ex.Message}");
            }
            return true;
        }
    }
}
