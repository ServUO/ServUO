using Server.Engines.Points;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    [Flipable(0xE41, 0xE40)]
    public class TrashChest : BaseTrash
    {
        [Constructable]
        public TrashChest()
            : base(0xE41)
        {
            Movable = false;
            m_Cleanup = new List<CleanupArray>();
        }

        public TrashChest(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultMaxWeight => 0;// A value of 0 signals unlimited weight
        public override bool IsDecoContainer => false;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Cleanup = new List<CleanupArray>();
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!base.OnDragDrop(from, dropped))
                return false;

            if (CleanUpBritanniaData.Enabled && !AddCleanupItem(from, dropped))
            {
                if (dropped.LootType == LootType.Blessed)
                {
                    from.SendLocalizedMessage(1075256); // That is blessed; you cannot throw it away.
                    return false;
                }
            }

            PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, Utility.Random(1042891, 8));
            Empty();

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            if (CleanUpBritanniaData.Enabled && !AddCleanupItem(from, item))
            {
                if (item.LootType == LootType.Blessed)
                {
                    from.SendLocalizedMessage(1075256); // That is blessed; you cannot throw it away.
                    return false;
                }
            }

            PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, Utility.Random(1042891, 8));
            Empty();

            return true;
        }

        public void Empty()
        {
            List<Item> items = Items;

            if (items.Count > 0)
            {
                for (int i = items.Count - 1; i >= 0; --i)
                {
                    if (i >= items.Count)
                        continue;

                    ConfirmCleanupItem(items[i]);

                    if (.01 > Utility.RandomDouble())
                        TrashBarrel.DropToCavernOfDiscarded(items[i]);
                    else
                        items[i].Delete();
                }

                if (m_Cleanup.Any(x => x.mobiles != null))
                {
                    foreach (Mobile m in m_Cleanup.Select(x => x.mobiles).Distinct())
                    {
                        if (m_Cleanup.Find(x => x.mobiles == m && x.confirm) != null)
                        {
                            double point = m_Cleanup.Where(x => x.mobiles == m && x.confirm).Sum(x => x.points);
                            m.SendLocalizedMessage(1151280, string.Format("{0}\t{1}", point.ToString(), m_Cleanup.Count(r => r.mobiles == m))); // You have received approximately ~1_VALUE~points for turning in ~2_COUNT~items for Clean Up Britannia.
                            PointsSystem.CleanUpBritannia.AwardPoints(m, point);
                        }
                    }
                    m_Cleanup.Clear();
                }
            }
        }
    }
}
