//PowerScroll Exchange!!
//By: DxMonkey - AKA - Tresdni
//Ultima Eclipse - www.ultimaeclipse.com
using System;
using Server.Items;
using Server.Gumps;

namespace Server.Items
{
	public class PowerscrollExchange : Item
	{
		public static int i_PSReward = 0;
		
		[Constructable]
		public PowerscrollExchange() : base( 0xE77 )
		{
			Movable = false;
			Hue = 1153;
			Name = "The Powerscroll Exchange (Double-Click For Menu)";

		}

		public override void OnDoubleClick( Mobile from )
		{

			if ( !from.HasGump( typeof( PSCreditsGump ) ) )
				{
					from.SendGump( new PSCreditsGump());
				}
			else
				from.SendMessage ( "You already have this menu open!" );
            
		}
		
		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			
			if ( dropped is PowerScroll )
			{
				PowerScroll deed = (PowerScroll)dropped;
				i_PSReward = 0; //default value
				if ( deed.Value == 120.0 )
					i_PSReward = 500;
				else if ( deed.Value == 115.0 )
					i_PSReward = 50;
				else if ( deed.Value == 110.0 )
					i_PSReward = 5;
				else if ( deed.Value == 105.0 )
					i_PSReward = 1;		
			}
			
			
			
			else i_PSReward = 0;
			
			
			dropped.Delete();
			
			if ( i_PSReward > 0 )
			{
				from.AddToBackpack( new PSCredits(i_PSReward) );
				from.SendMessage( 1173, "You were rewarded {0} Powerscroll Credits for this scroll.", i_PSReward );
			}
			
			else if (i_PSReward == 0 ) from.SendMessage( 1173, "This was not a Powerscroll, but it was deleted anyways." );
			
			return true;
		
		}

		public PowerscrollExchange( Serial serial ) : base( serial )
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