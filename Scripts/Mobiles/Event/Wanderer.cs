using System;

namespace Server.Mobiles
{
    public class Wanderer : Mobile
    {
        private readonly Timer m_Timer;
        [Constructable]
        public Wanderer()
        {
            Name = "Me";
            Body = 0x1;
            AccessLevel = AccessLevel.Counselor;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public Wanderer(Serial serial)
            : base(serial)
        {
            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public override void OnDelete()
        {
            m_Timer.Stop();

            base.OnDelete();
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

        private class InternalTimer : Timer
        {
            private readonly Wanderer m_Owner;
            private int m_Count = 0;
            public InternalTimer(Wanderer owner)
                : base(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1))
            {
                m_Owner = owner;
            }

            protected override void OnTick()
            {
                if ((m_Count++ & 0x3) == 0)
                {
                    m_Owner.Direction = (Direction)(Utility.Random(8) | 0x80);
                }

                m_Owner.Move(m_Owner.Direction);
            }
        }
    }
}