using System;

namespace Server.Items
{
    public class TrumpetDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1098388; } } // trumpet

        [Constructable]
        public TrumpetDeed()
        {
        }

        public TrumpetDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon { get { return new TrumpetAddon(); } }

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

    public class TrumpetAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new TrumpetDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public TrumpetAddon()
        {
            AddComponent(new InstrumentedAddonComponent(0x4C3C, 0x670), 0, 0, 10);
        }

        public TrumpetAddon(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}