using System;
using Server.Items;

namespace Server.Items
{
	[Flipable( 0x2645, 0x2646 )]
	public class HelmoftheShadowKnight : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 6; } }
		public override int BaseFireResistance{ get{ return 0; } }
		public override int BaseColdResistance{ get{ return 6; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 0; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override int AosStrReq{ get{ return 75; } }
		public override int OldStrReq{ get{ return 40; } }

		public override int OldDexBonus{ get{ return -1; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.Iron; } }

		[Constructable]
		public HelmoftheShadowKnight() : base( 0x2645 )
		{
			Weight = 5.0;
			Hue = 1175;
			Name = "Helm of the Shadowknight";
			SkillBonuses.SetValues( 0, SkillName.Necromancy, 5.0 );
			Attributes.SpellDamage = 5;
			Attributes.WeaponDamage = 5;
		}

		public HelmoftheShadowKnight( Serial serial ) : base( serial )
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