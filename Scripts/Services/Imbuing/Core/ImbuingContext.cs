using System;
using Server;

namespace Server
{
    public class ImbuingContext
    {
        private Mobile m_Player;
        private Item m_LastImbued;
        private int m_Imbue_Mod, m_Imbue_ModInt, m_Imbue_ModVal;
        private int m_Imbue_IWmax, m_Imb_SFBonus, m_ImbMenu_Cat, m_ImbMenu_ModInc;

        public Mobile Mobile 
        { 
            get { return m_Player; } 
        }              
     
        public Item LastImbued 
        {
            get { return m_LastImbued; } 
            set { m_LastImbued = value; } 
        }

        public int Imbue_Mod 
        { 
            get { return m_Imbue_Mod; } 
            set { m_Imbue_Mod = value; }
        }

        public int Imbue_ModInt 
        { 
            get { return m_Imbue_ModInt; } 
            set { m_Imbue_ModInt = value; } 
        }

        public int Imbue_ModVal 
        { 
            get { return m_Imbue_ModVal; } 
            set { m_Imbue_ModVal = value; } 
        }

        public int Imbue_SFBonus 
        { 
            get { return m_Imb_SFBonus; } 
            set { m_Imb_SFBonus = value; } 
        }

        public int ImbMenu_Cat 
        { 
            get { return m_ImbMenu_Cat; } 
            set { m_ImbMenu_Cat = value; } 
        }

        public int ImbMenu_ModInc 
        { 
            get { return m_ImbMenu_ModInc; } 
            set { m_ImbMenu_ModInc = value; } 
        }

        public int Imbue_IWmax 
        {
            get { return m_Imbue_IWmax; } 
            set { m_Imbue_IWmax = value; } 
        }

        public ImbuingContext(Mobile mob)
        {
            m_Player = mob;
        }

        public ImbuingContext(Mobile owner, GenericReader reader)
        {
            int v = reader.ReadInt();

            m_Player = owner;
            m_LastImbued = reader.ReadItem();
            m_Imbue_Mod = reader.ReadInt();
            m_Imbue_ModInt = reader.ReadInt();
            m_Imbue_ModVal = reader.ReadInt();
            m_Imbue_IWmax = reader.ReadInt();
            m_ImbMenu_Cat = reader.ReadInt();
            m_ImbMenu_ModInc = reader.ReadInt();

            m_Imb_SFBonus = 0;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            writer.Write(m_LastImbued);
            writer.Write(m_Imbue_Mod);
            writer.Write(m_Imbue_ModInt);
            writer.Write(m_Imbue_ModVal);
            writer.Write(m_Imbue_IWmax);
            writer.Write(m_ImbMenu_Cat);
            writer.Write(m_ImbMenu_ModInc);
        }
    }
}