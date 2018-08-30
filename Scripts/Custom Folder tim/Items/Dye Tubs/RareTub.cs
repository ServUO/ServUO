////////////////////////////////////
/////
/////By graverobbing
/////
////////////////////////////////////

using System;

namespace Server.Items
{
	public class RareTub : DyeTub
	{
		[Constructable]
		public RareTub()
		{
			Name = "Raretub";
			Hue = DyedHue = Utility.RandomMinMax(2470,2520);
			Redyable = false;
		}

		public RareTub( Serial serial ) : base( serial )
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