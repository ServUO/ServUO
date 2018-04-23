// Scripted by Lord Greywolf
// Age of Avatars
// You can use this script how ever you want
// As is, spindle it, mutalate it, change settings, what ever
// just remember if you use it in a package, or update and resubmit it, to give credit where credit is due
// please see the Anatomy Train gump for all comments on how to modify this file, besides the couple of comments in here (most are in anatomy)
using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
	public class ArmsLoreTrainingGump : Gump
	{
		string[] goodsay = new string[]
		{
			"I think we have a winner!",
			"At least some one has been watching?",
			"Good Job",
			"You may become a fighter yet",
			"Right on the money",
			"Give this guy a Cupie Doll",
			"You have been listening to me",
			"Sheep are in trouble now",
			"Good going, but keep on practicing",
			"I think he hot it!",
			"You finialy remembered which was left & right",
			"Bam, you got it right",
			"You are improving",
			"You got it right this time, care to try again?",
		};

		string[] badsay = new string[]
		{
			"Go back to knitting sweaters",
			"Do you know which end of the weapon to even hold?",
			"I hope you where joking",
			"What are you thinking?",
			"We are not doing the Hokie Pokie here",
			"Guessing is not a good idea in battle",
			"Are you even listening in class?",
			"I think counting sheep might be to hard for you",
			"I hope you did not pay in advance for this course",
			"Remember the Pointy End Goes in the Other Guy",
			"Did you mean your other left?",
			"I think we need an other dunce cap over here",
			"Are you trying to hit it or tickle it?",
			"At least try to get the correct answer once in a while",
		};

		private static void SayRandom(string[] say, Mobile m)
		{
			m.SendMessage(say[Utility.Random(say.Length)]);
		}

		private int m_thespot;
		public int thespot { get { return m_thespot; } set { m_thespot = value; } }

		public ArmsLoreTrainingGump() : base( 50, 50 )
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
			if (Utility.RandomBool()) this.AddImage(222, -32, 12);
			else this.AddImage(222, -32, 13);
			if (thespot == 1) this.AddImage(222, -32, 50464); // the type of weapon to show
			else if (thespot == 2) this.AddImage(222, -32, 50575); // the type of weapon to show
			else if (thespot == 3) this.AddImage(222, -32, 50610); // the type of weapon to show
			else if (thespot == 4) this.AddImage(222, -32, 50617); // the type of weapon to show
			else if (thespot == 5) this.AddImage(222, -32, 50620); // the type of weapon to show
			else if (thespot == 6) this.AddImage(222, -32, 50622); // the type of weapon to show
			this.AddLabel(30, 45, 33, @"What is the type of weapon"); // wording of what to ask
			this.AddLabel(30, 75, 33, @"that the dummy is holding?");
			this.AddLabel(30, 105, 33, @"General Catagory only");
			this.AddButton(30, 200, 210, 211, (int)Buttons.Button1, GumpButtonType.Reply, 0);
			this.AddButton(30, 230, 210, 211, (int)Buttons.Button2, GumpButtonType.Reply, 0);
			this.AddButton(30, 260, 210, 211, (int)Buttons.Button3, GumpButtonType.Reply, 0);
			this.AddButton(30, 290, 210, 211, (int)Buttons.Button4, GumpButtonType.Reply, 0);
			this.AddButton(30, 320, 210, 211, (int)Buttons.Button5, GumpButtonType.Reply, 0);
			this.AddButton(30, 350, 210, 211, (int)Buttons.Button6, GumpButtonType.Reply, 0);
			this.AddLabel(60, 200, 33, @"The Dummy is holding a sword");
			this.AddLabel(60, 230, 33, @"The Dummy is holding a bow");
			this.AddLabel(60, 260, 33, @"The Dummy is holding an axe");
			this.AddLabel(60, 290, 33, @"The Dummy is holding a staff");
			this.AddLabel(60, 320, 33, @"The Dummy is holding a club");
			this.AddLabel(60, 350, 33, @"The Dummy is holding a dagger");
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
			pm.CheckSkill( SkillName.ArmsLore, 0.0, 160.0 );
			switch ( info.ButtonID )
			{
				case 0: default: { break; }
				case 1:
				{
					if (thespot == 1)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ArmsLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 2:
				{
					if (thespot == 2)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ArmsLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 3:
				{
					if (thespot == 3)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ArmsLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 4:
				{
					if (thespot == 4)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ArmsLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 5:
				{
					if (thespot == 5)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ArmsLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 6:
				{
					if (thespot == 6)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.ArmsLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 7: { break; }
			}
		}
	}
}