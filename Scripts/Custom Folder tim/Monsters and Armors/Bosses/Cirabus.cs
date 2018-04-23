//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/26/2017 1:31:22 PM
//=================================================
using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "an Cirabus corpse" )]
	public class Cirabus : BogThing
	{
		[Constructable]
		public Cirabus()
		{
			Name = "Cirabus";
			Hue = 2237;
			SetStr( 300, 400 );
			SetDex( 300, 350 );
			SetInt( 80, 100 );

			SetHits( 12000, 12000 );
			SetStam( 450, 450 );
			SetMana( 100, 100 );

			SetSkill( SkillName.Wrestling, 175, 225 );
			SetSkill( SkillName.MagicResist, 100, 120 );
			SetSkill( SkillName.Anatomy, 100, 120 );
			SetSkill( SkillName.Tactics, 100, 120 );

			SetResistance( ResistanceType.Physical, 25, 25 );
			SetResistance( ResistanceType.Fire, 10, 10 );
			SetResistance( ResistanceType.Cold, 55, 70 );
			SetResistance( ResistanceType.Poison, 100, 100 );
			SetResistance( ResistanceType.Energy, 35, 65 );

			Fame = 9000;
			Karma = 5000;

			PackItem(new Gorgetofthemagi());
			PackItem(new SwampBoots());
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 1 );
		}

		//public override void OnDeath( Container c )
		//{
		//	base.OnDeath( c );	
		//	if ( Utility.RandomMinMax( 1,  ) == 1 )
		//		c.DropItem( new Gorgetofthemagi(  ) );
		//	if ( Utility.RandomMinMax( 1,  ) == 1 )
		//		c.DropItem( new SwampBoots(  ) );
		//}

		public Cirabus( Serial serial ) : base( serial )
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
