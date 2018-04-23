//Yrenwick Dragon Ultima IX pack, MiniQuest System & WyrmguardChest.cs created by Yrenwick Dragon (G. Younk)
//From the Ultima: Britannia shard  http://www.mac512.com/ultima/britannia/
//This script was created on 5/31/04


using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2641, 0x2642 )]
	public class WyrmguardChest : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 4; } }
		public override int BaseFireResistance{ get{ return 2; } }
		public override int BaseColdResistance{ get{ return 4; } }
		public override int BasePoisonResistance{ get{ return 2; } }
		public override int BaseEnergyResistance{ get{ return 3; } }

		public override int InitMinHits{ get{ return 55; } }
		public override int InitMaxHits{ get{ return 75; } }

		public override int AosStrReq{ get{ return 75; } }
		public override int OldStrReq{ get{ return 60; } }

		public override int OldDexBonus{ get{ return -8; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }
		//public override CraftResource DefaultResource{ get{ return CraftResource.RedScales; } }

		[Constructable]
		public WyrmguardChest() : base( 0x2641 )
		{
			Weight = 10.0;
			Name = "WyrmGuard Chest";
       		Hue = 14; 
		}

		public WyrmguardChest( Serial serial ) : base( serial )
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
				Weight = 15.0;
		}
	}
}
