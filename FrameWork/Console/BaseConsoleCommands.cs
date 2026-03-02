using System;
using System.Collections.Generic;
using System.Text;

namespace FrameWork
{
    [ConsoleHandler("uptime", 0, "Shows the current server uptime.")]
    public class UptimeCommand : IConsoleHandler
    {
        public bool HandleCommand(string command, List<string> args)
        {
            Log.Info("ConsoleMgr", "Server Uptime: " + ConsoleMgr.GetUptime);
            return true;
        }
    }

    [ConsoleHandler("exit", 0, "Stops the server safely.")]
    public class ExitCommand : IConsoleHandler
    {
        public bool HandleCommand(string command, List<string> args)
        {
            Log.Info("ConsoleMgr", "Stopping server...");
            ConsoleMgr.Stop();
            return true;
        }
    }

    [ConsoleHandler("help", 0, "Lists all available console commands.")]
    public class HelpCommand : IConsoleHandler
    {
        public bool HandleCommand(string command, List<string> args)
        {
            ConsoleMgr.ListAllCommands();
            return true;
        }
    }
}
