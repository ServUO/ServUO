namespace Server.Items
{
    public class LuckyDagger : Item
    {
        public override int LabelNumber => 1151983;  // Lucky Dagger

        [Constructable]
        public LuckyDagger()
            : base(0xF52)
        {
            Hue = 0x8A5;
        }

        public LuckyDagger(Serial serial)
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