using System;
using System.Collections.Generic;
using System.IO;

namespace LosBuilder.Generation
{
    internal sealed class BuildOptions
    {
        private readonly List<int> _zoneIds = new List<int>();

        public LosCommand Command { get; private set; }
        public string InputRoot { get; private set; }
        public string OutputRoot { get; private set; }
        public string InputBinPath { get; private set; }
        public string LeftBinPath { get; private set; }
        public string RightBinPath { get; private set; }
        public List<int> ZoneIds { get { return _zoneIds; } }
        public bool Overwrite { get; private set; }

        public bool GenerateAllZones { get { return ZoneIds.Count == 0; } }

        public static bool TryParse(string[] args, out BuildOptions options, out string errorMessage)
        {
            options = null;
            errorMessage = null;

            if (args == null || args.Length == 0)
            {
                errorMessage = "Missing command.";
                return false;
            }

            BuildOptions parsed = new BuildOptions();
            parsed.Overwrite = true;

            if (args[0].Equals("generate", StringComparison.OrdinalIgnoreCase))
            {
                parsed.Command = LosCommand.Generate;
                return TryParseGenerate(args, parsed, out options, out errorMessage);
            }

            if (args[0].Equals("inspect", StringComparison.OrdinalIgnoreCase))
            {
                parsed.Command = LosCommand.Inspect;
                return TryParseInspect(args, parsed, out options, out errorMessage);
            }

            if (args[0].Equals("compare", StringComparison.OrdinalIgnoreCase))
            {
                parsed.Command = LosCommand.Compare;
                return TryParseCompare(args, parsed, out options, out errorMessage);
            }

            errorMessage = "Unsupported command. Supported commands are 'generate', 'inspect', and 'compare'.";
            return false;
        }

        private static bool TryParseGenerate(string[] args, BuildOptions parsed, out BuildOptions options, out string errorMessage)
        {
            options = null;
            errorMessage = null;

            for (int i = 1; i < args.Length; i++)
            {
                string current = args[i];

                if (current.Equals("--input-root", StringComparison.OrdinalIgnoreCase))
                {
                    string value;
                    if (!TryReadValue(args, ref i, out value))
                    {
                        errorMessage = "Missing value for --input-root.";
                        return false;
                    }

                    parsed.InputRoot = Path.GetFullPath(value);
                    continue;
                }

                if (current.Equals("--output-root", StringComparison.OrdinalIgnoreCase))
                {
                    string value;
                    if (!TryReadValue(args, ref i, out value))
                    {
                        errorMessage = "Missing value for --output-root.";
                        return false;
                    }

                    parsed.OutputRoot = Path.GetFullPath(value);
                    continue;
                }

                if (current.Equals("--zone", StringComparison.OrdinalIgnoreCase))
                {
                    string value;
                    int zoneId;
                    if (!TryReadValue(args, ref i, out value) || !int.TryParse(value, out zoneId) || zoneId <= 0)
                    {
                        errorMessage = "Invalid value for --zone.";
                        return false;
                    }

                    parsed.ZoneIds.Add(zoneId);
                    continue;
                }

                if (current.Equals("--overwrite", StringComparison.OrdinalIgnoreCase))
                {
                    parsed.Overwrite = true;
                    continue;
                }

                errorMessage = "Unknown argument: " + current;
                return false;
            }

            if (string.IsNullOrWhiteSpace(parsed.InputRoot))
            {
                errorMessage = "Missing --input-root.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(parsed.OutputRoot))
            {
                errorMessage = "Missing --output-root.";
                return false;
            }

            options = parsed;
            return true;
        }

        private static bool TryParseInspect(string[] args, BuildOptions parsed, out BuildOptions options, out string errorMessage)
        {
            options = null;
            errorMessage = null;

            for (int i = 1; i < args.Length; i++)
            {
                string current = args[i];
                if (current.Equals("--input-bin", StringComparison.OrdinalIgnoreCase) || current.Equals("--bin", StringComparison.OrdinalIgnoreCase))
                {
                    string value;
                    if (!TryReadValue(args, ref i, out value))
                    {
                        errorMessage = "Missing value for --input-bin.";
                        return false;
                    }

                    parsed.InputBinPath = Path.GetFullPath(value);
                    continue;
                }

                errorMessage = "Unknown argument: " + current;
                return false;
            }

            if (string.IsNullOrWhiteSpace(parsed.InputBinPath))
            {
                errorMessage = "Missing --input-bin.";
                return false;
            }

            options = parsed;
            return true;
        }

        private static bool TryParseCompare(string[] args, BuildOptions parsed, out BuildOptions options, out string errorMessage)
        {
            options = null;
            errorMessage = null;

            for (int i = 1; i < args.Length; i++)
            {
                string current = args[i];
                if (current.Equals("--left-bin", StringComparison.OrdinalIgnoreCase))
                {
                    string value;
                    if (!TryReadValue(args, ref i, out value))
                    {
                        errorMessage = "Missing value for --left-bin.";
                        return false;
                    }

                    parsed.LeftBinPath = Path.GetFullPath(value);
                    continue;
                }

                if (current.Equals("--right-bin", StringComparison.OrdinalIgnoreCase))
                {
                    string value;
                    if (!TryReadValue(args, ref i, out value))
                    {
                        errorMessage = "Missing value for --right-bin.";
                        return false;
                    }

                    parsed.RightBinPath = Path.GetFullPath(value);
                    continue;
                }

                errorMessage = "Unknown argument: " + current;
                return false;
            }

            if (string.IsNullOrWhiteSpace(parsed.LeftBinPath))
            {
                errorMessage = "Missing --left-bin.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(parsed.RightBinPath))
            {
                errorMessage = "Missing --right-bin.";
                return false;
            }

            options = parsed;
            return true;
        }

        private static bool TryReadValue(string[] args, ref int index, out string value)
        {
            value = null;

            if (index + 1 >= args.Length)
                return false;

            index++;
            value = args[index];
            return true;
        }
    }
}
