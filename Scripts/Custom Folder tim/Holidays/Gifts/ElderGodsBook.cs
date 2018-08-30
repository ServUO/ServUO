//
//	Santa Quest 2013 - version 2.0, by Arachaic
//
//
using System;
using Server;

namespace Server.Items
{
	// books: Brown 0xFEF, Tan 0xFF0, Red 0xFF1, Blue 0xFF2, 
	// OpenSmall 0xFF3, Open 0xFF4, OpenOld 0xFBD, 0xFBE

	public class ElderGodsBook : BaseBook
	{
		private const string TITLE = "The Elder Gods";
		private const string AUTHOR = "J.H. Smithcraft";
		private const int PAGES = 11;
		private const bool WRITABLE = false;
		
		[Constructable]
		public ElderGodsBook() : base( Utility.RandomList( 0x2253 ), TITLE, AUTHOR, PAGES, WRITABLE )
		{
			Hue = 1608;

			// NOTE: There are 8 lines per page and
			// approx 22 to 24 characters per line.
			//  0----+----1----+----2----+
			int cnt = 0;
			string[] lines;

			lines = new string[]
			{
				"In the beginning, gods", 
				"ruled our world. They",
				"walked as we do on the",
				"surface and lived as we",
				"do now. However, the gods",
				", just as all creatures,",
				"grew tired of the day to",
				"day life.",
			};
			Pages[cnt++].Lines = lines;

			lines = new string[]
			{
				"Factions began to arrise",
				", each loyal to a god.",
				"Great wars were fought",
				"and in the end the old",
				"gods returned to the",
				"earth to sleep for untold",
				"years, though many left",
				"traveling through space.",
			};
			Pages[cnt++].Lines = lines;

			lines = new string[]
			{
				"The Gods",
				"God of The Waters",
				"He Who Slumbers rules",
				"over the waters and", 
				"protects fishermen and",
				"sea life alike.",
				"His temple is said to ",
				"rest at the bottom of",
			};

			Pages[cnt++].Lines = lines;


			lines = new string[]
			{
				"the ocean near what is",
				"now known as Bucc's",
				"Den.",
				"",
				"God of The Forest",
				"Oaks rules over the",
				"forest protecting all",
				"natural creatures within.",
			};

			Pages[cnt++].Lines = lines;


			lines = new string[]
			{
				"It is said that he incased",
				"himself in the mountain",
				"ridge where Cove now sits.",
				"",
				"God Of Death",
				"Dozer rules over the",
				"dead in Hell. It is said",
				"that a portal to hell lies",
			};

			Pages[cnt++].Lines = lines;

			lines = new string[]
			{
				"at the end of the dungeon",
				"Depise and that Dozer can",
				"still be seen coming and",
				"going, ushering the souls",
				"of the dead into hell.",
				"",
				"The God of Time",
				"The Astro Daemon controls",
			};
			Pages[cnt++].Lines = lines;
			
			lines = new string[]
			{
				"space and time. He bends",
				"it as he sees fit. It",
				"is impossible to truely",
				"kill such a beast for he",
				"knows the outcome of",
				"everything and has the ",
				"ability to change it even",
				"after death.",
			};
			Pages[cnt++].Lines = lines;

			lines = new string[]
			{
				"His current location is",
				"unknown for he left ",
				"Sosaria with other gods",
				"after the wars. This",
				"God is by far the most",
				"powerful on record and",
				"is arrival will announce",
				"the end for us all.",
			};
			Pages[cnt++].Lines = lines;

			lines = new string[]
			{
				"God of Life",
				"Moy is the God Of Life.",
				"He gave breath to all",
				"that lives today.",
				"From him man was",
				"birthed and the trees",
				"rooted. His where",
				"abouts are unknown.",
			};
			Pages[cnt++].Lines = lines;

			lines = new string[]
			{
				"The Old Gods are gone",
				", but not forgotten.",
				"Many prophets have told",
				"of a day when they will",
				"return. On this day",
				"the dead will walk and",
				"the rivers will run red",
				"with the blood of all.",
			};
			Pages[cnt++].Lines = lines;

		}

		public ElderGodsBook( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); 
		}
	}
}