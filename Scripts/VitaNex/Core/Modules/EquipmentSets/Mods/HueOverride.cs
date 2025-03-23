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

namespace VitaNex.Modules.EquipmentSets
{
	public class HueOverrideSetMod : EquipmentSetMod
	{
		public int Hue { get; private set; }

		public HueOverrideSetMod(int partsReq, bool display, int hue)
			: base("Taste The Rainbow", String.Empty, partsReq, display)
		{
			Hue = hue;
		}

		protected override bool OnActivate(Mobile m, Tuple<EquipmentSetPart, Item>[] equipped)
		{
			m.SolidHueOverride = Hue;
			return true;
		}

		protected override bool OnDeactivate(Mobile m, Tuple<EquipmentSetPart, Item>[] equipped)
		{
			m.SolidHueOverride = -1;
			return true;
		}
	}
}