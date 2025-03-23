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
using Server.Mobiles;
using Server.Spells.Fifth;

using VitaNex.Collections;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public abstract partial class PvPBattle
	{
		public List<PvPTeam> Teams { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool AutoAssign { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool UseTeamColors { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool UseIncognito { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool RequireCapacity { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool RewardTeam { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int MinCapacity => GetMinCapacity();

		[CommandProperty(AutoPvP.Access)]
		public virtual int MaxCapacity => GetMaxCapacity();

		[CommandProperty(AutoPvP.Access)]
		public virtual int CurrentCapacity => GetCurrentCapacity();

		[CommandProperty(AutoPvP.Access)]
		public virtual bool IsFull => (CurrentCapacity >= MaxCapacity);

		public int GetMinCapacity()
		{
			return Teams.Aggregate(0, (v, t) => v + t.MinCapacity);
		}

		public int GetMaxCapacity()
		{
			return Teams.Aggregate(0, (v, t) => v + t.MaxCapacity);
		}

		public int GetCurrentCapacity()
		{
			return Teams.Aggregate(0, (v, t) => v + t.Count);
		}

		public bool HasCapacity()
		{
			return HasCapacity(0);
		}

		public bool HasCapacity(int min)
		{
			if (min <= 0)
			{
				return CurrentCapacity > 1;
			}

			if (!RequireCapacity)
			{
				return true;
			}

			return CurrentCapacity >= min;
		}

		public virtual bool AddTeam(string name, int capacity, int color)
		{
			return AddTeam(name, 0, capacity, color);
		}

		public virtual bool AddTeam(string name, int minCapacity, int capacity, int color)
		{
			return AddTeam(new PvPTeam(this, name, minCapacity, capacity, color));
		}

		public virtual bool AddTeam(PvPTeam team)
		{
			if (!ContainsTeam(team))
			{
				Teams.Add(team);
				OnTeamAdded(team);
				return true;
			}

			return false;
		}

		public bool RemoveTeam(PvPTeam team)
		{
			if (Teams.Remove(team))
			{
				OnTeamRemoved(team);
				return true;
			}

			return false;
		}

		public bool ContainsTeam(PvPTeam team)
		{
			return Teams.Contains(team);
		}

		public void ResetTeam(PvPTeam team)
		{
			if (team != null && !team.Deleted)
			{
				team.Reset();
			}
		}

		public void ForEachTeam(Action<PvPTeam> action)
		{
			Teams.ForEachReverse(
				t =>
				{
					if (t != null && !t.Deleted)
					{
						action(t);
					}
					else
					{
						Teams.Remove(t);
					}
				});
		}

		public void ForEachTeam<T>(Action<T> action)
			where T : PvPTeam
		{
			Teams.ForEachReverse(
				t =>
				{
					if (t != null && !t.Deleted)
					{
						if (t is T)
						{
							action((T)t);
						}
					}
					else
					{
						Teams.Remove(t);
					}
				});
		}

		public virtual bool CanDamageOwnTeam(PlayerMobile damager, PlayerMobile target)
		{
			return Options.Rules.CanDamageOwnTeam && State == PvPBattleState.Running;
		}

		public virtual bool CanDamageEnemyTeam(PlayerMobile damager, PlayerMobile target)
		{
			return Options.Rules.CanDamageEnemyTeam && State == PvPBattleState.Running;
		}

		public virtual bool CanHealOwnTeam(PlayerMobile healer, PlayerMobile target)
		{
			return Options.Rules.CanHealOwnTeam && (State == PvPBattleState.Preparing || State == PvPBattleState.Running);
		}

		public virtual bool CanHealEnemyTeam(PlayerMobile healer, PlayerMobile target)
		{
			return Options.Rules.CanHealEnemyTeam && (State == PvPBattleState.Preparing || State == PvPBattleState.Running);
		}

		public virtual IEnumerable<PvPTeam> GetTeams()
		{
			return Teams.Where(t => t != null && !t.Deleted);
		}

		public virtual IOrderedEnumerable<PvPTeam> GetTeamsRanked()
		{
			return Teams.Where(t => t != null && !t.Deleted).Order();
		}

		public virtual int CompareTeam(PvPTeam a, PvPTeam b)
		{
			if (ReferenceEquals(a, b))
			{
				return 0;
			}

			var result = 0;

			if (a.CompareNull(b, ref result))
			{
				return result;
			}

			return a.CompareTo(b);
		}

		public virtual int CountAliveTeams()
		{
			return Teams.Count(t => t.Dead.Count < t.Count);
		}

		public virtual IEnumerable<PvPTeam> GetAliveTeams()
		{
			return Teams.Where(t => t != null && !t.Deleted && t.Dead.Count < t.Count);
		}

		public virtual PvPTeam GetMostEmptyTeam()
		{
			return Teams.Where(t => t != null && !t.Deleted).Lowest(t => t.Count);
		}

		public virtual PvPTeam GetMostFullTeam()
		{
			return Teams.Where(t => t != null && !t.Deleted).Lowest(t => t.Count);
		}

		public virtual PvPTeam GetRandomTeam()
		{
			return Teams.Where(t => t != null && !t.Deleted).GetRandom();
		}

		public virtual PvPTeam GetAutoAssignTeam(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return null;
			}

			if (IsParticipant(pm, out var team) && team != null && !team.Deleted)
			{
				return team;
			}

			return Teams.OrderBy(GetAssignPriority).FirstOrDefault();
		}

		public virtual double GetAssignPriority(PvPTeam team)
		{
			GetAssignPriority(team, out var weight);

			return weight;
		}

		protected virtual void GetAssignPriority(PvPTeam team, out double weight)
		{
			if (team == null || team.Deleted)
			{
				weight = Double.MaxValue;
				return;
			}

			if (team.IsEmpty)
			{
				weight = Double.MinValue;
				return;
			}

			weight = team.Aggregate(0.0, (v, m) => v + (m.SkillsTotal * m.RawStatTotal));
		}

		public PvPTeam FindTeam(PlayerMobile pm)
		{
			return FindTeam<PvPTeam>(pm);
		}

		public T FindTeam<T>(PlayerMobile pm)
			where T : PvPTeam
		{
			return pm != null ? Teams.OfType<T>().FirstOrDefault(t => t.IsMember(pm)) : null;
		}

		public virtual void TeamRespawn(PvPTeam team)
		{
			if (team != null && !team.Deleted)
			{
				team.ForEachMember(team.Respawn);
			}
		}

		public virtual void TeamEject(PvPTeam team)
		{
			if (team != null && !team.Deleted)
			{
				team.ForEachMember(member => team.RemoveMember(member, true));
			}
		}

		public virtual void OnTeamInit(PvPTeam team)
		{ }

		public virtual void OnTeamSync(PvPTeam team)
		{ }

		public virtual void OnTeamMicroSync(PvPTeam team)
		{ }

		public virtual void OnTeamAdded(PvPTeam team)
		{ }

		public virtual void OnTeamRemoved(PvPTeam team)
		{ }

		public virtual void OnTeamFrozen(PvPTeam team)
		{ }

		public virtual void OnTeamUnfrozen(PvPTeam team)
		{ }

		public virtual void OnTeamMemberFrozen(PvPTeam team, PlayerMobile pm)
		{ }

		public virtual void OnTeamMemberUnfrozen(PvPTeam team, PlayerMobile pm)
		{ }

		public virtual void OnTeamMemberDeath(PvPTeam team, PlayerMobile pm)
		{
			LocalBroadcast("{0} has died.", pm.RawName);

			UpdateStatistics(team, pm, s => ++s.Deaths);

			var pk = pm.FindMostRecentDamager(false) as PlayerMobile;

			if (pk != null && !pk.Deleted)
			{
				if (IsParticipant(pk, out var pkt))
				{
					if (KillPoints > 0 && !pk.Account.IsSharedWith(pm.Account))
					{
						RevokePoints(pm, KillPoints);
					}

					pm.LastKiller = pk;

					UpdateStatistics(pkt, pk, s => ++s.Kills);

					if (KillPoints > 0 && !pk.Account.IsSharedWith(pm.Account))
					{
						AwardPoints(pk, KillPoints);
					}
				}
			}

			TeleportToHomeBase(team, pm);
		}

		public virtual void OnAfterTeamMemberDeath(PvPTeam team, PlayerMobile pm)
		{
			RefreshStats(pm, true, true);

			if (!TryKickOnDeath(team, pm, true) && !TryRespawnOnDeath(team, pm, true))
			{
				TeleportToHomeBase(team, pm);

				team.Respawn(pm, false);
			}
		}

		public virtual void OnTeamMemberResurrected(PvPTeam team, PlayerMobile pm)
		{
			UpdateStatistics(team, pm, s => ++s.Resurrections);
		}

		public virtual void OnAfterTeamMemberResurrected(PvPTeam team, PlayerMobile pm)
		{
			team.Dead.Remove(pm);
		}

		public virtual void OnTeamMemberAdded(PvPTeam team, PlayerMobile pm)
		{
			team.Broadcast("{0} has joined the battle.", pm.RawName);

			if (UseTeamColors)
			{
				pm.SolidHueOverride = team.Color;
			}

			if (UseIncognito)
			{
				pm.BeginAction(typeof(IncognitoSpell));

				pm.NameMod = NameList.RandomName(pm.Female ? "female" : "male");

				var race = pm.Race ?? Race.DefaultRace;

				pm.BodyMod = race.Body(pm);
				pm.HueMod = race.RandomSkinHue();

				pm.SetHairMods(race.RandomHair(pm.Female), race.RandomFacialHair(pm.Female));
			}
		}

		public virtual void OnTeamMemberRemoved(PvPTeam team, PlayerMobile pm)
		{
			team.Broadcast("{0} has left the battle.", pm.RawName);

			pm.SolidHueOverride = -1;

			pm.EndAction(typeof(IncognitoSpell));

			pm.NameMod = null;
			pm.BodyMod = 0;
			pm.HueMod = -1;

			pm.SetHairMods(-1, -1);
		}

		public virtual bool TryKickOnDeath(PvPTeam team, PlayerMobile pm, bool isLoss)
		{
			if (team.KickOnDeath)
			{
				if (isLoss)
				{
					OnLose(pm);
				}

				team.RemoveMember(pm, true);

				return true;
			}

			return false;
		}

		public virtual bool TryRespawnOnDeath(PvPTeam team, PlayerMobile pm, bool isDelayed)
		{
			if (team.RespawnOnDeath)
			{
				if (isDelayed && team.RespawnDelay > TimeSpan.Zero)
				{
					Timer.DelayCall(team.RespawnDelay, team.Respawn, pm);
				}
				else
				{
					team.Respawn(pm);
				}

				return true;
			}

			return false;
		}

		protected virtual void ProcessRanks()
		{
			if (Options.Missions.Enabled)
			{
				ProcessMissionRanks();
			}
			else if (RewardTeam)
			{
				ProcessTeamRanks(1);
			}
			else
			{
				ProcessPlayerRanks(1);
			}
		}

		protected virtual void ProcessMissionRanks()
		{
			var players = ListPool<PlayerMobile>.AcquireObject();

			players.AddRange(GetParticipants());

			if (CheckMissions(out var team, out var player))
			{
				if (team != null)
				{
					WorldBroadcast("{0} has won {1}!", team.Name, Name);

					team.ForEachMember(OnWin);

					players.RemoveAll(team.IsMember);
				}
				else if (player != null)
				{
					WorldBroadcast("{0} has won {1}!", player.Name, Name);

					OnWin(player);

					players.Remove(player);
				}
			}

			foreach (var p in players)
			{
				OnLose(p);
			}

			ObjectPool.Free(ref players);
		}

		protected virtual void ProcessTeamRanks(int limit)
		{
			if (limit <= 0)
			{
				return;
			}

			var teams = GetTeams().Where(t => !t.IsEmpty).ToLookup(GetScore, o => o);

			if (teams.Count <= 0)
			{
				return;
			}

			bool win;

			foreach (var o in teams.OrderByDescending(o => o.Key))
			{
				win = limit > 0 && o.Key > 0 && o.Any() && --limit >= 0;

				foreach (var team in o)
				{
					if (win)
					{
						WorldBroadcast("{0} has won {1}!", team.Name, Name);

						team.ForEachMember(OnWin);
					}
					else
					{
						team.ForEachMember(OnLose);
					}
				}
			}
		}

		protected virtual void ProcessPlayerRanks(int limit)
		{
			if (limit <= 0)
			{
				return;
			}

			var players = GetParticipants().ToLookup(GetScore, o => o);

			if (players.Count <= 0)
			{
				return;
			}

			bool win;

			foreach (var o in players.OrderByDescending(o => o.Key))
			{
				win = limit > 0 && o.Key > 0 && o.Any() && --limit >= 0;

				foreach (var player in o)
				{
					if (win)
					{
						WorldBroadcast("{0} has won {1}!", player.Name, Name);

						OnWin(player);
					}
					else
					{
						OnLose(player);
					}
				}
			}
		}

		protected double GetScore(PvPTeam team)
		{
			if (team != null)
			{
				return team.GetScore();
			}

			return 0;
		}

		protected double GetMissionsScore(PvPTeam team)
		{
			if (team != null)
			{
				return team.GetMissionsScore();
			}

			return 0;
		}

		protected double GetScore(PlayerMobile pm)
		{
			if (Options.Missions.Enabled)
			{
				return GetMissionsScore(pm);
			}

			return GetStatistic(pm, e => e.Points);
		}

		public double GetMissionsScore(PlayerMobile pm)
		{
			if (Options.Missions.Enabled)
			{
				return Options.Missions.ComputeScore(this, pm);
			}

			return 0;
		}
	}
}