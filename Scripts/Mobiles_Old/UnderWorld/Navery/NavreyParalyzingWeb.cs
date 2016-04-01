using System;
using Server.Mobiles;

namespace Server.Items
{
    [Flipable(0x0EE3, 0x0EE4, 0x0EE5, 0x0EE6)]
    public class NavreyParalyzingWeb : Item
    {
        private static readonly TimeSpan duration = TimeSpan.FromSeconds(60.0);
        private Timer m_Timer;
        private DateTime m_End;
        public NavreyParalyzingWeb()
            : base(0x0EE3 + Utility.Random(4))
        {
            this.Visible = true;
            this.Movable = false;

            this.m_Timer = new InternalTimer(this, duration);
            this.m_Timer.Start();

            this.m_End = DateTime.UtcNow + duration;
        }

        public NavreyParalyzingWeb(Serial serial)
            : base(serial)
        {
        }

        public override bool BlocksFit
        {
            get
            {
                return true;
            }
        }
        public override void OnDelete()
        {
            base.OnDelete();

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            // remove paralyze from all chars in this location
            foreach (Mobile m in this.Map.GetMobilesInRange(this.Location, 0))
            {
                if (null != m)
                    m.Paralyzed = false;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.WriteDeltaTime(this.m_End);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_End = reader.ReadDeltaTime();

                        this.m_Timer = new InternalTimer(this, this.m_End - DateTime.UtcNow);
                        this.m_Timer.Start();

                        break;
                    }
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is Navrey)
                return true;

            if (AccessLevel.Player == m.AccessLevel)
            {
                m.Paralyze(duration);
    
                m.PlaySound(0x204);
                m.FixedEffect(0x376A, 10, 16);
            }

            return true;
        }

        private class InternalTimer : Timer
        {
            private readonly Item m_Item;
            public InternalTimer(Item item, TimeSpan duration)
                : base(duration)
            {
                this.Priority = TimerPriority.OneSecond;
                this.m_Item = item;
            }

            protected override void OnTick()
            {
                this.m_Item.Delete();
            }
        }
    }
}