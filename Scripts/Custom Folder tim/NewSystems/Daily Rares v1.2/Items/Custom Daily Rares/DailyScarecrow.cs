using System;

namespace Server.Items
{
	[Flipable( 0x1E35, 0x1E34)]
	public class DailyScareCrow : BaseDailyRare
	{
		public override int ArtifactRarity{ get{ return 0; } }
		[Constructable]
		public DailyScareCrow() : base( 0x1E35 )
		{
		}

		public DailyScareCrow( Serial serial ) : base( serial )
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