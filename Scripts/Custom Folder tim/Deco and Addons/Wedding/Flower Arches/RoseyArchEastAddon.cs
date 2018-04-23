using System;

namespace Server.Items
{
    public class RoseyArchEastAddon : BaseAddon
    {
        [Constructable]
        public RoseyArchEastAddon()
        {
            this.AddComponent(new AddonComponent(40613), 0, 0, 0);
            this.AddComponent(new AddonComponent(40614), -1, 0, 0);
			this.AddComponent(new AddonComponent(40615), -2, 0, 0);
            this.AddComponent(new AddonComponent(40616), -3, 0, 0);
			this.AddComponent(new AddonComponent(40617), -4, 0, 0);
        }

        public RoseyArchEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new RoseyArchEastDeed();
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

    public class RoseyArchEastDeed : BaseAddonDeed
    {
        [Constructable]
        public RoseyArchEastDeed()
        {
			Name = "Rosey Arch East Deed";
        }

        public RoseyArchEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new RoseyArchEastAddon();
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