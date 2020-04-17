using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class EffectItem : Item
    {
        public static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(5.0);
        private static readonly List<EffectItem> m_Free = new List<EffectItem>();// List of available EffectItems
        public EffectItem(Serial serial)
            : base(serial)
        {
        }

        private EffectItem()
            : base(1)// nodraw
        {
            Movable = false;
        }

        public override bool Decays => true;
        public static EffectItem Create(Point3D p, Map map, TimeSpan duration)
        {
            EffectItem item = null;

            for (int i = m_Free.Count - 1; item == null && i >= 0; --i) // We reuse new entries first so decay works better
            {
                EffectItem free = m_Free[i];

                m_Free.RemoveAt(i);

                if (!free.Deleted && free.Map == Map.Internal)
                    item = free;
            }

            if (item == null)
                item = new EffectItem();
            else
                item.ItemID = 1;

            item.MoveToWorld(p, map);
            item.BeginFree(duration);

            return item;
        }

        public void BeginFree(TimeSpan duration)
        {
            new FreeTimer(this, duration).Start();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Delete();
        }

        private class FreeTimer : Timer
        {
            private readonly EffectItem m_Item;
            public FreeTimer(EffectItem item, TimeSpan delay)
                : base(delay)
            {
                m_Item = item;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Item.Internalize();

                m_Free.Add(m_Item);
            }
        }
    }
}