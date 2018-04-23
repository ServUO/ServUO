using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a clucker corpse" )]
	public class Clucker : BaseCreature
	{
		[Constructable]
		public Clucker() : base( AIType.AI_Archer, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = "Colonel Sanders Escapee";
			Body = 0xD0;
			BaseSoundID = 0x6E;

			SetStr( 325, 350 );
			SetDex( 450, 500 );
			SetInt( 0, 10 );

			SetHits( 550, 600 );

			SetDamage( 25, 35 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 80 );
			SetResistance( ResistanceType.Fire, 80 );
			SetResistance( ResistanceType.Cold, 80 );
			SetResistance( ResistanceType.Poison, 80 );
			SetResistance( ResistanceType.Energy, 80 );

			SetSkill( SkillName.Anatomy, 120.0 );
			SetSkill( SkillName.Archery, 120.0 );
			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0 );

			Fame = 6500;
			Karma = 0;

			VirtualArmor = 50;

			Bow Bow = new Bow();
			AddItem( new Bow() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Gems );
		}

                public override bool BardImmune{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
       		public override int Meat { get { return 10; } }
        	public override MeatType MeatType { get { return MeatType.Bird; } }
		public override int Feathers { get { return 25; } }

		public Clucker( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 0x6E )
				BaseSoundID = 0x6E;
		}

	}
}