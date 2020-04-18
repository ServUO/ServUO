namespace Server.Items
{
    [Flipable(0x1E34, 0x1E35)]
    public class ScarecrowComponent : AddonComponent
    {
        public ScarecrowComponent()
            : base(0x1E34)
        {
        }

        public ScarecrowComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1076608;// Scarecrow
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

    public class ScarecrowAddon : BaseAddon
    {
        [Constructable]
        public ScarecrowAddon()
            : base()
        {
            AddComponent(new ScarecrowComponent(), 0, 0, 0);
        }

        public ScarecrowAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ScarecrowDeed();
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

    public class ScarecrowDeed : BaseAddonDeed
    {
        [Constructable]
        public ScarecrowDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public ScarecrowDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ScarecrowAddon();
        public override int LabelNumber => 1076608;// Scarecrow
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