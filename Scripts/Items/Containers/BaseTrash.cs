using Server.Engines.Points;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class BaseTrash : Container
    {
        internal List<CleanupArray> m_Cleanup;

        public class CleanupArray
        {
            public Mobile mobiles { get; set; }
            public Item items { get; set; }
            public double points { get; set; }
            public bool confirm { get; set; }
            public Serial serials { get; set; }
        }

        public BaseTrash(int itemID)
            : base(itemID)
        {
        }

        public BaseTrash(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public void AddCleanupItem(Mobile from, Item item)
        {
            double checkbagpoint;

            if (item is BaseContainer)
            {
                Container c = (Container)item;

                List<Item> list = c.FindItemsByType<Item>();

                for (int i = list.Count - 1; i >= 0; --i)
                {
                    checkbagpoint = CleanUpBritanniaData.GetPoints(list[i]);

                    if (checkbagpoint > 0 && m_Cleanup.Find(x => x.serials == list[i].Serial) == null)
                        m_Cleanup.Add(new CleanupArray { mobiles = from, items = list[i], points = checkbagpoint, serials = list[i].Serial });
                }
            }
            else
            {
                checkbagpoint = CleanUpBritanniaData.GetPoints(item);

                if (checkbagpoint > 0 && m_Cleanup.Find(x => x.serials == item.Serial) == null)
                    m_Cleanup.Add(new CleanupArray { mobiles = from, items = item, points = checkbagpoint, serials = item.Serial });
            }
        }

        public void ConfirmCleanupItem(Item item)
        {
            if (item is BaseContainer)
            {
                Container c = (Container)item;

                List<Item> list = c.FindItemsByType<Item>();

                m_Cleanup.Where(r => list.Select(k => k.Serial).Contains(r.items.Serial)).ToList().ForEach(k => k.confirm = true);
            }
            else
            {
                m_Cleanup.Where(r => r.items.Serial == item.Serial).ToList().ForEach(k => k.confirm = true);
            }
        }
    }
}
