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
            this.m_Category = category;
            this.m_ItemType = itemType;
            this.m_RequiredExpansion = Expansion.None;
            this.m_Name = name;
            this.m_Args = args;
            category.Entries.Add(this);
        }

        public RewardEntry(RewardCategory category, string name, Type itemType, params object[] args)
        {
            this.m_Category = category;
            this.m_ItemType = itemType;
            this.m_RequiredExpansion = Expansion.None;
            this.m_NameString = name;
            this.m_Args = args;
            category.Entries.Add(this);
        }

        public RewardEntry(RewardCategory category, int name, Type itemType, Expansion requiredExpansion, params object[] args)
        {
            this.m_Category = category;
            this.m_ItemType = itemType;
            this.m_RequiredExpansion = requiredExpansion;
            this.m_Name = name;
            this.m_Args = args;
            category.Entries.Add(this);
        }

        public RewardEntry(RewardCategory category, string name, Type itemType, Expansion requiredExpansion, params object[] args)
        {
            this.m_Category = category;
            this.m_ItemType = itemType;
            this.m_RequiredExpansion = requiredExpansion;
            this.m_NameString = name;
            this.m_Args = args;
            category.Entries.Add(this);
        }

        public RewardList List
        {
            get
            {
                return this.m_List;
            }
            set
            {
                this.m_List = value;
            }
        }
        public RewardCategory Category
        {
            get
            {
                return this.m_Category;
            }
        }
        public Type ItemType
        {
            get
            {
                return this.m_ItemType;
            }
        }
        public Expansion RequiredExpansion
        {
            get
            {
                return this.m_RequiredExpansion;
            }
        }
        public int Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public string NameString
        {
            get
            {
                return this.m_NameString;
            }
        }
        public object[] Args
        {
            get
            {
                return this.m_Args;
            }
        }
        public Item Construct()
        {
            try
            {
                Item item = Activator.CreateInstance(this.m_ItemType, this.m_Args) as Item;

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