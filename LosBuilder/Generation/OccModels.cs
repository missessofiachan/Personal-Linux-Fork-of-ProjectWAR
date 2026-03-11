using System.Collections.Generic;
using System.Numerics;

namespace LosBuilder.Generation
{
    internal enum LosCommand
    {
        Generate,
        Inspect,
        Compare
    }

    internal enum OccChunkType
    {
        Terrain = 4,
        Collision = 5,
        Region = 7,
        Water = 8
    }

    internal sealed class OccFile
    {
        private readonly List<OccChunkInfo> _chunks = new List<OccChunkInfo>();

        public string Path { get; set; }
        public long Length { get; set; }
        public byte Version { get; set; }
        public byte HeaderSize { get; set; }
        public List<OccChunkInfo> Chunks { get { return _chunks; } }
        public OccRegionChunk Region { get; set; }
        public OccTerrainChunk Terrain { get; set; }
        public OccGeometryChunk Collision { get; set; }
        public OccGeometryChunk Water { get; set; }
    }

    internal class OccChunkInfo
    {
        public int Type { get; set; }
        public int Size { get; set; }
    }

    internal sealed class OccRegionChunk : OccChunkInfo
    {
        private readonly List<OccRegionZoneEntry> _zones = new List<OccRegionZoneEntry>();

        public int RegionId { get; set; }
        public int ZoneCount { get; set; }
        public List<OccRegionZoneEntry> Zones { get { return _zones; } }
    }

    internal sealed class OccRegionZoneEntry
    {
        public int ZoneId { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int ZoneNifCount { get; set; }
        public int ZoneFixtureCount { get; set; }
    }

    internal sealed class OccTerrainChunk : OccChunkInfo
    {
        public int RegionId { get; set; }
        public int ZoneId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int HoleWidth { get; set; }
        public int HoleHeight { get; set; }
        public ushort[] HeightMap { get; set; }
        public byte[] HoleMap { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public int OpenHoleCount { get; set; }
        public int ClosedHoleCount { get; set; }
        public ulong HeightHash { get; set; }
        public ulong HoleHash { get; set; }
    }

    internal sealed class OccGeometryChunk : OccChunkInfo
    {
        private readonly List<OccFixtureBucket> _fixtureBuckets = new List<OccFixtureBucket>();

        public int RegionId { get; set; }
        public int ZoneId { get; set; }
        public Vector3[] Vertices { get; set; }
        public OccTriangle[] Triangles { get; set; }
        public int DeclaredFixtureCount { get; set; }
        public int TriangleCount { get; set; }
        public int IndexSize { get; set; }
        public int UniqueFixtureCount { get; set; }
        public int SurfaceTypeCount { get; set; }
        public bool TrianglesGroupedByUniqueId { get; set; }
        public ulong VertexHash { get; set; }
        public ulong TriangleHash { get; set; }
        public Vector3 MinBounds { get; set; }
        public Vector3 MaxBounds { get; set; }
        public List<OccFixtureBucket> FixtureBuckets { get { return _fixtureBuckets; } }
    }

    internal sealed class OccFixtureBucket
    {
        public int UniqueId { get; set; }
        public int FixtureId { get; set; }
        public int SurfaceType { get; set; }
        public int TriangleStartIndex { get; set; }
        public int TriangleCount { get; set; }
    }
}
