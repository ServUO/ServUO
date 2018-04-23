using System;
using System.Collections;
using Server;

namespace Knives.Chat3
{
    public class HelpContentsGump : GumpPlus
    {
        private string c_Search = "";
        private string c_Topic = "";
        private int c_Page;

        public HelpContentsGump(Mobile m)
            : base(m, 100, 100)
        {
        }

        protected override void BuildGump()
        {
            int width = 200;
            int y = 10;

            if (c_Topic != "")
                width = 300;

            AddHtml(0, y, width, "<CENTER>Help Contents Search");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddTextField(width/2-70, y+=25, 140, 21, 0x480, 0xBBC, "Search", c_Search);
            AddButton(width / 2 - 90, y+3, 0x2716, "Search", new GumpCallback(Search));
            AddButton(width / 2 + 80, y+3, 0x2716, "Search", new GumpCallback(Search));

            if (c_Topic != "")
            {
                AddHtml(0, y+=35, width, "<CENTER>" + c_Topic);

                AddHtml(20, y += 25, width - 20, 100, General.GetHelp(c_Topic), false, true);
                AddBackgroundZero(0, 0, width, y+=120, 0x1400);
                return;
            }

            if (c_Search == "")
            {
                AddHtml(20, y+=35, width-40, 90, "<CENTER>" + General.Local(262), false, false);
                AddBackgroundZero(0, 0, width, y+110, 0x1400);
                return;
            }

            ArrayList list = new ArrayList();
            foreach (string str in General.Help.Keys)
            {
                if (str.ToLower().IndexOf(c_Search.ToLower()) != -1)
                    list.Add(str);
                else if (General.GetHelp(str).ToLower().IndexOf(c_Search.ToLower()) != -1)
                    list.Add(str);
            }

            if (list.Count == 0)
            {
                AddHtml(0, y += 35, width, "<CENTER>" + General.Local(263));
                AddBackgroundZero(0, 0, width, y + 40, 0x1400);
                return;
            }

            AddHtml(0, y += 25, width, "<CENTER>" + list.Count + (list.Count == 0 ? General.Local(265) : General.Local(264)));

            list.Sort(new InternalSort());

            int perpage = 10;

            if (list.Count < c_Page * perpage)
                c_Page = 0;

            if (c_Page != 0)
                AddButton(width / 2 - 20, y - 3, 0x25E4, 0x25E5, "Page Down", new GumpCallback(PageDown));
            if (perpage * (c_Page + 1) < list.Count)
                AddButton(width / 2, y - 3, 0x25E8, 0x25E9, "Page Up", new GumpCallback(PageUp));

            y += 5;

            for (int i = c_Page * perpage; i < (c_Page + 1) * perpage && i < list.Count; ++i)
            {
                AddHtml(30, y+=20, width-30, list[i].ToString());
                AddButton(10, y+3, 0x2716, "Select", new GumpStateCallback(Select), list[i]);
            }

            AddBackgroundZero(0, 0, width, y+40, 0x1400);
        }

        private void PageUp()
        {
            c_Page++;
            NewGump();
        }

        private void PageDown()
        {
            c_Page--;
            NewGump();
        }

        private void Search()
        {
            c_Topic = "";
            c_Search = GetTextField("Search");
            NewGump();
        }

        private void Select(object obj)
        {
            c_Topic = obj.ToString();
            NewGump();
        }


        private class InternalSort : IComparer
        {
            public InternalSort()
            {
            }

            public int Compare(object x, object y)
            {
                return Insensitive.Compare(x.ToString(), y.ToString());
            }
        }
    }
}