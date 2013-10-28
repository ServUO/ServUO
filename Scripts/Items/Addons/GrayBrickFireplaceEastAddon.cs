using System;

namespace Server.Items
{
    public class GrayBrickFireplaceEastAddon : BaseAddon
    {
        [Constructable]
        public GrayBrickFireplaceEastAddon()
        {
            this.AddComponent(new AddonComponent(0x93D), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x937), 0, 1, 0);
        }

        public GrayBrickFireplaceEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new GrayBrickFireplaceEastDeed();
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

    public class GrayBrickFireplaceEastDeed : BaseAddonDeed
    {
        [Constructable]
        public GrayBrickFireplaceEastDeed()
        {
        }

        public GrayBrickFireplaceEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new GrayBrickFireplaceEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1061846;
            }
        }// grey brick fireplace (east)
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