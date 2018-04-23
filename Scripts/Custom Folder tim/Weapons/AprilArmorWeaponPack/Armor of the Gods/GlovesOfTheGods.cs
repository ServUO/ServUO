//Made By Makoro Shimoro
using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1414, 0x1418 )]
	public class GlovesOfTheGods : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }

		public override int AosStrReq{ get{ return 70; } }
		public override int OldStrReq{ get{ return 30; } }

		public override int OldDexBonus{ get{ return -2; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public GlovesOfTheGods() : base( 0x1414 )
		{
			Name = "Gloves Of The Gods";
			Hue = 1159;
			Weight = 2.0;
			Attributes.BonusHits = 5;
			Attributes.AttackChance = 15;
			PhysicalBonus = 5;
			FireBonus = 7;
			ColdBonus = 8;
			PoisonBonus = 7;
			EnergyBonus = 8;
		}

		public GlovesOfTheGods( Serial serial ) : base( serial )
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
			
			if ( Weight == 1.0 )
				Weight = 2.0;
		}
	}
}