using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public class ExodusSacrificalDagger : BaseKnife
    {
        private int m_Lifespan;
        private Timer m_Timer;

        [Constructable]
        public ExodusSacrificalDagger() : base(0x2D2D)
        {
            this.Weight = 4.0;
            this.Layer = Layer.OneHanded;

            if (this.Lifespan > 0)
            {
                this.m_Lifespan = this.Lifespan;
                this.StartTimer();
            }
        }

        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }
        public override int AosStrengthReq { get { return 15; } }
        public override SkillName DefSkill { get { return SkillName.Fencing; } }
        public override float MlSpeed { get { return 2.00f; } }
        public override int AosMinDamage { get { return 10; } }
        public override int AosMaxDamage { get { return 12; } }
        public override int PhysicalResistance { get { return 12; } }

        public override void OnDoubleClick( Mobile from )
        {
            RobeofRite robe = from.FindItemOnLayer(Layer.OuterTorso) as RobeofRite;
            ExodusSacrificalDagger dagger = from.FindItemOnLayer(Layer.OneHanded) as ExodusSacrificalDagger;

            if (robe != null && dagger != null)
            {
                if (PeerlessExodusAltar.m_Rituals != null)
                {
                    if (PeerlessExodusAltar.m_Rituals.Count != 0)
                    {
                        from.SendLocalizedMessage(1153604); // Target the summoning tome or yourself to declare your intentions for performing this ritual...
                        ExodusTomeAltar.RitualTarget(this, from, null);                        
                    }
                }                
            }
            else
                base.OnDoubleClick(from);
        }

        public ExodusSacrificalDagger(Serial serial) : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1153500;
            }
        } // exodus sacrificial dagger

        public virtual int Lifespan
        {
            get
            {
                return 604800;
            }
        }

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

                if (weeks > 0)
                    list.Add(string.Format("Lifespan: {0} {1}", weeks, weeks == 1 ? "week" : "weeks"));
                else if (days > 0)
                    list.Add(string.Format("Lifespan: {0} {1}", days, days == 1 ? "day" : "days"));
                else if (hours > 0)
                    list.Add(string.Format("Lifespan: {0} {1}", hours, hours == 1 ? "hour" : "hours"));
                else if (minutes > 0)
                    list.Add(string.Format("Lifespan: {0} {1}", minutes, minutes == 1 ? "minute" : "minutes"));
                else
                    list.Add(1072517, this.m_Lifespan.ToString()); // Lifespan: ~1_val~ seconds
            }
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
 