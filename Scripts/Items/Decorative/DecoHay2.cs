namespace Server.Items
{
    public class DecoHay2 : Item
    {
        [Constructable]
        public DecoHay2()
            : base(0xF34)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoHay2(Serial serial)
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