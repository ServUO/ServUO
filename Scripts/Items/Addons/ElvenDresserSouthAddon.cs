using System;

namespace Server.Items
{
    public class ElvenDresserAddonSouth : BaseAddonContainer
    {
        public override BaseAddonContainerDeed Deed => new ElvenDresserDeedSouth();

        public override int DefaultGumpID => 0x51;
        public override int DefaultDropSound => 0x42;
        public override bool RetainDeedHue => true;

        [Constructable]
        public ElvenDresserAddonSouth()
            : base(0x30E6)
        {
            AddComponent(new AddonContainerComponent(0x30E5), -1, 0, 0);
        }

        public ElvenDresserAddonSouth(Serial serial)
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

    public class ElvenDresserDeedSouth : BaseAddonContainerDeed
    {
        public override BaseAddonContainer Addon => new ElvenDresserAddonSouth();
        public override int LabelNumber => 1072864;

        [Constructable]
        public ElvenDresserDeedSouth()
        {
        }

        public ElvenDresserDeedSouth(Serial serial)
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

    public class ElvenDresserSouthAddon : BaseAddon
    {
        [Constructable]
        public ElvenDresserSouthAddon()
        {
            AddComponent(new AddonComponent(0x30E5), 0, 0, 0);
            AddComponent(new AddonComponent(0x30E6), 1, 0, 0);
        }

        public ElvenDresserSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ElvenDresserSouthDeed();
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

                ElvenDresserAddonSouth addon = new ElvenDresserAddonSouth();
                addon.MoveToWorld(new Point3D(p.X + 1, p.Y, p.Z), map);
                house.Addons[addon] = house.Owner;
            }
        }
    }

    public class ElvenDresserSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public ElvenDresserSouthDeed()
        {
        }

        public ElvenDresserSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ElvenDresserSouthAddon();
        public override int LabelNumber => 1072864;// elven dresser (south)
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
                ElvenDresserDeedSouth deed = new ElvenDresserDeedSouth();
                c.DropItem(deed);
            }
            else if (Parent == null)
            {
                Multis.BaseHouse house = Multis.BaseHouse.FindHouseAt(this);

                ElvenDresserDeedSouth deed = new ElvenDresserDeedSouth();
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
