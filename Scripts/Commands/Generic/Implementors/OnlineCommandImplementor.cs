using Server.Network;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Commands.Generic
{
    public class OnlineCommandImplementor : BaseCommandImplementor
    {
        public OnlineCommandImplementor()
        {
            Accessors = new string[] { "Online" };
            SupportRequirement = CommandSupport.Online;
            SupportsConditionals = true;
            AccessLevel = AccessLevel.GameMaster;
            Usage = "Online <command> [condition]";
            Description = "Invokes the command on all mobiles that are currently logged in. Optional condition arguments can further restrict the set of objects.";
        }

        public override void Compile(Mobile from, BaseCommand command, ref string[] args, ref object obj)
        {
            try
            {
                Extensions ext = Extensions.Parse(from, ref args);

                bool items, mobiles;

                if (!CheckObjectTypes(from, command, ext, out items, out mobiles))
                    return;

                if (!mobiles) // sanity check
                {
                    command.LogFailure("This command does not support items.");
                    return;
                }

                ArrayList list = new ArrayList();

                List<NetState> states = NetState.Instances;

                for (int i = 0; i < states.Count; ++i)
                {
                    NetState ns = states[i];
                    Mobile mob = ns.Mobile;

                    if (mob == null)
                        continue;

                    if (!BaseCommand.IsAccessible(from, mob))
                        continue;

                    if (ext.IsValid(mob))
                        list.Add(mob);
                }

                ext.Filter(list);

                obj = list;
            }
            catch (Exception e)
            {
                from.SendMessage(e.Message);
                Diagnostics.ExceptionLogging.LogException(e);
            }
        }
    }
}
