using System;
using Server.Targeting;
using Server.Multis;
using Server.Mobiles;
using Server.Engines.PartySystem;
using System.Linq;
using Server.Engines.Exodus;

namespace Server.Items
{
    public class ExodusSummoningAlter : BaseDecayingItem
    {
        [Constructable]
        public ExodusSummoningAlter() : base(0x14F0)
        {
            this.LootType = LootType.Regular;
            this.Weight = 1;
        }

        public override int Lifespan { get { return 604800; } }
        public override bool UseSeconds { get { return false; } }

        public override int LabelNumber { get { return 1153502; } } // exodus summoning altar

        public ExodusSummoningAlter(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
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


        public static bool CheckExodus() // Before ritual check
        {
            return ClockworkExodus.Instances.FirstOrDefault(m => m.Region.IsPartOf("Ver Lor Reg") && ((m.Hits >= m.HitsMax * 0.60 && m.MinHits >= m.HitsMax * 0.60) || (m.Hits >= m.HitsMax * 0.75))) != null;
        }

        public void GetExodusAlter(Mobile from)
        {
            if (ExodusTomeAltar.Altar == null && VerLorRegController.Active && VerLorRegController.Mobile != null && CheckExodus())
            {
                if (CheckParty(from))
                {
                    if (from.Region != null && (from.Map == Map.Trammel || from.Map == Map.Felucca))
                    {
                        Point3D p = Point3D.Zero;

                        if (from.Region.IsPartOf("Shrine of Compassion"))
                        {
                            p = new Point3D(1858, 875, 12);
                        }
                        else if (from.Region.IsPartOf("Shrine of Honesty"))
                        {
                            p = new Point3D(4209, 564, 60);
                        }
                        else if (from.Region.IsPartOf("Shrine of Honor"))
                        {
                            p = new Point3D(1727, 3528, 15);
                        }
                        else if (from.Region.IsPartOf("Shrine of Humility"))
                        {
                            p = new Point3D(4274, 3697, 12);
                        }
                        else if (from.Region.IsPartOf("Shrine of Justice"))
                        {
                            p = new Point3D(1301, 634, 28);
                        }
                        else if (from.Region.IsPartOf("Shrine of Sacrifice"))
                        {
                            p = new Point3D(3355, 290, 16);
                        }
                        else if (from.Region.IsPartOf("Shrine of Spirituality"))
                        {
                            p = new Point3D(1606, 2490, 20);
                        }
                        else if (from.Region.IsPartOf("Shrine of Valor"))
                        {
                            p = new Point3D(2492, 3931, 17);
                        }

                        if (p != Point3D.Zero)
                        {
                            ExodusTomeAltar.Altar = new ExodusTomeAltar(from);
                            ExodusTomeAltar.Altar.MoveToWorld(p, from.Map);
                            PeerlessExodusAltar.m_Rituals.Add(new RitualArray { RitualMobile = from, Ritual1 = false, Ritual2 = false });
                            this.Delete();
                        }                        
                    }
                    else
                        from.SendMessage("That is not the right place to perform thy ritual.");
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