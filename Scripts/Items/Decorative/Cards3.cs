namespace Server.Items
{
    public class Cards3 : Item
    {
        [Constructable]
        public Cards3()
            : base(0xE15)
        {
            Movable = true;
            Stackable = false;
        }

        public Cards3(Serial serial)
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