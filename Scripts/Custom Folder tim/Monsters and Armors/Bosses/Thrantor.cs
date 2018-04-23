//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/26/2017 1:41:28 PM
//=================================================
using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "an Thrantor corpse" )]
	public class Thrantor : Lizardman
	{
		[Constructable]
		public Thrantor()
		{
			Name = "Thrantor";
			Hue = 2541;
			SetStr( 450, 575 );
			SetDex( 400, 450 );
			SetInt( 50, 100 );

			SetHits( 12000, 12000 );
			SetStam( 500, 600 );
			SetMana( 170, 220 );


			SetResistance( ResistanceType.Physical, 30, 30 );
			SetResistance( ResistanceType.Fire, 25, 25 );
			SetResistance( ResistanceType.Poison, 70, 70 );
			SetResistance( ResistanceType.Energy, 70, 70 );

			Fame = 9000;
			Karma = 5000;

			PackItem(new TacticalMask());
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 1 );
		}

		//public override void OnDeath( Container c )
		//{
		//	base.OnDeath( c );	
		//	if ( Utility.RandomMinMax( 1,  ) == 1 )
		//		c.DropItem( new TacticalMask(  ) );
		//}

		public Thrantor( Serial serial ) : base( serial )
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
