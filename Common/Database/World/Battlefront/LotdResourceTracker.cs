using System;
using FrameWork;

namespace Common.Database.World.Battlefront
{
    [DataTable(PreCache = false, TableName = "lotd_resource_tracker", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class LotdResourceTracker : DataObject
    {
        [PrimaryKey]
        public byte TrackerId { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte State { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte OwningRealm { get; set; }

        [DataElement(AllowDbNull = false)]
        public int OrderResourcePoints { get; set; }

        [DataElement(AllowDbNull = false)]
        public int DestructionResourcePoints { get; set; }

        [DataElement(AllowDbNull = false)]
        public int Threshold { get; set; }

        [DataElement(AllowDbNull = false)]
        public int PointsPerBattlefrontLock { get; set; }

        [DataElement(AllowDbNull = false)]
        public int UnlockDurationMinutes { get; set; }

        [DataElement]
        public DateTime? UnlockEndsOnUtc { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte LastScoringRealm { get; set; }

        [DataElement(AllowDbNull = false)]
        public DateTime LastUpdatedOnUtc { get; set; }
    }
}
