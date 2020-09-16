using Server.Items;
using Server.Mobiles;
using System;

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
        private readonly TextDefinition m_Tooltip;
        private readonly int m_Hue;
        private readonly double m_Points;
        private readonly bool m_QuestItem;

        public CollectionItem(Type type, int itemID, TextDefinition tooltip, int hue, double points, bool questitem = false)
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
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
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

        public Type Type => m_Type;  // image info
        public int ItemID => m_ItemID;
        public int X => m_X;
        public int Y => m_Y;
        public int Width => m_Width;
        public int Height => m_Height;
        public TextDefinition Tooltip => m_Tooltip;
        public int Hue => m_Hue;
        public double Points => m_Points;
        public bool QuestItem => m_QuestItem;

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

        public CollectionHuedItem(Type type, int itemID, TextDefinition tooltip, int hue, double points, int[] hues)
            : base(type, itemID, tooltip, hue, points)
        {
            m_Hues = hues;
        }

        public int[] Hues => m_Hues;
    }

    public class CollectionTitle : CollectionItem
    {
        private readonly object m_Title;

        public CollectionTitle(object title, TextDefinition tooltip, double points)
            : base(null, 0xFF1, tooltip, 0x0, points)
        {
            m_Title = title;
        }

        public object Title => m_Title;

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

        public CollectionTreasureMap(int level, TextDefinition tooltip, double points)
            : base(typeof(TreasureMap), 0x14EB, tooltip, 0x0, points)
        {
            m_Level = level;
        }

        public int Level => m_Level;

        public override bool Validate(PlayerMobile from, Item item)
        {
            TreasureMap map = item as TreasureMap;

            if (map != null && map.Level == m_Level)
                return true;

            return false;
        }
    }

    public class CollectionSpellbook : CollectionItem
    {
        private readonly SpellbookType m_Type;

        public CollectionSpellbook(SpellbookType type, int itemID, TextDefinition tooltip, double points)
            : base(typeof(Spellbook), itemID, tooltip, 0x0, points)
        {
            m_Type = type;
        }

        public SpellbookType SpellbookType => m_Type;

        public override bool Validate(PlayerMobile from, Item item)
        {
            Spellbook spellbook = item as Spellbook;

            if (spellbook != null && spellbook.SpellbookType == m_Type && spellbook.Content == 0)
                return true;

            return false;
        }
    }
}
