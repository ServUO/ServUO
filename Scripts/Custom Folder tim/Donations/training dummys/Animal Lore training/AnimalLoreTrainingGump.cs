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
	public class AnimalLoreTrainingGump : Gump
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
			"The Zoo might just hire you now",
			"Good going, but keep on practicing",
			"I think he hot it!",
			"Ok, I will give you that one, lucky guess",
			"Bam, you got it right",
			"You are improving",
			"You got it right this time, care to try again?",
		};

		string[] badsay = new string[]
		{
			"Have you ever herd an aminal laugh that much before?",
			"I did not think an animal could roll around that much",
			"I hope you where joking",
			"What are you thinking?",
			"I think you need some glasses",
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

		public AnimalLoreTrainingGump() : base( 50, 50 )
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
			if (thespot == 1) this.AddItem(295, 49, 8392); // 1st animal pic
			else if (thespot == 2) this.AddItem(295, 49, 8399); // these are pics of in game items instead of
			else if (thespot == 3) this.AddItem(295, 49, 8406); // gump images, so we use additem instead
			else if (thespot == 4) this.AddItem(295, 49, 8442); // again x, y and then the id of the item
			else if (thespot == 5) this.AddItem(295, 49, 8451);
			else if (thespot == 6) this.AddItem(295, 49, 8459);
			this.AddLabel(30, 45, 33, @"What is the type of critter");
			this.AddLabel(30, 75, 33, @"that is pictured here?");
			this.AddLabel(30, 105, 33, @"General Catagory only");
			this.AddButton(30, 200, 210, 211, (int)Buttons.Button1, GumpButtonType.Reply, 0);
			this.AddButton(30, 230, 210, 211, (int)Buttons.Button2, GumpButtonType.Reply, 0);
			this.AddButton(30, 260, 210, 211, (int)Buttons.Button3, GumpButtonType.Reply, 0);
			this.AddButton(30, 290, 210, 211, (int)Buttons.Button4, GumpButtonType.Reply, 0);
			this.AddButton(30, 320, 210, 211, (int)Buttons.Button5, GumpButtonType.Reply, 0);
			this.AddButton(30, 350, 210, 211, (int)Buttons.Button6, GumpButtonType.Reply, 0);
			this.AddLabel(60, 200, 33, @"The critter shown is a type of Ettin");
			this.AddLabel(60, 230, 33, @"The critter shown is a type of Bear");
			this.AddLabel(60, 260, 33, @"The critter shown is a type of Dragon");
			this.AddLabel(60, 290, 33, @"The critter shown is a type of Treant");
			this.AddLabel(60, 320, 33, @"The critter shown is a type of Cow");
			this.AddLabel(60, 350, 33, @"The critter shown is a type of Elemental");
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
			pm.CheckSkill( SkillName.AnimalLore, 0.0, 160.0 );
			switch ( info.ButtonID )
			{
				case 0: default: { break; }
				case 1:
				{
					if (thespot == 1)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.AnimalLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 2:
				{
					if (thespot == 2)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.AnimalLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 3:
				{
					if (thespot == 3)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.AnimalLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 4:
				{
					if (thespot == 4)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.AnimalLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 5:
				{
					if (thespot == 5)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.AnimalLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 6:
				{
					if (thespot == 6)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.AnimalLore, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 7: { break; }
			}
		}
	}
}