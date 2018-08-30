using System;

namespace Server.Items
{
	public class DailyFlax : BaseDailyRare
	{
		public override int ArtifactRarity{ get{ return 0; } }
		[Constructable]
		public DailyFlax() : base( 0x1A9B )
		{
		}

		public DailyFlax( Serial serial ) : base( serial )
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