using System;
using System.Collections;
using Server;
using Server.Network;

namespace Server.Items
{
	public class Chocolatecoin : Food
	{

		[Constructable]
		public Chocolatecoin() : this( 1 )
		{
		}
		
		[Constructable]
		public Chocolatecoin( int amount ) : base( 0xF8F, amount )
		{
			this.Stackable = true;
			this.Hue = 1271;
			this.Name = "a Chocolate coin";
			this.ItemID = 6256;
			this.Amount = amount;
			this.Weight = 0;
			this.FillFactor = 1;
		}

		public Chocolatecoin( Serial serial ) : base( serial )
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
