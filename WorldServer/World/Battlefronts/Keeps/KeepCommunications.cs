using FrameWork;
using GameData;
using NLog;
using System.Linq;
using WorldServer.World.Battlefronts.Apocalypse;
using WorldServer.World.Objects;
using Opcodes = WorldServer.NetWork.Opcodes;

namespace WorldServer.World.Battlefronts.Keeps
{
    public class KeepCommunications : IKeepCommunications
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void SendKeepStatus(Player plr, BattleFrontKeep keep)
        {
            if (keep.Region == null)
                return;

            var aliveMainDoors = keep.Doors
                .Where(x =>
                    x?.Info != null
                    && x.Info.GameObjectId == 100
                    && (x.Info.Number == (int)KeepDoorType.OuterMain || x.Info.Number == (int)KeepDoorType.InnerMain)
                    && x.GameObject != null
                    && !x.GameObject.IsDead
                    && x.GameObject.PctHealth > 0)
                .ToList();

            byte trackedObjectiveCount = (byte)aliveMainDoors.Count;
            byte trackedObjectiveHealth = 0;

            if (keep.KeepStatus == KeepStatus.KEEPSTATUS_KEEP_LORD_UNDER_ATTACK)
            {
                var keepLord = keep.KeepLord?.Creature;
                if (keepLord != null && !keepLord.IsDead)
                {
                    trackedObjectiveCount = 1;
                    trackedObjectiveHealth = keepLord.PctHealth;
                }
                else
                {
                    trackedObjectiveCount = 0;
                    trackedObjectiveHealth = 0;
                }
            }
            else if (trackedObjectiveCount > 0)
            {
                // During door phases, prefer the inner door once it is the current objective.
                var trackedDoor = aliveMainDoors.SingleOrDefault(x => x.Info.Number == (int)KeepDoorType.InnerMain)
                                 ?? aliveMainDoors.SingleOrDefault(x => x.Info.Number == (int)KeepDoorType.OuterMain)
                                 ?? aliveMainDoors.FirstOrDefault();

                if (trackedDoor?.GameObject != null)
                    trackedObjectiveHealth = trackedDoor.GameObject.PctHealth;
            }

            var Out = new PacketOut((byte)Opcodes.F_KEEP_STATUS, 26);
            Out.WriteByte(keep.Info.KeepId);
            {
                Out.WriteByte(keep.KeepStatus == KeepStatus.KEEPSTATUS_LOCKED ? (byte)1 : (byte)keep.KeepStatus);
                Out.WriteByte(0); // ?
                Out.WriteByte((byte)keep.Realm);
                Out.WriteByte(trackedObjectiveCount);
                Out.WriteByte(keep.Rank); // Rank
                Out.WriteByte(trackedObjectiveHealth); // Door/Lord health
                Out.WriteByte(0); // Next rank %
            }

            Out.Fill(0, 18);

            if (plr != null)
                plr.SendPacket(Out);
            else
                lock (Player._Players)
                {
                    foreach (var player in Player._Players)
                        player.SendCopy(Out);
                }

            _logger.Trace($"F_KEEP_STATUS {keep.Info.Name} Status : {keep.KeepStatus} ");
        }

    }


}
