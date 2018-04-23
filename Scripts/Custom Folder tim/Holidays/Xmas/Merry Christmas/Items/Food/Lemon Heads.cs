using System;
using System.Collections;
using Server;
using Server.Network;

namespace Server.Items
{
	public class Lemonhead : Food
	{

		[Constructable]
		public Lemonhead() : this( 1 )
		{
		}
		
		[Constructable]
		public Lemonhead( int amount ) : base( 0xF8F, amount )
		{
			this.Stackable = true;
			this.Hue = 0;
			this.Name = "a Lemon head";
			this.ItemID = 5928;
			this.Amount = amount;
			this.Weight = 1;
			this.FillFactor = 1;
		}

		public Lemonhead( Serial serial ) : base( serial )
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
