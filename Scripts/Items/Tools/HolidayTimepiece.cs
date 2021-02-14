namespace Server.Items
{
    public class HolidayTimepiece : Clock
    {
        public override int LabelNumber => 1041113;// a holiday timepiece

        [Constructable]
        public HolidayTimepiece()
            : base(0x1086)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Layer = Layer.Bracelet;
        }

        public HolidayTimepiece(Serial serial)
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
