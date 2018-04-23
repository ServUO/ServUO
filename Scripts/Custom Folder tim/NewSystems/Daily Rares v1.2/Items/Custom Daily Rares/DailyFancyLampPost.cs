using System;

namespace Server.Items
{
	public class DailyFancyLampPost : BaseDailyRareLight
	{
		public override int LitItemID{ get { return 0xB24; } }
		public override int UnlitItemID{ get { return 0xB25; } }

		public override int ArtifactRarity{ get{ return 0; } }
		[Constructable]
		public DailyFancyLampPost() : base( 0xB25 )
		{
			Duration = TimeSpan.Zero;

			Burning = false;
			Light = LightType.Circle300;
		}

		public DailyFancyLampPost( Serial serial ) : base( serial )
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