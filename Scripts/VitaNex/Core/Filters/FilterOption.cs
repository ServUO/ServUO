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
#endregion

namespace Server
{
	public struct FilterOption
	{
		public string Category { get; private set; }
		public string Name { get; private set; }

		public string Property { get; private set; }
		public object Value { get; private set; }

		public bool IsDefault { get; private set; }

		public bool IsEmpty => String.IsNullOrWhiteSpace(Name) || String.IsNullOrWhiteSpace(Property);

		public FilterOption(string category)
			: this(category, String.Empty, String.Empty, null, false)
		{ }

		public FilterOption(string category, string name, string property, object value, bool isDefault)
			: this()
		{
			Category = category;
			Name = name;
			Property = property;
			Value = value;
			IsDefault = isDefault;
		}

		public bool IsSelected(IFilter filter)
		{
			if (filter != null && !String.IsNullOrWhiteSpace(Property))
			{
				if (filter.GetPropertyValue(Property, out var value))
				{
					return Equals(value, Value);
				}
			}

			return false;
		}

		public bool Select(IFilter filter)
		{
			if (filter != null && !String.IsNullOrWhiteSpace(Property))
			{
				return filter.SetPropertyValue(Property, Value);
			}

			return false;
		}
	}
}