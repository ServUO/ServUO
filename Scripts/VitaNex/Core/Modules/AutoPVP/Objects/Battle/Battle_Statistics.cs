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
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public abstract partial class PvPBattle
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual bool Ranked { get; set; }

		public long GetStatistic(PlayerMobile pm, Func<PvPProfileHistoryEntry, long> fetch)
		{
			if (pm == null || fetch == null)
			{
				return 0;
			}

			return Teams.Aggregate(0L, (v, t) => v + GetStatistic(t, pm, fetch));
		}

		public long GetStatistic(PvPTeam t, PlayerMobile pm, Func<PvPProfileHistoryEntry, long> fetch)
		{
			if (t == null || t.Deleted || pm == null || fetch == null)
			{
				return 0;
			}

			var s = t.GetStatistics(pm);

			if (s != null)
			{
				return VitaNexCore.TryCatchGet(fetch, s, AutoPvP.CMOptions.ToConsole);
			}

			return 0;
		}

		public bool UpdateStatistics(PvPTeam t, PlayerMobile pm, Action<PvPProfileHistoryEntry> update)
		{
			if (t == null || t.Deleted || pm == null || update == null)
			{
				return false;
			}

			var s = t.GetStatistics(pm);

			if (s == null)
			{
				return false;
			}

			var success = true;

			VitaNexCore.TryCatch(
				update,
				s,
				x =>
				{
					AutoPvP.CMOptions.ToConsole(x);
					success = false;
				});

			if (t.IsMember(pm))
			{
				t.UpdateActivity(pm);
			}

			return success;
		}

		public void ResetStatistics()
		{
			ForEachTeam(ResetStatistics);
		}

		public void ResetStatistics(PvPTeam t)
		{
			if (t != null)
			{
				t.Statistics.Clear();
			}
		}

		private void TransferStatistics()
		{
			ForEachTeam(TransferStatistics);
		}

		private void TransferStatistics(PvPTeam t)
		{
			if (t != null)
			{
				t.Statistics.ForEachReverse(o => TransferStatistics(o.Key, o.Value));
			}
		}

		private void TransferStatistics(PlayerMobile pm, PvPProfileHistoryEntry e)
		{
			var profile = AutoPvP.EnsureProfile(pm);

			OnTransferStatistics(profile, e);
			OnTransferPoints(profile, e.Points);
		}

		protected virtual void OnTransferStatistics(PvPProfile profile, PvPProfileHistoryEntry stats)
		{
			if (Ranked && profile != null && stats != null)
			{
				stats.AddTo(profile.Statistics, true);
			}
		}

		protected virtual void OnTransferPoints(PvPProfile profile, long points)
		{
			if (!Ranked || profile == null || points == 0)
			{
				return;
			}

			profile.RawPoints += points;

			if (IsOnline(profile.Owner))
			{
				profile.Owner.SendMessage(
					"You have {0} {1:#,0} Battle Point{2} from {3}!",
					points > 0 ? "gained" : "lost",
					points,
					points != 1 ? "s" : String.Empty,
					Name);
			}
		}
	}
}