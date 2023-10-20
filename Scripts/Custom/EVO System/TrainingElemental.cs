#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using Server;
using Server.Mobiles;

namespace Xanthos.Evo
{
	[CorpseName( "a training elemental corpse" )]
	public class TrainingElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 115.0; } }
		public override double DispelFocus{ get{ return 50.0; } }
		public override bool AutoDispel{ get{ return false; } }
		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		private const decimal kReflectDamagePercent = 10M;
		private const int kHits = 88000;
		private const bool kInvulnerable = false;
		private const bool kCanWalk = true;

		[Constructable]
		public TrainingElemental() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Training Elemental";
			Body = 14;
			BaseSoundID = 268;
			Hue = 0x21;

			SetStr( 50, 50 );
			SetDex( 35, 35 );
			SetInt( 30, 30 );
			SetHits( kHits, kHits );
			SetDamage( 1, 1 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 20 );
			SetDamageType( ResistanceType.Cold, 20 );
			SetDamageType( ResistanceType.Poison, 20 );
			SetDamageType( ResistanceType.Energy, 20 );

			SetResistance( ResistanceType.Physical, 50 );
			SetResistance( ResistanceType.Fire, 50 );
			SetResistance( ResistanceType.Cold, 50 );
			SetResistance( ResistanceType.Poison, 50 );
			SetResistance( ResistanceType.Energy, 50 );

			SetSkill( SkillName.MagicResist, 100.0, 115.00 );
			SetSkill( SkillName.Tactics, 100.0, 120.00 );
			SetSkill( SkillName.Wrestling, 100.0, 120.00 );
			SetSkill( SkillName.Anatomy, 100.0, 120.00 );

			CantWalk = !kCanWalk;

			VirtualArmor = 50;
		}

		public override void GenerateLoot()
		{
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			base.OnDamage( amount, from, willKill );

			if ( kReflectDamagePercent > 0 && null != from && !(from.Deleted))
				from.Damage( (int)(Math.Round( amount / kReflectDamagePercent )), this );
		}

		public override void OnThink()
		{
			base.OnThink();

#pragma warning disable
			if ( kInvulnerable && Hits < HitsMax )
				Hits = HitsMax;
#pragma warning enable
		}

		public TrainingElemental( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}