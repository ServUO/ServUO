using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1410, 0x1417 )]
	public class ArmsofNast : BaseArmor
	{
                public override int ArtifactRarity{ get{return 5000; } }
		public override int BasePhysicalResistance{ get{ return 70; } }
		public override int BaseFireResistance{ get{ return 70; } }
		public override int BaseColdResistance{ get{ return 70; } }
		public override int BasePoisonResistance{ get{ return 70; } }
		public override int BaseEnergyResistance{ get{ return 70; } }

		public override int InitMinHits{ get{ return 561; } }
		public override int InitMaxHits{ get{ return 561; } }

		public override int AosStrReq{ get{ return 100; } }
		public override int OldStrReq{ get{ return 40; } }


                public override int AosDexReq{ get{ return 100; } }
		public override int OldDexBonus{ get{ return -2; } }

                public override int AosIntReq{ get{ return 100; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public ArmsofNast() : base( 0x1410 )
		{
                   LootType = LootType.Blessed;
                   ArmorAttributes.MageArmor = 1;
                   Name = "Arms of Nast";
                   Hue = 1109;
			Weight = 5.0;
		}

		public ArmsofNast( Serial serial ) : base( serial )
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
				Weight = 5.0;
		}
	}
}