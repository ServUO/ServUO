
using System;
using Server;

namespace Server.Items
{
	public class SilverCrowLegs : PlateLegs 
	{
		public override int ArtifactRarity{ get{ return 62; } }

		public override int InitMinHits{ get{ return 500; } }
		public override int InitMaxHits{ get{ return 500; } }

		[Constructable]
		public SilverCrowLegs()
		{
			Weight = 7.0; 
            		Name = "Silver Crow Legs"; 
            		Hue = 1953;

			Attributes.BonusDex = 10;
			Attributes.DefendChance = 10;
			Attributes.Luck = 10;
			Attributes.SpellDamage = 20;
			Attributes.RegenStam = 2;
			//ArmorAttributes.SelfRepair = 10;

			ColdBonus = 15;
			EnergyBonus = 15;
			FireBonus = 15;
			PhysicalBonus = 15;
			PoisonBonus = 15;
			StrRequirement = 60;


		}

		public SilverCrowLegs( Serial serial ) : base( serial )
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