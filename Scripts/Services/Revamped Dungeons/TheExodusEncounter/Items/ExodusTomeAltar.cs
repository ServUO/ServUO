using System;
using Server.Mobiles;
using Server.Targeting;
using System.Linq;
using Server.Engines.PartySystem;

namespace Server.Items
{
    public class ExodusTomeAltar : PeerlessExodusAltar
    {
        public static ExodusTomeAltar Altar { get; set; }

        private Item m_ExodusAlterAddon;

        [Constructable]
        public ExodusTomeAltar(Mobile from) : base(0x2259)
        {
            this.Hue = 1932;
            this.TeleportDest = new Point3D(764, 640, 0);       //Exodus city stairs

            this.m_ExodusAlterAddon = new ExodusAlterAddon();
            this.m_ExodusAlterAddon.Movable = false;
        }

        public override int LabelNumber { get { return 1153602; } } // Exodus Summoning Tome 

        public ExodusTomeAltar(Serial serial) : base(serial)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            return false;
        }        

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_ExodusAlterAddon != null)
                this.m_ExodusAlterAddon.Delete();

            if (Altar != null)
                Altar = null;
        }

        public override void OnMapChange()
        {
            if (this.Deleted)
                return;

            if (this.m_ExodusAlterAddon != null)
                this.m_ExodusAlterAddon.Map = this.Map;
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (this.Deleted)
                return;

            if (this.m_ExodusAlterAddon != null)
                this.m_ExodusAlterAddon.Location = new Point3D(this.X - 1, this.Y - 1, this.Z - 18);
        } 

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((Item)m_ExodusAlterAddon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_ExodusAlterAddon = reader.ReadItem();

                        break;
                    }
            }      
        }

        public static Target RitualTarget(Item item, Mobile from, RobeofRite robe)
        {            
            Target t = new InternalTarget(item, from, robe);
            from.Target = t;           
            
            return t;
        }        

        public class InternalTarget : Target
        {
            private Item m_Item;
            private RobeofRite m_Robe;
            private Mobile m_Mobile;
            private Mobile m_First_Ritual_Mobile;
            private int m_Ritual_Mobile;
            private bool m_Ritual1;
            private bool m_Ritual2;

            public InternalTarget(Item item, Mobile from, RobeofRite robe) : base(-1, true, TargetFlags.None)
            {
                m_Item = item;
                m_Mobile = from;
                m_Robe = robe;                
            }           

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is ExodusTomeAltar)
                {
                    ExodusTomeAltar altar = (ExodusTomeAltar)targeted;

                    m_First_Ritual_Mobile = altar.Rituals.First().RitualMobile;
                    m_Ritual_Mobile = altar.Rituals.Count(s => s.RitualMobile == m_Mobile);

                    if (altar.Rituals != null && altar.Rituals.Count != 0 && m_Mobile != null)
                    {
                        if (CheckParty(m_First_Ritual_Mobile, m_Mobile) || m_First_Ritual_Mobile == from)
                        {
                            if (m_First_Ritual_Mobile != m_Mobile && m_Ritual_Mobile == 0)
                                altar.Rituals.Add(new RitualArray { RitualMobile = m_Mobile, Ritual1 = false, Ritual2 = false });

                            m_Ritual1 = altar.Rituals.Find(s => s.RitualMobile == m_Mobile).Ritual1;
                            m_Ritual2 = altar.Rituals.Find(s => s.RitualMobile == m_Mobile).Ritual2;

                            if (m_Item is ExodusSummoningRite)
                            {
                                if (!m_Ritual1 && !m_Ritual2)
                                {
                                    from.Say(1153597); // You place the rite within the tome and begin to meditate...
                                    altar.Rituals.Find(s => s.RitualMobile == m_Mobile).Ritual1 = true;
                                    m_Item.Delete();

                                    if (m_Robe != null)
                                        m_Robe.CoolDown = TimeSpan.FromMinutes(14); // Do not give anyone                                   
                                }
                            }
                            else if (m_Item is ExodusSacrificalDagger)
                            {
                                if (m_Ritual1 && !m_Ritual2)
                                {
                                    from.Say(1153597); // You place the rite within the tome and begin to meditate...
                                    altar.Rituals.Find(s => s.RitualMobile == m_Mobile).Ritual2 = true;
                                    m_Item.Delete();
                                    Effects.SendLocationParticles(EffectItem.Create(((ExodusTomeAltar)targeted).Location, ((ExodusTomeAltar)targeted).Map, EffectItem.DefaultDuration), 0x373A, 10, 10, 2023);
                                }
                                else
                                {
                                    from.SendLocalizedMessage(1153603); // You must first use the Summoning Rite on a Summoning Tome. 
                                }
                            }
                        }
                        else
                            from.SendLocalizedMessage(1153596); // You must join a party with the players you wish to perform the ritual with.  
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1153601); // That is not a Summoning Tome. 
                }                      
            }

            protected virtual bool CheckParty(Mobile from, Mobile m)
            {
                Party party = Party.Get(from);

                if (party != null)
                {
                    foreach (PartyMemberInfo info in party.Members)
                    {
                        Mobile pm = info.Mobile;

                        if (pm.InRange(m.Location, 5))
                        {
                            if (pm == m)
                                return true;
                        }
                    }
                }
                return false;
            }
        }
    }

    public class RitualArray
    {
        public Mobile RitualMobile { get; set; }

        public bool Ritual1 { get; set; }

        public bool Ritual2 { get; set; }
    }
}