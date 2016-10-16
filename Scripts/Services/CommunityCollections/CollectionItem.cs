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
            this.m_Type = type;
            this.m_ItemID = itemID;
            this.m_Tooltip = tooltip;
            this.m_Hue = hue;
            this.m_Points = points;
            this.m_QuestItem = questitem;

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

                Item.Measure(Item.GetBitmap(this.m_ItemID), out this.m_X, out this.m_Y, out mx, out my);

                this.m_Width = mx - this.m_X;
                this.m_Height = my - this.m_Y;
            }
            else
            {
                m_X = rec.X;
                m_Y = rec.Y;
                m_Width = rec.Width;
                m_Height = rec.Height;
            }
        }

        public Type Type
        {
            get
            {
                return this.m_Type;
            }
        }
        // image info
        public int ItemID
        {
            get
            {
                return this.m_ItemID;
            }
        }
        public int X
        {
            get
            {
                return this.m_X;
            }
        }
        public int Y
        {
            get
            {
                return this.m_Y;
            }
        }
        public int Width
        {
            get
            {
                return this.m_Width;
            }
        }
        public int Height
        {
            get
            {
                return this.m_Height;
            }
        }
        public int Tooltip
        {
            get
            {
                return this.m_Tooltip;
            }
        }
        public int Hue
        {
            get
            {
                return this.m_Hue;
            }
        }
        public double Points
        {
            get
            {
                return this.m_Points;
            }
        }

        public bool QuestItem { get { return m_QuestItem; } }

        public virtual bool Validate(PlayerMobile from, Item item)
        {
            return true;
        }

        public virtual void OnGiveReward(PlayerMobile to, IComunityCollection collection, int hue)
        {
        }
    }

    public class CollectionHuedItem : CollectionItem
    {
        private readonly int[] m_Hues;
        public CollectionHuedItem(Type type, int itemID, int tooltip, int hue, double points, int[] hues)
            : base(type, itemID, tooltip, hue, points)
        {
            this.m_Hues = hues;
        }

        public int[] Hues
        {
            get
            {
                return this.m_Hues;
            }
        }
    }

    public class CollectionTitle : CollectionItem
    { 
        private readonly object m_Title;
        public CollectionTitle(object title, int tooltip, double points)
            : base(null, 0xFF1, tooltip, 0x0, points)
        {
            this.m_Title = title;
        }

        public object Title
        {
            get
            {
                return this.m_Title;
            }
        }
        public override void OnGiveReward(PlayerMobile to, IComunityCollection collection, int hue)
        {
            if (to.AddCollectionTitle(this.m_Title))
            {
                if (this.m_Title is int)
                    to.SendLocalizedMessage(1073625, "#" + (int)this.m_Title); // The title "~1_TITLE~" has been bestowed upon you. 
                else if (this.m_Title is string)
                    to.SendLocalizedMessage(1073625, (string)this.m_Title); // The title "~1_TITLE~" has been bestowed upon you. 
					
                to.AddCollectionPoints(collection.CollectionID, (int)this.Points * -1);				
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
            this.m_Level = level;
        }

        public int Level
        {
            get
            {
                return this.m_Level;
            }
        }
        public override bool Validate(PlayerMobile from, Item item)
        {
            TreasureMap map = (TreasureMap)item;
			
            if (map.Level == this.m_Level)
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
            this.m_Type = type;
        }

        public SpellbookType SpellbookType
        {
            get
            {
                return this.m_Type;
            }
        }
        public override bool Validate(PlayerMobile from, Item item)
        {
            Spellbook spellbook = (Spellbook)item;
			
            if (spellbook.SpellbookType == this.m_Type && spellbook.Content == 0)
                return true;
			
            return false;
        }
    }
}