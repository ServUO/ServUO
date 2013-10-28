using System;
using Server.Factions;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
    public class GuildChangeTypeGump : Gump
    {
        private readonly Mobile m_Mobile;
        private readonly Guild m_Guild;
        public GuildChangeTypeGump(Mobile from, Guild guild)
            : base(20, 30)
        {
            this.m_Mobile = from;
            this.m_Guild = guild;

            this.Dragable = false;

            this.AddPage(0);
            this.AddBackground(0, 0, 550, 400, 5054);
            this.AddBackground(10, 10, 530, 380, 3000);

            this.AddHtmlLocalized(20, 15, 510, 30, 1013062, false, false); // <center>Change Guild Type Menu</center>

            this.AddHtmlLocalized(50, 50, 450, 30, 1013066, false, false); // Please select the type of guild you would like to change to

            this.AddButton(20, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(85, 100, 300, 30, 1013063, false, false); // Standard guild

            this.AddButton(20, 150, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddItem(50, 143, 7109);
            this.AddHtmlLocalized(85, 150, 300, 300, 1013064, false, false); // Order guild

            this.AddButton(20, 200, 4005, 4007, 3, GumpButtonType.Reply, 0);
            this.AddItem(45, 200, 7107);
            this.AddHtmlLocalized(85, 200, 300, 300, 1013065, false, false); // Chaos guild

            this.AddButton(300, 360, 4005, 4007, 4, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(335, 360, 150, 30, 1011012, false, false); // CANCEL
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (GuildGump.BadLeader(this.m_Mobile, this.m_Guild))
                return;

            PlayerState pl = PlayerState.Find(this.m_Mobile);

            if (pl != null)
            {
                this.m_Mobile.SendLocalizedMessage(1010405); // You cannot change guild types while in a Faction!
            }
            else if (this.m_Guild.TypeLastChange.AddDays(7) > DateTime.UtcNow)
            {
                this.m_Mobile.SendLocalizedMessage(1005292); // Your guild type will be changed in one week.
            }
            else
            {
                GuildType newType;

                switch ( info.ButtonID )
                {
                    default:
                        return; // Close
                    case 1:
                        newType = GuildType.Regular;
                        break;
                    case 2:
                        newType = GuildType.Order;
                        break;
                    case 3:
                        newType = GuildType.Chaos;
                        break;
                }

                if (this.m_Guild.Type == newType)
                    return;

                this.m_Guild.Type = newType;
                this.m_Guild.GuildMessage(1018022, true, newType.ToString()); // Guild Message: Your guild type has changed:
            }

            GuildGump.EnsureClosed(this.m_Mobile);
            this.m_Mobile.SendGump(new GuildmasterGump(this.m_Mobile, this.m_Guild));
        }
    }
}