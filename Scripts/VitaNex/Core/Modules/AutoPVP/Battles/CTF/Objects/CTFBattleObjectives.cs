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

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class CTFBattleObjectives : PvPBattleObjectives
	{
		[CommandProperty(AutoPvP.Access)]
		public long FlagsCaptured { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long FlagsDropped { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long FlagsStolen { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public long FlagsReturned { get; set; }

		public override bool IsEmpty => base.IsEmpty && FlagsCaptured + FlagsDropped + FlagsStolen + FlagsReturned <= 0;

		public CTFBattleObjectives()
		{ }

		public CTFBattleObjectives(GenericReader reader)
			: base(reader)
		{ }

		public override void SetDefaults()
		{
			base.SetDefaults();

			FlagsCaptured = 0;
			FlagsDropped = 0;
			FlagsStolen = 0;
			FlagsReturned = 0;
		}

		public override double ComputeScore(PvPTeam t, ref double min, ref double max, ref double total)
		{
			var score = base.ComputeScore(t, ref min, ref max, ref total);

			if (t == null || t.Deleted)
			{
				return score;
			}

			var ct = t as CTFTeam;

			if (ct == null)
			{
				return score;
			}

			double val;

			if (FlagsCaptured > 0)
			{
				score += val = Math.Min(1.0, ct.GetTotalFlagsCaptured() / (double)FlagsCaptured);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (FlagsDropped > 0)
			{
				score += val = Math.Min(1.0, ct.GetTotalFlagsDropped() / (double)FlagsDropped);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (FlagsStolen > 0)
			{
				score += val = Math.Min(1.0, ct.GetTotalFlagsStolen() / (double)FlagsStolen);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (FlagsReturned > 0)
			{
				score += val = Math.Min(1.0, ct.GetTotalFlagsReturned() / (double)FlagsReturned);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			return score;
		}

		public override double ComputeScore(PvPBattle b, PlayerMobile p, ref double min, ref double max, ref double total)
		{
			var score = base.ComputeScore(b, p, ref min, ref max, ref total);

			if (b == null || b.Deleted)
			{
				return score;
			}

			if (p == null || p.Deleted)
			{
				return score;
			}

			double val;

			if (FlagsCaptured > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o["Flags Captured"]) / (double)FlagsCaptured);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (FlagsDropped > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o["Flags Dropped"]) / (double)FlagsDropped);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (FlagsStolen > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o["Flags Stolen"]) / (double)FlagsStolen);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			if (FlagsReturned > 0)
			{
				score += val = Math.Min(1.0, b.GetStatistic(p, o => o["Flags Returned"]) / (double)FlagsReturned);

				min = Math.Min(min, val);
				max = Math.Max(max, val);

				++total;
			}

			return score;
		}

		public override string GetStatus(PvPTeam t)
		{
			var status = base.GetStatus(t);

			if (t == null || t.Deleted)
			{
				return status;
			}

			var ct = t as CTFTeam;

			if (ct == null)
			{
				return status;
			}

			var lines = new StringBuilder(status);

			if (FlagsCaptured > 0)
			{
				lines.AppendLine("Flags Captured: {0:#,0} / {1:#,0}", ct.GetTotalFlagsCaptured(), FlagsCaptured);
			}

			if (FlagsDropped > 0)
			{
				lines.AppendLine("Flags Dropped: {0:#,0} / {1:#,0}", ct.GetTotalFlagsDropped(), FlagsDropped);
			}

			if (FlagsStolen > 0)
			{
				lines.AppendLine("Flags Stolen: {0:#,0} / {1:#,0}", ct.GetTotalFlagsStolen(), FlagsStolen);
			}

			if (FlagsReturned > 0)
			{
				lines.AppendLine("Flags Returned: {0:#,0} / {1:#,0}", ct.GetTotalFlagsReturned(), FlagsReturned);
			}

			return lines.ToString();
		}

		public override string GetStatus(PvPBattle b, PlayerMobile p)
		{
			var status = base.GetStatus(b, p);

			if (b == null || b.Deleted)
			{
				return status;
			}

			if (p == null || p.Deleted)
			{
				return status;
			}

			var lines = new StringBuilder(status);

			if (FlagsCaptured > 0)
			{
				lines.AppendLine("Flags Captured: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o["Flags Captured"]), FlagsCaptured);
			}

			if (FlagsDropped > 0)
			{
				lines.AppendLine("Flags Dropped: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o["Flags Dropped"]), FlagsDropped);
			}

			if (FlagsStolen > 0)
			{
				lines.AppendLine("Flags Stolen: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o["Flags Stolen"]), FlagsStolen);
			}

			if (FlagsReturned > 0)
			{
				lines.AppendLine("Flags Returned: {0:#,0} / {1:#,0}", b.GetStatistic(p, o => o["Flags Returned"]), FlagsReturned);
			}

			return lines.ToString();
		}

		public override void GetHtmlString(StringBuilder html)
		{
			base.GetHtmlString(html);

			var len = html.Length;

			if (FlagsCaptured > 0)
			{
				html.AppendLine("Flags Captured: {0:#,0}", FlagsCaptured);
			}

			if (FlagsDropped > 0)
			{
				html.AppendLine("Flags Dropped: {0:#,0}", FlagsDropped);
			}

			if (FlagsStolen > 0)
			{
				html.AppendLine("Flags Stolen: {0:#,0}", FlagsStolen);
			}

			if (FlagsReturned > 0)
			{
				html.AppendLine("Flags Returned: {0:#,0}", FlagsReturned);
			}

			if (len < html.Length)
			{
				html.Insert(len, String.Empty.WrapUOHtmlColor(Color.PaleGoldenrod, false));
				html.AppendLine(String.Empty.WrapUOHtmlColor(Color.White, false));
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(FlagsCaptured);
			writer.Write(FlagsDropped);
			writer.Write(FlagsStolen);
			writer.Write(FlagsReturned);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			FlagsCaptured = reader.ReadLong();
			FlagsDropped = reader.ReadLong();
			FlagsStolen = reader.ReadLong();
			FlagsReturned = reader.ReadLong();
		}
	}
}