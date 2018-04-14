using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using System.Linq;

namespace Server.Gumps
{
	public abstract class BaseRewardGump : Gump
	{
		public int Index { get; private set; }
		public Mobile Owner { get; private set; }
        public PlayerMobile User { get; private set; }
        public int Page { get; private set; }
        public int Title { get; private set; }

        public double Points { get; protected set; }
        public List<CollectionItem> Collection { get; protected set; }

        public virtual int YDist  {  get  { return 10; } }

        public BaseRewardGump(Mobile owner, PlayerMobile user, List<CollectionItem> col, int title, double points = -1.0)
            : base(50, 50)
		{
            user.CloseGump(typeof(BaseRewardGump));

			Owner = owner;
            User = user;
            Collection = col;
            Title = title;

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

            if (points == -1)
                Points = GetPoints(user);
            else
                Points = points;

			AddHtmlLocalized(70, 35, 270, 20, Title, 0x1, false, false);
            AddHtmlLocalized(50, 65, 150, 20, 1072843, 0x1, false, false); // Your Reward Points:
            AddPoints();		
            AddImageTiled(35, 85, 270, 2, 0x23C5);			
            AddHtmlLocalized(35, 90, 270, 20, 1072844, 0x1, false, false); // Please Choose a Reward:

            while (Collection != null && Index < Collection.Count)
                DisplayRewardPage();
		}

        protected virtual void AddPoints()
        {
            AddLabel(230, 65, 0x64, String.Format(((int)Points).ToString()));
        }
		
		public void DisplayRewardPage()
		{
            AddPage(Page);
			
			int offset = 110;
            int next = 0;
            int max = GetMax();

            while (offset + next < 320 && Index < Collection.Count)
			{
                CollectionItem item = Collection[Index];
				int height = Math.Max(item.Height, 20);

				if (Points >= item.Points)
                {
                    AddButton(35, offset + (int)(height / 2) - 5, 0x837, 0x838, 200 + Index, GumpButtonType.Reply, 0);
                    AddTooltip(item.Tooltip);
                }
				
				int y = offset - item.Y;

                if (item.Height < 20)
                    y += (20 - item.Height) / 2;

                Item i = null;

                if (Owner.Backpack != null && item.Type != null)
                    i = Owner.Backpack.FindItemByType(item.Type);

                int hue = GetItemHue(i, item);

                AddItem(55 - item.X + max / 2 - item.Width / 2, y, item.ItemID, hue);

                if (i != null)
                    AddItemProperty(i.Serial);
                else if (item.Tooltip > 0)
                    AddTooltip(item.Tooltip);

                AddLabel(65 + max, offset + (int)(height / 2) - 10, Points >= item.Points ? 0x64 : 0x21, item.Points.ToString());
				
                offset += YDist + height;
                Index++;

                if (Index < Collection.Count)
                    next = Math.Max(Collection[Index].Height, 20);
                else
                    next = 0;
			}

            if (Page > 1)
            {
                AddButton(150, 335, 0x15E3, 0x15E7, 0, GumpButtonType.Page, Page - 1);
                AddHtmlLocalized(170, 335, 60, 20, 1074880, 0x1, false, false); // Previous			
            }

            Page++;

            if (Index < Collection.Count)
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
				CollectionItem item = Collection[info.ButtonID - 200];
                double points = GetPoints(from);
				
				if(item != null && item.Points <= points)
				{
					from.SendGump(new aConfirmRewardGump(Owner, item, info.ButtonID - 200, OnConfirmed));
				}
				else
					from.SendLocalizedMessage(1073122); // You don't have enough points for that!
			}
		}

        public abstract double GetPoints(Mobile m);
        public virtual void RemovePoints(double points)
        {
        }

        public virtual void OnConfirmed(CollectionItem citem, int index)
        {
            Item item = Activator.CreateInstance(citem.Type) as Item;

            if (item != null)
            {
                if (User.Backpack == null || !User.Backpack.TryDropItem(User, item, false))
                {
                    User.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                    item.Delete();
                }
                else
                {
                    OnItemCreated(item);

                    User.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                    RemovePoints(citem.Points);
                    User.PlaySound(0x5A8);
                }
            }
        }

        public virtual void OnItemCreated(Item item)
        {
        }

        public virtual int GetItemHue(Item i, CollectionItem item)
        {
            int hue = 0x3E9;

            if (Points >= item.Points)
                hue = item.Hue;

            return hue;
        }
		
		public virtual int GetMax()
        {
            int max = 0;
		
            if (Collection != null)
            {
                for (int i = 0; i < Collection.Count; i++)
                {
                    if (max < Collection[i].Width)
                        max = Collection[i].Width;
                }
            }

            return max;
        }
	}
	
	public class aConfirmRewardGump : BaseConfirmGump
	{
		public override int TitleNumber { get { return 1074974; } }
		public override int LabelNumber { get { return 1074975; } }
		
		public Mobile Owner { get; set; }
        public int Index { get; set; }
		public CollectionItem Item { get; set; }

        public Action<CollectionItem, int> ConfirmCallback { get; set; }

		public aConfirmRewardGump(Mobile owner, CollectionItem item, int index, Action<CollectionItem, int> callback)
		{
			Item = item;
            Index = index;
			Owner = owner;

            ConfirmCallback = callback;
		}
		
		public override void Confirm(Mobile from)
		{
            if (from.InRange(Owner.Location, 5) && Item != null)
            {
                if (ConfirmCallback != null)
                    ConfirmCallback(Item, Index);
            }
		}
	}
}
