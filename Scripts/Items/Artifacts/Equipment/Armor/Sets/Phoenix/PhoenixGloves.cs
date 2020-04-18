namespace Server.Items
{
    public class PhoenixGloves : RingmailGloves
    {
        [Constructable]
        public PhoenixGloves()
        {
            Hue = 0x8E;
            LootType = LootType.Blessed;
        }

        public PhoenixGloves(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1041605;// ringmail gloves of the phoenix

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}