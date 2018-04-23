using System;

namespace Server.Items
{
	[Flipable( 8756, 8757 )]
	public class DailyDragonTrophy : BaseDailyRare
	{
		public override int ArtifactRarity{ get{ return 0; } }
		
		[Constructable]
		public DailyDragonTrophy() : base( 8757 )
		{
			Name = "A Red Dragon Trophy";
		}

		public DailyDragonTrophy( Serial serial ) : base( serial )
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