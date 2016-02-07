using System;
using Server.Gumps;
using Server.Network;

namespace Server
{
    public class VirtueInfoGump : Gump
    {
        private readonly Mobile m_Beholder;
        private readonly int m_Desc;
        private readonly string m_Page;
        private readonly VirtueName m_Virtue;
        public VirtueInfoGump(Mobile beholder, VirtueName virtue, int description)
            : this(beholder, virtue, description, null)
        {
        }

        public VirtueInfoGump(Mobile beholder, VirtueName virtue, int description, string webPage)
            : base(0, 0)
        {
            this.m_Beholder = beholder;
            this.m_Virtue = virtue;
            this.m_Desc = description;
            this.m_Page = webPage;

            int value = beholder.Virtues.GetValue((int)virtue);

            this.AddPage(0);

            this.AddImage(30, 40, 2080);
            this.AddImage(47, 77, 2081);
            this.AddImage(47, 147, 2081);
            this.AddImage(47, 217, 2081);
            this.AddImage(47, 267, 2083);
            this.AddImage(70, 213, 2091);

            this.AddPage(1);

            int maxValue = VirtueHelper.GetMaxAmount(this.m_Virtue);

            int valueDesc;
            int dots;

            if (value < 4000)
                dots = value / 400;
            else if (value < 10000)
                dots = (value - 4000) / 600;
            else if (value < maxValue)
                dots = (value - 10000) / ((maxValue - 10000) / 10);
            else
                dots = 10;

            for (int i = 0; i < 10; ++i)
                this.AddImage(95 + (i * 17), 50, i < dots ? 2362 : 2360);

            if (value < 1)
                valueDesc = 1052044; // You have not started on the path of this Virtue.
            else if (value < 400)
                valueDesc = 1052045; // You have barely begun your journey through the path of this Virtue.
            else if (value < 2000)
                valueDesc = 1052046; // You have progressed in this Virtue, but still have much to do.
            else if (value < 3600)
                valueDesc = 1052047; // Your journey through the path of this Virtue is going well.
            else if (value < 4000)
                valueDesc = 1052048; // You feel very close to achieving your next path in this Virtue.
            else if (dots < 1)
                valueDesc = 1052049; // You have achieved a path in this Virtue.
            else if (dots < 9)
                valueDesc = 1052047; // Your journey through the path of this Virtue is going well.
            else if (dots < 10)
                valueDesc = 1052048; // You feel very close to achieving your next path in this Virtue.
            else
                valueDesc = 1052050; // You have achieved the highest path in this Virtue.

            this.AddHtmlLocalized(157, 73, 200, 40, 1051000 + (int)virtue, false, false);
            this.AddHtmlLocalized(75, 95, 220, 140, description, false, false);
            this.AddHtmlLocalized(70, 224, 229, 60, valueDesc, false, false);

            this.AddButton(65, 277, 1209, 1209, 1, GumpButtonType.Reply, 0);

            this.AddButton(280, 43, 4014, 4014, 2, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(83, 275, 400, 40, (webPage == null) ? 1052055 : 1052052, false, false); // This virtue is not yet defined. OR -click to learn more (opens webpage)
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch ( info.ButtonID )
            {
                case 1:
                    {
                        this.m_Beholder.SendGump(new VirtueInfoGump(this.m_Beholder, this.m_Virtue, this.m_Desc, this.m_Page));

                        if (this.m_Page != null)
                            state.Send(new LaunchBrowser(this.m_Page)); //No message about web browser starting on OSI
                        break;
                    }
                case 2:
                    {
                        this.m_Beholder.SendGump(new VirtueStatusGump(this.m_Beholder));
                        break;
                    }
            }
        }
    }
}