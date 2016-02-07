using System;
using Server.Multis;
using Server.Regions;

namespace Server.Items
{
    public class NoHousingItem : Item
    {
        private NoHousingDelayTimer m_Timer;
        private Rectangle3D[] m_Area;
        private SimpleNoHousingRegion m_Region;
        [Constructable]
        public NoHousingItem(BaseHouse house)
            : base(0x2FD5)
        {
            this.m_Timer = new NoHousingDelayTimer(this);
            this.m_Timer.Start();

            this.m_Area = house.Region.Area;
            this.m_Region = new SimpleNoHousingRegion(house.Region.Map, this.m_Area);
            this.m_Region.Register();

            this.Visible = false;
            this.Movable = false;
        }

        public NoHousingItem(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (this.m_Region != null)
                this.m_Region.Unregister();

            if (this.m_Timer != null && this.m_Timer.Running)
                this.m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version

            if (this.m_Timer != null)
                writer.Write(this.m_Timer.Next);
            else
                writer.Write(DateTime.UtcNow);

            writer.Write(this.m_Area.Length);

            foreach (Rectangle3D rect in this.m_Area)
                writer.Write(rect);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();

            DateTime next = reader.ReadDateTime();
            this.m_Area = new Rectangle3D[reader.ReadInt()];

            for (int i = 0; i < this.m_Area.Length; i++)
                this.m_Area[i] = reader.ReadRect3D();

            this.m_Region = new SimpleNoHousingRegion(this.Map, this.m_Area);
            this.m_Region.Register();

            if (next < DateTime.UtcNow)
            {
                this.m_Timer = new NoHousingDelayTimer(this, next - DateTime.UtcNow);
                this.m_Timer.Start();
            }
            else
                this.Delete();		
        }

        private class SimpleNoHousingRegion : BaseRegion
        {
            public SimpleNoHousingRegion(Map map, Rectangle3D[] area)
                : base(null, map, Region.DefaultPriority, area)
            {
            }

            public override bool AllowHousing(Mobile from, Point3D p)
            {
                return false;
            }
        }

        private class NoHousingDelayTimer : Timer
        {
            private readonly NoHousingItem m_Item;
            public NoHousingDelayTimer(NoHousingItem item)
                : this(item, DefaultDelay)
            {
            }

            public NoHousingDelayTimer(NoHousingItem item, TimeSpan delay)
                : base(delay)
            {
                this.m_Item = item;
                this.Priority = TimerPriority.OneMinute;
            }

            public static TimeSpan DefaultDelay
            {
                get
                {
                    return TimeSpan.FromMinutes(Utility.RandomMinMax(60, 120));
                }
            }
            protected override void OnTick()
            {
                if (this.m_Item != null && !this.m_Item.Deleted)
                    this.m_Item.Delete();
            }
        }
    }
}