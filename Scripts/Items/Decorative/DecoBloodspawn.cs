namespace Server.Items
{
    public class DecoBloodspawn : Item
    {
        [Constructable]
        public DecoBloodspawn()
            : base(0xF7C)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoBloodspawn(Serial serial)
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