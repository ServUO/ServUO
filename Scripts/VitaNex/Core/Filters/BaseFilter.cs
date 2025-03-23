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
using System.Linq;
#endregion

namespace Server
{
	public abstract class BaseFilter : IFilter
	{
		public abstract string Name { get; }

		public abstract bool IsDefault { get; }

		public abstract FilterOptions Options { get; }

		public BaseFilter()
		{ }

		public BaseFilter(GenericReader reader)
		{
			Deserialize(reader);
		}

		public abstract void Clear();

		public virtual bool Filter(object obj)
		{
			return IsDefault || obj != null;
		}

		public virtual IEnumerable Shake(IEnumerable objects)
		{
			return objects.Cast<object>().Where(Filter);
		}

		public abstract void Serialize(GenericWriter writer);
		public abstract void Deserialize(GenericReader reader);
	}

	public abstract class BaseFilter<T> : BaseFilter, IFilter<T>
	{
		public BaseFilter()
		{ }

		public BaseFilter(GenericReader reader)
			: base(reader)
		{ }

		public virtual bool Filter(T obj)
		{
			return IsDefault || obj != null;
		}

		public override sealed bool Filter(object obj)
		{
			return obj is T && Filter((T)obj);
		}

		public virtual IEnumerable<T> Shake(IEnumerable<T> objects)
		{
			return objects.Where(Filter);
		}

		public override sealed IEnumerable Shake(IEnumerable objects)
		{
			return Shake(objects.OfType<T>());
		}
	}
}