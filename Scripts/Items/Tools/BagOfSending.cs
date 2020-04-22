using Server.ContextMenus;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public enum BagOfSendingHue
    {
        Yellow,
        Blue,
        Red,
        Green
    }

    public class BagOfSending : Item, TranslocationItem
    {
        private int m_Charges;
        private int m_Recharges;
        private BagOfSendingHue m_BagOfSendingHue;
        [Constructable]
        public BagOfSending()
            : this(RandomHue())
        {
        }

        [Constructable]
        public BagOfSending(BagOfSendingHue hue)
            : base(0xE76)
        {
            Weight = 2.0;

            BagOfSendingHue = hue;

            m_Charges = Utility.RandomMinMax(3, 9);
        }

        public BagOfSending(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return m_Charges;
            }
            set
            {
                if (value > MaxCharges)
                    m_Charges = MaxCharges;
                else if (value < 0)
                    m_Charges = 0;
                else
                    m_Charges = value;

                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Recharges
        {
            get
            {
                return m_Recharges;
            }
            set
            {
                if (value > MaxRecharges)
                    m_Recharges = MaxRecharges;
                else if (value < 0)
                    m_Recharges = 0;
                else
                    m_Recharges = value;

                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges => 30;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRecharges => 255;
        public string TranslocationItemName => "bag of sending";
        public override int LabelNumber => 1054104;// a bag of sending
        [CommandProperty(AccessLevel.GameMaster)]
        public BagOfSendingHue BagOfSendingHue
        {
            get
            {
                return m_BagOfSendingHue;
            }
            set
            {
                m_BagOfSendingHue = value;

                switch (value)
                {
                    case BagOfSendingHue.Yellow:
                        Hue = 0x8A5;
                        break;
                    case BagOfSendingHue.Blue:
                        Hue = 0x8AD;
                        break;
                    case BagOfSendingHue.Red:
                        Hue = 0x89B;
                        break;
                    case BagOfSendingHue.Green:
                        Hue = 0x08A0;
                        break;
                }
            }
        }
        public static BagOfSendingHue RandomHue()
        {
            switch (Utility.Random(4))
            {
                case 0:
                    return BagOfSendingHue.Yellow;
                case 1:
                    return BagOfSendingHue.Blue;
                case 2:
                    return BagOfSendingHue.Red;
                default:
                    return BagOfSendingHue.Green;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060741, m_Charges.ToString()); // charges: ~1_val~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive)
                list.Add(new UseBagEntry(this, Charges > 0 && IsChildOf(from.Backpack)));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Region.IsPartOf<Regions.Jail>())
            {
                from.SendMessage("You may not do that in jail.");
            }
            else if (!IsChildOf(from.Backpack))
            {
                MessageHelper.SendLocalizedMessageTo(this, from, 1062334, 0x59); // The bag of sending must be in your backpack.
            }
            else if (Charges == 0)
            {
                MessageHelper.SendLocalizedMessageTo(this, from, 1042544, 0x59); // This item is out of charges.
            }
            else
            {
                from.Target = new SendTarget(this);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version

            writer.WriteEncodedInt(m_Recharges);

            writer.WriteEncodedInt(m_Charges);
            writer.WriteEncodedInt((int)m_BagOfSendingHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    {
                        m_Recharges = reader.ReadEncodedInt();
                        goto case 0;
                    }
                case 0:
                    {
                        m_Charges = Math.Min(reader.ReadEncodedInt(), MaxCharges);
                        m_BagOfSendingHue = (BagOfSendingHue)reader.ReadEncodedInt();
                        break;
                    }
            }
        }

        private class UseBagEntry : ContextMenuEntry
        {
            private readonly BagOfSending m_Bag;
            public UseBagEntry(BagOfSending bag, bool enabled)
                : base(6189)
            {
                m_Bag = bag;

                if (!enabled)
                    Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if (m_Bag.Deleted)
                    return;

                Mobile from = Owner.From;

                if (from.CheckAlive())
                    m_Bag.OnDoubleClick(from);
            }
        }

        private class SendTarget : Target
        {
            private readonly BagOfSending m_Bag;
            public SendTarget(BagOfSending bag)
                : base(-1, false, TargetFlags.None)
            {
                m_Bag = bag;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Bag.Deleted)
                    return;

                if (from.Region.IsPartOf<Regions.Jail>())
                {
                    from.SendMessage("You may not do that in jail.");
                }
                else if (!m_Bag.IsChildOf(from.Backpack))
                {
                    MessageHelper.SendLocalizedMessageTo(m_Bag, from, 1062334, 0x59); // The bag of sending must be in your backpack. 1054107 is gone from client, using generic response
                }
                else if (m_Bag.Charges == 0)
                {
                    MessageHelper.SendLocalizedMessageTo(m_Bag, from, 1042544, 0x59); // This item is out of charges.
                }
                else if (targeted is Item)
                {
                    Item item = (Item)targeted;
                    int reqCharges = 1; // (int)Math.Max(1, Math.Ceiling(item.TotalWeight / 10.0));
                                        // change was ML, however reverted during ML period so we can put it at 1

                    if (!item.IsChildOf(from.Backpack))
                    {
                        MessageHelper.SendLocalizedMessageTo(m_Bag, from, 1054152, 0x59); // You may only send items from your backpack to your bank box.
                    }
                    else if (item is BagOfSending || item is Container)
                    {
                        from.Send(new AsciiMessage(m_Bag.Serial, m_Bag.ItemID, MessageType.Regular, 0x3B2, 3, "", "You cannot send a container through the bag of sending."));
                    }
                    else if (item.LootType == LootType.Cursed)
                    {
                        MessageHelper.SendLocalizedMessageTo(m_Bag, from, 1054108, 0x59); // The bag of sending rejects the cursed item.
                    }
                    else if (!item.VerifyMove(from) || item is Engines.Quests.QuestItem || item.QuestItem)
                    {
                        MessageHelper.SendLocalizedMessageTo(m_Bag, from, 1054109, 0x59); // The bag of sending rejects that item.
                    }
                    else if (Spells.SpellHelper.IsDoomGauntlet(from.Map, from.Location))
                    {
                        from.SendLocalizedMessage(1062089); // You cannot use that here.
                    }
                    else if (!from.BankBox.TryDropItem(from, item, false))
                    {
                        MessageHelper.SendLocalizedMessageTo(m_Bag, from, 1054110, 0x59); // Your bank box is full.
                    }
                    else if (reqCharges > m_Bag.Charges)
                    {
                        from.SendLocalizedMessage(1079932); //You don't have enough charges to send that much weight
                    }
                    else
                    {
                        m_Bag.Charges -= reqCharges;
                        MessageHelper.SendLocalizedMessageTo(m_Bag, from, 1054150, 0x59); // The item was placed in your bank box.
                    }
                }
            }
        }
    }
}
