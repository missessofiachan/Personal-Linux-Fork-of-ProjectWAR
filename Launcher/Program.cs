using System;
using System.Windows.Forms;

namespace Launcher
{
    /// <summary>
    /// Bootstraps the Windows Forms launcher application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Starts the launcher UI message loop.
        /// </summary>
        /// <param name="args">Command-line arguments supplied to the launcher.</param>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LauncherForm());
        }
    }
}
