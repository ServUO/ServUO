namespace Server.Items
{
    public class DecoSilverIngots4 : Item
    {
        [Constructable]
        public DecoSilverIngots4()
            : base(0x1BF9)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoSilverIngots4(Serial serial)
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