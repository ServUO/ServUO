//GM Arthanys - Mystara Shard: www.mystara.com.br
using System;
using Server.Items;
using Server.Gumps;
using Server.Accounting;

namespace Server.Items
{
	[Flipable( 0xE40, 0xE41 )]
	public class SuggestionBox : Item
	{
		[Constructable]
		public SuggestionBox() : base( 0xE40 )
		{
			Movable = false;
			Hue = 1965;
			Name = "Suggestion Box";
		}

		public override void OnDoubleClick( Mobile from )
		{
                  
			from.SendGump (new Suggestion());
		
		   	
					
		}

		public SuggestionBox( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
