using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LosBuilder.Generation
{
    internal static class TerrainRasterReader
    {
        public static TerrainData Load(string zoneFolder)
        {
            PcxImage terrain = PcxImage.Load(Path.Combine(zoneFolder, "terrain.pcx"));
            PcxImage offset = PcxImage.Load(Path.Combine(zoneFolder, "offset.pcx"));
            PcxImage holemap = PcxImage.TryLoad(Path.Combine(zoneFolder, "holemap.pcx"));
            int scaleFactor;
            int offsetFactor;
            LoadSector(Path.Combine(zoneFolder, "sector.dat"), out scaleFactor, out offsetFactor);

            if (terrain.Width != offset.Width || terrain.Height != offset.Height)
                throw new InvalidOperationException("Terrain and offset raster dimensions do not match.");

            int width = terrain.Width;
            int height = terrain.Height;
            int holeWidth = holemap != null ? holemap.Width : 0;
            int holeHeight = holemap != null ? holemap.Height : 0;

            ushort[] heights = new ushort[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int value = scaleFactor * terrain.GetValue(x, y) + offsetFactor * offset.GetValue(x, y);
                    heights[(y * width) + x] = (ushort)Math.Max(0, value);
                }
            }

            byte[] holes = new byte[holeWidth * holeHeight];
            if (holemap != null)
            {
                for (int y = 0; y < holeHeight; y++)
                {
                    for (int x = 0; x < holeWidth; x++)
                        holes[(y * holeWidth) + x] = holemap.GetValue(x, y);
                }
            }

            return new TerrainData
            {
                Width = width,
                Height = height,
                HoleWidth = holeWidth,
                HoleHeight = holeHeight,
                HeightMap = heights,
                HoleMap = holes
            };
        }

        private static void LoadSector(string path, out int scaleFactor, out int offsetFactor)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Missing sector.dat", path);

            scaleFactor = 0;
            offsetFactor = 0;

            foreach (string line in File.ReadLines(path))
            {
                if (line.StartsWith("scalefactor=", StringComparison.OrdinalIgnoreCase))
                    scaleFactor = ParseInt(line);
                else if (line.StartsWith("offsetfactor=", StringComparison.OrdinalIgnoreCase))
                    offsetFactor = ParseInt(line);
            }

        }

        private static int ParseInt(string line)
        {
            int separator = line.IndexOf('=');
            if (separator < 0)
                return 0;

            int value;
            if (int.TryParse(line.Substring(separator + 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                return value;

            return 0;
        }

        private sealed class PcxImage
        {
            private readonly int _width;
            private readonly int _height;
            private readonly byte[] _pixels;

            private PcxImage(int width, int height, byte[] pixels)
            {
                _width = width;
                _height = height;
                _pixels = pixels;
            }

            public int Width { get { return _width; } }
            public int Height { get { return _height; } }

            public static PcxImage Load(string path)
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException("Missing PCX raster", path);

                return TryLoad(path);
            }

            public static PcxImage TryLoad(string path)
            {
                if (!File.Exists(path))
                    return null;

                using (FileStream stream = File.OpenRead(path))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte manufacturer = reader.ReadByte();
                    byte version = reader.ReadByte();
                    byte encoding = reader.ReadByte();
                    byte bitsPerPixel = reader.ReadByte();
                    ushort xStart = reader.ReadUInt16();
                    ushort yStart = reader.ReadUInt16();
                    ushort xEnd = reader.ReadUInt16();
                    ushort yEnd = reader.ReadUInt16();
                    reader.ReadUInt16();
                    reader.ReadUInt16();
                    reader.ReadBytes(48);
                    reader.ReadByte();
                    byte numBitPlanes = reader.ReadByte();
                    ushort bytesPerLine = reader.ReadUInt16();
                    reader.ReadUInt16();
                    reader.ReadUInt16();
                    reader.ReadUInt16();
                    reader.ReadBytes(54);

                    if (manufacturer != 0x0A || version != 5 || encoding != 1 || bitsPerPixel != 8 || numBitPlanes != 1)
                        throw new InvalidOperationException("Unsupported PCX encoding in " + path);

                    int width = xEnd + 1 - xStart;
                    int height = yEnd + 1 - yStart;
                    byte[] pixels = new byte[width * height];
                    byte[] rowBuffer = new byte[bytesPerLine];

                    for (int row = 0; row < height; row++)
                    {
                        DecodeRow(reader, rowBuffer, bytesPerLine);
                        Buffer.BlockCopy(rowBuffer, 0, pixels, row * width, width);
                    }

                    return new PcxImage(width, height, pixels);
                }
            }

            public byte GetValue(int x, int y)
            {
                return _pixels[(y * Width) + x];
            }

            private static void DecodeRow(BinaryReader reader, byte[] rowBuffer, int bytesPerLine)
            {
                int position = 0;
                while (position < bytesPerLine)
                {
                    byte value = reader.ReadByte();
                    if ((value & 0xC0) == 0xC0)
                    {
                        int runLength = value & 0x3F;
                        byte repeated = reader.ReadByte();
                        for (int run = 0; run < runLength && position < bytesPerLine; run++)
                            rowBuffer[position++] = repeated;
                    }
                    else
                    {
                        rowBuffer[position++] = value;
                    }
                }
            }
        }
    }
}
