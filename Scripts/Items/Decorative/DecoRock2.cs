namespace Server.Items
{
    public class DecoRock2 : Item
    {
        [Constructable]
        public DecoRock2()
            : base(0x1363)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoRock2(Serial serial)
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