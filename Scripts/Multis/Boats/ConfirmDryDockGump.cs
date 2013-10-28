using System;
using Server.Gumps;
using Server.Network;

namespace Server.Multis
{
    public class ConfirmDryDockGump : Gump
    {
        private readonly Mobile m_From;
        private readonly BaseBoat m_Boat;
        public ConfirmDryDockGump(Mobile from, BaseBoat boat)
            : base(150, 200)
        {
            this.m_From = from;
            this.m_Boat = boat;

            this.m_From.CloseGump(typeof(ConfirmDryDockGump));

            this.AddPage(0);

            this.AddBackground(0, 0, 220, 170, 5054);
            this.AddBackground(10, 10, 200, 150, 3000);

            this.AddHtmlLocalized(20, 20, 180, 80, 1018319, true, false); // Do you wish to dry dock this boat?

            this.AddHtmlLocalized(55, 100, 140, 25, 1011011, false, false); // CONTINUE
            this.AddButton(20, 100, 4005, 4007, 2, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(55, 125, 140, 25, 1011012, false, false); // CANCEL
            this.AddButton(20, 125, 4005, 4007, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 2)
                this.m_Boat.EndDryDock(this.m_From);
        }
    }
}