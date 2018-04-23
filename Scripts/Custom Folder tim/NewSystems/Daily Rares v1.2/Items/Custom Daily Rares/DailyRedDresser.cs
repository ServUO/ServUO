using System;

namespace Server.Items
{
	[Flipable( 0xA38, 0xA30)]
	public class DailyRedDresser : BaseDailyRareContainer
	{
		public override int DefaultGumpID{ get{ return 0x48; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		public override int ArtifactRarity{ get{ return 0; } }
		[Constructable]
		public DailyRedDresser() : base( 0xA38 )
		{
		}

		
		public DailyRedDresser( Serial serial ) : base( serial )
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