using Server.Targeting;
using System;

namespace Server.Items
{
    public interface ICommodity /* added IsDeedable prop so expansion-based deedables can determine true/false */
    {
        TextDefinition Description { get; }
        bool IsDeedable { get; }
    }

    public static class CommodityDeedExtensions
    {
        public static int GetAmount(this Container cont, Type type, bool recurse, bool includeDeeds)
        {
            int amount = cont.GetAmount(type, recurse);

            Item[] deeds = cont.FindItemsByType(typeof(CommodityDeed), recurse);
            foreach (CommodityDeed deed in deeds)
            {
                if (deed.Commodity == null)
                    continue;
                if (deed.Commodity.GetType() == type)
                    amount += deed.Commodity.Amount;
            }

            return amount;
        }

        public static int GetAmount(this Container cont, Type[] types, bool recurse, bool includeDeeds)
        {
            int amount = cont.GetAmount(types, recurse);

            Item[] deeds = cont.FindItemsByType(typeof(CommodityDeed), recurse);
            foreach (CommodityDeed deed in deeds)
            {
                if (deed.Commodity == null)
                    continue;
                foreach (Type type in types)
                {
                    if (deed.Commodity.GetType() == type)
                    {
                        amount += deed.Commodity.Amount;
                        break;
                    }
                }
            }

            return amount;
        }

        public static int ConsumeTotal(this Container cont, Type type, int amount, bool recurse, bool includeDeeds)
        {
            int left = amount;

            Item[] items = cont.FindItemsByType(type, recurse);
            foreach (Item item in items)
            {
                if (item.Amount <= left)
                {
                    left -= item.Amount;
                    item.Delete();
                }
                else
                {
                    item.Amount -= left;
                    left = 0;
                    break;
                }
            }

            if (!includeDeeds)
                return amount - left;

            Item[] deeds = cont.FindItemsByType(typeof(CommodityDeed), recurse);
            foreach (CommodityDeed deed in deeds)
            {
                if (deed.Commodity == null)
                    continue;
                if (deed.Commodity.GetType() != type)
                    continue;
                if (deed.Commodity.Amount <= left)
                {
                    left -= deed.Commodity.Amount;
                    deed.Delete();
                }
                else
                {
                    deed.Commodity.Amount -= left;
                    deed.InvalidateProperties();
                    left = 0;
                    break;
                }
            }

            return amount - left;
        }
    }

    public class CommodityDeed : Item
    {
        public CommodityDeed(Item commodity)
            : base(0x14F0)
        {
            Weight = 1.0;
            Hue = 0x47;

            Commodity = commodity;

            LootType = LootType.Blessed;
        }

        [Constructable]
        public CommodityDeed()
            : this(null)
        {
        }

        public CommodityDeed(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Item Commodity { get; private set; }


        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (Commodity == null)
            {
                list.Add(1047016);
            }
            else
            {
                list.Add(1115599, string.Format("{0}\t#{1}", Commodity.Amount, Commodity.LabelNumber));
            }
        }

        public bool SetCommodity(Item item)
        {
            InvalidateProperties();

            if (Commodity == null && item is ICommodity && ((ICommodity)item).IsDeedable)
            {
                Commodity = item;
                Commodity.Internalize();
                Hue = 0x592;

                InvalidateProperties();

                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write(Commodity);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Commodity = reader.ReadItem();

            switch (version)
            {
                case 0:
                    {
                        if (Commodity != null)
                        {
                            Hue = 0x592;
                        }
                        break;
                    }
            }
        }

        public override void OnDelete()
        {
            if (Commodity != null)
                Commodity.Delete();

            base.OnDelete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Commodity != null)
            {
                list.Add(1060747); // filled
            }
            else
            {
                list.Add(1060748); // unfilled
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            int number;

            BankBox box = from.FindBankNoCreate();
            CommodityDeedBox cox = CommodityDeedBox.Find(this);
            GalleonHold hold = RootParent as GalleonHold;

            // Veteran Rewards mods
            if (Commodity != null)
            {
                if (box != null && IsChildOf(box))
                {
                    number = 1047031; // The commodity has been redeemed.

                    box.DropItem(Commodity);

                    Commodity = null;
                    Delete();
                }
                else if (cox != null)
                {
                    if (cox.IsSecure)
                    {
                        number = 1047031; // The commodity has been redeemed.

                        cox.DropItem(Commodity);

                        Commodity = null;
                        Delete();
                    }
                    else
                        number = 1080525; // The commodity deed box must be secured before you can use it.
                }
                else if (hold != null)
                {
                    number = 1047031; // The commodity has been redeemed.

                    hold.DropItem(Commodity);
                    Commodity = null;

                    Delete();
                }
                else
                {
                    number = 1080526; // That must be in your bank box or commodity deed box to use it.
                }
            }
            else if (cox != null && !cox.IsSecure)
            {
                number = 1080525; // The commodity deed box must be secured before you can use it.
            }
            else if ((box == null || !IsChildOf(box)) && cox == null && hold == null)
            {
                number = 1080526; // That must be in your bank box or commodity deed box to use it.
            }
            else
            {
                number = 1047029; // Target the commodity to fill this deed with.

                from.Target = new InternalTarget(this);
            }

            from.SendLocalizedMessage(number);
        }

        private class InternalTarget : Target
        {
            private readonly CommodityDeed m_Deed;

            public InternalTarget(CommodityDeed deed)
                : base(3, false, TargetFlags.None)
            {
                m_Deed = deed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Deed.Deleted)
                    return;

                int number;

                if (m_Deed.Commodity != null)
                {
                    number = 1047028; // The commodity deed has already been filled.
                }
                else if (targeted is Item)
                {
                    BankBox box = from.FindBankNoCreate();
                    CommodityDeedBox cox = CommodityDeedBox.Find(m_Deed);
                    GalleonHold hold = ((Item)targeted).RootParent as GalleonHold;

                    // Veteran Rewards mods
                    if (box != null && m_Deed.IsChildOf(box) && ((Item)targeted).IsChildOf(box) ||
                        (cox != null && cox.IsSecure && ((Item)targeted).IsChildOf(cox)) ||
                        hold != null)
                    {
                        if (m_Deed.SetCommodity((Item)targeted))
                        {
                            number = 1047030; // The commodity deed has been filled.
                        }
                        else
                        {
                            number = 1047027; // That is not a commodity the bankers will fill a commodity deed with.
                        }
                    }
                    else
                    {
                        number = 1080526; // That must be in your bank box or commodity deed box to use it.
                    }
                }
                else
                {
                    number = 1047027; // That is not a commodity the bankers will fill a commodity deed with.
                }

                from.SendLocalizedMessage(number);
            }
        }
    }
}
