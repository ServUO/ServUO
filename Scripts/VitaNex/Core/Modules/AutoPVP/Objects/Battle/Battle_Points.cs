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
using Server;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public abstract partial class PvPBattle
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual int KillPoints { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int PointsBase { get; set; }

		public virtual int GetAwardPoints(PvPTeam team, PlayerMobile pm)
		{
			if (!IsRunning || team == null || team.Deleted || (pm != null && !team.IsMember(pm)))
			{
				return 0;
			}

			return PointsBase;
		}

		public virtual void AwardPoints(PlayerMobile pm)
		{
			if (pm != null)
			{
				AwardPoints(pm, GetAwardPoints(FindTeam(pm), pm));
			}
		}

		public virtual void AwardPoints(PlayerMobile pm, int points)
		{
			if (pm != null && points > 0)
			{
				if (IsParticipant(pm, out var t))
				{
					UpdateStatistics(t, pm, o => o.PointsGained += points);
				}
			}
		}

		public virtual void RevokePoints(PlayerMobile pm)
		{
			if (pm != null)
			{
				RevokePoints(pm, GetAwardPoints(FindTeam(pm), pm));
			}
		}

		public virtual void RevokePoints(PlayerMobile pm, int points)
		{
			if (pm != null && points > 0)
			{
				if (IsParticipant(pm, out var t))
				{
					UpdateStatistics(t, pm, o => o.PointsLost += points);
				}
			}
		}

		public virtual void AwardTeamPoints(PvPTeam team)
		{
			if (team != null)
			{
				team.ForEachMember(pm => UpdateStatistics(team, pm, o => o.PointsGained += GetAwardPoints(team, pm)));
			}
		}

		public virtual void AwardTeamPoints(PvPTeam team, int points)
		{
			if (team != null && points > 0)
			{
				team.ForEachMember(pm => UpdateStatistics(team, pm, o => o.PointsGained += points));
			}
		}

		public virtual void RevokeTeamPoints(PvPTeam team)
		{
			if (team != null)
			{
				team.ForEachMember(pm => UpdateStatistics(team, pm, o => o.PointsLost += GetAwardPoints(team, pm)));
			}
		}

		public virtual void RevokeTeamPoints(PvPTeam team, int points)
		{
			if (team != null)
			{
				team.ForEachMember(pm => UpdateStatistics(team, pm, o => o.PointsLost += points));
			}
		}
	}
}