#region References
using System;
using System.Collections.Generic;

using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class BuyItemStateComparer : IComparer<BuyItemState>
	{
		public static BuyItemStateComparer Instance { get; private set; }

		static BuyItemStateComparer()
		{
			Instance = new BuyItemStateComparer();
		}

		public int Compare(BuyItemState l, BuyItemState r)
		{
			if (l == null && r == null)
			{
				return 0;
			}

			if (l == null)
			{
				return -1;
			}

			if (r == null)
			{
				return 1;
			}

			return l.ID.CompareTo(r.ID);
		}
	}

	public class BuyItemResponse
	{
		public Serial Serial { get; }
		public int Amount { get; }

		public BuyItemResponse(Serial serial, int amount)
		{
			Serial = serial;
			Amount = amount;
		}
	}

	public class SellItemResponse
	{
		public Item Item { get; }
		public int Amount { get; }

		public SellItemResponse(Item i, int amount)
		{
			Item = i;
			Amount = amount;
		}
	}

	public class SellItemState
	{
		public Item Item { get; }
		public int Price { get; }
		public string Name { get; }

		public SellItemState(Item item, int price, string name)
		{
			Item = item;
			Price = price;
			Name = name;
		}
	}

	public class BuyItemState
	{
		private static readonly Serial m_DefaultSerial = new Serial(0x7FC0FFEE);

		private static long m_LastID = -1;

		public long ID { get; }

		public Serial MySerial { get; }
		public Serial ContainerSerial { get; }

		public int ItemID { get; }
		public int Amount { get; }
		public int Hue { get; }

		public string Description { get; }

		public int Price { get; }

		public BuyItemState(Container cont, IBuyItemInfo info)
		{
			ID = ++m_LastID;

			Description = info.Name;
			ContainerSerial = cont?.Serial ?? Serial.MinusOne;
			MySerial = info.GetDisplayEntity()?.Serial ?? m_DefaultSerial;
			Price = info.Price;
			Amount = info.Amount;
			ItemID = info.ItemID;
			Hue = info.Hue;
		}

		public BuyItemState(string name, Serial cont, Serial serial, int price, int amount, int itemID, int hue)
		{
			ID = ++m_LastID;

			Description = name;
			ContainerSerial = cont;
			MySerial = serial;
			Price = price;
			Amount = amount;
			ItemID = itemID;
			Hue = hue;
		}
	}
}

namespace Server
{
	public interface IShopSellInfo
	{
		//get display name for an item
		string GetNameFor(IEntity item);

		//get price for an item which the player is selling
		int GetSellPriceFor(IEntity item);
		int GetSellPriceFor(IEntity item, IVendor vendor);

		//get price for an item which the player is buying
		int GetBuyPriceFor(IEntity item);
		int GetBuyPriceFor(IEntity item, IVendor vendor);

		//can we sell this item to this vendor?
		bool IsSellable(IEntity item);

		//What do we sell?
		Type[] Types { get; }

		//does the vendor resell this item?
		bool IsResellable(IEntity item);

		void OnSold(Mobile seller, IVendor vendor, IEntity entity, int amount);
	}

	public interface IBuyItemInfo
	{
		//get a new instance of an object (we just bought it)
		IEntity GetEntity();

		//get a cached instance of an object (vendor menu display)
		IEntity GetDisplayEntity();

		void DeleteDisplayEntity();

		bool EconomyItem { get; }

		int ControlSlots { get; set; }

		int PriceScalar { get; set; }

		bool Stackable { get; set; }

		int TotalBought { get; set; }
		int TotalSold { get; set; }

		//type of the item
		Type Type { get; set; }

		//display price of the item
		int Price { get; set; }

		//display name of the item
		string Name { get; set; }

		//display hue
		int Hue { get; set; }

		//display id
		int ItemID { get; set; }

		//amount in stock
		int Amount { get; set; }

		//max amount in stock
		int MaxAmount { get; set; }

		//Attempt to restock with item, (return true if restock sucessful)
		bool Restock(Item item, int amount);

		//called when its time for the whole shop to restock
		void OnRestock();

		void OnBought(Mobile buyer, IVendor vendor, IEntity entity, int amount);
	}
}