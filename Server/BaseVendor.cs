#region Header
// **********
// ServUO - BaseVendor.cs
// **********
#endregion

#region References
using System.Collections.Generic;
#endregion

namespace Server.Mobiles
{
	public class BuyItemStateComparer : IComparer<BuyItemState>
	{
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

			return l.MySerial.CompareTo(r.MySerial);
		}
	}

	public class BuyItemResponse
	{
		private readonly Serial m_Serial;
		private readonly int m_Amount;

		public BuyItemResponse(Serial serial, int amount)
		{
			m_Serial = serial;
			m_Amount = amount;
		}

		public Serial Serial { get { return m_Serial; } }

		public int Amount { get { return m_Amount; } }
	}

	public class SellItemResponse
	{
		private readonly Item m_Item;
		private readonly int m_Amount;

		public SellItemResponse(Item i, int amount)
		{
			m_Item = i;
			m_Amount = amount;
		}

		public Item Item { get { return m_Item; } }

		public int Amount { get { return m_Amount; } }
	}

	public class SellItemState
	{
		private readonly Item m_Item;
		private readonly int m_Price;
		private readonly string m_Name;

		public SellItemState(Item item, int price, string name)
		{
			m_Item = item;
			m_Price = price;
			m_Name = name;
		}

		public Item Item { get { return m_Item; } }

		public int Price { get { return m_Price; } }

		public string Name { get { return m_Name; } }
	}

	public class BuyItemState
	{
		private readonly Serial m_ContSer;
		private readonly Serial m_MySer;
		private readonly int m_ItemID;
		private readonly int m_Amount;
		private readonly int m_Hue;
		private readonly int m_Price;
		private readonly string m_Desc;

		public BuyItemState(string name, Serial cont, Serial serial, int price, int amount, int itemID, int hue)
		{
			m_Desc = name;
			m_ContSer = cont;
			m_MySer = serial;
			m_Price = price;
			m_Amount = amount;
			m_ItemID = itemID;
			m_Hue = hue;
		}

		public int Price { get { return m_Price; } }

		public Serial MySerial { get { return m_MySer; } }

		public Serial ContainerSerial { get { return m_ContSer; } }

		public int ItemID { get { return m_ItemID; } }

		public int Amount { get { return m_Amount; } }

		public int Hue { get { return m_Hue; } }

		public string Description { get { return m_Desc; } }
	}
}