using System;
using Server;

namespace Server.Items
{
    public class LongMetalTableSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LongMetalTableSouthDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LongMetalTableSouthAddon()
        {
            AddComponent(new AddonComponent(0x4CC1), -1, 1, 0);
            AddComponent(new AddonComponent(0x4CBD), -1, 0, 0);
            AddComponent(new AddonComponent(0x4CBE), 0, 0, 0);
            AddComponent(new AddonComponent(0x4CC0), 0, 1, 0);
            AddComponent(new AddonComponent(0x4CBC), 1, 0, 0);
            AddComponent(new AddonComponent(0x4CBF), 1, 1, 0);
        }

        public LongMetalTableSouthAddon(Serial serial)
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

    public class LongMetalTableSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LongMetalTableSouthAddon(); } }
        public override int LabelNumber { get { return 1154164; } } // Long Metal Table (South)

        [Constructable]
        public LongMetalTableSouthDeed()
        {
        }

        public LongMetalTableSouthDeed(Serial serial)
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