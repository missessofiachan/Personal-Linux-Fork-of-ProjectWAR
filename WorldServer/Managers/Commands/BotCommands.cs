using Common;
using WorldServer.World.Objects;
using WorldServer.World.Positions;
using System;

namespace WorldServer.Managers.Commands
{
    internal static class BotCommands
    {
        public static void BotSpawn(Player player, string text)
        {
            try
            {
                string[] args = text.Split(' ');
                if (args.Length < 4)
                {
                    player.SendClientMessage(""Usage: .bot spawn <realm> <tier> <rr> <namePrefix>"");
                    return;
                }

                Realms realm = (Realms)int.Parse(args[0]);
                int tier = int.Parse(args[1]);
                int rr = int.Parse(args[2]);
                string prefix = args[3];

                BotManager.Instance.SpawnBotGroup(realm, tier, rr, prefix, player.ZoneId);
                player.SendClientMessage($""Spawned bot group {prefix} for realm {realm} at Warcamp in zone {player.ZoneId}"");
            }
            catch (Exception ex)
            {
                player.SendClientMessage($""Error spawning bots: {ex.Message}"");
            }
        }
    }
}
