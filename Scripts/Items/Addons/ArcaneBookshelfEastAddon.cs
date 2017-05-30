using System;

namespace Server.Items
{
    public class ArcaneBookShelfAddonEast : BaseAddonContainer
    {
        public override BaseAddonContainerDeed Deed { get { return new ArcaneBookShelfDeedEast(); } }
        public override bool RetainDeedHue { get { return true; } }
        public override int DefaultGumpID { get { return 0x107; } }
        public override int DefaultDropSound { get { return 0x42; } }

        [Constructable]
        public ArcaneBookShelfAddonEast()
            : base(0x3086)
        {
            AddComponent(new AddonContainerComponent(0x3087), 0, -1, 0);
        }

        public ArcaneBookShelfAddonEast(Serial serial)
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

    public class ArcaneBookShelfDeedEast : BaseAddonContainerDeed
    {
        public override BaseAddonContainer Addon { get { return new ArcaneBookShelfAddonEast(); } }
        public override int LabelNumber { get { return 1073371; } } // arcane bookshelf (east)

        [Constructable]
        public ArcaneBookShelfDeedEast()
            : base()
        {
        }

        public ArcaneBookShelfDeedEast(Serial serial)
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

    public class ArcaneBookshelfEastAddon : BaseAddon
    {
        [Constructable]
        public ArcaneBookshelfEastAddon()
        {
            this.AddComponent(new AddonComponent(0x3084), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x3085), -1, 0, 0);
        }

        public ArcaneBookshelfEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new ArcaneBookshelfEastDeed();
            }
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

            Timer.DelayCall(TimeSpan.FromSeconds(10), Replace);
        }

        private void Replace()
        {
            Server.Multis.BaseHouse house = Server.Multis.BaseHouse.FindHouseAt(this);

            if (house != null)
            {
                Point3D p = this.Location;
                Map map = this.Map;

                house.Addons.Remove(this);
                Delete();

                var addon = new ArcaneBookShelfAddonSouth();
                addon.MoveToWorld(p, map);
                house.Addons[addon] = house.Owner;
            }
        }
    }

    public class ArcaneBookshelfEastDeed : BaseAddonDeed
    {
        [Constructable]
        public ArcaneBookshelfEastDeed()
        {
        }

        public ArcaneBookshelfEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new ArcaneBookshelfEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073371;
            }
        }// arcane bookshelf (east)
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
            Container c = this.Parent as Container;

            if (c != null)
            {
                var deed = new ArcaneBookShelfDeedEast();
                c.DropItem(deed);
            }
            else if (this.Parent == null)
            {
                Server.Multis.BaseHouse house = Server.Multis.BaseHouse.FindHouseAt(this);

                var deed = new ArcaneBookShelfDeedEast();
                deed.MoveToWorld(this.Location, this.Map);

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