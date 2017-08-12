using System;
using Server.Targeting;
using Server.Gumps;
using Server.Network;
using System.Collections.Generic;
using Server.Multis;

namespace Server.Items
{
    [FlipableAttribute(0x0FBF, 0x0FC0)]
    public class PenOfWisdom : Item, IUsesRemaining
    {
        public override int LabelNumber { get { return 1115358; } } // Pen of Wisdom		
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        [Constructable]
        public PenOfWisdom() : this(100)
        {
        }

        [Constructable]
        public PenOfWisdom(int uses)
            : base(0x0FC0)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
            m_UsesRemaining = uses;
            Hue = 1260;
            ShowUsesRemaining = true;
        }

        public virtual bool ShowUsesRemaining
        {
            get { return true; }
            set { }
        }

        public PenOfWisdom(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1115359); // Please select the source runebook. Recall runes and Mark scrolls at the base level of your backpack are consumed.
                from.Target = new SourceTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)this.m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_UsesRemaining = reader.ReadInt();
        }

        public bool CheckAccess(Mobile m, Runebook book)
        {
            if (!book.IsLockedDown || m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(book);

            return house != null && house.IsCoOwner(m);
        }

        private class SourceTarget : Target
        {
            private readonly PenOfWisdom m_Pen;

            public SourceTarget(PenOfWisdom pen) : base(12, false, TargetFlags.None)
            {
                m_Pen = pen;
            }

            protected override void OnTarget(Mobile from, object targ)
            {
                if (targ is Runebook || targ is RunicAtlas)
                {
                    Runebook book = targ as Runebook;

                    if (!book.IsChildOf(from.Backpack) && book.Movable)
                    {
                        from.SendLocalizedMessage(1115329); // Runebooks you wish to copy must be in your backpack.
                    }
                    else if (!m_Pen.CheckAccess(from, book) && !book.Movable)
                    {
                        from.SendLocalizedMessage(1115332); // Only the house owner and co-owners can copy the lockdowned runebook with the Pen.
                    }
                    else if (book.Entries.Count == 0)
                    {
                        from.SendLocalizedMessage(1115362); // Can't copy an empty runebook.
                    }
                    else if (book.Openers.Count != 0)
                    {
                        from.SendLocalizedMessage(1115361); // Someone else is using this runebook.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1115363); // Please select the destination runebook.
                        from.Target = new CopyTarget(m_Pen, book);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1115333); // This item only works on runebooks or runic atlas.
                }
            }
        }

        private class CopyTarget : Target
        {
            private readonly PenOfWisdom m_Pen;
            private readonly Runebook m_SourceBook;

            public CopyTarget(PenOfWisdom pen, Runebook book) : base(12, false, TargetFlags.None)
            {
                m_Pen = pen;
                m_SourceBook = book;
            }

            protected override void OnTarget(Mobile from, object targ)
            {
                if (targ is Runebook)
                {
                    Runebook book = targ as Runebook;

                    if (!book.IsChildOf(from.Backpack))
                    {
                        from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                    }
                    else if (book == m_SourceBook)
                    {
                        from.SendLocalizedMessage(1115360); // You can't select the same runebook!
                    }
                    else if (book.Openers.Count != 0)
                    {
                        from.SendLocalizedMessage(1115361); // Someone else is using this runebook.
                    }
                    else
                    {
                        if (!from.HasGump(typeof(PenOfWisdomGump)))
                            from.SendGump(new PenOfWisdomGump(from, m_Pen, m_SourceBook, book, null));
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1115333); // This item only works on runebooks or runic atlas.
                }
            }
        }
    }

    public class PenOfWisdomGump : Gump
    {
        private readonly List<RunebookEntry> m_Checked;
        private readonly Runebook m_SourceBook;
        private readonly Runebook m_CopyBook;
        private readonly int m_RuneAmount;
        private readonly int m_MarkScrollAmount;
        private readonly int m_Blank;
        private readonly PenOfWisdom m_Pen;

        public PenOfWisdomGump(Mobile from, PenOfWisdom pen, Runebook sourcebook, Runebook copybook, List<RunebookEntry> list) : base(50, 50)
        {
            Container bp = from.Backpack;

            m_Pen = pen;
            m_SourceBook = sourcebook;
            m_CopyBook = copybook;
            m_MarkScrollAmount = bp.GetAmount(typeof(MarkScroll), true);
            m_RuneAmount = bp.GetAmount(typeof(RecallRune), true);
            m_Blank = copybook.MaxEntries - copybook.Entries.Count;

            if (list == null)
            {
                m_Checked = new List<RunebookEntry>();
            }
            else
            {
                m_Checked = list;
            }

            Closable = false;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(4, 39, 391, 313, 9200);
            AddImageTiled(8, 45, 380, 53, 2624);
            AddHtmlLocalized(7, 50, 380, 53, 1115428, String.Format("{0}\t{1}\t{2}\t{3}", m_MarkScrollAmount.ToString(), m_RuneAmount.ToString(), m_Checked.Count, m_Blank.ToString()), 0xFFFFFF, false, false); // <CENTER>Pen of Wisdom<br>(Mark Scrolls: ~1_VAL~, Runes: ~2_VAL~ | Selected: ~3_VAL~, Blank: ~4_VAL~)</CENTER>

            AddImageTiled(8, 101, 188, 220, 2624);
            AddImageTiled(199, 101, 188, 220, 2624);
            AddButton(12, 325, 4017, 4018, 20, GumpButtonType.Reply, 0);
            AddLabel(48, 325, 2100, @"Cancel");
            AddButton(153, 325, 4011, 4012, 21, GumpButtonType.Reply, 0);
            AddHtmlLocalized(189, 325, 78, 22, 1115427, 0xFFFFFF, false, false); // Select All
            AddButton(309, 325, 4023, 4024, 22, GumpButtonType.Reply, 0);
            AddLabel(344, 325, 2100, @"Okay");

            string description;

            for (int i = 0; i < sourcebook.Entries.Count; i++)
            {
                description = sourcebook.Entries[i].Description;

                if (description == null)
                {
                    if (i + 1 < 10)
                    {
                        description = "0" + (i + 1).ToString();
                    }
                    else
                    {
                        description = (i + 1).ToString();
                    }
                }

                if (i < 8)
                {
                    AddButton(15, 110 + i * 25, m_Checked.Contains(sourcebook.Entries[i]) ? 211 : 210, m_Checked.Contains(sourcebook.Entries[i]) ? 210 : 211, i, GumpButtonType.Reply, 0);
                    AddLabel(45, 110 + i * 25, RunebookGump.GetMapHue(sourcebook.Entries[i].Map), String.Format("{0}", description));
                }
                else
                {
                    AddButton(205, 110 + ((i - 8) * 25), m_Checked.Contains(sourcebook.Entries[i]) ? 211 : 210, m_Checked.Contains(sourcebook.Entries[i]) ? 210 : 211, i, GumpButtonType.Reply, 0);
                    AddLabel(235, 110 + ((i - 8) * 25), RunebookGump.GetMapHue(sourcebook.Entries[i].Map), String.Format("{0}", description));
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_Checked == null)
                return;

            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 20: // Cancel
                    {
                        break;
                    }
                case 21: // Select All
                    {
                        m_Checked.Clear();

                        for (int i = 0; i < m_SourceBook.Entries.Count; i++)
                        {
                            m_Checked.Add(m_SourceBook.Entries[i]);
                        }

                        if (!from.HasGump(typeof(PenOfWisdomGump)))
                            from.SendGump(new PenOfWisdomGump(from, m_Pen, m_SourceBook, m_CopyBook, m_Checked));

                        break;
                    }
                case 22: // OK
                    {
                        if (m_MarkScrollAmount < m_Checked.Count || m_RuneAmount < m_Checked.Count)
                        {
                            from.SendLocalizedMessage(1115364); // You don't have enough recall runes and Mark scrolls to do that.
                        }
                        else if (m_Blank < m_Checked.Count)
                        {
                            from.SendLocalizedMessage(1115330); // The destination runebook doesn't have enough space.
                        }
                        else if (!m_SourceBook.IsChildOf(from.Backpack) && m_SourceBook.Movable || !m_CopyBook.IsChildOf(from.Backpack))
                        {
                            from.SendLocalizedMessage(1115329); // Runebooks you wish to copy must be in your backpack.
                        }
                        else
                        {
                            foreach (RunebookEntry entry in m_Checked)
                            {
                                m_CopyBook.Entries.Add(entry);
                            }

                            Container bp = from.Backpack;

                            bp.ConsumeTotal(typeof(MarkScroll), m_Checked.Count, true);
                            bp.ConsumeTotal(typeof(RecallRune), m_Checked.Count, true);
                            m_Pen.UsesRemaining -= 1;
                            m_Pen.InvalidateProperties();

                            from.SendLocalizedMessage(1115331); // The Pen magically marks runes and binds them to the runebook.
                            from.SendLocalizedMessage(1115366); // The pen's magical power is consumed and it crumbles to dust.
                        }

                        break;
                    }
                default:
                    {
                        int index = info.ButtonID;

                        if (m_Checked.Contains(m_SourceBook.Entries[index]))
                        {
                            m_Checked.Remove(m_SourceBook.Entries[index]);
                        }
                        else
                        {
                            m_Checked.Add(m_SourceBook.Entries[index]);
                        }

                        if (!from.HasGump(typeof(PenOfWisdomGump)))
                            from.SendGump(new PenOfWisdomGump(from, m_Pen, m_SourceBook, m_CopyBook, m_Checked));

                        break;
                    }
            }
        }
    }
}