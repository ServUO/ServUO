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
using System.Linq;

using Server;

using VitaNex.IO;
#endregion

namespace VitaNex.Modules.AutoDonate
{
	[PropertyObject]
	public sealed class DonationStatistics
	{
		[CommandProperty(AutoDonate.Access)]
		public DataStoreStatus ProfileStatus => AutoDonate.Profiles.Status;

		[CommandProperty(AutoDonate.Access)]
		public int ProfileCount => AutoDonate.Profiles.Count;

		[CommandProperty(AutoDonate.Access)]
		public double TotalIncome => ProfileCount <= 0 ? 0 : AutoDonate.Profiles.Values.Sum(p => p.TotalValue);

		[CommandProperty(AutoDonate.Access)]
		public long TotalCredit => ProfileCount <= 0 ? 0 : AutoDonate.Profiles.Values.Sum(p => p.TotalCredit);

		[CommandProperty(AutoDonate.Access)]
		public int TierMin => ProfileCount <= 0 ? 0 : AutoDonate.Profiles.Values.Min(p => p.Tier);

		[CommandProperty(AutoDonate.Access)]
		public int TierMax => ProfileCount <= 0 ? 0 : AutoDonate.Profiles.Values.Max(p => p.Tier);

		[CommandProperty(AutoDonate.Access)]
		public double TierAverage => ProfileCount <= 0 ? 0 : AutoDonate.Profiles.Values.Average(p => p.Tier);

		[CommandProperty(AutoDonate.Access)]
		public DataStoreStatus TransStatus => AutoDonate.Transactions.Status;

		[CommandProperty(AutoDonate.Access)]
		public int TransCount => AutoDonate.Transactions.Count;

		[CommandProperty(AutoDonate.Access)]
		public double TransMin => TransCount <= 0 ? 0 : AutoDonate.Transactions.Values.Min(p => p.Total);

		[CommandProperty(AutoDonate.Access)]
		public double TransMax => TransCount <= 0 ? 0 : AutoDonate.Transactions.Values.Max(p => p.Total);

		[CommandProperty(AutoDonate.Access)]
		public double TransAverage => TransCount <= 0 ? 0 : AutoDonate.Transactions.Values.Average(t => t.Total);

		[CommandProperty(AutoDonate.Access)]
		public long CreditMin => TransCount <= 0 ? 0 : AutoDonate.Transactions.Values.Min(p => p.Credit);

		[CommandProperty(AutoDonate.Access)]
		public long CreditMax => TransCount <= 0 ? 0 : AutoDonate.Transactions.Values.Max(p => p.Credit);

		[CommandProperty(AutoDonate.Access)]
		public double CreditAverage => TransCount <= 0 ? 0 : AutoDonate.Transactions.Values.Average(t => t.Credit);

		public override string ToString()
		{
			return "Donation Statistics";
		}
	}
}