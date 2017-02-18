using System;

namespace Server.Items
{
    public class ShipSign : Item
    {
        private int m_Cliloc;

        public override int LabelNumber { get { return m_Cliloc; } }


        [Constructable]
        public ShipSign(int id, int cliloc)
            : base(0xBD2)
        {
            this.ItemID = id;
            this.m_Cliloc = cliloc;
            this.Movable = false;          
        }

        public ShipSign(Serial serial)
            : base(serial)
        {
        } 

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

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