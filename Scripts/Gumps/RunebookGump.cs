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
            this.m_Book = book;

            this.AddBackground();
            this.AddIndex();

            for (int page = 0; page < 8; ++page)
            {
                this.AddPage(2 + page);

                this.AddButton(125, 14, 2205, 2205, 0, GumpButtonType.Page, 1 + page);

                if (page < 7)
                    this.AddButton(393, 14, 2206, 2206, 0, GumpButtonType.Page, 3 + page);

                for (int half = 0; half < 2; ++half)
                    this.AddDetails((page * 2) + half, half);
            }
        }

        public Runebook Book
        {
            get
            {
                return this.m_Book;
            }
        }
        public static bool HasSpell(Mobile from, int spellID)
        {
            Spellbook book = Spellbook.Find(from, spellID);

            return (book != null && book.HasSpell(spellID));
        }

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
                return "(indescript)";

            return name;
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (this.m_Book.Deleted || !from.InRange(this.m_Book.GetWorldLocation(), (Core.ML ? 3 : 1)) || !Multis.DesignContext.Check(from))
            {
                this.m_Book.Openers.Remove(from);
                return;
            }

            int buttonID = info.ButtonID;

            if (buttonID == 1) // Rename book
            {
                if (!this.m_Book.IsLockedDown || from.AccessLevel >= AccessLevel.GameMaster)
                {
                    from.SendLocalizedMessage(502414); // Please enter a title for the runebook:
                    from.Prompt = new InternalPrompt(this.m_Book);
                }
                else
                {
                    this.m_Book.Openers.Remove(from);
					
                    from.SendLocalizedMessage(502413, null, 0x35); // That cannot be done while the book is locked down.
                }
            }
            else
            {
                buttonID -= 2;

                int index = buttonID / 6;
                int type = buttonID % 6;

                if (index >= 0 && index < this.m_Book.Entries.Count)
                {
                    RunebookEntry e = (RunebookEntry)this.m_Book.Entries[index];

                    switch ( type )
                    {
                        case 0: // Use charges
                            {
                                if (this.m_Book.CurCharges <= 0)
                                {
                                    from.CloseGump(typeof(RunebookGump));
                                    from.SendGump(new RunebookGump(from, this.m_Book));

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

                                    this.m_Book.OnTravel();
                                    new RecallSpell(from, this.m_Book, e, this.m_Book).Cast();
								
                                    this.m_Book.Openers.Remove(from);
                                }

                                break;
                            }
                        case 1: // Drop rune
                            {
                                if (!this.m_Book.IsLockedDown || from.AccessLevel >= AccessLevel.GameMaster)
                                {
                                    this.m_Book.DropRune(from, e, index);

                                    from.CloseGump(typeof(RunebookGump));
                                    if (!Core.ML)
                                        from.SendGump(new RunebookGump(from, this.m_Book));
                                }
                                else
                                {
                                    this.m_Book.Openers.Remove(from);
								
                                    from.SendLocalizedMessage(502413, null, 0x35); // That cannot be done while the book is locked down.
                                }

                                break;
                            }
                        case 2: // Set default
                            {
                                if (this.m_Book.CheckAccess(from))
                                {
                                    this.m_Book.Default = e;

                                    from.CloseGump(typeof(RunebookGump));
                                    from.SendGump(new RunebookGump(from, this.m_Book));

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

                                    this.m_Book.OnTravel();
                                    new RecallSpell(from, null, e, null).Cast();
                                }
                                else
                                {
                                    from.SendLocalizedMessage(500015); // You do not have that spell!
                                }
							
                                this.m_Book.Openers.Remove(from);

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

                                    this.m_Book.OnTravel();
                                    new GateTravelSpell(from, null, e).Cast();
                                }
                                else
                                {
                                    from.SendLocalizedMessage(500015); // You do not have that spell!
                                }
							
                                this.m_Book.Openers.Remove(from);

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

                                        this.m_Book.OnTravel();
                                        new SacredJourneySpell(from, null, e, null).Cast();
                                    }
                                    else
                                    {
                                        from.SendLocalizedMessage(500015); // You do not have that spell!
                                    }
                                }
							
                                this.m_Book.Openers.Remove(from);

                                break;
                            }
                    }
                }
                else
                    this.m_Book.Openers.Remove(from);
            }
        }

        private void AddBackground()
        {
            this.AddPage(0);

            // Background image
            this.AddImage(100, 10, 2200);

            // Two separators
            for (int i = 0; i < 2; ++i)
            {
                int xOffset = 125 + (i * 165);

                this.AddImage(xOffset, 50, 57);
                xOffset += 20;

                for (int j = 0; j < 6; ++j, xOffset += 15)
                    this.AddImage(xOffset, 50, 58);

                this.AddImage(xOffset - 5, 50, 59);
            }

            // First four page buttons
            for (int i = 0, xOffset = 130, gumpID = 2225; i < 4; ++i, xOffset += 35, ++gumpID)
                this.AddButton(xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 2 + i);

            // Next four page buttons
            for (int i = 0, xOffset = 300, gumpID = 2229; i < 4; ++i, xOffset += 35, ++gumpID)
                this.AddButton(xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 6 + i);

            // Charges
            this.AddHtmlLocalized(140, 40, 80, 18, 1011296, false, false); // Charges:
            this.AddHtml(220, 40, 30, 18, this.m_Book.CurCharges.ToString(), false, false);

            // Max charges
            this.AddHtmlLocalized(300, 40, 100, 18, 1011297, false, false); // Max Charges:
            this.AddHtml(400, 40, 30, 18, this.m_Book.MaxCharges.ToString(), false, false);
        }

        private void AddIndex()
        {
            // Index
            this.AddPage(1);

            // Rename button
            this.AddButton(125, 15, 2472, 2473, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(158, 22, 100, 18, 1011299, false, false); // Rename book

            // List of entries
            List<RunebookEntry> entries = this.m_Book.Entries;

            for (int i = 0; i < 16; ++i)
            {
                string desc;
                int hue;

                if (i < entries.Count)
                {
                    desc = this.GetName(entries[i].Description);
                    hue = this.GetMapHue(entries[i].Map);
                }
                else
                {
                    desc = "Empty";
                    hue = 0;
                }

                // Use charge button
                this.AddButton(130 + ((i / 8) * 160), 65 + ((i % 8) * 15), 2103, 2104, 2 + (i * 6) + 0, GumpButtonType.Reply, 0);

                // Description label
                this.AddLabelCropped(145 + ((i / 8) * 160), 60 + ((i % 8) * 15), 115, 17, hue, desc);
            }

            // Turn page button
            this.AddButton(393, 14, 2206, 2206, 0, GumpButtonType.Page, 2);
        }

        private void AddDetails(int index, int half)
        {
            // Use charge button
            this.AddButton(130 + (half * 160), 65, 2103, 2104, 2 + (index * 6) + 0, GumpButtonType.Reply, 0);

            string desc;
            int hue;

            if (index < this.m_Book.Entries.Count)
            {
                RunebookEntry e = (RunebookEntry)this.m_Book.Entries[index];

                desc = this.GetName(e.Description);
                hue = this.GetMapHue(e.Map);

                // Location labels
                int xLong = 0, yLat = 0;
                int xMins = 0, yMins = 0;
                bool xEast = false, ySouth = false;

                if (Sextant.Format(e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                {
                    this.AddLabel(135 + (half * 160), 80, 0, String.Format("{0}° {1}'{2}", yLat, yMins, ySouth ? "S" : "N"));
                    this.AddLabel(135 + (half * 160), 95, 0, String.Format("{0}° {1}'{2}", xLong, xMins, xEast ? "E" : "W"));
                }

                // Drop rune button
                this.AddButton(135 + (half * 160), 115, 2437, 2438, 2 + (index * 6) + 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(150 + (half * 160), 115, 100, 18, 1011298, false, false); // Drop rune

                // Set as default button
                int defButtonID = e != this.m_Book.Default ? 2361 : 2360;
				
                this.AddButton(160 + (half * 140), 20, defButtonID, defButtonID, 2 + (index * 6) + 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(175 + (half * 140), 15, 100, 18, 1011300, false, false); // Set default

                if (Core.AOS)
                {
                    this.AddButton(135 + (half * 160), 140, 2103, 2104, 2 + (index * 6) + 3, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(150 + (half * 160), 136, 110, 20, 1062722, false, false); // Recall

                    this.AddButton(135 + (half * 160), 158, 2103, 2104, 2 + (index * 6) + 4, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(150 + (half * 160), 154, 110, 20, 1062723, false, false); // Gate Travel

                    this.AddButton(135 + (half * 160), 176, 2103, 2104, 2 + (index * 6) + 5, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(150 + (half * 160), 172, 110, 20, 1062724, false, false); // Sacred Journey
                }
                else
                {
                    // Recall button
                    this.AddButton(135 + (half * 160), 140, 2271, 2271, 2 + (index * 6) + 3, GumpButtonType.Reply, 0);

                    // Gate button
                    this.AddButton(205 + (half * 160), 140, 2291, 2291, 2 + (index * 6) + 4, GumpButtonType.Reply, 0);
                }
            }
            else
            {
                desc = "Empty";
                hue = 0;
            }

            // Description label
            this.AddLabelCropped(145 + (half * 160), 60, 115, 17, hue, desc);
        }

        private class InternalPrompt : Prompt
        {
            private readonly Runebook m_Book;
            public InternalPrompt(Runebook book)
            {
                this.m_Book = book;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (this.m_Book.Deleted || !from.InRange(this.m_Book.GetWorldLocation(), (Core.ML ? 3 : 1)))
                    return;

                if (this.m_Book.CheckAccess(from))
                {
                    this.m_Book.Description = Utility.FixHtml(text.Trim());

                    from.CloseGump(typeof(RunebookGump));
                    from.SendGump(new RunebookGump(from, this.m_Book));

                    from.SendMessage("The book's title has been changed.");
                }
                else
                {
                    this.m_Book.Openers.Remove(from);
					
                    from.SendLocalizedMessage(502416); // That cannot be done while the book is locked down.
                }
            }

            public override void OnCancel(Mobile from)
            {
                from.SendLocalizedMessage(502415); // Request cancelled.

                if (!this.m_Book.Deleted && from.InRange(this.m_Book.GetWorldLocation(), (Core.ML ? 3 : 1)))
                {
                    from.CloseGump(typeof(RunebookGump));
                    from.SendGump(new RunebookGump(from, this.m_Book));
                }
            }
        }
    }
}