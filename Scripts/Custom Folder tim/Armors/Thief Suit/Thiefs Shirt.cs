// Created by G00BER
using System;
using Server;

namespace Server.Items
{
	public class ThiefShirt : FancyShirt
	{
		public override int LabelNumber{ get{ return 1094910; } } // Thief Shirt


		[Constructable]
		public ThiefShirt()
		{
			Hue = 0;
			Name = "Thief Shirt";

			SkillBonuses.SetValues( 0, SkillName.Stealing, 5.0 );
			SkillBonuses.SetValues( 1, SkillName.Stealth, 5.0 );
			SkillBonuses.SetValues( 2, SkillName.Begging, 5.0 );
		}

		public ThiefShirt( Serial serial ) : base( serial )
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
