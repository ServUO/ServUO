using System;

namespace Server.Items
{
    public class DaviesLockerEastAddon : BaseAddon
    {
        [Constructable]
        public DaviesLockerEastAddon()
            : this(0)
        {
        }

        [Constructable]
        public DaviesLockerEastAddon(int hue)
        {
            this.AddComponent(new LocalizedAddonComponent(0x4BF8, 1153534), 1, -1, 0);
            this.AddComponent(new LocalizedAddonComponent(0x4BF7, 1153534), 1, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x4BF6, 1153534), 1, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0x4BF9, 1153534), 0, -1, 0);
            this.AddComponent(new LocalizedAddonComponent(0x4BFA, 1153534), 0, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x4BFB, 1153534), 0, 1, 0);

            this.Hue = hue;
        }

        public DaviesLockerEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new DaviesLockerEastDeed();
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

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DaviesLockerEastDeed : BaseAddonDeed
    {
        [Constructable]
        public DaviesLockerEastDeed()
        {
        }

        public DaviesLockerEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new DaviesLockerEastAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1153534;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DaviesLockerSouthAddon : BaseAddon
    {
        [Constructable]
        public DaviesLockerSouthAddon()
            : this(0)
        {
        }

        [Constructable]
        public DaviesLockerSouthAddon(int hue)
        {
            this.AddComponent(new LocalizedAddonComponent(0x4BFC, 1153534), 0, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0x4BFD, 1153534), -1, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0x4BFE, 1153534), 1, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0x4BFF, 1153534), -1, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x4C00, 1153534), 0, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x4C01, 1153534), 1, 0, 0);
            this.Hue = hue;
        }

        public DaviesLockerSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new DaviesLockerSouthDeed();
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

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DaviesLockerSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public DaviesLockerSouthDeed()
        {
        }

        public DaviesLockerSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new DaviesLockerSouthAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1153534;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}

 