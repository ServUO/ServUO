using System;

namespace Server.Engines.Craft
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CraftItemIDAttribute : Attribute
    {
        private readonly int m_ItemID;
        public CraftItemIDAttribute(int itemID)
        {
            m_ItemID = itemID;
        }

        public int ItemID => m_ItemID;
    }
}