using System;

namespace Server.Items
{
    public class FloweryArchEastAddon : BaseAddon
    {
        [Constructable]
        public FloweryArchEastAddon()
        {
            this.AddComponent(new AddonComponent(40585), 0, 0, 0);
            this.AddComponent(new AddonComponent(40586), -1, 0, 0);
			this.AddComponent(new AddonComponent(40587), -2, 0, 0);
            this.AddComponent(new AddonComponent(40588), -3, 0, 0);
        }

        public FloweryArchEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new FloweryArchEastDeed();
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

    public class FloweryArchEastDeed : BaseAddonDeed
    {
        [Constructable]
        public FloweryArchEastDeed()
        {
			Name = "Flowery Arch East Deed";
        }

        public FloweryArchEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FloweryArchEastAddon();
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