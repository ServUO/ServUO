using Server.Mobiles;

namespace Server.Gumps
{
    public class StoryGump : BaseGump
    {
        public TextDefinition Title { get; set; }
        public PageData[] PageEntries { get; set; }

        public StoryGump(PlayerMobile pm)
            : base(pm, 100, 100)
        {
        }

        public StoryGump(PlayerMobile pm, TextDefinition title, params PageData[] entries)
            : base(pm, 100, 100)
        {
            Title = title;
            PageEntries = entries;
        }

        public override void AddGumpLayout()
        {
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddImageTiled(50, 20, 400, 460, 0x1404);
            AddImageTiled(50, 29, 30, 450, 0x28DC);
            AddImageTiled(34, 140, 17, 339, 0x242F);
            AddImage(48, 135, 0x28AB);
            AddImage(-16, 285, 0x28A2);
            AddImage(0, 10, 0x28B5);
            AddImage(25, 0, 0x28B4);
            AddImageTiled(83, 15, 350, 15, 0x280A);
            AddImage(34, 479, 0x2842);
            AddImage(442, 479, 0x2840);
            AddImageTiled(51, 479, 392, 17, 0x2775);
            AddImageTiled(415, 29, 44, 450, 0xA2D);
            AddImageTiled(415, 29, 30, 450, 0x28DC);
            AddImage(370, 50, 0x589);

            AddImage(379, 60, 0x15A9);
            AddImage(425, 0, 0x28C9);
            AddImage(90, 33, 0x232D);
            AddImageTiled(130, 65, 175, 1, 0x238D);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            if (Title.Number > 0)
            {
                AddHtmlLocalized(140, 45, 250, 24, Title.Number, 0x7FFF, false, false);
            }
            else
            {
                AddHtml(140, 45, 250, 24, Color("#FFFFFF", Title.String), false, false);
            }

            for (int i = 0; i < PageEntries.Length; i++)
            {
                RenderPage(PageEntries[i]);
            }
        }

        public void RenderPage(PageData page)
        {
            AddPage(page.Page);
            TextDefinition textDef = page.Text;

            if (textDef.Number > 0)
            {
                AddHtmlLocalized(107, 140, 300, 150, textDef.Number, 0x7FFF, false, true);
            }
            else
            {
                AddHtml(107, 140, 300, 150, Color("#FFFFFF", textDef.String), false, true);
            }

            if (page.Selections != null)
            {
                for (int i = 0; i < page.Selections.Length; i++)
                {
                    int y = 300 + (i * 20);
                    SelectionEntry entry = page.Selections[i];

                    AddButton(115, y, 0x26B0, 0x26B1, 0, GumpButtonType.Page, entry.PageTo);

                    if (entry.Title.Number > 0)
                    {
                        AddHtmlLocalized(145, y, 250, 24, entry.Title.Number, 0x7FFF, false, false);
                    }
                    else
                    {
                        AddHtml(145, y, 250, 24, Color("#FFFFFF", entry.Title.String), false, false);
                    }
                }
            }
        }
    }

    public class PageData
    {
        public int Page { get; set; }
        public TextDefinition Text { get; set; }
        public SelectionEntry[] Selections { get; set; }

        public PageData(int page, TextDefinition text, params SelectionEntry[] selections)
        {
            Page = page;
            Text = text;
            Selections = selections;
        }
    }

    public class SelectionEntry
    {
        public TextDefinition Title { get; set; }
        public int PageTo { get; set; }

        public SelectionEntry(TextDefinition text, int pageTo)
        {
            Title = text;
            PageTo = pageTo;
        }
    }
}