using System;

namespace Server.Items
{
	[Flipable( 7783, 7776)]
	public class DailyVBTrophy : BaseDailyRare
	{
		public override int ArtifactRarity{ get{ return 0; } }
		
		[Constructable]
		public DailyVBTrophy() : base( 7783 )
		{
			Name = "Prized Valorite-Fur Bear Trophy";
			Hue = 2518;
		}

		public DailyVBTrophy( Serial serial ) : base( serial )
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