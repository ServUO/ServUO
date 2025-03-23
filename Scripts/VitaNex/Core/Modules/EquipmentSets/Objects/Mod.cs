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

using Server;

using VitaNex.Network;
#endregion

namespace VitaNex.Modules.EquipmentSets
{
	public abstract class EquipmentSetMod : IExpansionCheck
	{
		public List<Mobile> ActiveOwners { get; private set; }

		public string Name { get; set; }
		public string Desc { get; set; }

		public int PartsRequired { get; set; }

		public bool Display { get; set; }

		public ExpansionFlags Expansions { get; set; }

		public EquipmentSetMod(
			string name = "Set Mod",
			string desc = null,
			int partsReq = 1,
			bool display = true,
			ExpansionFlags ex = ExpansionFlags.None)
		{
			Name = name;
			Desc = desc ?? String.Empty;

			Display = display;

			PartsRequired = partsReq;

			Expansions = ex;

			ActiveOwners = new List<Mobile>();
		}

		public bool IsActive(Mobile m)
		{
			return m != null && ActiveOwners.Contains(m);
		}

		public bool Activate(Mobile m, Tuple<EquipmentSetPart, Item>[] equipped)
		{
			if (m == null || m.Deleted || equipped == null || !this.CheckExpansion())
			{
				return false;
			}

			if (OnActivate(m, equipped))
			{
				ActiveOwners.Update(m);
				return true;
			}

			return false;
		}

		public bool Deactivate(Mobile m, Tuple<EquipmentSetPart, Item>[] equipped)
		{
			if (m == null || m.Deleted || equipped == null)
			{
				return false;
			}

			if (OnDeactivate(m, equipped))
			{
				ActiveOwners.Remove(m);
				return true;
			}

			return false;
		}

		protected abstract bool OnActivate(Mobile m, Tuple<EquipmentSetPart, Item>[] equipped);
		protected abstract bool OnDeactivate(Mobile m, Tuple<EquipmentSetPart, Item>[] equipped);

		public virtual void GetProperties(Mobile viewer, ExtendedOPL list, bool equipped)
		{
			if (!this.CheckExpansion())
			{
				return;
			}

			string value;

			if (String.IsNullOrEmpty(Desc))
			{
				if (String.IsNullOrWhiteSpace(Name))
				{
					return;
				}

				value = String.Format("[{0:#,0}] {1}", PartsRequired, Name.ToUpperWords());
			}
			else if (String.IsNullOrWhiteSpace(Name))
			{
				value = String.Format("[{0:#,0}]: {1}", PartsRequired, Desc);
			}
			else
			{
				value = String.Format("[{0:#,0}] {1}: {2}", PartsRequired, Name.ToUpperWords(), Desc);
			}

			if (String.IsNullOrWhiteSpace(value))
			{
				return;
			}

			var color = equipped && IsActive(viewer)
				? EquipmentSets.CMOptions.ModNameColorRaw
				: EquipmentSets.CMOptions.InactiveColorRaw;

			value = value.WrapUOHtmlColor(color);

			list.Add(value);
		}

		public override string ToString()
		{
			return String.Format("{0}", Name);
		}
	}
}