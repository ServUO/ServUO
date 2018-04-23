// Created by Tom Sibilsky aka Neptune

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Malacoda" )]
              public class Malacoda : BaseCreature
              {
		 public override WeaponAbility GetWeaponAbility()
		  {
		return Utility.RandomBool() ? WeaponAbility.CrushingBlow : WeaponAbility.ConcussionBlow;
		  
                      }
			private Timer m_Timer;
                                 [Constructable]
  public Malacoda() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
                            {
                                Name = "Malacoda";
				Title = "Leader of the Malabranche";
				Body = 400;
				Female = false; 
				Hue = 33775;
				
                                
                            SetStr( 8400 );
                            SetDex( 8400 );
                            SetInt( 8400 );
                            SetHits( 850000 );
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

                        SetSkill( SkillName.EvalInt, 320.0 );
			SetSkill( SkillName.Magery, 590.0 );
			SetSkill( SkillName.Meditation, 640.0 );
			SetSkill( SkillName.Poisoning, 480.0 );
			SetSkill( SkillName.MagicResist, 590.0 );
			SetSkill( SkillName.Tactics, 790.0 );
			SetSkill( SkillName.Wrestling, 630.0 );
			SetSkill( SkillName.Swords, 400.0 );
			SetSkill( SkillName.Anatomy, 700.0 );
			SetSkill( SkillName.Parry, 530.0 );


			m_Timer = new TeleportTimer( this );
			m_Timer.Start();



            Fame = 15000;
            Karma = -15000;
            VirtualArmor = 85;

			PackGold( 20000, 30000 );

	    		MalabrancheChest Chest = new MalabrancheChest();
			Chest.Movable = false;
			AddItem(Chest);
			
			MalabrancheArms Arms = new MalabrancheArms();
			Arms.Movable = false;
			AddItem(Arms);
			
			MalabrancheLegs Legs = new MalabrancheLegs();
			Legs.Movable = false;
			AddItem(Legs);
			
			MalabrancheGloves Gloves = new MalabrancheGloves();
			Gloves.Movable = false;
			AddItem(Gloves);
			
			MalabrancheVest HalfApron = new MalabrancheVest();
			HalfApron.Movable = false;
			AddItem(HalfApron);
			
			MalabrancheHelm Helm = new MalabrancheHelm();
			Helm.Movable = false;
			AddItem(Helm);

	    		MalabrancheRobe Robe = new MalabrancheRobe();
			Robe.Movable = false;
			AddItem(Robe);

            






                            }
                                 public override void GenerateLoot()
		{
			switch ( Utility.Random( 75 ))
			{
		case 0: PackItem( new MalabrancheRobe() ); break;
		case 1: PackItem( new MalabrancheHelm() ); break;
		case 2: PackItem( new MalabrancheLegs() ); break;
		case 3: PackItem( new MalabrancheArms() ); break;
		case 4: PackItem( new MalabrancheGloves() ); break;
		case 5: PackItem( new MalabrancheChest() ); break;
		case 6: PackItem( new MalabrancheVest() ); break;
				
				
		 }		
			


				
		 
		}
		
	public override bool HasBreath{ get{ return true ; } }
	public override int BreathFireDamage{ get{ return 20; } }
	public override int BreathColdDamage{ get{ return 20; } }
			
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


public Malacoda( Serial serial ) : base( serial )
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
