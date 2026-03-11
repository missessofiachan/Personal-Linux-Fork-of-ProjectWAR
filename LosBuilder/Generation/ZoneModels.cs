using System.Collections.Generic;
using System.Numerics;

namespace LosBuilder.Generation
{
    internal sealed class ZoneMetadata
    {
        public int ZoneId { get; set; }
        public int RegionId { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
    }

    internal sealed class ZoneNifRecord
    {
        public int Id { get; set; }
        public string TextualName { get; set; }
        public string FileName { get; set; }
        public int Collide { get; set; }
    }

    internal sealed class ZoneFixtureRecord
    {
        public int Id { get; set; }
        public int NifId { get; set; }
        public string TextualName { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int A { get; set; }
        public float ScalePercent { get; set; }
        public int Collide { get; set; }
        public int UniqueId { get; set; }
        public float AxisAngleRadians { get; set; }
        public float AxisX { get; set; }
        public float AxisY { get; set; }
        public float AxisZ { get; set; }
    }

    internal sealed class FigleafFixtureRecord
    {
        public string SourceName { get; set; }
        public string CollisionSourceName { get; set; }
    }

    internal sealed class TerrainData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int HoleWidth { get; set; }
        public int HoleHeight { get; set; }
        public ushort[] HeightMap { get; set; }
        public byte[] HoleMap { get; set; }
    }

    internal sealed class WaterBodySource
    {
        private readonly List<WaterPointPair> _pointPairs = new List<WaterPointPair>();

        public int Id { get; set; }
        public int SurfaceType { get; set; }
        public float Height { get; set; }
        public List<WaterPointPair> PointPairs { get { return _pointPairs; } }
    }

    internal sealed class WaterPointPair
    {
        public Vector2 Left { get; set; }
        public Vector2 Right { get; set; }
    }

    internal struct OccTriangle
    {
        public int I0;
        public int I1;
        public int I2;
        public int UniqueId;
    }

    internal sealed class GeometryChunkData
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<OccTriangle> _triangles = new List<OccTriangle>();

        public List<Vector3> Vertices { get { return _vertices; } }
        public List<OccTriangle> Triangles { get { return _triangles; } }
        public int FixtureCount { get; set; }
    }
}
