namespace Server.Items
{
    public class DecoGinsengRoot : Item
    {
        [Constructable]
        public DecoGinsengRoot()
            : base(0x18EB)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoGinsengRoot(Serial serial)
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