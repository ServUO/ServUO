using System;

namespace Server.Items
{
    public class IvyArchEastAddon : BaseAddon
    {
        [Constructable]
        public IvyArchEastAddon()
        {
            this.AddComponent(new AddonComponent(40645), 0, 0, 0);
            this.AddComponent(new AddonComponent(40646), -1, 0, 0);
			this.AddComponent(new AddonComponent(40647), -2, 0, 0);
            this.AddComponent(new AddonComponent(40648), -3, 0, 0);
        }

        public IvyArchEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new IvyArchEastDeed();
            }
        }
		/*
        public int Quantity
        {
            get
            {
                return 500;
            }
            set
            {
            }
        }
		*/
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

    public class IvyArchEastDeed : BaseAddonDeed
    {
        [Constructable]
        public IvyArchEastDeed()
        {
			Name = "Ivy Arch East Deed";
        }

        public IvyArchEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new IvyArchEastAddon();
            }
        }
		/*
        public override int LabelNumber
        {
            get
            {
                return 1044350;
            }
        }// water trough (south)
		*/
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