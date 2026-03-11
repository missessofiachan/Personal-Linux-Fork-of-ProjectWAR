using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace LosBuilder.Generation
{
    internal static class OccReader
    {
        private const ulong FnvOffsetBasis = 14695981039346656037UL;
        private const ulong FnvPrime = 1099511628211UL;

        public static OccFile Read(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("OCC file not found.", path);

            OccFile occFile = new OccFile
            {
                Path = Path.GetFullPath(path),
                Length = new FileInfo(path).Length
            };

            using (FileStream stream = File.OpenRead(path))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string fileCode = new string(new[]
                {
                    reader.ReadChar(),
                    reader.ReadChar(),
                    reader.ReadChar()
                });

                if (!fileCode.Equals("OCC", StringComparison.Ordinal))
                    throw new InvalidDataException("Invalid OCC header.");

                occFile.Version = reader.ReadByte();
                occFile.HeaderSize = reader.ReadByte();
                stream.Position = occFile.HeaderSize;

                while (stream.Position < stream.Length)
                {
                    int chunkType = reader.ReadInt32();
                    int chunkSize = reader.ReadInt32();
                    long chunkStart = stream.Position;
                    OccChunkInfo chunk = ReadChunk(reader, chunkType, chunkSize);

                    occFile.Chunks.Add(chunk);
                    AssignChunk(occFile, chunk);
                    stream.Position = chunkStart + chunkSize;
                }
            }

            return occFile;
        }

        private static OccChunkInfo ReadChunk(BinaryReader reader, int chunkType, int chunkSize)
        {
            switch ((OccChunkType)chunkType)
            {
                case OccChunkType.Region:
                    return ReadRegionChunk(reader, chunkType, chunkSize);
                case OccChunkType.Terrain:
                    return ReadTerrainChunk(reader, chunkType, chunkSize);
                case OccChunkType.Collision:
                case OccChunkType.Water:
                    return ReadGeometryChunk(reader, chunkType, chunkSize);
                default:
                    return new OccChunkInfo
                    {
                        Type = chunkType,
                        Size = chunkSize
                    };
            }
        }

        private static OccRegionChunk ReadRegionChunk(BinaryReader reader, int chunkType, int chunkSize)
        {
            OccRegionChunk chunk = new OccRegionChunk
            {
                Type = chunkType,
                Size = chunkSize,
                RegionId = reader.ReadInt32(),
                ZoneCount = reader.ReadInt32()
            };

            for (int i = 0; i < chunk.ZoneCount; i++)
            {
                chunk.Zones.Add(new OccRegionZoneEntry
                {
                    ZoneId = reader.ReadInt32(),
                    OffsetX = reader.ReadInt32(),
                    OffsetY = reader.ReadInt32(),
                    ZoneNifCount = reader.ReadInt32(),
                    ZoneFixtureCount = reader.ReadInt32()
                });
            }

            return chunk;
        }

        private static OccTerrainChunk ReadTerrainChunk(BinaryReader reader, int chunkType, int chunkSize)
        {
            OccTerrainChunk chunk = new OccTerrainChunk
            {
                Type = chunkType,
                Size = chunkSize,
                RegionId = reader.ReadInt32(),
                ZoneId = reader.ReadInt32(),
                Width = reader.ReadInt32(),
                Height = reader.ReadInt32(),
                HoleWidth = reader.ReadInt32(),
                HoleHeight = reader.ReadInt32()
            };

            int heightCount = checked(chunk.Width * chunk.Height);
            chunk.HeightMap = new ushort[heightCount];

            int minHeight = int.MaxValue;
            int maxHeight = int.MinValue;
            ulong heightHash = FnvOffsetBasis;
            for (int i = 0; i < heightCount; i++)
            {
                ushort value = reader.ReadUInt16();
                chunk.HeightMap[i] = value;

                if (value < minHeight)
                    minHeight = value;
                if (value > maxHeight)
                    maxHeight = value;

                heightHash = AddByte(heightHash, (byte)(value & 0xFF));
                heightHash = AddByte(heightHash, (byte)(value >> 8));
            }

            int holeCount = checked(chunk.HoleWidth * chunk.HoleHeight);
            chunk.HoleMap = reader.ReadBytes(holeCount);

            ulong holeHash = FnvOffsetBasis;
            int openHoleCount = 0;
            for (int i = 0; i < chunk.HoleMap.Length; i++)
            {
                byte value = chunk.HoleMap[i];
                holeHash = AddByte(holeHash, value);
                if (value == 0)
                    openHoleCount++;
            }

            chunk.MinHeight = minHeight == int.MaxValue ? 0 : minHeight;
            chunk.MaxHeight = maxHeight == int.MinValue ? 0 : maxHeight;
            chunk.OpenHoleCount = openHoleCount;
            chunk.ClosedHoleCount = holeCount - openHoleCount;
            chunk.HeightHash = heightHash;
            chunk.HoleHash = holeHash;

            return chunk;
        }

        private static OccGeometryChunk ReadGeometryChunk(BinaryReader reader, int chunkType, int chunkSize)
        {
            OccGeometryChunk chunk = new OccGeometryChunk
            {
                Type = chunkType,
                Size = chunkSize,
                RegionId = reader.ReadInt32(),
                ZoneId = reader.ReadInt32()
            };

            int vertexCount = reader.ReadInt32();
            chunk.Vertices = new Vector3[vertexCount];

            ulong vertexHash = FnvOffsetBasis;
            Vector3 minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 maxBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < vertexCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                Vector3 vertex = new Vector3(x, y, z);
                chunk.Vertices[i] = vertex;

                vertexHash = AddSingle(vertexHash, x);
                vertexHash = AddSingle(vertexHash, y);
                vertexHash = AddSingle(vertexHash, z);

                minBounds = Vector3.Min(minBounds, vertex);
                maxBounds = Vector3.Max(maxBounds, vertex);
            }

            chunk.DeclaredFixtureCount = reader.ReadInt32();
            chunk.TriangleCount = reader.ReadInt32();
            chunk.IndexSize = reader.ReadInt32();
            chunk.Triangles = new OccTriangle[chunk.TriangleCount];

            ulong triangleHash = FnvOffsetBasis;
            HashSet<int> uniqueIds = new HashSet<int>();
            HashSet<int> surfaceTypes = new HashSet<int>();
            Dictionary<int, int> runCountByUniqueId = new Dictionary<int, int>();

            int currentUniqueId = int.MinValue;
            OccFixtureBucket currentBucket = null;

            for (int i = 0; i < chunk.TriangleCount; i++)
            {
                OccTriangle triangle = new OccTriangle
                {
                    I0 = reader.ReadInt32(),
                    I1 = reader.ReadInt32(),
                    I2 = reader.ReadInt32(),
                    UniqueId = reader.ReadInt32()
                };

                chunk.Triangles[i] = triangle;
                triangleHash = AddInt32(triangleHash, triangle.I0);
                triangleHash = AddInt32(triangleHash, triangle.I1);
                triangleHash = AddInt32(triangleHash, triangle.I2);
                triangleHash = AddInt32(triangleHash, triangle.UniqueId);

                uniqueIds.Add(triangle.UniqueId);
                surfaceTypes.Add(triangle.UniqueId >> 24);

                if (triangle.UniqueId != currentUniqueId)
                {
                    currentUniqueId = triangle.UniqueId;
                    currentBucket = new OccFixtureBucket
                    {
                        UniqueId = triangle.UniqueId,
                        FixtureId = triangle.UniqueId & 0xFFFFFF,
                        SurfaceType = triangle.UniqueId >> 24,
                        TriangleStartIndex = i,
                        TriangleCount = 0
                    };

                    chunk.FixtureBuckets.Add(currentBucket);

                    int runCount;
                    runCountByUniqueId.TryGetValue(triangle.UniqueId, out runCount);
                    runCountByUniqueId[triangle.UniqueId] = runCount + 1;
                }

                currentBucket.TriangleCount++;
            }

            bool groupedByUniqueId = true;
            foreach (KeyValuePair<int, int> entry in runCountByUniqueId)
            {
                if (entry.Value > 1)
                {
                    groupedByUniqueId = false;
                    break;
                }
            }

            chunk.UniqueFixtureCount = uniqueIds.Count;
            chunk.SurfaceTypeCount = surfaceTypes.Count;
            chunk.TrianglesGroupedByUniqueId = groupedByUniqueId;
            chunk.VertexHash = vertexHash;
            chunk.TriangleHash = triangleHash;
            chunk.MinBounds = vertexCount == 0 ? Vector3.Zero : minBounds;
            chunk.MaxBounds = vertexCount == 0 ? Vector3.Zero : maxBounds;

            return chunk;
        }

        private static void AssignChunk(OccFile occFile, OccChunkInfo chunk)
        {
            OccRegionChunk regionChunk = chunk as OccRegionChunk;
            if (regionChunk != null)
            {
                occFile.Region = regionChunk;
                return;
            }

            OccTerrainChunk terrainChunk = chunk as OccTerrainChunk;
            if (terrainChunk != null)
            {
                occFile.Terrain = terrainChunk;
                return;
            }

            OccGeometryChunk geometryChunk = chunk as OccGeometryChunk;
            if (geometryChunk == null)
                return;

            if (geometryChunk.Type == (int)OccChunkType.Collision)
                occFile.Collision = geometryChunk;
            else if (geometryChunk.Type == (int)OccChunkType.Water)
                occFile.Water = geometryChunk;
        }

        private static ulong AddSingle(ulong hash, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
                hash = AddByte(hash, bytes[i]);

            return hash;
        }

        private static ulong AddInt32(ulong hash, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
                hash = AddByte(hash, bytes[i]);

            return hash;
        }

        private static ulong AddByte(ulong hash, byte value)
        {
            hash ^= value;
            hash *= FnvPrime;
            return hash;
        }
    }
}
