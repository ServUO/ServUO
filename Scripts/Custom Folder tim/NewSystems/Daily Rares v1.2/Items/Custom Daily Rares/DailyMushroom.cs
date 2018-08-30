using System;

namespace Server.Items
{
	/*[Flipable( 0x222E, 0x222F, 0x2230, 0x2231)]*/
	public class DailyMushroom : BaseDailyRare
	{
		public override int ArtifactRarity{ get{ return 0; } }
		[Constructable]
		public DailyMushroom() : base(Utility.RandomList( 0x222E, 0x222F, 0x2230, 0x2231) )
		{
		}

		public DailyMushroom( Serial serial ) : base( serial )
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