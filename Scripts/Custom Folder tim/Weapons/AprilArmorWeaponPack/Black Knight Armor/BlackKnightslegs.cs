
using System;
using Server;

namespace Server.Items
{
	public class Blackknightsleggings : PlateLegs 
	{
		public override int ArtifactRarity{ get{ return 20; } }

		public override int InitMinHits{ get{ return 500; } }
		public override int InitMaxHits{ get{ return 1000; } }

		[Constructable]
		public Blackknightsleggings()
		{
			Weight = 7.0; 
            		Name = "Black Knights Leggings"; 
            		Hue = 4455;

			Attributes.BonusHits = 5;
			Attributes.DefendChance = 10;
			Attributes.Luck = 10;
			Attributes.WeaponDamage = 3;
			Attributes.WeaponSpeed = 10;
			ArmorAttributes.SelfRepair = 10;

			ColdBonus = 2;
			EnergyBonus = 4;
			FireBonus = 3;
			PhysicalBonus = 15;
			PoisonBonus = 8;
			StrRequirement = 60;


		}

		public Blackknightsleggings( Serial serial ) : base( serial )
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