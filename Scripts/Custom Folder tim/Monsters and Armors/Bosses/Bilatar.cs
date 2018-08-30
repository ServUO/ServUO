//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/26/2017 1:08:51 PM
//=================================================
using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "an Bilatar corpse" )]
	public class Bilatar : Nightmare
	{
		[Constructable]
		public Bilatar()
		{
			Name = "Bilatar";
			Hue = 1445;
			SetStr( 450, 650 );
			SetDex( 375, 400 );
			SetInt( 190, 190 );

			SetHits( 8700, 8700 );
			SetStam( 250, 250 );
			SetMana( 255, 255 );

			SetSkill( SkillName.Wrestling, 200, 240 );
			SetSkill( SkillName.MagicResist, 100, 145 );

			SetResistance( ResistanceType.Physical, 50, 50 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 70, 75 );
			SetResistance( ResistanceType.Poison, 50, 50 );
			SetResistance( ResistanceType.Energy, 50, 50 );

			Fame = 7000;
			Karma = 3000;

			PackItem(new Legsofthemagi());

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 1 );
		}

		public Bilatar( Serial serial ) : base( serial )
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
