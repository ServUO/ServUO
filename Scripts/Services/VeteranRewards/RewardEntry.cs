using System;

namespace Server.Engines.VeteranRewards
{
    public class RewardEntry
    {
        private readonly RewardCategory m_Category;
        private readonly Type m_ItemType;
        private readonly int m_Name;
        private readonly string m_NameString;
        private readonly object[] m_Args;
        private RewardList m_List;
		
        public RewardEntry(RewardCategory category, int name, Type itemType, params object[] args)
        {
            m_Category = category;
            m_ItemType = itemType;
            m_Name = name;
            m_Args = args;
            category.Entries.Add(this);
        }

        public RewardEntry(RewardCategory category, string name, Type itemType, params object[] args)
        {
            m_Category = category;
            m_ItemType = itemType;
            m_NameString = name;
            m_Args = args;
            category.Entries.Add(this);
        }

        public RewardList List
        {
            get
            {
                return m_List;
            }
            set
            {
                m_List = value;
            }
        }
        public RewardCategory Category => m_Category;
        public Type ItemType => m_ItemType;
        public int Name => m_Name;
        public string NameString => m_NameString;
        public object[] Args => m_Args;
        public Item Construct()
        {
            try
            {
                Item item = Activator.CreateInstance(m_ItemType, m_Args) as Item;

                if (item is IRewardItem)
                    ((IRewardItem)item).IsRewardItem = true;

                return item;
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
            }

            return null;
        }
    }
}
