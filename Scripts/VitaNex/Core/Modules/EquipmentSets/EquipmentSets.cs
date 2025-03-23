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

#if ServUO58
#define ServUOX
#endif

#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Server;
using Server.Mobiles;
using Server.Network;

using VitaNex.Network;
#endregion

namespace VitaNex.Modules.EquipmentSets
{
	public static partial class EquipmentSets
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		private static readonly object _OPLLock = new object(), _INVLock = new object();

		public static readonly Type TypeOfEquipmentSet = typeof(EquipmentSet);

		public static EquipmentSetsOptions CMOptions { get; private set; }

		public static Type[] SetTypes { get; private set; }

		public static Dictionary<Type, EquipmentSet> Sets { get; private set; }

		public static OutgoingPacketOverrideHandler EquipItemParent { get; private set; }

		public static PacketHandler EquipItemRequestParent { get; private set; }
		public static PacketHandler DropItemRequestParent { get; private set; }

#if !ServUOX
		public static PacketHandler EquipItemRequestParent6017 { get; private set; }
		public static PacketHandler DropItemRequestParent6017 { get; private set; }
#endif

		private static void OnLogin(LoginEventArgs e)
		{
			if (CMOptions.ModuleEnabled && e.Mobile != null)
			{
				Invalidate(e.Mobile);
			}
		}

		private static void GetProperties(Item item, Mobile viewer, ExtendedOPL list)
		{
			if (!CMOptions.ModuleEnabled || item == null || item.Deleted || !item.Layer.IsEquip() || list == null || World.Loading)
			{
				return;
			}

			if (viewer == null && item.Parent is Mobile)
			{
				viewer = (Mobile)item.Parent;
			}

			if (viewer == null)
			{
				return;
			}

			if (!item.BeginAction(_OPLLock))
			{
				return;
			}

			var itemType = item.GetType();
			var equipped = item.IsEquipped() || item.IsShopItem();

			var parent = item.Parent as Mobile;

			var npc = parent != null && ((parent is BaseCreature || !parent.Player) && !parent.IsControlled<PlayerMobile>());

			foreach (var set in FindSetsFor(itemType).Where(s => s.Display && !s.Parts.Any(p => p.Display && p.IsTypeOf(itemType) && !p.DisplaySet)))
			{
				set.GetProperties(viewer, list, equipped);

				if (npc)
				{
					continue;
				}

				if (set.DisplayParts)
				{
					foreach (var part in set.Parts.Where(p => p.Display))
					{
						part.GetProperties(viewer, list, equipped);
					}
				}

				if (!set.DisplayMods)
				{
					continue;
				}

				foreach (var mod in set.Mods.Where(mod => mod.Display))
				{
					mod.GetProperties(viewer, list, equipped);
				}
			}

			item.EndAction(_OPLLock);
		}

		private static void EquipItem(NetState state, PacketReader reader, ref byte[] buffer, ref int length)
		{
			var pos = reader.Seek(0, SeekOrigin.Current);

			reader.Seek(1, SeekOrigin.Begin);

#if ServUOX
			var item = World.FindItem(reader.ReadSerial());
#else
			var item = World.FindItem(reader.ReadInt32());
#endif

			reader.Seek(pos, SeekOrigin.Begin);

			if (EquipItemParent != null)
			{
				EquipItemParent(state, reader, ref buffer, ref length);
			}

			if (!CMOptions.ModuleEnabled || item == null || item.Deleted || !item.Layer.IsEquip())
			{
				return;
			}

			if (CMOptions.ModuleDebug)
			{
				CMOptions.ToConsole("EquipItem: {0} equiped {1}", state.Mobile, item);
			}

			Timer.DelayCall(Invalidate, state.Mobile);
		}

		private static void EquipItemRequest(NetState state, PacketReader pvSrc)
		{
			EquipItemRequest(EquipItemRequestParent, state, pvSrc);
		}

#if !ServUOX
		private static void EquipItemRequest6017(NetState state, PacketReader pvSrc)
		{
			EquipItemRequest(EquipItemRequestParent6017, state, pvSrc);
		}
#endif

		private static void EquipItemRequest(PacketHandler parent, NetState state, PacketReader pvSrc)
		{
			var item = state.Mobile.Holding;

			if (parent != null && parent.OnReceive != null)
			{
				parent.OnReceive(state, pvSrc);
			}

			if (!CMOptions.ModuleEnabled || item == null || item.Deleted || !item.Layer.IsEquip())
			{
				return;
			}

			if (CMOptions.ModuleDebug)
			{
				CMOptions.ToConsole("EquipItemRequest: {0} equiping {1}", state.Mobile, item);
			}

			Timer.DelayCall(Invalidate, state.Mobile);
		}

		private static void DropItemRequest(NetState state, PacketReader pvSrc)
		{
			DropItemRequest(DropItemRequestParent, state, pvSrc);
		}

#if !ServUOX
		private static void DropItemRequest6017(NetState state, PacketReader pvSrc)
		{ 
			DropItemRequest(DropItemRequestParent6017, state, pvSrc);
		}
#endif

		private static void DropItemRequest(PacketHandler parent, NetState state, PacketReader pvSrc)
		{
			var item = state.Mobile.Holding;

			if (parent != null && parent.OnReceive != null)
			{
				parent.OnReceive(state, pvSrc);
			}

			if (!CMOptions.ModuleEnabled || item == null || item.Deleted || !item.Layer.IsEquip())
			{
				return;
			}

			if (CMOptions.ModuleDebug)
			{
				CMOptions.ToConsole("DropItemRequest: {0} dropping {1}", state.Mobile, item);
			}

			Timer.DelayCall(Invalidate, state.Mobile);
		}

		public static T ResolveSet<T>() where T : EquipmentSet
		{
			return Sets.GetValue(typeof(T)) as T;
		}

		public static List<EquipmentSet> GetSetsFor(Item item)
		{
			return FindSetsFor(item.GetType()).ToList();
		}

		public static List<EquipmentSet> GetSetsFor(Type type)
		{
			return FindSetsFor(type).ToList();
		}

		public static IEnumerable<EquipmentSet> FindSetsFor(Item item)
		{
			return FindSetsFor(item.GetType());
		}

		public static IEnumerable<EquipmentSet> FindSetsFor(Type type)
		{
			return Sets.Values.Where(set => set.HasPartTypeOf(type));
		}

		public static void Invalidate(Mobile owner)
		{
			if (CMOptions.ModuleEnabled && owner != null)
			{
				owner.Items.ForEachReverse(item => Invalidate(owner, item));
			}
		}

		public static void Invalidate(Mobile owner, Item item)
		{
			if (!CMOptions.ModuleEnabled || owner == null)
			{
				return;
			}

			if (item == null || item.Deleted || !item.Layer.IsEquip())
			{
				return;
			}

			if (!item.BeginAction(_INVLock))
			{
				return;
			}

			if (!CMOptions.ModuleDebug)
			{
				foreach (var set in FindSetsFor(item))
				{
					set.Invalidate(owner, item);
				}
			}
			else
			{
				var sets = GetSetsFor(item);

				CMOptions.ToConsole("Found {0} sets for '{1}'", sets.Count, item);

				if (sets.Count > 0)
				{
					CMOptions.ToConsole("'{0}'", String.Join("', '", sets.Select(s => s.Name)));

					sets.ForEach(set => set.Invalidate(owner, item));
				}

				sets.Free(true);
			}

			item.EndAction(_INVLock);
		}

		public static IEnumerable<EquipmentSet> FindActiveSets(Mobile owner)
		{
			return Sets.Values.Where(s => s.ActiveOwners.Contains(owner));
		}

		public static IEnumerable<EquipmentSetMod> FindActiveMods(Mobile owner)
		{
			return FindActiveSets(owner).SelectMany(s => s.Mods.Where(m => m.ActiveOwners.Contains(owner)));
		}

		public static IEnumerable<EquipmentSetPart> FindEquippedParts(Mobile owner)
		{
			return FindActiveSets(owner).SelectMany(s => s.Parts.Where(p => p.EquipOwners.Contains(owner)));
		}

		public static IEnumerable<T> FindActiveSets<T>(Mobile owner) where T : EquipmentSet
		{
			return Sets.Values.OfType<T>().Where(s => s.ActiveOwners.Contains(owner));
		}

		public static IEnumerable<T> FindActiveMods<T>(Mobile owner) where T : EquipmentSetMod
		{
			return FindActiveSets(owner).SelectMany(s => s.Mods.OfType<T>().Where(m => m.IsActive(owner)));
		}

		public static IEnumerable<T> FindEquippedParts<T>(Mobile owner) where T : EquipmentSetPart
		{
			return FindActiveSets(owner).SelectMany(s => s.Parts.OfType<T>().Where(p => p.IsEquipped(owner)));
		}

		public static int AddSet<T>(Mobile owner) where T : EquipmentSet, new()
		{
			return AddSet<T>(owner, null);
		}

		public static int AddSet<T>(Mobile owner, Action<Item> action) where T : EquipmentSet, new()
		{
			var count = 0;

			foreach (var o in GenerateEquip<T>(owner))
			{
				++count;

				if (action != null)
				{
					action(o);
				}
			}

			return count;
		}

		public static int AddSet(Type setType, Mobile owner)
		{
			return AddSet(setType, owner, null);
		}

		public static int AddSet(Type setType, Mobile owner, Action<Item> action)
		{
			var count = 0;

			foreach (var o in GenerateEquip(setType, owner))
			{
				++count;

				if (action != null)
				{
					action(o);
				}
			}

			return count;
		}

		public static Item GenerateRandomPart<T>() where T : EquipmentSet, new()
		{
			return GenerateRandomPart(typeof(T));
		}

		public static Item GenerateRandomPart(Type setType)
		{
			var set = Sets.GetValue(setType);

			return set == null ? null : set.GenerateRandomPart();
		}

		public static IEnumerable<Item> GenerateParts<T>() where T : EquipmentSet, new()
		{
			return GenerateParts(typeof(T));
		}

		public static IEnumerable<Item> GenerateParts(Type setType)
		{
			var set = Sets.GetValue(setType);

			if (set == null)
			{
				yield break;
			}

			var parts = set.GenerateParts();

			if (parts == null)
			{
				yield break;
			}

			foreach (var p in parts)
			{
				yield return p;
			}
		}

		public static IEnumerable<Item> GenerateEquip<T>(Mobile owner) where T : EquipmentSet, new()
		{
			return GenerateEquip(typeof(T), owner);
		}

		public static IEnumerable<Item> GenerateEquip(Type setType, Mobile owner)
		{
			foreach (var p in GenerateParts(setType))
			{
				if (!owner.EquipItem(p) && (!owner.Player || !owner.PlaceInBackpack(p)))
				{
					p.Delete();
					continue;
				}

				if (!owner.Player)
				{
					p.Movable = false;
				}

				yield return p;
			}
		}
	}
}
