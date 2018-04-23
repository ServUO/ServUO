// Scripted by Lord Greywolf
// Age of Avatars
// You can use this script how ever you want
// As is, spindle it, mutalate it, change settings, what ever
// just remember if you use it in a package, or update and resubmit it, to give credit where credit is due
// this one is used also as a base for the tactics  & arms lore training gumps too
using System;
using Server;
using Server.Gumps; // seems to run better with this in - may not be needed :)
using Server.Mobiles; // because we make a call to player mobile, we need this
using Server.Items; // may not be needed, but again seems to run smooth with it
using Server.Network; // need this to call the netstate in the button on respons section

namespace Server.Gumps // can be what ever we want, but using the standard of gumps, so other things can call it easier if need be
{
	public class AnatomyTrainingGump : Gump // name of our gump, and stating that it is a gump
	{
		string[] goodsay = new string[] // this is a list of strings for "correct" responses, can add/modify/remove with no problem to the list
		{	// just remember must start with " and end with ", -- and can not use " inside of it - use ' or * to signify a quote
			"I think we have a winner!",
			"At least some one has been watching?",
			"Good Job",
			"You may get a date yet",
			"Right on the money",
			"Give this guy a Cupie Doll",
			"You have been listening to me",
			"Dates are in trouble now",
			"Good going, but keep on practicing",
			"I think he hot it!",
			"You finialy remembered which was left & right",
			"Bam, you got it right",
			"You are improving",
			"You got it right this time, care to try again?",
		};

		string[] badsay = new string[] // string list for incorrect answers - as above same rules apply
		{
			"Go back to hiding in dark corners",
			"Do you know you are sapposed to point to the correct answer?",
			"I hope you where joking",
			"What are you thinking?",
			"We are not doing the Hokie Pokie here",
			"Guessing is not a good idea, could lead to problems later on",
			"Are you even listening in class?",
			"I think counting sheep might be to hard for you",
			"I hope you did not pay in advance for this course",
			"Don't be shy, it is just a dummy",
			"Did you mean your other left?",
			"I think we need an other dunce cap over here",
			"Are you trying to point at it or tickle it?",
			"At least try to get the correct answer once in a while",
		};

		private static void SayRandom(string[] say, Mobile m) // this is what sends the message to the player
		{ // we pas allong the time of string list to use and the mobile
			m.SendMessage(say[Utility.Random(say.Length)]);
			// send message to mobile m -- the message is a random one from the message list choosen
		}

		private int m_thespot; // the numer being used as a "marker" to sink up question with correct answer
		public int thespot { get { return m_thespot; } set { m_thespot = value; } } // standard settings so it is usable through out the script

		public AnatomyTrainingGump() : base( 50, 50 ) // making it public so anything can call it and having it start at 50 x by 50 y pixels from the top left corner
		{
			thespot = Utility.Random(6) + 1; // generate which set to use (display & answer)
			this.Closable=true; // setting it so the gump can be closed by right clicking
			this.Disposable=true; // setting it so it can be closed
			this.Dragable=true; // setting it so it can be moved by the player
			this.Resizable=false; // setting it so it can not be resized
			this.AddPage(0); // setting up page 1 (it only has 1 page, but still need this)
			this.AddImage(0, 0, 71); // adding in our background images
			this.AddImage(128, -3, 71); // because of their roug edges, they each
			this.AddImage(241, -4, 71); // need to be placed so they line up nicely
			this.AddImage(1, 130, 71); // instead of an even spacing
			this.AddImage(127, 130, 71); // 1st number is x form the top left corner of the gump
			this.AddImage(243, 127, 71); // 2nd number is the y placement
			this.AddImage(3, 252, 71); // 3rd number is the gump id number
			this.AddImage(118, 253, 71); // this remains true for all images placed
			this.AddImage(244, 256, 71); // last of background images
			if (Utility.RandomBool()) this.AddImage(222, -32, 12); // does a random check of true/false - if true male pic
			else this.AddImage(222, -32, 13); // if false female pick
			if (thespot == 1) this.AddImage(307, 12, 3); // if #1 is the set to use place in the image (head spot here)
			else if (thespot == 2) this.AddImage(268, 60, 3); // else if #2 place this one
			else if (thespot == 3) this.AddImage(344, 60, 3); // etc
			else if (thespot == 4) this.AddImage(310, 77, 3);
			else if (thespot == 5) this.AddImage(278, 130, 3);
			else if (thespot == 6) this.AddImage(331, 130, 3);
			this.AddLabel(30, 45, 33, @"What is the name of the body"); // these are lables to ask our question with
			this.AddLabel(30, 75, 33, @"part marked here on the dummy?");
			this.AddLabel(30, 105, 33, @"Remember this is the Dummy's");
			this.AddLabel(30, 135, 33, @"part and not yours");
			this.AddButton(30, 200, 210, 211, (int)Buttons.Button1, GumpButtonType.Reply, 0); // this is the button for answer #1
			this.AddButton(30, 230, 210, 211, (int)Buttons.Button2, GumpButtonType.Reply, 0); // for #2
			this.AddButton(30, 260, 210, 211, (int)Buttons.Button3, GumpButtonType.Reply, 0); // etc
			this.AddButton(30, 290, 210, 211, (int)Buttons.Button4, GumpButtonType.Reply, 0); // 1st 2 numbers are x & y
			this.AddButton(30, 320, 210, 211, (int)Buttons.Button5, GumpButtonType.Reply, 0); // 3rd & 4th are normal look and "pushed" look
			this.AddButton(30, 350, 210, 211, (int)Buttons.Button6, GumpButtonType.Reply, 0);
			this.AddLabel(60, 200, 33, @"The Dummy's Head"); // these are the labels for the answers
			this.AddLabel(60, 230, 33, @"The Dummy's Right Arm");
			this.AddLabel(60, 260, 33, @"The Dummy's Left Arm");
			this.AddLabel(60, 290, 33, @"The Dummy's Navel/Torso");
			this.AddLabel(60, 320, 33, @"The Dummy's Right Leg");
			this.AddLabel(60, 350, 33, @"The Dummy's Left Leg");
			this.AddButton(332, 362, 241, 243, (int)Buttons.Button7, GumpButtonType.Reply, 0); // this is the exit button
		}
		
		public enum Buttons // this enumerates out buttons
		{
			Button0, // added so 1st button is not treaded as right clicking
			Button1, // our answer buttons
			Button2,
			Button3,
			Button4,
			Button5,
			Button6,
			Button7, // and lastly our exit button
		}

		public override void OnResponse(NetState sender, RelayInfo info ) // this is called when you click a button
		{
			Mobile m = sender.Mobile; // we want to make the person that clicked into a mobile
			PlayerMobile pm = m as PlayerMobile; // and we also need to make them into a playermobile
			if (pm == null) return; // and if some how a critter dbl click it, this exits it out
			pm.CheckSkill( SkillName.Anatomy, 0.0, 160.0 ); // this calls our check for skill gain possiblility
								// raising the 160 makes it "harder to do" and lowering it makes it easier
								// but since nothing else but gaining here, 160 is a good average number for it
			switch ( info.ButtonID ) // we want to go to the button pushed
			{
				case 0: default: { break; } // if right clicked or some how then use a macro on it with the wrong number set so no out of range messages
				case 1: // if button 1 was pushed (all other button for ansers will be the same as this set)
				{
					if (thespot == 1) // if this button was the correct answer - matches the set that was choosen
					{
						SayRandom(goodsay, pm); // gives them a random "correct" response from the list of sayings
						pm.CheckSkill( SkillName.Anatomy, 0.0, 160.0 ); // bonus skill check for getting the correct answer
					}
					else SayRandom(badsay, pm); // if wrong answer - just says the random "incorrect" response string
				} break; // end of that button
				case 2:
				{
					if (thespot == 2)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.Anatomy, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 3:
				{
					if (thespot == 3)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.Anatomy, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 4:
				{
					if (thespot == 4)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.Anatomy, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 5:
				{
					if (thespot == 5)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.Anatomy, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 6:
				{
					if (thespot == 6)
					{
						SayRandom(goodsay, pm);
						pm.CheckSkill( SkillName.Anatomy, 0.0, 160.0 );
					}
					else SayRandom(badsay, pm);
				} break;
				case 7: { break; } // if the cancel button was pushed
			}
		}
	}
}