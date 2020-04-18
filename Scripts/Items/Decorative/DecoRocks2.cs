namespace Server.Items
{
    public class DecoRocks2 : Item
    {
        [Constructable]
        public DecoRocks2()
            : base(0x136D)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoRocks2(Serial serial)
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