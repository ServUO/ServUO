using System;
using Server;

namespace Knives.Chat3
{
    public class ViewAll
    {
        public static void Initialize()
        {
            RUOVersion.AddCommand("ViewAll", AccessLevel.Player, new ChatCommandHandler(OnView));
            RUOVersion.AddCommand("Va", AccessLevel.Player, new ChatCommandHandler(OnView));
        }

        private static void OnView(CommandInfo e)
        {
            General.List(e.Mobile, 0);
        }
    }
}