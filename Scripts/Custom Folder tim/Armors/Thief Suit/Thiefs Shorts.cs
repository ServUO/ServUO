// Created by G00BER
using System;
using Server;

namespace Server.Items
{
	public class ThiefShorts : ShortPants
	{
		public override int LabelNumber{ get{ return 1094910; } } // ThiefShorts


		[Constructable]
		public ThiefShorts()
		{
			Hue = 0;
			Name = "Thief Shorts";

			SkillBonuses.SetValues( 0, SkillName.Stealing, 5.0 );
			SkillBonuses.SetValues( 1, SkillName.Stealth, 5.0 );
			SkillBonuses.SetValues( 2, SkillName.Begging, 5.0 );
		}

		public ThiefShorts( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
