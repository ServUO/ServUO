using System;

namespace Server.Items
{
	public class DailyPlateOfCookies : Food
	{
		
		
		[Constructable]
		public DailyPlateOfCookies() : base( 0x160C )
		{
			Name = "Plate Of Cookies";
			Weight = 1.0;
		}

		public DailyPlateOfCookies( Serial serial ) : base( serial )
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