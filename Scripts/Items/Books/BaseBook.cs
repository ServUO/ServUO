using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class BookPageInfo
    {
        private string[] m_Lines;

        public string[] Lines
        {
            get
            {
                return m_Lines;
            }
            set
            {
                m_Lines = value;
            }
        }

        public BookPageInfo()
        {
            m_Lines = new string[0];
        }

        public BookPageInfo(params string[] lines)
        {
            m_Lines = lines;
        }

        public BookPageInfo(GenericReader reader)
        {
            int length = reader.ReadInt();

            m_Lines = new string[length];

            for (int i = 0; i < m_Lines.Length; ++i)
                m_Lines[i] = Utility.Intern(reader.ReadString());
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(m_Lines.Length);

            for (int i = 0; i < m_Lines.Length; ++i)
                writer.Write(m_Lines[i]);
        }
    }

    public class BaseBook : Item, ISecurable
    {
        private string m_Title;
        private string m_Author;
        private BookPageInfo[] m_Pages;
        private bool m_Writable;
        private SecureLevel m_SecureLevel;
		
        [CommandProperty(AccessLevel.GameMaster)]
        public string Title
        {
            get
            {
                return m_Title;
            }
            set
            {
                m_Title = value;
                InvalidateProperties();
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public string Author
        {
            get
            {
                return m_Author;
            }
            set
            {
                m_Author = value;
                InvalidateProperties();
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Writable
        {
            get
            {
                return m_Writable;
            }
            set
            {
                m_Writable = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PagesCount
        {
            get
            {
                return m_Pages.Length;
            }
        }

        public BookPageInfo[] Pages
        {
            get
            {
                return m_Pages;
            }
        }

        [Constructable]
        public BaseBook(int itemID)
            : this(itemID, 20, true)
        {
        }

        [Constructable]
        public BaseBook(int itemID, int pageCount, bool writable)
            : this(itemID, null, null, pageCount, writable)
        {
        }

        [Constructable]
        public BaseBook(int itemID, string title, string author, int pageCount, bool writable)
            : base(itemID)
        {
            m_Title = title;
            m_Author = author;
            m_Writable = writable;

            BookContent content = DefaultContent;

            if (content == null)
            {
                m_Pages = new BookPageInfo[pageCount];

                for (int i = 0; i < m_Pages.Length; ++i)
                    m_Pages[i] = new BookPageInfo();
            }
            else
            {
                m_Pages = content.Copy();
            }
        }

        // Intended for defined books only
        public BaseBook(int itemID, bool writable)
            : base(itemID)
        {
            m_Writable = writable;

            BookContent content = DefaultContent;

            if (content == null)
            {
                m_Pages = new BookPageInfo[0];
            }
            else
            {
                m_Title = content.Title;
                m_Author = content.Author;
                m_Pages = content.Copy();
            }
        }

        public virtual BookContent DefaultContent
        {
            get
            {
                return null;
            }
        }
	
        public BaseBook(Serial serial)
            : base(serial)
        {
        }

        [Flags]
        private enum SaveFlags
        {
            None = 0x00,
            Title = 0x01,
            Author = 0x02,
            Writable = 0x04,
            Content = 0x08
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            BookContent content = DefaultContent;

            SaveFlags flags = SaveFlags.None;

            if (m_Title != (content == null ? null : content.Title))
                flags |= SaveFlags.Title;

            if (m_Author != (content == null ? null : content.Author))
                flags |= SaveFlags.Author;

            if (m_Writable)
                flags |= SaveFlags.Writable;

            if (content == null || !content.IsMatch(m_Pages))
                flags |= SaveFlags.Content;

            writer.Write((int)4); // version

            writer.Write((int)m_SecureLevel);

            writer.Write((byte)flags);

            if ((flags & SaveFlags.Title) != 0)
                writer.Write(m_Title);

            if ((flags & SaveFlags.Author) != 0)
                writer.Write(m_Author);

            if ((flags & SaveFlags.Content) != 0)
            {
                writer.WriteEncodedInt(m_Pages.Length);

                for (int i = 0; i < m_Pages.Length; ++i)
                    m_Pages[i].Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 4:
                    {
                        m_SecureLevel = (SecureLevel)reader.ReadInt();
                        goto case 3;
                    }
                case 3:
                case 2:
                    {
                        BookContent content = DefaultContent;

                        SaveFlags flags = (SaveFlags)reader.ReadByte();

                        if ((flags & SaveFlags.Title) != 0)
                            m_Title = Utility.Intern(reader.ReadString());
                        else if (content != null)
                            m_Title = content.Title;

                        if ((flags & SaveFlags.Author) != 0)
                            m_Author = reader.ReadString();
                        else if (content != null)
                            m_Author = content.Author;

                        m_Writable = (flags & SaveFlags.Writable) != 0;

                        if ((flags & SaveFlags.Content) != 0)
                        {
                            m_Pages = new BookPageInfo[reader.ReadEncodedInt()];

                            for (int i = 0; i < m_Pages.Length; ++i)
                                m_Pages[i] = new BookPageInfo(reader);
                        }
                        else
                        {
                            if (content != null)
                                m_Pages = content.Copy();
                            else
                                m_Pages = new BookPageInfo[0];
                        }

                        break;
                    }
                case 1:
                case 0:
                    {
                        m_Title = reader.ReadString();
                        m_Author = reader.ReadString();
                        m_Writable = reader.ReadBool();

                        if (version == 0 || reader.ReadBool())
                        {
                            m_Pages = new BookPageInfo[reader.ReadInt()];

                            for (int i = 0; i < m_Pages.Length; ++i)
                                m_Pages[i] = new BookPageInfo(reader);
                        }
                        else
                        {
                            BookContent content = DefaultContent;

                            if (content != null)
                                m_Pages = content.Copy();
                            else
                                m_Pages = new BookPageInfo[0];
                        }

                        break;
                    }
            }

            if (version < 3 && (Weight == 1 || Weight == 2))
                Weight = -1;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (m_Title != null && m_Title.Length > 0)
                list.Add(m_Title);
            else
                base.AddNameProperty(list);
        }

        /*public override void GetProperties( ObjectPropertyList list )
        {
        base.GetProperties( list );

        if ( m_Title != null && m_Title.Length > 0 )
        list.Add( 1060658, "Title\t{0}", m_Title ); // ~1_val~: ~2_val~

        if ( m_Author != null && m_Author.Length > 0 )
        list.Add( 1060659, "Author\t{0}", m_Author ); // ~1_val~: ~2_val~

        if ( m_Pages != null && m_Pages.Length > 0 )
        list.Add( 1060660, "Pages\t{0}", m_Pages.Length ); // ~1_val~: ~2_val~
        }*/
		
        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "{0} by {1}", m_Title, m_Author);
            LabelTo(from, "[{0} pages]", m_Pages.Length);
        }
		
        public override void OnDoubleClick(Mobile from)
        {
            if (m_Title == null && m_Author == null && m_Writable == true)
            {
                Title = "a book";
                Author = from.Name;
            }

            from.Send(new BookHeader(from, this));
            from.Send(new BookPageDetails(this));
        }

        public string ContentAsString
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (BookPageInfo bpi in m_Pages)
                {
                    foreach (string line in bpi.Lines)
                    {
                        sb.AppendLine(line);
                    }
                }

                return sb.ToString();
            }
        }

        public string[] ContentAsStringArray
        {
            get
            {
                List<string> lines = new List<string>();

                foreach (BookPageInfo bpi in m_Pages)
                {
                    lines.AddRange(bpi.Lines);
                }

                return lines.ToArray();
            }
        }

        public virtual void ContentChangeEC(NetState state, PacketReader pvSrc)
        {
            int page = pvSrc.ReadUInt16();
            int lineCount = pvSrc.ReadUInt16();
            int index = page - 1;

            if (index < 0 || index >= m_Pages.Length)
                return;

            if (lineCount == 0xFFFF)
            {
                // send for new page
                state.Send(new BookPageDetails(this, page, m_Pages[index]));
            }
            else if (m_Writable && state.Mobile != null && state.Mobile.InRange(GetWorldLocation(), 1))
            {
                // updates after page is moved away from
                if (lineCount <= 19)
                {
                    string[] lines = new string[lineCount];

                    for (int j = 0; j < lineCount; ++j)
                        if ((lines[j] = pvSrc.ReadUTF8StringSafe()).Length >= 80)
                            return;

                    m_Pages[index].Lines = lines;
                }
                else
                {
                    return;
                }
            }
        }

        public static void Initialize()
        {
            PacketHandlers.Register(0xD4, 0, true, new OnPacketReceive(HeaderChange));
            PacketHandlers.Register(0x66, 0, true, new OnPacketReceive(ContentChange));
            PacketHandlers.Register(0x93, 99, true, new OnPacketReceive(OldHeaderChange));
        }

        public static void OldHeaderChange(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            BaseBook book = World.FindItem(pvSrc.ReadInt32()) as BaseBook;

            if (book == null || !book.Writable || !from.InRange(book.GetWorldLocation(), 1) || !book.IsAccessibleTo(from))
                return;

            pvSrc.Seek(4, SeekOrigin.Current); // Skip flags and page count

            string title = pvSrc.ReadStringSafe(60);
            string author = pvSrc.ReadStringSafe(30);

            book.Title = Utility.FixHtml(title);
            book.Author = Utility.FixHtml(author);
        }

        public static void HeaderChange(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            BaseBook book = World.FindItem(pvSrc.ReadInt32()) as BaseBook;

            if (book == null || !book.Writable || !from.InRange(book.GetWorldLocation(), 1) || !book.IsAccessibleTo(from))
                return;

            pvSrc.Seek(4, SeekOrigin.Current); // Skip flags and page count

            int titleLength = pvSrc.ReadUInt16();

            if (titleLength > 60)
                return;

            string title = pvSrc.ReadUTF8StringSafe(titleLength);

            int authorLength = pvSrc.ReadUInt16();

            if (authorLength > 30)
                return;

            string author = pvSrc.ReadUTF8StringSafe(authorLength);

            book.Title = Utility.FixHtml(title);
            book.Author = Utility.FixHtml(author);
        }

        public static void ContentChange(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            BaseBook book = World.FindItem(pvSrc.ReadInt32()) as BaseBook;

            if (book == null || !book.Writable || !from.InRange(book.GetWorldLocation(), 1) || !book.IsAccessibleTo(from))
                return;

            int pageCount = pvSrc.ReadUInt16();

            if (pageCount > book.PagesCount)
                return;

            for (int i = 0; i < pageCount; ++i)
            {
                int index = pvSrc.ReadUInt16();

                if (index >= 1 && index <= book.PagesCount)
                {
                    --index;

                    int lineCount = pvSrc.ReadUInt16();

                    if (lineCount <= 8)
                    {
                        string[] lines = new string[lineCount];

                        for (int j = 0; j < lineCount; ++j)
                            if ((lines[j] = pvSrc.ReadUTF8StringSafe()).Length >= 80)
                                return;

                        book.Pages[index].Lines = lines;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        #region ISecurable Members

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return m_SecureLevel;
            }
            set
            {
                m_SecureLevel = value;
            }
        }
        #endregion
    }

    public sealed class BookPageDetails : Packet
    {
        public BookPageDetails(BaseBook book)
            : base(0x66)
        {
            EnsureCapacity(256);

            m_Stream.Write((int)book.Serial);
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

        public BookPageDetails(BaseBook book, int page, BookPageInfo info)
            : base(0x66)
        {
            EnsureCapacity(256);

            m_Stream.Write((int)book.Serial);
            m_Stream.Write((ushort)0x1);

            m_Stream.Write((ushort)page);
            m_Stream.Write((ushort)info.Lines.Length);

            for (int i = 0; i < info.Lines.Length; ++i)
            {
                byte[] buffer = Utility.UTF8.GetBytes(info.Lines[i]);

                m_Stream.Write(buffer, 0, buffer.Length);
                m_Stream.Write((byte)0);
            }
        }
    }

    public sealed class BookHeader : Packet
    {
        public BookHeader(Mobile from, BaseBook book)
            : base(0xD4)
        {
            string title = book.Title == null ? "" : book.Title;
            string author = book.Author == null ? "" : book.Author;

            byte[] titleBuffer = Utility.UTF8.GetBytes(title);
            byte[] authorBuffer = Utility.UTF8.GetBytes(author);

            EnsureCapacity(15 + titleBuffer.Length + authorBuffer.Length);

            m_Stream.Write((int)book.Serial);
            m_Stream.Write((bool)true);
            m_Stream.Write((bool)book.Writable && from.InRange(book.GetWorldLocation(), 1));
            m_Stream.Write((ushort)book.PagesCount);

            m_Stream.Write((ushort)(titleBuffer.Length + 1));
            m_Stream.Write(titleBuffer, 0, titleBuffer.Length);
            m_Stream.Write((byte)0); // terminate

            m_Stream.Write((ushort)(authorBuffer.Length + 1));
            m_Stream.Write(authorBuffer, 0, authorBuffer.Length);
            m_Stream.Write((byte)0); // terminate
        }
    }
}