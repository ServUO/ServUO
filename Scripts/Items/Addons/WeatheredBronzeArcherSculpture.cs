namespace Server.Items
{
    [Flipable(0x9D2E, 0x9D2F)]
    public class WeatheredBronzeArcherComponent : AddonComponent
    {
        public override int LabelNumber => 1156884;  // weathered bronze archer sculpture

        public WeatheredBronzeArcherComponent()
            : base(0x9D2E)
        {
        }

        public WeatheredBronzeArcherComponent(Serial serial)
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

    public class WeatheredBronzeArcherAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new WeatheredBronzeArcherDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public WeatheredBronzeArcherAddon()
        {
            AddComponent(new WeatheredBronzeArcherComponent(), 0, 0, 0);
        }

        public WeatheredBronzeArcherAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WeatheredBronzeArcherDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new WeatheredBronzeArcherAddon();
        public override int LabelNumber => 1156884;  // weathered bronze archer sculpture

        [Constructable]
        public WeatheredBronzeArcherDeed()
        {
        }

        public WeatheredBronzeArcherDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}