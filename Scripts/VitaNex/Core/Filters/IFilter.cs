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
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Server
{
	public interface IFilter
	{
		string Name { get; }
		bool IsDefault { get; }
		FilterOptions Options { get; }

		bool Filter(object obj);

		IEnumerable Shake(IEnumerable objects);

		void Clear();

		void Serialize(GenericWriter writer);
		void Deserialize(GenericReader reader);
	}

	public interface IFilter<T> : IFilter
	{
		bool Filter(T obj);

		IEnumerable<T> Shake(IEnumerable<T> objects);
	}
}