using FrameWork;

namespace WorldServer.NetWork.Handler
{
    /// <summary>
    /// Silent acknowledgement handlers for client opcodes that have no server-side action
    /// but are sent legitimately by the client during normal gameplay.
    /// These prevent "Can not Handle opcode" error log spam.
    /// </summary>
    public class AckHandlers : IPacketHandler
    {
        /// <summary>
        /// 0x0E - F_CURRENT_EVENTS: client requests current world events info.
        /// Silently acknowledged; the server does not currently implement current events.
        /// </summary>
        [PacketHandler(PacketHandlerType.TCP, (int)Opcodes.F_CURRENT_EVENTS, (int)eClientState.WorldEnter, "onCurrentEvents")]
        public static void F_CURRENT_EVENTS(BaseClient client, PacketIn packet)
        {
        }

        /// <summary>
        /// 0x83 - S_WORLD_SENT: client sends an acknowledgement after receiving world data.
        /// This is a client-to-server ack; no action required.
        /// </summary>
        [PacketHandler(PacketHandlerType.TCP, (int)Opcodes.S_WORLD_SENT, (int)eClientState.WorldEnter, "onWorldSentAck")]
        public static void S_WORLD_SENT(BaseClient client, PacketIn packet)
        {
        }
    }
}
