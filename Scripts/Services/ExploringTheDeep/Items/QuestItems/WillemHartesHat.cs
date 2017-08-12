using System;
using Server.Network;

namespace Server.Items
{
    public class WillemHartesHat : FeatheredHat
    {
        private int m_Lifespan;
        private Timer m_Timer;
        public override int LabelNumber { get { return 1154236; } } // Willem Harte's Hat
        
        [Constructable]
        public WillemHartesHat()
            : base(0x171A)
        {
            this.Hue = 72;
            this.StrRequirement = 10;

            if (this.Lifespan > 0)
            {
                this.m_Lifespan = this.Lifespan;
                this.StartTimer();
            }
        }
		
		public override void OnDoubleClick(Mobile from)
        {
			base.OnDoubleClick(from);			
			
			from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154237); // *The hat emits a sour smelling odor indicative of spending a significant period of time in the belly of a dragon.*
        }

        public virtual int Lifespan { get { return 3600; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TimeLeft
        {
            get
            {
                return this.m_Lifespan;
            }
            set
            {
                this.m_Lifespan = value;
                this.InvalidateProperties();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.Lifespan > 0)
            {
                TimeSpan t = TimeSpan.FromSeconds(this.m_Lifespan);

                int weeks = (int)t.Days / 7;
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
                    list.Add(1072517, this.m_Lifespan.ToString()); // Lifespan: ~1_val~ seconds
            }
			
			list.Add(1072351); // Quest Item
        }

        public virtual void StartTimer()
        {
            if (this.m_Timer != null)
                return;

            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), new TimerCallback(Slice));
            this.m_Timer.Priority = TimerPriority.OneSecond;
        }

        public virtual void StopTimer()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;
        }

        public virtual void Slice()
        {
            this.m_Lifespan -= 10;

            this.InvalidateProperties();

            if (this.m_Lifespan <= 0)
                this.Decay();
        }

        public virtual void Decay()
        {
            if (this.RootParent is Mobile)
            {
                Mobile parent = (Mobile)this.RootParent;

                if (this.Name == null)
                    parent.SendLocalizedMessage(1072515, "#" + this.LabelNumber); // The ~1_name~ expired...
                else
                    parent.SendLocalizedMessage(1072515, this.Name); // The ~1_name~ expired...

                Effects.SendLocationParticles(EffectItem.Create(parent.Location, parent.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(parent.Location, parent.Map, 0x201);
            }
            else
            {
                Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(this.Location, this.Map, 0x201);
            }

            this.StopTimer();
            this.Delete();
        }

        public override int BaseFireResistance { get { return 5; } }
        public override int BaseColdResistance { get { return 9; } }
        public override int BasePoisonResistance { get { return 5; } }
        public override int BaseEnergyResistance { get { return 5; } }
        public override int InitMinHits { get { return 20; } }
        public override int InitMaxHits { get { return 30; } }

        public WillemHartesHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)this.m_Lifespan);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            this.m_Lifespan = reader.ReadInt();

            this.StartTimer();
        }
    }
}
