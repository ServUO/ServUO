using System;

namespace Server.Items
{
	[Flipable( 17097, 17098 )]
	public class DailyUOBanner : BaseDailyRare
	{
		public override int ArtifactRarity{ get{ return 0; } }
		
		[Constructable]
		public DailyUOBanner() : base( 17097 )
		{
		}

		public DailyUOBanner( Serial serial ) : base( serial )
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

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}