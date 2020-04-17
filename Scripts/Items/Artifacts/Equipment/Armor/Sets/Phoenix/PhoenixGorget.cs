namespace Server.Items
{
    public class PhoenixGorget : StuddedGorget
    {
        [Constructable]
        public PhoenixGorget()
        {
            Hue = 0x8E;
            LootType = LootType.Blessed;
        }

        public PhoenixGorget(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1041604;// studded gorget of the phoenix

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