using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ClientDataMatrix.Services
{
    internal static class ConsoleManager
    {
        private const int AttachParentProcess = -1;
        private static bool _initialized;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        public static void EnsureConsole()
        {
            if (_initialized)
                return;

            if (!AttachConsole(AttachParentProcess))
                AllocConsole();

            Stream stdout = Console.OpenStandardOutput();
            Stream stderr = Console.OpenStandardError();
            Console.SetOut(new StreamWriter(stdout, Encoding.UTF8) { AutoFlush = true });
            Console.SetError(new StreamWriter(stderr, Encoding.UTF8) { AutoFlush = true });
            _initialized = true;
        }
    }
}
