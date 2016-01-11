//Revamped Nerun's NinjaAI && ServUO 
//by KilraYan

using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Spells;
using Server.Spells.Ninjitsu;
using System.Collections.Generic;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	public class NinjaAI : MeleeAI
	{
		private DateTime m_NextCastTime;
		private DateTime m_NextRanged;
		private DateTime m_NextMorphTime;

		private static double mirrorChance = 0.4; //chance to perform mirror image
		private static double rangedattackChance = 1; //chance to ranged attacks.
		private static double hidechance = 1; //chance to hide also during fights
		private static double comboChance = 1; //chance to mix shadowjump and special move!
		private static bool formChange = true; //ninjas animal form
		private static int morphtime = 30; //times of polimorph in secs
		private static bool dorangedattackflee = true; //do ranged attack during flee!

		public NinjaAI(BaseCreature bc) : base(bc)
		{
			m_NextCastTime = DateTime.UtcNow;
			m_NextMorphTime = DateTime.UtcNow;
		}
		
		private bool TryPerformHide()
        {
            if (!m_Mobile.Alive || m_Mobile.Deleted || m_Mobile.Hidden)
                return false;
			
            if (m_Mobile.Combatant != null && !m_Mobile.Hidden && Core.TickCount - m_Mobile.NextSkillTime >= 0)
            {
				double distance = m_Mobile.GetDistanceToSqrt( m_Mobile.Combatant );		
				double skill = m_Mobile.Skills[SkillName.Hiding].Value;
				double hidedistance = 400/skill;
				
				if (Utility.RandomDouble() < hidechance && distance > hidedistance) //ninja successfully hidden!
				{
                HideSelf();
				return true;
				}
				return false;
			}

			return false;
		}

		private static int[] m_Bodies = new int[]
		{
			0x84,
			0x7A,
			0xF6,
			0x19,
			0xDC,
			0xDA,
			0x51,
			0x15,
			0xD9,
			0xC9,
			0xEE,					
			0xCD
		};

		private static int[] m_Offsets = new int[]
		{
		 	 0, 0,
			-1,-1,
			 0,-1,
			 1,-1,
			-1, 0,
			 1, 0,
			-1,-1,
			 0, 1,
			 1, 1,
		};
		
		private void HideSelf()
        {
            Effects.SendLocationParticles(EffectItem.Create(m_Mobile.Location, m_Mobile.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);

            m_Mobile.PlaySound(0x22F);
            m_Mobile.Hidden = true;
            m_Mobile.UseSkill(SkillName.Stealth);
        }
		
		public virtual SpecialMove GetHiddenSpecialMove()
		{			
			double skill = m_Mobile.Skills[SkillName.Ninjitsu].Value;
			
			SpecialMove special = SpecialMove.GetCurrentMove(m_Mobile);
			
			if (special != null)
				return special;
			
			if (skill < 20  || m_Mobile.Mana < 30)
				return null;
			
			int avail = 1;
			
			if (m_Mobile.AllowedStealthSteps != 0)
			{			
				if(skill >= 30)
					avail = 2;
				Mobile combatant = m_Mobile.Combatant;
				if (skill >= 85)
				{
					avail = 3;
					//Mobile combatant = m_Mobile.Combatant;
					if ( combatant != null && (combatant.Hits < (Utility.Random( 10 ) + 10) || !combatant.Warmode))
					return SpellRegistry.GetSpecialMove(501);
				}
			
				switch(Utility.Random(avail))
				{
					case 0: combatant.Say("Getting BackStab"); return SpellRegistry.GetSpecialMove(505);//new Backstab(); skill = 20;
					case 1: combatant.Say("Getting SurpriseAttack");return SpellRegistry.GetSpecialMove(504); //new SurpriseAttack(); skill = 30;
					case 2: combatant.Say("Getting DeathStrike"); return SpellRegistry.GetSpecialMove(501); //new DeathStrike(); skill = 85;
				}
			}
			else
			{
				if(skill < 30)
				return null;
			
				if (skill < 80)
					avail = 2;

				if(skill >= 85)
				{
					avail = 3;
					Mobile combatant = m_Mobile.Combatant;
				
					if ( combatant != null && (int)combatant.Hits < (Utility.Random( 10 ) + 10) )
					return SpellRegistry.GetSpecialMove(501);
				}

				switch(Utility.Random(avail))
				{
					case 0: return SpellRegistry.GetSpecialMove(500); //new FocusAttack();
					case 1: return SpellRegistry.GetSpecialMove(503); //new KiAttack();
					case 2: return SpellRegistry.GetSpecialMove(501); //new DeathStrike();
				}
			}
			return null;
		}
		
		public virtual SpecialMove GetSpecialMove()
		{			
			double skill = m_Mobile.Skills[SkillName.Ninjitsu].Value;
			
			SpecialMove special = SpecialMove.GetCurrentMove(m_Mobile);
			
			if (special != null)
				return special;
			
			if(skill < 30 || m_Mobile.Mana < 30)
				return null;
			
			int avail = 1;
			
			if (skill < 80)
				avail = 2;

			if(skill >= 85)
			{
				avail = 3;
				Mobile combatant = m_Mobile.Combatant;
				
				if ( combatant != null && (int)combatant.Hits < (Utility.Random( 10 ) + 10) )
				return SpellRegistry.GetSpecialMove(501);
			}

			switch(Utility.Random(avail))
			{
				case 0: return SpellRegistry.GetSpecialMove(500); //new FocusAttack();
				case 1: return SpellRegistry.GetSpecialMove(503); //new KiAttack();
                case 2: return SpellRegistry.GetSpecialMove(501); //new DeathStrike();
			}
			return null;
		}
		
		public bool DoRangedAttack()
		{
			Mobile c = m_Mobile.Combatant;
			
			if(c == null || Utility.RandomDouble () > rangedattackChance)
				return false;
			
			double distance = m_Mobile.GetDistanceToSqrt( c );

			if (distance < 2 || distance > 6)
				return false;
			
			int skill = (int)m_Mobile.Skills[SkillName.Ninjitsu].Value;
			int seconds = 400/skill;
			
			List<INinjaWeapon> list = new List<INinjaWeapon>();
			int d = (int)m_Mobile.GetDistanceToSqrt(c.Location);
			
			foreach(Item item in m_Mobile.Items)
				if(item is INinjaWeapon && ((INinjaWeapon)item).UsesRemaining > 0 && d >= ((INinjaWeapon)item).WeaponMinRange && d <= ((INinjaWeapon)item).WeaponMaxRange)
					list.Add(item as INinjaWeapon);

            if (m_Mobile.Backpack != null)
            {
                foreach (Item item in m_Mobile.Backpack.Items)
                    if (item is INinjaWeapon && ((INinjaWeapon)item).UsesRemaining > 0 && d >= ((INinjaWeapon)item).WeaponMinRange && d <= ((INinjaWeapon)item).WeaponMaxRange)
                        list.Add(item as INinjaWeapon);
            }
			
			if(list.Count > 0)
			{
				INinjaWeapon toUse = list[Utility.Random(list.Count)];
				
				if(toUse != null)
				{
				NinjaWeapon.Shoot(m_Mobile, c, toUse);
				m_NextRanged = DateTime.UtcNow + TimeSpan.FromSeconds(seconds + Utility.RandomMinMax(-1, +1));
				return true;
				}
			}
			return false;
		}
		
		public bool DoChoosenAttack()
		{
			if (m_Mobile == null)
				return false;
			
			SpecialMove special = SpecialMove.GetCurrentMove(m_Mobile);
			
			if (special != null)
				return false;
			else if(m_NextCastTime < DateTime.UtcNow)
			{
				if ( m_Mobile.Hidden )
					special = GetHiddenSpecialMove();
				else
					special = GetSpecialMove();
				
				SpecialMove.SetCurrentMove(m_Mobile, special);
				m_NextCastTime = DateTime.UtcNow + GetCastDelay();
				return true;
			}
			return false;
		}

		public override bool DoActionWander()
		{
			m_Mobile.DebugSay( "I am wandering around." );
			
			if ( formChange && m_NextMorphTime < DateTime.UtcNow)
			{
				if ( m_Mobile.BodyMod == 0 ) 
				{
					ChangeForm( m_Bodies[ Utility.Random( m_Bodies.Length ) ] );
				} 
				else
				{
					ChangeForm ( 0 );
				}
			}

			//relax mode
			m_Mobile.DebugSay( "Threat gone, I am going to relax down myself..." );
			
			if (m_Mobile.Hidden)
				m_Mobile.Hidden = false;
			
			SpecialMove special = SpecialMove.GetCurrentMove(m_Mobile);
			if(special != null)
			{
				SpecialMove.SetCurrentMove(m_Mobile, null);
				return true;
			}
			//relax mode
			
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "Enemy detected: going to enter sneak mode and attack" );
				if ( !m_Mobile.Hidden && Utility.RandomDouble() < m_Mobile.Skills[SkillName.Hiding].Value/100 && (m_Mobile.GetDistanceToSqrt( m_Mobile.FocusMob ) > 3 || m_Mobile.FocusMob.Warmode == false))
					TryPerformHide();
	
				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
				return base.DoActionWander();

			return true;
		}
		
		public override bool DoActionCombat()
		{				
			if ( formChange && m_Mobile.BodyMod != 0 )
			{
				ChangeForm( 0 );
			}
			
			Mobile combatant = m_Mobile.Combatant;
			
			if ( combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map || !combatant.Alive || combatant.IsDeadBondedPet )
			{
				m_Mobile.DebugSay( "My combatant is gone, so my guard is up" );

				Action = ActionType.Guard;

				return true;
			}
			
			double distance = m_Mobile.GetDistanceToSqrt( combatant );

			if ( MoveTo( combatant, true, m_Mobile.RangeFight ) )
			{	
				m_Mobile.Direction = m_Mobile.GetDirectionTo( combatant );
			}
			else if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name );
				if ( combatant.Poisoned == false && m_NextRanged <= DateTime.UtcNow)
				DoRangedAttack();
				TryPerformHide();
				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else if ( m_Mobile.GetDistanceToSqrt( combatant ) > m_Mobile.RangePerception + 1 )
			{
				m_Mobile.DebugSay( "I cannot find {0}, so my guard is up", combatant.Name );
				TryPerformHide();
				Action = ActionType.Guard;
				return true;
			}
			else
			{		
				m_Mobile.DebugSay( "I should be closer or do ranged attack or comboattack etc to {0}", combatant.Name );
			}

			if ( !m_Mobile.Controlled && !m_Mobile.Summoned )
			{	
				double mobilehperc = (double)m_Mobile.Hits/(double)m_Mobile.HitsMax;
				if ( mobilehperc < Utility.RandomDouble() + 0.1 ) 
				{
					// I am wounded, should I change tactic or keep bulling?

					bool flee = false;
					double combatanthperc = (double)combatant.Hits/(double)combatant.HitsMax;
					
					if ( mobilehperc < combatanthperc )
					{
						// We are more hurt than them

						int diff = (int)((combatanthperc - mobilehperc)*100 -10);

						flee = (Utility.Random( 0, 100 ) < (10 + diff)); // (10 + diff)% chance to flee
					}
					else
					{
						flee = Utility.Random( 0, 100 ) < 10; // 10% chance to flee
					}

					if ( flee )
					{
						if ( m_Mobile.Debug )
						{
							m_Mobile.DebugSay( "I am going to flee from {0}", combatant.Name );
						}

						Action = ActionType.Flee;
					}
				}
			}

			double dstToTarget = m_Mobile.GetDistanceToSqrt( combatant );

			TryPerformHide();
			
			if ( dstToTarget > 2.0 && m_Mobile.AllowedStealthSteps != 0 && CanUseAbility( 50.0, 45, comboChance ) )
			{
				if (DoChoosenAttack())
				{
				PerformShadowjump( combatant );
				return true;
				}
			}
			
			if ( combatant.Poisoned == false && m_NextRanged <= DateTime.UtcNow )
			{
				if (DoRangedAttack())
				return true;
			}
			if ( PerformMirror() )
			{
				return true;
			}
			else 
				DoChoosenAttack();
				
			return true;
		}
		
		public override bool DoActionFlee()
		{
			m_Mobile.FocusMob = m_Mobile.Combatant;
			
			if ( m_Mobile.FocusMob == null || m_Mobile.FocusMob.Deleted || m_Mobile.FocusMob.Map != m_Mobile.Map )
			{
				m_Mobile.DebugSay( "I have lost im" );
				Action = ActionType.Guard;
				return true;
			}
			
			double myhitsperc = m_Mobile.Hits/m_Mobile.HitsMax;
			
			if ( m_Mobile.Hits == m_Mobile.HitsMax || Utility.RandomDouble() < myhitsperc - 0.1 || (m_Mobile.Hidden == true && Utility.RandomDouble() < 0.3))
			{
				m_Mobile.DebugSay( "I recovered enough, so I will continue fighting" );
				Action = ActionType.Combat;
			}
			else
			{				
				double distance = m_Mobile.GetDistanceToSqrt( m_Mobile.FocusMob );

				if ( !m_Mobile.Hidden )
				{
					if (!TryPerformHide())
					PerformMirror();
				}
				if ( WalkMobileRange( m_Mobile.FocusMob, 1, false, m_Mobile.RangePerception*2, m_Mobile.RangePerception*3 ) )
				{
					m_Mobile.DebugSay( "I Have fled" );
					Action = ActionType.Guard;
					return true;
				}
				else
				{
					m_Mobile.DebugSay( "I am fleeing!" );
					if ( dorangedattackflee && m_NextRanged <= DateTime.UtcNow && m_Mobile.Combatant.Poisoned == false && (!m_Mobile.Hidden || (m_Mobile.Hidden && myhitsperc > 0.3 && Utility.RandomDouble() < .5)))
						{
							if (!DoRangedAttack())
							DoChoosenAttack();
						}
					else
						DoChoosenAttack();
				}
			}

			return true;
		}
		
		//////
		public TimeSpan GetCastDelay()
        {
			
            int skill = (int)m_Mobile.Skills[SkillName.Ninjitsu].Value;
			int seconds = 400/skill;
			
			if (m_Mobile.Mana < 30)
				seconds += 35 - m_Mobile.Mana;
			
			seconds += Utility.RandomMinMax(-1,1);
			
			if (seconds < 1)
				seconds = 1;
			
			return TimeSpan.FromSeconds(seconds);
        }
		
		//NEW FUNCTIONS
		
		private void ChangeForm( int body )
		{		
			if(CanUseAbility( 40.0, 0, 1 ) && body != 0)
			{
				m_Mobile.FixedEffect( 0x37C4, 10, 42, 4, 3 );
				m_Mobile.BodyMod = body;
				m_NextMorphTime = DateTime.UtcNow + TimeSpan.FromSeconds(morphtime + Utility.RandomMinMax (-10,+10));
				return;
			} 
			else if (body == 0)
			{
				m_Mobile.FixedEffect( 0x37C4, 10, 42, 4, 3 );
				m_Mobile.BodyMod = body;
				m_NextMorphTime = DateTime.UtcNow + TimeSpan.FromSeconds(morphtime + Utility.RandomMinMax (-10,0));
				return;
			}			
			return;
		}
		
		public override bool Think()
		{
			try
			{
			if( m_Mobile.Deleted )
				return false;

			if( CheckFlee() )
				return true;

			switch( Action )
			{
				case ActionType.Wander:
				m_Mobile.OnActionWander();
				return DoActionWander();

				case ActionType.Combat:
				m_Mobile.OnActionCombat();
				return DoActionCombat();

				case ActionType.Guard:
				m_Mobile.OnActionGuard();
				return DoActionGuard();

				case ActionType.Flee:
				m_Mobile.OnActionFlee();
				return DoActionFlee();

				case ActionType.Interact:
				m_Mobile.OnActionInteract();
				return DoActionInteract();

				case ActionType.Backoff:
				m_Mobile.OnActionBackoff();
				return DoActionBackoff();

				default:
				return false;
			}
			}
			catch ( Exception e )
			{
				Console.WriteLine( "Catched Exception from EliteNinja when " + Action.ToString() );
				Console.WriteLine( e.ToString() );
				return false;
			}
		}

		// Shadowjump 
		private bool PerformShadowjump( Mobile toTarget )
		{
			if ( m_Mobile.Skills[ SkillName.Ninjitsu ].Value < 50.0 || m_Mobile.Mana < 15)
			{
				return false;
			}

			if ( toTarget != null )
			{
				Map map = m_Mobile.Map;

				if ( map == null )
				{
					return false;
				}

				int px, py, ioffset = 0;

				px = toTarget.X;
				py = toTarget.Y;

				if ( Action == ActionType.Flee )
				{
					double outerradius = m_Mobile.Skills[ SkillName.Ninjitsu ].Value/10.0;
					double radiusoffset = 2.0;
					// random point for direction vector
					int rpx = Utility.Random( 40 ) - 20 + toTarget.X;
					int rpy = Utility.Random( 40 ) - 20 + toTarget.Y;
					// get vector
					int dx = rpx - toTarget.X;
					int dy = rpy - toTarget.Y;
					// get vector's length
					double l = Math.Sqrt( (double) (dx*dx + dy*dy) );

					if ( l == 0 )
					{
						return false;
					}
					// normalize vector
					double dpx = ((double) dx)/l;
					double dpy = ((double) dy)/l;
					// move 
					px += (int) (dpx*(outerradius - radiusoffset) + Math.Sign( dpx )*(radiusoffset + 0.5));
					py += (int) (dpy*(outerradius - radiusoffset) + Math.Sign( dpy )*(radiusoffset + 0.5));
				}
				else
				{
					ioffset = 2;
				}

				for ( int i = ioffset; i < m_Offsets.Length; i += 2 )
				{
					int x = m_Offsets[ i ], y = m_Offsets[ i + 1 ];

					Point3D p = new Point3D( px + x, py + y, 0 );

					LandTarget lt = new LandTarget( p, map );

					if ( m_Mobile.InLOS( lt ) && map.CanSpawnMobile( px + x, py + y, lt.Z ) && !SpellHelper.CheckMulti( p, map ) )
					{
						m_Mobile.Location = new Point3D( lt.X, lt.Y, lt.Z );
						m_Mobile.ProcessDelta();
						m_Mobile.Mana -= 15;
						return true;
					}
				}
			}

			return false;
		}
		
		private bool PerformMirror()
		{			
			if(CanUseAbility( 40.0, 10, mirrorChance ) && (m_Mobile.Followers < m_Mobile.FollowersMax) && m_NextCastTime < DateTime.UtcNow)
			{
				new MirrorImage( m_Mobile, null ).Cast();
                m_NextCastTime = DateTime.UtcNow + GetCastDelay();
				return true;
			}
						
			return false;
		}

		public override bool DoActionGuard()
		{
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionGuard();
			}

			return true;
		}

		private bool CanUseAbility( double limit, int mana, double chance )
		{
			if ( m_Mobile.Skills[ SkillName.Ninjitsu ].Value >= limit && m_Mobile.Mana >= mana )
			{
				if ( chance >= Utility.RandomDouble() )
				{
					return true;
				}
			}

			return false;
		}
		
	}
}
