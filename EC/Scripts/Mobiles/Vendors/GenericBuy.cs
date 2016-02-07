using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class GenericBuyInfo : IBuyItemInfo
    {
        private Type m_Type;
        private string m_Name;
        private int m_Price;
        private int m_MaxAmount, m_Amount;
        private int m_ItemID;
        private int m_Hue;
        private object[] m_Args;
        private IEntity m_DisplayEntity;
        private int m_PriceScalar;
        public GenericBuyInfo(Type type, int price, int amount, int itemID, int hue)
            : this(null, type, price, amount, itemID, hue, null)
        {
        }

        public GenericBuyInfo(string name, Type type, int price, int amount, int itemID, int hue)
            : this(name, type, price, amount, itemID, hue, null)
        {
        }

        public GenericBuyInfo(Type type, int price, int amount, int itemID, int hue, object[] args)
            : this(null, type, price, amount, itemID, hue, args)
        {
        }

        public GenericBuyInfo(string name, Type type, int price, int amount, int itemID, int hue, object[] args)
        {
            this.m_Type = type;
            this.m_Price = price;
            this.m_MaxAmount = this.m_Amount = amount;
            this.m_ItemID = itemID;
            this.m_Hue = hue;
            this.m_Args = args;

            if (name == null)
                this.m_Name = itemID < 0x4000 ? (1020000 + itemID).ToString() : (1078872 + itemID).ToString();
            else
                this.m_Name = name;
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
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        public int DefaultPrice
        {
            get
            {
                return this.m_PriceScalar;
            }
        }
        public int PriceScalar
        {
            get
            {
                return this.m_PriceScalar;
            }
            set
            {
                this.m_PriceScalar = value;
            }
        }
        public int Price
        {
            get
            {
                if (this.m_PriceScalar != 0)
                {
                    if (this.m_Price > 5000000)
                    {
                        long price = this.m_Price;

                        price *= this.m_PriceScalar;
                        price += 50;
                        price /= 100;

                        if (price > int.MaxValue)
                            price = int.MaxValue;

                        return (int)price;
                    }

                    return (((this.m_Price * this.m_PriceScalar) + 50) / 100);
                }

                return this.m_Price;
            }
            set
            {
                this.m_Price = value;
            }
        }
        public int ItemID
        {
            get
            {
                return this.m_ItemID;
            }
            set
            {
                this.m_ItemID = value;
            }
        }
        public int Hue
        {
            get
            {
                return this.m_Hue;
            }
            set
            {
                this.m_Hue = value;
            }
        }
        public int Amount
        {
            get
            {
                return this.m_Amount;
            }
            set
            {
                if (value < 0)
                    value = 0;
                this.m_Amount = value;
            }
        }
        public int MaxAmount
        {
            get
            {
                return this.m_MaxAmount;
            }
            set
            {
                this.m_MaxAmount = value;
            }
        }
        public object[] Args
        {
            get
            {
                return this.m_Args;
            }
            set
            {
                this.m_Args = value;
            }
        }
        public void DeleteDisplayEntity()
        {
            if (this.m_DisplayEntity == null)
                return;

            this.m_DisplayEntity.Delete();
            this.m_DisplayEntity = null;
        }

        public IEntity GetDisplayEntity()
        {
            if (this.m_DisplayEntity != null && !this.IsDeleted(this.m_DisplayEntity))
                return this.m_DisplayEntity;

            bool canCache = this.CanCacheDisplay;

            if (canCache)
                this.m_DisplayEntity = DisplayCache.Cache.Lookup(this.m_Type);

            if (this.m_DisplayEntity == null || this.IsDeleted(this.m_DisplayEntity))
                this.m_DisplayEntity = this.GetEntity();

            DisplayCache.Cache.Store(this.m_Type, this.m_DisplayEntity, canCache);

            return this.m_DisplayEntity;
        }

        //get a new instance of an object (we just bought it)
        public virtual IEntity GetEntity()
        {
            if (this.m_Args == null || this.m_Args.Length == 0)
                return (IEntity)Activator.CreateInstance(this.m_Type);

            return (IEntity)Activator.CreateInstance(this.m_Type, this.m_Args);
            //return (Item)Activator.CreateInstance( m_Type );
        }

        //Attempt to restock with item, (return true if restock sucessful)
        public bool Restock(Item item, int amount)
        {
            return false;
            /*if ( item.GetType() == m_Type )
            {
            if ( item is BaseWeapon )
            {
            BaseWeapon weapon = (BaseWeapon)item;
            if ( weapon.Quality == WeaponQuality.Low || weapon.Quality == WeaponQuality.Exceptional || (int)weapon.DurabilityLevel > 0 || (int)weapon.DamageLevel > 0 || (int)weapon.AccuracyLevel > 0 )
            return false;
            }
            if ( item is BaseArmor )
            {
            BaseArmor armor = (BaseArmor)item;
            if ( armor.Quality == ArmorQuality.Low || armor.Quality == ArmorQuality.Exceptional || (int)armor.Durability > 0 || (int)armor.ProtectionLevel > 0 )
            return false;
            }
            m_Amount += amount;
            return true;
            }
            else
            {
            return false;
            }*/
        }

        public void OnRestock()
        {
            if (this.m_Amount <= 0)
            {
                this.m_MaxAmount *= 2;

                if (this.m_MaxAmount >= 999)
                    this.m_MaxAmount = 999;
            }
            else
            {
                /* NOTE: According to UO.com, the quantity is halved if the item does not reach 0
                * Here we implement differently: the quantity is halved only if less than half
                * of the maximum quantity was bought. That is, if more than half is sold, then
                * there's clearly a demand and we should not cut down on the stock.
                */
                int halfQuantity = this.m_MaxAmount;

                if (halfQuantity >= 999)
                    halfQuantity = 640;
                else if (halfQuantity > 20)
                    halfQuantity /= 2;

                if (this.m_Amount >= halfQuantity)
                    this.m_MaxAmount = halfQuantity;
            }

            this.m_Amount = this.m_MaxAmount;
        }

        private bool IsDeleted(IEntity obj)
        {
            if (obj is Item)
                return ((Item)obj).Deleted;
            else if (obj is Mobile)
                return ((Mobile)obj).Deleted;

            return false;
        }

        private class DisplayCache : Container
        {
            private static DisplayCache m_Cache;
            private Dictionary<Type, IEntity> m_Table;
            private List<Mobile> m_Mobiles;
            public DisplayCache()
                : base(0)
            {
                this.m_Table = new Dictionary<Type, IEntity>();
                this.m_Mobiles = new List<Mobile>();
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
                this.m_Table.TryGetValue(key, out e);
                return e;
            }

            public void Store(Type key, IEntity obj, bool cache)
            {
                if (cache)
                    this.m_Table[key] = obj;

                if (obj is Item)
                    this.AddItem((Item)obj);
                else if (obj is Mobile)
                    this.m_Mobiles.Add((Mobile)obj);
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                for (int i = 0; i < this.m_Mobiles.Count; ++i)
                    this.m_Mobiles[i].Delete();

                this.m_Mobiles.Clear();

                for (int i = this.Items.Count - 1; i >= 0; --i)
                    if (i < this.Items.Count)
                        this.Items[i].Delete();

                if (m_Cache == this)
                    m_Cache = null;
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version

                writer.Write(this.m_Mobiles);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                this.m_Mobiles = reader.ReadStrongMobileList();

                for (int i = 0; i < this.m_Mobiles.Count; ++i)
                    this.m_Mobiles[i].Delete();

                this.m_Mobiles.Clear();

                for (int i = this.Items.Count - 1; i >= 0; --i)
                    if (i < this.Items.Count)
                        this.Items[i].Delete();

                if (m_Cache == null)
                    m_Cache = this;
                else
                    this.Delete();

                this.m_Table = new Dictionary<Type, IEntity>();
            }
        }
    }
}