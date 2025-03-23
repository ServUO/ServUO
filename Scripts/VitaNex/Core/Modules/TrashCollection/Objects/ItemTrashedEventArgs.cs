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

namespace VitaNex.Modules.TrashCollection
{
	public class ItemTrashedEventArgs : EventArgs
	{
		public ItemTrashedEventArgs(BaseTrashHandler handler, Mobile from, Item trashed, int tokens, bool message)
		{
			Handler = handler;
			Mobile = from;
			Trashed = trashed;
			Tokens = tokens;
			Message = message;
		}

		public BaseTrashHandler Handler { get; private set; }
		public Mobile Mobile { get; private set; }
		public Item Trashed { get; private set; }
		public int Tokens { get; set; }
		public bool Message { get; set; }
		public bool HandledTokens { get; set; }
	}
}