namespace Server.Items
{
    public class DecoMandrakeRoot : Item
    {
        [Constructable]
        public DecoMandrakeRoot()
            : base(0x18DE)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoMandrakeRoot(Serial serial)
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