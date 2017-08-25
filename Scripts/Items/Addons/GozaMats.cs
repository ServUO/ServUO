using System;

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
            this.AddComponent(new LocalizedAddonComponent(0x28a4, 1030688), 1, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x28a5, 1030688), 0, 0, 0);
            this.Hue = hue;
        }

        public GozaMatEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new GozaMatEastDeed();
            }
        }
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GozaMatEastDeed : BaseAddonDeed
    {
        public override bool UseCraftResource { get { return false; } }

        [Constructable]
        public GozaMatEastDeed()
        {
        }

        public GozaMatEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new GozaMatEastAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1030404;
            }
        }// goza (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
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
            this.AddComponent(new LocalizedAddonComponent(0x28a6, 1030688), 0, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0x28a7, 1030688), 0, 0, 0);
            this.Hue = hue;
        }

        public GozaMatSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new GozaMatSouthDeed();
            }
        }
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GozaMatSouthDeed : BaseAddonDeed
    {
        public override bool UseCraftResource { get { return false; } }

        [Constructable]
        public GozaMatSouthDeed()
        {
        }

        public GozaMatSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new GozaMatSouthAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1030405;
            }
        }// goza (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
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
            this.AddComponent(new LocalizedAddonComponent(0x28a8, 1030688), 0, 0, 0);
            this.Hue = hue;
        }

        public SquareGozaMatEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SquareGozaMatEastDeed();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1030688;
            }
        }// goza mat
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SquareGozaMatEastDeed : BaseAddonDeed
    {
        public override bool UseCraftResource { get { return false; } }

        [Constructable]
        public SquareGozaMatEastDeed()
        {
        }

        public SquareGozaMatEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SquareGozaMatEastAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1030407;
            }
        }// square goza (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
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
            this.AddComponent(new LocalizedAddonComponent(0x28a9, 1030688), 0, 0, 0);
            this.Hue = hue;
        }

        public SquareGozaMatSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SquareGozaMatSouthDeed();
            }
        }
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SquareGozaMatSouthDeed : BaseAddonDeed
    {
        public override bool UseCraftResource { get { return false; } }

        [Constructable]
        public SquareGozaMatSouthDeed()
        {
        }

        public SquareGozaMatSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SquareGozaMatSouthAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1030406;
            }
        }// square goza (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
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
            this.AddComponent(new LocalizedAddonComponent(0x28AB, 1030688), 0, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x28AA, 1030688), 1, 0, 0);
            this.Hue = hue;
        }

        public BrocadeGozaMatEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new BrocadeGozaMatEastDeed();
            }
        }
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BrocadeGozaMatEastDeed : BaseAddonDeed
    {
        public override bool UseCraftResource { get { return false; } }

        [Constructable]
        public BrocadeGozaMatEastDeed()
        {
        }

        public BrocadeGozaMatEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new BrocadeGozaMatEastAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1030408;
            }
        }// brocade goza (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
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
            this.AddComponent(new LocalizedAddonComponent(0x28AD, 1030688), 0, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x28AC, 1030688), 0, 1, 0);
            this.Hue = hue;
        }

        public BrocadeGozaMatSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new BrocadeGozaMatSouthDeed();
            }
        }
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BrocadeGozaMatSouthDeed : BaseAddonDeed
    {
        public override bool UseCraftResource { get { return false; } }

        [Constructable]
        public BrocadeGozaMatSouthDeed()
        {
        }

        public BrocadeGozaMatSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new BrocadeGozaMatSouthAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1030409;
            }
        }// brocade goza (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
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
            this.AddComponent(new LocalizedAddonComponent(0x28AE, 1030688), 0, 0, 0);
            this.Hue = hue;
        }

        public BrocadeSquareGozaMatEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new BrocadeSquareGozaMatEastDeed();
            }
        }
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BrocadeSquareGozaMatEastDeed : BaseAddonDeed
    {
        public override bool UseCraftResource { get { return false; } }

        [Constructable]
        public BrocadeSquareGozaMatEastDeed()
        {
        }

        public BrocadeSquareGozaMatEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new BrocadeSquareGozaMatEastAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1030411;
            }
        }// brocade square goza (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
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
            this.AddComponent(new LocalizedAddonComponent(0x28AF, 1030688), 0, 0, 0);
            this.Hue = hue;
        }

        public BrocadeSquareGozaMatSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new BrocadeSquareGozaMatSouthDeed();
            }
        }
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BrocadeSquareGozaMatSouthDeed : BaseAddonDeed
    {
        public override bool UseCraftResource { get { return false; } }

        [Constructable]
        public BrocadeSquareGozaMatSouthDeed()
        {
        }

        public BrocadeSquareGozaMatSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new BrocadeSquareGozaMatSouthAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1030410;
            }
        }// brocade square goza (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}