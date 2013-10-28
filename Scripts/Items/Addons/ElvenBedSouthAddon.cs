using System;

namespace Server.Items
{
    public class ElvenBedSouthAddon : BaseAddon
    {
        [Constructable]
        public ElvenBedSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x3050), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x3051), 0, -1, 0);
        }

        public ElvenBedSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new ElvenBedSouthDeed();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class ElvenBedSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public ElvenBedSouthDeed()
        {
        }

        public ElvenBedSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new ElvenBedSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1072860;
            }
        }// elven bed (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}