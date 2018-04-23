
using System;
using Server;

namespace Server.Items
{
	public class Blackknightsgloves : PlateGloves 
	{
		public override int ArtifactRarity{ get{ return 20; } }

		public override int InitMinHits{ get{ return 300; } }
		public override int InitMaxHits{ get{ return 600; } }

		[Constructable]
		public Blackknightsgloves()
		{
			Weight = 2.0; 
            		Name = "Black Knights Gloves"; 
            		Hue = 4455;

			Attributes.BonusHits = 2;
			Attributes.DefendChance = 5;
			Attributes.Luck = 10;
			Attributes.WeaponDamage = 2;
			Attributes.WeaponSpeed = 10;

			ArmorAttributes.SelfRepair = 10;

			ColdBonus = 3;
			EnergyBonus = 2;
			FireBonus = 4;
			PhysicalBonus = 5;
			PoisonBonus = 1;
			StrRequirement = 50;

		}

		public Blackknightsgloves( Serial serial ) : base( serial )
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