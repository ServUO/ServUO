using System;
using System.Collections;
using Server;

namespace Knives.Chat3
{
    public class HistoryGump : GumpPlus
    {
        private Channel c_Channel;

        public HistoryGump(Mobile m, Channel c)
            : base(m, 100, 100)
        {
            m.CloseGump(typeof(HistoryGump));

            c_Channel = c;
        }

        protected override void BuildGump()
        {
            int width = 300;
            int y = 10;

            AddHtml(0, y, width, "<CENTER>" + c_Channel.NameFor(Owner) + " " +  General.Local(206));
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);
            AddButton(20, y + 3, 0x2716, "Refresh", new GumpCallback(Refresh));

            string txt = "";
            Channel.ChatHistory ch;
            ArrayList list = c_Channel.GetHistory(Owner);
            for(int i = list.Count-1; i >= 0; --i)
            {
                ch = (Channel.ChatHistory)list[i];

                txt += String.Format("    {0}: {1}<BR>", HTML.Yellow + ch.Mobile.RawName, ch.Txt);
            }

            AddHtml(10, y+=25, width-20, 300, txt, false, true);

            y += 300;

            AddBackgroundZero(0, 0, width, y + 40, Data.GetData(Owner).DefaultBack);
        }

        private void Refresh()
        {
            NewGump();
        }
    }
}