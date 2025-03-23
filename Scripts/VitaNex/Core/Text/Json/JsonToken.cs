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

namespace VitaNex.Text
{
	public enum JsonToken
	{
		None = 0,
		ObjectOpen,
		ObjectClose,
		ArrayOpen,
		ArrayClose,
		Colon,
		Comma,
		String,
		Number,
		True,
		False,
		Null
	}
}