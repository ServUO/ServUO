using System;
using Server.Gumps;

/*
** SimpleNote
** updated 1/3/04
** ArteGordon
** adds a simple item that displays text messages in a scroll gump.  The size can be varied and the note text and text-color can be specified.
** The title of the note and its color can also be set.
*/
namespace Server.Items
{
    public class SimpleNote : Item
    {
        private int m_size = 1;
        private string m_NoteString;
        private string m_TitleString;
        private int m_TextColor = 0x3e8;
        private int m_TitleColor = 0xef0000;// cyan 0xf70000, black 0x3e8, brown 0xef0000 darkblue 0x7fff
        [Constructable]
        public SimpleNote()
            : base(0x14EE)
        { 
            this.Name = "A note";
            this.TitleString = "A note";
        }

        public SimpleNote(Serial serial)
            : base(serial)
        { 
        }

        [CommandProperty(AccessLevel.Spawner)]
        public string NoteString
        {
            get
            {
                return this.m_NoteString;
            }
            set
            {
                this.m_NoteString = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string TitleString
        {
            get
            {
                return this.m_TitleString;
            }
            set
            {
                this.m_TitleString = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int Size
        {
            get
            {
                return this.m_size;
            }
            set 
            {
                this.m_size = value;
                if (this.m_size < 1)
                    this.m_size = 1;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int TextColor
        {
            get
            {
                return this.m_TextColor;
            }
            set
            {
                this.m_TextColor = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int TitleColor
        {
            get
            {
                return this.m_TitleColor;
            }
            set
            {
                this.m_TitleColor = value;
                this.InvalidateProperties();
            }
        }
        public override void Serialize(GenericWriter writer)
        { 
            base.Serialize(writer); 

            writer.Write((int)0); // version 
         
            writer.Write(this.m_NoteString);
            writer.Write(this.m_TitleString);
            writer.Write(this.m_TextColor);
            writer.Write(this.m_TitleColor);
            writer.Write(this.m_size);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch ( version )
            {
                case 0:
                    {
                        this.m_NoteString = reader.ReadString();
                        this.m_TitleString = reader.ReadString();
                        this.m_TextColor = reader.ReadInt();
                        this.m_TitleColor = reader.ReadInt();
                        this.m_size = reader.ReadInt();
                    }
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            SimpleNoteGump g = new SimpleNoteGump(this);
            from.SendGump(g);
        }
    }

    public class SimpleNoteGump : Gump
    {
        private readonly SimpleNote m_Note;
        public SimpleNoteGump(SimpleNote note)
            : base(0, 0)
        {
            this.m_Note = note;

            this.AddPage(0);
            this.AddAlphaRegion(40, 41, 225, /*371*/70 * note.Size);
            // scroll top
            this.AddImageTiled(3, 5, 300, 37, 0x820);
            // scroll middle, upper portion
            this.AddImageTiled(19, 41, 263, 70, 0x821);
            for (int i = 1; i < note.Size; i++)
            {
                // scroll middle , lower portion
                this.AddImageTiled(19, 41 + 70 * i, 263, 70, 0x822);
            }
            // scroll bottom
            this.AddImageTiled(20, 111 + 70 * (note.Size - 1), 273, 34, 0x823);
            // title string
            this.AddHtml(55, 10, 200, 37, SimpleNoteGump.HtmlFormat(note.TitleString, note.TitleColor), false, false);
            // text string
            this.AddHtml(40, 41, 225, 70 * note.Size, SimpleNoteGump.HtmlFormat(note.NoteString, note.TextColor), false, false);
        }

        public static string HtmlFormat(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0}>{1}</BASEFONT>", color, text);
        }
    }
}