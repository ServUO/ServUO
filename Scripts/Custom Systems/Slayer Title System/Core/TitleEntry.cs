using System;

namespace CustomsFramework.Systems.SlayerTitleSystem
{
    public class TitleEntry
    {
        private String m_Title;
        public String Title { get { return m_Title; } }

        private Int32 m_CountNeeded;
        public Int32 CountNeeded { get { return m_CountNeeded; } }

        public TitleEntry(String title, Int32 countNeeded)
        {
            m_Title = title;
            m_CountNeeded = countNeeded;
        }
    }
}
