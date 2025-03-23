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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Mobiles;

using VitaNex.Schedules;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class TournamentBattle : PvPBattle, IEnumerable<TournamentMatch>
	{
		[CommandProperty(AutoPvP.Access)]
		public List<TournamentMatch> Matches { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public TimeSpan MatchDelay { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public TimeSpan MatchDuration { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public double MatchSuddenDeath { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int FinalBestOfCur { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int FinalBestOfMax { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool IsFinalMatch => State == PvPBattleState.Running && Matches.Count > 0 && CurrentCapacity <= 2;

		public TournamentBattle()
		{
			Name = "Tournament";
			Category = "Tournaments";
			Description = "An elimination tournament!";

			AddTeam("Contenders", 10, 50, 85);

			Schedule.Info.Months = ScheduleMonths.All;
			Schedule.Info.Days = ScheduleDays.Sunday;
			Schedule.Info.Times = ScheduleTimes.Noon;

			Options.Timing.QueuePeriod = TimeSpan.FromHours(1.0);
			Options.Timing.PreparePeriod = TimeSpan.FromMinutes(10.0);
			Options.Timing.RunningPeriod = TimeSpan.FromHours(5.0);
			Options.Timing.EndedPeriod = TimeSpan.FromHours(5.0);

			Options.Rules.AllowBeneficial = true;
			Options.Rules.AllowHarmful = true;
			Options.Rules.AllowHousing = false;
			Options.Rules.AllowPets = false;
			Options.Rules.AllowSpawn = false;
			Options.Rules.AllowSpeech = true;
			Options.Rules.CanBeDamaged = true;
			Options.Rules.CanDamageEnemyTeam = true;
			Options.Rules.CanDamageOwnTeam = true;
			Options.Rules.CanDie = false;
			Options.Rules.CanHeal = true;
			Options.Rules.CanHealEnemyTeam = true;
			Options.Rules.CanHealOwnTeam = true;
			Options.Rules.CanMount = false;
			Options.Rules.CanFly = false;
			Options.Rules.CanResurrect = false;
			Options.Rules.CanUseStuckMenu = false;
			Options.Rules.CanMoveThrough = true;
			Options.Rules.CanEquip = true;

			RequireCapacity = true;

			UseTeamColors = false;

			IdleKick = false;
			IdleThreshold = TimeSpan.FromMinutes(30.0);

			LogoutDelay = TimeSpan.FromMinutes(5.0);

			MatchDelay = TimeSpan.FromSeconds(10.0);
			MatchDuration = TimeSpan.FromMinutes(15.0);

			MatchSuddenDeath = 0.25;

			FinalBestOfCur = 0;
			FinalBestOfMax = 3;
		}

		public TournamentBattle(GenericReader reader)
			: base(reader)
		{ }

		protected override void EnsureConstructDefaults()
		{
			base.EnsureConstructDefaults();

			Matches = new List<TournamentMatch>();
		}

		protected override void RegisterSubCommands()
		{
			base.RegisterSubCommands();

			RegisterSubCommand(
				"scores",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					UpdateUI(state.Mobile, false);

					return true;
				},
				"Display the current match rankings.");
		}

		public override bool Validate(Mobile viewer, List<string> errors, bool pop = true)
		{
			if (!base.Validate(viewer, errors, pop) && pop)
			{
				return false;
			}

			if (!Teams.All(t => t is TournamentTeam))
			{
				errors.Add("One or more teams are not of the TournamentTeam type.");
				errors.Add("[Options] -> [View Teams]");

				if (pop)
				{
					return false;
				}
			}

			return true;
		}

		protected override void GetAssignPriority(PvPTeam team, out double weight)
		{
			base.GetAssignPriority(team, out weight);

			if (weight > Double.MinValue && weight < Double.MaxValue)
			{
				weight = team.Members.Count;
			}
		}

		public override bool AddTeam(string name, int minCapacity, int capacity, int color)
		{
			return AddTeam(new TournamentTeam(this, name, minCapacity, capacity, color));
		}

		public override bool AddTeam(PvPTeam team)
		{
			if (team == null || team.Deleted)
			{
				return false;
			}

			if (team is TournamentTeam)
			{
				return base.AddTeam(team);
			}

			var added = AddTeam(team.Name, team.MinCapacity, team.MinCapacity, team.Color);

			team.Delete();

			return added;
		}

		public override void OnTeamMemberDeath(PvPTeam team, PlayerMobile pm)
		{
			base.OnTeamMemberDeath(team, pm);

			if (team is TournamentTeam)
			{
				var o = FindActiveMatch(pm);

				if (o != null)
				{
					o.HandleDeath(pm);

					o.Sync(this, (TournamentTeam)team);
				}
			}
		}

		public override void OnAfterTeamMemberDeath(PvPTeam team, PlayerMobile pm)
		{
			base.OnAfterTeamMemberDeath(team, pm);

			UpdateUI(true);
		}

		public override bool TryKickOnDeath(PvPTeam team, PlayerMobile pm, bool isLoss)
		{
			if (isLoss && IsFinalMatch && FinalBestOfCur < FinalBestOfMax)
			{
				var o = FindActiveMatch(pm);

				int min = 0, max = 0;

				if (o == null)
				{
					++FinalBestOfCur;
				}
				else if (o.CountAlive <= 1)
				{
					foreach (var c in o.Players.Where(p => p != null).Select(CountMatchesWon))
					{
						min = Math.Min(min, c);
						max = Math.Max(max, c);
					}

					++FinalBestOfCur;
				}

				if (max - min < (FinalBestOfMax / 2) + 1)
				{
					return false;
				}
			}

			return base.TryKickOnDeath(team, pm, isLoss);
		}

		public void UpdateUI(bool refreshOnly)
		{
			foreach (var pm in GetLocalBroadcastList())
			{
				UpdateUI(pm, refreshOnly);
			}
		}

		public void UpdateUI(PlayerMobile pm, bool refreshOnly)
		{
			var g = pm.FindGump<TournamentArchiveUI>();

			if (g == null)
			{
				if (refreshOnly)
				{
					return;
				}

				g = new TournamentArchiveUI(pm, this);
			}

			g.Refresh(true);
		}

		public override void OnHeal(Mobile healer, Mobile healed, int heal)
		{
			base.OnHeal(healer, healed, heal);

			if (heal > 0 && healer == healed && healer is PlayerMobile)
			{
				var p = (PlayerMobile)healer;
				var o = FindActiveMatch(p);

				if (o != null)
				{
					if (MatchSuddenDeath > 0)
					{
						var t = o.Expire.TotalSeconds / o.Duration.TotalSeconds;

						if (t < MatchSuddenDeath)
						{
							heal = (int)(heal * (t / MatchSuddenDeath));
						}
					}

					o.RecordHeal(p, heal);
				}
			}
		}

		public override void OnDamage(Mobile attacker, Mobile damaged, int damage)
		{
			base.OnDamage(attacker, damaged, damage);

			if (damage > 0 && attacker != damaged && attacker is PlayerMobile)
			{
				var p = (PlayerMobile)attacker;
				var o = FindActiveMatch(p);

				if (o != null)
				{
					o.RecordDamage(p, damage);
				}
			}
		}

		protected override void OnSpectatorAdded(PlayerMobile pm)
		{
			base.OnSpectatorAdded(pm);

			UpdateUI(pm, false);
		}

		protected override void OnBattleStarted()
		{
			base.OnBattleStarted();

			SyncMatches();

			UpdateUI(true);
		}

		protected override void OnBattleEnded()
		{
			SyncMatches();

			Matches.RemoveAll(o => o.IsEmpty);

			if (Matches.Count > 0)
			{
				var a = TournamentArchives.CreateArchive(this);

				if (a != null)
				{
					foreach (var pm in GetLocalBroadcastList())
					{
						new TournamentArchiveUI(pm, a).Send();
					}
				}
				else
				{
					UpdateUI(false);
				}
			}

			base.OnBattleEnded();
		}

		protected override void OnBattleCancelled()
		{
			SyncMatches();

			Matches.RemoveAll(o => o.IsEmpty);

			base.OnBattleCancelled();
		}

		protected override void OnReset()
		{
			Matches.Clear();

			FinalBestOfCur = 0;

			base.OnReset();
		}

		protected override void OnMicroSync()
		{
			base.OnMicroSync();

			SyncMatches();
		}

		protected void SyncMatches()
		{
			if (!IsInternal)
			{
				ForEachTeam<TournamentTeam>(SyncMatch);
			}
		}

		protected void SyncMatch(TournamentTeam team)
		{
			var m = Matches.Find(o => o.Team == team.Serial.ValueHash && !o.IsComplete);

			if (m == null || m.IsDisposed)
			{
				if (!IsRunning || CurrentCapacity <= CountActiveParticipants())
				{
					return;
				}

				Matches.Add(m = new TournamentMatch(Matches.Count, this, team));
			}

			m.Sync(this, team);
		}

		public Queue<PlayerMobile> GetMatchQueue(int capacity)
		{
			var list = GetParticipants();

			list = list.ToLookup(CountMatchesCompleted).Lowest(o => o.Key);

			list = list.Not(InActiveMatch);
			list = list.Randomize();

			if (capacity > 0)
			{
				list = list.Take(capacity);
			}

			return list.ToQueue();
		}

		public IEnumerable<PlayerMobile> GetMatchParticipants()
		{
			return Matches.SelectMany(o => o.Players.Where(p => p != null)).Distinct();
		}

		public int CountActiveParticipants()
		{
			return Matches.Where(o => !o.IsComplete).Aggregate(0, (v, o) => v + o.CountAlive);
		}

		public int CountMatchesWon(PlayerMobile pm)
		{
			return Matches.Count(o => o.IsComplete && o.Winner == pm);
		}

		public int CountMatchesCompleted(PlayerMobile pm)
		{
			return Matches.Count(o => o.IsComplete && o.Contains(pm));
		}

		public bool InActiveMatch(PlayerMobile pm)
		{
			return Matches.Any(o => !o.IsComplete && o.Contains(pm));
		}

		public TournamentMatch FindActiveMatch(PlayerMobile pm)
		{
			return Matches.Find(o => !o.IsComplete && o.Contains(pm));
		}

		public override bool CheckSkillUse(Mobile user, int skill)
		{
			if (!base.CheckSkillUse(user, skill))
			{
				return false;
			}

			var o = FindActiveMatch(user as PlayerMobile);

			return o != null && !o.IsDelayed;
		}

		public override bool CheckSpellCast(Mobile caster, ISpell spell)
		{
			if (!base.CheckSpellCast(caster, spell))
			{
				return false;
			}

			var o = FindActiveMatch(caster as PlayerMobile);

			return o != null && !o.IsDelayed;
		}

		public override bool CheckAllowHarmful(Mobile m, Mobile target, out bool handled)
		{
			var val = base.CheckAllowHarmful(m, target, out handled);

			if (!handled || !val || !m.InRegion(BattleRegion) || !target.InRegion(BattleRegion))
			{
				return val;
			}

			var pmS = m as PlayerMobile;
			var pmT = target as PlayerMobile;

			var o = FindActiveMatch(pmS);

			return o != null && !o.IsDelayed && o == FindActiveMatch(pmT);
		}

		public override bool CheckAllowBeneficial(Mobile m, Mobile target, out bool handled)
		{
			var val = base.CheckAllowBeneficial(m, target, out handled);

			if (!handled || !val || !m.InRegion(BattleRegion) || !target.InRegion(BattleRegion))
			{
				return val;
			}

			var pmS = m as PlayerMobile;
			var pmT = target as PlayerMobile;

			var o = FindActiveMatch(pmS);

			return o != null && !o.IsDelayed && o == FindActiveMatch(pmT);
		}

		protected override int NotorietyHandler(PlayerMobile source, PlayerMobile target, out bool handled)
		{
			var noto = base.NotorietyHandler(source, target, out handled);

			if (!handled || noto == BattleNotoriety.Bubble || noto == Notoriety.Invulnerable)
			{
				return noto;
			}

			if (!source.InRegion(BattleRegion) || !target.InRegion(BattleRegion))
			{
				return noto;
			}

			var o = FindActiveMatch(source);

			if (o != null && !o.IsDelayed && o == FindActiveMatch(target))
			{
				return noto;
			}

			return Notoriety.Invulnerable;
		}

		public override TimeSpan GetStateTimeLeft(DateTime when, PvPBattleState state)
		{
			var time = base.GetStateTimeLeft(when, state);

			if (state == PvPBattleState.Running)
			{
				var o = Matches.Where(r => r.IsRunning).Highest(r => r.Expire);

				if (o != null)
				{
					if (o.Expire > time)
					{
						time = o.Expire;
					}
				}
				else if (HasCapacity())
				{
					time = MatchDelay + MatchDuration;
				}
			}

			return time;
		}

		public IEnumerator<TournamentMatch> GetEnumerator()
		{
			return Matches.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(1);

			writer.Write(MatchSuddenDeath);

			writer.Write(MatchDelay);
			writer.Write(MatchDuration);

			writer.Write(FinalBestOfCur);
			writer.Write(FinalBestOfMax);

			writer.WriteBlockList(Matches, (w, r) => r.Serialize(w));
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var v = reader.GetVersion();

			if (v > 0)
			{
				MatchSuddenDeath = reader.ReadDouble();
			}
			else
			{
				MatchSuddenDeath = 0.25;
			}

			MatchDelay = reader.ReadTimeSpan();
			MatchDuration = reader.ReadTimeSpan();

			FinalBestOfCur = reader.ReadInt();
			FinalBestOfMax = reader.ReadInt();

			Matches = reader.ReadBlockList(r => new TournamentMatch(r), Matches);
		}
	}
}