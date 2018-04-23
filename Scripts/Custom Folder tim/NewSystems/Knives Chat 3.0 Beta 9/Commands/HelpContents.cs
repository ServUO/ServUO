using System;
using Server;
using Server.Network;

namespace Knives.Chat3
{
    public class HelpContents
    {
        public static void Initialize()
        {
            RUOVersion.AddCommand("HelpContents", AccessLevel.Player, new ChatCommandHandler(OnHelp));
            RUOVersion.AddCommand("hc", AccessLevel.Player, new ChatCommandHandler(OnHelp));
        }

        private static void OnHelp(CommandInfo e)
        {
            new HelpContentsGump(e.Mobile);
        }
    }
}