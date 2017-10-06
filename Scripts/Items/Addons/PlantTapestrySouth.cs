using System;
using Server;

namespace Server.Items
{
    public class PlantTapestrySouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new PlantTapestrySouthDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public PlantTapestrySouthAddon()
        {
            AddComponent(new AddonComponent(0x4C9C), 0, 0, 0);
            AddComponent(new AddonComponent(0x4C9D), 1, 0, 0);
        }

        public PlantTapestrySouthAddon(Serial serial)
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

    public class PlantTapestrySouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new PlantTapestrySouthAddon(); } }
        public override int LabelNumber { get { return 1154146; } } // Plant Tapestry (South)

        [Constructable]
        public PlantTapestrySouthDeed()
        {
        }

        public PlantTapestrySouthDeed(Serial serial)
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