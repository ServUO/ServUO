namespace Server.Items
{
    public class ShadowPillarAddon : BaseAddon
    {
        [Constructable]
        public ShadowPillarAddon()
        {
            AddComponent(new LocalizedAddonComponent(0x3650, 1076679), 0, 0, 0);
        }

        public ShadowPillarAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ShadowPillarDeed();

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

    public class ShadowPillarDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1076679;  // Shadow Pillar

        public override bool ExcludeDeedHue => true;

        [Constructable]
        public ShadowPillarDeed()
        {
            LootType = LootType.Blessed;
            Hue = 1908;
        }

        public ShadowPillarDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ShadowPillarAddon();


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
