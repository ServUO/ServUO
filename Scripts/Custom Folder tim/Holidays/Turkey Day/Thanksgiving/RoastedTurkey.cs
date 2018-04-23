using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{

	public class RoastedTurkey : Food
	{
		[Constructable]
		public RoastedTurkey() : this( 1 )
		{
		}

		[Constructable]
		public RoastedTurkey( int amount ) : base( amount, 0x9B7 )
		{
			Name = "2013 Oven Roasted Turkey";
			Hue = 1608;
			this.Weight = 10.0;
			this.FillFactor = 20;
		}

		public RoastedTurkey( Serial serial ) : base( serial )
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