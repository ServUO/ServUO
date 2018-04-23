using System;

namespace Server.Items
{
	[Flipable( 10588, 10589 )]
	public class DailyPinkFuton : BaseDailyRare
	{
		public override int ArtifactRarity{ get{ return 0; } }
		
		[Constructable]
		public DailyPinkFuton() : base( 10588 )
		{
			Name = "Pink Futon";
		}

		public DailyPinkFuton( Serial serial ) : base( serial )
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