using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Chivalry;
using Server.Targeting;
using Server.Prompts;

namespace Server.Gumps
{
    public class InternalRunebookGump : Gump
    {
        private InternalRunebook m_Book;
        public InternalRunebook Book { get { return m_Book; } }

        private int m_BookNum;
        public int BookNum { get { return m_BookNum; } }

        private MasterRunebook m_MasterBook;
        public MasterRunebook MasterBook { get { return m_MasterBook; } }

        public int GetMapHue(Map map)
        {
            if (map == Map.Trammel)
                return 10;
            else if (map == Map.Felucca)
                return 81;
            else if (map == Map.Ilshenar)
                return 1102;
            else if (map == Map.Malas)
                return 1102;
            else if (map == Map.Tokuno)
                return 1154;

            return 0;
        }

        public string GetName(string name)
        {
            if (name == null || (name = name.Trim()).Length <= 0)
                return "(nondescript)";

            return name;
        }

        private void AddBackground()
        {
            AddPage(0);

            // Background image
            AddImage(100, 10, 2200);

            // Two separators
            for (int i = 0; i < 2; ++i)
            {
                int xOffset = 125 + (i * 165);

                AddImage(xOffset, 50, 57);
                xOffset += 20;

                for (int j = 0; j < 6; ++j, xOffset += 15)
                    AddImage(xOffset, 50, 58);

                AddImage(xOffset - 5, 50, 59);
            }

            // First four page buttons
            for (int i = 0, xOffset = 130, gumpID = 2225; i < 4; ++i, xOffset += 35, ++gumpID)
                AddButton(xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 2 + i);

            // Next four page buttons
            for (int i = 0, xOffset = 300, gumpID = 2229; i < 4; ++i, xOffset += 35, ++gumpID)
                AddButton(xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 6 + i);

            // Charges
            AddHtmlLocalized(132, 32, 80, 18, 1011296, false, false); // Charges:
            AddHtml(185, 32, 50, 18, string.Format("{0}/{1}", m_Book.CurCharges.ToString(), m_Book.MaxCharges.ToString()), false, false);

            // Max charges
            //AddHtmlLocalized(300, 40, 100, 18, 1011297, false, false); // Max Charges:
            //AddHtml(400, 40, 30, 18, m_Book.MaxCharges.ToString(), false, false);
        }

        private void AddIndex()
        {
            // Index 
            AddPage(1);

            // Scroll Button (for adding Recall Scrolls)
            //AddButton(236, 39, 5402, 5402, 97, GumpButtonType.Reply, 0);
            AddButton(236, 34, 2510, 2510, 197, GumpButtonType.Reply, 0);
            AddItem(221, 32, 8012);

            // Remove Button (for dropping Runebook)
            AddItem(350, 30, 0x22C5, 0x461);
            AddButton(380, 36, 2181, 2181, 196, GumpButtonType.Reply, 0);

            // List of entries
            List<RunebookEntry> entries = m_Book.Entries;

            for (int i = 0; i < 16; ++i)
            {
                string desc;
                int hue;

                if (i < entries.Count)
                {
                    desc = GetName(((RunebookEntry)entries[i]).Description);
                    hue = GetMapHue(((RunebookEntry)entries[i]).Map);
                }
                else
                {
                    desc = "Empty";
                    hue = 0;
                }

                // Use charge button
                AddButton(130 + ((i / 8) * 160), 65 + ((i % 8) * 15), 2103, 2104, 2 + (i * 6) + 0, GumpButtonType.Reply, 0);

                if (i == entries.Count)
                {
                    // (Button for adding Recall Rune for new Location)
                    AddButton(145 + ((i / 8) * 160), 62 + ((i % 8) * 15), 2510, 2510, 198, GumpButtonType.Reply, 0); //or 30087 or 4202
                    AddItem(129 + ((i / 8) * 160), 65 + ((i % 8) * 15), 7959);
                }
                else
                    // Description label
                    AddLabelCropped(145 + ((i / 8) * 160), 60 + ((i % 8) * 15), 115, 17, hue, desc);
            }

            // Turn page button
            //AddButton(393, 14, 2206, 2206, 0, GumpButtonType.Page, 2);
        }

        private void AddDetails(int index, int half)
        {
            // Use charge button
            AddButton(130 + (half * 160), 65, 2103, 2104, 2 + (index * 6) + 0, GumpButtonType.Reply, 0);

            string desc;
            int hue;

            if (index < m_Book.Entries.Count)
            {
                RunebookEntry e = (RunebookEntry)m_Book.Entries[index];

                desc = GetName(e.Description);
                hue = GetMapHue(e.Map);

                // Location labels
                int xLong = 0, yLat = 0;
                int xMins = 0, yMins = 0;
                bool xEast = false, ySouth = false;

                if (Sextant.Format(e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                {
                    AddLabel(135 + (half * 160), 80, 0, String.Format("{0}° {1}'{2}", yLat, yMins, ySouth ? "S" : "N"));
                    AddLabel(135 + (half * 160), 95, 0, String.Format("{0}° {1}'{2}", xLong, xMins, xEast ? "E" : "W"));
                }

                // Drop rune button
                AddButton(135 + (half * 160), 115, 2437, 2438, 2 + (index * 6) + 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(150 + (half * 160), 115, 100, 18, 1011298, false, false); // Drop rune

                if (Core.AOS)
                {
                    AddButton(135 + (half * 160), 140, 2103, 2104, 2 + (index * 6) + 3, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(150 + (half * 160), 136, 110, 20, 1062722, false, false); // Recall

                    AddButton(135 + (half * 160), 158, 2103, 2104, 2 + (index * 6) + 4, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(150 + (half * 160), 154, 110, 20, 1062723, false, false); // Gate Travel

                    AddButton(135 + (half * 160), 176, 2103, 2104, 2 + (index * 6) + 5, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(150 + (half * 160), 172, 110, 20, 1062724, false, false); // Sacred Journey
                }
                else
                {
                    // Recall button
                    AddButton(135 + (half * 160), 140, 2271, 2271, 2 + (index * 6) + 3, GumpButtonType.Reply, 0);

                    // Gate button
                    AddButton(205 + (half * 160), 140, 2291, 2291, 2 + (index * 6) + 4, GumpButtonType.Reply, 0);
                }
            }
            else
            {
                desc = "Empty";
                hue = 0;
            }

            // Description label
            AddLabelCropped(145 + (half * 160), 60, 115, 17, hue, desc);
        }

        public InternalRunebookGump(Mobile from, InternalRunebook book, MasterRunebook masterbook, int booknum)
            : base(150, 200)
        {
            m_Book = book;
            m_MasterBook = masterbook;
            m_BookNum = booknum;

            AddBackground();

            AddLabel(137, 12, 0, m_Book.Name);
            AddLabel(312, 32, 0, "Return");
            AddButton(288, 34, 2223, 2223, 199, GumpButtonType.Reply, 0);

            if (booknum > 0)
            {
                AddButton(283, 14, 9766, 9767, booknum + 199, GumpButtonType.Reply, 0);
                AddLabel(303, 12, 0, string.Format("{0}", ((int)(booknum)).ToString()));
            }
            if (booknum < 50)
            {
                AddButton(407, 14, 9762, 9763, booknum + 201, GumpButtonType.Reply, 0);
                AddLabel(387, 12, 0, string.Format("{0}", ((int)(booknum + 2)).ToString()));
            }

            AddIndex();

            for (int page = 0; page < 8; ++page)
            {
                AddPage(2 + page);

                //AddButton(125, 14, 2205, 2205, 0, GumpButtonType.Page, 1 + page);

                //if (page < 7)
                //    AddButton(393, 14, 2206, 2206, 0, GumpButtonType.Page, 3 + page);

                for (int half = 0; half < 2; ++half)
                    AddDetails((page * 2) + half, half);
            }
        }

        public static bool HasSpell(Mobile from, int spellID)
        {
            Spellbook book = Spellbook.Find(from, spellID);

            return (book != null && book.HasSpell(spellID));
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (m_MasterBook.Deleted || !from.InRange(m_MasterBook.GetWorldLocation(), 1) || !Multis.DesignContext.Check(from))
                return;

            int buttonID = info.ButtonID;

            if (buttonID == 196)
            {
                from.CloseGump(typeof(InternalRunebookGump));
                from.SendGump(new OKTargetGump("Remove Runebook?", 16777215,
                    "Are you sure you want to remove this Runebook?",
                    16777215, 300, 300, new GenericOKCallback(RemoveRunebook_Callback), m_MasterBook, m_Book, m_BookNum));
            }
            else
                if (buttonID == 197)
                {
                    from.CloseGump(typeof(InternalRunebookGump));
                    from.SendGump(new InternalRunebookGump(from, m_Book, m_MasterBook, m_BookNum));
                    from.SendMessage("Target a Recall Scroll to add charges.");
                    from.Target = new InternalTarget(m_Book, m_MasterBook, m_BookNum);
                }
                else
                    if (buttonID == 198)
                    {
                        from.CloseGump(typeof(InternalRunebookGump));
                        from.SendGump(new InternalRunebookGump(from, m_Book, m_MasterBook, m_BookNum));
                        from.SendMessage("Target a Recall Rune to add a destination.");
                        from.Target = new InternalRuneTarget(m_Book, m_MasterBook, m_BookNum);
                    }
                    else
                        if (buttonID == 199)
                        {
                            from.CloseGump(typeof(InternalRunebookGump));
                            from.SendGump(new MasterRunebookGump(from, m_MasterBook));
                        }
                        else

                            if (buttonID > 199)
                            {
                                from.CloseGump(typeof(InternalRunebookGump));
                                from.SendGump(new InternalRunebookGump(from, m_MasterBook.Books[buttonID - 200], m_MasterBook, buttonID - 200));
                            }
                            else
                            {
                                buttonID -= 2;

                                int index = buttonID / 6;
                                int type = buttonID % 6;

                                if (index >= 0 && index < m_Book.Entries.Count)
                                {
                                    RunebookEntry e = (RunebookEntry)m_Book.Entries[index];

                                    switch (type)
                                    {
                                        case 0: // Use charges
                                            {
                                                if (m_Book.CurCharges <= 0)
                                                {
                                                    from.CloseGump(typeof(InternalRunebookGump));
                                                    from.SendGump(new InternalRunebookGump(from, m_Book, m_MasterBook, m_BookNum));

                                                    from.SendLocalizedMessage(502412); // There are no charges left on that item.
                                                }
                                                else
                                                {
                                                    int xLong = 0, yLat = 0;
                                                    int xMins = 0, yMins = 0;
                                                    bool xEast = false, ySouth = false;

                                                    if (Sextant.Format(e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                                                    {
                                                        string location = String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                                                        from.SendMessage(location);
                                                    }

                                                    m_Book.OnTravel();

                                                    new RecallSpell(from, m_Book, e, m_Book).Cast();
                                                }

                                                break;
                                            }
                                        case 1: // Drop rune
                                            {
                                                if (m_Book.CheckAccess(from))
                                                {
                                                    m_Book.DropRune(from, e, index);

                                                    from.CloseGump(typeof(InternalRunebookGump));
                                                    from.SendGump(new InternalRunebookGump(from, m_Book, m_MasterBook, m_BookNum));
                                                }
                                                else
                                                {
                                                    from.SendLocalizedMessage(502413); // That cannot be done while the book is locked down.
                                                }

                                                break;
                                            }
                                        case 3: // Recall
                                            {
                                                if (HasSpell(from, 31))
                                                {
                                                    int xLong = 0, yLat = 0;
                                                    int xMins = 0, yMins = 0;
                                                    bool xEast = false, ySouth = false;

                                                    if (Sextant.Format(e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                                                    {
                                                        string location = String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                                                        from.SendMessage(location);
                                                    }

                                                    m_Book.OnTravel();
                                                    new RecallSpell(from, null, e, null).Cast();
                                                }
                                                else
                                                {
                                                    from.SendLocalizedMessage(500015); // You do not have that spell!
                                                }

                                                break;
                                            }
                                        case 4: // Gate
                                            {
                                                if (HasSpell(from, 51))
                                                {
                                                    int xLong = 0, yLat = 0;
                                                    int xMins = 0, yMins = 0;
                                                    bool xEast = false, ySouth = false;

                                                    if (Sextant.Format(e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                                                    {
                                                        string location = String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                                                        from.SendMessage(location);
                                                    }

                                                    m_Book.OnTravel();
                                                    new GateTravelSpell(from, null, e).Cast();
                                                }
                                                else
                                                {
                                                    from.SendLocalizedMessage(500015); // You do not have that spell!
                                                }

                                                break;
                                            }
                                        case 5: // Sacred Journey
                                            {
                                                if (Core.AOS)
                                                {
                                                    if (HasSpell(from, 209))
                                                    {
                                                        int xLong = 0, yLat = 0;
                                                        int xMins = 0, yMins = 0;
                                                        bool xEast = false, ySouth = false;

                                                        if (Sextant.Format(e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                                                        {
                                                            string location = String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                                                            from.SendMessage(location);
                                                        }

                                                        m_Book.OnTravel();
                                                        new SacredJourneySpell(from, null, e, null).Cast();
                                                    }
                                                    else
                                                    {
                                                        from.SendLocalizedMessage(500015); // You do not have that spell!
                                                    }
                                                }

                                                break;
                                            }
                                    }
                                }
                            }
        }

        private class InternalTarget : Target
        {
            private InternalRunebook m_RuneBook;
            private MasterRunebook m_Master;
            private int m_Booknum;

            public InternalTarget(InternalRunebook book, MasterRunebook master, int booknum)
                : base(3, false, TargetFlags.None)
            {
                m_RuneBook = book;
                m_Master = master;
                m_Booknum = booknum;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is RecallScroll)
                {
                    Item scrolls = targeted as Item;
                    if (m_RuneBook.CurCharges < m_RuneBook.MaxCharges)
                    {
                        from.Send(new PlaySound(0x249, from.Location));

                        int amount = scrolls.Amount;

                        if (amount > (m_RuneBook.MaxCharges - m_RuneBook.CurCharges))
                        {
                            scrolls.Consume(m_RuneBook.MaxCharges - m_RuneBook.CurCharges);
                            m_RuneBook.CurCharges = m_RuneBook.MaxCharges;
                        }
                        else
                        {
                            m_RuneBook.CurCharges += amount;
                            scrolls.Delete();
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(502410); // This book already has the maximum amount of charges.
                    }
                }
                else
                    from.SendMessage("That is not a Recall Scroll.");
                from.CloseGump(typeof(InternalRunebookGump));
                from.SendGump(new InternalRunebookGump(from, m_RuneBook, m_Master, m_Booknum));
            }
        }

        private class InternalRuneTarget : Target
        {
            private InternalRunebook m_RuneBook;
            private MasterRunebook m_Master;
            private int m_Booknum;

            public InternalRuneTarget(InternalRunebook book, MasterRunebook master, int booknum)
                : base(3, false, TargetFlags.None)
            {
                m_RuneBook = book;
                m_Master = master;
                m_Booknum = booknum;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is RecallRune)
                {
                    if (m_RuneBook.Entries.Count < 16)
                    {
                        RecallRune rune = (RecallRune)targeted;

                        if (rune.Marked && rune.TargetMap != null)
                        {
                            m_RuneBook.Entries.Add(new RunebookEntry(rune.Target, rune.TargetMap, rune.Description, rune.House));

                            rune.Delete();

                            from.Send(new PlaySound(0x42, from.Location));

                            string desc = rune.Description;

                            if (desc == null || (desc = desc.Trim()).Length == 0)
                                desc = "(nondescript)";

                            from.SendMessage(desc);
                        }
                        else
                        {
                            from.SendLocalizedMessage(502409); // This rune does not have a marked location.
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(502401); // This runebook is full.
                    }
                }
                else
                    if (targeted == from && from.AccessLevel >= AccessLevel.GameMaster)
                    {
                        from.SendGump(new OKTargetGump("No Recall Rune?", 16777215,
                            "Do you want to Target your location instead of a Recall Rune?",
                            16777215, 300, 300, new GenericOKCallback(TargetPoint3D_Callback), m_Master, m_RuneBook, m_Booknum));
                        return;
                    }
                    from.SendMessage("That is not a Recall Rune.");
                from.CloseGump(typeof(InternalRunebookGump));
                from.SendGump(new InternalRunebookGump(from, m_RuneBook, m_Master, m_Booknum));
            }
        }

        public static void RemoveRunebook_Callback(Mobile from, bool okay, MasterRunebook master, InternalRunebook book, int id)
        {
            if (okay)
            {
                Container pack = from.Backpack;
                if (pack == null || pack.Deleted)
                {
                    from.SendMessage("Unable to find a backpack in which to place a new Runebook.");
                    return;
                }
                int count = book.Entries.Count;
                if (count > 0)
                {
                    if (from.AccessLevel < AccessLevel.GameMaster)
                    {
                        if (from.Backpack.ConsumeTotal(new Type[] { typeof(BlankScroll), typeof(RecallScroll), typeof(GateTravelScroll) },
                            new int[] { 10, 1, 1 }) >= 0)
                        {
                            from.SendMessage("You do not have the materials needed to create the runebook.");
                            return;
                        }
                        if (from.CheckSkill(SkillName.Inscribe, 100, 120))
                        {
                            from.SendMessage("You failed to extract the book. Some materials were lost.");
                            return;
                        }
                    }
                    
                    Runebook runebook = new Runebook(book.MaxCharges);
                    for (int x = 0; x < count; x++)
                    {
                        RunebookEntry rbe = new RunebookEntry(
                            ((RunebookEntry)book.Entries[0]).Location, ((RunebookEntry)book.Entries[0]).Map,
                            ((RunebookEntry)book.Entries[0]).Description, ((RunebookEntry)book.Entries[0]).House);
                        runebook.Entries.Add(rbe);
                        book.Entries.RemoveAt(0);
                    }
                    runebook.CurCharges = book.CurCharges;
                    runebook.Name = book.Name;
                    book.Name = string.Format("Book #{0}", ((int)(id + 1)).ToString());
                    pack.DropItem(runebook);
                    from.SendMessage("Runebook extracted. Some materials were used.");
                }
                else
                {
                    from.SendMessage("You cannot remove an empty Runebook.");
                }
                from.CloseGump(typeof(InternalRunebookGump));
                from.SendGump(new InternalRunebookGump(from, book, master, id));
            }
            else
            {
                from.CloseGump(typeof(InternalRunebookGump));
                from.SendGump(new InternalRunebookGump(from, book, master, id));
            }
        }

        public static void TargetPoint3D_Callback(Mobile from, bool okay, MasterRunebook master, InternalRunebook book, int id)
        {
            if (okay)
            {
                if (book.Entries.Count < 16)
                {
                    book.Entries.Add(new RunebookEntry(from.Location, from.Map, "", null));
                    from.SendMessage("Enter the description for this location.");
                    from.Prompt = new DescriptionPrompt(master, book, id);
                }
                else
                {
                    from.SendLocalizedMessage(502401); // This runebook is full.
                    from.CloseGump(typeof(InternalRunebookGump));
                    from.SendGump(new InternalRunebookGump(from, book, master, id));
                }
            }
            else
            {
                from.CloseGump(typeof(InternalRunebookGump));
                from.SendGump(new InternalRunebookGump(from, book, master, id));
            }
        }

        private class DescriptionPrompt : Prompt
        {
            MasterRunebook m_Master;
            InternalRunebook m_Book;
            int m_Id;

            public DescriptionPrompt(MasterRunebook master, InternalRunebook book, int id)
            {
                m_Master = master;
                m_Book = book;
                m_Id = id;
            }

            public override void OnResponse(Mobile from, string text)
            {
                int index = m_Book.Entries.Count - 1;
                m_Book.Entries.RemoveAt(index);
                m_Book.Entries.Add(new RunebookEntry(from.Location, from.Map, text, null));
                from.SendMessage(text);
                from.CloseGump(typeof(InternalRunebookGump));
                from.SendGump(new InternalRunebookGump(from, m_Book, m_Master, m_Id));
            }
        }
    }
}