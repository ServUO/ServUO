namespace Server.Items
{
    public class ChocolateChipSeastar : BaseFish
    {
        [Constructable]
        public ChocolateChipSeastar()
            : base(0xA395)
        {
        }

        public ChocolateChipSeastar(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1159131;// Chocolate Chip Seastar
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
