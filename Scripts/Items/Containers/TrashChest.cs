using Server.ContextMenus;
using Server.Engines.Points;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Empty();

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            this.PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, Utility.Random(1042891, 8));
            Empty();

            return true;
        }

        public class CountArray
        {
            public Mobile m { get; set; }

            public double points { get; set; }
        }

        public void Empty()
        {
            List<Item> items = this.Items;

            List<Item> bags;

            if (items.Count > 0)
            {
                List<CountArray> _list = new List<CountArray>();
                
                for (int i = items.Count - 1; i >= 0; --i)
                {
                    if (i >= items.Count)
                        continue;

                    if (items[i] is BaseContainer)
                    {
                        bags = items[i].Items;

                        if (bags.Count > 0)
                        {
                            for (int b = bags.Count - 1; b >= 0; --b)
                            {
                                if (b >= bags.Count)
                                    continue;

                                double checkbagpoint = CleanUpBritanniaData.GetPoints(bags[b]);

                                if (checkbagpoint != 0)
                                    _list.Add(new CountArray { m = items[i].CleanupOwner, points = checkbagpoint });
                            }
                        }
                    }

                    double checkpoint = CleanUpBritanniaData.GetPoints(items[i]);

                    if (checkpoint != 0)
                        _list.Add(new CountArray { m = items[i].CleanupOwner, points = checkpoint });

                    items[i].Delete();
                }

                if (_list.Any(x => x.m != null))
                {
                    foreach (var item in _list.Select(x => x.m).Distinct())
                    {
                        double point = _list.Where(x => x.m == item).Sum(x => x.points);
                        item.SendLocalizedMessage(1151280, String.Format("{0}\t{1}", point.ToString(), _list.Count(r => r.m == item))); // You have received approximately ~1_VALUE~points for turning in ~2_COUNT~items for Clean Up Britannia.
                        PointsSystem.CleanUpBritannia.AwardPoints(item, point);
                    }
                }
            }
        }
    }
}