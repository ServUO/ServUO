namespace Server.Items
{
    public class WrongBedrollBase : Item
    {
        private Point3D m_PointDest;
        private BedrollSpawner m_Spawner;

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D PointDest
        {
            get { return m_PointDest; }
            set { m_PointDest = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BedrollSpawner BedrollSpawner
        {
            get { return m_Spawner; }
            set { m_Spawner = value; }
        }

        [Constructable]
        public WrongBedrollBase(int id)
            : base(id)
        {
            Movable = false;
        }

        public WrongBedrollBase(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            from.RevealingAction();

            if (PointDest != Point3D.Zero)
            {
                MysteriousTunnel mt = new MysteriousTunnel();
                Effects.PlaySound(from.Location, from.Map, 0x3BD);

                mt.PointDest = PointDest;
                mt.MoveToWorld(Location, Map);
                m_Spawner.MysteriousTunnels.Add(mt);
            }
            else
            {
                if (Utility.RandomDouble() < 0.5)
                {
                    Point3D loc = GetWorldLocation();
                    Map facet = Map;

                    SendMessageTo(from, 502999, 0x3B2); // You set off a trap!
                    AOS.Damage(from, 40, 0, 100, 0, 0, 0);

                    switch (Utility.Random(3))
                    {
                        case 0:
                            {
                                Effects.SendLocationEffect(loc, facet, 0x36BD, 15, 10);
                                Effects.PlaySound(loc, facet, 0x307);
                                break;
                            }
                        case 1:
                            {
                                Effects.PlaySound(loc, Map, 0x307);
                                Effects.SendLocationEffect(new Point3D(loc.X - 1, loc.Y, loc.Z), Map, 0x36BD, 15);
                                Effects.SendLocationEffect(new Point3D(loc.X + 1, loc.Y, loc.Z), Map, 0x36BD, 15);
                                Effects.SendLocationEffect(new Point3D(loc.X, loc.Y - 1, loc.Z), Map, 0x36BD, 15);
                                Effects.SendLocationEffect(new Point3D(loc.X, loc.Y + 1, loc.Z), Map, 0x36BD, 15);
                                Effects.SendLocationEffect(new Point3D(loc.X + 1, loc.Y + 1, loc.Z + 11), Map, 0x36BD, 15);
                                break;
                            }
                        case 2:
                            {
                                Effects.SendLocationEffect(loc, facet, 0x113A, 10, 20);
                                Effects.PlaySound(loc, facet, 0x231);
                                break;
                            }
                    }
                }
                else
                {
                    if (Utility.RandomDouble() < 0.01)
                    {
                        Item soap = new Soap();

                        Effects.PlaySound(from.Location, from.Map, 0x247);  //powder

                        if (Utility.RandomBool())
                        {
                            from.AddToBackpack(soap);
                            from.SendLocalizedMessage(1152268, string.Format("soap"));
                        }
                        else
                        {
                            soap.MoveToWorld(Location, Map);
                        }
                    }
                    else
                    {
                        Effects.PlaySound(from.Location, from.Map, 0x3E3);  //leather
                        from.SendLocalizedMessage(1152212); //You tear the bedroll to pieces but find nothing.
                    }
                }
            }

            Delete();
        }

        private void SendMessageTo(Mobile to, int number, int hue)
        {
            if (Deleted || !to.CanSee(this))
                return;

            to.Send(new Network.MessageLocalized(Serial, ItemID, Network.MessageType.Regular, hue, 3, number, "", ""));
        }

        private void SendMessageTo(Mobile to, string text, int hue)
        {
            if (Deleted || !to.CanSee(this))
                return;

            to.Send(new Network.UnicodeMessage(Serial, ItemID, Network.MessageType.Regular, hue, 3, "ENU", "", text));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Spawner);
            writer.Write(m_PointDest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Spawner = (BedrollSpawner)reader.ReadItem();
            m_PointDest = reader.ReadPoint3D();
        }
    }

    public class WrongBedrollSouth : WrongBedrollBase
    {
        public override int LabelNumber => 1022645;  // bedroll

        [Constructable]
        public WrongBedrollSouth()
            : base(0x0A56)
        {
        }

        public WrongBedrollSouth(Serial serial)
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
    }

    public class WrongBedrollEast : WrongBedrollBase
    {
        public override int LabelNumber => 1022645;  // bedroll

        [Constructable]
        public WrongBedrollEast()
            : base(0xA55)
        {
        }

        public WrongBedrollEast(Serial serial)
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
    }
}