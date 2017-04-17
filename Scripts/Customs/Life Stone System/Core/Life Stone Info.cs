using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items; 

namespace Server
{
    public class LifeStoneInfo
    {
        private Mobile m_LifeStoneOwner;
        private LifeStone m_LifeStone;

        public Mobile LifeStoneOwner { get { return m_LifeStoneOwner; } set { m_LifeStoneOwner = value; } }
        public LifeStone Stone { get { return m_LifeStone; } set { m_LifeStone = value; } }

        public LifeStoneInfo(Mobile from, LifeStone item)
        {
            m_LifeStoneOwner = from;
            m_LifeStone = item;
        }

        public LifeStoneInfo(GenericReader reader)
        {
            Deserialize(reader);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(m_LifeStoneOwner);
            writer.Write(m_LifeStone);
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_LifeStoneOwner = reader.ReadMobile();
                        m_LifeStone = (LifeStone)reader.ReadItem();
                        break;
                    }
            }
        }
    }
}