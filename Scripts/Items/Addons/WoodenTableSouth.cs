using System;
using Server;

namespace Server.Items
{
    public class WoodenTableSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new WoodenTableSouthDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public WoodenTableSouthAddon()
        {
            AddComponent(new AddonComponent(0x4CC8), 0, 0, 0);
            AddComponent(new AddonComponent(0x4CC9), 1, 0, 0);
        }

        public WoodenTableSouthAddon(Serial serial)
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

    public class WoodenTableSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new WoodenTableSouthAddon(); } }
        public override int LabelNumber { get { return 1154156; } } // Wooden Table (South)

        [Constructable]
        public WoodenTableSouthDeed()
        {
        }

        public WoodenTableSouthDeed(Serial serial)
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