using System;
using Server;

namespace Server.Items
{
	public class PSCredits : Item
	{
		[Constructable]
		public PSCredits() : this( 1 )
		{
		}

		[Constructable]
		public PSCredits( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public PSCredits( int amount ) : base( 0xF8E )
		{
			Stackable = true;
			Name = "Powerscroll Credits";
			Hue = 1153;
			Weight = 0;
			LootType = LootType.Blessed;
			Amount = amount;
		}

		public PSCredits( Serial serial ) : base( serial )
		{
		}

		public override int GetDropSound()
		{
			if ( Amount <= 1 )
				return 0x2E4;
			else if ( Amount <= 5 )
				return 0x2E5;
			else
				return 0x2E6;
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