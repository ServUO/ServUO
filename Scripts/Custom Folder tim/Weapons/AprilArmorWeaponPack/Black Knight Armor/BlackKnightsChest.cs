
using System;
using Server;

namespace Server.Items
{
	public class Blackknightschest : PlateChest 
	{
		public override int ArtifactRarity{ get{ return 20; } }

		public override int InitMinHits{ get{ return 500; } }
		public override int InitMaxHits{ get{ return 1000; } }

		[Constructable]
		public Blackknightschest()
		{
			Weight = 7.0; 
            		Name = "Black Knights Chest"; 
            		Hue = 4455;

			Attributes.AttackChance = 5;
			Attributes.BonusStr = 2;
			Attributes.DefendChance = 15;
			Attributes.Luck = 10;
			Attributes.WeaponDamage = 5;
			Attributes.WeaponSpeed = 10;

			ArmorAttributes.SelfRepair = 10;

			ColdBonus = 5;
			EnergyBonus = 2;
			FireBonus = 10;;
			PhysicalBonus = 12;
			PoisonBonus = 6;
			StrRequirement = 65;


		}

		public Blackknightschest( Serial serial ) : base( serial )
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