using System;

namespace Server.Items
{
    public class StoneFireplaceEastAddon : BaseAddon
    {
        [Constructable]
        public StoneFireplaceEastAddon()
        {
            this.AddComponent(new AddonComponent(0x959), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x953), 0, 1, 0);
        }

        public StoneFireplaceEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new StoneFireplaceEastDeed();
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

    public class StoneFireplaceEastDeed : BaseAddonDeed
    {
        [Constructable]
        public StoneFireplaceEastDeed()
        {
        }

        public StoneFireplaceEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new StoneFireplaceEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1061848;
            }
        }// stone fireplace (east)
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