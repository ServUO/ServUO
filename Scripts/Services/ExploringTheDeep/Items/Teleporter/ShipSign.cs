namespace Server.Items
{
    public class ShipSign : Item
    {
        private int m_Cliloc;

        public override int LabelNumber => m_Cliloc;


        [Constructable]
        public ShipSign(int id, int cliloc)
            : base(0xBD2)
        {
            ItemID = id;
            m_Cliloc = cliloc;
            Movable = false;
        }

        public ShipSign(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Cliloc);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Cliloc = reader.ReadInt();
        }
    }
}