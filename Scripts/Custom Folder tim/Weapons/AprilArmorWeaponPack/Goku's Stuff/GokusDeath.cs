//////////////////////
//Created by KyleMan//
//////////////////////
using System;
using Server;

namespace Server.Items
{
	public class GokusDeath : Item
	{
		[Constructable]
		public GokusDeath() : base( 0x1869 )
		{
			Weight = 1.0;
			Name = "oh my god you killed goku";
			Hue = 1152;
		}

		public GokusDeath( Serial serial ) : base( serial )
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
		
		public override void OnDoubleClick( Mobile from ) 
		{ 
			switch ( Utility.Random( 10 ) )
			{
				default:
				case  0: from.SendMessage( "You're Going On a Guilt Trip Now" ); break;
				case  1: from.SendMessage( "You Killed Goku" ); break;
				case  2: from.SendMessage( "How Could You?" ); break;
				case  3: from.SendMessage( "You Destroyed the Worlds Only Hope" ); break;
				case  4: from.SendMessage( "Whose Going to Tell ChiChi?" ); break;
				case  5: from.SendMessage( "Stop Clicking Me" ); break;
				case  6: from.SendMessage( "Congrates on Killing Goku" ); break;
				case  7: from.SendMessage( "Job Well Done" ); break;
				case  8: from.SendMessage( "*crys*" ); break;
				case  9: from.SendMessage( "The World is Mourning Gokus Loss" ); break;
			}
		}
	}	
}