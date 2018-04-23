using System;

namespace Server.Items
{
	[Flipable( 18098, 18099 )]
	public class DailyScribeScrollG : BaseDailyRare
	{
		public override int ArtifactRarity{ get{ return 0; } }
		
		[Constructable]
		public DailyScribeScrollG() : base( 18098 )
		{
		}

		public DailyScribeScrollG( Serial serial ) : base( serial )
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