
using System;
using Server;

namespace Server.Items
{
	public class Blackknightsgorget : PlateGorget 
	{
		public override int ArtifactRarity{ get{ return 20; } }

		public override int InitMinHits{ get{ return 300; } }
		public override int InitMaxHits{ get{ return 600; } }

		[Constructable]
		public Blackknightsgorget()
		{
			Weight = 3.0; 
            		Name = "Black Knights Gorget"; 
            		Hue = 4455;

			Attributes.BonusHits = 3;
			Attributes.DefendChance = 10;
			Attributes.Luck = 10;
			Attributes.WeaponDamage = 4;
			Attributes.WeaponSpeed = 10;

			ArmorAttributes.SelfRepair = 10;

			ColdBonus = 2;
			EnergyBonus = 8;
			FireBonus = 3;
			PhysicalBonus = 8;
			PoisonBonus = 6;
			StrRequirement = 55;

		}

		public Blackknightsgorget( Serial serial ) : base( serial )
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