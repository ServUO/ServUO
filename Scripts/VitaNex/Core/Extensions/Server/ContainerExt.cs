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
using System.Linq;

using Server.Items;
#endregion

namespace Server
{
	public static class ContainerExtUtility
	{
		public static bool HasItem<TItem>(
			this Container container,
			int amount = 1,
			bool children = true,
			Func<TItem, bool> predicate = null)
			where TItem : Item
		{
			return predicate == null
				? HasItem(container, typeof(TItem), amount, children)
				: HasItem(container, typeof(TItem), amount, children, i => predicate(i as TItem));
		}

		public static bool HasItem(
			this Container container,
			Type type,
			int amount = 1,
			bool children = true,
			Func<Item, bool> predicate = null)
		{
			if (container == null || type == null || amount < 1)
			{
				return false;
			}

			long total = 0;

			total = container.FindItemsByType(type, true)
							 .Where(i => i != null && !i.Deleted && i.TypeEquals(type, children) && (predicate == null || predicate(i)))
							 .Aggregate(total, (c, i) => c + i.Amount);

			return total >= amount;
		}

		public static bool HasItems(
			this Container container,
			Type[] types,
			int[] amounts = null,
			bool children = true,
			Func<Item, bool> predicate = null)
		{
			if (container == null || types == null || types.Length == 0)
			{
				return false;
			}

			if (amounts == null)
			{
				amounts = new int[0];
			}

			var count = 0;

			for (var i = 0; i < types.Length; i++)
			{
				var t = types[i];
				var amount = amounts.InBounds(i) ? amounts[i] : 1;

				if (HasItem(container, t, amount, children, predicate))
				{
					++count;
				}
			}

			return count >= types.Length;
		}

		public static bool DropItemStack(this Container c, Item item)
		{
			if (c == null || c.Deleted || item == null || item.Deleted)
			{
				return false;
			}

			return DropItemStack(c, c.RootParent as Mobile, item);
		}

		public static bool DropItemStack(this Container c, Mobile m, Item item)
		{
			if (c == null || c.Deleted || item == null || item.Deleted)
			{
				return false;
			}

			c.DropItem(item);

			if (item.Stackable)
			{
				MergeStacks(c, item.GetType(), m);
			}

			return true;
		}

		public static void MergeStacks<T>(this Container c, Mobile m)
			where T : Item
		{
			MergeStacks(c, typeof(T), m);
		}

		public static void MergeStacks(this Container c, Mobile m)
		{
			MergeStacks(c, null, m);
		}

		public static void MergeStacks(this Container c, Type t, Mobile m)
		{
			if (c == null || c.Deleted || (t != null && t.TypeEquals<Container>(true)))
			{
				return;
			}

			var i = c.Items.Count;

			while (--i >= 1)
			{
				var o = c.Items[i];

				if (!o.Stackable || (t != null && !o.TypeEquals(t, false)))
				{
					continue;
				}

				foreach (var s in c.Items.Take(i).NotType<Item, Container>().Where(s => t == null || s.TypeEquals(t, false)))
				{
					if (s.StackWith(m, o))
					{
						break;
					}
				}
			}
		}
	}
}