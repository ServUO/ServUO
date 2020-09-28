using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0x0FBF, 0x0FC0)]
    public class PenOfWisdom : Item, IUsesRemaining
    {
        public override int LabelNumber => 1115358;  // Pen of Wisdom		
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

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (IsChildOf(from.Backpack))
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
            writer.Write(0); // version

            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_UsesRemaining = reader.ReadInt();
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
            private readonly PenOfWisdom Pen;

            public SourceTarget(PenOfWisdom pen) : base(12, false, TargetFlags.None)
            {
                Pen = pen;
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
                    else if (!Pen.CheckAccess(from, book) && !book.Movable)
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
                        from.Target = new CopyTarget(Pen, book);
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
            private readonly PenOfWisdom Pen;
            private readonly Runebook SourceBook;

            public CopyTarget(PenOfWisdom pen, Runebook book) : base(12, false, TargetFlags.None)
            {
                Pen = pen;
                SourceBook = book;
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
                    else if (book == SourceBook)
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
                            from.SendGump(new PenOfWisdomGump(from, Pen, SourceBook, book, null));
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
        public readonly int EntryColor = 0xFFFFFF;

        public readonly List<RunebookEntry> Checked;
        public readonly Runebook SourceBook;
        public readonly Runebook CopyBook;
        public readonly int RuneAmount;
        public readonly int MarkScrollAmount;
        public readonly int Blank;
        public readonly PenOfWisdom Pen;

        public PenOfWisdomGump(Mobile from, PenOfWisdom pen, Runebook sourcebook, Runebook copybook, List<RunebookEntry> list)
            : base(50, 50)
        {
            Container bp = from.Backpack;

            Pen = pen;
            SourceBook = sourcebook;
            CopyBook = copybook;
            MarkScrollAmount = bp.GetAmount(typeof(MarkScroll), true);
            RuneAmount = bp.GetAmount(typeof(RecallRune), true);
            Blank = copybook.MaxEntries - copybook.Entries.Count;

            if (list == null)
            {
                Checked = new List<RunebookEntry>();
            }
            else
            {
                Checked = list;
            }

            Closable = false;
            Disposable = true;
            Dragable = true;

            int entrycount = SourceBook.Entries.Count;

            int y = entrycount <= 16 ? 0 : 25;

            AddPage(0);

            AddBackground(4, 39, 391, 313 + y, 9200);
            AddImageTiled(8, 45, 380, 53, 2624);

            AddHtmlLocalized(7, 50, 380, 53, 1115428, string.Format("@{0}@{1}@{2}@{3}", MarkScrollAmount.ToString(), RuneAmount.ToString(), Checked.Count, Blank.ToString()), EntryColor, false, false); // <CENTER>Pen of Wisdom<br>(Mark Scrolls: ~1_VAL~, Runes: ~2_VAL~ | Selected: ~3_VAL~, Blank: ~4_VAL~)</CENTER>

            AddImageTiled(8, 101, 188, 220, 2624);
            AddImageTiled(199, 101, 188, 220, 2624);

            AddButton(12, 325 + y, 4017, 4018, 20, GumpButtonType.Reply, 0);
            AddHtmlLocalized(48, 326 + y, 78, 20, 1006045, EntryColor, false, false); // Cancel

            AddButton(153, 325 + y, 4011, 4012, 21, GumpButtonType.Reply, 0);
            AddHtmlLocalized(189, 326 + y, 78, 20, 1115427, EntryColor, false, false); // Select All

            AddButton(309, 325 + y, 4023, 4024, 22, GumpButtonType.Reply, 0);
            AddHtmlLocalized(344, 326 + y, 78, 20, 1156596, EntryColor, false, false); // Okay

            string description;

            int page = 1;
            int yy = 0;

            AddPage(page);

            for (int i = 0; i < entrycount; i++)
            {
                if (page > 1)
                {
                    AddButton(50, 325, 4014, 4015, 0, GumpButtonType.Page, page - 1);
                    AddHtmlLocalized(85, 326, 150, 20, 1011067, EntryColor, false, false); // Previous page
                }

                description = SourceBook.Entries[i].Description;

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

                if (yy < 8)
                {
                    AddButton(15, 110 + (yy * 25), Checked.Contains(SourceBook.Entries[i]) ? 211 : 210, Checked.Contains(SourceBook.Entries[i]) ? 210 : 211, i, GumpButtonType.Reply, 0);
                    AddLabelCropped(45, 110 + (yy * 25), 115, 17, RunebookGump.GetMapHue(SourceBook.Entries[i].Map), string.Format("{0}", description));
                }
                else
                {
                    AddButton(205, 110 + ((yy - 8) * 25), Checked.Contains(SourceBook.Entries[i]) ? 211 : 210, Checked.Contains(SourceBook.Entries[i]) ? 210 : 211, i, GumpButtonType.Reply, 0);
                    AddLabelCropped(235, 110 + ((yy - 8) * 25), 115, 17, RunebookGump.GetMapHue(SourceBook.Entries[i].Map), string.Format("{0}", description));
                }

                yy++;

                bool pages = (i + 1) % 16 == 0;

                if (pages && entrycount - 1 != i)
                {
                    AddButton(200, 325, 4005, 4006, 0, GumpButtonType.Page, page + 1);
                    AddHtmlLocalized(235, 326, 150, 20, 1011066, EntryColor, false, false); // Next page
                    page++;
                    AddPage(page);
                    yy = 0;
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (Checked == null)
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
                        Checked.Clear();

                        for (int i = 0; i < SourceBook.Entries.Count; i++)
                        {
                            Checked.Add(SourceBook.Entries[i]);
                        }

                        if (!from.HasGump(typeof(PenOfWisdomGump)))
                            from.SendGump(new PenOfWisdomGump(from, Pen, SourceBook, CopyBook, Checked));

                        break;
                    }
                case 22: // OK
                    {
                        if (MarkScrollAmount < Checked.Count || RuneAmount < Checked.Count)
                        {
                            from.SendLocalizedMessage(1115364); // You don't have enough recall runes and Mark scrolls to do that.
                        }
                        else if (Blank < Checked.Count)
                        {
                            from.SendLocalizedMessage(1115330); // The destination runebook doesn't have enough space.
                        }
                        else if (!SourceBook.IsChildOf(from.Backpack) && SourceBook.Movable || !CopyBook.IsChildOf(from.Backpack))
                        {
                            from.SendLocalizedMessage(1115329); // Runebooks you wish to copy must be in your backpack.
                        }
                        else
                        {
                            foreach (RunebookEntry entry in Checked)
                            {
                                CopyBook.Entries.Add(entry);
                            }

                            Container bp = from.Backpack;

                            bp.ConsumeTotal(typeof(MarkScroll), Checked.Count, true);
                            bp.ConsumeTotal(typeof(RecallRune), Checked.Count, true);
                            Pen.UsesRemaining--;

                            if (Pen.UsesRemaining <= 0)
                            {
                                Pen.Delete();
                                from.SendLocalizedMessage(1115366); // The pen's magical power is consumed and it crumbles to dust.
                            }
                            else
                            {
                                from.SendLocalizedMessage(1115331); // The Pen magically marks runes and binds them to the runebook.
                            }
                        }

                        break;
                    }
                default:
                    {
                        int index = info.ButtonID;

                        if (Checked.Contains(SourceBook.Entries[index]))
                        {
                            Checked.Remove(SourceBook.Entries[index]);
                        }
                        else
                        {
                            Checked.Add(SourceBook.Entries[index]);
                        }

                        if (!from.HasGump(typeof(PenOfWisdomGump)))
                            from.SendGump(new PenOfWisdomGump(from, Pen, SourceBook, CopyBook, Checked));

                        break;
                    }
            }
        }
    }
}
