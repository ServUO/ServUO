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

using Server;
#endregion

namespace VitaNex
{
	[Flags]
	public enum ExpansionFlags
	{
		None = 0x0,

		T2A = 0x1,
		UOR = 0x2,
		UOTD = 0x4,
		LBR = 0x8,
		AOS = 0x10,
		SE = 0x20,
		ML = 0x40,
		SA = 0x80,
		HS = 0x100,
		TOL = 0x200,
		EJ = 0x400,

		PreUOR = T2A,
		PostUOR = UOR | UOTD | LBR | AOS | SE | ML | SA | HS | TOL | EJ,

		PreAOS = T2A | UOR | UOTD | LBR,
		PostAOS = AOS | SE | ML | SA | HS | TOL | EJ,

		PreSE = T2A | UOR | UOTD | LBR | AOS,
		PostSE = SE | ML | SA | HS | TOL | EJ,

		PreML = T2A | UOR | UOTD | LBR | AOS | SE,
		PostML = ML | SA | HS | TOL | EJ,

		PreSA = T2A | UOR | UOTD | LBR | AOS | SE | ML,
		PostSA = SA | HS | TOL | EJ,

		PreHS = T2A | UOR | UOTD | LBR | AOS | SE | ML | SA,
		PostHS = HS | TOL | EJ,

		PreTOL = T2A | UOR | UOTD | LBR | AOS | SE | ML | SA | HS,
		PostTOL = TOL | EJ,

		PreEJ = T2A | UOR | UOTD | LBR | AOS | SE | ML | SA | HS | TOL,
		PostEJ = EJ,

		All = ~None
	}

	public interface IExpansionCheck
	{
		ExpansionFlags Expansions { get; }
	}

	public static class ExpansionFlagsExtension
	{
		public static bool CheckExpansion(this IExpansionCheck o)
		{
			return o != null && CheckExpansion(o.Expansions);
		}

		public static bool CheckExpansion(this IExpansionCheck o, Expansion ex)
		{
			return o != null && CheckExpansion(o.Expansions, ex);
		}

		public static string GetName(this ExpansionFlags flags)
		{
			return GetName(flags, ", ");
		}

		public static string GetName(this ExpansionFlags flags, string separator)
		{
			if (flags == ExpansionFlags.None)
			{
				return "None";
			}

			return String.Join(separator, flags.EnumerateValues<ExpansionFlags>(true).Not(s => s == ExpansionFlags.None));
		}

		public static bool CheckExpansion(this ExpansionFlags flags)
		{
			return CheckExpansion(flags, Core.Expansion);
		}

		public static bool CheckExpansion(this ExpansionFlags flags, Expansion ex)
		{
			if (flags == ExpansionFlags.None)
			{
				return true;
			}

			switch (ex)
			{
				case Expansion.T2A:
					return flags.HasFlag(ExpansionFlags.T2A);
				case Expansion.UOR:
					return flags.HasFlag(ExpansionFlags.UOR);
				case Expansion.UOTD:
					return flags.HasFlag(ExpansionFlags.UOTD);
				case Expansion.LBR:
					return flags.HasFlag(ExpansionFlags.LBR);
				case Expansion.AOS:
					return flags.HasFlag(ExpansionFlags.AOS);
				case Expansion.SE:
					return flags.HasFlag(ExpansionFlags.SE);
				case Expansion.ML:
					return flags.HasFlag(ExpansionFlags.ML);
				case Expansion.SA:
					return flags.HasFlag(ExpansionFlags.SA);
				case Expansion.HS:
					return flags.HasFlag(ExpansionFlags.HS);
				case Expansion.TOL:
					return flags.HasFlag(ExpansionFlags.TOL);
				case Expansion.EJ:
					return flags.HasFlag(ExpansionFlags.EJ);
				default:
					return false;
			}
		}
	}
}