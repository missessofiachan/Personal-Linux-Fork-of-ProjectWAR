using ClientDataMatrix.Configuration;
using ClientDataMatrix.Model;
using ClientDataMatrix.Services;
using ClientDataMatrix.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace ClientDataMatrix
{
    internal static class Program
    {
        private sealed class ToolArguments
        {
            public string Command { get; set; }
            public ushort AbilityId { get; set; }
            public string OutputRoot { get; set; }
            public string ExtractedRootPath { get; set; }
            public bool ShowUsage { get; set; }
        }

        [STAThread]
        private static int Main(string[] args)
        {
            try
            {
                return Run(args);
            }
            catch (Exception exception)
            {
                ConsoleManager.EnsureConsole();
                Console.Error.WriteLine(exception.GetType().Name + ": " + exception.Message);
                Console.Error.WriteLine(exception);
                return 1;
            }
        }

        private static int Run(string[] args)
        {
            ToolArguments toolArguments = ParseArguments(args);
            if (toolArguments == null)
                return 0;

            if (toolArguments.ShowUsage)
            {
                ConsoleManager.EnsureConsole();
                PrintUsage();
                return 0;
            }

            string outputRoot = Path.GetFullPath(toolArguments.OutputRoot ?? Path.Combine("docs", "data-matrix"));
            if (string.Equals(toolArguments.Command, "gui", StringComparison.OrdinalIgnoreCase))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(ExtractedDataRootResolver.Resolve(toolArguments.ExtractedRootPath), outputRoot));
                return 0;
            }

            ConsoleManager.EnsureConsole();
            MatrixAnalysisSession session = MatrixAnalysisSession.Load(toolArguments.ExtractedRootPath);

            if (string.Equals(toolArguments.Command, "doctor_ability", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolArguments.Command, "export_graph_ability", StringComparison.OrdinalIgnoreCase))
            {
                AbilityAnalysisResult report = session.BuildAbilityAnalysis(toolArguments.AbilityId);
                session.WriteAbilityReport(outputRoot, report);
                Console.WriteLine("Ability report written to " + Path.Combine(outputRoot, "ability"));
                Console.WriteLine("Primary markdown: " + Path.Combine(outputRoot, "ability", toolArguments.AbilityId.ToString(CultureInfo.InvariantCulture) + ".md"));
                return 0;
            }

            if (string.Equals(toolArguments.Command, "report_conflicts", StringComparison.OrdinalIgnoreCase))
            {
                ConflictReportDocument report = session.BuildConflictReport();
                session.WriteConflictReport(outputRoot, report);
                Console.WriteLine("Conflict report written to " + Path.Combine(outputRoot, "conflicts"));
                Console.WriteLine("Primary markdown: " + Path.Combine(outputRoot, "conflicts", "client-conflicts.md"));
                return 0;
            }

            if (string.Equals(toolArguments.Command, "report_coverage", StringComparison.OrdinalIgnoreCase))
            {
                CoverageReportDocument report = session.BuildCoverageReport();
                session.WriteCoverageReport(outputRoot, report);
                Console.WriteLine("Coverage report written to " + Path.Combine(outputRoot, "coverage"));
                Console.WriteLine("Primary markdown: " + Path.Combine(outputRoot, "coverage", "ability-coverage.md"));
                return 0;
            }

            if (string.Equals(toolArguments.Command, "report_domains", StringComparison.OrdinalIgnoreCase))
            {
                DomainLedgerDocument report = session.BuildDomainLedger();
                session.WriteDomainLedgerReport(outputRoot, report);
                Console.WriteLine("Domain ledger written to " + Path.Combine(outputRoot, "reference"));
                Console.WriteLine("Primary markdown: " + Path.Combine(outputRoot, "reference", "core-identity-domains.md"));
                return 0;
            }

            if (string.Equals(toolArguments.Command, "report_requirements", StringComparison.OrdinalIgnoreCase))
            {
                RequirementLedgerDocument report = session.BuildRequirementLedger();
                session.WriteRequirementLedgerReport(outputRoot, report);
                Console.WriteLine("Requirement ledger written to " + Path.Combine(outputRoot, "reference"));
                Console.WriteLine("Primary markdown: " + Path.Combine(outputRoot, "reference", "requirement-ledger.md"));
                return 0;
            }

            if (string.Equals(toolArguments.Command, "report_tokens", StringComparison.OrdinalIgnoreCase))
            {
                TokenDictionaryDocument report = session.BuildTokenDictionary();
                session.WriteTokenDictionaryReport(outputRoot, report);
                Console.WriteLine("Token dictionary written to " + Path.Combine(outputRoot, "reference"));
                Console.WriteLine("Primary markdown: " + Path.Combine(outputRoot, "reference", "com-token-dictionary.md"));
                return 0;
            }

            if (string.Equals(toolArguments.Command, "report_operations", StringComparison.OrdinalIgnoreCase))
            {
                OperationSchemaDocument report = session.BuildOperationSchemas();
                session.WriteOperationSchemaReport(outputRoot, report);
                Console.WriteLine("Operation schema report written to " + Path.Combine(outputRoot, "reference"));
                Console.WriteLine("Primary markdown: " + Path.Combine(outputRoot, "reference", "component-operation-schemas.md"));
                return 0;
            }

            PrintUsage();
            return 1;
        }

        private static ToolArguments ParseArguments(string[] args)
        {
            args = args ?? new string[0];
            List<string> positionalArguments = new List<string>();
            ToolArguments parsedArguments = new ToolArguments
            {
                OutputRoot = Path.Combine("docs", "data-matrix")
            };

            for (int index = 0; index < args.Length; ++index)
            {
                string currentArgument = args[index];
                if (string.Equals(currentArgument, "--output", StringComparison.OrdinalIgnoreCase))
                {
                    if (index + 1 >= args.Length)
                        throw new ArgumentException("Missing value for --output.");
                    parsedArguments.OutputRoot = args[++index];
                    continue;
                }

                if (string.Equals(currentArgument, "--root", StringComparison.OrdinalIgnoreCase))
                {
                    if (index + 1 >= args.Length)
                        throw new ArgumentException("Missing value for --root.");
                    parsedArguments.ExtractedRootPath = args[++index];
                    continue;
                }

                if (string.Equals(currentArgument, "--help", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(currentArgument, "-h", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(currentArgument, "/?", StringComparison.OrdinalIgnoreCase))
                {
                    parsedArguments.ShowUsage = true;
                    return parsedArguments;
                }

                positionalArguments.Add(currentArgument);
            }

            if (positionalArguments.Count == 0)
            {
                parsedArguments.Command = "gui";
                return parsedArguments;
            }

            if (positionalArguments.Count >= 1
                && string.Equals(positionalArguments[0], "gui", StringComparison.OrdinalIgnoreCase))
            {
                parsedArguments.Command = "gui";
                return parsedArguments;
            }

            if (positionalArguments.Count >= 3
                && string.Equals(positionalArguments[0], "doctor", StringComparison.OrdinalIgnoreCase)
                && string.Equals(positionalArguments[1], "ability", StringComparison.OrdinalIgnoreCase))
            {
                parsedArguments.Command = "doctor_ability";
                parsedArguments.AbilityId = ParseAbilityId(positionalArguments[2]);
                return parsedArguments;
            }

            if (positionalArguments.Count >= 4
                && string.Equals(positionalArguments[0], "export", StringComparison.OrdinalIgnoreCase)
                && string.Equals(positionalArguments[1], "graph", StringComparison.OrdinalIgnoreCase)
                && string.Equals(positionalArguments[2], "ability", StringComparison.OrdinalIgnoreCase))
            {
                parsedArguments.Command = "export_graph_ability";
                parsedArguments.AbilityId = ParseAbilityId(positionalArguments[3]);
                return parsedArguments;
            }

            if (positionalArguments.Count >= 2
                && string.Equals(positionalArguments[0], "report", StringComparison.OrdinalIgnoreCase)
                && string.Equals(positionalArguments[1], "conflicts", StringComparison.OrdinalIgnoreCase))
            {
                parsedArguments.Command = "report_conflicts";
                return parsedArguments;
            }

            if (positionalArguments.Count >= 2
                && string.Equals(positionalArguments[0], "report", StringComparison.OrdinalIgnoreCase)
                && string.Equals(positionalArguments[1], "coverage", StringComparison.OrdinalIgnoreCase))
            {
                parsedArguments.Command = "report_coverage";
                return parsedArguments;
            }

            if (positionalArguments.Count >= 2
                && string.Equals(positionalArguments[0], "report", StringComparison.OrdinalIgnoreCase)
                && string.Equals(positionalArguments[1], "domains", StringComparison.OrdinalIgnoreCase))
            {
                parsedArguments.Command = "report_domains";
                return parsedArguments;
            }

            if (positionalArguments.Count >= 2
                && string.Equals(positionalArguments[0], "report", StringComparison.OrdinalIgnoreCase)
                && string.Equals(positionalArguments[1], "requirements", StringComparison.OrdinalIgnoreCase))
            {
                parsedArguments.Command = "report_requirements";
                return parsedArguments;
            }

            if (positionalArguments.Count >= 2
                && string.Equals(positionalArguments[0], "report", StringComparison.OrdinalIgnoreCase)
                && string.Equals(positionalArguments[1], "tokens", StringComparison.OrdinalIgnoreCase))
            {
                parsedArguments.Command = "report_tokens";
                return parsedArguments;
            }

            if (positionalArguments.Count >= 2
                && string.Equals(positionalArguments[0], "report", StringComparison.OrdinalIgnoreCase)
                && string.Equals(positionalArguments[1], "operations", StringComparison.OrdinalIgnoreCase))
            {
                parsedArguments.Command = "report_operations";
                return parsedArguments;
            }

            parsedArguments.ShowUsage = true;
            return parsedArguments;
        }

        private static ushort ParseAbilityId(string rawValue)
        {
            ushort abilityId;
            if (!ushort.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out abilityId))
                throw new ArgumentException("Ability id must be a valid unsigned 16-bit integer.");
            return abilityId;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("ClientDataMatrix");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  ClientDataMatrix [gui] [--root <path>] [--output <path>]");
            Console.WriteLine("  ClientDataMatrix doctor ability <abilityId> [--root <path>] [--output <path>]");
            Console.WriteLine("  ClientDataMatrix export graph ability <abilityId> [--root <path>] [--output <path>]");
            Console.WriteLine("  ClientDataMatrix report conflicts [--root <path>] [--output <path>]");
            Console.WriteLine("  ClientDataMatrix report coverage [--root <path>] [--output <path>]");
            Console.WriteLine("  ClientDataMatrix report domains [--root <path>] [--output <path>]");
            Console.WriteLine("  ClientDataMatrix report requirements [--root <path>] [--output <path>]");
            Console.WriteLine("  ClientDataMatrix report tokens [--root <path>] [--output <path>]");
            Console.WriteLine("  ClientDataMatrix report operations [--root <path>] [--output <path>]");
            Console.WriteLine();
            Console.WriteLine("Running without a command launches the GUI.");
            Console.WriteLine("Default output path: docs\\data-matrix");
            Console.WriteLine("Default extracted root: C:\\Users\\Admin\\Pictures\\WAR_extracted");
        }
    }
}
