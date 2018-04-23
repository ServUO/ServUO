using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a elite ninja corpse" )]
	public class ArenaNinja : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.TalonStrike;
		}

		public static int AbilityRange { get { return 10; } }

		private static int m_MinTime = 8;
		private static int m_MaxTime = 16;

		private DateTime m_NextAbilityTime;

		[Constructable]
		public ArenaNinja() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an elite ninja";
			Body = 0x190;
			Hue = Utility.RandomSkinHue();

			SetStr( 986, 1185 );
			SetDex( 177, 255 );
			SetInt( 151, 250 );

			SetHits( 1192, 1251 );

			SetDamage( 25, 35 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 65, 80 );
			SetResistance( ResistanceType.Fire, 60, 80 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Anatomy, 25.1, 50.0 );
			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 95.5, 100.0 );
			SetSkill( SkillName.Meditation, 25.1, 50.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 150.1, 160.0 );

			Fame = 24000;
			Karma = -24000;

			VirtualArmor = 90;

			Tekagi weapon = new Tekagi();
			weapon.Skill = SkillName.Wrestling;
			weapon.Movable = false;

			AddItem( new LeatherNinjaHood() );
			AddItem( new LeatherNinjaJacket() );
			AddItem( new LeatherNinjaPants() );
			AddItem( new LeatherNinjaBelt() );
			AddItem( new LeatherNinjaMitts() );
			AddItem( new NinjaTabi() );
			AddItem( weapon );

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 4 );
		}

		public override bool OnBeforeDeath()
		{
			this.Say( "Meet Your Next Challenge!!!" );
			ArenaMaster am = new ArenaMaster();			
			am.MoveToWorld( new Point3D( 2369, 1127, -90 ), Map.Malas );
			am.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
			am.PlaySound( 0x208 );
			am.Team = this.Team;
			am.Combatant = this.Combatant;

			return true;
		}

		public override bool AlwaysMurderer { get { return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool CanRummageCorpses { get { return true; } }

		public ArenaNinja( Serial serial ) : base( serial )
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