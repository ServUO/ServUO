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
using Server.Items;
#endregion

namespace VitaNex.Items
{
	public class LuckyDipBankCheckPrize : LuckyDipPrize
	{
		public int Worth => Args[0] as int? ?? 0;

		public LuckyDipBankCheckPrize()
			: this(0.0, 0)
		{ }

		public LuckyDipBankCheckPrize(double chance, int worth)
			: base(chance, typeof(BankCheck), worth)
		{ }
	}
}