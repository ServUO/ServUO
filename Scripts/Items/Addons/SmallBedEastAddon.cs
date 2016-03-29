using System;

namespace Server.Items
{
    public class SmallBedEastAddon : BaseAddon
    {
        [Constructable]
        public SmallBedEastAddon()
        {
            this.AddComponent(new AddonComponent(0xA5D), 0, 0, 0);
            this.AddComponent(new AddonComponent(0xA62), 1, 0, 0);
        }

        public SmallBedEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SmallBedEastDeed();
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

    public class SmallBedEastDeed : BaseAddonDeed
    {
        [Constructable]
        public SmallBedEastDeed()
        {
        }

        public SmallBedEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SmallBedEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044322;
            }
        }// small bed (east)
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