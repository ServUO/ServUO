using System;

namespace Server.Items
{
    public class PlagueBeastHeart : PlagueBeastInnard
    {
        private Timer m_Timer;
        public PlagueBeastHeart()
            : base(0x1363, 0x21)
        {
            this.m_Timer = new InternalTimer(this);
            this.m_Timer.Start();
        }

        public PlagueBeastHeart(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (this.m_Timer != null && this.m_Timer.Running)
                this.m_Timer.Stop();
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
			
            this.m_Timer = new InternalTimer(this);
            this.m_Timer.Start();
        }

        private class InternalTimer : Timer
        {
            private readonly PlagueBeastHeart m_Heart;
            private bool m_Delay;
            public InternalTimer(PlagueBeastHeart heart)
                : base(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5))
            {
                this.m_Heart = heart;
            }

            protected override void OnTick()
            {
                if (this.m_Heart == null || this.m_Heart.Deleted || this.m_Heart.Owner == null || !this.m_Heart.Owner.Alive)
                {
                    this.Stop();
                    return;
                }

                if (this.m_Heart.ItemID == 0x1363)
                {
                    if (this.m_Delay)
                    {
                        this.m_Heart.ItemID = 0x1367;
                        this.m_Heart.Owner.PlaySound(0x11F);
                    }

                    this.m_Delay = !this.m_Delay;
                }
                else
                {
                    this.m_Heart.ItemID = 0x1363;
                    this.m_Heart.Owner.PlaySound(0x120);
                    this.m_Delay = false;
                }
            }
        }
    }
}