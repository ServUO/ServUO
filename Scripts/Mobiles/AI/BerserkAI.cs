namespace Server.Mobiles
{
	public class BerserkAI : BaseAI
	{
		public BerserkAI(BaseCreature m)
			: base(m)
		{ }

		public override bool DoActionWander()
		{
			m_Mobile.DebugSay("I have No Combatant");

			if (AcquireFocusMob(m_Mobile.RangePerception, FightMode.Closest, false, true, true))
			{
				if (m_Mobile.Debug)
					m_Mobile.DebugSay("I have detected " + m_Mobile.FocusMob.Name + " and I will attack");

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionWander();
			}

			return true;
		}

		public override bool DoActionCombat()
		{
			if (m_Mobile.Combatant == null || m_Mobile.Combatant.Deleted)
			{
				m_Mobile.DebugSay("My combatant is deleted");
				Action = ActionType.Guard;
				return true;
			}

			if (WalkMobileRange(m_Mobile.Combatant, 1, true, m_Mobile.RangeFight, m_Mobile.RangeFight))
			{
				if (!DirectionLocked)
					m_Mobile.Direction = m_Mobile.GetDirectionTo(m_Mobile.Combatant.Location);
			}
			else
			{
				if (m_Mobile.Combatant != null)
				{
					if (m_Mobile.Debug)
						m_Mobile.DebugSay("I am still not in range of " + m_Mobile.Combatant.Name);

					if ((int)m_Mobile.GetDistanceToSqrt(m_Mobile.Combatant) > m_Mobile.RangePerception + 1)
					{
						if (m_Mobile.Debug)
							m_Mobile.DebugSay("I have lost " + m_Mobile.Combatant.Name);

						Action = ActionType.Guard;
						return true;
					}
				}
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, true, true))
			{
				if (m_Mobile.Debug)
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
	}
}