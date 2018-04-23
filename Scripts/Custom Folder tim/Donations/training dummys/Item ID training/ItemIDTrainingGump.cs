// Scripted by Lord Greywolf
// Age of Avatars
// You can use this script how ever you want
// As is, spindle it, mutalate it, change settings, what ever
// just remember if you use it in a package, or update and resubmit it, to give credit where credit is due
// please refer back to the anatomy gump for most things in here, only the major differences are being shown
using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
	public class ItemIDTrainingGump : Gump
	{
		string[] goodsay = new string[]
		{
			"I think we have a winner!",
			"At least some one has been watching?",
			"Good Job",
			"You may actualy learn something here",
			"Right on the money",
			"Give this guy a Cupie Doll",
			"You have been listening to me",
			"Merchants are in trouble now",
			"Good going, but keep on practicing",
			"I think he hot it!",
			"Ok, I will give you that one, lucky guess",
			"Bam, you got it right",
			"You are improving",
			"You got it right this time, care to try again?",
		};

		string[] badsay = new string[]
		{
			"Merchants are saying 'sucker coming into the store'",
			"I have some ocean from property at Mt Kendall for sale",
			"I hope you where joking",
			"What are you thinking?",
			"We are not doing the Name that Tune here",
			"Thinking about something else?",
			"Are you even listening in class?",
			"I think counting sheep might be to hard for you",
			"I hope you did not pay in advance for this course",
			"You may want to consider a different profession",
			"You have no idea, do you?",
			"I think we need an other dunce cap over here",
			"Maybe next time you will get it right",
			"At least try to get the correct answer once in a while",
		};

		private static void SayRandom(string[] say, Mobile m)
		{
			m.SendMessage(say[Utility.Random(say.Length)]);
		}

		private int m_thespot;
		public int thespot { get { return m_thespot; } set { m_thespot = value; } }

		public ItemIDTrainingGump() : base( 50, 50 )
		{
			thespot = Utility.Random(6) + 1;
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddImage(0, 0, 71);
			this.AddImage(128, -3, 71);
			this.AddImage(241, -4, 71);
			this.AddImage(1, 130, 71);
			this.AddImage(127, 130, 71);
			this.AddImage(243, 127, 71);
			this.AddImage(3, 252, 71);
			this.AddImage(118, 253, 71);
			this.AddImage(244, 256, 71);
				// no longer need the male female item being shown so it is removed
			if (thespot == 1) this.AddItem(295, 49, Utility.RandomList(2472, 2473, 2474, 2475, 2476)); // 1st animal pic from a random list of items
			else if (thespot == 2) this.AddItem(295, 49, Utility.RandomList(2508, 2509, 2510, 2511)); // these are pics of in game items instead of
			else if (thespot == 3) this.AddItem(295, 49, Utility.RandomList(3740, 3741, 3742, 3761, 3762, 3763, 3764)); // gump images, so we use additem instead
			else if (thespot == 4) this.AddItem(295, 49, Utility.RandomMinMax(2835, 2854)); // again x, y and then the id of the item
			else if (thespot == 5) this.AddItem(295, 49, Utility.RandomMinMax(3960, 3985)); // these 3 use a random from a spread of numbers from hig to low for item id
			else if (thespot == 6) this.AddItem(295, 49, Utility.RandomMinMax(5051, 5090));
			this.AddLabel(30, 45, 33, @"What is the type of item");
			this.AddLabel(30, 75, 33, @"that is pictured here?");
			this.AddLabel(30, 105, 33, @"General Catagory only");
			this.AddButton(30, 200, 210, 211, (int)Buttons.Button1, GumpButtonType.Reply, 0);
			this.AddButton(30, 230, 210, 211, (int)Buttons.Button2, GumpButtonType.Reply, 0);
			this.AddButton(30, 260, 210, 211, (int)Buttons.Button3, GumpButtonType.Reply, 0);
			this.AddButton(30, 290, 210, 211, (int)Buttons.Button4, GumpButtonType.Reply, 0);
			this.AddButton(30, 320, 210, 211, (int)Buttons.Button5, GumpButtonType.Reply, 0);
			this.AddButton(30, 350, 210, 211, (int)Buttons.Button6, GumpButtonType.Reply, 0);
			this.AddLabel(60, 200, 33, @"The Item shown is a type of container");
			this.AddLabel(60, 230, 33, @"The Item shown is a type of fish");
			this.AddLabel(60, 260, 33, @"The Item shown is a type of instrument");
			this.AddLabel(60, 290, 33, @"The Item shown is a type of potion");
			this.AddLabel(60, 320, 33, @"The Item shown is a type of reagent");
			this.AddLabel(60, 350, 33, @"The Item shown is a type of armor");
			this.AddButton(332, 362, 241, 243, (int)Buttons.Button7, GumpButtonType.Reply, 0);
		}
		
		public enum Buttons
		{
			Button0,
			Button1,
			Button2,
			Button3,
			Button4,
			Button5,
			Button6,
			Button7,
		}

		public override void OnResponse(NetState sender, RelayInfo info )
		{
			Mobile m = sender.Mobile;
			PlayerMobile pm = m as PlayerMobile;
			if (pm == null) return;
			pm.CheckSkill( SkillName.ItemID, 0.0, 160.0 );
			switch ( info.ButtonID )
			{
				case 0: default: { break; }
				case 1:
				{
					if (thespot == 1)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ItemID, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 2:
				{
					if (thespot == 2)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ItemID, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 3:
				{
					if (thespot == 3)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ItemID, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 4:
				{
					if (thespot == 4)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ItemID, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 5:
				{
					if (thespot == 5)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ItemID, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 6:
				{
					if (thespot == 6)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ItemID, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 7: { break; }
			}
		}
	}
}