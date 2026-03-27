using Common;
using FrameWork;
using System.Collections.Generic;
using WorldServer.World.Objects;

namespace WorldServer.NetWork
{
    public class BotClient : GameClient
    {
        public BotClient(TCPManager srv)
            : base(srv)
        {
            // Set state to Playing to allow interactions
            State = (int)eClientState.Playing;
        }

        public override void SendPacket(PacketOut packet)
        {
            // Do nothing, we swallow the packets
        }

        public override bool SendPacketNoBlock(PacketOut packet)
        {
            // Always successful
            return true;
        }

        public override bool SendPacketsNoBlock(List<PacketOut> packetList, int lengthPerSend)
        {
            // Always successful
            return true;
        }

        public override void Disconnect(string reason)
        {
            // Bots don't disconnect normally, but if asked, just set state
            State = (int)eClientState.Disconnected;
            if (Plr != null)
                Plr.RemoveFromWorld();
        }
    }
}
