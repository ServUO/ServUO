using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Gumps;

namespace Server.Services.TownCryer
{
    public class TownCryerGreetingsGump : BaseTownCryerGump
    {
        public int Page { get; private set; }

        public int Pages { get { return TownCryerSystem.GreetingsEntries.Count; } }

        public TownCryerGreetingsGump(PlayerMobile pm, TownCrier cryer, int page = 0)
            : base(pm, cryer)
        {
            Page = page;
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            var text = TownCryerSystem.GreetingsEntries[0];

            if (Page >= 0 && Page < TownCryerSystem.GreetingsEntries.Count)
            {
                text = TownCryerSystem.GreetingsEntries[Page];
            }

            if (text.Number > 0)
            {
                AddHtmlLocalized(78, 150, 700, 400, text.Number, false, false);
            }
            else
            {
                AddHtml(78, 150, 700, 400, text.String, false, false);
            }

            AddButton(350, 570, 0x605, 0x606, 1, GumpButtonType.Reply, 0);
            AddButton(380, 570, 0x609, 0x60A, 2, GumpButtonType.Reply, 0);
            AddButton(430, 570, 0x607, 0x608, 3, GumpButtonType.Reply, 0);
            AddButton(455, 570, 0x603, 0x604, 4, GumpButtonType.Reply, 0);

            AddHtml(395, 570, 35, 20, Center(String.Format("{0}/{1}", (Page + 1).ToString(), (Pages + 1).ToString())), false, false);

            AddButton(525, 625, 0x5FF, 0x600, 5, GumpButtonType.Reply, 0);
            AddHtmlLocalized(550, 625, 200, 20, 1158386, false, false); // Close and do not show this version again
        }

        public override void OnResponse(RelayInfo info)
        {
            int button = info.ButtonID;

            switch (button)
            {
                case 0: break;
                case 1: // <<
                    Page = 0;
                    Refresh();
                    break;
                case 2: // <
                    Page = Math.Max(0, Page - 1);
                    Refresh();
                    break;
                case 3: // >
                    Page = Math.Min(Pages, Page + 1);
                    Refresh();
                    break;
                case 4: // >>
                    Page = Pages;
                    Refresh();
                    break;
                case 5: // No Show
                    User.HideTownCrierGreetingGump = true;
                    break;
            }
        }
    }
}