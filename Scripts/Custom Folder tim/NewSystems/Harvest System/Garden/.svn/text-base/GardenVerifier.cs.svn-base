//==============================================//
// Base Created by Dupre					//
// Masterfully Modified (to be a garden)--by DarkJustin from The Bluegrass Shard-- Where Tweaking is life... :-p							//
//==============================================//
using System; 
using Server; 
using Server.Items;
using System.Collections;
using Server.Multis;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{   
   public class GardenVerifier : Item
	{
		[Constructable]
		public GardenVerifier() : base( )
		{
		ItemID = 6256;
		Stackable = false;
		Name = "Garden Verifier (ADMIN/GM DO NOT DELETE)";
		Weight = 0;
		Movable = false;
		Visible = false;
		}
		
		public GardenVerifier( Serial serial ) : base( serial )
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
