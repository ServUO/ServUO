namespace Server.Items
{
    public class DecoRocks : Item
    {
        [Constructable]
        public DecoRocks()
            : base(0x1367)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoRocks(Serial serial)
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