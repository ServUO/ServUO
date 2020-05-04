using Server.Gumps;
using Server.Items;
using Server.Regions;
using System.Linq;

namespace Server.Multis
{
    public abstract class BaseBoatDeed : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int MultiID { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction BoatDirection { get; set; }

        public BaseBoatDeed(int id, Point3D offset)
            : base(0x14F2)
        {
            Weight = 1.0;

            MultiID = id;
            Offset = offset;
            BoatDirection = Direction.North;
        }

        public BaseBoatDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(MultiID);
            writer.Write(Offset);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        MultiID = reader.ReadInt();
                        Offset = reader.ReadPoint3D();

                        break;
                    }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseBoat boat = BaseBoat.FindBoatAt(from, from.Map);

            if (from.AccessLevel < AccessLevel.GameMaster && (from.Map == Map.Ilshenar || from.Map == Map.Malas))
            {
                from.SendLocalizedMessage(1010567, null, 0x25); // You may not place a boat from this location.
            }
            else if (BaseBoat.HasBoat(from) && !Boat.IsRowBoat)
            {
                from.SendLocalizedMessage(1116758); // You already have a ship deployed!
            }
            else if (from.Region.IsPartOf(typeof(HouseRegion)) || boat != null && (boat.GetType() == Boat.GetType() || !boat.IsRowBoat && !(this is RowBoatDeed)))
            {
                from.SendLocalizedMessage(1010568, null, 0x25); // You may not place a ship while on another ship or inside a house.
            }
            else if (!from.HasGump(typeof(BoatPlacementGump)))
            {
                from.SendLocalizedMessage(502482); // Where do you wish to place the ship?

                from.SendGump(new BoatPlacementGump(this, from));
            }
        }

        public abstract BaseBoat Boat { get; }

        public void OnPlacement(Mobile from, Point3D p, int itemID, Direction d)
        {
            if (Deleted)
            {
                return;
            }
            else
            {
                Map map = from.Map;

                if (map == null)
                    return;

                if (from.AccessLevel < AccessLevel.GameMaster && (map == Map.Ilshenar || map == Map.Malas))
                {
                    from.SendLocalizedMessage(1043284); // A ship can not be created here.
                    return;
                }

                BaseBoat b = BaseBoat.FindBoatAt(from, from.Map);

                if (from.Region.IsPartOf(typeof(HouseRegion)) || b != null && (b.GetType() == Boat.GetType() || !b.IsRowBoat && !(this is RowBoatDeed)))
                {
                    from.SendLocalizedMessage(1010568, null, 0x25); // You may not place a ship while on another ship or inside a house.
                    return;
                }

                BoatDirection = d;
                BaseBoat boat = Boat;

                if (boat == null)
                    return;

                p = new Point3D(p.X - Offset.X, p.Y - Offset.Y, p.Z - Offset.Z);

                if (BaseBoat.IsValidLocation(p, map) && boat.CanFit(p, map, itemID))
                {
                    if (boat.IsRowBoat)
                    {
                        BaseBoat lastrowboat = World.Items.Values.OfType<BaseBoat>().Where(x => x.Owner == from && x.IsRowBoat && x.Map != Map.Internal && !x.MobilesOnBoard.Any()).OrderByDescending(y => y.Serial).FirstOrDefault();

                        if (lastrowboat != null)
                            lastrowboat.Delete();
                    }
                    else
                    {
                        Delete();
                    }

                    boat.Owner = from;
                    boat.ItemID = itemID;

                    if (boat is BaseGalleon)
                    {
                        ((BaseGalleon)boat).SecurityEntry = new SecurityEntry((BaseGalleon)boat);
                        ((BaseGalleon)boat).BaseBoatHue = RandomBasePaintHue();
                    }

                    if (boat.IsClassicBoat)
                    {
                        uint keyValue = boat.CreateKeys(from);

                        if (boat.PPlank != null)
                            boat.PPlank.KeyValue = keyValue;

                        if (boat.SPlank != null)
                            boat.SPlank.KeyValue = keyValue;
                    }

                    boat.MoveToWorld(p, map);
                    boat.OnAfterPlacement(true);

                    LighthouseAddon addon = LighthouseAddon.GetLighthouse(from);

                    if (addon != null)
                    {
                        if (boat.CanLinkToLighthouse)
                            from.SendLocalizedMessage(1154592); // You have linked your boat lighthouse.
                        else
                            from.SendLocalizedMessage(1154597); // Failed to link to lighthouse.
                    }
                }
                else
                {
                    boat.Delete();
                    from.SendLocalizedMessage(1043284); // A ship can not be created here.
                }
            }
        }

        private int RandomBasePaintHue()
        {
            if (0.6 > Utility.RandomDouble())
            {
                return Utility.RandomMinMax(1701, 1754);
            }

            return Utility.RandomMinMax(1801, 1908);
        }
    }
}
