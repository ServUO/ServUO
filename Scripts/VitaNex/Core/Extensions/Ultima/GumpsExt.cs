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

#if ServUO58
#define ServUOX
#endif

#region References
using System.Drawing;
#endregion

namespace Ultima
{
	public static class GumpsExtUtility
	{
//#if ServUOX
//		public static Bitmap GetGump(int index)
//		{
//			return Server.GumpData.GetGump(index);
//		}

//		public static Bitmap GetGump(int index, int hue, bool onlyHueGrayPixels)
//		{
//			return Server.GumpData.GetGump(index, hue, onlyHueGrayPixels);
//		}

//        #region Ultima SDK Signatures

//		public static Bitmap GetGump(int index, out bool patched)
//		{
//			patched = false;

//			return GetGump(index);
//		}

//        #endregion
//#elif ServUO
        public static Bitmap GetGump(int index)
        {
            return GetGump(index, out _);
        }

        public static Bitmap GetGump(int index, out bool patched)
        {
			return Gumps.GetGump(index, out patched);
        }
//#else
//		public static Bitmap GetGump(int index)
//		{
//			return GetGump(index, out _);
//		}

//		public static Bitmap GetGump(int index, out bool patched)
//		{
//			var param = new object[] { index, false };

//			var img = Bootstrap.Invoke<Bitmap>("Gumps", "GetGump", param);

//			patched = (bool)param[1];

//			return img;
//		}
//#endif

        public static Size GetImageSize(int id)
		{
			var img = GetGump(id);

			if (img == null)
			{
				return new Size(0, 0);
			}

			return new Size(img.Width, img.Height);
		}

		public static int GetImageWidth(int id)
		{
			var img = GetGump(id);

			if (img == null)
			{
				return 0;
			}

			return img.Width;
		}

		public static int GetImageHeight(int id)
		{
			var img = GetGump(id);

			if (img == null)
			{
				return 0;
			}

			return img.Height;
		}
	}
}
