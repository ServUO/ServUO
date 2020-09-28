namespace Server.Items
{
    [Flipable(0x9AA, 0xE7D)]
    public class BoxOfShadowItems : BaseContainer
    {
        public override int LabelNumber => 1076713;  // A Box of Shadow Items

        public override int DefaultGumpID => 0x43;

        [Constructable]
        public BoxOfShadowItems()
            : base(0x9AA)
        {
            Weight = 4.0;
            Hue = 1902;

            DropItem(new FireDemonStatueDeed());
            DropItem(new GlobeOfSosariaDeed());
            DropItem(new ObsidianPillarDeed());
            DropItem(new ObsidianRockDeed());
            DropItem(new ShadowAltarDeed());
            DropItem(new ShadowBannerDeed());
            DropItem(new ShadowFirePitDeed());
            DropItem(new ShadowPillarDeed());
            DropItem(new SpikeColumnDeed());
            DropItem(new SpikePostDeed());
        }

        public BoxOfShadowItems(Serial serial)
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
