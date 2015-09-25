using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Multis
{
    public class GalleonConfirmDryDockGump : Gump
    {
        private Mobile _from;
        private BaseGalleon _galleon;

        public GalleonConfirmDryDockGump(Mobile from, BaseGalleon galleon)
            : base(150, 200)
        {
            _from = from;
            _galleon = galleon;

            _from.CloseGump(typeof(ConfirmDryDockGump));

            AddPage(0);

            AddBackground(0, 0, 220, 170, 5054);
            AddBackground(10, 10, 200, 150, 3000);

            AddHtmlLocalized(20, 20, 180, 80, 1018319, true, false); // Do you wish to dry dock this boat?

            AddHtmlLocalized(55, 100, 140, 25, 1011011, false, false); // CONTINUE
            AddButton(20, 100, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(55, 125, 140, 25, 1011012, false, false); // CANCEL
            AddButton(20, 125, 4005, 4007, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 2)
                _galleon.EndDryDock(_from);
        }
    }
}