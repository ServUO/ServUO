//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/26/2017 1:02:33 PM
//=================================================
using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "an Aragoth corpse" )]
	public class Aragoth : Cyclops
	{
		[Constructable]
		public Aragoth()
		{
			Name = "Aragoth";
			Hue = 0;
			SetStr( 400, 600 );
			SetDex( 300, 400 );
			SetInt( 100, 175 );

			SetHits( 9500, 9500 );
			SetStam( 210, 210 );
			SetMana( 190, 190 );

			SetSkill( SkillName.Swords, 100, 120 );
			SetSkill( SkillName.Parry, 100, 120 );
			SetSkill( SkillName.Anatomy, 100, 120 );
			SetSkill( SkillName.Tactics, 100, 120 );

			SetResistance( ResistanceType.Physical, 50, 50 );
			SetResistance( ResistanceType.Fire, 50, 50 );
			SetResistance( ResistanceType.Cold, 50, 50 );
			SetResistance( ResistanceType.Poison, 50, 50 );
			SetResistance( ResistanceType.Energy, 10, 10 );

			Fame = 7000;
			Karma = 3000;

			PackItem(new Chestofthemagi());
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 1 );
		}


		public Aragoth( Serial serial ) : base( serial )
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
