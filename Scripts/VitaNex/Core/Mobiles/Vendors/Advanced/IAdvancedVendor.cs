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

namespace VitaNex.Mobiles
{
	public interface IAdvancedVendor : IVendor
	{
		AdvancedSBInfo AdvancedStock { get; }

		int Discount { get; set; }
		bool DiscountEnabled { get; set; }
		bool DiscountYell { get; set; }

		ObjectProperty CashProperty { get; set; }
		TypeSelectProperty<object> CashType { get; set; }

		TextDefinition CashName { get; set; }
		TextDefinition CashAbbr { get; set; }

		bool ShowCashName { get; set; }

		bool Trading { get; set; }
		bool CanRestock { get; set; }

		void ResolveCurrency(out Type type, out TextDefinition name);
	}
}