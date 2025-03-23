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
using System.Text;

using Server;
using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPStatistics
	{
		public PlayerMobile Player { get; private set; }
		public PvPProfileHistoryEntry Entry { get; private set; }

		public PvPStatistics(PlayerMobile m)
			: this(m, new PvPProfileHistoryEntry(AutoPvP.CurrentSeason.Number))
		{ }

		public PvPStatistics(PlayerMobile m, PvPProfileHistoryEntry entry)
		{
			Player = m;
			Entry = entry;
		}

		public static implicit operator PvPProfileHistoryEntry(PvPStatistics o)
		{
			return o.Entry;
		}
	}

	[PropertyObject]
	public class PvPTeam : IEnumerable<PlayerMobile>, IHued, IComparable<PvPTeam>
	{
		private string _Name;
		private int _Color;

		private int _MaxCapacity;
		private int _MinCapacity;

		private Point3D _HomeBase = Point3D.Zero;
		private Point3D _SpawnPoint = Point3D.Zero;

		public List<PlayerMobile> Idlers { get; private set; }

		public Dictionary<PlayerMobile, DateTime> Members { get; private set; }
		public Dictionary<PlayerMobile, DateTime> Dead { get; private set; }

		public Dictionary<PlayerMobile, PvPProfileHistoryEntry> Statistics { get; private set; }

		[CommandProperty(AutoPvP.Access, true)]
		public PvPSerial Serial { get; private set; }

		[CommandProperty(AutoPvP.Access, true)]
		public bool Initialized { get; private set; }

		[CommandProperty(AutoPvP.Access, true)]
		public bool Deleted { get; private set; }

		[CommandProperty(AutoPvP.Access, true)]
		public PvPBattle Battle { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPTeamGate Gate { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual string Name { get => _Name; set => _Name = value ?? String.Empty; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int MinCapacity
		{
			get => _MinCapacity;
			set => _MinCapacity = Math.Max(1, Math.Min(MaxCapacity, value));
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual int MaxCapacity { get => _MaxCapacity; set => _MaxCapacity = Math.Max(MinCapacity, value); }

		[Hue, CommandProperty(AutoPvP.Access)]
		public virtual int Color { get => _Color; set => _Color = Math.Max(0, value); }

		[CommandProperty(AutoPvP.Access)]
		public virtual Point3D HomeBase
		{
			get => _HomeBase;
			set
			{
				if (Deserializing || (Battle.BattleRegion != null && Battle.BattleRegion.Contains(value)))
				{
					_HomeBase = value;
				}
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual Point3D SpawnPoint
		{
			get => _SpawnPoint;
			set
			{
				if (Deserializing || (Battle.BattleRegion != null && Battle.BattleRegion.Contains(value)))
				{
					_SpawnPoint = value;
				}
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual MapPoint GateLocation { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool KickOnDeath { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool RespawnOnStart { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool RespawnOnDeath { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public TimeSpan RespawnDelay { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int RespawnRangeMin { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int RespawnRangeMax { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int Count => Members.Count;

		[CommandProperty(AutoPvP.Access)]
		public bool IsAlive => Dead.Count < Count;

		[CommandProperty(AutoPvP.Access)]
		public bool IsFull => Count >= MaxCapacity;

		[CommandProperty(AutoPvP.Access)]
		public bool IsEmpty => Count <= 0;

		public PlayerMobile this[int index]
		{
			get
			{
				if (index >= 0 && index <= Members.Count)
				{
					return Members.Keys.ElementAt(index);
				}

				return null;
			}
		}

		public DateTime? this[PlayerMobile pm]
		{
			get => IsMember(pm) ? Members[pm] : default(DateTime?);
			set => Members[pm] = value ?? DateTime.UtcNow;
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual Map Map => Battle.Map;

		int IHued.HuedItemID => 1065;

		protected bool Deserialized { get; private set; }
		protected bool Deserializing { get; private set; }

		protected virtual void EnsureConstructDefaults()
		{
			Idlers = new List<PlayerMobile>();
			Dead = new Dictionary<PlayerMobile, DateTime>();
			Members = new Dictionary<PlayerMobile, DateTime>();
			Statistics = new Dictionary<PlayerMobile, PvPProfileHistoryEntry>();
		}

		private PvPTeam(bool deserializing)
		{
			Deserialized = deserializing;

			EnsureConstructDefaults();
		}

		public PvPTeam(PvPBattle battle, string name = "Incognito", int minCapacity = 1, int maxCapacity = 1, int color = 12)
			: this(false)
		{
			Serial = new PvPSerial();
			Battle = battle;
			Name = name;
			Color = color;

			GateLocation = MapPoint.Empty;

			KickOnDeath = true;
			RespawnOnStart = true;
			RespawnOnDeath = false;
			RespawnDelay = TimeSpan.FromSeconds(10.0);

			MinCapacity = minCapacity;
			MaxCapacity = maxCapacity;

			Reset();
		}

		public PvPTeam(PvPBattle battle, GenericReader reader)
			: this(true)
		{
			Battle = battle;

			Deserializing = true;
			Deserialize(reader);
			Deserializing = false;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<PlayerMobile> GetEnumerator()
		{
			return Members.Keys.GetEnumerator();
		}

		public PvPProfileHistoryEntry GetStatistics(PlayerMobile pm)
		{
			var e = Statistics.GetValue(pm);

			if (e == null && IsMember(pm))
			{
				Statistics[pm] = e = new PvPProfileHistoryEntry(AutoPvP.CurrentSeason.Number);
			}

			return e;
		}

		public void ForEachMember(Action<PlayerMobile> action)
		{
			Members.Keys.ForEachReverse(
				o =>
				{
					if (!o.Deleted)
					{
						action(o);
					}
					else
					{
						Members.Remove(o);
					}
				});
		}

		public virtual bool CanRespawn(PlayerMobile member)
		{
			return member != null && !member.Deleted && IsMember(member);
		}

		public virtual void Respawn(PlayerMobile member)
		{
			Respawn(member, true);
		}

		public virtual void Respawn(PlayerMobile member, bool teleport)
		{
			if (member == null || member.Deleted || !CanRespawn(member))
			{
				return;
			}

			if (member.Alive)
			{
				OnMemberResurrected(member);
			}

			Battle.RefreshStats(member);

			if (teleport)
			{
				Battle.TeleportToSpawnPoint(this, member);
			}
		}

		public bool Validate(Mobile viewer = null)
		{
			return Validate(viewer, new List<string>());
		}

		public virtual bool Validate(Mobile viewer, List<string> errors, bool pop = true)
		{
			if (Deleted)
			{
				errors.Add("This Team has been deleted.");
				return false;
			}

			if (Battle == null || Battle.Deleted)
			{
				errors.Add("This Team is unlinked.");
				return false;
			}

			if (String.IsNullOrWhiteSpace(Name))
			{
				errors.Add("Select a valid Name.");
				errors.Add("[Options] -> [Edit Options]");

				if (pop)
				{
					return false;
				}
			}

			if (HomeBase == Point3D.Zero)
			{
				errors.Add("Select a valid Home Base.");
				errors.Add("[Options] -> [Edit Options]");

				if (pop)
				{
					return false;
				}
			}
			else if (Battle.BattleRegion != null && !Battle.BattleRegion.Contains(HomeBase, Map))
			{
				errors.Add("Home Base must be within the Battle Region.");
				errors.Add("[Options] -> [Edit Options]");

				if (pop)
				{
					return false;
				}
			}

			if (SpawnPoint == Point3D.Zero)
			{
				errors.Add("Select a valid Spawn Point.");
				errors.Add("[Options] -> [Edit Options]");

				if (pop)
				{
					return false;
				}
			}
			else if (Battle.BattleRegion != null && !Battle.BattleRegion.Contains(SpawnPoint, Map))
			{
				errors.Add("Spawn Point must be within the Battle Region.");
				errors.Add("[Options] -> [Edit Options]");

				if (pop)
				{
					return false;
				}
			}

			return errors.Count <= 0;
		}

		public virtual bool IsReady()
		{
			if (Battle.RequireCapacity)
			{
				return Count >= MinCapacity;
			}

			return Count > 0;
		}

		public virtual void Reset()
		{
			if (Dead != null)
			{
				Dead.Clear();
			}
			else
			{
				Dead = new Dictionary<PlayerMobile, DateTime>();
			}

			if (Idlers != null)
			{
				Idlers.Clear();
			}
			else
			{
				Idlers = new List<PlayerMobile>();
			}

			if (Members != null)
			{
				ForEachMember(pm => RemoveMember(pm, true));
				Members.Clear();
			}
			else
			{
				Members = new Dictionary<PlayerMobile, DateTime>();
			}

			if (Statistics != null)
			{
				Statistics.Clear();
			}
			else
			{
				Statistics = new Dictionary<PlayerMobile, PvPProfileHistoryEntry>();
			}
		}

		public virtual void UpdateActivity(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			Idlers.Remove(pm);

			if (IsMember(pm))
			{
				Members[pm] = DateTime.UtcNow;
			}
		}

		public virtual void AddMember(PlayerMobile pm, bool teleport)
		{
			if (IsMember(pm))
			{
				return;
			}

			Members[pm] = DateTime.UtcNow;

			if (teleport)
			{
				Battle.TeleportToHomeBase(this, pm);

				if (Battle.IsRunning)
				{
					Timer.DelayCall(
						RespawnDelay,
						() =>
						{
							if (Battle.IsRunning)
							{
								Battle.TeleportToSpawnPoint(this, pm);
							}
						});
				}
			}

			OnMemberAdded(pm);
		}

		public virtual void RemoveMember(PlayerMobile pm, bool teleport)
		{
			if (!IsMember(pm))
			{
				return;
			}

			Dead.Remove(pm);
			Idlers.Remove(pm);

			Members.Remove(pm);
			OnMemberRemoved(pm);

			if (teleport)
			{
				Battle.TeleportToSpectateLocation(pm);
			}
		}

		public bool IsMember(PlayerMobile pm)
		{
			return Members != null && Members.ContainsKey(pm);
		}

		public bool IsDead(PlayerMobile pm)
		{
			return Dead != null && Dead.ContainsKey(pm);
		}

		public bool IsIdle(PlayerMobile pm)
		{
			return Idlers != null && Idlers.Contains(pm);
		}

		public virtual void OnMemberResurrected(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			Battle.OnTeamMemberResurrected(this, pm);
			Battle.OnAfterTeamMemberResurrected(this, pm);
		}

		public virtual void OnMemberDeath(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			Dead[pm] = DateTime.UtcNow;

			Battle.OnTeamMemberDeath(this, pm);
			Battle.OnAfterTeamMemberDeath(this, pm);
		}

		public virtual void OnMemberAdded(PlayerMobile pm)
		{
			if (pm != null && !pm.Deleted)
			{
				Battle.OnTeamMemberAdded(this, pm);
			}
		}

		public virtual void OnMemberRemoved(PlayerMobile pm)
		{
			if (pm != null && !pm.Deleted)
			{
				Battle.OnTeamMemberRemoved(this, pm);
			}
		}

		public virtual void PlaySound(int soundID)
		{
			ForEachMember(pm => PlaySound(pm, soundID));
		}

		public virtual void PlaySound(PlayerMobile pm, int soundID)
		{
			Battle.PlaySound(pm, soundID);
		}

		public virtual void SendSound(int soundID)
		{
			ForEachMember(pm => SendSound(pm, soundID));
		}

		public virtual void SendSound(PlayerMobile pm, int soundID)
		{
			Battle.SendSound(pm, soundID);
		}

		public virtual void Broadcast(string message, params object[] args)
		{
			ForEachMember(pm => pm.SendMessage(Battle.Options.Broadcasts.Local.MessageHue, message, args));
		}

		public long GetTotalPoints()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o.Points);
		}

		public long GetTotalPointsGained()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o.PointsGained);
		}

		public long GetTotalPointsLost()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o.PointsLost);
		}

		public long GetTotalKills()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o.Kills);
		}

		public long GetTotalDeaths()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o.Deaths);
		}

		public long GetTotalResurrections()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o.Resurrections);
		}

		public long GetTotalDamageTaken()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o.DamageTaken);
		}

		public long GetTotalDamageDone()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o.DamageDone);
		}

		public long GetTotalHealingTaken()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o.HealingTaken);
		}

		public long GetTotalHealingDone()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o.HealingDone);
		}

		public void Delete()
		{
			if (Deleted)
			{
				return;
			}

			OnDeleted();

			Reset();

			Battle.RemoveTeam(this);

			if (Gate != null)
			{
				Gate.Delete();
				Gate = null;
			}

			Members = null;
			Dead = null;
			Idlers = null;

			Battle = null;

			Deleted = true;
		}

		public void Init()
		{
			if (Initialized)
			{
				return;
			}

			OnInit();

			Battle.OnTeamInit(this);

			Initialized = true;
		}

		protected virtual void OnInit()
		{ }

		protected virtual void OnDeleted()
		{ }

		public virtual void OnBattleOpened()
		{ }

		public virtual void OnBattlePreparing()
		{
			ForEachMember(pm => Battle.TeleportToHomeBase(this, pm));
		}

		public virtual void OnBattleStarted()
		{
			if (RespawnOnStart)
			{
				ForEachMember(pm => Battle.TeleportToSpawnPoint(this, pm));
			}
		}

		public virtual void OnBattleEnded()
		{
			Battle.TeamEject(this);
		}

		public virtual void OnBattleCancelled()
		{
			Battle.TeamEject(this);
		}

		public virtual void OnFrozen()
		{ }

		public virtual void OnFrozen(PlayerMobile pm)
		{ }

		public virtual void OnUnfrozen()
		{ }

		public virtual void OnUnfrozen(PlayerMobile pm)
		{ }

		public void Freeze()
		{
			ForEachMember(FreezeMember);
			OnFrozen();

			if (Battle != null)
			{
				Battle.OnTeamFrozen(this);
			}
		}

		public void Unfreeze()
		{
			ForEachMember(UnfreezeMember);
			OnUnfrozen();

			if (Battle != null)
			{
				Battle.OnTeamUnfrozen(this);
			}
		}

		public void FreezeMember(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted || pm.Frozen || !IsMember(pm))
			{
				return;
			}

			pm.Frozen = true;
			OnFrozen(pm);

			if (Battle != null)
			{
				Battle.OnTeamMemberFrozen(this, pm);
			}
		}

		public void UnfreezeMember(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted || !pm.Frozen || !IsMember(pm))
			{
				return;
			}

			pm.Frozen = false;
			OnUnfrozen(pm);

			if (Battle != null)
			{
				Battle.OnTeamMemberUnfrozen(this, pm);
			}
		}

		public void Sync()
		{
			OnSync();

			if (Battle != null)
			{
				Battle.OnTeamSync(this);
			}
		}

		protected virtual void OnSync()
		{ }

		public void MicroSync()
		{
			if (!Members.IsNullOrEmpty() && !Idlers.IsNullOrEmpty() && Battle != null && Battle.IsRunning)
			{
				if (Battle.IdleKick)
				{
					var then = DateTime.UtcNow - Battle.IdleThreshold;

					foreach (var o in Members)
					{
						if (o.Value > then)
						{
							Idlers.Remove(o.Key);
						}
						else if (o.Value < then)
						{
							Idlers.Update(o.Key);
						}
					}

					Idlers.RemoveAll(IsDead);

					if (Idlers.Count > 0)
					{
						Idlers.ForEachReverse(pm => Battle.Quit(pm, true));
					}
				}
				else
				{
					Idlers.Clear();
				}
			}

			OnMicroSync();

			if (Battle != null)
			{
				Battle.OnTeamMicroSync(this);
			}
		}

		protected virtual void OnMicroSync()
		{ }

		public virtual void InvalidateGate()
		{
			if (Battle == null || Battle.IsInternal || Battle.Hidden || !Battle.QueueAllowed || GateLocation.InternalOrZero)
			{
				if (Gate == null)
				{
					return;
				}

				Gate.Delete();
				Gate = null;
				return;
			}

			if (Gate == null || Gate.Deleted)
			{
				Gate = new PvPTeamGate(this);

				if (GateLocation.MoveToWorld(Gate))
				{
					Gate.MoveToWorld(GateLocation, GateLocation);
				}
			}

			if (Gate.Team == null)
			{
				Gate.Team = this;
			}
		}

		public virtual int CompareTo(PvPTeam t)
		{
			if (ReferenceEquals(this, t))
			{
				return 0;
			}

			var res = 0;

			if (this.CompareNull(t, ref res))
			{
				return res;
			}

			if (Deleted && !t.Deleted)
			{
				return 1;
			}

			if (!Deleted && t.Deleted)
			{
				return -1;
			}

			if (IsEmpty && !t.IsEmpty)
			{
				return 1;
			}

			if (!IsEmpty && t.IsEmpty)
			{
				return -1;
			}

			var a = GetComparisonValue();
			var b = t.GetComparisonValue();

			if (a > b)
			{
				return -1;
			}

			if (a < b)
			{
				return 1;
			}

			return 0;
		}

		protected virtual double GetComparisonValue()
		{
			return GetScore();
		}

		public double GetScore()
		{
			if (Battle.Options.Missions.Enabled)
			{
				return GetMissionsScore();
			}

			return GetTotalPoints();
		}

		public double GetMissionsScore()
		{
			if (!Battle.Options.Missions.Enabled)
			{
				return 0;
			}

			var score = Battle.Options.Missions.ComputeScore(this);

			return this.Aggregate(score, (s, p) => s + Battle.Options.Missions.ComputeScore(Battle, p));
		}

		public virtual string ToHtmlString(Mobile viewer = null, bool big = true)
		{
			var html = new StringBuilder();

			if (big)
			{
				html.Append("<BIG>");
			}

			GetHtmlString(viewer, html);

			if (big)
			{
				html.Append("</BIG>");
			}

			return html.ToString();
		}

		public virtual void GetHtmlStatistics(Mobile viewer, StringBuilder html)
		{
			html.AppendLine("Points Total: {0:#,0}", GetTotalPoints());
			html.AppendLine("Points Gained: {0:#,0}", GetTotalPointsGained());
			html.AppendLine("Points Lost: {0:#,0}", GetTotalPointsLost());
			html.AppendLine("----------");
			html.AppendLine("Kills: {0:#,0}", GetTotalKills());
			html.AppendLine("Deaths: {0:#,0}", GetTotalDeaths());
			html.AppendLine("Resurrections: {0:#,0}", GetTotalResurrections());
			html.AppendLine("----------");
			html.AppendLine("Damage Taken: {0:#,0}", GetTotalDamageTaken());
			html.AppendLine("Damage Done: {0:#,0}", GetTotalDamageDone());
			html.AppendLine("Healing Taken: {0:#,0}", GetTotalHealingTaken());
			html.AppendLine("Healing Done: {0:#,0}", GetTotalHealingDone());
		}

		public virtual void GetHtmlString(Mobile viewer, StringBuilder html)
		{
			string t;

			html.Append(String.Empty.WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));

			if (Deleted)
			{
				t = "This Team no longer exists.";
				t = t.WrapUOHtmlBig();
				t = t.WrapUOHtmlColor(System.Drawing.Color.OrangeRed, SuperGump.DefaultHtmlColor);

				html.AppendLine(t);

				return;
			}

			html.AppendLine("Team: {0} {1}".WrapUOHtmlBig(), Name, IsFull ? "(Full)" : String.Empty);
			html.AppendLine();

			int curCap = Count, maxCap = MaxCapacity;

			html.AppendLine("{0:#,0} invites available of {1:#,0} max.", maxCap - curCap, maxCap);
			html.AppendLine();

			IEnumerable<PlayerMobile> members = this.OrderByDescending(o => Battle.Options.Missions.ComputeScore(Battle, o));

			if (Battle.Options.Missions.Enabled)
			{
				html.Append(String.Empty.WrapUOHtmlColor(System.Drawing.Color.PaleGoldenrod, false));

				html.AppendLine("Mission Objectives".WrapUOHtmlBig());
				html.AppendLine();

				html.Append(Battle.Options.Missions.GetStatus(this));
				html.AppendLine();

				members = members.Enumerate(
					(i, o) =>
					{
						t = String.Format("{0:#,0}: {1}", i + 1, o.Name);
						t = t.WrapUOHtmlColor(viewer.GetNotorietyColor(o), SuperGump.DefaultHtmlColor);

						html.AppendLine(t);

						html.Append(Battle.Options.Missions.GetStatus(Battle, o));
						html.AppendLine();
					});
			}

			html.Append(String.Empty.WrapUOHtmlColor(System.Drawing.Color.YellowGreen, false));

			html.AppendLine("Statistic Totals".WrapUOHtmlBig());
			html.AppendLine();

			GetHtmlStatistics(viewer, html);

			html.AppendLine();

			html.Append(String.Empty.WrapUOHtmlColor(System.Drawing.Color.Cyan, false));

			html.AppendLine("Members: {0:#,0} / {1:#,0}".WrapUOHtmlBig(), Count, maxCap);
			html.AppendLine();

			var j = 0;

			foreach (var o in members)
			{
				t = String.Format("{0:#,0}: {1}", ++j, o.Name);
				t = t.WrapUOHtmlColor(viewer.GetNotorietyColor(o), SuperGump.DefaultHtmlColor);

				html.AppendLine(t);
			}

			html.AppendLine();

			html.Append(String.Empty.WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(8);

			if (version > 4)
			{
				writer.WriteBlock(
					w =>
					{
						if (version > 5)
						{
							Serial.Serialize(w);
						}
						else
						{
							w.WriteType(Serial, t => Serial.Serialize(w));
						}
					});
			}

			switch (version)
			{
				case 8:
				case 7:
				{
					writer.Write(RespawnRangeMin);
					writer.Write(RespawnRangeMax);
				}
				goto case 6;
				case 6:
				case 5:
				case 4:
				case 3:
					writer.Write(RespawnOnStart);
					goto case 2;
				case 2:
					writer.Write(KickOnDeath);
					goto case 1;
				case 1:
				{
					GateLocation.Serialize(writer);
					writer.Write(Gate);
				}
				goto case 0;
				case 0:
				{
					writer.Write(_Name);
					writer.Write(_MinCapacity);
					writer.Write(_MaxCapacity);
					writer.Write(_Color);
					writer.Write(_HomeBase);
					writer.Write(_SpawnPoint);

					writer.Write(RespawnOnDeath);
					writer.Write(RespawnDelay);

					if (version < 8)
					{
						writer.WriteBlock(w => w.WriteType(Statistics));
					}
				}
				break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			if (version > 4)
			{
				reader.ReadBlock(
					r =>
					{
						if (version > 5)
						{
							Serial = new PvPSerial(r);
						}
						else
						{
							Serial = r.ReadTypeCreate<PvPSerial>(r);
							Serial = new PvPSerial();
						}
					});
			}
			else
			{
				Serial = new PvPSerial();
			}

			switch (version)
			{
				case 8:
				case 7:
				{
					RespawnRangeMin = reader.ReadInt();
					RespawnRangeMax = reader.ReadInt();
				}
				goto case 6;
				case 6:
				case 5:
				case 4:
				case 3:
					RespawnOnStart = reader.ReadBool();
					goto case 2;
				case 2:
					KickOnDeath = reader.ReadBool();
					goto case 1;
				case 1:
				{
					GateLocation = new MapPoint(reader);
					Gate = reader.ReadItem<PvPTeamGate>();
				}
				goto case 0;
				case 0:
				{
					_Name = reader.ReadString();
					_MinCapacity = reader.ReadInt();
					_MaxCapacity = reader.ReadInt();
					_Color = reader.ReadInt();
					_HomeBase = reader.ReadPoint3D();
					_SpawnPoint = reader.ReadPoint3D();

					RespawnOnDeath = reader.ReadBool();
					RespawnDelay = reader.ReadTimeSpan();

					if (version < 8)
					{
						reader.ReadBlock(r => r.ReadType()); // Statistics
					}
				}
				break;
			}

			if (version < 4)
			{
				RespawnOnStart = true;
			}

			if (version < 1)
			{
				GateLocation = MapPoint.Empty;
			}

			if (Gate != null)
			{
				Gate.Team = this;
			}

			if (Battle == null)
			{
				Timer.DelayCall(Delete);
			}
		}
	}
}
