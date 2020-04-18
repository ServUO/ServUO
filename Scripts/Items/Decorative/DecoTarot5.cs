namespace Server.Items
{
    public class DecoTarot5 : Item
    {
        [Constructable]
        public DecoTarot5()
            : base(0x12A9)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoTarot5(Serial serial)
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