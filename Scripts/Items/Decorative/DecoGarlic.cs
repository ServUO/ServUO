namespace Server.Items
{
    public class DecoGarlic : Item
    {
        [Constructable]
        public DecoGarlic()
            : base(0x18E1)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoGarlic(Serial serial)
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