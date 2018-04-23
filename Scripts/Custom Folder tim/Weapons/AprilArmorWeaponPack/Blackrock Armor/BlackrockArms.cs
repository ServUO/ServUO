//Yrenwick Dragon Ultima IX pack, MiniQuest System & BlackrockArms.cs created by Yrenwick Dragon (G. Younk)
//From the Ultima: Britannia shard  http://www.mac512.com/ultima/britannia/
//This script was created on 5/31/04

using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2657, 0x2658 )]
	public class BlackrockArms : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 9; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

		public override int InitMinHits{ get{ return 55; } }
		public override int InitMaxHits{ get{ return 75; } }

		public override int AosStrReq{ get{ return 75; } }
		public override int OldStrReq{ get{ return 20; } }

		public override int OldDexBonus{ get{ return -2; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }
		//public override CraftResource DefaultResource{ get{ return CraftResource.RedScales; } }

		[Constructable]
		public BlackrockArms() : base( 0x2657 )
		{
			Weight = 5.0;
			Attributes.ReflectPhysical = 5;
			Name = "Blackrock Arms";
       		Hue = 1; 
		}

		public BlackrockArms( Serial serial ) : base( serial )
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
