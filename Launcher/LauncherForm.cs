using NLog;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
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
        private static readonly Size FixedClientSize = new Size(1280, 720);

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Patcher _patcher;
        private bool _fixed720pLayoutApplied;

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
            ApplyFixed720pLayout();
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
        /// Applies the fixed 1280x720 launcher layout and places every interactive control inside that window.
        /// </summary>
        private void ApplyFixed720pLayout()
        {
            if (_fixed720pLayoutApplied)
                return;

            _fixed720pLayoutApplied = true;

            SuspendLayout();

            AutoScaleMode = AutoScaleMode.None;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = FixedClientSize;
            MinimumSize = FixedClientSize;
            MaximumSize = FixedClientSize;

            ConfigureFixedControlBehavior();
            ConfigureWindowChrome();
            ConfigureMainControls();
            ConfigureCreateAccountPanel();

            ResumeLayout(true);
            CenterToScreen();
        }

        /// <summary>
        /// Disables designer anchor behavior so the fixed 720p positions do not drift after layout.
        /// </summary>
        private void ConfigureFixedControlBehavior()
        {
            ApplyFixedBehavior(this);
        }

        /// <summary>
        /// Recursively clears anchor and dock behavior for a fixed-position layout.
        /// </summary>
        /// <param name="parent">The control subtree to normalize.</param>
        private static void ApplyFixedBehavior(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                control.Dock = DockStyle.None;

                if (control.Controls.Count > 0)
                    ApplyFixedBehavior(control);
            }
        }

        /// <summary>
        /// Positions the window controls along the top edge of the fixed 720p canvas.
        /// </summary>
        private void ConfigureWindowChrome()
        {
            buttonPanelCreateAccount.Bounds = new Rectangle(986, 18, 180, 30);
            buttonPanelCreateAccount.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold, GraphicsUnit.Pixel);

            bnMinimise.Bounds = new Rectangle(1178, 18, 40, 30);
            bnMinimise.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Pixel);

            bnClose.Bounds = new Rectangle(1224, 18, 40, 30);
            bnClose.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Positions the login controls and status text inside the fixed 720p window.
        /// </summary>
        private void ConfigureMainControls()
        {
            const int groupLeft = 860;
            const int labelWidth = 120;
            const int fieldWidth = 250;
            const int rowGap = 14;
            const int fieldX = groupLeft + labelWidth + 12;

            lblDownloading.AutoSize = false;
            lblDownloading.Bounds = new Rectangle(groupLeft, 484, 390, 20);
            lblDownloading.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblDownloading.TextAlign = ContentAlignment.MiddleLeft;

            lblConnection.AutoSize = false;
            lblConnection.Bounds = new Rectangle(groupLeft, 506, 390, 20);
            lblConnection.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblConnection.TextAlign = ContentAlignment.MiddleLeft;

            label4.AutoSize = false;
            label4.Bounds = new Rectangle(groupLeft, 536, labelWidth, 30);
            label4.TextAlign = ContentAlignment.MiddleRight;
            label4.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Pixel);

            T_username.Multiline = false;
            T_username.Bounds = new Rectangle(fieldX, 534, fieldWidth, 32);
            T_username.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);

            label8.AutoSize = false;
            label8.Bounds = new Rectangle(groupLeft, 536 + 32 + rowGap, labelWidth, 30);
            label8.TextAlign = ContentAlignment.MiddleRight;
            label8.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Pixel);

            T_password.Bounds = new Rectangle(fieldX, 534 + 32 + rowGap, fieldWidth, 32);
            T_password.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);

            bnConnectToServer.Bounds = new Rectangle(fieldX, 534 + ((32 + rowGap) * 2), fieldWidth, 40);
            bnConnectToServer.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Pixel);

            lblVersion.AutoSize = true;
            lblVersion.Location = new Point(16, 694);
            lblVersion.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Pixel);

            bnConnectLocal.Bounds = new Rectangle(fieldX + fieldWidth - 120, 650, 120, 26);
            bnConnectLocal.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Centers the create-account panel and arranges its fields inside the fixed 720p window.
        /// </summary>
        private void ConfigureCreateAccountPanel()
        {
            panelCreateAccount.Size = new Size(590, 300);
            panelCreateAccount.Location = new Point((ClientSize.Width - panelCreateAccount.Width) / 2, (ClientSize.Height - panelCreateAccount.Height) / 2);
            panelCreateAccount.BackgroundImageLayout = ImageLayout.Center;

            label1.Bounds = new Rectangle(20, 18, 550, 30);
            label1.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);

            buttonAccountClose.Bounds = new Rectangle(542, 8, 38, 34);
            buttonAccountClose.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Pixel);

            label2.Bounds = new Rectangle(32, 76, 140, 30);
            label2.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Pixel);

            textBoxUsername.Multiline = false;
            textBoxUsername.Bounds = new Rectangle(184, 74, 374, 34);
            textBoxUsername.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);

            label3.Bounds = new Rectangle(32, 122, 140, 30);
            label3.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Pixel);

            textBoxPassword.Multiline = false;
            textBoxPassword.Bounds = new Rectangle(184, 120, 374, 34);
            textBoxPassword.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);

            buttonCreate.Bounds = new Rectangle(184, 176, 374, 42);
            buttonCreate.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);

            bnCreateLocal.Bounds = new Rectangle(442, 230, 116, 28);
            bnCreateLocal.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Pixel);
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
