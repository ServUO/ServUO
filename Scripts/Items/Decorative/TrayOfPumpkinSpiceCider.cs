namespace Server.Items
{
    [Flipable(0x9D30, 0x9D31)]
    public class TrayOfPumpkinSpiceCider : Item
    {
        public override int LabelNumber => 1124264;  // Tray of Pumpkin Spice Cider

        [Constructable]
        public TrayOfPumpkinSpiceCider()
            : base(0x9D30)
        {
        }

        public TrayOfPumpkinSpiceCider(Serial serial)
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
