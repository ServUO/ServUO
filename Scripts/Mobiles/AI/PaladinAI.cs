using System;
using System.Collections;
using System.Collections.Generic;
using Server.Targeting;
using Server.Network;
using Server.Spells;
using Server.Spells.Chivalry;
using Server.Items;

namespace Server.Mobiles
{
	public class PaladinAI : MeleeAI
	{
		public PaladinAI( BaseCreature m ) : base (m)
		{
		}
		
		public override bool Think()
		{			
			if ( m_Mobile.Deleted )
				return false;
		
			Target targ = m_Mobile.Target;
		
			if ( targ != null )
			{
				ProcessTarget( targ );

				return true;
			}
		
			return base.Think();
		}
		
		public override bool DoActionWander()
		{
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
				m_NextCastTime = DateTime.Now;
				m_NextHealTime = DateTime.Now;
			}
			else
			{
				m_Mobile.DebugSay( "I am wandering" );

				m_Mobile.Warmode = false;

				base.DoActionWander();

				if ( !m_Mobile.Controlled )
				{
					Spell spell = CheckCastHealingSpell();

					if ( spell != null )
						spell.Cast();
				}
			}

			return true;
		}

		public override bool DoActionCombat()
		{
			bool ret = base.DoActionCombat();
			
			if ( m_Mobile.Spell == null && DateTime.Now > m_NextCastTime )
			{
				Spell spell = null;
				
				if ( m_Mobile.Poisoned ) // Top cast priority is cure
				{
					m_Mobile.DebugSay( "I am going to cure myself" );

					spell = new CleanseByFireSpell( m_Mobile, null );
				}
				else if ( FindDispelTarget( true ) != null )
				{
					m_Mobile.DebugSay( "I am going to area dispel" );

					spell = new DispelEvilSpell( m_Mobile, null );					
				}
				else if(m_Mobile.Combatant != null)
				{
					spell = ChooseSpell();
				}
				
				if ( spell != null )
					spell.Cast();
					
				TimeSpan delay;

				if ( spell is DispelEvilSpell )
					delay = TimeSpan.FromSeconds( m_Mobile.ActiveSpeed );
				else
					delay = GetDelay();
					
				m_NextCastTime = DateTime.Now + delay;
			}			

			return ret;
		}
		
		private TimeSpan GetDelay()
		{
			double del = m_Mobile.Skills[ SkillName.Chivalry ].Value * 0.03;
			double min = 6.0 - (del * 0.75);
			double max = 6.0 - (del * 1.25);

			return TimeSpan.FromSeconds( min + ((max - min) * Utility.RandomDouble()) );
		}
		
		private DateTime m_NextCastTime = DateTime.Now;
		private DateTime m_NextHealTime = DateTime.Now;
		
		public virtual Spell CheckCastHealingSpell()
		{
			// Summoned creatures never heal themselves.
			if ( m_Mobile.Summoned )
				return null;
				
			if ( m_Mobile.Controlled )
			{
				if ( DateTime.Now < m_NextHealTime )
					return null;
			}
			
			if ( m_Mobile.Skills[ SkillName.Chivalry ].Value * 0.0005 < Utility.RandomDouble() )
				return null;
			
			Spell spell = null;

			if ( m_Mobile.Hits < (m_Mobile.HitsMax - 10) )
				spell = new CloseWoundsSpell( m_Mobile, null );
			
			double delay;

			if ( m_Mobile.Int >= 500 )
				delay = Utility.RandomMinMax( 7, 10 );
			else
				delay = Math.Sqrt( 600 - m_Mobile.Int );			
			
			m_NextHealTime = DateTime.Now + TimeSpan.FromSeconds( delay );
			
			return spell;
		}
		
		public Mobile FindDispelTarget( bool activeOnly )
		{
			if ( m_Mobile.Deleted || m_Mobile.Int < 95 || CanDispel( m_Mobile ) || m_Mobile.AutoDispel )
				return null;

			List<AggressorInfo> aggressed = m_Mobile.Aggressed;
			List<AggressorInfo> aggressors = m_Mobile.Aggressors;

			Mobile active = null;
			double activePrio = 0.0;

			Mobile comb = m_Mobile.Combatant as Mobile;

			if ( comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && m_Mobile.InRange( comb, 12 ) && CanDispel( comb ) )
			{
				active = comb;
				activePrio = m_Mobile.GetDistanceToSqrt( comb );

				if ( activePrio <= 2 )
					return active;
			}

			for ( int i = 0; i < aggressed.Count; ++i )
			{
				AggressorInfo info = (AggressorInfo)aggressed[i];
				Mobile m = (Mobile)info.Defender;

				if ( m != comb && m.Combatant == m_Mobile && m_Mobile.InRange( m, 12 ) && CanDispel( m ) )
				{
					double prio = m_Mobile.GetDistanceToSqrt( m );

					if ( active == null || prio < activePrio )
					{
						active = m;
						activePrio = prio;

						if ( activePrio <= 2 )
							return active;
					}
				}
			}

			for ( int i = 0; i < aggressors.Count; ++i )
			{
				AggressorInfo info = (AggressorInfo)aggressors[i];
				Mobile m = (Mobile)info.Attacker;

				if ( m != comb && m.Combatant == m_Mobile && m_Mobile.InRange( m, 12 ) && CanDispel( m ) )
				{
					double prio = m_Mobile.GetDistanceToSqrt( m );

					if ( active == null || prio < activePrio )
					{
						active = m;
						activePrio = prio;

						if ( activePrio <= 2 )
							return active;
					}
				}
			}

			return active;
		}

		public bool CanDispel( Mobile m )
		{
			return ( m is BaseCreature && ((BaseCreature)m).Summoned && m_Mobile.CanBeHarmful( m, false ) && !((BaseCreature)m).IsAnimatedDead );
		}
		
		public virtual Spell ChooseSpell()
		{			
			Spell spell = CheckCastHealingSpell();
			
			if ( spell != null )
				return spell;
			
			switch ( Utility.Random( 5 ) )
			{
				case 0:
				case 1:				
					BaseWeapon weapon = m_Mobile.Weapon as BaseWeapon;
					
					if ( weapon != null && !weapon.Consecrated )
						spell = new ConsecrateWeaponSpell( m_Mobile, null );				
					
					break;
				case 2:
				case 3:				
					if ( !DivineFurySpell.UnderEffect( m_Mobile ) )
						spell = new DivineFurySpell( m_Mobile, null );		
						
					break;	
				case 4:
					spell = new HolyLightSpell( m_Mobile, null );							
					break;	
			}
			
			return spell;
		}
		
		private void ProcessTarget( Target targ )
		{
			targ.Invoke( m_Mobile, m_Mobile );
		}
	}
}