using System;
using Server;

///////////////////
//  This is an attempt to define gumps as easier-to-use constants
///////////////////

namespace Server.Gumps
{
	public class GumpUtil
	{
		public const int Background_BlackEdged = 0x0053;
		public const int Background_Scroll = 0x09D8;
		public const int Background_OrnamentedGrey = 0x0A28;
		public const int Background_BlackGoldBorder = 0x0A3C;
		public const int Background_Parchment = 0x0BB8;
		public const int Background_FancyParchment = 0x0DAC;
		public const int Background_DarkBlue = 0x0E10;
		public const int Background_PlainGrey = 0x13BE;
		public const int Background_RoughGrey = 0x13EC;
		public const int Background_DarkGrey = 0x1400;
		public const int Background_FancyScroll = 0x1432;
		public const int Background_LightGrey = 0x2486;

		public const int Background_PureBlack = 0x0A40;

		public const int GoldArrowUp1 = 0x15E0;
		public const int GoldArrowUp2 = 0x15E4;
		public const int GoldArrowRight1 = 0x15E1;
		public const int GoldArrowRight2 = 0x15E5;

		public const int GoldArrowDown1 = 0x15E2;
		public const int GoldArrowDown2 = 0x15E6;
		public const int GoldArrowLeft1 = 0x15E3;
		public const int GoldArrowLeft2 = 0x15E7;

		public const int ScrollUpArrow = 0x0824;
		public const int ScrollDownArrow = 0x0825;


		public const int ButtonSmallBlueUp = 0x0837;
		public const int ButtonSmallBlueDown = 0x0838;
		public const int ButtonBlueUp = 0x0845;
		public const int ButtonBlueDown = 0x0846;
		public const int ButtonRed = 0x0938;
		public const int ButtonGreen = 0x0939;
		public const int ButtonBlue = 0x093A;

		public const int ButtonCancelUp = 0x0849;
		public const int ButtonCancelDown = 0x0848;
		public const int ButtonApplyUp = 0x084C;
		public const int ButtonApplyDown = 0x084B;
		public const int ButtonOKUp = 0x0852;
		public const int ButtonOKDown = 0x0851;


		public const int ButtonGreenOKUp = 0x00F9;
		public const int ButtonGreenOKDown = 0x00F8;
		public const int ButtonRedCancelUp = 0x00F3;
		public const int ButtonRedCancelDown = 0x00F1;


		public const int BookBackground1 = 0x0898;
		public const int BookBackground2 = 0x0899;
		public const int BookBackground3 = 0x089A;
		public const int BookBackground4 = 0x089B;
		public const int BookLastPage = 0x089D;
		public const int BookNextPage = 0x089E;

		public const int GoldBordedBook = 0x01F4;
		public const int GoldBordedBookLastPage = 0x01F5;
		public const int GoldBordedBookNextPage = 0x01F6;


		public const int CheckBoxBlueOff = 0x0868;
		public const int CheckBoxBlueOn = 0x086A;
		public const int CheckBoxPlusOff = 0x09CE;
		public const int CheckBoxPlusOn = 0x09CF;


		public const int BUTTONID_NEXT_PAGE = 1000;
		public const int BUTTONID_LAST_PAGE = 1001;
		public const int BUTTONID_CONFIRM = 1002;
		public const int BUTTONID_MINIMIZE = 1003;
		public const int BUTTONID_HELP = 1010;




		/// <summary>
		/// A good standard hue for light-colored text on a black background
		/// </summary>
		private const int LabelHue = 0x480;


		/// <summary>
		/// Try to provide standard name hues (based on accesslevel) across all gumps
		/// </summary>
		/// <param name="m">The mobile who's hue we want to know</param>
		/// <returns></returns>
		public static int GetHueFor(Mobile m)
		{
			if (m == null)
				return LabelHue;

			switch (m.AccessLevel)
			{
				case AccessLevel.Owner:
				case AccessLevel.Developer:
				case AccessLevel.Administrator: return 0x516;
				case AccessLevel.Seer: return 0x144;
				case AccessLevel.GameMaster: return 0x21;
				case AccessLevel.Counselor: return 0x2;
				case AccessLevel.Player:
				default:
					{
						if (m.Kills >= 5)
							return 0x21;
						else if (m.Criminal)
							return 0x3B1;

						return 0x58;
					}
			}
		}
	}
}