using System;
using Server;
using Server.Gumps;
using Server.Items;

namespace Server.Multis
{
    public abstract class BaseDockedBoat : Item
    {
        private string m_ShipName;

        #region High Seas
        private SecurityEntry m_SecurityEntry;
        private PilotEntry m_PilotEntry;
        private Mobile m_Owner;
        private int m_BaseHue;
        #endregion

        [CommandProperty(AccessLevel.GameMaster)]
        public int MultiID { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipName
        {
            get
            {
                if (BoatItem == null || BoatItem.ShipName == null || BoatItem.ShipName.Trim().Length == 0)
                    return "Unnamed Ship";

                return BoatItem.ShipName;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction BoatDirection { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat BoatItem { get; set; }

        public BaseDockedBoat(int id, Point3D offset, BaseBoat boat)
            : base(0x14F4)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;

            MultiID = id;
            Offset = offset;

            BoatDirection = Direction.North;
            BoatItem = boat;

            Hue = boat.Hue;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (BoatItem != null && !BoatItem.Deleted)
                BoatItem.Delete();
        }

        public BaseDockedBoat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)5); // version

            writer.Write(MultiID);
            writer.Write(Offset);
            writer.Write(m_ShipName);
            writer.Write(BoatItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 5:
                    {
                        MultiID = reader.ReadInt();
                        Offset = reader.ReadPoint3D();
                        m_ShipName = reader.ReadString();
                        BoatItem = reader.ReadItem() as BaseBoat;
                        break;
                    }
                case 4:
                    {
                        switch (reader.ReadInt())
                        {
                            default:
                            case 0: break;
                            case 1:
                                m_PilotEntry = new PilotEntry(reader);
                                break;
                        }
                        goto case 3;
                    }
                case 3:
                    {
                        m_BaseHue = reader.ReadInt();

                        if (version < 5)
                            reader.ReadItem();
                        reader.ReadItem();
                        m_Owner = reader.ReadMobile();
                        goto case 2;
                    }
                case 2:
                    {
                        switch (reader.ReadInt())
                        {
                            default:
                            case 0: break;
                            case 1:
                                m_SecurityEntry = new SecurityEntry(null, reader);
                                break;
                        }
                        goto case 1;
                    }
                case 1:
                case 0:
                    {
                        MultiID = reader.ReadInt();
                        Offset = reader.ReadPoint3D();
                        m_ShipName = reader.ReadString();

                        if (version == 0)
                            reader.ReadUInt();

                        break;
                    }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (Core.HS && BaseBoat.HasBoat(from))
            {
                from.SendLocalizedMessage(1116758); //You already have a ship deployed!
            }
            else if (!from.HasGump(typeof(BoatPlacementGump)))
            {
                from.SendLocalizedMessage(502482); // Where do you wish to place the ship?
                from.SendGump(new BoatPlacementGump(this, from));
            }
        }

        public abstract BaseBoat Boat { get; }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1041644, ShipName); //The ~1_VAL~ (Dry Docked)
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(LabelNumber);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (ShipName != null)
                LabelTo(from, ShipName);
            else
                base.OnSingleClick(from);
        }

        public void OnPlacement(Mobile from, Point3D p, int itemID, Direction d)
        {
            if (Deleted)
            {
                return;
            }
            else if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                Map map = from.Map;

                if (map == null)
                    return;

                BoatDirection = d;
                BaseBoat boat = BoatItem;

                if (boat == null || boat.Deleted)
                    boat = Boat;

                if (boat == null)
                    return;

                Mobile oldOwner = boat.Owner;

                boat.BoatItem = this;
                boat.Owner = from;

                if (oldOwner != from && boat is BaseGalleon)
                    ((BaseGalleon)boat).SecurityEntry = new SecurityEntry((BaseGalleon)boat);

                p = new Point3D(p.X - Offset.X, p.Y - Offset.Y, p.Z - Offset.Z);

                if (BaseBoat.IsValidLocation(p, map) && boat.CanFit(p, map, itemID) && map != Map.Ilshenar && map != Map.Malas)
                {
                    boat.SetFacing(d);
                    boat.MoveToWorld(p, map);
                    boat.OnPlacement(from);
                    boat.Refresh();

                    boat.OnAfterPlacement(false);

                    var addon = LighthouseAddon.GetLighthouse(from);

                    if (addon != null)
                    {
                        if (boat.CanLinkToLighthouse)
                            from.SendLocalizedMessage(1154592); // You have linked your boat lighthouse.
                        else
                            from.SendLocalizedMessage(1154597); // Failed to link to lighthouse.
                    }

                    if (boat.IsClassicBoat)
                    {
                        uint keyValue = boat.CreateKeys(from);

                        if (boat.PPlank != null)
                            boat.PPlank.KeyValue = keyValue;

                        if (boat.SPlank != null)
                            boat.SPlank.KeyValue = keyValue;
                    }

                    Internalize();
                }
                else
                {
                    from.SendLocalizedMessage(1043284); // A ship can not be created here.
                }
            }
        }
    }
}
