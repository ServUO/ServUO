using System;

namespace Server.Items
{
    public class AlchemistTableEastAddon : BaseAddon
    {
        [Constructable]
        public AlchemistTableEastAddon()
        {
            AddComponent(new AddonComponent(0x3077), 0, 0, 0);
            AddComponent(new AddonComponent(0x3078), 0, -1, 0);
        }

        public AlchemistTableEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new AlchemistTableEastDeed();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class AlchemistTableEastDeed : BaseAddonDeed
    {
        [Constructable]
        public AlchemistTableEastDeed()
        {
        }

        public AlchemistTableEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new AlchemistTableEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073397;
            }
        }// alchemist table (east)
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