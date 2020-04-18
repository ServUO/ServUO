namespace Server.Items
{
    public class Chessmen3 : Item
    {
        [Constructable]
        public Chessmen3()
            : base(0xE14)
        {
            Movable = true;
            Stackable = false;
        }

        public Chessmen3(Serial serial)
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