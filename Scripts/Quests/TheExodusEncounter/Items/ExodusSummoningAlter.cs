using System;
using Server.Targeting;
using Server.Multis;
using Server.Mobiles;
using Server.Engines.PartySystem;

namespace Server.Items
{
    public class ExodusSummoningAlter : Item
    {
        private int m_Lifespan;
        private Timer m_Timer;

        [Constructable]
        public ExodusSummoningAlter() : base(0x14F0)
        {
            this.LootType = LootType.Regular;
            this.Weight = 1;

            if (this.Lifespan > 0)
            {
                this.m_Lifespan = this.Lifespan;
                this.StartTimer();
            }
        }

        public virtual int Lifespan { get { return 604800; } }

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

        public override int LabelNumber { get { return 1153502; } } // exodus summoning altar

        public ExodusSummoningAlter(Serial serial)
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
        
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                GetExodusAlter(from);
            }
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        protected virtual bool CheckParty(Mobile from)
        {
            Party party = Party.Get(from);

            if (party != null)
            {
                foreach (PartyMemberInfo info in party.Members)
                {
                    Mobile m = info.Mobile;

                    if (m.InRange(from.Location, 5))
                    {
                        if (m == from)
                            return true;
                    }
                }
            }
            return false;
        }


        protected virtual bool CheckExodus()
        {
            foreach (Mobile m in World.Mobiles.Values)
            {
                Region r = m.Region;

                if (r.IsPartOf("Ver Lor Reg"))
                    if (m is ClockworkExodus)
                        if (m.Hits < m.HitsMax * 0.75)
                            return false;
                        else
                            return true;
            }

            return true;
        }

        public void GetExodusAlter(Mobile from)
        {
            bool checktome = false;

            foreach (Item item in World.Items.Values)
            {
                if (item is ExodusTomeAltar)
                    checktome = true;
            }

            if (!checktome)
            {
                if (CheckParty(from))
                {
                    if (from.Region != null && (from.Map == Map.Trammel || from.Map == Map.Felucca))
                    {
                        ExodusTomeAltar alter;

                        if (CheckExodus())
                        {
                            if (from.Region.IsPartOf("Shrine of Compassion"))
                            {
                                alter = new ExodusTomeAltar(from);
                                alter.MoveToWorld(new Point3D(1858, 875, 12), from.Map);
                                PeerlessExodusAltar.m_Rituals.Add(new RitualArray { RitualMobile = from, Ritual1 = false, Ritual2 = false });
                                this.Delete();
                                return;
                            }
                            else if (from.Region.IsPartOf("Shrine of Honesty"))
                            {
                                alter = new ExodusTomeAltar(from);
                                alter.MoveToWorld(new Point3D(4209, 564, 60), from.Map);
                                PeerlessExodusAltar.m_Rituals.Add(new RitualArray { RitualMobile = from, Ritual1 = false, Ritual2 = false });
                                this.Delete();
                                return;
                            }
                            else if (from.Region.IsPartOf("Shrine of Honor"))
                            {
                                alter = new ExodusTomeAltar(from);
                                alter.MoveToWorld(new Point3D(1727, 3528, 15), from.Map);
                                PeerlessExodusAltar.m_Rituals.Add(new RitualArray { RitualMobile = from, Ritual1 = false, Ritual2 = false });
                                this.Delete();
                                return;
                            }
                            else if (from.Region.IsPartOf("Shrine of Humility"))
                            {
                                alter = new ExodusTomeAltar(from);
                                alter.MoveToWorld(new Point3D(4274, 3697, 12), from.Map);
                                PeerlessExodusAltar.m_Rituals.Add(new RitualArray { RitualMobile = from, Ritual1 = false, Ritual2 = false });
                                this.Delete();
                                return;
                            }
                            else if (from.Region.IsPartOf("Shrine of Justice"))
                            {
                                alter = new ExodusTomeAltar(from);
                                alter.MoveToWorld(new Point3D(1301, 634, 28), from.Map);
                                PeerlessExodusAltar.m_Rituals.Add(new RitualArray { RitualMobile = from, Ritual1 = false, Ritual2 = false });
                                this.Delete();
                                return;
                            }
                            else if (from.Region.IsPartOf("Shrine of Sacrifice"))
                            {
                                alter = new ExodusTomeAltar(from);
                                alter.MoveToWorld(new Point3D(3355, 290, 16), from.Map);
                                PeerlessExodusAltar.m_Rituals.Add(new RitualArray { RitualMobile = from, Ritual1 = false, Ritual2 = false });
                                this.Delete();
                                return;
                            }
                            else if (from.Region.IsPartOf("Shrine of Spirituality"))
                            {
                                alter = new ExodusTomeAltar(from);
                                alter.MoveToWorld(new Point3D(1606, 2490, 20), from.Map);
                                PeerlessExodusAltar.m_Rituals.Add(new RitualArray { RitualMobile = from, Ritual1 = false, Ritual2 = false });
                                this.Delete();
                                return;
                            }
                            else if (from.Region.IsPartOf("Shrine of Valor"))
                            {
                                alter = new ExodusTomeAltar(from);
                                alter.MoveToWorld(new Point3D(2492, 3931, 17), from.Map);
                                PeerlessExodusAltar.m_Rituals.Add(new RitualArray { RitualMobile = from, Ritual1 = false, Ritual2 = false });
                                this.Delete();
                                return;
                            }
                        }
                    }

                    from.SendMessage("That is not the right place to permorm thy ritual.");
                }
                else
                    from.SendLocalizedMessage(1153595); // You must first join the party of the person who built this altar. 
            }
            else
            {
                from.SendLocalizedMessage(1075213); // The master of this realm has already been summoned and is engaged in combat.  Your opportunity will come after he has squashed the current batch of intruders!
            }
        }
    }
}