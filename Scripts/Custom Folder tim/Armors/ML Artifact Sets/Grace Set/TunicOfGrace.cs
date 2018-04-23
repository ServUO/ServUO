using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class TunicOfGrace : DragonChest
	{
		public override int LabelNumber{ get{ return 1075047; } } // Tunic of Grace

		public override int BasePhysicalResistance{ get{ return 15; } }
		public override int BaseFireResistance{ get{ return 10; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 17; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Dragon; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.Iron; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public TunicOfGrace()
		{
			Name = "Tunic of Grace";
		
			SkillBonuses.SetValues( 0, SkillName.MagicResist, 10.0 );

			Attributes.DefendChance = 15;

			ArmorAttributes.SelfRepair = 3;
		}

		public override Race RequiredRace
		{
			get
			{
				return Race.Elf;
			}
		}

		public TunicOfGrace( Serial serial ) : base( serial )
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