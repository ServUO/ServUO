namespace Server.Items
{
    public class DecoGoldIngots3 : Item
    {
        [Constructable]
        public DecoGoldIngots3()
            : base(0x1BED)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoGoldIngots3(Serial serial)
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