namespace Server.Items
{
    public class DecoDeckOfTarot : Item
    {
        [Constructable]
        public DecoDeckOfTarot()
            : base(0x12AB)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoDeckOfTarot(Serial serial)
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