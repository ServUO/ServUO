namespace Server.Items
{
    public class BrownBearRugEastAddon : BaseAddon
    {
        [Constructable]
        public BrownBearRugEastAddon()
        {
            AddComponent(new AddonComponent(0x1E40), 1, 1, 0);
            AddComponent(new AddonComponent(0x1E41), 1, 0, 0);
            AddComponent(new AddonComponent(0x1E42), 1, -1, 0);
            AddComponent(new AddonComponent(0x1E43), 0, -1, 0);
            AddComponent(new AddonComponent(0x1E44), 0, 0, 0);
            AddComponent(new AddonComponent(0x1E45), 0, 1, 0);
            AddComponent(new AddonComponent(0x1E46), -1, 1, 0);
            AddComponent(new AddonComponent(0x1E47), -1, 0, 0);
            AddComponent(new AddonComponent(0x1E48), -1, -1, 0);
        }

        public BrownBearRugEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BrownBearRugEastDeed();
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

    public class BrownBearRugEastDeed : BaseAddonDeed
    {
        [Constructable]
        public BrownBearRugEastDeed()
        {
        }

        public BrownBearRugEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new BrownBearRugEastAddon();
        public override int LabelNumber => 1049397;// a brown bear rug deed facing east
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

    public class BrownBearRugSouthAddon : BaseAddon
    {
        [Constructable]
        public BrownBearRugSouthAddon()
        {
            AddComponent(new AddonComponent(0x1E36), 1, 1, 0);
            AddComponent(new AddonComponent(0x1E37), 0, 1, 0);
            AddComponent(new AddonComponent(0x1E38), -1, 1, 0);
            AddComponent(new AddonComponent(0x1E39), -1, 0, 0);
            AddComponent(new AddonComponent(0x1E3A), 0, 0, 0);
            AddComponent(new AddonComponent(0x1E3B), 1, 0, 0);
            AddComponent(new AddonComponent(0x1E3C), 1, -1, 0);
            AddComponent(new AddonComponent(0x1E3D), 0, -1, 0);
            AddComponent(new AddonComponent(0x1E3E), -1, -1, 0);
        }

        public BrownBearRugSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BrownBearRugSouthDeed();
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

    public class BrownBearRugSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public BrownBearRugSouthDeed()
        {
        }

        public BrownBearRugSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new BrownBearRugSouthAddon();
        public override int LabelNumber => 1049398;// a brown bear rug deed facing south
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

    public class PolarBearRugEastAddon : BaseAddon
    {
        [Constructable]
        public PolarBearRugEastAddon()
        {
            AddComponent(new AddonComponent(0x1E53), 1, 1, 0);
            AddComponent(new AddonComponent(0x1E54), 1, 0, 0);
            AddComponent(new AddonComponent(0x1E55), 1, -1, 0);
            AddComponent(new AddonComponent(0x1E56), 0, -1, 0);
            AddComponent(new AddonComponent(0x1E57), 0, 0, 0);
            AddComponent(new AddonComponent(0x1E58), 0, 1, 0);
            AddComponent(new AddonComponent(0x1E59), -1, 1, 0);
            AddComponent(new AddonComponent(0x1E5A), -1, 0, 0);
            AddComponent(new AddonComponent(0x1E5B), -1, -1, 0);
        }

        public PolarBearRugEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new PolarBearRugEastDeed();
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

    public class PolarBearRugEastDeed : BaseAddonDeed
    {
        [Constructable]
        public PolarBearRugEastDeed()
        {
        }

        public PolarBearRugEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new PolarBearRugEastAddon();
        public override int LabelNumber => 1049399;// a polar bear rug deed facing east
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

    public class PolarBearRugSouthAddon : BaseAddon
    {
        [Constructable]
        public PolarBearRugSouthAddon()
        {
            AddComponent(new AddonComponent(0x1E49), 1, 1, 0);
            AddComponent(new AddonComponent(0x1E4A), 0, 1, 0);
            AddComponent(new AddonComponent(0x1E4B), -1, 1, 0);
            AddComponent(new AddonComponent(0x1E4C), -1, 0, 0);
            AddComponent(new AddonComponent(0x1E4D), 0, 0, 0);
            AddComponent(new AddonComponent(0x1E4E), 1, 0, 0);
            AddComponent(new AddonComponent(0x1E4F), 1, -1, 0);
            AddComponent(new AddonComponent(0x1E50), 0, -1, 0);
            AddComponent(new AddonComponent(0x1E51), -1, -1, 0);
        }

        public PolarBearRugSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new PolarBearRugSouthDeed();
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

    public class PolarBearRugSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public PolarBearRugSouthDeed()
        {
        }

        public PolarBearRugSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new PolarBearRugSouthAddon();
        public override int LabelNumber => 1049400;// a polar bear rug deed facing south
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