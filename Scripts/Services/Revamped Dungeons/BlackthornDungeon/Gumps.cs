using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using System.Linq;
using Server.Engines.Points;
using Server.Commands;

namespace Server.Engines.Blackthorn
{
	public class BlackthornRewardGump : Gump
	{
		public int Index { get; private set; }
		public Mobile Owner { get; private set; }
        public PlayerMobile User { get; private set; }
        public int Page { get; set; }
	
		public BlackthornRewardGump(Mobile owner, PlayerMobile user) : base(50, 50)
		{
            user.CloseGump(typeof(BlackthornRewardGump));

			Owner = owner;
            User = user;
		
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
			
			Index = 0;
            Page = 1;

            int points = (int)PointsSystem.Blackthorn.GetPoints(User);
			
			AddHtmlLocalized(70, 35, 270, 20, 1154516, 0x1, false, false); // Blackthorn Artifacts
            AddHtmlLocalized(50, 65, 150, 20, 1072843, 0x1, false, false); // Your Reward Points:
            AddLabel(230, 65, 0x64, points.ToString());				
            AddImageTiled(35, 85, 270, 2, 0x23C5);			
            AddHtmlLocalized(35, 90, 270, 20, 1072844, 0x1, false, false); // Please Choose a Reward:
			
			while (BlackthornRewards.Rewards != null && Index < BlackthornRewards.Rewards.Count)
                DisplayRewardPage();
		}
		
		public void DisplayRewardPage()
		{
            AddPage(Page);
			double points = PointsSystem.Blackthorn.GetPoints(User);
			
			int offset = 110;
            int next = 0;
			int max = GetMax(BlackthornRewards.Rewards);
			
			while (offset + next < 300 && Index < BlackthornRewards.Rewards.Count)
			{
				CollectionItem item = BlackthornRewards.Rewards[Index];
				
				int height = Math.Max(item.Height, 20);
				
				if (points >= item.Points)
                {
                    this.AddButton(35, offset + (int)(height / 2) - 5, 0x837, 0x838, 200 + Index, GumpButtonType.Reply, 0);
                    this.AddTooltip(item.Tooltip);
                }
				
				int y = offset - item.Y;

                if (item.Height < 20)
                    y += (20 - item.Height) / 2;

                Item i = Owner.Backpack.FindItemByType(item.Type);
                int hue;

                if (i != null)
                    hue = points >= item.Points ? i.Hue : 0x3E9;
                else
                    hue = points >= item.Points ? CraftResources.GetHue((CraftResource)item.Hue) : 0x3E9;

                AddItem(55 - item.X + max / 2 - item.Width / 2, y, item.ItemID, hue);

                if (i != null)
                    AddItemProperty(i.Serial);
                else
                    AddTooltip(item.Tooltip);

                AddLabel(65 + max, offset + (int)(height / 2) - 10, points >= item.Points ? 0x64 : 0x21, item.Points.ToString());
				
                offset += 5 + height;
                Index++;

                if (Index < BlackthornRewards.Rewards.Count)
                    next = Math.Max(BlackthornRewards.Rewards[Index].Height, 20);
                else
                    next = 0;
			}
			
            if (Page > 1)
            {
                AddButton(150, 335, 0x15E3, 0x15E7, 0, GumpButtonType.Page, Page - 1);
                AddHtmlLocalized(170, 335, 60, 20, 1074880, 0x1, false, false); // Previous			
            }
			
            Page++;
			
            if (Index < BlackthornRewards.Rewards.Count)
            {
                AddButton(300, 335, 0x15E1, 0x15E5, 0, GumpButtonType.Page, Page);
                AddHtmlLocalized(240, 335, 60, 20, 1072854, 0x1, false, false); // <div align=right>Next</div>
            }
		}
		
		public override void OnResponse(Server.Network.NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			if(info.ButtonID >= 200 && from is PlayerMobile)
			{
				CollectionItem item = BlackthornRewards.Rewards[info.ButtonID - 200];
				double points = PointsSystem.Blackthorn.GetPoints(from);
				
				if(item != null && item.Points <= points)
				{
					from.SendGump(new ConfirmRewardGump(Owner, item, info.ButtonID - 200));
				}
				else
					from.SendLocalizedMessage(1073122); // You don't have enough points for that!
			}
		}
		
		public int GetMax(List<CollectionItem> list)
        {
            int max = 0;
		
            if (list != null)
            {
                for (int i = 0; i < list.Count; i ++)
                    if (max < list[i].Width)
                        max = list[i].Width;
            }

            return max;
        }
	}
	
	public class ConfirmRewardGump : BaseConfirmGump
	{
		public override int TitleNumber { get { return 1074974; } }
		public override int LabelNumber { get { return 1074975; } }
		
		public Mobile Owner { get; set; }
        public int Index { get; set; }
		public CollectionItem Item { get; set; }
		
		public ConfirmRewardGump(Mobile owner, CollectionItem item, int index)
		{
			Item = item;
            Index = index;
			Owner = owner;
		}
		
		public override void Confirm(Mobile from)
		{
			if(from.InRange(Owner.Location, 5) && Item != null)
			{
                Item item = Activator.CreateInstance(Item.Type) as Item;
				
				if(item != null)
				{
					if(from.Backpack == null || !from.Backpack.TryDropItem(from, item, false))
					{
						from.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
						item.Delete();
					}
					else
					{
						PointsSystem.Blackthorn.DeductPoints(from as PlayerMobile, Item.Points);
					
						from.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
						from.PlaySound(0x5A7);
					}
				}
			}
		}
	}
}