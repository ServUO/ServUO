using System;
using System.Collections.Generic;
using Server.Items;
using System.Linq;

namespace Server.Mobiles
{
    public class GenericBuyInfo : IBuyItemInfo
    {
        public static Dictionary<Type, int> BuyPrices = new Dictionary<Type, int>();
        private Type m_Type;
        private string m_Name;
        private int m_Price;
        private int m_MaxAmount, m_Amount;
        private int m_ItemID;
        private int m_Hue;
        private object[] m_Args;
        private IEntity m_DisplayEntity;
        private int m_PriceScalar;
        private bool m_Stackable;
        private int m_TotalBought;
        private int m_TotalSold;

        public GenericBuyInfo(Type type, int price, int amount, int itemID, int hue, bool stacks = false)
            : this(null, type, price, amount, itemID, hue, null, stacks)
        {
        }

        public GenericBuyInfo(string name, Type type, int price, int amount, int itemID, int hue, bool stacks = false)
            : this(name, type, price, amount, itemID, hue, null, stacks)
        {
        }

        public GenericBuyInfo(Type type, int price, int amount, int itemID, int hue, object[] args, bool stacks = false)
            : this(null, type, price, amount, itemID, hue, args, stacks)
        {
        }

        public GenericBuyInfo(string name, Type type, int price, int amount, int itemID, int hue, object[] args, bool stacks = false)
        {
            if(type != null)
                BuyPrices[type] = price;

            m_Type = type;
            m_Price = price;
            m_ItemID = itemID;
            m_Hue = hue;
            m_Args = args;
            m_Stackable = stacks;

            if (type != null && EconomyItem)
            {
                m_MaxAmount = m_Amount = BaseVendor.EconomyStockAmount;
            }
            else
            {
                m_MaxAmount = m_Amount = amount;
            }

            if(Siege.SiegeShard)
            {
                m_Price *= 3;
            }

            if (name == null)
                m_Name = itemID < 0x4000 ? (1020000 + itemID).ToString() : (1078872 + itemID).ToString();
            else
                m_Name = name;
        }

        public virtual int ControlSlots
        {
            get
            {
                return 0;
            }
        }
        public virtual bool CanCacheDisplay
        {
            get
            {
                return false;
            }
        }//return ( m_Args == null || m_Args.Length == 0 ); } 
        public Type Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
        public int DefaultPrice
        {
            get
            {
                return m_PriceScalar;
            }
        }
        public int PriceScalar
        {
            get
            {
                return m_PriceScalar;
            }
            set
            {
                m_PriceScalar = value;
            }
        }

        public int TotalBought
        {
            get { return m_TotalBought; }
            set { m_TotalBought = value; }
        }

        public int TotalSold
        {
            get { return m_TotalSold; }
            set { m_TotalSold = value; }
        }

        public bool Stackable
        {
            get { return m_Stackable; }
            set { m_Stackable = value; }
        }

        public bool EconomyItem { get { return BaseVendor.UseVendorEconomy && m_Stackable; } }

        public int Price
        {
            get
            {
                int ecoInc = 0;

                if (EconomyItem)
                {
                    if (m_TotalBought >= BaseVendor.BuyItemChange)
                    {
                        ecoInc += m_TotalBought / BaseVendor.BuyItemChange;
                    }

                    if (m_TotalSold >= BaseVendor.SellItemChange)
                    {
                        ecoInc -= m_TotalSold / BaseVendor.SellItemChange;
                    }
                }

                if (m_PriceScalar != 0)
                {
                    if (m_Price > 5000000)
                    {
                        long price = m_Price;

                        price *= m_PriceScalar;
                        price += 50;
                        price /= 100;

                        if (price > int.MaxValue)
                            price = int.MaxValue;

                        if (EconomyItem && (int)price + ecoInc < 2)
                        {
                            return 2;
                        }

                        return (int)price + ecoInc;
                    }

                    if (EconomyItem && (((m_Price * m_PriceScalar) + 50) / 100) + ecoInc < 2)
                    {
                        return 2;
                    }

                    return (((m_Price * m_PriceScalar) + 50) / 100) + ecoInc;
                }

                if (EconomyItem && m_Price + ecoInc < 2)
                {
                    return 2;
                }

                return m_Price + ecoInc;
            }
            set
            {
                m_Price = value;
            }
        }
        public int ItemID
        {
            get
            {
                return m_ItemID;
            }
            set
            {
                m_ItemID = value;
            }
        }
        public int Hue
        {
            get
            {
                return m_Hue;
            }
            set
            {
                m_Hue = value;
            }
        }
        public int Amount
        {
            get
            {
                return m_Amount;
            }
            set
            {
                // Amount is ALWAYS 500
                if (EconomyItem)
                {
                    m_Amount = BaseVendor.EconomyStockAmount;
                }
                else
                {
                    if (value < 0)
                        value = 0;

                    m_Amount = value;
                }
            }
        }
        public int MaxAmount
        {
            get
            {
                // Amount is ALWAYS 500
                if (EconomyItem)
                {
                    return BaseVendor.EconomyStockAmount;
                }

                return m_MaxAmount;
            }
            set
            {
                m_MaxAmount = value;
            }
        }
        public object[] Args
        {
            get
            {
                return m_Args;
            }
            set
            {
                m_Args = value;
            }
        }
        public void DeleteDisplayEntity()
        {
            if (m_DisplayEntity == null)
                return;

            m_DisplayEntity.Delete();
            m_DisplayEntity = null;
        }

        public IEntity GetDisplayEntity()
        {
            if (m_DisplayEntity != null && !IsDeleted(m_DisplayEntity))
                return m_DisplayEntity;

            bool canCache = CanCacheDisplay;

            if (canCache)
                m_DisplayEntity = DisplayCache.Cache.Lookup(m_Type);

            if (m_DisplayEntity == null || IsDeleted(m_DisplayEntity))
                m_DisplayEntity = GetEntity();

            DisplayCache.Cache.Store(m_Type, m_DisplayEntity, canCache);

            return m_DisplayEntity;
        }

        //get a new instance of an object (we just bought it)
        public virtual IEntity GetEntity()
        {
            if (m_Args == null || m_Args.Length == 0)
                return (IEntity)Activator.CreateInstance(m_Type);

            return (IEntity)Activator.CreateInstance(m_Type, m_Args);
            //return (Item)Activator.CreateInstance( m_Type );
        }

        //Attempt to restock with item, (return true if restock sucessful)
        public bool Restock(Item item, int amount)
        {
            if (item == null || item.GetType() != m_Type)
            {
                return false;
            }

            return EconomyItem;
        }

        public void OnRestock()
        {
            if (m_Amount <= 0)
            {
                m_MaxAmount *= 2;

                if (m_MaxAmount >= 999)
                    m_MaxAmount = 999;
            }
            else
            {
                /* NOTE: According to UO.com, the quantity is halved if the item does not reach 0
                * Here we implement differently: the quantity is halved only if less than half
                * of the maximum quantity was bought. That is, if more than half is sold, then
                * there's clearly a demand and we should not cut down on the stock.
                */
                int halfQuantity = m_MaxAmount;

                if (halfQuantity >= 999)
                    halfQuantity = 640;
                else if (halfQuantity > 20)
                    halfQuantity /= 2;

                if (m_Amount >= halfQuantity)
                    m_MaxAmount = halfQuantity;
            }

            m_Amount = m_MaxAmount;
        }

        private bool IsDeleted(IEntity obj)
        {
            if (obj is Item)
                return ((Item)obj).Deleted;
            else if (obj is Mobile)
                return ((Mobile)obj).Deleted;

            return false;
        }

        public void OnBought(BaseVendor vendor, int amount)
        {
            if (EconomyItem)
            {
                foreach (var bii in vendor.GetBuyInfo().OfType<GenericBuyInfo>())
                {
                    if (bii.Type == m_Type || (m_Type == typeof(UncutCloth) && bii.Type == typeof(Cloth)) || (m_Type == typeof(Cloth) && bii.Type == typeof(UncutCloth)))
                    {
                        bii.TotalBought += amount;
                    }
                }
            }
        }

        public void OnSold(BaseVendor vendor, int amount)
        {
            if (EconomyItem)
            {
                foreach (var bii in vendor.GetBuyInfo().OfType<GenericBuyInfo>())
                {
                    if (bii.Type == m_Type || (m_Type == typeof(UncutCloth) && bii.Type == typeof(Cloth)) || (m_Type == typeof(Cloth) && bii.Type == typeof(UncutCloth)))
                    {
                        bii.TotalSold += amount;
                    }
                }
            }
        }

        public static bool IsDisplayCache(IEntity e)
        {
            if (e is Mobile)
            {
                return DisplayCache.Cache.Mobiles != null && DisplayCache.Cache.Mobiles.Contains((Mobile)e);
            }

            return DisplayCache.Cache.Table != null && DisplayCache.Cache.Table.ContainsValue(e);
        }

        private class DisplayCache : Container
        {
            private static DisplayCache m_Cache;
            private Dictionary<Type, IEntity> m_Table;
            private List<Mobile> m_Mobiles;

            public List<Mobile> Mobiles { get { return m_Mobiles; } }
            public Dictionary<Type, IEntity> Table { get { return m_Table; } }

            public DisplayCache()
                : base(0)
            {
                m_Table = new Dictionary<Type, IEntity>();
                m_Mobiles = new List<Mobile>();
            }

            public DisplayCache(Serial serial)
                : base(serial)
            {
            }

            public static DisplayCache Cache
            {
                get
                {
                    if (m_Cache == null || m_Cache.Deleted)
                        m_Cache = new DisplayCache();

                    return m_Cache;
                }
            }
            public IEntity Lookup(Type key)
            {
                IEntity e = null;
                m_Table.TryGetValue(key, out e);
                return e;
            }

            public void Store(Type key, IEntity obj, bool cache)
            {
                if (cache)
                    m_Table[key] = obj;

                if (obj is Item)
                    AddItem((Item)obj);
                else if (obj is Mobile)
                    m_Mobiles.Add((Mobile)obj);
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                for (int i = 0; i < m_Mobiles.Count; ++i)
                    m_Mobiles[i].Delete();

                m_Mobiles.Clear();

                for (int i = Items.Count - 1; i >= 0; --i)
                    if (i < Items.Count)
                        Items[i].Delete();

                if (m_Cache == this)
                    m_Cache = null;
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version

                writer.Write(m_Mobiles);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                m_Mobiles = reader.ReadStrongMobileList();

                for (int i = 0; i < m_Mobiles.Count; ++i)
                    m_Mobiles[i].Delete();

                m_Mobiles.Clear();

                for (int i = Items.Count - 1; i >= 0; --i)
                    if (i < Items.Count)
                        Items[i].Delete();

                if (m_Cache == null)
                    m_Cache = this;
                else
                    Delete();

                m_Table = new Dictionary<Type, IEntity>();
            }
        }
    }
}
