namespace Server.Items
{
    public class AnniversaryVaseShort : Item
    {
        public override int LabelNumber => 1156148;  // Short 18th Anniversary Vase

        [Constructable]
        public AnniversaryVaseShort() : base(0x9BCA)
        {
        }

        public AnniversaryVaseShort(Serial serial) : base(serial)
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