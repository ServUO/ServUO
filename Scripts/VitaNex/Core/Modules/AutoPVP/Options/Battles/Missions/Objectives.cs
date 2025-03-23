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
using System.Drawing;
using System.Text;

using Server;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPBattleObjectives : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public bool AllRequired { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long PointsTotal { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long PointsGained { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long PointsLost { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long Kills { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long Deaths { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long Resurrections { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long DamageTaken { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long DamageDone { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long HealingTaken { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long HealingDone { get; set; }

		public virtual bool IsEmpty => PointsTotal + PointsGained + PointsLost
					 + Kills + Deaths + Resurrections
					 + DamageTaken + DamageDone
					 + HealingTaken + HealingDone
					<= 0;

		public PvPBattleObjectives()
		{
			SetDefaults();
		}

		public PvPBattleObjectives(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			base.Clear();

			SetDefaults();
		}

		public override void Reset()
		{
			base.Reset();

			SetDefaults();
		}

		public virtual void SetDefaults()
		{
			AllRequired = false;

			PointsTotal = 0;
			PointsGained = 0;
			PointsLost = 0;
			Kills = 0;
			Deaths = 0;
			Resurrections = 0;
			DamageTaken = 0;
			DamageDone = 0;
			HealingTaken = 0;
			HealingDone = 0;
		}

		public double ComputeScorePotential(PvPTeam t)
		{
			double min = 0, max = 0, total = 0;

			ComputeScore(t, ref min, ref max, ref total);

			return total;
		}

		public double ComputeScore(PvPTeam t)
		{
			double min = 0, max = 0, total = 0;

			return ComputeScore(t, ref min, ref max, ref total);
		}

		public virtual double ComputeScore(PvPTeam t, ref double min, ref double max, ref double total)
		{
			if (t == null || t.Deleted)
			{
				return 0;
			}

			double val, score = 0.0;

			if (PointsTotal > 0)
			{
				score += val = Math.Min(1.0, t.GetTotalPoints() / (double)PointsTotal);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (PointsGained > 0)
			{
				score += val = Math.Min(1.0, t.GetTotalPointsGained() / (double)PointsGained);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (PointsLost > 0)
			{
				score += val = Math.Min(1.0, t.GetTotalPointsLost() / (double)PointsLost);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (Kills > 0)
			{
				score += val = Math.Min(1.0, t.GetTotalKills() / (double)Kills);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (Deaths > 0)
			{
				score += val = Math.Min(1.0, t.GetTotalDeaths() / (double)Deaths);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (Resurrections > 0)
			{
				score += val = Math.Min(1.0, t.GetTotalResurrections() / (double)Resurrections);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (DamageTaken > 0)
			{
				score += val = Math.Min(1.0, t.GetTotalDamageTaken() / (double)DamageTaken);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (DamageDone > 0)
			{
				score += val = Math.Min(1.0, t.GetTotalDamageDone() / (double)DamageDone);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (HealingTaken > 0)
			{
				score += val = Math.Min(1.0, t.GetTotalHealingTaken() / (double)HealingTaken);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (HealingDone > 0)
			{
				score += val = Math.Min(1.0, t.GetTotalHealingDone() / (double)HealingDone);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			return score;
		}

		public double ComputeScorePotential(PvPBattle b, PlayerMobile p)
		{
			double min = 0, max = 0, total = 0;

			ComputeScore(b, p, ref min, ref max, ref total);

			return total;
		}

		public double ComputeScore(PvPBattle b, PlayerMobile p)
		{
			double min = 0, max = 0, total = 0;

			return ComputeScore(b, p, ref min, ref max, ref total);
		}

		public virtual double ComputeScore(PvPBattle b, PlayerMobile p, ref double min, ref double max, ref double total)
		{
			if (b == null || b.Deleted)
			{
				return 0;
			}

			if (p == null || p.Deleted)
			{
				return 0;
			}

			double val, score = 0.0;

			if (PointsTotal > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o.Points) / (double)PointsTotal);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (PointsGained > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o.PointsGained) / (double)PointsGained);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (PointsLost > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o.PointsLost) / (double)PointsLost);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (Kills > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o.Kills) / (double)Kills);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (Deaths > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o.Deaths) / (double)Deaths);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (Resurrections > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o.Resurrections) / (double)Resurrections);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (DamageTaken > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o.DamageTaken) / (double)DamageTaken);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (DamageDone > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o.DamageDone) / (double)DamageDone);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (HealingTaken > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o.HealingTaken) / (double)HealingTaken);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (HealingDone > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o.HealingDone) / (double)HealingDone);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			return score;
		}

		public virtual bool Completed(PvPTeam t)
		{
			double min = 0, max = 0, total = 0;

			var score = ComputeScore(t, ref min, ref max, ref total);

			if (score <= 0 || max <= 0)
			{
				return false;
			}

			if (AllRequired)
			{
				return score >= total;
			}

			return max >= 1.0;
		}

		public virtual bool Completed(PvPBattle b, PlayerMobile p)
		{
			double min = 0, max = 0, total = 0;

			var score = ComputeScore(b, p, ref min, ref max, ref total);

			if (score <= 0 || total <= 0)
			{
				return false;
			}

			if (AllRequired)
			{
				return score >= total;
			}

			return max >= 1.0;
		}

		public virtual string GetStatus(PvPTeam t)
		{
			if (t == null || t.Deleted)
			{
				return String.Empty;
			}

			var lines = new StringBuilder();

			if (PointsTotal > 0)
			{
				lines.AppendLine("Points Total: {0:#,0} / {1:#,0}", t.GetTotalPoints(), PointsTotal);
			}

			if (PointsGained > 0)
			{
				lines.AppendLine("Points Gained: {0:#,0} / {1:#,0}", t.GetTotalPointsGained(), PointsGained);
			}

			if (PointsLost > 0)
			{
				lines.AppendLine("Points Lost: {0:#,0} / {1:#,0}", t.GetTotalPointsLost(), PointsLost);
			}

			if (Kills > 0)
			{
				lines.AppendLine("Kills: {0:#,0} / {1:#,0}", t.GetTotalKills(), Kills);
			}

			if (Deaths > 0)
			{
				lines.AppendLine("Deaths: {0:#,0} / {1:#,0}", t.GetTotalDeaths(), Deaths);
			}

			if (Resurrections > 0)
			{
				lines.AppendLine("Resurrections: {0:#,0} / {1:#,0}", t.GetTotalResurrections(), Resurrections);
			}

			if (DamageTaken > 0)
			{
				lines.AppendLine("Damage Taken: {0:#,0} / {1:#,0}", t.GetTotalDamageTaken(), DamageTaken);
			}

			if (DamageDone > 0)
			{
				lines.AppendLine("Damage Done: {0:#,0} / {1:#,0}", t.GetTotalDamageDone(), DamageDone);
			}

			if (HealingTaken > 0)
			{
				lines.AppendLine("Healing Taken: {0:#,0} / {1:#,0}", t.GetTotalHealingTaken(), HealingTaken);
			}

			if (HealingDone > 0)
			{
				lines.AppendLine("Healing Done: {0:#,0} / {1:#,0}", t.GetTotalHealingDone(), HealingDone);
			}

			return lines.ToString();
		}

		public virtual string GetStatus(PvPBattle b, PlayerMobile p)
		{
			if (b == null || b.Deleted)
			{
				return String.Empty;
			}

			if (p == null || p.Deleted)
			{
				return String.Empty;
			}

			var lines = new StringBuilder();

			if (PointsTotal > 0)
			{
				lines.AppendLine("Points Total: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o.Points), PointsTotal);
			}

			if (PointsGained > 0)
			{
				lines.AppendLine("Points Gained: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o.PointsGained), PointsGained);
			}

			if (PointsLost > 0)
			{
				lines.AppendLine("Points Lost: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o.PointsLost), PointsLost);
			}

			if (Kills > 0)
			{
				lines.AppendLine("Kills: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o.Kills), Kills);
			}

			if (Deaths > 0)
			{
				lines.AppendLine("Deaths: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o.Deaths), Deaths);
			}

			if (Resurrections > 0)
			{
				lines.AppendLine("Resurrections: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o.Resurrections), Resurrections);
			}

			if (DamageTaken > 0)
			{
				lines.AppendLine("Damage Taken: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o.DamageTaken), DamageTaken);
			}

			if (DamageDone > 0)
			{
				lines.AppendLine("Damage Done: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o.DamageDone), DamageDone);
			}

			if (HealingTaken > 0)
			{
				lines.AppendLine("Healing Taken: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o.HealingTaken), HealingTaken);
			}

			if (HealingDone > 0)
			{
				lines.AppendLine("Healing Done: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o.HealingDone), HealingDone);
			}

			return lines.ToString();
		}

		public virtual void GetHtmlString(StringBuilder html)
		{
			var len = html.Length;

			if (PointsTotal > 0)
			{
				html.AppendLine("Points Total: {0:#,0}", PointsTotal);
			}

			if (PointsGained > 0)
			{
				html.AppendLine("Points Gained: {0:#,0}", PointsGained);
			}

			if (PointsLost > 0)
			{
				html.AppendLine("Points Lost: {0:#,0}", PointsLost);
			}

			if (Kills > 0)
			{
				html.AppendLine("Kills: {0:#,0}", Kills);
			}

			if (Deaths > 0)
			{
				html.AppendLine("Deaths: {0:#,0}", Deaths);
			}

			if (Resurrections > 0)
			{
				html.AppendLine("Resurrections: {0:#,0}", Resurrections);
			}

			if (DamageTaken > 0)
			{
				html.AppendLine("Damage Taken: {0:#,0}", DamageTaken);
			}

			if (DamageDone > 0)
			{
				html.AppendLine("Damage Done: {0:#,0}", DamageDone);
			}

			if (HealingTaken > 0)
			{
				html.AppendLine("Healing Taken: {0:#,0}", HealingTaken);
			}

			if (HealingDone > 0)
			{
				html.AppendLine("Healing Done: {0:#,0}", HealingDone);
			}

			if (len < html.Length)
			{
				html.Insert(len, AllRequired ? "(Complete All)\n" : "(Complete Any)\n");
				html.Insert(len, String.Empty.WrapUOHtmlColor(Color.PaleGoldenrod, false));
				html.Append(String.Empty.WrapUOHtmlColor(Color.White, false));
			}
		}

		public virtual string ToHtmlString()
		{
			var html = new StringBuilder();

			GetHtmlString(html);

			return html.ToString();
		}

		public override string ToString()
		{
			return "Mission Objectives";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(PointsTotal);
			writer.Write(PointsGained);
			writer.Write(PointsLost);
			writer.Write(Kills);
			writer.Write(Deaths);
			writer.Write(Resurrections);
			writer.Write(DamageTaken);
			writer.Write(DamageDone);
			writer.Write(HealingTaken);
			writer.Write(HealingDone);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			PointsTotal = reader.ReadLong();
			PointsGained = reader.ReadLong();
			PointsLost = reader.ReadLong();
			Kills = reader.ReadLong();
			Deaths = reader.ReadLong();
			Resurrections = reader.ReadLong();
			DamageTaken = reader.ReadLong();
			DamageDone = reader.ReadLong();
			HealingTaken = reader.ReadLong();
			HealingDone = reader.ReadLong();
		}
	}
}