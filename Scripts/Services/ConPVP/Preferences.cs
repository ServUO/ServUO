using System;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.ConPVP
{
    public class PreferencesController : Item
    {
        private Preferences m_Preferences;
        [Constructable]
        public PreferencesController()
            : base(0x1B7A)
        {
            this.Visible = false;
            this.Movable = false;

            this.m_Preferences = new Preferences();

            if (Preferences.Instance == null)
                Preferences.Instance = this.m_Preferences;
            else
                this.Delete();
        }

        public PreferencesController(Serial serial)
            : base(serial)
        {
        }

        //[CommandProperty( AccessLevel.GameMaster )]
        public Preferences Preferences
        {
            get
            {
                return this.m_Preferences;
            }
            set
            {
            }
        }
        public override string DefaultName
        {
            get
            {
                return "preferences controller";
            }
        }
        public override void Delete()
        {
            if (Preferences.Instance != this.m_Preferences)
                base.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            this.m_Preferences.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Preferences = new Preferences(reader);
                        Preferences.Instance = this.m_Preferences;
                        break;
                    }
            }
        }
    }

    public class Preferences
    {
        private static Preferences m_Instance;
        private readonly ArrayList m_Entries;
        private readonly Hashtable m_Table;
        public Preferences()
        {
            this.m_Table = new Hashtable();
            this.m_Entries = new ArrayList();
        }

        public Preferences(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        int count = reader.ReadEncodedInt();

                        this.m_Table = new Hashtable(count);
                        this.m_Entries = new ArrayList(count);

                        for (int i = 0; i < count; ++i)
                        {
                            PreferencesEntry entry = new PreferencesEntry(reader, this, version);

                            if (entry.Mobile != null)
                            {
                                this.m_Table[entry.Mobile] = entry;
                                this.m_Entries.Add(entry);
                            }
                        }

                        break;
                    }
            }
        }

        public static Preferences Instance
        {
            get
            {
                return m_Instance;
            }
            set
            {
                m_Instance = value;
            }
        }
        public ArrayList Entries
        {
            get
            {
                return this.m_Entries;
            }
        }
        public PreferencesEntry Find(Mobile mob)
        {
            PreferencesEntry entry = (PreferencesEntry)this.m_Table[mob];

            if (entry == null)
            {
                this.m_Table[mob] = entry = new PreferencesEntry(mob, this);
                this.m_Entries.Add(entry);
            }

            return entry;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version;

            writer.WriteEncodedInt((int)this.m_Entries.Count);

            for (int i = 0; i < this.m_Entries.Count; ++i)
                ((PreferencesEntry)this.m_Entries[i]).Serialize(writer);
        }
    }

    public class PreferencesEntry
    {
        private readonly Mobile m_Mobile;
        private readonly ArrayList m_Disliked;
        private readonly Preferences m_Preferences;
        public PreferencesEntry(Mobile mob, Preferences prefs)
        {
            this.m_Preferences = prefs;
            this.m_Mobile = mob;
            this.m_Disliked = new ArrayList();
        }

        public PreferencesEntry(GenericReader reader, Preferences prefs, int version)
        {
            this.m_Preferences = prefs;

            switch ( version )
            {
                case 0:
                    {
                        this.m_Mobile = reader.ReadMobile();

                        int count = reader.ReadEncodedInt();

                        this.m_Disliked = new ArrayList(count);

                        for (int i = 0; i < count; ++i)
                            this.m_Disliked.Add(reader.ReadString());

                        break;
                    }
            }
        }

        public Mobile Mobile
        {
            get
            {
                return this.m_Mobile;
            }
        }
        public ArrayList Disliked
        {
            get
            {
                return this.m_Disliked;
            }
        }
        public void Serialize(GenericWriter writer)
        {
            writer.Write((Mobile)this.m_Mobile);

            writer.WriteEncodedInt((int)this.m_Disliked.Count);

            for (int i = 0; i < this.m_Disliked.Count; ++i)
                writer.Write((string)this.m_Disliked[i]);
        }
    }

    public class PreferencesGump : Gump
    {
        private readonly Mobile m_From;
        private readonly PreferencesEntry m_Entry;
        private int m_ColumnX = 12;
        public PreferencesGump(Mobile from, Preferences prefs)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Entry = prefs.Find(from);

            if (this.m_Entry == null)
                return;

            List<Arena> arenas = Arena.Arenas;

            this.AddPage(0);

            int height = 12 + 20 + (arenas.Count * 31) + 24 + 12;

            this.AddBackground(0, 0, 499 + 40 - 365, height, 0x2436);

            for (int i = 1; i < arenas.Count; i += 2)
                this.AddImageTiled(12, 32 + (i * 31), 475 + 40 - 365, 30, 0x2430);

            this.AddAlphaRegion(10, 10, 479 + 40 - 365, height - 20);

            this.AddColumnHeader(35, null);
            this.AddColumnHeader(115, "Arena");

            this.AddButton(499 + 40 - 365 - 12 - 63 - 4 - 63, height - 12 - 24, 247, 248, 1, GumpButtonType.Reply, 0);
            this.AddButton(499 + 40 - 365 - 12 - 63, height - 12 - 24, 241, 242, 2, GumpButtonType.Reply, 0);

            for (int i = 0; i < arenas.Count; ++i)
            {
                Arena ar = arenas[i];

                string name = ar.Name;

                if (name == null)
                    name = "(no name)";

                int x = 12;
                int y = 32 + (i * 31);

                int color = 0xCCFFCC;

                this.AddCheck(x + 3, y + 1, 9730, 9727, this.m_Entry.Disliked.Contains(name), i);
                x += 35;

                this.AddBorderedText(x + 5, y + 5, 115 - 5, name, color, 0);
                x += 115;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_Entry == null)
                return;

            if (info.ButtonID != 1)
                return;

            this.m_Entry.Disliked.Clear();

            List<Arena> arenas = Arena.Arenas;

            for (int i = 0; i < info.Switches.Length; ++i)
            {
                int idx = info.Switches[i];

                if (idx >= 0 && idx < arenas.Count)
                    this.m_Entry.Disliked.Add(arenas[idx].Name);
            }
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        private void AddBorderedText(int x, int y, int width, string text, int color, int borderColor)
        {
            /*AddColoredText( x - 1, y, width, text, borderColor );
            AddColoredText( x + 1, y, width, text, borderColor );
            AddColoredText( x, y - 1, width, text, borderColor );
            AddColoredText( x, y + 1, width, text, borderColor );*/
            /*AddColoredText( x - 1, y - 1, width, text, borderColor );
            AddColoredText( x + 1, y + 1, width, text, borderColor );*/
            this.AddColoredText(x, y, width, text, color);
        }

        private void AddColoredText(int x, int y, int width, string text, int color)
        {
            if (color == 0)
                this.AddHtml(x, y, width, 20, text, false, false);
            else
                this.AddHtml(x, y, width, 20, this.Color(text, color), false, false);
        }

        private void AddColumnHeader(int width, string name)
        {
            this.AddBackground(this.m_ColumnX, 12, width, 20, 0x242C);
            this.AddImageTiled(this.m_ColumnX + 2, 14, width - 4, 16, 0x2430);

            if (name != null)
                this.AddBorderedText(this.m_ColumnX, 13, width, this.Center(name), 0xFFFFFF, 0);

            this.m_ColumnX += width;
        }
    }
}