using System;

namespace Server.Items
{
    public class RobeofRite : Robe
    {
        public override int LabelNumber => 1153510;  // robe of rite

        private int m_Lifespan;
        private Timer m_Timer;

        [Constructable]
        public RobeofRite() : base(0x1F03)
        {
            Weight = 3;
            Hue = 2702;
            StrRequirement = 10;

            if (Lifespan > 0)
            {
                m_Lifespan = Lifespan;
                StartTimer();
            }
        }

        public RobeofRite(Serial serial) : base(serial)
        {
        }

        public virtual int Lifespan => 604800;

        [CommandProperty(AccessLevel.GameMaster)]
        public int TimeLeft
        {
            get { return m_Lifespan; }
            set
            {
                m_Lifespan = value;
                InvalidateProperties();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Lifespan > 0)
            {
                TimeSpan t = TimeSpan.FromSeconds(m_Lifespan);

                int weeks = t.Days / 7;
                int days = t.Days;
                int hours = t.Hours;
                int minutes = t.Minutes;

                if (weeks > 1)
                    list.Add(1153092, weeks.ToString()); // Lifespan: ~1_val~ weeks
                else if (days > 1)
                    list.Add(1153091, days.ToString()); // Lifespan: ~1_val~ days
                else if (hours > 1)
                    list.Add(1153090, hours.ToString()); // Lifespan: ~1_val~ hours
                else if (minutes > 1)
                    list.Add(1153089, minutes.ToString()); // Lifespan: ~1_val~ minutes
                else
                    list.Add(1072517, m_Lifespan.ToString()); // Lifespan: ~1_val~ seconds
            }
        }

        public virtual void StartTimer()
        {
            if (m_Timer != null)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), Slice);
            m_Timer.Priority = TimerPriority.OneSecond;
        }

        public virtual void StopTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }

        public virtual void Slice()
        {
            m_Lifespan -= 10;

            InvalidateProperties();

            if (m_Lifespan <= 0)
                Decay();
        }

        public virtual void Decay()
        {
            if (RootParent is Mobile)
            {
                Mobile parent = (Mobile)RootParent;

                if (Name == null)
                    parent.SendLocalizedMessage(1072515, "#" + LabelNumber); // The ~1_name~ expired...
                else
                    parent.SendLocalizedMessage(1072515, Name); // The ~1_name~ expired...

                Effects.SendLocationParticles(EffectItem.Create(parent.Location, parent.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(parent.Location, parent.Map, 0x201);
            }
            else
            {
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(Location, Map, 0x201);
            }

            StopTimer();
            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.Write(m_Lifespan);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Lifespan = reader.ReadInt();

            StartTimer();
        }
    }
}

