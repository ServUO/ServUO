using System;
using Server;

namespace Server.Items
{
    public class FancyCouchWestAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new FancyCouchWestDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public FancyCouchWestAddon()
        {
            AddComponent(new AddonComponent(0x9C5F), 0, -1, 0);
            AddComponent(new AddonComponent(0x9C5E), 0, 0, 0);
            AddComponent(new AddonComponent(0x9C5D), 0, 1, 0);
        }

        public FancyCouchWestAddon(Serial serial)
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

    public class FancyCouchWestDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new FancyCouchWestAddon(); } }
        public override int LabelNumber { get { return 1156583; } } // Fancy Couch (West)

        [Constructable]
        public FancyCouchWestDeed()
        {
        }

        public FancyCouchWestDeed(Serial serial)
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