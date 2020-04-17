namespace Server.Items
{
    public class DecoGoldIngots4 : Item
    {
        [Constructable]
        public DecoGoldIngots4()
            : base(0x1BEE)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoGoldIngots4(Serial serial)
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