namespace Server.Items
{
    public class DecoGarlicBulb : Item
    {
        [Constructable]
        public DecoGarlicBulb()
            : base(0x18E3)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoGarlicBulb(Serial serial)
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