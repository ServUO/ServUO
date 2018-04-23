
using System;
using Server;

namespace Server.Items
{
	public class SilverCrowArms : PlateArms 
	{
		public override int ArtifactRarity{ get{ return 62; } }

		public override int InitMinHits{ get{ return 500; } }
		public override int InitMaxHits{ get{ return 500; } }

		[Constructable]
		public SilverCrowArms()
		{
			Weight = 6.0; 
            		Name = "Silver Crow Arms"; 
            		Hue = 1953;

			
			Attributes.BonusHits = 10;
			Attributes.DefendChance = 20;
			Attributes.Luck = 100;
			Attributes.WeaponDamage = 5;
			Attributes.WeaponSpeed = 5;

			//ArmorAttributes.SelfRepair = 30;

			ColdBonus = 15;
			EnergyBonus = 15;
			FireBonus = 15;
			PhysicalBonus = 15;
			PoisonBonus = 15;
			StrRequirement = 60;


		}

		public SilverCrowArms( Serial serial ) : base( serial )
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