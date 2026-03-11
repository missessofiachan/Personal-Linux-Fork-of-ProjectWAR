using System;
using LosBuilder.Generation;

namespace LosBuilder
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            BuildOptions options;
            string errorMessage;
            if (!BuildOptions.TryParse(args, out options, out errorMessage))
            {
                if (!string.IsNullOrWhiteSpace(errorMessage))
                    Console.Error.WriteLine(errorMessage);

                PrintUsage();
                return 1;
            }

            try
            {
                switch (options.Command)
                {
                    case LosCommand.Generate:
                        LosGenerator generator = new LosGenerator(Console.WriteLine, Console.Error.WriteLine);
                        return generator.Generate(options) ? 0 : 2;
                    case LosCommand.Inspect:
                        return OccInspector.Inspect(options.InputBinPath, Console.WriteLine) ? 0 : 2;
                    case LosCommand.Compare:
                        return OccInspector.Compare(options.LeftBinPath, options.RightBinPath, Console.WriteLine) ? 0 : 2;
                    default:
                        Console.Error.WriteLine("Unsupported command.");
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 3;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("LosBuilder usage:");
            Console.WriteLine("  LosBuilder generate --input-root <WAR_extracted> --output-root <outputFolder> [--zone <zoneId>] [--overwrite]");
            Console.WriteLine("  LosBuilder inspect --input-bin <losBinPath>");
            Console.WriteLine("  LosBuilder compare --left-bin <losBinPath> --right-bin <losBinPath>");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  LosBuilder generate --input-root C:\\WAR_extracted --output-root C:\\ProjectWAR\\deps\\los");
            Console.WriteLine("  LosBuilder generate --input-root C:\\WAR_extracted --output-root C:\\temp\\los --zone 280");
            Console.WriteLine("  LosBuilder inspect --input-bin C:\\ProjectWAR\\deps\\los\\280.bin");
            Console.WriteLine("  LosBuilder compare --left-bin C:\\ProjectWAR\\deps\\los\\280.bin --right-bin C:\\temp\\los\\280.bin");
        }
    }
}
