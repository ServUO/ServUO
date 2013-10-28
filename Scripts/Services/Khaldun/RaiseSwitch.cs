using System;
using Server.Network;

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
            this.Movable = false;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public RaisableItem RaisableItem
        {
            get
            {
                return this.m_RaisableItem;
            }
            set
            {
                this.m_RaisableItem = value;
            }
        }
        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(this, 2))
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            if (this.RaisableItem != null && this.RaisableItem.Deleted)
                this.RaisableItem = null;

            this.Flip();

            if (this.RaisableItem != null)
            {
                if (this.RaisableItem.IsRaisable)
                {
                    this.RaisableItem.Raise();
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

            writer.WriteEncodedInt((int)0); // version

            writer.Write((Item)this.m_RaisableItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_RaisableItem = (RaisableItem)reader.ReadItem();

            this.Reset();
        }

        protected virtual void Flip()
        {
            if (this.ItemID != 0x1093)
            {
                this.ItemID = 0x1093;

                this.StopResetTimer();
            }
            else
            {
                this.ItemID = 0x1095;

                if (this.RaisableItem != null && this.RaisableItem.CloseDelay >= TimeSpan.Zero)
                    this.StartResetTimer(this.RaisableItem.CloseDelay);
                else
                    this.StartResetTimer(TimeSpan.FromMinutes(2.0));
            }

            Effects.PlaySound(this.Location, this.Map, 0x3E8);
        }

        protected void StartResetTimer(TimeSpan delay)
        {
            this.StopResetTimer();

            this.m_ResetTimer = new ResetTimer(this, delay);
            this.m_ResetTimer.Start();
        }

        protected void StopResetTimer()
        {
            if (this.m_ResetTimer != null)
            {
                this.m_ResetTimer.Stop();
                this.m_ResetTimer = null;
            }
        }

        protected virtual void Reset()
        {
            if (this.ItemID != 0x1093)
                this.Flip();
        }

        private class ResetTimer : Timer
        {
            private readonly RaiseSwitch m_RaiseSwitch;
            public ResetTimer(RaiseSwitch raiseSwitch, TimeSpan delay)
                : base(delay)
            {
                this.m_RaiseSwitch = raiseSwitch;

                this.Priority = ComputePriority(delay);
            }

            protected override void OnTick()
            {
                if (this.m_RaiseSwitch.Deleted)
                    return;

                this.m_RaiseSwitch.m_ResetTimer = null;

                this.m_RaiseSwitch.Reset();
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

        public int CurrentRange
        {
            get
            {
                return this.Visible ? 3 : 2;
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (Utility.InRange(m.Location, this.Location, this.CurrentRange) || Utility.InRange(oldLocation, this.Location, this.CurrentRange))
                this.Refresh();
        }

        public override void OnMapChange()
        {
            if (!this.Deleted)
                this.Refresh();
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (!this.Deleted)
                this.Refresh();
        }

        public void Refresh()
        {
            bool found = false;
            foreach (Mobile mob in this.GetMobilesInRange(this.CurrentRange))
            {
                if (mob.Hidden && mob.IsStaff())
                    continue;

                found = true;
                break;
            }

            this.Visible = found;
        }

        public override void Serialize(GenericWriter writer)
        {
            if (this.RaisableItem != null && this.RaisableItem.Deleted)
                this.RaisableItem = null;

            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Refresh));
        }

        protected override void Flip()
        {
        }

        protected override void Reset()
        {
        }
    }
}