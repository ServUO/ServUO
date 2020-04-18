using Server.Engines.Exodus;
using Server.Engines.PartySystem;
using Server.Mobiles;
using Server.Targeting;
using System.Linq;

namespace Server.Items
{
    public class ExodusSummoningAlter : BaseDecayingItem
    {
        public override int LabelNumber => 1153502;  // exodus summoning altar

        [Constructable]
        public ExodusSummoningAlter() : base(0x14F0)
        {
            LootType = LootType.Regular;
            Weight = 1;
        }

        public override int Lifespan => 604800;
        public override bool UseSeconds => false;

        public ExodusSummoningAlter(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1054107); // This item must be in your backpack.
            }
            else if (Party.Get(from) == null)
            {
                from.SendLocalizedMessage(1153596); // You must join a party with the players you wish to perform the ritual with. 
            }
            else
            {
                from.SendLocalizedMessage(1153675); // The Summoning Altar must be built upon a shrine, within Trammel or Felucca it matters not...                
                from.Target = new SummoningTarget(from, this);
            }
        }

        public class SummoningTarget : Target
        {
            private readonly Mobile m_Mobile;
            private readonly Item m_Deed;

            public SummoningTarget(Mobile from, Item deed) : base(2, true, TargetFlags.None)
            {
                m_Mobile = from;
                m_Deed = deed;
            }

            public static bool IsValidTile(int itemID)
            {
                return (itemID >= 0x149F && itemID <= 0x14D6);
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is StaticTarget)
                {
                    StaticTarget targ = (StaticTarget)targeted;

                    if (IsValidTile(targ.ItemID) && (from.Map == Map.Felucca || from.Map == Map.Trammel))
                    {
                        bool alter = from.Map.GetItemsInRange(targ.Location, 5).Where(x => x is ExodusTomeAltar).Any();

                        if (alter)
                        {
                            from.SendLocalizedMessage(1153590); // An altar has already been built here. 
                        }
                        else if (ExodusTomeAltar.Altar == null && VerLorRegController.Active && VerLorRegController.Mobile != null && CheckExodus())
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
                            else
                            {
                                from.SendLocalizedMessage(500269); // You cannot build that there.
                                return;
                            }

                            if (p != Point3D.Zero)
                            {
                                ExodusTomeAltar altar = new ExodusTomeAltar(from);
                                altar.MoveToWorld(p, from.Map);
                                altar.Owner = from;
                                m_Deed.Delete();
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(1075213); // The master of this realm has already been summoned and is engaged in combat.  Your opportunity will come after he has squashed the current batch of intruders!
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(500269); // You cannot build that there.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(500269); // You cannot build that there.
                }
            }
        }

        public static bool CheckExodus() // Before ritual check
        {
            return ClockworkExodus.Instances.FirstOrDefault(m => m.Region.IsPartOf("Ver Lor Reg") && ((m.Hits >= m.HitsMax * 0.60 && m.MinHits >= m.HitsMax * 0.60) || (m.Hits >= m.HitsMax * 0.75))) != null;
        }
    }
}