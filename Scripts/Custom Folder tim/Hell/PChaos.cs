using System;
using Server;

namespace Server.Items
{
	public class PChaos : Item
	{
		[Constructable]
		public PChaos() : this( 1 )
		{
		}

		[Constructable]
		public PChaos( int amount ) : base( 0xF7D )
		{
			Stackable = false;
                        Name = "Eluvial Slime";
                        Hue = 2126;
			Weight = 1;
			Amount = amount;
		}

		

		public PChaos( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}