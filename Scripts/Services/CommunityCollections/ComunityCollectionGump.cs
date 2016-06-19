using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Prompts;

namespace Server.Gumps
{
    public class ComunityCollectionGump : Gump
    {
        private readonly PlayerMobile m_Owner;
        private readonly IComunityCollection m_Collection;
        private readonly Point3D m_Location;
        private readonly Section m_Section;
        private readonly CollectionHuedItem m_Item;
        private int m_Index;
        private int m_Page;
        private int m_Max;
        public ComunityCollectionGump(PlayerMobile from, IComunityCollection collection, Point3D location)
            : this(from, collection, location, Section.Donates)
        {
        }

        public ComunityCollectionGump(PlayerMobile from, IComunityCollection collection, Point3D location, Section section)
            : this(from, collection, location, section, null)
        {
        }

        public ComunityCollectionGump(PlayerMobile from, IComunityCollection collection, Point3D location, Section section, CollectionHuedItem item)
            : base(250, 50)
        {
            this.m_Owner = from;
            this.m_Collection = collection;
            this.m_Location = location;
            this.m_Section = section;
            this.m_Item = item;
		
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
			
            this.AddPage(0);
			
            this.AddImage(0, 0, 0x1F40);			
            this.AddImageTiled(20, 37, 300, 308, 0x1F42);			
            this.AddImage(20, 325, 0x1F43);			
            this.AddImage(35, 8, 0x39);			
            this.AddImageTiled(65, 8, 257, 10, 0x3A);			
            this.AddImage(290, 8, 0x3B);			
            this.AddImage(32, 33, 0x2635);			
            this.AddImageTiled(70, 55, 230, 2, 0x23C5);
			
            this.AddHtmlLocalized(70, 35, 270, 20, 1072835, 0x1, false, false); // Community Collection
			
            // add pages
            if (this.m_Collection == null)
                return;
			
            this.m_Index = 0;
            this.m_Page = 1;			
						
            switch ( this.m_Section )
            {
                case Section.Donates: 
                    this.GetMax(this.m_Collection.Donations);
				
                    while (this.m_Collection.Donations != null && this.m_Index < this.m_Collection.Donations.Count)
                        this.DisplayDonationPage();
                    break;
                case Section.Rewards:
                    this.GetMax(this.m_Collection.Rewards);
					
                    while (this.m_Collection.Rewards != null && this.m_Index < this.m_Collection.Rewards.Count)
                        this.DisplayRewardPage();
                    break;
                case Section.Hues:
                    while (this.m_Item != null && this.m_Index < this.m_Item.Hues.Length)
                        this.DisplayHuePage();
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
            this.m_Max = 0;
		
            if (list != null)
            {
                for (int i = 0; i < list.Count; i ++)
                    if (this.m_Max < list[i].Width)
                        this.m_Max = list[i].Width;
            }
        }

        public void DisplayDonationPage()
        { 
            this.AddPage(this.m_Page);
			
            // title
            this.AddHtmlLocalized(50, 65, 150, 20, 1072836, 0x1, false, false); // Current Tier:
            this.AddLabel(230, 65, 0x64, this.m_Collection.Tier.ToString());
            this.AddHtmlLocalized(50, 85, 150, 20, 1072837, 0x1, false, false); // Current Points:
            this.AddLabel(230, 85, 0x64, this.m_Collection.Points.ToString());
            this.AddHtmlLocalized(50, 105, 150, 20, 1072838, 0x1, false, false); // Points Until Next Tier:
            this.AddLabel(230, 105, 0x64, this.m_Collection.CurrentTier.ToString());
			
            this.AddImageTiled(35, 125, 270, 2, 0x23C5);
            this.AddHtmlLocalized(35, 130, 270, 20, 1072840, 0x1, false, false); // Donations Accepted:
			
            // donations
            int offset = 150;
            int next = 0;
			
            while (offset + next < 330 && this.m_Index < this.m_Collection.Donations.Count)
            {
                CollectionItem item = this.m_Collection.Donations[this.m_Index];

                int height = Math.Max(item.Height, 20);
				
                if (this.m_Owner.Backpack != null && this.m_Owner.Backpack.GetAmount(item.Type, true, true) > 0)
                {
                    this.AddButton(35, offset + (int)(height / 2) - 5, 0x837, 0x838, 300 + this.m_Index, GumpButtonType.Reply, 0);
                    this.AddTooltip(item.Tooltip);
                }

                int y = offset - item.Y;

                if (item.Height < 20)
                    y += (20 - item.Height) / 2;
					
                this.AddItem(55 - item.X + this.m_Max / 2 - item.Width / 2, y, item.ItemID, item.Hue);
                this.AddTooltip(item.Tooltip);
				
                if (item.Points < 1 && item.Points > 0)
                    this.AddLabel(65 + this.m_Max, offset + (int)(height / 2) - 10, 0x64, "1 per " + ((int)Math.Pow(item.Points, -1)).ToString());
                else 
                    this.AddLabel(65 + this.m_Max, offset + (int)(height / 2) - 10, 0x64, item.Points.ToString());
				
                this.AddTooltip(item.Tooltip);

                if(m_Owner.Backpack != null)
                {
                    int count = 0;
                    if (item.Type == typeof(BankCheck))
                        count = m_Owner.Backpack.GetChecksWorth(true);
                    else
                        count = m_Owner.Backpack.GetAmount(item.Type, true, true);
                    if (count > 0)
                    {
                        AddLabel(230, offset + (int)(height / 2) - 10, 0x64, count.ToString());
                    }
                }

                offset += 5 + height;
                this.m_Index += 1;

                if (this.m_Index < this.m_Collection.Donations.Count)
                    next = Math.Max(this.m_Collection.Donations[this.m_Index].Height, 20);
                else
                    next = 0;
            }
			
            // buttons
            this.AddButton(50, 335, 0x15E3, 0x15E7, (int)Buttons.Rewards, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(75, 335, 100, 20, 1072842, 0x1, false, false); // Rewards
			
            if (this.m_Page > 1)
            {
                this.AddButton(150, 335, 0x15E3, 0x15E7, (int)Buttons.Next, GumpButtonType.Page, this.m_Page - 1);
                this.AddHtmlLocalized(170, 335, 60, 20, 1074880, 0x1, false, false); // Previous			
            }
			
            this.m_Page += 1;
						
            if (this.m_Index < this.m_Collection.Donations.Count)
            {
                this.AddButton(300, 335, 0x15E1, 0x15E5, (int)Buttons.Next, GumpButtonType.Page, this.m_Page);
                this.AddHtmlLocalized(240, 335, 60, 20, 1072854, 0x1, false, false); // <div align=right>Next</div>
            }
        }

        public void DisplayRewardPage()
        {
            int points = this.m_Owner.GetCollectionPoints(this.m_Collection.CollectionID);
			
            this.AddPage(this.m_Page);
			
            // title
            this.AddHtmlLocalized(50, 65, 150, 20, 1072843, 0x1, false, false); // Your Reward Points:
            this.AddLabel(230, 65, 0x64, points.ToString());				
            this.AddImageTiled(35, 85, 270, 2, 0x23C5);			
            this.AddHtmlLocalized(35, 90, 270, 20, 1072844, 0x1, false, false); // Please Choose a Reward:
			
            // rewards
            int offset = 110;
            int next = 0;
			
            while (offset + next < 300 && this.m_Index < this.m_Collection.Rewards.Count)
            {
                CollectionItem item = this.m_Collection.Rewards[this.m_Index];

                int height = Math.Max(item.Height, 20);
				
                if (points >= item.Points)
                {
                    this.AddButton(35, offset + (int)(height / 2) - 5, 0x837, 0x838, 200 + this.m_Index, GumpButtonType.Reply, 0);
                    this.AddTooltip(item.Tooltip);
                }

                int y = offset - item.Y;

                if (item.Height < 20)
                    y += (20 - item.Height) / 2;
				
                this.AddItem(55 - item.X + this.m_Max / 2 - item.Width / 2, y, item.ItemID, points >= item.Points ? item.Hue : 0x3E9);
                this.AddTooltip(item.Tooltip);
                this.AddLabel(65 + this.m_Max, offset + (int)(height / 2) - 10, points >= item.Points ? 0x64 : 0x21, item.Points.ToString());
                this.AddTooltip(item.Tooltip);
				
                offset += 5 + height;
                this.m_Index += 1;

                if (this.m_Index < this.m_Collection.Donations.Count)
                    next = Math.Max(this.m_Collection.Donations[this.m_Index].Height, 20);
                else
                    next = 0;
            }
			
            // buttons
            this.AddButton(50, 335, 0x15E3, 0x15E7, (int)Buttons.Status, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(75, 335, 100, 20, 1072845, 0x1, false, false); // Status
			
            if (this.m_Page > 1)
            {
                this.AddButton(150, 335, 0x15E3, 0x15E7, (int)Buttons.Next, GumpButtonType.Page, this.m_Page - 1);
                this.AddHtmlLocalized(170, 335, 60, 20, 1074880, 0x1, false, false); // Previous			
            }
			
            this.m_Page += 1;
			
            if (this.m_Index < this.m_Collection.Rewards.Count)
            {
                this.AddButton(300, 335, 0x15E1, 0x15E5, (int)Buttons.Next, GumpButtonType.Page, this.m_Page);
                this.AddHtmlLocalized(240, 335, 60, 20, 1072854, 0x1, false, false); // <div align=right>Next</div>
            }
        }

        public void DisplayHuePage()
        { 
            int points = this.m_Owner.GetCollectionPoints(this.m_Collection.CollectionID);
			
            this.AddPage(this.m_Page);
			
            // title
            this.AddHtmlLocalized(50, 65, 150, 20, 1072843, 0x1, false, false); // Your Reward Points:
            this.AddLabel(230, 65, 0x64, points.ToString());
				
            this.AddImageTiled(35, 85, 270, 2, 0x23C5);
			
            this.AddHtmlLocalized(35, 90, 270, 20, 1074255, 0x1, false, false); // Please select a hue for your Reward:

            // hues
            int height = Math.Max(this.m_Item.Height, 20);
            int offset = 110;

            while (offset + height < 290 && this.m_Index < this.m_Item.Hues.Length)
            {
                this.AddButton(35, offset + (int)(height / 2) - 5, 0x837, 0x838, 100 + this.m_Index, GumpButtonType.Reply, 0);
                this.AddTooltip(this.m_Item.Tooltip);					
				
                this.AddItem(55 - this.m_Item.X, offset - this.m_Item.Y, this.m_Item.ItemID, this.m_Item.Hues[this.m_Index]);
                this.AddTooltip(this.m_Item.Tooltip);

                offset += 5 + height;
                this.m_Index += 1;
            }
			
            this.m_Page += 1;
			
            // buttons			
            this.AddButton(50, 335, 0x15E3, 0x15E7, (int)Buttons.Rewards, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(75, 335, 100, 20, 1072842, 0x1, false, false); // Rewards
			
            if (this.m_Index < this.m_Item.Hues.Length && this.m_Page > 2)
            {
                if (this.m_Index < this.m_Item.Hues.Length)
                    this.AddButton(270, 335, 0x15E1, 0x15E5, (int)Buttons.Next, GumpButtonType.Page, this.m_Page);
                else
                    this.AddButton(270, 335, 0x15E1, 0x15E5, (int)Buttons.Next, GumpButtonType.Page, 1);
				
                this.AddHtmlLocalized(210, 335, 60, 20, 1074256, 0x1, false, false); // More Hues
            }
        }

        public override void OnResponse(Server.Network.NetState state, RelayInfo info)
        {
            if (this.m_Collection == null || !this.m_Owner.InRange(this.m_Location, 2))
                return;
			
            if (info.ButtonID == (int)Buttons.Rewards)
                this.m_Owner.SendGump(new ComunityCollectionGump(this.m_Owner, this.m_Collection, this.m_Location, Section.Rewards));
            else if (info.ButtonID == (int)Buttons.Status)
                this.m_Owner.SendGump(new ComunityCollectionGump(this.m_Owner, this.m_Collection, this.m_Location, Section.Donates));
            else if (info.ButtonID >= 300 && this.m_Collection.Donations != null && info.ButtonID - 300 < this.m_Collection.Donations.Count && this.m_Section == Section.Donates)
            {
                CollectionItem item = this.m_Collection.Donations[info.ButtonID - 300];
				
                this.m_Owner.SendLocalizedMessage(1073178); // Please enter how much of that item you wish to donate:
                this.m_Owner.Prompt = new InternalPrompt(this.m_Collection, item, this.m_Location);
            }
            else if (info.ButtonID >= 200 && this.m_Collection.Rewards != null && info.ButtonID - 200 < this.m_Collection.Rewards.Count && this.m_Section == Section.Rewards)
            {
                CollectionItem item = this.m_Collection.Rewards[info.ButtonID - 200];
                int points = this.m_Owner.GetCollectionPoints(this.m_Collection.CollectionID);
				
                if (item.Points <= points)
                {
                    if (item is CollectionHuedItem)
					{
						this.m_Owner.SendGump(new ComunityCollectionGump(this.m_Owner, this.m_Collection, this.m_Location, Section.Hues, (CollectionHuedItem)item));
					}
					else
					{
						this.m_Owner.CloseGump(typeof(ConfirmRewardGump));
						this.m_Owner.SendGump(new ConfirmRewardGump(this.m_Collection, this.m_Location, item, 0));
					}
				}
                else
                    this.m_Owner.SendLocalizedMessage(1073122); // You don't have enough points for that!
            }
            else if (info.ButtonID >= 100 && this.m_Item != null && info.ButtonID - 200 < this.m_Item.Hues.Length && this.m_Section == Section.Hues)
			{
				this.m_Owner.CloseGump(typeof(ConfirmRewardGump));
				this.m_Owner.SendGump(new ConfirmRewardGump(this.m_Collection, this.m_Location, this.m_Item, this.m_Item.Hues[info.ButtonID - 100]));
			}
		}

        private class InternalPrompt : Prompt
        {
            private readonly IComunityCollection m_Collection;
            private readonly CollectionItem m_Selected;
            private readonly Point3D m_Location;
            public InternalPrompt(IComunityCollection collection, CollectionItem selected, Point3D location)
            {
                this.m_Collection = collection;
                this.m_Selected = selected;
                this.m_Location = location;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!from.InRange(this.m_Location, 2) || !(from is PlayerMobile))
                    return;

                HandleResponse(from, text);
                from.SendGump(new ComunityCollectionGump((PlayerMobile)from, this.m_Collection, this.m_Location));
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

                // Make sure we have enough
                if(m_Selected.Type == typeof(BankCheck))
                {
                    int count = from.Backpack.GetChecksWorth(true);
                    if(count < amount)
                    {
                        from.SendLocalizedMessage(1073182); // You do not have enough to make a donation of that magnitude!
                        return;
                    }
                }
                else
                {
                    int count = from.Backpack.GetAmount(m_Selected.Type, true, true);
                    if(count < amount)
                    {
                        if(m_Selected.Type == typeof(Gold))
                            from.SendLocalizedMessage(1073182); // You do not have enough to make a donation of that magnitude!
                        else
                            from.SendLocalizedMessage(1073167); // You do not have enough of that item to make a donation!
                        return;
                    }
                }

                // Donate
                if(m_Selected.Type == typeof(BankCheck))
                {
                    from.Backpack.TakeFromChecks(amount, true);
                }
                else
                {
                    from.Backpack.ConsumeTotal(m_Selected.Type, amount, true, true);
                }
                m_Collection.Donate((PlayerMobile)from, m_Selected, amount);
            }

            public override void OnCancel(Mobile from)
            { 
                if (!(from is PlayerMobile))
                    return;
			
                from.SendLocalizedMessage(1073184); // You cancel your donation.
				
                if (from.InRange(this.m_Location, 2))
                    from.SendGump(new ComunityCollectionGump((PlayerMobile)from, this.m_Collection, this.m_Location));
            }
        }
    }
}
