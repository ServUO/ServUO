using System;
using Server;

namespace Server.Items
{
	public class TacticsOfDeciet : OrcHelm
	{
		public override int LabelNumber{ get{ return 1094914; } } // Tactics of Deceit [Replica]

		public override int BasePhysicalResistance{ get{ return 11; } }
		public override int BaseFireResistance{ get{ return 6; } }
		public override int BaseColdResistance{ get{ return 18; } }
		public override int BasePoisonResistance{ get{ return 15; } }
		public override int BaseEnergyResistance{ get{ return 13; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public TacticsOfDeciet()
		{
			Name = "Tactics Of Deciet [Replica]";
		
			Hue = 0x38F;

			Attributes.RegenHits = 3;

			ArmorAttributes.MageArmor = 1;

			SkillBonuses.Skill_1_Name = SkillName.Tactics;
			SkillBonuses.Skill_1_Value = 10;
		}

		public TacticsOfDeciet( Serial serial ) : base( serial )
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
