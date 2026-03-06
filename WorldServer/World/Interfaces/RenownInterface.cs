using System;
using System.Collections.Generic;
using System.Linq;
using SystemData;
using Common;
using FrameWork;
using GameData;
using WorldServer.Managers;
using WorldServer.World.Abilities;
using WorldServer.World.Abilities.Buffs;
using WorldServer.World.Abilities.Components;
using WorldServer.World.Objects;
using Opcodes = WorldServer.NetWork.Opcodes;

namespace WorldServer.World.Interfaces
{
    public class RenownInterface : BaseInterface
    {
        private const int MAX_SPENDABLE_RENOWN_POINTS = 100;

        private ushort[][] _trained = new ushort[7][];
        public int PointsSpent { get; private set; }

        private readonly List<ushort> _passiveIds = new List<ushort>();
        private readonly List<ushort> _activeIds = new List<ushort>();

        private readonly List<ushort> _activeAbilities = new List<ushort>();
        private readonly List<ushort> _tacticBuffIds = new List<ushort>();
        private readonly List<Tuple<string, byte, int>> _savedCommandInfo = new List<Tuple<string, byte, int>>(); 

        private Player _player;

        public bool HasRenownAbility(ushort spellId)
        {
            return _passiveIds.Contains(spellId) || _activeIds.Contains(spellId) || _activeAbilities.Contains(spellId) || _tacticBuffIds.Contains(spellId);
        }

        public RenownInterface()
        {
            for (int i = 0; i < 7; ++i)
                _trained[i] = new ushort[20];
        }

        public override bool Load()
        {
            if (Loaded)
                return true;

            if (_Owner == null)
            {
                Log.Error("RenownInterface", "No Owner.");
                return base.Load();
            }

            _player = (Player)_Owner;

            if (_player._Value == null)
            {
                Log.Error("RenownInterface", "No character values.");
                return base.Load();
            }

            LoadSkills();

            SendRenownAbilityInfo();

            return base.Load();
        }

        public void LoadSkills()
        {
            ResetTrainingState();

            if (string.IsNullOrWhiteSpace(_player._Value.RenownSkills))
                return;

            string command = "";
            byte stat = 0;
            int value = 0;

            string[] temp = _player._Value.RenownSkills.Split(';');
            foreach (string str in temp)
            {
                if (string.IsNullOrWhiteSpace(str))
                    continue;

                string[] parts = str.Split(':');
                if (parts.Length != 2)
                    continue;

                if (!int.TryParse(parts[0], out int tree) || !int.TryParse(parts[1], out int pos))
                    continue;

                if (tree < 0 || tree >= 7 || pos < 0 || pos >= 20)
                    continue;

                byte dbTree = (byte)(tree + 9);
                if (!CharMgr.RenownAbilityInfo.TryGetValue(dbTree, out List<CharacterInfoRenown> treeEntries))
                    continue;
                if (pos >= treeEntries.Count)
                    continue;

                CharacterInfoRenown ren = treeEntries[pos];

                _trained[tree][pos] = ren.SpellId;
                PointsSpent += ren.Renown_Costs;

                if (ren.Slotreq == 0 || !ren.Passive)
                {
                    if (value != 0 || (!string.IsNullOrEmpty(command) && !ren.Passive))
                    {
                        ExecuteCommand(command, stat, value);
                        _savedCommandInfo.Add(new Tuple<string, byte, int>(command, stat, value));
                    }

                    command = ren.CommandName;
                    stat = ren.Stat;
                    value = ren.Value;
                }
                else
                    value += ren.Value;

                if (ren.Passive)
                    _passiveIds.Add(ren.SpellId);
                else
                    _activeIds.Add(ren.SpellId);
            }

            if (!string.IsNullOrEmpty(command))
            {
                ExecuteCommand(command, stat, value);
                _savedCommandInfo.Add(new Tuple<string, byte, int>(command, stat, value));
            }

            if (PointsSpent > GetMaxPoints())
            {
                _player.SendClientMessage("Your renown point spend exceeds the number of points available to you. Your renown specialization has been reset.", ChatLogFilters.CHATLOGFILTERS_CSR_TELL_RECEIVE);
                Respec();
                return;
            }

            _player.StsInterface.ApplyStats();
            _player.StsInterface.SendRenownStats();
        }

        private void ExecuteCommand(string cmd, byte stat, int value)
        {
            switch (cmd)
            {
                case "ModifyStat":
                    _player.StsInterface.SetRenownStat((Stats)stat, (ushort) value);
                    break;
                case "ModifyEvasion":
                    _player.StsInterface.SetRenownStat(Stats.Evade, (ushort)value);
                    _player.StsInterface.SetRenownStat(Stats.Disrupt, (ushort)value);
                    break;
                case "IncreaseAPPool":
                    _player.MaxActionPoints = (ushort)(_player.MaxActionPoints + value);
                    break;
                case "AddAbility":
                    _activeAbilities.Add((ushort)value);
                    _player.AbtInterface.GrantAbility((ushort)value);
                    break;
                case "AddBuff":
                    BuffInfo buffInfo = AbilityMgr.GetBuffInfo((ushort) value);
                    if (buffInfo == null)
                    {
                        _player.SendClientMessage("The requested renown buff is not implemented (id: " + value + ").");
                    }
                    else
                    {
                        _tacticBuffIds.Add((ushort)value);
                        _player.BuffInterface.QueueBuff(new BuffQueueInfo(_player, _player.EffectiveLevel, buffInfo));
                    }
                    break;
            }
        }

        private void ReverseCommand(Tuple<string, byte, int> cmdInfo)
        {
            switch (cmdInfo.Item1)
            {
                case "ModifyStat":
                    
                    _player.StsInterface.SetRenownStat((Stats)cmdInfo.Item2, 0);
                    break;
                case "ModifyEvasion":
                  
                    _player.StsInterface.SetRenownStat(Stats.Evade, 0);
                    _player.StsInterface.SetRenownStat(Stats.Disrupt, 0);
                    break;
                case "IncreaseAPPool":
                 
                    _player.MaxActionPoints = (ushort)(_player.MaxActionPoints - cmdInfo.Item3);
                    break;
                case "AddAbility":
                    _activeAbilities.Remove((ushort)cmdInfo.Item3);
                    _player.AbtInterface.RemoveGrantedAbility((ushort)cmdInfo.Item3);
                    break;
                case "AddBuff":
                    _tacticBuffIds.Remove((ushort)cmdInfo.Item3);
                    _player.BuffInterface.RemoveBuffByEntry((ushort)cmdInfo.Item3);
                    break;
                default:
                    _player.SendClientMessage("Assigned nonfunctional renown skill", ChatLogFilters.CHATLOGFILTERS_C_ABILITY_ERROR);
                    _player.SendClientMessage("This skill doesn't work yet.", ChatLogFilters.CHATLOGFILTERS_USER_ERROR);
                    break;
            }
        }

        private void Clear()
        {
            if (_activeAbilities.Count > 0)
            {
                // Remove actives from skillbar
                PacketOut Out = new PacketOut((byte)Opcodes.F_CHARACTER_INFO, 32);

                Out.WriteByte(0x0B); 
                Out.WriteByte((byte)_activeAbilities.Count);

                foreach (ushort abilityID in _activeAbilities.ToList())
                    Out.WriteUInt16(abilityID);

                _player.SendPacket(Out);
            }

            ReverseAllCommands();
            _activeAbilities.Clear();
        }

        private void ReverseAllCommands()
        {
            // Reverse all renown modifications
            foreach (var cmdInfo in _savedCommandInfo)
                ReverseCommand(cmdInfo);

            // Fix stats
            _player.StsInterface.ApplyStats();
            _player.StsInterface.SendRenownStats();
            _savedCommandInfo.Clear();
        }

        public override void Save()
        {
            if (_player?._Value == null)
                return;

            string renownString = "";

            for(int i=0;i < 7; i++)
                for (int y = 0; y < 20; y++)
                {
                    if (_trained[i][y] > 0)
                        renownString += ""+i+":"+y+";"; 
                }
            _player._Value.RenownSkills = renownString;
            _player._Value.Dirty = true;
            CharMgr.Database.SaveObject(_player._Value);
        }

        private void ResetTrainingState()
        {
            PointsSpent = 0;

            for (int i = 0; i < _trained.Length; ++i)
            {
                if (_trained[i] == null || _trained[i].Length != 20)
                    _trained[i] = new ushort[20];
                else
                    Array.Clear(_trained[i], 0, _trained[i].Length);
            }

            _passiveIds.Clear();
            _activeIds.Clear();
            _activeAbilities.Clear();
            _tacticBuffIds.Clear();
            _savedCommandInfo.Clear();
        }

        private static byte ClampToByte(int value)
        {
            if (value < 0)
                return 0;
            if (value > byte.MaxValue)
                return byte.MaxValue;
            return (byte)value;
        }

        private int GetAvailablePointsInt()
        {
            int pointsAvailable = GetMaxPoints() - PointsSpent;
            return pointsAvailable < 0 ? 0 : pointsAvailable;
        }

        public byte GetAvailablePoints()
        {
            return ClampToByte(GetAvailablePointsInt());
        }

        public void PurchaseRenownAbility(byte tree,byte skillPos)
        {
            if (tree < 9 || tree > 15 || skillPos == 0 || !CharMgr.RenownAbilityInfo.TryGetValue(tree, out List<CharacterInfoRenown> treeEntries) || treeEntries.Count < skillPos)
            {
                _player.SendClientMessage("This ability is not implemented.");
                return;
            }

            int treeIndex = tree - 9;
            int skillIndex = skillPos - 1;
            CharacterInfoRenown selected = treeEntries[skillIndex];

            // Duplicate buys are invalid and can corrupt point accounting.
            if (_trained[treeIndex][skillIndex] != 0)
                return;

            int pointsAvailable = GetAvailablePointsInt();
            if (pointsAvailable < selected.Renown_Costs)
                return;

            if (selected.Slotreq > 0)
            {
                int requiredIndex = selected.Slotreq - 1;
                if (requiredIndex < 0 || requiredIndex >= treeEntries.Count)
                    return;
                ushort requiredSpellId = treeEntries[requiredIndex].SpellId;
                if (!_passiveIds.Contains(requiredSpellId) && !_activeIds.Contains(requiredSpellId))
                    return;
            }

            _trained[treeIndex][skillIndex] = selected.SpellId;


            uint respeccost = 58000;   // (uint)PointsSpend * 1000;

            //_player.buffInterface.QueueBuff(new BuffQueueInfo(_player, _player.EffectiveLevel, AbilityMgr.GetBuffInfo(CharMgr._Inforenown[tree][skillPos - 1].SpellId)));

            //(byte)(Trained[(tree - 9), Skillpos - 1] > 0 ? 1 : 0)

            // Monotonic state generation: DO NOT call the global Save() method here 
            // because it triggers race conditions with multiple threaded purchases.
            // Instead, we build the sorted explicit string locally and assign it directly.
            string renownString = "";
            for (int i = 0; i < 7; i++)
            {
                for (int y = 0; y < 20; y++)
                {
                    if (_trained[i][y] > 0)
                        renownString += "" + i + ":" + y + ";";
                }
            }
            
            _player._Value.RenownSkills = renownString;
            _player._Value.Dirty = true;
            CharMgr.Database.SaveObject(_player._Value);

            List<ushort> lastActives = new List<ushort>(_activeAbilities);

            ReverseAllCommands();
            LoadSkills();

            if (lastActives.Count > 0)
            {
                for (int i = lastActives.Count - 1; i >= 0; --i)
                {
                    if (_activeAbilities.Contains(lastActives[i]))
                        lastActives.RemoveAt(i);
                }

                if (lastActives.Count > 0)
                {
                    // Remove previous actives from skillbar
                    PacketOut skillClearPacket = new PacketOut((byte) Opcodes.F_CHARACTER_INFO, 32);

                    skillClearPacket.WriteByte(0x0B);
                    skillClearPacket.WriteByte((byte) lastActives.Count);

                    foreach (ushort abilityID in lastActives)
                        skillClearPacket.WriteUInt16(abilityID);

                    _player.SendPacket(skillClearPacket);
                }
            }

            SendRenownAbility(selected,1);

            byte pointsSpentByte = ClampToByte(PointsSpent);
            byte pointsAvailableByte = ClampToByte(GetAvailablePointsInt());

            PacketOut Out = new PacketOut((byte)Opcodes.F_CAREER_PACKAGE_UPDATE, 20);
            Out.WriteByte(9);
            Out.WriteByte(1);
            Out.WriteByte(0);
            Out.WriteByte(pointsSpentByte);
            Out.WriteByte(pointsAvailableByte);
            Out.Fill(0, 7);
            Out.WriteUInt32(respeccost);
            Out.WriteUInt32(0);

            _player.SendPacket(Out);


            // Check for debolster, and notify if required
            if (_player.AdjustedLevel < _player.Level)
                _player.CheckDebolsterValid();
        }

        public void Respec()
        {
            // RB   6/18/2016   Limiting spendable renown points to character level, instead of locking renown growth
            byte pointsAvailable = GetMaxPoints();

            Clear();
            ResetTrainingState();

            _player._Value.RenownSkills = "";
            _player._Value.Dirty = true;

            CharMgr.Database.SaveObject(_player._Value);

            SendRenownAbilityInfo();

            PacketOut Out = new PacketOut((byte)Opcodes.F_CAREER_PACKAGE_UPDATE, 20);
            Out.WriteByte(9);
            Out.WriteByte(1);
            Out.WriteByte(0);
            Out.WriteByte(0);
            Out.WriteByte(/*_player.Renown*/ pointsAvailable);
            Out.Fill(0, 7);
            Out.WriteUInt32(58000);
            Out.WriteUInt32(0);

            _player.SendPacket(Out);
        }

        public void SendRenownAbilityInfo()
        {
            /*
            byte pointsAvailable = _player.Renown;
            pointsAvailable -= PointsSpent;
            */

            // RB   6/18/2016   Limiting spendable renown points to character level, instead of locking renown growth
            int pointsAvailable = GetAvailablePointsInt();
            byte pointsSpentByte = ClampToByte(PointsSpent);
            byte pointsAvailableByte = ClampToByte(pointsAvailable);

            uint respecCost = (uint)PointsSpent * 1000;

            respecCost = 58000; 

            string renName = "Renown Stats A";

            PacketOut Out = new PacketOut((byte)Opcodes.F_CAREER_CATEGORY, 512);

            Out.WriteByte(0x09);
            Out.WriteByte(1);
            Out.WriteByte(0);
            Out.WriteByte(pointsSpentByte);
            Out.WriteByte(pointsAvailableByte);
            Out.Fill(0, 3);
            Out.WriteUInt32(respecCost);     //?
            Out.WriteByte(0);
            Out.WriteByte(1);
            Out.WriteByte(0x86);
            Out.WriteByte(0xA0);
            Out.WritePascalString(renName);
            Out.WriteByte(0);
            Out.WriteByte(0x14);   // ability count
            Out.WriteByte(0);
            for (int i = 1; i <= 20; i++)
            {
                Out.WriteByte((byte)i);
                Out.WriteByte(0);
            }
            Out.Fill(0, 2);
            _player.SendPacket(Out);

            for(int i=0;i < CharMgr.RenownAbilityInfo[9].Count; i++)
            {
                SendRenownAbility(CharMgr.RenownAbilityInfo[9][i],(byte)(_trained[0][i] > 0 ? 1:0));
            }

            Out = new PacketOut((byte)Opcodes.F_CAREER_CATEGORY, 48);
            Out.WriteUInt16(0x0A01);
            Out.WriteByte(0);
            Out.WriteByte(pointsSpentByte);
            Out.WriteByte(pointsAvailableByte);
            Out.Fill(0, 3);
            Out.WriteUInt32(respecCost);
            Out.WriteHexStringBytes("000182B80E52656E6F776E20537461747320420014000100020003000400050006000700080009000A000B000C000D000E000F00100011001200130014000000");
            _player.SendPacket(Out);


            for (int i = 0; i < CharMgr.RenownAbilityInfo[10].Count; i++)
            {
                SendRenownAbility(CharMgr.RenownAbilityInfo[10][i], (byte)(_trained[1][i] > 0 ? 1 : 0));
            }


            Out = new PacketOut((byte)Opcodes.F_CAREER_CATEGORY, 48);


            Out.WriteUInt16(0x0B01);
            Out.WriteByte(0);
            Out.WriteByte(pointsSpentByte);
            Out.WriteByte(pointsAvailableByte);
            Out.Fill(0, 3);
            Out.WriteUInt32(respecCost);
            Out.WriteHexStringBytes("000138801952656E6F776E204F6666656E7369766520437269746963616C0010000100020003000400050006000700080009000A000B000C000D000E000F0010000000");
            _player.SendPacket(Out);

            for (int i = 0; i < CharMgr.RenownAbilityInfo[11].Count; i++)
            {
                SendRenownAbility(CharMgr.RenownAbilityInfo[11][i], (byte)(_trained[2][i] > 0 ? 1 : 0));
            }


            Out = new PacketOut((byte)Opcodes.F_CAREER_CATEGORY, 48);

            Out.WriteUInt16(0x0C01);
            Out.WriteByte(0);
            Out.WriteByte(pointsSpentByte);
            Out.WriteByte(pointsAvailableByte);
            Out.Fill(0, 3);
            Out.WriteUInt32(respecCost);
            Out.WriteHexStringBytes("000075301952656E6F776E20446566656E7369766520437269746963616C000700010002000300040005000600070008000000");
            _player.SendPacket(Out);

            
            for (int i = 0; i < CharMgr.RenownAbilityInfo[12].Count; i++)
            {
                SendRenownAbility(CharMgr.RenownAbilityInfo[12][i], (byte)(_trained[3][i] > 0 ? 1 : 0));
            }
      
            Out = new PacketOut((byte)Opcodes.F_CAREER_CATEGORY, 48);
            Out.WriteUInt16(0x0D01);
            Out.WriteByte(0);
            Out.WriteByte(pointsSpentByte);
            Out.WriteByte(pointsAvailableByte);
            Out.Fill(0, 3);
            Out.WriteUInt32(respecCost);
            Out.WriteHexStringBytes("000138801752656E6F776E2047656E65726963205061737369766573000B000100020003000400050006000700080009000A000B000000");
            _player.SendPacket(Out);

            for (int i = 0; i < CharMgr.RenownAbilityInfo[13].Count; i++)
            {
                SendRenownAbility(CharMgr.RenownAbilityInfo[13][i], (byte)(_trained[4][i] > 0 ? 1 : 0));
            }

            Out = new PacketOut((byte)Opcodes.F_CAREER_CATEGORY, 48);
            Out.WriteUInt16(0x0E01);
            Out.WriteByte(0);
            Out.WriteByte(pointsSpentByte);
            Out.WriteByte(pointsAvailableByte);
            Out.Fill(0, 3);
            Out.WriteUInt32(respecCost);
            Out.WriteHexStringBytes("000138801952656E6F776E20446566656E736976652050617373697665730011000100020003000400050006000700080009000A000B000C000D000E000F00100011000000");
            _player.SendPacket(Out);

            for (int i = 0; i < CharMgr.RenownAbilityInfo[14].Count; i++)
            {
                SendRenownAbility(CharMgr.RenownAbilityInfo[14][i], (byte)(_trained[5][i] > 0 ? 1 : 0));
            }
         
            Out = new PacketOut((byte)Opcodes.F_CAREER_CATEGORY, 48);
            Out.WriteUInt16(0x0F01);
            Out.WriteByte(0);
            Out.WriteByte(pointsSpentByte);
            Out.WriteByte(pointsAvailableByte);
            Out.Fill(0, 3);
            Out.WriteUInt32(respecCost);
            Out.WriteHexStringBytes("000138801052656E6F776E20416374697661746573000A000100020003000400050006000700080009000A000000");
            _player.SendPacket(Out);

            for (int i = 0; i < CharMgr.RenownAbilityInfo[15].Count; i++)
            {
                SendRenownAbility(CharMgr.RenownAbilityInfo[15][i], (byte)(_trained[6][i] > 0 ? 1 : 0));
            }
        }

        public void SendRenownAbility(CharacterInfoRenown ren,byte trained)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_CAREER_PACKAGE_INFO, 64);
            Out.WriteByte(ren.Tree);
            Out.WriteByte(1);
            Out.WriteByte(0);
            Out.WriteByte(ren.Position);
            Out.Fill(0, 2);
            Out.WriteByte(1);
            Out.WriteByte(0);
            Out.WriteByte(trained);                               // skill trained
            Out.WriteByte(2);
            Out.Fill(0, 14);
            Out.WriteByte(ren.Renown_Costs);    // renown costs
            Out.WriteByte(1);
            Out.WriteByte(0);
            Out.WriteByte(0);
            Out.WriteUInt16(ren.SpellId);    // ability id
            if (!string.IsNullOrEmpty(ren.Unk))
                Out.WriteHexStringBytes(ren.Unk.Replace(" ",""));

            else
            {
                Out.WriteByte(3);
                Out.Fill(0, 3);
                Out.WriteUInt16(0x0204);                //???
            }
            Out.WritePascalString("");              // name
            if (ren.Slotreq == 0)
            {
                Out.Fill(0, 5);
            }
            else   // requirement
            {
                Out.WriteByte(1);
                Out.WriteByte(0);
                Out.WriteByte(ren.Slotreq);
                Out.Fill(0, 5);
            }
            _player.SendPacket(Out);
        }

        private byte GetMaxPoints()
        {
            int points = _player.Level >= 36 || _player.Level >= _player.RenownRank
                ? _player.RenownRank
                : _player.Level;

            if (points > MAX_SPENDABLE_RENOWN_POINTS)
                points = MAX_SPENDABLE_RENOWN_POINTS;
            if (points < 0)
                points = 0;

            return (byte)points;
        }
    }
}
