using Server.Mobiles;
using Server.Multis;
using Server.Network;
using System;
using System.Collections;

namespace Server.Gumps
{
    public class VendorInventoryGump : Gump
    {
        private readonly BaseHouse m_House;
        private readonly ArrayList m_Inventories;

        public VendorInventoryGump(BaseHouse house, Mobile from)
            : base(50, 50)
        {
            m_House = house;
            m_Inventories = new ArrayList(house.VendorInventories);

            AddBackground(0, 0, 420, 50 + 20 * m_Inventories.Count, 0x13BE);

            AddImageTiled(10, 10, 400, 20, 0xA40);
            AddHtmlLocalized(15, 10, 200, 20, 1062435, 0x7FFF, false, false); // Reclaim Vendor Inventory
            AddHtmlLocalized(330, 10, 50, 20, 1062465, 0x7FFF, false, false); // Expires

            AddImageTiled(10, 40, 400, 20 * m_Inventories.Count, 0xA40);

            for (int i = 0; i < m_Inventories.Count; i++)
            {
                VendorInventory inventory = (VendorInventory)m_Inventories[i];

                int y = 40 + 20 * i;

                if (inventory.Owner == from)
                    AddButton(10, y, 0xFA5, 0xFA7, i + 1, GumpButtonType.Reply, 0);

                AddLabel(45, y, 0x481, string.Format("{0} ({1})", inventory.ShopName, inventory.VendorName));

                TimeSpan expire = inventory.ExpireTime - DateTime.UtcNow;
                int hours = (int)expire.TotalHours;

                AddLabel(320, y, 0x481, hours.ToString());
                AddHtmlLocalized(350, y, 50, 20, 1062466, 0x7FFF, false, false); // hour(s)
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            Mobile from = sender.Mobile;
            HouseSign sign = m_House.Sign;

            if (m_House.Deleted || sign == null || sign.Deleted || !from.CheckAlive())
                return;

            if (from.Map != sign.Map || !from.InRange(sign, 5))
            {
                from.SendLocalizedMessage(1062429); // You must be within five paces of the house sign to use this option.
                return;
            }

            int index = info.ButtonID - 1;
            if (index < 0 || index >= m_Inventories.Count)
                return;

            VendorInventory inventory = (VendorInventory)m_Inventories[index];

            if (inventory.Owner != from || !m_House.VendorInventories.Contains(inventory))
                return;

            int totalItems = 0;
            int givenToBackpack = 0;
            int givenToBankBox = 0;

            for (int i = inventory.Items.Count - 1; i >= 0; i--)
            {
                Item item = inventory.Items[i];

                if (item.Deleted)
                {
                    inventory.Items.RemoveAt(i);
                    continue;
                }

                totalItems += 1 + item.TotalItems;

                if (from.PlaceInBackpack(item))
                {
                    inventory.Items.RemoveAt(i);
                    givenToBackpack += 1 + item.TotalItems;
                }
                else if (from.BankBox.TryDropItem(from, item, false))
                {
                    inventory.Items.RemoveAt(i);
                    givenToBankBox += 1 + item.TotalItems;
                }
            }

            from.SendLocalizedMessage(1062436, totalItems.ToString() + "\t" + inventory.Gold.ToString()); // The vendor you selected had ~1_COUNT~ items in its inventory, and ~2_AMOUNT~ gold in its account.

            int givenGold = Banker.DepositUpTo(from, inventory.Gold);
            inventory.Gold -= givenGold;

            from.SendLocalizedMessage(1060397, givenGold.ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.
            from.SendLocalizedMessage(1062437, givenToBackpack.ToString() + "\t" + givenToBankBox.ToString()); // ~1_COUNT~ items have been removed from the shop inventory and placed in your backpack.  ~2_BANKCOUNT~ items were removed from the shop inventory and placed in your bank box.

            if (inventory.Gold > 0 || inventory.Items.Count > 0)
            {
                from.SendLocalizedMessage(1062440); // Some of the shop inventory would not fit in your backpack or bank box.  Please free up some room and try again.
            }
            else
            {
                inventory.Delete();
                from.SendLocalizedMessage(1062438); // The shop is now empty of inventory and funds, so it has been deleted.
            }
        }
    }
}