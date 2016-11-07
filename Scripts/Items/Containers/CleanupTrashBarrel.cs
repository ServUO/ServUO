using System;
using System.Collections.Generic;
using Server.Multis;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Engines.Points;
using System.Linq;

namespace Server.Items
{
    public class CleanupTrashBarrel : BaseTrash
    {
        private Timer m_Timer;

        [Constructable]
        public CleanupTrashBarrel()
            : base(0xFAE)
        {
            this.Hue = 2500;
            this.Movable = false;
            this.Name = "Tash - Keep Britannia Clean";
            this.m_Cleanup = new List<CleanupArray>();
        }

        public CleanupTrashBarrel(Serial serial)
            : base(serial)
        {
        }
        
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

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from is PlayerMobile)
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
                this.m_Mobile = mobile;
            }

            public override void OnClick()
            {
                m_Mobile.Target = new AppraiseforCleanupTarget(m_Mobile);
                m_Mobile.SendLocalizedMessage(1151299); //Target items to see how many Clean Up Britannia points you will receive for throwing them away. Continue targeting items until done, then press the ESC key to cancel the targeting cursor.
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!base.OnDragDrop(from, dropped))
                return false;

            if (dropped.LootType == LootType.Blessed)
            {
                this.Empty(1075256); // That is blessed; you cannot throw it away.
                return false;
            }                

            AddCleanupItem(from, dropped);

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

            if (item.LootType == LootType.Blessed)
            {
                this.Empty(1075256); // That is blessed; you cannot throw it away.
                return false;
            }

            AddCleanupItem(from, item);

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
            private readonly CleanupTrashBarrel m_Barrel;
            public EmptyTimer(CleanupTrashBarrel barrel)
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