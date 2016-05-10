using Server.ContextMenus;
using Server.Multis;
using Server.Network;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class SetPowerToolAnimationEntry : ContextMenuEntry
    {
        private readonly Item m_Item;
        private readonly Mobile m_From;

        public SetPowerToolAnimationEntry(Item item, Mobile from)
            : base(1155742, 6)
        {
            this.m_Item = item;
            this.m_From = from;
        }
        
        public static void AddTo(Mobile from, Item item, List<ContextMenuEntry> list)
        {
            BaseHouse house = BaseHouse.FindHouseAt(item);

            if (house == null && from.AccessLevel <= AccessLevel.VIP)
            {
                return;
            }

            bool accessable = false;
            switch ((item as SpecialVeteranCraftAddon).Level)
            {
                case SecureLevel.Owner:
                    accessable = house.IsOwner(from);
                    break;
                case SecureLevel.CoOwners:
                    accessable = house.IsCoOwner(from);
                    break;
                case SecureLevel.Friends:
                    accessable = house.IsFriend(from);
                    break;
                case SecureLevel.Guild:
                    accessable = house.IsGuildMember(from);
                    break;
                case SecureLevel.Anyone: accessable = true;
                    break;
            }
            if (accessable)
            {
                list.Add(new SetPowerToolAnimationEntry(item, from));
            }
        }

        public override void OnClick()
        {
            (m_Item as SpecialVeteranCraftAddon).ShowAnimation = !(m_Item as SpecialVeteranCraftAddon).ShowAnimation;
            //m_Item.LabelTo(m_From, 1155743);

            m_From.NetState.Send(new MessageLocalized(m_Item.Serial, m_Item.ItemID, MessageType.Label, 0x3B2, 3, 1155743, "", ""));
            /* 
            * Again not sure what to use since it seems more fitting to place it either over the item or the player
            * but public seems wrong in thise case too.
            */
            // m_From.SendLocalizedMessage(1155743);
        }
    }
}
