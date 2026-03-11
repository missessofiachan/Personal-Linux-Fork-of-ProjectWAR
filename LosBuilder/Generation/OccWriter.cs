using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace LosBuilder.Generation
{
    internal static class OccWriter
    {
        public static void Write(
            string outputPath,
            ZoneMetadata metadata,
            TerrainData terrain,
            GeometryChunkData collision,
            GeometryChunkData water,
            int zoneNifCount,
            int zoneFixtureCount)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            using (FileStream stream = File.Create(outputPath))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write((byte)'O');
                writer.Write((byte)'C');
                writer.Write((byte)'C');
                writer.Write((byte)3);
                writer.Write((byte)16);
                writer.Write(new byte[11]);

                WriteChunk(writer, 7, payload =>
                {
                    payload.Write(metadata.RegionId);
                    payload.Write(1);
                    payload.Write(metadata.ZoneId);
                    payload.Write(metadata.OffsetX);
                    payload.Write(metadata.OffsetY);
                    payload.Write(zoneNifCount);
                    payload.Write(zoneFixtureCount);
                });

                WriteChunk(writer, 4, payload =>
                {
                    payload.Write(metadata.RegionId);
                    payload.Write(metadata.ZoneId);
                    payload.Write(terrain.Width);
                    payload.Write(terrain.Height);
                    payload.Write(terrain.HoleWidth);
                    payload.Write(terrain.HoleHeight);

                    foreach (ushort value in terrain.HeightMap)
                        payload.Write(value);

                    payload.Write(terrain.HoleMap);
                });

                WriteGeometryChunk(writer, 5, metadata, collision);

                if (water != null && water.Triangles.Count > 0)
                    WriteGeometryChunk(writer, 8, metadata, water);
            }
        }

        private static void WriteGeometryChunk(BinaryWriter writer, int chunkType, ZoneMetadata metadata, GeometryChunkData geometry)
        {
            WriteChunk(writer, chunkType, payload =>
            {
                payload.Write(metadata.RegionId);
                payload.Write(metadata.ZoneId);
                payload.Write(geometry.Vertices.Count);

                foreach (Vector3 vertex in geometry.Vertices)
                {
                    payload.Write(vertex.X);
                    payload.Write(vertex.Y);
                    payload.Write(vertex.Z);
                }

                payload.Write(geometry.FixtureCount);
                payload.Write(geometry.Triangles.Count);
                payload.Write(4);

                foreach (OccTriangle triangle in geometry.Triangles)
                {
                    payload.Write(triangle.I0);
                    payload.Write(triangle.I1);
                    payload.Write(triangle.I2);
                    payload.Write(triangle.UniqueId);
                }
            });
        }

        private static void WriteChunk(BinaryWriter writer, int chunkType, System.Action<BinaryWriter> writePayload)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter payloadWriter = new BinaryWriter(memoryStream))
            {
                writePayload(payloadWriter);
                payloadWriter.Flush();

                writer.Write(chunkType);
                writer.Write((int)memoryStream.Length);
                writer.Write(memoryStream.ToArray());
            }
        }
    }
}
