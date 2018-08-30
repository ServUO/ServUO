using System;

namespace Server.Items
{
	public class RibcageXylophone : BaseInstrument
	{
		public override int InitMinUses{ get{ return 750; } }
		public override int InitMaxUses{ get{ return 750; } }

		[Constructable]
		public RibcageXylophone() : base( 0x1B17, 0x1C3, 0x1C7 )
		{
			Weight = 2.0;
			Name = "Moldy Ribcage Xylophone";
			Hue = 665;
			Slayer = SlayerName.Repond;
		}

		public RibcageXylophone( Serial serial ) : base( serial )
		{
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

			if ( Weight == 3.0 )
				Weight = 5.0;
		}
	}
}