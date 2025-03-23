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
using System.Drawing;
using System.Linq;

using Server;
using Server.Mobiles;

using VitaNex.IO;
using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.Toolbar
{
	public static partial class Toolbars
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static Type[] EntryTypes { get; private set; }

		public static ToolbarsOptions CMOptions { get; private set; }

		public static BinaryDataStore<PlayerMobile, ToolbarState> Profiles { get; private set; }

		public static ToolbarState DefaultEntries { get; set; }

		public static Action LoadDefaultEntries { get; set; }
		public static Action ClearDefaultEntries { get; set; }

		public static void RegisterEntry(int x, int y, ToolbarEntry entry)
		{
			DefaultEntries.SetContent(x, y, entry);
		}

		private static void ClearDefaults()
		{
			DefaultEntries = ToolbarState.NewEmpty;
		}

		private static void LoadDefaults()
		{
			ClearDefaults();

			RegisterEntry(0, 0, new ToolbarLink("http://core.vita-nex.com", "VitaNexCore", true, true, true));
			RegisterEntry(1, 0, new ToolbarCommand("MyCommands", "Command List"));
			RegisterEntry(2, 0, new ToolbarCommand("Pages", "View Pages", minAccess: AccessLevel.Counselor));
			RegisterEntry(3, 0, new ToolbarCommand("Go", "Go To...", minAccess: AccessLevel.Counselor));
			RegisterEntry(4, 0, new ToolbarCommand("Who", "Online List", minAccess: AccessLevel.Counselor));
			RegisterEntry(5, 0, new ToolbarCommand("Where", minAccess: AccessLevel.Counselor));

			RegisterEntry(0, 1, new ToolbarCommand("Self", "Hide", minAccess: AccessLevel.Counselor, args: "Hide"));
			RegisterEntry(1, 1, new ToolbarCommand("Self", "Unhide", minAccess: AccessLevel.Counselor, args: "Unhide"));
			RegisterEntry(2, 1, new ToolbarCommand("Tele", "Teleport", minAccess: AccessLevel.Counselor));
			RegisterEntry(3, 1, new ToolbarCommand("M", "Multi Teleport", minAccess: AccessLevel.Counselor, args: "Tele"));
			RegisterEntry(4, 1, new ToolbarCommand("Remove", minAccess: AccessLevel.GameMaster));
			RegisterEntry(5, 1, new ToolbarCommand("M", "Multi Remove", minAccess: AccessLevel.GameMaster, args: "Remove"));

			RegisterEntry(0, 2, new ToolbarCommand("Props", "Properties", minAccess: AccessLevel.Counselor));
			RegisterEntry(1, 2, new ToolbarCommand("Kill", minAccess: AccessLevel.GameMaster));
			RegisterEntry(2, 2, new ToolbarCommand("Admin", "Admin Panel", minAccess: AccessLevel.Administrator));
		}

		public static void OpenAll()
		{
			foreach (var p in Profiles.Where(
				p => p.Key != null && p.Value != null && p.Key.IsOnline() && p.Key.AccessLevel >= CMOptions.Access))
			{
				VitaNexCore.TryCatch(() => p.Value.GetToolbarGump().Send(), CMOptions.ToConsole);
			}
		}

		public static void CloseAll()
		{
			foreach (var p in Profiles.Where(p => p.Key != null && p.Value != null && p.Key.IsOnline()))
			{
				VitaNexCore.TryCatch(() => p.Value.GetToolbarGump().Close(true), CMOptions.ToConsole);
			}
		}

		private static void OnLogin(LoginEventArgs e)
		{
			var user = e.Mobile as PlayerMobile;

			if (user != null && !user.Deleted && user.NetState != null && user.AccessLevel >= CMOptions.Access &&
				CMOptions.LoginPopup)
			{
				SuperGump.Send(EnsureState(user).GetToolbarGump());
			}
		}

		public static Point GetOffset(PlayerMobile user)
		{
			var loc = new Point(0, 28);

			if (user == null || user.Deleted)
			{
				return loc;
			}

			if (Profiles.ContainsKey(user))
			{
				loc = new Point(Profiles[user].X, Profiles[user].Y);
			}

			return loc;
		}

		public static void SetOffset(PlayerMobile user, Point loc)
		{
			if (user == null || user.Deleted || !Profiles.ContainsKey(user))
			{
				return;
			}

			Profiles[user].X = loc.X;
			Profiles[user].Y = loc.Y;
		}

		public static void SetGlobalPosition()
		{
			VitaNexCore.TryCatch(
				() => Profiles.Values.ForEach(
					state => VitaNexCore.TryCatch(
						() =>
						{
							state.SetDefaultPosition();

							var tb = state.GetToolbarGump();

							if (tb != null && tb.IsOpen)
							{
								tb.Refresh(true);
							}
						},
						CMOptions.ToConsole)),
				CMOptions.ToConsole);
		}

		public static void SetGlobalSize()
		{
			VitaNexCore.TryCatch(
				() => Profiles.Values.ForEach(
					state => VitaNexCore.TryCatch(
						() =>
						{
							state.SetDefaultSize();

							var tb = state.GetToolbarGump();

							if (tb != null && tb.IsOpen)
							{
								tb.Refresh(true);
							}
						},
						CMOptions.ToConsole)),
				CMOptions.ToConsole);
		}

		public static void SetGlobalTheme()
		{
			VitaNexCore.TryCatch(
				() => Profiles.Values.ForEach(
					state => VitaNexCore.TryCatch(
						() =>
						{
							state.SetDefaultTheme();

							var tb = state.GetToolbarGump();

							if (tb != null && tb.IsOpen)
							{
								tb.Refresh(true);
							}
						},
						CMOptions.ToConsole)),
				CMOptions.ToConsole);
		}

		public static void SetGlobalEntries()
		{
			VitaNexCore.TryCatch(
				() => Profiles.Values.ForEach(
					state => VitaNexCore.TryCatch(
						() =>
						{
							state.SetDefaultEntries();

							var tb = state.GetToolbarGump();

							if (tb != null && tb.IsOpen)
							{
								tb.Refresh(true);
							}
						},
						CMOptions.ToConsole)),
				CMOptions.ToConsole);
		}

		public static void SetGlobalDefaults()
		{
			VitaNexCore.TryCatch(
				() => Profiles.Values.ForEach(
					state => VitaNexCore.TryCatch(
						() =>
						{
							state.SetDefaults();

							var tb = state.GetToolbarGump();

							if (tb != null && tb.IsOpen)
							{
								tb.Refresh(true);
							}
						},
						CMOptions.ToConsole)),
				CMOptions.ToConsole);
		}

		public static ToolbarState EnsureState(PlayerMobile user)
		{
			if (user == null)
			{
				return null;
			}

			if (!Profiles.TryGetValue(user, out var state) || state == null)
			{
				Profiles[user] = state = new ToolbarState(user);
			}

			return state;
		}
	}
}