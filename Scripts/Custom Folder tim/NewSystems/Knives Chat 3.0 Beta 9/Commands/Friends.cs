using System;
using Server;

namespace Knives.Chat3
{
    public class Friends
    {
        public static void Initialize()
        {
            RUOVersion.AddCommand("Friends", AccessLevel.Player, new ChatCommandHandler(OnFriends));
            RUOVersion.AddCommand("Fri", AccessLevel.Player, new ChatCommandHandler(OnFriends));
        }

        private static void OnFriends(CommandInfo e)
        {
            General.List(e.Mobile, 3);
        }
    }
}