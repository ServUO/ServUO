using System;

namespace Server.Items
{
    public class EnchantedHotItemSocket : ItemSocket
    {
        public Container Container { get; set; }

        public override TimeSpan TickDuration => TimeSpan.FromSeconds(3);

        public EnchantedHotItemSocket()
        {
        }

        public EnchantedHotItemSocket(Container c)
        {
            Container = c;

            BeginTimer();
        }

        protected override void OnTick()
        {
            if (Owner == null || Owner.Deleted)
            {
                EndTimer();
            }
            else if (Owner.RootParent is Mobile m)
            {
                if (m.Region != null && m.Region.IsPartOf("Wrong"))
                {
                    AOS.Damage(m, Utility.RandomMinMax(10, 13), 0, 100, 0, 0, 0);

                    if (0.2 > Utility.RandomDouble())
                    {
                        m.SendLocalizedMessage(1152086); // Ouch! These stolen items are hot!
                    }
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            list.Add(1152081); // Enchanted Hot Item
            list.Add(1152082); // (Escape from dungeon to remove spell)
        }

        public override void OnRemoved()
        {
            if (Owner is BaseWeapon)
            {
                Owner.Hue = ((BaseWeapon)Owner).GetElementalDamageHue();
            }
            else
            {
                Owner.Hue = 0;
            }
        }

        public override void OnAfterDuped(ItemSocket oldSocket)
        {
            if (oldSocket is EnchantedHotItemSocket)
            {
                Container = ((EnchantedHotItemSocket)oldSocket).Container;
            }
        }

        public static void OnEnterRegion(OnEnterRegionEventArgs e)
        {
            Mobile m = e.From;

            if (!m.Player || e.OldRegion == null || e.NewRegion == null || m.Backpack == null)
                return;

            if (e.OldRegion.IsPartOf("Wrong") && !e.NewRegion.IsPartOf("Wrong"))
            {
                bool found = false;

                m.Backpack.Items.IterateReverse(i =>
                {
                    EnchantedHotItemSocket socket = i.GetSocket<EnchantedHotItemSocket>();

                    if (socket != null)
                    {
                        found = true;
                        socket.Remove();
                    }
                });

                if (found)
                {
                    m.SendLocalizedMessage(1152085); // The curse is removed from the item you stole!
                }
            }
        }

        public static bool CheckDrop(Mobile from, Container droppedTo, Item dropped)
        {
            EnchantedHotItemSocket socket = dropped.GetSocket<EnchantedHotItemSocket>();

            if (socket != null)
            {
                Container c = socket.Container;

                if (droppedTo != c && droppedTo != from.Backpack)
                {
                    from.SendLocalizedMessage(1152083); // The stolen item magically returns to the trunk where you found it.

                    if (c != null)
                    {
                        c.DropItem(dropped);
                    }
                    else
                    {
                        dropped.Delete();
                    }

                    return false;
                }
            }

            return true;
        }

        public static bool CheckDrop(Mobile from, Item dropped)
        {
            EnchantedHotItemSocket socket = dropped.GetSocket<EnchantedHotItemSocket>();

            if (socket != null)
            {
                Container c = socket.Container;

                from.SendLocalizedMessage(1152083); // The stolen item magically returns to the trunk where you found it.

                if (c != null)
                {
                    c.DropItem(dropped);
                }
                else
                {
                    dropped.Delete();
                }

                return false;
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Container);
        }

        public override void Deserialize(Item owner, GenericReader reader)
        {
            base.Deserialize(owner, reader);
            reader.ReadInt(); // version

            Container = reader.ReadItem() as Container;

            BeginTimer();
        }

        public static void Initialize()
        {
            EventSink.OnEnterRegion += OnEnterRegion;
        }
    }
}
