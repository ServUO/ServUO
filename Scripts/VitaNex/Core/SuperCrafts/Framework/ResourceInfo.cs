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

namespace VitaNex.SuperCrafts
{
	public class ResourceInfo
	{
		public Type TypeOf { get; private set; }
		public TextDefinition Name { get; private set; }
		public int Amount { get; private set; }

		public ResourceInfo(Type t, TextDefinition name, int amount)
		{
			TypeOf = t;
			Name = name.IsNullOrWhiteSpace() ? new TextDefinition("Unknown") : name;
			Amount = Math.Max(1, amount);
		}
	}
}