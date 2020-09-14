using Server.Accounting;
using Server.Gumps;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Engines.Help
{
    public class SpeechLogGump : Gump
    {
        public static readonly int MaxEntriesPerPage = 30;
        private readonly Mobile m_Player;
        private readonly List<SpeechLogEntry> m_Log;
        private readonly int m_Page;
        public SpeechLogGump(Mobile player, SpeechLog log)
            : this(player, new List<SpeechLogEntry>(log))
        {
        }

        public SpeechLogGump(Mobile player, List<SpeechLogEntry> log)
            : this(player, log, (log.Count - 1) / MaxEntriesPerPage)
        {
        }

        public SpeechLogGump(Mobile player, List<SpeechLogEntry> log, int page)
            : base(500, 30)
        {
            m_Player = player;
            m_Log = log;
            m_Page = page;

            AddImageTiled(0, 0, 300, 425, 0xA40);
            AddAlphaRegion(1, 1, 298, 423);

            string playerName = player.Name;
            string playerAccount = player.Account is Account ? player.Account.Username : "???";

            AddHtml(10, 10, 280, 20, string.Format("<basefont color=#A0A0FF><center>SPEECH LOG - {0} (<i>{1}</i>)</center></basefont>", playerName, Utility.FixHtml(playerAccount)), false, false);

            int lastPage = (log.Count - 1) / MaxEntriesPerPage;

            string sLog;

            if (page < 0 || page > lastPage)
            {
                sLog = "";
            }
            else
            {
                int max = log.Count - (lastPage - page) * MaxEntriesPerPage;
                int min = Math.Max(max - MaxEntriesPerPage, 0);

                StringBuilder builder = new StringBuilder();

                for (int i = min; i < max; i++)
                {
                    SpeechLogEntry entry = log[i];

                    Mobile m = entry.From;

                    string name = m.Name;
                    string account = m.Account is Account ? m.Account.Username : "???";
                    string speech = entry.Speech;

                    if (i != min)
                        builder.Append("<br>");

                    builder.AppendFormat("<u>{0}</u> (<i>{1}</i>): {2}", name, Utility.FixHtml(account), Utility.FixHtml(speech));
                }

                sLog = builder.ToString();
            }

            AddHtml(10, 40, 280, 350, sLog, false, true);

            if (page > 0)
                AddButton(10, 395, 0xFAE, 0xFB0, 1, GumpButtonType.Reply, 0); // Previous page

            AddLabel(45, 395, 0x481, string.Format("Current page: {0}/{1}", page + 1, lastPage + 1));

            if (page < lastPage)
                AddButton(261, 395, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0); // Next page
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 1: // Previous page
                    {
                        if (m_Page - 1 >= 0)
                            from.SendGump(new SpeechLogGump(m_Player, m_Log, m_Page - 1));

                        break;
                    }
                case 2: // Next page
                    {
                        if ((m_Page + 1) * MaxEntriesPerPage < m_Log.Count)
                            from.SendGump(new SpeechLogGump(m_Player, m_Log, m_Page + 1));

                        break;
                    }
            }
        }
    }
}