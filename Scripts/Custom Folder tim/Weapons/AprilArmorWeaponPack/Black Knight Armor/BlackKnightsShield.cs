
using System;
using Server;

namespace Server.Items
{
	public class Blackknightsshield : MetalKiteShield  
	{
		public override int ArtifactRarity{ get{ return 20; } }

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 100; } }

		[Constructable]
		public Blackknightsshield()
		{
			Weight = 4.0; 
            		Name = "Black Knights Shield"; 
            		Hue = 4455;


			Attributes.BonusInt = 5;
			Attributes.DefendChance = 20;
			Attributes.Luck = 10;
			Attributes.ReflectPhysical = 5;
			Attributes.WeaponDamage = 5;
			Attributes.WeaponSpeed = 10;

			ArmorAttributes.DurabilityBonus = 1000;

			ArmorAttributes.SelfRepair = 5;

			ColdBonus = 8;
			EnergyBonus = 5;
			FireBonus = 2;
			PhysicalBonus = 20;
			StrRequirement = 60;

		}

		public Blackknightsshield( Serial serial ) : base( serial )
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