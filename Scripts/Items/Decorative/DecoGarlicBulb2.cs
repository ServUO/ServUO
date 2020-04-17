namespace Server.Items
{
    public class DecoGarlicBulb2 : Item
    {
        [Constructable]
        public DecoGarlicBulb2()
            : base(0x18E4)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoGarlicBulb2(Serial serial)
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