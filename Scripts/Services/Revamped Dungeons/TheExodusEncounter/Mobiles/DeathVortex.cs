using System;
using Server.Mobiles;

namespace Server.Items
{
    public class DeathVortexTrap : BaseTrap
    {
        private Timer m_Timer;

        [Constructable]
        public DeathVortexTrap()
            : base(0x3789)
        {
            this.Hue = 2070;
            this.Movable = false;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public DeathVortexTrap(Serial serial)
            : base(serial)
        {
        }

        public override void OnDelete()
        {
            m_Timer.Stop();

            base.OnDelete();
        }

        public override bool PassivelyTriggered { get { return true; } }
        public override TimeSpan PassiveTriggerDelay { get { return TimeSpan.FromSeconds(2.0); } }
        public override int PassiveTriggerRange { get { return 3; } }
        public override TimeSpan ResetDelay { get { return TimeSpan.FromSeconds(0.2); } }

        public override void OnTrigger(Mobile from)
        {
            if (from.IsStaff())
                return;

            if (from.Alive && this.CheckRange(from.Location, 1) && !(from is ClockworkExodus))
                StamManaDrain(from);
        }

        public void StamManaDrain(Mobile defender)
        {
            switch (Utility.Random(2)) // 50%/50% for stamina leech or mana leech
            {
                case 0:
                    {
                        if (defender.Alive)
                        {
                            int manaToLeech = (int)(defender.Mana * 0.6); // defender loses 1/2 of their mana
                            defender.Mana -= manaToLeech;
                        }
                        break;
                    }
                case 1:
                    {
                        if (defender.Alive)
                        {
                            int stamToLeech = (int)(defender.Stam * 0.7); // defender loses 9/10 of their stamina
                            defender.Stam -= stamToLeech;
                        }
                        break;
                    }
            }

            defender.SendLocalizedMessage(1152694, "", 0x22); // Your life force is drained by the death vortex! 
        }
        

        private class InternalTimer : Timer
        {
            private DeathVortexTrap m_Item;

            public InternalTimer(DeathVortexTrap item) : base(TimeSpan.FromSeconds(15.0))
            {
                m_Item = item;

                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                if (m_Item != null)
                    m_Item.Delete();

                Stop();
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

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }
    }
}