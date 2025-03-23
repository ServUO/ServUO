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
using System.Collections.Generic;

using Server;
using Server.Gumps;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public class TrashHandlerIgnoreListGump : TypeListGump
	{
		public TrashHandlerIgnoreListGump(Mobile user, BaseTrashHandler handler, Gump parent = null)
			: base(
				user,
				parent,
				list: handler.Ignored,
				title: handler.GetType().Name + " Ignore List",
				emptyText: "There are no Types in the list.")
		{
			TrashHandler = handler;
		}

		public BaseTrashHandler TrashHandler { get; set; }

		public override List<Type> GetExternalList()
		{
			return TrashHandler.Ignored;
		}
	}
}