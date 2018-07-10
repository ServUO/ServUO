#region Header
// **********
// ServUO - GuardedRegion.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Xml;

using Server.Commands;
using Server.Mobiles;
#endregion

namespace Server.Regions
{
	public class GuardedRegion : BaseRegion
	{
		private static readonly object[] m_GuardParams = new object[1];
		private readonly Type m_GuardType;
		private readonly Dictionary<Mobile, GuardTimer> m_GuardCandidates = new Dictionary<Mobile, GuardTimer>();
		private bool m_Disabled;

		public GuardedRegion(string name, Map map, int priority, params Rectangle3D[] area)
			: base(name, map, priority, area)
		{
			m_GuardType = DefaultGuardType;
		}

		public GuardedRegion(string name, Map map, int priority, params Rectangle2D[] area)
			: base(name, map, priority, area)
		{
			m_GuardType = DefaultGuardType;
		}

		public GuardedRegion(XmlElement xml, Map map, Region parent)
			: base(xml, map, parent)
		{
			XmlElement el = xml["guards"];

			if (ReadType(el, "type", ref m_GuardType, false))
			{
				if (!typeof(Mobile).IsAssignableFrom(m_GuardType))
				{
					Console.WriteLine("Invalid guard type for region '{0}'", this);
					m_GuardType = DefaultGuardType;
				}
			}
			else
			{
				m_GuardType = DefaultGuardType;
			}

			bool disabled = false;
			if (ReadBoolean(el, "disabled", ref disabled, false))
			{
				Disabled = disabled;
			}
		}

		public bool Disabled { get { return m_Disabled; } set { m_Disabled = value; } }
		public virtual bool AllowReds { get { return Core.AOS; } }

		public virtual Type DefaultGuardType
		{
			get
			{
				if (Map == Map.Ilshenar || Map == Map.Malas)
				{
					return typeof(ArcherGuard);
				}
				else
				{
					return typeof(WarriorGuard);
				}
			}
		}

		public static void Initialize()
		{
			CommandSystem.Register("CheckGuarded", AccessLevel.GameMaster, CheckGuarded_OnCommand);
			CommandSystem.Register("SetGuarded", AccessLevel.Administrator, SetGuarded_OnCommand);
			CommandSystem.Register("ToggleGuarded", AccessLevel.Administrator, ToggleGuarded_OnCommand);
		}

		public static GuardedRegion Disable(GuardedRegion reg)
		{
			reg.Disabled = true;
			return reg;
		}

		public virtual bool IsDisabled()
		{
			return m_Disabled;
		}

		public virtual bool CheckVendorAccess(BaseVendor vendor, Mobile from)
		{
			if (from.AccessLevel >= AccessLevel.GameMaster || IsDisabled())
			{
				return true;
			}

			return (from.Kills < 5);
		}

		public override bool OnBeginSpellCast(Mobile m, ISpell s)
		{
			if (!IsDisabled() && !s.OnCastInTown(this))
			{
				m.SendLocalizedMessage(500946); // You cannot cast this in town!
				return false;
			}

			return base.OnBeginSpellCast(m, s);
		}

		public override bool AllowHousing(Mobile from, Point3D p)
		{
			return false;
		}

		public override void MakeGuard(Mobile focus)
		{
			BaseGuard useGuard = null;
            IPooledEnumerable eable = focus.GetMobilesInRange(8);

            foreach (Mobile m in eable)
			{
				if (m is BaseGuard)
				{
					BaseGuard g = (BaseGuard)m;

					if (g.Focus == null) // idling
					{
						useGuard = g;
						break;
					}
				}
			}

            eable.Free();

			if (useGuard == null)
			{
				m_GuardParams[0] = focus;

				try
				{
					Activator.CreateInstance(m_GuardType, m_GuardParams);
				}
				catch
				{ }
			}
			else
			{
				useGuard.Focus = focus;
			}
		}

		public override void OnEnter(Mobile m)
		{
			if (IsDisabled())
			{
				return;
			}

			if (!AllowReds && m.Murderer)
			{
				CheckGuardCandidate(m);
			}
		}

		public override void OnExit(Mobile m)
		{
			if (IsDisabled())
			{
				return;
			}
		}

		public override void OnSpeech(SpeechEventArgs args)
		{
			base.OnSpeech(args);

			if (IsDisabled())
			{
				return;
			}

			if (args.Mobile.Alive && args.HasKeyword(0x0007)) // *guards*
			{
				CallGuards(args.Mobile.Location);
			}
		}

		public override void OnAggressed(Mobile aggressor, Mobile aggressed, bool criminal)
		{
			base.OnAggressed(aggressor, aggressed, criminal);

			if (!IsDisabled() && aggressor != aggressed && criminal)
			{
				CheckGuardCandidate(aggressor, aggressor is BaseCreature && ((BaseCreature)aggressor).IsAggressiveMonster);
			}
		}

		public override void OnGotBeneficialAction(Mobile helper, Mobile helped)
		{
			base.OnGotBeneficialAction(helper, helped);

			if (IsDisabled() || Siege.SiegeShard)
			{
				return;
			}

			int noto = Notoriety.Compute(helper, helped);

			if (helper != helped && (noto == Notoriety.Criminal || noto == Notoriety.Murderer))
			{
				CheckGuardCandidate(helper);
			}
		}

		public override void OnCriminalAction(Mobile m, bool message)
		{
			base.OnCriminalAction(m, message);

			if (!IsDisabled())
			{
				CheckGuardCandidate(m);
			}
		}

        public void CheckGuardCandidate(Mobile m)
        {
            CheckGuardCandidate(m, false);
        }

		public void CheckGuardCandidate(Mobile m, bool autoCallGuards)
		{
			if (IsDisabled())
			{
				return;
			}

			if (IsGuardCandidate(m))
			{
				GuardTimer timer = null;
				m_GuardCandidates.TryGetValue(m, out timer);

                if (autoCallGuards)
                {
                    MakeGuard(m);

                    if (timer != null)
                    {
                        timer.Stop();
                        m_GuardCandidates.Remove(m);
                        m.SendLocalizedMessage(502276); // Guards can no longer be called on you.
                    }
                }
				else if (timer == null)
				{
					timer = new GuardTimer(m, m_GuardCandidates);
					timer.Start();

					m_GuardCandidates[m] = timer;
					m.SendLocalizedMessage(502275); // Guards can now be called on you!

					Map map = m.Map;

					if (map != null)
					{
						Mobile fakeCall = null;
						double prio = 0.0;

                        IPooledEnumerable eable = m.GetMobilesInRange(8);

                        foreach (Mobile v in eable)
						{
							if (!v.Player && v != m && !IsGuardCandidate(v) &&
								((v is BaseCreature) ? ((BaseCreature)v).IsHumanInTown() : (v.Body.IsHuman && v.Region.IsPartOf(this))))
							{
								double dist = m.GetDistanceToSqrt(v);

								if (fakeCall == null || dist < prio)
								{
									fakeCall = v;
									prio = dist;
								}
							}
						}

                        eable.Free();

						if (fakeCall != null)
						{
							fakeCall.Say(Utility.RandomList(1007037, 501603, 1013037, 1013038, 1013039, 1013041, 1013042, 1013043, 1013052));
							MakeGuard(m);
							timer.Stop();
							m_GuardCandidates.Remove(m);
							m.SendLocalizedMessage(502276); // Guards can no longer be called on you.
						}
					}
				}
				else
				{
					timer.Stop();
					timer.Start();
				}
			}
		}

		public void CallGuards(Point3D p)
		{
			if (IsDisabled())
			{
				return;
			}

			IPooledEnumerable eable = Map.GetMobilesInRange(p, 14);

			foreach (Mobile m in eable)
			{
				if (IsGuardCandidate(m))
				{
                    if (m_GuardCandidates.ContainsKey(m) || (!AllowReds && m.Murderer && m.Region.IsPartOf(this)))
                    {
                        GuardTimer timer = null;
                        m_GuardCandidates.TryGetValue(m, out timer);

                        if (timer != null)
                        {
                            timer.Stop();
                            m_GuardCandidates.Remove(m);
                        }

                        MakeGuard(m);
                        m.SendLocalizedMessage(502276); // Guards can no longer be called on you.
                    }
                    else if (m is BaseCreature && ((BaseCreature)m).IsAggressiveMonster && m.Region.IsPartOf(this))
                    {
                        MakeGuard(m);
                    }
					break;
				}
			}

			eable.Free();
		}

		public bool IsGuardCandidate(Mobile m)
		{
			if (m is BaseGuard || m.GuardImmune || !m.Alive || m.IsStaff() || m.Blessed || (m is BaseCreature && ((BaseCreature)m).IsInvulnerable) ||
				IsDisabled())
			{
				return false;
			}

			return (!AllowReds && m.Murderer) || m.Criminal || (m is BaseCreature && ((BaseCreature)m).IsAggressiveMonster);
		}

		[Usage("CheckGuarded")]
		[Description("Returns a value indicating if the current region is guarded or not.")]
		private static void CheckGuarded_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;
			GuardedRegion reg = (GuardedRegion)from.Region.GetRegion(typeof(GuardedRegion));

			if (reg == null)
			{
				from.SendMessage("You are not in a guardable region.");
			}
			else if (reg.Disabled)
			{
				from.SendMessage("The guards in this region have been disabled.");
			}
			else
			{
				from.SendMessage("This region is actively guarded.");
			}
		}

		[Usage("SetGuarded <true|false>")]
		[Description("Enables or disables guards for the current region.")]
		private static void SetGuarded_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;

			if (e.Length == 1)
			{
				GuardedRegion reg = (GuardedRegion)from.Region.GetRegion(typeof(GuardedRegion));

				if (reg == null)
				{
					from.SendMessage("You are not in a guardable region.");
				}
				else
				{
					reg.Disabled = !e.GetBoolean(0);

					if (reg.Disabled)
					{
						from.SendMessage("The guards in this region have been disabled.");
					}
					else
					{
						from.SendMessage("The guards in this region have been enabled.");
					}
				}
			}
			else
			{
				from.SendMessage("Format: SetGuarded <true|false>");
			}
		}

		[Usage("ToggleGuarded")]
		[Description("Toggles the state of guards for the current region.")]
		private static void ToggleGuarded_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;
			GuardedRegion reg = (GuardedRegion)from.Region.GetRegion(typeof(GuardedRegion));

			if (reg == null)
			{
				from.SendMessage("You are not in a guardable region.");
			}
			else
			{
				reg.Disabled = !reg.Disabled;

				if (reg.Disabled)
				{
					from.SendMessage("The guards in this region have been disabled.");
				}
				else
				{
					from.SendMessage("The guards in this region have been enabled.");
				}
			}
		}

		private class GuardTimer : Timer
		{
			private readonly Mobile m_Mobile;
			private readonly Dictionary<Mobile, GuardTimer> m_Table;

			public GuardTimer(Mobile m, Dictionary<Mobile, GuardTimer> table)
				: base(TimeSpan.FromSeconds(15.0))
			{
				Priority = TimerPriority.TwoFiftyMS;

				m_Mobile = m;
				m_Table = table;
			}

			protected override void OnTick()
			{
				if (m_Table.ContainsKey(m_Mobile))
				{
					m_Table.Remove(m_Mobile);
					m_Mobile.SendLocalizedMessage(502276); // Guards can no longer be called on you.
				}
			}
		}
	}
}