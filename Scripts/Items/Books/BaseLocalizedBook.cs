using Server.Gumps;
using System;

namespace Server.Items
{
    public abstract class BaseLocalizedBook : Item
    {
        public virtual object Title => "a book";
        public virtual object Author => "unknown";

        public abstract int[] Contents { get; }

        public BaseLocalizedBook() : base(4082)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
                from.SendSound(0x55);
            }
        }

        private class InternalGump : Gump
        {
            public readonly int Page1X = 40;
            public readonly int Page2X = 230;
            public readonly int StartY = 30;
            public readonly int Width = 140;
            public readonly int Height = 175;

            private readonly BaseLocalizedBook m_Book;

            public InternalGump(BaseLocalizedBook book)
                : base(50, 50)
            {
                m_Book = book;
                int page = 0;
                int pages = (int)Math.Ceiling(m_Book.Contents.Length / 2.0);

                AddPage(page);
                AddImage(0, 0, 500);

                page++;
                AddPage(page);

                if (book.Title is int)
                    AddHtmlLocalized(Page1X, 60, Width, 48, (int)book.Title, false, false);
                else if (book.Title is string)
                    AddHtml(Page1X, 60, Width, 48, (string)book.Title, false, false);
                else
                    AddLabel(Page1X, 60, 0, "A Book");

                AddHtml(40, 130, 200, 16, "by", false, false);

                if (book.Author is int)
                    AddHtmlLocalized(Page1X, 155, Width, 16, (int)book.Author, false, false);
                else if (book.Author is string)
                    AddHtml(Page1X, 155, Width, 16, (string)book.Author, false, false);
                else
                    AddLabel(Page1X, 155, 0, "unknown");

                for (int i = 0; i < m_Book.Contents.Length; i++)
                {
                    int cliloc = m_Book.Contents[i];
                    bool endPage = false;
                    int x = Page1X;

                    if (cliloc <= 0)
                        continue;

                    if (page == 1)
                    {
                        x = Page2X;
                        endPage = true;
                    }
                    else
                    {
                        if ((i + 1) % 2 == 0)
                            x = Page1X;
                        else if (page <= pages)
                        {
                            endPage = true;
                            x = Page2X;
                        }
                    }

                    AddHtmlLocalized(x, StartY, Width, Height, cliloc, false, false);

                    if (page < pages)
                        AddButton(356, 0, 502, 502, 0, GumpButtonType.Page, page + 1);

                    if (page > 0)
                        AddButton(0, 0, 501, 501, 0, GumpButtonType.Page, page - 1);

                    if (endPage)
                    {
                        page++;
                        AddPage(page);
                    }
                }
            }
        }

        public BaseLocalizedBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }
}