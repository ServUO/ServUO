using System;

namespace Server.Items
{
    public class CelloDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1098390; } } // cello

        [Constructable]
        public CelloDeed()
        {
        }

        public CelloDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon { get { return new CelloAddon(); } }

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

    public class CelloAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new CelloDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public CelloAddon()
        {
            AddComponent(new InstrumentedAddonComponent(0x4C3E, 0x66E), 0, 0, 10);
        }

        public CelloAddon(Serial serial)
            : base(serial)
        {
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
}