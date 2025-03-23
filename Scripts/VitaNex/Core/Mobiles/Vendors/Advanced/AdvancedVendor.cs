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
using System.Drawing;
using System.Linq;

using Server;
using Server.Accounting;
using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;
using Server.Multis;

using VitaNex.Collections;
using VitaNex.Network;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Mobiles
{
	public class DynamicVendor : AdvancedVendor
	{
		[Constructable]
		public DynamicVendor()
			: base("the Vendor", typeof(Gold), "Gold", "GP")
		{ }

		public DynamicVendor(Serial serial)
			: base(serial)
		{ }

		protected override void InitBuyInfo()
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public abstract class AdvancedVendor : BaseVendor, IAdvancedVendor
	{
		public static event Action<AdvancedVendor> OnCreated;
		public static event Action<AdvancedSBInfo> OnInit;

		public static bool ConsumeCash(Type type, Container cont, double amount)
		{
			return ConsumeCash(type, cont, amount, true);
		}

		public static bool ConsumeCash(Type type, Container cont, double amount, bool recurse)
		{
			if (type == null || cont == null)
			{
				return false;
			}

			if (amount <= 0)
			{
				return true;
			}

			var items = new Queue<Item>(FindCash(type, cont, recurse));
			var total = items.Aggregate(0.0, (c, o) => c + o.Amount);

			if (total < amount)
			{
				items.Free(true);

				return false;
			}

			var consume = amount;

			while (consume > 0)
			{
				var o = items.Dequeue();

				if (o.Amount > consume)
				{
					o.Consume((int)consume);

					consume = 0;
				}
				else
				{
					consume -= o.Amount;

					o.Delete();
				}
			}

			items.Free(true);

			return true;
		}

		private static IEnumerable<Item> FindCash(Type type, Container cont, bool recurse)
		{
			if (type == null || cont == null || cont.Items.Count == 0)
			{
				yield break;
			}

			if (cont is ILockable && ((ILockable)cont).Locked)
			{
				yield break;
			}

			if (cont is TrapableContainer && ((TrapableContainer)cont).TrapType != TrapType.None)
			{
				yield break;
			}

			var count = cont.Items.Count;

			while (--count >= 0)
			{
				if (count >= cont.Items.Count)
				{
					continue;
				}

				var item = cont.Items[count];

				if (item is Container)
				{
					if (!recurse)
					{
						continue;
					}

					foreach (var o in FindCash(type, (Container)item, true))
					{
						yield return o;
					}
				}
				else if (item.TypeEquals(type))
				{
					yield return item;
				}
			}
		}

		public static TimeSpan DefaultRestockDelay = TimeSpan.FromMinutes(60);

		public static List<AdvancedVendor> Instances { get; private set; }

		static AdvancedVendor()
		{
			Instances = new List<AdvancedVendor>();
		}

		public static bool IsDynamicStock(Item item)
		{
			var root = item.RootParent as AdvancedVendor;

			return root != null && item.IsChildOf(root.Backpack) && root._DynamicStock.GetValue(item) != null;
		}

		private readonly Dictionary<Item, DynamicBuyInfo> _DynamicStock = new Dictionary<Item, DynamicBuyInfo>();

		public Dictionary<Item, DynamicBuyInfo> DynamicStock => _DynamicStock;

		private readonly List<SBInfo> _SBInfos = new List<SBInfo>();

		protected override sealed List<SBInfo> SBInfos => _SBInfos;

		public AdvancedSBInfo AdvancedStock { get; private set; }

		private DateTime _NextYell = DateTime.UtcNow.AddSeconds(Utility.RandomMinMax(30, 120));

		private Mobile _LastBuyer, _LastSeller;

		[CommandProperty(AccessLevel.Administrator)]
		public int Discount { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool DiscountEnabled { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool DiscountYell { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public ObjectProperty CashProperty { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public TypeSelectProperty<object> CashType { get; set; }

		private TextDefinition _CashName;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public TextDefinition CashName
		{
			get => _CashName;
			set
			{
				_CashName = value;
				InvalidateProperties();
			}
		}

		private TextDefinition _CashAbbr;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public TextDefinition CashAbbr
		{
			get => _CashAbbr;
			set
			{
				_CashAbbr = value;
				InvalidateProperties();
			}
		}

		private bool _ShowCashName;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool ShowCashName
		{
			get => _ShowCashName;
			set
			{
				_ShowCashName = value;
				InvalidateProperties();
			}
		}

		private bool _Trading = true;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool Trading
		{
			get => _Trading;
			set
			{
				_Trading = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool CanRestock { get; set; } = true;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public TimeSpan RestockInterval { get; set; } = DefaultRestockDelay;

		public override TimeSpan RestockDelay => RestockInterval;

		public override VendorShoeType ShoeType => Utility.RandomBool() ? VendorShoeType.ThighBoots : VendorShoeType.Boots;

		public virtual Race DefaultRace => Race.Human;

		public virtual Race[] RequiredRaces => null;

		public override bool CanTeach => false;

#if ServUOX
		public AdvancedVendor(string title, Type cashType, TextDefinition? cashName, TextDefinition? cashAbbr = null, bool showCashName = true)
#else
		public AdvancedVendor(string title, Type cashType, TextDefinition cashName, TextDefinition cashAbbr = null, bool showCashName = true)
#endif
			: this(title)
		{
			CashProperty = new ObjectProperty();
			CashType = cashType ?? typeof(Gold);
			CashName = cashName ?? "Gold";
			CashAbbr = cashAbbr ?? String.Empty;
			ShowCashName = showCashName;
		}

#if ServUOX
		public AdvancedVendor(string title, string cashProp, TextDefinition? cashName, TextDefinition? cashAbbr = null, bool showCashName = true)
#else
		public AdvancedVendor(string title, string cashProp, TextDefinition cashName, TextDefinition cashAbbr = null, bool showCashName = true)
#endif
			: this(title)
		{
			CashProperty = new ObjectProperty(cashProp);
			CashType = typeof(ObjectProperty);
			CashName = cashName ?? "Credits";
			CashAbbr = cashAbbr ?? String.Empty;
			ShowCashName = showCashName;
		}

		private AdvancedVendor(string title)
			: base(title)
		{
			Instances.Add(this);

			Female = Utility.RandomBool();
			Name = NameList.RandomName(Female ? "female" : "male");

			Race = DefaultRace;

			if (Backpack == null)
			{
				AddItem(new Backpack
				{
					Movable = false
				});
			}

			if (OnCreated != null)
			{
				Timer.DelayCall(v => OnCreated(v), this);
			}
		}

		public AdvancedVendor(Serial serial)
			: base(serial)
		{
			Instances.Add(this);
		}

		public override void OnSubItemAdded(Item item)
		{
			base.OnSubItemAdded(item);

			if (!World.Loading && item.IsChildOf(Backpack))
			{
				var info = _DynamicStock.GetValue(item);

				if (info == null)
				{
					_DynamicStock[item] = new DynamicBuyInfo(item);

					item.Movable = false;
				}
			}
		}

		public override void OnSubItemRemoved(Item item)
		{
			base.OnSubItemRemoved(item);

			if (!World.Loading)
			{
				var info = _DynamicStock.GetValue(item);

				if (_DynamicStock.Remove(item) && info != null)
				{
					info.Free();

					item.Movable = true;
				}
			}
		}

		public override void GetChildProperties(ObjectPropertyList list, Item item)
		{
			base.GetChildProperties(list, item);

			var info = _DynamicStock.GetValue(item);

			if (info != null)
			{
				var raw = info.RawPrice;
				var price = raw < 0 ? "Not For Sale" : raw == 0 ? "Free" : raw.ToString("#,0");

				// Price: ~1_COST~
				list.Add(1043304, price);
			}
		}

		public override void GetChildContextMenuEntries(Mobile m, List<ContextMenuEntry> list, Item item)
		{
			base.GetChildContextMenuEntries(m, list, item);

			if (m == null || m.AccessLevel < AccessLevel.GameMaster)
			{
				return;
			}

			var info = _DynamicStock.GetValue(item);

			if (info != null)
			{
				list.Add(new CustomContextEntry(1150627, u => OnSetPrice(u, info)));
			}
		}

		public override void GetContextMenuEntries(Mobile m, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(m, list);

			if (m != null && m.AccessLevel >= AccessLevel.GameMaster)
			{
				// Refresh
				list.Add(new CustomContextEntry(1015002, OnRestock));
			}
		}

		protected virtual void OnRestock(Mobile m)
		{
			Restock();

			if (m != null)
			{
				m.SendMessage("They have been restocked!");
			}
		}

		protected virtual void OnSetPrice(Mobile m, DynamicBuyInfo info)
		{
			new InputDialogGump(m)
			{
				InputText = info.RawPrice.ToString(),
				Icon = info.ItemID,
				IconHue = info.Hue,
				IconItem = true,
				Title = "Set Price",
				Html = "Enter a price for " + info.Name + "\n\nEnter -1 to mark not for sale.",
				Callback = (b, t) =>
				{
					if (Int32.TryParse(t, out var value))
					{
						info.RawPrice = Math.Max(-1, value);

						if (info.Item != null)
						{
							info.Item.InvalidateProperties();
						}
					}
				}
			}.Send();
		}

#if !ServUO
		public override int GetPriceScalar(Mobile buyer)
#else
        public override int GetPriceScalar()
#endif
        {
			var scalar = 100;

			if (CashType.TypeEquals<Gold>())
            {
#if !ServUO
                scalar = base.GetPriceScalar(buyer);
#else
                scalar = base.GetPriceScalar();
#endif
            }

			if (DiscountEnabled)
			{
				scalar -= Math.Max(0, Math.Min(100, Discount));
			}

			return Math.Max(0, scalar);
		}

		protected override void OnRaceChange(Race oldRace)
		{
			base.OnRaceChange(oldRace);

			Items.ForEachReverse(item =>
			{
				if (item != null && !item.Deleted && !(item is Container) && !(item is IMount) && !(item is IMountItem))
				{
					item.Delete();
				}
			});

			Hue = Race.RandomSkinHue();

#if ServUO
			FaceItemID = Race.RandomFace(this);
			FaceHue = Hue;
#endif
			
			HairItemID = Race.RandomHair(this);
			HairHue = Race.RandomHairHue();

			FacialHairItemID = Race.RandomFacialHair(this);
			FacialHairHue = HairHue;

			InitOutfit();
		}

		public virtual void ResolveCurrency(out Type type, out TextDefinition name)
		{
			type = CashType;
			name = CashName;
		}

		public virtual object GetCashObject(Mobile m)
		{
			return m;
		}

		private bool _WasStocked, _Restocking;

		public override void Restock()
		{
			if (_Restocking || !CanRestock)
			{
				return;
			}

			_Restocking = true;

			LastRestock = DateTime.UtcNow;

			var buyInfo = GetBuyInfo(null);

			foreach (var bii in buyInfo)
			{
				if (bii is DynamicBuyInfo)
				{
					((DynamicBuyInfo)bii).Update();
				}
				else
				{
					bii.OnRestock();
				}
			}

			if (!_WasStocked)
			{
				OnStocked();
			}
			else
			{
				OnRestocked();
			}

			_WasStocked = true;
			_Restocking = false;
		}

		public override sealed void InitSBInfo()
		{
			ClearBuyInfo();

			_SBInfos.Add(AdvancedStock = new AdvancedSBInfo(this));

			InitBuyInfo();

			if (OnInit != null)
			{
				OnInit(AdvancedStock);
			}
		}

		public void ClearBuyInfo()
		{
			_SBInfos.ForEach(sb => sb.BuyInfo.ForEach(b => b.DeleteDisplayEntity()));
			_SBInfos.Clear();
		}

		protected virtual void OnStocked()
		{ }

		protected virtual void OnRestocked()
		{ }

		protected abstract void InitBuyInfo();

		public void AddStock<TObj>(int price, string name = null, int amount = 100, object[] args = null)
		{
			AddStock(typeof(TObj), price, name, amount, args);
		}

		public virtual void AddStock(Type type, int price, string name = null, int amount = 100, object[] args = null)
		{
			AdvancedStock.AddStock(type, price, name, amount, args);
		}

		public void RemoveStock<TObj>()
		{
			RemoveStock(typeof(TObj));
		}

		public virtual void RemoveStock(Type type)
		{
			AdvancedStock.RemoveStock(type);
		}

		public void AddOrder<TObj>(int price)
		{
			AddOrder(typeof(TObj), price);
		}

		public virtual void AddOrder(Type type, int price)
		{
			AdvancedStock.AddOrder(type, price);
		}

		public void RemoveOrder<TObj>()
		{
			RemoveOrder(typeof(TObj));
		}

		public virtual void RemoveOrder(Type type)
		{
			AdvancedStock.RemoveOrder(type);
		}

		public override bool CheckVendorAccess(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (m.AccessLevel >= AccessLevel.GameMaster)
			{
				return true;
			}

			if (!Trading || !DesignContext.Check(m))
			{
				return false;
			}

			var races = RequiredRaces;

			if (races != null && races.Length > 0 && (m.Race == null || !races.Contains(m.Race)))
			{
				return false;
			}

			return base.CheckVendorAccess(m);
		}

		public override void OnThink()
		{
			base.OnThink();

			if (Deleted || Map == null || Map == Map.Internal)
			{
				return;
			}

			if (RestockDelay > TimeSpan.Zero && LastRestock + RestockDelay < DateTime.UtcNow)
			{
				Restock();
			}

			if (!DiscountEnabled || !DiscountYell || Discount <= 0 || DateTime.UtcNow <= _NextYell)
			{
				return;
			}

			Yell("Sale! {0}% Off!", Discount.ToString("#,0"));
			_NextYell = DateTime.UtcNow.AddSeconds(Utility.RandomMinMax(20, 120));
		}

		public override void OnDelete()
		{
			base.OnDelete();

			Instances.Remove(this);
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			Instances.Remove(this);
		}

		private GenericBuyInfo LookupDisplayObject(IBuyItemInfo[] buyInfo, object obj)
		{
			return buyInfo.OfType<GenericBuyInfo>().FirstOrDefault(gbi => gbi.GetDisplayEntity() == obj);
		}

		private void ProcessSinglePurchase(
			BuyItemResponse buy,
			IBuyItemInfo bii,
			ICollection<BuyItemResponse> validBuy,
			ref int controlSlots,
			ref bool fullPurchase,
			ref double totalCost)
		{
			if (!Trading || CashType == null || !CashType.IsNotNull)
			{
				return;
			}

			var amount = buy.Amount;

			if (amount > bii.Amount)
			{
				amount = bii.Amount;
			}

			if (amount <= 0)
			{
				return;
			}

			var slots = bii.ControlSlots * amount;

			if (controlSlots >= slots)
			{
				controlSlots -= slots;
			}
			else
			{
				fullPurchase = false;
				return;
			}

			totalCost += (double)bii.Price * amount;
			validBuy.Add(buy);
		}

		private void ProcessValidPurchase(int amount, IBuyItemInfo bii, Mobile buyer, Container cont)
		{
			if (!Trading || CashType == null || !CashType.IsNotNull)
			{
				return;
			}

			if (amount > bii.Amount)
			{
				amount = bii.Amount;
			}

			if (amount < 1)
			{
				return;
			}

			bii.Amount -= amount;

			var o = bii.GetEntity();

			if (o is Item)
			{
				var item = (Item)o;

				if (item.Stackable)
				{
					item.Amount = amount;

					if (cont != null)
					{
						cont.DropItem(item);
					}
					else
					{
						item.MoveToWorld(buyer.Location, buyer.Map);
					}

					OnItemReceived(buyer, item, bii);

					if (cont != null && !item.Deleted && item.IsChildOf(cont))
					{
						cont.MergeStacks(item.GetType(), buyer);
					}
				}
				else
				{
					item.Stackable = true;
					item.Amount = 1;
					item.Stackable = false;

					if (cont != null)
					{
						cont.DropItem(item);
					}
					else
					{
						item.MoveToWorld(buyer.Location, buyer.Map);
					}

					OnItemReceived(buyer, item, bii);

					for (var i = 1; i < amount; i++)
					{
						item = (Item)bii.GetEntity();

						if (item == null)
						{
							continue;
						}

						item.Stackable = true;
						item.Amount = 1;
						item.Stackable = false;

						if (cont != null)
						{
							cont.DropItem(item);
						}
						else
						{
							item.MoveToWorld(buyer.Location, buyer.Map);
						}

						OnItemReceived(buyer, item, bii);
					}
				}
			}
			else if (o is Mobile)
			{
				var m = (Mobile)o;

				m.Direction = (Direction)Utility.Random(8);
				m.MoveToWorld(buyer.Location, buyer.Map);
				m.PlaySound(m.GetIdleSound());

				if (m is BaseCreature)
				{
					((BaseCreature)m).SetControlMaster(buyer);
				}

				OnMobileReceived(buyer, m, bii);

				for (var i = 1; i < amount; ++i)
				{
					m = (Mobile)bii.GetEntity();

					if (m == null)
					{
						continue;
					}

					m.Direction = (Direction)Utility.Random(8);
					m.MoveToWorld(buyer.Location, buyer.Map);

					if (m is BaseCreature)
					{
						((BaseCreature)m).SetControlMaster(buyer);
					}

					OnMobileReceived(buyer, m, bii);
				}
			}
		}

		protected virtual void OnItemReceived(Mobile buyer, Item item, IBuyItemInfo buy)
		{
			if (buy is DynamicBuyInfo)
			{
				item.Movable = true;

				if (item is Container)
				{
					foreach (var o in ((Container)item).FindItemsByType<Item>(true, o => !o.Movable))
					{
						o.Movable = true;
					}
				}
			}
		}

		protected virtual void OnMobileReceived(Mobile buyer, Mobile mob, IBuyItemInfo buy)
		{ }

		public virtual bool CanSellItem(Item item, IShopSellInfo ssi)
		{
			if (item == null || item.Deleted || !item.Movable || !item.IsStandardLoot() || !ssi.IsSellable(item))
			{
				return false;
			}

			if (_DynamicStock.GetValue(item) != null)
			{
				return false;
			}

			if (item is Container && item.Items.Count > 0)
			{
				return false;
			}

			var p = item.Parent as Item;

			while (p != null)
			{
				if (p is ILockable && ((ILockable)p).Locked)
				{
					return false;
				}

				p = p.Parent as Item;
			}

			return true;
		}

		public override IBuyItemInfo[] GetBuyInfo()
		{
			return GetBuyInfo(_LastBuyer);
		}

		public override IShopSellInfo[] GetSellInfo()
		{
			return GetSellInfo(_LastSeller);
		}

		public virtual IBuyItemInfo[] GetBuyInfo(Mobile m)
		{
			var info = base.GetBuyInfo();

			if (_DynamicStock.Count > 0)
			{
				var buffer = ListPool<IBuyItemInfo>.AcquireObject();

				buffer.AddRange(_DynamicStock.Values.Where(o => o.RawPrice >= 0));

				buffer.AddRange(info);

				info = buffer.ToArray();

				ObjectPool.Free(ref buffer);
			}

#if !ServUO
			var priceScalar = GetPriceScalar(m);
#else
            var priceScalar = GetPriceScalar();
#endif

			foreach (var o in info)
			{
				o.PriceScalar = priceScalar;
			}

			return info;
		}

		public virtual IShopSellInfo[] GetSellInfo(Mobile m)
		{
			return base.GetSellInfo();
		}

		public override void VendorBuy(Mobile m)
		{
			_LastBuyer = m;

			base.VendorBuy(m);
		}

		public override void VendorSell(Mobile m)
		{
			_LastSeller = m;

			base.VendorSell(m);
		}

		public override bool OnBuyItems(Mobile buyer, List<BuyItemResponse> list)
		{
			if (!Trading || !IsActiveSeller || CashType == null || !CashType.IsNotNull)
			{
				return false;
			}

			if (!buyer.CheckAlive())
			{
				return false;
			}

			if (!CheckVendorAccess(buyer))
			{
				Say("I can't serve you! Company policy.");
				//Say(501522); // I shall not treat with scum like thee!
				return false;
			}

            UpdateBuyInfo();

			var info = GetSellInfo(buyer);
			var buyInfo = GetBuyInfo(buyer);
			var totalCost = 0.0;
			var validBuy = new List<BuyItemResponse>(list.Count);
			var fromBank = false;
			var fullPurchase = true;
			var controlSlots = buyer.FollowersMax - buyer.Followers;

			foreach (var buy in list)
			{
				var ser = buy.Serial;
				var amount = buy.Amount;

				if (ser.IsItem)
				{
					var item = World.FindItem(ser);

					if (item == null)
					{
						continue;
					}

					var gbi = LookupDisplayObject(buyInfo, item);

					if (gbi != null)
					{
						ProcessSinglePurchase(buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost);
					}
					else if (item != BuyPack && item.IsChildOf(BuyPack))
					{
						if (amount > item.Amount)
						{
							amount = item.Amount;
						}

						if (amount <= 0)
						{
							continue;
						}

						foreach (var ssi in info.Where(ssi => ssi.IsSellable(item) && ssi.IsResellable(item)))
						{
							totalCost += (double)ssi.GetBuyPriceFor(item) * amount;
							validBuy.Add(buy);
							break;
						}
					}
				}
				else if (ser.IsMobile)
				{
					var mob = World.FindMobile(ser);

					if (mob == null)
					{
						continue;
					}

					var gbi = LookupDisplayObject(buyInfo, mob);

					if (gbi != null)
					{
						ProcessSinglePurchase(buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost);
					}
				}
			}

			if (fullPurchase && validBuy.Count == 0)
			{
				// Thou hast bought nothing!
				SayTo(buyer, 500190);
			}
			else if (validBuy.Count == 0)
			{
				// Your order cannot be fulfilled, please try again.
				SayTo(buyer, 500187);
			}

			if (validBuy.Count == 0)
			{
				return false;
			}

			var bought = buyer.AccessLevel >= AccessLevel.GameMaster;

			ConsumeCurrency(buyer, ref totalCost, ref bought, ref fromBank);

			if (!bought)
			{
				// Begging thy pardon, but thou cant afford that.
				SayTo(buyer, 500192);
				return false;
			}

			buyer.PlaySound(0x32);

			var cont = buyer.Backpack ?? buyer.BankBox;

			foreach (var buy in validBuy)
			{
				var ser = buy.Serial;
				var amount = buy.Amount;

				if (amount < 1)
				{
					continue;
				}

				if (ser.IsItem)
				{
					var item = World.FindItem(ser);

					if (item == null)
					{
						continue;
					}

					var gbi = LookupDisplayObject(buyInfo, item);

					if (gbi != null)
					{
						ProcessValidPurchase(amount, gbi, buyer, cont);
					}
					else
					{
						if (amount > item.Amount)
						{
							amount = item.Amount;
						}

						if (info.Where(ssi => ssi.IsSellable(item)).Any(ssi => ssi.IsResellable(item)))
						{
							Item buyItem;

							if (amount >= item.Amount)
							{
								buyItem = item;
							}
							else
							{
								buyItem = LiftItemDupe(item, item.Amount - amount) ?? item;
							}

							if (cont != null)
							{
								cont.DropItem(buyItem);
							}
							else
							{
								buyItem.MoveToWorld(buyer.Location, buyer.Map);
							}

							OnItemReceived(buyer, buyItem, null);

							if (cont != null && !buyItem.Deleted && buyItem.Stackable)
							{
								cont.MergeStacks(buyItem.GetType(), buyer);
							}
						}
					}
				}
				else if (ser.IsMobile)
				{
					var mob = World.FindMobile(ser);

					if (mob == null)
					{
						continue;
					}

					var gbi = LookupDisplayObject(buyInfo, mob);

					if (gbi != null)
					{
						ProcessValidPurchase(amount, gbi, buyer, cont);
					}
				}
			}

			if (fullPurchase)
			{
				if (buyer.AccessLevel >= AccessLevel.GameMaster)
				{
					SayTo(buyer, true, "I would not presume to charge thee anything.  Here are the goods you requested.");
				}
				else if (fromBank)
				{
					SayTo(
						buyer,
						true,
						"The total of thy purchase is {0:#,0} {1}, which has been withdrawn from your bank account.  My thanks for the patronage.",
						totalCost,
						CashName.GetString(buyer));
				}
				else
				{
					SayTo(
						buyer,
						true,
						"The total of thy purchase is {0:#,0} {1}.  My thanks for the patronage.",
						totalCost,
						CashName.GetString(buyer));
				}
			}
			else if (buyer.AccessLevel >= AccessLevel.GameMaster)
			{
				SayTo(
					buyer,
					true,
					"I would not presume to charge thee anything.  " +
					"Unfortunately, I could not sell you all the goods you requested.");
			}
			else if (fromBank)
			{
				SayTo(
					buyer,
					true,
					"The total of thy purchase is {0:#,0} {1}, which has been withdrawn from your bank account.  " +
					"My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.",
					totalCost,
					CashName.GetString(buyer));
			}
			else
			{
				SayTo(
					buyer,
					true,
					"The total of thy purchase is {0:#,0} {1}.  My thanks for the patronage.  " +
					"Unfortunately, I could not sell you all the goods you requested.",
					totalCost,
					CashName.GetString(buyer));
			}

			return true;
		}

		public virtual void ConsumeCurrency(Mobile buyer, ref double totalCost, ref bool bought, ref bool fromBank)
		{
			if (!bought && CashType.TypeEquals<ObjectProperty>())
			{
				var cashSource = GetCashObject(buyer) ?? buyer;

				bought = CashProperty.Consume(cashSource, totalCost);

				if (bought)
				{
					SayTo(buyer, "{0:#,0} {1} has been deducted from your total.", totalCost, CashName.GetString(buyer));
				}
				else
				{
					// Begging thy pardon, but thou cant afford that.
					SayTo(buyer, 500192);
				}

				return;
			}

			var isGold = CashType.TypeEquals<Gold>();

			var cont = buyer.Backpack;

			if (!bought && cont != null && isGold)
			{
				try
				{
					var lt = ScriptCompiler.FindTypeByName("GoldLedger") ?? Type.GetType("GoldLedger");

					if (lt != null)
					{
						var ledger = cont.FindItemByType(lt);

						if (ledger != null && !ledger.Deleted)
						{
							var lp = lt.GetProperty("Gold");

							if (lp != null)
							{
								if (lp.PropertyType.IsEqual<long>())
								{
									var lg = (long)lp.GetValue(ledger, null);

									if (lg >= totalCost)
									{
										lp.SetValue(ledger, lg - (long)totalCost, null);
										bought = true;
									}
								}
								else if (lp.PropertyType.IsEqual<int>())
								{
									var lg = (int)lp.GetValue(ledger, null);

									if (lg >= totalCost)
									{
										lp.SetValue(ledger, lg - (int)totalCost, null);
										bought = true;
									}
								}

								if (bought)
								{
									buyer.SendMessage(2125, "{0:#,0} gold has been withdrawn from your ledger.", totalCost);
									return;
								}
							}
						}
					}
				}
				catch
				{ }
			}

			if (!bought && cont != null && ConsumeCash(CashType, cont, totalCost))
			{
				bought = true;
			}

			if (!bought && isGold)
			{
				if (totalCost <= Int32.MaxValue)
				{
					if (Banker.Withdraw(buyer, (int)totalCost))
					{
						bought = true;
						fromBank = true;
					}
				}
				else if (buyer.Account != null && AccountGold.Enabled)
				{
					if (buyer.Account.WithdrawCurrency(totalCost / AccountGold.CurrencyThreshold))
					{
						bought = true;
						fromBank = true;
					}
				}
			}

			if (!bought && !isGold)
			{
				cont = buyer.FindBankNoCreate();

				if (cont != null && ConsumeCash(CashType, cont, totalCost))
				{
					bought = true;
					fromBank = true;
				}
			}
		}

		public override bool OnSellItems(Mobile seller, List<SellItemResponse> list)
		{
			if (!Trading || CashType == null || !CashType.IsNotNull)
			{
				return false;
			}

			if (!IsActiveBuyer)
			{
				return false;
			}

			if (!seller.CheckAlive())
			{
				return false;
			}

			if (!CheckVendorAccess(seller))
			{
				Say("I can't serve you! Company policy.");
				//Say(501522); // I shall not treat with scum like thee!
				return false;
			}

			seller.PlaySound(0x32);

			var info = GetSellInfo(seller);
			var buyInfo = GetBuyInfo(seller);
			var giveCurrency = 0.0;
			Container cont;

			var finalList = list.Where(resp => CanSell(seller, info, resp)).ToArray();

			if (finalList.Length > 500)
			{
				SayTo(seller, true, "You may only sell 500 items at a time!");
				return false;
			}

			if (finalList.Length == 0)
			{
				return true;
			}

			foreach (var resp in finalList)
			{
				foreach (var ssi in info)
				{
					if (!ssi.IsSellable(resp.Item))
					{
						continue;
					}

					var amount = resp.Amount;

					if (amount > resp.Item.Amount)
					{
						amount = resp.Item.Amount;
					}

					var worth = (double)GetSellPrice(seller, ssi, resp) * amount;

					if (ssi.IsResellable(resp.Item))
					{
						var found = false;

						if (buyInfo.Any(bii => bii.Restock(resp.Item, amount)))
						{
							resp.Item.Consume(amount);
							found = true;
						}

						if (!found)
						{
							cont = BuyPack;

							if (amount < resp.Item.Amount)
							{
								var item = LiftItemDupe(resp.Item, resp.Item.Amount - amount);

								cont.DropItem(item ?? resp.Item);
							}
							else
							{
								cont.DropItem(resp.Item);
							}
						}
					}
					else
					{
						resp.Item.Consume(amount);
					}

					giveCurrency += worth;
					break;
				}
			}

			if (giveCurrency <= 0)
			{
				return false;
			}

			while (giveCurrency > 0)
			{
				Item c = null;

				if (CashType.TypeEquals<ObjectProperty>())
				{
					var cashSource = GetCashObject(seller) ?? seller;

					if (!CashProperty.Add(cashSource, giveCurrency))
					{
						c = new Static(0x14F0, 1)
						{
							Name = String.Format("Staff I.O.U [{0:#,0} {1}]", giveCurrency, CashName.GetString(seller)),
							Hue = 85,
							Movable = true,
							BlessedFor = seller,
							LootType = LootType.Blessed
						};
					}

					giveCurrency = 0;
				}

				if (c == null && giveCurrency > 0)
				{
					c = CashType.CreateInstance<Item>();

					if (c == null)
					{
						c = new Static(0x14F0, 1)
						{
							Name = String.Format("Staff I.O.U [{0:#,0} {1}]", giveCurrency, CashName.GetString(seller)),
							Hue = 85,
							Movable = true,
							BlessedFor = seller,
							LootType = LootType.Blessed
						};

						giveCurrency = 0;
					}
					else
					{
						if (giveCurrency >= 60000)
						{
							c.Amount = 60000;
							giveCurrency -= 60000;
						}
						else
						{
							c.Amount = (int)giveCurrency;
							giveCurrency = 0;
						}
					}
				}

				c.GiveTo(seller);
			}

			seller.PlaySound(0x0037); //Gold dropping sound

			return true;
		}

		public virtual bool CanSell(Mobile seller, IShopSellInfo[] info, SellItemResponse resp)
		{
			return resp.Item != null && resp.Item.RootParent == seller && resp.Amount > 0 && resp.Item.Movable &&
				   (!(resp.Item is Container) || resp.Item.Items.Count == 0) && info.Any(ssi => ssi.IsSellable(resp.Item)) &&
				   _DynamicStock.GetValue(resp.Item) == null;
		}

		public virtual int GetSellPrice(Mobile seller, IShopSellInfo info, SellItemResponse resp)
		{
			return info.GetSellPriceFor(resp.Item);
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			using (var eopl = new ExtendedOPL(list))
			{
				if (!Trading)
				{
					eopl.Add("Not Trading".WrapUOHtmlColor(Color.OrangeRed));
					return;
				}

				if (!ShowCashName)
				{
					return;
				}

				var name = CashName.GetString();

				if (!String.IsNullOrWhiteSpace(name))
				{
					eopl.Add("Trades For {0}".WrapUOHtmlColor(Color.SkyBlue), name);
				}

				var races = RequiredRaces;

				if (races != null && races.Length > 0)
				{
					foreach (var r in races)
					{
						eopl.Add("Trades With {0}".WrapUOHtmlColor(Color.LawnGreen), r.PluralName);
					}
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			_DynamicStock.RemoveRange(o => o.Key.Deleted || o.Value == null || o.Value.Item != o.Key);

			base.Serialize(writer);

			var version = writer.SetVersion(5);

			switch (version)
			{
				case 5:
					writer.Write(RestockInterval);
					goto case 4;
				case 4:
				{
					writer.WriteDictionary(_DynamicStock, (w, item, info) => info.Serialize(w));
				}
				goto case 3;
				case 3:
				{
					writer.Write(_WasStocked);
					writer.Write(CanRestock);
				}
				goto case 2;
				case 2:
					writer.WriteTextDef(_CashAbbr);
					goto case 1;
				case 1:
					CashProperty.Serialize(writer);
					goto case 0;
				case 0:
				{
					CashType.Serialize(writer);

					writer.WriteTextDef(_CashName);
					writer.Write(_ShowCashName);

					writer.Write(_Trading);

					writer.Write(Discount);
					writer.Write(DiscountEnabled);
					writer.Write(DiscountYell);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 5:
					RestockInterval = reader.ReadTimeSpan();
					goto case 4;
				case 4:
				{
					reader.ReadDictionary(
						r =>
						{
							var info = new DynamicBuyInfo(r);

							return new KeyValuePair<Item, DynamicBuyInfo>(info.Item, info);
						},
						_DynamicStock);
				}
				goto case 3;
				case 3:
				{
					_WasStocked = reader.ReadBool();
					CanRestock = reader.ReadBool();
				}
				goto case 2;
				case 2:
					_CashAbbr = reader.ReadTextDef();
					goto case 1;
				case 1:
					CashProperty = new ObjectProperty(reader);
					goto case 0;
				case 0:
				{
					if (version < 1)
					{
						var t = new ItemTypeSelectProperty(reader);
						CashType = t.InternalType;
					}
					else
					{
						CashType = new TypeSelectProperty<object>(reader);
					}

					_CashName = reader.ReadTextDef();
					_ShowCashName = reader.ReadBool();

					_Trading = reader.ReadBool();

					Discount = reader.ReadInt();
					DiscountEnabled = reader.ReadBool();
					DiscountYell = reader.ReadBool();
				}
				break;
			}

			if (CashProperty == null)
			{
				CashProperty = new ObjectProperty();
			}

			if (version < 3)
			{
				CanRestock = true;
			}

			if (version < 2)
			{
				_CashAbbr = String.Join(String.Empty, _CashName.GetString().Select(Char.IsUpper));
			}

			_DynamicStock.RemoveRange(o => o.Key.Deleted || o.Value == null || o.Value.Item != o.Key);
		}
	}
}
