#region Header
// **********
// ServUO - ThiefAI.cs
// **********
#endregion

#region References
using System;

using Server.Items;
#endregion

//
// This is a first simple AI
//
//

namespace Server.Mobiles
{
	public class ThiefAI : BaseAI
	{
		private Item m_toDisarm;

		public ThiefAI(BaseCreature m)
			: base(m)
		{ }

		public override bool DoActionWander()
		{
			m_Mobile.DebugSay("I have no combatant");

			if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
			{
				m_Mobile.DebugSay("I have detected {0}, attacking", m_Mobile.FocusMob.Name);

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
			Mobile combatant = m_Mobile.Combatant as Mobile;

			if (combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map)
			{
				m_Mobile.DebugSay("My combatant is gone, so my guard is up");

				Action = ActionType.Guard;

				return true;
			}

			if (WalkMobileRange(combatant, 1, true, m_Mobile.RangeFight, m_Mobile.RangeFight))
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo(combatant);
				if (m_toDisarm == null)
				{
					m_toDisarm = combatant.FindItemOnLayer(Layer.OneHanded);
				}

				if (m_toDisarm == null)
				{
					m_toDisarm = combatant.FindItemOnLayer(Layer.TwoHanded);
				}

				if (m_toDisarm != null && m_toDisarm.IsChildOf(m_Mobile.Backpack))
				{
					m_toDisarm = combatant.FindItemOnLayer(Layer.OneHanded);
					if (m_toDisarm == null)
					{
						m_toDisarm = combatant.FindItemOnLayer(Layer.TwoHanded);
					}
				}
				if (!Core.AOS && !m_Mobile.DisarmReady && m_Mobile.Skills[SkillName.Wrestling].Value >= 80.0 &&
					m_Mobile.Skills[SkillName.ArmsLore].Value >= 80.0 && m_toDisarm != null)
				{
					EventSink.InvokeDisarmRequest(new DisarmRequestEventArgs(m_Mobile));
				}

				if (m_toDisarm != null && m_toDisarm.IsChildOf(combatant.Backpack) && m_Mobile.NextSkillTime <= Core.TickCount &&
					(m_toDisarm.LootType != LootType.Blessed && m_toDisarm.LootType != LootType.Newbied))
				{
					m_Mobile.DebugSay("Trying to steal from combatant.");
					m_Mobile.UseSkill(SkillName.Stealing);
					if (m_Mobile.Target != null)
					{
						m_Mobile.Target.Invoke(m_Mobile, m_toDisarm);
					}
				}
				else if (m_toDisarm == null && m_Mobile.NextSkillTime <= Core.TickCount)
				{
					Container cpack = combatant.Backpack;

					if (cpack != null)
					{
						Item steala = cpack.FindItemByType(typeof(Bandage));
						if (steala != null)
						{
							m_Mobile.DebugSay("Trying to steal from combatant.");
							m_Mobile.UseSkill(SkillName.Stealing);
							if (m_Mobile.Target != null)
							{
								m_Mobile.Target.Invoke(m_Mobile, steala);
							}
						}
						Item stealb = cpack.FindItemByType(typeof(Nightshade));
						if (stealb != null)
						{
							m_Mobile.DebugSay("Trying to steal from combatant.");
							m_Mobile.UseSkill(SkillName.Stealing);
							if (m_Mobile.Target != null)
							{
								m_Mobile.Target.Invoke(m_Mobile, stealb);
							}
						}
						Item stealc = cpack.FindItemByType(typeof(BlackPearl));
						if (stealc != null)
						{
							m_Mobile.DebugSay("Trying to steal from combatant.");
							m_Mobile.UseSkill(SkillName.Stealing);
							if (m_Mobile.Target != null)
							{
								m_Mobile.Target.Invoke(m_Mobile, stealc);
							}
						}

						Item steald = cpack.FindItemByType(typeof(MandrakeRoot));
						if (steald != null)
						{
							m_Mobile.DebugSay("Trying to steal from combatant.");
							m_Mobile.UseSkill(SkillName.Stealing);
							if (m_Mobile.Target != null)
							{
								m_Mobile.Target.Invoke(m_Mobile, steald);
							}
						}
						else if (steala == null && stealb == null && stealc == null && steald == null)
						{
							m_Mobile.DebugSay("I am going to flee from {0}", combatant.Name);

							Action = ActionType.Flee;
						}
					}
				}
			}
			else
			{
				m_Mobile.DebugSay("I should be closer to {0}", combatant.Name);
			}

			if (m_Mobile.Hits < m_Mobile.HitsMax * 20 / 100 && m_Mobile.CanFlee)
			{
				// We are low on health, should we flee?
				bool flee = false;

				if (m_Mobile.Hits < combatant.Hits)
				{
					// We are more hurt than them
					int diff = combatant.Hits - m_Mobile.Hits;

					flee = (Utility.Random(0, 100) > (10 + diff)); // (10 + diff)% chance to flee
				}
				else
				{
					flee = Utility.Random(0, 100) > 10; // 10% chance to flee
				}

				if (flee)
				{
					m_Mobile.DebugSay("I am going to flee from {0}", combatant.Name);

					Action = ActionType.Flee;
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
			if (m_Mobile.Hits > m_Mobile.HitsMax / 2)
			{
				m_Mobile.DebugSay("I am stronger now, so I will continue fighting");
				Action = ActionType.Combat;
			}
			else
			{
				m_Mobile.FocusMob = m_Mobile.Combatant as Mobile;
				base.DoActionFlee();
			}

			return true;
		}
	}
}