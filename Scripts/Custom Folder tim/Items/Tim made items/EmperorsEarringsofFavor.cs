//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/25/2017 12:13:12 AM
//=================================================

using System;
using Server;

namespace Server.Items
{
	public class EmperorsEarringsofFavor : GoldEarrings
	{
		public override int ArtifactRarity{ get{ return 100; } }

		[Constructable]
		public EmperorsEarringsofFavor()
		{
			Name = "Emperor's Earrings of Favor";
			Hue = 25;
			LootType = LootType.Blessed;
			Weight = 0;
			SkillBonuses.SetValues( 0, SkillName.Swords, 5 );
			SkillBonuses.SetValues( 1, SkillName.Tactics, 5 );
			Attributes.BonusDex = 6;
			Attributes.BonusHits = 6;
			Attributes.WeaponDamage = 10;
			Attributes.WeaponSpeed = 10;
		}

		public EmperorsEarringsofFavor( Serial serial ) : base( serial )
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
