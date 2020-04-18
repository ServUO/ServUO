namespace Server.Items
{
    public class DecoSilverIngots5 : Item
    {
        [Constructable]
        public DecoSilverIngots5()
            : base(0x1BFA)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoSilverIngots5(Serial serial)
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