using Server.Network;
using System;
using System.Collections;

namespace Server.Commands.Generic
{
    public class IPAddressCommandImplementor : BaseCommandImplementor
    {
        public IPAddressCommandImplementor()
        {
            Accessors = new string[] { "IPAddress" };
            SupportRequirement = CommandSupport.IPAddress;
            SupportsConditionals = true;
            AccessLevel = AccessLevel.Administrator;
            Usage = "IPAddress <command> [condition]";
            Description = "Invokes the command on one mobile from each IP address that is logged in. Optional condition arguments can further restrict the set of objects.";
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
                ArrayList addresses = new ArrayList();

                System.Collections.Generic.List<NetState> states = NetState.Instances;

                for (int i = 0; i < states.Count; ++i)
                {
                    NetState ns = states[i];
                    Mobile mob = ns.Mobile;

                    if (mob != null && !addresses.Contains(ns.Address) && ext.IsValid(mob))
                    {
                        list.Add(mob);
                        addresses.Add(ns.Address);
                    }
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
