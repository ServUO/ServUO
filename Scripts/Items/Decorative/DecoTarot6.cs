namespace Server.Items
{
    public class DecoTarot6 : Item
    {
        [Constructable]
        public DecoTarot6()
            : base(0x12AA)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoTarot6(Serial serial)
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