using System;
using Server;

namespace Server.Items
{
	public class LeggingsOfTheRoyalCouncil : PlateLegs
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public LeggingsOfTheRoyalCouncil()
		{
			Name = "Leggings Of The Royal Council";
			Hue = 1150;
			Attributes.BonusStam = 8;
			Attributes.AttackChance = 20;
			PoisonBonus = 9;
			FireBonus = 9;
			PhysicalBonus = 8;
			EnergyBonus = 8;
			ColdBonus = 9;	
		}

		public LeggingsOfTheRoyalCouncil( Serial serial ) : base( serial )
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