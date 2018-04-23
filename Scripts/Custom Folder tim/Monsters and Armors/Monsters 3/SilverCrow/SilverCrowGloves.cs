
using System;
using Server;

namespace Server.Items
{
	public class SilverCrowGloves : PlateGloves 
	{
		public override int ArtifactRarity{ get{ return 62; } }

		public override int InitMinHits{ get{ return 300; } }
		public override int InitMaxHits{ get{ return 300; } }

		[Constructable]
		public SilverCrowGloves()
		{
			Weight = 2.0; 
            		Name = "Silver Crow Gloves"; 
            		Hue = 1953;

			Attributes.BonusStam = 5;
			Attributes.DefendChance = 10;
			Attributes.Luck = 100;
			Attributes.SpellDamage = 10;
			Attributes.RegenHits = 3;

			//ArmorAttributes.SelfRepair = 100;

			SkillBonuses.SetValues( 0, SkillName.Wrestling, 20.0 );

			ColdBonus = 15;
			EnergyBonus = 15;
			FireBonus = 15;
			PhysicalBonus = 15;
			PoisonBonus = 15;
			StrRequirement = 50;

		}

		public SilverCrowGloves( Serial serial ) : base( serial )
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