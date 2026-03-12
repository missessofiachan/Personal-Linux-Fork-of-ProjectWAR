using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Niflib;
using Niflib.Extensions;

namespace LosBuilder.Generation
{
    internal sealed class LosGenerator
    {
        private readonly Action<string> _info;
        private readonly Action<string> _error;

        public LosGenerator(Action<string> info, Action<string> error)
        {
            _info = info;
            _error = error;
        }

        public bool Generate(BuildOptions options)
        {
            string figleafPath = Path.Combine(options.InputRoot, "assetdb", "figleaf.db");
            string assetDbRoot = Path.Combine(options.InputRoot, "assetdb");
            string zonesRoot = Path.Combine(options.InputRoot, "zones");

            if (!File.Exists(figleafPath))
                throw new FileNotFoundException("figleaf.db not found", figleafPath);
            if (!Directory.Exists(assetDbRoot))
                throw new DirectoryNotFoundException("assetdb folder not found: " + assetDbRoot);
            if (!Directory.Exists(zonesRoot))
                throw new DirectoryNotFoundException("zones folder not found: " + zonesRoot);

            FigleafMetadataReader figleaf = FigleafMetadataReader.Load(figleafPath);
            AssetIndex assetIndex = AssetIndex.Build(assetDbRoot, _info);
            List<int> targetZones = options.GenerateAllZones
                ? figleaf.Zones.Keys.OrderBy(x => x).ToList()
                : options.ZoneIds.Distinct().OrderBy(x => x).ToList();

            Directory.CreateDirectory(options.OutputRoot);

            List<int> failures = new List<int>();
            Dictionary<string, TriangleCollection> meshCache = new Dictionary<string, TriangleCollection>(StringComparer.OrdinalIgnoreCase);

            foreach (int zoneId in targetZones)
            {
                if (!TryGenerateZone(zoneId, zonesRoot, options.OutputRoot, figleaf, assetIndex, meshCache))
                    failures.Add(zoneId);
            }

            if (failures.Count == 0)
            {
                _info("LOS generation completed successfully for " + targetZones.Count + " zone(s).");
                return true;
            }

            _error("LOS generation failed for zone(s): " + string.Join(", ", failures));
            return false;
        }

        private bool TryGenerateZone(
            int zoneId,
            string zonesRoot,
            string outputRoot,
            FigleafMetadataReader figleaf,
            AssetIndex assetIndex,
            Dictionary<string, TriangleCollection> meshCache)
        {
            try
            {
                ZoneMetadata metadata;
                if (!figleaf.Zones.TryGetValue(zoneId, out metadata))
                {
                    _error("Skipping zone " + zoneId + ": no figleaf region metadata.");
                    return false;
                }

                string zoneFolder = Path.Combine(zonesRoot, "zone" + zoneId.ToString("D3"));
                if (!Directory.Exists(zoneFolder))
                {
                    _error("Skipping zone " + zoneId + ": missing source folder " + zoneFolder);
                    return false;
                }

                TerrainData terrain = TerrainRasterReader.Load(zoneFolder);
                Dictionary<int, ZoneNifRecord> zoneNifs = CsvReaders.ReadZoneNifs(zoneFolder);
                List<ZoneFixtureRecord> fixtures = CsvReaders.ReadFixtures(zoneFolder);
                List<WaterBodySource> waterBodies = WaterXmlReader.Load(zoneFolder);

                GeometryChunkData collision = BuildCollisionGeometry(zoneId, zoneNifs, fixtures, figleaf, assetIndex, meshCache);
                GeometryChunkData water = BuildWaterGeometry(waterBodies);

                string outputPath = Path.Combine(outputRoot, zoneId + ".bin");
                OccWriter.Write(outputPath, metadata, terrain, collision, water, zoneNifs.Count, fixtures.Count);

                _info("Generated zone " + zoneId + " to " + outputPath
                    + " [terrain=" + terrain.Width + "x" + terrain.Height
                    + ", collisionTris=" + collision.Triangles.Count
                    + ", waterTris=" + water.Triangles.Count + "]");
                return true;
            }
            catch (Exception ex)
            {
                _error("Zone " + zoneId + " failed: " + ex);
                return false;
            }
        }

        private GeometryChunkData BuildCollisionGeometry(
            int zoneId,
            Dictionary<int, ZoneNifRecord> zoneNifs,
            List<ZoneFixtureRecord> fixtures,
            FigleafMetadataReader figleaf,
            AssetIndex assetIndex,
            Dictionary<string, TriangleCollection> meshCache)
        {
            GeometryChunkData geometry = new GeometryChunkData();
            int fixtureCount = 0;

            foreach (ZoneFixtureRecord fixture in fixtures.OrderBy(x => x.UniqueId))
            {
                if (!IsCollidableFixture(fixture, zoneNifs))
                    continue;

                ZoneNifRecord zoneNif;
                if (!zoneNifs.TryGetValue(fixture.NifId, out zoneNif))
                {
                    throw new InvalidOperationException(
                        "Zone " + zoneId + " fixture " + fixture.UniqueId + " references missing NIF id " + fixture.NifId + ".");
                }

                FigleafFixtureRecord figleafFixture;
                figleaf.FixtureBySource.TryGetValue(AssetIndex.NormalizeLookupKey(zoneNif.FileName), out figleafFixture);

                string collisionSourceName = figleafFixture != null ? figleafFixture.CollisionSourceName : null;
                string meshPath;
                if (!assetIndex.TryResolve(zoneNif.FileName, collisionSourceName, out meshPath))
                {
                    throw new FileNotFoundException(
                        "Unable to resolve collision mesh for fixture " + fixture.UniqueId + " (" + zoneNif.FileName + ").",
                        collisionSourceName ?? zoneNif.FileName);
                }

                if (!meshCache.ContainsKey(meshPath))
                {
                    _info("Loading mesh " + meshPath + " for zone fixture " + fixture.UniqueId + ".");
                }

                TriangleCollection mesh;
                try
                {
                    mesh = LoadMesh(meshPath, meshCache);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        "Unable to load mesh '" + meshPath + "' for fixture " + fixture.UniqueId
                        + " source '" + zoneNif.FileName + "'"
                        + (string.IsNullOrWhiteSpace(collisionSourceName) ? string.Empty : " collision '" + collisionSourceName + "'")
                        + ": " + ex.Message,
                        ex);
                }

                if (mesh.Indices == null || mesh.Indices.Length == 0 || mesh.Vertices == null || mesh.Vertices.Length == 0)
                    continue;

                int baseVertexIndex = geometry.Vertices.Count;
                Quaternion rotation = BuildFixtureRotation(fixture);
                float scale = fixture.ScalePercent <= 0f ? 1f : fixture.ScalePercent / 100f;
                Vector3 translation = new Vector3(fixture.X, fixture.Y, fixture.Z);

                for (int i = 0; i < mesh.Vertices.Length; i++)
                {
                    Vector3 scaled = mesh.Vertices[i] * scale;
                    Vector3 rotated = Vector3.Transform(scaled, rotation);
                    Vector3 world = rotated + translation;
                    geometry.Vertices.Add(new Vector3(65535f - world.X, world.Y, world.Z));
                }

                foreach (TriangleIndex triangle in mesh.Indices)
                {
                    geometry.Triangles.Add(new OccTriangle
                    {
                        I0 = baseVertexIndex + (int)triangle.A,
                        I1 = baseVertexIndex + (int)triangle.B,
                        I2 = baseVertexIndex + (int)triangle.C,
                        UniqueId = fixture.UniqueId & 0xFFFFFF
                    });
                }

                fixtureCount++;
            }

            geometry.FixtureCount = fixtureCount;
            return geometry;
        }

        private static GeometryChunkData BuildWaterGeometry(List<WaterBodySource> waterBodies)
        {
            GeometryChunkData geometry = new GeometryChunkData();
            int fixtureCount = 0;

            foreach (WaterBodySource body in waterBodies)
            {
                if (body.PointPairs.Count < 2)
                    continue;

                int baseVertexIndex = geometry.Vertices.Count;
                for (int i = 0; i < body.PointPairs.Count; i++)
                {
                    WaterPointPair pair = body.PointPairs[i];
                    geometry.Vertices.Add(new Vector3(65535f - pair.Left.X, pair.Left.Y, body.Height));
                    geometry.Vertices.Add(new Vector3(65535f - pair.Right.X, pair.Right.Y, body.Height));
                }

                int uniqueId = (body.SurfaceType << 24) | ((0xFFFF + body.Id) & 0xFFFFFF);
                for (int pairIndex = 0; pairIndex < body.PointPairs.Count - 1; pairIndex++)
                {
                    int left0 = baseVertexIndex + (pairIndex * 2);
                    int right0 = left0 + 1;
                    int left1 = left0 + 2;
                    int right1 = left0 + 3;

                    geometry.Triangles.Add(new OccTriangle
                    {
                        I0 = left0,
                        I1 = right0,
                        I2 = right1,
                        UniqueId = uniqueId
                    });
                    geometry.Triangles.Add(new OccTriangle
                    {
                        I0 = left0,
                        I1 = left1,
                        I2 = right1,
                        UniqueId = uniqueId
                    });
                }

                fixtureCount++;
            }

            geometry.FixtureCount = fixtureCount;
            return geometry;
        }

        private static TriangleCollection LoadMesh(string meshPath, Dictionary<string, TriangleCollection> meshCache)
        {
            TriangleCollection cached;
            if (meshCache.TryGetValue(meshPath, out cached))
                return cached;

            using (FileStream stream = File.OpenRead(meshPath))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                NiFile file = new NiFile(reader);
                TriangleCollection collision = new TriangleCollection { Vertices = new Vector3[0], Indices = new TriangleIndex[0] };
                TriangleCollection allGeo = new TriangleCollection { Vertices = new Vector3[0], Indices = new TriangleIndex[0] };

                foreach (NiNode root in file.GetRoots())
                {
                    TriangleCollection col = root.GetCollisionTrianglesFromNode();
                    TriangleCollection all = root.GetTrianglesFromNode(onlyDrawable: false);
                    TriangleCollection nextCol, nextAll;
                    TriangleWalker.Concat(ref collision, ref col, out nextCol);
                    TriangleWalker.Concat(ref allGeo, ref all, out nextAll);
                    collision = nextCol;
                    allGeo = nextAll;
                }

                // Use collision-flagged (invisible) nodes; fall back to all geometry if none.
                TriangleCollection result = collision.Indices.Length > 0 ? collision : allGeo;
                meshCache[meshPath] = result;
                return result;
            }
        }

        private static bool IsCollidableFixture(ZoneFixtureRecord fixture, Dictionary<int, ZoneNifRecord> zoneNifs)
        {
            if (fixture.Collide != 0)
                return true;

            ZoneNifRecord zoneNif;
            return zoneNifs.TryGetValue(fixture.NifId, out zoneNif) && zoneNif.Collide != 0;
        }

        private static Quaternion BuildFixtureRotation(ZoneFixtureRecord fixture)
        {
            Vector3 axis = new Vector3(fixture.AxisX, fixture.AxisY, fixture.AxisZ);
            if (axis.LengthSquared() > 0.000001f && Math.Abs(fixture.AxisAngleRadians) > 0.000001f)
                return Quaternion.CreateFromAxisAngle(Vector3.Normalize(axis), fixture.AxisAngleRadians);

            if (fixture.A == 0)
                return Quaternion.Identity;

            float radians = (float)(-fixture.A * Math.PI / 180.0);
            return Quaternion.CreateFromAxisAngle(Vector3.UnitZ, radians);
        }
    }
}
