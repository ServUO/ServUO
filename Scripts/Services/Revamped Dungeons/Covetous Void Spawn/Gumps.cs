using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using System.Linq;
using Server.Engines.Points;
using Server.Commands;

namespace Server.Engines.VoidPool
{
	public enum ScoreType
	{
		Current,
		BestSingle,
		OverallTotal,
        BestWave
	}

	public class VoidPoolGump : Gump
	{
        public static void Initialize()
        {
            CommandSystem.Register("VoidPool", AccessLevel.Player, SendGump_OnCommand);
        }

        public static readonly int Red = 0x4800;
        public static readonly int Orange = 0xB104;
	
		public PlayerMobile User { get; set; }
		public VoidPoolController Controller { get; set; }
	
		public VoidPoolGump(VoidPoolController controller, PlayerMobile pm) : base(50, 50)
		{
			Controller = controller;
			User = pm;
			
			AddGumpLayout();
		}
		
		public void AddGumpLayout()
		{
            AddBackground(0, 0, 400, 565, 9350);
			
			AddHtmlLocalized(10, 10, 200, 16, 1152531, Red, false, false); // The Void Pool
			AddHtmlLocalized(10, 30, 200, 16, Controller.Map == Map.Felucca ? 1012001 : 1012000, Red, false, false); // FEl/Tram
			
			if(Controller.OnGoing)
			{
				AddHtmlLocalized(10, 50, 200, 16, 1152914, Orange, false, false); // Current Battle:
				AddHtmlLocalized(180, 50, 200, 16, 1152915, Controller.Wave.ToString(), Orange, false, false); // Wave ~1_WAVE~
			}
			else
			{
				AddHtmlLocalized(10, 50, 200, 16, 1152916, Orange, false, false); // Next Battle:
				
				if(Controller.NextStart > DateTime.UtcNow)
					AddHtmlLocalized(180, 50, 200, 16, 1152917, ((int)(Controller.NextStart - DateTime.UtcNow).TotalMinutes).ToString(), Orange, false, false); // Starts in ~1_MIN~ min.
			}
			
			AddButton(140, 70, 4005, 4006, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(180, 70, 200, 16, 1152535, Orange, false, false); 		//Current Battle Scoreboard

            AddButton(140, 90, 4005, 4006, 2, GumpButtonType.Reply, 0);
			AddHtmlLocalized(180, 90, 200, 16, 1152536, Orange, false, false); 		//Best Single Battle Scoreboard

            AddButton(140, 110, 4005, 4006, 3, GumpButtonType.Reply, 0);
			AddHtmlLocalized(180, 110, 200, 16, 1152537, Orange, false, false); 		//Overall Total Scores

            if (VoidPoolStats.BestWave != null)
            {
                AddButton(140, 130, 4005, 4006, 4, GumpButtonType.Reply, 0);
                AddHtml(180, 130, 200, 16, String.Format("<basefont color=#A52A2A>Best Wave: {0}", VoidPoolStats.BestWave.Waves.ToString()), false, false);
            }
			
			AddHtmlLocalized(10, 150, 400, 16, 1152552, Orange, false, false);			// See Loyalty Menu for Reward Points
			AddHtmlLocalized(10, 170, 400, 16, 1152553, Orange, false, false);			// See Vela in Cove for rewards
			
			AddHtmlLocalized(10, 190, 380, 175, 1152533, Orange, true, true);
			AddHtmlLocalized(10, 375, 380, 175, 1152534, Orange, true, true);
		}
		
		public override void OnResponse(Server.Network.NetState state, RelayInfo info)
		{
			switch(info.ButtonID)
			{
				default: break;
				case 1:
					User.SendGump(new ScoresGump(Controller, User, ScoreType.Current));
					break;
				case 2:
                    User.SendGump(new ScoresGump(Controller, User, ScoreType.BestSingle));
					break;
				case 3:
                    User.SendGump(new ScoresGump(Controller, User, ScoreType.OverallTotal));
					break;
                case 4:
                    if(VoidPoolStats.BestWave != null)
                        User.SendGump(new ScoresGump(Controller, User, ScoreType.BestWave));
                    break;
			}
		}

        [Usage("VoidPool")]
        [Description("Sends Void Pool Gump to player.")]
        public static void SendGump_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (VoidPoolController.InstanceTram != null || VoidPoolController.InstanceFel != null)
            {
                e.Mobile.SendGump(new VoidPoolGump(from.Map == Map.Trammel ? VoidPoolController.InstanceTram : VoidPoolController.InstanceFel, from as PlayerMobile));
            }
        }
	}

    public class ScoresGump : Gump
    {
        public static readonly int Red = 0x4800;
        public static readonly int Orange = 0xB104;

        public PlayerMobile User { get; private set; }
        public VoidPoolController Controller { get; private set; }
        public Dictionary<Mobile, long> Score { get; private set; }
        public ScoreType ScoreType { get; set; }

        public ScoresGump(VoidPoolController controller, PlayerMobile pm, ScoreType type)
            : base(50, 50)
        {
            Controller = controller;
            User = pm;
            ScoreType = type;

            AddGumpLayout();
        }

        public void AddGumpLayout()
        {
            int page = 0;
            object loc;
            AddBackground(0, 0, 500, 620, 9350);
            AddPage(page);

            if (this.ScoreType == ScoreType.BestWave && VoidPoolStats.BestWave == null)
                return;

            switch (this.ScoreType)
            {
                default:
                case ScoreType.Current: Score = Controller.CurrentScore; loc = 1152535; break;
                case ScoreType.BestSingle: Score = VoidPoolStats.BestSingle; loc = 1152536; break;
                case ScoreType.OverallTotal: Score = VoidPoolStats.OverallTotal; loc = 1152537; break;
                case ScoreType.BestWave: Score = VoidPoolStats.BestWave.Score; loc = "Best Wave Scoreboard"; break;
            }

            AddHtmlLocalized(10, 10, 200, 16, 1152531, Red, false, false); // The Void Pool
            AddHtmlLocalized(10, 30, 200, 16, Controller.Map == Map.Felucca ? 1012001 : 1012000, Red, false, false); // FEl/Tram

            if(loc is int)
                AddHtmlLocalized(10, 50, 200, 16, (int)loc, Red, false, false);
            else if (loc is string)
                AddHtml(10, 50, 200, 16, String.Format("<basefont color=#8B0000>{0}", (string)loc), false, false);

            if (this.ScoreType == ScoreType.BestWave)
            {
                AddHtml(200, 30, 200, 16, String.Format("<basefont color=#8B0000>Total Waves: {0}", VoidPoolStats.BestWave.Waves.ToString()), false, false);
                AddHtml(200, 50, 200, 16, String.Format("<basefont color=#8B0000>Total Score: {0}", VoidPoolStats.BestWave.TotalScore.ToString()), false, false);
            }

            AddHtmlLocalized(10, 90, 100, 16, 1152541, Orange, false, false); // RANK
            AddHtmlLocalized(100, 90, 300, 16, 1152542, Orange, false, false); // PLAYER
            AddHtmlLocalized(400, 90, 100, 16, 1152543, Orange, false, false); // SCORE

            page++;
            AddPage(page);

            if (Score != null)
            {
                int index = 0;
                int yOffset = 0;

                foreach (KeyValuePair<Mobile, long> table in Score.OrderBy(kvp => -kvp.Value))
                {
                    AddHtml(10, 130 + (20 * yOffset), 50, 16, (index + 1).ToString(), false, false);
                    AddHtml(100, 130 + (20 * yOffset), 300, 16, table.Key.Name, false, false);
                    AddHtml(400, 130 + (20 * yOffset), 100, 16, table.Value.ToString(), false, false);

                    index++;
                    yOffset++;

                    if (index > 0 && index % 19 == 0 && index < Score.Count - 1)
                    {
                        page++;
                        yOffset = 0;

                        AddHtmlLocalized(320, 570, 100, 16, 1044045, Orange, false, false); // NEXT PAGE
                        AddButton(404, 570, 4005, 4006, 0, GumpButtonType.Page, page);

                        AddPage(page);

                        AddHtmlLocalized(120, 570, 100, 16, 1044044, Orange, false, false); // PREV PAGE
                        AddButton(80, 570, 4014, 4015, 0, GumpButtonType.Page, page - 1);
                    }
                }
            }
        }
    }
	
	public class VoidPoolRewardGump : Gump
	{
		public int Index { get; private set; }
		public Mobile Owner { get; private set; }
        public PlayerMobile User { get; private set; }
        public int Page { get; set; }
	
		public VoidPoolRewardGump(Mobile owner, PlayerMobile user) : base(50, 50)
		{
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

            int points = (int)PointsSystem.VoidPool.GetPoints(User);
			
			AddHtmlLocalized(70, 35, 270, 20, 1152531, 0x1, false, false); // The Void Pool
			AddHtmlLocalized(50, 65, 150, 20, 1072843, 0x1, false, false); // Your Reward Points:
            AddLabel(230, 65, 0x64, points.ToString());				
            AddImageTiled(35, 85, 270, 2, 0x23C5);			
            AddHtmlLocalized(35, 90, 270, 20, 1072844, 0x1, false, false); // Please Choose a Reward:
			
			while (VoidPoolRewards.Rewards != null && Index < VoidPoolRewards.Rewards.Count)
                DisplayRewardPage();
		}
		
		public void DisplayRewardPage()
		{
            AddPage(Page);
			double points = PointsSystem.VoidPool.GetPoints(User);
			
			int offset = 110;
            int next = 0;
			int max = GetMax(VoidPoolRewards.Rewards);
			
			while (offset + next < 300 && Index < VoidPoolRewards.Rewards.Count)
			{
				CollectionItem item = VoidPoolRewards.Rewards[Index];
				
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

                if (Index < VoidPoolRewards.Rewards.Count)
                    next = Math.Max(VoidPoolRewards.Rewards[Index].Height, 20);
                else
                    next = 0;
			}
			
            if (Page > 1)
            {
                AddButton(150, 335, 0x15E3, 0x15E7, 0, GumpButtonType.Page, Page - 1);
                AddHtmlLocalized(170, 335, 60, 20, 1074880, 0x1, false, false); // Previous			
            }
			
            Page++;
			
            if (Index < VoidPoolRewards.Rewards.Count)
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
				CollectionItem item = VoidPoolRewards.Rewards[info.ButtonID - 200];
				double points = PointsSystem.VoidPool.GetPoints(from);
				
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
				Item item;
				
				if(Index >= 0 && Index < 23)
				{
					item = Activator.CreateInstance(Item.Type, (CraftResource)Item.Hue) as Item;
				}
				else
					item = Activator.CreateInstance(Item.Type) as Item;
				
				if(item != null)
				{
					if(from.Backpack == null || !from.Backpack.TryDropItem(from, item, false))
					{
						from.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
						item.Delete();
					}
					else
					{
						PointsSystem.VoidPool.DeductPoints(from as PlayerMobile, Item.Points);
					
						from.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
						from.PlaySound(0x5A7);
					}
				}
			}
		}
	}
}