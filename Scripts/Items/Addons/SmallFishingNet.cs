namespace Server.Items
{
    [Flipable(0x1EA3, 0x1EA4)]
    public class SmallFishingNetComponent : AddonComponent
    {
        public SmallFishingNetComponent()
            : base(0x1EA3)
        {
        }

        public SmallFishingNetComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1076286;// Small Fish Net
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

    public class SmallFishingNetAddon : BaseAddon
    {
        [Constructable]
        public SmallFishingNetAddon()
            : base()
        {
            AddComponent(new SmallFishingNetComponent(), 0, 0, 0);
        }

        public SmallFishingNetAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new SmallFishingNetDeed();
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

    public class SmallFishingNetDeed : BaseAddonDeed
    {
        [Constructable]
        public SmallFishingNetDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public SmallFishingNetDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new SmallFishingNetAddon();
        public override int LabelNumber => 1076286;// Small Fish Net
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