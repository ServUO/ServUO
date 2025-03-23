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
using System.Globalization;
using System.IO;

using Server.Items;
using Server.Multis;
using Server.Network;

using VitaNex;
using VitaNex.Collections;
#endregion

namespace Server
{
	[Flags]
	public enum GiveFlags
	{
		None = 0x0,
		Pack = 0x1,
		Bank = 0x2,
		Corpse = 0x4,
		Feet = 0x8,

		Delete = 0x40000000,

		PackBank = Pack | Bank,
		PackCorpse = Pack | Corpse,
		PackFeet = Pack | Feet,
		PackDelete = Pack | Delete,

		PackBankCorpse = PackBank | Corpse,
		PackBankFeet = PackBank | Feet,
		PackBankDelete = PackBank | Delete,

		PackBankCorpseFeet = PackBankCorpse | Feet,
		PackBankCorpseDelete = PackBankCorpse | Delete,
		PackBankFeetDelete = PackBankFeet | Delete,

		PackBankCorpseFeetDelete = PackBankCorpseFeet | Delete,

		PackCorpseFeet = PackCorpse | Feet,
		PackCorpseDelete = PackCorpse | Delete,
		PackCorpseFeetDelete = PackCorpseFeet | Delete,

		PackFeetDelete = PackFeet | Delete,

		BankCorpse = Bank | Corpse,
		BankFeet = Bank | Feet,
		BankDelete = Bank | Delete,

		BankCorpseFeet = BankCorpse | Feet,
		BankCorpseDelete = BankCorpse | Delete,
		BankFeetDelete = BankFeet | Delete,

		BankCorpseFeetDelete = BankCorpseFeet | Delete,

		CorpseFeet = Corpse | Feet,
		CorpseDelete = Corpse | Delete,

		CorpseFeetDelete = CorpseFeet | Delete,

		FeetDelete = Feet | Delete,

		All = ~None
	}

	public static class ItemExtUtility
	{
		public static T BinaryClone<T>(this T item)
			where T : Item
		{
			var t = item.GetType();

			var o = t.CreateInstanceSafe<T>(Serial.NewItem);

			if (o != null)
			{
				try
				{
					using (var ms = new MemoryStream())
					{
						var w = ms.GetBinaryWriter();

						item.Serialize(w);

						ms.Position = 0;

						var r = ms.GetBinaryReader();

						o.Deserialize(r);

						w.Close();
						r.Close();
					}

					var m = o.Parent as Mobile;

					o.Amount = 1;

					if (o.Items != null)
					{
						o.Items.Clear();
					}

					o.Internalize();

					o.Parent = null;

					if (m != null)
					{
						o.OnRemoved(m);

						m.OnItemRemoved(o);
					}
				}
				catch
				{
					o.Delete();
					o = null;
				}
			}

			return o;
		}

		public static void InvalidateProperties<T>(this Item item)
		{
			if (item is T)
			{
				item.InvalidateProperties();
			}

			var i = item.Items.Count;

			while (--i >= 0)
			{
				InvalidateProperties<T>(item.Items[i]);
			}
		}

		public static void InvalidateProperties(this Item item, Type type)
		{
			if (item.TypeEquals(type))
			{
				item.InvalidateProperties();
			}

			var i = item.Items.Count;

			while (--i >= 0)
			{
				InvalidateProperties(item.Items[i], type);
			}
		}

		public static int GetAnimID(this Item item)
		{
			return item != null && item.Layer.IsValid() ? ArtworkSupport.LookupAnimation(item.ItemID) : 0;
		}

		public static int GetGumpID(this Item item, bool female)
		{
			return item != null && item.Layer.IsValid() ? ArtworkSupport.LookupGump(item.ItemID, female) : 0;
		}

		public static PaperdollBounds GetGumpBounds(this Item item)
		{
			if (item == null)
			{
				return PaperdollBounds.Empty;
			}

			if (item.Layer == Layer.TwoHanded)
			{
				if (item is BaseRanged)
				{
					return PaperdollBounds.MainHand;
				}

				if (item is BaseEquipableLight || item is BaseShield)
				{
					return PaperdollBounds.OffHand;
				}
			}

			return PaperdollBounds.Find(item.Layer);
		}

		public static Mobile FindOwner(this Item item)
		{
			return FindOwner<Mobile>(item);
		}

		public static TMobile FindOwner<TMobile>(this Item item)
			where TMobile : Mobile
		{
			if (item == null || item.Deleted)
			{
				return null;
			}

			var owner = item.RootParent as TMobile;

			if (owner == null)
			{
				var h = BaseHouse.FindHouseAt(item);

				if (h != null)
				{
					owner = h.Owner as TMobile;
				}
			}

			return owner;
		}

		public static GiveFlags GiveTo(this Item item, Mobile m, GiveFlags flags = GiveFlags.All, bool message = true)
		{
			if (item == null || item.Deleted || m == null || m.Deleted || flags == GiveFlags.None)
			{
				return GiveFlags.None;
			}

			var pack = flags.HasFlag(GiveFlags.Pack);
			var bank = flags.HasFlag(GiveFlags.Bank);
			var feet = flags.HasFlag(GiveFlags.Feet);
			var corpse = flags.HasFlag(GiveFlags.Corpse);
			var delete = flags.HasFlag(GiveFlags.Delete);

			if (pack && (m.Backpack == null || m.Backpack.Deleted || !m.Backpack.CheckHold(m, item, false)))
			{
				pack = false;
				flags &= ~GiveFlags.Pack;
			}

			if (bank && (!m.Player || !m.BankBox.CheckHold(m, item, false)))
			{
				bank = false;
				flags &= ~GiveFlags.Bank;
			}

			if (corpse && (m.Alive || m.Corpse == null || m.Corpse.Deleted))
			{
				corpse = false;
				flags &= ~GiveFlags.Corpse;
			}

			if (feet && (m.Map == null || m.Map == Map.Internal) && (m.LogoutMap == null || m.LogoutMap == Map.Internal))
			{
				feet = false;
				flags &= ~GiveFlags.Feet;
			}

			var result = VitaNexCore.TryCatchGet(
				f =>
				{
					if (pack && m.Backpack.DropItemStack(m, item))
					{
						return GiveFlags.Pack;
					}

					if (bank && m.BankBox.DropItemStack(m, item))
					{
						return GiveFlags.Bank;
					}

					if (corpse && m.Corpse.DropItemStack(m, item))
					{
						return GiveFlags.Corpse;
					}

					if (feet)
					{
						if (m.Map != null && m.Map != Map.Internal)
						{
							item.MoveToWorld(m.Location, m.Map);

							if (m.Player)
							{
								item.SendInfoTo(m.NetState);
							}

							return GiveFlags.Feet;
						}

						if (m.LogoutMap != null && m.LogoutMap != Map.Internal)
						{
							item.MoveToWorld(m.LogoutLocation, m.LogoutMap);

							return GiveFlags.Feet;
						}
					}

					if (delete)
					{
						item.Delete();

						return GiveFlags.Delete;
					}

					return GiveFlags.None;
				},
				flags);

			if (!message || result == GiveFlags.None || result == GiveFlags.Delete)
			{
				return result;
			}

			var amount = String.Empty;
			var name = ResolveName(item, m);

			var p = item.Stackable && item.Amount > 1;

			if (p)
			{
				amount = item.Amount.ToString("#,0") + " ";

				if (!Insensitive.EndsWith(name, "s") && !Insensitive.EndsWith(name, "z"))
				{
					name += "s";
				}
			}

			switch (result)
			{
				case GiveFlags.Pack:
					m.SendMessage("{0}{1} {2} been placed in your pack.", amount, name, p ? "have" : "has");
					break;
				case GiveFlags.Bank:
					m.SendMessage("{0}{1} {2} been placed in your bank.", amount, name, p ? "have" : "has");
					break;
				case GiveFlags.Corpse:
					m.SendMessage("{0}{1} {2} been placed in your corpse.", amount, name, p ? "have" : "has");
					break;
				case GiveFlags.Feet:
					m.SendMessage("{0}{1} {2} been placed at your feet.", amount, name, p ? "have" : "has");
					break;
			}

			return result;
		}

		public static bool WasReceived(this GiveFlags flags)
		{
			return flags != GiveFlags.None && flags != GiveFlags.Delete;
		}

		private static readonly Dictionary<Item, List<object>> _Actions = new Dictionary<Item, List<object>>();

		public static bool BeginAction(this Item item, object toLock)
		{

			if (!_Actions.TryGetValue(item, out var actions) || actions == null)
			{
				ObjectPool.Acquire(out actions);

				_Actions[item] = actions;
			}

			if (!actions.Contains(toLock))
			{
				actions.Add(toLock);
				return true;
			}

			return false;
		}

		public static bool CanBeginAction(this Item item, object toLock)
		{
			var actions = _Actions.GetValue(item);

			return actions == null || !actions.Contains(toLock);
		}

		public static void EndAction(this Item item, object toLock)
		{
			var actions = _Actions.GetValue(item);

			if (actions == null)
			{
				return;
			}

			actions.Remove(toLock);

			if (actions.Count == 0)
			{
				_Actions.Remove(item);

				ObjectPool.Free(ref actions);
			}
		}

		public static bool BeginAction<T>(this Item item, T locker, TimeSpan duration)
		{
			var o = BeginAction(item, locker);

			if (o)
			{
				Timer.DelayCall(duration, EndAction, Tuple.Create(item, locker));
			}

			return o;
		}

		private static void EndAction<T>(Tuple<Item, T> t)
		{
			if (!CanBeginAction(t.Item1, t.Item2))
			{
				EndAction(t.Item1, t.Item2);
			}
		}

		public static bool BeginAction<T>(this Item item, T locker, TimeSpan duration, Action<Item> callback)
		{
			var o = BeginAction(item, locker);

			if (o)
			{
				Timer.DelayCall(duration, EndAction, Tuple.Create(item, locker, callback));
			}

			return o;
		}

		private static void EndAction<T>(Tuple<Item, T, Action<Item>> t)
		{
			if (!CanBeginAction(t.Item1, t.Item2))
			{
				EndAction(t.Item1, t.Item2);

				if (t.Item3 != null)
				{
					t.Item3(t.Item1);
				}
			}
		}

		public static bool BeginAction<T>(this Item item, T locker, TimeSpan duration, Action<Item, T> callback)
		{
			var o = BeginAction(item, locker);

			if (o)
			{
				Timer.DelayCall(duration, EndAction, Tuple.Create(item, locker, callback));
			}

			return o;
		}

		private static void EndAction<T>(Tuple<Item, T, Action<Item, T>> t)
		{
			if (!CanBeginAction(t.Item1, t.Item2))
			{
				EndAction(t.Item1, t.Item2);

				if (t.Item3 != null)
				{
					t.Item3(t.Item1, t.Item2);
				}
			}
		}

		public static string ResolveName(this Item item)
		{
			return ResolveName(item, Clilocs.DefaultLanguage);
		}

		public static string ResolveName(this Item item, Mobile viewer)
		{
			return ResolveName(item, viewer.GetLanguage());
		}

		public static string ResolveName(this Item item, ClilocLNG lng)
		{
			if (item == null)
			{
				return String.Empty;
			}

			var label = item.GetOPLHeader(lng);

			if (!String.IsNullOrEmpty(label))
			{
				label = label.Replace("\t", " ").Replace("\u0009", " ").Replace("<br>", "\n").Replace("<BR>", "\n").Trim();
			}

			if (!String.IsNullOrEmpty(label))
			{
				label = label.StripHtml(false);
				label = label.Trim();

				int idx;

				if ((idx = label.IndexOf('\n')) >= 0)
				{
					if (idx > 0)
					{
						label = label.Substring(0, idx);
					}
					else
					{
						label = label.TrimStart('\n');
					}
				}

				label = label.Trim();

				if ((idx = label.IndexOf(' ')) >= 0)
				{
					if (idx > 0)
					{

						if (Int32.TryParse(label.Substring(0, idx), NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
						{
							label = label.Substring(idx + 1);
						}
					}
					else
					{
						label = label.TrimStart(' ');
					}
				}

				label = label.Trim();
			}

			if (String.IsNullOrWhiteSpace(label) && item.Name != null)
			{
				label = item.Name;
			}

			if (String.IsNullOrWhiteSpace(label) && item.DefaultName != null)
			{
				label = item.DefaultName;
			}

			if (String.IsNullOrWhiteSpace(label) && item.LabelNumber > 0)
			{
				label = lng.GetString(item.LabelNumber);
			}

			if (String.IsNullOrWhiteSpace(label) && TileData.ItemTable.InBounds(item.ItemID))
			{
				label = TileData.ItemTable[item.ItemID].Name;
			}

			if (String.IsNullOrWhiteSpace(label))
			{
				label = item.GetType().Name.SpaceWords();
			}

			if (!String.IsNullOrEmpty(label))
			{
				label = label.StripExcessWhiteSpace().Trim();
			}

			return label;
		}

		public static bool HasUsesRemaining(this Item item)
		{

			return CheckUsesRemaining(item, false, out var uses);
		}

		public static bool HasUsesRemaining(this Item item, out int uses)
		{
			return CheckUsesRemaining(item, false, out uses);
		}

		public static bool CheckUsesRemaining(this Item item, bool deplete, out int uses)
		{
			return CheckUsesRemaining(item, deplete ? 1 : 0, out uses);
		}

		public static bool CheckUsesRemaining(this Item item, int deplete, out int uses)
		{
			uses = -1;

			if (item == null || item.Deleted)
			{
				return false;
			}

			if (!(item is IUsesRemaining))
			{
				return true;
			}

			var u = (IUsesRemaining)item;

			if (u.UsesRemaining <= 0)
			{
				uses = 0;
				return false;
			}

			if (deplete > 0)
			{
				if (u.UsesRemaining < deplete)
				{
					uses = u.UsesRemaining;
					return false;
				}

				u.UsesRemaining = Math.Max(0, u.UsesRemaining - deplete);
			}

			uses = u.UsesRemaining;
			return true;
		}

		public static bool CheckUse(
			this Item item,
			Mobile from,
			bool handle = true,
			bool allowDead = false,
			int range = -1,
			bool packOnly = false,
			bool inTrade = false,
			bool inDisplay = true,
			AccessLevel access = AccessLevel.Player)
		{
			return CheckDoubleClick(item, from, handle, allowDead, range, packOnly, inTrade, inDisplay, access);
		}

		public static bool CheckDoubleClick(
			this Item item,
			Mobile from,
			bool handle = true,
			bool allowDead = false,
			int range = -1,
			bool packOnly = false,
			bool inTrade = false,
			bool inDisplay = true,
			AccessLevel access = AccessLevel.Player)
		{
			if (item == null || item.Deleted || from == null || from.Deleted)
			{
				return false;
			}

			if (from.AccessLevel < access)
			{
				if (handle)
				{
					from.SendMessage("You do not have sufficient access to use this item.");
				}

				return false;
			}

			if (!from.CanSee(item) && !(item is IAddon))
			{
				if (handle)
				{
					from.SendMessage("This item can't be seen.");
					item.OnDoubleClickCantSee(from);
				}

				return false;
			}

			if (!item.IsAccessibleTo(from))
			{
				if (handle)
				{
					item.OnDoubleClickNotAccessible(from);
				}

				return false;
			}

			if (item.InSecureTrade && !inTrade)
			{
				if (handle)
				{
					item.OnDoubleClickSecureTrade(from);
				}

				return false;
			}

			if (((item.Parent == null && !item.Movable && !item.IsLockedDown && !item.IsSecure && !item.InSecureTrade) ||
				 IsShopItem(item)) && !inDisplay)
			{
				if (handle)
				{
					from.SendMessage("This item can not be accessed because it is part of a display.");
				}

				return false;
			}

			if (!from.Alive && !allowDead)
			{
				if (handle)
				{
					item.OnDoubleClickDead(from);
				}

				return false;
			}

			if (range >= 0 && !packOnly && !from.InRange(item, range))
			{
				if (handle)
				{
					if (range > 0)
					{
						from.SendMessage("You must be within {0:#,0} paces to use this item.", range);
					}
					else
					{
						from.SendMessage("You must be standing on this item to use it.");
					}

					item.OnDoubleClickOutOfRange(from);
				}

				return false;
			}

			if (packOnly && item.RootParent != from)
			{
				if (handle)
				{
					// This item must be in your backpack.
					from.SendLocalizedMessage(1054107);
				}

				return false;
			}

			return true;
		}

		public static bool IsShopItem(this Item item)
		{
			return HasParent(item, "Server.Mobiles.GenericBuyInfo+DisplayCache");
		}

		public static bool IsParentOf(this Item item, Item child)
		{
			if (item == null || child == null || item == child)
			{
				return false;
			}

			var p = child.Parent as Item;

			while (p != null && p != item)
			{
				p = p.Parent as Item;
			}

			return item == p;
		}

		public static bool HasParent<TEntity>(this Item item)
			where TEntity : IEntity
		{
			return HasParent(item, typeof(TEntity));
		}

		public static bool HasParent(this Item item, string typeName)
		{
			if (item == null || String.IsNullOrWhiteSpace(typeName))
			{
				return false;
			}

			var t = Type.GetType(typeName, false, false) ?? ScriptCompiler.FindTypeByFullName(typeName, false) ??
					ScriptCompiler.FindTypeByName(typeName, false);

			return HasParent(item, t);
		}

		public static bool HasParent(this Item item, Type t)
		{
			if (item == null || t == null)
			{
				return false;
			}

			var p = item.Parent;

			while (p is Item)
			{
				if (p.GetType().IsEqualOrChildOf(t))
				{
					return true;
				}

				var i = (Item)p;

				if (i.Parent == null)
				{
					break;
				}

				p = i.Parent;
			}

			return p is Mobile && p.GetType().IsEqualOrChildOf(t);
		}

		public static bool IsEquipped(this Item item)
		{
			return item != null && item.Parent is Mobile && ((Mobile)item.Parent).FindItemOnLayer(item.Layer) == item;
		}

		public static bool IsEquippedBy(this Item item, Mobile m)
		{
			return item != null && item.Parent == m && m.FindItemOnLayer(item.Layer) == item;
		}

		#region *OverheadMessage
		public static void PrivateOverheadMessage(this Item item, MessageType type, int hue, bool ascii, string text, NetState state)
		{
			if (state == null)
			{
				return;
			}

			if (ascii)
			{
				state.Send(new AsciiMessage(item.Serial, item.ItemID, type, hue, 3, item.Name, text));
			}
			else
			{
				state.Send(new UnicodeMessage(item.Serial, item.ItemID, type, hue, 3, "ENU", item.Name, text));
			}
		}

		public static void PrivateOverheadMessage(this Item item, MessageType type, int hue, int number, NetState state)
		{
			PrivateOverheadMessage(item, type, hue, number, "", state);
		}

		public static void PrivateOverheadMessage(this Item item, MessageType type, int hue, int number, string args, NetState state)
		{
			if (state != null)
			{
				state.Send(new MessageLocalized(item.Serial, item.ItemID, type, hue, 3, number, item.Name, args));
			}
		}

		public static void NonlocalOverheadMessage(this Item item, MessageType type, int hue, int number)
		{
			NonlocalOverheadMessage(item, type, hue, number, "");
		}

		public static void NonlocalOverheadMessage(this Item item, MessageType type, int hue, int number, string args)
		{
			if (item == null || item.Map == null)
			{
				return;
			}

			var p = Packet.Acquire(new MessageLocalized(item.Serial, item.ItemID, type, hue, 3, number, item.Name, args));

			var eable = item.GetClientsInRange(Core.GlobalMaxUpdateRange);

			foreach (var state in eable)
			{
				if (state.Mobile.InUpdateRange(item) && state.Mobile.CanSee(item))
					state.Send(p);
			}

			eable.Free();

			Packet.Release(p);
		}

		public static void NonlocalOverheadMessage(this Item item, MessageType type, int hue, bool ascii, string text)
		{
			if (item == null || item.Map == null)
			{
				return;
			}

			Packet p;

			if (ascii)
			{
				p = new AsciiMessage(item.Serial, item.ItemID, type, hue, 3, item.Name, text);
			}
			else
			{
				p = new UnicodeMessage(item.Serial, item.ItemID, type, hue, 3, "ENU", item.Name, text);
			}

			p.Acquire();

			var eable = item.GetClientsInRange(Core.GlobalMaxUpdateRange);

			foreach (var state in eable)
			{
				if (state.Mobile.InUpdateRange(item) && state.Mobile.CanSee(item))
					state.Send(p);
			}

			eable.Free();

			Packet.Release(p);
		}
		#endregion
	}
}