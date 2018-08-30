using System;

namespace Server.Items
{
	[Flipable( 0x2936, 0x2937)]
	public class DailyMenu : BaseDailyRare
	{
		public override int ArtifactRarity{ get{ return 0; } }
		[Constructable]
		public DailyMenu() : base( 0x2936 )
		{
			Name = "a menu";
		}

		public DailyMenu( Serial serial ) : base( serial )
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