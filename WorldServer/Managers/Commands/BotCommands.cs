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

                if (BotManager.Instance.TryGetLoadedBotGroup(prefix) != null)
                {
                    player.SendClientMessage($"Bot group {prefix} is already active. Duplicate live spawns are blocked.");
                    return true;
                }

                if (BotManager.Instance.HasLoadedBotsWithPrefix(prefix))
                {
                    player.SendClientMessage($"Bot prefix {prefix} is partially active already. Clean up the existing bots before respawning.");
                    return true;
                }

                var group = BotManager.Instance.SpawnBotGroup(realm, tier, rr, prefix, player.Zone?.ZoneId ?? 0);
                if (group == null)
                    player.SendClientMessage($"Bot group {prefix} was not spawned. Check the WorldServer log for details.");
                else
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
