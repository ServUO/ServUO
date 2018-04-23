
using System;
using Server;

namespace Server.Items
{
	public class SilverCrowChest : PlateChest 
	{
		public override int ArtifactRarity{ get{ return 62; } }

		public override int InitMinHits{ get{ return 500; } }
		public override int InitMaxHits{ get{ return 500; } }

		[Constructable]
		public SilverCrowChest()
		{
			Weight = 7.0; 
            		Name = "Silver Crow Chest"; 
            		Hue = 1953;

			Attributes.AttackChance = 5;
			Attributes.BonusStr = 10;
			Attributes.DefendChance = 25;
			Attributes.Luck = 100;
			Attributes.SpellDamage = 15;
			//Attributes.NightSight = 1;

			//ArmorAttributes.SelfRepair = 10;

			ColdBonus = 15;
			EnergyBonus = 15;
			FireBonus = 15;;
			PhysicalBonus = 15;
			PoisonBonus = 15;
			StrRequirement = 65;


		}

		public SilverCrowChest( Serial serial ) : base( serial )
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