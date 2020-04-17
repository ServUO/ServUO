namespace Server.Items
{
    [Flipable(0xC14, 0xC15)]
    public class BrokenBookcaseComponent : AddonComponent
    {
        public BrokenBookcaseComponent()
            : base(0xC14)
        {
        }

        public BrokenBookcaseComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1076258;// Broken Bookcase
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

    public class BrokenBookcaseAddon : BaseAddon
    {
        [Constructable]
        public BrokenBookcaseAddon()
            : base()
        {
            AddComponent(new BrokenBookcaseComponent(), 0, 0, 0);
        }

        public BrokenBookcaseAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BrokenBookcaseDeed();
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

    public class BrokenBookcaseDeed : BaseAddonDeed
    {
        [Constructable]
        public BrokenBookcaseDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public BrokenBookcaseDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new BrokenBookcaseAddon();
        public override int LabelNumber => 1076258;// Broken Bookcase
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