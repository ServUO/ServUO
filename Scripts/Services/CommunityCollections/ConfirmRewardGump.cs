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
            m_Collection = collection;
            m_Location = location;
            m_Item = item;
            m_Hue = hue;

            if (m_Item != null)
                AddItem(150, 100, m_Item.ItemID, m_Item.Hue);
        }

        public override int TitleNumber => 1074974;// Confirm Selection
        public override int LabelNumber => 1074975;// Are you sure you wish to select this?
        public override void Confirm(Mobile from)
        {
            if (m_Collection == null || !from.InRange(m_Location, 2))
                return;

            if (from is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)from;
                if (player.GetCollectionPoints(m_Collection.CollectionID) < m_Item.Points)
                {
                    player.SendLocalizedMessage(1073122); // You don't have enough points for that!
                }
                else if (m_Item.CanSelect(player))
                {
                    m_Collection.Reward(player, m_Item, m_Hue);
                }
            }
        }
    }
}