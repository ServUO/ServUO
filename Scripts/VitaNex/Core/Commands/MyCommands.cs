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
using System.Globalization;
using System.Linq;

using Server;
using Server.Commands;
using Server.Gumps;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Commands
{
	public static class MyCommandsCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register("MyCommands", AccessLevel.Player, e => new MyCommandsGump(e.Mobile).Send());
		}
	}

	public class MyCommandsGump : ListGump<CommandEntry>
	{
		public MyCommandsGump(Mobile user, Gump parent = null)
			: base(user, parent, title: "My Commands", emptyText: "No commands to display.")
		{
			Sorted = true;
			Modal = false;
			CanMove = false;
			CanResize = false;
		}

		public override string GetSearchKeyFor(CommandEntry key)
		{
			return key != null ? key.Command : base.GetSearchKeyFor(null);
		}

		public override int SortCompare(CommandEntry a, CommandEntry b)
		{
			var res = 0;

			if (a.CompareNull(b, ref res))
			{
				return res;
			}

			if (a.AccessLevel > b.AccessLevel)
			{
				return -1;
			}

			if (a.AccessLevel < b.AccessLevel)
			{
				return 1;
			}

			return String.Compare(a.Command, b.Command, StringComparison.OrdinalIgnoreCase);
		}

		protected override void CompileList(List<CommandEntry> list)
		{
			list.Clear();

			var commands = CommandUtility.EnumerateCommands(User.AccessLevel);

			commands = commands.Where(c => !Insensitive.Equals(c.Command, "MyCommands"));

			list.AddRange(commands);

			base.CompileList(list);
		}

		protected override void SelectEntry(GumpButton button, CommandEntry entry)
		{
			base.SelectEntry(button, entry);

			User.SendMessage(0x55, "Using Command: {0}", entry.Command);
			CommandSystem.Handle(User, String.Format("{0}{1}", CommandSystem.Prefix, entry.Command));
			Refresh();
		}

		protected override string GetLabelText(int index, int pageIndex, CommandEntry entry)
		{
			return entry != null && !String.IsNullOrWhiteSpace(entry.Command)
				? entry.Command[0].ToString(CultureInfo.InvariantCulture).ToUpper() + entry.Command.Substring(1)
				: base.GetLabelText(index, pageIndex, entry);
		}

		protected override int GetLabelHue(int index, int pageIndex, CommandEntry entry)
		{
			if (entry == null)
			{
				return base.GetLabelHue(index, pageIndex, null);
			}

			if (entry.AccessLevel >= AccessLevel.Administrator)
			{
				return 0x516;
			}

			if (entry.AccessLevel > AccessLevel.GameMaster)
			{
				return 0x144;
			}

			if (entry.AccessLevel > AccessLevel.Counselor)
			{
				return 0x21;
			}

			if (entry.AccessLevel > AccessLevel.Player)
			{
				return 0x30;
			}

			return TextHue;
		}
	}
}