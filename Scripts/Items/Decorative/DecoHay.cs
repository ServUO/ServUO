namespace Server.Items
{
    public class DecoHay : Item
    {
        [Constructable]
        public DecoHay()
            : base(0xF35)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoHay(Serial serial)
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