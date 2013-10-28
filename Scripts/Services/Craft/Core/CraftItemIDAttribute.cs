using System;

namespace Server.Engines.Craft
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CraftItemIDAttribute : Attribute
    {
        private readonly int m_ItemID;
        public CraftItemIDAttribute(int itemID)
        {
            this.m_ItemID = itemID;
        }

        public int ItemID
        {
            get
            {
                return this.m_ItemID;
            }
        }
    }
}