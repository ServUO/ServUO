using System;
using Server;
using Server.Network;

namespace Knives.Chat3
{
    public class All
    {
        public static void Initialize()
        {
            RUOVersion.AddCommand("All", AccessLevel.GameMaster, new ChatCommandHandler(OnAll));
        }

        private static void OnAll(CommandInfo e)
        {
            foreach (NetState ns in NetState.Instances)
                if (ns.Mobile != null)
                    ns.Mobile.SendMessage(Data.GetData(e.Mobile).StaffC, "<{0}> {1}: {2}", General.Local(261), e.Mobile.RawName, e.ArgString );
        }
    }
}