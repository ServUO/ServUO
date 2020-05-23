namespace Server.Items
{
    public class GozaMatEastAddon : BaseAddon
    {
        [Constructable]
        public GozaMatEastAddon()
            : this(0)
        {
        }

        [Constructable]
        public GozaMatEastAddon(int hue)
        {
            AddComponent(new LocalizedAddonComponent(0x28a4, 1030688), 1, 0, 0);
            AddComponent(new LocalizedAddonComponent(0x28a5, 1030688), 0, 0, 0);
            Hue = hue;
        }

        public GozaMatEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new GozaMatEastDeed();
        public override bool RetainDeedHue => true;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class GozaMatEastDeed : BaseAddonDeed
    {
        public override bool UseCraftResource => false;

        [Constructable]
        public GozaMatEastDeed()
        {
        }

        public GozaMatEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new GozaMatEastAddon(Hue);
        public override int LabelNumber => 1030404;// goza (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class GozaMatSouthAddon : BaseAddon
    {
        [Constructable]
        public GozaMatSouthAddon()
            : this(0)
        {
        }

        [Constructable]
        public GozaMatSouthAddon(int hue)
        {
            AddComponent(new LocalizedAddonComponent(0x28a6, 1030688), 0, 1, 0);
            AddComponent(new LocalizedAddonComponent(0x28a7, 1030688), 0, 0, 0);
            Hue = hue;
        }

        public GozaMatSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new GozaMatSouthDeed();
        public override bool RetainDeedHue => true;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class GozaMatSouthDeed : BaseAddonDeed
    {
        public override bool UseCraftResource => false;

        [Constructable]
        public GozaMatSouthDeed()
        {
        }

        public GozaMatSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new GozaMatSouthAddon(Hue);
        public override int LabelNumber => 1030405;// goza (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class SquareGozaMatEastAddon : BaseAddon
    {
        [Constructable]
        public SquareGozaMatEastAddon()
            : this(0)
        {
        }

        [Constructable]
        public SquareGozaMatEastAddon(int hue)
        {
            AddComponent(new LocalizedAddonComponent(0x28a8, 1030688), 0, 0, 0);
            Hue = hue;
        }

        public SquareGozaMatEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new SquareGozaMatEastDeed();
        public override int LabelNumber => 1030688;// goza mat
        public override bool RetainDeedHue => true;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class SquareGozaMatEastDeed : BaseAddonDeed
    {
        public override bool UseCraftResource => false;

        [Constructable]
        public SquareGozaMatEastDeed()
        {
        }

        public SquareGozaMatEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new SquareGozaMatEastAddon(Hue);
        public override int LabelNumber => 1030407;// square goza (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class SquareGozaMatSouthAddon : BaseAddon
    {
        [Constructable]
        public SquareGozaMatSouthAddon()
            : this(0)
        {
        }

        [Constructable]
        public SquareGozaMatSouthAddon(int hue)
        {
            AddComponent(new LocalizedAddonComponent(0x28a9, 1030688), 0, 0, 0);
            Hue = hue;
        }

        public SquareGozaMatSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new SquareGozaMatSouthDeed();
        public override bool RetainDeedHue => true;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class SquareGozaMatSouthDeed : BaseAddonDeed
    {
        public override bool UseCraftResource => false;

        [Constructable]
        public SquareGozaMatSouthDeed()
        {
        }

        public SquareGozaMatSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new SquareGozaMatSouthAddon(Hue);
        public override int LabelNumber => 1030406;// square goza (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class BrocadeGozaMatEastAddon : BaseAddon
    {
        [Constructable]
        public BrocadeGozaMatEastAddon()
            : this(0)
        {
        }

        [Constructable]
        public BrocadeGozaMatEastAddon(int hue)
        {
            AddComponent(new LocalizedAddonComponent(0x28AB, 1030688), 0, 0, 0);
            AddComponent(new LocalizedAddonComponent(0x28AA, 1030688), 1, 0, 0);
            Hue = hue;
        }

        public BrocadeGozaMatEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BrocadeGozaMatEastDeed();
        public override bool RetainDeedHue => true;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class BrocadeGozaMatEastDeed : BaseAddonDeed
    {
        public override bool UseCraftResource => false;

        [Constructable]
        public BrocadeGozaMatEastDeed()
        {
        }

        public BrocadeGozaMatEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new BrocadeGozaMatEastAddon(Hue);
        public override int LabelNumber => 1030408;// brocade goza (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class BrocadeGozaMatSouthAddon : BaseAddon
    {
        [Constructable]
        public BrocadeGozaMatSouthAddon()
            : this(0)
        {
        }

        [Constructable]
        public BrocadeGozaMatSouthAddon(int hue)
        {
            AddComponent(new LocalizedAddonComponent(0x28AD, 1030688), 0, 0, 0);
            AddComponent(new LocalizedAddonComponent(0x28AC, 1030688), 0, 1, 0);
            Hue = hue;
        }

        public BrocadeGozaMatSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BrocadeGozaMatSouthDeed();
        public override bool RetainDeedHue => true;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class BrocadeGozaMatSouthDeed : BaseAddonDeed
    {
        public override bool UseCraftResource => false;

        [Constructable]
        public BrocadeGozaMatSouthDeed()
        {
        }

        public BrocadeGozaMatSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new BrocadeGozaMatSouthAddon(Hue);
        public override int LabelNumber => 1030409;// brocade goza (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class BrocadeSquareGozaMatEastAddon : BaseAddon
    {
        [Constructable]
        public BrocadeSquareGozaMatEastAddon()
            : this(0)
        {
        }

        [Constructable]
        public BrocadeSquareGozaMatEastAddon(int hue)
        {
            AddComponent(new LocalizedAddonComponent(0x28AE, 1030688), 0, 0, 0);
            Hue = hue;
        }

        public BrocadeSquareGozaMatEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BrocadeSquareGozaMatEastDeed();
        public override bool RetainDeedHue => true;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class BrocadeSquareGozaMatEastDeed : BaseAddonDeed
    {
        public override bool UseCraftResource => false;

        [Constructable]
        public BrocadeSquareGozaMatEastDeed()
        {
        }

        public BrocadeSquareGozaMatEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new BrocadeSquareGozaMatEastAddon(Hue);
        public override int LabelNumber => 1030411;// brocade square goza (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class BrocadeSquareGozaMatSouthAddon : BaseAddon
    {
        [Constructable]
        public BrocadeSquareGozaMatSouthAddon()
            : this(0)
        {
        }

        [Constructable]
        public BrocadeSquareGozaMatSouthAddon(int hue)
        {
            AddComponent(new LocalizedAddonComponent(0x28AF, 1030688), 0, 0, 0);
            Hue = hue;
        }

        public BrocadeSquareGozaMatSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BrocadeSquareGozaMatSouthDeed();
        public override bool RetainDeedHue => true;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class BrocadeSquareGozaMatSouthDeed : BaseAddonDeed
    {
        public override bool UseCraftResource => false;

        [Constructable]
        public BrocadeSquareGozaMatSouthDeed()
        {
        }

        public BrocadeSquareGozaMatSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new BrocadeSquareGozaMatSouthAddon(Hue);
        public override int LabelNumber => 1030410;// brocade square goza (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class SquareGozaMatAddon : BaseAddon
    {
        [Constructable]
        public SquareGozaMatAddon()
            : this(0)
        {
        }

        [Constructable]
        public SquareGozaMatAddon(int hue)
        {
            AddComponent(new LocalizedAddonComponent(0x2A3C, 1113621), 0, 0, 0);
            Hue = hue;
        }

        public SquareGozaMatAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new SquareGozaMatDeed();
        public override bool RetainDeedHue => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class SquareGozaMatDeed : BaseAddonDeed
    {
        public override bool UseCraftResource => false;

        [Constructable]
        public SquareGozaMatDeed()
        {
        }

        public SquareGozaMatDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new SquareGozaMatAddon(Hue);
        public override int LabelNumber => 1030410;// brocade square goza (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}
