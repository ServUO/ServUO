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
#endregion

namespace VitaNex.Modules.Toolbar
{
	public enum ToolbarTheme
	{
		Default = 0,
		Paper,
		Stone
	}

	public static class ToolbarThemes
	{
		public static ToolbarThemeBase GetTheme(ToolbarTheme theme)
		{
			switch (theme)
			{
				case ToolbarTheme.Paper:
					return ToolbarThemePaper.Instance;
				case ToolbarTheme.Stone:
					return ToolbarThemeStone.Instance;
				default:
					return ToolbarThemeDefault.Instance;
			}
		}
	}

	public abstract class ToolbarThemeBase
	{
		public abstract ToolbarTheme ThemeID { get; }

		public abstract string Name { get; }

		public abstract Color TitleLabelColor { get; }
		public abstract int TitleBackground { get; }

		public abstract int EntrySeparator { get; }
		public abstract int EntryBackgroundN { get; }
		public abstract Color EntryLabelColorN { get; }
		public abstract int EntryBackgroundH { get; }
		public abstract Color EntryLabelColorH { get; }

		public abstract int EntryOptionsN { get; }
		public abstract int EntryOptionsP { get; }
	}

	public sealed class ToolbarThemeDefault : ToolbarThemeBase
	{
		private static readonly ToolbarThemeDefault _Instance = new ToolbarThemeDefault();

		public static ToolbarThemeDefault Instance => _Instance;
		public override ToolbarTheme ThemeID => ToolbarTheme.Default;
		public override string Name => "Default";

		public override int TitleBackground => 9274;
		public override Color TitleLabelColor => Color.Gold;
		public override int EntrySeparator => 9790;
		public override int EntryBackgroundN => 9274;
		public override Color EntryLabelColorN => Color.Gold;
		public override int EntryBackgroundH => 9204;
		public override Color EntryLabelColorH => Color.LightBlue;
		public override int EntryOptionsN => 9791;
		public override int EntryOptionsP => 9790;
	}

	public sealed class ToolbarThemePaper : ToolbarThemeBase
	{
		private static readonly ToolbarThemePaper _Instance = new ToolbarThemePaper();

		public static ToolbarThemePaper Instance => _Instance;
		public override ToolbarTheme ThemeID => ToolbarTheme.Paper;
		public override string Name => "Paper";

		public override int TitleBackground => 9394;
		public override Color TitleLabelColor => Color.DarkSlateGray;
		public override int EntrySeparator => 11340;
		public override int EntryBackgroundN => 9394;
		public override Color EntryLabelColorN => Color.DarkSlateGray;
		public override int EntryBackgroundH => 9384;
		public override Color EntryLabelColorH => Color.Chocolate;
		public override int EntryOptionsN => 11350;
		public override int EntryOptionsP => 11340;
	}

	public sealed class ToolbarThemeStone : ToolbarThemeBase
	{
		private static readonly ToolbarThemeStone _Instance = new ToolbarThemeStone();

		public static ToolbarThemeStone Instance => _Instance;
		public override ToolbarTheme ThemeID => ToolbarTheme.Stone;
		public override string Name => "Stone";

		public override int TitleBackground => 5124;
		public override Color TitleLabelColor => Color.GhostWhite;
		public override int EntrySeparator => 11340;
		public override int EntryBackgroundN => 5124;
		public override Color EntryLabelColorN => Color.GhostWhite;
		public override int EntryBackgroundH => 9204;
		public override Color EntryLabelColorH => Color.Cyan;
		public override int EntryOptionsN => 11374;
		public override int EntryOptionsP => 11340;
	}
}