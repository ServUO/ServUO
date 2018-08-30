using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class GlovesOfGrace : DragonGloves
	{
		public override int LabelNumber{ get{ return 1075047; } } // Gloves of Grace

		public override int BasePhysicalResistance{ get{ return 10; } }
		public override int BaseFireResistance{ get{ return 9; } }
		public override int BaseColdResistance{ get{ return 7; } }
		public override int BasePoisonResistance{ get{ return 7; } }
		public override int BaseEnergyResistance{ get{ return 15; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Dragon; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.Iron; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public GlovesOfGrace()
		{
			Name = "Gloves of Grace";
		
			SkillBonuses.SetValues( 0, SkillName.MagicResist, 5.0 );

			Attributes.DefendChance = 10;

			ArmorAttributes.SelfRepair = 2;
		}

		public override Race RequiredRace
		{
			get
			{
				return Race.Elf;
			}
		}

		public GlovesOfGrace( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}