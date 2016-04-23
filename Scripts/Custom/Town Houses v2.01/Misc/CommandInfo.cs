using System;
using Server;

namespace Knives.TownHouses
{
    public delegate void TownHouseCommandHandler(CommandInfo info);

    public class CommandInfo
    {
        private Mobile c_Mobile;
        private string c_Command;
        private string c_ArgString;
        private string[] c_Arguments;

        public Mobile Mobile { get { return c_Mobile; } }
        public string Command { get { return c_Command; } }
        public string ArgString { get { return c_ArgString; } }
        public string[] Arguments { get { return c_Arguments; } }

        public CommandInfo(Mobile m, string com, string args, string[] arglist)
        {
            c_Mobile = m;
            c_Command = com;
            c_ArgString = args;
            c_Arguments = arglist;
        }

        public string GetString(int num)
        {
            if (c_Arguments.Length > num)
                return c_Arguments[num];

            return "";
        }
    }
}