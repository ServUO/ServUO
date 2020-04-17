namespace Server.Items
{
    [Furniture]
    [Flipable(0x4C80, 0x4C81)]
    public class UpholsteredChairComponent : AddonComponent
    {
        public override int LabelNumber => 1154173;  // Upholstered Chair

        public UpholsteredChairComponent()
            : base(0x4C80)
        {
        }

        public UpholsteredChairComponent(Serial serial)
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

    public class UpholsteredChairDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1154173;  // Upholstered Chair       

        [Constructable]
        public UpholsteredChairDeed()
        {
        }

        public override BaseAddon Addon => new UpholsteredChairAddon();

        public UpholsteredChairDeed(Serial serial)
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

    public class UpholsteredChairAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new UpholsteredChairDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public UpholsteredChairAddon()
        {
            AddComponent(new UpholsteredChairComponent(), 0, 0, 0);
        }

        public UpholsteredChairAddon(Serial serial)
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