using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using Common;
using FrameWork;

namespace AccountCacher {
    [ConsoleHandler("create", 0, "New Account <Username,Password,GMLevel(1-5)>")]
    public class CreateAccount : IConsoleHandler {
        private string[] _bannedNames = { "zyklon", "fuck", "hitler", "nigger", "nigga", "faggot", "jihad", "muhajid" };

        public bool HandleCommand(string command, List<string> args) {
            if (args.Count < 3)
            {
                new GmLevelInfo().HandleCommand("gmlevels", null);
                return true;
            }

            string Username = args[0];
            string Password = args[1];
            int GmLevel;

            if (!int.TryParse(args[2], out GmLevel))
            {
                Log.Error("CreateAccount", "Invalid GM Level. Must be a number (1-5).");
                new GmLevelInfo().HandleCommand("gmlevels", null);
                return true;
            }

            Program.AcctMgr.CreateAccount(Username, Password, GmLevel);
            return true;
        }
    }

    [ConsoleHandler("gmlevels", 0, "Displays GM level descriptions")]
    public class GmLevelInfo : IConsoleHandler
    {
        // AI Agent (Antigravity): GM levels updated to hierarchical 1-5 system.
        public bool HandleCommand(string command, List<string> args)
        {
            Log.Info("GM Levels", "--------------------------------------------------");
            Log.Info("GM Levels", "1: Player (Standard Access)", ConsoleColor.White);
            Log.Info("GM Levels", "2: Staff (Basic Moderation)", ConsoleColor.Cyan);
            Log.Info("GM Levels", "3: GM (Moderate Power)", ConsoleColor.Green);
            Log.Info("GM Levels", "4: Developer (Technical & Overrides)", ConsoleColor.Magenta);
            Log.Info("GM Levels", "5: Admin (Highest Power)", ConsoleColor.Red);
            Log.Info("GM Levels", "--------------------------------------------------");
            return true;
        }
    }

    [ConsoleHandler("reset", 2, "Reset Password <Username,Password>")]
    public class ResetPassword : IConsoleHandler
    {
        public bool HandleCommand(string command, List<string> args)
        {
            string userName = args[0];
            string password = args[1];

            var account = Program.AcctMgr.LoadAccount(userName);
            if (account == null)
            {
                Log.Error("ResetPassword", $"Could not locate {userName} to reset password");
                return false;
            }
            else
            {
                account.Password = password;
                account.CryptPassword = Account.ConvertSHA256(userName + ":" + password);
                AccountMgr.Database.SaveObject(account);
                AccountMgr.Database.ForceSave();
                Log.Success("ResetPassword", $"Password reset for {userName} to {password}");
            }
           

            return true;

        }
    }
}
