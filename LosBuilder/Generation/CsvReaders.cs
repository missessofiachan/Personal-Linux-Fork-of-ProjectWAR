using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LosBuilder.Generation
{
    internal static class CsvReaders
    {
        public static Dictionary<int, ZoneNifRecord> ReadZoneNifs(string zoneFolder)
        {
            string path = Path.Combine(zoneFolder, "nifs.csv");
            Dictionary<int, ZoneNifRecord> results = new Dictionary<int, ZoneNifRecord>();

            if (!File.Exists(path))
                return results;

            foreach (string[] columns in ReadRows(path))
            {
                if (columns.Length < 8)
                    continue;

                int id = ParseInt(columns, 0);
                if (id <= 0)
                    continue;

                results[id] = new ZoneNifRecord
                {
                    Id = id,
                    TextualName = columns[1],
                    FileName = columns[2],
                    Collide = ParseInt(columns, 7)
                };
            }

            return results;
        }

        public static List<ZoneFixtureRecord> ReadFixtures(string zoneFolder)
        {
            string path = Path.Combine(zoneFolder, "fixtures.csv");
            List<ZoneFixtureRecord> results = new List<ZoneFixtureRecord>();

            if (!File.Exists(path))
                return results;

            foreach (string[] columns in ReadRows(path))
            {
                if (columns.Length < 19)
                    continue;

                int id = ParseInt(columns, 0);
                if (id <= 0)
                    continue;

                results.Add(new ZoneFixtureRecord
                {
                    Id = id,
                    NifId = ParseInt(columns, 1),
                    TextualName = columns[2],
                    X = ParseFloat(columns, 3),
                    Y = ParseFloat(columns, 4),
                    Z = ParseFloat(columns, 5),
                    A = ParseInt(columns, 6),
                    ScalePercent = ParseFloat(columns, 7, 100f),
                    Collide = ParseInt(columns, 8),
                    UniqueId = ParseInt(columns, 14),
                    AxisAngleRadians = ParseFloat(columns, 15),
                    AxisX = ParseFloat(columns, 16),
                    AxisY = ParseFloat(columns, 17),
                    AxisZ = ParseFloat(columns, 18)
                });
            }

            return results;
        }

        private static IEnumerable<string[]> ReadRows(string path)
        {
            int lineIndex = 0;
            foreach (string line in File.ReadLines(path))
            {
                lineIndex++;
                if (lineIndex <= 2 || string.IsNullOrWhiteSpace(line))
                    continue;

                yield return line.Split(',');
            }
        }

        private static int ParseInt(string[] columns, int index)
        {
            if (index >= columns.Length)
                return 0;

            int value;
            int.TryParse(columns[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            return value;
        }

        private static float ParseFloat(string[] columns, int index, float defaultValue = 0f)
        {
            if (index >= columns.Length)
                return defaultValue;

            float value;
            if (float.TryParse(columns[index], NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                return value;

            return defaultValue;
        }
    }
}
