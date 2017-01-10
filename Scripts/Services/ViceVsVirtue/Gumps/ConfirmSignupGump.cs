using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Guilds;
using Server.Network;

namespace Server.Engines.VvV
{
    public class ConfirmSignupGump : Gump
    {
        public PlayerMobile User { get; set; }

        public ConfirmSignupGump(PlayerMobile pm)
            : base(50, 50)
        {
            User = pm;

            AddBackground(0, 0, 360, 300, 83);

            AddHtmlLocalized(0, 25, 360, 20, 1154645, "#1155565",0xFFFF, false, false); // Vice vs Virtue Signup
            AddHtmlLocalized(10, 55, 340, 210, 1155566, 0xFFFF, false, false);
            /*Greetings! You are about to join Vice vs Virtue! VvV is an exhilarating Player vs Player 
             * experience that you can have fun with whether you have hours or only a few minutes to 
             * jump into the action!  Be forewarned, once you join VvV you will be freely attackable 
             * by other VvV participants in non-consensual PvP facets.<br><br>Will you answer the call
             * and lead your guild to victory?*/

            AddButton(115, 230, 0x2622, 0x2623, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(140, 230, 150, 20, 1155567, 0xFFFF, false, false); // Learn more about VvV!

            AddButton(10, 268, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 268, 100, 20, 1049011, 0xFFFF, false, false); // I Accept!

            AddButton(325, 268, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            AddHtml(285, 268, 100, 20, "<basefont color=#FFFFFF>Cancel", false, false);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: break;
                case 1:
                    User.LaunchBrowser("http://uo.com/wiki/ultima-online-wiki/publish-notes/publish-86/");
                    User.SendGump(new ConfirmSignupGump(User));
                    break;
                case 2:
                    Guild g = User.Guild as Guild;

                    if (g != null)
                    {
                        ViceVsVirtueSystem.Instance.AddPlayer(User);
                    }
                    break;
            }
        }
    }
}