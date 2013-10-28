using System;

namespace Server.Items
{
    public abstract class BaseTrap : Item
    {
        private DateTime m_NextPassiveTrigger, m_NextActiveTrigger;
        public BaseTrap(int itemID)
            : base(itemID)
        {
            this.Movable = false;
        }

        public BaseTrap(Serial serial)
            : base(serial)
        {
        }

        public virtual bool PassivelyTriggered
        {
            get
            {
                return false;
            }
        }
        public virtual TimeSpan PassiveTriggerDelay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        public virtual int PassiveTriggerRange
        {
            get
            {
                return -1;
            }
        }
        public virtual TimeSpan ResetDelay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }// Tell the core that we implement OnMovement
        public virtual void OnTrigger(Mobile from)
        {
        }

        public virtual int GetEffectHue()
        {
            int hue = this.Hue & 0x3FFF;

            if (hue < 2)
                return 0;

            return hue - 1;
        }

        public bool CheckRange(Point3D loc, Point3D oldLoc, int range)
        {
            return this.CheckRange(loc, range) && !this.CheckRange(oldLoc, range);
        }

        public bool CheckRange(Point3D loc, int range)
        {
            return ((this.Z + 8) >= loc.Z && (loc.Z + 16) > this.Z) &&
                   Utility.InRange(this.GetWorldLocation(), loc, range);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m.Location == oldLocation)
                return;

            if (this.CheckRange(m.Location, oldLocation, 0) && DateTime.UtcNow >= this.m_NextActiveTrigger)
            {
                this.m_NextActiveTrigger = this.m_NextPassiveTrigger = DateTime.UtcNow + this.ResetDelay;

                this.OnTrigger(m);
            }
            else if (this.PassivelyTriggered && this.CheckRange(m.Location, oldLocation, this.PassiveTriggerRange) && DateTime.UtcNow >= this.m_NextPassiveTrigger)
            {
                this.m_NextPassiveTrigger = DateTime.UtcNow + this.PassiveTriggerDelay;

                this.OnTrigger(m);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}