namespace Server.Mobiles
{
	public class SWPlayerMobile : PlayerMobile
	{
		public SWPlayerMobile()
		{
		}


		public SWPlayerMobile( Serial serial ) : base( serial )
		{
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)8 ); // version
		}


		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}