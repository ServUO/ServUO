namespace Server.Items
{
    public class DecoGoldIngot2 : Item
    {
        [Constructable]
        public DecoGoldIngot2()
            : base(0x1BEC)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoGoldIngot2(Serial serial)
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