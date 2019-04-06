using System;
using Server.Network;

namespace Server.Gumps
{
    public class ReLoginClaimGump : Gump
    {
        public ReLoginClaimGump()
            : base(100, 100)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(19, 14, 404, 105, 9200);
            AddImageTiled(26, 23, 389, 63, 2702);
            AddButton(28, 91, 4023, 4024, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(32, 30, 375, 46, 1076251, 0xFFFFFF, false, false); // Your pet was unable to join you while you are a ghost.Please re-login once you have ressurected to claim your pets.
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    break;
            }
        }

    }
}
