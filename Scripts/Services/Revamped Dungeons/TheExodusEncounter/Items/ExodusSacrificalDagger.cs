using System;
using Server.Targeting;
using Server.Engines.PartySystem;
using System.Linq;
using Server.Mobiles;

namespace Server.Items
{
	public class ExodusSacrificalDagger : BaseKnife
    {
        public override int LabelNumber { get { return 1153500; } } // exodus sacrificial dagger
        private int m_Lifespan;
        private Timer m_Timer;

        [Constructable]
        public ExodusSacrificalDagger() : base(0x2D2D)
        {
            this.Weight = 4.0;
            this.Layer = Layer.OneHanded;
            this.Hue = 2500;

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

        public override void OnDoubleClick(Mobile from)
        {
            RobeofRite robe = from.FindItemOnLayer(Layer.OuterTorso) as RobeofRite;
            ExodusSacrificalDagger dagger = from.FindItemOnLayer(Layer.OneHanded) as ExodusSacrificalDagger;

            if (Party.Get(from) == null)
            {
                from.SendLocalizedMessage(1153596); // You must join a party with the players you wish to perform the ritual with. 
            }
            else if (robe == null || dagger == null)
            {
                from.SendLocalizedMessage(1153591); // Thou art not properly attired to perform such a ritual.
            }
            else if (!((PlayerMobile)from).UseSummoningRite)
            {
                from.SendLocalizedMessage(1153603); // You must first use the Summoning Rite on a Summoning Tome.
                return;
            }
            else
            {
                from.SendLocalizedMessage(1153604); // Target the summoning tome or yourself to declare your intentions for performing this ritual...
                from.Target = new SacrificalTarget(this);
            }
        }

        public class SacrificalTarget : Target
        {
            private Item m_Dagger;

            public SacrificalTarget(Item dagger) : base(2, true, TargetFlags.None)
            {
                m_Dagger = dagger;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is ExodusTomeAltar)
                {
                    ExodusTomeAltar altar = (ExodusTomeAltar)targeted;

                    if (altar.CheckParty(altar.Owner, from))
                    {  
                        bool SacrificalRitual = altar.Rituals.Find(s => s.RitualMobile == from).Ritual2;

                        if (!SacrificalRitual)
                        {
                            ((PlayerMobile)from).UseSummoningRite = false;
                            from.Say(1153605); // *You thrust the dagger into your flesh as tribute to Exodus!*
                            altar.Rituals.Find(s => s.RitualMobile == from).Ritual2 = true;
                            m_Dagger.Delete();
                            Misc.Titles.AwardKarma(from, 10000, true);
                            Effects.SendLocationParticles(EffectItem.Create(altar.Location, altar.Map, TimeSpan.FromSeconds(2)), 0x373A, 10, 10, 2023);

                            from.SendLocalizedMessage(1153598, from.Name); // ~1_PLAYER~ has read the Summoning Rite! 
                        }
                        else
                        {
                            from.SendLocalizedMessage(1153599); // You've already used this item in another ritual. 
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1153595); // You must first join the party of the person who built this altar.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1153601); // That is not a Summoning Tome. 
                }
            }
        }

        public ExodusSacrificalDagger(Serial serial) : base(serial)
        {
        }        

        public virtual int Lifespan { get { return 604800; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TimeLeft
        {
            get { return this.m_Lifespan; }
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
 