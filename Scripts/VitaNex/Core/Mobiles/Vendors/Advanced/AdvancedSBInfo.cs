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
using System.Linq;
using System.Reflection;

using Server;
using Server.Commands;
using Server.Mobiles;
#endregion

namespace VitaNex.Mobiles
{
	public class AdvancedSBInfo : SBInfo
	{
		public IAdvancedVendor Vendor { get; private set; }

#if ServUOX
		private readonly List<IBuyItemInfo> _BuyInfo = new List<IBuyItemInfo>();
		public sealed override List<IBuyItemInfo> BuyInfo { get { return _BuyInfo; } }
#else
		private readonly List<GenericBuyInfo> _BuyInfo = new List<GenericBuyInfo>();
		public override sealed List<GenericBuyInfo> BuyInfo => _BuyInfo;
#endif

		private readonly AdvancedSellInfo _SellInfo;
		public override sealed IShopSellInfo SellInfo => _SellInfo;

		public int StockCount => _BuyInfo.Count;
		public int OrderCount => _SellInfo.Table.Count;

		public AdvancedSBInfo(IAdvancedVendor vendor)
		{
			Vendor = vendor;

			_SellInfo = new AdvancedSellInfo(this);

			InitBuyInfo();
		}

		public virtual void InitBuyInfo()
		{ }

		public void AddStock<TObj>(int price, string name = null, int amount = 100, object[] args = null)
		{
			AddStock(typeof(TObj), price, name, amount, args);
		}

		public virtual void AddStock(Type type, int price, string name = null, int amount = 100, object[] args = null)
		{
			AddStock(new AdvancedBuyInfo(this, type, price, name, amount, args));
		}

		public virtual void AddStock(AdvancedBuyInfo buy)
		{
			_BuyInfo.Add(buy);
		}

		public void RemoveStock<TObj>()
		{
			RemoveStock(typeof(TObj));
		}

		public virtual void RemoveStock(Type type)
		{
			_BuyInfo.OfType<AdvancedBuyInfo>().Where(b => b.Type.TypeEquals(type)).ForEach(RemoveStock);
		}

		public virtual void RemoveStock(AdvancedBuyInfo buy)
		{
			_BuyInfo.Remove(buy);
		}

		public void AddOrder<TObj>(int price)
		{
			AddOrder(typeof(TObj), price);
		}

		public virtual void AddOrder(Type type, int price)
		{
			_SellInfo.Add(type, price);
		}

		public void RemoveOrder<TObj>()
		{
			RemoveOrder(typeof(TObj));
		}

		public virtual void RemoveOrder(Type type)
		{
			_SellInfo.Remove(type);
		}

		public IEnumerable<KeyValuePair<Type, int>> EnumerateOrders()
		{
			return EnumerateOrders(null);
		}

		public IEnumerable<KeyValuePair<Type, int>> EnumerateOrders(Func<Type, int, bool> predicate)
		{
			return predicate != null ? _SellInfo.Table.Where(kv => predicate(kv.Key, kv.Value)) : _SellInfo.Table;
		}

		public IEnumerable<AdvancedBuyInfo> EnumerateStock()
		{
			return EnumerateStock(null);
		}

		public IEnumerable<AdvancedBuyInfo> EnumerateStock(Func<AdvancedBuyInfo, bool> predicate)
		{
			return predicate != null ? _BuyInfo.OfType<AdvancedBuyInfo>().Where(predicate) : _BuyInfo.OfType<AdvancedBuyInfo>();
		}
	}

	public class DynamicBuyInfo : GenericBuyInfo
	{
		private readonly bool _Init;

		public Item Item { get; private set; }

#if ServUOX
        public override int ControlSlots
        {
            get
            {
                if (!Item.GetPropertyValue("ControlSlots", out int slots))
                {
                    slots = base.ControlSlots;
                }

                return slots;
            }
            set
            {
                Item.SetPropertyValue("ControlSlots", value);

                base.ControlSlots = value;
            }
        }
#else
		private int? _Slots;

		public int Slots
		{
			get
			{
				var def = _Slots ?? base.ControlSlots;

				if (!Item.GetPropertyValue("ControlSlots", out int slots))
				{
					slots = def;
				}

				return slots;
			}
			set
            {
                Item.SetPropertyValue("ControlSlots", value);

                if (value >= 0 && value != base.ControlSlots)
				{
					_Slots = value;
				}
				else
				{
					_Slots = null;
				}
			}
		}

		public override int ControlSlots => Slots;
#endif

		public int RawPrice
		{
			get
			{
				var scalar = PriceScalar;

				PriceScalar = 0;

				var price = Price;

				PriceScalar = scalar;

				return price;
			}
			set 
			{
#if ServUOX
				PriceBase = value;
#else
				Price = value;
#endif
			}
		}

		public DynamicBuyInfo(Item item)
			: base(item.ResolveName(), item.GetType(), -1, item.Amount, item.ItemID, item.Hue)
		{
			Item = item;

			_Init = true;
		}

		public DynamicBuyInfo(GenericReader reader)
			: base("Item", typeof(Item), -1, 0, 0, 0)
		{
			Deserialize(reader);

			_Init = true;
		}

		public virtual void Serialize(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.Write(Item);

			writer.WriteType(Type);

			writer.Write(Name);
			writer.Write(ItemID);
			writer.Write(Hue);

			writer.Write(Amount);
			writer.Write(MaxAmount);

			writer.Write(RawPrice);
			writer.Write(PriceScalar);
		}

		public virtual void Deserialize(GenericReader reader)
		{
			reader.GetVersion();

			Item = reader.ReadItem();

			Type = reader.ReadType();

			Name = reader.ReadString();
			ItemID = reader.ReadInt();
			Hue = reader.ReadInt();

			Amount = reader.ReadInt();
			MaxAmount = reader.ReadInt();

			RawPrice = reader.ReadInt();
			PriceScalar = reader.ReadInt();
		}

		public void Update()
		{
			Update(Item);
		}

		public void Update(Item item)
		{
			if (item != null && item.Deleted)
			{
				item = null;
			}

			var changed = Item != item;

			if (changed)
			{
				DeleteDisplayEntity();
			}

			Item = item;

			if (Item != null)
			{
				if (changed)
				{
					Type = Item.GetType();
					Price = -1;
					Args = null;
				}

				Name = Item.ResolveName();
				Amount = MaxAmount = Item.Amount;
				ItemID = Item.ItemID;
				Hue = Item.Hue;
			}
			else
			{
				Type = null;
				Price = -1;
				Args = null;

				Name = null;
				Amount = MaxAmount = 0;
				ItemID = 0;
				Hue = 0;
			}
		}

		public void Free()
		{
			Update(null);
		}

		public override IEntity GetEntity()
		{
			if (!_Init)
			{
				return null;
			}

			if (Item != null && !Item.Deleted)
			{
#if !ServUO
				try
				{
					var ctor = Item.GetType().GetConstructor(Type.EmptyTypes);

					if (ctor.Invoke(null) is Item item)
					{
						Dupe.CopyProperties(item, Item);

						Item.OnAfterDuped(item);

                        item.Parent = null;

                        item.InvalidateProperties();

						return item;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Vendor entity creation failed:\n{0}", ex);
				}
#else
                return Dupe.DupeItem(Item);
#endif
			}

            return base.GetEntity();
		}
	}

	public class AdvancedBuyInfo : GenericBuyInfo
	{
		public AdvancedSBInfo Parent { get; private set; }

#if ServUOX
		public int Slots { get { return ControlSlots; } set { ControlSlots = value; } }
#else
		public int Slots { get; set; }

		public override sealed int ControlSlots => Slots;
#endif

		public AdvancedBuyInfo(AdvancedSBInfo parent, Type type, int price, string name = null, int amount = 100, object[] args = null)
			: base(name, type, price, amount, 0, 0, args)
		{
			Parent = parent;

			var e = GetDisplayEntity();

			if (e is Mobile)
			{
				var m = (Mobile)e;

				if (String.IsNullOrWhiteSpace(name))
				{
					Name = m.RawName ?? type.Name.SpaceWords();
				}
				else
				{
					m.RawName = name;
				}

				ItemID = ShrinkTable.Lookup(m);
				Hue = m.Hue;

				if (m is BaseCreature)
				{
					Slots = ((BaseCreature)m).ControlSlots;
				}
			}
			else if (e is Item)
			{
				var i = (Item)e;

				if (String.IsNullOrWhiteSpace(name))
				{
					Name = i.ResolveName();
				}
				else
				{
					i.Name = name;
				}

				ItemID = i.ItemID;
				Hue = i.Hue;

				if (i.GetPropertyValue("ControlSlots", out int slots))
				{
					Slots = slots;
				}
			}
			else if (String.IsNullOrWhiteSpace(name))
			{
				Name = type.Name.SpaceWords();
			}
		}
	}

	public class AdvancedSellInfo : GenericSellInfo
	{
		private static readonly FieldInfo _TableField =
			typeof(GenericSellInfo).GetField("m_Table", BindingFlags.Instance | BindingFlags.NonPublic) ??
			typeof(GenericSellInfo).GetField("_Table", BindingFlags.Instance | BindingFlags.NonPublic);

		public AdvancedSBInfo Parent { get; private set; }
		public Dictionary<Type, int> Table { get; private set; }

		public AdvancedSellInfo(AdvancedSBInfo parent)
		{
			Parent = parent;

			Table = _TableField.GetValue(this) as Dictionary<Type, int> ?? new Dictionary<Type, int>();
		}

		public void Add<TObj>(int price)
		{
			Add(typeof(TObj), price);
		}

		public virtual new void Add(Type type, int price)
		{
			base.Add(type, price);
		}

		public bool Remove<TObj>()
		{
			return Remove(typeof(TObj));
		}

		public virtual bool Remove(Type type)
		{
			return Table.Remove(type);
		}
	}
}
