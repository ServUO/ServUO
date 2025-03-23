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
using System.Drawing;
using System.Linq;
using System.Text;

using Server;
using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	[PropertyObject]
	public class PvPProfile : IEnumerable<PvPProfileHistoryEntry>
	{
		[CommandProperty(AutoPvP.Access)]
		public bool Deleted { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public PlayerMobile Owner { get; private set; }

		private long _Points;

		[CommandProperty(AutoPvP.Access)]
		public long RawPoints { get => _Points; set => _Points = Math.Max(0, value); }

		[CommandProperty(AutoPvP.Access)]
		public long Points
		{
			get => RawPoints;
			set
			{
				var oldVal = RawPoints;

				RawPoints = value;

				if (oldVal != RawPoints)
				{
					OnPointsChanged(oldVal);
				}
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public List<PvPBattle> Subscriptions { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public long TotalDamageTaken => GetTotalDamageTaken();

		[CommandProperty(AutoPvP.Access)]
		public long TotalDamageDone => GetTotalDamageDone();

		[CommandProperty(AutoPvP.Access)]
		public long TotalHealingTaken => GetTotalHealingTaken();

		[CommandProperty(AutoPvP.Access)]
		public long TotalHealingDone => GetTotalHealingDone();

		[CommandProperty(AutoPvP.Access)]
		public long TotalDeaths => GetTotalDeaths();

		[CommandProperty(AutoPvP.Access)]
		public long TotalResurrections => GetTotalResurrections();

		[CommandProperty(AutoPvP.Access)]
		public long TotalKills => GetTotalKills();

		[CommandProperty(AutoPvP.Access)]
		public long TotalPointsGained => GetTotalPointsGained();

		[CommandProperty(AutoPvP.Access)]
		public long TotalPointsLost => GetTotalPointsLost();

		[CommandProperty(AutoPvP.Access)]
		public long TotalPoints => TotalPointsGained - TotalPointsLost;

		[CommandProperty(AutoPvP.Access)]
		public long TotalWins => GetTotalWins();

		[CommandProperty(AutoPvP.Access)]
		public long TotalLosses => GetTotalLosses();

		[CommandProperty(AutoPvP.Access)]
		public long TotalBattles => GetTotalBattles();

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPProfileHistory History { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public PvPProfileHistoryEntry Statistics => History.EnsureEntry();

		public PvPProfile(PlayerMobile owner)
		{
			Owner = owner;

			History = new PvPProfileHistory(this);

			Subscriptions = new List<PvPBattle>();

			SubscribeAllBattles();
		}

		public PvPProfile(GenericReader reader)
		{
			Deserialize(reader);
		}

		public void SubscribeAllBattles()
		{
			foreach (var battle in AutoPvP.GetBattles())
			{
				Subscribe(battle);
			}
		}

		public bool IsSubscribed(PvPBattle battle)
		{
			return battle != null && !battle.Deleted && Subscriptions.Contains(battle);
		}

		public void Subscribe(PvPBattle battle)
		{
			if (battle != null && !battle.Deleted && !battle.IsInternal)
			{
				Subscriptions.Update(battle);
			}
		}

		public void Unsubscribe(PvPBattle battle)
		{
			if (battle != null && !battle.Deleted)
			{
				Subscriptions.Remove(battle);
			}
		}

		protected virtual void OnPointsChanged(long oldVal)
		{
			if (_Points > oldVal)
			{
				OnPointsGained(_Points - oldVal);
			}
			else if (_Points < oldVal)
			{
				OnPointsLost(oldVal - _Points);
			}
		}

		protected virtual void OnPointsGained(long value)
		{
			Statistics.PointsGained += value;

			Owner.SendMessage("You have gained {0:#,0} Battle Point{1}!", value, value != 1 ? "s" : String.Empty);
		}

		protected virtual void OnPointsLost(long value)
		{
			Statistics.PointsLost += value;

			Owner.SendMessage("You have lost {0:#,0} Battle Point{1}!", value, value != 1 ? "s" : String.Empty);
		}

		public long GetTotalDamageTaken()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.DamageTaken);
		}

		public long GetTotalDamageDone()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.DamageDone);
		}

		public long GetTotalHealingTaken()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.HealingTaken);
		}

		public long GetTotalHealingDone()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.HealingDone);
		}

		public long GetTotalResurrections()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.Resurrections);
		}

		public long GetTotalDeaths()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.Deaths);
		}

		public long GetTotalKills()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.Kills);
		}

		public long GetTotalPointsGained()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.PointsGained);
		}

		public long GetTotalPointsLost()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.PointsLost);
		}

		public long GetTotalWins()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.Wins);
		}

		public long GetTotalLosses()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.Losses);
		}

		public long GetTotalBattles()
		{
			return History.Values.Aggregate(0L, (c, entry) => c + entry.Battles);
		}

		public IEnumerable<KeyValuePair<string, long>> GetMiscStatisticTotals()
		{
			return History.Values.SelectMany(e => e.MiscStats)
						  .ToLookup(o => o.Key, o => o.Value)
						  .Select(o => new KeyValuePair<string, long>(o.Key, o.Aggregate(0L, (c, v) => c + v)));
		}

		public void Remove()
		{
			if (AutoPvP.RemoveProfile(this))
			{
				OnRemoved();
			}
		}

		public void Delete()
		{
			if (Deleted)
			{
				return;
			}

			Deleted = true;

			Remove();

			History = null;

			OnDeleted();
		}

		protected virtual void OnDeleted()
		{ }

		protected virtual void OnRemoved()
		{ }

		public virtual void Init()
		{ }

		public string ToHtmlString(Mobile viewer = null, bool big = true)
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

		public virtual void GetHtmlString(Mobile viewer, StringBuilder html)
		{
			html.Append("".WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));

			if (Deleted)
			{
				html.Append("<B>This profile has been deleted.</B>");
				return;
			}

			html.AppendLine("<B>PvP Profile for {0}</B>", Owner.RawName);
			html.AppendLine();

			html.Append("".WrapUOHtmlColor(Color.YellowGreen, false));
			html.AppendLine("<B>Statistics</B>");
			html.AppendLine();

			html.AppendLine("* Overall Points: {0}", _Points.ToString("#,0"));
			html.AppendLine();

			Statistics.GetHtmlString(viewer, html);

			html.Append("".WrapUOHtmlColor(Color.Cyan, false));
			html.AppendLine("<B>Statisctics For All Seasons:</B>");
			html.AppendLine();

			html.AppendLine("<B>Main Statistic Totals</B>");
			html.AppendLine();

			html.AppendLine("* Battles Attended: {0}", TotalBattles.ToString("#,0"));
			html.AppendLine("* Battles Won: {0}", TotalWins.ToString("#,0"));
			html.AppendLine("* Battles Lost: {0}", TotalLosses.ToString("#,0"));
			html.AppendLine("* Points Gained: {0}", TotalPointsGained.ToString("#,0"));
			html.AppendLine("* Points Lost: {0}", TotalPointsLost.ToString("#,0"));
			html.AppendLine("* Kills: {0}", TotalKills.ToString("#,0"));
			html.AppendLine("* Deaths: {0}", TotalDeaths.ToString("#,0"));
			html.AppendLine("* Resurrections: {0}", TotalResurrections.ToString("#,0"));
			html.AppendLine("* Damage Taken: {0}", TotalDamageTaken.ToString("#,0"));
			html.AppendLine("* Damage Done: {0}", TotalDamageDone.ToString("#,0"));
			html.AppendLine("* Healing Taken: {0}", TotalHealingTaken.ToString("#,0"));
			html.AppendLine("* Healing Done: {0}", TotalHealingDone.ToString("#,0"));
			html.AppendLine();

			html.Append("".WrapUOHtmlColor(Color.GreenYellow, false));
			html.AppendLine("<B>Misc Statistic Totals</B>");
			html.AppendLine();

			foreach (var kvp in GetMiscStatisticTotals())
			{
				html.AppendLine("* {0}: {1}", kvp.Key, kvp.Value.ToString("#,0"));
			}

			html.AppendLine();
			html.Append("".WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));
		}

		public IEnumerator<PvPProfileHistoryEntry> GetEnumerator()
		{
			return History.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			writer.Write(Deleted);
			writer.Write(Owner);

			switch (version)
			{
				case 0:
				{
					writer.Write(_Points);

					writer.WriteBlock(w => w.WriteType(History, t => History.Serialize(w)));

					writer.WriteBlockList(Subscriptions, (w, b) => w.WriteType(b.Serial, t => b.Serial.Serialize(w)));
				}
				break;
			}
		}

		public void Deserialize(GenericReader reader)
		{
			var version = reader.ReadInt();

			Deleted = reader.ReadBool();
			Owner = reader.ReadMobile<PlayerMobile>();

			switch (version)
			{
				case 0:
				{
					_Points = reader.ReadLong();

					History = reader.ReadBlock(r => r.ReadTypeCreate<PvPProfileHistory>(this, r)) ?? new PvPProfileHistory(this);

					Subscriptions = reader.ReadBlockList(
						r =>
						{
							var serial = r.ReadTypeCreate<PvPSerial>(r) ?? new PvPSerial(r);

							return AutoPvP.FindBattleByID(serial);
						},
						Subscriptions);
				}
				break;
			}
		}
	}
}