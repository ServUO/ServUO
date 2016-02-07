using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class GenericSellInfo : IShopSellInfo
    {
        private readonly Dictionary<Type, int> m_Table = new Dictionary<Type, int>();
        private Type[] m_Types;
        public GenericSellInfo()
        {
        }

        public Type[] Types
        {
            get
            {
                if (this.m_Types == null)
                {
                    this.m_Types = new Type[this.m_Table.Keys.Count];
                    this.m_Table.Keys.CopyTo(this.m_Types, 0);
                }

                return this.m_Types;
            }
        }
        public void Add(Type type, int price)
        {
            this.m_Table[type] = price;
            this.m_Types = null;
        }

        public int GetSellPriceFor(Item item)
        {
            int price = 0;
            this.m_Table.TryGetValue(item.GetType(), out price);

            if (item is BaseArmor)
            {
                BaseArmor armor = (BaseArmor)item;

                if (armor.Quality == ArmorQuality.Low)
                    price = (int)(price * 0.60);
                else if (armor.Quality == ArmorQuality.Exceptional)
                    price = (int)(price * 1.25);

                price += 100 * (int)armor.Durability;

                price += 100 * (int)armor.ProtectionLevel;

                if (price < 1)
                    price = 1;
            }
            else if (item is BaseWeapon)
            {
                BaseWeapon weapon = (BaseWeapon)item;

                if (weapon.Quality == WeaponQuality.Low)
                    price = (int)(price * 0.60);
                else if (weapon.Quality == WeaponQuality.Exceptional)
                    price = (int)(price * 1.25);

                price += 100 * (int)weapon.DurabilityLevel;

                price += 100 * (int)weapon.DamageLevel;

                if (price < 1)
                    price = 1;
            }
            else if (item is BaseBeverage)
            {
                int price1 = price, price2 = price;

                if (item is Pitcher)
                {
                    price1 = 3;
                    price2 = 5;
                }
                else if (item is BeverageBottle)
                {
                    price1 = 3;
                    price2 = 3;
                }
                else if (item is Jug)
                {
                    price1 = 6;
                    price2 = 6;
                }

                BaseBeverage bev = (BaseBeverage)item;

                if (bev.IsEmpty || bev.Content == BeverageType.Milk)
                    price = price1;
                else
                    price = price2;
            }

            return price;
        }

        public int GetBuyPriceFor(Item item)
        {
            return (int)(1.90 * this.GetSellPriceFor(item));
        }

        public string GetNameFor(Item item)
        {
            if (item.Name != null)
                return item.Name;
            else
                return item.LabelNumber.ToString();
        }

        public bool IsSellable(Item item)
        {
            if (item.QuestItem)
                return false;

            //if ( item.Hue != 0 )
            //return false;

            return this.IsInList(item.GetType());
        }

        public bool IsResellable(Item item)
        {
            if (item.QuestItem)
                return false;

            //if ( item.Hue != 0 )
            //return false;

            return this.IsInList(item.GetType());
        }

        public bool IsInList(Type type)
        {
            return this.m_Table.ContainsKey(type);
        }
    }
}