//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/26/2017 11:59:42 AM
//=================================================
using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "an Silazar corpse" )]
	public class Silazar : AncientWyrm
	{
		[Constructable]
		public Silazar()
		{
			Name = "Silazar";
			Hue = 100;
			SetStr( 2200, 3400 );
			SetDex( 1200, 2400 );
			SetInt( 800, 950 );

			SetHits( 14000, 25000 );
			SetStam( 750, 900 );
			SetMana( 600, 1200 );

			SetSkill( SkillName.Wrestling, 100, 240 );
			SetSkill( SkillName.MagicResist, 100, 240 );
			SetSkill( SkillName.Anatomy, 100, 240 );
			SetSkill( SkillName.Magery, 100, 240 );
			SetSkill( SkillName.Healing, 100, 240 );
			SetSkill( SkillName.Tactics, 100, 240 );
			SetSkill( SkillName.Meditation, 100, 240 );
			SetSkill( SkillName.Focus, 100, 240 );

			SetResistance( ResistanceType.Physical, 70, 70 );
			SetResistance( ResistanceType.Fire, 70, 70 );
			SetResistance( ResistanceType.Cold, 65, 65 );
			SetResistance( ResistanceType.Poison, 70, 70 );
			SetResistance( ResistanceType.Energy, 60, 60 );

			Fame = 10000;
			Karma = 5000;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 1 );
		}

		public Silazar( Serial serial ) : base( serial )
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
