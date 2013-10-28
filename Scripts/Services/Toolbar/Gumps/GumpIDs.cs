#region Header
// **********
// ServUO - GumpIDs.cs
// **********
#endregion

namespace Services.Toolbar.Gumps
{
	public class GumpIDs
	{
		public enum MiscIDs
		{
			Background = 0,
			Color = 1,
			Buttonground = 2,
			ButtonOffset = 3,
		}

		public enum ButtonIDs
		{
			Minimize = 0,
			Maximize = 1,
			Customize = 2,
			SpecialCommand = 3,

			Send = 4,
			Teleport = 5,
			Gate = 6,
		}

		public static int Skins = 2;

		public static string[] Fonts = new[]
		{
			"", "<b>", "<i>", "<b><i>", "<small>", "<b><small>", "<i><small>", "<b><i><small>", "<big>", "<b><big>", "<i><big>",
			"<b><i><big>"
		};

		public static GumpIDs[] Misc = new[]
		{
			new GumpIDs(0, "Background", new[,] {{9200}, {9270}}), new GumpIDs(1, "Color", new[,] {{0}, {0}}),
			new GumpIDs(2, "Buttonground", new[,] {{9200}, {9350}}), new GumpIDs(3, "ButtonOffset", new[,] {{5}, {7}})
		};

		public static GumpIDs[] Buttons = new[]
		{
			new GumpIDs(0, "Minimize", new[,] {{5603, 5607, 16, 16}, {5537, 5539, 19, 21}}),
			new GumpIDs(1, "Maximize", new[,] {{5601, 5605, 16, 16}, {5540, 5542, 19, 21}}),
			new GumpIDs(2, "Customize", new[,] {{22153, 22155, 16, 16}, {5525, 5527, 62, 24}}),
			new GumpIDs(3, "SpecialCommand", new[,] {{2117, 2118, 15, 15}, {9720, 9722, 29, 29}}),
			new GumpIDs(4, "Send", new[,] {{2360, 2360, 11, 11}, {2360, 2360, 11, 11}}),
			new GumpIDs(5, "Teleport", new[,] {{2361, 2361, 11, 11}, {2361, 2361, 11, 11}}),
			new GumpIDs(6, "Gate", new[,] {{2362, 2362, 11, 11}, {2361, 2361, 11, 11}})
		};

		public int ID { get; set; }
		public int[,] Content { get; set; }
		public string Name { get; set; }

		public GumpIDs(int iD, string name, int[,] content)
		{
			ID = iD;
			Content = content;
			Name = name;
		}
	}
}