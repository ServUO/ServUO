//Made By Makoro Shimoro
using System;
using Server.Items;

namespace Server.Items
{
	public class GorgetOfTheGods : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }

		public override int AosStrReq{ get{ return 45; } }
		public override int OldStrReq{ get{ return 30; } }

		public override int OldDexBonus{ get{ return -1; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public GorgetOfTheGods() : base( 0x1413 )
		{
			Name = "Gorget Of The Gods";
			Hue = 1159;
			Weight = 2.0;
			Attributes.BonusDex = 5;
			Attributes.BonusHits = 10;
			Attributes.AttackChance = 10;
			Attributes.LowerManaCost = 8;
			Attributes.RegenHits = 2;
			PhysicalBonus = 5;
			FireBonus = 7;
			ColdBonus = 8;
			PoisonBonus = 7;
			EnergyBonus = 8;
		}

		public GorgetOfTheGods( Serial serial ) : base( serial )
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