using MYPHandler;
using NLog;
using nsHashDictionary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Launcher
{
    /// <summary>
    /// Manages the launcher's TCP session, packet processing, and client patch/start workflow.
    /// </summary>
    public static class Client
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly AsyncCallback AsyncTcpSendCallbackDelegate = AsyncTcpSendCallback;
        private static readonly AsyncCallback ReceiveCallback = OnReceiveHandler;
        private static readonly Queue<byte[]> TcpQueue = new Queue<byte[]>(256);
        private static byte[] _tcpSendBuffer = new byte[65000];
        private static bool _isSendingTcp;
        private static byte[] _packetBuffer = new byte[2048];
        private static int _packetBufferOffset;
        private static Socket _socket;

        /// <summary>
        /// Gets or sets the launcher protocol version sent during the initial handshake.
        /// </summary>
        public static int Version { get; set; } = 1;

        /// <summary>
        /// Gets or sets the local server IP address used by the launcher.
        /// </summary>
        public static string LocalServerIP { get; set; } = "127.0.0.1";

        /// <summary>
        /// Gets or sets the local server TCP port used by the launcher.
        /// </summary>
        public static int LocalServerPort { get; set; } = 8000;

        /// <summary>
        /// Gets or sets the test server TCP port used by the launcher.
        /// </summary>
        public static int TestServerPort { get; set; } = 8000;

        /// <summary>
        /// Gets or sets a value indicating whether the launcher has completed the startup handshake.
        /// </summary>
        public static bool Started { get; set; }

        /// <summary>
        /// Gets or sets the current account name used for login.
        /// </summary>
        public static string User { get; set; }

        /// <summary>
        /// Gets or sets the authentication token returned by the launcher server.
        /// </summary>
        public static string AuthToken { get; set; }

        /// <summary>
        /// Gets or sets the configured client language.
        /// </summary>
        public static string Language { get; set; } = "English";

        /// <summary>
        /// Writes a diagnostic message to the launcher form.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static void PrintStatus(string message)
        {
            LauncherForm.Instance?.Print(message);
        }

        /// <summary>
        /// Opens a TCP connection to the launcher server and starts the handshake.
        /// </summary>
        /// <param name="ip">The server IP address.</param>
        /// <param name="port">The server TCP port.</param>
        /// <returns><c>true</c> if the connection succeeds; otherwise, <c>false</c>.</returns>
        public static bool Connect(string ip, int port)
        {
            try
            {
                Close();

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Logger.Info($"Connecting to Launcher Server {ip}:{port}");
                _socket.Connect(ip, port);
                ConfigureKeepAlive();
                BeginReceive();
                SendVersionCheck();
            }
            catch (Exception exception)
            {
                ShowConnectionError(ip, port, exception.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Closes the active TCP socket if one exists.
        /// </summary>
        public static void Close()
        {
            try
            {
                _socket?.Close();
                _socket = null;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Gets the launcher directory that contains the configuration file.
        /// </summary>
        /// <returns>The absolute launcher directory path.</returns>
        private static string GetLauncherDirectory()
        {
            return Application.StartupPath;
        }

        /// <summary>
        /// Gets the WAR client root directory that contains <c>WAR.exe</c> and the archive files.
        /// </summary>
        /// <returns>The WAR client root directory.</returns>
        private static DirectoryInfo GetWarDirectory()
        {
            return Directory.GetParent(GetLauncherDirectory());
        }

        /// <summary>
        /// Combines a file name with the WAR client root.
        /// </summary>
        /// <param name="warDirectory">The WAR client root directory.</param>
        /// <param name="fileName">The file name to locate.</param>
        /// <returns>The absolute file path.</returns>
        private static string GetWarFilePath(DirectoryInfo warDirectory, string fileName)
        {
            if (warDirectory == null)
                throw new DirectoryNotFoundException("Unable to determine the WAR client root directory from the launcher location.");

            return Path.Combine(warDirectory.FullName, fileName);
        }

        /// <summary>
        /// Gets the path to the launcher's login configuration file.
        /// </summary>
        /// <returns>The absolute launcher configuration path.</returns>
        private static string GetLoginConfigPath()
        {
            return Path.Combine(GetLauncherDirectory(), "mythloginserviceconfig.xml");
        }

        /// <summary>
        /// Updates the user's client language identifier inside <c>UserSettings.xml</c>.
        /// </summary>
        public static void UpdateLanguage()
        {
            if (Language.Length <= 0)
                return;

            int languageId = GetLanguageId(Language);
            string userSettingsPath = Path.Combine(GetWarFilePath(GetWarDirectory(), "user"), "UserSettings.xml");

            try
            {
                using (StreamReader reader = new StreamReader(userSettingsPath))
                {
                    string line;
                    StringBuilder totalStream = new StringBuilder();
                    while ((line = reader.ReadLine()) != null)
                    {
                        PrintStatus(line);
                        int position = line.IndexOf("Language id=");
                        if (position > 0)
                        {
                            position = line.IndexOf("\"") + 1;
                            int endPosition = line.LastIndexOf("\"");
                            line = line.Remove(position, endPosition - position);
                            line = line.Insert(position, languageId.ToString());
                        }

                        totalStream.AppendLine(line);
                    }

                    File.WriteAllText(userSettingsPath, totalStream.ToString());
                }
            }
            catch (Exception exception)
            {
                PrintStatus("Writing : " + exception);
            }
        }

        /// <summary>
        /// Requests the latest realm list from the launcher server.
        /// </summary>
        public static void RequestRealmList()
        {
            PacketOut packet = new PacketOut((byte)Opcodes.CL_INFO);
            SendTCP(packet);
        }

        /// <summary>
        /// Sends a launcher packet after finalizing its length header.
        /// </summary>
        /// <param name="packet">The packet to transmit.</param>
        public static void SendTCP(PacketOut packet)
        {
            Logger.Info($"Sending TCP Packet {packet.Opcode}");
            packet.WritePacketLength();
            SendTCP(packet.ToArray());
        }

        /// <summary>
        /// Sends a raw byte buffer over the active TCP connection.
        /// </summary>
        /// <param name="buffer">The serialized packet buffer.</param>
        public static void SendTCP(byte[] buffer)
        {
            if (_tcpSendBuffer == null)
                return;

            if (_socket == null || !_socket.Connected)
                return;

            try
            {
                lock (TcpQueue)
                {
                    if (_isSendingTcp)
                    {
                        TcpQueue.Enqueue(buffer);
                        return;
                    }

                    _isSendingTcp = true;
                }

                Buffer.BlockCopy(buffer, 0, _tcpSendBuffer, 0, buffer.Length);
                _socket.BeginSend(_tcpSendBuffer, 0, buffer.Length, SocketFlags.None, AsyncTcpSendCallbackDelegate, null);
            }
            catch (Exception exception)
            {
                Logger.Trace(exception.Message);
                Close();
            }
        }

        /// <summary>
        /// Sends a packet buffer without rewriting its packet length.
        /// </summary>
        /// <param name="packet">The packet to transmit.</param>
        public static void SendTCPRaw(PacketOut packet)
        {
            SendTCP(packet.ToArray());
        }

        /// <summary>
        /// Begins an asynchronous read on the active TCP socket.
        /// </summary>
        public static void BeginReceive()
        {
            Logger.Debug($"Socket Connected {_socket?.Connected}");

            if (_socket == null || !_socket.Connected)
                return;

            int bufferSize = _packetBuffer.Length;
            if (_packetBufferOffset >= bufferSize)
            {
                Close();
                return;
            }

            _socket.BeginReceive(_packetBuffer, _packetBufferOffset, bufferSize - _packetBufferOffset, SocketFlags.None, ReceiveCallback, null);
        }

        /// <summary>
        /// Parses a fully buffered packet and dispatches it to the opcode handler.
        /// </summary>
        /// <param name="packet">The packet to process.</param>
        public static void ProcessReceivedPacket(PacketIn packet)
        {
            lock (packet)
            {
                packet.Size = packet.GetUint32();
                packet.Opcode = packet.GetUint8();
                Logger.Debug($"ProcessReceivedPacket size : {packet.Size} opcode : {packet.Opcode}");
                HandlePacket(packet);
            }
        }

        /// <summary>
        /// Routes an incoming launcher packet to the appropriate handler.
        /// </summary>
        /// <param name="packet">The packet to handle.</param>
        public static void HandlePacket(PacketIn packet)
        {
            if (!Enum.IsDefined(typeof(Opcodes), (byte)packet.Opcode))
            {
                PrintStatus($"Invalid opcode : {packet.Opcode:X02}");
                return;
            }

            Logger.Trace($"HandlePacket{packet}");

            switch ((Opcodes)packet.Opcode)
            {
                case Opcodes.LCR_CHECK:
                    HandleCheckPacket(packet);
                    break;
                case Opcodes.LCR_START:
                    HandleStartPacket(packet);
                    break;
                case Opcodes.LCR_INFO:
                    Logger.Info($"Processing LCR_INFO : Number Realms : {packet.GetUint8()} Name : {packet.GetString()} Parsed : {packet.GetParsedString()}");
                    break;
                case Opcodes.LCR_CREATE:
                    HandleCreatePacket(packet);
                    break;
            }
        }

        /// <summary>
        /// Marks the launcher handshake as complete.
        /// </summary>
        public static void Start()
        {
            if (Started)
                return;

            Started = true;
        }

        /// <summary>
        /// Sends the launcher's version check and optional config file size to the server.
        /// </summary>
        public static void SendVersionCheck()
        {
            Logger.Info("Starting SendVersionCheck (CL_CHECK)");
            PacketOut packet = new PacketOut((byte)Opcodes.CL_CHECK);
            packet.WriteUInt32((uint)Version);

            FileInfo configurationFile = new FileInfo(GetLoginConfigPath());
            if (configurationFile.Exists)
            {
                packet.WriteByte(1);
                packet.WriteUInt64((ulong)configurationFile.Length);
            }
            else
            {
                packet.WriteByte(0);
            }

            SendTCP(packet);
        }

        /// <summary>
        /// Applies the executable patch required for private server connectivity.
        /// </summary>
        /// <param name="warDirectory">The WAR client root directory.</param>
        public static void PatchWarExecutable(DirectoryInfo warDirectory)
        {
            if (!LauncherForm.Instance.AllowServerPatch)
            {
                Logger.Info("Not patching WAR.exe");
                return;
            }

            string warExecutablePath = GetWarFilePath(warDirectory, "WAR.exe");
            Logger.Info("Patching WAR.exe");
            using (Stream stream = new FileStream(warExecutablePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                int encryptAddress = (0x00957FBE + 3) - 0x00400000;
                stream.Seek(encryptAddress, SeekOrigin.Begin);
                stream.WriteByte(0x01);

                byte[] decryptPatch1 = { 0x90, 0x90, 0x90, 0x90, 0x57, 0x8B, 0xF8, 0xEB, 0x32 };
                int decryptAddress1 = 0x009580CB - 0x00400000;
                stream.Seek(decryptAddress1, SeekOrigin.Begin);
                stream.Write(decryptPatch1, 0, decryptPatch1.Length);

                byte[] decryptPatch2 = { 0x90, 0x90, 0x90, 0x90, 0xEB, 0x08 };
                int decryptAddress2 = 0x0095814B - 0x00400000;
                stream.Seek(decryptAddress2, SeekOrigin.Begin);
                stream.Write(decryptPatch2, 0, decryptPatch2.Length);
            }

            Logger.Info("Done patching WAR.exe");
        }

        /// <summary>
        /// Replaces the launcher config inside <c>data.myp</c> so the client connects to the selected server.
        /// </summary>
        /// <param name="warDirectory">The WAR client root directory.</param>
        public static void UpdateWarArchiveData(DirectoryInfo warDirectory)
        {
            if (!LauncherForm.Instance.AllowMythicArchivePatch)
            {
                Logger.Info("Not patching data.myp");
                return;
            }

            try
            {
                Logger.Info("Updating mythloginserviceconfig.xml and data.myp");
                string loginConfigPath = GetLoginConfigPath();
                string archivePath = GetWarFilePath(warDirectory, "data.myp");

                using (FileStream fileStream = new FileStream(loginConfigPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    HashDictionary hashDictionary = new HashDictionary();
                    hashDictionary.AddHash(0x3FE03665, 0x349E2A8C, "mythloginserviceconfig.xml", 0);
                    MYPHandler.MYPHandler mypHandler = new MYPHandler.MYPHandler(archivePath, null, null, hashDictionary);
                    mypHandler.GetFileTable();

                    FileInArchive archiveFile = mypHandler.SearchForFile("mythloginserviceconfig.xml");
                    if (archiveFile == null)
                    {
                        Logger.Error("Can not find config file in data.myp");
                        return;
                    }

                    if (!File.Exists(loginConfigPath))
                    {
                        Logger.Error("Missing file : mythloginserviceconfig.xml");
                        return;
                    }

                    mypHandler.ReplaceFile(archiveFile, fileStream);
                }
            }
            catch (Exception exception)
            {
                PrintStatus(exception.ToString());
            }
        }

        /// <summary>
        /// Configures TCP keepalive timers for the launcher socket.
        /// </summary>
        private static void ConfigureKeepAlive()
        {
            int size = sizeof(uint);
            uint enabled = 1;
            uint keepAliveInterval = 10000;
            uint retryInterval = 1000;
            byte[] keepAliveSettings = new byte[size * 3];

            Array.Copy(BitConverter.GetBytes(enabled), 0, keepAliveSettings, 0, size);
            Array.Copy(BitConverter.GetBytes(keepAliveInterval), 0, keepAliveSettings, size, size);
            Array.Copy(BitConverter.GetBytes(retryInterval), 0, keepAliveSettings, size * 2, size);
            _socket.IOControl(IOControlCode.KeepAliveValues, keepAliveSettings, null);
        }

        /// <summary>
        /// Displays a connection failure message on the UI thread when necessary.
        /// </summary>
        /// <param name="ip">The target IP address.</param>
        /// <param name="port">The target port.</param>
        /// <param name="message">The failure description.</param>
        private static void ShowConnectionError(string ip, int port, string message)
        {
            string errorMessage = "Can not connect to : " + ip + ":" + port + "\n" + message;
            if (LauncherForm.Instance != null && LauncherForm.Instance.InvokeRequired)
            {
                LauncherForm.Instance.Invoke(new Action(() => MessageBox.Show(errorMessage)));
                return;
            }

            MessageBox.Show(errorMessage);
        }

        /// <summary>
        /// Maps the configured language name to the numeric WAR language identifier.
        /// </summary>
        /// <param name="language">The language name.</param>
        /// <returns>The WAR client language identifier.</returns>
        private static int GetLanguageId(string language)
        {
            switch (language)
            {
                case "French":
                    return 2;
                case "English":
                    return 1;
                case "Deutch":
                    return 3;
                case "Italian":
                    return 4;
                case "Spanish":
                    return 5;
                case "Korean":
                    return 6;
                case "Chinese":
                    return 7;
                case "Japanese":
                    return 9;
                case "Russian":
                    return 10;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Continues sending queued TCP packets after an asynchronous send completes.
        /// </summary>
        /// <param name="result">The asynchronous send result.</param>
        private static void AsyncTcpSendCallback(IAsyncResult result)
        {
            try
            {
                Queue<byte[]> queue = TcpQueue;
                _socket.EndSend(result);

                int count = 0;
                byte[] data = _tcpSendBuffer;
                if (data == null)
                    return;

                lock (queue)
                {
                    if (queue.Count > 0)
                        count = CombinePackets(data, queue);

                    if (count <= 0)
                    {
                        _isSendingTcp = false;
                        return;
                    }
                }

                _socket.BeginSend(data, 0, count, SocketFlags.None, AsyncTcpSendCallbackDelegate, null);
            }
            catch (Exception)
            {
                Close();
            }
        }

        /// <summary>
        /// Combines queued packets into a single send buffer when space allows.
        /// </summary>
        /// <param name="buffer">The destination send buffer.</param>
        /// <param name="queue">The queued packet buffers.</param>
        /// <returns>The number of bytes copied into <paramref name="buffer"/>.</returns>
        private static int CombinePackets(byte[] buffer, Queue<byte[]> queue)
        {
            int index = 0;
            do
            {
                byte[] packet = queue.Peek();
                if (index + packet.Length > buffer.Length)
                {
                    if (index == 0)
                    {
                        queue.Dequeue();
                        continue;
                    }

                    break;
                }

                Buffer.BlockCopy(packet, 0, buffer, index, packet.Length);
                index += packet.Length;
                queue.Dequeue();
            }
            while (queue.Count > 0);

            return index;
        }

        /// <summary>
        /// Buffers data from the socket, reconstructs complete packets, and dispatches them.
        /// </summary>
        /// <param name="result">The asynchronous receive result.</param>
        private static void OnReceiveHandler(IAsyncResult result)
        {
            try
            {
                int bytesRead = _socket.EndReceive(result);
                Logger.Debug($"Receiving {bytesRead} bytes");

                if (bytesRead <= 0)
                {
                    Close();
                    return;
                }

                int bufferSize = _packetBufferOffset + bytesRead;
                byte[] packetStream = new byte[bufferSize];
                Buffer.BlockCopy(_packetBuffer, 0, packetStream, 0, bufferSize);
                _packetBufferOffset = 0;

                int offset = 0;
                while (offset < packetStream.Length)
                {
                    if (packetStream.Length - offset < 5)
                        break;

                    uint size = (uint)((packetStream[offset] << 24) | (packetStream[offset + 1] << 16) | (packetStream[offset + 2] << 8) | packetStream[offset + 3]);
                    uint totalSize = size + 1;
                    if (packetStream.Length - offset < totalSize)
                        break;

                    PacketIn packet = new PacketIn(packetStream, offset, (int)totalSize);
                    ProcessReceivedPacket(packet);
                    offset += (int)totalSize;
                }

                if (offset < packetStream.Length)
                {
                    _packetBufferOffset = packetStream.Length - offset;
                    Buffer.BlockCopy(packetStream, offset, _packetBuffer, 0, _packetBufferOffset);
                }

                BeginReceive();
            }
            catch (Exception exception)
            {
                Logger.Debug($"Exception : {exception.Message}");
            }
        }

        /// <summary>
        /// Handles the launcher's version check response.
        /// </summary>
        /// <param name="packet">The response packet.</param>
        private static void HandleCheckPacket(PacketIn packet)
        {
            byte result = packet.GetUint8();
            switch ((CheckResult)result)
            {
                case CheckResult.LAUNCHER_OK:
                    Start();
                    break;
                case CheckResult.LAUNCHER_VERSION:
                    string message = packet.GetString();
                    PrintStatus(message);
                    Close();
                    break;
                case CheckResult.LAUNCHER_FILE:
                    SaveConfigFile(packet.GetString());
                    break;
            }
        }

        /// <summary>
        /// Handles the launcher server's login response and optionally starts the WAR client.
        /// </summary>
        /// <param name="packet">The login response packet.</param>
        private static void HandleStartPacket(PacketIn packet)
        {
            LauncherForm.Instance.ReceiveStart();

            byte response = packet.GetUint8();
            Logger.Debug($"HandlePacket. Response Code : {response}");

            if (response == 1)
            {
                Logger.Warn("Invalid User / Pass");
                LauncherForm.Instance.UpdateConnectionStatus("Invalid User / Pass");
                return;
            }

            if (response == 2)
            {
                Logger.Warn("Account is banned");
                LauncherForm.Instance.UpdateConnectionStatus("Account is banned");
                return;
            }

            if (response == 3)
            {
                Logger.Warn("Account is not active");
                LauncherForm.Instance.UpdateConnectionStatus("Account is not active");
                return;
            }

            if (response > 3)
            {
                Logger.Error("Unknown Response");
                LauncherForm.Instance.UpdateConnectionStatus("Unknown Response");
                return;
            }

            AuthToken = packet.GetString();
            Logger.Info($"Authentication Token Received : {AuthToken}");

            try
            {
                DirectoryInfo warDirectory = GetWarDirectory();
                if (!ValidateClientStartupFiles(warDirectory))
                    return;

                LauncherForm.Instance.UpdateConnectionStatus("Patching...");
                PatchWarExecutable(warDirectory);
                UpdateWarArchiveData(warDirectory);
                LauncherForm.Instance.UpdateConnectionStatus(LauncherForm.Instance.AllowWarClientLaunch ? "Patched. Starting WAR.exe" : "Patched.");

                Logger.Info($"Starting Client {warDirectory.FullName}\\WAR.exe");
                if (LauncherForm.Instance.AllowWarClientLaunch)
                {
                    Process process = new Process
                    {
                        StartInfo =
                        {
                            WorkingDirectory = warDirectory.FullName,
                            FileName = "WAR.exe",
                            Arguments = " --acctname=" + Convert.ToBase64String(Encoding.ASCII.GetBytes(User)) + " --sesstoken=" +
                                        Convert.ToBase64String(Encoding.ASCII.GetBytes(AuthToken))
                        }
                    };

                    Logger.Info($"Starting process WAR.exe (in {warDirectory})");
                    process.Start();
                }
                else
                {
                    Logger.Info($"Not launching WAR.exe (in {warDirectory})  --acctname={Convert.ToBase64String(Encoding.ASCII.GetBytes(User))} --sesstoken={Convert.ToBase64String(Encoding.ASCII.GetBytes(AuthToken))}");
                }
            }
            catch (Exception exception)
            {
                Logger.Info($"Failed to start Client {exception}");
                LauncherForm.Instance.UpdateConnectionStatus("Failed to start client.");
            }
        }

        /// <summary>
        /// Handles the account creation response from the launcher server.
        /// </summary>
        /// <param name="packet">The account creation response packet.</param>
        private static void HandleCreatePacket(PacketIn packet)
        {
            byte response = packet.GetUint8();
            Logger.Debug($"HandlePacket. Response Code : {response}");

            if (response == 0)
            {
                Logger.Warn("Account name busy!");
                LauncherForm.Instance.UpdateConnectionStatus("Account name busy!");
                return;
            }

            if (response == 1)
            {
                Logger.Warn("Account created!");
                LauncherForm.Instance.UpdateConnectionStatus("Account created!");
                return;
            }

            if (response == 2)
            {
                Logger.Warn("Account is banned!");
                LauncherForm.Instance.UpdateConnectionStatus("Account is banned!");
            }
        }

        /// <summary>
        /// Persists the configuration file sent by the launcher server.
        /// </summary>
        /// <param name="fileContents">The XML contents to write locally.</param>
        private static void SaveConfigFile(string fileContents)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(fileContents);
            using (FileStream stream = new FileInfo(GetLoginConfigPath()).Create())
                stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Validates the local files required to launch WAR.exe.
        /// </summary>
        /// <param name="warDirectory">The game root directory.</param>
        /// <returns><c>true</c> if the required files exist; otherwise, <c>false</c>.</returns>
        private static bool ValidateClientStartupFiles(DirectoryInfo warDirectory)
        {
            Logger.Info("Double checking mythlogin file exists.");
            string loginConfigPath = GetLoginConfigPath();
            if (!File.Exists(loginConfigPath))
            {
                Logger.Warn($"{loginConfigPath} does not exist.");
                LauncherForm.Instance.UpdateConnectionStatus("Cannot locate mythloginserviceconfig.xml");
                return false;
            }

            if (warDirectory == null || !warDirectory.Exists)
            {
                Logger.Warn("Unable to resolve WAR client root from the launcher path.");
                LauncherForm.Instance.UpdateConnectionStatus("Is your launcher in the Launcher folder?");
                return false;
            }

            string warExecutablePath = GetWarFilePath(warDirectory, "WAR.exe");
            if (LauncherForm.Instance.AllowWarClientLaunch && !File.Exists(warExecutablePath))
            {
                Logger.Warn($"{warExecutablePath} does not exist.");
                LauncherForm.Instance.UpdateConnectionStatus("Cannot locate WAR.exe");
                return false;
            }

            if (LauncherForm.Instance.AllowMythicArchivePatch)
            {
                string archivePath = GetWarFilePath(warDirectory, "data.myp");
                if (!File.Exists(archivePath))
                {
                    Logger.Warn($"{archivePath} does not exist.");
                    LauncherForm.Instance.UpdateConnectionStatus("Cannot locate data.myp");
                    return false;
                }
            }

            return true;
        }
    }
}
