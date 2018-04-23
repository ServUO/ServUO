using System;

namespace Server.Items
{
    public class WeddingTableLongSouthAddon : BaseAddon
    {
        [Constructable]
        public WeddingTableLongSouthAddon()
        {
            this.AddComponent(new AddonComponent(40654), 0, 0, 0);
            this.AddComponent(new AddonComponent(40655), -1, 0, 0);
			this.AddComponent(new AddonComponent(40655), -2, 0, 0);
            this.AddComponent(new AddonComponent(40656), -3, 0, 0);
        }

        public WeddingTableLongSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new WeddingTableLongSouthDeed();
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

    public class WeddingTableLongSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public WeddingTableLongSouthDeed()
        {
			Name = "Wedding Table Long South Deed";
        }

        public WeddingTableLongSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new WeddingTableLongSouthAddon();
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
	
	public class WeddingTableSouthAddon : BaseAddon
    {
        [Constructable]
        public WeddingTableSouthAddon()
        {
            this.AddComponent(new AddonComponent(40654), 0, 0, 0);
            this.AddComponent(new AddonComponent(40655), -1, 0, 0);
			this.AddComponent(new AddonComponent(40656), -2, 0, 0);
        }

        public WeddingTableSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new WeddingTableSouthDeed();
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

    public class WeddingTableSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public WeddingTableSouthDeed()
        {
			Name = "Wedding Table South Deed";
        }

        public WeddingTableSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new WeddingTableSouthAddon();
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
}