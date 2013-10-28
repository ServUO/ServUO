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
            this.m_Init = false;
        }

        public bool Init
        {
            get
            {
                return this.m_Init;
            }
            set
            {
                this.m_Init = value;
            }
        }
        public Type ResType
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
        public string NameString
        {
            get
            {
                return this.m_NameString;
            }
            set
            {
                this.m_NameString = value;
            }
        }
        public int NameNumber
        {
            get
            {
                return this.m_NameNumber;
            }
            set
            {
                this.m_NameNumber = value;
            }
        }
        public void Add(CraftSubRes craftSubRes)
        {
            this.List.Add(craftSubRes);
        }

        public void Remove(int index)
        {
            if (index > this.Count - 1 || index < 0)
            {
            }
            else
            {
                this.List.RemoveAt(index);
            }
        }

        public CraftSubRes GetAt(int index)
        {
            return (CraftSubRes)this.List[index];
        }

        public CraftSubRes SearchFor(Type type)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                CraftSubRes craftSubRes = (CraftSubRes)this.List[i];
                if (craftSubRes.ItemType == type)
                {
                    return craftSubRes;
                }
            }
            return null;
        }
    }
}