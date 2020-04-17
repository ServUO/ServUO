namespace Server.Items
{
    public class DecoRoseOfTrinsic3 : Item
    {
        [Constructable]
        public DecoRoseOfTrinsic3()
            : base(0x234B)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoRoseOfTrinsic3(Serial serial)
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