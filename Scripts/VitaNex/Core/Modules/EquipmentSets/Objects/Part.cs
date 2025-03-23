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
using System.Collections.Generic;
using System.Linq;

using Server;

using VitaNex.Network;
#endregion

namespace VitaNex.Modules.EquipmentSets
{
	public class EquipmentSetPart
	{
		private static readonly Item[] _EmptyItems = new Item[0];

		private readonly List<Mobile> _EquipOwners = new List<Mobile>();

		public List<Mobile> EquipOwners => _EquipOwners;

		public string Name { get; set; }

		public Type[] Types { get; set; }

		public bool IncludeChildTypes { get; set; }

		public bool Display { get; set; }
		public bool DisplaySet { get; set; }

		public EquipmentSetPart(string name, Type type)
			: this(name, new[] { type })
		{ }

		public EquipmentSetPart(string name, Type type, bool childTypes)
			: this(name, new[] { type }, childTypes)
		{ }

		public EquipmentSetPart(string name, Type type, bool childTypes, bool display, bool displaySet)
			: this(name, new[] { type }, childTypes, display, displaySet)
		{ }

		public EquipmentSetPart(string name, params Type[] types)
			: this(name, types, false)
		{ }

		public EquipmentSetPart(string name, Type[] types, bool childTypes)
			: this(name, types, childTypes, true, true)
		{ }

		public EquipmentSetPart(string name, Type[] types, bool childTypes, bool display, bool displaySet)
		{
			Name = name;
			Types = types;

			IncludeChildTypes = childTypes;

			Display = display;
			DisplaySet = displaySet;
		}

		public bool IsTypeOf(Type type)
		{
			return type != null && Types != null && Types.Any(t => type.TypeEquals(t, IncludeChildTypes));
		}

		public bool IsEquipped(Mobile m)
		{
			return IsEquipped(m, out var item);
		}

		public bool IsEquipped(Mobile m, out Item item)
		{
			item = null;

			if (m == null)
			{
				return false;
			}

			item = m.Items.Find(i => IsTypeOf(i.GetType()));

			if (item != null)
			{
				EquipOwners.Update(m);
				return true;
			}

			EquipOwners.Remove(m);
			return false;
		}

		public Item[] CreateParts(params object[] args)
		{
			if (Types == null || Types.Length == 0)
			{
				return _EmptyItems;
			}

			var items = new Item[Types.Length];

			items.SetAll(i => Types[i].CreateInstanceSafe<Item>(args));

			return items;
		}

		public Item CreatePart(int index, params object[] args)
		{
			if (Types == null || !Types.InBounds(index))
			{
				return null;
			}

			return Types[index].CreateInstance<Item>(args);
		}

		public Item CreateRandomPart(params object[] args)
		{
			if (Types == null || Types.Length == 0)
			{
				return null;
			}

			return CreatePart(Utility.Random(Types.Length), args);
		}

		public virtual void GetProperties(Mobile viewer, ExtendedOPL list, bool equipped)
		{
			list.Add(
				equipped && IsEquipped(viewer, out var item)
					? item.ResolveName(viewer).ToUpperWords().WrapUOHtmlColor(EquipmentSets.CMOptions.PartNameColorRaw)
					: Name.ToUpperWords().WrapUOHtmlColor(EquipmentSets.CMOptions.InactiveColorRaw));
		}

		public override string ToString()
		{
			return String.Format("{0}", Name);
		}
	}
}