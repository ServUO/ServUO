using System;
using Server.Items;

namespace Server.Items
{
	public class Blackknightshelm : BaseArmor
	{
                public override int ArtifactRarity{ get{ return 20; } } 

		public override int BasePhysicalResistance{ get{ return 8; } }
		public override int BaseFireResistance{ get{ return 5; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 4; } }
		public override int BaseEnergyResistance{ get{ return 9; } }

		public override int InitMinHits{ get{ return 90; } }
		public override int InitMaxHits{ get{ return 115; } }

		public override int AosStrReq{ get{ return 70; } }
		public override int OldStrReq{ get{ return 40; } }

		public override int OldDexBonus{ get{ return -1; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public Blackknightshelm() : base( 0x1412 )
		{
			Weight = 5.0;
                        Name = "Black Knights Helm";
            		Hue = 4455;
		}

		public Blackknightshelm( Serial serial ) : base( serial )
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