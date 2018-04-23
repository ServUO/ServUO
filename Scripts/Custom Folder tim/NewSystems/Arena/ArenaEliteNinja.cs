using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a elite ninja corpse" )]
	public class ArenaEliteNinja : BaseCreature
	{
		public static int AbilityRange { get { return 10; } }

		private static int m_MinTime = 8;
		private static int m_MaxTime = 16;

		private DateTime m_NextAbilityTime;

		[Constructable]
		public ArenaEliteNinja() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an elite ninja";
			Body = 0x190;
			Hue = Utility.RandomSkinHue();

			SetStr( 125, 175 );
			SetDex( 175, 275 );
			SetInt( 85, 105 );

			SetHits( 250, 350 );

			SetDamage( 8, 22 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 60 );
			SetResistance( ResistanceType.Fire, 45, 65 );
			SetResistance( ResistanceType.Cold, 25, 45 );
			SetResistance( ResistanceType.Poison, 40, 60 );
			SetResistance( ResistanceType.Energy, 40, 65 );

			Fame = 5000;
			Karma = -5000;

			SetSkill( SkillName.MagicResist, 80.0, 100.0 );
			SetSkill( SkillName.Tactics, 115.0, 130.0 );
			SetSkill( SkillName.Wrestling, 95.0, 120.0 );
			SetSkill( SkillName.Anatomy, 105.0, 120.0 );
			SetSkill( SkillName.Fencing, 30.0, 50.0 );

			SetSkill( SkillName.Ninjitsu, 50.0, 120.0 );
			SetSkill( SkillName.Hiding, 100.0, 120.0 );
			SetSkill( SkillName.Stealth, 100.0, 120.0 );

			AddItem( new Kama() );
			AddItem( new LeatherNinjaHood() );
			AddItem( new LeatherNinjaJacket() );
			AddItem( new LeatherNinjaPants() );
			AddItem( new LeatherNinjaBelt() );
			AddItem( new LeatherNinjaMitts() );
			AddItem( new NinjaTabi() );

			AddItem( new Gold( 750, 900 ) );

			//PackMagicItems( 1, 3 );

			PackItem( new BookOfNinjitsu() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Gems, 2 );
		}


		public override bool AlwaysMurderer { get { return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool CanRummageCorpses { get { return true; } }

		public ArenaEliteNinja( Serial serial ) : base( serial )
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