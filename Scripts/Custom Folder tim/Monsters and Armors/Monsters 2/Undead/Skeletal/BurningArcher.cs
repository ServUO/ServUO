using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a burning skeletal corpse" )]
	public class BurningArcher : BaseCreature
	{
		[Constructable]
		public BurningArcher() : base( AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a burning archer";
			Body = 148;
			BaseSoundID = 451;
			Hue = 1256;

			SetStr( 176, 180 );
			SetDex( 56, 75 );
			SetInt( 186, 210 );

			SetHits( 96, 110 );

			SetDamage( 4, 10 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 50 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 90, 100 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.Anatomy, 60.2, 100.0 );
			SetSkill( SkillName.Archery, 80.1, 90.0 );
			SetSkill( SkillName.MagicResist, 65.1, 90.0 );
			SetSkill( SkillName.Tactics, 50.1, 75.0 );
			SetSkill( SkillName.Wrestling, 50.1, 75.0 );

			Fame = 3000;
			Karma = -3000;

			VirtualArmor = 38;
			AddItem( new Bow() );
			PackItem( new Arrow( Utility.RandomMinMax( 150, 170 ) ) );			
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			
		}
		
		public override bool BleedImmune{ get{ return true; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public BurningArcher( Serial serial ) : base( serial )
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