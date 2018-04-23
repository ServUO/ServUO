/*
Script Name: ValentineCandy.cs
Author: CEO
Version: 1.0
Public Release: 02/10/07
*/ 
using System;
using Server;
using Server.Network;
/*
 * NECCO is the oldest continuously run, multi-line candy company 
 * that produces classic candy like Sweethearts Conversation Hearts, 
 * NECCO“ Wafers, Mary Jane“ and Clark“ bars.  With more than 100 
 * years of history, Sweethearts Conversation Hearts are the original 
 * and top-selling Valentineís Day treat.  Each year, NECCO produces 
 * eight billion Sweethearts Conversation Hearts during the six weeks 
 * leading up to Valentineís Day. 
 * 
 * http://www.necco.com/Default.asp
 * 
 */

namespace Server.Items
{
	public class ValentineCandy : Food
	{
		private InternalTimer toothache;
		private int phraseIndex;
		private string[] NicePhrases = new string[] { "Be Mine" , "Be Good", "Miss You", "Nice Girl", "So Fine", "Got Love?",
                                                "Be True", "Kiss Me", "Sure Love", "Thank You", "True Love", "Go Girl",
												"Meet me in Tram", "To Fel!", "Doom Doom Doom", "Let's do a Champ!",
												"Luna Bank Time", "Wanna Hunt?", "IMA Mage", "Tamer's Rock", "Warrior's have big feet",
												"To The Casino!", "Necros r Scary", "Do U Sew?", "Miner's Rock!",
												"Where's My Money?", "I Love this Shard", "No Geeks Here", "Are you sure?",
												"Whaaaat's UuuuuP?", "Me & You", "Let's Go", "Champ ne1?", "What's an FAQ?",
												"Wishful thinking", "You Wanna What?", "Bow To Me!", "I'm Broke", "I'm Rich",
												"You Rule", "You Wish", "TLC", "Only You", "Marry Me", "Let's Kiss",
												"I Heart U", "Friend", "My Hero", "EUO Rocks", "ROFLMAO", "Whatever",
												"Let It Be", "It's Love", "Unreadable!", "How Nice", "Hug Me", "Hi Love",
                                                "Sweet Talk", "My Pet", "ur a 10", "ur a qt", "ur a star", "ur kind", "LOL",
                                                "Bear Hug", "Go Fish", "Love Bird", "Whiz Kid", "Wise Up", "Write Me",
												"Dream Girl", "Get Real", "First Kiss", "Call Me", "CEO Rulez", "Sweet Thing",
                                                "Take A Walk", "Purr Fect", "Cool Cat", "Top Dog", "Yes Dear", "You & Me",
                                                "Puppy Love", "URA Tiger", "Lover Boy", "Lover Girl", "Email Me" };
		private string[] MeanPhrases = new string[] { "That Stinks" , "What Smells?", "Take a Hike", "You Stink", "Get Lost", "Tamer's r Lamers",
												"Noob", "Trammy", "PVP Meat", "Loser", "Balron Breath", "Lizard Face", "You're Wierd", "Fight Me!",
                                                "Wishful Thinking", "Eat My Fireball!", "Mage's r weak", "Warrior's r Dumb", "I'm Bad",
												"Die!", "Eat Dirt", "RoadKill", "Meany", "Huh?", "RTFM", "Get a Job", "Bank Sitter" };

		[Constructable]
		public ValentineCandy()
			: base(0x9EA)
		{
			Stackable = false;
			this.Name = "a Valentine's Day Candy";
			this.Weight = 1.0;
			this.FillFactor = 0;
			//Original NECCO Flavors: cherry-32, banana-54, lemon-356, grape-18, orange-43 and wintergreen-88
			this.Hue = Utility.RandomList(32, 54, 356, 18, 43, 88);
			if (Utility.RandomDouble() < 0.10)
				this.phraseIndex = Utility.Random(MeanPhrases.Length) * -1;
			else
				this.phraseIndex = Utility.Random(NicePhrases.Length);
		}

		public ValentineCandy(Serial serial)
			: base(serial)
		{
		}

		private int GetPhrase()
		{
			return 0;
		}

		public override bool Eat(Mobile from)
		{
			from.PlaySound(Utility.Random(0x3A, 3));

			if (from.Body.IsHuman && !from.Mounted)
				from.Animate(34, 5, 1, true, false, 0);

			if (Poison != null)
				from.ApplyPoison(Poisoner, Poison);

			if (Utility.RandomDouble() < 0.05)
				GiveToothAche(from, 0);
			else
				from.SendLocalizedMessage(1077387);
			from.PublicOverheadMessage(MessageType.Regular, this.Hue, false, phraseIndex < 0 ? MeanPhrases[Math.Abs(phraseIndex)] : NicePhrases[phraseIndex]);
			Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x36B0, 1, 14, this.Hue - 1, 7, 9915, 0);
			Effects.PlaySound(from.Location, from.Map, 0x229);
			Consume();
			return true;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			AddNameProperties(list);
			list.Add(1070722, phraseIndex < 0 ? MeanPhrases[Math.Abs(phraseIndex)] : NicePhrases[phraseIndex]);
		}

		public void GiveToothAche(Mobile from, int seq)
		{
			if (toothache != null)
				toothache.Stop();
			from.SendLocalizedMessage(1077388 + seq);
			if (seq < 5)
			{
				toothache = new InternalTimer(this, from, seq, TimeSpan.FromSeconds(15));
				toothache.Start();
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
			writer.Write(phraseIndex);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			phraseIndex = reader.ReadInt();
		}

		private class InternalTimer : Timer
		{
			private ValentineCandy i_ValentineCandy;
			private int i_sequencer;
			private Mobile i_mobile;

			public InternalTimer(ValentineCandy ValentineCandy, Mobile m, int sequencer, TimeSpan delay)
				: base(delay)
			{
				Priority = TimerPriority.OneSecond;
				i_ValentineCandy = ValentineCandy;
				i_mobile = m;
				i_sequencer = sequencer;
			}

			protected override void OnTick()
			{
				if (i_mobile != null)
				{
					i_ValentineCandy.GiveToothAche(i_mobile, i_sequencer + 1);
				}
			}
		}
	}
}