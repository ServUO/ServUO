using System;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
    public enum CraftMarkOption
    {
        MarkItem,
        DoNotMark,
        PromptForMark
    }

    #region SA
    public enum CraftQuestOption
    {
        QuestItem,
        NonQuestItem
    }
    #endregion
	
    public class CraftContext
    {
        private readonly List<CraftItem> m_Items;
        private int m_LastResourceIndex;
        private int m_LastResourceIndex2;
        private int m_LastGroupIndex;
        private bool m_DoNotColor;
        private CraftMarkOption m_MarkOption;
        private CraftQuestOption m_QuestOption;

        #region Hue State Vars
        private bool m_CheckedHues;
        private List<int> m_Hues;
        private Item m_CompareHueTo;

        public bool CheckedHues
        {
            get
            {
                return this.m_CheckedHues;
            }
            set
            {
                this.m_CheckedHues = value;
            }
        }
        public List<int> Hues
        {
            get
            {
                return this.m_Hues;
            }
            set
            {
                this.m_Hues = value;
            }
        }
        public Item CompareHueTo
        {
            get
            {
                return this.m_CompareHueTo;
            }
            set
            {
                this.m_CompareHueTo = value;
            }
        }
        #endregion

        public List<CraftItem> Items
        {
            get
            {
                return this.m_Items;
            }
        }
        public int LastResourceIndex
        {
            get
            {
                return this.m_LastResourceIndex;
            }
            set
            {
                this.m_LastResourceIndex = value;
            }
        }
        public int LastResourceIndex2
        {
            get
            {
                return this.m_LastResourceIndex2;
            }
            set
            {
                this.m_LastResourceIndex2 = value;
            }
        }
        public int LastGroupIndex
        {
            get
            {
                return this.m_LastGroupIndex;
            }
            set
            {
                this.m_LastGroupIndex = value;
            }
        }
        public bool DoNotColor
        {
            get
            {
                return this.m_DoNotColor;
            }
            set
            {
                this.m_DoNotColor = value;
            }
        }
        public CraftMarkOption MarkOption
        {
            get
            {
                return this.m_MarkOption;
            }
            set
            {
                this.m_MarkOption = value;
            }
        }
        #region SA
        public CraftQuestOption QuestOption
        {
            get
            {
                return this.m_QuestOption;
            }
            set
            {
                this.m_QuestOption = value;
            }
        }
        #endregion

        public CraftContext()
        {
            this.m_Items = new List<CraftItem>();
            this.m_LastResourceIndex = -1;
            this.m_LastResourceIndex2 = -1;
            this.m_LastGroupIndex = -1;

            this.m_CheckedHues = false;
            this.m_Hues = new List<int>();
            this.m_CompareHueTo = null;
        }

        public CraftItem LastMade
        {
            get
            {
                if (this.m_Items.Count > 0)
                    return this.m_Items[0];

                return null;
            }
        }

        public void OnMade(CraftItem item)
        {
            this.m_Items.Remove(item);

            if (this.m_Items.Count == 10)
                this.m_Items.RemoveAt(9);

            this.m_Items.Insert(0, item);
        }
		
        public void ResetHueStateVars()
        {
            this.m_CheckedHues = false;
            this.m_Hues = new List<int>();
            this.m_CompareHueTo = null;
        }
    }
}