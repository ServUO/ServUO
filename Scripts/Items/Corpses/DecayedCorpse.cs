using System;

namespace Server.Items
{
    public class DecayedCorpse : Container
    {
        private static readonly TimeSpan m_DefaultDecayTime = TimeSpan.FromMinutes(7.0);
        private Timer m_DecayTimer;
        private DateTime m_DecayTime;
        public DecayedCorpse(string name)
            : base(Utility.Random(0xECA, 9))
        {
            this.Movable = false;
            this.Name = name;

            this.BeginDecay(m_DefaultDecayTime);
        }

        public DecayedCorpse(Serial serial)
            : base(serial)
        {
        }

        // Do not display (x items, y stones)
        public override bool DisplaysContent
        {
            get
            {
                return false;
            }
        }
        public void BeginDecay(TimeSpan delay)
        {
            if (this.m_DecayTimer != null)
                this.m_DecayTimer.Stop();

            this.m_DecayTime = DateTime.UtcNow + delay;

            this.m_DecayTimer = new InternalTimer(this, delay);
            this.m_DecayTimer.Start();
        }

        public override void OnAfterDelete()
        {
            if (this.m_DecayTimer != null)
                this.m_DecayTimer.Stop();

            this.m_DecayTimer = null;
        }

        // Do not display (x items, y stones)
        public override bool CheckContentDisplay(Mobile from)
        {
            return false;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1046414, this.Name); // the remains of ~1_NAME~
        }

        public override void OnSingleClick(Mobile from)
        {
            this.LabelTo(from, 1046414, this.Name); // the remains of ~1_NAME~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(this.m_DecayTimer != null);

            if (this.m_DecayTimer != null)
                writer.WriteDeltaTime(this.m_DecayTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.BeginDecay(m_DefaultDecayTime);

                        break;
                    }
                case 1:
                    {
                        if (reader.ReadBool())
                            this.BeginDecay(reader.ReadDeltaTime() - DateTime.UtcNow);

                        break;
                    }
            }
        }

        private class InternalTimer : Timer
        {
            private readonly DecayedCorpse m_Corpse;
            public InternalTimer(DecayedCorpse c, TimeSpan delay)
                : base(delay)
            {
                this.m_Corpse = c;
                this.Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                this.m_Corpse.Delete();
            }
        }
    }
}