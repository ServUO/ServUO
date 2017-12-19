using System;
using Server.Items;

namespace Server.Engines.Craft
{
    public abstract class CustomCraft
    {
        private readonly Mobile m_From;
        private readonly CraftItem m_CraftItem;
        private readonly CraftSystem m_CraftSystem;
        private readonly Type m_TypeRes;
        private readonly ITool m_Tool;
        private readonly int m_Quality;
        public CustomCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, ITool tool, int quality)
        {
            this.m_From = from;
            this.m_CraftItem = craftItem;
            this.m_CraftSystem = craftSystem;
            this.m_TypeRes = typeRes;
            this.m_Tool = tool;
            this.m_Quality = quality;
        }

        public Mobile From
        {
            get
            {
                return this.m_From;
            }
        }
        public CraftItem CraftItem
        {
            get
            {
                return this.m_CraftItem;
            }
        }
        public CraftSystem CraftSystem
        {
            get
            {
                return this.m_CraftSystem;
            }
        }
        public Type TypeRes
        {
            get
            {
                return this.m_TypeRes;
            }
        }
        public ITool Tool
        {
            get
            {
                return this.m_Tool;
            }
        }
        public int Quality
        {
            get
            {
                return this.m_Quality;
            }
        }
        public abstract void EndCraftAction();

        public abstract Item CompleteCraft(out int message);
    }
}