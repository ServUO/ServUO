namespace Server.Items
{
    public class DecoIronIngots3 : Item
    {
        [Constructable]
        public DecoIronIngots3()
            : base(0x1BF0)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoIronIngots3(Serial serial)
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