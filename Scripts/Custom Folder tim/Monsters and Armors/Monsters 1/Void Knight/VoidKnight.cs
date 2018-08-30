// Created by Atown

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of a Void Knight" )]
              public class VoidKnight : ShadowKnight
              {
				private Timer m_Timer;
                                 [Constructable]
                                    public VoidKnight() : base()
                            {
                                               Name = "Void Knight";
                                               Hue = 1931;
                                               //Body = 311; // Uncomment these lines and input values
                                               //BaseSoundID = 0x2CE; // To use your own custom body and sound.
                                               SetStr( 5500, 5795 );
                                               SetDex( 2500, 2555 );
                                               SetInt( 9000, 9999 );
                                               SetHits( 150000 );
                                               SetDamage( 250, 300 );
                                               SetDamageType( ResistanceType.Physical, 100 );
                                               SetDamageType( ResistanceType.Cold, 100 );
                                               SetDamageType( ResistanceType.Fire, 100 );
                                               SetDamageType( ResistanceType.Energy, 100 );
                                               SetDamageType( ResistanceType.Poison, 100 );

                                               SetResistance( ResistanceType.Physical, 120 );
                                               SetResistance( ResistanceType.Cold, 90 );
                                               SetResistance( ResistanceType.Fire, 90 );
                                               SetResistance( ResistanceType.Energy, 90 );
                                               SetResistance( ResistanceType.Poison, 90 );
                                              			
			SetSkill( SkillName.Magery, 290.0, 300.0 );
			SetSkill( SkillName.Poisoning, 400.0, 610.0 );
			SetSkill( SkillName.MagicResist, 575.0, 600.0 );
			SetSkill( SkillName.Tactics, 350.0, 400.0 );
			SetSkill( SkillName.Wrestling, 675.0, 800.0 );
			SetSkill( SkillName.Anatomy, 325.0, 350.0 );
			SetSkill( SkillName.Parry, 300.0, 350.0 );



			m_Timer = new TeleportTimer( this );
			m_Timer.Start();




					       Fame = 15000;
                                               Karma = 15000;
                                               VirtualArmor = 90;
     						 switch ( Utility.Random( 150 ))
			         {
				case 0: PackItem( new VoidBlade() ); break;
				case 1: PackItem( new AbysalBow() ); break;
				case 2: PackItem( new CerementOfTheVoidKnight() ); break;
				case 3: PackItem( new VoidKnightArms() ); break;
				case 4: PackItem( new VoidKnightLegs() ); break;
				case 5: PackItem( new VoidKnightChest() ); break;
				case 6: PackItem( new VoidKnightGloves() ); break;
				case 7: PackItem( new VoidKnightsWarHelm() ); break;
                            }}
                            		        public override bool HasBreath{ get{ return true ; } }
			 public override int BreathFireDamage{ get{ return 11; } }
			public override int BreathColdDamage{ get{ return 11; } }
			
//                public override bool IsScaryToPets{ get{ return true; } }
				public override bool AutoDispel{ get{ return true; } }
                public override bool BardImmune{ get{ return true; } }
                public override bool Unprovokable{ get{ return true; } }
                public override Poison HitPoison{ get{ return Poison. Lethal ; } }
private class TeleportTimer : Timer
		{
			private Mobile m_Owner;

			private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				0, -1,
				0,  1,
				1, -1,
				1,  0,
				1,  1
			};

			public TeleportTimer( Mobile owner ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.1 ) )
			{
				m_Owner = owner;
			}

			protected override void OnTick()
			{
				if ( m_Owner.Deleted )
				{
					Stop();
					return;
				}

				Map map = m_Owner.Map;

				if ( map == null )
					return;

				if ( 0.5 < Utility.RandomDouble() )
					return;

				Mobile toTeleport = null;

				foreach ( Mobile m in m_Owner.GetMobilesInRange( 16 ) )
				{
					if ( m != m_Owner && m.Player && m_Owner.CanBeHarmful( m ) && m_Owner.CanSee( m ) )
					{
						toTeleport = m;
						break;
					}
				}

				if ( toTeleport != null )
				{
					int offset = Utility.Random( 8 ) * 2;

					Point3D to = m_Owner.Location;

					for ( int i = 0; i < m_Offsets.Length; i += 2 )
					{
						int x = m_Owner.X + m_Offsets[(offset + i) % m_Offsets.Length];
						int y = m_Owner.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

						if ( map.CanSpawnMobile( x, y, m_Owner.Z ) )
						{
							to = new Point3D( x, y, m_Owner.Z );
							break;
						}
						else
						{
							int z = map.GetAverageZ( x, y );

							if ( map.CanSpawnMobile( x, y, z ) )
							{
								to = new Point3D( x, y, z );
								break;
							}
						}
					}

					Mobile m = toTeleport;

					Point3D from = m.Location;

					m.Location = to;

					Server.Spells.SpellHelper.Turn( m_Owner, toTeleport );
					Server.Spells.SpellHelper.Turn( toTeleport, m_Owner );

					m.ProcessDelta();

					Effects.SendLocationParticles( EffectItem.Create( from, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
					Effects.SendLocationParticles( EffectItem.Create(   to, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );

					m.PlaySound( 0x1FE );

					m_Owner.Combatant = toTeleport;
				}
			}
		}
public VoidKnight( Serial serial ) : base( serial )
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
