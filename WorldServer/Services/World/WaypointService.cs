using Common;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldServer.Services.World
{
    [Service]
    public class WaypointService : ServiceBase
    {
        private static readonly object _idLock = new object();
        private static uint _nextWaypointId;
        public static Lookup<uint, Waypoint> LookupWaypoints;
        public static List<Waypoint> TableWaypoints;

       [LoadingFunction(true)]
        public static void LoadNpcWaypoints()
        {
            Log.Debug("WorldMgr", "Loading Npc Waypoints...");

            TableWaypoints = Database.SelectAllObjects<Waypoint>().ToList();
            LookupWaypoints = (Lookup<uint, Waypoint>)TableWaypoints.ToLookup(W => W.GUID, W => W);

            lock (_idLock)
            {
                _nextWaypointId = TableWaypoints.Count > 0 ? TableWaypoints.Max(w => w.GUID) + 1 : 1;
            }

            if (TableWaypoints != null)
                Log.Success("LoadNpcWaypoints", "Loaded " + TableWaypoints.Count + " Waypoints");
        }

        public static uint GetNextWaypointId()
        {
            lock (_idLock)
            {
                return _nextWaypointId++;
            }
        }

        public static List<Waypoint> GetNextWayPoint(uint first, List<Waypoint> matchingList)
        {
            var match = TableWaypoints.SingleOrDefault(x => x.GUID == first);
            if (match != null)
            {
                matchingList.Add(match);
                if (match.NextWaypointGUID != 0)
                    GetNextWayPoint(match.NextWaypointGUID, matchingList);
            }
            return matchingList;

        }

        public static List<Waypoint> GetNpcWaypoints(uint initialWayPoint)
        {
            //var match = TableWaypoints.SingleOrDefault(x => x.GUID == initialWayPoint);
            //if (match != null)
            //{
            //    var result = GetNextWayPoint(match.GUID, new List<Waypoint>());
            //    return result;
            //}
            //return null;

            return TableWaypoints.Where(x => x.CreatureSpawnGUID == initialWayPoint).ToList();


            //IEnumerable<Waypoint> NpcWaypoints = LookupWaypoints[WayPointUID];
            //return NpcWaypoints.ToList();
        }

        public static List<Waypoint> GetKeepNpcWaypoints(int infoWaypointGuid)
        {
            return TableWaypoints.Where(x => x.GameObjectSpawnGUID == infoWaypointGuid).ToList();
        }

        public static void DatabaseAddWaypoint(Waypoint AddWp)
        {
            Database.AddObject(AddWp);
            Database.ForceSave();
            lock (_idLock)
            {
                TableWaypoints.Add(AddWp);
            }
        }

        public static void DatabaseSaveWaypoint(Waypoint SaveWp)
        {
            Database.SaveObject(SaveWp);
        }

        public static void DatabaseDeleteWaypoint(Waypoint DeleteWp)
        {
            Database.DeleteObject(DeleteWp);
            lock (_idLock)
            {
                TableWaypoints.Remove(DeleteWp);
            }
        }

		/// <summary>
		/// calculates waypoint offset in range of from to to
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static int ShuffleWaypointOffset(int from, int to)
		{
			Random rnd = new Random();
			bool sign = rnd.NextDouble() > 0.5;
			int offset = Convert.ToInt32(from + rnd.NextDouble() * 100);
			if (offset > to) offset = to;
			return sign ? offset : -offset;
		}
	}
}
