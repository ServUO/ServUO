using System;
using System.Collections;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.ConPVP
{
    public class ReadyGump : Gump
    {
        private readonly Mobile m_From;
        private readonly DuelContext m_Context;
        private readonly int m_Count;
        public ReadyGump(Mobile from, DuelContext context, int count)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Context = context;
            this.m_Count = count;

            ArrayList parts = context.Participants;

            int height = 25 + 20;

            for (int i = 0; i < parts.Count; ++i)
            {
                Participant p = (Participant)parts[i];

                height += 4;

                if (p.Players.Length > 1)
                    height += 22;

                height += (p.Players.Length * 22);
            }

            height += 25;

            this.Closable = false;
            this.Dragable = false;

            this.AddPage(0);

            this.AddBackground(0, 0, 260, height, 9250);
            this.AddBackground(10, 10, 240, height - 20, 0xDAC);

            if (count == -1)
            {
                this.AddHtml(35, 25, 190, 20, this.Center("Ready"), false, false);
            }
            else
            {
                this.AddHtml(35, 25, 190, 20, this.Center("Starting"), false, false);
                this.AddHtml(35, 25, 190, 20, "<DIV ALIGN=RIGHT>" + count.ToString(), false, false);
            }

            int y = 25 + 20;

            for (int i = 0; i < parts.Count; ++i)
            {
                Participant p = (Participant)parts[i];

                y += 4;

                bool isAllReady = true;
                int yStore = y;
                int offset = 0;

                if (p.Players.Length > 1)
                {
                    this.AddHtml(35 + 14, y, 176, 20, String.Format("Participant #{0}", i + 1), false, false);
                    y += 22;
                    offset = 10;
                }

                for (int j = 0; j < p.Players.Length; ++j)
                {
                    DuelPlayer pl = p.Players[j];

                    if (pl != null && pl.Ready)
                    {
                        this.AddImage(35 + offset, y + 4, 0x939);
                    }
                    else
                    {
                        this.AddImage(35 + offset, y + 4, 0x938);
                        isAllReady = false;
                    }

                    string name = (pl == null ? "(Empty)" : pl.Mobile.Name);

                    this.AddHtml(35 + offset + 14, y, 166, 20, name, false, false);

                    y += 22;
                }

                if (p.Players.Length > 1)
                    this.AddImage(35, yStore + 4, isAllReady ? 0x939 : 0x938);
            }
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
        }
    }
}