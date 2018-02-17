using System;
using Server.Items;
using Server.Mobiles;

namespace Server
{
    public class CollectionItem 
    {
        private readonly Type m_Type;
        private readonly int m_ItemID;
        private readonly int m_X;
        private readonly int m_Y;
        private readonly int m_Width;
        private readonly int m_Height;
        private readonly int m_Tooltip;
        private readonly int m_Hue;
        private readonly double m_Points;
        private readonly bool m_QuestItem;

        public CollectionItem(Type type, int itemID, int tooltip, int hue, double points, bool questitem = false)
        {
            m_Type = type;
            m_ItemID = itemID;
            m_Tooltip = tooltip;
            m_Hue = hue;
            m_Points = points;
            m_QuestItem = questitem;

            Rectangle2D rec;

            try
            {
                rec = ItemBounds.Table[m_ItemID];
            }
            catch
            {
                rec = new Rectangle2D(0, 0, 0, 0);
            }

            if (rec.X == 0 && rec.Y == 0 && rec.Width == 0 && rec.Height == 0)
            {
                int mx, my;
                mx = my = 0;

                Item.Measure(Item.GetBitmap(m_ItemID), out m_X, out m_Y, out mx, out my);

                m_Width = mx - m_X;
                m_Height = my - m_Y;
            }
            else
            {
                m_X = rec.X;
                m_Y = rec.Y;
                m_Width = rec.Width;
                m_Height = rec.Height;
            }
        }

        public Type Type { get { return m_Type; } } // image info
        public int ItemID { get { return m_ItemID; } }
        public int X { get { return m_X; } }
        public int Y { get { return m_Y; } }
        public int Width { get { return m_Width; } }
        public int Height { get { return m_Height; } }
        public int Tooltip { get { return m_Tooltip; } }
        public int Hue { get { return m_Hue; } }
        public double Points { get { return m_Points; } }
        public bool QuestItem { get { return m_QuestItem; } }

        public virtual bool Validate(PlayerMobile from, Item item)
        {
            return true;
        }

        public virtual bool CanSelect(PlayerMobile from)
        {
            return true;
        }

        public virtual void OnGiveReward(PlayerMobile to, Item item, IComunityCollection collection, int hue)
        {
        }
    }

    public class CollectionHuedItem : CollectionItem
    {
        private readonly int[] m_Hues;

        public CollectionHuedItem(Type type, int itemID, int tooltip, int hue, double points, int[] hues)
            : base(type, itemID, tooltip, hue, points)
        {
            m_Hues = hues;
        }

        public int[] Hues { get { return m_Hues; } }
    }

    public class CollectionTitle : CollectionItem
    { 
        private readonly object m_Title;

        public CollectionTitle(object title, int tooltip, double points)
            : base(null, 0xFF1, tooltip, 0x0, points)
        {
            m_Title = title;
        }

        public object Title { get { return m_Title; } }

        public override void OnGiveReward(PlayerMobile to, Item item, IComunityCollection collection, int hue)
        {
            if (to.AddRewardTitle(m_Title))
            {
                if (m_Title is int)
                    to.SendLocalizedMessage(1073625, "#" + (int)m_Title); // The title "~1_TITLE~" has been bestowed upon you. 
                else if (m_Title is string)
                    to.SendLocalizedMessage(1073625, (string)m_Title); // The title "~1_TITLE~" has been bestowed upon you. 
					
                to.AddCollectionPoints(collection.CollectionID, (int)Points * -1);				
            }
            else
                to.SendLocalizedMessage(1073626); // You already have that title!
        }
    }

    public class CollectionTreasureMap : CollectionItem
    {
        private readonly int m_Level;

        public CollectionTreasureMap(int level, int tooltip, double points)
            : base(typeof(TreasureMap), 0x14EB, tooltip, 0x0, points)
        {
            m_Level = level;
        }

        public int Level { get { return m_Level; } }

        public override bool Validate(PlayerMobile from, Item item)
        {
            TreasureMap map = (TreasureMap)item;
			
            if (map.Level == m_Level)
                return true;
			
            return false;
        }
    }

    public class CollectionSpellbook : CollectionItem
    {
        private readonly SpellbookType m_Type;

        public CollectionSpellbook(SpellbookType type, int itemID, int tooltip, double points)
            : base(typeof(Spellbook), itemID, tooltip, 0x0, points)
        {
            m_Type = type;
        }

        public SpellbookType SpellbookType { get { return m_Type; } }

        public override bool Validate(PlayerMobile from, Item item)
        {
            Spellbook spellbook = (Spellbook)item;
			
            if (spellbook.SpellbookType == m_Type && spellbook.Content == 0)
                return true;
			
            return false;
        }
    }
}