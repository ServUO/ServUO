using System;

namespace Server.Engines.Craft
{
    public class CraftSubResCol : System.Collections.CollectionBase
    {
        private Type m_Type;
        private string m_NameString;
        private int m_NameNumber;
        private bool m_Init;
        public CraftSubResCol()
        {
            m_Init = false;
        }

        public bool Init
        {
            get
            {
                return m_Init;
            }
            set
            {
                m_Init = value;
            }
        }
        public Type ResType
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }
        public string NameString
        {
            get
            {
                return m_NameString;
            }
            set
            {
                m_NameString = value;
            }
        }
        public int NameNumber
        {
            get
            {
                return m_NameNumber;
            }
            set
            {
                m_NameNumber = value;
            }
        }
        public void Add(CraftSubRes craftSubRes)
        {
            List.Add(craftSubRes);
        }

        public void Remove(int index)
        {
            if (index > Count - 1 || index < 0)
            {
            }
            else
            {
                List.RemoveAt(index);
            }
        }

        public CraftSubRes GetAt(int index)
        {
            return (CraftSubRes)List[index];
        }

        public CraftSubRes SearchFor(Type type)
        {
            for (int i = 0; i < List.Count; i++)
            {
                CraftSubRes craftSubRes = (CraftSubRes)List[i];
                if (craftSubRes.ItemType == type)
                {
                    return craftSubRes;
                }
            }
            return null;
        }
    }
}