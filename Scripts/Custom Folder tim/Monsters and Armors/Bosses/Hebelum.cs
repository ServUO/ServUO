//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/26/2017 1:13:20 PM
//=================================================
using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "an Hebelum corpse" )]
	public class Hebelum : PatchworkSkeleton
	{
		[Constructable]
		public Hebelum()
		{
			Name = "Hebelum";
			Hue = 0;
			SetStr( 450, 500 );
			SetDex( 210, 275 );
			SetInt( 70, 100 );

			SetHits( 9000, 9000 );
			SetStam( 300, 300 );
			SetMana( 150, 150 );

			SetSkill( SkillName.Wrestling, 150, 210 );
			SetSkill( SkillName.Tactics, 100, 210 );
			SetSkill( SkillName.Anatomy, 100, 210 );

			SetResistance( ResistanceType.Physical, 45, 45 );
			SetResistance( ResistanceType.Fire, 10, 10 );
			SetResistance( ResistanceType.Cold, 70, 70 );
			SetResistance( ResistanceType.Poison, 55, 55 );
			SetResistance( ResistanceType.Energy, 10, 10 );

			Fame = 7000;
			Karma = 3000;

			PackItem(new Armsofthemagi());

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 1 );
		}

	
		public Hebelum( Serial serial ) : base( serial )
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
