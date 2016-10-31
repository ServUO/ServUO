using Server.ContextMenus;
using Server.Engines.Points;
using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    [FlipableAttribute(0xE41, 0xE40)]
    public class TrashChest : Container
    {
        [Constructable]
        public TrashChest()
            : base(0xE41)
        {
            this.Movable = false;
        }

        public TrashChest(Serial serial)
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

            this.PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, Utility.Random(1042891, 8));

            PointsSystem.CleanUpBritannia.AwardPoints(dropped.CleanupOwner, CleanUpBritanniaData.GetPoints(dropped));
            dropped.Delete();

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            this.PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, Utility.Random(1042891, 8));
            item.Delete();

            return true;
        }
    }
}