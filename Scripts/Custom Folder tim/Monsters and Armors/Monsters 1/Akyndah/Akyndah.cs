using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Fifth;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
	[CorpseName( " corpse of Akyndah " )]
              public class Akyndah : Daemon
              {
                                 [Constructable]
                                    public Akyndah() : base()
                            {
                                               Name = "Lady Akyndah";
					       Title = "The Spider Queen";
                                               Hue = 2654;
                                               Body = 258; // Uncomment these lines and input values
                                               //BaseSoundID = ; // To use your own custom body and sound.
                                               SetStr( 1230 );
                                               SetDex( 1590 );
                                               SetInt( 1470 );
                                               SetHits( 80000, 120000 );
                                               SetDamage( 20, 50 );
                                               SetDamageType( ResistanceType.Physical, 150 );
                                               SetDamageType( ResistanceType.Cold, 150 );
                                               SetDamageType( ResistanceType.Fire, 150 );
                                               SetDamageType( ResistanceType.Energy, 150 );
                                               SetDamageType( ResistanceType.Poison, 150 );

                                               SetResistance( ResistanceType.Physical, 200 );
                                               SetResistance( ResistanceType.Cold, 200 );
                                               SetResistance( ResistanceType.Fire, 200 );
                                               SetResistance( ResistanceType.Energy, 200 );
                                               SetResistance( ResistanceType.Poison, 200 );
                                               Fame = -1000;
                                               Karma = -1000;
                                               VirtualArmor = 40;

                                               switch ( Utility.Random( 40 ))
                                               {                                   
            	                                   case 0: AddItem( new AkyndahEarrings() ); break;
			                           //case 1: AddItem( new () ); break;
                                               }
                                            }
            
                                            public override void GenerateLoot()
                                            {
            	                               PackGold( 40000 );
            	                               AddLoot( LootPack.FilthyRich, 2);
            	                               AddLoot( LootPack.Gems, Utility.Random( 1, 5));
                                            }    

                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                                 public override bool AlwaysMurderer{ get{ return true; } }

		public void Polymorph( Mobile m )
		{
			if ( !m.CanBeginAction( typeof( PolymorphSpell ) ) || !m.CanBeginAction( typeof( IncognitoSpell ) ) || m.IsBodyMod )
				return;

			IMount mount = m.Mount;

			if ( mount != null )
				mount.Rider = null;

			if ( m.Mounted )
				return;

			if ( m.BeginAction( typeof( PolymorphSpell ) ) )
			{
				Item disarm = m.FindItemOnLayer( Layer.OneHanded );

				if ( disarm != null && disarm.Movable )
					m.AddToBackpack( disarm );

				disarm = m.FindItemOnLayer( Layer.TwoHanded );

				if ( disarm != null && disarm.Movable )
					m.AddToBackpack( disarm );

				m.BodyMod = 28;
				m.HueMod = 2654;

				new ExpirePolymorphTimer( m ).Start();
			}
		}

		private class ExpirePolymorphTimer : Timer
		{
			private Mobile m_Owner;

			public ExpirePolymorphTimer( Mobile owner ) : base( TimeSpan.FromMinutes( 3.0 ) )
			{
				m_Owner = owner;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				if ( !m_Owner.CanBeginAction( typeof( PolymorphSpell ) ) )
				{
					m_Owner.BodyMod = 0;
					m_Owner.HueMod = -1;
					m_Owner.EndAction( typeof( PolymorphSpell ) );
				}
			}
		}

		public void SpawnSpiderServents( Mobile target )
		{
			Map map = this.Map;

			if ( map == null )
				return;

			int rats = 0;

			foreach ( Mobile m in this.GetMobilesInRange( 10 ) )
			{
				if ( m is Ratman || m is RatmanArcher || m is RatmanMage )
					++rats;
			}

			if ( rats < 16 )
			{
				PlaySound( 0x3D );

				int newRats = Utility.RandomMinMax( 3, 6 );

				for ( int i = 0; i < newRats; ++i )
				{
					BaseCreature rat;

					switch ( Utility.Random( 3 ) )
					{
						default:
						case 0: case 1:	rat = new SpiderServant(); break;
							
					}

					rat.Team = this.Team;

					bool validLocation = false;
					Point3D loc = this.Location;

					for ( int j = 0; !validLocation && j < 10; ++j )
					{
						int x = X + Utility.Random( 3 ) - 1;
						int y = Y + Utility.Random( 3 ) - 1;
						int z = map.GetAverageZ( x, y );

						if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
							loc = new Point3D( x, y, Z );
						else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
							loc = new Point3D( x, y, z );
					}

					rat.MoveToWorld( loc, map );
					rat.Combatant = target;
				}
			}
		}

		public void DoSpecialAbility( Mobile target )
		{
			if ( 0.6 >= Utility.RandomDouble() ) // 60% chance to polymorph attacker into a ratman
				Polymorph( target );

			if ( 0.2 >= Utility.RandomDouble() ) // 20% chance to more ratmen
				SpawnSpiderServents( target );

			if ( Hits < 500 && !IsBodyMod ) // Baracoon is low on life, polymorph into a ratman
				Polymorph( this );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			DoSpecialAbility( attacker );
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			DoSpecialAbility( defender );
		}

		public Akyndah( Serial serial ) : base( serial )
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