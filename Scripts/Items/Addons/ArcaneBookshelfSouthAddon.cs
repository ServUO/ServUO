using System;

namespace Server.Items
{
    public class ArcaneBookShelfAddonSouth : BaseAddonContainer
    {
        public override BaseAddonContainerDeed Deed { get { return new ArcaneBookShelfDeedSouth(); } }
        public override bool RetainDeedHue { get { return true; } }
        public override int DefaultGumpID { get { return 0x107; } }
        public override int DefaultDropSound { get { return 0x42; } }

        [Constructable]
        public ArcaneBookShelfAddonSouth()
            : base(0x3084)
        {
            AddComponent(new AddonContainerComponent(0x3085), -1, 0, 0);
        }

        public ArcaneBookShelfAddonSouth(Serial serial)
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

    public class ArcaneBookShelfDeedSouth : BaseAddonContainerDeed
    {
        public override BaseAddonContainer Addon { get { return new ArcaneBookShelfAddonSouth(); } }
        public override int LabelNumber { get { return 1072871; } } // arcane bookshelf (south)

        [Constructable]
        public ArcaneBookShelfDeedSouth()
            : base()
        {
        }

        public ArcaneBookShelfDeedSouth(Serial serial)
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

    public class ArcaneBookshelfSouthAddon : BaseAddon
    {
        [Constructable]
        public ArcaneBookshelfSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x3087), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x3086), 0, 1, 0);
        }

        public ArcaneBookshelfSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new ArcaneBookshelfSouthDeed();
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

                var addon = new ArcaneBookShelfAddonEast();
                addon.MoveToWorld(new Point3D(p.X, p.Y + 1, p.Z), map);
                house.Addons[addon] = house.Owner;
            }
        }
    }

    public class ArcaneBookshelfSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public ArcaneBookshelfSouthDeed()
        {
        }

        public ArcaneBookshelfSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new ArcaneBookshelfSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1072871;
            }
        }// arcane bookshelf (south)
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
                var deed = new ArcaneBookShelfDeedSouth();
                c.DropItem(deed);
            }
            else if (this.Parent == null)
            {
                Server.Multis.BaseHouse house = Server.Multis.BaseHouse.FindHouseAt(this);

                var deed = new ArcaneBookShelfDeedSouth();
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