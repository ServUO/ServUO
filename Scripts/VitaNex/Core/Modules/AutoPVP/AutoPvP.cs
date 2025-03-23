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
using System.Linq;

using Server;
using Server.Accounting;
using Server.Mobiles;

using VitaNex.IO;
using VitaNex.Schedules;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public delegate void SeasonChangedHandler(PvPSeason newSeason, PvPSeason oldSeason);

	public static partial class AutoPvP
	{
		public const AccessLevel Access = AccessLevel.Seer;

		public static ScheduleInfo DefaultSeasonSchedule = new ScheduleInfo(ScheduleMonths.All, ScheduleDays.Monday, ScheduleTimes.Midnight);

		public static AutoPvPOptions CMOptions { get; private set; }

		public static BinaryDataStore<int, PvPSeason> Seasons { get; private set; }
		public static BinaryDataStore<PlayerMobile, PvPProfile> Profiles { get; private set; }
		public static BinaryDirectoryDataStore<PvPSerial, PvPBattle> Battles { get; private set; }

		public static Dictionary<IAccount, Timer> Deserters { get; private set; }

		public static Type[] BattleTypes { get; set; }
		public static PvPScenario[] Scenarios { get; set; }

		public static Schedule SeasonSchedule { get; set; }

		public static PvPSeason CurrentSeason => EnsureSeason(CMOptions.Advanced.Seasons.CurrentSeason);
		public static DateTime NextSeasonTime => SeasonSchedule.NextGlobalTick ?? DateTime.UtcNow;

		private static DateTime LastSort { get; set; }
		private static TimeSpan CacheDelay { get; set; }
		private static List<PvPProfile> CachedSort { get; set; }

		public static event Action<PvPSeason> OnSeasonChanged;

		public static event Action<PvPBattle, PvPTeam, PlayerMobile> OnQueueJoin;
		public static event Action<PvPBattle, PvPTeam, PlayerMobile> OnQueueLeave;
		public static event Action<PvPBattle, PvPTeam, PlayerMobile> OnQueueUpdate;

		public static event Action<PvPBattle, PvPRegion, Mobile> OnEnterBattle;
		public static event Action<PvPBattle, PvPRegion, Mobile> OnExitBattle;

		public static event Action<PvPBattle, string> OnBattleLocalBroadcast;
		public static event Action<PvPBattle, string> OnBattleWorldBroadcast;

		public static event Action<PvPBattle> OnBattleStateChanged;

		public static void ChangeSeason(Schedule schedule)
		{
			if (!CMOptions.ModuleEnabled)
			{
				return;
			}

			if (CMOptions.Advanced.Seasons.SkippedTicks < CMOptions.Advanced.Seasons.SkipTicks)
			{
				++CMOptions.Advanced.Seasons.SkippedTicks;
				return;
			}

			CMOptions.Advanced.Seasons.SkippedTicks = 0;

			var old = CurrentSeason;

			EnsureSeason(++CMOptions.Advanced.Seasons.CurrentSeason).Start();

			old.End();

			SeasonChanged(old);
		}

		public static void SeasonChanged(PvPSeason old)
		{
			var idx = 0;

			foreach (var profile in GetSortedProfiles(old).Where(o => o.Owner.AccessLevel <= AccessLevel.Player))
			{
				if (idx < CMOptions.Advanced.Seasons.TopListCount)
				{
					IssueWinnerRewards(old, ++idx, profile);
				}
				else if (idx < CMOptions.Advanced.Seasons.TopListCount + CMOptions.Advanced.Seasons.RunnersUpCount)
				{
					IssueLoserRewards(old, ++idx, profile);
				}
				else
				{
					break;
				}
			}

			if (OnSeasonChanged != null)
			{
				OnSeasonChanged(old);
			}
		}

		public static void IssueWinnerRewards(this PvPSeason season, int rank, PvPProfile profile)
		{
			var rewards = CMOptions.Advanced.Seasons.Rewards.Winner.GiveReward(profile.Owner);

			if (rewards == null)
			{
				rewards = new List<Item>();
			}
			else
			{
				var fmt = "{0} (Season {1} - Rank {2})";

				rewards.ForEach(r => r.Name = String.Format(fmt, r.ResolveName(profile.Owner), season.Number, rank));
			}

			if (!season.Winners.TryGetValue(profile.Owner, out var list) || list == null)
			{
				season.Winners[profile.Owner] = rewards;
			}
			else
			{
				list.AddRange(rewards);

				rewards.Free(true);
			}
		}

		public static void IssueLoserRewards(this PvPSeason season, int rank, PvPProfile profile)
		{
			var rewards = CMOptions.Advanced.Seasons.Rewards.Loser.GiveReward(profile.Owner);

			if (rewards == null)
			{
				rewards = new List<Item>();
			}
			else
			{
				var fmt = "{0} (Season {1} - Rank {2})";

				rewards.ForEach(r => r.Name = String.Format(fmt, r.ResolveName(profile.Owner), season.Number, rank));
			}

			if (!season.Losers.TryGetValue(profile.Owner, out var list) || list == null)
			{
				season.Losers[profile.Owner] = rewards;
			}
			else
			{
				list.AddRange(rewards);

				rewards.Free(true);
			}
		}

		public static PvPSeason EnsureSeason(int num, bool replace = false)
		{
			if (!Seasons.TryGetValue(num, out var season) || season == null || replace)
			{
				Seasons[num] = season = new PvPSeason(num);
			}

			return season;
		}

		public static PvPProfile EnsureProfile(PlayerMobile pm, bool replace = false)
		{
			if (!Profiles.ContainsKey(pm))
			{
				Profiles.Add(pm, new PvPProfile(pm));
			}
			else if (replace || Profiles[pm] == null || Profiles[pm].Deleted)
			{
				Profiles[pm] = new PvPProfile(pm);
			}

			return Profiles[pm];
		}

		public static IEnumerable<PvPProfile> GetSortedProfiles(PvPSeason season = null)
		{
			return GetSortedProfiles(CMOptions.Advanced.Profiles.RankingOrder, season);
		}

		public static IEnumerable<PvPProfile> GetSortedProfiles(IEnumerable<PvPProfile> profiles, PvPSeason season = null)
		{
			return GetSortedProfiles(CMOptions.Advanced.Profiles.RankingOrder, profiles, season);
		}

		public static IEnumerable<PvPProfile> GetSortedProfiles(PvPProfileRankOrder order, PvPSeason season = null)
		{
			return GetSortedProfiles(order, null, season);
		}

		public static IEnumerable<PvPProfile> GetSortedProfiles(PvPProfileRankOrder order, IEnumerable<PvPProfile> profiles, PvPSeason season = null)
		{
			if (profiles == null)
			{
				profiles = Profiles.Values;

				if (Profiles.Count > 1024)
				{
					profiles = profiles.AsParallel();
				}
			}

			return profiles.OrderByDescending(p => GetSortedValue(order, p, season));
		}

		public static long GetSortedValue(PvPProfileRankOrder order, PvPProfile profile, PvPSeason season = null)
		{
			switch (order)
			{
				case PvPProfileRankOrder.Points:
				{
					if (season == null)
					{
						return profile.TotalPoints;
					}

					return profile.History.EnsureEntry(season).Points;
				}
				case PvPProfileRankOrder.Wins:
				{
					if (season == null)
					{
						return profile.TotalWins;
					}

					return profile.History.EnsureEntry(season).Wins;
				}
				case PvPProfileRankOrder.Kills:
				{
					if (season == null)
					{
						return profile.TotalKills;
					}

					return profile.History.EnsureEntry(season).Kills;
				}
			}

			return 0;
		}

		public static void InvokeQueueJoin(PvPBattle battle, PvPTeam team, PlayerMobile m)
		{
			if (OnQueueJoin != null)
			{
				OnQueueJoin(battle, team, m);
			}
		}

		public static void InvokeQueueLeave(PvPBattle battle, PvPTeam team, PlayerMobile m)
		{
			if (OnQueueLeave != null)
			{
				OnQueueLeave(battle, team, m);
			}
		}

		public static void InvokeQueueUpdate(PvPBattle battle, PvPTeam team, PlayerMobile m)
		{
			if (OnQueueUpdate != null)
			{
				OnQueueUpdate(battle, team, m);
			}
		}

		public static void InvokeEnterBattle(PvPBattle battle, PvPRegion region, Mobile m)
		{
			if (OnEnterBattle != null)
			{
				OnEnterBattle(battle, region, m);
			}
		}

		public static void InvokeExitBattle(PvPBattle battle, PvPRegion region, Mobile m)
		{
			if (OnExitBattle != null)
			{
				OnExitBattle(battle, region, m);
			}
		}

		public static void InvokeBattleLocalBroadcast(PvPBattle battle, string message)
		{
			if (OnBattleLocalBroadcast != null)
			{
				OnBattleLocalBroadcast(battle, message);
			}
		}

		public static void InvokeBattleWorldBroadcast(PvPBattle battle, string message)
		{
			if (OnBattleWorldBroadcast != null)
			{
				OnBattleWorldBroadcast(battle, message);
			}
		}

		public static void InvokeBattleStateChanged(PvPBattle battle)
		{
			if (OnBattleStateChanged != null)
			{
				OnBattleStateChanged(battle);
			}
		}

		public static PvPBattle FindBattleByID(int uid)
		{
			return Battles.Where(o => o.Key.ValueHash.Equals(uid)).Select(kvp => kvp.Value).FirstOrDefault();
		}

		public static PvPBattle FindBattleByID(PvPSerial serial)
		{
			return Battles.Where(o => o.Key.Equals(serial)).Select(kvp => kvp.Value).FirstOrDefault();
		}

		public static PvPBattle FindBattle(PlayerMobile pm)
		{
			return FindBattle<PvPBattle>(pm);
		}

		public static T FindBattle<T>(PlayerMobile pm)
			where T : PvPBattle
		{
			if (IsParticipant(pm, out T battle) || IsSpectator(pm, out battle))
			{
				return battle;
			}

			return default(T);
		}

		public static bool IsParticipant(PlayerMobile pm)
		{
			return pm != null && IsParticipant<PvPBattle>(pm);
		}

		public static bool IsParticipant<T>(PlayerMobile pm)
			where T : PvPBattle
		{
			return pm != null && Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>().Any(b => b.IsParticipant(pm));
		}

		public static bool IsParticipant<T>(PlayerMobile pm, out T battle)
			where T : PvPBattle
		{
			battle = default(T);

			if (pm == null)
			{
				return false;
			}

			battle = Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>().FirstOrDefault(b => b.IsParticipant(pm));

			return battle != null;
		}

		public static ILookup<PvPBattle, IEnumerable<PlayerMobile>> GetParticipants()
		{
			return GetParticipants<PvPBattle>();
		}

		public static ILookup<T, IEnumerable<PlayerMobile>> GetParticipants<T>()
			where T : PvPBattle
		{
			var battles = Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>();

			return battles.ToLookup(b => b, b => b.GetParticipants());
		}

		public static ILookup<PvPBattle, int> CountParticipants()
		{
			return CountParticipants<PvPBattle>();
		}

		public static ILookup<T, int> CountParticipants<T>()
			where T : PvPBattle
		{
			var battles = Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>();

			return battles.ToLookup(b => b, b => b.GetParticipants().Count());
		}

		public static int TotalParticipants()
		{
			return TotalParticipants<PvPBattle>();
		}

		public static int TotalParticipants<T>()
			where T : PvPBattle
		{
			var battles = Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>();

			return battles.Aggregate(0, (c, b) => c + b.GetParticipants().Count());
		}

		public static bool IsSpectator(PlayerMobile pm)
		{
			return pm != null && IsSpectator<PvPBattle>(pm);
		}

		public static bool IsSpectator<T>(PlayerMobile pm)
			where T : PvPBattle
		{
			return pm != null && Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>().Any(b => b.IsSpectator(pm));
		}

		public static bool IsSpectator<T>(PlayerMobile pm, out T battle)
			where T : PvPBattle
		{
			battle = default(T);

			if (pm == null)
			{
				return false;
			}

			battle = Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>().FirstOrDefault(b => b.IsSpectator(pm));

			return battle != null;
		}

		public static ILookup<PvPBattle, IEnumerable<PlayerMobile>> GetSpectators()
		{
			return GetSpectators<PvPBattle>();
		}

		public static ILookup<T, IEnumerable<PlayerMobile>> GetSpectators<T>()
			where T : PvPBattle
		{
			var battles = Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>();

			return battles.ToLookup(b => b, b => b.GetSpectators());
		}

		public static ILookup<PvPBattle, int> CountSpectators()
		{
			return CountSpectators<PvPBattle>();
		}

		public static ILookup<T, int> CountSpectators<T>()
			where T : PvPBattle
		{
			var battles = Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>();

			return battles.ToLookup(b => b, b => b.Spectators.Count);
		}

		public static int TotalSpectators()
		{
			return TotalSpectators<PvPBattle>();
		}

		public static int TotalSpectators<T>()
			where T : PvPBattle
		{
			var battles = Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>();

			return battles.Aggregate(0, (c, b) => c + b.Spectators.Count);
		}

		public static void AddDeserter(PlayerMobile pm)
		{
			AddDeserter(pm, true);
		}

		public static void AddDeserter(PlayerMobile pm, bool message)
		{
			SetDeserter(pm, true, message);
		}

		public static void RemoveDeserter(PlayerMobile pm)
		{
			RemoveDeserter(pm, true);
		}

		public static void RemoveDeserter(PlayerMobile pm, bool message)
		{
			SetDeserter(pm, false, message);
		}

		public static void SetDeserter(PlayerMobile pm, bool state, bool message)
		{
			if (pm == null)
			{
				return;
			}

			var t = Deserters.GetValue(pm.Account);

			if (t != null && t.Running)
			{
				t.Stop();
			}

			if (state && CMOptions.Advanced.Misc.DeserterLockout > TimeSpan.Zero)
			{
				Deserters[pm.Account] = t = Timer.DelayCall(CMOptions.Advanced.Misc.DeserterLockout, RemoveDeserter, pm);

				if (message)
				{
					pm.SendMessage(0x22, "You have deserted your team and must wait until you can join another battle.");
				}

				if (!CMOptions.Advanced.Misc.DeserterAssoc)
				{
					return;
				}

				foreach (var a in pm.Account.FindSharedAccounts().Where(a => a != pm.Account && !Deserters.ContainsKey(a)))
				{
					Deserters[a] = t;

					var p = a.GetOnlineMobile();

					if (p != null)
					{
						p.SendMessage(0x22, "{0} has deserted a battle!", pm.RawName);
						p.SendMessage(0x22, "You must wait until you can join a battle because you have associated accounts.");
					}
				}
			}
			else
			{
				if (Deserters.Remove(pm.Account) && message)
				{
					pm.SendMessage(0x55, "You are no longer known as a deserter and may now join battles.");
				}

				if (t == null)
				{
					return;
				}

				Deserters.RemoveRange(
					o =>
					{
						if (o.Value == null)
						{
							return true;
						}

						if (o.Value == t)
						{
							var p = o.Key.GetOnlineMobile();

							if (p != null)
							{
								p.SendMessage(0x55, "You are no longer associated with a deserter and may now join battles.");
							}

							return true;
						}

						return false;
					});
			}
		}

		public static bool IsDeserter(PlayerMobile pm)
		{
			return pm != null && pm.Account != null && Deserters.GetValue(pm.Account) != null;
		}

		public static IEnumerable<PvPBattle> GetBattles(params PvPBattleState[] states)
		{
			return GetBattles<PvPBattle>(states);
		}

		public static IEnumerable<T> GetBattles<T>(params PvPBattleState[] states)
			where T : PvPBattle
		{
			if (states == null || states.Length == 0)
			{
				return Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>();
			}

			return Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>().Where(b => states.Contains(b.State));
		}

		public static int CountBattles(params PvPBattleState[] states)
		{
			return CountBattles<PvPBattle>(states);
		}

		public static int CountBattles<T>(params PvPBattleState[] states)
			where T : PvPBattle
		{
			if (states == null || states.Length == 0)
			{
				return Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>().Count();
			}

			return Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>().Count(b => states.Contains(b.State));
		}

		public static void DeleteAllBattles()
		{
			Battles.Values.Where(b => b != null && !b.Deleted).ForEach(b => b.Delete());
		}

		public static void InternalizeAllBattles()
		{
			Battles.Values.Where(b => b != null && !b.Deleted).ForEach(b => b.State = PvPBattleState.Internal);
		}

		public static PvPBattle CreateBattle(PvPScenario scenario)
		{
			if (scenario == null)
			{
				return null;
			}

			var battle = scenario.CreateBattle();

			Battles[battle.Serial] = battle;

			battle.Init();

			return battle;
		}

		public static bool RemoveBattle(PvPBattle battle)
		{
			return battle != null && Battles.Remove(battle.Serial);
		}

		public static bool RemoveProfile(PvPProfile profile)
		{
			if (profile != null && Profiles.GetValue(profile.Owner) == profile && Profiles.Remove(profile.Owner))
			{
				profile.Remove();
				return true;
			}

			return false;
		}

		public static void DeleteAllProfiles()
		{
			Profiles.Values.Where(p => p != null && !p.Deleted).ForEach(p => p.Delete());
		}

		public static int TotalQueued()
		{
			return TotalQueued<PvPBattle>();
		}

		public static int TotalQueued<T>()
			where T : PvPBattle
		{
			var battles = Battles.Values.Where(b => b != null && !b.Deleted).OfType<T>();

			return battles.Aggregate(0, (c, b) => c + b.Queue.Count);
		}
	}
}