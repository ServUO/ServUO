#define BOOKTEXTENTRY
using Server.Network;

namespace Server.Items
{
    public class XmlTextEntryBook : BaseEntryBook
    {
        public XmlTextEntryBook(int itemID, string title, string author, int pageCount, bool writable) : base(itemID, title, author, pageCount, writable)
        {
        }

        public XmlTextEntryBook(Serial serial) : base(serial)
        {
        }

        public void FillTextEntryBook(string text)
        {
            int pagenum = 0;
            int current = 0;

            // break up the text into single line length pieces
            while (text != null && current < text.Length)
            {
                int lineCount = 10;
                string[] lines = new string[lineCount];

                // place the line on the page
                for (int i = 0; i < lineCount; i++)
                {
                    if (current < text.Length)
                    {
                        // make each line 25 chars long
                        int length = text.Length - current;
                        if (length > 20) length = 20;
                        lines[i] = text.Substring(current, length);
                        current += length;
                    }
                    else
                    {
                        // fill up the remaining lines
                        lines[i] = string.Empty;
                    }
                }

                if (pagenum >= PagesCount)
                    return;
                Pages[pagenum].Lines = lines;
                pagenum++;
            }
            // empty the remaining contents
            for (int j = pagenum; j < PagesCount; j++)
            {
                if (Pages[j].Lines.Length > 0)
                    for (int i = 0; i < Pages[j].Lines.Length; i++)
                    {
                        Pages[j].Lines[i] = string.Empty;
                    }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            Delete();
        }
    }

    public class BaseEntryBook : Item
    {
        private string m_Title;
        private string m_Author;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Author
        {
            get { return m_Author; }
            set { m_Author = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Writable { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PagesCount => Pages.Length;

        public BookPageInfo[] Pages { get; }

        [Constructable]
        public BaseEntryBook(int itemID, string title, string author, int pageCount, bool writable) : base(itemID)
        {
            m_Title = title;
            m_Author = author;
            Pages = new BookPageInfo[pageCount];
            Writable = writable;

            for (int i = 0; i < Pages.Length; ++i)
                Pages[i] = new BookPageInfo();
        }

        public BaseEntryBook(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
#if (BOOKTEXTENTRY)
        public override void OnDoubleClick(Mobile from)
        {
            from.Send(new EntryBookHeader(from, this));
            from.Send(new EntryBookPageDetails(this));
        }
#endif
    }

#if (BOOKTEXTENTRY)
    public sealed class EntryBookPageDetails : Packet
    {
        public EntryBookPageDetails(BaseEntryBook book) : base(0x66)
        {
            EnsureCapacity(256);

            m_Stream.Write(book.Serial);
            m_Stream.Write((ushort)book.PagesCount);

            for (int i = 0; i < book.PagesCount; ++i)
            {
                BookPageInfo page = book.Pages[i];

                m_Stream.Write((ushort)(i + 1));
                m_Stream.Write((ushort)page.Lines.Length);

                for (int j = 0; j < page.Lines.Length; ++j)
                {
                    byte[] buffer = Utility.UTF8.GetBytes(page.Lines[j]);

                    m_Stream.Write(buffer, 0, buffer.Length);
                    m_Stream.Write((byte)0);
                }
            }
        }
    }

    public sealed class EntryBookHeader : Packet
    {
        public EntryBookHeader(Mobile from, BaseEntryBook book) : base(0xD4)
        {
            string title = book.Title ?? "";
            string author = book.Author ?? "";

            byte[] titleBuffer = Utility.UTF8.GetBytes(title);
            byte[] authorBuffer = Utility.UTF8.GetBytes(author);

            EnsureCapacity(15 + titleBuffer.Length + authorBuffer.Length);

            m_Stream.Write(book.Serial);
            m_Stream.Write(true);
            m_Stream.Write(book.Writable && from.InRange(book.GetWorldLocation(), 1));
            m_Stream.Write((ushort)book.PagesCount);

            m_Stream.Write((ushort)(titleBuffer.Length + 1));
            m_Stream.Write(titleBuffer, 0, titleBuffer.Length);
            m_Stream.Write((byte)0); // terminate

            m_Stream.Write((ushort)(authorBuffer.Length + 1));
            m_Stream.Write(authorBuffer, 0, authorBuffer.Length);
            m_Stream.Write((byte)0); // terminate
        }
    }
#endif
}

