using System;

namespace Server.Items
{
	[Flipable( 0x0E15, 0x0E16, 0x0E18, 0x0E19 )]
	public class DailyPlayingCards : BaseDailyRare
	{
		public override int ArtifactRarity{ get{ return 0; } }
		
		[Constructable]
		public DailyPlayingCards() : base( 0x0E19 )
		{
			Name = "Playing Cards";
		}

		public DailyPlayingCards( Serial serial ) : base( serial )
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