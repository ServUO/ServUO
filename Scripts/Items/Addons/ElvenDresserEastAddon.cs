using System;

namespace Server.Items
{
    public class ElvenDresserAddonEast : BaseAddonContainer
    {
        public override BaseAddonContainerDeed Deed => new ElvenDresserDeedEast();

        public override int DefaultGumpID => 0x51;
        public override int DefaultDropSound => 0x42;
        public override bool RetainDeedHue => true;

        [Constructable]
        public ElvenDresserAddonEast() : base(0x30E4)
        {
            AddComponent(new AddonContainerComponent(0x30E3), 0, -1, 0);
        }

        public ElvenDresserAddonEast(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class ElvenDresserDeedEast : BaseAddonContainerDeed
    {
        public override BaseAddonContainer Addon => new ElvenDresserAddonEast();
        public override int LabelNumber => 1073388;

        [Constructable]
        public ElvenDresserDeedEast()
        {
        }

        public ElvenDresserDeedEast(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class ElvenDresserEastAddon : BaseAddon
    {
        [Constructable]
        public ElvenDresserEastAddon()
        {
            AddComponent(new AddonComponent(0x30E4), 0, 0, 0);
            AddComponent(new AddonComponent(0x30E3), 0, -1, 0);
        }

        public ElvenDresserEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ElvenDresserEastDeed();
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            Timer.DelayCall(TimeSpan.FromSeconds(10), Replace);
        }

        private void Replace()
        {
            Multis.BaseHouse house = Multis.BaseHouse.FindHouseAt(this);

            if (house != null)
            {
                Point3D p = Location;
                Map map = Map;

                house.Addons.Remove(this);
                Delete();

                ElvenDresserAddonEast addon = new ElvenDresserAddonEast();
                addon.MoveToWorld(p, map);
                house.Addons[addon] = house.Owner;
            }
        }
    }

    public class ElvenDresserEastDeed : BaseAddonDeed
    {
        [Constructable]
        public ElvenDresserEastDeed()
        {
        }

        public ElvenDresserEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ElvenDresserEastAddon();
        public override int LabelNumber => 1073388;// elven dresser (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            Timer.DelayCall(TimeSpan.FromSeconds(10), Replace);
        }

        private void Replace()
        {
            Container c = Parent as Container;

            if (c != null)
            {
                ElvenDresserDeedEast deed = new ElvenDresserDeedEast();
                c.DropItem(deed);
            }
            else if (Parent == null)
            {
                Multis.BaseHouse house = Multis.BaseHouse.FindHouseAt(this);

                ElvenDresserDeedEast deed = new ElvenDresserDeedEast();
                deed.MoveToWorld(Location, Map);

                deed.IsLockedDown = IsLockedDown;
                deed.IsSecure = IsSecure;
                deed.Movable = Movable;

                if (house != null && house.LockDowns.ContainsKey(this))
                {
                    house.LockDowns.Remove(this);
                    house.LockDowns.Add(deed, house.Owner);
                }
                else if (house != null && house.IsSecure(this))
                {
                    house.ReleaseSecure(house.Owner, this);
                    house.AddSecure(house.Owner, deed);
                }
            }

            Delete();
        }
    }
}
