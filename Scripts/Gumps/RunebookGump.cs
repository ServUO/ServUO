using System;
using System.Collections.Generic;
using Server.Items;
using Server.Network;
using Server.Prompts;
using Server.Spells.Chivalry;
using Server.Spells.Fourth;
using Server.Spells.Seventh;

namespace Server.Gumps
{
    public class RunebookGump : Gump
    {
        private readonly Runebook m_Book;
        public RunebookGump(Mobile from, Runebook book)
            : base(150, 200)
        {
            m_Book = book;

            AddBackground();
            AddIndex();

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

        public Runebook Book
        {
            get
            {
                return m_Book;
            }
        }
        public static bool HasSpell(Mobile from, int spellID)
        {
            Spellbook book = Spellbook.Find(from, spellID);

            return (book != null && book.HasSpell(spellID));
        }

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

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (m_Book.Deleted || !from.InRange(m_Book.GetWorldLocation(), (Core.ML ? 3 : 1)) || !Multis.DesignContext.Check(from))
            {
                m_Book.Openers.Remove(from);
                return;
            }

            int buttonID = info.ButtonID;

            if (buttonID == 0) // Close
            {
                m_Book.Openers.Remove(from);
            }
            else if (buttonID == 1) // Rename book
            {
                if (!m_Book.IsLockedDown || from.AccessLevel >= AccessLevel.GameMaster)
                {
                    from.SendLocalizedMessage(502414); // Please enter a title for the runebook:
                    from.Prompt = new InternalPrompt(m_Book);
                }
                else
                {
                    m_Book.Openers.Remove(from);
					
                    from.SendLocalizedMessage(502413, null, 0x35); // That cannot be done while the book is locked down.
                }
            }
            else
            {
                buttonID -= 2;

                int index = buttonID / 6;
                int type = buttonID % 6;

                if (index >= 0 && index < m_Book.Entries.Count)
                {
                    RunebookEntry e = (RunebookEntry)m_Book.Entries[index];

                    switch ( type )
                    {
                        case 0: // Use charges
                            {
                                if (m_Book.CurCharges <= 0)
                                {
                                    from.CloseGump(typeof(RunebookGump));
                                    from.SendGump(new RunebookGump(from, m_Book));

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
								
                                    m_Book.Openers.Remove(from);
                                }

                                break;
                            }
                        case 1: // Drop rune
                            {
                                if (!m_Book.IsLockedDown || from.AccessLevel >= AccessLevel.GameMaster)
                                {
                                    m_Book.DropRune(from, e, index);

                                    from.CloseGump(typeof(RunebookGump));
                                    if (!Core.ML)
                                        from.SendGump(new RunebookGump(from, m_Book));
                                }
                                else
                                {
                                    m_Book.Openers.Remove(from);
								
                                    from.SendLocalizedMessage(502413, null, 0x35); // That cannot be done while the book is locked down.
                                }

                                break;
                            }
                        case 2: // Set default
                            {
                                if (m_Book.CheckAccess(from))
                                {
                                    m_Book.Default = e;

                                    from.CloseGump(typeof(RunebookGump));
                                    from.SendGump(new RunebookGump(from, m_Book));

                                    from.SendLocalizedMessage(502417); // New default location set.
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
							
                                m_Book.Openers.Remove(from);

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
							
                                m_Book.Openers.Remove(from);

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
							
                                m_Book.Openers.Remove(from);

                                break;
                            }
                    }
                }
                else
                    m_Book.Openers.Remove(from);
            }
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
            AddHtmlLocalized(140, 40, 80, 18, 1011296, false, false); // Charges:
            AddHtml(220, 40, 30, 18, m_Book.CurCharges.ToString(), false, false);

            // Max charges
            AddHtmlLocalized(300, 40, 100, 18, 1011297, false, false); // Max Charges:
            AddHtml(400, 40, 30, 18, m_Book.MaxCharges.ToString(), false, false);
        }

        private void AddIndex()
        {
            // Index
            AddPage(1);

            // Rename button
            AddButton(125, 15, 2472, 2473, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(158, 22, 100, 18, 1011299, false, false); // Rename book

            // List of entries
            List<RunebookEntry> entries = m_Book.Entries;

            for (int i = 0; i < 16; ++i)
            {
                string desc;
                int hue;

                if (i < entries.Count)
                {
                    desc = GetName(entries[i].Description);
                    hue = GetMapHue(entries[i].Map);
                }
                else
                {
                    desc = "Empty";
                    hue = 0;
                }

                // Use charge button
                AddButton(130 + ((i / 8) * 160), 65 + ((i % 8) * 15), 2103, 2104, 2 + (i * 6) + 0, GumpButtonType.Reply, 0);

                // Description label
                AddLabelCropped(145 + ((i / 8) * 160), 60 + ((i % 8) * 15), 115, 17, hue, desc);
            }

            // Turn page button
            AddButton(393, 14, 2206, 2206, 0, GumpButtonType.Page, 2);
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

                // Set as default button
                int defButtonID = e != m_Book.Default ? 2361 : 2360;
				
                AddButton(160 + (half * 140), 20, defButtonID, defButtonID, 2 + (index * 6) + 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(175 + (half * 140), 15, 100, 18, 1011300, false, false); // Set default

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

        private class InternalPrompt : Prompt
        {
            public override int MessageCliloc { get { return 502414; } }
            private readonly Runebook m_Book;
            public InternalPrompt(Runebook book)
            {
                m_Book = book;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (m_Book.Deleted || !from.InRange(m_Book.GetWorldLocation(), (Core.ML ? 3 : 1)))
                    return;

                if (m_Book.CheckAccess(from))
                {
                    m_Book.Description = Utility.FixHtml(text.Trim());

                    from.CloseGump(typeof(RunebookGump));
                    from.SendGump(new RunebookGump(from, m_Book));

                    from.SendMessage("The book's title has been changed.");
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

                if (!m_Book.Deleted && from.InRange(m_Book.GetWorldLocation(), (Core.ML ? 3 : 1)))
                {
                    from.CloseGump(typeof(RunebookGump));
                    from.SendGump(new RunebookGump(from, m_Book));
                }
            }
        }
    }
}