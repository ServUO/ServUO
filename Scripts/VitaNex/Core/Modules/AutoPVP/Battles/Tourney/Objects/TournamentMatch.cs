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

namespace VitaNex.Modules.AutoPvP.Battles
{
	public sealed class TournamentMatch : PropertyObject, IDisposable
	{
		private long _MessageBuffer;

		public bool IsDisposed { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public TournamentRecords Records { get; private set; }

		[CommandProperty(AutoPvP.Access, true)]
		public int Index { get; private set; }

		[CommandProperty(AutoPvP.Access, true)]
		public int Team { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public PlayerMobile[] Players { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public bool[] Dead { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public int[][] Statistics { get; private set; }

		[CommandProperty(AutoPvP.Access, true)]
		public PlayerMobile Winner { get; set; }

		[CommandProperty(AutoPvP.Access, true)]
		public DateTime DateStart { get; set; }

		[CommandProperty(AutoPvP.Access, true)]
		public DateTime DateEnd { get; set; }

		[CommandProperty(AutoPvP.Access, true)]
		public TimeSpan Delay { get; set; }

		[CommandProperty(AutoPvP.Access, true)]
		public TimeSpan Duration { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public TimeSpan TotalTime => Delay + Duration;

		[CommandProperty(AutoPvP.Access)]
		public TimeSpan Expire
		{
			get
			{
				if (DateStart > DateTime.MinValue)
				{
					var now = DateTime.UtcNow;
					var exp = DateStart + TotalTime;

					if (exp > now)
					{
						return exp - now;
					}
				}

				return TimeSpan.Zero;
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public bool IsRunning => Expire > TimeSpan.Zero;

		[CommandProperty(AutoPvP.Access)]
		public bool IsDelayed => DateStart + Delay > DateTime.UtcNow;

		[CommandProperty(AutoPvP.Access)]
		public bool IsComplete => DateEnd < DateTime.MaxValue;

		[CommandProperty(AutoPvP.Access)]
		public int Capacity => Players.Length;

		[CommandProperty(AutoPvP.Access)]
		public int Count => Players.Count(o => o != null);

		[CommandProperty(AutoPvP.Access)]
		public int CountAlive => Players.Count(o => o != null && IsAlive(o));

		[CommandProperty(AutoPvP.Access)]
		public int CountDead => Players.Count(o => o != null && IsDead(o));

		[CommandProperty(AutoPvP.Access)]
		public bool IsEmpty => Count == 0;

		[CommandProperty(AutoPvP.Access)]
		public bool IsFull => Count == Capacity;

		public TournamentMatch(int index, int team, TimeSpan delay, TimeSpan duration)
		{
			Index = index;
			Team = team;
			Delay = delay;
			Duration = duration;

			Records = new TournamentRecords();

			Players = new PlayerMobile[2];
			Dead = new bool[Players.Length];

			Statistics = new int[Players.Length][];
			Statistics.SetAll(i => new int[2]); // Damage, Healing

			DateStart = DateTime.MinValue;
			DateEnd = DateTime.MaxValue;
		}

		public TournamentMatch(int index, TournamentBattle battle, TournamentTeam team)
			: this(index, team.Serial.ValueHash, battle.MatchDelay, battle.MatchDuration)
		{ }

		public TournamentMatch(GenericReader reader)
			: base(reader)
		{ }

		public void RecordDamage(PlayerMobile pm, int amount)
		{
			var i = Players.IndexOf(pm);

			if (i >= 0)
			{
				Statistics[i][0] += amount;

				Record("{0} deals {1:#,0} damage.", pm.RawName, amount);
			}
		}

		public void RecordHeal(PlayerMobile pm, int amount)
		{
			var i = Players.IndexOf(pm);

			if (i >= 0)
			{
				Statistics[i][1] += amount;

				Record("{0} heals {1:#,0} health.", pm.RawName, amount);
			}
		}

		public int GetDamageDone(PlayerMobile pm)
		{
			var i = Players.IndexOf(pm);

			if (i >= 0)
			{
				return Statistics[i][0];
			}

			return -1;
		}

		public int GetHealingDone(PlayerMobile pm)
		{
			var i = Players.IndexOf(pm);

			if (i >= 0)
			{
				return Statistics[i][1];
			}

			return -1;
		}

		public double ComputeScore(PlayerMobile pm)
		{
			return GetDamageDone(pm) - GetHealingDone(pm);
		}

		public void Record(string format, params object[] args)
		{
			Records.Record(format, args);
		}

		public void Record(string value)
		{
			Records.Record(value);
		}

		public void ForEachPlayer(Action<PlayerMobile> action)
		{
			if (action == null)
			{
				return;
			}

			foreach (var pm in Players.Where(o => o != null))
			{
				action(pm);
			}
		}

		public void Broadcast(int hue, string format, params object[] args)
		{
			Record(format, args);

			ForEachPlayer(pm => pm.SendMessage(hue, format, args));
		}

		public void Broadcast(int hue, string message)
		{
			Record(message);

			ForEachPlayer(pm => pm.SendMessage(hue, message));
		}

		public void Broadcast(string format, params object[] args)
		{
			Record(format, args);

			ForEachPlayer(pm => pm.SendMessage(format, args));
		}

		public void Broadcast(string message)
		{
			Record(message);

			ForEachPlayer(pm => pm.SendMessage(message));
		}

		public void HandleDeath(PlayerMobile pm)
		{
			var i = Players.IndexOf(pm);

			if (Dead.InBounds(i))
			{
				Dead[i] = true;
			}
		}

		public bool IsDead(PlayerMobile pm)
		{
			var i = Players.IndexOf(pm);

			if (Dead.InBounds(i))
			{
				return Dead[i];
			}

			return false;
		}

		public bool IsAlive(PlayerMobile pm)
		{
			var i = Players.IndexOf(pm);

			if (Dead.InBounds(i))
			{
				return !Dead[i];
			}

			return false;
		}

		public bool Contains(PlayerMobile pm)
		{
			return Players.Contains(pm);
		}

		public void Sync(TournamentBattle b, TournamentTeam t)
		{
			if (IsDisposed || IsComplete)
			{
				return;
			}

			if (b == null || b.Deleted || b.IsInternal || b.IsQueueing || b.IsPreparing)
			{
				return;
			}

			if (t == null || t.Deleted || t.Serial.ValueHash != Team)
			{
				return;
			}

			if (b.IsRunning && DateStart == DateTime.MinValue) // Populate & Start
			{
				for (var i = 0; i < Capacity; i++)
				{
					var pm = Players[i];

					if (pm != null && !b.IsAliveParticipant(pm))
					{
						Players[i] = null;
						Dead[i] = false;
						Statistics[i].SetAll(0);

						Broadcast(0x22, "{0} has left the match! [{1} / {2}]", pm.RawName, Count, Capacity);
					}
				}

				if (!IsFull)
				{
					var q = b.GetMatchQueue(Capacity - Count);

					for (var i = 0; i < Capacity && q.Count > 0; i++)
					{
						var pm = Players[i];

						if (pm == null)
						{
							Players[i] = pm = q.Dequeue();
							Dead[i] = false;
							Statistics[i].SetAll(0);

							Broadcast(0x55, "{0} has joined the match! [{1} / {2}]", pm.RawName, Count, Capacity);
						}
					}

					q.Free(true);
				}

				if (IsFull)
				{
					Broadcast("The match is now full, prepare to fight!");

					Start(b, t);
				}
			}
			else if (b.IsRunning && Expire > TimeSpan.Zero) // Tick
			{
				var started = !IsDelayed;

				if (_MessageBuffer <= Core.TickCount)
				{
					var time = started ? Expire : ((DateStart + Delay) - DateTime.UtcNow);

					if (time.Hours > 0 && time.Hours <= 3)
					{
						_MessageBuffer = Core.TickCount + (Math.Min(15, time.Minutes) * 60000);
					}
					else if (time.Minutes > 0 && time.Minutes <= 5)
					{
						_MessageBuffer = Core.TickCount + (Math.Min(15, time.Seconds) * 1000);
					}
					else if (time.Seconds > 0 && time.Seconds <= 10)
					{
						_MessageBuffer = Core.TickCount + Math.Min(1000, time.Milliseconds);
					}
					else
					{
						_MessageBuffer = Int64.MaxValue;
					}

					var seconds = (int)Math.Ceiling(time.TotalSeconds);

					if (seconds > 5)
					{
						Broadcast(
							"The match will {0} in {1} second{2}!",
							!started ? "begin" : "end",
							seconds,
							seconds != 1 ? "s" : String.Empty);
					}
					else if (seconds > 0)
					{
						Broadcast("{0}...", seconds);
					}
					else
					{
						Broadcast(!started ? "FIGHT!" : "CEASE!");
					}
				}

				PlayerMobile o = null;

				var count = 0;

				foreach (var pm in Players.Where(p => IsAlive(p) && b.IsAliveParticipant(p)).OrderBy(ComputeScore))
				{
					o = pm;
					++count;
				}

				if (count <= 1)
				{
					if (!started)
					{
						Broadcast("Not enough players to start the match.");
					}

					Winner = o;

					if (Winner != null)
					{
						if (!started)
						{
							Broadcast("{0} has won the match by default!", Winner.RawName);
						}
						else
						{
							Broadcast("{0} has won the match!", Winner.RawName);
						}
					}
					else
					{
						Broadcast("No winner has been declared.");
					}

					End(b, t);
				}
			}
			else if ((b.IsRunning || b.IsEnded) && DateEnd == DateTime.MaxValue) // End
			{
				var started = !IsDelayed;

				if (started)
				{
					Broadcast("TIME UP!");
				}

				Winner = Players.Where(p => IsAlive(p) && b.IsAliveParticipant(p)).Highest(ComputeScore);

				if (Winner != null)
				{
					if (!started)
					{
						Broadcast("{0} has won the match by default!", Winner.RawName);
					}
					else
					{
						Broadcast("{0} has won the match!", Winner.RawName);
					}
				}
				else
				{
					Broadcast("No winner has been declared.");
				}

				End(b, t);
			}
		}

		private void Start(TournamentBattle b, TournamentTeam t)
		{
			DateStart = DateTime.UtcNow;

			foreach (var pm in Players.Where(p => IsAlive(p) && b.IsParticipant(p)))
			{
				b.RefreshStats(pm, true, true);
				b.TeleportToSpawnPoint(t, pm);
			}
		}

		private void End(TournamentBattle b, TournamentTeam t)
		{
			DateEnd = DateTime.UtcNow;

			foreach (var pm in Players.Where(p => IsAlive(p) && b.IsParticipant(p)))
			{
				b.RefreshStats(pm, true, true);
				b.TeleportToHomeBase(t, pm);
			}
		}

		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			Players.Clear();

			Records = null;
			Players = null;
			Dead = null;
			Statistics = null;
			Winner = null;

			IsDisposed = true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(Index);
			writer.Write(Team);

			writer.WriteArray(Players, (w, o) => w.Write(o));
			writer.WriteArray(Dead, (w, o) => w.Write(o));
			writer.WriteArray(Statistics, (w, o) => w.WriteArray(o, (w1, o1) => w1.Write(o1)));

			writer.Write(Delay);
			writer.Write(Duration);

			writer.Write(DateStart);
			writer.Write(DateEnd);

			writer.Write(Winner);

			Records.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Index = reader.ReadInt();
			Team = reader.ReadInt();

			Players = reader.ReadArray(r => r.ReadMobile<PlayerMobile>(), Players);
			Dead = reader.ReadArray(r => r.ReadBool(), Dead);
			Statistics = reader.ReadArray(r => r.ReadArray(r1 => r1.ReadInt()), Statistics);

			Delay = reader.ReadTimeSpan();
			Duration = reader.ReadTimeSpan();

			DateStart = reader.ReadDateTime();
			DateEnd = reader.ReadDateTime();

			Winner = reader.ReadMobile<PlayerMobile>();

			Records = new TournamentRecords(reader);
		}
	}
}