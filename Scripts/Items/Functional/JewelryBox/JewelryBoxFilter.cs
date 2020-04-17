namespace Server.Items
{
    public class JewelryBoxFilter
    {
        public bool IsDefault => (!Ring && !Bracelet && !Earrings && !Necklace && !Talisman);

        public void Clear()
        {
            m_Ring = false;
            m_Bracelet = false;
            m_Earrings = false;
            m_Necklace = false;
            m_Talisman = false;
        }

        private bool m_Ring;

        public bool Ring
        {
            get { return m_Ring; }
            set
            {
                if (value == true)
                    Clear();

                m_Ring = value;
            }
        }

        private bool m_Bracelet;
        public bool Bracelet
        {
            get { return m_Bracelet; }
            set
            {
                if (value == true)
                    Clear();

                m_Bracelet = value;
            }
        }

        private bool m_Earrings;
        public bool Earrings
        {
            get { return m_Earrings; }
            set
            {
                if (value == true)
                    Clear();

                m_Earrings = value;
            }
        }

        private bool m_Necklace;
        public bool Necklace
        {
            get { return m_Necklace; }
            set
            {
                if (value == true)
                    Clear();

                m_Necklace = value;
            }
        }

        private bool m_Talisman;
        public bool Talisman
        {
            get { return m_Talisman; }
            set
            {
                if (value == true)
                    Clear();

                m_Talisman = value;
            }
        }

        public JewelryBoxFilter()
        {
        }

        public JewelryBoxFilter(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    Ring = reader.ReadBool();
                    Bracelet = reader.ReadBool();
                    Earrings = reader.ReadBool();
                    Necklace = reader.ReadBool();
                    Talisman = reader.ReadBool();
                    break;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            if (IsDefault)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(1);

                writer.Write(Ring);
                writer.Write(Bracelet);
                writer.Write(Earrings);
                writer.Write(Necklace);
                writer.Write(Talisman);
            }
        }
    }
}
