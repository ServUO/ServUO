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
using System.Text;

using Server;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPProfileHistoryEntry : PropertyObject, IEquatable<PvPProfileHistoryEntry>
	{
		[CommandProperty(AutoPvP.Access)]
		public PvPSerial UID { get; private set; }

		[CommandProperty(AutoPvP.Access, true)]
		public int Season { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual long Battles { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual long Wins { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual long Losses { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual long PointsGained { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual long PointsLost { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long Points => PointsGained - PointsLost;

		[CommandProperty(AutoPvP.Access)]
		public virtual long DamageTaken { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual long DamageDone { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual long HealingTaken { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual long HealingDone { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual long Kills { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual long Deaths { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual long Resurrections { get; set; }

		public virtual Dictionary<string, long> MiscStats { get; protected set; }

		public virtual long this[string stat]
		{
			get => MiscStats.GetValue(stat);
			set
			{
				if (String.IsNullOrWhiteSpace(stat))
				{
					return;
				}

				if (MiscStats.ContainsKey(stat))
				{
					if (value < 0)
					{
						MiscStats.Remove(stat);
					}
					else
					{
						MiscStats[stat] = value;
					}
				}
				else if (value >= 0)
				{
					MiscStats[stat] = value;
				}
			}
		}

		public PvPProfileHistoryEntry(int season)
		{
			Season = season;

			UID = new PvPSerial(Season + "~" + TimeStamp.UtcNow + "~" + Utility.RandomDouble());

			MiscStats = new Dictionary<string, long>();
		}

		public PvPProfileHistoryEntry(GenericReader reader)
			: base(reader)
		{ }

		public virtual void SetDefaults()
		{
			Battles = 0;
			Wins = 0;
			Losses = 0;
			PointsGained = 0;
			PointsLost = 0;

			DamageTaken = 0;
			DamageDone = 0;
			HealingTaken = 0;
			HealingDone = 0;
			Kills = 0;
			Deaths = 0;
			Resurrections = 0;

			MiscStats.Clear();
		}

		public override void Clear()
		{
			SetDefaults();
		}

		public override void Reset()
		{
			SetDefaults();
		}

		public virtual long GetMiscStat(string stat)
		{
			return this[stat];
		}

		public virtual void SetMiscStat(string stat, long value)
		{
			this[stat] = value;
		}

		public void MergeFrom(PvPProfileHistoryEntry e, bool points)
		{
			if (e != null)
			{
				e.MergeTo(this, points);
			}
		}

		public void MergeTo(PvPProfileHistoryEntry e, bool points)
		{
			if (e == null)
			{
				return;
			}

			AddTo(e, points);

			Battles = 0;
			Wins = 0;
			Losses = 0;

			if (points)
			{
				PointsGained = 0;
				PointsLost = 0;
			}

			DamageTaken = 0;
			DamageDone = 0;
			HealingTaken = 0;
			HealingDone = 0;
			Kills = 0;
			Deaths = 0;
			Resurrections = 0;

			MiscStats.Clear();
		}

		public void TakeFrom(PvPProfileHistoryEntry e, bool points)
		{
			if (e != null)
			{
				e.AddTo(this, points);
			}
		}

		public void AddTo(PvPProfileHistoryEntry e, bool points)
		{
			if (e == null)
			{
				return;
			}

			e.Battles += Battles;
			e.Wins += Wins;
			e.Losses += Losses;

			if (points)
			{
				e.PointsGained += PointsGained;
				e.PointsLost += PointsLost;
			}

			e.DamageTaken += DamageTaken;
			e.DamageDone += DamageDone;
			e.HealingTaken += HealingTaken;
			e.HealingDone += HealingDone;
			e.Kills += Kills;
			e.Deaths += Deaths;
			e.Resurrections += Resurrections;

			foreach (var o in MiscStats)
			{
				e[o.Key] += o.Value;
			}
		}

		public string ToHtmlString(bool big)
		{
			return ToHtmlString(null, big);
		}

		public string ToHtmlString(Mobile viewer, bool big)
		{
			var sb = new StringBuilder();

			if (big)
			{
				sb.Append("<BIG>");
			}

			GetHtmlString(viewer, sb);

			if (big)
			{
				sb.Append("</BIG>");
			}

			return sb.ToString();
		}

		public virtual void GetHtmlString(Mobile viewer, StringBuilder html)
		{
			html.Append(String.Empty.WrapUOHtmlColor(Color.Cyan, false));
			html.AppendLine("Statistics For Season: {0:#,0}".WrapUOHtmlBold(), Season);
			html.AppendLine();

			html.AppendLine("Statistics:".WrapUOHtmlBold());
			html.AppendLine();

			html.AppendLine("Battles Attended: {0:#,0}", Battles);
			html.AppendLine("Battles Won: {0:#,0}", Wins);
			html.AppendLine("Battles Lost: {0:#,0}", Losses);
			html.AppendLine();
			html.AppendLine("Points Total: {0:#,0}", Points);
			html.AppendLine("Points Gained: {0:#,0}", PointsGained);
			html.AppendLine("Points Lost: {0:#,0}", PointsLost);
			html.AppendLine();
			html.AppendLine("Kills: {0:#,0}", Kills);
			html.AppendLine("Deaths: {0:#,0}", Deaths);
			html.AppendLine("Resurrections: {0:#,0}", Resurrections);
			html.AppendLine("Damage Taken: {0:#,0}", DamageTaken);
			html.AppendLine("Damage Given: {0:#,0}", DamageDone);
			html.AppendLine("Healing Taken: {0:#,0}", HealingTaken);
			html.AppendLine("Healing Given: {0:#,0}", HealingDone);
			html.AppendLine();

			html.Append(String.Empty.WrapUOHtmlColor(Color.GreenYellow, false));
			html.AppendLine("Misc Statistics:");

			foreach (var kvp in MiscStats)
			{
				html.AppendLine("{0}: {1:#,0}", kvp.Key, kvp.Value);
			}

			html.AppendLine();
			html.Append(String.Empty.WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));
		}

		public override sealed int GetHashCode()
		{
			return UID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is PvPProfileHistoryEntry && Equals((PvPProfileHistoryEntry)obj);
		}

		public virtual bool Equals(PvPProfileHistoryEntry other)
		{
			return !ReferenceEquals(other, null) && UID == other.UID;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			UID.Serialize(writer);

			switch (version)
			{
				case 1:
				case 0:
				{
					writer.Write(Season);

					writer.Write(DamageTaken);
					writer.Write(DamageDone);
					writer.Write(HealingTaken);
					writer.Write(HealingDone);
					writer.Write(Kills);
					writer.Write(Deaths);
					writer.Write(Resurrections);

					writer.Write(PointsGained);
					writer.Write(PointsLost);

					writer.Write(Wins);
					writer.Write(Losses);
					writer.Write(Battles);

					writer.WriteBlockDictionary(
						MiscStats,
						(w, k, v) =>
						{
							w.Write(k);
							w.Write(v);
						});
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			UID = version > 0 ? new PvPSerial(reader) : new PvPSerial(TimeStamp.UtcNow + "~" + Utility.RandomDouble());

			switch (version)
			{
				case 1:
				case 0:
				{
					Season = reader.ReadInt();

					DamageTaken = reader.ReadLong();
					DamageDone = reader.ReadLong();
					HealingTaken = reader.ReadLong();
					HealingDone = reader.ReadLong();
					Kills = reader.ReadLong();
					Deaths = reader.ReadLong();
					Resurrections = reader.ReadLong();

					PointsGained = reader.ReadLong();
					PointsLost = reader.ReadLong();

					Wins = reader.ReadLong();
					Losses = reader.ReadLong();
					Battles = reader.ReadLong();

					MiscStats = reader.ReadBlockDictionary(
						r =>
						{
							var k = r.ReadString();
							var v = r.ReadLong();

							return new KeyValuePair<string, long>(k, v);
						},
						MiscStats);
				}
				break;
			}
		}

		public static bool operator ==(PvPProfileHistoryEntry left, PvPProfileHistoryEntry right)
		{
			return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
		}

		public static bool operator !=(PvPProfileHistoryEntry left, PvPProfileHistoryEntry right)
		{
			return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : !left.Equals(right);
		}
	}
}