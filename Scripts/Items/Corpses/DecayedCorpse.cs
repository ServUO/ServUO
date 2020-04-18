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
            Movable = false;
            Name = name;

            BeginDecay(m_DefaultDecayTime);
        }

        public DecayedCorpse(Serial serial)
            : base(serial)
        {
        }

        // Do not display (x items, y stones)
        public override bool DisplaysContent => false;
        public void BeginDecay(TimeSpan delay)
        {
            if (m_DecayTimer != null)
                m_DecayTimer.Stop();

            m_DecayTime = DateTime.UtcNow + delay;

            m_DecayTimer = new InternalTimer(this, delay);
            m_DecayTimer.Start();
        }

        public override void OnAfterDelete()
        {
            if (m_DecayTimer != null)
                m_DecayTimer.Stop();

            m_DecayTimer = null;
        }

        // Do not display (x items, y stones)
        public override bool CheckContentDisplay(Mobile from)
        {
            return false;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1046414, Name); // the remains of ~1_NAME~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(m_DecayTimer != null);

            if (m_DecayTimer != null)
                writer.WriteDeltaTime(m_DecayTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        BeginDecay(m_DefaultDecayTime);

                        break;
                    }
                case 1:
                    {
                        if (reader.ReadBool())
                            BeginDecay(reader.ReadDeltaTime() - DateTime.UtcNow);

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
                m_Corpse = c;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                m_Corpse.Delete();
            }
        }
    }
}
