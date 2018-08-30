using System;

namespace Server.Items
{
    public class RoseyArchSouthAddon : BaseAddon
    {
        [Constructable]
        public RoseyArchSouthAddon()
        {
            this.AddComponent(new AddonComponent(40649), 0, 0, 0);
            this.AddComponent(new AddonComponent(40650), 0, -1, 0);
			this.AddComponent(new AddonComponent(40651), 0, -2, 0);
            this.AddComponent(new AddonComponent(40652), 0, -3, 0);
			this.AddComponent(new AddonComponent(40653), 0, -4, 0);
        }

        public RoseyArchSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new RoseyArchSouthDeed();
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

    public class RoseyArchSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public RoseyArchSouthDeed()
        {
			Name = "Rosey Arch South Deed";
        }

        public RoseyArchSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new RoseyArchSouthAddon();
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