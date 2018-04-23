//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/26/2017 1:24:39 PM
//=================================================
using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "an Karatok corpse" )]
	public class Karatok : Lich
	{
		[Constructable]
		public Karatok()
		{
			Name = "Karatok";
			Hue = 0;
			SetStr( 175, 215 );
			SetDex( 190, 235 );
			SetInt( 385, 450 );

			SetHits( 10500, 10500 );
			SetStam( 190, 250 );
			SetMana( 600, 800 );

			SetSkill( SkillName.Anatomy, 100, 120 );
			SetSkill( SkillName.Wrestling, 100, 120 );
			SetSkill( SkillName.Magery, 140, 180 );

			SetResistance( ResistanceType.Physical, 40, 40 );
			SetResistance( ResistanceType.Fire, 20, 20 );
			SetResistance( ResistanceType.Cold, 20, 20 );
			SetResistance( ResistanceType.Poison, 75, 75 );
			SetResistance( ResistanceType.Energy, 50, 50 );

			Fame = 9000;
			Karma = 5000;

			PackItem(new HatOfTheMagi());
			PackItem(new EmperorsEarringsofFavor());

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 1 );
		}

		//public override void OnDeath( Container c )
		//{
		//	base.OnDeath( c );	
		//	if ( Utility.RandomMinMax( 1,  ) == 1 )
		//		c.DropItem( new HatOfTheMagi(  ) );
		//	if ( Utility.RandomMinMax( 1,  ) == 1 )
		//		c.DropItem( new BowiesEarrings(  ) );
		//}

		public Karatok( Serial serial ) : base( serial )
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
