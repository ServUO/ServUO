using System;

namespace Server.Items
{
    public class PlagueBeastHeart : PlagueBeastInnard
    {
        private Timer m_Timer;
        public PlagueBeastHeart()
            : base(0x1363, 0x21)
        {
            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public PlagueBeastHeart(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null && m_Timer.Running)
                m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        private class InternalTimer : Timer
        {
            private readonly PlagueBeastHeart m_Heart;
            private bool m_Delay;
            public InternalTimer(PlagueBeastHeart heart)
                : base(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5))
            {
                m_Heart = heart;
            }

            protected override void OnTick()
            {
                if (m_Heart == null || m_Heart.Deleted || m_Heart.Owner == null || !m_Heart.Owner.Alive)
                {
                    Stop();
                    return;
                }

                if (m_Heart.ItemID == 0x1363)
                {
                    if (m_Delay)
                    {
                        m_Heart.ItemID = 0x1367;
                        m_Heart.Owner.PlaySound(0x11F);
                    }

                    m_Delay = !m_Delay;
                }
                else
                {
                    m_Heart.ItemID = 0x1363;
                    m_Heart.Owner.PlaySound(0x120);
                    m_Delay = false;
                }
            }
        }
    }
}