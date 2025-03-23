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
using System.Linq;

using Server;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public enum PvPBattleState
	{
		Internal,
		Queueing,
		Preparing,
		Running,
		Ended
	}

	public abstract partial class PvPBattle
	{
		private bool _StateTransition;

		private PvPBattleState _State = PvPBattleState.Internal;

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleState State
		{
			get => _State;
			set
			{
				if (_State == value)
				{
					return;
				}

				var oldState = _State;

				_State = value;

				if (!Deserializing)
				{
					OnStateChanged(oldState);
				}
			}
		}

		[CommandProperty(AutoPvP.Access, true)]
		public virtual PvPBattleState LastState { get; private set; }

		[CommandProperty(AutoPvP.Access, true)]
		public virtual DateTime LastStateChange { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public bool IsInternal => State == PvPBattleState.Internal;

		[CommandProperty(AutoPvP.Access)]
		public bool IsQueueing => State == PvPBattleState.Queueing;

		[CommandProperty(AutoPvP.Access)]
		public bool IsPreparing => State == PvPBattleState.Preparing;

		[CommandProperty(AutoPvP.Access)]
		public bool IsRunning => State == PvPBattleState.Running;

		[CommandProperty(AutoPvP.Access)]
		public bool IsEnded => State == PvPBattleState.Ended;

		public void InvalidateState()
		{
			if (_StateTransition || Deleted || Hidden || IsInternal)
			{
				return;
			}

			var state = State;
			var reset = false;

			switch (state)
			{
				case PvPBattleState.Queueing:
				{
					if (CanPrepareBattle(ref reset))
					{
						state = PvPBattleState.Preparing;
					}
				}
				break;
				case PvPBattleState.Preparing:
				{
					if (CanStartBattle(ref reset))
					{
						state = PvPBattleState.Running;
					}
					else if (!reset && GetStateTimeLeft(state) <= TimeSpan.Zero)
					{
						state = PvPBattleState.Ended;
					}
				}
				break;
				case PvPBattleState.Running:
				{
					if (CanEndBattle(ref reset))
					{
						state = PvPBattleState.Ended;
					}
				}
				break;
				case PvPBattleState.Ended:
				{
					if (CanOpenBattle(ref reset))
					{
						state = PvPBattleState.Queueing;
					}
				}
				break;
			}

			if (reset)
			{
				Options.Timing.SetTime(State, DateTime.UtcNow);
			}

			State = state;
		}

		protected virtual void OnStateChanged(PvPBattleState oldState)
		{
			_StateTransition = true;

			LastState = oldState;

			if (IsInternal)
			{
				Options.Timing.SetAllTimes(DateTime.MinValue);

				OnBattleInternalized();
			}
			else
			{
				if (LastState == PvPBattleState.Internal)
				{
					InvalidateRegions();
				}

				Options.Timing.SetTime(State, DateTime.UtcNow);

				switch (State)
				{
					case PvPBattleState.Queueing:
						OnBattleOpened();
						break;
					case PvPBattleState.Preparing:
						OnBattlePreparing();
						break;
					case PvPBattleState.Running:
						OnBattleStarted();
						break;
					case PvPBattleState.Ended:
						OnBattleEnded();
						break;
				}
			}

			LastStateChange = DateTime.UtcNow;

			_StateTransition = false;

			AutoPvP.InvokeBattleStateChanged(this);

			PvPBattlesUI.RefreshAll(this);
		}

		protected bool CanOpenBattle(ref bool timeReset)
		{
			if (!IsEnded)
			{
				return false;
			}

			if (GetStateTimeLeft(PvPBattleState.Ended) > TimeSpan.Zero)
			{
				return false;
			}

			return true;
		}

		protected bool CanPrepareBattle(ref bool timeReset)
		{
			if (!IsQueueing)
			{
				return false;
			}

			if (GetStateTimeLeft(PvPBattleState.Queueing) > TimeSpan.Zero)
			{
				return false;
			}

			if (!RequireCapacity || Queue.Count >= MinCapacity)
			{
				return true;
			}

			if (Schedule == null || !Schedule.Enabled || Schedule.NextGlobalTick == null)
			{
				timeReset = true;
			}

			return false;
		}

		protected bool CanStartBattle(ref bool timeReset)
		{
			if (!IsPreparing)
			{
				return false;
			}

			if (GetStateTimeLeft(PvPBattleState.Preparing) > TimeSpan.Zero)
			{
				return false;
			}

			if (Teams.All(t => t.IsReady()))
			{
				return true;
			}

			if (Schedule == null || !Schedule.Enabled || Schedule.NextGlobalTick == null)
			{
				timeReset = true;
			}

			return false;
		}

		protected bool CanEndBattle(ref bool timeReset)
		{
			if (!IsRunning)
			{
				return false;
			}

			if (GetStateTimeLeft(PvPBattleState.Running) <= TimeSpan.Zero)
			{
				return true;
			}

			if (!InviteWhileRunning && !HasCapacity())
			{
				return true;
			}

			if (Teams.All(t => !t.RespawnOnDeath && !t.IsAlive))
			{
				return true;
			}

			if (CheckMissions())
			{
				return true;
			}

			return false;
		}

		protected virtual void OnBattleInternalized()
		{
			if (LastState == PvPBattleState.Running)
			{
				OnBattleCancelled();
			}

			Reset();

			foreach (var p in AutoPvP.Profiles.Values.Where(p => p != null && !p.Deleted && p.IsSubscribed(this)))
			{
				p.Unsubscribe(this);
			}

			PvPBattlesUI.RefreshAll(this);
		}

		protected virtual void OnBattleOpened()
		{
			Hidden = false;

			if (Options.Broadcasts.World.OpenNotify)
			{
				WorldBroadcast("{0} is queueing volunteers!", Name);
			}

			if (Options.Broadcasts.Local.OpenNotify)
			{
				LocalBroadcast("{0} is queueing volunteers!", Name);
			}

			SendGlobalSound(Options.Sounds.BattleOpened);

			Reset();

			ForEachTeam(t => t.OnBattleOpened());

			if (LastState == PvPBattleState.Internal)
			{
				foreach (var p in AutoPvP.Profiles.Values.Where(p => p != null && !p.Deleted && !p.IsSubscribed(this)))
				{
					p.Subscribe(this);
				}
			}

			PvPBattlesUI.RefreshAll(this);
		}

		protected virtual void OnBattlePreparing()
		{
			Hidden = false;

			if (Options.Broadcasts.World.StartNotify)
			{
				WorldBroadcast("{0} is preparing!", Name);
			}

			if (Options.Broadcasts.Local.StartNotify)
			{
				LocalBroadcast("{0} is preparing!", Name);
			}

			SendGlobalSound(Options.Sounds.BattlePreparing);

			ForEachTeam(t => t.OnBattlePreparing());

			PvPBattlesUI.RefreshAll(this);
		}

		protected virtual void OnBattleStarted()
		{
			Hidden = false;

			if (Options.Broadcasts.World.StartNotify)
			{
				WorldBroadcast("{0} has begun!", Name);
			}

			if (Options.Broadcasts.Local.StartNotify)
			{
				LocalBroadcast("{0} has begun!", Name);
			}

			SendGlobalSound(Options.Sounds.BattleStarted);

			OpendDoors(true);

			ForEachTeam(t => t.OnBattleStarted());

			PvPBattlesUI.RefreshAll(this);
		}

		protected virtual void OnBattleEnded()
		{
			if (LastState == PvPBattleState.Preparing)
			{
				OnBattleCancelled();
			}

			Hidden = false;

			if (Options.Broadcasts.World.EndNotify)
			{
				WorldBroadcast("{0} has ended!", Name);
			}

			if (Options.Broadcasts.Local.EndNotify)
			{
				LocalBroadcast("{0} has ended!", Name);
			}

			SendGlobalSound(Options.Sounds.BattleEnded);

			CloseDoors(true);

			ProcessRanks();

			ForEachTeam(t => t.OnBattleEnded());

			TransferStatistics();

			PvPBattlesUI.RefreshAll(this);
		}

		protected virtual void OnBattleCancelled()
		{
			Hidden = false;

			if (Options.Broadcasts.World.EndNotify)
			{
				WorldBroadcast("{0} has been cancelled!", Name);
			}

			if (Options.Broadcasts.Local.EndNotify)
			{
				LocalBroadcast("{0} has been cancelled!", Name);
			}

			SendGlobalSound(Options.Sounds.BattleCanceled);

			CloseDoors(true);

			if (!Deleted && !IsInternal && QueueAllowed)
			{
				ForEachTeam(t => t.ForEachMember(o => Queue[o] = t));
			}

			ForEachTeam(t => t.OnBattleCancelled());
		}

		public TimeSpan GetStateTimeLeft()
		{
			return GetStateTimeLeft(DateTime.UtcNow);
		}

		public TimeSpan GetStateTimeLeft(DateTime when)
		{
			return GetStateTimeLeft(when, State);
		}

		public TimeSpan GetStateTimeLeft(PvPBattleState state)
		{
			return GetStateTimeLeft(DateTime.UtcNow, state);
		}

		public virtual TimeSpan GetStateTimeLeft(DateTime when, PvPBattleState state)
		{
			var time = 0.0;

			switch (state)
			{
				case PvPBattleState.Queueing:
				{
					time = (Options.Timing.OpenedWhen.Add(Options.Timing.QueuePeriod) - when).TotalSeconds;

					if (Schedule != null && Schedule.Enabled)
					{
						if (Schedule.CurrentGlobalTick != null)
						{
							time = (Schedule.CurrentGlobalTick.Value - when).TotalSeconds;
						}
						else if (Schedule.NextGlobalTick != null)
						{
							time = (Schedule.NextGlobalTick.Value - when).TotalSeconds;
						}
					}
				}
				break;
				case PvPBattleState.Preparing:
					time = (Options.Timing.PreparedWhen.Add(Options.Timing.PreparePeriod) - when).TotalSeconds;
					break;
				case PvPBattleState.Running:
					time = (Options.Timing.StartedWhen.Add(Options.Timing.RunningPeriod) - when).TotalSeconds;
					break;
				case PvPBattleState.Ended:
					time = (Options.Timing.EndedWhen.Add(Options.Timing.EndedPeriod) - when).TotalSeconds;
					break;
			}

			if (time > 0)
			{
				return TimeSpan.FromSeconds(time);
			}

			return TimeSpan.Zero;
		}
	}
}