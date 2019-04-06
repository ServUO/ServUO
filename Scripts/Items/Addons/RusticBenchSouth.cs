using System;
using Server;

namespace Server.Items
{
    public class RusticBenchSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new RusticBenchSouthDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public RusticBenchSouthAddon()
        {
            AddComponent(new AddonComponent(0x0E50), 0, 0, 0);
            AddComponent(new AddonComponent(0x0E51), 1, 0, 0);
        }

        public RusticBenchSouthAddon(Serial serial)
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

    public class RusticBenchSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new RusticBenchSouthAddon(); } }
        public override int LabelNumber { get { return 1150593; } } // rustic bench (south)

        [Constructable]
        public RusticBenchSouthDeed()
        {
        }

        public RusticBenchSouthDeed(Serial serial)
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