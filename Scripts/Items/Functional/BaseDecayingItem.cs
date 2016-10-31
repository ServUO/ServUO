using System;
using Server;

namespace Server.Items
{
	public class BaseDecayingItem : Item
	{
        private int m_Lifespan;
        private Timer m_Timer;

		public virtual int Lifespan { get { return 0; } }
        public virtual bool UseSeconds { get { return true; } }
		
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

        public BaseDecayingItem(int itemID) : base(itemID)
        {
            LootType = LootType.Blessed;
		
            if (Lifespan > 0)
            {
                m_Lifespan = this.Lifespan;
                StartTimer();
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
            if (m_Timer != null || Lifespan == 0)
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
                Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(this.Location, this.Map, 0x201);
            }
			
            StopTimer();
            Delete();
        }
		
		public BaseDecayingItem (Serial serial) : base(serial)
		{
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

            if(Lifespan > 0)
                StartTimer();
        }
	}
}
