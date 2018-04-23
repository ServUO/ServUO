/*
 * The two lines following this entry specify what RunUO version you are running.
 * In order to switch to RunUO 1.0 Final, remove the '//' in front of that setting
 * and add '//' in front of '#define RunUO_2_RC1'.  Warning:  If you comment both
 * out, many commands in this system will not work.  Enjoy!
 */

#define RunUO_2_RC1
//#define RunUO_1_Final

using System;
using System.Collections;
using Server;
using Server.Network;

#if (RunUO_2_RC1)
    using Server.Commands;
#endif

namespace Knives.Chat3
{
    public class RUOVersion
    {
        private static Hashtable s_Commands = new Hashtable();

        public static void AddCommand(string com, AccessLevel acc, ChatCommandHandler cch)
        {
            s_Commands[com.ToLower()] = cch;

            #if(RunUO_1_Final)
                Server.Commands.Register(com, acc, new CommandEventHandler(OnCommand));
            #elif(RunUO_2_RC1)
                Server.Commands.CommandSystem.Register(com, acc, new CommandEventHandler(OnCommand));
            #endif
        }

        public static void RemoveCommand(string com)
        {
            s_Commands[com.ToLower()] = null;

            #if (RunUO_1_Final)
                Server.Commands.Entries.Remove(com);
            #else
                Server.Commands.CommandSystem.Entries.Remove(com);
            #endif
        }

        public static void OnCommand(CommandEventArgs e)
        {
            if (s_Commands[e.Command.ToLower()] == null)
                return;

            ((ChatCommandHandler)s_Commands[e.Command.ToLower()])(new CommandInfo(e.Mobile, e.Command, e.ArgString, e.Arguments));
        }

        public static bool GuildChat(MessageType type)
        {
            #if (RunUO_1_Final)
                return false;
            #else
                return type == MessageType.Guild;
            #endif
        }
    }
}