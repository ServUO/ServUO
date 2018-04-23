using System;
using System.Collections;
using Server;
using Server.Network;

namespace Server.Items
{
	public class SummerSausage : Food
	{

		[Constructable]
		public SummerSausage() : this( 1 )
		{
		}
		
		[Constructable]
		public SummerSausage( int amount ) : base( 0x9C0, amount )
		{
			this.Stackable = true;
			this.Hue = 0;
			this.Name = "a Summer sausage";
			this.ItemID = 0x9C0;
			this.Amount = amount;
			this.Weight = 1;
			this.FillFactor = 2;
		}

		public SummerSausage( Serial serial ) : base( serial )
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
