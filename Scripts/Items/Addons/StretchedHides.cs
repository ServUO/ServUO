namespace Server.Items
{
    public class SmallStretchedHideEastAddon : BaseAddon
    {
        [Constructable]
        public SmallStretchedHideEastAddon()
        {
            AddComponent(new AddonComponent(0x1069), 0, 0, 0);
        }

        public SmallStretchedHideEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new SmallStretchedHideEastDeed();
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

    public class SmallStretchedHideEastDeed : BaseAddonDeed
    {
        [Constructable]
        public SmallStretchedHideEastDeed()
        {
        }

        public SmallStretchedHideEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new SmallStretchedHideEastAddon();
        public override int LabelNumber => 1049401;// a small stretched hide deed facing east
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

    public class SmallStretchedHideSouthAddon : BaseAddon
    {
        [Constructable]
        public SmallStretchedHideSouthAddon()
        {
            AddComponent(new AddonComponent(0x107A), 0, 0, 0);
        }

        public SmallStretchedHideSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new SmallStretchedHideSouthDeed();
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

    public class SmallStretchedHideSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public SmallStretchedHideSouthDeed()
        {
        }

        public SmallStretchedHideSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new SmallStretchedHideSouthAddon();
        public override int LabelNumber => 1049402;// a small stretched hide deed facing south
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

    public class MediumStretchedHideEastAddon : BaseAddon
    {
        [Constructable]
        public MediumStretchedHideEastAddon()
        {
            AddComponent(new AddonComponent(0x106B), 0, 0, 0);
        }

        public MediumStretchedHideEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new MediumStretchedHideEastDeed();
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

    public class MediumStretchedHideEastDeed : BaseAddonDeed
    {
        [Constructable]
        public MediumStretchedHideEastDeed()
        {
        }

        public MediumStretchedHideEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new MediumStretchedHideEastAddon();
        public override int LabelNumber => 1049403;// a medium stretched hide deed facing east
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

    public class MediumStretchedHideSouthAddon : BaseAddon
    {
        [Constructable]
        public MediumStretchedHideSouthAddon()
        {
            AddComponent(new AddonComponent(0x107C), 0, 0, 0);
        }

        public MediumStretchedHideSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new MediumStretchedHideSouthDeed();
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

    public class MediumStretchedHideSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public MediumStretchedHideSouthDeed()
        {
        }

        public MediumStretchedHideSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new MediumStretchedHideSouthAddon();
        public override int LabelNumber => 1049404;// a medium stretched hide deed facing south
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