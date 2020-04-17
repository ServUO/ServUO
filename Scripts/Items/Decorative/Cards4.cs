namespace Server.Items
{
    public class Cards4 : Item
    {
        [Constructable]
        public Cards4()
            : base(0xE17)
        {
            Movable = true;
            Stackable = false;
        }

        public Cards4(Serial serial)
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