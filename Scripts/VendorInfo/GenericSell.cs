using System;
using System.Collections.Generic;
using System.Linq;

using Server.Items;

namespace Server.Mobiles
{
    public class GenericSellInfo : IShopSellInfo
    {
        private readonly Dictionary<Type, int> m_Table = new Dictionary<Type, int>();

        private Type[] m_Types;

        public Type[] Types
        {
            get
            {
                if (m_Types == null)
                {
                    m_Types = new Type[m_Table.Keys.Count];
                    m_Table.Keys.CopyTo(m_Types, 0);
                }

                return m_Types;
            }
		}

		public virtual void Add(Type type, int price)
        {
            m_Table[type] = price;
            m_Types = null;
        }

		public virtual int GetSellPriceFor(IEntity item)
        {
            return GetSellPriceFor(item, null);
        }

		public virtual int GetSellPriceFor(IEntity e, IVendor vendor)
        {
			Type itemType = e.GetType();

            m_Table.TryGetValue(itemType, out int price);

            if (vendor != null && BaseVendor.UseVendorEconomy)
			{
				IBuyItemInfo[] buyList = vendor.GetBuyInfo();
				IBuyItemInfo buyInfo = buyList.FirstOrDefault(bii => bii.EconomyItem && bii.Type == itemType);

                if (buyInfo != null)
                {
                    int sold = buyInfo.TotalSold;

                    price = (int)(buyInfo.Price * .75);

                    return Math.Max(1, price);
                }
            }

            if (e is BaseArmor armor)
            {
                if (armor.Quality == ItemQuality.Low)
                    price = (int)(price * 0.60);
                else if (armor.Quality == ItemQuality.Exceptional)
                    price = (int)(price * 1.25);

                price += 5 * armor.ArmorAttributes.DurabilityBonus;

                if (price < 1)
                    price = 1;
            }
            else if (e is BaseWeapon weapon)
            {
                if (weapon.Quality == ItemQuality.Low)
                    price = (int)(price * 0.60);
                else if (weapon.Quality == ItemQuality.Exceptional)
                    price = (int)(price * 1.25);

                price += 100 * weapon.WeaponAttributes.DurabilityBonus;

                price += 10 * weapon.Attributes.WeaponDamage;

                if (price < 1)
                    price = 1;
            }
            else if (e is BaseBeverage bev)
            {
                int price1 = price, price2 = price;

                if (bev is Pitcher)
                {
                    price1 = 3;
                    price2 = 5;
                }
                else if (bev is BeverageBottle)
                {
                    price1 = 3;
                    price2 = 3;
                }
                else if (bev is Jug)
                {
                    price1 = 6;
                    price2 = 6;
                }

                if (bev.IsEmpty || bev.Content == BeverageType.Milk)
                    price = price1;
                else
                    price = price2;
            }

            return price;
        }

		public virtual int GetBuyPriceFor(IEntity item)
        {
            return GetBuyPriceFor(item, null);
        }

		public virtual int GetBuyPriceFor(IEntity item, IVendor vendor)
        {
            return (int)(1.90 * GetSellPriceFor(item, vendor));
        }

		public virtual string GetNameFor(IEntity e)
        {
            if (e.Name != null)
                return e.Name;

			if (e is Item item)
				return item.LabelNumber.ToString();

			return e.GetType().Name;
        }

		public virtual bool IsSellable(IEntity e)
        {
            if (e is Item item && item.QuestItem)
                return false;

            //if (e.Hue != 0)
            //    return false;

            return IsInList(e.GetType());
        }

		public virtual bool IsResellable(IEntity e)
        {
            if (e is Item item && item.QuestItem)
                return false;

            //if (e.Hue != 0)
            //    return false;

            return IsInList(e.GetType());
        }

		public virtual bool IsInList(Type type)
        {
            return m_Table.ContainsKey(type);
		}

		public virtual void OnSold(Mobile seller, IVendor vendor, IEntity item, int amount)
		{
			Type itemType = item.GetType();

			IBuyItemInfo[] buyList = vendor.GetBuyInfo();
			IBuyItemInfo buyInfo = buyList.FirstOrDefault(bii => bii.EconomyItem && bii.Type == itemType);

			if (buyInfo != null)
			{
				foreach (IBuyItemInfo bii in buyList)
				{
					if (bii.Type == itemType || (itemType == typeof(UncutCloth) && bii.Type == typeof(Cloth)) || (itemType == typeof(Cloth) && bii.Type == typeof(UncutCloth)))
						bii.TotalSold += amount;
				}
			}

			EventSink.InvokeValidVendorSell(new ValidVendorSellEventArgs(seller, vendor, item, GetSellPriceFor(item, vendor)));
		}
	}
}
