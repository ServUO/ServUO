using System;
using Server;

namespace Knives.Chat3
{
    public class LoggingGump : GumpPlus
    {
        public LoggingGump(Mobile m)
            : base(m, 100, 100)
        {
        }

        protected override void BuildGump()
        {
            int width = 300;
            int y = 10;

            AddHtml(0, y, width, "<CENTER>" + General.Local(237));
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(238));
            AddButton(width / 2 - 60, y, Data.LogChat ? 0x2343 : 0x2342, "Log Chat", new GumpCallback(Chat));
            AddButton(width / 2 + 40, y, Data.LogChat ? 0x2343 : 0x2342, "Log Chat", new GumpCallback(Chat));

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(239));
            AddButton(width / 2 - 60, y, Data.LogPms ? 0x2343 : 0x2342, "Log Pms", new GumpCallback(Pms));
            AddButton(width / 2 + 40, y, Data.LogPms ? 0x2343 : 0x2342, "Log Pms", new GumpCallback(Pms));

            AddBackgroundZero(0, 0, width, y+40, Data.GetData(Owner).DefaultBack);
        }

        private void Chat()
        {
            Data.LogChat = !Data.LogChat;
            NewGump();
        }

        private void Pms()
        {
            Data.LogPms = !Data.LogPms;
            NewGump();
        }
    }
}