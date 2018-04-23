//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/29/2017 4:14:33 PM
//=================================================
using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "an Homiculus corpse" )]
	public class Homiculus : RottingCorpse
	{
		[Constructable]
		public Homiculus()
		{
			Name = "Homiculus";
			Hue = 1121;
			SetStr( 900, 1100 );
			SetDex( 375, 410 );
			SetInt( 85, 85 );

			SetHits( 14000, 14000 );
			SetStam( 345, 345 );
			SetMana( 100, 100 );

			SetSkill( SkillName.Wrestling, 220, 275 );
			SetSkill( SkillName.Anatomy, 220, 275 );
			SetSkill( SkillName.Tactics, 220, 275 );

			SetResistance( ResistanceType.Physical, 15, 15 );
			SetResistance( ResistanceType.Fire, 15, 15 );
			SetResistance( ResistanceType.Cold, 45, 45 );
			SetResistance( ResistanceType.Poison, 25, 25 );
			SetResistance( ResistanceType.Energy, 60, 60 );

			Fame = 22000;
			Karma = 22000;

			PackItem(new StaffOfTheMagi());
			PackItem(new EmperorsDragon());
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 1 );
		}

		public Homiculus( Serial serial ) : base( serial )
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
