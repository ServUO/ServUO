using Server.Network;
using System;

namespace Server.Items
{
    public class WillemHartesHat : FeatheredHat
    {
        private int m_Lifespan;
        private Timer m_Timer;
        public override int LabelNumber => 1154236;  // Willem Harte's Hat

        [Constructable]
        public WillemHartesHat()
            : base(0x171A)
        {
            Hue = 72;
            StrRequirement = 10;

            if (Lifespan > 0)
            {
                m_Lifespan = Lifespan;
                StartTimer();
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154237); // *The hat emits a sour smelling odor indicative of spending a significant period of time in the belly of a dragon.*
        }

        public virtual int Lifespan => 3600;

        [CommandProperty(AccessLevel.GameMaster)]
        public int TimeLeft
        {
            get
            {
                return m_Lifespan;
            }
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

            list.Add(1072351); // Quest Item
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

        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;
        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        public WillemHartesHat(Serial serial)
            : base(serial)
        {
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
