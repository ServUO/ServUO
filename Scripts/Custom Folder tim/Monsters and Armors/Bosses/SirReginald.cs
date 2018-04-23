//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/29/2017 4:11:08 PM
//=================================================
using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "an SirReginald corpse" )]
	public class SirReginald : Spectre
	{
		[Constructable]
		public SirReginald()
		{
			Name = "Adniral Reginald";
			Hue = 926;
			SetStr( 450, 900 );
			SetDex( 200, 310 );
			SetInt( 100, 190 );

			SetHits( 13200, 13200 );
			SetStam( 250, 250 );
			SetMana( 250, 250 );

			SetSkill( SkillName.Magery, 150, 175 );
			SetSkill( SkillName.Wrestling, 90, 125 );
			SetSkill( SkillName.Magery, 100, 125 );

			SetResistance( ResistanceType.Physical, 15, 15 );
			SetResistance( ResistanceType.Fire, 15, 15 );
			SetResistance( ResistanceType.Cold, 100, 100 );
			SetResistance( ResistanceType.Poison, 100, 100 );
			SetResistance( ResistanceType.Energy, 15, 15 );

			Fame = 22000;
			Karma = 11000;

			PackItem(new AdmiralsHat());
			PackItem(new PendantOfTheMagi());
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.SuperBoss, 1);
		}

		public SirReginald( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

	}
}
