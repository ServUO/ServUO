using System;

namespace Server.Items
{
    public class ElvenForgeAddon : BaseAddon
    {
        [Constructable]
        public ElvenForgeAddon()
        {
            AddComponent(new ForgeComponent(0x2DD8), 0, 0, 0);
        }

        public ElvenForgeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new ElvenForgeDeed();
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

    public class ElvenForgeDeed : BaseAddonDeed
    {
        [Constructable]
        public ElvenForgeDeed()
        {
        }

        public ElvenForgeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new ElvenForgeAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1072875;
            }
        }// squirrel statue (east)
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