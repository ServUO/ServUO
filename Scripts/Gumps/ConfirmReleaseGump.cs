using System;
using Server.Mobiles;

namespace Server.Gumps
{
    public class ConfirmReleaseGump : Gump
    {
        private readonly Mobile m_From;
        private readonly BaseCreature m_Pet;
        public ConfirmReleaseGump(Mobile from, BaseCreature pet)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Pet = pet;

            this.m_From.CloseGump(typeof(ConfirmReleaseGump));

            this.AddPage(0);

            this.AddBackground(0, 0, 270, 120, 5054);
            this.AddBackground(10, 10, 250, 100, 3000);

            this.AddHtmlLocalized(20, 15, 230, 60, 1046257, true, true); // Are you sure you want to release your pet?

            this.AddButton(20, 80, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 80, 75, 20, 1011011, false, false); // CONTINUE

            this.AddButton(135, 80, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(170, 80, 75, 20, 1011012, false, false); // CANCEL
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 2)
            {
                if (!this.m_Pet.Deleted && this.m_Pet.Controlled && this.m_From == this.m_Pet.ControlMaster && this.m_From.CheckAlive() /*&& m_Pet.CheckControlChance( m_From )*/)
                {
                    if (this.m_Pet.Map == this.m_From.Map && this.m_Pet.InRange(this.m_From, 14))
                    {
                        this.m_Pet.ControlTarget = null;
                        this.m_Pet.ControlOrder = OrderType.Release;
                    }
                }
            }
        }
    }
}