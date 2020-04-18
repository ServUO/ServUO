namespace Server.Items
{
    public class DecoIronIngots : Item
    {
        [Constructable]
        public DecoIronIngots()
            : base(0x1BF1)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoIronIngots(Serial serial)
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