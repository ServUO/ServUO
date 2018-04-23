using System;
using Server;

namespace Server.Items
{
	public class CaptainJohnsPants : LongPants
	{
		public override int LabelNumber{ get{ return 1094911; } } // Captain John's Hat [Replica]

		public override int BasePhysicalResistance{ get{ return 2; } }
		public override int BaseFireResistance{ get{ return 6; } }
		public override int BaseColdResistance{ get{ return 9; } }
		public override int BasePoisonResistance{ get{ return 7; } }
		public override int BaseEnergyResistance{ get{ return 23; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public CaptainJohnsPants()
		{
			Hue = 0x455;
			Name = "Captain Johns Pants [Replica]";
			Attributes.BonusDex = 10;
			Attributes.NightSight = 1;
			Attributes.AttackChance = 10;

			SkillBonuses.Skill_1_Name = SkillName.Swords;
			SkillBonuses.Skill_1_Value = 10;
		}

		public CaptainJohnsPants( Serial serial ) : base( serial )
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
