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
#endregion

namespace VitaNex.Collections
{
	public sealed class GridPool<T> : ObjectPool<Grid<T>>
	{
		public GridPool()
		{ }

		public GridPool(int capacity)
			: base(capacity)
		{ }

		protected override bool Sanitize(Grid<T> o)
		{
			o.Resize(0, 0);

			return o.Count == 0;
		}
	}
}
