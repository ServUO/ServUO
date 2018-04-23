//By Zero
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class BaseWorldArty : GruesomeStandardArtifact
  {
      [Constructable]
		public BaseWorldArty()
		{
          		Name = "Base World Stealable" ;
          		Hue = 1919 ;
	  	        ItemID = 3796 ;
		        
		}

		public BaseWorldArty( Serial serial ) : base( serial )
		{
		}


		public override int ArtifactRarity{ get{ return 12; } }

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
