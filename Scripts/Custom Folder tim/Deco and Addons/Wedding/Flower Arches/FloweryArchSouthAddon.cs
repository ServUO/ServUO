using System;

namespace Server.Items
{
    public class FloweryArchSouthAddon : BaseAddon
    {
        [Constructable]
        public FloweryArchSouthAddon()
        {
            this.AddComponent(new AddonComponent(40600), 0, 0, 0);
            this.AddComponent(new AddonComponent(40601), 0, -1, 0);
			this.AddComponent(new AddonComponent(40602), 0, -2, 0);
            this.AddComponent(new AddonComponent(40603), 0, -3, 0);
        }

        public FloweryArchSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new FloweryArchSouthDeed();
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

    public class FloweryArchSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public FloweryArchSouthDeed()
        {
			Name = "Flowery Arch South Deed";
        }

        public FloweryArchSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FloweryArchSouthAddon();
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