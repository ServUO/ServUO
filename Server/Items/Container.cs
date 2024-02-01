#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Server.Network;
#endregion

namespace Server.Items
{
	public delegate void OnItemConsumed(Item item, int amount);

	public delegate int CheckItemGroup(Item a, Item b);

	public delegate bool ResValidator(Item item);

	public delegate void ContainerSnoopHandler(Container cont, Mobile from);

	public class Container : Item
	{
		public static ContainerSnoopHandler SnoopHandler { get; set; }

		private ContainerData m_ContainerData;

		private int m_DropSound;
		private int m_GumpID;
		private int m_MaxItems = -1;
		private int m_MaxWeight = -1;

		protected int m_TotalItems;
		protected int m_TotalWeight;
		protected int m_TotalGold;

		internal List<Item> m_Items;

		public ContainerData ContainerData
		{
			get
			{
				if (m_ContainerData == null)
				{
					UpdateContainerData();
				}

				return m_ContainerData;
			}
			set => m_ContainerData = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override int ItemID
		{
			get => base.ItemID;
			set
			{
				var oldID = ItemID;

				base.ItemID = value;

				if (ItemID != oldID)
				{
					UpdateContainerData();
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int GumpID { get => m_GumpID == -1 ? DefaultGumpID : m_GumpID; set => m_GumpID = value; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DropSound { get => m_DropSound == -1 ? DefaultDropSound : m_DropSound; set => m_DropSound = value; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxItems
		{
			get => m_MaxItems == -1 ? DefaultMaxItems : m_MaxItems;
			set
			{
				m_MaxItems = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int MaxWeight
		{
			get
			{
				if (Parent is Container c && c.MaxWeight == 0)
				{
					return 0;
				}

				return m_MaxWeight == -1 ? DefaultMaxWeight : m_MaxWeight;
			}
			set
			{
				m_MaxWeight = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool LiftOverride { get; set; }

		public virtual Type[] SupportedContents => Type.EmptyTypes;

		public virtual void UpdateContainerData()
		{
			ContainerData = ContainerData.GetData(ItemID);
		}

		public virtual Rectangle2D Bounds => ContainerData.Bounds;
		public virtual int DefaultGumpID => ContainerData.GumpID;
		public virtual int DefaultDropSound => ContainerData.DropSound;

		public virtual int DefaultMaxItems => GlobalMaxItems;
		public virtual int DefaultMaxWeight => GlobalMaxWeight;

		public virtual bool CheckHoldCount => true;
		public virtual bool CheckHoldWeight => true;
		public virtual bool CheckHoldParent => true;

		public virtual bool IsDecoContainer => !Movable && !IsLockedDown && !IsSecure && Parent == null && !LiftOverride;

		public virtual int GetDroppedSound(Item item)
		{
			var dropSound = item.GetDropSound();

			return dropSound != -1 ? dropSound : DropSound;
		}

		public override void OnAfterDuped(Item newItem)
		{
			base.OnAfterDuped(newItem);

			if (newItem is Container cont)
			{
				cont.m_MaxItems = m_MaxItems;
				cont.m_MaxWeight = m_MaxWeight;
			}
		}

		public override void OnSnoop(Mobile from)
		{
			SnoopHandler?.Invoke(this, from);
		}

		public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
		{
			if (!from.IsStaff() && IsDecoContainer)
			{
				reject = LRReason.CannotLift;
				return false;
			}

			return base.CheckLift(from, item, ref reject);
		}

		public override bool CheckItemUse(Mobile from, Item item)
		{
			if (item != this && from.AccessLevel < AccessLevel.GameMaster && IsDecoContainer)
			{
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				return false;
			}

			return base.CheckItemUse(from, item);
		}

		public bool CheckHold(Item item)
		{
			return CheckHold(null, item, false);
		}

		public bool CheckHold(Mobile m, Item item, bool message)
		{
			return CheckHold(m, item, message, true, true, 0, 0);
		}

		public bool CheckHold(Mobile m, Item item, bool message, bool checkItems)
		{
			return CheckHold(m, item, message, checkItems, true, 0, 0);
		}

		public bool CheckHold(Mobile m, Item item, bool message, bool checkItems, bool checkWeight)
		{
			return CheckHold(m, item, message, checkItems, checkWeight, 0, 0);
		}

		public bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems)
		{
			return CheckHold(m, item, message, checkItems, true, plusItems, 0);
		}

		public bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
		{
			return CheckHold(m, item, message, checkItems, true, plusItems, plusWeight);
		}

		public virtual bool CheckHold(Mobile m, Item item, bool message, bool checkItems, bool checkWeight, int plusItems, int plusWeight)
		{
			if (m == null || !m.IsStaff())
			{
				if (IsDecoContainer)
				{
					if (m != null && message)
					{
						SendCantStoreMessage(m, item);
					}

					return false;
				}

				if (checkItems && CheckHoldCount)
				{
					if (!CanHold(m, item))
					{
						if (m != null && message)
						{
							SendDisallowedMessage(m, item);
						}

						return false;
					}

					var maxItems = MaxItems;

					if (maxItems != 0 && (TotalItems + plusItems + item.TotalItems + (item.IsVirtualItem ? 0 : 1)) > maxItems)
					{
						if (m != null && message)
						{
							SendFullItemsMessage(m, item);
						}

						return false;
					}
				}

				if (checkWeight && CheckHoldWeight)
				{
					var maxWeight = MaxWeight;

					if (maxWeight != 0 && (TotalWeight + plusWeight + item.TotalWeight + item.PileWeight) > maxWeight)
					{
						if (m != null && message)
						{
							SendFullWeightMessage(m, item);
						}

						return false;
					}
				}
			}

			if (!CheckHoldParent)
			{
				return true;
			}

			var parent = Parent;

			while (parent != null)
			{
				if (parent is Container cp)
				{
					return cp.CheckHold(m, item, message, checkItems, checkWeight, plusItems, plusWeight);
				}

				if (parent is Item ip)
				{
					parent = ip.Parent;
				}
				else
				{
					break;
				}
			}

			return true;
		}

		public bool CanHold(Item item)
		{
			return CanHold(null, item);
		}

		public virtual bool CanHold(Mobile m, Item item)
		{
			if (item == null || item.Deleted)
			{
				return false;
			}

			if (m != null && m.AccessLevel >= AccessLevel.GameMaster)
			{
				return true;
			}

			var types = SupportedContents;

			if (types == null || types.Length <= 0)
			{
				return true;
			}

			var type = item.GetType();

			foreach (var t in types)
			{
				if (t.IsAssignableFrom(type))
				{
					return true;
				}
			}

			return false;
		}

		public virtual bool CheckStack(Mobile from, Item item)
		{
			if (item == null || item.Deleted || !item.Stackable)
			{
				return false;
			}

			foreach (var i in Items)
			{
				if (i.WillStack(from, item))
				{
					return true;
				}
			}

			return false;
		}

		public virtual void SendFullItemsMessage(Mobile to, Item item)
		{
			to.SendLocalizedMessage(1080017); // That container cannot hold more items.
		}

		public virtual void SendFullWeightMessage(Mobile to, Item item)
		{
			to.SendLocalizedMessage(1080016); // That container cannot hold more weight.
		}

		public virtual void SendCantStoreMessage(Mobile to, Item item)
		{
			to.SendLocalizedMessage(500176); // That is not your container, you can't store things here.
		}

		public virtual void SendDisallowedMessage(Mobile to, Item item)
		{
			to.SendMessage("This item can't be stored in that container.");
		}

		public virtual bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			if (!CheckHold(from, item, true, true, true))
			{
				return false;
			}

			item.Location = new Point3D(p.m_X, p.m_Y, 0);
			AddItem(item);

			from.SendSound(GetDroppedSound(item), GetWorldLocation());

			return true;
		}

		private class GroupComparer : IComparer
		{
			private readonly CheckItemGroup m_Grouper;

			public GroupComparer(CheckItemGroup grouper)
			{
				m_Grouper = grouper;
			}

			public int Compare(object x, object y)
			{
				var a = (Item)x;
				var b = (Item)y;

				return m_Grouper(a, b);
			}
		}

		#region Consume[...]

		public bool ConsumeTotalGrouped(Type type, int amount, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(type, amount, FindOptions.Default, callback, grouper);
		}

		public bool ConsumeTotalGrouped(Type type, int amount, bool recurse, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(type, amount, InitFindOptions(recurse), callback, grouper);
		}

		public bool ConsumeTotalGrouped(Type type, int amount, FindOptions options, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(type, amount, options, null, callback, grouper);
		}

		public bool ConsumeTotalGrouped(Type type, int amount, ResValidator validator, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(type, amount, FindOptions.Default, validator, callback, grouper);
		}

		public bool ConsumeTotalGrouped(Type type, int amount, bool recurse, ResValidator validator, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(type, amount, InitFindOptions(recurse), validator, callback, grouper);
		}

		public bool ConsumeTotalGrouped(Type type, int amount, FindOptions options, ResValidator validator, OnItemConsumed callback, CheckItemGroup grouper)
		{
			if (grouper == null)
			{
				throw new ArgumentNullException();
			}

			if (CheckLocked(this, options))
			{
				return false;
			}

			var typedItems = FindItemsByType(type, options);

			var groups = new List<List<Item>>();
			var idx = 0;

			while (idx < typedItems.Length)
			{
				var a = typedItems[idx++];

				if (validator != null && !validator(a))
				{
					continue;
				}

				var group = new List<Item>
				{
					a
				};

				while (idx < typedItems.Length)
				{
					var b = typedItems[idx];

					if (validator != null && !validator(b))
					{
						continue;
					}

					var v = grouper(a, b);

					if (v == 0)
					{
						group.Add(b);
					}
					else
					{
						break;
					}

					++idx;
				}

				groups.Add(group);
			}

			var items = new Item[groups.Count][];
			var totals = new int[groups.Count];

			var hasEnough = false;

			for (var i = 0; i < groups.Count; ++i)
			{
				items[i] = groups[i].ToArray();

				for (var j = 0; j < items[i].Length; ++j)
				{
					totals[i] += items[i][j].Amount;
				}

				if (totals[i] >= amount)
				{
					hasEnough = true;
				}
			}

			if (!hasEnough)
			{
				return false;
			}

			for (var i = 0; i < items.Length; ++i)
			{
				if (totals[i] >= amount)
				{
					var need = amount;

					for (var j = 0; j < items[i].Length; ++j)
					{
						var item = items[i][j];

						var theirAmount = item.Amount;

						if (theirAmount < need)
						{
							callback?.Invoke(item, theirAmount);

							item.Delete();
							need -= theirAmount;
						}
						else
						{
							callback?.Invoke(item, need);

							item.Consume(need);
							break;
						}
					}

					break;
				}
			}

			return true;
		}

		public int ConsumeTotalGrouped(Type[] types, int[] amounts, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(types, amounts, FindOptions.Default, callback, grouper);
		}

		public int ConsumeTotalGrouped(Type[] types, int[] amounts, bool recurse, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(types, amounts, InitFindOptions(recurse), callback, grouper);
		}

		public int ConsumeTotalGrouped(Type[] types, int[] amounts, FindOptions options, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(types, amounts, options, null, callback, grouper);
		}

		public int ConsumeTotalGrouped(Type[] types, int[] amounts, ResValidator validator, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(types, amounts, FindOptions.Default, validator, callback, grouper);
		}

		public int ConsumeTotalGrouped(Type[] types, int[] amounts, bool recurse, ResValidator validator, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(types, amounts, InitFindOptions(recurse), validator, callback, grouper);
		}

		public int ConsumeTotalGrouped(Type[] types, int[] amounts, FindOptions options, ResValidator validator, OnItemConsumed callback, CheckItemGroup grouper)
		{
			if (types.Length != amounts.Length)
			{
				throw new ArgumentException();
			}
			else if (grouper == null)
			{
				throw new ArgumentNullException();
			}

			if (CheckLocked(this, options))
			{
				return Int32.MaxValue;
			}

			var items = new Item[types.Length][][];
			var totals = new int[types.Length][];

			for (var i = 0; i < types.Length; ++i)
			{
				var typedItems = FindItemsByType(types[i], options);

				var groups = new List<List<Item>>();
				var idx = 0;

				while (idx < typedItems.Length)
				{
					var a = typedItems[idx++];

					if (validator != null && !validator(a))
					{
						continue;
					}

					var group = new List<Item>
					{
						a
					};

					while (idx < typedItems.Length)
					{
						var b = typedItems[idx];

						if (validator != null && !validator(b))
						{
							continue;
						}

						var v = grouper(a, b);

						if (v == 0)
						{
							group.Add(b);
						}
						else
						{
							break;
						}

						++idx;
					}

					groups.Add(group);
				}

				items[i] = new Item[groups.Count][];
				totals[i] = new int[groups.Count];

				var hasEnough = false;

				for (var j = 0; j < groups.Count; ++j)
				{
					items[i][j] = groups[j].ToArray();

					for (var k = 0; k < items[i][j].Length; ++k)
					{
						totals[i][j] += items[i][j][k].Amount;
					}

					if (totals[i][j] >= amounts[i])
					{
						hasEnough = true;
					}
				}

				if (!hasEnough)
				{
					return i;
				}
			}

			for (var i = 0; i < items.Length; ++i)
			{
				for (var j = 0; j < items[i].Length; ++j)
				{
					if (totals[i][j] >= amounts[i])
					{
						var need = amounts[i];

						for (var k = 0; k < items[i][j].Length; ++k)
						{
							var item = items[i][j][k];

							var theirAmount = item.Amount;

							if (theirAmount < need)
							{
								callback?.Invoke(item, theirAmount);

								item.Delete();
								need -= theirAmount;
							}
							else
							{
								callback?.Invoke(item, need);

								item.Consume(need);
								break;
							}
						}

						break;
					}
				}
			}

			return -1;
		}

		public int ConsumeTotalGrouped(Type[][] types, int[] amounts, bool recurse, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(types, amounts, InitFindOptions(recurse), callback, grouper);
		}

		public int ConsumeTotalGrouped(Type[][] types, int[] amounts, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(types, amounts, FindOptions.Default, callback, grouper);
		}

		public int ConsumeTotalGrouped(Type[][] types, int[] amounts, FindOptions options, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(types, amounts, options, null, callback, grouper);
		}

		public int ConsumeTotalGrouped(Type[][] types, int[] amounts, ResValidator validator, OnItemConsumed callback, CheckItemGroup grouper)
		{
			return ConsumeTotalGrouped(types, amounts, FindOptions.Default, validator, callback, grouper);
		}

		public int ConsumeTotalGrouped(Type[][] types, int[] amounts, bool recurse, ResValidator validator, OnItemConsumed callback, CheckItemGroup grouper) 
		{
			return ConsumeTotalGrouped(types, amounts, InitFindOptions(recurse), validator, callback, grouper);
		}

		public int ConsumeTotalGrouped(Type[][] types, int[] amounts, FindOptions options, ResValidator validator, OnItemConsumed callback, CheckItemGroup grouper)
		{
			if (types.Length != amounts.Length)
			{
				throw new ArgumentException();
			}
			else if (grouper == null)
			{
				throw new ArgumentNullException();
			}

			if (CheckLocked(this, options))
			{
				return Int32.MaxValue;
			}

			var items = new Item[types.Length][][];
			var totals = new int[types.Length][];

			for (var i = 0; i < types.Length; ++i)
			{
				var typedItems = FindItemsByType(types[i], options);

				var groups = new List<List<Item>>();
				var idx = 0;

				while (idx < typedItems.Length)
				{
					var a = typedItems[idx++];

					if (validator != null && !validator(a))
					{
						continue;
					}

					var group = new List<Item>
					{
						a
					};

					while (idx < typedItems.Length)
					{
						var b = typedItems[idx];

						if (validator != null && !validator(b))
						{
							continue;
						}

						var v = grouper(a, b);

						if (v == 0)
						{
							group.Add(b);
						}
						else
						{
							break;
						}

						++idx;
					}

					groups.Add(group);
				}

				items[i] = new Item[groups.Count][];
				totals[i] = new int[groups.Count];

				var hasEnough = false;

				for (var j = 0; j < groups.Count; ++j)
				{
					items[i][j] = groups[j].ToArray();

					for (var k = 0; k < items[i][j].Length; ++k)
					{
						totals[i][j] += items[i][j][k].Amount;
					}

					if (totals[i][j] >= amounts[i])
					{
						hasEnough = true;
					}
				}

				if (!hasEnough)
				{
					return i;
				}
			}

			for (var i = 0; i < items.Length; ++i)
			{
				for (var j = 0; j < items[i].Length; ++j)
				{
					if (totals[i][j] >= amounts[i])
					{
						var need = amounts[i];

						for (var k = 0; k < items[i][j].Length; ++k)
						{
							var item = items[i][j][k];

							var theirAmount = item.Amount;

							if (theirAmount < need)
							{
								callback?.Invoke(item, theirAmount);

								item.Delete();
								need -= theirAmount;
							}
							else
							{
								callback?.Invoke(item, need);

								item.Consume(need);
								break;
							}
						}

						break;
					}
				}
			}

			return -1;
		}

		public int ConsumeTotal(Type[][] types, int[] amounts)
		{
			return ConsumeTotal(types, amounts, FindOptions.Default);
		}

		public int ConsumeTotal(Type[][] types, int[] amounts, bool recurse)
		{
			return ConsumeTotal(types, amounts, InitFindOptions(recurse));
		}

		public int ConsumeTotal(Type[][] types, int[] amounts, FindOptions options)
		{
			return ConsumeTotal(types, amounts, options, null);
		}

		public int ConsumeTotal(Type[][] types, int[] amounts, OnItemConsumed callback)
		{
			return ConsumeTotal(types, amounts, FindOptions.Default, callback);
		}

		public int ConsumeTotal(Type[][] types, int[] amounts, bool recurse, OnItemConsumed callback)
		{
			return ConsumeTotal(types, amounts, InitFindOptions(recurse), callback);
		}

		public int ConsumeTotal(Type[][] types, int[] amounts, FindOptions options, OnItemConsumed callback)
		{
			if (types.Length != amounts.Length)
			{
				throw new ArgumentException();
			}

			if (CheckLocked(this, options))
			{
				return Int32.MaxValue;
			}

			var items = new Item[types.Length][];
			var totals = new int[types.Length];

			for (var i = 0; i < types.Length; ++i)
			{
				items[i] = FindItemsByType(types[i], options);

				for (var j = 0; j < items[i].Length; ++j)
				{
					totals[i] += items[i][j].Amount;
				}

				if (totals[i] < amounts[i])
				{
					return i;
				}
			}

			for (var i = 0; i < types.Length; ++i)
			{
				var need = amounts[i];

				for (var j = 0; j < items[i].Length; ++j)
				{
					var item = items[i][j];

					var theirAmount = item.Amount;

					if (theirAmount < need)
					{
						callback?.Invoke(item, theirAmount);

						item.Delete();
						need -= theirAmount;
					}
					else
					{
						callback?.Invoke(item, need);

						item.Consume(need);
						break;
					}
				}
			}

			return -1;
		}

		public int ConsumeTotal(Type[] types, int[] amounts)
		{
			return ConsumeTotal(types, amounts, FindOptions.Default);
		}

		public int ConsumeTotal(Type[] types, int[] amounts, bool recurse)
		{
			return ConsumeTotal(types, amounts, InitFindOptions(recurse));
		}

		public int ConsumeTotal(Type[] types, int[] amounts, FindOptions options)
		{
			return ConsumeTotal(types, amounts, options, null);
		}

		public int ConsumeTotal(Type[] types, int[] amounts, OnItemConsumed callback)
		{
			return ConsumeTotal(types, amounts, FindOptions.Default, callback);
		}

		public int ConsumeTotal(Type[] types, int[] amounts, bool recurse, OnItemConsumed callback)
		{
			return ConsumeTotal(types, amounts, InitFindOptions(recurse), callback);
		}

		public int ConsumeTotal(Type[] types, int[] amounts, FindOptions options, OnItemConsumed callback)
		{
			if (types.Length != amounts.Length)
			{
				throw new ArgumentException();
			}

			if (CheckLocked(this, options))
			{
				return Int32.MaxValue;
			}

			var items = new Item[types.Length][];
			var totals = new int[types.Length];

			for (var i = 0; i < types.Length; ++i)
			{
				items[i] = FindItemsByType(types[i], options);

				for (var j = 0; j < items[i].Length; ++j)
				{
					totals[i] += items[i][j].Amount;
				}

				if (totals[i] < amounts[i])
				{
					return i;
				}
			}

			for (var i = 0; i < types.Length; ++i)
			{
				var need = amounts[i];

				for (var j = 0; j < items[i].Length; ++j)
				{
					var item = items[i][j];

					var theirAmount = item.Amount;

					if (theirAmount < need)
					{
						callback?.Invoke(item, theirAmount);

						item.Delete();
						need -= theirAmount;
					}
					else
					{
						callback?.Invoke(item, need);

						item.Consume(need);
						break;
					}
				}
			}

			return -1;
		}

		public bool ConsumeTotal(Type type, int amount)
		{
			return ConsumeTotal(type, amount, FindOptions.Default);
		}

		public bool ConsumeTotal(Type type, int amount, bool recurse)
		{
			return ConsumeTotal(type, amount, InitFindOptions(recurse));
		}

		public bool ConsumeTotal(Type type, int amount, FindOptions options)
		{
			return ConsumeTotal(type, amount, options, null);
		}

		public bool ConsumeTotal(Type type, int amount, OnItemConsumed callback)
		{
			return ConsumeTotal(type, amount, FindOptions.Default, callback);
		}

		public bool ConsumeTotal(Type type, int amount, bool recurse, OnItemConsumed callback)
		{
			return ConsumeTotal(type, amount, InitFindOptions(recurse), callback);
		}

		public bool ConsumeTotal(Type type, int amount, FindOptions options, OnItemConsumed callback)
		{
			if (CheckLocked(this, options))
			{
				return false;
			}

			var total = 0;

			var items = FindItemsByType(type, options);

			// First pass, compute total
			for (var i = 0; i < items.Length; ++i)
			{
				total += items[i].Amount;
			}

			if (total >= amount)
			{
				// We've enough, so consume it
				var need = amount;

				for (var i = 0; i < items.Length; ++i)
				{
					var item = items[i];

					var theirAmount = item.Amount;

					if (theirAmount < need)
					{
						callback?.Invoke(item, theirAmount);
						
						item.Delete();
						need -= theirAmount;
					}
					else
					{
						callback?.Invoke(item, need);

						item.Consume(need);

						return true;
					}
				}
			}

			return false;
		}

		public int ConsumeUpTo(Type type, int amount)
		{
			return ConsumeUpTo(type, amount, FindOptions.Default);
		}

		public int ConsumeUpTo(Type type, int amount, bool recurse)
		{
			return ConsumeUpTo(type, amount, InitFindOptions(recurse));
		}

		public int ConsumeUpTo(Type type, int amount, FindOptions options)
		{
			return ConsumeUpTo(type, amount, options, null);
		}

		public int ConsumeUpTo(Type type, int amount, OnItemConsumed callback)
		{
			return ConsumeUpTo(type, amount, FindOptions.Default, callback);
		}

		public int ConsumeUpTo(Type type, int amount, bool recurse, OnItemConsumed callback)
		{
			return ConsumeUpTo(type, amount, InitFindOptions(recurse), callback);
		}

		public int ConsumeUpTo(Type type, int amount, FindOptions options, OnItemConsumed callback)
		{
			var consumed = 0;

			if (CheckLocked(this, options))
			{
				return consumed;
			}

			var toDelete = new Queue<Item>();

			RecurseConsumeUpTo(this, type, amount, options, callback, ref consumed, toDelete);

			while (toDelete.Count > 0)
			{
				toDelete.Dequeue().Delete();
			}

			return consumed;
		}

		private static void RecurseConsumeUpTo(Item current, Type type, int amount, FindOptions options, OnItemConsumed callback, ref int consumed, Queue<Item> toDelete)
		{
			if (current != null && current.Items.Count > 0)
			{
				if (CheckLocked(current, options))
				{
					return;
				}

				var list = current.Items;

				for (var i = 0; i < list.Count; ++i)
				{
					var item = list[i];

					if (CheckType(item, type))
					{
						var need = amount - consumed;
						var theirAmount = item.Amount;

						if (theirAmount <= need)
						{
							callback?.Invoke(item, theirAmount);

							toDelete.Enqueue(item);
							consumed += theirAmount;
						}
						else
						{
							callback?.Invoke(item, need);

							item.Amount -= need;
							consumed += need;

							return;
						}
					}

					if (CheckRecurse(item, options))
					{
						RecurseConsumeUpTo(item, type, amount, options, callback, ref consumed, toDelete);
					}
				}
			}
		}

		#endregion

		#region Get[BestGroup]Amount

		public int GetBestGroupAmount(Type type, CheckItemGroup grouper)
		{
			return GetBestGroupAmount(type, FindOptions.Default, grouper);
		}

		public int GetBestGroupAmount(Type type, bool recurse, CheckItemGroup grouper)
		{
			return GetBestGroupAmount(type, InitFindOptions(recurse), grouper);
		}

		public int GetBestGroupAmount(Type type, FindOptions options, CheckItemGroup grouper)
		{
			if (grouper == null)
			{
				throw new ArgumentNullException();
			}

			var best = 0;

			if (CheckLocked(this, options))
			{
				return best;
			}

			var typedItems = FindItemsByType(type, options);

			var groups = new List<List<Item>>();
			var idx = 0;

			while (idx < typedItems.Length)
			{
				var a = typedItems[idx++];
				var group = new List<Item>
				{
					a
				};

				while (idx < typedItems.Length)
				{
					var b = typedItems[idx];
					var v = grouper(a, b);

					if (v == 0)
					{
						group.Add(b);
					}
					else
					{
						break;
					}

					++idx;
				}

				groups.Add(group);
			}

			for (var i = 0; i < groups.Count; ++i)
			{
				var items = groups[i];

				var total = 0;

				for (var j = 0; j < items.Count; ++j)
				{
					total += items[j].Amount;
				}

				if (total >= best)
				{
					best = total;
				}
			}

			return best;
		}

		public int GetBestGroupAmount(Type[] types, CheckItemGroup grouper)
		{
			return GetBestGroupAmount(types, FindOptions.Default, grouper);
		}

		public int GetBestGroupAmount(Type[] types, bool recurse, CheckItemGroup grouper)
		{
			return GetBestGroupAmount(types, InitFindOptions(recurse), grouper);
		}

		public int GetBestGroupAmount(Type[] types, FindOptions options, CheckItemGroup grouper)
		{
			if (grouper == null)
			{
				throw new ArgumentNullException();
			}

			var best = 0;

			if (CheckLocked(this, options))
			{
				return best;
			}

			var typedItems = FindItemsByType(types, options);

			var groups = new List<List<Item>>();
			var idx = 0;

			while (idx < typedItems.Length)
			{
				var a = typedItems[idx++];
				var group = new List<Item>
				{
					a
				};

				while (idx < typedItems.Length)
				{
					var b = typedItems[idx];
					var v = grouper(a, b);

					if (v == 0)
					{
						group.Add(b);
					}
					else
					{
						break;
					}

					++idx;
				}

				groups.Add(group);
			}

			for (var j = 0; j < groups.Count; ++j)
			{
				var items = groups[j];
				var total = 0;

				for (var k = 0; k < items.Count; ++k)
				{
					total += items[k].Amount;
				}

				if (total >= best)
				{
					best = total;
				}
			}

			return best;
		}

		public int GetBestGroupAmount(Type[][] types, CheckItemGroup grouper)
		{
			return GetBestGroupAmount(types, FindOptions.Default, grouper);
		}

		public int GetBestGroupAmount(Type[][] types, bool recurse, CheckItemGroup grouper)
		{
			return GetBestGroupAmount(types, InitFindOptions(recurse), grouper);
		}

		public int GetBestGroupAmount(Type[][] types, FindOptions options, CheckItemGroup grouper)
		{
			if (grouper == null)
			{
				throw new ArgumentNullException();
			}

			var best = 0;

			if (CheckLocked(this, options))
			{
				return best;
			}

			for (var i = 0; i < types.Length; ++i)
			{
				var typedItems = FindItemsByType(types[i], options);

				var groups = new List<List<Item>>();
				var idx = 0;

				while (idx < typedItems.Length)
				{
					var a = typedItems[idx++];
					var group = new List<Item>
					{
						a
					};

					while (idx < typedItems.Length)
					{
						var b = typedItems[idx];
						var v = grouper(a, b);

						if (v == 0)
						{
							group.Add(b);
						}
						else
						{
							break;
						}

						++idx;
					}

					groups.Add(group);
				}

				for (var j = 0; j < groups.Count; ++j)
				{
					var items = groups[j];
					var total = 0;

					for (var k = 0; k < items.Count; ++k)
					{
						total += items[k].Amount;
					}

					if (total >= best)
					{
						best = total;
					}
				}
			}

			return best;
		}

		public int GetAmount<T>()
		{
			return GetAmount(typeof(T));
		}

		public int GetAmount<T>(bool recurse)
		{
			return GetAmount(typeof(T), recurse);
		}

		public int GetAmount<T>(FindOptions options)
		{
			return GetAmount(typeof(T), options);
		}

		public int GetAmount(Type type)
		{
			return GetAmount(type, FindOptions.Default);
		}

		public int GetAmount(Type type, bool recurse)
		{
			return GetAmount(type, InitFindOptions(recurse));
		}

		public int GetAmount(Type type, FindOptions options)
		{
			var amount = 0;

			if (CheckLocked(this, options))
			{
				return amount;
			}

			var items = FindItemsByType(type, options);

			for (var i = 0; i < items.Length; ++i)
			{
				amount += items[i].Amount;
			}

			return amount;
		}

		public int GetAmount(Type[] types)
		{
			return GetAmount(types, FindOptions.Default);
		}

		public int GetAmount(Type[] types, bool recurse)
		{
			return GetAmount(types, InitFindOptions(recurse));
		}

		public int GetAmount(Type[] types, FindOptions options)
		{
			var amount = 0;

			if (CheckLocked(this, options))
			{
				return amount;
			}

			var items = FindItemsByType(types, options);

			for (var i = 0; i < items.Length; ++i)
			{
				amount += items[i].Amount;
			}

			return amount;
		}

		#endregion

		private static readonly List<Item> m_FindItemsList = new List<Item>();

		#region Non-Generic FindItem[s] by Type

		public Item[] FindItemsByType(Type type)
		{
			return FindItemsByType(type, FindOptions.Default);
		}

		public Item[] FindItemsByType(Type type, bool recurse)
		{
			return FindItemsByType(type, InitFindOptions(recurse));
		}

		public Item[] FindItemsByType(Type type, FindOptions options)
		{
			if (m_FindItemsList.Count > 0)
			{
				m_FindItemsList.Clear();
			}

			RecurseFindItemsByType(this, type, options, m_FindItemsList);

			return m_FindItemsList.ToArray();
		}

		private static void RecurseFindItemsByType(Item current, Type type, FindOptions options, List<Item> list)
		{
			if (current != null && current.Items.Count > 0)
			{
				if (CheckLocked(current, options))
				{
					return;
				}

				var items = current.Items;

				for (var i = 0; i < items.Count; ++i)
				{
					var item = items[i];

					if (CheckType(item, type))
					{
						list.Add(item);
					}

					if (CheckRecurse(item, options))
					{
						RecurseFindItemsByType(item, type, options, list);
					}
				}
			}
		}

		public Item[] FindItemsByType(Type[] types)
		{
			return FindItemsByType(types, FindOptions.Default);
		}

		public Item[] FindItemsByType(Type[] types, bool recurse)
		{
			return FindItemsByType(types, InitFindOptions(recurse));
		}

		public Item[] FindItemsByType(Type[] types, FindOptions options)
		{
			if (m_FindItemsList.Count > 0)
			{
				m_FindItemsList.Clear();
			}

			RecurseFindItemsByType(this, types, options, m_FindItemsList);

			return m_FindItemsList.ToArray();
		}

		private static void RecurseFindItemsByType(Item current, Type[] types, FindOptions options, List<Item> list)
		{
			if (current != null && current.Items.Count > 0)
			{
				if (CheckLocked(current, options))
				{
					return;
				}

				var items = current.Items;

				for (var i = 0; i < items.Count; ++i)
				{
					var item = items[i];

					if (CheckType(item, types))
					{
						list.Add(item);
					}

					if (CheckRecurse(item, options))
					{
						RecurseFindItemsByType(item, types, options, list);
					}
				}
			}
		}

		public Item FindItemByType(Type type)
		{
			return FindItemByType(type, FindOptions.Default);
		}

		public Item FindItemByType(Type type, bool recurse)
		{
			return FindItemByType(type, InitFindOptions(recurse));
		}

		public Item FindItemByType(Type type, FindOptions options)
		{
			return RecurseFindItemByType(this, type, options);
		}

		private static Item RecurseFindItemByType(Item current, Type type, FindOptions options)
		{
			if (current != null && current.Items.Count > 0)
			{
				if (CheckLocked(current, options))
				{
					return null;
				}

				var list = current.Items;

				for (var i = 0; i < list.Count; ++i)
				{
					var item = list[i];

					if (CheckType(item, type))
					{
						return item;
					}

					if (CheckRecurse(item, options))
					{
						var check = RecurseFindItemByType(item, type, options);

						if (check != null)
						{
							return check;
						}
					}
				}
			}

			return null;
		}

		public Item FindItemByType(Type[] types)
		{
			return FindItemByType(types, FindOptions.Default);
		}

		public Item FindItemByType(Type[] types, bool recurse)
		{
			return FindItemByType(types, InitFindOptions(recurse));
		}

		public Item FindItemByType(Type[] types, FindOptions options)
		{
			return RecurseFindItemByType(this, types, options);
		}

		private static Item RecurseFindItemByType(Item current, Type[] types, FindOptions options)
		{
			if (current != null && current.Items.Count > 0)
			{
				if (CheckLocked(current, options))
				{
					return null;
				}

				var list = current.Items;

				for (var i = 0; i < list.Count; ++i)
				{
					var item = list[i];

					if (CheckType(item, types))
					{
						return item;
					}
					
					if (CheckRecurse(item, options))
					{
						var check = RecurseFindItemByType(item, types, options);

						if (check != null)
						{
							return check;
						}
					}
				}
			}

			return null;
		}

		#endregion

		#region Generic FindItem[s] by Type

		public T[] FindItemsByType<T>() where T : Item
		{
			return FindItemsByType<T>(FindOptions.Default);
		}

		public T[] FindItemsByType<T>(bool recurse) where T : Item
		{
			return FindItemsByType<T>(InitFindOptions(recurse));
		}

		public T[] FindItemsByType<T>(FindOptions options) where T : Item
		{
			return FindItemsByType<T>(options, null);
		}

		public T[] FindItemsByType<T>(Predicate<T> predicate) where T : Item
		{
			return FindItemsByType(FindOptions.Default, predicate);
		}

		public T[] FindItemsByType<T>(bool recurse, Predicate<T> predicate) where T : Item
		{
			return FindItemsByType(InitFindOptions(recurse), predicate);
		}

		public T[] FindItemsByType<T>(FindOptions options, Predicate<T> predicate) where T : Item
		{
			if (m_FindItemsList.Count > 0)
			{
				m_FindItemsList.Clear();
			}

			RecurseFindItemsByType(this, options, m_FindItemsList, predicate);

			return m_FindItemsList.Cast<T>().ToArray();
		}

		private static void RecurseFindItemsByType<T>(Item current, FindOptions options, List<Item> list, Predicate<T> predicate) where T : Item
		{
			if (current != null && current.Items.Count > 0)
			{
				if (CheckLocked(current, options))
				{
					return;
				}

				var items = current.Items;

				for (var i = 0; i < items.Count; ++i)
				{
					var item = items[i];

					if (item is T typedItem)
					{
						if (predicate == null || predicate(typedItem))
						{
							list.Add(typedItem);
						}
					}

					if (CheckRecurse(item, options))
					{
						RecurseFindItemsByType(item, options, list, predicate);
					}
				}
			}
		}

		public T FindItemByType<T>() where T : Item
		{
			return FindItemByType<T>(FindOptions.Default);
		}

		public T FindItemByType<T>(bool recurse) where T : Item
		{
			return FindItemByType<T>(InitFindOptions(recurse));
		}

		public T FindItemByType<T>(FindOptions options) where T : Item
		{
			return FindItemByType<T>(options, null);
		}

		public T FindItemByType<T>(Predicate<T> predicate) where T : Item
		{
			return FindItemByType(FindOptions.Default, predicate);
		}

		public T FindItemByType<T>(bool recurse, Predicate<T> predicate) where T : Item
		{
			return FindItemByType(InitFindOptions(recurse), predicate);
		}

		public T FindItemByType<T>(FindOptions options, Predicate<T> predicate) where T : Item
		{
			return RecurseFindItemByType(this, options, predicate);
		}

		private static T RecurseFindItemByType<T>(Item current, FindOptions options, Predicate<T> predicate) where T : Item
		{
			if (current != null && current.Items.Count > 0)
			{
				if (CheckLocked(current, options))
				{
					return null;
				}

				var list = current.Items;

				for (var i = 0; i < list.Count; ++i)
				{
					var item = list[i];

					if (item is T typedItem)
					{
						if (predicate == null || predicate(typedItem))
						{
							return typedItem;
						}
					}

					if (CheckRecurse(item, options))
					{
						var check = RecurseFindItemByType(item, options, predicate);

						if (check != null)
						{
							return check;
						}
					}
				}
			}

			return null;
		}

		#endregion

		#region FindItem[s]

		public Item FindItem(Predicate<Item> predicate)
		{
			return FindItemByType<Item>(predicate);
		}

		public Item FindItem(bool recurse, Predicate<Item> predicate)
		{
			return FindItemByType<Item>(recurse, predicate);
		}

		public Item FindItem(FindOptions options, Predicate<Item> predicate)
		{
			return FindItemByType<Item>(options, predicate);
		}

		public Item[] FindItems()
		{
			return FindItemsByType<Item>();
		}

		public Item[] FindItems(bool recurse)
		{
			return FindItemsByType<Item>(recurse);
		}

		public Item[] FindItems(FindOptions options)
		{
			return FindItemsByType<Item>(options);
		}

		public Item[] FindItems(Predicate<Item> predicate)
		{
			return FindItemsByType<Item>(predicate);
		}

		public Item[] FindItems(bool recurse, Predicate<Item> predicate)
		{
			return FindItemsByType<Item>(recurse, predicate);
		}

		public Item[] FindItems(FindOptions options, Predicate<Item> predicate)
		{
			return FindItemsByType<Item>(options, predicate);
		}

		#endregion

		#region FindOptions

		[Flags]
		public enum FindOptions
		{
			None = 0x0,

			/// <summary>
			///		Includes child containers in the search.
			/// </summary>
			Recursive = 0x1,

			/// <summary>
			///		Containers that implement <see cref="ILockable"/> and have <see cref="ILockable.Locked"/> 
			///		set to true will have their contents excluded from the search.
			/// </summary>
			Unlocked = 0x2,

			/// <summary>
			///		A combination of the <see cref="Recursive"/> and <see cref="Unlocked"/> flags.
			/// </summary>
			RecursiveUnlocked = Recursive | Unlocked,

			/// <summary>
			///		Default behavior, equivalent to the <see cref="Recursive"/> flag.
			/// </summary>
			Default = Recursive,

			/// <summary>
			///		A combination of all <see cref="FindOptions"/> flags.
			/// </summary>
			All = ~None
		}

		private static bool CheckRecurse(Item item, FindOptions options)
		{
			return options.HasFlag(FindOptions.Recursive) && item is Container;
		}

		private static bool CheckLocked(Item item, FindOptions options)
		{
			return options.HasFlag(FindOptions.Unlocked) && item is ILockable lockable && lockable.Locked;
		}

		private static FindOptions InitFindOptions(bool recurse)
		{
			var value = FindOptions.Default;

			if (recurse)
			{
				value |= FindOptions.Recursive;
			}
			else
			{
				value &= ~FindOptions.Recursive;
			}

			return value;
		}

		#endregion

		private static bool CheckType(Item item, Type type)
		{
			return CheckType(item.GetType(), type);
		}

		private static bool CheckType(Type item, Type type)
		{
			return type.IsAssignableFrom(item);
		}

		private static bool CheckType(Item item, Type[] types)
		{
			var t = item.GetType();

			for (var i = 0; i < types.Length; ++i)
			{
				if (CheckType(t, types[i]))
				{
					return true;
				}
			}

			return false;
		}

		private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
		{
			if (setIf)
			{
				flags |= toSet;
			}
		}

		private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
		{
			return (flags & toGet) != 0;
		}

		[Flags]
		private enum SaveFlag : byte
		{
			None = 0x00000000,
			MaxItems = 0x00000001,
			GumpID = 0x00000002,
			DropSound = 0x00000004,
			LiftOverride = 0x00000008,
			GridPositions = 0x00000010,
			MaxWeight = 0x00000080,
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(3); // version

			var flags = SaveFlag.None;

			SetSaveFlag(ref flags, SaveFlag.MaxItems, m_MaxItems != -1);
			SetSaveFlag(ref flags, SaveFlag.MaxWeight, m_MaxWeight != -1);
			SetSaveFlag(ref flags, SaveFlag.GumpID, m_GumpID != -1);
			SetSaveFlag(ref flags, SaveFlag.DropSound, m_DropSound != -1);
			SetSaveFlag(ref flags, SaveFlag.LiftOverride, LiftOverride);

			writer.Write((byte)flags);

			if (GetSaveFlag(flags, SaveFlag.MaxItems))
			{
				writer.WriteEncodedInt(m_MaxItems);
			}

			if (GetSaveFlag(flags, SaveFlag.MaxWeight))
			{
				writer.WriteEncodedInt(m_MaxWeight);
			}

			if (GetSaveFlag(flags, SaveFlag.GumpID))
			{
				writer.WriteEncodedInt(m_GumpID);
			}

			if (GetSaveFlag(flags, SaveFlag.DropSound))
			{
				writer.WriteEncodedInt(m_DropSound);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 3:
				case 2:
					{
						var flags = (SaveFlag)reader.ReadByte();

						if (GetSaveFlag(flags, SaveFlag.MaxItems))
						{
							m_MaxItems = reader.ReadEncodedInt();
						}
						else
						{
							m_MaxItems = -1;
						}

						if (GetSaveFlag(flags, SaveFlag.MaxWeight))
						{
							m_MaxWeight = reader.ReadEncodedInt();
						}
						else
						{
							m_MaxWeight = -1;
						}

						if (GetSaveFlag(flags, SaveFlag.GumpID))
						{
							m_GumpID = reader.ReadEncodedInt();
						}
						else
						{
							m_GumpID = -1;
						}

						if (GetSaveFlag(flags, SaveFlag.DropSound))
						{
							m_DropSound = reader.ReadEncodedInt();
						}
						else
						{
							m_DropSound = -1;
						}

						LiftOverride = GetSaveFlag(flags, SaveFlag.LiftOverride);

						break;
					}
				case 1:
					{
						m_MaxItems = reader.ReadInt();
						goto case 0;
					}
				case 0:
					{
						if (version < 1)
						{
							m_MaxItems = GlobalMaxItems;
							m_MaxWeight = GlobalMaxWeight;
						}

						m_GumpID = reader.ReadInt();
						m_DropSound = reader.ReadInt();

						if (m_GumpID == DefaultGumpID)
						{
							m_GumpID = -1;
						}

						if (m_DropSound == DefaultDropSound)
						{
							m_DropSound = -1;
						}

						if (m_MaxItems == DefaultMaxItems)
						{
							m_MaxItems = -1;
						}

						if (m_MaxWeight == DefaultMaxWeight)
						{
							m_MaxWeight = -1;
						}

						reader.ReadPoint2D();
						reader.ReadPoint2D();

						break;
					}
			}

			UpdateContainerData();
		}

		[ConfigProperty("CarryWeight.GlobalMaxItems")]
		public static int GlobalMaxItems { get => Config.Get("CarryWeight.GlobalMaxItems", 125); set => Config.Set("CarryWeight.GlobalMaxItems", value); }

		[ConfigProperty("CarryWeight.GlobalMaxWeight")]
		public static int GlobalMaxWeight { get => Config.Get("CarryWeight.GlobalMaxWeight", 400); set => Config.Set("CarryWeight.GlobalMaxWeight", value); }

		public Container(int itemID)
			: base(itemID)
		{
			m_GumpID = -1;
			m_DropSound = -1;
			m_MaxItems = -1;
			m_MaxWeight = -1;

			UpdateContainerData();
		}

		public override int GetTotal(TotalType type)
		{
			switch (type)
			{
				case TotalType.Gold:
				return m_TotalGold;

				case TotalType.Items:
				return m_TotalItems;

				case TotalType.Weight:
				return m_TotalWeight;
			}

			return base.GetTotal(type);
		}

		public override void UpdateTotal(Item sender, TotalType type, int delta)
		{
			if (sender != this && delta != 0 && !sender.IsVirtualItem)
			{
				switch (type)
				{
					case TotalType.Gold:
					m_TotalGold += delta;
					break;

					case TotalType.Items:
					m_TotalItems += delta;
					InvalidateProperties();
					break;

					case TotalType.Weight:
					m_TotalWeight += delta;
					InvalidateProperties();
					break;
				}
			}

			base.UpdateTotal(sender, type, delta);
		}

		public override void UpdateTotals()
		{
			m_TotalGold = 0;
			m_TotalItems = 0;
			m_TotalWeight = 0;

			var items = m_Items;

			if (items == null)
			{
				return;
			}

			for (var i = 0; i < items.Count; ++i)
			{
				var item = items[i];

				item.UpdateTotals();

				if (item.IsVirtualItem)
				{
					continue;
				}

				m_TotalGold += item.TotalGold;
				m_TotalItems += item.TotalItems + 1;
				m_TotalWeight += item.TotalWeight + item.PileWeight;
			}
		}

		public Container(Serial serial)
			: base(serial)
		{
		}

		public virtual bool OnStackAttempt(Mobile from, Item stack, Item dropped)
		{
			if (!CheckHold(from, dropped, true, false, true))
			{
				return false;
			}

			return stack.StackWith(from, dropped);
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (TryDropItem(from, dropped, true))
			{
				from.SendSound(GetDroppedSound(dropped), GetWorldLocation());

				return true;
			}
			else
			{
				return false;
			}
		}

		public virtual bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
		{
			if (CheckStack(from, dropped))
			{
				if (!CheckHold(from, dropped, sendFullMessage, false, true))
				{
					return false;
				}

				var list = Items;

				for (var i = 0; i < list.Count; ++i)
				{
					var item = list[i];

					if (!(item is Container) && item.StackWith(from, dropped, false))
					{
						return true;
					}
				}
			}

			if (!CheckHold(from, dropped, sendFullMessage, true, true))
			{
				return false;
			}

			DropItem(dropped);

			return true;
		}

		public virtual void DropItem(Item dropped)
		{
			if (dropped == null)
			{
				return;
			}

			AddItem(dropped);

			var bounds = dropped.GetGraphicBounds();
			var ourBounds = Bounds;

			int x, y;

			if (bounds.Width >= ourBounds.Width)
			{
				x = (ourBounds.Width - bounds.Width) / 2;
			}
			else
			{
				x = Utility.Random(ourBounds.Width - bounds.Width);
			}

			if (bounds.Height >= ourBounds.Height)
			{
				y = (ourBounds.Height - bounds.Height) / 2;
			}
			else
			{
				y = Utility.Random(ourBounds.Height - bounds.Height);
			}

			x += ourBounds.X;
			x -= bounds.X;

			y += ourBounds.Y;
			y -= bounds.Y;

			dropped.Location = new Point3D(x, y, 0);
		}

		public virtual void Destroy()
		{
			var loc = GetWorldLocation();
			var map = Map;

			if (map != null && map != Map.Internal)
			{
				var items = Items;

				for (var i = items.Count - 1; i >= 0; --i)
				{
					if (i < items.Count)
					{
						items[i].SetLastMoved();
						items[i].MoveToWorld(loc, map);
					}
				}
			}

			Delete();
		}

		public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
		{
			if (!base.AllowSecureTrade(from, to, newOwner, accepted))
			{
				return false;
			}

			var allItems = FindItemsByType<Item>(true);

			return allItems.All(o => o.AllowSecureTrade(from, to, newOwner, accepted));
		}

		public override void OnDoubleClickSecureTrade(Mobile from)
		{
			if (from.InRange(GetWorldLocation(), 2))
			{
				DisplayTo(from);

				var cont = GetSecureTradeCont();

				if (cont != null)
				{
					var trade = cont.Trade;

					if (trade != null && trade.From.Mobile == from)
					{
						DisplayTo(trade.To.Mobile);
					}
					else if (trade != null && trade.To.Mobile == from)
					{
						DisplayTo(trade.From.Mobile);
					}
				}
			}
			else
			{
				from.SendLocalizedMessage(500446); // That is too far away.
			}
		}

		public virtual bool DisplaysContent => true;

		public virtual bool CheckContentDisplay(Mobile from)
		{
			if (!DisplaysContent)
			{
				return false;
			}

			var root = RootParent;

			if (root == null || root is Item || root == from || from.IsStaff())
			{
				return true;
			}

			return false;
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (CheckContentDisplay(from))
			{
				LabelTo(from, "({0} items, {1} stones)", TotalItems, TotalWeight);
			}
		}

		public List<Mobile> Openers { get; set; }

		public virtual bool IsPublicContainer => false;

		public override void OnDelete()
		{
			base.OnDelete();

			Openers = null;
		}

		public virtual void DisplayTo(Mobile to)
		{
			ProcessOpeners(to);

			var ns = to.NetState;

			if (ns == null)
			{
				return;
			}

			ContainerDisplay.Send(ns, this);
			ContainerContent.Send(ns, this);

			if (to.ViewOPL)
			{
				foreach (var o in Items)
				{
					to.Send(o.OPLPacket);
				}
			}
		}

		public void ProcessOpeners(Mobile opener)
		{
			if (!IsPublicContainer)
			{
				var contains = false;

				if (Openers != null)
				{
					var map = Map;

					for (var i = 0; i < Openers.Count; ++i)
					{
						var mob = Openers[i];

						if (mob == opener)
						{
							contains = true;
						}
						else if (mob.Map != map || !mob.InUpdateRange(this))
						{
							Openers.RemoveAt(i--);
						}
					}
				}

				if (!contains)
				{
					if (Openers == null)
					{
						Openers = new List<Mobile>();
					}

					Openers.Add(opener);
				}
				else if (Openers != null && Openers.Count == 0)
				{
					Openers = null;
				}
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (DisplaysContent) //CheckContentDisplay( from ) )
			{
				if (Core.ML)
				{
					if (MaxWeight <= 0 || IsLockedDown || IsSecure || ParentsContain<Item>()) //Root Parent is the Mobile.  Parent could be another containter.
					{
						list.Add(1073841, "{0}\t{1}\t{2}", TotalItems, MaxItems, TotalWeight);
						// Contents: ~1_COUNT~/~2_MAXCOUNT~ items, ~3_WEIGHT~ stones
					}
					else
					{
						list.Add(1072241, "{0}\t{1}\t{2}\t{3}", TotalItems, MaxItems, TotalWeight, MaxWeight);
						// Contents: ~1_COUNT~/~2_MAXCOUNT~ items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones
					}
				}
				else
				{
					list.Add(1050044, "{0}\t{1}", TotalItems, TotalWeight); // ~1_COUNT~ items, ~2_WEIGHT~ stones
				}
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from.IsStaff() || from.InRange(GetWorldLocation(), 2))
			{
				DisplayTo(from);
			}
			else
			{
				from.SendLocalizedMessage(500446); // That is too far away.
			}
		}
	}

	public class ContainerData
	{
		static ContainerData()
		{
			m_Table = new Dictionary<int, ContainerData>();

			var path = Path.Combine(Core.BaseDirectory, "Data/containers.cfg");

			if (!File.Exists(path))
			{
				Default = new ContainerData(0x3C, new Rectangle2D(44, 65, 142, 94), 0x48);
				return;
			}

			using (var reader = new StreamReader(path))
			{
				string line;

				while ((line = reader.ReadLine()) != null)
				{
					line = line.Trim();

					if (line.Length == 0 || line.StartsWith("#"))
					{
						continue;
					}

					try
					{
						var split = line.Split('\t');

						if (split.Length >= 3)
						{
							var gumpID = Utility.ToInt32(split[0]);

							var aRect = split[1].Split(' ');
							if (aRect.Length < 4)
							{
								continue;
							}

							var x = Utility.ToInt32(aRect[0]);
							var y = Utility.ToInt32(aRect[1]);
							var width = Utility.ToInt32(aRect[2]);
							var height = Utility.ToInt32(aRect[3]);

							var bounds = new Rectangle2D(x, y, width, height);

							var dropSound = Utility.ToInt32(split[2]);

							var data = new ContainerData(gumpID, bounds, dropSound);

							if (Default == null)
							{
								Default = data;
							}

							if (split.Length >= 4)
							{
								var aIDs = split[3].Split(',');

								for (var i = 0; i < aIDs.Length; i++)
								{
									var id = Utility.ToInt32(aIDs[i]);

									if (m_Table.ContainsKey(id))
									{
										Console.WriteLine(@"Warning: double ItemID entry in Data\containers.cfg");
									}
									else
									{
										m_Table[id] = data;
									}
								}
							}
						}
					}
					catch
					{ }
				}
			}

			if (Default == null)
			{
				Default = new ContainerData(0x3C, new Rectangle2D(44, 65, 142, 94), 0x48);
			}
		}

		private static readonly Dictionary<int, ContainerData> m_Table;

		public static ContainerData Default { get; set; }

		public static ContainerData GetData(int itemID)
		{
			m_Table.TryGetValue(itemID, out var data);

			if (data != null)
			{
				return data;
			}

			return Default;
		}

		public int GumpID { get; }
		public Rectangle2D Bounds { get; }
		public int DropSound { get; }

		public ContainerData(int gumpID, Rectangle2D bounds, int dropSound)
		{
			GumpID = gumpID;
			Bounds = bounds;
			DropSound = dropSound;
		}
	}
}
