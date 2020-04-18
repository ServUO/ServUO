using System;

namespace Server.Engines.VeteranRewards
{
    public class RewardEntry
    {
        private readonly RewardCategory m_Category;
        private readonly Type m_ItemType;
        private readonly Expansion m_RequiredExpansion;
        private readonly int m_Name;
        private readonly string m_NameString;
        private readonly object[] m_Args;
        private RewardList m_List;
        public RewardEntry(RewardCategory category, int name, Type itemType, params object[] args)
        {
            m_Category = category;
            m_ItemType = itemType;
            m_RequiredExpansion = Expansion.None;
            m_Name = name;
            m_Args = args;
            category.Entries.Add(this);
        }

        public RewardEntry(RewardCategory category, string name, Type itemType, params object[] args)
        {
            m_Category = category;
            m_ItemType = itemType;
            m_RequiredExpansion = Expansion.None;
            m_NameString = name;
            m_Args = args;
            category.Entries.Add(this);
        }

        public RewardEntry(RewardCategory category, int name, Type itemType, Expansion requiredExpansion, params object[] args)
        {
            m_Category = category;
            m_ItemType = itemType;
            m_RequiredExpansion = requiredExpansion;
            m_Name = name;
            m_Args = args;
            category.Entries.Add(this);
        }

        public RewardEntry(RewardCategory category, string name, Type itemType, Expansion requiredExpansion, params object[] args)
        {
            m_Category = category;
            m_ItemType = itemType;
            m_RequiredExpansion = requiredExpansion;
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
        public Expansion RequiredExpansion => m_RequiredExpansion;
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
            catch
            {
            }

            return null;
        }
    }
}