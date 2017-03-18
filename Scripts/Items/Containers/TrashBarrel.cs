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
            this.Hue = 0x3B2;
            this.Movable = false;
            this.m_Cleanup = new List<CleanupArray>();
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

            if (this.Items.Count > 0)
            {
                this.m_Timer = new EmptyTimer(this);
                this.m_Timer.Start();
            }

            this.m_Cleanup = new List<CleanupArray>();
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!base.OnDragDrop(from, dropped))
                return false;

            if (!AddCleanupItem(from, dropped))
            {
                if (dropped.LootType == LootType.Blessed)
                {
                    from.SendLocalizedMessage(1075256); // That is blessed; you cannot throw it away.
                    return false;
                }
            }

            if (this.TotalItems >= 50)
            {
                this.Empty(501478); // The trash is full!  Emptying!
            }
            else
            {
                this.SendLocalizedMessageTo(from, 1010442); // The item will be deleted in three minutes

                if (this.m_Timer != null)
                    this.m_Timer.Stop();
                else
                    this.m_Timer = new EmptyTimer(this);

                this.m_Timer.Start();
            }

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            if (!AddCleanupItem(from, item))
            {
                if (item.LootType == LootType.Blessed)
                {
                    from.SendLocalizedMessage(1075256); // That is blessed; you cannot throw it away.
                    return false;
                }
            }

            if (this.TotalItems >= 50)
            {
                this.Empty(501478); // The trash is full!  Emptying!
            }
            else
            {
                this.SendLocalizedMessageTo(from, 1010442); // The item will be deleted in three minutes

                if (this.m_Timer != null)
                    this.m_Timer.Stop();
                else
                    this.m_Timer = new EmptyTimer(this);

                this.m_Timer.Start();
            }

            return true;
        }

        public void OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.IsCoOwner(from))
            {
                Effects.PlaySound(this.Location, this.Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.
                this.Destroy();
            }
        }

        public void Empty(int message)
        {
            List<Item> items = this.Items;

            if (items.Count > 0)
            {
                this.PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, message, "");

                for (int i = items.Count - 1; i >= 0; --i)
                {
                    if (i >= items.Count)
                        continue;

                    ConfirmCleanupItem(items[i]);
                    items[i].Delete();
                }

                if (this.m_Cleanup.Any(x => x.mobiles != null))
                {
                    foreach (var m in this.m_Cleanup.Select(x => x.mobiles).Distinct())
                    {
                        if (this.m_Cleanup.Find(x => x.mobiles == m && x.confirm) != null)
                        {
                            double point = this.m_Cleanup.Where(x => x.mobiles == m && x.confirm).Sum(x => x.points);
                            m.SendLocalizedMessage(1151280, String.Format("{0}\t{1}", point.ToString(), this.m_Cleanup.Count(r => r.mobiles == m))); // You have received approximately ~1_VALUE~points for turning in ~2_COUNT~items for Clean Up Britannia.
                            PointsSystem.CleanUpBritannia.AwardPoints(m, point);
                        }
                    }
                    this.m_Cleanup.Clear();
                }
            }

            if (this.m_Timer != null)
                this.m_Timer.Stop();            

            this.m_Timer = null;
        }

        private class EmptyTimer : Timer
        {
            private readonly TrashBarrel m_Barrel;
            public EmptyTimer(TrashBarrel barrel)
                : base(TimeSpan.FromMinutes(3.0))
            {
                this.m_Barrel = barrel;
                this.Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                this.m_Barrel.Empty(501479); // Emptying the trashcan!
            }
        }
    }
}