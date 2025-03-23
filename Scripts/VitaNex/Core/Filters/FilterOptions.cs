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
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server
{
	public class FilterOptions : List<FilterOption>
	{
		public IEnumerable<FilterOption> this[string category] => this.Where(e => Insensitive.Equals(e.Category, category));

		public IEnumerable<FilterOption> this[string category, string name] => this.Where(e => Insensitive.Equals(e.Category, category) && Insensitive.Equals(e.Name, name));

		public IEnumerable<string> Categories => this.ToLookup(e => e.Category, e => e).Select(g => g.Key);

		public FilterOptions()
		{ }

		public FilterOptions(int capacity)
			: base(capacity)
		{ }

		public FilterOptions(IEnumerable<FilterOption> entries)
			: base(entries)
		{ }

		public void Add(string category)
		{
			Add(new FilterOption(category));
		}

		public void Add(string category, string name, string property, object value, bool isDefault)
		{
			Add(new FilterOption(category, name, property, value, isDefault));
		}

		public bool Remove(string category)
		{
			return RemoveAll(e => Insensitive.Equals(e.Category, category)) > 0;
		}

		public bool Remove(string category, string name)
		{
			return RemoveAll(e => Insensitive.Equals(e.Category, category) && Insensitive.Equals(e.Name, name)) > 0;
		}

		public int Total(string category)
		{
			return this.Count(e => Insensitive.Equals(e.Category, category));
		}

		public int Total(string category, string name)
		{
			return this.Count(e => Insensitive.Equals(e.Category, category) && Insensitive.Equals(e.Name, name));
		}
	}
}