using Server.Accounting;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Prompts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Gumps
{
    public class CommunityCollectionGump : Gump
    {
        private readonly PlayerMobile m_Owner;
        private readonly IComunityCollection m_Collection;
        private readonly Point3D m_Location;
        private readonly Section m_Section;
        private readonly CollectionHuedItem m_Item;
        private int m_Index;
        private int m_Page;
        private int m_Max;

        public CommunityCollectionGump(PlayerMobile from, IComunityCollection collection, Point3D location)
            : this(from, collection, location, Section.Donates)
        {
        }

        public CommunityCollectionGump(PlayerMobile from, IComunityCollection collection, Point3D location, Section section)
            : this(from, collection, location, section, null)
        {
        }

        public CommunityCollectionGump(PlayerMobile from, IComunityCollection collection, Point3D location, Section section, CollectionHuedItem item)
            : base(250, 50)
        {
            m_Owner = from;
            m_Collection = collection;
            m_Location = location;
            m_Section = section;
            m_Item = item;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            AddImage(0, 0, 0x1F40);
            AddImageTiled(20, 37, 300, 308, 0x1F42);
            AddImage(20, 325, 0x1F43);
            AddImage(35, 8, 0x39);
            AddImageTiled(65, 8, 257, 10, 0x3A);
            AddImage(290, 8, 0x3B);
            AddImage(32, 33, 0x2635);
            AddImageTiled(70, 55, 230, 2, 0x23C5);

            AddHtmlLocalized(70, 35, 270, 20, 1072835, 0x1, false, false); // Community Collection

            // add pages
            if (m_Collection == null)
                return;

            m_Index = 0;
            m_Page = 1;

            switch (m_Section)
            {
                case Section.Donates:
                    GetMax(m_Collection.Donations);

                    while (m_Collection.Donations != null && m_Index < m_Collection.Donations.Count)
                        DisplayDonationPage();
                    break;
                case Section.Rewards:
                    GetMax(m_Collection.Rewards);

                    while (m_Collection.Rewards != null && m_Index < m_Collection.Rewards.Count)
                        DisplayRewardPage();
                    break;
                case Section.Hues:
                    while (m_Item != null && m_Index < m_Item.Hues.Length)
                        DisplayHuePage();
                    break;
            }
        }

        public enum Section
        {
            Donates,
            Rewards,
            Hues,
        }

        private enum Buttons
        {
            Close,
            Rewards,
            Status,
            Next,
        }
        public void GetMax(List<CollectionItem> list)
        {
            m_Max = 0;

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                    if (m_Max < list[i].Width)
                        m_Max = list[i].Width;
            }
        }

        public void DisplayDonationPage()
        {
            AddPage(m_Page);

            // title
            AddHtmlLocalized(50, 65, 150, 20, 1072836, 0x1, false, false); // Current Tier:
            AddLabel(230, 65, 0x64, m_Collection.Tier.ToString());
            AddHtmlLocalized(50, 85, 150, 20, 1072837, 0x1, false, false); // Current Points:
            AddLabel(230, 85, 0x64, m_Collection.Points.ToString());
            AddHtmlLocalized(50, 105, 150, 20, 1072838, 0x1, false, false); // Points Until Next Tier:
            AddLabel(230, 105, 0x64, m_Collection.CurrentTier.ToString());

            AddImageTiled(35, 125, 270, 2, 0x23C5);
            AddHtmlLocalized(35, 130, 270, 20, 1072840, 0x1, false, false); // Donations Accepted:

            // donations
            int offset = 150;
            int next = 0;

            while (offset + next < 330 && m_Index < m_Collection.Donations.Count)
            {
                CollectionItem item = m_Collection.Donations[m_Index];
                Type type = item.Type;
                Account acct = m_Owner.Account as Account;

                int height = Math.Max(item.Height, 20);

                int amount = 0;

                if (item.Type == typeof(Gold) && acct != null)
                {
                    amount = acct.TotalGold + m_Owner.Backpack.GetAmount(item.Type);
                }
                else
                {
                    amount = GetTypes(m_Owner, item);
                }

                if (amount > 0)
                {
                    AddButton(35, offset + height / 2 - 5, 0x837, 0x838, 300 + m_Index, GumpButtonType.Reply, 0);
                    TextDefinition.AddTooltip(this, item.Tooltip);
                }

                int y = offset - item.Y;

                if (item.Height < 20)
                    y += (20 - item.Height) / 2;

                AddItem(55 - item.X + m_Max / 2 - item.Width / 2, y, item.ItemID, item.Hue);
                TextDefinition.AddTooltip(this, item.Tooltip);

                if (item.Points < 1 && item.Points > 0)
                    AddLabel(65 + m_Max, offset + height / 2 - 10, 0x64, "1 per " + ((int)Math.Pow(item.Points, -1)).ToString());
                else
                    AddLabel(65 + m_Max, offset + height / 2 - 10, 0x64, item.Points.ToString());

                TextDefinition.AddTooltip(this, item.Tooltip);

                if (amount > 0)
                    AddLabel(235, offset + height / 2 - 5, 0xB1, amount.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US")));

                offset += 5 + height;
                m_Index += 1;

                if (m_Index < m_Collection.Donations.Count)
                    next = Math.Max(m_Collection.Donations[m_Index].Height, 20);
                else
                    next = 0;
            }

            // buttons
            AddButton(50, 335, 0x15E3, 0x15E7, (int)Buttons.Rewards, GumpButtonType.Reply, 0);
            AddHtmlLocalized(75, 335, 100, 20, 1072842, 0x1, false, false); // Rewards

            if (m_Page > 1)
            {
                AddButton(150, 335, 0x15E3, 0x15E7, (int)Buttons.Next, GumpButtonType.Page, m_Page - 1);
                AddHtmlLocalized(170, 335, 60, 20, 1074880, 0x1, false, false); // Previous			
            }

            m_Page += 1;

            if (m_Index < m_Collection.Donations.Count)
            {
                AddButton(300, 335, 0x15E1, 0x15E5, (int)Buttons.Next, GumpButtonType.Page, m_Page);
                AddHtmlLocalized(240, 335, 60, 20, 1072854, 0x1, false, false); // <div align=right>Next</div>
            }
        }

        public void DisplayRewardPage()
        {
            int points = m_Owner.GetCollectionPoints(m_Collection.CollectionID);

            AddPage(m_Page);

            // title
            AddHtmlLocalized(50, 65, 150, 20, 1072843, 0x1, false, false); // Your Reward Points:
            AddLabel(230, 65, 0x64, points.ToString());
            AddImageTiled(35, 85, 270, 2, 0x23C5);
            AddHtmlLocalized(35, 90, 270, 20, 1072844, 0x1, false, false); // Please Choose a Reward:

            // rewards
            int offset = 110;
            int next = 0;

            while (offset + next < 300 && m_Index < m_Collection.Rewards.Count)
            {
                CollectionItem item = m_Collection.Rewards[m_Index];

                if (item.QuestItem && SkipQuestReward(m_Owner, item))
                {
                    m_Index++;
                    continue;
                }

                int height = Math.Max(item.Height, 20);

                if (points >= item.Points && item.CanSelect(m_Owner))
                {
                    AddButton(35, offset + height / 2 - 5, 0x837, 0x838, 200 + m_Index, GumpButtonType.Reply, 0);
                    TextDefinition.AddTooltip(this, item.Tooltip);
                }

                int y = offset - item.Y;

                if (item.Height < 20)
                    y += (20 - item.Height) / 2;

                AddItem(55 - item.X + m_Max / 2 - item.Width / 2, y, item.ItemID, points >= item.Points ? item.Hue : 0x3E9);
                TextDefinition.AddTooltip(this, item.Tooltip);
                AddLabel(65 + m_Max, offset + height / 2 - 10, points >= item.Points ? 0x64 : 0x21, item.Points.ToString());
                TextDefinition.AddTooltip(this, item.Tooltip);

                offset += 5 + height;
                m_Index += 1;

                if (m_Index < m_Collection.Donations.Count)
                    next = Math.Max(m_Collection.Donations[m_Index].Height, 20);
                else
                    next = 0;
            }

            // buttons
            AddButton(50, 335, 0x15E3, 0x15E7, (int)Buttons.Status, GumpButtonType.Reply, 0);
            AddHtmlLocalized(75, 335, 100, 20, 1072845, 0x1, false, false); // Status

            if (m_Page > 1)
            {
                AddButton(150, 335, 0x15E3, 0x15E7, (int)Buttons.Next, GumpButtonType.Page, m_Page - 1);
                AddHtmlLocalized(170, 335, 60, 20, 1074880, 0x1, false, false); // Previous			
            }

            m_Page += 1;

            if (m_Index < m_Collection.Rewards.Count)
            {
                AddButton(300, 335, 0x15E1, 0x15E5, (int)Buttons.Next, GumpButtonType.Page, m_Page);
                AddHtmlLocalized(240, 335, 60, 20, 1072854, 0x1, false, false); // <div align=right>Next</div>
            }
        }

        public void DisplayHuePage()
        {
            int points = m_Owner.GetCollectionPoints(m_Collection.CollectionID);

            AddPage(m_Page);

            // title
            AddHtmlLocalized(50, 65, 150, 20, 1072843, 0x1, false, false); // Your Reward Points:
            AddLabel(230, 65, 0x64, points.ToString());

            AddImageTiled(35, 85, 270, 2, 0x23C5);

            AddHtmlLocalized(35, 90, 270, 20, 1074255, 0x1, false, false); // Please select a hue for your Reward:

            // hues
            int height = Math.Max(m_Item.Height, 20);
            int offset = 110;

            while (offset + height < 290 && m_Index < m_Item.Hues.Length)
            {
                AddButton(35, offset + height / 2 - 5, 0x837, 0x838, 100 + m_Index, GumpButtonType.Reply, 0);
                TextDefinition.AddTooltip(this, m_Item.Tooltip);

                AddItem(55 - m_Item.X, offset - m_Item.Y, m_Item.ItemID, m_Item.Hues[m_Index]);
                TextDefinition.AddTooltip(this, m_Item.Tooltip);

                offset += 5 + height;
                m_Index += 1;
            }

            m_Page += 1;

            // buttons			
            AddButton(50, 335, 0x15E3, 0x15E7, (int)Buttons.Rewards, GumpButtonType.Reply, 0);
            AddHtmlLocalized(75, 335, 100, 20, 1072842, 0x1, false, false); // Rewards

            if (m_Index < m_Item.Hues.Length && m_Page > 2)
            {
                if (m_Index < m_Item.Hues.Length)
                    AddButton(270, 335, 0x15E1, 0x15E5, (int)Buttons.Next, GumpButtonType.Page, m_Page);
                else
                    AddButton(270, 335, 0x15E1, 0x15E5, (int)Buttons.Next, GumpButtonType.Page, 1);

                AddHtmlLocalized(210, 335, 60, 20, 1074256, 0x1, false, false); // More Hues
            }
        }

        public override void OnResponse(Network.NetState state, RelayInfo info)
        {
            if (m_Collection == null || !m_Owner.InRange(m_Location, 2))
                return;

            if (info.ButtonID == (int)Buttons.Rewards)
                m_Owner.SendGump(new CommunityCollectionGump(m_Owner, m_Collection, m_Location, Section.Rewards));
            else if (info.ButtonID == (int)Buttons.Status)
                m_Owner.SendGump(new CommunityCollectionGump(m_Owner, m_Collection, m_Location, Section.Donates));
            else if (info.ButtonID >= 300 && m_Collection.Donations != null && info.ButtonID - 300 < m_Collection.Donations.Count && m_Section == Section.Donates)
            {
                CollectionItem item = m_Collection.Donations[info.ButtonID - 300];

                m_Owner.SendLocalizedMessage(1073178); // Please enter how much of that item you wish to donate:
                m_Owner.Prompt = new InternalPrompt(m_Collection, item, m_Location);
            }
            else if (info.ButtonID >= 200 && m_Collection.Rewards != null && info.ButtonID - 200 < m_Collection.Rewards.Count && m_Section == Section.Rewards)
            {
                CollectionItem item = m_Collection.Rewards[info.ButtonID - 200];
                int points = m_Owner.GetCollectionPoints(m_Collection.CollectionID);

                if (item.CanSelect(m_Owner))
                {
                    if (item.Points <= points)
                    {
                        if (item is CollectionHuedItem)
                        {
                            m_Owner.SendGump(new CommunityCollectionGump(m_Owner, m_Collection, m_Location, Section.Hues, (CollectionHuedItem)item));
                        }
                        else
                        {
                            m_Owner.CloseGump(typeof(ConfirmRewardGump));
                            m_Owner.SendGump(new ConfirmRewardGump(m_Collection, m_Location, item, 0));
                        }
                    }
                    else
                        m_Owner.SendLocalizedMessage(1073122); // You don't have enough points for that!
                }
            }
            else if (info.ButtonID >= 100 && m_Item != null && info.ButtonID - 200 < m_Item.Hues.Length && m_Section == Section.Hues)
            {
                m_Owner.CloseGump(typeof(ConfirmRewardGump));
                m_Owner.SendGump(new ConfirmRewardGump(m_Collection, m_Location, m_Item, m_Item.Hues[info.ButtonID - 100]));
            }
        }

        private class InternalPrompt : Prompt
        {
            private readonly IComunityCollection m_Collection;
            private readonly CollectionItem m_Selected;
            private readonly Point3D m_Location;
            public InternalPrompt(IComunityCollection collection, CollectionItem selected, Point3D location)
            {
                m_Collection = collection;
                m_Selected = selected;
                m_Location = location;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!from.InRange(m_Location, 2) || !(from is PlayerMobile))
                    return;

                HandleResponse(from, text);
                from.SendGump(new CommunityCollectionGump((PlayerMobile)from, m_Collection, m_Location));
            }

            private void HandleResponse(Mobile from, string text)
            {
                int amount = Utility.ToInt32(text);

                if (amount <= 0)
                {
                    from.SendLocalizedMessage(1073181); // That is not a valid donation quantity.
                    return;
                }

                if (from.Backpack == null)
                    return;

                if (m_Selected.Type == typeof(Gold))
                {
                    if (amount * m_Selected.Points < 1)
                    {
                        from.SendLocalizedMessage(1073167); // You do not have enough of that item to make a donation!
                        from.SendGump(new CommunityCollectionGump((PlayerMobile)from, m_Collection, m_Location));
                        return;
                    }

                    Item[] items = from.Backpack.FindItemsByType(m_Selected.Type, true);
                    Account acct = from.Account as Account;

                    int goldcount = 0;
                    int accountcount = acct == null ? 0 : acct.TotalGold;
                    int amountRemaining = amount;
                    foreach (Item item in items)
                        goldcount += item.Amount;

                    if (goldcount >= amountRemaining)
                    {
                        foreach (Item item in items)
                        {
                            if (item.Amount <= amountRemaining)
                            {
                                item.Delete();
                                amountRemaining -= item.Amount;
                            }
                            else
                            {
                                item.Amount -= amountRemaining;
                                amountRemaining = 0;
                            }

                            if (amountRemaining == 0)
                                break;
                        }
                    }
                    else if (goldcount + accountcount >= amountRemaining)
                    {
                        foreach (Item item in items)
                        {
                            amountRemaining -= item.Amount;
                            item.Delete();
                        }

                        Banker.Withdraw(from, amountRemaining);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1073182); // You do not have enough to make a donation of that magnitude!
                        from.SendGump(new CommunityCollectionGump((PlayerMobile)from, m_Collection, m_Location));
                        return;
                    }

                    from.Backpack.ConsumeTotal(m_Selected.Type, amount, true, true);
                    m_Collection.Donate((PlayerMobile)from, m_Selected, amount);
                }
                else
                {
                    if (amount * m_Selected.Points < 1)
                    {
                        from.SendLocalizedMessage(1073167); // You do not have enough of that item to make a donation!
                        from.SendGump(new CommunityCollectionGump((PlayerMobile)from, m_Collection, m_Location));
                        return;
                    }

                    List<Item> items = FindTypes((PlayerMobile)from, m_Selected);

                    if (items.Count > 0)
                    {
                        // count items
                        int count = 0;

                        for (int i = 0; i < items.Count; i++)
                        {
                            Item item = GetActual(items[i]);

                            if (item != null && !item.Deleted)
                                count += item.Amount;
                        }

                        // check
                        if (amount > count)
                        {
                            from.SendLocalizedMessage(1073182); // You do not have enough to make a donation of that magnitude!
                            from.SendGump(new CommunityCollectionGump((PlayerMobile)from, m_Collection, m_Location));
                            return;
                        }
                        else if (amount * m_Selected.Points < 1)
                        {
                            from.SendLocalizedMessage(m_Selected.Type == typeof(Gold) ? 1073182 : 1073167); // You do not have enough of that item to make a donation!
                            from.SendGump(new CommunityCollectionGump((PlayerMobile)from, m_Collection, m_Location));
                            return;
                        }

                        // donate
                        int deleted = 0;

                        for (int i = 0; i < items.Count && deleted < amount; i++)
                        {
                            Item item = GetActual(items[i]);

                            if (item == null || item.Deleted)
                            {
                                continue;
                            }

                            if (item.Stackable && item.Amount + deleted > amount && !item.Deleted)
                            {
                                item.Amount -= amount - deleted;
                                deleted += amount - deleted;
                            }
                            else if (!item.Deleted)
                            {
                                deleted += item.Amount;
                                items[i].Delete();
                            }

                            if (items[i] is CommodityDeed && !items[i].Deleted)
                            {
                                items[i].InvalidateProperties();
                            }
                        }

                        m_Collection.Donate((PlayerMobile)from, m_Selected, amount);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1073182); // You do not have enough to make a donation of that magnitude!
                    }

                    ColUtility.Free(items);
                }
            }

            public override void OnCancel(Mobile from)
            {
                if (!(from is PlayerMobile))
                    return;

                from.SendLocalizedMessage(1073184); // You cancel your donation.

                if (from.InRange(m_Location, 2))
                    from.SendGump(new CommunityCollectionGump((PlayerMobile)from, m_Collection, m_Location));
            }
        }

        private bool SkipQuestReward(PlayerMobile pm, CollectionItem item)
        {
            if (pm.Quests != null)
            {
                foreach (BaseQuest q in pm.Quests)
                {
                    if (!q.Completed)
                    {
                        foreach (BaseObjective obj in q.Objectives)
                        {
                            if (obj is CollectionsObtainObjective && item.Type == ((CollectionsObtainObjective)obj).Obtain)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool CheckType(Item item, Type type, bool checkDerives)
        {
            if (item is CommodityDeed && ((CommodityDeed)item).Commodity != null)
            {
                item = ((CommodityDeed)item).Commodity;
            }

            Type t = item.GetType();

            if (type == t)
            {
                return true;
            }
            else if (!checkDerives)
            {
                return false;
            }
            else
            {
                if (type == typeof(Lobster) && BaseHighseasFish.Lobsters.Any(x => x == t))
                {
                    return true;
                }
                else if (type == typeof(Crab) && BaseHighseasFish.Crabs.Any(x => x == t))
                {
                    return true;
                }
                else if (type == typeof(Fish) && t != typeof(BaseCrabAndLobster) && !t.IsSubclassOf(typeof(BaseCrabAndLobster)) && (t.IsSubclassOf(type) || t == typeof(BaseHighseasFish) || t.IsSubclassOf(typeof(BaseHighseasFish))))
                {
                    return true;
                }
                else
                {
                    return t.IsSubclassOf(type);
                }
            }
        }

        public static int GetTypes(PlayerMobile pm, CollectionItem colItem)
        {
            Type type = colItem.Type;
            bool derives = type == typeof(BaseScales) || type == typeof(Fish) || type == typeof(Crab) || type == typeof(Lobster);

            int count = 0;

            foreach (Item item in pm.Backpack.Items)
            {
                if (CheckType(item, type, derives) && colItem.Validate(pm, GetActual(item)))
                {
                    if (item is CommodityDeed)
                    {
                        count += ((CommodityDeed)item).Commodity.Amount;
                    }
                    else
                    {
                        count += item.Amount;
                    }
                }
            }

            return count;
        }

        public static List<Item> FindTypes(PlayerMobile pm, CollectionItem colItem)
        {
            Type type = colItem.Type;
            bool derives = type == typeof(BaseScales) || type == typeof(Fish) || type == typeof(Crab) || type == typeof(Lobster);

            List<Item> list = new List<Item>();

            foreach (Item item in pm.Backpack.Items)
            {
                if (CheckType(item, type, derives) && colItem.Validate(pm, GetActual(item)))
                {
                    list.Add(item);
                }
            }

            return list;
        }

        public static Item GetActual(Item item)
        {
            if (item is CommodityDeed)
            {
                return ((CommodityDeed)item).Commodity;
            }

            return item;
        }
    }
}
