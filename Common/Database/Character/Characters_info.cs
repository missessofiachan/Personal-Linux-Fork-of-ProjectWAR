using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    // Fixed value of a character
    [DataTable(PreCache = false, TableName = "characters_value", DatabaseName = "Characters")]
    [Serializable]
    public class Character_value : DataObject
    {
        private uint _characterId;
        private byte _level;
        private uint _xp;
        private int _xpMode;
        private uint _restXp;
        private uint _renown;
        private byte _renownRank;
        private uint _money;
        private int _speed;
        private uint _playedTime;
        private int _lastSeen;
        private int _regionId;
        private ushort _zoneId;
        private int _worldX;
        private int _worldY;
        private int _worldZ;
        private int _worldO;
        private ushort _rallyPoint;
        private uint _skills;
        private byte _bagBuy;
        private byte _bankBuy;
        private bool _online;
        private byte _gearShow;
        private ushort _titleId;

        private string _masterySkills;
        private string _renownSkills;

        private byte _gatheringSkill;
        private byte _gatheringSkillLevel;
        private byte _craftingSkill;
        private byte _craftingSkillLevel;

        private byte _craftingBags;

        private uint _rvrkills;
        private uint _rvrdeaths;

        private string _lockouts = string.Empty;
        private int _disconecttime;

        [PrimaryKey]
        public uint CharacterId
        {
            get { return _characterId; }
            set { _characterId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Level
        {
            get { return _level; }
            set { _level = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint Xp
        {
            get { return _xp; }
            set { _xp = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int XpMode
        {
            get { return _xpMode; }
            set { _xpMode = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint RestXp
        {
            get { return _restXp; }
            set { _restXp = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint Renown
        {
            get { return _renown; }
            set { _renown = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte RenownRank
        {
            get { return _renownRank; }
            set { _renownRank = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint Money
        {
            get { return _money; }
            set { _money = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int Speed
        {
            get { return _speed; }
            set { _speed = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint PlayedTime
        {
            get { return _playedTime; }
            set { _playedTime = value; Dirty = true; }
        }

        [DataElement]
        public int LastSeen
        {
            get { return _lastSeen; }
            set { _lastSeen = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int RegionId
        {
            get { return _regionId; }
            set { _regionId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort ZoneId
        {
            get { return _zoneId; }
            set { _zoneId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldX
        {
            get { return _worldX; }
            set { _worldX = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldY
        {
            get { return _worldY; }
            set { _worldY = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldZ
        {
            get { return _worldZ; }
            set { _worldZ = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldO
        {
            get { return _worldO; }
            set { _worldO = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort RallyPoint
        {
            get { return _rallyPoint; }
            set { _rallyPoint = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte BagBuy
        {
            get { return _bagBuy; }
            set { _bagBuy = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte BankBuy
        {
            get { return _bankBuy; }
            set { _bankBuy = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint Skills
        {
            get { return _skills; }
            set { _skills = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public bool Online
        {
            get { return _online; }
            set { _online = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte GearShow
        {
            get { return _gearShow; }
            set { _gearShow = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort TitleId
        {
            get { return _titleId; }
            set { _titleId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string RenownSkills
        {
            get { return _renownSkills; }
            set { _renownSkills = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string MasterySkills
        {
            get { return _masterySkills; }
            set { _masterySkills = value; Dirty = true; }
        }

        [DataElement] public ushort Morale1 { get; set; }
        [DataElement] public ushort Morale2 { get; set; }
        [DataElement] public ushort Morale3 { get; set; }
        [DataElement] public ushort Morale4 { get; set; }

        public ushort GetMorale(byte index)
        {
            switch (index)
            {
                case 1:
                    return Morale1;

                case 2:
                    return Morale2;

                case 3:
                    return Morale3;

                case 4:
                    return Morale4;
            }

            return 0;
        }

        public void SetMorale(byte index, ushort value)
        {
            switch (index)
            {
                case 1:
                    Morale1 = value;
                    Dirty = true;
                    break;

                case 2:
                    Morale2 = value;
                    Dirty = true;
                    break;

                case 3:
                    Morale3 = value;
                    Dirty = true;
                    break;

                case 4:
                    Morale4 = value;
                    Dirty = true;
                    break;
            }
        }

        [DataElement]
        public ushort Tactic1 { get; set; }

        [DataElement]
        public ushort Tactic2 { get; set; }

        [DataElement]
        public ushort Tactic3 { get; set; }

        [DataElement]
        public ushort Tactic4 { get; set; }

        [DataElement]
        public ushort Tactic5 { get; set; }

        [DataElement]
        public ushort Tactic6 { get; set; }

        [DataElement]
        public ushort Tactic7 { get; set; }

        [DataElement]
        public ushort Tactic8 { get; set; }

        public List<ushort> GetTactics()
        {
            List<ushort> tacList = new List<ushort> { Tactic1 };

            if (Tactic2 != 0)
                tacList.Add(Tactic2);
            if (Tactic3 != 0)
                tacList.Add(Tactic3);
            if (Tactic4 != 0)
                tacList.Add(Tactic4);
            if (Tactic5 != 0)
                tacList.Add(Tactic5);
            if (Tactic6 != 0)
                tacList.Add(Tactic6);
            if (Tactic7 != 0)
                tacList.Add(Tactic7);
            if (Tactic8 != 0)
                tacList.Add(Tactic8);

            return tacList;
        }

        public void SetTactic(byte index, ushort value)
        {
            switch (index)
            {
                case 1:
                    Tactic1 = value;
                    Dirty = true;
                    break;

                case 2:
                    Tactic2 = value;
                    Dirty = true;
                    break;

                case 3:
                    Tactic3 = value;
                    Dirty = true;
                    break;

                case 4:
                    Tactic4 = value;
                    Dirty = true;
                    break;

                case 5:
                    Tactic5 = value;
                    Dirty = true;
                    break;

                case 6:
                    Tactic6 = value;
                    Dirty = true;
                    break;

                case 7:
                    Tactic7 = value;
                    Dirty = true;
                    break;

                case 8:
                    Tactic8 = value;
                    Dirty = true;
                    break;
            }
        }

        [DataElement(AllowDbNull = false)]
        public byte GatheringSkill
        {
            get { return _gatheringSkill; }
            set { _gatheringSkill = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte GatheringSkillLevel
        {
            get { return _gatheringSkillLevel; }
            set { _gatheringSkillLevel = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte CraftingSkill
        {
            get { return _craftingSkill; }
            set { _craftingSkill = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte CraftingSkillLevel
        {
            get { return _craftingSkillLevel; }
            set { _craftingSkillLevel = value; Dirty = true; }
        }

        private bool _experimentalMode;

        [DataElement(AllowDbNull = false)]
        public bool ExperimentalMode
        {
            get { return _experimentalMode; }
            set { _experimentalMode = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint RVRKills
        {
            get { return _rvrkills; }
            set { _rvrkills = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint RVRDeaths
        {
            get { return _rvrdeaths; }
            set { _rvrdeaths = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte CraftingBags
        {
            get { return _craftingBags; }
            set { _craftingBags = value; Dirty = true; }
        }

        private uint _pendingXp;

        [DataElement()]
        public uint PendingXp
        {
            get { return _pendingXp; }
            set { _pendingXp = value; Dirty = true; }
        }

        private uint _pendingRenown;

        [DataElement()]
        public uint PendingRenown
        {
            get { return _pendingRenown; }
            set { _pendingRenown = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string Lockouts
        {
            get { return _lockouts; }
            set { _lockouts = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int DisconcetTime
        {
            get { return _disconecttime; }
            set { _disconecttime = value; Dirty = true; }
        }

        public List<string> GetAllLockouts()
        {
            List<string> lockouts = new List<string>();

            if (_lockouts == null || _lockouts.Length == 0)
                return lockouts;

            for (int i = 1; i < _lockouts.Split('~').Length; i++)
            {
                lockouts.Add("~" + _lockouts.Split('~')[i]);
            }

            return lockouts;
        }

        public string GetLockout(ushort zone)
        {
            if (_lockouts == null)
                return null;
            for (int i = 0; i < _lockouts.Split('~').Length; i++)
                if (_lockouts.Split('~')[i].Split(':')[0] == "" + zone)
                {
                    string ret = _lockouts.Split('~')[i];
                    if (!ret.StartsWith("~"))
                        ret = "~" + ret;
                    return ret;
                }

            return null;
        }

        public void ClearLockouts(int lockoutTimer)
        {
            string newLockouts = string.Empty;
            List<string> all = GetAllLockouts();

            for (int i = 0; i < all.Count; i++)
            {//removed + lockoutTimer * 60 on the left side of the >=
                if (int.Parse(all[i].Split(':')[1]) >= TCPManager.GetTimeStamp())
                    newLockouts += all[i];
            }

            Lockouts = newLockouts;
            Dirty = true;
        }

        public void RemoveLockout(string Lockout)
        {
            Lockouts = Lockouts.Replace(Lockout, string.Empty);
            Dirty = true;
        }

        public void AddLockout(Instance_Lockouts Lockout)
        {
            if (Lockout == null || string.IsNullOrWhiteSpace(Lockout.InstanceID))
                return;

            string lockoutId = Lockout.InstanceID.TrimStart('~');
            string[] lockoutParts = lockoutId.Split(':');
            if (lockoutParts.Length < 2)
                return;

            if (!ushort.TryParse(lockoutParts[0], out ushort incomingZoneId))
                return;

            if (!int.TryParse(lockoutParts[1], out int incomingTimestamp))
                return;

            HashSet<uint> incomingBossIds = new HashSet<uint>();
            if (!string.IsNullOrWhiteSpace(Lockout.Bosseskilled))
            {
                foreach (string token in Lockout.Bosseskilled.Split(':'))
                {
                    if (uint.TryParse(token, out uint bossId) && bossId > 0)
                        incomingBossIds.Add(bossId);
                }
            }

            List<string> mergedLockouts = new List<string>();
            bool mergedIncomingZone = false;
            int mergedZoneTimestamp = incomingTimestamp;
            HashSet<uint> mergedZoneBossIds = new HashSet<uint>(incomingBossIds);

            foreach (string raw in (_lockouts ?? string.Empty).Split('~'))
            {
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                string normalized = raw.StartsWith("~") ? raw : "~" + raw;
                string[] currentParts = normalized.TrimStart('~').Split(':');
                if (currentParts.Length < 2 || !ushort.TryParse(currentParts[0], out ushort currentZoneId))
                {
                    mergedLockouts.Add(normalized);
                    continue;
                }

                if (currentZoneId != incomingZoneId)
                {
                    mergedLockouts.Add(normalized);
                    continue;
                }

                mergedIncomingZone = true;

                int currentTimestamp = 0;
                int.TryParse(currentParts[1], out currentTimestamp);
                mergedZoneTimestamp = Math.Max(currentTimestamp, mergedZoneTimestamp);
                for (int i = 2; i < currentParts.Length; i++)
                {
                    if (uint.TryParse(currentParts[i], out uint bossId) && bossId > 0)
                        mergedZoneBossIds.Add(bossId);
                }
            }

            string incomingZoneEntry = "~" + incomingZoneId + ":" + (mergedIncomingZone ? mergedZoneTimestamp : incomingTimestamp);
            if (mergedZoneBossIds.Count > 0)
                incomingZoneEntry += ":" + string.Join(":", mergedZoneBossIds.OrderBy(x => x));
            mergedLockouts.Add(incomingZoneEntry);

            Lockouts = string.Join(string.Empty, mergedLockouts);
            Dirty = true;
        }
    }
}
