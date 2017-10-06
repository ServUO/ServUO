using System;
using Server;

namespace Server.Items
{
    public class FancyWoodenShelfEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new FancyWoodenShelfEastDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public FancyWoodenShelfEastAddon()
        {
            AddComponent(new AddonComponent(0x4C39), 0, 0, 0);
        }

        public FancyWoodenShelfEastAddon(Serial serial)
            : base(serial)
        {
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

    public class FancyWoodenShelfEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new FancyWoodenShelfEastAddon(); } }
        public override int LabelNumber { get { return 1154159; } } // Fancy Wooden Shelf (East)

        [Constructable]
        public FancyWoodenShelfEastDeed()
        {
        }

        public FancyWoodenShelfEastDeed(Serial serial)
            : base(serial)
        {
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