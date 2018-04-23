//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 12/9/2017 3:46:11 PM
//=================================================
using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "an Great White Hart corpse" )]
	public class GreatWhiteHart : GreatHart
	{
		[Constructable]
		public GreatWhiteHart()
		{
			Name = "Great White Hart";
			Hue = 2142;
			SetStr( 375, 550 );
			SetDex( 180, 220 );
			SetInt( 75, 100 );

			SetHits( 4500, 6250 );
			SetStam( 100, 150 );
			SetMana( 70, 135 );

			SetSkill( SkillName.Magery, 85, 120 );
			SetSkill( SkillName.Wrestling, 100, 120 );
			SetSkill( SkillName.MagicResist, 150, 200 );
			SetSkill( SkillName.Hiding, 150, 200 );

			SetResistance( ResistanceType.Physical, 55, 55 );
			SetResistance( ResistanceType.Fire, 55, 55 );
			SetResistance( ResistanceType.Cold, 55, 55 );
			SetResistance( ResistanceType.Poison, 55, 55 );
			SetResistance( ResistanceType.Energy, 55, 55 );

			Fame = 5000;
			Karma = 5000;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 3 );
		}

		public GreatWhiteHart( Serial serial ) : base( serial )
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
