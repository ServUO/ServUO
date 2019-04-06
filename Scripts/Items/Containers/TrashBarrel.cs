using System;
using System.Collections.Generic;
using Server.Multis;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Engines.Points;
using System.Linq;

namespace Server.Items
{
    public class TrashBarrel : BaseTrash, IChopable
    {
        private Timer m_Timer;

        [Constructable]
        public TrashBarrel()
            : base(0xE77)
        {
            Hue = 0x3B2;
            Movable = false;
            m_Cleanup = new List<CleanupArray>();
        }

        public TrashBarrel(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041064;
            }
        }// a trash barrel
        public override int DefaultMaxWeight
        {
            get
            {
                return 0;
            }
        }// A value of 0 signals unlimited weight
        public override bool IsDecoContainer
        {
            get
            {
                return false;
            }
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

            if (Items.Count > 0)
            {
                m_Timer = new EmptyTimer(this);
                m_Timer.Start();
            }

            m_Cleanup = new List<CleanupArray>();
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!base.OnDragDrop(from, dropped))
                return false;

            AddCleanupItem(from, dropped);

            if (TotalItems >= 50)
            {
                Empty(501478); // The trash is full!  Emptying!
            }
            else
            {
                SendLocalizedMessageTo(from, 1010442); // The item will be deleted in three minutes

                if (m_Timer != null)
                    m_Timer.Stop();
                else
                    m_Timer = new EmptyTimer(this);

                m_Timer.Start();
            }

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            AddCleanupItem(from, item);

            if (TotalItems >= 50)
            {
                Empty(501478); // The trash is full!  Emptying!
            }
            else
            {
                SendLocalizedMessageTo(from, 1010442); // The item will be deleted in three minutes

                if (m_Timer != null)
                    m_Timer.Stop();
                else
                    m_Timer = new EmptyTimer(this);

                m_Timer.Start();
            }

            return true;
        }

        public void OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.IsCoOwner(from))
            {
                Effects.PlaySound(Location, Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.
                Destroy();
            }
        }

        public void Empty(int message)
        {
            List<Item> items = Items;

            if (items.Count > 0)
            {
                PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, message, "");

                for (int i = items.Count - 1; i >= 0; --i)
                {
                    if (i >= items.Count)
                        continue;

                    ConfirmCleanupItem(items[i]);

                    #region SA
                    if (.01 > Utility.RandomDouble())
                        DropToCavernOfDiscarded(items[i]);
                    else
                        items[i].Delete();
                    #endregion
                }

                if (m_Cleanup.Any(x => x.mobiles != null))
                {
                    foreach (var m in m_Cleanup.Select(x => x.mobiles).Distinct())
                    {
                        if (m_Cleanup.Find(x => x.mobiles == m && x.confirm) != null)
                        {
                            double point = m_Cleanup.Where(x => x.mobiles == m && x.confirm).Sum(x => x.points);
                            m.SendLocalizedMessage(1151280, String.Format("{0}\t{1}", point.ToString(), m_Cleanup.Count(r => r.mobiles == m))); // You have received approximately ~1_VALUE~points for turning in ~2_COUNT~items for Clean Up Britannia.
                            PointsSystem.CleanUpBritannia.AwardPoints(m, point);
                        }
                    }
                    m_Cleanup.Clear();
                }
            }

            if (m_Timer != null)
                m_Timer.Stop();            

            m_Timer = null;
        }

        private class EmptyTimer : Timer
        {
            private readonly TrashBarrel m_Barrel;
            public EmptyTimer(TrashBarrel barrel)
                : base(TimeSpan.FromMinutes(3.0))
            {
                m_Barrel = barrel;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                m_Barrel.Empty(501479); // Emptying the trashcan!
            }
        }

        #region SA
        public static void DropToCavernOfDiscarded(Item item)
        {
            if (item == null || item.Deleted)
                return;

            Rectangle2D rec = new Rectangle2D(901, 482, 40, 27);
            Map map = Map.TerMur;

            for (int i = 0; i < 50; i++)
            {
                int x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
                int y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
                int z = map.GetAverageZ(x, y);

                Point3D p = new Point3D(x, y, z);

                if (map.CanSpawnMobile(p))
                {
                    item.MoveToWorld(p, map);
                    return;
                }
            }

            item.Delete();
        }
        #endregion
    }
}