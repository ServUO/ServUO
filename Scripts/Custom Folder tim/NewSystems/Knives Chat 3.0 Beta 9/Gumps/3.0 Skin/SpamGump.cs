using System;
using Server;

namespace Knives.Chat3
{
    public class SpamGump : GumpPlus
    {
        public SpamGump(Mobile m)
            : base(m, 100, 100)
        {
        }

        protected override void BuildGump()
        {
            int width = 300;
            int y = 10;

            AddHtml(0, y, width, "<CENTER>" + General.Local(215));
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddHtml(0, y += 25, width/2-10, "<DIV ALIGN=RIGHT>" + General.Local(144));
            AddTextField(width/2+10, y, 30, 21, 0x480, 0xBBA, "Chat Spam", "" + Data.ChatSpam);
            AddHtml(width/2+45, y, 20, "s");
            AddButton(width / 2 - 5, y + 4, 0x2716, "Submit", new GumpCallback(Submit));

            AddHtml(0, y+=25, width/2-10, "<DIV ALIGN=RIGHT>" + General.Local(145));
            AddTextField(width/2+10, y, 30, 21, 0x480, 0xBBA, "Msg Spam", "" + Data.MsgSpam);
            AddHtml(width/2+45, y, 20, "s");
            AddButton(width / 2 - 5, y + 4, 0x2716, "Submit", new GumpCallback(Submit));

            AddHtml(0, y += 25, width/2-10, "<DIV ALIGN=RIGHT>" + General.Local(146));
            AddTextField(width/2+10, y, 30, 21, 0x480, 0xBBA, "Request Spam", "" + Data.RequestSpam);
            AddHtml(width/2+45, y, 100, "h");
            AddButton(width / 2 - 5, y + 4, 0x2716, "Submit", new GumpCallback(Submit));

            AddBackgroundZero(0, 0, width, y+40, Data.GetData(Owner).DefaultBack);
        }

        private void Submit()
        {
            Data.ChatSpam = GetTextFieldInt("Chat Spam");
            Data.MsgSpam = GetTextFieldInt("Msg Spam");
            Data.RequestSpam = GetTextFieldInt("Request Spam");

            NewGump();
        }
    }
}