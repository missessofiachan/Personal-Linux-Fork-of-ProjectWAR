using FrameWork;
using GameData;
using WorldServer.World.Abilities.Components;
using WorldServer.World.Objects;
using Opcodes = WorldServer.NetWork.Opcodes;

namespace WorldServer.World.Abilities.CareerInterfaces
{
    public class CareerInterface_WPDoK : CareerInterface
    {
        private ushort _resourceID;
        private int _updateTimer;
        private long _lastUpdateTime;
        private float _conversionMod;

        public CareerInterface_WPDoK(Player player) : base(player)
        {
            _maxResource = 250;
            _careerResource = _maxResource;
            if (player.Info.CareerLine == (byte) CareerLine.CAREERLINE_WARRIOR_PRIEST)
            {
                _resourceID = 308;
                _conversionMod = 0.16f;
            }
            else
            {
                _resourceID = 314;
                _conversionMod = 0.1f;
            }

            _resourceTimeout = 0;
        }

        public override void Notify_PlayerLoaded()
        {
            SendResource();
        }

        public override void Update(long tick)
        {
            if (_lastUpdateTime > 0)
            {
                _updateTimer += (int)(tick - _lastUpdateTime);

                if (_updateTimer >= 1000)
                {
                    int newResourceVal = 0;
                    while (_updateTimer >= 1000)
                    {
                        if (CurrentStance == 1 && DrainActive(tick))
                        {
                            if (!myPlayer.CbtInterface.IsInCombat)
                                newResourceVal += 15;
                            else
                                newResourceVal -= 5;
                        }
                        else if (!myPlayer.CbtInterface.IsInCombat)
                            newResourceVal += 20;
                        _updateTimer -= 1000;
                    }

                    if (!myPlayer.IsDead)
                    {
                        if (_careerResource < 250 && newResourceVal > 0)
                        {
                            AddResource((byte) newResourceVal, true);
                            myPlayer.BuffInterface.NotifyResourceEvent((byte) BuffCombatEvents.ResourceSet, _lastResource, ref _careerResource);
                        }

                        else if (_careerResource > 0 && newResourceVal < 0)
                        {
                            ConsumeResource((byte) -newResourceVal, true);
                            myPlayer.BuffInterface.NotifyResourceEvent((byte) BuffCombatEvents.ResourceSet, _lastResource, ref _careerResource);
                        }
                    }
                }
            }

            _lastUpdateTime = tick;
        }

        public override byte GetCurrentResourceLevel(byte type)
        {
            return (byte)(_careerResource * _conversionMod);
        }

        public override byte GetLevelForResource(byte res, byte which)
        {
            return (byte)(res * _conversionMod);
        }

        public override void SendResource()
        {
            PacketOut Out;
            if (_lastResource != 0)
            {
                Out = new PacketOut((byte)Opcodes.F_INIT_EFFECTS, 12);
                Out.WriteByte(1);
                Out.WriteByte(BUFF_REMOVE); // add
                Out.WriteUInt16(0x7C00); // unk3, God only knows
                Out.WriteUInt16(_Owner.Oid);
                Out.WriteByte(255); // buffID - some number I pulled out of the air
                Out.WriteByte(0);
                Out.WriteUInt16R(_resourceID); // Balance
                Out.WriteByte(00);

                myPlayer.SendPacket(Out);
            }
            if (_careerResource == 0)
                return; // zero resource means there's no buff left on the client

            Out = new PacketOut((byte)Opcodes.F_INIT_EFFECTS, 18);
            Out.WriteByte(1);
            Out.WriteByte(BUFF_ADD); // add
            Out.WriteUInt16(0); // unk3, God only knows
            Out.WriteUInt16(_Owner.Oid);
            Out.WriteByte(255); // buffID - some number I pulled out of the air
            Out.WriteByte(0);
            Out.WriteUInt16R(_resourceID); // Balance
            Out.WriteByte(00); // Duration
            Out.WriteUInt16R(_Owner.Oid);

            Out.WriteByte(0x01);
            Out.WriteByte(00);
            Out.WriteZigZag(_careerResource);

            Out.WriteByte(00);
            myPlayer.SendPacket(Out);
        }

        public override EArchetype GetArchetype()
        {
            // Check for Damage mastery
            if (myPlayer.Info.CareerLine == 12)
            {
                int dpsMastery = myPlayer.AbtInterface.GetMasteryLevelFor(3);

                if (dpsMastery > myPlayer.AbtInterface.GetMasteryLevelFor(2) && dpsMastery > myPlayer.AbtInterface.GetMasteryLevelFor(1))
                    return EArchetype.ARCHETYPE_DPS;
            }

            else
            {
                int dpsMastery = myPlayer.AbtInterface.GetMasteryLevelFor(2);

                if (dpsMastery > myPlayer.AbtInterface.GetMasteryLevelFor(3) && dpsMastery > myPlayer.AbtInterface.GetMasteryLevelFor(1))
                    return EArchetype.ARCHETYPE_DPS;
            }

            return EArchetype.ARCHETYPE_Healer;
        }
     

        private int _currentStance;

        private int CurrentStance
        {
            get { return _currentStance; }
            set
            {
                if (_currentStance == 2)
                    myPlayer.BuffInterface.RemoveBuffByEntry(18139);
                _currentStance = value;
            }
        }

        private long _drainTimeEnd;

        private int DRAIN_DURATION_MS = 10000;

        private bool DrainActive(long tick)
        {
            return tick < _drainTimeEnd;
        }

        public void UpdateDrain()
        {
            _drainTimeEnd = TCPManager.GetTimeStampMS() + DRAIN_DURATION_MS;
        }
    }
}
