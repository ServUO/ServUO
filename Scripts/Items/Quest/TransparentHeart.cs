namespace Server.Items
{
    public class TransparentHeart : GoldEarrings
    {
        [Constructable]
        public TransparentHeart()
            : base()
        {
            LootType = LootType.Blessed;
            Weight = 1;
            Hue = 0x4AB;
        }

        public TransparentHeart(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075400;// Transparent Heart
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