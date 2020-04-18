namespace Server.Items
{
    [Flipable(0xC17, 0xC18)]
    public class BrokenCoveredChairComponent : AddonComponent
    {
        public BrokenCoveredChairComponent()
            : base(0xC17)
        {
        }

        public BrokenCoveredChairComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1076257;// Broken Covered Chair
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

    public class BrokenCoveredChairAddon : BaseAddon
    {
        [Constructable]
        public BrokenCoveredChairAddon()
            : base()
        {
            AddComponent(new BrokenCoveredChairComponent(), 0, 0, 0);
        }

        public BrokenCoveredChairAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BrokenCoveredChairDeed();
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

    public class BrokenCoveredChairDeed : BaseAddonDeed
    {
        [Constructable]
        public BrokenCoveredChairDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public BrokenCoveredChairDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new BrokenCoveredChairAddon();
        public override int LabelNumber => 1076257;// Broken Covered Chair
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