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
using System.Drawing;

using Server;
using Server.Commands;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.CastBars
{
	public sealed class CastBarsOptions : CoreModuleOptions
	{
		private string _PositionCommand;
		private string _ToggleCommand;

		[CommandProperty(SpellCastBars.Access)]
		public string PositionCommand
		{
			get => _PositionCommand;
			set => CommandUtility.Replace(_PositionCommand, AccessLevel.Player, HandlePositionCommand, (_PositionCommand = value));
		}

		[CommandProperty(SpellCastBars.Access)]
		public string ToggleCommand
		{
			get => _ToggleCommand;
			set => CommandUtility.Replace(_ToggleCommand, AccessLevel.Player, HandleToggleCommand, (_ToggleCommand = value));
		}

		[CommandProperty(SpellCastBars.Access)]
		public int GumpWidth { get; set; }

		[CommandProperty(SpellCastBars.Access)]
		public int GumpHeight { get; set; }

		[CommandProperty(SpellCastBars.Access)]
		public int GumpPadding { get; set; }

		[CommandProperty(SpellCastBars.Access)]
		public int GumpBackground { get; set; }

		[CommandProperty(SpellCastBars.Access)]
		public int GumpForeground { get; set; }

		[CommandProperty(SpellCastBars.Access)]
		public ProgressBarFlow GumpFlow { get; set; }

		[CommandProperty(SpellCastBars.Access)]
		public KnownColor GumpTextColor { get; set; }

		[CommandProperty(SpellCastBars.Access)]
		public bool GumpDisplayPercent { get; set; }

		[CommandProperty(SpellCastBars.Access)]
		public bool GumpDisplayText { get; set; }

		public CastBarsOptions()
			: base(typeof(SpellCastBars))
		{
			PositionCommand = "CastBarPos";
			ToggleCommand = "CastBarToggle";

			GumpWidth = ProgressBarGump.DefaultWidth;
			GumpHeight = ProgressBarGump.DefaultHeight;

			GumpPadding = ProgressBarGump.DefaultPadding;

			GumpBackground = ProgressBarGump.DefaultBackgroundID;
			GumpForeground = ProgressBarGump.DefaultForegroundID;

			GumpFlow = ProgressBarGump.DefaultFlow;
			GumpTextColor = SuperGump.DefaultHtmlColor.ToKnownColor();

			GumpDisplayPercent = false;
			GumpDisplayText = true;
		}

		public CastBarsOptions(GenericReader reader)
			: base(reader)
		{ }

		public void HandlePositionCommand(CommandEventArgs e)
		{
			SpellCastBars.HandlePositionCommand(e.Mobile as PlayerMobile);
		}

		public void HandleToggleCommand(CommandEventArgs e)
		{
			SpellCastBars.HandleToggleCommand(e.Mobile as PlayerMobile);
		}

		public override void Clear()
		{
			base.Clear();

			PositionCommand = null;
			ToggleCommand = null;

			GumpWidth = ProgressBarGump.DefaultWidth;
			GumpHeight = ProgressBarGump.DefaultHeight;

			GumpPadding = ProgressBarGump.DefaultPadding;

			GumpBackground = ProgressBarGump.DefaultBackgroundID;
			GumpForeground = ProgressBarGump.DefaultForegroundID;

			GumpFlow = ProgressBarGump.DefaultFlow;
			GumpTextColor = SuperGump.DefaultHtmlColor.ToKnownColor();

			GumpDisplayPercent = false;
			GumpDisplayText = true;
		}

		public override void Reset()
		{
			base.Reset();

			PositionCommand = "CastBarPos";
			ToggleCommand = "CastBarToggle";

			GumpWidth = ProgressBarGump.DefaultWidth;
			GumpHeight = ProgressBarGump.DefaultHeight;

			GumpPadding = ProgressBarGump.DefaultPadding;

			GumpBackground = ProgressBarGump.DefaultBackgroundID;
			GumpForeground = ProgressBarGump.DefaultForegroundID;

			GumpFlow = ProgressBarGump.DefaultFlow;
			GumpTextColor = SuperGump.DefaultHtmlColor.ToKnownColor();

			GumpDisplayPercent = false;
			GumpDisplayText = true;
		}

		public override string ToString()
		{
			return "Cast-Bars Config";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				{
					writer.Write(GumpWidth);
					writer.Write(GumpHeight);
					writer.Write(GumpPadding);
					writer.Write(GumpBackground);
					writer.Write(GumpForeground);
					writer.WriteFlag(GumpFlow);
					writer.WriteFlag(GumpTextColor);
					writer.Write(GumpDisplayPercent);
					writer.Write(GumpDisplayText);
				}
				goto case 0;
				case 0:
				{
					writer.Write(PositionCommand);
					writer.Write(ToggleCommand);
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
				case 1:
				{
					GumpWidth = reader.ReadInt();
					GumpHeight = reader.ReadInt();
					GumpPadding = reader.ReadInt();
					GumpBackground = reader.ReadInt();
					GumpForeground = reader.ReadInt();
					GumpFlow = reader.ReadFlag<ProgressBarFlow>();
					GumpTextColor = reader.ReadFlag<KnownColor>();
					GumpDisplayPercent = reader.ReadBool();
					GumpDisplayText = reader.ReadBool();
				}
				goto case 0;
				case 0:
				{
					PositionCommand = reader.ReadString();
					ToggleCommand = reader.ReadString();
				}
				break;
			}

			if (version > 0)
			{
				return;
			}

			GumpWidth = ProgressBarGump.DefaultWidth;
			GumpHeight = ProgressBarGump.DefaultHeight;

			GumpPadding = ProgressBarGump.DefaultPadding;

			GumpBackground = ProgressBarGump.DefaultBackgroundID;
			GumpForeground = ProgressBarGump.DefaultForegroundID;

			GumpFlow = ProgressBarGump.DefaultFlow;
			GumpTextColor = SuperGump.DefaultHtmlColor.ToKnownColor();

			GumpDisplayPercent = false;
			GumpDisplayText = true;
		}
	}
}