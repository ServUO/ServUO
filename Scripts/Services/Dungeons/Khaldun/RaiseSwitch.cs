using Server.Network;
using System;

namespace Server.Items
{
    public class RaiseSwitch : Item
    {
        private RaisableItem m_RaisableItem;
        private ResetTimer m_ResetTimer;
        [Constructable]
        public RaiseSwitch()
            : this(0x1093)
        {
        }

        public RaiseSwitch(Serial serial)
            : base(serial)
        {
        }

        protected RaiseSwitch(int itemID)
            : base(itemID)
        {
            Movable = false;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public RaisableItem RaisableItem
        {
            get
            {
                return m_RaisableItem;
            }
            set
            {
                m_RaisableItem = value;
            }
        }
        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(this, 2))
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            if (RaisableItem != null && RaisableItem.Deleted)
                RaisableItem = null;

            Flip();

            if (RaisableItem != null)
            {
                if (RaisableItem.IsRaisable)
                {
                    RaisableItem.Raise();
                    m.LocalOverheadMessage(MessageType.Regular, 0x5A, true, "You hear a grinding noise echoing in the distance.");
                }
                else
                {
                    m.LocalOverheadMessage(MessageType.Regular, 0x5A, true, "You flip the switch again, but nothing happens.");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_RaisableItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_RaisableItem = (RaisableItem)reader.ReadItem();

            Reset();
        }

        protected virtual void Flip()
        {
            if (ItemID != 0x1093)
            {
                ItemID = 0x1093;

                StopResetTimer();
            }
            else
            {
                ItemID = 0x1095;

                if (RaisableItem != null && RaisableItem.CloseDelay >= TimeSpan.Zero)
                    StartResetTimer(RaisableItem.CloseDelay);
                else
                    StartResetTimer(TimeSpan.FromMinutes(2.0));
            }

            Effects.PlaySound(Location, Map, 0x3E8);
        }

        protected void StartResetTimer(TimeSpan delay)
        {
            StopResetTimer();

            m_ResetTimer = new ResetTimer(this, delay);
            m_ResetTimer.Start();
        }

        protected void StopResetTimer()
        {
            if (m_ResetTimer != null)
            {
                m_ResetTimer.Stop();
                m_ResetTimer = null;
            }
        }

        protected virtual void Reset()
        {
            if (ItemID != 0x1093)
                Flip();
        }

        private class ResetTimer : Timer
        {
            private readonly RaiseSwitch m_RaiseSwitch;
            public ResetTimer(RaiseSwitch raiseSwitch, TimeSpan delay)
                : base(delay)
            {
                m_RaiseSwitch = raiseSwitch;

                Priority = ComputePriority(delay);
            }

            protected override void OnTick()
            {
                if (m_RaiseSwitch.Deleted)
                    return;

                m_RaiseSwitch.m_ResetTimer = null;

                m_RaiseSwitch.Reset();
            }
        }
    }

    public class DisappearingRaiseSwitch : RaiseSwitch
    {
        [Constructable]
        public DisappearingRaiseSwitch()
            : base(0x108F)
        {
        }

        public DisappearingRaiseSwitch(Serial serial)
            : base(serial)
        {
        }

        public int CurrentRange => Visible ? 3 : 2;
        public override bool HandlesOnMovement => true;
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (Utility.InRange(m.Location, Location, CurrentRange) || Utility.InRange(oldLocation, Location, CurrentRange))
                Refresh();
        }

        public override void OnMapChange()
        {
            if (!Deleted)
                Refresh();
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (!Deleted)
                Refresh();
        }

        public void Refresh()
        {
            bool found = false;
            IPooledEnumerable eable = GetMobilesInRange(CurrentRange);

            foreach (Mobile mob in eable)
            {
                if (mob.Hidden && mob.IsStaff())
                    continue;

                found = true;
                break;
            }
            eable.Free();
            Visible = found;
        }

        public override void Serialize(GenericWriter writer)
        {
            if (RaisableItem != null && RaisableItem.Deleted)
                RaisableItem = null;

            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            Timer.DelayCall(TimeSpan.Zero, Refresh);
        }

        protected override void Flip()
        {
        }

        protected override void Reset()
        {
        }
    }
}
