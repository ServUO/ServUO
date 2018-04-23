// Created by Tom Sibilsky aka Neptune

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Cratylus" )]
              public class Cratylus : BaseCreature
              {
				private Timer m_Timer;
                                 [Constructable]
                                    public Cratylus() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
                            {
                                               Name = "Cratylus";
						Title = "Beast of Norath";
					             Body = 788;
						        Female = false; 
						           Hue = 2245;
				
                                 //Body = 149; // Uncomment these lines and input values
                                 //BaseSoundID = 0x4B0; // To use your own custom body and sound.
                                               SetStr( 3000 );
                                               SetDex( 2000 );
                                               SetInt( 4000 );
                                               SetHits( 60000 );
                                               SetDamage( 60, 85 );
                                               SetDamageType( ResistanceType.Cold, 100 );
                                               SetDamageType( ResistanceType.Fire, 100 );
                                               SetDamageType( ResistanceType.Energy, 100 );
                                               SetDamageType( ResistanceType.Poison, 100 );

                                               SetResistance( ResistanceType.Physical, 190 );
                                               SetResistance( ResistanceType.Cold, 190 );
                                               SetResistance( ResistanceType.Fire, 190 );
                                               SetResistance( ResistanceType.Energy, 190 );
                                               SetResistance( ResistanceType.Poison, 190 );

			SetSkill( SkillName.EvalInt, 320.0, 350.0 );
			SetSkill( SkillName.Magery, 290.0, 300.0 );
			SetSkill( SkillName.Meditation, 200.0, 310.0 );
			SetSkill( SkillName.Poisoning, 400.0, 610.0 );
			SetSkill( SkillName.MagicResist, 575.0, 600.0 );
			SetSkill( SkillName.Tactics, 690.0, 800.0 );
			SetSkill( SkillName.Wrestling, 675.0, 800.0 );
			SetSkill( SkillName.Swords, 375.0, 400.0 );
			SetSkill( SkillName.Anatomy, 575.0, 600.0 );
			SetSkill( SkillName.Parry, 675.0, 700.0 );


			m_Timer = new TeleportTimer( this );
			m_Timer.Start();



            Fame = 15000;
            Karma = -15000;
            VirtualArmor = 85;

			PackGold( 5000, 15000 );






                            }
                                 public override void GenerateLoot()
		{
			switch ( Utility.Random( 75 ))
			{
				case 0: PackItem( new CratylusChest() ); break;
				case 1: PackItem( new CratylusArms() ); break;
				case 2: PackItem( new CratylusLegs() ); break;
				case 3: PackItem( new CratylusGloves() ); break;
				case 4: PackItem( new CratylusGorget() ); break;
				
				
		 }		
			


				
		 
		}
		
		        public override bool HasBreath{ get{ return true ; } }
			 public override int BreathFireDamage{ get{ return 11; } }
			public override int BreathColdDamage{ get{ return 11; } }
			
//                public override bool IsScaryToPets{ get{ return true; } }
				public override bool AutoDispel{ get{ return true; } }
                public override bool BardImmune{ get{ return true; } }
                public override bool Unprovokable{ get{ return true; } }
                public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                public override bool AlwaysMurderer{ get{ return true; } }
//				public override bool IsScaredOfScaryThings{ get{ return false; } }






		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( from is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)from;

				if ( bc.Controlled || bc.BardTarget == this )
					damage = 0; // Immune to pets and provoked creatures
			}
		}
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


public Cratylus( Serial serial ) : base( serial )
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
