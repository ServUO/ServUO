using System;

namespace Server.Items
{
    public class PeerlessKey : Item
    {
        private int m_Lifespan;
        private Timer m_Timer;

        [Constructable]
        public PeerlessKey(int itemID)
            : base(itemID)
        {
            LootType = LootType.Blessed;
		
            if (Lifespan > 0)
            {
                m_Lifespan = Lifespan;
                StartTimer();
            }
        }

        public PeerlessKey(Serial serial)
            : base(serial)
        {
        }

        public virtual int Lifespan { get { return 604800; } }
        public virtual bool UseSeconds { get { return false; } }

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
                if (UseSeconds)
                    list.Add(1072517, m_Lifespan.ToString()); // Lifespan: ~1_val~ seconds
                else
                {
                    TimeSpan t = TimeSpan.FromSeconds(TimeLeft);

                    int weeks = (int)t.Days / 7;
                    int days = t.Days;
                    int hours = t.Hours;
                    int minutes = t.Minutes;

                    if (weeks > 1)
                        list.Add(1153092, (t.Days / 7).ToString()); // Lifespan: ~1_val~ weeks
                    else if (days > 1)
                        list.Add(1153091, t.Days.ToString()); // Lifespan: ~1_val~ days
                    else if (hours > 1)
                        list.Add(1153090, t.Hours.ToString()); // Lifespan: ~1_val~ hours
                    else if (minutes > 1)
                        list.Add(1153089, t.Minutes.ToString()); // Lifespan: ~1_val~ minutes
                    else
                        list.Add(1072517, t.Seconds.ToString()); // Lifespan: ~1_val~ seconds
                }
            }
        }

        public virtual void StartTimer()
        {
            if (m_Timer != null)
                return;
				
            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), new TimerCallback(Slice));
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
            writer.Write((int)0); // version
			
            writer.Write((int)m_Lifespan);
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
