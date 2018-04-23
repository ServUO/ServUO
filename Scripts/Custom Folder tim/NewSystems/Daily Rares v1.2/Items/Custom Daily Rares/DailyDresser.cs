using System;

namespace Server.Items
{
	[Flipable( 0xA2C, 0xA34)]
	public class DailyDresser : BaseDailyRareContainer
	{
		public override int DefaultGumpID{ get{ return 0x51; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		public override int ArtifactRarity{ get{ return 0; } }
		
		[Constructable]
		public DailyDresser() : base( 0xA2C )
		{
		}

		public DailyDresser( Serial serial ) : base( serial )
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