using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class MasterRunebookGump : Gump
    {
        private MasterRunebook m_Book;
        public MasterRunebook Book { get { return m_Book; } set { m_Book = value; } }

        public MasterRunebookGump(Mobile owner, MasterRunebook book)
            : base(150, 60)
        {
            m_Book = book;
            AddPage(0);
            AddBackground(0, 0, 420, 440, 3500);
            AddItem(66, 14, 0x2252, m_Book.Hue);
            AddLabel(110, 14, 0, "Lokai's Master Runebook");
            AddItem(260, 14, 0x2252, m_Book.Hue);

            int count = 0;
            for (int x = 0; x < 3; x++)
            {
                AddPage(1 + x);
                if (x < 2)
                    AddButton(338, 407, 2471, 2470, 0, GumpButtonType.Page, 2 + x); //Next Button
                if (x > 0)
                    AddButton(30, 407, 2468, 2467, 0, GumpButtonType.Page, x); // Previous Button

                int XposB = 15, YposB = 44, XposL = 35, YposL = 41, bk = 16;
                for (int y = 0; y < 17; y++)
                {
                    string name = "";
                    try { name = m_Book.Books[count].Name.Trim(); }
                    catch { }
                    if (name == null || name.Length < 1) name = string.Format("Book #{0}", ((int)(count + 1)).ToString());
                    AddTextEntry(XposL, YposL, 320, 19, 0, count + 100, name);
                    AddButton(XposB, YposB, 1802, 1802, bk, GumpButtonType.Reply, 0);
                    AddItem(XposB - 13, YposB - 4, 8901, 0x461);
                    YposB += 20;
                    YposL += 20;
                    count++;
                    bk++;
                }
                XposB += 130;
                XposL += 130;
                YposB = 44;
                YposL = 41;
            }
        }

        private string GetString(RelayInfo info, int id)
        {
            TextRelay t = info.GetTextEntry(id);
            return (t == null ? null : t.Text.Trim());
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            string bookName;
            for (int x = 0; x < 51; x++)
            {
                bookName = GetString(info, x + 100);
                if (bookName != null && bookName.Length > 0) Book.Books[x].Name = bookName;
                else Book.Books[x].Name = string.Format("Book #{0}", ((int)(x + 1)).ToString());
            }
            Mobile from = state.Mobile;
            if (info.ButtonID > 0)
                from.SendGump(new InternalRunebookGump(from, Book.Books[info.ButtonID - 16], Book, info.ButtonID - 16));
        }
    }
}
