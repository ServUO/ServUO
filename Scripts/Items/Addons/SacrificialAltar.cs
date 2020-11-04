using Server.ContextMenus;
using Server.Engines.Points;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    [FlipableAddon(Direction.South, Direction.East)]
    public class SacrificialAltarAddon : BaseAddonContainer
    {
        public override int LabelNumber => 1074818;  // Sacrificial Altar
        public override bool IsDecoContainer => false;
        public override bool Security => false;
        public override int DefaultGumpID => 0x9;
        public override int DefaultDropSound => 740;

        private Timer m_Timer;
        private List<CleanupArray> m_Cleanup;

        public override BaseAddonContainerDeed Deed => new SacrificialAltarDeed();

        [Constructable]
        public SacrificialAltarAddon()
            : base(0x2A9B)
        {
            Direction = Direction.South;
            AddComponent(new LocalizedContainerComponent(0x2A9A, 1074818), 1, 0, 0);
            m_Cleanup = new List<CleanupArray>();
        }

        public SacrificialAltarAddon(Serial serial)
            : base(serial)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!base.OnDragDrop(from, dropped))
                return false;

            if (TotalItems >= 50)
            {
                SendLocalizedMessageTo(from, 501478); // The trash is full!  Emptying!
                Empty();
            }
            else
            {
                SendLocalizedMessageTo(from, 1010442); // The item will be deleted in three minutes

                if (m_Timer != null)
                    m_Timer.Stop();

                m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), Empty);
            }

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            if (TotalItems >= 50)
            {
                SendLocalizedMessageTo(from, 501478); // The trash is full!  Emptying!
                Empty();
            }
            else
            {
                SendLocalizedMessageTo(from, 1010442); // The item will be deleted in three minutes

                if (m_Timer != null)
                    m_Timer.Stop();

                m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), Empty);
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadEncodedInt();

            if (Items.Count > 0)
                m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), Empty);

            m_Cleanup = new List<CleanupArray>();
        }

        public virtual void Flip(Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    ItemID = 0x2A9C;
                    AddComponent(new LocalizedContainerComponent(0x2A9D, 1074818), 0, -1, 0);
                    break;
                case Direction.South:
                    ItemID = 0x2A9B;
                    AddComponent(new LocalizedContainerComponent(0x2A9A, 1074818), 1, 0, 0);
                    break;
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (CleanUpBritanniaData.Enabled)
            {
                list.Add(new AppraiseforCleanup(from));
            }
        }

        private class AppraiseforCleanup : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;

            public AppraiseforCleanup(Mobile mobile)
                : base(1151298, 12) // Appraise for Cleanup
            {
                m_Mobile = mobile;
            }

            public override void OnClick()
            {
                m_Mobile.Target = new AppraiseforCleanupTarget(m_Mobile);
                m_Mobile.SendLocalizedMessage(1151299); // Target items to see how many Clean Up Britannia points you will receive for throwing them away. Continue targeting items until done, then press the ESC key to cancel the targeting cursor.
            }
        }

        public virtual void Empty()
        {
            List<Item> items = Items;

            if (items.Count > 0)
            {
                Point3D location = Location;
                location.Z += 10;

                Effects.SendLocationEffect(location, Map, 0x3709, 10, 10, 0x356, 0);
                Effects.PlaySound(location, Map, 0x32E);

                for (int i = items.Count - 1; i >= 0; --i)
                {
                    if (i >= items.Count)
                        continue;

                    ConfirmCleanupItem(items[i]);

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

            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
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

    public class SacrificialAltarDeed : BaseAddonContainerDeed
    {
        public override int LabelNumber => 1074818;  // Sacrificial Altar

        [Constructable]
        public SacrificialAltarDeed()
        {
            LootType = LootType.Blessed;
        }

        public SacrificialAltarDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainer Addon => new SacrificialAltarAddon();

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadEncodedInt();
        }
    }
}
