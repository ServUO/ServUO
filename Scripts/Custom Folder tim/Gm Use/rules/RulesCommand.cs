using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Spells;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;

namespace Server.Commands
{

    public class RulesCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("rules", AccessLevel.Player, new CommandEventHandler(Rules_OnCommand));
        }


        public static void Rules_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Map map = from.Map;

            if (from.HasGump(typeof(RulesGump)))
                from.CloseGump(typeof(RulesGump));


            from.SendGump(new RulesGump());
        }
    }
}
