using System;
using Server;

namespace Server.Items
{
	public class BurglarsGloves : StuddedGloves
	{
		public override int LabelNumber{ get{ return 1063473; } }

		public override int BasePhysicalResistance{ get{ return 10; } }
		public override int BaseFireResistance{ get{ return 5; } }
		public override int BaseColdResistance{ get{ return 7; } }
		public override int BasePoisonResistance{ get{ return 10; } }
		public override int BaseEnergyResistance{ get{ return 10; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BurglarsGloves()
		{
			Name = "Burglars Gloves";
		
			Hue = Utility.RandomBool() ? 0x58C : 0x10;

			SkillBonuses.SetValues( 0, SkillName.Stealing, 5.0 );
			SkillBonuses.SetValues( 1, SkillName.Stealth, 5.0 );
			SkillBonuses.SetValues( 2, SkillName.Snooping, 5.0 );

			Attributes.BonusDex = 5;
		}

		public BurglarsGloves( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}