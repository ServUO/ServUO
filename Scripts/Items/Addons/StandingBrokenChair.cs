namespace Server.Items
{
    [Flipable(0xC1B, 0xC1C, 0xC1E, 0xC1D)]
    public class StandingBrokenChairComponent : AddonComponent
    {
        public StandingBrokenChairComponent()
            : base(0xC1B)
        {
        }

        public StandingBrokenChairComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1076259;// Standing Broken Chair
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

    public class StandingBrokenChairAddon : BaseAddon
    {
        [Constructable]
        public StandingBrokenChairAddon()
            : base()
        {
            AddComponent(new StandingBrokenChairComponent(), 0, 0, 0);
        }

        public StandingBrokenChairAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new StandingBrokenChairDeed();
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

    public class StandingBrokenChairDeed : BaseAddonDeed
    {
        [Constructable]
        public StandingBrokenChairDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public StandingBrokenChairDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new StandingBrokenChairAddon();
        public override int LabelNumber => 1076259;// Standing Broken Chair
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