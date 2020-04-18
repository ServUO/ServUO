namespace Server.Items
{
    [Flipable(0x9D09, 0x9D0A)]
    public class WeatheredBronzeFairySculptureComponent : AddonComponent
    {
        public override int LabelNumber => 1156883;  // weathered bronze fairy sculpture

        public WeatheredBronzeFairySculptureComponent()
            : base(0x9D09)
        {
        }

        public WeatheredBronzeFairySculptureComponent(Serial serial)
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

    public class WeatheredBronzeFairySculptureAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new WeatheredBronzeFairySculptureDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public WeatheredBronzeFairySculptureAddon()
        {
            AddComponent(new WeatheredBronzeFairySculptureComponent(), 0, 0, 0);
        }

        public WeatheredBronzeFairySculptureAddon(Serial serial)
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

    public class WeatheredBronzeFairySculptureDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new WeatheredBronzeFairySculptureAddon();
        public override int LabelNumber => 1156883;  // weathered bronze fairy sculpture

        [Constructable]
        public WeatheredBronzeFairySculptureDeed()
        {
        }

        public WeatheredBronzeFairySculptureDeed(Serial serial)
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