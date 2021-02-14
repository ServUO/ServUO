namespace Server.Items
{
    [Flipable(0x3BBB, 0x3BBC)]
    public class HolidayGarland : Item
    {
        public override int LabelNumber => 1095239; // a holiday garland

        [Constructable]
        public HolidayGarland()
            : base(0x3BBB)
        {
        }

        public HolidayGarland(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
