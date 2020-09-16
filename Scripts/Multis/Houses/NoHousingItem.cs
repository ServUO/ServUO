using Server.Multis;
using Server.Regions;
using System;

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
            m_Timer = new NoHousingDelayTimer(this);
            m_Timer.Start();

            m_Area = house.Region.Area;
            m_Region = new SimpleNoHousingRegion(house.Region.Map, m_Area);
            m_Region.Register();

            Visible = false;
            Movable = false;
        }

        public NoHousingItem(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (m_Region != null)
                m_Region.Unregister();

            if (m_Timer != null && m_Timer.Running)
                m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            if (m_Timer != null)
                writer.Write(m_Timer.Next);
            else
                writer.Write(DateTime.UtcNow);

            writer.Write(m_Area.Length);

            foreach (Rectangle3D rect in m_Area)
                writer.Write(rect);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            DateTime next = reader.ReadDateTime();
            m_Area = new Rectangle3D[reader.ReadInt()];

            for (int i = 0; i < m_Area.Length; i++)
                m_Area[i] = reader.ReadRect3D();

            m_Region = new SimpleNoHousingRegion(Map, m_Area);
            m_Region.Register();

            if (next < DateTime.UtcNow)
            {
                m_Timer = new NoHousingDelayTimer(this, next - DateTime.UtcNow);
                m_Timer.Start();
            }
            else
                Delete();
        }

        private class SimpleNoHousingRegion : BaseRegion
        {
            public SimpleNoHousingRegion(Map map, Rectangle3D[] area)
                : base(null, map, DefaultPriority, area)
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
                m_Item = item;
                Priority = TimerPriority.OneMinute;
            }

            public static TimeSpan DefaultDelay => TimeSpan.FromMinutes(Utility.RandomMinMax(60, 120));
            protected override void OnTick()
            {
                if (m_Item != null && !m_Item.Deleted)
                    m_Item.Delete();
            }
        }
    }
}