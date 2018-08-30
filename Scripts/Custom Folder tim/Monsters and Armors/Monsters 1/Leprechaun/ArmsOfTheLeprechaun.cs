
using System;
using Server;

namespace Server.Items
{
	public class ArmsOfTheLeprechaun : PlateArms
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public ArmsOfTheLeprechaun()
		{
			Hue = 69;
                        Name = " Arms Of The Lepricon";
			SkillBonuses.SetValues( 0, SkillName.Magery, 25.0 );
			Attributes.SpellDamage = 10;
                        Attributes.Luck = 1000;
			ArmorAttributes.MageArmor = 1;
                        Attributes.LowerRegCost = 50;
         		Attributes.BonusHits = 5;
         		Attributes.BonusMana = 5;
         		Attributes.CastSpeed = 1;
        		Attributes.SpellDamage = 25;
			Attributes.RegenMana = 2;
         		Attributes.CastRecovery = 1;
                        FireBonus = 5;
			ColdBonus = 5;
                        PoisonBonus = 5;
                        PhysicalBonus = 5;
                        EnergyBonus = 5;
		}

		public ArmsOfTheLeprechaun( Serial serial ) : base( serial )
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