using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Common;
using FrameWork;
using WorldServer.World.Objects;

namespace WorldServer.NetWork
{
    public enum eClientState
    {
        NotConnected = 0x00,
        Connecting = 0x01,
        CharScreen = 0x02,
        WorldEnter = 0x03,
        Playing = 0x04,
        Linkdead = 0x05,
        Disconnected = 0x06
    } ;

    public class GameClient : BaseClient
    {
        public Account _Account = null;
        public Player Plr = null;
        private Thread _logThread = null;
        private readonly AutoResetEvent _packetLogWakeEvent = new AutoResetEvent(false);
        private readonly object _packetLogThreadLock = new object();
        private volatile bool _packetLogThreadStopRequested;
        private List<string> _packetLog = new List<string>();

        private CircularBuffer<object> _pLogBuf = new CircularBuffer<object>(100);

        public CircularBuffer<object> PLogBuf
        {
            get
            {
                return _pLogBuf;
            }
        }

        public GameClient(TCPManager srv)
            : base(srv)
        {

        }

        public override void OnConnect()
        {
            Log.Debug("WorldServer..Connection", GetIp());
            State = (int)eClientState.Connecting;
        }
        public override void OnDisconnect(string reason)
        {
            string ipString = GetIp();

            if (ipString == "disconnected")
                ipString = "Disconnected client";

            Log.Debug($"{ _Account?.Username ?? "Unknown user" } ({ipString}) disconnected", reason);

            if (_logThread != null)
                StopPacketLogThread();

            if (Plr != null)
            {
                Plr.Client = null;
                Plr.Destroy();
            }
        }

        public bool IsPlaying()
        {
            return State == (int)eClientState.Playing;
        }
        public bool HasPlayer()
        {
            return Plr != null;
        }
        public bool HasAccount()
        {
            return _Account != null;
        }

        private ushort _opcode;
        private long _packetSize;

        public bool ReadingData;
        public ushort SequenceID,SessionID,Unk1;
        public byte Unk2;

        private void LogInPacket(PacketIn packet)
        {
            EnsurePacketLogThread();

            lock(_packetLog)
            {
                _packetLog.Add(Utils.ToLogHexString((byte)packet.Opcode, false, packet.ToArray()));
            }

            _packetLogWakeEvent.Set();
        }

        private void LogOutPacket(PacketOut packet)
        {
            EnsurePacketLogThread();

            lock (_packetLog)
            {
                _packetLog.Add(Utils.ToLogHexString((byte)packet.Opcode, true, packet.ToArray()));
            }

            _packetLogWakeEvent.Set();
        }

        private void EnsurePacketLogThread()
        {
            if (_logThread != null)
                return;

            lock (_packetLogThreadLock)
            {
                if (_logThread != null)
                    return;

                _packetLogThreadStopRequested = false;
                _logThread = new Thread(PacketLogThread)
                {
                    IsBackground = true,
                    Name = "GameClientPacketLog"
                };
                _logThread.Start();
            }
        }

        private void StopPacketLogThread()
        {
            Thread logThread;
            lock (_packetLogThreadLock)
            {
                logThread = _logThread;
                if (logThread == null)
                    return;

                _packetLogThreadStopRequested = true;
                _packetLogWakeEvent.Set();
            }

            try
            {
                if (!logThread.Join(2000))
                    Log.Notice("GameClient", "Timed out while waiting for the packet log thread to stop.");
            }
            catch (Exception ex)
            {
                Log.Error("GameClient", "Failed to stop packet log thread: " + ex.Message);
            }

            lock (_packetLogThreadLock)
            {
                if (ReferenceEquals(_logThread, logThread))
                    _logThread = null;
            }
        }

        private void PacketLogThread()
        {
            try
            {
                while (true)
                {
                    FlushPacketLog();
                    if (_packetLogThreadStopRequested)
                        return;

                    Socket socket = Socket;
                    if (socket == null || !socket.Connected)
                        return;

                    _packetLogWakeEvent.WaitOne(5000);
                }
            }
            finally
            {
                lock (_packetLogThreadLock)
                {
                    if (ReferenceEquals(_logThread, Thread.CurrentThread))
                        _logThread = null;
                }
            }
        }
        private void FlushPacketLog()
        {
            var packets = new List<String>();
            lock(_packetLog)
            {
                packets = _packetLog.ToList();
                _packetLog.Clear();
            }

            if (_Account != null)
            {
                if (!Directory.Exists("PacketLogs"))
                    Directory.CreateDirectory("PacketLogs");

                string file = Path.Combine("PacketLogs", _Account.Username + ".txt");
                StringBuilder log = new StringBuilder();
                foreach (var packet in packets)
                    log.AppendLine(packet);

                File.AppendAllText(file, log.ToString());
            }
        }

        public override void SendPacket(PacketOut packet)
        {
            if(PacketLog)
                LogOutPacket(packet);

            PLogBuf.Enqueue(packet);

            base.SendPacket(packet);
        }

        public override bool SendPacketNoBlock(PacketOut packet)
        {
            if (PacketLog)
                LogOutPacket(packet);

            PLogBuf.Enqueue(packet);


            return base.SendPacketNoBlock(packet);
        }

        public override bool SendPacketsNoBlock(List<PacketOut> packetList, int lengthPerSend)
        {
            foreach (PacketOut packet in packetList)
            {
                if (PacketLog)
                    LogOutPacket(packet);

                PLogBuf.Enqueue(packet);
            }

            return base.SendPacketsNoBlock(packetList, lengthPerSend);
        }

        protected override void OnReceive(byte[] packetBuffer)
        {
            // Wrap the input stream in a PacketIn
            Log.Debug("HandlePacket", $"Packet...{packetBuffer.Length}");
            PacketIn inStream = new PacketIn(packetBuffer, 0, packetBuffer.Length, true, true);

            lock (this)
            {
                long bufferLength = inStream.Length;

                while (bufferLength > 0)
                {
                    // Read the header
                    if (!ReadingData)
                    {
                        if (bufferLength < 2)
                        {
                            Log.Debug("OnReceive", "Invalid header (buffer length " + bufferLength + ")");
                            return;
                        }

                        _packetSize = inStream.GetUint16();
                        bufferLength -= 2;

                        if (bufferLength < _packetSize + 10)
                            return;

                        inStream.Size = (ulong)_packetSize+10;
                        Decrypt(inStream);

                        SequenceID = inStream.GetUint16();
                        SessionID = inStream.GetUint16();
                        Unk1 = inStream.GetUint16();
                        Unk2 = inStream.GetUint8();
                        _opcode = inStream.GetUint8();
                        bufferLength -= 8;

#if DEBUG
                        if (bufferLength > _packetSize + 2)
                            Log.Debug("OnReceive", "Packet contains multiple opcodes " + bufferLength + ">" + (_packetSize + 2));
#endif

                        ReadingData = true;
                    }
                    else
                    {
                        ReadingData = false;

                        if (bufferLength >= _packetSize + 2)
                        {
                            byte[] bPack = new byte[_packetSize+2];
                            inStream.Read(bPack, 0, (int)(_packetSize + 2));

                            PacketIn packet = new PacketIn(bPack, 0, bPack.Length)
                            {
                                Opcode = _opcode,
                                Size = (ulong) _packetSize
                            };

                            if (PacketLog)
                            {
                                LogInPacket(packet);
                            }

                            PLogBuf.Enqueue(packet);

                            if (Plr != null && Plr.IsInWorld())
                                Plr.ReceivePacket(packet);
                            else
                                Server.HandlePacket(this, packet);

                            Log.Tcp("PacketSize", bPack, 0, bPack.Length);

                            bufferLength -= _packetSize + 2;
                        }
                        else
                        {
                            Log.Error("OnReceive", "Packet size smaller than total received bytes: " + bufferLength + "<" + (_packetSize + 2));
                            break;
                        }
                    }
                }
            }
        }
    }
}
