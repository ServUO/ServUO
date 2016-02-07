using System;
using Server.Mobiles;

namespace Server.Gumps
{
    public class ConfirmRewardGump : BaseConfirmGump
    { 
        private readonly IComunityCollection m_Collection;
        private readonly Point3D m_Location;
        private readonly CollectionItem m_Item;
        private readonly int m_Hue;
        public ConfirmRewardGump(IComunityCollection collection, Point3D location, CollectionItem item)
            : this(collection, location, item, 0)
        {
        }

        public ConfirmRewardGump(IComunityCollection collection, Point3D location, CollectionItem item, int hue)
            : base()
        {
            this.m_Collection = collection;
            this.m_Location = location;
            this.m_Item = item;
            this.m_Hue = hue;
			
            if (this.m_Item != null)			
                this.AddItem(150, 100, this.m_Item.ItemID, this.m_Item.Hue);
        }

        public override int TitleNumber
        {
            get
            {
                return 1074974;
            }
        }// Confirm Selection
        public override int LabelNumber
        {
            get
            {
                return 1074975;
            }
        }// Are you sure you wish to select this?
        public override void Confirm(Mobile from)
        { 
            if (this.m_Collection == null || !from.InRange(this.m_Location, 2))
                return;
			
            if (from is PlayerMobile)	
                this.m_Collection.Reward((PlayerMobile)from, this.m_Item, this.m_Hue);
        }
    }
}