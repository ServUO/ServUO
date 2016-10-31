using System;
using System.Collections.Generic;
using Server.Multis;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Engines.Points;

namespace Server.Items
{
    public class TrashBarrel : Container, IChopable
    {
        private Timer m_Timer;
        [Constructable]
        public TrashBarrel()
            : base(0xE77)
        {
            this.Hue = 0x3B2;
            this.Movable = false;
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

            dropped.CleanupOwner = from;

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

            item.CleanupOwner = from;

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

                    PointsSystem.CleanUpBritannia.AwardPoints(items[i].CleanupOwner, CleanUpBritanniaData.GetPoints(items[i]));
                    items[i].Delete();
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