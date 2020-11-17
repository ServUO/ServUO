using Server.Gumps;
using Server.Mobiles;
using Server.Prompts;
using Server.Spells.Chivalry;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using System;

namespace Server.Items
{
    [Flipable(39958, 39959)]
    public class RunicAtlas : Runebook
    {
        public override int MaxEntries => 48;
        public override int LabelNumber => 1156443;  // a runic atlas

        public int Selected { get; set; }

        [Constructable]
        public RunicAtlas() : base(100, 39958)
        {
            Selected = -1;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile && (from.InRange(GetWorldLocation(), 2) || from.AccessLevel >= AccessLevel.Counselor))
            {
                if (CheckAccess(from) || from.AccessLevel >= AccessLevel.Counselor)
                {
                    if (DateTime.UtcNow < NextUse)
                    {
                        from.SendLocalizedMessage(502406); // This book needs time to recharge.
                        return;
                    }

                    from.CloseGump(typeof(RunicAtlasGump));
                    BaseGump.SendGump(new RunicAtlasGump((PlayerMobile)from, this));
                    Openers.Add(from);
                }
                else
                    from.SendLocalizedMessage(502436); // That is not accessible.
            }
        }

        public override bool HasGump(Mobile toCheck)
        {
            RunicAtlasGump bookGump = toCheck.FindGump<RunicAtlasGump>();

            if (bookGump != null && bookGump.Atlas == this)
            {
                return true;
            }

            return false;
        }

        public override void CloseGump(Mobile m)
        {
            m.CloseGump(typeof(RunicAtlasGump));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            int entries = Entries.Count;
            bool d = base.OnDragDrop(from, dropped);

            if (from is PlayerMobile && d && Entries.Count > entries)
            {
                int newPage = Math.Max(0, (Entries.Count - 1) / 16);

                RunicAtlasGump g = from.FindGump(typeof(RunicAtlasGump)) as RunicAtlasGump;

                if (g != null && g.Atlas == this)
                {
                    g.Page = newPage;
                    g.Refresh();
                }
                else
                {
                    if (g != null)
                        from.CloseGump(typeof(RunicAtlasGump));

                    g = new RunicAtlasGump((PlayerMobile)from, this)
                    {
                        Page = newPage
                    };
                    BaseGump.SendGump(g);
                }
            }

            return d;
        }

        public override int OnCraft(int quality, bool makersMark, Mobile from, Engines.Craft.CraftSystem craftSystem, Type typeRes, ITool tool, Engines.Craft.CraftItem craftItem, int resHue)
        {
            if (makersMark)
                Crafter = from;

            Quality = (BookQuality)(quality - 1);

            if (Quality == BookQuality.Exceptional)
            {
                MaxCharges = Utility.RandomList(80, 90, 100);
            }
            else
            {
                MaxCharges = 80;
            }

            return quality;
        }

        public RunicAtlas(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(Selected);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            Selected = reader.ReadInt();
        }
    }

    public class RunicAtlasGump : BaseGump
    {
        public static string ToCoordinates(Point3D location, Map map)
        {
            int xLong = 0, yLat = 0, xMins = 0, yMins = 0;
            bool xEast = false, ySouth = false;

            bool valid = Sextant.Format(location, map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth);

            return valid ? string.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W") : "Nowhere";
        }

        public RunicAtlas Atlas { get; }
        public int Selected => Atlas == null ? -1 : Atlas.Selected;
        public int Page { get; set; }

        public RunicAtlasGump(PlayerMobile pm, RunicAtlas atlas)
            : base(pm, 100, 100)
        {
            TypeID = 0x1F2;
            Atlas = atlas;
            Page = 0;
        }

        public static int GetMapHue(Map map)
        {
            if (map == Map.Trammel)
                return 0xA;
            if (map == Map.Felucca)
                return 0x51;
            if (map == Map.Malas)
                return 0x44E;
            if (map == Map.Tokuno)
                return 0x482;
            if (map == Map.TerMur)
                return 0x66D;

            return 0;
        }

        public override void AddGumpLayout()
        {
            AddImage(0, 0, 39923);

            AddHtmlLocalized(60, 9, 147, 22, 1011296, false, false); //Charges:
            AddHtml(110, 9, 97, 22, string.Format("{0} / {1}", Atlas.CurCharges, Atlas.MaxCharges), false, false);

            AddHtmlLocalized(264, 9, 144, 18, 1011299, false, false); // rename book 
            AddButton(248, 14, 2103, 2103, 1, GumpButtonType.Reply, 0);

            int startIndex = Page * 16;
            int index = 0;

            for (int i = startIndex; i < startIndex + 16; i++)
            {
                string desc;
                int hue;

                if (i < Atlas.Entries.Count)
                {
                    desc = RunebookGump.GetName(Atlas.Entries[i].Description);
                    hue = Selected == i ? 0x14B : GetMapHue(Atlas.Entries[i].Map);
                }
                else
                {
                    desc = "Empty";
                    hue = 0;
                }

                // Select Button
                AddButton(46 + ((index / 8) * 205), 55 + ((index % 8) * 20), 2103, 2104, i + 100, GumpButtonType.Reply, 0);

                // Description label
                AddLabelCropped(62 + ((index / 8) * 205), 50 + ((index % 8) * 20), 144, 18, hue, desc);

                index++;
            }

            RunebookEntry entry = null;

            if (Selected >= 0 && Selected < Atlas.Entries.Count)
            {
                entry = Atlas.Entries[Selected];
            }

            string coords = entry != null ? RunebookGump.GetLocation(entry) : "Nowhere";

            AddHtml(25, 254, 182, 18, string.Format("<center>{0}</center>", coords), false, false);

            AddHtmlLocalized(62, 290, 144, 18, 1011300, false, false); // Set default                        
            AddButton(46, 295, 2103, 2103, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(62, 310, 144, 18, 1011298, false, false); // Drop rune
            AddButton(46, 315, 2103, 2103, 3, GumpButtonType.Reply, 0);

            AddHtml(25, 348, 182, 18, string.Format("<center>{0}</center>", entry != null ? entry.Description : "Empty"), false, false);

            int hy = 284;
            int by = 289;

            AddHtmlLocalized(280, hy, 128, 18, 1077595, false, false); // Recall (Spell)
            AddButton(264, by, 2103, 2103, 4, GumpButtonType.Reply, 0);

            hy += 18;
            by += 18;

            if (Atlas.CurCharges != 0)
            {
                AddHtmlLocalized(280, hy, 128, 18, 1077594, false, false); // Recall (Charge)
                AddButton(264, by, 2103, 2103, 5, GumpButtonType.Reply, 0);

                hy += 18;
                by += 18;
            }

            if (User.Skills[SkillName.Magery].Value >= 66.0)
            {
                AddHtmlLocalized(280, hy, 128, 18, 1015214, false, false); // Gate Travel
                AddButton(264, by, 2103, 2103, 6, GumpButtonType.Reply, 0);

                hy += 18;
                by += 18;
            }

            AddHtmlLocalized(280, hy, 128, 18, 1060502, false, false); // Sacred Journey
            AddButton(264, by, 2103, 2103, 7, GumpButtonType.Reply, 0);

            if (Page < 2)
            {
                AddButton(374, 3, 2206, 2206, 1150, GumpButtonType.Reply, 0);
            }

            if (Page > 0)
            {
                AddButton(23, 5, 2205, 2205, 1151, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (Atlas.Deleted || !User.InRange(Atlas.GetWorldLocation(), 3))
                return;

            if (info.ButtonID >= 100 && info.ButtonID < 1000)
            {
                SelectEntry(info.ButtonID - 100);
            }
            else
            {
                RunebookEntry entry = null;

                if (Selected >= 0 && Selected < Atlas.Entries.Count)
                {
                    entry = Atlas.Entries[Selected];
                }

                switch (info.ButtonID)
                {
                    case 0: Atlas.Openers.Remove(User); break;
                    case 1: RenameBook(); break;
                    case 2:
                        {
                            if (entry != null)
                            {
                                SetDefault();
                            }
                            else
                            {
                                Atlas.Openers.Remove(User);
                            }
                            break;
                        }
                    case 3:
                        {
                            if (entry != null)
                            {
                                DropRune();
                            }
                            else
                            {
                                User.SendLocalizedMessage(502422); // There is no rune to be dropped.
                                Atlas.Openers.Remove(User);
                            }
                            break;
                        }
                    case 4:
                        {
                            if (entry != null)
                            {
                                RecallSpell();
                            }
                            else
                            {
                                User.SendLocalizedMessage(502423); // This place in the book is empty.
                                Atlas.Openers.Remove(User);
                            }
                            break;
                        }
                    case 5:
                        {
                            if (entry != null)
                            {
                                RecallCharge();
                            }
                            else
                            {
                                User.SendLocalizedMessage(502423); // This place in the book is empty.
                                Atlas.Openers.Remove(User);
                            }
                            break;
                        }
                    case 6:
                        {
                            if (entry != null)
                            {
                                GateTravel();
                            }
                            else
                            {
                                User.SendLocalizedMessage(502423); // This place in the book is empty.
                                Atlas.Openers.Remove(User);
                            }
                            break;
                        }
                    case 7:
                        {
                            if (entry != null)
                            {
                                SacredJourney();
                            }
                            else
                            {
                                User.SendLocalizedMessage(502423); // This place in the book is empty.
                                Atlas.Openers.Remove(User);
                            }
                            break;
                        }
                    case 1150:
                        Page++;
                        Refresh();
                        break;
                    case 1151:
                        Page--;
                        Refresh();
                        break;
                }
            }
        }

        public void RenameBook()
        {
            if (Atlas.CheckAccess(User) && Atlas.Movable || User.AccessLevel >= AccessLevel.GameMaster)
            {
                User.Prompt = new InternalPrompt(Atlas);
            }
            else
            {
                Atlas.Openers.Remove(User);
                User.SendLocalizedMessage(502413); // That cannot be done while the book is locked down.
            }
        }

        private void SelectEntry(int id)
        {
            Atlas.Selected = id;
            Refresh();
        }

        private void SetDefault()
        {
            if (Atlas.CheckAccess(User) || User.AccessLevel >= AccessLevel.GameMaster)
            {
                Atlas.DefaultIndex = Selected;
                Refresh();
                User.SendLocalizedMessage(502417, "", 0x35); // New default location set.
            }
            else
            {
                Atlas.Openers.Remove(User);
                User.SendLocalizedMessage(502413, null, 0x35); // That cannot be done while the book is locked down.
            }
        }

        private void DropRune()
        {
            if (Atlas.CheckAccess(User) && Atlas.Movable || User.AccessLevel >= AccessLevel.GameMaster)
            {
                Atlas.DropRune(User, Atlas.Entries[Selected], Selected);
                Refresh();
            }
            else
            {
                Atlas.Openers.Remove(User);
                User.SendLocalizedMessage(502413, null, 0x35); // That cannot be done while the book is locked down.
            }
        }

        public void SendLocationMessage(RunebookEntry e, Mobile from)
        {
            if (e.Type == RecallRuneType.Ship)
                return;

            string coords = ToCoordinates(e.Location, e.Map);

            if (coords != "Nowhere")
            {
                from.SendAsciiMessage(ToCoordinates(e.Location, e.Map));
            }
        }

        private void RecallSpell()
        {
            RunebookEntry e = Atlas.Entries[Selected];

            if (RunebookGump.HasSpell(User, 31))
            {
                SendLocationMessage(e, User);

                Atlas.OnTravel();
                new RecallSpell(User, null, e, null).Cast();
            }
            else
            {
                User.SendLocalizedMessage(500015); // You do not have that spell!
            }

            Atlas.Openers.Remove(User);
        }

        private void RecallCharge()
        {
            RunebookEntry e = Atlas.Entries[Selected];

            if (Atlas.CurCharges <= 0)
            {
                Refresh();
                User.SendLocalizedMessage(502412); // There are no charges left on that item.
            }
            else
            {
                SendLocationMessage(e, User);

                Atlas.OnTravel();

                if (new RecallSpell(User, Atlas, e, Atlas).Cast())
                    Atlas.NextUse = DateTime.UtcNow;

                Atlas.Openers.Remove(User);
            }
        }

        private void GateTravel()
        {
            RunebookEntry e = Atlas.Entries[Selected];

            if (RunebookGump.HasSpell(User, 51))
            {
                SendLocationMessage(e, User);

                Atlas.OnTravel();

                if (new GateTravelSpell(User, null, e).Cast())
                    Atlas.NextUse = DateTime.UtcNow;
            }
            else
            {
                User.SendLocalizedMessage(500015); // You do not have that spell!
            }

            Atlas.Openers.Remove(User);
        }

        private void SacredJourney()
        {
            RunebookEntry e = Atlas.Entries[Selected];

            if (RunebookGump.HasSpell(User, 209))
            {
                SendLocationMessage(e, User);

                Atlas.OnTravel();
                new SacredJourneySpell(User, null, e, null).Cast();
                Atlas.NextUse = DateTime.UtcNow;
            }
            else
            {
                User.SendLocalizedMessage(500015); // You do not have that spell!
            }

            Atlas.Openers.Remove(User);
        }

        private class InternalPrompt : Prompt
        {
            public override int MessageCliloc => 502414;  // Please enter a title for the runebook:
            public RunicAtlas Atlas { get; }

            public InternalPrompt(RunicAtlas atlas)
            {
                Atlas = atlas;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (Atlas.Deleted || !from.InRange(Atlas.GetWorldLocation(), 3) || !(from is PlayerMobile))
                    return;

                if (Atlas.CheckAccess(from) || from.AccessLevel >= AccessLevel.GameMaster)
                {
                    Atlas.Description = Utility.FixHtml(text.Trim());

                    from.CloseGump(typeof(RunicAtlasGump));
                    SendGump(new RunicAtlasGump((PlayerMobile)from, Atlas));
                    from.SendLocalizedMessage(1041531); // You have changed the title of the rune book.
                }
                else
                {
                    Atlas.Openers.Remove(from);
                    from.SendLocalizedMessage(502416); // That cannot be done while the book is locked down.
                }
            }

            public override void OnCancel(Mobile from)
            {
                from.SendLocalizedMessage(502415); // Request cancelled.

                if (from is PlayerMobile && !Atlas.Deleted && from.InRange(Atlas.GetWorldLocation(), 3))
                {
                    from.CloseGump(typeof(RunicAtlasGump));
                    SendGump(new RunicAtlasGump((PlayerMobile)from, Atlas));
                }
            }
        }
    }
}
