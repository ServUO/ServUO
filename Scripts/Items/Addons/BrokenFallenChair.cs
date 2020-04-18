namespace Server.Items
{
    [Flipable(0xC19, 0xC1A)]
    public class BrokenFallenChairComponent : AddonComponent
    {
        public BrokenFallenChairComponent()
            : base(0xC19)
        {
        }

        public BrokenFallenChairComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1076264;// Broken Fallen Chair
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
                ItemID = 0xC19;
        }
    }

    public class BrokenFallenChairAddon : BaseAddon
    {
        [Constructable]
        public BrokenFallenChairAddon()
            : base()
        {
            AddComponent(new BrokenFallenChairComponent(), 0, 0, 0);
        }

        public BrokenFallenChairAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BrokenFallenChairDeed();
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

    public class BrokenFallenChairDeed : BaseAddonDeed
    {
        [Constructable]
        public BrokenFallenChairDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public BrokenFallenChairDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new BrokenFallenChairAddon();
        public override int LabelNumber => 1076264;// Broken Fallen Chair
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