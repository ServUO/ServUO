using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Gumps
{
    public delegate void GenericOKCallback(Mobile from, bool okay, MasterRunebook master, InternalRunebook book, int id);

    public class OKTargetGump : Gump
    {
        private GenericOKCallback m_Callback;
        private MasterRunebook m_Master;
        private InternalRunebook m_Book;
        private int m_Id;

        public OKTargetGump(string header, int headerColor, string content, int contentColor, int width, int height,
            GenericOKCallback callback, MasterRunebook master, InternalRunebook book, int id)
            : base((640 - width) / 2, (480 - height) / 2)
        {
            m_Callback = callback;
            m_Master = master;
            m_Book = book;
            m_Id = id;

            Closable = false;

            AddPage(0);

            AddBackground(0, 0, width, height, 5054);

            AddImageTiled(10, 10, width - 20, 20, 2624);
            AddAlphaRegion(10, 10, width - 20, 20);
            AddHtml(10, 10, width - 20, 20, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", headerColor, header), false, false);

            AddImageTiled(10, 40, width - 20, height - 80, 2624);
            AddAlphaRegion(10, 40, width - 20, height - 80);

            AddHtml(10, 40, width - 20, height - 80, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", contentColor, content), false, true);

            AddImageTiled(10, height - 30, width - 20, 20, 2624);
            AddAlphaRegion(10, height - 30, width - 20, 20);

            AddButton(10, height - 30, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, height - 30, 170, 20, 1049717, 32767, false, false); // YES

            AddButton(10 + ((width - 20) / 2), height - 30, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40 + ((width - 20) / 2), height - 30, 170, 20, 1049718, 32767, false, false); // NO
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            Mobile m = sender.Mobile;
            if (info.ButtonID == 1 && m_Callback != null)
                m_Callback(m, true, m_Master, m_Book, m_Id);
            else if (m_Callback != null)
                m_Callback(m, false, m_Master, m_Book, m_Id);
        }
    }
}