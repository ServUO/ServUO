using System;

namespace Server.Items
{
	public class RareIngot : IronIngot
	{
		
		
		[Constructable]
		public RareIngot()
		{
			int chance = Utility.Random( 100 );

			if ( chance <= 5 )
				ItemID = 0x1BEF;
		}

		public RareIngot( Serial serial ) : base( serial )
		{
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1049644, "Daily Rare" );
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}