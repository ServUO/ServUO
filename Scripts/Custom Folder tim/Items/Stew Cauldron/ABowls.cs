using System;

namespace Server.Items
{
	public class PewterBowlOfStew : Food
	{
		[Constructable]
		public PewterBowlOfStew() : base( 0x1603 )
		{
                        Name = "Bowl Of Stew";
			Stackable = false;
			Weight = 2.0;
			FillFactor = 10;
		}

		public override bool Eat( Mobile from )
		{
			if ( !base.Eat( from ) )
				return false;

			from.AddToBackpack( new EmptyPewterTub() );
			return true;
		}

		public PewterBowlOfStew( Serial serial ) : base( serial )
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
