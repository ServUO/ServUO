using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0x4910, 0x4911)]
    public class ChestOfSending : Item, ISecurable
    {
        public static readonly int MaxCharges = 50;

        private SecureLevel m_Level;
        private int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
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
        public DateTime NextRecharge { get; set; }

        public override int LabelNumber => 1150418;  // a chest of sending

        [Constructable]
        public ChestOfSending() : base(0x4910)
        {
            m_Level = SecureLevel.CoOwners;
            m_Charges = 50;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else if (!from.Region.IsPartOf<HouseRegion>())
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
            else if (!IsLockedDown && !IsSecure)
                from.SendLocalizedMessage(1112573); // This must be locked down or secured in order to use it.
            else if (m_Charges == 0)
                from.SendLocalizedMessage(1019073); // This item is out of charges.
            else if (CheckAccessible(from, this))
                from.Target = new SendTarget(this);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060741, m_Charges.ToString()); // charges: ~1_val~
            list.Add(1150598); // auto recharge
        }

        public bool CheckAccessible(Mobile from, Item item)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true; // Staff can access anything

            BaseHouse house = BaseHouse.FindHouseAt(item);

            if (house == null)
                return false;

            switch (m_Level)
            {
                case SecureLevel.Owner: return house.IsOwner(from);
                case SecureLevel.CoOwners: return house.IsCoOwner(from);
                case SecureLevel.Friends: return house.IsFriend(from);
                case SecureLevel.Anyone: return true;
                case SecureLevel.Guild: return house.IsGuildMember(from);
            }

            return false;
        }

        private class SendTarget : Target
        {
            private readonly ChestOfSending m_Chest;

            public SendTarget(ChestOfSending chest) : base(-1, false, TargetFlags.None)
            {
                m_Chest = chest;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Chest.Deleted)
                    return;

                Item item = targeted as Item;

                if (item == null || from.Backpack == null)
                    return;

                if (!from.Region.IsPartOf<HouseRegion>())
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
                else if (!item.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1054107); // This item must be in your backpack.
                }
                else if (item is Container || item is BagOfSending || item is ChestOfSending)
                {
                    from.SendLocalizedMessage(1150420, "#1150424"); // You cannot send a container through the ~1_NAME~.
                }
                else if (!m_Chest.IsLockedDown && !m_Chest.IsSecure)
                {
                    from.SendLocalizedMessage(1112573); // This must be locked down or secured in order to use it.
                }
                else if (m_Chest.Charges == 0)
                {
                    from.SendLocalizedMessage(1019073); // This item is out of charges.
                }
                else if (!m_Chest.CheckAccessible(from, m_Chest))
                {
                }
                else if (m_Chest.InSecureTrade)
                {
                    from.SendLocalizedMessage(1150422, "#1150424"); // The ~1_NAME~ will not function while being traded.
                }
                else if (!item.VerifyMove(from) || item is Engines.Quests.QuestItem)
                {
                    from.SendLocalizedMessage(1150421, "#1150424"); // The ~1_NAME~ rejects that item.
                }
                else if (!from.BankBox.TryDropItem(from, item, false))
                {
                    from.SendLocalizedMessage(1054110); // Your bank box is full.
                }
                else
                {
                    m_Chest.Charges--;
                    from.SendLocalizedMessage(1054150); // The item was placed in your bank box.
                }
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.CheckAlive())
                list.Add(new UseChestEntry(this, CheckAccessible(from, this)));

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        private class UseChestEntry : ContextMenuEntry
        {
            private readonly ChestOfSending m_Chest;

            public UseChestEntry(ChestOfSending chest, bool enabled) : base(1150419, 2)
            {
                m_Chest = chest;

                if (!enabled)
                    Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if (m_Chest.Deleted)
                    return;

                Mobile from = Owner.From;

                if (from.CheckAlive())
                    m_Chest.OnDoubleClick(from);
            }
        }

        public ChestOfSending(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            writer.Write((int)m_Level);
            writer.Write(m_Charges);

            if (NextRecharge < DateTime.UtcNow)
            {
                Charges++;
                NextRecharge = DateTime.UtcNow + TimeSpan.FromHours(Utility.RandomMinMax(11, 13));
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                case 1:
                    if (version == 1)
                        reader.ReadInt();
                    goto case 0;
                case 0:
                    m_Level = (SecureLevel)reader.ReadInt();
                    m_Charges = reader.ReadInt();
                    break;
            }
        }
    }
}
