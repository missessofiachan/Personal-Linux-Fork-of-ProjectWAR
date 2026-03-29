using System;
using System.Collections.Generic;
using WorldServer.Services.World;
using WorldServer.World.Positions;

namespace WorldServer.Managers
{
    /// <summary>
    /// Builds simple waypoint-guided paths for bot navigation by sampling the NPC
    /// patrol waypoint data already loaded in memory by <see cref="WaypointService"/>.
    ///
    /// Algorithm:
    ///   1. Project every loaded waypoint onto the straight-line vector from
    ///      <c>start</c> to <c>destination</c>.
    ///   2. Keep only waypoints whose scalar projection falls between
    ///      <c>MIN_WAYPOINT_SPACING</c> and <c>pathLength - MIN_WAYPOINT_SPACING</c>
    ///      (i.e. neither behind the start nor past the destination).
    ///   3. Discard waypoints further than <c>MAX_LATERAL_DEVIATION</c> units off
    ///      the direct line.
    ///   4. Sort survivors by progress along the path and thin them so no two
    ///      consecutive waypoints are closer than <c>MIN_WAYPOINT_SPACING</c>.
    ///   5. Append the raw destination as the final step.
    ///
    /// Callers (BotBrain) walk the returned list one point at a time, advancing to
    /// the next point when within <see cref="ARRIVAL_THRESHOLD_RAW"/> world units.
    ///
    /// <b>Coordinate system note:</b> all X/Y/Z values are in world coordinates
    /// (the same space used by <c>Unit.WorldPosition</c> and
    /// <c>MovementInterface.Move()</c>).  Waypoint rows in the database store their
    /// position in this same space, matching how NPC movement interpolation works.
    /// </summary>
    public static class BotPathfinder
    {
        /// <summary>
        /// Maximum perpendicular distance (world units) from the direct start→end line
        /// for a waypoint to be included as an intermediate step.
        /// ≈ 167 feet at 12 raw units per foot.
        /// </summary>
        private const float MAX_LATERAL_DEVIATION = 2000f;

        /// <summary>
        /// Minimum spacing (world units) between consecutive path waypoints.
        /// Avoids packing too many near-duplicate points from dense patrol routes.
        /// ≈ 17 feet.
        /// </summary>
        private const float MIN_WAYPOINT_SPACING = 200f;

        /// <summary>
        /// Distance (world units) within which a bot considers itself to have arrived
        /// at an intermediate waypoint and should advance to the next one.
        /// ≈ 25 feet at 12 raw units per foot.
        /// </summary>
        public const int ARRIVAL_THRESHOLD_RAW = 300;

        /// <summary>
        /// Builds an ordered list of world-space positions leading from
        /// <paramref name="start"/> to <paramref name="destination"/> using nearby
        /// NPC patrol waypoints as intermediate steps.
        /// The destination is always the last element.
        /// Returns a single-element list (just the destination) when the waypoint
        /// table is empty or no suitable intermediates exist.
        /// </summary>
        public static List<Point3D> BuildPath(Point3D start, Point3D destination)
        {
            var path = new List<Point3D>();

            float dx = destination.X - start.X;
            float dy = destination.Y - start.Y;
            float pathLen = (float)Math.Sqrt(dx * dx + dy * dy);

            if (pathLen < 1f || WaypointService.TableWaypoints == null
                             || WaypointService.TableWaypoints.Count == 0)
            {
                path.Add(destination);
                return path;
            }

            // Unit direction vector along the direct path.
            float ux = dx / pathLen;
            float uy = dy / pathLen;

            // Collect candidates: (scalar progress along path, position)
            var candidates = new List<(float progress, Point3D pos)>();

            foreach (var wp in WaypointService.TableWaypoints)
            {
                float wx = (float)wp.X - start.X;
                float wy = (float)wp.Y - start.Y;

                float proj = wx * ux + wy * uy;

                // Reject waypoints before the start corridor or after destination.
                if (proj < MIN_WAYPOINT_SPACING || proj > pathLen - MIN_WAYPOINT_SPACING)
                    continue;

                // Reject waypoints too far off the direct line.
                float lateral = Math.Abs(wx * uy - wy * ux);
                if (lateral > MAX_LATERAL_DEVIATION)
                    continue;

                candidates.Add((proj, new Point3D((int)wp.X, (int)wp.Y, (int)wp.Z)));
            }

            // Sort by progress so bots walk in the correct direction.
            candidates.Sort((a, b) => a.progress.CompareTo(b.progress));

            // Thin: skip waypoints that are too close to the last accepted one.
            float minSpacingSq = MIN_WAYPOINT_SPACING * MIN_WAYPOINT_SPACING;
            Point3D last = start;
            foreach (var (_, pos) in candidates)
            {
                float sepX = pos.X - last.X;
                float sepY = pos.Y - last.Y;
                if (sepX * sepX + sepY * sepY < minSpacingSq)
                    continue;

                path.Add(pos);
                last = pos;
            }

            // Always end at the exact destination.
            path.Add(destination);
            return path;
        }
    }
}
