using System;

namespace Server.Items
{
    public abstract class BaseTrap : Item
    {
        private DateTime m_NextPassiveTrigger, m_NextActiveTrigger;
        public BaseTrap(int itemID)
            : base(itemID)
        {
            Movable = false;
        }

        public BaseTrap(Serial serial)
            : base(serial)
        {
        }

        public virtual bool PassivelyTriggered => false;
        public virtual TimeSpan PassiveTriggerDelay => TimeSpan.Zero;
        public virtual int PassiveTriggerRange => -1;
        public virtual TimeSpan ResetDelay => TimeSpan.Zero;
        public override bool HandlesOnMovement => true;// Tell the core that we implement OnMovement
        public virtual void OnTrigger(Mobile from)
        {
        }

        public virtual int GetEffectHue()
        {
            int hue = Hue & 0x3FFF;

            if (hue < 2)
                return 0;

            return hue - 1;
        }

        public bool CheckRange(Point3D loc, Point3D oldLoc, int range)
        {
            return CheckRange(loc, range) && !CheckRange(oldLoc, range);
        }

        public bool CheckRange(Point3D loc, int range)
        {
            return ((Z + 8) >= loc.Z && (loc.Z + 16) > Z) &&
                   Utility.InRange(GetWorldLocation(), loc, range);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m.Location == oldLocation)
                return;

            if (CheckRange(m.Location, oldLocation, 0) && DateTime.UtcNow >= m_NextActiveTrigger)
            {
                m_NextActiveTrigger = m_NextPassiveTrigger = DateTime.UtcNow + ResetDelay;

                OnTrigger(m);
            }
            else if (PassivelyTriggered && CheckRange(m.Location, oldLocation, PassiveTriggerRange) && DateTime.UtcNow >= m_NextPassiveTrigger)
            {
                m_NextPassiveTrigger = DateTime.UtcNow + PassiveTriggerDelay;

                OnTrigger(m);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}