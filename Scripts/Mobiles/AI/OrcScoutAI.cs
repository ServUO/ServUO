#region Header
// **********
// ServUO - OrcScoutAI.cs
// **********
#endregion

#region References
using System;

using Server.Items;
using Server.Spells;
using Server.Targeting;
#endregion

namespace Server.Mobiles
{
	public class OrcScoutAI : BaseAI
	{
		private static readonly double teleportChance = 0.04;
		private static readonly int[] m_Offsets = new[] {0, 0, -1, -1, 0, -1, 1, -1, -1, 0, 1, 0, -1, -1, 0, 1, 1, 1,};

		public OrcScoutAI(BaseCreature m)
			: base(m)
		{ }

		public override bool DoActionWander()
		{
			m_Mobile.DebugSay("I have no combatant");

			PerformHide();

			if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
			{
				if (m_Mobile.Debug)
				{
					m_Mobile.DebugSay("I have detected {0}, attacking", m_Mobile.FocusMob.Name);
				}

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				if (m_Mobile.Combatant != null)
				{
					Action = ActionType.Combat;
					return true;
				}

				base.DoActionWander();
			}

			return true;
		}

		public override bool DoActionCombat()
		{
			Mobile combatant = m_Mobile.Combatant as Mobile;

			if (combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map || !combatant.Alive ||
				combatant.IsDeadBondedPet)
			{
				m_Mobile.DebugSay("My combatant is gone, so my guard is up");

				Action = ActionType.Guard;

				return true;
			}

			if (Utility.RandomDouble() < teleportChance)
			{
				TryToTeleport();
			}

			if (!m_Mobile.InRange(combatant, m_Mobile.RangePerception))
			{
				// They are somewhat far away, can we find something else?
				if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if (!m_Mobile.InRange(combatant, m_Mobile.RangePerception * 3))
				{
					m_Mobile.Combatant = null;
				}

				combatant = m_Mobile.Combatant as Mobile;

				if (combatant == null)
				{
					m_Mobile.DebugSay("My combatant has fled, so I am on guard");
					Action = ActionType.Guard;

					return true;
				}
			}

			/*if ( !m_Mobile.InLOS( combatant ) )
            {
            if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
            {
            m_Mobile.Combatant = combatant = m_Mobile.FocusMob;
            m_Mobile.FocusMob = null;
            }
            }*/

			if (MoveTo(combatant, true, m_Mobile.RangeFight))
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo(combatant);
			}
			else if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
			{
				if (m_Mobile.Debug)
				{
					m_Mobile.DebugSay("My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name);
				}

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;

				return true;
			}
			else if (m_Mobile.GetDistanceToSqrt(combatant) > m_Mobile.RangePerception + 1)
			{
				if (m_Mobile.Debug)
				{
					m_Mobile.DebugSay("I cannot find {0}, so my guard is up", combatant.Name);
				}

				Action = ActionType.Guard;

				return true;
			}
			else
			{
				if (m_Mobile.Debug)
				{
					m_Mobile.DebugSay("I should be closer to {0}", combatant.Name);
				}
			}

			if (!m_Mobile.Controlled && !m_Mobile.Summoned)
			{
				if (m_Mobile.Hits < m_Mobile.HitsMax * 20 / 100)
				{
					// We are low on health, should we flee?
					bool flee = false;

					if (m_Mobile.Hits < combatant.Hits)
					{
						// We are more hurt than them
						int diff = combatant.Hits - m_Mobile.Hits;

						flee = (Utility.Random(0, 100) < (10 + diff)); // (10 + diff)% chance to flee
					}
					else
					{
						flee = Utility.Random(0, 100) < 10; // 10% chance to flee
					}

					if (flee)
					{
						if (m_Mobile.Debug)
						{
							m_Mobile.DebugSay("I am going to flee from {0}", combatant.Name);
						}

						Action = ActionType.Flee;

						if (Utility.RandomDouble() < teleportChance + 0.1)
						{
							TryToTeleport();
						}
					}
				}
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
			{
				if (m_Mobile.Debug)
				{
					m_Mobile.DebugSay("I have detected {0}, attacking", m_Mobile.FocusMob.Name);
				}

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

				PerformHide();

				if (WalkMobileRange(m_Mobile.FocusMob, 1, false, m_Mobile.RangePerception * 2, m_Mobile.RangePerception * 3))
				{
					m_Mobile.DebugSay("I Have fled");
					Action = ActionType.Guard;
					return true;
				}
				else
				{
					m_Mobile.DebugSay("I am fleeing!");
				}
			}

			return true;
		}

		private Mobile FindNearestAggressor()
		{
			Mobile nearest = null;

			double dist = 9999.0;

			foreach (Mobile m in m_Mobile.GetMobilesInRange(m_Mobile.RangePerception))
			{
				if (m.Player && !m.Hidden && m.IsPlayer() && m.Combatant == m_Mobile)
				{
					if (dist > m.GetDistanceToSqrt(m_Mobile))
					{
						nearest = m;
					}
				}
			}

			return nearest;
		}

		private void TryToTeleport()
		{
			Mobile m = FindNearestAggressor();

			if (m == null || m.Map == null || m_Mobile.Map == null)
			{
				return;
			}

			if (m_Mobile.GetDistanceToSqrt(m) > m_Mobile.RangePerception + 1)
			{
				return;
			}

			int px = m_Mobile.X;
			int py = m_Mobile.Y;

			int dx = m_Mobile.X - m.X;
			int dy = m_Mobile.Y - m.Y;

			// get vector's length

			double l = Math.Sqrt((dx * dx + dy * dy));

			if (l == 0)
			{
				int rand = Utility.Random(8) + 1;
				rand *= 2;
				dx = m_Offsets[rand];
				dy = m_Offsets[rand + 1];
				l = Math.Sqrt((dx * dx + dy * dy));
			}

			// normalize vector
			double dpx = (dx) / l;
			double dpy = (dy) / l;
			// move 
			px += (int)(dpx * (4 + Utility.Random(3)));
			py += (int)(dpy * (4 + Utility.Random(3)));

			for (int i = 0; i < m_Offsets.Length; i += 2)
			{
				int x = m_Offsets[i], y = m_Offsets[i + 1];

				Point3D p = new Point3D(px + x, py + y, 0);

				LandTarget lt = new LandTarget(p, m_Mobile.Map);

				if (m_Mobile.InLOS(lt) && m_Mobile.Map.CanSpawnMobile(px + x, py + y, lt.Z) &&
					!SpellHelper.CheckMulti(p, m_Mobile.Map))
				{
					m_Mobile.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
					m_Mobile.PlaySound(0x1FE);

					m_Mobile.Location = new Point3D(lt.X, lt.Y, lt.Z);
					m_Mobile.ProcessDelta();

					return;
				}
			}

			return;
		}

		private void HideSelf()
		{
			if (Core.TickCount >= m_Mobile.NextSkillTime)
			{
				Effects.SendLocationParticles(
					EffectItem.Create(m_Mobile.Location, m_Mobile.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);

				m_Mobile.PlaySound(0x22F);
				m_Mobile.Hidden = true;

				m_Mobile.UseSkill(SkillName.Stealth);
			}
		}

		private void PerformHide()
		{
			if (!m_Mobile.Alive || m_Mobile.Deleted)
			{
				return;
			}

			if (!m_Mobile.Hidden)
			{
				double chance = 0.05;

				if (m_Mobile.Hits < 20)
				{
					chance = 0.1;
				}

				if (m_Mobile.Poisoned)
				{
					chance = 0.01;
				}

				if (Utility.RandomDouble() < chance)
				{
					HideSelf();
				}
			}
		}
	}
}