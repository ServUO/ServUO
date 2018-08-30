using System;
using Server;

namespace Server.Items
{
	public class FergelGrimoire : NecromancerSpellbook
	{
		public override int LabelNumber{ get{ return 1061109; } } // Breath of the Dead

		[Constructable]
		public FergelGrimoire()
		{
			Name = "Fergel's Grimoire";
			Hue = 1175;
			Slayer = SlayerName.Silver;
			LootType = LootType.Blessed;
			Attributes.AttackChance = 15;
			Attributes.WeaponDamage = 30;
			Attributes.Luck = 100;
			Attributes.BonusInt = 5;
			Attributes.BonusMana = 5;
			Attributes.DefendChance = 50;

		}

		public FergelGrimoire( Serial serial ) : base( serial )
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
		}
	}
}