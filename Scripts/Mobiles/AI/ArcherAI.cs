#region Header
// **********
// ServUO - ArcherAI.cs
// **********
#endregion

#region References
using System;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class ArcherAI : BaseAI
	{
		public ArcherAI(BaseCreature m)
			: base(m)
		{ }

		public override bool DoActionWander()
		{
			m_Mobile.DebugSay("I have no combatant");

			if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
			{
				m_Mobile.DebugSay("I have detected {0} and I will attack", m_Mobile.FocusMob.Name);

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				return base.DoActionWander();
			}

			return true;
		}

		public override bool DoActionCombat()
		{
            IDamageable c = m_Mobile.Combatant;

			if (c == null || c.Deleted ||
                !c.Alive || (c is Mobile && ((Mobile)c).IsDeadBondedPet))
			{
				m_Mobile.DebugSay("My combatant is deleted");
				Action = ActionType.Guard;
				return true;
			}

			if (Core.TickCount - m_Mobile.LastMoveTime > 1000)
			{
				if (WalkMobileRange(c, 1, true, m_Mobile.RangeFight, m_Mobile.Weapon.MaxRange))
				{
					// Be sure to face the combatant
					m_Mobile.Direction = m_Mobile.GetDirectionTo(c.Location);
				}
				else
				{
					if (c != null)
					{
						m_Mobile.DebugSay("I am still not in range of {0}", c.Name);

						if ((int)m_Mobile.GetDistanceToSqrt(c) > m_Mobile.RangePerception + 1)
						{
							m_Mobile.DebugSay("I have lost {0}", c.Name);

							Action = ActionType.Guard;
							return true;
						}
					}
				}
			}

            if (!m_Mobile.Controlled && !m_Mobile.Summoned && m_Mobile.CanFlee)
            {
/*                // When we have no ammo, we flee
                Container pack = m_Mobile.Backpack;

                if (pack == null || pack.FindItemByType(typeof(Arrow)) == null)
                {
                    Action = ActionType.Flee;
                    return true;
                }
*/
                if (m_Mobile.Hits < m_Mobile.HitsMax * 20 / 100)
                {
                    // We are low on health, should we flee?
                    if (Utility.Random(100) <= Math.Max(10, 10 + c.Hits - m_Mobile.Hits))
                    {
                        m_Mobile.DebugSay("I am going to flee from {0}", c.Name);
                        Action = ActionType.Flee;
                    }
                }
            }

			return true;
		}

		public override bool DoActionGuard()
		{
			if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
			{
				m_Mobile.DebugSay("I have detected {0}, attacking", m_Mobile.FocusMob.Name);

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionGuard();
			}

			return true;
		}

        public override bool DoActionFlee()
        {
            Mobile c = m_Mobile.Combatant as Mobile;

            // When we have no ammo, we flee
//            Container pack = m_Mobile.Backpack;
//            bool hasAmmo = !(pack == null || pack.FindItemByType(typeof(Arrow)) == null);
// They can shoot even with no ammo!

            if (/*hasAmmo && */m_Mobile.Hits > (m_Mobile.HitsMax / 2))
            {
                // If I have a target, go back and fight them
                if (c != null && m_Mobile.GetDistanceToSqrt(c) <= m_Mobile.RangePerception * 2)
                {
                    m_Mobile.DebugSay("I am stronger now, reengaging {0}", c.Name);
                    Action = ActionType.Combat;
                }
                else
                {
                    m_Mobile.DebugSay("I am stronger now, my guard is up");
                    Action = ActionType.Guard;
                }
            }
            else
            {
                base.DoActionFlee();
            }

            return true;
        }
    }
}
