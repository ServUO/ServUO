
using System;
using Server;

namespace Server.Items
{
	public class Blackknightsarms : PlateArms 
	{
		public override int ArtifactRarity{ get{ return 20; } }

		public override int InitMinHits{ get{ return 500; } }
		public override int InitMaxHits{ get{ return 1000; } }

		[Constructable]
		public Blackknightsarms()
		{
			Weight = 6.0; 
            		Name = "Black Knights Arms"; 
            		Hue = 4455;

			
			Attributes.BonusHits = 2;
			Attributes.DefendChance = 12;
			Attributes.Luck = 10;
			Attributes.WeaponDamage = 2;
			Attributes.WeaponSpeed = 10;

			ArmorAttributes.SelfRepair = 10;

			ColdBonus = 10;
			EnergyBonus = 5;
			FireBonus = 7;
			PhysicalBonus = 12;
			PoisonBonus = 4;
			StrRequirement = 60;


		}

		public Blackknightsarms( Serial serial ) : base( serial )
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