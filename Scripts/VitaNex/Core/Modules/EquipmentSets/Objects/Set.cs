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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server;

using VitaNex.Network;
#endregion

namespace VitaNex.Modules.EquipmentSets
{
	public abstract class EquipmentSet : IEnumerable<EquipmentSetPart>
	{
		public static Item[] GenerateParts<TSet>()
			where TSet : EquipmentSet
		{
			return GenerateParts(typeof(TSet));
		}

		public static Item[] GenerateParts(Type set)
		{
			var s = EquipmentSets.Sets.Values.FirstOrDefault(t => t.TypeEquals(set));

			if (s != null)
			{
				return s.GenerateParts();
			}

			return new Item[0];
		}

		public List<Mobile> ActiveOwners { get; private set; }

		public List<EquipmentSetPart> Parts { get; protected set; }
		public List<EquipmentSetMod> Mods { get; protected set; }

		public EquipmentSetPart this[int index] { get => Parts[index]; set => Parts[index] = value; }

		public int Count => Parts.Count;

		public string Name { get; set; }

		public bool Display { get; set; }
		public bool DisplayParts { get; set; }
		public bool DisplayMods { get; set; }

		public EquipmentSet(
			string name,
			bool display = true,
			bool displayParts = true,
			bool displayMods = true,
			IEnumerable<EquipmentSetPart> parts = null,
			IEnumerable<EquipmentSetMod> mods = null)
		{
			ActiveOwners = new List<Mobile>();

			Name = name;

			Display = display;
			DisplayParts = displayParts;
			DisplayMods = displayMods;

			Parts = parts.Ensure().ToList();
			Mods = mods.Ensure().ToList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<EquipmentSetPart> GetEnumerator()
		{
			return Parts.GetEnumerator();
		}

		public bool Contains(EquipmentSetPart part)
		{
			return Parts.Contains(part);
		}

		public void Add(EquipmentSetPart part)
		{
			Parts.Add(part);
		}

		public void AddRange(IEnumerable<EquipmentSetPart> parts)
		{
			Parts.AddRange(parts);
		}

		public bool Remove(EquipmentSetPart part)
		{
			return Parts.Remove(part);
		}

		public bool Contains(EquipmentSetMod mod)
		{
			return Mods.Contains(mod);
		}

		public void Add(EquipmentSetMod mod)
		{
			Mods.Add(mod);
		}

		public void AddRange(IEnumerable<EquipmentSetMod> mods)
		{
			Mods.AddRange(mods);
		}

		public bool Remove(EquipmentSetMod mod)
		{
			return Mods.Remove(mod);
		}

		public bool HasPartTypeOf(Type type)
		{
			return Parts.Exists(part => part.IsTypeOf(type));
		}

		public int CountEquippedParts(Mobile m)
		{
			var count = 0;

			foreach (var part in Parts)
			{
				if (part.IsEquipped(m, out var item))
				{
					++count;
				}
			}

			return count;
		}

		public IEnumerable<Tuple<EquipmentSetPart, Item>> FindEquippedParts(Mobile m)
		{
			foreach (var part in Parts)
			{
				if (part.IsEquipped(m, out var item))
				{
					yield return Tuple.Create(part, item);
				}
			}
		}

		public Tuple<EquipmentSetPart, Item>[] GetEquippedParts(Mobile m)
		{
			return FindEquippedParts(m).ToArray();
		}

		public IEnumerable<EquipmentSetMod> FindAvailableMods(Mobile m, EquipmentSetPart[] equipped)
		{
			return Mods.Where(mod => equipped.Length >= mod.PartsRequired);
		}

		public EquipmentSetMod[] GetAvailableMods(Mobile m, EquipmentSetPart[] equipped)
		{
			return FindAvailableMods(m, equipped).ToArray();
		}

		public Item GenerateRandomPart()
		{
			var p = Parts.GetRandom();

			return p != null ? p.CreateRandomPart() : null;
		}

		public Item[] GenerateParts()
		{
			return Parts.SelectMany(part => part.CreateParts()).Not(item => item == null || item.Deleted).ToArray();
		}

		public void Invalidate(Mobile m, Item item)
		{
			var totalActive = 0;

			var type = item.GetType();
			var changedPart = Tuple.Create(Parts.FirstOrDefault(p => p.IsTypeOf(type)), item);
			var equippedParts = GetEquippedParts(m);

			foreach (var mod in Mods)
			{
				if (mod.IsActive(m))
				{
					++totalActive;

					if (equippedParts.Length < mod.PartsRequired && Deactivate(m, equippedParts, changedPart, mod))
					{
						--totalActive;
					}
				}
				else if (mod.CheckExpansion())
				{
					if (equippedParts.Length >= mod.PartsRequired && Activate(m, equippedParts, changedPart, mod))
					{
						++totalActive;
					}
				}
			}

			if (EquipmentSets.CMOptions.ModuleDebug)
			{
				EquipmentSets.CMOptions.ToConsole("Mods: {0}, Parts: {1}", totalActive, equippedParts.Length);
			}

			SetActiveOwner(m, totalActive > 0);

			InvalidateAllProperties(m, equippedParts.Select(t => t.Item2), changedPart.Item2);

			m.UpdateResistances();
			m.UpdateSkillMods();
		}

		public void InvalidateAllProperties(Mobile m, IEnumerable<Item> equipped, Item changed)
		{
			if (World.Loading || World.Saving || m == null || m.Deleted || m.Map == null || m.Map == Map.Internal)
			{
				return;
			}

			m.InvalidateProperties();

			if (equipped != null)
			{
				foreach (var item in equipped.Where(item => item != changed))
				{
					InvalidateItemProperties(item);
				}
			}

			if (changed != null)
			{
				InvalidateItemProperties(changed);
			}
		}

		public void InvalidateItemProperties(Item item)
		{
			if (World.Loading || World.Saving || item == null || item.Deleted)
			{
				return;
			}

			item.ClearProperties();
			item.InvalidateProperties();
		}

		private void SetActiveOwner(Mobile m, bool state)
		{
			if (state)
			{
				ActiveOwners.Update(m);
			}
			else
			{
				ActiveOwners.Remove(m);
			}
		}

		public bool Activate(
			Mobile m,
			Tuple<EquipmentSetPart, Item>[] equipped,
			Tuple<EquipmentSetPart, Item> added,
			EquipmentSetMod mod)
		{
			return OnActivate(m, equipped, added, mod) && mod.Activate(m, equipped);
		}

		public bool Deactivate(
			Mobile m,
			Tuple<EquipmentSetPart, Item>[] equipped,
			Tuple<EquipmentSetPart, Item> added,
			EquipmentSetMod mod)
		{
			return OnDeactivate(m, equipped, added, mod) && mod.Deactivate(m, equipped);
		}

		protected virtual bool OnActivate(
			Mobile m,
			Tuple<EquipmentSetPart, Item>[] equipped,
			Tuple<EquipmentSetPart, Item> added,
			EquipmentSetMod mod)
		{
			return m != null && !m.Deleted && equipped != null && mod != null && !mod.IsActive(m);
		}

		protected virtual bool OnDeactivate(
			Mobile m,
			Tuple<EquipmentSetPart, Item>[] equipped,
			Tuple<EquipmentSetPart, Item> removed,
			EquipmentSetMod mod)
		{
			return m != null && !m.Deleted && equipped != null && mod != null && mod.IsActive(m);
		}

		public virtual void GetProperties(Mobile viewer, ExtendedOPL list, bool equipped)
		{
			list.Add(String.Empty);

			var name = Name.ToUpperWords();
			var count = Parts.Count;

			if (!equipped)
			{
				list.Add("{0} [{1:#,0}]".WrapUOHtmlColor(EquipmentSets.CMOptions.SetNameColorRaw), name, count);
			}
			else
			{
				var cur = CountEquippedParts(viewer);

				list.Add("{0} [{1:#,0} / {2:#,0}]".WrapUOHtmlColor(EquipmentSets.CMOptions.SetNameColorRaw), name, cur, count);
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}