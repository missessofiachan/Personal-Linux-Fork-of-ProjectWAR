using System.Threading;
using FrameWork;
using WorldServer.World.Abilities.Components;
using WorldServer.World.Objects;
using WorldServer.World.Positions;
using Object = WorldServer.World.Objects.Object;
using Opcodes = WorldServer.NetWork.Opcodes;

namespace WorldServer.World.Abilities.Buffs.SpecialBuffs
{
    class InteractionBuff: NewBuff
    {
        private const int MaxInteractionDriftFeet = 8;
        private const int MaxInteractionTargetRangeFeet = 20;

        private Object _target;
        private Point3D _startPosition;

        public override void StartBuff()
        {
            BuffState = (byte)EBuffState.Running;

            _buffInterface.AddEventSubscription(this, (byte)BuffCombatEvents.ReceivingDamage);
            _buffInterface.AddEventSubscription(this, (byte)BuffCombatEvents.AbilityStarted);

            if (Caster.AbtInterface.IsCasting())
                BuffHasExpired = true;
        }

        public void SetObject(Object target)
        {
            _target = target;
            _startPosition = new Point3D(Caster.WorldPosition);

            PacketOut Out = new PacketOut((byte)Opcodes.F_SET_ABILITY_TIMER, 12);
                Out.WriteByte(0);
                Out.WriteByte(1);
                Out.WriteByte(1);
                Out.WriteByte(5);
                Out.WriteByte(0);
                Out.WriteByte(0);
                Out.WriteUInt16((ushort)(_buffInfo.Duration * 1000));
                Out.WriteUInt16(_target.Oid);
                Out.WriteByte(0);
                Out.WriteByte(0);
            ((Player)Caster).SendPacket(Out);
            ((Player)Caster).KneelDown(_target.Oid, true, (ushort)(_buffInfo.Duration * 1000));
        }

        public override void Update(long tick)
        {
            if (BuffState != (byte) EBuffState.Running)
                return;

            if (!CanContinueInteraction())
            {
                BuffEnded(true, false);
                return;
            }

            long curTime = TCPManager.GetTimeStampMS();

            if (EndTime > 0 && curTime >= EndTime)
                BuffEnded(false, false);
        }

        private bool CanContinueInteraction()
        {
            if (_target == null || !(Caster is Player player))
                return false;

            if (player.IsDead || player.PendingDisposal || player.Region != _target.Region)
                return false;

            if (_startPosition != null && !player.PointWithinRadiusFeet(_startPosition, MaxInteractionDriftFeet))
                return false;

            return _target.GetDistanceToObject(player, true) <= MaxInteractionTargetRangeFeet;
        }

        protected override void BuffEnded(bool wasRemoved, bool wasManual)
        {
            if (Interlocked.CompareExchange(ref BuffEndLock, 1, 0) != 0)
                return;

                BuffHasExpired = true;
                WasManuallyRemoved = wasManual;

                if (wasRemoved)
                    BuffState = (byte)EBuffState.Removed;
                else BuffState = (byte)EBuffState.Ended;

            Interlocked.Exchange(ref BuffEndLock, 0);

            if (_target != null)
            {
                if (wasRemoved)
                    _target.NotifyInteractionBroken(this);
                else
                    _target.NotifyInteractionComplete(this);

                if (!HasSentEnd)
                {
                    PacketOut Out = new PacketOut((byte) Opcodes.F_SET_ABILITY_TIMER, 12);
                    Out.WriteByte(0);
                    Out.WriteByte(1);
                    Out.WriteByte(1);
                    Out.Fill(0, 9);
                    ((Player) Caster).SendPacket(Out);
                }
                ((Player)Caster).KneelDown(_target.Oid, false);
            }

            _buffInterface.RemoveEventSubscription(this, (byte)BuffCombatEvents.ReceivingDamage);
            _buffInterface.RemoveEventSubscription(this, (byte)BuffCombatEvents.AbilityStarted);
        }

        public override void InvokeDamageEvent(byte eventId, AbilityDamageInfo damageInfo, Unit eventInstigator)
        {
			if (eventId == (byte)BuffCombatEvents.ReceivingDamage && !damageInfo.IsAoE)
			{
				TryCancel();
			}
        }

        public override void InvokeCastEvent(byte eventID, AbilityInfo abInfo)
        {
            HasSentEnd = true;
            TryCancel();
        }

        private void TryCancel()
        {
            if (BuffState != (byte)EBuffState.Running)
                return;

            if (Interlocked.CompareExchange(ref BuffStackLock, 1, 0) != 0)
                return;

            if (StackLevel == 0)
            {
                Interlocked.Exchange(ref BuffStackLock, 0);
                return;
            }

            RemoveStack();

            Interlocked.Exchange(ref BuffStackLock, 0);
        }

        public static InteractionBuff GetNew()
        {
            return new InteractionBuff();
        }
    }
}
