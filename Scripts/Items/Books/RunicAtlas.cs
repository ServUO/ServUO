using System;
using Server.Network;
using Server.Gumps;
using Server.Prompts;
using Server.Mobiles;
using Server.Items;
using Server.Misc;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Chivalry;

namespace Server.Items
{
    [FlipableAttribute(39958, 39959)]
    public class RunicAtlas : Runebook
	{
        public override int MaxEntries { get { return 48; } }
        public override int LabelNumber { get { return 1156443; } } // a runic atlas

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
                    
                    BaseGump.SendGump(new RunicAtlasGump((PlayerMobile)from, this));
                    Openers.Add(from);
                }
                else
                    from.SendLocalizedMessage(502436); // That is not accessible.
            }
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

                    g = new RunicAtlasGump((PlayerMobile)from, this);
                    g.Page = newPage;
                    from.SendGump(g);
                }
            }

            return d;
        }

        public override int OnCraft(int quality, bool makersMark, Mobile from, Server.Engines.Craft.CraftSystem craftSystem, Type typeRes, ITool tool, Server.Engines.Craft.CraftItem craftItem, int resHue)
        {
            if (makersMark)
                Crafter = from;

            Quality = (BookQuality)(quality - 1);

            MaxCharges = 100;

            return quality;
        }

		public RunicAtlas( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
            writer.Write(Selected);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
            Selected = reader.ReadInt();

            if (MaxCharges != 100)
                MaxCharges = 100;
		}
	}

    public class RunicAtlasGump : BaseGump
	{
		public static string ToCoordinates(Point3D location, Map map)
		{
			int xLong = 0, yLat = 0, xMins = 0, yMins = 0;
			bool xEast = false, ySouth = false;

			bool valid = Sextant.Format(location, map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth);

			return valid ? String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W") : "unknown";
		}

        public RunicAtlas Atlas { get; set; }
        public int Selected { get { return Atlas == null ? -1 : Atlas.Selected; } }
        public int Page { get; set; }

        public RunicAtlasGump(PlayerMobile pm, RunicAtlas atlas)
            : base(pm, 150, 200)
        {
            Atlas = atlas;
            Page = 0;
        }

        public override void AddGumpLayout()
        {
            AddImage(0, 0, 39923);

            AddHtmlLocalized(60, 8, 180, 16, 1060728, String.Format("{0}\t{1}", Atlas.CurCharges, Atlas.MaxCharges), 1, false, false); //charges: ~1_val~ / ~2_val~
            
            AddHtmlLocalized(265, 8, 150, 16, 1011299, false, false); // rename book 
            AddButton(250, 12, 2103, 2103, 1, GumpButtonType.Reply, 0);

            int startIndex = Page * 16;
            int index = 0;

            for (int i = startIndex; i < startIndex + 16; i++)
            {
                string desc;
				        int hue;

                if (i < Atlas.Entries.Count)
                {
                    desc = RunebookGump.GetName(Atlas.Entries[i].Description);
                    hue = Selected == i ? 0x90 : RunebookGump.GetMapHue(Atlas.Entries[i].Map);
                }
                else
                {
                    desc = "Empty";
                    hue = 0;
                }

                // Select Button
                AddButton(45 + ((index / 8) * 205), 64 + ((index % 8) * 20), 2103, 2104, i + 100, GumpButtonType.Reply, 0);

                // Description label
                AddLabelCropped(60 + ((index / 8) * 205), 60 + ((index % 8) * 20), 115, 17, hue, desc);

                index++;
            }

            if (Selected >= 0 && Selected < Atlas.Entries.Count)
            {
                RunebookEntry entry = Atlas.Entries[Selected];

                if (entry != null)
                {
                    string coords = ToCoordinates(entry.Location, entry.Map);

                    if(coords != "unknown")
                        AddHtml(40, 250, 155, 16, String.Format("<center>{0}</center>", coords), false, false);

                    AddHtmlLocalized(70, 291, 150, 16, 1011300, false, false); // Set default

                    if (Atlas.DefaultIndex != Selected)
                        AddButton(45, 295, 2103, 2103, 2, GumpButtonType.Reply, 0);

                    AddHtmlLocalized(70, 311, 150, 16, 1011298, false, false); // drop rune
                    AddButton(45, 315, 2103, 2103, 3, GumpButtonType.Reply, 0);

                    AddHtml(25, 345, 180, 16, String.Format("<center>{0}</center>", entry.Description), false, false); 

                    AddHtmlLocalized(280, 286, 100, 16, 1077595, false, false); // Recall (Spell)
                    AddButton(265, 290, 2103, 2103, 4, GumpButtonType.Reply, 0);

                    AddHtmlLocalized(280, 303, 100, 16, 1077594, false, false); // Recall (Charge)
                    AddButton(265, 307, 2103, 2103, 5, GumpButtonType.Reply, 0);

                    AddHtmlLocalized(280, 320, 100, 16, 1015214, false, false); // Gate Travel
                    AddButton(265, 324, 2103, 2103, 6, GumpButtonType.Reply, 0);

                    AddHtmlLocalized(280, 337, 100, 16, 1060502, false, false); // Sacred Journey
                    AddButton(265, 341, 2103, 2103, 7, GumpButtonType.Reply, 0);
                }
            }

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
            if (info.ButtonID >= 100 && info.ButtonID < 1000)
            {
                SelectEntry(info.ButtonID - 100);
            }
            else
            {
                switch (info.ButtonID)
                {
                    case 1: RenameBook(); break;
                    case 2: SetDefault(); break;
                    case 3: DropRune(); break;
                    case 4: RecallSpell(); break;
                    case 5: RecallCharge(); break;
                    case 6: GateTravel(); break;
                    case 7: SacredJourney(); break;
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
            if (Atlas.Movable || User.AccessLevel >= AccessLevel.GameMaster)
            {
                User.SendLocalizedMessage(502414); // Please enter a title for the runebook:
                User.Prompt = new InternalPrompt(Atlas);
            }
            else
            {
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
            if (Atlas.Movable || User.AccessLevel >= AccessLevel.GameMaster)
            {
                Atlas.DefaultIndex = Selected;
                Refresh();
            }
            else
            {
                Atlas.Openers.Remove(User);
                User.SendLocalizedMessage(502413, null, 0x35); // That cannot be done while the book is locked down.
            }
        }

        private void DropRune()
        {
            if (Atlas.Movable || User.AccessLevel >= AccessLevel.GameMaster)
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

        private void RecallSpell()
        {
            RunebookEntry e = Atlas.Entries[Selected];

            if (RunebookGump.HasSpell(User, 31))
            {
                string coords = ToCoordinates(e.Location, e.Map);

                if(coords != "unknown")
                    User.SendMessage(ToCoordinates(e.Location, e.Map));

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
                string coords = ToCoordinates(e.Location, e.Map);

                if(coords != "unkown")
                    User.SendMessage(ToCoordinates(e.Location, e.Map));

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
                string coords = ToCoordinates(e.Location, e.Map);

                if(coords != "unknown")
                    User.SendMessage(ToCoordinates(e.Location, e.Map));

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

            if (Core.AOS)
            {
                if (RunebookGump.HasSpell(User, 209))
                {
                    User.SendMessage(ToCoordinates(e.Location, e.Map));

                    Atlas.OnTravel();
                    new SacredJourneySpell(User, null, e, null).Cast();
                    Atlas.NextUse = DateTime.UtcNow;
                }
                else
                {
                    User.SendLocalizedMessage(500015); // You do not have that spell!
                }
            }

            Atlas.Openers.Remove(User);
        }

        private class InternalPrompt : Prompt
        {
            public RunicAtlas Atlas { get; private set; }

            public InternalPrompt(RunicAtlas atlas)
            {
                Atlas = atlas;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (Atlas.Deleted || !from.InRange(Atlas.GetWorldLocation(), 3) || !(from is PlayerMobile))
                    return;

                if (Atlas.Movable || from.AccessLevel >= AccessLevel.GameMaster)
                {
                    Atlas.Description = Utility.FixHtml(text.Trim());
                    from.SendGump(new RunicAtlasGump((PlayerMobile)from, Atlas));
                    from.SendMessage("The book's title has been changed.");
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

                if (from is PlayerMobile && !Atlas.Deleted && from.InRange(Atlas.GetWorldLocation(), (Core.ML ? 3 : 1)))
                {
                    from.SendGump(new RunicAtlasGump((PlayerMobile)from, Atlas));
                }
            }
        }
    }
}
