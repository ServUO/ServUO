using System;

namespace Server.Items
{
    public class WeddingTableLongEastAddon : BaseAddon
    {
        [Constructable]
        public WeddingTableLongEastAddon()
        {
            this.AddComponent(new AddonComponent(40657), 0, 0, 0);
            this.AddComponent(new AddonComponent(40658), 0, -1, 0);
			this.AddComponent(new AddonComponent(40658), 0, -2, 0);
            this.AddComponent(new AddonComponent(40659), 0, -3, 0);
        }

        public WeddingTableLongEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new WeddingTableLongEastDeed();
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

    public class WeddingTableLongEastDeed : BaseAddonDeed
    {
        [Constructable]
        public WeddingTableLongEastDeed()
        {
			Name = "Wedding Table Long East Deed";
        }

        public WeddingTableLongEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new WeddingTableLongEastAddon();
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
	
	public class WeddingTableEastAddon : BaseAddon
    {
        [Constructable]
        public WeddingTableEastAddon()
        {
            this.AddComponent(new AddonComponent(40657), 0, 0, 0);
            this.AddComponent(new AddonComponent(40658), 0, -1, 0);
			this.AddComponent(new AddonComponent(40659), 0, -2, 0);
        }

        public WeddingTableEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new WeddingTableEastDeed();
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

    public class WeddingTableEastDeed : BaseAddonDeed
    {
        [Constructable]
        public WeddingTableEastDeed()
        {
			Name = "Wedding Table East Deed";
        }

        public WeddingTableEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new WeddingTableEastAddon();
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