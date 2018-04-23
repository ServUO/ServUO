using System;

namespace Server.Items
{
    public class IvyArchSouthAddon : BaseAddon
    {
        [Constructable]
        public IvyArchSouthAddon()
        {
            this.AddComponent(new AddonComponent(40666), 0, 0, 0);
            this.AddComponent(new AddonComponent(40665), 0, -1, 0);
			this.AddComponent(new AddonComponent(40664), 0, -2, 0);
            this.AddComponent(new AddonComponent(40663), 0, -3, 0);
        }

        public IvyArchSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new IvyArchSouthDeed();
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

    public class IvyArchSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public IvyArchSouthDeed()
        {
			Name = "Ivy Arch South Deed";
        }

        public IvyArchSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new IvyArchSouthAddon();
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