using System;
using System.Collections.Generic;
using Server.Multis;

namespace Server.Mobiles
{
    public class VendorInventory
    {
        public static readonly TimeSpan GracePeriod = TimeSpan.FromDays(7.0);
        private readonly List<Item> m_Items;
        private readonly DateTime m_ExpireTime;
        private readonly Timer m_ExpireTimer;
        private BaseHouse m_House;
        private string m_VendorName;
        private string m_ShopName;
        private Mobile m_Owner;
        private int m_Gold;
        public VendorInventory(BaseHouse house, Mobile owner, string vendorName, string shopName)
        {
            this.m_House = house;
            this.m_Owner = owner;
            this.m_VendorName = vendorName;
            this.m_ShopName = shopName;

            this.m_Items = new List<Item>();

            this.m_ExpireTime = DateTime.UtcNow + GracePeriod;
            this.m_ExpireTimer = new ExpireTimer(this, GracePeriod);
            this.m_ExpireTimer.Start();
        }

        public VendorInventory(BaseHouse house, GenericReader reader)
        {
            this.m_House = house;

            int version = reader.ReadEncodedInt();

            this.m_Owner = reader.ReadMobile();
            this.m_VendorName = reader.ReadString();
            this.m_ShopName = reader.ReadString();

            this.m_Items = reader.ReadStrongItemList();
            this.m_Gold = reader.ReadInt();

            this.m_ExpireTime = reader.ReadDeltaTime();

            if (this.m_Items.Count == 0 && this.m_Gold == 0)
            {
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
            }
            else
            {
                TimeSpan delay = this.m_ExpireTime - DateTime.UtcNow;
                this.m_ExpireTimer = new ExpireTimer(this, delay > TimeSpan.Zero ? delay : TimeSpan.Zero);
                this.m_ExpireTimer.Start();
            }
        }

        public BaseHouse House
        {
            get
            {
                return this.m_House;
            }
            set
            {
                this.m_House = value;
            }
        }
        public string VendorName
        {
            get
            {
                return this.m_VendorName;
            }
            set
            {
                this.m_VendorName = value;
            }
        }
        public string ShopName
        {
            get
            {
                return this.m_ShopName;
            }
            set
            {
                this.m_ShopName = value;
            }
        }
        public Mobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
        public List<Item> Items
        {
            get
            {
                return this.m_Items;
            }
        }
        public int Gold
        {
            get
            {
                return this.m_Gold;
            }
            set
            {
                this.m_Gold = value;
            }
        }
        public DateTime ExpireTime
        {
            get
            {
                return this.m_ExpireTime;
            }
        }
        public void AddItem(Item item)
        {
            item.Internalize();
            this.m_Items.Add(item);
        }

        public void Delete()
        {
            foreach (Item item in this.Items)
            {
                item.Delete();
            }

            this.Items.Clear();
            this.Gold = 0;

            if (this.House != null)
                this.House.VendorInventories.Remove(this);

            this.m_ExpireTimer.Stop();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write((Mobile)this.m_Owner);
            writer.Write((string)this.m_VendorName);
            writer.Write((string)this.m_ShopName);

            writer.Write(this.m_Items, true);
            writer.Write((int)this.m_Gold);

            writer.WriteDeltaTime(this.m_ExpireTime);
        }

        private class ExpireTimer : Timer
        {
            private readonly VendorInventory m_Inventory;
            public ExpireTimer(VendorInventory inventory, TimeSpan delay)
                : base(delay)
            {
                this.m_Inventory = inventory;

                this.Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                BaseHouse house = this.m_Inventory.House;

                if (house != null)
                {
                    if (this.m_Inventory.Gold > 0)
                    {
                        if (house.MovingCrate == null)
                            house.MovingCrate = new MovingCrate(house);

                        Banker.Deposit(house.MovingCrate, this.m_Inventory.Gold);
                    }

                    foreach (Item item in this.m_Inventory.Items)
                    {
                        if (!item.Deleted)
                            house.DropToMovingCrate(item);
                    }

                    this.m_Inventory.Gold = 0;
                    this.m_Inventory.Items.Clear();
                }

                this.m_Inventory.Delete();
            }
        }
    }
}