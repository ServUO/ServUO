namespace Server.Items
{
    [Flipable(0x4C3E, 0x4C3F)]
    public class CelloComponent : InstrumentedAddonComponent
    {
        public override int LabelNumber => 1098390;  // cello

        public CelloComponent()
            : base(0x4C3E, 0x66D)
        {
        }

        public CelloComponent(Serial serial)
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

    public class CelloDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1098390;  // cello

        [Constructable]
        public CelloDeed()
        {
        }

        public CelloDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new CelloAddon();

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
        public override BaseAddonDeed Deed => new CelloDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public CelloAddon()
        {
            AddComponent(new CelloComponent(), 0, 0, 0);
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