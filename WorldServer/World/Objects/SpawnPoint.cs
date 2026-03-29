using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using WorldServer.Managers;
using WorldServer.Services.World;
using WorldServer.World.Positions;

namespace WorldServer.World.Objects
{
    public class SpawnPoint : Point3D
    {
        private const int ScenarioRespawnTerrainTolerance = 128;
        private const int ScenarioRespawnLift = 32;
        public ushort ZoneId { get; set; }

        public SpawnPoint(ushort zoneId, int x, int y, int z)
        {
            ZoneId = zoneId;
            X = x;
            Y = y;
            Z = z;
        }

        public SpawnPoint(Zone_Respawn respawn)
        {
            ushort targetZoneId = (ushort)(respawn.InZoneID != 0 ? respawn.InZoneID : respawn.ZoneID);
            Zone_Info zone = ZoneService.GetZone_Info(targetZoneId);
            ushort pinZ = ResolveRespawnPinZ(zone, targetZoneId, respawn.PinX, respawn.PinY, respawn.PinZ);
            Point3D world = ZoneService.GetWorldPosition(zone, respawn.PinX, respawn.PinY, pinZ);
            ZoneId = targetZoneId;

            X = world.X;
            Y = world.Y;
            Z = world.Z;
        }

        public  override string ToString()
        {
            return $"RESPAWN : ZoneId={ZoneId},X={X},Y={Y},Z={Z}";
        }

        public Point3D As3DPoint()
        {
            return new Point3D(X,Y,Z);
        }

        private static ushort ResolveRespawnPinZ(Zone_Info zone, ushort zoneId, ushort pinX, ushort pinY, ushort pinZ)
        {
            if (zone == null || !ScenarioService.IsScenarioZone(zoneId))
                return pinZ;

            int resolvedPinZ = pinZ;
            int terrainZ = ClientFileMgr.GetHeight(zone.ZoneId, pinX, pinY);
            if (terrainZ >= 0)
            {
                int delta = resolvedPinZ - terrainZ;
                if (resolvedPinZ < terrainZ || Math.Abs(delta) <= ScenarioRespawnTerrainTolerance)
                    resolvedPinZ = terrainZ;
            }

            resolvedPinZ = Math.Min(UInt16.MaxValue, resolvedPinZ + ScenarioRespawnLift);
            return (ushort)resolvedPinZ;
        }
    }
}
