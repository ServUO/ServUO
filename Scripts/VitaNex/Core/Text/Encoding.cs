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
using System.Text;
#endregion

namespace VitaNex.Text
{
	public enum EncodingType
	{
		Default,
		ASCII,
		Unicode,
		BigEndianUnicode,
		UTF7,
		UTF8,
		UTF32
	}

	public static class EncodingUtility
	{
		public static Encoding GetEncoding(this EncodingType e)
		{
			switch (e)
			{
				case EncodingType.ASCII:
					return Encoding.ASCII;
				case EncodingType.Unicode:
					return Encoding.Unicode;
				case EncodingType.BigEndianUnicode:
					return Encoding.BigEndianUnicode;
				case EncodingType.UTF7:
					return Encoding.UTF7;
				case EncodingType.UTF8:
					return Encoding.UTF8;
				case EncodingType.UTF32:
					return Encoding.UTF32;
				default:
					return Encoding.Default;
			}
		}
	}
}