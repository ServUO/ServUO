using System;
using Server;

namespace Server.Items
{
    public class LongMetalTableEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LongMetalTableEastDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LongMetalTableEastAddon()
        {
            AddComponent(new AddonComponent(0x4CC7), 0, 0, 0);
            AddComponent(new AddonComponent(0x4CC6), 0, 1, 0);
            AddComponent(new AddonComponent(0x4CC2), 1, 0, 0);
            AddComponent(new AddonComponent(0x4CC3), 1, 1, 0);
            AddComponent(new AddonComponent(0x4CC4), 1, -1, 0);
            AddComponent(new AddonComponent(0x4CC5), 0, -1, 0);
        }

        public LongMetalTableEastAddon(Serial serial)
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

    public class LongMetalTableEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LongMetalTableEastAddon(); } }
        public override int LabelNumber { get { return 1154165; } } // Long Metal Table (East)

        [Constructable]
        public LongMetalTableEastDeed()
        {
        }

        public LongMetalTableEastDeed(Serial serial)
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