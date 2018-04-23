using System;
using Server;

namespace Server.Items
{
	[Flipable( 0x1F03, 0x1F04 )]
    public class HolidayRobe : BaseOuterTorso, ITokunoDyable
	{
		public override int LabelNumber{ get{ return 1007150; } } // Happy Holidays !!

		[Constructable]
		public HolidayRobe() : base( 0x1F03, 0x1F04 )
		{
			Weight = 3.0;
			Hue = 1157;
			Attributes.Luck = 140;
			Attributes.RegenMana = 2;
			Attributes.DefendChance = 5;

		}

		public HolidayRobe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}