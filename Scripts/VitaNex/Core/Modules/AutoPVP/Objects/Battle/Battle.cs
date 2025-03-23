#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

using VitaNex.Schedules;
using VitaNex.SuperGumps;
using VitaNex.Text;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	[PropertyObject]
	public abstract partial class PvPBattle : IEquatable<PvPBattle>, IComparable<PvPBattle>
	{
		private static readonly FieldInfo _DoorTimerField = ResolveDoorTimerField();

		private static FieldInfo ResolveDoorTimerField()
		{
			var t = typeof(BaseDoor);

			return t.GetField("m_Timer", BindingFlags.Instance | BindingFlags.NonPublic)
				?? t.GetField("_Timer", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		private static void ForEachBattle<T>(Action<PvPBattle, T> a, T s)
		{
			foreach (var o in AutoPvP.Battles.Values)
			{
				a(o, s);
			}
		}

		private static void OnShutdown(ShutdownEventArgs e)
		{
			ForEachBattle((o, s) => o.ServerShutdownHandler(s), e);
		}

		private static void OnLogin(LoginEventArgs e)
		{
			ForEachBattle((o, s) => o.LoginHandler(s), e);
		}

		private static void OnLogout(LogoutEventArgs e)
		{
			ForEachBattle((o, s) => o.LogoutHandler(s), e);
		}

		private static void OnCheckEquipItem(CheckEquipItemEventArgs e)
		{
			if (e.Block || !(e.Mobile is PlayerMobile))
			{
				return;
			}

			var m = (PlayerMobile)e.Mobile;
			var b = AutoPvP.FindBattle(m);

			if (b != null && b.IsParticipant(m) && !b.CanUseItem(m, e.Item, e.Message))
			{
				e.Block = true;
			}
		}

		public static void Bind()
		{
			Unbind();

			EventSink.Shutdown += OnShutdown;
			EventSink.Login += OnLogin;
			EventSink.Logout += OnLogout;
			EventSink.CheckEquipItem += OnCheckEquipItem;
		}

		public static void Unbind()
		{
			EventSink.Shutdown -= OnShutdown;
			EventSink.Login -= OnLogin;
			EventSink.Logout -= OnLogout;
			EventSink.CheckEquipItem -= OnCheckEquipItem;
		}

		private int _CoreTicks;
		private PollTimer _CoreTimer;

		[CommandProperty(AutoPvP.Access, true)]
		public bool Initialized { get; private set; }

		[CommandProperty(AutoPvP.Access, true)]
		public bool Deleted { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool DebugMode { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool Hidden { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual Schedule Schedule { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleOptions Options { get; set; }

		private string _Name;

		[CommandProperty(AutoPvP.Access)]
		public virtual string Name
		{
			get => _Name;
			set
			{
				_Name = value;

				if (Schedule != null && _Name != null)
				{
					Schedule.Name = _Name;
				}

				if (!Deserializing)
				{
					InvalidateRegions();
				}
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual string Shortcut { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual string Category { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual string Description { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPSpectatorGate Gate { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public List<BaseDoor> Doors { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual Map Map { get => Options.Locations.Map; set => Options.Locations.Map = value; }

		protected virtual void EnsureConstructDefaults()
		{
			Queue = new Dictionary<PlayerMobile, PvPTeam>();
			SubCommandHandlers = new Dictionary<string, PvPBattleCommandInfo>();
			BounceInfo = new Dictionary<PlayerMobile, MapPoint>();

			Spectators = new List<PlayerMobile>();
			Teams = new List<PvPTeam>();
			Doors = new List<BaseDoor>();
		}

		public PvPBattle()
			: this(false)
		{
			Serial = new PvPSerial();
			Options = new PvPBattleOptions();
			Schedule = new Schedule("PvP Battle " + Serial, false);

			AutoAssign = true;
			UseTeamColors = true;

			Name = "PvP Battle";
			Description = "PvP Battle";
			Category = "Misc";

			IdleKick = true;
			IdleThreshold = TimeSpan.FromSeconds(30.0);

			PointsBase = 1;
			KillPoints = 1;

			QueueAllowed = true;
			SpectateAllowed = false;
		}

		public void Init()
		{
			if (Initialized)
			{
				return;
			}

			InvalidateRegions();
			RegisterSubCommands();

			if (_CoreTimer != null)
			{
				_CoreTimer.Callback = OnCoreTick;
				_CoreTimer.Condition = () => Initialized && !_StateTransition;
			}
			else
			{
				_CoreTimer = PollTimer.FromSeconds(1.0, OnCoreTick, () => Initialized && !_StateTransition);
			}

			Schedule.OnGlobalTick += OnScheduleTick;

			ForEachTeam(t => t.Init());

			OnInit();

			Initialized = true;

			if (!Validate())
			{
				State = PvPBattleState.Internal;
			}
		}

		protected virtual void OnCoreTick()
		{
			++_CoreTicks;

			MicroSync();
			InvalidateState();

			if (IsInternal || Hidden)
			{
				return;
			}

			BroadcastStateHandler();
			SuddenDeathHandler();

			if (CanUseWeather())
			{
				WeatherCycle();
			}

			if (_CoreTicks % 5 == 0 && !IsInternal)
			{
				if (CanSendInvites())
				{
					SendInvites();
				}
			}

			if (_CoreTicks % 10 == 0)
			{
				Sync();
			}
		}

		protected virtual void OnScheduleTick(Schedule schedule)
		{
			if (!Deleted && !Hidden && IsQueueing)
			{
				Timer.DelayCall(InvalidateState);
			}
		}

		protected virtual void ServerShutdownHandler(ShutdownEventArgs e)
		{
			Reset();
		}

		protected virtual void LogoutHandler(LogoutEventArgs e)
		{
			if (e == null || e.Mobile == null || e.Mobile.Deleted || e.Mobile.Region == null ||
				(!e.Mobile.InRegion(BattleRegion) && !e.Mobile.InRegion(SpectateRegion)))
			{
				return;
			}

			var pm = e.Mobile as PlayerMobile;

			if (pm == null)
			{
				return;
			}

			if (IsQueued(pm))
			{
				Dequeue(pm);
			}

			if (IsParticipant(pm) || IsSpectator(pm))
			{
				Timer.DelayCall(
					pm.GetLogoutDelay(),
					m =>
					{
						if (!IsOnline(m))
						{
							Quit(m, true);
						}
					},
					pm);
			}
		}

		protected virtual void LoginHandler(LoginEventArgs e)
		{
			if (e != null && e.Mobile != null && !e.Mobile.Deleted && e.Mobile.Region != null &&
				(e.Mobile.InRegion(BattleRegion) || e.Mobile.InRegion(SpectateRegion)))
			{
				InvalidateStray(e.Mobile);
			}
		}

		public void Sync()
		{
			ForEachTeam(t => t.Sync());

			if (Schedule != null && Schedule.Enabled)
			{
				Schedule.InvalidateNextTick();
			}

			OnSync();
		}

		public void MicroSync()
		{
			if (!Validate())
			{
				State = PvPBattleState.Internal;
				return;
			}

			if (BattleRegion != null)
			{
				BattleRegion.MicroSync();
			}

			if (SpectateRegion != null)
			{
				SpectateRegion.MicroSync();
			}

			ForEachTeam(t => t.MicroSync());

			OnMicroSync();
		}

		public void Reset()
		{
			OnReset();

			ForEachTeam(ResetTeam);
		}

		public void Delete()
		{
			if (Deleted)
			{
				return;
			}

			Reset();

			if (_CoreTimer != null)
			{
				_CoreTimer.Dispose();
				_CoreTimer = null;
			}

			_CoreTicks = 0;

			if (Gate != null)
			{
				Gate.Delete();
				Gate = null;
			}

			ForEachTeam(t => t.Delete());

			if (Schedule != null)
			{
				Schedule.Stop();
				Schedule.Enabled = false;
				Schedule.OnGlobalTick -= OnScheduleTick;
				Schedule = null;
			}

			if (_BattleRegion != null)
			{
				_BattleRegion.ClearPreview();
				_BattleRegion.Unregister();
				_BattleRegion = null;
			}

			if (_SpectateRegion != null)
			{
				_SpectateRegion.ClearPreview();
				_SpectateRegion.Unregister();
				_SpectateRegion = null;
			}

			if (Options != null)
			{
				Options.Clear();
			}

			OnDeleted();

			if (AutoPvP.RemoveBattle(this))
			{
				OnRemoved();
			}

			Deleted = true;
		}

		public virtual void ToggleDoors(bool secure)
		{
			Doors.ForEachReverse(
				d =>
				{
					if (d == null || d.Deleted || d.Map != Map)
					{
						Doors.Remove(d);
						return;
					}

					if ((!d.Open || !CanCloseDoor(d)) && (d.Open || !CanOpenDoor(d)))
					{
						return;
					}

					d.Open = !d.Open;
					d.Locked = secure;

					if (_DoorTimerField == null)
					{
						return;
					}

					var t = _DoorTimerField.GetValue(d) as Timer;

					if (t != null)
					{
						t.Stop();
					}
				});
		}

		public virtual void ToggleDoors(bool secure, bool open)
		{
			Doors.ForEachReverse(
				d =>
				{
					if (d == null || d.Deleted || d.Map != Map)
					{
						Doors.Remove(d);
						return;
					}

					if ((!d.Open || !CanCloseDoor(d)) && (d.Open || !CanOpenDoor(d)))
					{
						return;
					}

					d.Open = open;
					d.Locked = secure;

					if (_DoorTimerField == null)
					{
						return;
					}

					var t = _DoorTimerField.GetValue(d) as Timer;

					if (t != null)
					{
						t.Stop();
					}
				});
		}

		public virtual void OpendDoors(bool secure)
		{
			ToggleDoors(secure, true);
		}

		public virtual void CloseDoors(bool secure)
		{
			ToggleDoors(secure, false);
		}

		public virtual bool CanOpenDoor(BaseDoor door)
		{
			return door != null && !door.Deleted;
		}

		public virtual bool CanCloseDoor(BaseDoor door)
		{
			return door != null && !door.Deleted && door.CanClose();
		}

		public virtual bool CheckSuddenDeath()
		{
			if (!Options.SuddenDeath.Enabled || !IsRunning || Hidden)
			{
				return false;
			}

			if (Teams.Count > 1 && CountAliveTeams() > 1)
			{
				return CurrentCapacity <= Options.SuddenDeath.CapacityRequired;
			}

			return CurrentCapacity > 1 && CurrentCapacity <= Options.SuddenDeath.CapacityRequired;
		}

		protected virtual void SuddenDeathHandler()
		{
			if (!CheckSuddenDeath())
			{
				if (Options.SuddenDeath.Active)
				{
					Options.SuddenDeath.End();
					OnSuddenDeathEnd();
				}
			}
			else if (!Options.SuddenDeath.Active)
			{
				Options.SuddenDeath.Start();
				OnSuddenDeathStart();
			}
		}

		protected virtual void OnSuddenDeathStart()
		{
			LocalBroadcast("Sudden death! Prepare for the worst!");
		}

		protected virtual void OnSuddenDeathEnd()
		{
			LocalBroadcast("Sudden death has ended.");
		}

		public bool IsOnline(PlayerMobile pm)
		{
			return pm != null && pm.IsOnline();
		}

		public bool InCombat(PlayerMobile pm)
		{
			return pm != null && pm.InCombat();
		}

		protected virtual void OnReset()
		{ }

		protected virtual void OnDeleted()
		{ }

		protected virtual void OnInit()
		{ }

		protected virtual void OnSync()
		{ }

		protected virtual void OnMicroSync()
		{
			if (Validate())
			{
				InvalidateGates();
			}
		}

		protected virtual void OnRemoved()
		{ }

		protected virtual void OnRewarded(PlayerMobile pm, IEntity reward)
		{
			if (pm != null && !pm.Deleted && reward != null)
			{
				pm.SendMessage("You have been rewarded for your efforts in {0}.", Name);
			}
		}

		protected virtual void OnWin(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			UpdateStatistics(
				FindTeam(pm),
				pm,
				s =>
				{
					++s.Battles;
					++s.Wins;
				});

			GiveWinnerReward(pm);

			AwardPoints(pm);
		}

		protected virtual void OnLose(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			UpdateStatistics(
				FindTeam(pm),
				pm,
				s =>
				{
					++s.Battles;
					++s.Losses;
				});

			GiveLoserReward(pm);

			RevokePoints(pm);
		}

		public virtual void GiveWinnerReward(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			var rewards = Options.Rewards.Winner.GiveReward(pm);

			if (rewards != null)
			{
				rewards.ForEach(reward => OnRewarded(pm, reward));
			}
		}

		public virtual void GiveLoserReward(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			var rewards = Options.Rewards.Loser.GiveReward(pm);

			if (rewards != null)
			{
				rewards.ForEach(reward => OnRewarded(pm, reward));
			}
		}

		public virtual void SetRestrictedPets(Dictionary<Type, bool> list)
		{
			foreach (var kvp in list)
			{
				Options.Restrictions.Pets.SetRestricted(kvp.Key, kvp.Value);
			}
		}

		public virtual void SetRestrictedItems(Dictionary<Type, bool> list)
		{
			foreach (var kvp in list)
			{
				Options.Restrictions.Pets.SetRestricted(kvp.Key, kvp.Value);
			}
		}

		public virtual void SetRestrictedSpells(Dictionary<Type, bool> list)
		{
			foreach (var kvp in list)
			{
				Options.Restrictions.Spells.SetRestricted(kvp.Key, kvp.Value);
			}
		}

		public virtual void SetRestrictedSkills(Dictionary<SkillName, bool> list)
		{
			foreach (var kvp in list)
			{
				Options.Restrictions.Skills.SetRestricted(kvp.Key, kvp.Value);
			}
		}

		protected bool CheckMissions()
		{
			return CheckMissions(out var team, out var player) && (team != null || player != null);
		}

		protected bool CheckMissions(out PvPTeam team, out PlayerMobile player)
		{
			team = null;
			player = null;

			var i = Teams.Count;

			while (--i >= 0)
			{
				if (!Teams.InBounds(i))
				{
					continue;
				}

				var t = Teams[i];

				if (t == null || t.Deleted)
				{
					continue;
				}

				if (CheckMissions(t))
				{
					team = t;
					return true;
				}

				var p = t.Members.OrderByDescending(o => o.Value).Select(o => o.Key).FirstOrDefault(CheckMissions);

				if (p != null)
				{
					player = p;
					return true;
				}
			}

			return false;
		}

		protected virtual bool CheckMissions(PvPTeam team)
		{
			return Options.Missions.Completed(team);
		}

		protected virtual bool CheckMissions(PlayerMobile player)
		{
			return Options.Missions.Completed(this, player);
		}

		public virtual void RefreshStats(PlayerMobile pm)
		{
			RefreshStats(pm, true, true);
		}

		public virtual void RefreshStats(PlayerMobile pm, bool negate, bool resurrect)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			if (!pm.Alive && resurrect)
			{
				pm.Resurrect();
			}

			if (negate)
			{
				Negate(pm);
			}

			if (!pm.Alive)
			{
				return;
			}

			pm.Hits = pm.HitsMax;
			pm.Stam = pm.StamMax;
			pm.Mana = pm.ManaMax;
		}

		public virtual void Negate(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			SpellUtility.NegateAllEffects(m);

			if (DebugMode || m.AccessLevel <= AccessLevel.Counselor)
			{
				m.RevealingAction();
				m.DisruptiveAction();
			}

			if (m.Target != null)
			{
				m.Target.Cancel(m, TargetCancelType.Overriden);
			}

			m.Spell = null;

			if (m.Combatant != null)
			{
				if (m.Combatant is Mobile c && c.Combatant == m)
				{
					c.Combatant = null;
					c.Warmode = false;
				}

				m.Combatant = null;
			}

			if (m.Aggressed != null)
			{
				m.Aggressed.Clear();
			}

			if (m.Aggressors != null)
			{
				m.Aggressors.Clear();
			}

			if (m.Warmode && !m.InRegion(BattleRegion))
			{
				m.Warmode = false;
			}

			m.Criminal = false;

			m.Delta(MobileDelta.Noto);

			if ((DebugMode || m.AccessLevel <= AccessLevel.Counselor) && m.InRegion(BattleRegion))
			{
				m.Items.ForEachReverse(o => InvalidateItem(m, o));
			}
		}

		public virtual void InvalidateItem(Mobile m, Item item)
		{
			if (m == null || !m.Player || item == null || item.Deleted || item == m.FindBankNoCreate())
			{
				return;
			}

			if (!DebugMode && m.AccessLevel > AccessLevel.Counselor)
			{
				return;
			}

			if (item is Container)
			{
				if (!item.Items.IsNullOrEmpty())
				{
					item.Items.ForEachReverse(o => InvalidateItem(m, o));
				}

				return;
			}

			if (Options.Restrictions.Skills.IsRestricted(SkillName.Poisoning))
			{
				if (item.Layer.IsEquip() && !item.Layer.IsPackOrBank() && !item.Layer.IsMount())
				{
					if (item.GetPropertyValue("Poison", out Poison p) && p != null)
					{
						if (item.SetPropertyValue<Poison>("Poison", null))
						{
							m.SendMessage("The poison on your {0} has been removed.", item.ResolveName(m));
						}
					}
				}
			}

			if (CanUseItem(m, item, false))
			{
				return;
			}

			if (item is EtherealMount)
			{
				var em = (EtherealMount)item;

				if (em.Rider == m)
				{
					em.UnmountMe();
				}
			}
			else if (item.Movable && item.IsEquippedBy(m))
			{
				m.Backpack.DropItem(item);
			}
		}

		public virtual bool CanUseItem(Mobile m, Item item, bool message)
		{
			if (m == null || item == null || item.Deleted)
			{
				return false;
			}

			var type = item.GetType();

			if (type.HasInterface("IShrinkItem"))
			{
				return item.GetPropertyValue("Link", out BaseCreature link) && CanUseMobile(m, link, message);
			}

			if ((!DebugMode && m.AccessLevel >= AccessLevel.Counselor) || IsInternal || Hidden)
			{
				return true;
			}

			if (!IsParticipant(m as PlayerMobile))
			{
				return true;
			}

			if (item is EtherealMount && (!Options.Rules.CanMountEthereal || !Options.Rules.CanMount))
			{
				if (message)
				{
					m.SendMessage("You are not allowed to ride a mount in this battle.");
				}

				return false;
			}

			if (Options.Restrictions.Items.IsRestricted(item))
			{
				if (message)
				{
					m.SendMessage("You can not use that in this battle.");
				}

				return false;
			}

			if (!Options.Restrictions.Items.AllowNonExceptional)
			{
				if (item.GetPropertyValue("Quality", out int quality) && quality < 2)
				{
					if (message)
					{
						m.SendMessage("You can only use exceptional items in this battle.");
					}

					return false;
				}
			}

			return true;
		}

		public virtual bool CanUseMobile(Mobile m, Mobile target, bool message)
		{
			if (m == null || target == null || target.Deleted)
			{
				return false;
			}

			if ((!DebugMode && m.AccessLevel >= AccessLevel.Counselor) || IsInternal || Hidden)
			{
				return true;
			}

			if (!IsParticipant(m as PlayerMobile))
			{
				return true;
			}

			if (target.IsControlledBy(m))
			{
				if (target is BaseMount && !Options.Rules.CanMount)
				{
					if (message)
					{
						m.SendMessage("You are not allowed to ride a mount in this battle.");
					}

					return false;
				}

				if (target is BaseCreature && Options.Restrictions.Pets.IsRestricted((BaseCreature)target))
				{
					if (message)
					{
						m.SendMessage("You can not use that in this battle.");
					}

					return false;
				}
			}

			return true;
		}

		public virtual bool CheckAccessibility(Item item, Mobile m)
		{
			return true;
		}

		public virtual bool OnDecay(Item item)
		{
			return true;
		}

		public virtual void SpellDamageScalar(Mobile caster, Mobile target, ref double damage)
		{ }

		public virtual int CompareTo(PvPBattle b)
		{
			if (ReferenceEquals(this, b))
			{
				return 0;
			}

			var result = 0;

			if (this.CompareNull(b, ref result))
			{
				return result;
			}

			if (Deleted && b.Deleted)
			{
				return 0;
			}

			if (Deleted && !b.Deleted)
			{
				return 1;
			}

			if (!Deleted && b.Deleted)
			{
				return -1;
			}

			if (Hidden && b.Hidden)
			{
				return 0;
			}

			if (Hidden && !b.Hidden)
			{
				return 1;
			}

			if (!Hidden && b.Hidden)
			{
				return -1;
			}

			var x = GetStateTimeLeft();
			var y = b.GetStateTimeLeft();

			if (x < y)
			{
				return -1;
			}

			if (x > y)
			{
				return 1;
			}

			var l = GetCurrentCapacity();
			var r = b.GetCurrentCapacity();

			if (l < r)
			{
				return 1;
			}

			if (l > r)
			{
				return -1;
			}

			return 0;
		}

		public virtual void GetHtmlString(Mobile viewer, StringBuilder html, bool preview = false)
		{
			var col = SuperGump.DefaultHtmlColor;

			html.Append(String.Empty.WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));

			if (Deleted)
			{
				html.AppendLine("This battle no longer exists.".WrapUOHtmlColor(Color.IndianRed, col));
				return;
			}

			html.AppendLine("{0} ({1})", Name, Ranked ? "Ranked" : "Unranked");
			html.AppendLine("State: {0}".WrapUOHtmlColor(Color.SkyBlue, col), State.ToString().SpaceWords());

			if (viewer != null && viewer.AccessLevel >= AutoPvP.Access)
			{
				var errors = new List<string>();

				if (!Validate(viewer, errors))
				{
					html.AppendLine(UniGlyph.CircleX + " This battle has failed validation: ".WrapUOHtmlColor(Color.IndianRed, false));
					html.AppendLine(String.Empty.WrapUOHtmlColor(Color.Yellow, false));
					html.AppendLine(String.Join("\n", errors));
					html.AppendLine(String.Empty.WrapUOHtmlColor(col, false));
				}
			}

			int curCap = CurrentCapacity, minCap = MinCapacity, maxCap = MaxCapacity;

			if (!preview)
			{
				if (IsPreparing && RequireCapacity && curCap < minCap)
				{
					var req = minCap - curCap;

					var fmt = "{0} more {1} required to start the battle.".WrapUOHtmlColor(Color.IndianRed, col);

					html.AppendLine(fmt, req, req != 1 ? "players are" : "player is");
				}

				if (!IsInternal)
				{
					var timeLeft = GetStateTimeLeft(DateTime.UtcNow);

					if (timeLeft > TimeSpan.Zero)
					{
						var fmt = "Time Left: {0}".WrapUOHtmlColor(Color.LawnGreen, col);

						html.AppendLine(fmt, timeLeft.ToSimpleString("h:m:s"));
					}
				}

				html.AppendLine();
			}

			if (!String.IsNullOrWhiteSpace(Description))
			{
				html.AppendLine(Description.WrapUOHtmlColor(Color.SkyBlue, col));
				html.AppendLine();
			}

			if (Schedule != null && Schedule.Enabled)
			{
				html.AppendLine(
					Schedule.NextGlobalTick != null
						? "This battle is scheduled.".WrapUOHtmlColor(Color.LawnGreen, col)
						: "This battle is scheduled, but has no future dates.".WrapUOHtmlColor(Color.IndianRed, col));
			}
			else
			{
				html.AppendLine("This battle is automatic.".WrapUOHtmlColor(Color.LawnGreen, col));
			}

			html.AppendLine();

			if (!preview)
			{
				html.Append(String.Empty.WrapUOHtmlColor(Color.YellowGreen, false));

				var fmt = "{0:#,0} players in the queue.";

				html.AppendLine(fmt, Queue.Count);

				fmt = "{0:#,0} players in {1:#,0} team{2} attending.";

				html.AppendLine(fmt, curCap, Teams.Count, Teams.Count != 1 ? "s" : String.Empty);

				fmt = "{0:#,0} invites available of {1:#,0} max.";

				html.AppendLine(fmt, maxCap - curCap, maxCap);
				html.AppendLine(String.Empty.WrapUOHtmlColor(col, false));
			}

			if (Options.Missions.Enabled)
			{
				html.Append(String.Empty.WrapUOHtmlColor(Color.PaleGoldenrod, false));

				Options.Missions.GetHtmlString(html);

				html.AppendLine(String.Empty.WrapUOHtmlColor(col, false));
			}

			GetHtmlCommandList(viewer, html, preview);
		}

		public virtual void GetHtmlCommandList(Mobile viewer, StringBuilder html, bool preview = false)
		{
			if (SubCommandHandlers.Count <= 0)
			{
				return;
			}

			html.Append(String.Empty.WrapUOHtmlColor(Color.White, false));

			html.AppendLine("Commands".WrapUOHtmlBig());
			html.AppendLine("(For use in Battle or Spectate regions)".WrapUOHtmlBig());
			html.AppendLine();

			foreach (var i in SubCommandHandlers.Values.Where(i => viewer == null || viewer.AccessLevel >= i.Access)
												.OrderByNatural(i => i.Command))
			{
				html.AppendLine("{0}{1} {2}".WrapUOHtmlBig(), SubCommandPrefix, i.Command, i.Usage);

				if (String.IsNullOrWhiteSpace(i.Description))
				{
					html.AppendLine();
					continue;
				}

				html.AppendLine(i.Description.WrapUOHtmlColor(Color.SkyBlue, Color.White));
				html.AppendLine();
			}

			html.Append(String.Empty.WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));
		}

		public override int GetHashCode()
		{
			return Serial.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is PvPBattle && Equals((PvPBattle)obj);
		}

		public bool Equals(PvPBattle other)
		{
			return !ReferenceEquals(other, null) && Serial == other.Serial;
		}

		public override string ToString()
		{
			return Name ?? "PvP Battle";
		}

		public string ToHtmlString(Mobile viewer = null, bool preview = false)
		{
			var html = new StringBuilder();

			GetHtmlString(viewer, html, preview);

			return html.ToString();
		}

		public static bool operator ==(PvPBattle left, PvPBattle right)
		{
			return ReferenceEquals(null, left) ? ReferenceEquals(null, right) : left.Equals(right);
		}

		public static bool operator !=(PvPBattle left, PvPBattle right)
		{
			return ReferenceEquals(null, left) ? !ReferenceEquals(null, right) : !left.Equals(right);
		}
	}
}
