using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Xml;

namespace LosBuilder.Generation
{
    internal static class WaterXmlReader
    {
        public static List<WaterBodySource> Load(string zoneFolder)
        {
            string path = Path.Combine(zoneFolder, "water.xml");
            List<WaterBodySource> results = new List<WaterBodySource>();

            if (!File.Exists(path))
                return results;

            XmlDocument document = new XmlDocument();
            document.Load(path);

            XmlNodeList bodyNodes = document.SelectNodes("/WaterSystem/WaterBody");
            if (bodyNodes == null)
                return results;

            int bodyId = 1;
            foreach (XmlNode bodyNode in bodyNodes)
            {
                XmlElement bodyElement = bodyNode as XmlElement;
                if (bodyElement == null)
                    continue;

                WaterBodySource body = new WaterBodySource
                {
                    Id = bodyId++,
                    SurfaceType = ResolveSurfaceType(bodyElement.GetAttribute("type")),
                    Height = -ParseFloat(bodyElement.GetAttribute("height"))
                };

                XmlNodeList pairNodes = bodyElement.SelectNodes("./ControlPoints/WaterBodyPointPair");
                if (pairNodes != null)
                {
                    foreach (XmlNode pairNode in pairNodes)
                    {
                        XmlElement pairElement = pairNode as XmlElement;
                        if (pairElement == null)
                            continue;

                        body.PointPairs.Add(new WaterPointPair
                        {
                            Left = ParsePoint(pairElement.GetAttribute("left")),
                            Right = ParsePoint(pairElement.GetAttribute("right"))
                        });
                    }
                }

                if (body.PointPairs.Count >= 2)
                    results.Add(body);
            }

            return results;
        }

        private static Vector2 ParsePoint(string value)
        {
            string[] tokens = value.Split(',');
            if (tokens.Length != 2)
                return Vector2.Zero;

            return new Vector2(ParseFloat(tokens[0]), ParseFloat(tokens[1]));
        }

        private static float ParseFloat(string value)
        {
            float parsed;
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
                return parsed;

            return 0f;
        }

        private static int ResolveSurfaceType(string typeName)
        {
            string name = (typeName ?? string.Empty).ToLowerInvariant();

            if (name.Contains("poison"))
                return 19;
            if (name.Contains("river"))
                return 11;
            if (name.Contains("marsh"))
                return 21;
            if (name.Contains("_tar_"))
                return 25;
            if (name.Contains("muck"))
                return 22;
            if (name.Contains("dirty"))
                return 14;
            if (name.Contains("lava"))
                return 23;
            if (name.Contains("magma"))
                return 24;
            if (name.Contains("ocean"))
                return 13;
            if (name.Contains("hotspring"))
                return 12;
            if (name.Contains("ice") || name.Contains("icy"))
                return 18;
            if (name.Contains("lake"))
                return 20;
            if (name.Contains("bog"))
                return 17;
            if (name.Contains("stream"))
                return 15;
            if (name.Contains("tainted"))
                return 16;
            if (name.Contains("death"))
                return 26;

            return 10;
        }
    }
}
