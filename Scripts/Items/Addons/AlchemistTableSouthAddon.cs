using System;

namespace Server.Items
{
    public class AlchemistTableSouthAddon : BaseAddon
    {
        [Constructable]
        public AlchemistTableSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x2DD4), 0, 0, 0);
        }

        public AlchemistTableSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new AlchemistTableSouthDeed();
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

    public class AlchemistTableSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public AlchemistTableSouthDeed()
        {
        }

        public AlchemistTableSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new AlchemistTableSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073396;
            }
        }// alchemist table (south)
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