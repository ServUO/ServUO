// Created by Tom Sibilsky aka Neptune

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Minion" )]
              public class TMMinion : BaseCreature
              {
		 public override WeaponAbility GetWeaponAbility()
		  {
		return Utility.RandomBool() ? WeaponAbility.CrushingBlow : WeaponAbility.ConcussionBlow;
		  
                      }
			private Timer m_Timer;
                                 [Constructable]
  public TMMinion() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
                            {
                                Name = "Minion";
				
				Body = 400;
				Female = false; 
				Hue = 2399;
				
                                
                            SetStr( 9000);
                            SetDex( 6000);
                            SetInt( 3000);
                            SetHits( 750000 );
                            SetDamage( 150, 200 );
                            SetDamageType( ResistanceType.Physical, 100 );
                            SetDamageType( ResistanceType.Cold, 100 );
                            SetDamageType( ResistanceType.Fire, 100 );
                            SetDamageType( ResistanceType.Energy, 100 );
                            SetDamageType( ResistanceType.Poison, 100 );

                            SetResistance( ResistanceType.Physical, 200 );
                            SetResistance( ResistanceType.Cold, 200 );
                            SetResistance( ResistanceType.Fire, 200 );
                            SetResistance( ResistanceType.Energy, 200 );
                            SetResistance( ResistanceType.Poison, 200 );

                        SetSkill( SkillName.EvalInt, 900.0 );
			SetSkill( SkillName.Magery, 900.0 );
			SetSkill( SkillName.Meditation, 600.0 );
			SetSkill( SkillName.Poisoning, 750.0 );
			SetSkill( SkillName.MagicResist, 600.0 );
			SetSkill( SkillName.Tactics, 1000.0 );
			SetSkill( SkillName.Wrestling, 1000.0 );
			SetSkill( SkillName.Swords, 500.0 );
			SetSkill( SkillName.Anatomy, 600.0 );
			SetSkill( SkillName.Parry, 750.0 );


			m_Timer = new TeleportTimer( this );
			m_Timer.Start();



            Fame = 15000;
            Karma = -15000;
            VirtualArmor = 85;

			PackGold( 20000, 30000 );

	    


            Item NeptuneShirt = new NeptuneShirt();
            NeptuneShirt.Movable = false;
            NeptuneShirt.Hue = 2422;
            AddItem(NeptuneShirt);






                            }
                                 public override void GenerateLoot()
		{
			switch ( Utility.Random( 150 ))
			{
		case 0: PackItem( new TwistedShield() ); break;
		case 1: PackItem( new TwistedChest() ); break;
		case 2: PackItem( new TwistedLegs() ); break;
		case 3: PackItem( new TwistedArms() ); break;
		case 4: PackItem( new TwistedGloves() ); break;
		case 5: PackItem( new TwistedHelm() ); break;
		case 6: PackItem( new TwistedScythe() ); break;
				
				
		 }		
			


				
		 
		}
		
	public override bool HasBreath{ get{ return true ; } }
	public override int BreathFireDamage{ get{ return 11; } }
	public override int BreathColdDamage{ get{ return 11; } }
			
//      public override bool IsScaryToPets{ get{ return true; } }
	public override bool AutoDispel{ get{ return true; } }
        public override bool BardImmune{ get{ return true; } }
        public override bool Unprovokable{ get{ return true; } }
        public override Poison HitPoison{ get{ return Poison. Lethal ; } }
        public override bool AlwaysMurderer{ get{ return true; } }
//	public override bool IsScaredOfScaryThings{ get{ return false; } }






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


	public TMMinion( Serial serial ) : base( serial )
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
