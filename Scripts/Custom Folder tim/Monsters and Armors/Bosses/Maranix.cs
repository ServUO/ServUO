//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/29/2017 3:34:33 PM
//=================================================
using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "an Maranix corpse" )]
	public class Maranix : EvilMage
	{
		[Constructable]
		public Maranix()
		{
			Name = "Maranix";
			Hue = 2159;
			SetStr( 280, 350 );
			SetDex( 150, 235 );
			SetInt( 470, 600 );

			SetHits( 15500, 15500 );
			SetStam( 255, 255 );
			SetMana( 600, 600 );

			SetSkill( SkillName.Magery, 120, 180 );
			SetSkill( SkillName.Tactics, 120, 180 );
			SetSkill( SkillName.Wrestling, 240, 300 );
			SetSkill( SkillName.MagicResist, 100, 150 );

			SetResistance( ResistanceType.Physical, 20, 20 );
			SetResistance( ResistanceType.Fire, 55, 55 );
			SetResistance( ResistanceType.Cold, 55, 55 );
			SetResistance( ResistanceType.Poison, 55, 55 );
			SetResistance( ResistanceType.Energy, 55, 55 );

			Fame = 24000;
			Karma = 24000;

			PackItem(new Glovesofthemagi());
			PackItem(new BeltOfLostSouls());
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
		}

		public Maranix( Serial serial ) : base( serial )
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
