namespace Server.Items
{
    public class DecoMandrake3 : Item
    {
        [Constructable]
        public DecoMandrake3()
            : base(0x18DF)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoMandrake3(Serial serial)
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