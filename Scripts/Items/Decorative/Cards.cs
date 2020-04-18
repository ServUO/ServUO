namespace Server.Items
{
    public class Cards : Item
    {
        [Constructable]
        public Cards()
            : base(0xE19)
        {
            Movable = true;
            Stackable = false;
        }

        public Cards(Serial serial)
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