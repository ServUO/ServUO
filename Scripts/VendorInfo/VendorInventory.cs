using Server.Accounting;
using Server.Multis;
using System;
using System.Collections.Generic;

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
            m_House = house;
            m_Owner = owner;
            m_VendorName = vendorName;
            m_ShopName = shopName;

            m_Items = new List<Item>();

            m_ExpireTime = DateTime.UtcNow + GracePeriod;
            m_ExpireTimer = new ExpireTimer(this, GracePeriod);
            m_ExpireTimer.Start();
        }

        public VendorInventory(BaseHouse house, GenericReader reader)
        {
            m_House = house;

            int version = reader.ReadEncodedInt();

            m_Owner = reader.ReadMobile();
            m_VendorName = reader.ReadString();
            m_ShopName = reader.ReadString();

            m_Items = reader.ReadStrongItemList();
            m_Gold = reader.ReadInt();

            m_ExpireTime = reader.ReadDeltaTime();

            if (m_Items.Count == 0 && m_Gold == 0)
            {
                Timer.DelayCall(TimeSpan.Zero, Delete);
            }
            else
            {
                TimeSpan delay = m_ExpireTime - DateTime.UtcNow;
                m_ExpireTimer = new ExpireTimer(this, delay > TimeSpan.Zero ? delay : TimeSpan.Zero);
                m_ExpireTimer.Start();
            }
        }

        public BaseHouse House
        {
            get
            {
                return m_House;
            }
            set
            {
                m_House = value;
            }
        }
        public string VendorName
        {
            get
            {
                return m_VendorName;
            }
            set
            {
                m_VendorName = value;
            }
        }
        public string ShopName
        {
            get
            {
                return m_ShopName;
            }
            set
            {
                m_ShopName = value;
            }
        }
        public Mobile Owner
        {
            get
            {
                return m_Owner;
            }
            set
            {
                m_Owner = value;
            }
        }
        public List<Item> Items => m_Items;
        public int Gold
        {
            get
            {
                return m_Gold;
            }
            set
            {
                m_Gold = value;
            }
        }
        public DateTime ExpireTime => m_ExpireTime;
        public void AddItem(Item item)
        {
            item.Internalize();
            m_Items.Add(item);
        }

        public void Delete()
        {
            foreach (Item item in Items)
            {
                item.Delete();
            }

            Items.Clear();
            Gold = 0;

            if (House != null)
                House.VendorInventories.Remove(this);

            m_ExpireTimer.Stop();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write(m_Owner);
            writer.Write(m_VendorName);
            writer.Write(m_ShopName);

            writer.Write(m_Items, true);
            writer.Write(m_Gold);

            writer.WriteDeltaTime(m_ExpireTime);
        }

        private class ExpireTimer : Timer
        {
            private readonly VendorInventory m_Inventory;
            public ExpireTimer(VendorInventory inventory, TimeSpan delay)
                : base(delay)
            {
                m_Inventory = inventory;

                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                BaseHouse house = m_Inventory.House;

                if (house != null)
                {
                    if (m_Inventory.Gold > 0)
                    {
                        if (AccountGold.Enabled && m_Inventory.Owner != null)
                        {
                            Banker.Deposit(house.Owner, m_Inventory.Gold, true);
                        }
                        else
                        {
                            if (house.MovingCrate == null)
                                house.MovingCrate = new MovingCrate(house);

                            Banker.Deposit(house.MovingCrate, m_Inventory.Gold);
                        }
                    }

                    foreach (Item item in m_Inventory.Items)
                    {
                        if (!item.Deleted)
                            house.DropToMovingCrate(item);
                    }

                    m_Inventory.Gold = 0;
                    m_Inventory.Items.Clear();
                }

                m_Inventory.Delete();
            }
        }
    }
}
