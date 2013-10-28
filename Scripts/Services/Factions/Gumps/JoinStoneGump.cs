using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Factions
{
    public class JoinStoneGump : FactionGump
    {
        private readonly PlayerMobile m_From;
        private readonly Faction m_Faction;
        public JoinStoneGump(PlayerMobile from, Faction faction)
            : base(20, 30)
        {
            this.m_From = from;
            this.m_Faction = faction;

            this.AddPage(0);

            this.AddBackground(0, 0, 550, 440, 5054);
            this.AddBackground(10, 10, 530, 420, 3000);

            this.AddHtmlText(20, 30, 510, 20, faction.Definition.Header, false, false);
            this.AddHtmlText(20, 130, 510, 100, faction.Definition.About, true, true);

            this.AddHtmlLocalized(20, 60, 100, 20, 1011429, false, false); // Led By : 
            this.AddHtml(125, 60, 200, 20, faction.Commander != null ? faction.Commander.Name : "Nobody", false, false);

            this.AddHtmlLocalized(20, 80, 100, 20, 1011457, false, false); // Tithe rate : 
            if (faction.Tithe >= 0 && faction.Tithe <= 100 && (faction.Tithe % 10) == 0)
                this.AddHtmlLocalized(125, 80, 350, 20, 1011480 + (faction.Tithe / 10), false, false);
            else
                this.AddHtml(125, 80, 350, 20, faction.Tithe + "%", false, false);

            this.AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 400, 200, 20, 1011425, false, false); // JOIN THIS FACTION

            this.AddButton(300, 400, 4005, 4007, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(335, 400, 200, 20, 1011012, false, false); // CANCEL
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
                this.m_Faction.OnJoinAccepted(this.m_From);
        }
    }
}