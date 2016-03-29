using System;

namespace Server.Items
{
    public class LightFlowerTapestryEastAddon : BaseAddon
    {
        [Constructable]
        public LightFlowerTapestryEastAddon()
        {
            this.AddComponent(new AddonComponent(0xFDC), 0, 0, 0);
            this.AddComponent(new AddonComponent(0xFDB), 0, 1, 0);
        }

        public LightFlowerTapestryEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new LightFlowerTapestryEastDeed();
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

    public class LightFlowerTapestryEastDeed : BaseAddonDeed
    {
        [Constructable]
        public LightFlowerTapestryEastDeed()
        {
        }

        public LightFlowerTapestryEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new LightFlowerTapestryEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1049393;
            }
        }// a flower tapestry deed facing east
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

    public class LightFlowerTapestrySouthAddon : BaseAddon
    {
        [Constructable]
        public LightFlowerTapestrySouthAddon()
        {
            this.AddComponent(new AddonComponent(0xFD9), 0, 0, 0);
            this.AddComponent(new AddonComponent(0xFDA), 1, 0, 0);
        }

        public LightFlowerTapestrySouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new LightFlowerTapestrySouthDeed();
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

    public class LightFlowerTapestrySouthDeed : BaseAddonDeed
    {
        [Constructable]
        public LightFlowerTapestrySouthDeed()
        {
        }

        public LightFlowerTapestrySouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new LightFlowerTapestrySouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1049394;
            }
        }// a flower tapestry deed facing south
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

    public class DarkFlowerTapestryEastAddon : BaseAddon
    {
        [Constructable]
        public DarkFlowerTapestryEastAddon()
        {
            this.AddComponent(new AddonComponent(0xFE0), 0, 0, 0);
            this.AddComponent(new AddonComponent(0xFDF), 0, 1, 0);
        }

        public DarkFlowerTapestryEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new DarkFlowerTapestryEastDeed();
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

    public class DarkFlowerTapestryEastDeed : BaseAddonDeed
    {
        [Constructable]
        public DarkFlowerTapestryEastDeed()
        {
        }

        public DarkFlowerTapestryEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new DarkFlowerTapestryEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1049395;
            }
        }// a dark flower tapestry deed facing east
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

    public class DarkFlowerTapestrySouthAddon : BaseAddon
    {
        [Constructable]
        public DarkFlowerTapestrySouthAddon()
        {
            this.AddComponent(new AddonComponent(0xFDD), 0, 0, 0);
            this.AddComponent(new AddonComponent(0xFDE), 1, 0, 0);
        }

        public DarkFlowerTapestrySouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new DarkFlowerTapestrySouthDeed();
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

    public class DarkFlowerTapestrySouthDeed : BaseAddonDeed
    {
        [Constructable]
        public DarkFlowerTapestrySouthDeed()
        {
        }

        public DarkFlowerTapestrySouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new DarkFlowerTapestrySouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1049396;
            }
        }// a dark flower tapestry deed facing south
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