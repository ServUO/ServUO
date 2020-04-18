namespace Server.Items
{
    [Flipable(0x3DAA, 0x3DA9)]
    public class SuitOfGoldArmorComponent : AddonComponent
    {
        public SuitOfGoldArmorComponent()
            : base(0x3DAA)
        {
        }

        public SuitOfGoldArmorComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1076265;// Suit of Gold Armor
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

    public class SuitOfGoldArmorAddon : BaseAddon
    {
        [Constructable]
        public SuitOfGoldArmorAddon()
            : base()
        {
            AddComponent(new SuitOfGoldArmorComponent(), 0, 0, 0);
        }

        public SuitOfGoldArmorAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new SuitOfGoldArmorDeed();
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

    public class SuitOfGoldArmorDeed : BaseAddonDeed
    {
        [Constructable]
        public SuitOfGoldArmorDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public SuitOfGoldArmorDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new SuitOfGoldArmorAddon();
        public override int LabelNumber => 1076265;// Suit of Gold Armor
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