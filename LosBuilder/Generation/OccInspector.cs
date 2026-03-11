using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace LosBuilder.Generation
{
    internal static class OccInspector
    {
        public static bool Inspect(string inputBinPath, Action<string> writeLine)
        {
            OccFile file = OccReader.Read(inputBinPath);

            writeLine("OCC file: " + file.Path);
            writeLine("  version=" + file.Version + ", headerSize=" + file.HeaderSize + ", length=" + file.Length.ToString("N0", CultureInfo.InvariantCulture) + " bytes");

            if (file.Region != null)
                PrintRegion(file.Region, writeLine);

            if (file.Terrain != null)
                PrintTerrain(file.Terrain, writeLine);

            if (file.Collision != null)
                PrintGeometry("Collision", file.Collision, writeLine);

            if (file.Water != null)
                PrintGeometry("Water", file.Water, writeLine);

            return true;
        }

        public static bool Compare(string leftBinPath, string rightBinPath, Action<string> writeLine)
        {
            OccFile left = OccReader.Read(leftBinPath);
            OccFile right = OccReader.Read(rightBinPath);

            writeLine("Comparing OCC files");
            writeLine("  left : " + left.Path);
            writeLine("  right: " + right.Path);
            writeLine("  length: " + FormatComparison(left.Length, right.Length));
            writeLine("  version: " + FormatComparison(left.Version, right.Version));
            writeLine("  headerSize: " + FormatComparison(left.HeaderSize, right.HeaderSize));

            CompareRegions(left.Region, right.Region, writeLine);
            CompareTerrain(left.Terrain, right.Terrain, writeLine);
            CompareGeometry("collision", left.Collision, right.Collision, writeLine);
            CompareGeometry("water", left.Water, right.Water, writeLine);

            return true;
        }

        private static void PrintRegion(OccRegionChunk chunk, Action<string> writeLine)
        {
            writeLine("Region chunk:");
            writeLine("  regionId=" + chunk.RegionId + ", zoneCount=" + chunk.ZoneCount);

            foreach (OccRegionZoneEntry zone in chunk.Zones)
            {
                writeLine("  zone " + zone.ZoneId
                    + ": offset=(" + zone.OffsetX + "," + zone.OffsetY + ")"
                    + ", zoneNifs=" + zone.ZoneNifCount
                    + ", zoneFixtures=" + zone.ZoneFixtureCount);
            }
        }

        private static void PrintTerrain(OccTerrainChunk chunk, Action<string> writeLine)
        {
            writeLine("Terrain chunk:");
            writeLine("  zoneId=" + chunk.ZoneId
                + ", size=" + chunk.Width + "x" + chunk.Height
                + ", holeSize=" + chunk.HoleWidth + "x" + chunk.HoleHeight);
            writeLine("  heightRange=" + chunk.MinHeight + ".." + chunk.MaxHeight
                + ", openHoles=" + chunk.OpenHoleCount
                + ", closedHoles=" + chunk.ClosedHoleCount);
            writeLine("  heightHash=0x" + chunk.HeightHash.ToString("X16", CultureInfo.InvariantCulture)
                + ", holeHash=0x" + chunk.HoleHash.ToString("X16", CultureInfo.InvariantCulture));
        }

        private static void PrintGeometry(string label, OccGeometryChunk chunk, Action<string> writeLine)
        {
            writeLine(label + " chunk:");
            writeLine("  zoneId=" + chunk.ZoneId
                + ", vertices=" + chunk.Vertices.Length.ToString("N0", CultureInfo.InvariantCulture)
                + ", triangles=" + chunk.TriangleCount.ToString("N0", CultureInfo.InvariantCulture)
                + ", declaredFixtures=" + chunk.DeclaredFixtureCount
                + ", uniqueFixtures=" + chunk.UniqueFixtureCount
                + ", surfaceTypes=" + chunk.SurfaceTypeCount);
            writeLine("  groupedByUniqueId=" + chunk.TrianglesGroupedByUniqueId
                + ", vertexHash=0x" + chunk.VertexHash.ToString("X16", CultureInfo.InvariantCulture)
                + ", triangleHash=0x" + chunk.TriangleHash.ToString("X16", CultureInfo.InvariantCulture));
            writeLine("  boundsMin=" + FormatVector(chunk.MinBounds)
                + ", boundsMax=" + FormatVector(chunk.MaxBounds));

            int previewCount = Math.Min(8, chunk.FixtureBuckets.Count);
            if (previewCount == 0)
                return;

            writeLine("  fixture buckets:");
            for (int i = 0; i < previewCount; i++)
            {
                OccFixtureBucket bucket = chunk.FixtureBuckets[i];
                writeLine("    [" + i + "] uniqueId=" + bucket.UniqueId
                    + " (surface=" + bucket.SurfaceType
                    + ", fixture=" + bucket.FixtureId
                    + "), start=" + bucket.TriangleStartIndex
                    + ", count=" + bucket.TriangleCount);
            }
        }

        private static void CompareRegions(OccRegionChunk left, OccRegionChunk right, Action<string> writeLine)
        {
            writeLine("Region:");
            if (left == null || right == null)
            {
                writeLine("  presence: " + FormatPresence(left != null, right != null));
                return;
            }

            writeLine("  regionId: " + FormatComparison(left.RegionId, right.RegionId));
            writeLine("  zoneCount: " + FormatComparison(left.ZoneCount, right.ZoneCount));

            int zoneCount = Math.Min(left.Zones.Count, right.Zones.Count);
            for (int i = 0; i < zoneCount; i++)
            {
                OccRegionZoneEntry leftZone = left.Zones[i];
                OccRegionZoneEntry rightZone = right.Zones[i];
                writeLine("  zone[" + i + "] zoneId: " + FormatComparison(leftZone.ZoneId, rightZone.ZoneId));
                writeLine("  zone[" + i + "] offsetX: " + FormatComparison(leftZone.OffsetX, rightZone.OffsetX));
                writeLine("  zone[" + i + "] offsetY: " + FormatComparison(leftZone.OffsetY, rightZone.OffsetY));
                writeLine("  zone[" + i + "] zoneNifs: " + FormatComparison(leftZone.ZoneNifCount, rightZone.ZoneNifCount));
                writeLine("  zone[" + i + "] zoneFixtures: " + FormatComparison(leftZone.ZoneFixtureCount, rightZone.ZoneFixtureCount));
            }
        }

        private static void CompareTerrain(OccTerrainChunk left, OccTerrainChunk right, Action<string> writeLine)
        {
            writeLine("Terrain:");
            if (left == null || right == null)
            {
                writeLine("  presence: " + FormatPresence(left != null, right != null));
                return;
            }

            writeLine("  size: " + FormatComparison(left.Width + "x" + left.Height, right.Width + "x" + right.Height));
            writeLine("  holeSize: " + FormatComparison(left.HoleWidth + "x" + left.HoleHeight, right.HoleWidth + "x" + right.HoleHeight));
            writeLine("  minHeight: " + FormatComparison(left.MinHeight, right.MinHeight));
            writeLine("  maxHeight: " + FormatComparison(left.MaxHeight, right.MaxHeight));
            writeLine("  openHoles: " + FormatComparison(left.OpenHoleCount, right.OpenHoleCount));
            writeLine("  closedHoles: " + FormatComparison(left.ClosedHoleCount, right.ClosedHoleCount));
            writeLine("  heightHash: " + FormatComparison(HashString(left.HeightHash), HashString(right.HeightHash)));
            writeLine("  holeHash: " + FormatComparison(HashString(left.HoleHash), HashString(right.HoleHash)));
        }

        private static void CompareGeometry(string label, OccGeometryChunk left, OccGeometryChunk right, Action<string> writeLine)
        {
            writeLine(label.Substring(0, 1).ToUpperInvariant() + label.Substring(1) + ":");
            if (left == null || right == null)
            {
                writeLine("  presence: " + FormatPresence(left != null, right != null));
                return;
            }

            writeLine("  vertices: " + FormatComparison(left.Vertices.Length, right.Vertices.Length));
            writeLine("  triangles: " + FormatComparison(left.TriangleCount, right.TriangleCount));
            writeLine("  declaredFixtures: " + FormatComparison(left.DeclaredFixtureCount, right.DeclaredFixtureCount));
            writeLine("  uniqueFixtures: " + FormatComparison(left.UniqueFixtureCount, right.UniqueFixtureCount));
            writeLine("  surfaceTypes: " + FormatComparison(left.SurfaceTypeCount, right.SurfaceTypeCount));
            writeLine("  groupedByUniqueId: " + FormatComparison(left.TrianglesGroupedByUniqueId, right.TrianglesGroupedByUniqueId));
            writeLine("  vertexHash: " + FormatComparison(HashString(left.VertexHash), HashString(right.VertexHash)));
            writeLine("  triangleHash: " + FormatComparison(HashString(left.TriangleHash), HashString(right.TriangleHash)));
            writeLine("  boundsMin: " + FormatComparison(FormatVector(left.MinBounds), FormatVector(right.MinBounds)));
            writeLine("  boundsMax: " + FormatComparison(FormatVector(left.MaxBounds), FormatVector(right.MaxBounds)));

            CompareFixtureBuckets(label, left.FixtureBuckets, right.FixtureBuckets, writeLine);
        }

        private static void CompareFixtureBuckets(string label, List<OccFixtureBucket> left, List<OccFixtureBucket> right, Action<string> writeLine)
        {
            writeLine("  fixtureBuckets: " + FormatComparison(left.Count, right.Count));

            int bucketCount = Math.Min(Math.Min(left.Count, right.Count), 5);
            for (int i = 0; i < bucketCount; i++)
            {
                OccFixtureBucket leftBucket = left[i];
                OccFixtureBucket rightBucket = right[i];
                writeLine("  " + label + " bucket[" + i + "] uniqueId: " + FormatComparison(leftBucket.UniqueId, rightBucket.UniqueId));
                writeLine("  " + label + " bucket[" + i + "] start: " + FormatComparison(leftBucket.TriangleStartIndex, rightBucket.TriangleStartIndex));
                writeLine("  " + label + " bucket[" + i + "] count: " + FormatComparison(leftBucket.TriangleCount, rightBucket.TriangleCount));
            }
        }

        private static string FormatPresence(bool left, bool right)
        {
            return left == right
                ? left.ToString()
                : left + " vs " + right;
        }

        private static string FormatComparison(object left, object right)
        {
            if (Equals(left, right))
                return left + " (match)";

            return left + " vs " + right;
        }

        private static string FormatVector(Vector3 value)
        {
            return "("
                + value.X.ToString("0.###", CultureInfo.InvariantCulture) + ", "
                + value.Y.ToString("0.###", CultureInfo.InvariantCulture) + ", "
                + value.Z.ToString("0.###", CultureInfo.InvariantCulture) + ")";
        }

        private static string HashString(ulong value)
        {
            return "0x" + value.ToString("X16", CultureInfo.InvariantCulture);
        }
    }
}
