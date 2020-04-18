using Server.ContextMenus;
using Server.Engines.Points;
using Server.Mobiles;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class CleanupArray
    {
        public Mobile mobiles { get; set; }
        public Item items { get; set; }
        public double points { get; set; }
        public bool confirm { get; set; }
        public Serial serials { get; set; }
    }

    public class BaseTrash : Container
    {
        internal List<CleanupArray> m_Cleanup;

        public BaseTrash(int itemID)
            : base(itemID)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (CleanUpBritanniaData.Enabled && from is PlayerMobile)
            {
                list.Add(new AppraiseforCleanup(from));
            }
        }

        private class AppraiseforCleanup : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;
            public AppraiseforCleanup(Mobile mobile)
                : base(1151298, 2) //Appraise for Cleanup
            {
                m_Mobile = mobile;
            }

            public override void OnClick()
            {
                m_Mobile.Target = new AppraiseforCleanupTarget(m_Mobile);
                m_Mobile.SendLocalizedMessage(1151299); //Target items to see how many Clean Up Britannia points you will receive for throwing them away. Continue targeting items until done, then press the ESC key to cancel the targeting cursor.
            }
        }

        public BaseTrash(Serial serial) : base(serial)
        {
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
        }

        public virtual bool AddCleanupItem(Mobile from, Item item)
        {
            if (!CleanUpBritanniaData.Enabled)
            {
                return false;
            }

            double checkbagpoint;
            bool added = false;

            if (item is BaseContainer)
            {
                Container c = (Container)item;

                List<Item> list = c.FindItemsByType<Item>();

                for (int i = list.Count - 1; i >= 0; --i)
                {
                    checkbagpoint = CleanUpBritanniaData.GetPoints(list[i]);

                    if (checkbagpoint > 0 && m_Cleanup.Find(x => x.serials == list[i].Serial) == null)
                    {
                        m_Cleanup.Add(new CleanupArray { mobiles = from, items = list[i], points = checkbagpoint, serials = list[i].Serial });

                        if (!added)
                            added = true;
                    }
                }
            }
            else
            {
                checkbagpoint = CleanUpBritanniaData.GetPoints(item);

                if (checkbagpoint > 0 && m_Cleanup.Find(x => x.serials == item.Serial) == null)
                {
                    m_Cleanup.Add(new CleanupArray { mobiles = from, items = item, points = checkbagpoint, serials = item.Serial });
                    added = true;
                }
            }

            return added;
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
