using Server.Items;
using Server.Network;
using Server.Prompts;
using Server.Spells.Chivalry;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using System.Collections.Generic;
using System.Linq;

namespace Server.Gumps
{
    public class RunebookGump : Gump
    {
        public void PrecompileStringTable()
        {
            Intern("Charges", true);              // 0
            Intern("Max Charges", true);          // 1
                                                  // Next 16 entries are Location Values
            for (int i = 0; i < 16; ++i)
            {
                string desc;
                if (i < Book.Entries.Count)
                    desc = GetName(Book.Entries[i].Description);
                else
                    desc = "Empty";

                Intern(desc, false);
            }

            Intern(Book.CurCharges.ToString(), false);
            Intern(Book.MaxCharges.ToString(), false);

            Intern("Drop Rune", true);
            Intern("Rename Book", true);
            Intern("Set Default", true);

            for (int i = 0; i < 16; ++i)
            {
                if (i < Book.Entries.Count)
                {
                    RunebookEntry e = Book.Entries[i];

                    Intern(GetLocation(e), false);
                }
                else
                {
                    Intern("Nowhere", false);
                }
            }
        }

        public Runebook Book { get; }

        public static int GetMapHue(Map map)
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
            else if (map == Map.TerMur)
                return 1645;

            return 0;
        }

        public static string GetName(string name)
        {
            if (name == null || (name = name.Trim()).Length <= 0)
                return "(indescript)";

            return name;
        }

        public static string GetLocation(RunebookEntry e)
        {
            string loc;

            // Location labels
            int xLong = 0, yLat = 0;
            int xMins = 0, yMins = 0;
            bool xEast = false, ySouth = false;

            if (e.Type == RecallRuneType.Ship)
            {
                loc = string.Format("Aboard {0}", e.Description.Substring(e.Description.IndexOf(",") + 2));
            }
            else if (Sextant.Format(e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
            {
                loc = string.Format("{0}o {1}'{2}, {3}o {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
            }
            else
            {
                loc = "Nowhere";
            }

            return loc;
        }

        private void AddBackground()
        {
            AddPage(0);

            // Background image
            AddImage(100, 10, 2200);

            // Two seperators
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
            AddHtmlIntern(140, 40, 80, 18, 0, false, false);    // Charges:	
            AddHtmlIntern(300, 40, 100, 18, 1, false, false);   // Max Charges:	

            AddHtmlIntern(220, 40, 30, 18, 18, false, false);   // Charges
            AddHtmlIntern(400, 40, 30, 18, 19, false, false);   // Max charges
        }

        private void AddIndex()
        {
            // Index
            AddPage(1);

            // Rename button
            AddButton(125, 15, 2472, 2473, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(158, 22, 100, 18, 1011299, false, false); // Rename book

            // List of entries
            List<RunebookEntry> entries = Book.Entries;

            for (int i = 0; i < 16; ++i)
            {
                int hue;

                if (i < entries.Count)
                {
                    hue = GetMapHue(entries[i].Map);
                }
                else
                {
                    hue = 0;
                }

                // Use charge button
                AddButton(130 + ((i / 8) * 160), 65 + ((i % 8) * 15), 2103, 2104, 10 + i, GumpButtonType.Reply, 0);

                // Description label
                AddLabelCroppedIntern(145 + ((i / 8) * 160), 60 + ((i % 8) * 15), 115, 17, hue, i + 2);
            }

            if (entries.Count != 0)
            {
                // Turn page button
                AddButton(393, 14, 2206, 2206, 0, GumpButtonType.Page, 2);
            }
        }

        private void AddDetails(int index, int half)
        {
            List<RunebookEntry> entries = Book.Entries;

            if (entries.Count != 0)
            {
                // Use charge button
                AddButton(130 + (half * 160), 65, 2103, 2104, 10 + index, GumpButtonType.Reply, 0);

                if (index < 16)
                {
                    if (Book.Entries.ElementAtOrDefault(index) != null)
                    {
                        RunebookEntry e = Book.Entries[index];

                        // Description label
                        AddLabelCroppedIntern(145 + (half * 160), 60, 115, 17, GetMapHue(e.Map), index + 2);

                        // Location label
                        AddHtmlIntern(135 + (half * 160), 80, 130, 38, index + 23, false, false);

                        // Drop rune button
                        AddButton(135 + (half * 160), 115, 2437, 2438, 200 + index, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(150 + (half * 160), 115, 100, 18, 1011298, false, false); // Drop rune

                        // Set as default button
                        int defButtonID = e != Book.Default ? 2361 : 2360;

                        AddButton(160 + (half * 140), 20, defButtonID, defButtonID, 300 + index, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(175 + (half * 140), 15, 100, 18, 1011300, false, false); // Set default
                    }
                    else
                    {
                        AddLabelIntern(145 + (half * 160), 60, 0, index + 2);
                    }

                    AddButton(135 + (half * 160), 140, 2103, 2104, 50 + index, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(150 + (half * 160), 136, 110, 20, 1062722, false, false); // Recall

                    AddButton(135 + (half * 160), 158, 2103, 2104, 100 + index, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(150 + (half * 160), 154, 110, 20, 1062723, false, false); // Gate Travel

                    AddButton(135 + (half * 160), 176, 2103, 2104, 75 + index, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(150 + (half * 160), 172, 110, 20, 1062724, false, false); // Sacred Journey
                }
            }
        }

        public RunebookGump(Mobile from, Runebook book)
            : base(150, 200)
        {
            TypeID = 0x59;
            Book = book;

            PrecompileStringTable();
            AddBackground();
            AddIndex();

            if (Book.Entries.Count != 0)
            {
                for (int page = 0; page < 8; ++page)
                {
                    AddPage(2 + page);

                    AddButton(125, 14, 2205, 2205, 0, GumpButtonType.Page, 1 + page);

                    if (page < 7)
                        AddButton(393, 14, 2206, 2206, 0, GumpButtonType.Page, 3 + page);

                    for (int half = 0; half < 2; ++half)
                        AddDetails((page * 2) + half, half);
                }
            }
        }

        public static bool HasSpell(Mobile from, int spellID)
        {
            Spellbook book = Spellbook.Find(from, spellID);

            return (book != null && book.HasSpell(spellID));
        }

        private class InternalPrompt : Prompt
        {
            public override int MessageCliloc => 502414;  // Please enter a title for the runebook:
            private readonly Runebook m_Book;

            public InternalPrompt(Runebook book)
                : base(book)
            {
                m_Book = book;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (m_Book.Deleted || !from.InRange(m_Book.GetWorldLocation(), 3))
                    return;

                if (m_Book.CheckAccess(from))
                {
                    m_Book.Description = Utility.FixHtml(text.Trim());

                    from.CloseGump(typeof(RunebookGump));
                    from.SendGump(new RunebookGump(from, m_Book));

                    from.SendLocalizedMessage(1041531); // You have changed the title of the rune book.
                }
                else
                {
                    m_Book.Openers.Remove(from);

                    from.SendLocalizedMessage(502416); // That cannot be done while the book is locked down.
                }
            }

            public override void OnCancel(Mobile from)
            {
                from.SendLocalizedMessage(502415); // Request cancelled.

                if (!m_Book.Deleted && from.InRange(m_Book.GetWorldLocation(), 3))
                {
                    from.CloseGump(typeof(RunebookGump));
                    from.SendGump(new RunebookGump(from, m_Book));
                }
            }
        }

        public void SendLocationMessage(RunebookEntry e, Mobile from)
        {
            if (e.Type == RecallRuneType.Ship)
                return;

            int xLong = 0, yLat = 0;
            int xMins = 0, yMins = 0;
            bool xEast = false, ySouth = false;

            if (Sextant.Format(e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
            {
                from.SendAsciiMessage(string.Format("{0}o {1}'{2}, {3}o {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W"));
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (Book.Deleted || !from.InRange(Book.GetWorldLocation(), 3) || !Multis.DesignContext.Check(from))
            {
                Book.Openers.Remove(from);
                return;
            }

            int buttonID = info.ButtonID;

            if (buttonID == 0) // Close
            {
                Book.Openers.Remove(from);
            }
            else if (buttonID == 1) // Rename book
            {
                if (Book.CheckAccess(from) && Book.Movable != false)
                {
                    from.Prompt = new InternalPrompt(Book);
                }
                else
                {
                    Book.Openers.Remove(from);

                    from.SendLocalizedMessage(502413); // That cannot be done while the book is locked down.
                }
            }
            else
            {
                int index = buttonID % 25;
                int type = buttonID / 25;

                if (type == 0 || type == 1)
                    index = buttonID - 10;

                if (Book.Entries.ElementAtOrDefault(index) != null)
                {
                    if (index >= 0 && index < Book.Entries.Count)
                    {
                        RunebookEntry e = Book.Entries[index];

                        switch (type)
                        {
                            case 0:
                            case 1: // Use charges
                                {
                                    if (Book.CurCharges <= 0)
                                    {
                                        from.CloseGump(typeof(RunebookGump));
                                        from.SendGump(new RunebookGump(from, Book));

                                        from.SendLocalizedMessage(502412); // There are no charges left on that item.
                                    }
                                    else
                                    {
                                        SendLocationMessage(e, from);

                                        Book.OnTravel();
                                        new RecallSpell(from, Book, e, Book).Cast();

                                        Book.Openers.Remove(from);
                                    }

                                    break;
                                }
                            case 8: // Drop rune
                                {
                                    if (Book.CheckAccess(from) && Book.Movable != false)
                                    {
                                        Book.DropRune(from, e, index);

                                        from.CloseGump(typeof(RunebookGump));
                                        from.SendGump(new RunebookGump(from, Book));
                                    }
                                    else
                                    {
                                        Book.Openers.Remove(from);

                                        from.SendLocalizedMessage(502413); // That cannot be done while the book is locked down.
                                    }

                                    break;
                                }
                            case 12: // Set default
                                {
                                    if (Book.CheckAccess(from))
                                    {
                                        Book.Default = e;

                                        from.CloseGump(typeof(RunebookGump));
                                        from.SendGump(new RunebookGump(from, Book));

                                        from.SendLocalizedMessage(502417, "", 0x35); // New default location set.

                                        Book.Openers.Remove(from);
                                    }
                                    else
                                    {
                                        from.SendLocalizedMessage(502413); // That cannot be done while the book is locked down.
                                    }

                                    break;
                                }
                            case 2: // Recall
                                {
                                    if (HasSpell(from, 31))
                                    {
                                        SendLocationMessage(e, from);

                                        Book.OnTravel();
                                        new RecallSpell(from, null, e, null).Cast();
                                    }
                                    else
                                    {
                                        from.SendLocalizedMessage(500015); // You do not have that spell!
                                    }

                                    Book.Openers.Remove(from);

                                    break;
                                }
                            case 4: // Gate
                                {
                                    if (HasSpell(from, 51))
                                    {
                                        SendLocationMessage(e, from);

                                        Book.OnTravel();
                                        new GateTravelSpell(from, null, e).Cast();
                                    }
                                    else
                                    {
                                        from.SendLocalizedMessage(500015); // You do not have that spell!
                                    }

                                    Book.Openers.Remove(from);

                                    break;
                                }
                            case 3: // Sacred Journey
                                {
                                    if (HasSpell(from, 209))
                                    {
                                        SendLocationMessage(e, from);

                                        Book.OnTravel();
                                        new SacredJourneySpell(from, null, e, null).Cast();
                                    }
                                    else
                                    {
                                        from.SendLocalizedMessage(500015); // You do not have that spell!
                                    }

                                    Book.Openers.Remove(from);

                                    break;
                                }

                            default:
                                break;
                        }
                    }
                    else
                    {
                        Book.Openers.Remove(from);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502423); // This place in the book is empty.
                    Book.Openers.Remove(from);
                }
            }
        }
    }
}
