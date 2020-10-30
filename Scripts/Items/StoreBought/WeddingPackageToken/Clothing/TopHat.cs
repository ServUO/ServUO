namespace Server.Items
{
    [Flipable(0x9EF9, 0x9EFA)]
    public class TopHat : BaseHat
    {
        public override int LabelNumber => 1124721; // top hat

        [Constructable]
        public TopHat()
            : this(0)
        {
        }

        [Constructable]
        public TopHat(int hue)
            : base(0x9EF9, hue)
        {
			LootType = LootType.Blessed;
			Weight = 3.0;
        }

        public TopHat(Serial serial)
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
