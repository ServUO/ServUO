using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Prompts;

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
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;

            this.m_Entries = new ArrayList();
            this.m_Filter = new BOBFilter();

            this.m_Level = SecureLevel.CoOwners;
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
                return this.m_BookName;
            }
            set
            {
                this.m_BookName = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }
        public ArrayList Entries
        {
            get
            {
                return this.m_Entries;
            }
        }
        public BOBFilter Filter
        {
            get
            {
                return this.m_Filter;
            }
        }
        public int ItemCount
        {
            get
            {
                return this.m_ItemCount;
            }
            set
            {
                this.m_ItemCount = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else if (this.m_Entries.Count == 0)
                from.SendLocalizedMessage(1062381); // The book is empty.
            else if (from is PlayerMobile)
                from.SendGump(new BOBGump((PlayerMobile)from, this));
        }

        public override void OnDoubleClickSecureTrade(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (this.m_Entries.Count == 0)
            {
                from.SendLocalizedMessage(1062381); // The book is empty.
            }
            else
            {
                from.SendGump(new BOBGump((PlayerMobile)from, this));

                SecureTradeContainer cont = this.GetSecureTradeCont();

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
                if (!this.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1062385); // You must have the book in your backpack to add deeds to it.
                    return false;
                }
                else if (!from.Backpack.CheckHold(from, dropped, true, true))
                    return false;
                else if (this.m_Entries.Count < 500)
                {
                    if (dropped is LargeBOD)
                        this.m_Entries.Add(new BOBLargeEntry((LargeBOD)dropped));
                    else if (dropped is SmallBOD) // Sanity
                        this.m_Entries.Add(new BOBSmallEntry((SmallBOD)dropped));
					
                    this.InvalidateProperties();
					
                    if (this.m_Entries.Count / 5 > this.m_ItemCount)
                    {
                        this.m_ItemCount++;
                        this.InvalidateItems();
                    }

                    from.SendSound(0x42, this.GetWorldLocation());
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
                total = this.m_ItemCount;

            return total;
        }

        public void InvalidateItems()
        {
            if (this.RootParent is Mobile)
            {
                Mobile m = (Mobile)this.RootParent;

                m.UpdateTotals();
                this.InvalidateContainers(this.Parent);
            }
        }

        public void InvalidateContainers(object parent)
        {
            if (parent != null && parent is Container)
            {
                Container c = (Container)parent;
				
                c.InvalidateProperties();
                this.InvalidateContainers(c.Parent);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
			
            writer.Write((int)this.m_ItemCount);

            writer.Write((int)this.m_Level);

            writer.Write(this.m_BookName);

            this.m_Filter.Serialize(writer);

            writer.WriteEncodedInt((int)this.m_Entries.Count);

            for (int i = 0; i < this.m_Entries.Count; ++i)
            {
                object obj = this.m_Entries[i];

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

            switch ( version )
            {
                case 2:
                    {
                        this.m_ItemCount = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_BookName = reader.ReadString();

                        this.m_Filter = new BOBFilter(reader);

                        int count = reader.ReadEncodedInt();

                        this.m_Entries = new ArrayList(count);

                        for (int i = 0; i < count; ++i)
                        {
                            int v = reader.ReadEncodedInt();

                            switch ( v )
                            {
                                case 0:
                                    this.m_Entries.Add(new BOBLargeEntry(reader));
                                    break;
                                case 1:
                                    this.m_Entries.Add(new BOBSmallEntry(reader));
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

            list.Add(1062344, this.m_Entries.Count.ToString()); // Deeds in book: ~1_val~

            if (this.m_BookName != null && this.m_BookName.Length > 0)
                list.Add(1062481, this.m_BookName); // Book Name: ~1_val~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.CheckAlive() && this.IsChildOf(from.Backpack))
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
                this.m_From = from;
                this.m_Book = book;
            }

            public override void OnClick()
            {
                if (this.m_From.CheckAlive() && this.m_Book.IsChildOf(this.m_From.Backpack))
                {
                    this.m_From.Prompt = new NameBookPrompt(this.m_Book);
                    this.m_From.SendLocalizedMessage(1062479); // Type in the new name of the book:
                }
            }
        }

        private class NameBookPrompt : Prompt
        {
            public override int MessageCliloc { get { return 1062479; } }
            private readonly BulkOrderBook m_Book;
            public NameBookPrompt(BulkOrderBook book)
            {
                this.m_Book = book;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (text.Length > 40)
                    text = text.Substring(0, 40);

                if (from.CheckAlive() && this.m_Book.IsChildOf(from.Backpack))
                {
                    this.m_Book.BookName = Utility.FixHtml(text.Trim());

                    from.SendLocalizedMessage(1062480); // The bulk order book's name has been changed.
                }
            }

            public override void OnCancel(Mobile from)
            {
            }
        }
    }
}