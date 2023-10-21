using System;
using Server;
using Server.Misc;
using System.Collections;
using Server.Network;
using Server.Items;
using Xanthos.Evo;

namespace Server.Mobiles
{
	[CorpseName( "a queen lich corpse" )]
	public class QueenLich : BaseCreature
	{
		public Timer m_DeathTimer;

		[Constructable]
		public QueenLich () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a queen lich";
			Body = 79;
			BaseSoundID = 362;
			Hue = 1175;

			SetStr( 1300, 1400 );
			SetDex( 125, 195 );
			SetInt( 906, 1026 );

			SetHits( 15000, 20500 );

			SetDamage( 17, 20 );

			SetDamageType( ResistanceType.Physical, 100 );
			SetDamageType( ResistanceType.Fire, 100 );
			SetDamageType( ResistanceType.Cold, 100 );
			SetDamageType( ResistanceType.Poison, 100 );
			SetDamageType( ResistanceType.Energy, 100 );

			SetResistance( ResistanceType.Physical, 99 );
			SetResistance( ResistanceType.Fire, 99 );
			SetResistance( ResistanceType.Cold, 99 );
			SetResistance( ResistanceType.Poison, 99 );
			SetResistance( ResistanceType.Energy, 99 );

			SetSkill( SkillName.EvalInt, 125.1, 130.0 );
			SetSkill( SkillName.Magery, 125.1, 130.0 );
			SetSkill( SkillName.Meditation, 125.1, 130.0 );
			SetSkill( SkillName.MagicResist, 130.5, 130.5 );
			SetSkill( SkillName.Tactics, 180.1, 180.0 );
			SetSkill( SkillName.Wrestling, 150.1, 150.0 );

			Fame = 50000;
			Karma = -50000;

			VirtualArmor = 100;

			PackGem();
			PackGem();
//			PackPotion();
			PackGold( 11000, 15000 );
//			PackScroll( 2, 8 );
//			PackMagicItems( 3, 5, 0.95, 0.95 );
//			PackMagicItems( 4, 5, 0.80, 0.65 );
//			PackMagicItems( 4, 5, 0.80, 0.65 );
//			PackSlayer();
			//PackItem( new HiryuEvoEgg() );

			switch ( Utility.Random( 100 ))
			{
			case 0: PackItem( new MutantEvoEgg() ); break;

			}

		}

		public override bool CanBeControlledBy( Mobile m )
		{
			if ( m.Skills[SkillName.AnimalTaming].Base < 95 )
				return false;
			return base.CanBeControlledBy( m );
		}

		public override int GetIdleSound()
		{
			return 0x2D3;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override bool AutoDispel{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public QueenLich( Serial serial ) : base( serial )
		{
		}

		private DateTime m_NextBreathe;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant as Mobile;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.Now >= m_NextBreathe )
			{
				Breathe( combatant );

				m_NextBreathe = DateTime.Now + TimeSpan.FromSeconds( 12.0 + (3.0 * Utility.RandomDouble()) ); // 12-15 seconds
			}
		}

		public void Breathe( Mobile m )
		{
			DoHarmful( m );

			new BreatheTimer( m, this ).Start();

			this.Frozen = true;

			this.MovingParticles( m, 0x36D4, 1, 0, false, true, Utility.RandomList( 0 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
		}

		private class BreatheTimer : Timer
		{
			private QueenLich d;
			private Mobile m_Mobile;

			public BreatheTimer( Mobile m, QueenLich owner ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				d = owner;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				d.Frozen = false;

				m_Mobile.PlaySound( 0x11D );
				AOS.Damage( m_Mobile, Utility.RandomMinMax( 40, 50 ), 0, 100, 0, 0, 0 );
				Stop();
			}
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