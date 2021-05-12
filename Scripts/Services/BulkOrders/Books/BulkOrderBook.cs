using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Prompts;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
    public class BulkOrderBook : Item, ISecurable
    {
        private ArrayList m_Entries;
        private BOBFilter m_Filter;
        private string m_BookName;
        private SecureLevel m_Level;
        private int m_ItemCount;
        [Constructable]
        public BulkOrderBook()
            : base(0x2259)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;

            m_Entries = new ArrayList();
            m_Filter = new BOBFilter();

            m_Level = SecureLevel.CoOwners;
        }

        public BulkOrderBook(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string BookName
        {
            get
            {
                return m_BookName;
            }
            set
            {
                m_BookName = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return m_Level;
            }
            set
            {
                m_Level = value;
            }
        }
        public ArrayList Entries => m_Entries;
        public BOBFilter Filter => m_Filter;
        public int ItemCount
        {
            get
            {
                return m_ItemCount;
            }
            set
            {
                m_ItemCount = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else if (m_Entries.Count == 0)
                from.SendLocalizedMessage(1062381); // The book is empty.
            else if (from is PlayerMobile)
                from.SendGump(new BOBGump((PlayerMobile)from, this));
        }

        public override void OnDoubleClickSecureTrade(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (m_Entries.Count == 0)
            {
                from.SendLocalizedMessage(1062381); // The book is empty.
            }
            else
            {
                from.SendGump(new BOBGump((PlayerMobile)from, this));

                SecureTradeContainer cont = GetSecureTradeCont();

                if (cont != null)
                {
                    SecureTrade trade = cont.Trade;

                    if (trade != null && trade.From.Mobile == from)
                        trade.To.Mobile.SendGump(new BOBGump((PlayerMobile)(trade.To.Mobile), this));
                    else if (trade != null && trade.To.Mobile == from)
                        trade.From.Mobile.SendGump(new BOBGump((PlayerMobile)(trade.From.Mobile), this));
                }
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is LargeBOD || dropped is SmallBOD)
            {
                if (!IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1062385); // You must have the book in your backpack to add deeds to it.
                    return false;
                }
                else if (!from.Backpack.CheckHold(from, dropped, true, true))
                    return false;
                else if (m_Entries.Count < 500)
                {
                    if (dropped is LargeBOD)
                        m_Entries.Add(new BOBLargeEntry((LargeBOD)dropped));
                    else if (dropped is SmallBOD) // Sanity
                        m_Entries.Add(new BOBSmallEntry((SmallBOD)dropped));

                    InvalidateProperties();

                    if (m_Entries.Count / 5 > m_ItemCount)
                    {
                        m_ItemCount++;
                        InvalidateItems();
                    }

                    from.SendSound(0x42, GetWorldLocation());
                    from.SendLocalizedMessage(1062386); // Deed added to book.

                    if (from is PlayerMobile)
                        from.SendGump(new BOBGump((PlayerMobile)from, this));

                    dropped.Delete();

                    return true;
                }
                else
                {
                    from.SendLocalizedMessage(1062387); // The book is full of deeds.
                    return false;
                }
            }

            from.SendLocalizedMessage(1062388); // That is not a bulk order deed.
            return false;
        }

        public override int GetTotal(TotalType type)
        {
            int total = base.GetTotal(type);

            if (type == TotalType.Items)
                total = m_ItemCount;

            return total;
        }

        public void InvalidateItems()
        {
            if (RootParent is Mobile)
            {
                Mobile m = (Mobile)RootParent;

                m.UpdateTotals();
                InvalidateContainers(Parent);
            }
        }

        public void InvalidateContainers(object parent)
        {
            if (parent != null && parent is Container)
            {
                Container c = (Container)parent;

                c.InvalidateProperties();
                InvalidateContainers(c.Parent);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            writer.Write(m_ItemCount);

            writer.Write((int)m_Level);

            writer.Write(m_BookName);

            m_Filter.Serialize(writer);

            writer.WriteEncodedInt(m_Entries.Count);

            for (int i = 0; i < m_Entries.Count; ++i)
            {
                object obj = m_Entries[i];

                if (obj is BOBLargeEntry)
                {
                    writer.WriteEncodedInt(0);
                    ((BOBLargeEntry)obj).Serialize(writer);
                }
                else if (obj is BOBSmallEntry)
                {
                    writer.WriteEncodedInt(1);
                    ((BOBSmallEntry)obj).Serialize(writer);
                }
                else
                {
                    writer.WriteEncodedInt(-1);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        m_ItemCount = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        m_BookName = reader.ReadString();

                        m_Filter = new BOBFilter(reader);

                        int count = reader.ReadEncodedInt();

                        m_Entries = new ArrayList(count);

                        for (int i = 0; i < count; ++i)
                        {
                            int v = reader.ReadEncodedInt();

                            switch (v)
                            {
                                case 0:
                                    m_Entries.Add(new BOBLargeEntry(reader));
                                    break;
                                case 1:
                                    m_Entries.Add(new BOBSmallEntry(reader));
                                    break;
                            }
                        }

                        break;
                    }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1062344, m_Entries.Count.ToString()); // Deeds in book: ~1_val~

            if (!string.IsNullOrEmpty(m_BookName))
                list.Add(1062481, m_BookName); // Book Name: ~1_val~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.CheckAlive() && IsChildOf(from.Backpack))
                list.Add(new NameBookEntry(from, this));

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        private class NameBookEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly BulkOrderBook m_Book;
            public NameBookEntry(Mobile from, BulkOrderBook book)
                : base(6216)
            {
                m_From = from;
                m_Book = book;
            }

            public override void OnClick()
            {
                if (m_From.CheckAlive() && m_Book.IsChildOf(m_From.Backpack))
                {
                    m_From.Prompt = new NameBookPrompt(m_Book);
                    m_From.SendLocalizedMessage(1062479); // Type in the new name of the book:
                }
            }
        }

        private class NameBookPrompt : Prompt
        {
            public override int MessageCliloc => 1062479;
            private readonly BulkOrderBook m_Book;
            public NameBookPrompt(BulkOrderBook book)
            {
                m_Book = book;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (text.Length > 40)
                    text = text.Substring(0, 40);

                if (from.CheckAlive() && m_Book.IsChildOf(from.Backpack))
                {
                    m_Book.BookName = Utility.FixHtml(text.Trim());

                    from.SendLocalizedMessage(1062480); // The bulk order book's name has been changed.
                }
            }

            public override void OnCancel(Mobile from)
            {
            }
        }
    }
}