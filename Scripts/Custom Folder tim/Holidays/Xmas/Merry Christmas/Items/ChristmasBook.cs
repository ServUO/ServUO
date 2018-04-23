using System;
using Server;

namespace Server.Items
{
	public class ChristmasBook : RedBook
	{
		[Constructable]
		public ChristmasBook() : base( "Merry christmas", "Staff", 3, false )
		{
			Hue = 1153;

			Pages[0].Lines = new string[]
				{
					"   Merry Christmas!  ",
					" ",
					"  Here is a Christmas",
					" goodie bag.",				
				};

			Pages[1].Lines = new string[]
				{
					"  To do this, simply.",
					"walk up to any vendor",
					"in any town and say",
					"'Merry Christmas'.  The",
					"vendor's will give you",
					"goodies and you may",
					"even receive a special",
					"Christmas novelty item."
				};

			Pages[2].Lines = new string[]
				{
					"  Have Fun and Good",
					"Luck!"
				};
		}

		public ChristmasBook( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}