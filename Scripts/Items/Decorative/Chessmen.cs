namespace Server.Items
{
    public class Chessmen : Item
    {
        [Constructable]
        public Chessmen()
            : base(0xE13)
        {
            Movable = true;
            Stackable = false;
        }

        public Chessmen(Serial serial)
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