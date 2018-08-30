using System;
using Server;

namespace Server.Items
{
	public class DailyCandle : BaseDailyRareLight
	{
		public override int LitItemID{ get { return 0xB1A; } }
		public override int UnlitItemID{ get { return 0xA26; } }

		public override int ArtifactRarity{ get{ return 0; } }
		
		[Constructable]
		public DailyCandle() : base( 0xA26 )
		{
			Duration = TimeSpan.Zero;

			Burning = false;
			Light = LightType.Circle150;
		}

		public DailyCandle( Serial serial ) : base( serial )
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
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}