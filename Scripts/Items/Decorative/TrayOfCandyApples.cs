namespace Server.Items
{
    [Flipable(0x9D32, 0x9D33)]
    public class TrayOfCandyApples : Item
    {
        public override int LabelNumber => 1124266;  // Tray of Candy Apples

        [Constructable]
        public TrayOfCandyApples()
            : base(0x9D32)
        {
        }

        public TrayOfCandyApples(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}
