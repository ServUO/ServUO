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
                return this.m_Lines;
            }
            set
            {
                this.m_Lines = value;
            }
        }

        public BookPageInfo()
        {
            this.m_Lines = new string[0];
        }

        public BookPageInfo(params string[] lines)
        {
            this.m_Lines = lines;
        }

        public BookPageInfo(GenericReader reader)
        {
            int length = reader.ReadInt();

            this.m_Lines = new string[length];

            for (int i = 0; i < this.m_Lines.Length; ++i)
                this.m_Lines[i] = Utility.Intern(reader.ReadString());
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(this.m_Lines.Length);

            for (int i = 0; i < this.m_Lines.Length; ++i)
                writer.Write(this.m_Lines[i]);
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
                return this.m_Title;
            }
            set
            {
                this.m_Title = value;
                this.InvalidateProperties();
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public string Author
        {
            get
            {
                return this.m_Author;
            }
            set
            {
                this.m_Author = value;
                this.InvalidateProperties();
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Writable
        {
            get
            {
                return this.m_Writable;
            }
            set
            {
                this.m_Writable = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PagesCount
        {
            get
            {
                return this.m_Pages.Length;
            }
        }

        public BookPageInfo[] Pages
        {
            get
            {
                return this.m_Pages;
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
            this.m_Title = title;
            this.m_Author = author;
            this.m_Writable = writable;

            BookContent content = this.DefaultContent;

            if (content == null)
            {
                this.m_Pages = new BookPageInfo[pageCount];

                for (int i = 0; i < this.m_Pages.Length; ++i)
                    this.m_Pages[i] = new BookPageInfo();
            }
            else
            {
                this.m_Pages = content.Copy();
            }
        }

        // Intended for defined books only
        public BaseBook(int itemID, bool writable)
            : base(itemID)
        {
            this.m_Writable = writable;

            BookContent content = this.DefaultContent;

            if (content == null)
            {
                this.m_Pages = new BookPageInfo[0];
            }
            else
            {
                this.m_Title = content.Title;
                this.m_Author = content.Author;
                this.m_Pages = content.Copy();
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

            BookContent content = this.DefaultContent;

            SaveFlags flags = SaveFlags.None;

            if (this.m_Title != (content == null ? null : content.Title))
                flags |= SaveFlags.Title;

            if (this.m_Author != (content == null ? null : content.Author))
                flags |= SaveFlags.Author;

            if (this.m_Writable)
                flags |= SaveFlags.Writable;

            if (content == null || !content.IsMatch(this.m_Pages))
                flags |= SaveFlags.Content;

            writer.Write((int)4); // version

            writer.Write((int)this.m_SecureLevel);

            writer.Write((byte)flags);

            if ((flags & SaveFlags.Title) != 0)
                writer.Write(this.m_Title);

            if ((flags & SaveFlags.Author) != 0)
                writer.Write(this.m_Author);

            if ((flags & SaveFlags.Content) != 0)
            {
                writer.WriteEncodedInt(this.m_Pages.Length);

                for (int i = 0; i < this.m_Pages.Length; ++i)
                    this.m_Pages[i].Serialize(writer);
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
                        this.m_SecureLevel = (SecureLevel)reader.ReadInt();
                        goto case 3;
                    }
                case 3:
                case 2:
                    {
                        BookContent content = this.DefaultContent;

                        SaveFlags flags = (SaveFlags)reader.ReadByte();

                        if ((flags & SaveFlags.Title) != 0)
                            this.m_Title = Utility.Intern(reader.ReadString());
                        else if (content != null)
                            this.m_Title = content.Title;

                        if ((flags & SaveFlags.Author) != 0)
                            this.m_Author = reader.ReadString();
                        else if (content != null)
                            this.m_Author = content.Author;

                        this.m_Writable = (flags & SaveFlags.Writable) != 0;

                        if ((flags & SaveFlags.Content) != 0)
                        {
                            this.m_Pages = new BookPageInfo[reader.ReadEncodedInt()];

                            for (int i = 0; i < this.m_Pages.Length; ++i)
                                this.m_Pages[i] = new BookPageInfo(reader);
                        }
                        else
                        {
                            if (content != null)
                                this.m_Pages = content.Copy();
                            else
                                this.m_Pages = new BookPageInfo[0];
                        }

                        break;
                    }
                case 1:
                case 0:
                    {
                        this.m_Title = reader.ReadString();
                        this.m_Author = reader.ReadString();
                        this.m_Writable = reader.ReadBool();

                        if (version == 0 || reader.ReadBool())
                        {
                            this.m_Pages = new BookPageInfo[reader.ReadInt()];

                            for (int i = 0; i < this.m_Pages.Length; ++i)
                                this.m_Pages[i] = new BookPageInfo(reader);
                        }
                        else
                        {
                            BookContent content = this.DefaultContent;

                            if (content != null)
                                this.m_Pages = content.Copy();
                            else
                                this.m_Pages = new BookPageInfo[0];
                        }

                        break;
                    }
            }

            if (version < 3 && (this.Weight == 1 || this.Weight == 2))
                this.Weight = -1;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (this.m_Title != null && this.m_Title.Length > 0)
                list.Add(this.m_Title);
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
            this.LabelTo(from, "{0} by {1}", this.m_Title, this.m_Author);
            this.LabelTo(from, "[{0} pages]", this.m_Pages.Length);
        }
		
        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Title == null && this.m_Author == null && this.m_Writable == true)
            {
                this.Title = "a book";
                this.Author = from.Name;
            }

            from.Send(new BookHeader(from, this));
            from.Send(new BookPageDetails(this));
        }

        public string ContentAsString
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (BookPageInfo bpi in this.m_Pages)
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

                foreach (BookPageInfo bpi in this.m_Pages)
                {
                    lines.AddRange(bpi.Lines);
                }

                return lines.ToArray();
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
                return this.m_SecureLevel;
            }
            set
            {
                this.m_SecureLevel = value;
            }
        }
        #endregion
    }

    public sealed class BookPageDetails : Packet
    {
        public BookPageDetails(BaseBook book)
            : base(0x66)
        {
            this.EnsureCapacity(256);

            this.m_Stream.Write((int)book.Serial);
            this.m_Stream.Write((ushort)book.PagesCount);

            for (int i = 0; i < book.PagesCount; ++i)
            {
                BookPageInfo page = book.Pages[i];

                this.m_Stream.Write((ushort)(i + 1));
                this.m_Stream.Write((ushort)page.Lines.Length);

                for (int j = 0; j < page.Lines.Length; ++j)
                {
                    byte[] buffer = Utility.UTF8.GetBytes(page.Lines[j]);

                    this.m_Stream.Write(buffer, 0, buffer.Length);
                    this.m_Stream.Write((byte)0);
                }
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

            this.EnsureCapacity(15 + titleBuffer.Length + authorBuffer.Length);

            this.m_Stream.Write((int)book.Serial);
            this.m_Stream.Write((bool)true);
            this.m_Stream.Write((bool)book.Writable && from.InRange(book.GetWorldLocation(), 1));
            this.m_Stream.Write((ushort)book.PagesCount);

            this.m_Stream.Write((ushort)(titleBuffer.Length + 1));
            this.m_Stream.Write(titleBuffer, 0, titleBuffer.Length);
            this.m_Stream.Write((byte)0); // terminate

            this.m_Stream.Write((ushort)(authorBuffer.Length + 1));
            this.m_Stream.Write(authorBuffer, 0, authorBuffer.Length);
            this.m_Stream.Write((byte)0); // terminate
        }
    }
}