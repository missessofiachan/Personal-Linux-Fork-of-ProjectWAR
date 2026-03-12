using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LosBuilder.Generation
{
    internal sealed class FigleafMetadataReader
    {
        private const int StringsHeaderPosition = 0x0C;
        private const int FixtureHeaderPosition = 0x78;
        private const int RegionHeaderPosition = 0x9C;
        private const int RegionRecordSize = 0x20;
        private const int ZoneRecordSize = 0x1E;

        private readonly Dictionary<int, ZoneMetadata> _zones = new Dictionary<int, ZoneMetadata>();
        private readonly Dictionary<string, FigleafFixtureRecord> _fixtureBySource = new Dictionary<string, FigleafFixtureRecord>(System.StringComparer.OrdinalIgnoreCase);

        public Dictionary<int, ZoneMetadata> Zones { get { return _zones; } }
        public Dictionary<string, FigleafFixtureRecord> FixtureBySource { get { return _fixtureBySource; } }

        public static FigleafMetadataReader Load(string path)
        {
            FigleafMetadataReader figleaf = new FigleafMetadataReader();

            using (FileStream stream = File.OpenRead(path))
            using (BinaryReader reader = new BinaryReader(stream, Encoding.GetEncoding(437)))
            {
                List<string> strings = figleaf.LoadStringTable(reader);
                figleaf.LoadFixtureTable(reader, strings);
                figleaf.LoadRegionTable(reader);
            }

            return figleaf;
        }

        private List<string> LoadStringTable(BinaryReader reader)
        {
            reader.BaseStream.Position = StringsHeaderPosition;
            uint entryCount = reader.ReadUInt32();
            uint offset = reader.ReadUInt32();
            reader.ReadUInt32();

            List<string> strings = new List<string>((int)entryCount);
            reader.BaseStream.Position = offset;

            for (int i = 0; i < entryCount; i++)
            {
                reader.ReadUInt32();
                reader.ReadByte();

                List<byte> buffer = new List<byte>(64);
                byte current;
                while ((current = reader.ReadByte()) != 0)
                    buffer.Add(current);

                reader.ReadByte();
                strings.Add(Encoding.GetEncoding(437).GetString(buffer.ToArray()));
            }

            return strings;
        }

        private void LoadFixtureTable(BinaryReader reader, List<string> strings)
        {
            reader.BaseStream.Position = FixtureHeaderPosition;
            uint entryCount = reader.ReadUInt32();
            uint offset = reader.ReadUInt32();
            reader.ReadUInt32();

            reader.BaseStream.Position = offset;
            for (int i = 0; i < entryCount; i++)
            {
                int sourceIndex = (int)reader.ReadUInt32();
                for (int skip = 0; skip < 13; skip++)
                    reader.ReadUInt32();

                int collisionSourceIndex = (int)reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();

                string sourceName = GetString(strings, sourceIndex);
                if (string.IsNullOrWhiteSpace(sourceName))
                    continue;

                FixtureBySource[AssetIndex.NormalizeLookupKey(sourceName)] = new FigleafFixtureRecord
                {
                    SourceName = sourceName,
                    CollisionSourceName = GetString(strings, collisionSourceIndex)
                };
            }
        }

        private void LoadRegionTable(BinaryReader reader)
        {
            reader.BaseStream.Position = RegionHeaderPosition;
            uint entryCount = reader.ReadUInt32();
            uint offset = reader.ReadUInt32();
            reader.ReadUInt32();

            for (int i = 0; i < entryCount; i++)
            {
                reader.BaseStream.Position = offset + (i * RegionRecordSize);
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadInt32();
                uint regionId = reader.ReadUInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                uint zoneCount = reader.ReadUInt32();
                int zoneStart = reader.ReadInt32();

                if (zoneCount == 0 || zoneStart <= 0)
                    continue;

                reader.BaseStream.Position = offset + zoneStart + (i * RegionRecordSize);
                for (int zoneIndex = 0; zoneIndex < zoneCount; zoneIndex++)
                {
                    ushort zoneId = reader.ReadUInt16();
                    reader.ReadUInt16();
                    ushort zoneXOff = reader.ReadUInt16();
                    ushort zoneYOff = reader.ReadUInt16();

                    for (int extra = 0; extra < (ZoneRecordSize - 8) / 2; extra++)
                        reader.ReadUInt16();

                    Zones[zoneId] = new ZoneMetadata
                    {
                        ZoneId = zoneId,
                        RegionId = (int)regionId,
                        OffsetX = zoneXOff << 13,
                        OffsetY = zoneYOff << 13
                    };
                }
            }
        }

        private static string GetString(List<string> strings, int index)
        {
            return index >= 0 && index < strings.Count ? strings[index] : null;
        }
    }
}
