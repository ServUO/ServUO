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

namespace VitaNex.Network
{
	public enum Season
	{
		/// <summary>
		///     Spring: 0
		/// </summary>
		Spring = 0,

		/// <summary>
		///     Summer: 1
		/// </summary>
		Summer = 1,

		/// <summary>
		///     Autumn: 2
		/// </summary>
		Autumn = 2,

		/// <summary>
		///     Winter: 3
		/// </summary>
		Winter = 3,

		/// <summary>
		///     Desolation: 4
		/// </summary>
		Desolation = 4,

		/// <summary>
		///     Fall: 2
		/// </summary>
		Fall = Autumn
	}

	public static class SeasonEnumExt
	{
		public static int GetID(this Season season)
		{
			return (int)season;
		}

		public static string GetName(this Season season)
		{
			return season.ToString();
		}
	}
}