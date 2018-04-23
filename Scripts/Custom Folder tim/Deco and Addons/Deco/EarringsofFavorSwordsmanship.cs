//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 5/29/2016 1:56:07 PM
//=================================================

using System;
using Server;

namespace Server.Items
{
	public class EarringsofFavorSwordsmanship : GoldEarrings
	{
		public override int ArtifactRarity{ get{ return 100; } }

		[Constructable]
		public EarringsofFavorSwordsmanship()
		{
			Name = "Earrings of Favor Swordsmanship";
			Hue = 70;
			LootType = LootType.Blessed;
			Weight = 0;
			SkillBonuses.SetValues( 0, SkillName.Swords, 5 );
			SkillBonuses.SetValues( 1, SkillName.Parry, 5 );
			SkillBonuses.SetValues( 2, SkillName.Tactics, 5 );
			SkillBonuses.SetValues( 3, SkillName.Anatomy, 5 );
		}

		public EarringsofFavorSwordsmanship( Serial serial ) : base( serial )
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
