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
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class DigitalNumericDisplayGump : SuperGump
	{
		public int NumericWidth { get; set; }
		public int NumericHeight { get; set; }
		public int[] Numerics { get; set; }
		public int DisplayID { get; set; }
		public int NumericDisplayID { get; set; }

		public DigitalNumericDisplayGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			int numericWidth = 30,
			int numericHeight = 60,
			int displayID = 10004,
			int numericDisplayID = 30073,
			params int[] numbers)
			: base(user, parent, x, y)
		{
			NumericWidth = numericWidth;
			NumericHeight = numericHeight;
			Numerics = numbers ?? new int[0];
			DisplayID = displayID;
			NumericDisplayID = numericDisplayID;

			ForceRecompile = true;
		}

		public static bool HasLines(ref int numeric, LCDLines lines)
		{
			return LCD.HasLines(Math.Max(0, Math.Min(9, numeric)), lines);
		}

		protected override void Compile()
		{
			for (var i = 0; i < Numerics.Length; i++)
			{
				Numerics[i] = Math.Max(0, Math.Min(9, Numerics[i]));
			}

			base.Compile();
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			int lineHeight = (NumericHeight / 16),
				lineWidth = (NumericWidth / 9),
				halfHeight = (NumericHeight / 2),
				halfWidth = (NumericWidth / 2),
				eachWidth = (NumericWidth + halfWidth),
				width = 20 + (eachWidth * Numerics.Length),
				height = 20 + NumericHeight;

			layout.Add("background/body/base", () => AddBackground(0, 0, width, height, 9270));

			CompileNumericEntries(layout, width, height, lineHeight, lineWidth, halfHeight, halfWidth, eachWidth);
		}

		protected virtual void CompileNumericEntries(
			SuperGumpLayout layout,
			int width,
			int height,
			int lineHeight,
			int lineWidth,
			int halfHeight,
			int halfWidth,
			int eachWidth)
		{
			for (var idx = 0; idx < Numerics.Length; idx++)
			{
				CompileNumericEntry(
					layout,
					lineHeight,
					lineWidth,
					halfHeight,
					halfWidth,
					idx,
					Numerics[idx],
					10 + (eachWidth * idx),
					10);
			}
		}

		protected virtual void CompileNumericEntry(
			SuperGumpLayout layout,
			int lineHeight,
			int lineWidth,
			int halfHeight,
			int halfWidth,
			int index,
			int numeric,
			int xOffset,
			int yOffset)
		{
			layout.Add(
				"imagetiled/bg" + index,
				() => AddImageTiled(xOffset, 10, NumericWidth + halfWidth, NumericHeight, DisplayID));

			if (HasLines(ref numeric, LCDLines.Top))
			{
				layout.Add(
					"imagetiled/lines/t" + index,
					() => AddImageTiled(xOffset, yOffset, NumericWidth, lineHeight, NumericDisplayID));
			}

			if (HasLines(ref numeric, LCDLines.TopLeft))
			{
				layout.Add(
					"imagetiled/lines/tl" + index,
					() => AddImageTiled(xOffset, yOffset, lineWidth, halfHeight, NumericDisplayID));
			}

			if (HasLines(ref numeric, LCDLines.TopRight))
			{
				layout.Add(
					"imagetiled/lines/tr" + index,
					() => AddImageTiled(xOffset + (NumericWidth - lineWidth), yOffset, lineWidth, halfHeight, NumericDisplayID));
			}

			if (HasLines(ref numeric, LCDLines.Middle))
			{
				layout.Add(
					"imagetiled/lines/m" + index,
					() => AddImageTiled(
						xOffset,
						yOffset + (halfHeight - (lineHeight / 2)),
						NumericWidth,
						lineHeight,
						NumericDisplayID));
			}

			if (HasLines(ref numeric, LCDLines.BottomLeft))
			{
				layout.Add(
					"imagetiled/lines/bl" + index,
					() => AddImageTiled(xOffset, yOffset + halfHeight, lineWidth, halfHeight, 30073));
			}

			if (HasLines(ref numeric, LCDLines.BottomRight))
			{
				layout.Add(
					"imagetiled/lines/br" + index,
					() => AddImageTiled(
						xOffset + (NumericWidth - lineWidth),
						yOffset + halfHeight,
						lineWidth,
						halfHeight,
						NumericDisplayID));
			}

			if (HasLines(ref numeric, LCDLines.Bottom))
			{
				layout.Add(
					"imagetiled/lines/b" + index,
					() => AddImageTiled(xOffset, yOffset + (NumericHeight - lineHeight), NumericWidth, lineHeight, NumericDisplayID));
			}
		}
	}
}