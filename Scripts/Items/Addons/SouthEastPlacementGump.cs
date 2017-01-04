using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class SouthEastGump : Gump
    {
        public Action<bool> Callback { get; set; }

        public SouthEastGump(Action<bool> callback)
            : base(50, 50)
        {
            Callback = callback;

            AddBackground(0, 0, 200, 150, 5054);
            AddBackground(10, 10, 180, 130, 3000);

            AddHtml(55, 50, 150, 16, "South", false, false);
            AddHtml(55, 80, 150, 16, "East", false, false);

            AddButton(20, 50, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddButton(20, 80, 4005, 4007, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID != 0)
            {
                if (Callback != null)
                    Callback(info.ButtonID == 1);
            }
        }
    }
}