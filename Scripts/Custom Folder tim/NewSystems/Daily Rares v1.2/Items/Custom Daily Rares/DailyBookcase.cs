using System;

namespace Server.Items
{
	[Flipable( 0xA97, 0xA99 )]
	public class DailyBookcase : BaseDailyRareContainer
	{
		public override int DefaultGumpID{ get{ return 0x4D; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 80, 5, 140, 70 ); }
		}

		public override int ArtifactRarity{ get{ return 0; } }
		
		[Constructable]
		public DailyBookcase() : base( 0xA97 )
		{
		}

		public DailyBookcase( Serial serial ) : base( serial )
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