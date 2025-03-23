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

using Server.Gumps;
using Server.Network;
#endregion

namespace VitaNex.SuperGumps
{
	public class GumpInputEC : SuperGumpEntry
	{
		private static readonly byte[] _LayoutName = Gump.StringToBuffer("echandleinput");

		public override string Compile()
		{
			if (!IsEnhancedClient)
			{
				return String.Empty;
			}

			return "{{ echandleinput }}";
		}

		public override void AppendTo(IGumpWriter disp)
		{
			if (!IsEnhancedClient)
			{
				AppendEmptyLayout(disp);
				return;
			}

			disp.AppendLayout(_LayoutName);
		}
	}
}