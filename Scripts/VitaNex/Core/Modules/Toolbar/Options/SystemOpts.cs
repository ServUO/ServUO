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
using Server.Commands;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.Toolbar
{
	public sealed class ToolbarsOptions : CoreModuleOptions
	{
		private int _DefaultX;
		private int _DefaultY;
		private int _DefaultHeight;
		private int _DefaultWidth;

		private string _PopupCommand;
		private string _PositionCommand;

		[CommandProperty(Toolbars.Access)]
		public int DefaultX { get => _DefaultX; set => _DefaultX = Math.Max(0, value); }

		[CommandProperty(Toolbars.Access)]
		public int DefaultY { get => _DefaultY; set => _DefaultY = Math.Max(0, value); }

		[CommandProperty(Toolbars.Access)]
		public int DefaultWidth { get => _DefaultWidth; set => _DefaultWidth = Math.Max(1, value); }

		[CommandProperty(Toolbars.Access)]
		public int DefaultHeight { get => _DefaultHeight; set => _DefaultHeight = Math.Max(1, value); }

		[CommandProperty(Toolbars.Access)]
		public ToolbarTheme DefaultTheme { get; set; }

		[CommandProperty(Toolbars.Access)]
		public string PositionCommand
		{
			get => _PositionCommand;
			set => CommandUtility.Replace(_PositionCommand, AccessLevel.Player, HandlePositionCommand, (_PositionCommand = value));
		}

		[CommandProperty(Toolbars.Access)]
		public string PopupCommand
		{
			get => _PopupCommand;
			set => CommandUtility.Replace(_PopupCommand, AccessLevel.Player, HandlePopupCommand, (_PopupCommand = value));
		}

		[CommandProperty(Toolbars.Access)]
		public bool LoginPopup { get; set; }

		[CommandProperty(Toolbars.Access)]
		public AccessLevel Access { get; set; }

		public ToolbarsOptions()
			: base(typeof(Toolbars))
		{
			DefaultX = 0;
			DefaultY = 28;
			DefaultWidth = 6;
			DefaultHeight = 4;

			DefaultTheme = ToolbarTheme.Default;

			PositionCommand = "ToolbarPos";
			PopupCommand = "Toolbar";

			LoginPopup = false;
			Access = Toolbars.Access;
		}

		public ToolbarsOptions(GenericReader reader)
			: base(reader)
		{ }

		public void HandlePositionCommand(CommandEventArgs e)
		{
			var user = e.Mobile as PlayerMobile;

			if (user == null || user.Deleted || user.NetState == null || !ModuleEnabled)
			{
				return;
			}

			if (user.AccessLevel < Access)
			{
				if (user.AccessLevel > AccessLevel.Player)
				{
					user.SendMessage("You do not have access to that command.");
				}

				return;
			}

			var tb = Toolbars.EnsureState(user).GetToolbarGump();
			SuperGump.Send(
				new OffsetSelectorGump(
					user,
					tb.Refresh(true),
					Toolbars.GetOffset(user),
					(self, oldValue) =>
					{
						Toolbars.SetOffset(user, self.Value);
						tb.X = self.Value.X;
						tb.Y = self.Value.Y;
						tb.Refresh(true);
					}));
		}

		public void HandlePopupCommand(CommandEventArgs e)
		{
			var user = e.Mobile as PlayerMobile;

			if (user == null || user.Deleted || user.NetState == null || !ModuleEnabled)
			{
				return;
			}

			if (user.AccessLevel < Access)
			{
				if (user.AccessLevel > AccessLevel.Player)
				{
					user.SendMessage("You do not have access to that command.");
				}

				return;
			}

			SuperGump.Send(Toolbars.EnsureState(user).GetToolbarGump());
		}

		public override void Clear()
		{
			base.Clear();

			DefaultX = 0;
			DefaultY = 28;
			DefaultWidth = 1;
			DefaultHeight = 1;

			DefaultTheme = ToolbarTheme.Default;

			PositionCommand = null;
			PopupCommand = null;

			LoginPopup = false;
			Access = Toolbars.Access;
		}

		public override void Reset()
		{
			base.Reset();

			DefaultX = 0;
			DefaultY = 28;
			DefaultWidth = 6;
			DefaultHeight = 4;

			DefaultTheme = ToolbarTheme.Default;

			PositionCommand = "ToolbarPos";
			PopupCommand = "Toolbar";

			LoginPopup = false;
			Access = Toolbars.Access;
		}

		public override string ToString()
		{
			return "Toolbars Config";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(2);

			switch (version)
			{
				case 2:
				{
					writer.WriteFlag(DefaultTheme);

					writer.Write(DefaultX);
					writer.Write(DefaultY);
				}
				goto case 1;
				case 1:
				{
					writer.WriteFlag(Access);
					writer.Write(LoginPopup);
				}
				goto case 0;
				case 0:
				{
					writer.Write(DefaultWidth);
					writer.Write(DefaultHeight);
					writer.Write(PositionCommand);
					writer.Write(PopupCommand);

					writer.WriteBlock(Toolbars.DefaultEntries.Serialize);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 2:
				{
					DefaultTheme = reader.ReadFlag<ToolbarTheme>();

					DefaultX = reader.ReadInt();
					DefaultY = reader.ReadInt();
				}
				goto case 1;
				case 1:
				{
					Access = reader.ReadFlag<AccessLevel>();
					LoginPopup = reader.ReadBool();
				}
				goto case 0;
				case 0:
				{
					DefaultWidth = reader.ReadInt();
					DefaultHeight = reader.ReadInt();
					PositionCommand = reader.ReadString();
					PopupCommand = reader.ReadString();

					reader.ReadBlock(Toolbars.DefaultEntries.Deserialize);
				}
				break;
			}

			if (version < 2)
			{
				DefaultTheme = ToolbarTheme.Default;

				DefaultX = 0;
				DefaultY = 28;
			}

			if (version < 1)
			{
				Access = Toolbars.Access;
			}
		}
	}
}