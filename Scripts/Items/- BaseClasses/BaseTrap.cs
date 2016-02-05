using System;

namespace Server.Items
{
    public abstract class BaseTrap : Item
    {
        public virtual bool RevealOnTrigger { get { return true; } }
        public virtual bool PassivelyTriggered { get { return false; } }
        public virtual TimeSpan PassiveTriggerDelay { get { return TimeSpan.Zero; } }
        public virtual int PassiveTriggerRange { get { return -1; } }
        public virtual TimeSpan ResetDelay { get { return TimeSpan.Zero; } }

        private DateTime m_NextPassiveTrigger, m_NextActiveTrigger;

        public virtual void OnTrigger(Mobile from)
        {
        }

        public override bool HandlesOnMovement { get { return true; } } // Tell the core that we implement OnMovement

        public virtual int GetEffectHue()
        {
            int hue = this.Hue & 0x3FFF;

            if (hue < 2)
            {
                return 0;
            }

            return hue - 1;
        }

        public bool CheckRange(Point3D loc, Point3D oldLoc, int range)
        {
            return CheckRange(loc, range) && !CheckRange(oldLoc, range);
        }

        public bool CheckRange(Point3D loc, int range)
        {
            return ((this.Z + 8) >= loc.Z && (loc.Z + 16) > this.Z)
                && Utility.InRange(GetWorldLocation(), loc, range);
        }

        public virtual int MessageHue { get { return 0; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
            {
                return;
            }

            base.OnMovement(m, oldLocation);

            if (m.Location == oldLocation)
            {
                return;
            }

            if (CheckRange(m.Location, oldLocation, 5))
            {

                double chance = m.Skills[SkillName.DetectHidden].Value / 100;

                if (chance >= Utility.RandomDouble())
                {
                    // [trapped]
                    PublicOverheadMessage(Network.MessageType.Regular, MessageHue, 500813, "");
                }
            }

            if (CheckRange(m.Location, oldLocation, 0) && DateTime.UtcNow >= m_NextActiveTrigger)
            {
                m_NextActiveTrigger = m_NextPassiveTrigger = DateTime.UtcNow + ResetDelay;

                if (RevealOnTrigger)
                {
                    this.Visible = true;
                }

                OnTrigger(m);
            }
            else if (PassivelyTriggered && CheckRange(m.Location, oldLocation, PassiveTriggerRange) && DateTime.UtcNow >= m_NextPassiveTrigger)
            {
                m_NextPassiveTrigger = DateTime.UtcNow + PassiveTriggerDelay;

                if (RevealOnTrigger)
                {
                    this.Visible = true;
                }

                OnTrigger(m);
            }
        }

        public BaseTrap(int itemID)
            : base(itemID)
        {
            Movable = false;
        }

        public BaseTrap(Serial serial)
            : base(serial)
        {
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