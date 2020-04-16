namespace Server.Items
{
    [Flipable(0x42C9, 0x42CA)]
    public class UltimaBanner : Item
    {
        [Constructable]
        public UltimaBanner()
            : base(0x42C9)
        {
            Weight = 2.0;
        }

        public UltimaBanner(Serial serial)
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
