using System;
using Server;

namespace Server.Items
{
	public class AlchemistsBibelot : GoldRing
	{
		public override int LabelNumber{ get{ return 1070638; } }

		[Constructable]
		public AlchemistsBibelot()
		{
			Name = "Alchemists Bibelot";
			Hue = 0x290;
			SkillBonuses.SetValues( 0, SkillName.Magery, 10.0 );
			Attributes.EnhancePotions = 30;
			Attributes.LowerRegCost = 20;
			Resistances.Poison = 10;
		}

		public AlchemistsBibelot( Serial serial ) : base( serial )
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