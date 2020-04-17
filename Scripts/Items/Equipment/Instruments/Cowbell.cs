namespace Server.Items
{
    [Flipable(0x4C5A, 0x4C5B)]
    public class CowBellComponent : InstrumentedAddonComponent
    {
        public override int LabelNumber => 1098418;  // cowbell

        public CowBellComponent()
            : base(0x4C5A, 0x66E)
        {
        }

        public CowBellComponent(Serial serial)
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

    public class CowBellDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1098418;  // cowbell

        [Constructable]
        public CowBellDeed()
        {
        }

        public CowBellDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new CowBellAddon();

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

    public class CowBellAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new CowBellDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public CowBellAddon()
        {
            AddComponent(new CowBellComponent(), 0, 0, 0);
        }

        public CowBellAddon(Serial serial)
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