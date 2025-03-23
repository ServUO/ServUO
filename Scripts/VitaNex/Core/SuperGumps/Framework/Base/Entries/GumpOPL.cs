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
using Server.Gumps;
using Server.Network;
#endregion

namespace VitaNex.SuperGumps
{
	public class GumpOPL : SuperGumpEntry
	{
		private const string _Format1 = "{{ itemproperty {0} }}";

		private static readonly byte[] _Layout1 = Gump.StringToBuffer("itemproperty");

		private int _Serial;

		public int Serial { get => _Serial; set => Delta(ref _Serial, value); }

		public GumpOPL(Serial serial)
		{
			_Serial = serial;
		}

		public override string Compile()
		{
			return String.Format(_Format1, _Serial);
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(_Layout1);
			disp.AppendLayout(_Serial);
		}
	}
}