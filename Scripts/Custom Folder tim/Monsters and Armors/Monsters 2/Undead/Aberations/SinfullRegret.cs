using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a sinfull regret corpse" )]
	public class SinfullRegret : BaseCreature
	{
		[Constructable]
		public SinfullRegret() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a sinful regret";
			Body = 199;
			BaseSoundID = 0x346;
			Hue = 1282;

			SetStr( 151, 225 );
			SetDex( 81, 135 );
			SetInt( 176, 180 );

			SetHits( 221, 320 );

			SetDamage( 17, 19 );

			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Fire, 30 );
			SetDamageType( ResistanceType.Cold, 30 );
			SetDamageType( ResistanceType.Poison, 10 );
			SetDamageType( ResistanceType.Energy, 60 );

			SetResistance( ResistanceType.Physical, 55, 75 );
			SetResistance( ResistanceType.Fire, 70, 85 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 80, 100 );

			SetSkill( SkillName.Wrestling, 85.1, 95.0 );
			SetSkill( SkillName.Tactics, 105.1, 125.0 );
			SetSkill( SkillName.MagicResist, 110.1, 125.0 );
			SetSkill( SkillName.Anatomy, 105.1, 125.0 );
		
			Fame = 10000;
			Karma = -10000;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );
			AddLoot( LootPack.Gems, 2 );
		}
		public override bool BleedImmune{ get{ return true; } }

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( 0.1 > Utility.RandomDouble() && !IsStunned( defender ) )
			{
				/* Lightning Fist
				 * Cliloc: 1070839
				 * Effect: Type: "3" From: "0x57D4F5B" To: "0x0" ItemId: "0x37B9" ItemIdName: "glow" FromLocation: "(884 715, 10)" ToLocation: "(884 715, 10)" Speed: "10" Duration: "5" FixedDirection: "True" Explode: "False"
				 * Damage: 35-65, 100% energy, resistable
				 * Freezes for 4 seconds
				 * Effect cannot stack
				 */

				defender.FixedEffect( 0x37B9, 10, 5 );
				defender.SendLocalizedMessage( 1070839 ); // The creature attacks with stunning force!
 
				// This should be done in place of the normal attack damage.
				//AOS.Damage( defender, this, Utility.RandomMinMax( 35, 65 ), 0, 0, 0, 0, 100 );

				defender.Frozen = true; 

				ExpireTimer timer = new ExpireTimer( defender, TimeSpan.FromSeconds( 4.0 ) );
				timer.Start();
				m_Table[defender] = timer;
			}
		}

		private static Hashtable m_Table = new Hashtable();

		public bool IsStunned( Mobile m )
		{
			return m_Table.Contains( m );
		}

		private class ExpireTimer : Timer
 		{
			private Mobile m_Mobile;

			public ExpireTimer( Mobile m, TimeSpan delay ) : base( delay )
			{
				m_Mobile = m;
				Priority = TimerPriority.TwoFiftyMS;
			}

			public void DoExpire()
			{
				m_Mobile.Frozen = false;
				Stop();
				m_Table.Remove( m_Mobile );
			}

			protected override void OnTick()
			{
				m_Mobile.SendLocalizedMessage( 1005603 ); // You can move again!
				DoExpire();
			}
		}

		public SinfullRegret( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}