using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x236C, 0x236D )]
	public class SamuraiDo : PlateDo
	{
		public override int LabelNumber{ get{ return 1062923; } } // Ancient Samurai Helm

		public override int BasePhysicalResistance{ get{ return 15; } }
		public override int BaseFireResistance{ get{ return 10; } }
		public override int BaseColdResistance{ get{ return 10; } }
		public override int BasePoisonResistance{ get{ return 15; } }
		public override int BaseEnergyResistance{ get{ return 10; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public SamuraiDo() : base( 0x236C )
		{
			Name = "Samurai Do";
		
			Weight = 5.0;
			LootType = LootType.Blessed;

			Attributes.DefendChance = 15;
			ArmorAttributes.SelfRepair = 10;
			ArmorAttributes.LowerStatReq = 100;
			ArmorAttributes.MageArmor = 1;
		}

		public SamuraiDo( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}