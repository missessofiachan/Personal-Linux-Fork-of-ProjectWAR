using NLog;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    /// <summary>
    /// Provides the main launcher window for login, account creation, and patch progress updates.
    /// </summary>
    public partial class LauncherForm : Form
    {
        private const int WindowHitTestMessage = 0x84;
        private const int WindowCaptionResult = 0x2;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Patcher _patcher;

        /// <summary>
        /// Gets the active launcher form instance so static networking callbacks can update the UI.
        /// </summary>
        public static LauncherForm Instance { get; private set; }

        /// <summary>
        /// Gets the local server IP address used for account creation and direct local login.
        /// </summary>
        public static string LocalServerIP { get; } = "127.0.0.1";

        /// <summary>
        /// Gets the test server IP address used for standard login requests.
        /// </summary>
        public static string TestServerIP { get; } = "127.0.0.1";

        /// <summary>
        /// Gets the local server TCP port used for local login and account creation.
        /// </summary>
        public static int LocalServerPort { get; } = 8000;

        /// <summary>
        /// Gets the test server TCP port used for standard login requests.
        /// </summary>
        public static int TestServerPort { get; } = 8000;

        /// <summary>
        /// Initializes a new instance of the <see cref="LauncherForm"/> class.
        /// </summary>
        public LauncherForm()
        {
            AllowWarClientLaunch = ReadOptionalBooleanSetting("AutoLaunch", true);
            AllowMythicArchivePatch = ReadOptionalBooleanSetting("PatchMYP", true);
            AllowServerPatch = ReadOptionalBooleanSetting("PatchExe", true);
            LaunchLocalServer = ReadOptionalBooleanSetting("LaunchLocal", false);

            InitializeComponent();
            Instance = this;

            bnConnectLocal.Visible = LaunchLocalServer;
            bnCreateLocal.Visible = LaunchLocalServer;
        }

        /// <summary>
        /// Gets a value indicating whether the launcher should expose local server login controls.
        /// </summary>
        public bool LaunchLocalServer { get; }

        /// <summary>
        /// Gets a value indicating whether the launcher should patch the archive assets.
        /// </summary>
        public bool AllowMythicArchivePatch { get; }

        /// <summary>
        /// Gets a value indicating whether the launcher should patch the WAR executable and data files.
        /// </summary>
        public bool AllowServerPatch { get; }

        /// <summary>
        /// Gets a value indicating whether the launcher should automatically start WAR.exe after login.
        /// </summary>
        public bool AllowWarClientLaunch { get; }

        /// <summary>
        /// Reads an optional boolean application setting and falls back to the supplied default when it is missing or invalid.
        /// </summary>
        /// <param name="keyName">The appSettings key to read.</param>
        /// <param name="defaultValue">The value to use when the setting is unavailable.</param>
        /// <returns>The configured boolean value or the provided default.</returns>
        private static bool ReadOptionalBooleanSetting(string keyName, bool defaultValue)
        {
            string settingValue = ConfigurationManager.AppSettings[keyName];
            return bool.TryParse(settingValue, out bool parsedValue) ? parsedValue : defaultValue;
        }

        /// <summary>
        /// Lets the borderless form be dragged by treating the client area as a title bar hit target.
        /// </summary>
        /// <param name="message">The Windows message being processed.</param>
        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (message.Msg == WindowHitTestMessage)
                message.Result = (IntPtr)WindowCaptionResult;
        }

        /// <summary>
        /// Initializes version labels, optional patching, and the saved username when the form loads.
        /// </summary>
        /// <param name="sender">The form instance.</param>
        /// <param name="e">The load event arguments.</param>
        private void LauncherForm_Load(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            lblVersion.Text = fileVersion.FileVersion;
            lblDownloading.Visible = false;

            if (AllowMythicArchivePatch)
            {
                string patchAddress = $"{ConfigurationManager.AppSettings["ServerPatchIPAddress"]}:{ConfigurationManager.AppSettings["ServerPatchPort"]}";
                Logger.Debug($"Calling patch server on {patchAddress}");

                _patcher = new Patcher(Logger, patchAddress);
                lblDownloading.Visible = true;

                string patchDirectory = ConfigurationManager.AppSettings["PatchDirectory"];
                Thread patchThread = new Thread(() => _patcher.PatchAsync(patchDirectory).Wait()) { IsBackground = true };
                patchThread.Start();
            }

            T_username.Text = ConfigurationManager.AppSettings["LastUserCode"];
        }

        /// <summary>
        /// Closes the TCP client when the launcher window shuts down.
        /// </summary>
        /// <param name="sender">The form instance.</param>
        /// <param name="e">The form closed event arguments.</param>
        private void Disconnect(object sender, FormClosedEventArgs e)
        {
            Client.Close();
        }

        /// <summary>
        /// Computes a lowercase SHA-256 hash for login credentials.
        /// </summary>
        /// <param name="value">The plain-text input to hash.</param>
        /// <returns>The hexadecimal SHA-256 digest.</returns>
        public static string ComputeSha256Hash(string value)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] data = sha256.ComputeHash(Encoding.Default.GetBytes(value));
                StringBuilder stringBuilder = new StringBuilder(data.Length * 2);

                for (int index = 0; index < data.Length; index++)
                    stringBuilder.Append(data[index].ToString("x2"));

                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Receives the launcher start acknowledgement from the server.
        /// </summary>
        public void ReceiveStart()
        {
        }

        /// <summary>
        /// Writes a diagnostic message to the launcher output hook.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void Print(string message)
        {
        }

        /// <summary>
        /// Sends the standard login request to the configured remote server.
        /// </summary>
        /// <param name="sender">The login button.</param>
        /// <param name="e">The click event arguments.</param>
        private async void ConnectToServerButton_Click(object sender, EventArgs e)
        {
            lblConnection.Text = $@"Connecting to : {TestServerIP}:{TestServerPort}";

            string userCode = T_username.Text.ToLower();
            string userPassword = T_password.Text.ToLower();

            await Task.Run(() =>
            {
                Client.Connect(TestServerIP, TestServerPort);
                Client.User = userCode;

                string encryptedPassword = ComputeSha256Hash(userCode + ":" + userPassword);
                Logger.Info($@"Connecting to : {TestServerIP}:{TestServerPort} as {userCode} [{encryptedPassword}]");
                Logger.Info($"Sending CL_START to {TestServerIP}:{TestServerPort}");

                PacketOut packet = new PacketOut((byte)Opcodes.CL_START);
                packet.WriteString(userCode);
                packet.WriteString(encryptedPassword);
                Client.SendTCP(packet);
            });

            SaveLastUsername(T_username.Text);
        }

        /// <summary>
        /// Persists the last successful username entry in the launcher configuration.
        /// </summary>
        /// <param name="username">The username to store.</param>
        private static void SaveLastUsername(string username)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (configuration.AppSettings.Settings["LastUserCode"] == null)
                configuration.AppSettings.Settings.Add("LastUserCode", username);
            else
                configuration.AppSettings.Settings["LastUserCode"].Value = username;

            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Exits the launcher application.
        /// </summary>
        /// <param name="sender">The close button.</param>
        /// <param name="e">The click event arguments.</param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Shows the account creation panel.
        /// </summary>
        /// <param name="sender">The account creation button.</param>
        /// <param name="e">The click event arguments.</param>
        private void OpenCreateAccountPanelButton_Click(object sender, EventArgs e)
        {
            panelCreateAccount.Visible = true;
        }

        /// <summary>
        /// Sends an account creation request to the configured remote server.
        /// </summary>
        /// <param name="sender">The create account button.</param>
        /// <param name="e">The click event arguments.</param>
        private async void CreateRemoteAccountButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxUsername.Text) || string.IsNullOrEmpty(textBoxPassword.Text))
                return;

            lblConnection.Text = $@"Connecting to : {TestServerIP}:{TestServerPort}";

            string userCode = textBoxUsername.Text.ToLower();
            string userPassword = textBoxPassword.Text.ToLower();

            await Task.Run(() =>
            {
                Client.Connect(TestServerIP, TestServerPort);
                Client.User = userCode;

                Logger.Info($@"Create account : {TestServerIP}:{TestServerPort} as {userCode}");
                Logger.Info($"Sending CL_CREATE to {TestServerIP}:{TestServerPort}");

                PacketOut packet = new PacketOut((byte)Opcodes.CL_CREATE);
                packet.WriteString(userCode);
                packet.WriteString(userPassword);
                Client.SendTCP(packet);
            });
        }

        /// <summary>
        /// Hides the account creation panel.
        /// </summary>
        /// <param name="sender">The panel close button.</param>
        /// <param name="e">The click event arguments.</param>
        private void CloseCreateAccountPanelButton_Click(object sender, EventArgs e)
        {
            panelCreateAccount.Visible = false;
        }

        /// <summary>
        /// Updates the status label from either the UI thread or a background thread.
        /// </summary>
        /// <param name="message">The message to show in the connection label.</param>
        public void UpdateConnectionStatus(string message)
        {
            if (lblConnection.InvokeRequired)
            {
                lblConnection.BeginInvoke(new Action(() => UpdateConnectionStatus(message)));
                return;
            }

            lblConnection.Text = message;
        }

        /// <summary>
        /// Sends an account creation request to the local server.
        /// </summary>
        /// <param name="sender">The local create account button.</param>
        /// <param name="e">The click event arguments.</param>
        private async void CreateLocalAccountButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxUsername.Text) || string.IsNullOrEmpty(textBoxPassword.Text))
                return;

            lblConnection.Text = $@"Connecting to : {LocalServerIP}:{LocalServerPort}";

            string userCode = textBoxUsername.Text.ToLower();
            string userPassword = textBoxPassword.Text.ToLower();

            await Task.Run(() =>
            {
                Client.Connect(LocalServerIP, LocalServerPort);
                Client.User = userCode;

                Logger.Info($@"Create account : {LocalServerIP}:{LocalServerPort} as {userCode}");
                Logger.Info($"Sending CL_CREATE to {LocalServerIP}:{LocalServerPort}");

                PacketOut packet = new PacketOut((byte)Opcodes.CL_CREATE);
                packet.WriteString(userCode);
                packet.WriteString(userPassword);
                Client.SendTCP(packet);
            });
        }

        /// <summary>
        /// Sends a login request to the local server.
        /// </summary>
        /// <param name="sender">The local login button.</param>
        /// <param name="e">The click event arguments.</param>
        private async void ConnectToLocalButton_Click(object sender, EventArgs e)
        {
            lblConnection.Text = $@"Connecting to : {LocalServerIP}:{LocalServerPort}";

            string userCode = T_username.Text.ToLower();
            string userPassword = T_password.Text.ToLower();

            await Task.Run(() =>
            {
                Client.Connect(LocalServerIP, LocalServerPort);
                Client.User = userCode;

                string encryptedPassword = ComputeSha256Hash(userCode + ":" + userPassword);
                Logger.Info($@"Connecting to : {LocalServerIP}:{LocalServerPort} as {userCode} [{encryptedPassword}]");
                Logger.Info($"Sending CL_START to {LocalServerIP}:{LocalServerPort}");

                PacketOut packet = new PacketOut((byte)Opcodes.CL_START);
                packet.WriteString(userCode);
                packet.WriteString(encryptedPassword);
                Client.SendTCP(packet);
            });
        }

        /// <summary>
        /// Refreshes the patch status banner while the background patcher runs.
        /// </summary>
        /// <param name="sender">The timer instance.</param>
        /// <param name="e">The tick event arguments.</param>
        private void PatchStatusTimer_Tick(object sender, EventArgs e)
        {
            if (!AllowMythicArchivePatch || _patcher == null)
                return;

            if (_patcher.CurrentState == Patcher.State.Downloading)
            {
                bnConnectToServer.Enabled = false;

                long percent = 0;
                if (_patcher.TotalDownloadSize > 0)
                    percent = (_patcher.Downloaded * 100) / _patcher.TotalDownloadSize;

                lblDownloading.Text = $"Downloading {_patcher.CurrentFile} ({percent}%)";
            }
            else if (_patcher.CurrentState == Patcher.State.RequestManifest)
            {
                bnConnectToServer.Enabled = false;
                lblDownloading.Text = "Looking for updates...";
            }
            else if (_patcher.CurrentState == Patcher.State.ProcessManifest)
            {
                bnConnectToServer.Enabled = false;
                lblDownloading.Text = "Processing updates...";
            }
            else if (_patcher.CurrentState == Patcher.State.Done || _patcher.CurrentState == Patcher.State.Error)
            {
                bnConnectToServer.Enabled = true;
                lblDownloading.Text = string.Empty;
            }
        }

        /// <summary>
        /// Submits the remote login when Enter is pressed in the username box.
        /// </summary>
        /// <param name="sender">The username text box.</param>
        /// <param name="e">The key event arguments.</param>
        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ConnectToServerButton_Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Submits the remote login when Enter is pressed in the password box.
        /// </summary>
        /// <param name="sender">The password text box.</param>
        /// <param name="e">The key event arguments.</param>
        private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ConnectToServerButton_Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Minimizes the launcher window.
        /// </summary>
        /// <param name="sender">The minimize button.</param>
        /// <param name="e">The click event arguments.</param>
        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
    }
}
