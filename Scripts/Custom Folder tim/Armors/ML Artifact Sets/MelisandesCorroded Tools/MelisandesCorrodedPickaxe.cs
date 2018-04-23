using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class MelisandesCorrodedPickaxe : Pickaxe
	{
		public override int LabelNumber{ get{ return 1072115; } } // Melisande's Corroded Pickaxe

		[Constructable]
		public MelisandesCorrodedPickaxe()
		{
			Name = "Melisandes Corroded Pickaxe";
			Hue = 0x494;

			SkillBonuses.SetValues( 0, SkillName.Mining, 5.0 );

			Attributes.SpellChanneling = 1;
			Attributes.WeaponSpeed = 15;
			Attributes.WeaponDamage = -50;

			WeaponAttributes.SelfRepair = 4;
		}

		public MelisandesCorrodedPickaxe( Serial serial ) : base( serial )
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