using System;

namespace Server.Items
{
    [Flipable(0x4C3C, 0x4C3D)]
    public class TrumpetComponent : InstrumentedAddonComponent
    {
        public override int LabelNumber { get { return 1098388; } } // trumpet

        public TrumpetComponent()
            : base(0x4C3C, 0x670)
        {
        }

        public TrumpetComponent(Serial serial)
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
            AddComponent(new TrumpetComponent(), 0, 0, 0);
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
            int version = reader.ReadEncodedInt();
        }
    }
}
