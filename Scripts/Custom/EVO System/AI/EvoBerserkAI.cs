#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Xanthos.Evo
{
	class EvoBerserkAI : BerserkAI
	{
		private bool m_CanAttackPlayers;

		public EvoBerserkAI( BaseCreature m, bool canAttackPlayers ) : base( m )
		{
			m_CanAttackPlayers = canAttackPlayers;
		}

		public override void EndPickTarget( Mobile from, Mobile target, OrderType order )
		{
			Mobile oldTarget = m_Mobile.ControlTarget;
			OrderType oldOrder = order;

			base.EndPickTarget( from, target, order );

			if ( OrderType.Attack == order && target is PlayerMobile && !m_CanAttackPlayers )
			{
				// Not allowed to attack players so reset what was changed by EndPickTarget
				m_Mobile.ControlTarget = oldTarget;
				m_Mobile.ControlOrder = oldOrder;
			}
		}

		public override bool DoOrderGuard()
		{
			if ( m_Mobile.IsDeadPet )
				return true;

			Mobile controlMaster = m_Mobile.ControlMaster;

			if ( controlMaster == null || controlMaster.Deleted )
				return true;

			Mobile combatant = m_Mobile.Combatant;

			List<AggressorInfo> aggressors = controlMaster.Aggressors;

			if ( aggressors.Count > 0 )
			{
				for ( int i = 0; i < aggressors.Count; ++i )
				{
					AggressorInfo info = aggressors[ i ];
					Mobile attacker = info.Attacker;

					if ( attacker != null && !attacker.Deleted && attacker.GetDistanceToSqrt( m_Mobile ) <= m_Mobile.RangePerception )
					{
						if ( combatant == null || attacker.GetDistanceToSqrt( controlMaster ) < combatant.GetDistanceToSqrt( controlMaster ) )
						{
							if ( ( attacker is PlayerMobile && m_CanAttackPlayers ) || !( attacker is PlayerMobile ) )
								combatant = attacker;
						}
					}
				}

				if ( combatant != null )
					m_Mobile.DebugSay( "Crap, my master has been attacked! I will atack one of those bastards!" );
			}

			if ( combatant != null && combatant != m_Mobile && combatant != m_Mobile.ControlMaster && !combatant.Deleted && combatant.Alive && !combatant.IsDeadBondedPet && m_Mobile.CanSee( combatant ) && m_Mobile.CanBeHarmful( combatant, false ) && combatant.Map == m_Mobile.Map )
			{
				m_Mobile.DebugSay( "Guarding from target..." );

				m_Mobile.Combatant = combatant;
				m_Mobile.FocusMob = combatant;
				Action = ActionType.Combat;

				/*
				 * We need to call Think() here or spell casting monsters will not use
				 * spells when guarding because their target is never processed.
				 */
				Think();
			}
			else
			{
				m_Mobile.DebugSay( "Nothing to guard from" );

				m_Mobile.Warmode = false;

				WalkMobileRange( controlMaster, 1, false, 0, 1 );
			}

			return true;
		}
	}
}
