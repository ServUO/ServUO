using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x1451, 0x1456 )]
	public class HeadofNast : BaseArmor
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
                
                public override int AosIntReq{ get{ return 100; } }
		public override int ArmorBase{ get{ return 30; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public HeadofNast() : base( 0x1451 )
		{ 
                    LootType = LootType.Blessed;
                   ArmorAttributes.MageArmor = 1;
                   Name = "Head of Nast";
                   Hue = 1608;
			Weight = 3.0;
		}

		public HeadofNast( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

			if ( Weight == 1.0 )
				Weight = 3.0;
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}