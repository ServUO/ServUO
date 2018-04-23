//Made By Makoro Shimoro
using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1415, 0x1416 )]
	public class ChestOfTheGods : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }

		public override int AosStrReq{ get{ return 95; } }
		public override int OldStrReq{ get{ return 60; } }

		public override int OldDexBonus{ get{ return -8; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public ChestOfTheGods() : base( 0x1415 )
		{
			Name = "Chest Of The Gods";
			Hue = 1159;
			Weight = 10.0;
			Attributes.BonusDex = 10;
			Attributes.RegenHits = 2;
			Attributes.DefendChance = 5;
			FireBonus = 7;
			PhysicalBonus = 5;
			ColdBonus = 8;
			PoisonBonus = 7;
			EnergyBonus = 8;
		}

		public ChestOfTheGods( Serial serial ) : base( serial )
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
				Weight = 10.0;
		}
	}
}