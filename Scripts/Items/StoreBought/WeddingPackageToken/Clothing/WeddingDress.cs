namespace Server.Items
{
    [Flipable(0x9EF0, 0x9EEF)]
    public class WeddingDress : BaseOuterTorso
    {
        [Constructable]
        public WeddingDress()
            : this(0)
        {
        }

        [Constructable]
        public WeddingDress(int hue)
            : base(0x9EF0, hue)
        {
			LootType = LootType.Blessed;
			Weight = 2.0;
        }

        public override bool AllowMaleWearer => false;

        public WeddingDress(Serial serial)
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
            reader.ReadInt();
        }
    }
}
