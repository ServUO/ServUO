using System;
using Server;

namespace Knives.Chat3
{
    public class GeneralGump : GumpPlus
    {
        public GeneralGump(Mobile m)
            : base(m, 100, 100)
        {
            m.CloseGump(typeof(GeneralGump));
        }

        protected override void BuildGump()
        {
            int width = 300;
            int y = 10;

            AddHtml(0, y, width, "<CENTER>" + General.Local(221));
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(222));
            AddButton(width / 2 - 60, y, Data.Debug ? 0x2343 : 0x2342, "Debug", new GumpCallback(Debug));
            AddButton(width / 2 + 40, y, Data.Debug ? 0x2343 : 0x2342, "Debug", new GumpCallback(Debug));

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(169));
            AddButton(width / 2 - 100, y + 3, 0x2716, "Reload Local", new GumpCallback(ReloadLocal));
            AddButton(width / 2 + 80, y + 3, 0x2716, "Reload Local", new GumpCallback(ReloadLocal));

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(293));
            AddButton(width / 2 - 100, y + 3, 0x2716, "Reload Help", new GumpCallback(ReloadHelp));
            AddButton(width / 2 + 80, y + 3, 0x2716, "Reload Help", new GumpCallback(ReloadHelp));

            AddBackgroundZero(0, 0, width, y + 40, Data.GetData(Owner).DefaultBack);
        }

        private void Debug()
        {
            Data.Debug = !Data.Debug;

            NewGump();
        }

        private void ReloadLocal()
        {
            General.LoadLocalFile();

            Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(168));

            NewGump();
        }

        private void ReloadHelp()
        {
            General.LoadHelpFile();

            Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(294));

            NewGump();
        }
    }
}