////////////////////////////////////
/////
/////By graverobbing
/////
////////////////////////////////////

using System;
using Server.Items;

namespace Server.Items
{
	public class RareTubStone : Item
	{
		public override string DefaultName
		{
			get { return "RareTub Stone"; }
		}

		[Constructable]
		public RareTubStone() : base( 3796 )
		{
			Movable = false;
			Hue = 2558;

		}

		public override void OnDoubleClick( Mobile from )
		{
		   	Item[] Token = from.Backpack.FindItemsByType( typeof( Gold ) );
		   	if ( from.Backpack.ConsumeTotal( typeof( Gold ), 15000 ) )//Cost
		{
         	RareTub RareTub = new RareTub();
		   	from.AddToBackpack( RareTub );
			from.SendMessage( "The total of thy purchase is 15000 gold. My thanks for the patronage." );//Gold removed message
		}
		   	else
		   	{
		   		from.SendMessage( "Begging thy pardon, but thou canst not afford that." );
		   	}
					
		}

		public RareTubStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}