using System;

namespace Server.Items
{
    public class SmallBedSouthAddon : BaseAddon
    {
        [Constructable]
        public SmallBedSouthAddon()
        {
            this.AddComponent(new AddonComponent(0xA63), 0, 0, 0);
            this.AddComponent(new AddonComponent(0xA5C), 0, 1, 0);
        }

        public SmallBedSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SmallBedSouthDeed();
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

    public class SmallBedSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public SmallBedSouthDeed()
        {
        }

        public SmallBedSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SmallBedSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044321;
            }
        }// small bed (south)
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