using System;
using Server;

namespace Server.Items
{
    public class FancyWoodenShelfSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new FancyWoodenShelfSouthDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public FancyWoodenShelfSouthAddon()
        {
            AddComponent(new AddonComponent(0x4C38), 0, 0, 0);
        }

        public FancyWoodenShelfSouthAddon(Serial serial)
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

    public class FancyWoodenShelfSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new FancyWoodenShelfSouthAddon(); } }
        public override int LabelNumber { get { return 1154158; } } // Fancy Wooden Shelf (South)

        [Constructable]
        public FancyWoodenShelfSouthDeed()
        {
        }

        public FancyWoodenShelfSouthDeed(Serial serial)
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